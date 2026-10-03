using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerAddresses16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RewardPoints",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MembershipRank",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Chuẩn",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "Address",
                value: "387 Lê Văn Việt, Phường Tăng Nhơn Phú A, Thành phố Thủ Đức, TP. Hồ Chí Minh");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "Address",
                value: "12 Phố Hàng Bài, Phường Tràng Tiền, Quận Hoàn Kiếm, Hà Nội");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                column: "Address",
                value: "45 Nguyễn Văn Linh, Phường Bình Hiên, Quận Hải Châu, Đà Nẵng");

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 4, "102 Xô Viết Nghệ Tĩnh, Phường 21, Quận Bình Thạnh, TP. Hồ Chí Minh", "Phạm Hoàng Dũng", "Kim Cương", "0934567890", 320 },
                    { 5, "88 Cầu Giấy, Phường Quan Hoa, Quận Cầu Giấy, Hà Nội", "Hoàng Thi Hương", "Bạc", "0971122334", 85 },
                    { 6, "15 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh", "Vũ Đình Trọng", "Vàng", "0909988776", 210 },
                    { 7, "230 Trần Hưng Đạo, Phường An Hòa, Quận Ninh Kiều, Cần Thơ", "Đặng Mai Phương", "Chuẩn", "0965432109", 15 },
                    { 8, "67 Quang Trung, Phường Lộc Thọ, Thành phố Nha Trang, Khánh Hòa", "Bùi Anh Tuấn", "Vàng", "0943210987", 120 },
                    { 9, "54 Lạch Tray, Phường Hàng Kênh, Quận Lê Chân, Hải Phòng", "Đỗ Thảo Nguyên", "Bạc", "0912345678", 45 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { 10, "19 Hùng Vương, Phường Phú Hội, Thành phố Huế, Thừa Thiên Huế", "Ngo Quoc Bao", "Chuẩn", "0987654321" });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 11, "78 Nguyễn Trãi, Phường 3, Quận 5, TP. Hồ Chí Minh", "Dương Khánh Linh", "Kim Cương", "0931122334", 450 },
                    { 12, "143 Nguyễn Chí Thanh, Phường Láng Hạ, Quận Đống Đa, Hà Nội", "Lý Minh Triết", "Bạc", "0978899001", 90 },
                    { 13, "25 Lê Hồng Phong, Phường 4, Thành phố Đà Lạt, Lâm Đồng", "Phan Thùy Trang", "Vàng", "0908877665", 180 },
                    { 14, "89 Đường 30 Tháng 4, Phường Trung Dũng, Thành phố Biên Hòa, Đồng Nai", "Vũ Hải Đăng", "Chuẩn", "0961122334", 25 },
                    { 15, "312 Đại lộ Bình Dương, Phường Phú Hòa, Thành phố Thủ Dầu Một, Bình Dương", "Trịnh Xuân Trường", "Vàng", "0945566778", 280 },
                    { 16, "56 Điện Biên Phủ, Phường Chính Gián, Quận Thanh Khê, Đà Nẵng", "Nguyễn Thị Ngọc Ánh", "Kim Cương", "0919988776", 510 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 16);

            migrationBuilder.AlterColumn<int>(
                name: "RewardPoints",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "MembershipRank",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Chuẩn");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "Address",
                value: "TP. Hồ Chí Minh");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "Address",
                value: "Hà Nội");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                column: "Address",
                value: "Đà Nẵng");
        }
    }
}
