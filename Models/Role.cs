using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models
{
    // Lớp biểu diễn thực thể Vai trò nhân viên trong siêu thị mini
    public class Role
    {
        // Mã định danh vai trò (Khóa chính)
        public int Id { get; set; }

        // Tên vai trò (Ví dụ: Admin, Cashier, Warehouse)
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [StringLength(100, ErrorMessage = "Tên vai trò tối đa 100 ký tự")]
        public string RoleName { get; set; } = string.Empty;

        // Mô tả chi tiết chức năng của vai trò
        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Description { get; set; }
    }
}
