using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormCustomerDashboard : Form
    {
        private static readonly Color Ink = Color.FromArgb(31, 41, 55);
        private static readonly Color Muted = Color.FromArgb(107, 114, 128);
        private readonly TextBox _search = new();
        private readonly DataGridView _grid = new();
        private readonly TextBox _id = new();
        private readonly TextBox _name = new();
        private readonly TextBox _phone = new();
        private readonly TextBox _address = new();
        private readonly NumericUpDown _points = new();
        private readonly ComboBox _rank = new();
        private readonly ComboBox _statusFilter = new();
        private readonly Label _customerCount = new();
        private readonly Label _pointCount = new();
        private readonly Label _vipCount = new();
        private readonly Button _add = new();
        private readonly Button _update = new();
        private readonly Button _delete = new();
        private List<CustomerDto> _customers = new();

        public FormCustomerDashboard()
        {
            if (!RolePermissionCatalog.HasPermission(SessionManager.CurrentRole, "CUSTOMER"))
            {
                MessageBox.Show("Bạn không có quyền truy cập màn hình Khách hàng.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
                return;
            }

            Text = "Khách hàng & tích điểm";
            BackColor = Color.FromArgb(244, 246, 249);
            Font = new Font("Segoe UI", 9F);
            Padding = new Padding(12, 8, 12, 10);
            MinimumSize = new Size(1180, 760);
            Size = new Size(1280, 820);
            WindowState = FormWindowState.Normal;

            BuildLayout();
            InitializeComboDefaults();
            Load += async (_, _) => await LoadAsync();
            _search.TextChanged += (_, _) => ApplyFilter();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.FromArgb(244, 246, 249),
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F)); // Title
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Toolbar (Search, Filter, Buttons)
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Main Content (Grid + Detail)
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F)); // KPI Row

            // 2. Title
            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "QUẢN LÝ KHÁCH HÀNG & TÍCH ĐIỂM",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Ink,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0)
            };

            // 3. Toolbar Panel
            var toolbarPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                BackColor = Color.FromArgb(244, 246, 249),
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            toolbarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            toolbarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            toolbarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            toolbarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            toolbarPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            toolbarPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));

            toolbarPanel.Controls.Add(new Label { Text = "Tìm kiếm", Font = new Font("Segoe UI", 8F, FontStyle.Bold), ForeColor = Ink, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 0);
            toolbarPanel.Controls.Add(new Label { Text = "Loại khách hàng", Font = new Font("Segoe UI", 8F, FontStyle.Bold), ForeColor = Ink, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 1, 0);

            _search.Dock = DockStyle.Fill;
            _search.Height = 30;
            _search.BorderStyle = BorderStyle.FixedSingle;
            _search.Margin = new Padding(0, 0, 8, 0);
            _search.PlaceholderText = "Tìm theo tên hoặc SĐT...";

            _statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _statusFilter.FlatStyle = FlatStyle.Flat;
            _statusFilter.Dock = DockStyle.Fill;
            _statusFilter.Height = 30;
            _statusFilter.Margin = new Padding(0, 0, 8, 0);
            _statusFilter.Items.AddRange(new object[] { "Tất cả", "Chuẩn", "Bạc", "Vàng", "Bạch Kim" });
            _statusFilter.SelectedIndex = 0;
            _statusFilter.SelectedIndexChanged += (_, _) => ApplyFilter();

            var importBtn = MakeButton("Import Excel", Color.FromArgb(31, 41, 55), Color.White);
            var addBtn = MakeButton("Thêm KH (F6)", Color.FromArgb(31, 41, 55), Color.White);
            addBtn.Click += (_, _) => StartNew();

            toolbarPanel.Controls.Add(_search, 0, 1);
            toolbarPanel.Controls.Add(_statusFilter, 1, 1);
            toolbarPanel.Controls.Add(importBtn, 2, 1);
            toolbarPanel.Controls.Add(addBtn, 3, 1);

            // 4. Content Area (Grid on Left, Details on Right)
            ConfigureGrid();
            var listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 8, 0),
                Padding = new Padding(4)
            };

            var listLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            listLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            listLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var listHeader = new Label
            {
                Dock = DockStyle.Fill,
                Text = "DANH SÁCH KHÁCH HÀNG THÀNH VIÊN",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Ink,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 0, 0)
            };

            _grid.Dock = DockStyle.Fill;

            listLayout.Controls.Add(listHeader, 0, 0);
            listLayout.Controls.Add(_grid, 0, 1);
            listPanel.Controls.Add(listLayout);

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(244, 246, 249),
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            content.Controls.Add(listPanel, 0, 0);
            content.Controls.Add(BuildDetailPanel(), 1, 0);

            // 5. KPIs
            var kpis = BuildKpiRow();

            root.Controls.Add(title, 0, 0);
            root.Controls.Add(toolbarPanel, 0, 1);
            root.Controls.Add(content, 0, 2);
            root.Controls.Add(kpis, 0, 3);

            Controls.Add(root);
        }

        private Control BuildKpiRow()
        {
            var kpis = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, BackColor = Color.FromArgb(244, 246, 249), Padding = new Padding(0, 4, 0, 0) };
            kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            AddKpi(kpis, 0, "TỔNG KHÁCH HÀNG", _customerCount, Color.FromArgb(37, 99, 235));
            AddKpi(kpis, 1, "THÀNH VIÊN VIP", _vipCount, Color.FromArgb(16, 185, 129));
            AddKpi(kpis, 2, "TỔNG ĐIỂM TÍCH LŨY", _pointCount, Color.FromArgb(202, 138, 4));
            return kpis;
        }

        private static void AddKpi(TableLayoutPanel layout, int col, string caption, Label value, Color color)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0, 0, 6, 0), Padding = new Padding(8, 6, 8, 6) };
            var accent = new Panel { Dock = DockStyle.Left, Width = 4, BackColor = color, Margin = new Padding(0, 2, 6, 2) };
            var title = new Label { Dock = DockStyle.Top, Height = 14, Text = caption, ForeColor = Muted, Font = new Font("Segoe UI", 7.5F, FontStyle.Bold) };

            value.Dock = DockStyle.Fill;
            value.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            value.ForeColor = color;
            value.TextAlign = ContentAlignment.MiddleLeft;

            var content = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(0), Margin = new Padding(0) };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 6F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            content.Controls.Add(accent, 0, 0);
            content.Controls.Add(title, 1, 0);
            content.Controls.Add(value, 1, 1);

            card.Controls.Add(content);
            layout.Controls.Add(card, col, 0);
        }

        private void ConfigureGrid()
        {
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.None;
            _grid.AllowUserToAddRows = false;
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Ink;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 30;
            _grid.RowTemplate.Height = 28;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            _grid.DefaultCellStyle.SelectionForeColor = Ink;
            _grid.GridColor = Color.FromArgb(226, 232, 240);

            _grid.Columns.Clear();
            AddColumn("CustomerId", "Mã", 8);
            AddColumn("CustomerName", "Họ Tên", 26);
            AddColumn("PhoneNumber", "SĐT", 18);
            AddColumn("Address", "Địa Chỉ", 24);
            AddColumn("RewardPoints", "Điểm", 10);
            AddColumn("MembershipRank", "Hạng", 14);

            _grid.SelectionChanged += (_, _) => SelectCurrent();
        }

        private void AddColumn(string property, string header, float weight)
            => _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = property,
                DataPropertyName = property,
                HeaderText = header,
                FillWeight = weight,
                MinimumWidth = 45
            });

        private Control BuildDetailPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12),
                AutoScroll = true
            };

            var detailLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F)); // Title
            detailLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Fields container
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Action Buttons

            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "CHI TIẾT KHÁCH HÀNG",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Ink,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 12,
                Padding = new Padding(0, 4, 0, 4)
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (var i = 0; i < 6; i++)
            {
                fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F)); // Label row
                fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F)); // Input row
            }

            ConfigureText(_id, true);
            ConfigureText(_name);
            ConfigureText(_phone);
            ConfigureText(_address);
            _points.Minimum = 0;
            _points.Maximum = 100000000;
            _points.ThousandsSeparator = true;
            _rank.DropDownStyle = ComboBoxStyle.DropDownList;
            _rank.Items.AddRange(new object[] { "Chuẩn", "Bạc", "Vàng", "Bạch Kim" });
            InitializeComboDefaults();

            AddField(fields, 0, "Mã khách hàng", _id);
            AddField(fields, 2, "Họ và tên", _name);
            AddField(fields, 4, "Số điện thoại", _phone);
            AddField(fields, 6, "Địa chỉ", _address);
            AddField(fields, 8, "Điểm tích lũy", _points);
            AddField(fields, 10, "Hạng thành viên", _rank);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 0), FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            _add.Text = "THÊM"; StyleButton(_add, Color.FromArgb(229, 231, 235), Ink);
            _update.Text = "CẬP NHẬT"; StyleButton(_update, Color.FromArgb(37, 99, 235), Color.White);
            _delete.Text = "XÓA"; StyleButton(_delete, Color.FromArgb(220, 38, 38), Color.White);
            _update.Enabled = false;
            _delete.Enabled = false;

            var clear = MakeButton("Làm trống", Color.FromArgb(229, 231, 235), Ink);
            _add.Click += async (_, _) => await SaveAsync(false);
            _update.Click += async (_, _) => await SaveAsync(true);
            _delete.Click += async (_, _) => await DeleteAsync();
            clear.Click += (_, _) => StartNew();

            actions.Controls.AddRange(new Control[] { _add, _update, _delete, clear });

            detailLayout.Controls.Add(title, 0, 0);
            detailLayout.Controls.Add(fields, 0, 1);
            detailLayout.Controls.Add(actions, 0, 2);
            panel.Controls.Add(detailLayout);
            return panel;
        }

        private static void AddField(TableLayoutPanel layout, int row, string caption, Control input)
        {
            layout.Controls.Add(new Label { Text = caption, Dock = DockStyle.Fill, ForeColor = Muted, Font = new Font("Segoe UI", 8F), TextAlign = ContentAlignment.BottomLeft }, 0, row);
            input.Dock = DockStyle.Fill;
            layout.Controls.Add(input, 0, row + 1);
        }

        private static void ConfigureText(TextBox box, bool readOnly = false)
        {
            box.ReadOnly = readOnly;
            box.BorderStyle = BorderStyle.FixedSingle;
            box.BackColor = readOnly ? Color.FromArgb(243, 244, 246) : Color.White;
        }

        private static void StyleButton(Button button, Color back, Color fore)
        {
            button.Width = 78;
            button.Height = 30;
            button.BackColor = back;
            button.ForeColor = fore;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            button.Margin = new Padding(0, 0, 4, 0);
        }

        private static Button MakeButton(string text, Color back, Color fore)
        {
            var button = new Button
            {
                Text = text,
                Height = 30,
                AutoSize = true,
                Padding = new Padding(8, 0, 8, 0),
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Margin = new Padding(0, 0, 4, 0)
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private async Task LoadAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _customers = await ApiClientService.GetAsync<List<CustomerDto>>("customers") ?? new List<CustomerDto>();
                foreach (var customer in _customers)
                {
                    customer.CustomerName = string.IsNullOrWhiteSpace(customer.CustomerName) ? string.Empty : customer.CustomerName.Trim();
                    customer.PhoneNumber = string.IsNullOrWhiteSpace(customer.PhoneNumber) ? string.Empty : customer.PhoneNumber.Trim();
                    customer.MembershipRank = NormalizeMembershipRank(customer.MembershipRank);
                }

                _customerCount.Text = _customers.Count.ToString("N0");
                _pointCount.Text = _customers.Sum(x => x.RewardPoints).ToString("N0");
                _vipCount.Text = _customers.Count(x => IsVipRank(x.MembershipRank)).ToString("N0");
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách khách hàng.\n" + ex.Message, "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { Cursor = Cursors.Default; }
        }

        private void ApplyFilter()
        {
            var keyword = _search.Text.Trim();
            var selectedRank = _statusFilter.SelectedItem?.ToString();

            var filtered = _customers.Where(x =>
                (selectedRank == null || selectedRank == "Tất cả" || NormalizeMembershipRank(x.MembershipRank).Equals(NormalizeMembershipRank(selectedRank), StringComparison.OrdinalIgnoreCase))
                && (keyword.Length == 0
                    || x.CustomerName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                    || x.PhoneNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();

            _grid.DataSource = filtered;
            _grid.ClearSelection();
            StartNew();
        }

        private void SelectCurrent()
        {
            if (_grid.CurrentRow?.DataBoundItem is not CustomerDto customer)
            {
                StartNew();
                return;
            }

            _id.Text = customer.CustomerId.ToString();
            _name.Text = customer.CustomerName;
            _phone.Text = customer.PhoneNumber;
            _address.Text = customer.Address ?? string.Empty;
            _points.Value = Math.Min(_points.Maximum, Math.Max(_points.Minimum, customer.RewardPoints));
            _rank.SelectedItem = NormalizeMembershipRank(customer.MembershipRank);
            _add.Enabled = false;
            _update.Enabled = true;
            _delete.Enabled = true;
        }

        private void StartNew()
        {
            _grid.ClearSelection();
            _id.Clear(); _name.Clear(); _phone.Clear(); _address.Clear(); _points.Value = 0;
            InitializeComboDefaults();
            _add.Enabled = true; _update.Enabled = false; _delete.Enabled = false;
        }

        private void InitializeComboDefaults()
        {
            if (_rank.Items.Count > 0 && (_rank.SelectedItem == null || string.IsNullOrWhiteSpace(_rank.SelectedItem.ToString())))
            {
                _rank.SelectedItem = "Chuẩn";
            }
        }

        private static string NormalizeMembershipRank(string? rank)
        {
            if (string.IsNullOrWhiteSpace(rank)) return "Chuẩn";
            var value = CollapseText(rank);
            return value switch
            {
                "standard" or "chuan" => "Chuẩn",
                "silver" or "bac" => "Bạc",
                "gold" or "vang" => "Vàng",
                "platinum" or "bachkim" or "bach-kim" => "Bạch Kim",
                _ => rank.Trim()
            };
        }

        private static string CollapseText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var normalized = value.Trim().Normalize(System.Text.NormalizationForm.FormD);
            var builder = new System.Text.StringBuilder();
            foreach (var ch in normalized)
            {
                var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(char.ToLowerInvariant(ch));
                }
            }
            return builder.ToString().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty);
        }

        private static bool IsVipRank(string? rank)
        {
            var value = NormalizeMembershipRank(rank);
            return value.Equals("Vàng", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Bạch Kim", StringComparison.OrdinalIgnoreCase);
        }

        private async Task SaveAsync(bool update)
        {
            if (string.IsNullOrWhiteSpace(_name.Text) || string.IsNullOrWhiteSpace(_phone.Text) || _rank.SelectedItem == null)
            {
                MessageBox.Show("Họ tên, số điện thoại và hạng thành viên là bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (update && !int.TryParse(_id.Text, out _))
            {
                MessageBox.Show("Hãy chọn khách hàng cần cập nhật.", "Chưa chọn khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var customer = new CustomerDto
            {
                CustomerId = update ? int.Parse(_id.Text) : 0,
                CustomerName = _name.Text.Trim(),
                PhoneNumber = _phone.Text.Trim(),
                Address = _address.Text.Trim(),
                RewardPoints = (int)_points.Value,
                MembershipRank = _rank.SelectedItem?.ToString() ?? "Chuẩn"
            };

            try
            {
                using var response = update
                    ? await ApiClientService.PutAsJsonAsync($"customers/{customer.CustomerId}", customer)
                    : await ApiClientService.PostAsJsonAsync("customers", customer);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Lưu khách hàng thất bại ({(int)response.StatusCode}).\n{error}", "Lỗi lưu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await LoadAsync();
                StartNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu khách hàng.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteAsync()
        {
            if (!int.TryParse(_id.Text, out var id))
            {
                MessageBox.Show("Hãy chọn khách hàng cần xóa.", "Chưa chọn khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xóa hồ sơ khách hàng mã {id}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using var response = await ApiClientService.DeleteAsync($"customers/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Xóa khách hàng thất bại ({(int)response.StatusCode}).\n{error}", "Lỗi xóa dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await LoadAsync();
                StartNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa khách hàng.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F6) { StartNew(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private sealed class CustomerDto
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string? Address { get; set; }
            public int RewardPoints { get; set; }
            public string MembershipRank { get; set; } = "Chuẩn";
        }
    }
}