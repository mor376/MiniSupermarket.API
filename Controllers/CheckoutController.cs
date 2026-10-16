using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/checkout")]
    [ApiController]
    [Authorize(Roles = "Admin,Cashier")]
    public class CheckoutController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IConfiguration configuration, ILogger<CheckoutController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequestDto request, CancellationToken cancellationToken)
        {
            var cashierUsername = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(cashierUsername))
            {
                return Unauthorized(new { message = "Không xác định được tài khoản thu ngân từ phiên đăng nhập." });
            }

            if (request.Items.Count == 0 || request.Items.Any(item => item.ProductId <= 0 || item.Quantity <= 0))
            {
                return BadRequest(new { message = "Giỏ hàng phải có sản phẩm và số lượng lớn hơn 0." });
            }

            var items = request.Items
                .GroupBy(item => item.ProductId)
                .Select(group => new { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) })
                .OrderBy(item => item.ProductId)
                .ToList();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                var products = new List<(int ProductId, string ProductName, decimal UnitPrice, int Quantity)>();
                foreach (var item in items)
                {
                    const string productSql = "SELECT ProductName, SellingPrice, StockQuantity FROM Products WITH (UPDLOCK, HOLDLOCK) WHERE ProductId = @ProductId;";
                    await using var command = new SqlCommand(productSql, connection, transaction);
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                    await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        throw new CheckoutValidationException($"Không tìm thấy sản phẩm ID {item.ProductId}.");
                    }

                    var name = reader.GetString(reader.GetOrdinal("ProductName"));
                    var price = reader.GetDecimal(reader.GetOrdinal("SellingPrice"));
                    var stock = reader.GetInt32(reader.GetOrdinal("StockQuantity"));
                    if (stock < item.Quantity)
                    {
                        throw new CheckoutValidationException($"Sản phẩm '{name}' không đủ tồn kho. Tồn kho: {stock}, yêu cầu: {item.Quantity}.");
                    }

                    products.Add((item.ProductId, name, price, item.Quantity));
                }

                var totalItems = products.Sum(item => item.Quantity);
                var finalAmount = products.Sum(item => item.UnitPrice * item.Quantity);
                var rewardPoints = (int)(finalAmount / 10000m);
                var orderCode = $"AP-HD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid():N}"[..31];

                const string insertOrderSql = @"
                    INSERT INTO Orders (OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
                    OUTPUT INSERTED.OrderId
                    VALUES (@OrderCode, GETDATE(), @CashierUsername, @CustomerId, @TotalItems, @FinalAmount, @RewardPoints);";

                int orderId;
                await using (var command = new SqlCommand(insertOrderSql, connection, transaction))
                {
                    command.Parameters.Add("@OrderCode", SqlDbType.VarChar, 50).Value = orderCode;
                    command.Parameters.Add("@CashierUsername", SqlDbType.NVarChar, 50).Value = cashierUsername;
                    command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = request.CustomerId;
                    command.Parameters.Add("@TotalItems", SqlDbType.Int).Value = totalItems;
                    command.Parameters.Add("@FinalAmount", SqlDbType.Decimal).Value = finalAmount;
                    command.Parameters["@FinalAmount"].Precision = 18;
                    command.Parameters["@FinalAmount"].Scale = 2;
                    command.Parameters.Add("@RewardPoints", SqlDbType.Int).Value = rewardPoints;
                    orderId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
                }

                foreach (var item in products)
                {
                    const string detailSql = @"
                        INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, LineTotal)
                        VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @LineTotal);";
                    await using (var command = new SqlCommand(detailSql, connection, transaction))
                    {
                        command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                        command.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                        command.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                        AddMoneyParameter(command, "@UnitPrice", item.UnitPrice);
                        AddMoneyParameter(command, "@LineTotal", item.UnitPrice * item.Quantity);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }

                    const string stockSql = "UPDATE Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductId = @ProductId AND StockQuantity >= @Quantity;";
                    await using (var command = new SqlCommand(stockSql, connection, transaction))
                    {
                        command.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                        command.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                        if (await command.ExecuteNonQueryAsync(cancellationToken) != 1)
                        {
                            throw new CheckoutValidationException($"Tồn kho sản phẩm '{item.ProductName}' đã thay đổi. Vui lòng thử lại.");
                        }
                    }
                }

                const string customerSql = "UPDATE Customers SET RewardPoints = RewardPoints + @Points WHERE CustomerId = @CustomerId;";
                await using (var command = new SqlCommand(customerSql, connection, transaction))
                {
                    command.Parameters.Add("@Points", SqlDbType.Int).Value = rewardPoints;
                    command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = request.CustomerId;
                    if (await command.ExecuteNonQueryAsync(cancellationToken) != 1)
                    {
                        throw new CheckoutValidationException($"Không tìm thấy khách hàng ID {request.CustomerId}.");
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                return Ok(new
                {
                    success = true,
                    message = "Thanh toán hóa đơn POS thành công!",
                    orderId,
                    orderCode,
                    totalItems,
                    finalAmount,
                    rewardPointsAdded = rewardPoints
                });
            }
            catch (CheckoutValidationException ex)
            {
                if (!await RollbackAsync(transaction))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Không thể hoàn tác giao dịch thanh toán." });
                }

                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                var rolledBack = await RollbackAsync(transaction);
                _logger.LogError(ex, "POS checkout transaction failed for cashier {CashierUsername}.", cashierUsername);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = rolledBack ? "Thanh toán thất bại; giao dịch đã được hoàn tác." : "Thanh toán thất bại và không thể xác nhận hoàn tác."
                });
            }
        }

        [HttpDelete("cancel/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelOrder(int orderId, CancellationToken cancellationToken)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { message = "Mã hóa đơn không hợp lệ." });
            }

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                int customerId;
                int rewardPoints;
                const string orderSql = "SELECT CustomerId, RewardPoints FROM Orders WITH (UPDLOCK, HOLDLOCK) WHERE OrderId = @OrderId;";
                await using (var command = new SqlCommand(orderSql, connection, transaction))
                {
                    command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        throw new CheckoutValidationException($"Không tìm thấy hóa đơn ID {orderId}.");
                    }

                    customerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                    rewardPoints = reader.GetInt32(reader.GetOrdinal("RewardPoints"));
                }

                var items = new List<(int ProductId, int Quantity)>();
                const string detailSql = "SELECT ProductId, Quantity FROM OrderDetails WHERE OrderId = @OrderId;";
                await using (var command = new SqlCommand(detailSql, connection, transaction))
                {
                    command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        items.Add((reader.GetInt32(0), reader.GetInt32(1)));
                    }
                }

                foreach (var item in items)
                {
                    const string restoreSql = "UPDATE Products SET StockQuantity = StockQuantity + @Quantity WHERE ProductId = @ProductId;";
                    await using var command = new SqlCommand(restoreSql, connection, transaction);
                    command.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                    if (await command.ExecuteNonQueryAsync(cancellationToken) != 1)
                    {
                        throw new CheckoutValidationException($"Không thể hoàn kho sản phẩm ID {item.ProductId}.");
                    }
                }

                const string pointsSql = "UPDATE Customers SET RewardPoints = CASE WHEN RewardPoints >= @Points THEN RewardPoints - @Points ELSE 0 END WHERE CustomerId = @CustomerId;";
                await using (var command = new SqlCommand(pointsSql, connection, transaction))
                {
                    command.Parameters.Add("@Points", SqlDbType.Int).Value = rewardPoints;
                    command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;
                    if (await command.ExecuteNonQueryAsync(cancellationToken) != 1)
                    {
                        throw new CheckoutValidationException($"Không tìm thấy khách hàng ID {customerId}.");
                    }
                }

                const string deleteDetailsSql = "DELETE FROM OrderDetails WHERE OrderId = @OrderId;";
                await using (var command = new SqlCommand(deleteDetailsSql, connection, transaction))
                {
                    command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                const string deleteOrderSql = "DELETE FROM Orders WHERE OrderId = @OrderId;";
                await using (var command = new SqlCommand(deleteOrderSql, connection, transaction))
                {
                    command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return Ok(new { success = true, message = $"Đã hủy hóa đơn {orderId} và hoàn kho." });
            }
            catch (CheckoutValidationException ex)
            {
                if (!await RollbackAsync(transaction))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Không thể hoàn tác giao dịch hủy hóa đơn." });
                }

                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                var rolledBack = await RollbackAsync(transaction);
                _logger.LogError(ex, "Order cancellation transaction failed for order {OrderId}.", orderId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = rolledBack ? "Hủy hóa đơn thất bại; giao dịch đã được hoàn tác." : "Hủy hóa đơn thất bại và không thể xác nhận hoàn tác."
                });
            }
        }

        private async Task<bool> RollbackAsync(SqlTransaction transaction)
        {
            try
            {
                await transaction.RollbackAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to roll back POS transaction.");
                return false;
            }
        }

        private static void AddMoneyParameter(SqlCommand command, string name, decimal value)
        {
            var parameter = command.Parameters.Add(name, SqlDbType.Decimal);
            parameter.Precision = 18;
            parameter.Scale = 2;
            parameter.Value = value;
        }

        private sealed class CheckoutValidationException : Exception
        {
            public CheckoutValidationException(string message) : base(message)
            {
            }
        }
    }
}
