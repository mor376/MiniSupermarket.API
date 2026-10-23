using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSupermarket.WinForms.Models;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormProductCatalog : Form
    {
        private static readonly Color Ink = Color.FromArgb(31, 41, 55);
        private static readonly Color Muted = Color.FromArgb(107, 114, 128);
        private static readonly Color Green = Color.FromArgb(22, 163, 74);
        private readonly TextBox _search = new();
        private readonly ComboBox _categoryFilter = new();
        private readonly ComboBox _statusFilter = new();
        private readonly DataGridView _grid = new();
        private readonly TextBox _id = new();
        private readonly TextBox _barcode = new();
        private readonly TextBox _name = new();
        private readonly ComboBox _category = new();
        private readonly TextBox _unit = new();
        private readonly NumericUpDown _cost = new();
        private readonly NumericUpDown _price = new();
        private readonly NumericUpDown _stock = new();
        private readonly Button _add = new();
        private readonly Button _update = new();
        private readonly Button _delete = new();
        private List<ProductCatalogDto> _products = new();

        public FormProductCatalog()
        {
            if (!RolePermissionCatalog.HasPermission(SessionManager.CurrentRole, "PRODUCT"))
            {
                MessageBox.Show("Bạn không có quyền truy cập màn hình Sản phẩm.", "Từ chối quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
                return;
            }

            Text = "Danh mục sản phẩm & giá";
            BackColor = Color.FromArgb(244, 246, 249);
            Font = new Font("Segoe UI", 9F);
            Padding = new Padding(16, 8, 16, 12);
            MinimumSize = new Size(1180, 760);
            Size = new Size(1280, 820);
            WindowState = FormWindowState.Normal;

            BuildLayout();
            Load += async (_, _) => await LoadAsync();
            _search.TextChanged += (_, _) => ApplyFilter();
            _categoryFilter.SelectedIndexChanged += (_, _) => ApplyFilter();
            _statusFilter.SelectedIndexChanged += (_, _) => ApplyFilter();
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
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Page Title Bar
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));  // Filter Bar
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Table & Actions Panel
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180)); // Details Panel

            root.Controls.Add(BuildPageTitle(), 0, 0);
            root.Controls.Add(BuildFilterBar(), 0, 1);
            root.Controls.Add(BuildTableAndActions(), 0, 2);
            root.Controls.Add(BuildDetailsPanel(), 0, 3);

            Controls.Add(root);
        }

        private Control BuildPageTitle()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(244, 246, 249) };
            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "DANH MỤC SẢN PHẨM & GIÁ",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Ink,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildFilterBar()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(244, 246, 249),
                Padding = new Padding(0, 0, 0, 4),
                ColumnCount = 3,
                RowCount = 2,
                Margin = new Padding(0)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27.5F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27.5F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            // Row 0: Labels
            panel.Controls.Add(new Label { Text = "Search Bar", Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), ForeColor = Ink, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 0);
            panel.Controls.Add(new Label { Text = "Ngành hàng:", Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), ForeColor = Ink, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 1, 0);
            panel.Controls.Add(new Label { Text = "Trạng thái:", Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), ForeColor = Ink, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 2, 0);

            // Row 1: Inputs & Buttons container
            var row1 = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0)
            };
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));

            _search.Dock = DockStyle.Fill;
            _search.Height = 28;
            _search.BorderStyle = BorderStyle.FixedSingle;
            _search.PlaceholderText = "Quét mã, nhập tên SP hoặc mã vạch...";

            var searchButton = new Button
            {
                Text = "Search",
                Height = 28,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4, 0, 0, 0),
                Dock = DockStyle.Fill
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.Click += (_, _) => ApplyFilter();

            var clearButton = new Button
            {
                Text = "Clear",
                Height = 28,
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = Ink,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4, 0, 0, 0),
                Dock = DockStyle.Fill
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.Click += (_, _) =>
            {
                _search.Clear();
                _categoryFilter.SelectedIndex = 0;
                _statusFilter.SelectedIndex = 0;
                ApplyFilter();
            };

            row1.Controls.Add(_search, 0, 0);
            row1.Controls.Add(searchButton, 1, 0);
            row1.Controls.Add(clearButton, 2, 0);

            _categoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _categoryFilter.FlatStyle = FlatStyle.Flat;
            _categoryFilter.Height = 28;
            _categoryFilter.Dock = DockStyle.Fill;
            _categoryFilter.Margin = new Padding(0, 0, 12, 0);
            _categoryFilter.Items.Add(new CategoryOption(0, "All"));
            _categoryFilter.SelectedIndex = 0;

            _statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _statusFilter.FlatStyle = FlatStyle.Flat;
            _statusFilter.Height = 28;
            _statusFilter.Dock = DockStyle.Fill;
            _statusFilter.Margin = new Padding(0, 0, 12, 0);
            _statusFilter.Items.AddRange(new object[] { "All", "Đang bán", "Sắp hết", "Cận hạn", "Ngừng bán" });
            _statusFilter.SelectedIndex = 0;

            panel.Controls.Add(row1, 0, 1);
            panel.Controls.Add(_categoryFilter, 1, 1);
            panel.Controls.Add(_statusFilter, 2, 1);

            return panel;
        }

        private Control BuildTableAndActions()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(244, 246, 249),
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));

            var gridCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(0),
                Margin = new Padding(0, 0, 8, 0)
            };

            var topHeaderPanel = new Panel { Dock = DockStyle.Top, Height = 28 };
            var gridHeader = new Label
            {
                Dock = DockStyle.Left,
                Text = "BẢNG DANH MỤC SẢN PHẨM",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Ink,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            var toolsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false
            };
            toolsPanel.Controls.Add(new Button { Text = "🗑", Width = 26, Height = 22, FlatStyle = FlatStyle.Flat });
            toolsPanel.Controls.Add(new Button { Text = "☰", Width = 26, Height = 22, FlatStyle = FlatStyle.Flat });
            toolsPanel.Controls.Add(new Button { Text = "🪟", Width = 26, Height = 22, FlatStyle = FlatStyle.Flat });

            topHeaderPanel.Controls.Add(gridHeader);
            topHeaderPanel.Controls.Add(toolsPanel);

            ConfigureGrid();
            _grid.Dock = DockStyle.Fill;

            var gridWrap = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            gridWrap.Controls.Add(_grid);

            gridCard.Controls.Add(gridWrap);
            gridCard.Controls.Add(topHeaderPanel);

            var actionCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(0)
            };
            var actionTitle = new Label { Dock = DockStyle.Top, Height = 24, Text = "THAO TÁC NHANH", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Ink };
            var actionList = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(0, 4, 0, 0), AutoScroll = true };

            var addAction = CreateActionButton("Thêm SP mới (F3)", Color.FromArgb(226, 232, 240), Ink);
            addAction.Click += (_, _) => StartNewProduct();
            var importAction = CreateActionButton("Nhập Excel", Color.FromArgb(226, 232, 240), Ink);
            importAction.Click += (_, _) => MessageBox.Show("Tính năng nhập Excel sẽ được triển khai sau.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var exportAction = CreateActionButton("Xuất Excel", Color.FromArgb(226, 232, 240), Ink);
            exportAction.Click += async (_, _) => await ExportToCsvAsync();
            var wholesaleAction = CreateActionButton("Cập nhật Giá sỉ", Color.FromArgb(226, 232, 240), Ink);
            wholesaleAction.Click += async (_, _) => await UpdateWholesalePriceAsync();

            actionList.Controls.Add(addAction);
            actionList.Controls.Add(importAction);
            actionList.Controls.Add(exportAction);
            actionList.Controls.Add(wholesaleAction);

            var lblBottomActionTitle = new Label { Dock = DockStyle.Top, Height = 24, Text = "Xeng tán SP", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Ink, Margin = new Padding(0, 10, 0, 0) };

            actionCard.Controls.Add(actionList);
            actionCard.Controls.Add(lblBottomActionTitle);
            actionCard.Controls.Add(actionTitle);

            table.Controls.Add(gridCard, 0, 0);
            table.Controls.Add(actionCard, 1, 0);
            return table;
        }

        private static Button CreateActionButton(string text, Color backColor, Color foreColor)
        {
            var button = new Button
            {
                Text = text,
                Width = 136,
                Height = 28,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F),
                Margin = new Padding(0, 0, 0, 4),
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void ConfigureGrid()
        {
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.FixedSingle;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.MultiSelect = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoGenerateColumns = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Ink;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 26;
            _grid.RowTemplate.Height = 26;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            _grid.DefaultCellStyle.SelectionForeColor = Ink;
            _grid.GridColor = Color.FromArgb(226, 232, 240);
            _grid.ScrollBars = ScrollBars.Both;

            _grid.Columns.Clear();
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "STT", DataPropertyName = nameof(ProductCatalogDto.ProductId), Width = 45, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã SP", DataPropertyName = nameof(ProductCatalogDto.Barcode), Width = 95, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên sản phẩm", DataPropertyName = nameof(ProductCatalogDto.ProductName), Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ĐVT", DataPropertyName = nameof(ProductCatalogDto.Unit), Width = 55 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá nhập", DataPropertyName = nameof(ProductCatalogDto.CostPrice), Width = 90, DefaultCellStyle = { Format = "N0" } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá bán lẻ", DataPropertyName = nameof(ProductCatalogDto.SellingPrice), Width = 95, DefaultCellStyle = { Format = "N0" } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngành hàng", DataPropertyName = nameof(ProductCatalogDto.CategoryName), Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tồn kho", DataPropertyName = nameof(ProductCatalogDto.StockQuantity), Width = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = nameof(ProductCatalogDto.Status), Width = 100 });

            _grid.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0) return;

                var row = _grid.Rows[e.RowIndex].DataBoundItem as ProductCatalogDto;
                if (row is null) return;

                var status = row.Status ?? string.Empty;

                if (string.Equals(status, "Cận hạn", StringComparison.OrdinalIgnoreCase))
                {
                    var cellStyle = e.CellStyle;
                    if (cellStyle is not null)
                    {
                        cellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        cellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                    }
                }
                else if (string.Equals(status, "Sắp hết", StringComparison.OrdinalIgnoreCase))
                {
                    var cellStyle = e.CellStyle;
                    if (cellStyle is not null)
                    {
                        cellStyle.BackColor = Color.FromArgb(255, 247, 237);
                        cellStyle.ForeColor = Color.FromArgb(146, 64, 14);
                    }
                }
            };

            _grid.SelectionChanged += (_, _) => SelectCurrentRow();

            var menu = new ContextMenuStrip();
            menu.Items.Add("Sửa sản phẩm", null, (_, _) => SelectCurrentRow());
            menu.Items.Add("Cập nhật giá", null, async (_, _) => await UpdateWholesalePriceAsync());
            menu.Items.Add("Xem chi tiết", null, (_, _) => SelectCurrentRow());
            menu.Items.Add("Xóa sản phẩm", null, async (_, _) => await DeleteAsync());
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

        private Control BuildDetailsPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10) };
            var title = new Label { Dock = DockStyle.Top, Height = 22, Text = "Xem chi tiết SP", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Ink };

            var formBody = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(0, 8, 0, 0) };
            formBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            formBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

            var fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 4, Margin = new Padding(0) };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));

            ConfigureInput(_id, readOnly: true);
            ConfigureInput(_barcode);
            ConfigureInput(_name);
            ConfigureInput(_unit);
            _cost.Minimum = 0;
            _cost.Maximum = 1000000000;
            _cost.DecimalPlaces = 0;
            _price.Minimum = 0;
            _price.Maximum = 1000000000;
            _price.DecimalPlaces = 0;
            _stock.Minimum = 0;
            _stock.Maximum = 1000000;
            _category.DropDownStyle = ComboBoxStyle.DropDownList;
            _category.FlatStyle = FlatStyle.Flat;

            AddDetailField(fields, 0, 0, "Mã SP", _id, true);
            AddDetailField(fields, 0, 2, "Mã vạch", _barcode);
            AddDetailField(fields, 0, 4, "Tên SP", _name);
            AddDetailField(fields, 2, 0, "ĐVT", _unit);
            AddDetailField(fields, 2, 2, "Giá nhập", _cost);
            AddDetailField(fields, 2, 4, "Giá bán lẻ", _price);
            AddDetailField(fields, 3, 0, "Tồn kho", _stock);
            AddDetailField(fields, 3, 2, "Ngành hàng", _category);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0, 4, 0, 0) };
            _add.Text = "THÊM";
            _update.Text = "CẬP NHẬT";
            _delete.Text = "XÓA";
            StyleProductAction(_add, Color.FromArgb(229, 231, 235), Ink);
            StyleProductAction(_update, Color.FromArgb(37, 99, 235), Color.White);
            StyleProductAction(_delete, Color.FromArgb(220, 38, 38), Color.White);
            _update.Enabled = false;
            _delete.Enabled = false;
            var clear = new Button { Text = "LÀM TRỐNG", Width = 110, Height = 34, BackColor = Color.FromArgb(229, 231, 235), ForeColor = Ink, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Margin = new Padding(4, 0, 0, 0) };
            clear.FlatAppearance.BorderSize = 0;
            clear.Click += (_, _) => StartNewProduct();
            _add.Click += async (_, _) => await SaveAsync(false);
            _update.Click += async (_, _) => await SaveAsync(true);
            _delete.Click += async (_, _) => await DeleteAsync();
            actions.Controls.AddRange(new Control[] { _add, _update, _delete, clear });

            formBody.Controls.Add(fields, 0, 0);
            formBody.Controls.Add(actions, 0, 1);
            panel.Controls.Add(formBody);
            panel.Controls.Add(title);
            return panel;
        }

        private static void AddDetailField(TableLayoutPanel layout, int rowIndex, int colIndex, string labelText, Control control, bool readOnly = false)
        {
            var label = new Label { Text = labelText, Dock = DockStyle.Fill, ForeColor = Muted, TextAlign = ContentAlignment.BottomLeft, Font = new Font("Segoe UI", 8F) };
            control.Dock = DockStyle.Fill;
            layout.Controls.Add(label, colIndex, rowIndex);
            layout.Controls.Add(control, colIndex + 1, rowIndex);
        }

        private static void ConfigureInput(Control control, bool readOnly = false)
        {
            control.Dock = DockStyle.Fill;
            control.BackColor = readOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            if (control is TextBox text)
            {
                text.BorderStyle = BorderStyle.FixedSingle;
                text.ReadOnly = readOnly;
            }
            else if (control is NumericUpDown number)
            {
                number.BorderStyle = BorderStyle.FixedSingle;
                number.TextAlign = HorizontalAlignment.Right;
            }
            else if (control is ComboBox combo)
            {
                combo.FlatStyle = FlatStyle.Flat;
                combo.DropDownStyle = ComboBoxStyle.DropDownList;
                combo.Margin = new Padding(0, 0, 4, 0);
            }
        }

        private static void StyleProductAction(Button button, Color backColor, Color foreColor)
        {
            button.Width = 104;
            button.Height = 34;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            button.Margin = new Padding(0, 0, 6, 0);
        }

        private async Task SaveAsync(bool update)
        {
            if (string.IsNullOrWhiteSpace(_barcode.Text) || string.IsNullOrWhiteSpace(_name.Text))
            {
                MessageBox.Show("Vui lòng nhập mã vạch và tên sản phẩm.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var categoryId = _category.SelectedValue is int selectedCategoryId
                ? selectedCategoryId
                : (_category.SelectedValue is string selectedCategoryText && int.TryParse(selectedCategoryText, out var parsedId) ? parsedId : 0);

            var payload = new
            {
                ProductId = update && int.TryParse(_id.Text, out var productId) ? productId : 0,
                Barcode = _barcode.Text.Trim(),
                ProductName = _name.Text.Trim(),
                CostPrice = _cost.Value,
                SellingPrice = _price.Value,
                StockQuantity = (int)_stock.Value,
                Unit = string.IsNullOrWhiteSpace(_unit.Text) ? "Cái" : _unit.Text.Trim(),
                CategoryId = categoryId
            };

            try
            {
                using var response = update
                    ? await ApiClientService.PutAsJsonAsync($"products/{payload.ProductId}", payload)
                    : await ApiClientService.PostAsJsonAsync("products", payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Lưu sản phẩm thất bại.\n{error}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await LoadAsync();
                StartNewProduct();
                MessageBox.Show(update ? "Cập nhật sản phẩm thành công." : "Thêm sản phẩm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu sản phẩm.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ExportToCsvAsync()
        {
            var list = (_grid.DataSource as List<ProductCatalogDto>) ?? (_grid.DataSource as IEnumerable<ProductCatalogDto>)?.ToList() ?? new List<ProductCatalogDto>();
            if (list.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "danh-sach-san-pham.csv" };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            var lines = new List<string>
            {
                "Mã SP,Mã vạch,Tên sản phẩm,ĐVT,Giá nhập,Giá bán lẻ,Ngành hàng,Tồn kho,Trạng thái"
            };

            foreach (var item in list)
            {
                lines.Add($"{item.ProductId},\"{item.Barcode.Replace("\"", "\"\"")}\",\"{item.ProductName.Replace("\"", "\"\"")}\",{item.Unit},\"{item.CostPrice}\",\"{item.SellingPrice}\",\"{item.CategoryName.Replace("\"", "\"\"")}\",{item.StockQuantity},\"{item.Status.Replace("\"", "\"\"")}\"");
            }

            File.WriteAllLines(dialog.FileName, lines);
            MessageBox.Show("Xuất file CSV thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task UpdateWholesalePriceAsync()
        {
            if (_grid.CurrentRow?.DataBoundItem is not ProductCatalogDto product)
            {
                MessageBox.Show("Hãy chọn sản phẩm cần cập nhật giá sỉ.", "Chưa chọn sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = new Form
            {
                Text = "Cập nhật giá sỉ",
                StartPosition = FormStartPosition.CenterParent,
                Width = 300,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label { Text = $"Nhập giá sỉ mới cho: {product.ProductName}", Dock = DockStyle.Top, Padding = new Padding(10, 12, 10, 0), AutoSize = true };
            var input = new NumericUpDown { Width = 220, Maximum = 1000000000, Minimum = 0, DecimalPlaces = 0, Value = product.CostPrice, Dock = DockStyle.Top, Margin = new Padding(10, 8, 10, 0) };
            var ok = new Button { Text = "Lưu", DialogResult = DialogResult.OK, Width = 80, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Margin = new Padding(0, 0, 8, 0) };
            var cancel = new Button { Text = "Huỷ", DialogResult = DialogResult.Cancel, Width = 80, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Margin = new Padding(0, 0, 8, 0) };
            var panel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Padding = new Padding(0, 8, 8, 0) };
            panel.Controls.Add(ok);
            panel.Controls.Add(cancel);
            form.Controls.Add(panel);
            form.Controls.Add(input);
            form.Controls.Add(label);
            form.AcceptButton = ok;
            form.CancelButton = cancel;

            if (form.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var payload = new
                {
                    ProductId = product.ProductId,
                    Barcode = product.Barcode,
                    ProductName = product.ProductName,
                    CostPrice = input.Value,
                    SellingPrice = product.SellingPrice,
                    StockQuantity = product.StockQuantity,
                    Unit = product.Unit,
                    CategoryId = product.CategoryId
                };

                using var response = await ApiClientService.PutAsJsonAsync($"products/{product.ProductId}", payload);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Cập nhật giá sỉ thất bại.\n{error}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await LoadAsync();
                SelectCurrentRow();
                MessageBox.Show("Cập nhật giá sỉ thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật giá sỉ.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var productsTask = ApiClientService.GetAsync<List<ProductCatalogDto>>("products");
                var categoriesTask = ApiClientService.GetAsync<List<CategoryOption>>("categories");
                await Task.WhenAll(productsTask, categoriesTask);
                _products = productsTask.Result ?? new List<ProductCatalogDto>();
                var categories = categoriesTask.Result ?? new List<CategoryOption>();
                _category.DataSource = null;
                _category.DataSource = categories;
                _category.DisplayMember = nameof(CategoryOption.CategoryName);
                _category.ValueMember = nameof(CategoryOption.CategoryId);
                _categoryFilter.Items.Clear();
                _categoryFilter.Items.Add(new CategoryOption(0, "All"));
                foreach (var item in categories) _categoryFilter.Items.Add(item);
                _categoryFilter.SelectedIndex = 0;
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh mục sản phẩm.\n" + ex.Message, "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private static string GetStatusText(ProductCatalogDto product)
        {
            if (product.StockQuantity <= 0)
            {
                return "Ngừng bán";
            }

            if (product.StockQuantity <= 5)
            {
                return "Cận hạn";
            }

            if (product.StockQuantity <= 15)
            {
                return "Sắp hết";
            }

            return "Đang bán";
        }

        private void ApplyFilter()
        {
            var keyword = _search.Text.Trim();
            var categoryId = (_categoryFilter.SelectedItem as CategoryOption)?.CategoryId ?? 0;
            var selectedStatusText = _statusFilter.SelectedItem as string ?? "All";

            var filtered = _products.Where(p =>
                (categoryId == 0 || p.CategoryId == categoryId)
                && (keyword.Length == 0 || p.ProductName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                    || p.Barcode.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                && (selectedStatusText == "All" || string.Equals(GetStatusText(p), selectedStatusText, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            _grid.DataSource = null;
            _grid.DataSource = filtered;
        }

        private void SelectCurrentRow()
        {
            if (_grid.CurrentRow?.DataBoundItem is not ProductCatalogDto product) return;
            _id.Text = product.ProductId.ToString();
            _barcode.Text = product.Barcode;
            _name.Text = product.ProductName;
            _cost.Value = Clamp(product.CostPrice, _cost.Maximum);
            _price.Value = Clamp(product.SellingPrice, _price.Maximum);
            _stock.Value = Math.Min(_stock.Maximum, Math.Max(_stock.Minimum, product.StockQuantity));
            _unit.Text = product.Unit;
            if (_category.Items.Count > 0) _category.SelectedValue = product.CategoryId;
            _add.Enabled = false;
            _update.Enabled = true;
            _delete.Enabled = true;
        }

        private static decimal Clamp(decimal value, decimal maximum) => Math.Min(maximum, Math.Max(0, value));

        private void StartNewProduct()
        {
            _grid.ClearSelection();
            _id.Clear();
            _barcode.Clear();
            _name.Clear();
            _unit.Text = "Cái";
            _cost.Value = 0;
            _price.Value = 0;
            _stock.Value = 0;
            if (_category.Items.Count > 0) _category.SelectedIndex = 0;
            _add.Enabled = true;
            _update.Enabled = false;
            _delete.Enabled = false;
            _barcode.Focus();
        }

        private async Task DeleteAsync()
        {
            if (!int.TryParse(_id.Text, out var id))
            {
                MessageBox.Show("Hãy chọn sản phẩm cần xóa.", "Chưa chọn sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"Xóa sản phẩm mã {id}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                using var response = await ApiClientService.DeleteAsync($"products/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Xóa sản phẩm thất bại ({(int)response.StatusCode}).\n{error}", "Lỗi xóa dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                await LoadAsync();
                StartNewProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa sản phẩm.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                StartNewProduct();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private sealed class CategoryOption
        {
            public CategoryOption() { }
            public CategoryOption(int categoryId, string categoryName) { CategoryId = categoryId; CategoryName = categoryName; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public override string ToString() => CategoryName;
        }
    }
}