using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options)
            : base(options)
        {
        }

        // Khai báo các DbSet đại diện cho các bảng trong SQL Server
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(user => user.UserId);
                entity.Property(user => user.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(user => user.Username).IsUnique();
                entity.Property(user => user.PasswordHash).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(user => user.FullName).IsRequired().HasMaxLength(100);
                entity.Property(user => user.Role).IsRequired().HasMaxLength(50);
                entity.Property(user => user.IsActive).HasDefaultValue(true);
            });

            // ---------------------------------------------------------
            // 1. CẤU HÌNH VÀ SEED DATA CHO BẢNG CATEGORIES (7 BẢN GHI)
            // ---------------------------------------------------------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
                entity.HasIndex(c => c.CategoryName).IsUnique();
                entity.Property(c => c.Description).HasMaxLength(255);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Rau, củ, trái cây", Description = "Rau xanh, củ quả và trái cây tươi" },
                new Category { CategoryId = 2, CategoryName = "Thịt, cá, trứng", Description = "Thực phẩm tươi sống và trứng" },
                new Category { CategoryId = 3, CategoryName = "Sữa & sản phẩm từ sữa", Description = "Sữa tươi, sữa chua và phô mai" },
                new Category { CategoryId = 4, CategoryName = "Nước uống & giải khát", Description = "Nước suối, nước ngọt và trà đóng chai" },
                new Category { CategoryId = 5, CategoryName = "Mì, gạo & thực phẩm khô", Description = "Mì ăn liền, gạo và thực phẩm đóng hộp" },
                new Category { CategoryId = 6, CategoryName = "Gia vị & dầu ăn", Description = "Nước mắm, dầu ăn và gia vị nấu ăn" },
                new Category { CategoryId = 7, CategoryName = "Bánh kẹo & đồ ăn vặt", Description = "Bánh quy, snack và kẹo" }
            );

            // ---------------------------------------------------------
            // 2. CẤU HÌNH VÀ SEED DATA CHO BẢNG PRODUCTS (21 BẢN GHI)
            // ---------------------------------------------------------
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.Barcode).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.HasIndex(p => p.Barcode).IsUnique();
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(150);
                entity.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Unit).IsRequired().HasMaxLength(20);
                entity.HasCheckConstraint("CK_Products_CostPrice", "[CostPrice] >= 0");
                entity.HasCheckConstraint("CK_Products_SellingPrice", "[SellingPrice] >= 0");
                entity.HasCheckConstraint("CK_Products_StockQuantity", "[StockQuantity] >= 0");

                // Cấu hình quan hệ Khóa ngoại liên kết với Categories
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Barcode = "AP-000001", ProductName = "Cải xanh", CostPrice = 9000, SellingPrice = 12000, StockQuantity = 40, Unit = "Bó", CategoryId = 1 },
                new Product { ProductId = 2, Barcode = "AP-000002", ProductName = "Cà chua", CostPrice = 13000, SellingPrice = 18000, StockQuantity = 35, Unit = "Kg", CategoryId = 1 },
                new Product { ProductId = 3, Barcode = "AP-000003", ProductName = "Khoai tây", CostPrice = 18000, SellingPrice = 25000, StockQuantity = 30, Unit = "Kg", CategoryId = 1 },
                new Product { ProductId = 4, Barcode = "AP-000004", ProductName = "Thịt heo ba rọi 500g", CostPrice = 55000, SellingPrice = 69500, StockQuantity = 20, Unit = "Khay", CategoryId = 2 },
                new Product { ProductId = 5, Barcode = "AP-000005", ProductName = "Trứng gà hộp 10 quả", CostPrice = 25000, SellingPrice = 32000, StockQuantity = 25, Unit = "Hộp", CategoryId = 2 },
                new Product { ProductId = 6, Barcode = "AP-000006", ProductName = "Cá basa phi lê 500g", CostPrice = 26000, SellingPrice = 32500, StockQuantity = 15, Unit = "Khay", CategoryId = 2 },
                new Product { ProductId = 7, Barcode = "AP-000007", ProductName = "Sữa tươi Vinamilk 1L", CostPrice = 29000, SellingPrice = 36000, StockQuantity = 45, Unit = "Hộp", CategoryId = 3 },
                new Product { ProductId = 8, Barcode = "AP-000008", ProductName = "Sữa chua Vinamilk có đường 100g", CostPrice = 5500, SellingPrice = 7000, StockQuantity = 60, Unit = "Hũ", CategoryId = 3 },
                new Product { ProductId = 9, Barcode = "AP-000009", ProductName = "Phô mai Con Bò Cười 8 miếng", CostPrice = 26000, SellingPrice = 32000, StockQuantity = 20, Unit = "Hộp", CategoryId = 3 },
                new Product { ProductId = 10, Barcode = "AP-000010", ProductName = "Nước suối Aquafina 500ml", CostPrice = 4000, SellingPrice = 6000, StockQuantity = 100, Unit = "Chai", CategoryId = 4 },
                new Product { ProductId = 11, Barcode = "AP-000011", ProductName = "Coca-Cola 1.5L", CostPrice = 15000, SellingPrice = 19000, StockQuantity = 48, Unit = "Chai", CategoryId = 4 },
                new Product { ProductId = 12, Barcode = "AP-000012", ProductName = "Trà xanh Không Độ 455ml", CostPrice = 7000, SellingPrice = 9000, StockQuantity = 50, Unit = "Chai", CategoryId = 4 },
                new Product { ProductId = 13, Barcode = "AP-000013", ProductName = "Mì Hảo Hảo tôm chua cay 75g", CostPrice = 3200, SellingPrice = 4500, StockQuantity = 150, Unit = "Gói", CategoryId = 5 },
                new Product { ProductId = 14, Barcode = "AP-000014", ProductName = "Gạo ST25 túi 5kg", CostPrice = 135000, SellingPrice = 165000, StockQuantity = 18, Unit = "Túi", CategoryId = 5 },
                new Product { ProductId = 15, Barcode = "AP-000015", ProductName = "Cá hộp sốt cà 155g", CostPrice = 14000, SellingPrice = 18000, StockQuantity = 30, Unit = "Hộp", CategoryId = 5 },
                new Product { ProductId = 16, Barcode = "AP-000016", ProductName = "Nước mắm Nam Ngư 500ml", CostPrice = 22000, SellingPrice = 29000, StockQuantity = 25, Unit = "Chai", CategoryId = 6 },
                new Product { ProductId = 17, Barcode = "AP-000017", ProductName = "Dầu ăn Neptune 1L", CostPrice = 42000, SellingPrice = 52000, StockQuantity = 22, Unit = "Chai", CategoryId = 6 },
                new Product { ProductId = 18, Barcode = "AP-000018", ProductName = "Hạt nêm Knorr 400g", CostPrice = 30000, SellingPrice = 38000, StockQuantity = 28, Unit = "Gói", CategoryId = 6 },
                new Product { ProductId = 19, Barcode = "AP-000019", ProductName = "Snack khoai tây O'Star 35g", CostPrice = 5500, SellingPrice = 8000, StockQuantity = 40, Unit = "Gói", CategoryId = 7 },
                new Product { ProductId = 20, Barcode = "AP-000020", ProductName = "Bánh quy Oreo 133g", CostPrice = 17000, SellingPrice = 22000, StockQuantity = 32, Unit = "Gói", CategoryId = 7 },
                new Product { ProductId = 21, Barcode = "AP-000021", ProductName = "Kẹo dẻo trái cây 90g", CostPrice = 9000, SellingPrice = 12000, StockQuantity = 25, Unit = "Gói", CategoryId = 7 }
            );

            // ---------------------------------------------------------
            // 3. CẤU HÌNH VÀ SEED DATA CHO BẢNG CUSTOMERS (16 BẢN GHI)
            // ---------------------------------------------------------
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(c => c.CustomerId);
                entity.Property(c => c.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(15).IsUnicode(false);
                entity.HasIndex(c => c.PhoneNumber).IsUnique();
                entity.Property(c => c.Address).HasMaxLength(200);
                entity.Property(c => c.RewardPoints).HasDefaultValue(0);
                entity.Property(c => c.MembershipRank).HasMaxLength(50).HasDefaultValue("Chuẩn");
            });

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "Nguyễn Minh Anh", PhoneNumber = "0900000001", Address = "Ấp 1, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 25, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 2, CustomerName = "Trần Quốc Bảo", PhoneNumber = "0900000002", Address = "Ấp 2, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 85, MembershipRank = "Bạc" },
                new Customer { CustomerId = 3, CustomerName = "Lê Thị Ngọc", PhoneNumber = "0900000003", Address = "Ấp 3, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 15, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 4, CustomerName = "Phạm Thanh Tùng", PhoneNumber = "0900000004", Address = "Ấp 4, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 240, MembershipRank = "Vàng" },
                new Customer { CustomerId = 5, CustomerName = "Hoàng Thu Hà", PhoneNumber = "0900000005", Address = "Ấp 5, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 60, MembershipRank = "Bạc" },
                new Customer { CustomerId = 6, CustomerName = "Võ Thành Đạt", PhoneNumber = "0900000006", Address = "Ấp 6, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 320, MembershipRank = "Vàng" },
                new Customer { CustomerId = 7, CustomerName = "Đặng Mỹ Linh", PhoneNumber = "0900000007", Address = "Khu dân cư An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 0, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 8, CustomerName = "Bùi Văn Hùng", PhoneNumber = "0900000008", Address = "Ấp 1, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 120, MembershipRank = "Bạc" },
                new Customer { CustomerId = 9, CustomerName = "Đỗ Thị Mai", PhoneNumber = "0900000009", Address = "Ấp 2, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 45, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 10, CustomerName = "Ngô Quốc Khánh", PhoneNumber = "0900000010", Address = "Ấp 3, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 0, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 11, CustomerName = "Dương Khánh Vy", PhoneNumber = "0900000011", Address = "Ấp 4, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 450, MembershipRank = "Vàng" },
                new Customer { CustomerId = 12, CustomerName = "Lý Minh Triết", PhoneNumber = "0900000012", Address = "Ấp 5, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 90, MembershipRank = "Bạc" },
                new Customer { CustomerId = 13, CustomerName = "Phan Thùy Trang", PhoneNumber = "0900000013", Address = "Ấp 6, xã An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 180, MembershipRank = "Bạc" },
                new Customer { CustomerId = 14, CustomerName = "Vũ Hải Đăng", PhoneNumber = "0900000014", Address = "Khu dân cư An Phước, huyện Long Thành, Đồng Nai", RewardPoints = 25, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 15, CustomerName = "Trịnh Xuân Trường", PhoneNumber = "0900000015", Address = "Xã Long Đức, huyện Long Thành, Đồng Nai", RewardPoints = 280, MembershipRank = "Vàng" },
                new Customer { CustomerId = 16, CustomerName = "Nguyễn Ngọc Ánh", PhoneNumber = "0900000016", Address = "Thị trấn Long Thành, huyện Long Thành, Đồng Nai", RewardPoints = 510, MembershipRank = "Bạch Kim" }
            );

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.OrderCode).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.HasIndex(o => o.OrderCode).IsUnique();
                entity.Property(o => o.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
                entity.Property(o => o.CashierUsername).IsRequired().HasMaxLength(50);
                entity.Property(o => o.FinalAmount).HasColumnType("decimal(18,2)");
                entity.Property(o => o.RewardPoints).HasDefaultValue(0);
                entity.HasOne(o => o.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, OrderCode = "AP-HD-20261016-001", CreatedDate = new DateTime(2026, 10, 16, 9, 12, 0), CashierUsername = "cashier01", CustomerId = 1, TotalItems = 3, FinalAmount = 30000, RewardPoints = 3 },
                new Order { OrderId = 2, OrderCode = "AP-HD-20261016-002", CreatedDate = new DateTime(2026, 10, 16, 10, 35, 0), CashierUsername = "cashier01", CustomerId = 3, TotalItems = 5, FinalAmount = 54000, RewardPoints = 5 },
                new Order { OrderId = 3, OrderCode = "AP-HD-20261016-003", CreatedDate = new DateTime(2026, 10, 16, 11, 5, 0), CashierUsername = "cashier02", CustomerId = 4, TotalItems = 3, FinalAmount = 241000, RewardPoints = 24 }
            );

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetails");
                entity.HasKey(d => d.OrderDetailId);
                entity.Property(d => d.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(d => d.LineTotal).HasColumnType("decimal(18,2)");
                entity.HasCheckConstraint("CK_OrderDetails_Quantity", "[Quantity] > 0");
                entity.HasOne(d => d.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail { OrderDetailId = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 12000, LineTotal = 24000 },
                new OrderDetail { OrderDetailId = 2, OrderId = 1, ProductId = 10, Quantity = 1, UnitPrice = 6000, LineTotal = 6000 },
                new OrderDetail { OrderDetailId = 3, OrderId = 2, ProductId = 13, Quantity = 4, UnitPrice = 4500, LineTotal = 18000 },
                new OrderDetail { OrderDetailId = 4, OrderId = 2, ProductId = 7, Quantity = 1, UnitPrice = 36000, LineTotal = 36000 },
                new OrderDetail { OrderDetailId = 5, OrderId = 3, ProductId = 14, Quantity = 1, UnitPrice = 165000, LineTotal = 165000 },
                new OrderDetail { OrderDetailId = 6, OrderId = 3, ProductId = 18, Quantity = 2, UnitPrice = 38000, LineTotal = 76000 }
            );
        }
    }
}