using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedAnPhuocStoreData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Rau, củ, trái cây", "Rau xanh, củ quả và trái cây tươi" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Thịt, cá, trứng", "Thực phẩm tươi sống và trứng" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Sữa & sản phẩm từ sữa", "Sữa tươi, sữa chua và phô mai" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Nước uống & giải khát", "Nước suối, nước ngọt và trà đóng chai" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Mì, gạo & thực phẩm khô", "Mì ăn liền, gạo và thực phẩm đóng hộp" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 6, "Gia vị & dầu ăn", "Nước mắm, dầu ăn và gia vị nấu ăn" },
                    { 7, "Bánh kẹo & đồ ăn vặt", "Bánh quy, snack và kẹo" }
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Ấp 1, xã An Phước, huyện Long Thành, Đồng Nai", "Nguyễn Minh Anh", "Chuẩn", "0900000001", 25 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Ấp 2, xã An Phước, huyện Long Thành, Đồng Nai", "Trần Quốc Bảo", "0900000002", 85 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Ấp 3, xã An Phước, huyện Long Thành, Đồng Nai", "Lê Thị Ngọc", "0900000003", 15 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Ấp 4, xã An Phước, huyện Long Thành, Đồng Nai", "Phạm Thanh Tùng", "Vàng", "0900000004", 240 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Ấp 5, xã An Phước, huyện Long Thành, Đồng Nai", "Hoàng Thu Hà", "0900000005", 60 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Ấp 6, xã An Phước, huyện Long Thành, Đồng Nai", "Võ Thành Đạt", "0900000006", 320 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 7,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Khu dân cư An Phước, huyện Long Thành, Đồng Nai", "Đặng Mỹ Linh", "0900000007", 0 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 8,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "Ấp 1, xã An Phước, huyện Long Thành, Đồng Nai", "Bùi Văn Hùng", "Bạc", "0900000008" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 9,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "Ấp 2, xã An Phước, huyện Long Thành, Đồng Nai", "Đỗ Thị Mai", "Chuẩn", "0900000009" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 10,
                columns: new[] { "Address", "CustomerName", "PhoneNumber" },
                values: new object[] { "Ấp 3, xã An Phước, huyện Long Thành, Đồng Nai", "Ngô Quốc Khánh", "0900000010" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 11,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "Ấp 4, xã An Phước, huyện Long Thành, Đồng Nai", "Dương Khánh Vy", "Vàng", "0900000011" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 12,
                columns: new[] { "Address", "PhoneNumber" },
                values: new object[] { "Ấp 5, xã An Phước, huyện Long Thành, Đồng Nai", "0900000012" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 13,
                columns: new[] { "Address", "MembershipRank", "PhoneNumber" },
                values: new object[] { "Ấp 6, xã An Phước, huyện Long Thành, Đồng Nai", "Bạc", "0900000013" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 14,
                columns: new[] { "Address", "PhoneNumber" },
                values: new object[] { "Khu dân cư An Phước, huyện Long Thành, Đồng Nai", "0900000014" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 15,
                columns: new[] { "Address", "PhoneNumber" },
                values: new object[] { "Xã Long Đức, huyện Long Thành, Đồng Nai", "0900000015" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 16,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "Thị trấn Long Thành, huyện Long Thành, Đồng Nai", "Nguyễn Ngọc Ánh", "Bạch Kim", "0900000016" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Barcode", "ProductName", "StockQuantity" },
                values: new object[] { "AP-000001", "Cải xanh", 40 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "Barcode", "ProductName", "StockQuantity" },
                values: new object[] { "AP-000002", "Cà chua", 35 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "AP-000003", 1, 25000m, "Khoai tây", 30 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "Barcode", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "AP-000004", 69500m, "Thịt heo ba rọi 500g", 20 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "AP-000005", 2, 32000m, "Trứng gà hộp 10 quả", 25 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "AP-000006", 2, 32500m, "Cá basa phi lê 500g", 15 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "AP-000007", 3, 36000m, "Sữa tươi Vinamilk 1L", 45 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 8, "AP-000008", 3, 7000m, "Sữa chua Vinamilk có đường 100g", 60 },
                    { 9, "AP-000009", 3, 32000m, "Phô mai Con Bò Cười 8 miếng", 20 },
                    { 10, "AP-000010", 4, 6000m, "Nước suối Aquafina 500ml", 100 },
                    { 11, "AP-000011", 4, 19000m, "Coca-Cola 1.5L", 48 },
                    { 12, "AP-000012", 4, 9000m, "Trà xanh Không Độ 455ml", 50 },
                    { 13, "AP-000013", 5, 4500m, "Mì Hảo Hảo tôm chua cay 75g", 150 },
                    { 14, "AP-000014", 5, 165000m, "Gạo ST25 túi 5kg", 18 },
                    { 15, "AP-000015", 5, 18000m, "Cá hộp sốt cà 155g", 30 },
                    { 16, "AP-000016", 6, 29000m, "Nước mắm Nam Ngư 500ml", 25 },
                    { 17, "AP-000017", 6, 52000m, "Dầu ăn Neptune 1L", 22 },
                    { 18, "AP-000018", 6, 38000m, "Hạt nêm Knorr 400g", 28 },
                    { 19, "AP-000019", 7, 8000m, "Snack khoai tây O'Star 35g", 40 },
                    { 20, "AP-000020", 7, 22000m, "Bánh quy Oreo 133g", 32 },
                    { 21, "AP-000021", 7, 12000m, "Kẹo dẻo trái cây 90g", 25 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Bánh kẹo & Đồ ăn vặt", "Snack, bánh quy, kẹo dẻo" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Nước giải khát & Trà", "Nước ngọt, nước khoáng, trà" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Sữa & Sản phẩm từ sữa", "Sữa tươi, sữa chua, phô mai" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Mì gói & Thực phẩm ăn liền", "Mì ăn liền, phở khô, cháo gói" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Gia vị & Dầu ăn", "Nước mắm, hạt nêm, dầu thực vật" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[] { "387 Lê Văn Việt, Phường Tăng Nhơn Phú A, Thành phố Thủ Đức, TP. Hồ Chí Minh", "Nguyễn Văn A", "Vàng", "0901122334", 150 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "12 Phố Hàng Bài, Phường Tràng Tiền, Quận Hoàn Kiếm, Hà Nội", "Trần Thị B", "0918877665", 50 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "45 Nguyễn Văn Linh, Phường Bình Hiên, Quận Hải Châu, Đà Nẵng", "Lê Văn C", "0983344556", 10 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[] { "102 Xô Viết Nghệ Tĩnh, Phường 21, Quận Bình Thạnh, TP. Hồ Chí Minh", "Phạm Hoàng Dũng", "Kim Cương", "0934567890", 320 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "88 Cầu Giấy, Phường Quan Hoa, Quận Cầu Giấy, Hà Nội", "Hoàng Thi Hương", "0971122334", 85 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "15 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh", "Vũ Đình Trọng", "0909988776", 210 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 7,
                columns: new[] { "Address", "CustomerName", "PhoneNumber", "RewardPoints" },
                values: new object[] { "230 Trần Hưng Đạo, Phường An Hòa, Quận Ninh Kiều, Cần Thơ", "Đặng Mai Phương", "0965432109", 15 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 8,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "67 Quang Trung, Phường Lộc Thọ, Thành phố Nha Trang, Khánh Hòa", "Bùi Anh Tuấn", "Vàng", "0943210987" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 9,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "54 Lạch Tray, Phường Hàng Kênh, Quận Lê Chân, Hải Phòng", "Đỗ Thảo Nguyên", "Bạc", "0912345678" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 10,
                columns: new[] { "Address", "CustomerName", "PhoneNumber" },
                values: new object[] { "19 Hùng Vương, Phường Phú Hội, Thành phố Huế, Thừa Thiên Huế", "Ngo Quoc Bao", "0987654321" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 11,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "78 Nguyễn Trãi, Phường 3, Quận 5, TP. Hồ Chí Minh", "Dương Khánh Linh", "Kim Cương", "0931122334" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 12,
                columns: new[] { "Address", "PhoneNumber" },
                values: new object[] { "143 Nguyễn Chí Thanh, Phường Láng Hạ, Quận Đống Đa, Hà Nội", "0978899001" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 13,
                columns: new[] { "Address", "MembershipRank", "PhoneNumber" },
                values: new object[] { "25 Lê Hồng Phong, Phường 4, Thành phố Đà Lạt, Lâm Đồng", "Vàng", "0908877665" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 14,
                columns: new[] { "Address", "PhoneNumber" },
                values: new object[] { "89 Đường 30 Tháng 4, Phường Trung Dũng, Thành phố Biên Hòa, Đồng Nai", "0961122334" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 15,
                columns: new[] { "Address", "PhoneNumber" },
                values: new object[] { "312 Đại lộ Bình Dương, Phường Phú Hòa, Thành phố Thủ Dầu Một, Bình Dương", "0945566778" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 16,
                columns: new[] { "Address", "CustomerName", "MembershipRank", "PhoneNumber" },
                values: new object[] { "56 Điện Biên Phủ, Phường Chính Gián, Quận Thanh Khê, Đà Nẵng", "Nguyễn Thị Ngọc Ánh", "Kim Cương", "0919988776" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Barcode", "ProductName", "StockQuantity" },
                values: new object[] { "8934563101011", "Snack Khoai Tây Lay's 68g", 50 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "Barcode", "ProductName", "StockQuantity" },
                values: new object[] { "8934563101028", "Bánh Quy Oreo Vị Vani 133g", 40 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "8934563202018", 2, 10000m, "Nước Ngọt Coca Cola 320ml", 100 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "Barcode", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "8934563202025", 9000m, "Trà Xanh C2 Hương Chanh 360ml", 80 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "8934563303015", 3, 8500m, "Sữa Tươi Tiệt Trùng Vinamilk 180ml", 120 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "8934563404012", 4, 4500m, "Mì Hảo Hảo Tôm Chua Cay 75g", 200 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7,
                columns: new[] { "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { "8934563505019", 5, 35000m, "Nước Mắm Nam Ngư 500ml", 30 });
        }
    }
}
