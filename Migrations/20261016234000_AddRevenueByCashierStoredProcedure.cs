using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MiniSupermarket.API.Data;

#nullable disable

namespace MiniSupermarket.API.Migrations
{
    [DbContext(typeof(SupermarketDbContext))]
    [Migration("20261016234000_AddRevenueByCashierStoredProcedure")]
    public partial class AddRevenueByCashierStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.sp_GetRevenueByCashier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CashierUsername,
        COUNT(OrderId) AS TotalOrders,
        SUM(FinalAmount) AS TotalRevenue,
        SUM(RewardPoints) AS TotalPointsAwarded
    FROM Orders
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC;
END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.sp_GetRevenueByCashier', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetRevenueByCashier;");
        }
    }
}
