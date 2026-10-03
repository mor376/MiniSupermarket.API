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
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------------------------------------------
            // 1. CẤU HÌNH VÀ SEED DATA CHO BẢNG CATEGORIES (5 BẢN GHI)
            // ---------------------------------------------------------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(255);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Bánh kẹo & Đồ ăn vặt", Description = "Snack, bánh quy, kẹo dẻo" },
                new Category { CategoryId = 2, CategoryName = "Nước giải khát & Trà", Description = "Nước ngọt, nước khoáng, trà" },
                new Category { CategoryId = 3, CategoryName = "Sữa & Sản phẩm từ sữa", Description = "Sữa tươi, sữa chua, phô mai" },
                new Category { CategoryId = 4, CategoryName = "Mì gói & Thực phẩm ăn liền", Description = "Mì ăn liền, phở khô, cháo gói" },
                new Category { CategoryId = 5, CategoryName = "Gia vị & Dầu ăn", Description = "Nước mắm, hạt nêm, dầu thực vật" }
            );

            // ---------------------------------------------------------
            // 2. CẤU HÌNH VÀ SEED DATA CHO BẢNG PRODUCTS (HIỆN DỮ LIỆU ĐỒNG BỘ FK)
            // ---------------------------------------------------------
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.Barcode).IsRequired().HasMaxLength(50);
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

                // Cấu hình quan hệ Khóa ngoại liên kết với Categories
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Barcode = "8934563101011", ProductName = "Snack Khoai Tây Lay's 68g", Price = 12000, StockQuantity = 50, CategoryId = 1 },
                new Product { ProductId = 2, Barcode = "8934563101028", ProductName = "Bánh Quy Oreo Vị Vani 133g", Price = 18000, StockQuantity = 40, CategoryId = 1 },
                new Product { ProductId = 3, Barcode = "8934563202018", ProductName = "Nước Ngọt Coca Cola 320ml", Price = 10000, StockQuantity = 100, CategoryId = 2 },
                new Product { ProductId = 4, Barcode = "8934563202025", ProductName = "Trà Xanh C2 Hương Chanh 360ml", Price = 9000, StockQuantity = 80, CategoryId = 2 },
                new Product { ProductId = 5, Barcode = "8934563303015", ProductName = "Sữa Tươi Tiệt Trùng Vinamilk 180ml", Price = 8500, StockQuantity = 120, CategoryId = 3 },
                new Product { ProductId = 6, Barcode = "8934563404012", ProductName = "Mì Hảo Hảo Tôm Chua Cay 75g", Price = 4500, StockQuantity = 200, CategoryId = 4 },
                new Product { ProductId = 7, Barcode = "8934563505019", ProductName = "Nước Mắm Nam Ngư 500ml", Price = 35000, StockQuantity = 30, CategoryId = 5 }
            );

            // ---------------------------------------------------------
            // 3. CẤU HÌNH VÀ SEED DATA CHO BẢNG CUSTOMERS (16 BẢN GHI)
            // ---------------------------------------------------------
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(c => c.CustomerId);
                entity.Property(c => c.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(c => c.Address).HasMaxLength(200);
                entity.Property(c => c.RewardPoints).HasDefaultValue(0);
                entity.Property(c => c.MembershipRank).HasMaxLength(50).HasDefaultValue("Chuẩn");
            });

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "Nguyễn Văn A", PhoneNumber = "0901122334", Address = "387 Lê Văn Việt, Phường Tăng Nhơn Phú A, Thành phố Thủ Đức, TP. Hồ Chí Minh", RewardPoints = 150, MembershipRank = "Vàng" },
                new Customer { CustomerId = 2, CustomerName = "Trần Thị B", PhoneNumber = "0918877665", Address = "12 Phố Hàng Bài, Phường Tràng Tiền, Quận Hoàn Kiếm, Hà Nội", RewardPoints = 50, MembershipRank = "Bạc" },
                new Customer { CustomerId = 3, CustomerName = "Lê Văn C", PhoneNumber = "0983344556", Address = "45 Nguyễn Văn Linh, Phường Bình Hiên, Quận Hải Châu, Đà Nẵng", RewardPoints = 10, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 4, CustomerName = "Phạm Hoàng Dũng", PhoneNumber = "0934567890", Address = "102 Xô Viết Nghệ Tĩnh, Phường 21, Quận Bình Thạnh, TP. Hồ Chí Minh", RewardPoints = 320, MembershipRank = "Kim Cương" },
                new Customer { CustomerId = 5, CustomerName = "Hoàng Thi Hương", PhoneNumber = "0971122334", Address = "88 Cầu Giấy, Phường Quan Hoa, Quận Cầu Giấy, Hà Nội", RewardPoints = 85, MembershipRank = "Bạc" },
                new Customer { CustomerId = 6, CustomerName = "Vũ Đình Trọng", PhoneNumber = "0909988776", Address = "15 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh", RewardPoints = 210, MembershipRank = "Vàng" },
                new Customer { CustomerId = 7, CustomerName = "Đặng Mai Phương", PhoneNumber = "0965432109", Address = "230 Trần Hưng Đạo, Phường An Hòa, Quận Ninh Kiều, Cần Thơ", RewardPoints = 15, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 8, CustomerName = "Bùi Anh Tuấn", PhoneNumber = "0943210987", Address = "67 Quang Trung, Phường Lộc Thọ, Thành phố Nha Trang, Khánh Hòa", RewardPoints = 120, MembershipRank = "Vàng" },
                new Customer { CustomerId = 9, CustomerName = "Đỗ Thảo Nguyên", PhoneNumber = "0912345678", Address = "54 Lạch Tray, Phường Hàng Kênh, Quận Lê Chân, Hải Phòng", RewardPoints = 45, MembershipRank = "Bạc" },
                new Customer { CustomerId = 10, CustomerName = "Ngo Quoc Bao", PhoneNumber = "0987654321", Address = "19 Hùng Vương, Phường Phú Hội, Thành phố Huế, Thừa Thiên Huế", RewardPoints = 0, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 11, CustomerName = "Dương Khánh Linh", PhoneNumber = "0931122334", Address = "78 Nguyễn Trãi, Phường 3, Quận 5, TP. Hồ Chí Minh", RewardPoints = 450, MembershipRank = "Kim Cương" },
                new Customer { CustomerId = 12, CustomerName = "Lý Minh Triết", PhoneNumber = "0978899001", Address = "143 Nguyễn Chí Thanh, Phường Láng Hạ, Quận Đống Đa, Hà Nội", RewardPoints = 90, MembershipRank = "Bạc" },
                new Customer { CustomerId = 13, CustomerName = "Phan Thùy Trang", PhoneNumber = "0908877665", Address = "25 Lê Hồng Phong, Phường 4, Thành phố Đà Lạt, Lâm Đồng", RewardPoints = 180, MembershipRank = "Vàng" },
                new Customer { CustomerId = 14, CustomerName = "Vũ Hải Đăng", PhoneNumber = "0961122334", Address = "89 Đường 30 Tháng 4, Phường Trung Dũng, Thành phố Biên Hòa, Đồng Nai", RewardPoints = 25, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 15, CustomerName = "Trịnh Xuân Trường", PhoneNumber = "0945566778", Address = "312 Đại lộ Bình Dương, Phường Phú Hòa, Thành phố Thủ Dầu Một, Bình Dương", RewardPoints = 280, MembershipRank = "Vàng" },
                new Customer { CustomerId = 16, CustomerName = "Nguyễn Thị Ngọc Ánh", PhoneNumber = "0919988776", Address = "56 Điện Biên Phủ, Phường Chính Gián, Quận Thanh Khê, Đà Nẵng", RewardPoints = 510, MembershipRank = "Kim Cương" }
            );
        }
    }
}