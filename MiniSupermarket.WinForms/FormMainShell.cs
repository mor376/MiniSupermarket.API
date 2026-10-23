#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private void FormMainShell_Load(object? sender, EventArgs e)
        {
            UpdateUserInfoAndClock();

            // Khởi tạo Timer chỉ để cập nhật đồng hồ (mỗi giây) tránh vẽ lại avatar/gây nháy hình
            _clockTimer = new Timer { Interval = 1000 };
            _clockTimer.Tick += (_, _) =>
            {
                if (lblUserInfo != null)
                {
                    var displayName = string.IsNullOrWhiteSpace(SessionManager.CurrentDisplayName)
                        ? SessionManager.CurrentUsername
                        : SessionManager.CurrentDisplayName;
                    var role = string.IsNullOrWhiteSpace(SessionManager.CurrentRole) ? "Tài khoản" : SessionManager.CurrentRole;
                    lblUserInfo.Text = $"{displayName}  ·  {role}  ·  {DateTime.Now:HH:mm:ss}";
                }
            };
            _clockTimer.Start();

            if (!ApplyRolePermissions(SessionManager.CurrentRole))
            {
                MessageBox.Show("Tài khoản chưa được phân quyền hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            OpenDefaultScreenByRole(SessionManager.CurrentRole);
        }

        private void UpdateUserInfoAndClock()
        {
            var displayName = string.IsNullOrWhiteSpace(SessionManager.CurrentDisplayName)
                ? SessionManager.CurrentUsername
                : SessionManager.CurrentDisplayName;

            var role = string.IsNullOrWhiteSpace(SessionManager.CurrentRole) ? "Tài khoản" : SessionManager.CurrentRole;

            if (lblUserName != null) lblUserName.Text = string.IsNullOrWhiteSpace(displayName) ? "Người dùng" : displayName;
            if (lblUserRole != null) lblUserRole.Text = role;
            if (pictureAvatar != null) pictureAvatar.Image = CreateAvatarImage(displayName);
            if (lblUserInfo != null) lblUserInfo.Text = $"{displayName}  ·  {role}  ·  {DateTime.Now:HH:mm:ss}";
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

            if (lblTitle != null) lblTitle.Text = screenTitle;
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
                    btn.ForeColor = Color.FromArgb(226, 232, 240);
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(37, 99, 235);
                }
            }

            activeButton.BackColor = Color.FromArgb(37, 99, 235);
            activeButton.ForeColor = Color.White;
            activeButton.FlatAppearance.BorderSize = 0;
            activeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
        }

        private static string NormalizeRole(string? role)
        {
            return (role ?? string.Empty).Trim();
        }

        private static HashSet<string> GetPermissionsForRole(string? role)
        {
            var permissions = new HashSet<string>(RolePermissionCatalog.GetEffectivePermissions(role), StringComparer.OrdinalIgnoreCase);
            if (permissions.Count == 0)
            {
                var normalized = NormalizeRole(role);
                if (normalized.Equals("ADMIN", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("ADMINISTRATOR", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("SUPERVISOR", StringComparison.OrdinalIgnoreCase))
                {
                    permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "POS", "CATEGORY", "PRODUCT", "CUSTOMER", "REPORTS", "USER_MANAGE"
                    };
                }
                else if (normalized.Equals("CASHIER", StringComparison.OrdinalIgnoreCase))
                {
                    permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "POS", "CUSTOMER"
                    };
                }
                else if (normalized.Equals("WAREHOUSE", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("WAREHOUSEMAN", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("STOCK", StringComparison.OrdinalIgnoreCase))
                {
                    permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "CATEGORY", "PRODUCT"
                    };
                }
                else if (string.IsNullOrWhiteSpace(normalized))
                {
                    permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "POS", "CATEGORY", "PRODUCT", "CUSTOMER", "REPORTS", "USER_MANAGE"
                    };
                }
            }
            return permissions;
        }

        private bool ApplyRolePermissions(string role)
        {
            var permissions = GetPermissionsForRole(role);

            if (btnPOS != null) btnPOS.Visible = permissions.Contains("POS");
            if (btnCategory != null) btnCategory.Visible = permissions.Contains("CATEGORY");
            if (btnProduct != null) btnProduct.Visible = permissions.Contains("PRODUCT");
            if (btnCustomer != null) btnCustomer.Visible = permissions.Contains("CUSTOMER");
            if (btnReports != null) btnReports.Visible = permissions.Contains("REPORTS");
            if (btnUserManage != null) btnUserManage.Visible = permissions.Contains("USER_MANAGE");

            return permissions.Count > 0;
        }

        private bool CanAccess(Button target)
        {
            var permissions = GetPermissionsForRole(SessionManager.CurrentRole);

            if (target == btnPOS) return permissions.Contains("POS");
            if (target == btnCategory) return permissions.Contains("CATEGORY");
            if (target == btnProduct) return permissions.Contains("PRODUCT");
            if (target == btnCustomer) return permissions.Contains("CUSTOMER");
            if (target == btnReports) return permissions.Contains("REPORTS");
            if (target == btnUserManage) return permissions.Contains("USER_MANAGE");

            return false;
        }

        private void OpenDefaultScreenByRole(string role)
        {
            if (RolePermissionCatalog.HasPermission(role, "POS") || GetPermissionsForRole(role).Contains("POS"))
            {
                OpenChildForm(new FormPOS(), "TRANG CHỦ  >  POS", btnPOS);
                return;
            }

            if (RolePermissionCatalog.HasPermission(role, "CUSTOMER") || GetPermissionsForRole(role).Contains("CUSTOMER"))
            {
                OpenChildForm(new FormCustomerDashboard(), "TRANG CHỦ  >  KHÁCH HÀNG", btnCustomer);
                return;
            }

            if (RolePermissionCatalog.HasPermission(role, "PRODUCT") || GetPermissionsForRole(role).Contains("PRODUCT"))
            {
                OpenChildForm(new FormInventoryView(), "TRANG CHỦ  >  QUẢN LÝ KHO & NHẬP HÀNG", btnCategory);
                return;
            }

            if (RolePermissionCatalog.HasPermission(role, "REPORTS") || GetPermissionsForRole(role).Contains("REPORTS"))
            {
                OpenChildForm(new FormAnalyticsDashboard(), "TRANG CHỦ  >  BÁO CÁO DOANH THU", btnReports);
                return;
            }

            if (RolePermissionCatalog.HasPermission(role, "USER_MANAGE") || GetPermissionsForRole(role).Contains("USER_MANAGE"))
            {
                OpenChildForm(new FormRolePermissionManagement(), "TRANG CHỦ  >  CÀI ĐẶT", btnUserManage);
                return;
            }

            if (NormalizeRole(role).Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                OpenChildForm(new FormPOS(), "TRANG CHỦ  >  POS", btnPOS);
            }
        }

        private static Bitmap CreateAvatarImage(string? displayName)
        {
            var name = string.IsNullOrWhiteSpace(displayName) ? "U" : displayName.Trim();
            var firstChar = name.FirstOrDefault(char.IsLetterOrDigit);
            var initial = char.IsLetterOrDigit(firstChar) ? char.ToUpperInvariant(firstChar).ToString() : "U";

            var bitmap = new Bitmap(64, 64);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.FromArgb(37, 99, 235));
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.FillEllipse(new SolidBrush(Color.FromArgb(59, 130, 246)), 0, 0, 64, 64);

            using var font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(Color.White);
            using var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            graphics.DrawString(initial, font, brush, new RectangleF(0, 0, 64, 64), format);
            return bitmap;
        }

        private void btnCategory_Click(object? sender, EventArgs e)
        {
            if (!CanAccess(btnCategory))
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng Quản lý Kho & Nhập hàng.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormInventoryView(), "TRANG CHỦ  >  QUẢN LÝ KHO & NHẬP HÀNG", btnCategory);
        }

        private void btnCustomer_Click(object? sender, EventArgs e)
        {
            if (!CanAccess(btnCustomer))
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng Khách hàng.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormCustomerDashboard(), "TRANG CHỦ  >  KHÁCH HÀNG", btnCustomer);
        }

        private void btnProduct_Click(object? sender, EventArgs e)
        {
            if (!CanAccess(btnProduct))
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng Danh mục Sản phẩm.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormProductCatalog(), "TRANG CHỦ  >  DANH MỤC SẢN PHẨM & GIÁ", btnProduct);
        }

        private void btnPOS_Click(object? sender, EventArgs e)
        {
            if (!CanAccess(btnPOS))
            {
                MessageBox.Show("Bạn không có quyền truy cập màn hình POS.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormPOS(), "TRANG CHỦ  >  POS", btnPOS);
        }

        private void btnReports_Click(object? sender, EventArgs e)
        {
            if (!CanAccess(btnReports))
            {
                MessageBox.Show("Bạn không có quyền xem báo cáo doanh thu.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormAnalyticsDashboard(), "TRANG CHỦ  >  BÁO CÁO DOANH THU", btnReports);
        }

        private void btnUserManage_Click(object? sender, EventArgs e)
        {
            if (!CanAccess(btnUserManage))
            {
                MessageBox.Show("Bạn không có quyền quản trị tài khoản hệ thống.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormRolePermissionManagement(), "TRANG CHỦ  >  CÀI ĐẶT", btnUserManage);
        }

        private void btnLogout_Click(object? sender, EventArgs e)
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