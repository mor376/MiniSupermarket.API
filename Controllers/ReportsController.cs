using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly string _connectionString;

        public ReportsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        [HttpGet("revenue-by-cashier")]
        public async Task<IActionResult> GetRevenueByCashier(CancellationToken cancellationToken)
        {
            var report = new List<CashierRevenueDto>();
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand("dbo.sp_GetRevenueByCashier", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                report.Add(new CashierRevenueDto
                {
                    CashierUsername = reader.GetString(reader.GetOrdinal("CashierUsername")),
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    TotalPointsAwarded = reader.IsDBNull(reader.GetOrdinal("TotalPointsAwarded"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("TotalPointsAwarded"))
                });
            }

            return Ok(new
            {
                success = true,
                generatedAt = DateTimeOffset.Now,
                totalCashiers = report.Count,
                grandTotalRevenue = report.Sum(item => item.TotalRevenue),
                data = report
            });
        }
    }
}
