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
        private readonly Label _customerCount = new();
        private readonly Label _pointCount = new();
        private readonly Label _vipCount = new();
        private readonly Button _add = new();
        private readonly Button _update = new();
        private readonly Button _delete = new();
        private List<CustomerDto> _customers = new();

        public FormCustomerDashboard()
        {
            Text = "Khách hàng & tích điểm";
            BackColor = Color.FromArgb(244, 246, 249);
            Font = new Font("Segoe UI", 9F);
            Padding = new Padding(20);
            BuildLayout();
            Load += async (_, _) => await LoadAsync();
            _search.TextChanged += (_, _) => ApplyFilter();
        }

        private void BuildLayout()
        {
            var title = new Label { Dock = DockStyle.Top, Height = 36, Text = "KHÁCH HÀNG & TÍCH ĐIỂM", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Ink };
            var subtitle = new Label { Dock = DockStyle.Top, Height = 28, Text = "Quản lý hồ sơ thành viên và điểm thưởng từ các giao dịch.", ForeColor = Muted };
            var header = new Panel { Dock = DockStyle.Top, Height = 72 };
            header.Controls.Add(subtitle);
            header.Controls.Add(title);

            var kpis = new TableLayoutPanel { Dock = DockStyle.Top, Height = 84, ColumnCount = 3, Padding = new Padding(0, 2, 0, 10) };
            for (var i = 0; i < 3; i++) kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            AddKpi(kpis, 0, "TỔNG KHÁCH HÀNG", _customerCount, Color.FromArgb(37, 99, 235));
            AddKpi(kpis, 1, "TỔNG ĐIỂM TÍCH LŨY", _pointCount, Color.FromArgb(202, 138, 4));
            AddKpi(kpis, 2, "THÀNH VIÊN VIP", _vipCount, Color.FromArgb(147, 51, 234));

            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, WrapContents = false, Padding = new Padding(0, 4, 0, 5) };
            _search.Width = 350;
            _search.PlaceholderText = "Tìm theo tên hoặc số điện thoại...";
            var refresh = MakeButton("Tải lại", Color.FromArgb(229, 231, 235), Ink);
            refresh.Click += async (_, _) => await LoadAsync();
            var add = MakeButton("＋  THÊM KHÁCH HÀNG (F6)", Color.FromArgb(22, 163, 74), Color.White);
            add.Click += (_, _) => StartNew();
            toolbar.Controls.AddRange(new Control[] { _search, refresh, add });

            ConfigureGrid();
            var list = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
            list.Controls.Add(_grid);
            list.Controls.Add(new Label { Dock = DockStyle.Top, Height = 32, Text = "DANH SÁCH KHÁCH HÀNG THÀNH VIÊN", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Ink });
            var detail = BuildDetailPanel();
            var split = new SplitContainer { Dock = DockStyle.Fill, Size = new Size(1000, 500), Orientation = Orientation.Vertical, SplitterWidth = 10, Panel1MinSize = 430, Panel2MinSize = 300, BackColor = Color.FromArgb(244, 246, 249) };
            split.Panel1.Padding = new Padding(0, 0, 8, 0);
            split.Panel2.Padding = new Padding(8, 0, 0, 0);
            split.Panel1.Controls.Add(list);
            split.Panel2.Controls.Add(detail);
            split.SizeChanged += (_, _) => SetSplitDistance(split);
            split.HandleCreated += (_, _) => SetSplitDistance(split);

            Controls.Add(split);
            Controls.Add(toolbar);
            Controls.Add(kpis);
            Controls.Add(header);
        }

        private static void SetSplitDistance(SplitContainer split)
        {
            var available = split.Width - split.SplitterWidth;
            if (available > split.Panel1MinSize + split.Panel2MinSize)
                split.SplitterDistance = Math.Max(split.Panel1MinSize, Math.Min(available - split.Panel2MinSize, available * 68 / 100));
        }

        private static void AddKpi(TableLayoutPanel layout, int col, string caption, Label value, Color color)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0, 0, 10, 0), Padding = new Padding(14, 8, 14, 6) };
            card.Controls.Add(value);
            value.Dock = DockStyle.Fill;
            value.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            value.ForeColor = color;
            card.Controls.Add(new Label { Dock = DockStyle.Top, Height = 20, Text = caption, ForeColor = Muted, Font = new Font("Segoe UI", 8F, FontStyle.Bold) });
            layout.Controls.Add(card, col, 0);
        }

        private void ConfigureGrid()
        {
            _grid.Dock = DockStyle.Fill;
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
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 246, 250);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _grid.RowTemplate.Height = 34;
            AddColumn("CustomerId", "Mã KH", 10);
            AddColumn("CustomerName", "Họ tên", 23);
            AddColumn("PhoneNumber", "Số điện thoại", 18);
            AddColumn("MembershipRank", "Hạng", 13);
            AddColumn("RewardPoints", "Điểm", 12);
            AddColumn("Address", "Địa chỉ", 24);
            _grid.SelectionChanged += (_, _) => SelectCurrent();
            _grid.CellFormatting += (_, e) =>
            {
                var rank = e.RowIndex >= 0 ? _grid.Rows[e.RowIndex].Cells["MembershipRank"].Value?.ToString() : null;
                if (rank != null && (rank.Contains("Vàng", StringComparison.OrdinalIgnoreCase) || rank.Contains("Bạch", StringComparison.OrdinalIgnoreCase)))
                    _grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(250, 245, 255);
            };
            var menu = new ContextMenuStrip();
            menu.Items.Add("Sửa thông tin", null, (_, _) => SelectCurrent());
            menu.Items.Add("Xóa khách hàng", null, async (_, _) => await DeleteAsync());
            _grid.ContextMenuStrip = menu;
            _grid.CellMouseDown += (_, e) =>
            {
                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {
                    _grid.ClearSelection();
                    _grid.Rows[e.RowIndex].Selected = true;
                    _grid.CurrentCell = _grid.Rows[e.RowIndex].Cells[0];
                }
            };
        }

        private void AddColumn(string property, string header, float weight) => _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = property, DataPropertyName = property, HeaderText = header, FillWeight = weight, MinimumWidth = 55 });

        private Control BuildDetailPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16) };
            var title = new Label { Dock = DockStyle.Top, Height = 32, Text = "CHI TIẾT KHÁCH HÀNG", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Ink };
            var fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 12, Padding = new Padding(0, 8, 0, 0) };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (var i = 0; i < 6; i++) { fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 21)); fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); }
            ConfigureText(_id, true);
            ConfigureText(_name);
            ConfigureText(_phone);
            ConfigureText(_address);
            _points.Minimum = 0;
            _points.Maximum = 100000000;
            _points.ThousandsSeparator = true;
            _rank.DropDownStyle = ComboBoxStyle.DropDownList;
            _rank.Items.AddRange(new object[] { "Chuẩn", "Bạc", "Vàng", "Bạch Kim" });
            AddField(fields, 0, "Mã khách hàng", _id);
            AddField(fields, 2, "Họ và tên", _name);
            AddField(fields, 4, "Số điện thoại", _phone);
            AddField(fields, 6, "Địa chỉ", _address);
            AddField(fields, 8, "Điểm tích lũy", _points);
            AddField(fields, 10, "Hạng thành viên", _rank);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(0, 12, 0, 0), WrapContents = true };
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
            panel.Controls.Add(fields);
            panel.Controls.Add(actions);
            panel.Controls.Add(title);
            return panel;
        }

        private static void AddField(TableLayoutPanel layout, int row, string caption, Control input)
        {
            layout.Controls.Add(new Label { Text = caption, Dock = DockStyle.Fill, ForeColor = Muted, TextAlign = ContentAlignment.BottomLeft }, 0, row);
            input.Dock = DockStyle.Fill;
            layout.Controls.Add(input, 0, row + 1);
        }

        private static void ConfigureText(TextBox box, bool readOnly = false) { box.ReadOnly = readOnly; box.BorderStyle = BorderStyle.FixedSingle; box.BackColor = readOnly ? Color.FromArgb(243, 244, 246) : Color.White; }

        private static Button MakeButton(string text, Color back, Color fore)
        {
            var button = new Button { Text = text, Height = 34, AutoSize = true, Padding = new Padding(10, 0, 10, 0), BackColor = back, ForeColor = fore, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Margin = new Padding(4, 0, 4, 0) };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private static void StyleButton(Button button, Color back, Color fore)
        {
            button.Text = button.Text;
            button.Width = 92;
            button.Height = 34;
            button.BackColor = back;
            button.ForeColor = fore;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            button.Margin = new Padding(2, 0, 2, 0);
        }

        private async Task LoadAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _customers = await ApiClientService.GetAsync<List<CustomerDto>>("customers") ?? new List<CustomerDto>();
                _customerCount.Text = _customers.Count.ToString("N0");
                _pointCount.Text = _customers.Sum(x => x.RewardPoints).ToString("N0");
                _vipCount.Text = _customers.Count(x => x.MembershipRank.Equals("Vàng", StringComparison.OrdinalIgnoreCase) || x.MembershipRank.Equals("Bạch Kim", StringComparison.OrdinalIgnoreCase)).ToString("N0");
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
            _grid.DataSource = _customers.Where(x => keyword.Length == 0
                || x.CustomerName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                || x.PhoneNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void SelectCurrent()
        {
            if (_grid.CurrentRow?.DataBoundItem is not CustomerDto customer) return;
            _id.Text = customer.CustomerId.ToString();
            _name.Text = customer.CustomerName;
            _phone.Text = customer.PhoneNumber;
            _address.Text = customer.Address ?? string.Empty;
            _points.Value = Math.Min(_points.Maximum, Math.Max(_points.Minimum, customer.RewardPoints));
            _rank.SelectedItem = customer.MembershipRank;
            _add.Enabled = false;
            _update.Enabled = true;
            _delete.Enabled = true;
        }

        private void StartNew()
        {
            _grid.ClearSelection();
            _id.Clear(); _name.Clear(); _phone.Clear(); _address.Clear(); _points.Value = 0; _rank.SelectedIndex = 0;
            _add.Enabled = true; _update.Enabled = false; _delete.Enabled = false;
            _name.Focus();
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
                MembershipRank = _rank.SelectedItem.ToString() ?? "Chuẩn"
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
            catch (Exception ex) { MessageBox.Show("Không thể lưu khách hàng.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
            catch (Exception ex) { MessageBox.Show("Không thể xóa khách hàng.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
