#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormMainShell : Form
    {
        private Form? _activeForm = null;
        private Timer? _clockTimer;

        public FormMainShell()
        {
            InitializeComponent();
        }

        private void FormMainShell_Load(object sender, EventArgs e)
        {
            lblUserInfo.Text = $"{SessionManager.CurrentUsername}  ·  {SessionManager.CurrentRole}  ·  {DateTime.Now:HH:mm:ss}";
            _clockTimer = new Timer { Interval = 1000 };
            _clockTimer.Tick += (_, _) => lblUserInfo.Text = $"{SessionManager.CurrentUsername}  ·  {SessionManager.CurrentRole}  ·  {DateTime.Now:HH:mm:ss}";
            _clockTimer.Start();

            if (!ApplyRolePermissions(SessionManager.CurrentRole))
            {
                MessageBox.Show("Tài khoản chưa được phân quyền hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            OpenDefaultScreenByRole(SessionManager.CurrentRole);
        }

        private bool OpenChildForm(Form childForm, string screenTitle, Button senderButton)
        {
            if (!CanAccess(senderButton))
            {
                childForm.Dispose();
                MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm.Dispose();
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
            return true;
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
        private bool ApplyRolePermissions(string role)
        {
            string upperRole = role.Trim().ToUpperInvariant();

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
                    return true;

                case "CASHIER":
                    // Bán hàng (Thu ngân) chỉ được xem POS & Khách hàng
                    btnPOS.Visible = true;
                    btnCustomer.Visible = true;
                    btnCategory.Visible = false;
                    btnProduct.Visible = false;
                    btnReports.Visible = false;
                    btnUserManage.Visible = false;
                    return true;

                case "WAREHOUSE":
                    // Nhân viên kho chỉ được làm việc với Danh mục & Sản phẩm
                    btnPOS.Visible = false;
                    btnCustomer.Visible = false;
                    btnReports.Visible = false;
                    btnUserManage.Visible = false;
                    btnCategory.Visible = true;
                    btnProduct.Visible = true;
                    return true;

                default:
                    btnPOS.Visible = false;
                    btnCategory.Visible = false;
                    btnProduct.Visible = false;
                    btnCustomer.Visible = false;
                    btnReports.Visible = false;
                    btnUserManage.Visible = false;
                    return false;
            }
        }

        private bool CanAccess(Button target)
        {
            string role = SessionManager.CurrentRole.Trim();
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (role.Equals("Cashier", StringComparison.OrdinalIgnoreCase))
            {
                return target == btnPOS || target == btnCustomer;
            }

            if (role.Equals("Warehouse", StringComparison.OrdinalIgnoreCase))
            {
                return target == btnCategory || target == btnProduct;
            }

            return false;
        }

        private void OpenDefaultScreenByRole(string role)
        {
            switch (role.Trim().ToUpperInvariant())
            {
                case "ADMIN":
                    OpenChildForm(new FormCategoryManagement(), "TRANG CHỦ  >  DANH MỤC", btnCategory);
                    break;
                case "CASHIER":
                    OpenChildForm(new FormPosDashboard(), "TRANG CHỦ  >  POS", btnPOS);
                    break;
                case "WAREHOUSE":
                    OpenChildForm(new FormProductCatalog(), "TRANG CHỦ  >  SẢN PHẨM", btnProduct);
                    break;
            }
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCategoryManagement(), "TRANG CHỦ  >  DANH MỤC", btnCategory);
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCustomerDashboard(), "TRANG CHỦ  >  KHÁCH HÀNG", btnCustomer);
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormProductCatalog(), "TRANG CHỦ  >  SẢN PHẨM", btnProduct);
        }

        private void btnPOS_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormPosDashboard(), "TRANG CHỦ  >  POS", btnPOS);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormAnalyticsDashboard(), "TRANG CHỦ  >  BÁO CÁO DOANH THU", btnReports);
        }

        private void btnUserManage_Click(object sender, EventArgs e)
        {
            OpenChildForm(
                new FormModulePlaceholder("Cài đặt & phân quyền", "Tài khoản đăng nhập hiện là dữ liệu demo trong bộ nhớ API; màn hình CRUD chưa được kết nối với bảng Users."),
                "TRANG CHỦ  >  CÀI ĐẶT",
                btnUserManage);
        }

        /// <summary>
        /// Xử lý Đăng xuất & Quay về màn hình Đăng nhập
        /// </summary>
        private void btnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn đăng xuất tài khoản này không?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SessionManager.JwtToken = string.Empty;
                SessionManager.CurrentUsername = string.Empty;
                SessionManager.CurrentRole = string.Empty;
                Close();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clockTimer?.Stop();
            _clockTimer?.Dispose();
            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm.Dispose();
                _activeForm = null;
            }

            base.OnFormClosed(e);
        }
    }
}