using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MiniSupermarket.API.Controllers
{
    public class ProductLookupDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    [Route("api/ado-products")]
    [ApiController]
    public class AdoProductController : ControllerBase
    {
        private readonly string _connectionString;

        public AdoProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<ProductLookupDto>> GetProductByBarcode(string barcode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                return BadRequest(new { message = "Mã vạch không được để trống." });
            }

            const string sql = "SELECT ProductId, Barcode, ProductName, SellingPrice AS Price, StockQuantity FROM Products WHERE Barcode = @Barcode;";
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Barcode", SqlDbType.VarChar, 50).Value = barcode.Trim();

            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm có mã vạch: {barcode}" });
            }

            return Ok(ReadProduct(reader));
        }

        [HttpGet("disconnected-cache")]
        public async Task<IActionResult> GetProductsDisconnected(CancellationToken cancellationToken)
        {
            const string sql = "SELECT ProductId, Barcode, ProductName, SellingPrice AS Price, StockQuantity FROM Products ORDER BY ProductId;";
            var table = new DataTable("CachedProducts");

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(table);
            }

            var products = table.Rows.Cast<DataRow>().Select(row => new ProductLookupDto
            {
                ProductId = row.Field<int>("ProductId"),
                Barcode = row.Field<string>("Barcode") ?? string.Empty,
                ProductName = row.Field<string>("ProductName") ?? string.Empty,
                Price = row.Field<decimal>("Price"),
                StockQuantity = row.Field<int>("StockQuantity")
            }).ToList();

            return Ok(new { totalLoaded = products.Count, cachedAt = DateTimeOffset.Now, data = products });
        }

        private static ProductLookupDto ReadProduct(SqlDataReader reader) => new()
        {
            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
            Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
            StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity"))
        };
    }
}
