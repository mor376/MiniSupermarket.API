using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSupermarket.API.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Mã sản phẩm không được trống")]
        [StringLength(50)]
        public string Barcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal SellingPrice { get; set; }

        [NotMapped]
        public decimal Price
        {
            get => SellingPrice;
            set => SellingPrice = value;
        }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Bình thường";

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = "Cái";

        // Khóa ngoại liên kết tới bảng Categories
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // BỔ SUNG: Thuộc tính lấy Tên nhóm hàng (Không lưu trong DB)
        [NotMapped]
        public string CategoryName => Category != null ? Category.CategoryName : string.Empty;

        [NotMapped]
        public string SupplierName => ResolveSupplierName(CategoryName);

        private static string ResolveSupplierName(string? categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return "Công ty TNHH Nông sản An Phước";
            }

            var normalized = categoryName.Trim();
            if (normalized.Contains("Rau", StringComparison.OrdinalIgnoreCase) || normalized.Contains("trái", StringComparison.OrdinalIgnoreCase))
            {
                return "Công ty TNHH Nông sản An Phước";
            }
            if (normalized.Contains("Thịt", StringComparison.OrdinalIgnoreCase) || normalized.Contains("cá", StringComparison.OrdinalIgnoreCase) || normalized.Contains("trứng", StringComparison.OrdinalIgnoreCase))
            {
                return "Công ty CP Thực phẩm Sài Gòn";
            }
            if (normalized.Contains("Sữa", StringComparison.OrdinalIgnoreCase))
            {
                return "Công ty TNHH Sữa Quốc Tế";
            }
            if (normalized.Contains("Nước", StringComparison.OrdinalIgnoreCase) || normalized.Contains("giải", StringComparison.OrdinalIgnoreCase))
            {
                return "Công ty CP Nước giải khát Long Thành";
            }
            if (normalized.Contains("Mì", StringComparison.OrdinalIgnoreCase) || normalized.Contains("gạo", StringComparison.OrdinalIgnoreCase) || normalized.Contains("khô", StringComparison.OrdinalIgnoreCase))
            {
                return "Công ty TNHH Hàng tiêu dùng Miền Nam";
            }
            if (normalized.Contains("Gia", StringComparison.OrdinalIgnoreCase) || normalized.Contains("dầu", StringComparison.OrdinalIgnoreCase))
            {
                return "Công ty CP Gia vị & Dầu ăn Việt";
            }
            if (normalized.Contains("Bánh", StringComparison.OrdinalIgnoreCase) || normalized.Contains("kẹo", StringComparison.OrdinalIgnoreCase) || normalized.Contains("snack", StringComparison.OrdinalIgnoreCase))
            {
                return "Công ty CP Bánh kẹo & Snack HCM";
            }

            return "Công ty TNHH TM Mặt hàng tươi An Nam";
        }
    }
}