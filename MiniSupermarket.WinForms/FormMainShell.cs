#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormMainShell : Form
    {
        private Form? _activeForm = null;

        public FormMainShell()
        {
            InitializeComponent();
        }

        private void FormMainShell_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin người dùng hiện tại
            lblUserInfo.Text = $"Nhân viên: {SessionManager.CurrentUsername} | Vai trò: [{SessionManager.CurrentRole}]";

            // Áp dụng phân quyền hiển thị menu Sidebar
            ApplyRolePermissions(SessionManager.CurrentRole);

            // Mở màn hình mặc định tương ứng với vai trò
            OpenDefaultScreenByRole(SessionManager.CurrentRole);
        }

        private void OpenChildForm(Form childForm, string screenTitle, Button senderButton)
        {
            if (_activeForm != null)
            {
                _activeForm.Close();
            }

            HighlightActiveButton(senderButton);

            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelMainContent.Controls.Clear();
            panelMainContent.Controls.Add(childForm);
            panelMainContent.Tag = childForm;

            lblTitle.Text = screenTitle;
            childForm.BringToFront();
            childForm.Show();
        }

        private void HighlightActiveButton(Button activeButton)
        {
            foreach (Control ctrl in panelSidebar.Controls)
            {
                if (ctrl is Button btn && btn != btnLogout)
                {
                    btn.BackColor = Color.FromArgb(24, 30, 48);
                }
            }
            activeButton.BackColor = Color.FromArgb(41, 100, 180);
        }

        /// <summary>
        /// Phân quyền ẩn / hiện menu Sidebar dựa vào Role từ JWT Token
        /// </summary>
        private void ApplyRolePermissions(string role)
        {
            string upperRole = role.ToUpper();

            switch (upperRole)
            {
                case "ADMIN":
                    // Admin được thấy toàn bộ menu
                    btnPOS.Visible = true;
                    btnCategory.Visible = true;
                    btnProduct.Visible = true;
                    btnCustomer.Visible = true;
                    btnReports.Visible = true;
                    btnUserManage.Visible = true;
                    break;

                case "CASHIER":
                    // Bán hàng (Thu ngân) chỉ được xem POS & Khách hàng
                    btnPOS.Visible = true;
                    btnCustomer.Visible = true;
                    btnCategory.Visible = false;
                    btnProduct.Visible = false;
                    btnReports.Visible = false;
                    btnUserManage.Visible = false;
                    break;

                case "WAREHOUSE":
                    // Nhân viên kho chỉ được làm việc với Danh mục & Sản phẩm
                    btnPOS.Visible = false;
                    btnCustomer.Visible = false;
                    btnReports.Visible = false;
                    btnUserManage.Visible = false;
                    btnCategory.Visible = true;
                    btnProduct.Visible = true;
                    break;

                default:
                    MessageBox.Show("Tài khoản chưa được phân quyền hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    break;
            }
        }

        private void OpenDefaultScreenByRole(string role)
        {
            string upperRole = role.ToUpper();
            if (upperRole == "ADMIN" || upperRole == "WAREHOUSE")
            {
                OpenChildForm(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC NHÓM HÀNG", btnCategory);
            }
            else if (upperRole == "CASHIER")
            {
                OpenChildForm(new FormCustomerManagement(), "QUẢN LÝ KHÁCH HÀNG & TÍCH ĐIỂM", btnCustomer);
            }
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC SẢN PHẨM", btnCategory);
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCustomerManagement(), "QUẢN LÝ KHÁCH HÀNG THÂN THIẾT", btnCustomer);
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormProductManagement(), "QUẢN LÝ SẢN PHẨM & KHO HÀNG", btnProduct);
        }

        private void btnPOS_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Màn hình Quầy bán hàng (POS) đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            if (SessionManager.CurrentRole.ToUpper() != "ADMIN")
            {
                MessageBox.Show("Bạn không có quyền truy cập báo cáo doanh thu!", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            MessageBox.Show("Màn hình Báo cáo Doanh thu đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUserManage_Click(object sender, EventArgs e)
        {
            if (SessionManager.CurrentRole.ToUpper() != "ADMIN")
            {
                MessageBox.Show("Bạn không có quyền quản trị tài khoản!", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            MessageBox.Show("Màn hình Quản trị Tài khoản đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Xử lý Đăng xuất & Quay về màn hình Đăng nhập
        /// </summary>
        private void btnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn đăng xuất tài khoản này không?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 1. Xóa thông tin phiên làm việc
                SessionManager.JwtToken = string.Empty;
                SessionManager.CurrentUsername = string.Empty;
                SessionManager.CurrentRole = string.Empty;

                // 2. Mở lại Form Đăng nhập (Thay FormLogin bằng tên Form đăng nhập trong dự án của bạn)
                this.Hide();
                FormLogin loginForm = new FormLogin();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Đăng nhập tài khoản mới thành công -> Khởi tạo lại Main Shell
                    this.Show();
                    FormMainShell_Load(sender, e);
                }
                else
                {
                    // Nếu tắt form đăng nhập thì đóng hẳn ứng dụng
                    this.Close();
                }
            }
        }
    }
}