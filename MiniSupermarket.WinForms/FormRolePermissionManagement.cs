using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public sealed class FormRolePermissionManagement : Form
    {
        private static readonly Color InkColor = Color.FromArgb(17, 24, 39);
        private static readonly Color SubTextColor = Color.FromArgb(107, 114, 128);
        private static readonly Color DarkButtonColor = Color.FromArgb(31, 41, 55);
        private static readonly Color LightGrayBg = Color.FromArgb(244, 245, 247);

        private DataGridView _grid = new();
        private Label _lblUserDetails = new();
        private Button _btnAddAccount = new();
        private Button _btnSave = new();
        private Button _btnStoreProfile = new();
        private Button _btnPosConfig = new();
        private Button _btnHistory = new();

        public FormRolePermissionManagement()
        {
            if (!RolePermissionCatalog.HasPermission(SessionManager.CurrentRole, "USER_MANAGE"))
            {
                MessageBox.Show("Bạn không có quyền quản trị phân quyền hệ thống.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
                return;
            }

            Text = "Cài đặt phân quyền";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = LightGrayBg;
            Padding = new Padding(16, 10, 16, 12);
            Font = new Font("Segoe UI", 9.5F);
            MinimumSize = new Size(1100, 700);
            Size = new Size(1180, 760);
            WindowState = FormWindowState.Normal;

            BuildLayout();
            Load += (_, _) => LoadGrid();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                BackColor = LightGrayBg,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            // Phân chia chiều cao từng phần chính xác
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));  // Top Breadcrumb & User Info
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));  // Title Bar
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));  // Section Header & Button F2
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // DataGridView List
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));  // Selected User Info Section
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));  // Bottom Action Buttons Section

            root.Controls.Add(BuildTopBar(), 0, 0);
            root.Controls.Add(BuildPageTitle(), 0, 1);
            root.Controls.Add(BuildTableTopBar(), 0, 2);
            root.Controls.Add(BuildGridSection(), 0, 3);
            root.Controls.Add(BuildSelectedInfoSection(), 0, 4);
            root.Controls.Add(BuildBottomActionSection(), 0, 5);

            Controls.Add(root);
        }

        private Control BuildTopBar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = LightGrayBg };
            var breadcrumb = new Label
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                Text = "Trang chủ  >  Phân quyền & Cài đặt",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = SubTextColor,
                TextAlign = ContentAlignment.MiddleLeft
            };
            var userInfo = new Label
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                Text = "Thu ngân: Nguyen Van A | Ca 1 ➔",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = DarkButtonColor,
                TextAlign = ContentAlignment.MiddleRight
            };
            panel.Controls.Add(userInfo);
            panel.Controls.Add(breadcrumb);
            return panel;
        }

        private Control BuildPageTitle()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = LightGrayBg };
            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "PHÂN QUYỀN VÀ CÀI ĐẶT ĐƠN GIẢN",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = InkColor,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildTableTopBar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = LightGrayBg };
            var title = new Label
            {
                Dock = DockStyle.Left,
                Text = "QUẢN LÝ TÀI KHOẢN NGƯỜI DÙNG",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = InkColor,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            _btnAddAccount = new Button
            {
                Dock = DockStyle.Right,
                Text = "THÊM TÀI KHOẢN (F2) 👤+",
                Width = 175,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = DarkButtonColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            _btnAddAccount.FlatAppearance.BorderSize = 0;
            _btnAddAccount.Click += (_, _) => CreateAccountPrompt();

            panel.Controls.Add(_btnAddAccount);
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildGridSection()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(1),
                Margin = new Padding(0, 0, 0, 8)
            };

            _grid.Dock = DockStyle.Fill;
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.None;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.RowHeadersVisible = true;
            _grid.RowHeadersWidth = 28;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = InkColor;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 26;
            _grid.RowTemplate.Height = 26;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            _grid.DefaultCellStyle.SelectionForeColor = InkColor;
            _grid.GridColor = Color.FromArgb(226, 232, 240);

            _grid.Columns.Clear();
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "UserId", DataPropertyName = "UserId", FillWeight = 10 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Username", DataPropertyName = "Username", FillWeight = 22 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Password", DataPropertyName = "Password", FillWeight = 20 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Full Name", DataPropertyName = "FullName", FillWeight = 26 });

            var roleCol = new DataGridViewComboBoxColumn
            {
                HeaderText = "Role",
                DataPropertyName = "Role",
                FillWeight = 18,
                ValueType = typeof(string),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            foreach (var role in RolePermissionCatalog.GetAllRoles())
            {
                roleCol.Items.Add(role);
            }
            _grid.Columns.Add(roleCol);

            _grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "IsActive", DataPropertyName = "IsActive", FillWeight = 10 });

            _grid.SelectionChanged += (_, _) => RefreshSelectionDetails();

            var menu = new ContextMenuStrip();
            menu.Items.Add("Xem chi tiết", null, (_, _) => { });
            menu.Items.Add("Sửa người dùng", null, (_, _) => { });
            menu.Items.Add("Reset mật khẩu", null, (_, _) => { });
            menu.Items.Add("Vô hiệu hóa", null, (_, _) => ToggleLockAccount());
            _grid.ContextMenuStrip = menu;

            card.Controls.Add(_grid);
            return card;
        }

        private Control BuildSelectedInfoSection()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 8, 12, 8),
                Margin = new Padding(0, 0, 0, 8)
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 20,
                Text = "THÔNG TIN TÀI KHOẢN ĐANG CHỌN (1)",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = SubTextColor
            };

            _lblUserDetails.Dock = DockStyle.Fill;
            _lblUserDetails.Text = "User details: - | -";
            _lblUserDetails.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblUserDetails.ForeColor = InkColor;
            _lblUserDetails.TextAlign = ContentAlignment.MiddleLeft;

            panel.Controls.Add(_lblUserDetails);
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildBottomActionSection()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 8, 12, 8)
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 18,
                Text = "CÁC CÀI ĐẶT KHÁCH",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = SubTextColor
            };

            var btnPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 4, 0, 0) };

            var flowLeft = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            _btnStoreProfile = MakeIconButton("🏪", "Hồ sơ cửa hàng");
            _btnPosConfig = MakeIconButton("🗄", "Cấu hình POS");
            _btnHistory = MakeIconButton("🕒", "Lịch sử");

            flowLeft.Controls.Add(_btnStoreProfile);
            flowLeft.Controls.Add(_btnPosConfig);
            flowLeft.Controls.Add(_btnHistory);

            _btnSave = new Button
            {
                Dock = DockStyle.Right,
                Text = "LƯU THAY ĐỔI",
                Width = 150,
                Height = 36,
                FlatStyle = FlatStyle.Flat,
                BackColor = DarkButtonColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += (_, _) => SaveCurrentPermissions();

            btnPanel.Controls.Add(flowLeft);
            btnPanel.Controls.Add(_btnSave);

            panel.Controls.Add(btnPanel);
            panel.Controls.Add(title);
            return panel;
        }

        private static Button MakeIconButton(string icon, string text)
        {
            var btn = new Button
            {
                Text = $"{icon}\n{text}",
                Width = 92,
                Height = 36,
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = InkColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                Margin = new Padding(0, 0, 8, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadGrid()
        {
            var validRoles = RolePermissionCatalog.GetAllRoles();
            var accounts = RolePermissionCatalog.Accounts.Select(a => new AccountRowModel
            {
                UserId = a.UserId,
                Username = a.Username,
                Password = "•••••••",
                FullName = a.FullName,
                Role = validRoles.Contains(a.Role, StringComparer.OrdinalIgnoreCase) ? a.Role : validRoles.FirstOrDefault() ?? "Admin",
                IsActive = a.IsActive
            }).ToList();

            _grid.DataSource = null;
            _grid.DataSource = accounts;

            if (_grid.Rows.Count > 0)
            {
                _grid.ClearSelection();
                _grid.Rows[0].Selected = true;
            }
            RefreshSelectionDetails();
        }

        private void RefreshSelectionDetails()
        {
            if (_grid.SelectedRows.Count == 0 && _grid.SelectedCells.Count == 0)
            {
                _lblUserDetails.Text = "User details: - | -";
                return;
            }

            var row = _grid.SelectedRows.Count > 0 ? _grid.SelectedRows[0] : _grid.Rows[_grid.SelectedCells[0].RowIndex];
            var username = row.Cells[1].Value?.ToString() ?? string.Empty;
            var role = row.Cells[4].Value?.ToString() ?? string.Empty;
            _lblUserDetails.Text = $"User details: {username} | {role}";
        }

        private void ToggleLockAccount()
        {
            if (_grid.SelectedRows.Count == 0 && _grid.SelectedCells.Count == 0) return;
            var row = _grid.SelectedRows.Count > 0 ? _grid.SelectedRows[0] : _grid.Rows[_grid.SelectedCells[0].RowIndex];
            var username = row.Cells[1].Value?.ToString() ?? string.Empty;
            var success = RolePermissionCatalog.ToggleLockAccount(username);
            if (success)
            {
                LoadGrid();
                MessageBox.Show("Đổi trạng thái tài khoản thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CreateAccountPrompt()
        {
            var dialog = new Form
            {
                Text = "Thêm tài khoản mới",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Width = 400,
                Height = 260,
                Font = new Font("Segoe UI", 9F)
            };

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                RowCount = 4,
                AutoSize = true
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var lblUsername = new Label { Text = "Tên đăng nhập", AutoSize = true, Anchor = AnchorStyles.Left };
            var lblFullName = new Label { Text = "Họ tên", AutoSize = true, Anchor = AnchorStyles.Left };
            var lblRole = new Label { Text = "Vai trò", AutoSize = true, Anchor = AnchorStyles.Left };
            var lblPassword = new Label { Text = "Mật khẩu", AutoSize = true, Anchor = AnchorStyles.Left };

            var txtUsername = new TextBox();
            var txtFullName = new TextBox();
            var comboRole = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var roleName in RolePermissionCatalog.GetAllRoles()) comboRole.Items.Add(roleName);
            if (comboRole.Items.Count > 0) comboRole.SelectedIndex = 0;
            var txtPassword = new TextBox { PasswordChar = '*' };

            var btnOk = new Button { Text = "Thêm", DialogResult = DialogResult.OK, Width = 85 };
            var btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Width = 85 };

            table.Controls.Add(lblUsername, 0, 0); table.Controls.Add(txtUsername, 1, 0);
            table.Controls.Add(lblFullName, 0, 1); table.Controls.Add(txtFullName, 1, 1);
            table.Controls.Add(lblRole, 0, 2); table.Controls.Add(comboRole, 1, 2);
            table.Controls.Add(lblPassword, 0, 3); table.Controls.Add(txtPassword, 1, 3);

            var flow = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40 };
            flow.Controls.Add(btnCancel);
            flow.Controls.Add(btnOk);
            dialog.Controls.Add(flow);
            dialog.Controls.Add(table);

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            var username = txtUsername.Text.Trim();
            var fullName = txtFullName.Text.Trim();
            var role = comboRole.Text.Trim();
            var password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin tài khoản.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!RolePermissionCatalog.AddAccount(username, fullName, role, password))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại hoặc thông tin không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadGrid();
            MessageBox.Show("Đã thêm tài khoản mới thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F2)
            {
                CreateAccountPrompt();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SaveCurrentPermissions()
        {
            if (_grid.Rows.Count == 0) return;

            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;
                var username = row.Cells[1].Value?.ToString();
                var role = row.Cells[4].Value?.ToString();
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role)) continue;

                var account = RolePermissionCatalog.Accounts.FirstOrDefault(a => string.Equals(a.Username, username, StringComparison.OrdinalIgnoreCase));
                if (account == null) continue;

                account.Role = role;
                if (row.Cells[5].Value is bool isActive) account.IsActive = isActive;
            }

            MessageBox.Show("Đã lưu thay đổi trạng thái tài khoản thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private sealed class AccountRowModel
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public bool IsActive { get; set; }
        }
    }
}