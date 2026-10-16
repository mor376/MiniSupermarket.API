using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class CompleteMiniSupermarketSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "SellingPrice");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 9000m, "Bó" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 13000m, "Kg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 18000m, "Kg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 55000m, "Khay" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 25000m, "Hộp" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 26000m, "Khay" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 29000m, "Hộp" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 5500m, "Hũ" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 26000m, "Hộp" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 4000m, "Chai" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 15000m, "Chai" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 7000m, "Chai" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 3200m, "Gói" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 135000m, "Túi" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 14000m, "Hộp" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 22000m, "Chai" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 42000m, "Chai" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 30000m, "Gói" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 5500m, "Gói" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 17000m, "Gói" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21,
                columns: new[] { "CostPrice", "Unit" },
                values: new object[] { 9000m, "Gói" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_CostPrice",
                table: "Products",
                sql: "[CostPrice] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_SellingPrice",
                table: "Products",
                sql: "[SellingPrice] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_StockQuantity",
                table: "Products",
                sql: "[StockQuantity] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_CostPrice",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_SellingPrice",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_StockQuantity",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "Products",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15);
        }
    }
}
