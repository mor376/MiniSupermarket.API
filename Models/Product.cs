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

        [Required(ErrorMessage = "Mã vạch sản phẩm không được trống")]
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
    }
}