using System;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // THỬ NGHIỆM ĐĂNG NHẬP THEO VAI TRÒ (Bỏ comment 1 trong 3 dòng dưới để kiểm thử):

            // Dòng 1: Đăng nhập vai trò Admin (Hiển thị toàn bộ 6 nút chức năng)
            SessionManager.CurrentUsername = "admin01";
            SessionManager.CurrentRole = "Admin";

            // Dòng 2: Đăng nhập vai trò Cashier (Chỉ hiển thị nút Bán hàng POS và Khách hàng)
            // SessionManager.CurrentUsername = "cashier01";
            // SessionManager.CurrentRole = "Cashier";

            // Dòng 3: Đăng nhập vai trò Warehouse (Chỉ hiển thị nút Danh mục và Sản phẩm)
            // SessionManager.CurrentUsername = "ware01";
            // SessionManager.CurrentRole = "Warehouse";

            Application.Run(new FormMainShell());
        }
    }
}