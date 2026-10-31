using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSupermarket.WinForms.Models;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormInventoryView : Form
    {
        private static readonly Color Ink = Color.FromArgb(15, 23, 42);
        private static readonly Color Muted = Color.FromArgb(107, 114, 128);
        private static readonly Color SoftBg = Color.FromArgb(244, 246, 250);
        private static readonly Color Border = Color.FromArgb(226, 232, 240);
        private static readonly Color Blue = Color.FromArgb(37, 99, 235);
        private static readonly Color Green = Color.FromArgb(22, 163, 74);
        private static readonly Color Amber = Color.FromArgb(245, 158, 11);
        private static readonly Color Red = Color.FromArgb(220, 38, 38);

        private readonly DataGridView _grid = new();
        private readonly Label _totalProductsValue = new();
        private readonly Label _lowStockValue = new();
        private readonly Label _urgentValue = new();
        private List<InventoryRow> _allRows = new();
        private ComboBox _categoryFilter = null!;
        private ComboBox _statusFilter = null!;
        private ComboBox _supplierFilter = null!;
        private Button _refreshButton = null!;
        private ComboBox _receiveSupplierCombo = null!;
        private ListBox _supplierListBox = null!;
        private List<string> _suppliers = new();

        public FormInventoryView()
        {
            Text = "Quản lý Kho & Nhập hàng";
            BackColor = SoftBg;
            Font = new Font("Segoe UI", 9F);
            Padding = new Padding(16, 8, 16, 12);
            MinimumSize = new Size(1180, 760);
            Size = new Size(1280, 820);
            WindowState = FormWindowState.Normal;

            BuildLayout();
            Load += async (_, _) =>
            {
                await LoadSupplierOptionsAsync();
                await LoadDataAsync();
            };
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Title Bar
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));  // KPI Cards
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid Panel
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 220)); // Bottom Panel

            root.Controls.Add(BuildTitleBar(), 0, 0);
            root.Controls.Add(BuildKpiCards(), 0, 1);
            root.Controls.Add(BuildGridPanel(), 0, 2);
            root.Controls.Add(BuildBottomPanel(), 0, 3);

            Controls.Add(root);
        }

        private Control BuildTitleBar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = SoftBg };
            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "QUẢN LÝ KHO NHẬP HÀNG (Inventory Warehouse)",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Ink,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildKpiCards()
        {
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = SoftBg,
                Margin = new Padding(0)
            };

            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            container.Controls.Add(CreateKpiCard("TỔNG MẶT HÀNG", _totalProductsValue, "mẫu hàng đang quản lý", Blue), 0, 0);
            container.Controls.Add(CreateKpiCard("SẢN PHẨM SẮP HẾT HÀNG (Tồn Min)", _lowStockValue, "màu cam", Amber), 1, 0);
            container.Controls.Add(CreateKpiCard("SẢN PHẨM CẬN HẠN (Dưới 30 ngày)", _urgentValue, "màu đỏ cảnh báo", Red), 2, 0);
            return container;
        }

        private Control CreateKpiCard(string title, Label valueLabel, string note, Color accent)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 4),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10, 6, 8, 4)
            };

            var titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 18,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                TextAlign = ContentAlignment.MiddleLeft
            };

            valueLabel.Dock = DockStyle.Top;
            valueLabel.Height = 26;
            valueLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            valueLabel.ForeColor = accent;
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;

            var noteLabel = new Label
            {
                Text = note,
                Dock = DockStyle.Top,
                Height = 16,
                Font = new Font("Segoe UI", 7.5F),
                ForeColor = accent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(noteLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            return card;
        }

        private Control BuildGridPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SoftBg,
                Padding = new Padding(0, 2, 0, 0)
            };

            var topHeaderPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 28
            };

            var sectionTitle = new Label
            {
                Dock = DockStyle.Left,
                Text = "BẢNG TỒN KHO THỰC TẾ (Real-time Inventory)",
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
            toolsPanel.Controls.Add(new Button { Text = "🗑", Width = 28, Height = 22, FlatStyle = FlatStyle.Flat });
            toolsPanel.Controls.Add(new Button { Text = "☰", Width = 28, Height = 22, FlatStyle = FlatStyle.Flat });
            toolsPanel.Controls.Add(new Button { Text = "🪟", Width = 28, Height = 22, FlatStyle = FlatStyle.Flat });

            topHeaderPanel.Controls.Add(sectionTitle);
            topHeaderPanel.Controls.Add(toolsPanel);

            var filterBar = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 32,
                ColumnCount = 6,
                Margin = new Padding(0, 0, 0, 4)
            };
            filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7F));
            filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7F));

            _categoryFilter = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Height = 24 };
            _statusFilter = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Height = 24 };
            _supplierFilter = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Height = 24 };
            _supplierFilter.Items.Add("Tất cả");
            _supplierFilter.SelectedIndex = 0;

            var datePick = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Height = 24 };
            datePick.Value = new DateTime(2025, 9, 7);

            var clear = new Button
            {
                Text = "Lọc",
                BackColor = Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(2, 1, 2, 1),
                Height = 24
            };
            clear.FlatAppearance.BorderSize = 0;
            clear.Click += (_, _) => ApplyFilters();

            _refreshButton = new Button
            {
                Text = "↻",
                BackColor = Border,
                ForeColor = Ink,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(2, 1, 2, 1),
                Height = 24
            };
            _refreshButton.FlatAppearance.BorderSize = 0;
            _refreshButton.Click += async (_, _) => await LoadDataAsync();

            filterBar.Controls.Add(_categoryFilter, 0, 0);
            filterBar.Controls.Add(_supplierFilter, 1, 0);
            filterBar.Controls.Add(_statusFilter, 2, 0);
            filterBar.Controls.Add(datePick, 3, 0);
            filterBar.Controls.Add(clear, 4, 0);
            filterBar.Controls.Add(_refreshButton, 5, 0);

            var gridWrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0)
            };

            ConfigureGrid();
            gridWrap.Controls.Add(_grid);

            panel.Controls.Add(gridWrap);
            panel.Controls.Add(filterBar);
            panel.Controls.Add(topHeaderPanel);

            return panel;
        }

        private void ConfigureGrid()
        {
            _grid.Dock = DockStyle.Fill;
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.FixedSingle;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.ColumnHeadersVisible = true;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _grid.ScrollBars = ScrollBars.Both;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Ink;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.RowTemplate.Height = 26;
            _grid.ColumnHeadersHeight = 26;
            _grid.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0) return;
                var row = _grid.Rows[e.RowIndex].DataBoundItem as InventoryRow;
                if (row == null) return;

                var cellStyle = e.CellStyle ?? new DataGridViewCellStyle();
                if (row.Status == "Cận hạn")
                {
                    cellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    cellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                    e.CellStyle = cellStyle;
                }
                else if (row.Status == "Sắp hết")
                {
                    cellStyle.BackColor = Color.FromArgb(255, 247, 237);
                    cellStyle.ForeColor = Color.FromArgb(146, 64, 14);
                    e.CellStyle = cellStyle;
                }
            };

            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "STT", DataPropertyName = nameof(InventoryRow.Stt), Width = 45 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã SP", DataPropertyName = nameof(InventoryRow.ProductCode), Width = 95 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên hàng hóa", DataPropertyName = nameof(InventoryRow.ProductName), Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngành hàng", DataPropertyName = nameof(InventoryRow.Category), Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nhà cung cấp", DataPropertyName = nameof(InventoryRow.SupplierName), Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ĐVT", DataPropertyName = nameof(InventoryRow.Unit), Width = 55 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tồn kho thực", DataPropertyName = nameof(InventoryRow.Stock), Width = 95 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá vốn", DataPropertyName = nameof(InventoryRow.CostPrice), Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hạn sử dụng", DataPropertyName = nameof(InventoryRow.Expiry), Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Vị trí kệ", DataPropertyName = nameof(InventoryRow.Location), Width = 80 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = nameof(InventoryRow.Status), Width = 90 });
        }

        private void PopulateFilterOptions(IEnumerable<InventoryRow> rows)
        {
            var categories = rows
                .Select(r => r.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            _categoryFilter.SelectedIndexChanged -= CategoryFilterChanged;
            _statusFilter.SelectedIndexChanged -= StatusFilterChanged;
            _supplierFilter.SelectedIndexChanged -= SupplierFilterChanged;

            _categoryFilter.Items.Clear();
            _categoryFilter.Items.Add("Tất cả");
            foreach (var category in categories)
            {
                _categoryFilter.Items.Add(category);
            }
            _categoryFilter.SelectedIndex = 0;

            _statusFilter.Items.Clear();
            _statusFilter.Items.Add("Tất cả");
            _statusFilter.Items.Add("Bình thường");
            _statusFilter.Items.Add("Sắp hết");
            _statusFilter.Items.Add("Cận hạn");
            _statusFilter.SelectedIndex = 0;

            var supplierNames = _suppliers.Count > 0 ? _suppliers : new List<string>
            {
                "Công ty TNHH Nông sản An Phước",
                "Công ty CP Thực phẩm Sài Gòn",
                "Công ty TNHH Sữa Quốc Tế",
                "Công ty CP Nước giải khát Long Thành",
                "Công ty TNHH Hàng tiêu dùng Miền Nam",
                "Công ty CP Gia vị & Dầu ăn Việt",
                "Công ty CP Bánh kẹo & Snack HCM",
                "Công ty TNHH TM Mặt hàng tươi An Nam"
            };

            _supplierFilter.Items.Clear();
            _supplierFilter.Items.Add("Tất cả");
            foreach (var supplier in supplierNames.Distinct())
            {
                _supplierFilter.Items.Add(supplier);
            }
            _supplierFilter.SelectedIndex = 0;

            _categoryFilter.SelectedIndexChanged += CategoryFilterChanged;
            _statusFilter.SelectedIndexChanged += StatusFilterChanged;
            _supplierFilter.SelectedIndexChanged += SupplierFilterChanged;
        }

        private void CategoryFilterChanged(object? sender, EventArgs e) => ApplyFilters();

        private void StatusFilterChanged(object? sender, EventArgs e) => ApplyFilters();

        private void SupplierFilterChanged(object? sender, EventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            if (_allRows == null)
            {
                return;
            }

            var filtered = _allRows.AsEnumerable();

            var selectedCategory = _categoryFilter.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(selectedCategory) && selectedCategory != "Tất cả")
            {
                filtered = filtered.Where(r => string.Equals(r.Category, selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            var selectedStatus = _statusFilter.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "Tất cả")
            {
                filtered = filtered.Where(r => string.Equals(r.Status, selectedStatus, StringComparison.OrdinalIgnoreCase));
            }

            var selectedSupplier = _supplierFilter.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(selectedSupplier) && selectedSupplier != "Tất cả")
            {
                filtered = filtered.Where(r => string.Equals(r.SupplierName, selectedSupplier, StringComparison.OrdinalIgnoreCase));
            }

            var list = filtered.ToList();
            UpdateKpiValues(list);
            _grid.DataSource = list;
        }

        private Control BuildBottomPanel()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = SoftBg,
                Padding = new Padding(0, 6, 0, 0)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            panel.Controls.Add(BuildReceiveForm(), 0, 0);
            panel.Controls.Add(BuildSupplierCard(), 1, 0);
            panel.Controls.Add(BuildActionCard(), 2, 0);
            return panel;
        }

        private Control BuildReceiveForm()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Text = "Màn hình nhập hàng",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Ink,
                Height = 24
            };

            var button = new Button
            {
                Text = "NHẬP KHO TỪ NCC (F7)",
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 0;

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(0, 2, 0, 4)
            };

            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            for (int i = 0; i < 5; i++) fields.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            fields.Controls.Add(new Label { Text = "Số phiếu nhập:", ForeColor = Muted, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            fields.Controls.Add(new TextBox { Text = "PN-2025-001", Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle }, 1, 0);
            fields.Controls.Add(new Label { Text = "Nhà cung cấp:", ForeColor = Muted, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            _receiveSupplierCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            RefreshSupplierControls();
            fields.Controls.Add(_receiveSupplierCombo, 1, 1);
            fields.Controls.Add(new Label { Text = "Ngày nhập:", ForeColor = Muted, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            var date = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            date.Value = new DateTime(2025, 9, 17);
            fields.Controls.Add(date, 1, 2);
            fields.Controls.Add(new Label { Text = "Hóa đơn nhập:", ForeColor = Muted, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            fields.Controls.Add(new TextBox { Text = "HD-0052", Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle }, 1, 3);
            fields.Controls.Add(new Label { Text = "Ghi chú:", ForeColor = Muted, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
            fields.Controls.Add(new TextBox { Text = "Nhập bổ sung theo kế hoạch", Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle }, 1, 4);

            card.Controls.Add(fields);
            card.Controls.Add(button);
            card.Controls.Add(title);
            return card;
        }

        private Control BuildSupplierCard()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Text = "Nhà cung cấp nổi bật",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Ink,
                Height = 24
            };

            _supplierListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(249, 250, 251),
                Margin = new Padding(0, 4, 0, 0)
            };
            RefreshSupplierControls();

            card.Controls.Add(_supplierListBox);
            card.Controls.Add(title);
            return card;
        }

        private Control BuildActionCard()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Text = "Danh mục tác vụ",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Ink,
                Height = 24
            };

            var list = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0),
                AutoScroll = true
            };

            list.Controls.Add(MakeTaskButton("Nhập kho từ NCC (F7)", Color.FromArgb(226, 232, 240), Ink));
            list.Controls.Add(MakeTaskButton("Kiểm kê kho", Color.FromArgb(226, 232, 240), Ink));
            list.Controls.Add(MakeTaskButton("Xuất hủy hàng hỏng", Color.FromArgb(226, 232, 240), Ink));
            list.Controls.Add(MakeTaskButton("Xem báo cáo tồn", Color.FromArgb(226, 232, 240), Ink));

            card.Controls.Add(list);
            card.Controls.Add(title);
            return card;
        }

        private static Button MakeTaskButton(string text, Color backColor, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                Height = 30,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Margin = new Padding(0, 0, 0, 6),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var products = await ApiClientService.GetAsync<List<ProductCatalogDto>>("products") ?? new List<ProductCatalogDto>();
                var rows = products.Select((p, index) => new InventoryRow
                {
                    Stt = index + 1,
                    ProductCode = p.Barcode,
                    ProductName = p.ProductName,
                    Category = string.IsNullOrWhiteSpace(p.CategoryName) ? "Chưa phân loại" : p.CategoryName,
                    Unit = string.IsNullOrWhiteSpace(p.Unit) ? "Cái" : p.Unit,
                    SupplierName = GetSupplierName(index, p.CategoryName),
                    Stock = p.StockQuantity,
                    CostPrice = p.CostPrice,
                    Expiry = DateTime.Today.AddDays(30 + index * 7).ToString("dd/MM/yyyy"),
                    Location = $"V{(index % 6) + 1}",
                    Status = GetInventoryStatus(p.StockQuantity)
                }).ToList();

                _allRows = rows;
                PopulateFilterOptions(rows);
                ApplyFilters();
                return;
            }
            catch
            {
            }

            var fallbackRows = new List<InventoryRow>
            {
                new() { Stt = 1, ProductCode = "AP-000001", ProductName = "Cà chua", Category = "Rau, củ, trái cây", Unit = "Kg", SupplierName = "Công ty TNHH Nông sản An Phước", Stock = 30, CostPrice = 13000, Expiry = "29/11/2026", Location = "V2", Status = "Bình thường" },
                new() { Stt = 2, ProductCode = "AP-000002", ProductName = "Khoai tây", Category = "Rau, củ, trái cây", Unit = "Kg", SupplierName = "Công ty TNHH Nông sản An Phước", Stock = 27, CostPrice = 18000, Expiry = "06/12/2026", Location = "V3", Status = "Bình thường" },
                new() { Stt = 3, ProductCode = "AP-000003", ProductName = "Thịt heo ba rọi 500g", Category = "Thịt, cá, trứng", Unit = "Khay", SupplierName = "Công ty CP Thực phẩm Sài Gòn", Stock = 17, CostPrice = 55000, Expiry = "13/12/2026", Location = "V4", Status = "Bình thường" },
                new() { Stt = 4, ProductCode = "AP-000004", ProductName = "Trứng gà hộp 10 quả", Category = "Thịt, cá, trứng", Unit = "Hộp", SupplierName = "Công ty CP Thực phẩm Sài Gòn", Stock = 24, CostPrice = 25000, Expiry = "20/12/2026", Location = "V5", Status = "Bình thường" },
                new() { Stt = 5, ProductCode = "AP-000005", ProductName = "Cá basa phi lê 500g", Category = "Thịt, cá, trứng", Unit = "Khay", SupplierName = "Công ty CP Thực phẩm Sài Gòn", Stock = 12, CostPrice = 26000, Expiry = "27/12/2026", Location = "V6", Status = "Sắp hết" },
            };

            _allRows = fallbackRows;
            PopulateFilterOptions(fallbackRows);
            ApplyFilters();
        }

        private void UpdateKpiValues(IEnumerable<InventoryRow> rows)
        {
            var list = rows.ToList();
            if (list.Count == 0)
            {
                _totalProductsValue.Text = "0";
                _lowStockValue.Text = "0";
                _urgentValue.Text = "0";
                return;
            }

            var lowStock = list.Count(r => r.Status == "Sắp hết");
            var urgent = list.Count(r => r.Status == "Cận hạn");

            _totalProductsValue.Text = list.Count.ToString("N0");
            _lowStockValue.Text = lowStock.ToString();
            _urgentValue.Text = urgent.ToString();
        }

        private string GetSupplierName(int index, string? categoryName)
        {
            var fallbackSuppliers = new[]
            {
                "Công ty TNHH Nông sản An Phước",
                "Công ty CP Thực phẩm Sài Gòn",
                "Công ty TNHH Sữa Quốc Tế",
                "Công ty CP Nước giải khát Long Thành",
                "Công ty TNHH Hàng tiêu dùng Miền Nam",
                "Công ty CP Gia vị & Dầu ăn Việt",
                "Công ty CP Bánh kẹo & Snack HCM",
                "Công ty TNHH TM Mặt hàng tươi An Nam"
            };

            if (_suppliers.Count == 0)
            {
                var normalizedCategory = string.IsNullOrWhiteSpace(categoryName) ? string.Empty : categoryName.Trim();
                if (normalizedCategory.Contains("Rau", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("trái", StringComparison.OrdinalIgnoreCase))
                {
                    return fallbackSuppliers[0];
                }
                if (normalizedCategory.Contains("Thịt", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("cá", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("trứng", StringComparison.OrdinalIgnoreCase))
                {
                    return fallbackSuppliers[1];
                }
                if (normalizedCategory.Contains("Sữa", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("sữa", StringComparison.OrdinalIgnoreCase))
                {
                    return fallbackSuppliers[2];
                }
                if (normalizedCategory.Contains("Nước", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("giải", StringComparison.OrdinalIgnoreCase))
                {
                    return fallbackSuppliers[3];
                }
                if (normalizedCategory.Contains("Mì", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("gạo", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("khô", StringComparison.OrdinalIgnoreCase))
                {
                    return fallbackSuppliers[4];
                }
                if (normalizedCategory.Contains("Gia", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("dầu", StringComparison.OrdinalIgnoreCase))
                {
                    return fallbackSuppliers[5];
                }
                if (normalizedCategory.Contains("Bánh", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("kẹo", StringComparison.OrdinalIgnoreCase) || normalizedCategory.Contains("snack", StringComparison.OrdinalIgnoreCase))
                {
                    return fallbackSuppliers[6];
                }
                return fallbackSuppliers[index % fallbackSuppliers.Length];
            }

            var normalizedCategoryName = string.IsNullOrWhiteSpace(categoryName) ? string.Empty : categoryName.Trim();
            if (normalizedCategoryName.Contains("Rau", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("trái", StringComparison.OrdinalIgnoreCase))
            {
                return _suppliers[0];
            }
            if (normalizedCategoryName.Contains("Thịt", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("cá", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("trứng", StringComparison.OrdinalIgnoreCase))
            {
                return _suppliers[Math.Min(1, _suppliers.Count - 1)];
            }
            if (normalizedCategoryName.Contains("Sữa", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("sữa", StringComparison.OrdinalIgnoreCase))
            {
                return _suppliers[Math.Min(2, _suppliers.Count - 1)];
            }
            if (normalizedCategoryName.Contains("Nước", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("giải", StringComparison.OrdinalIgnoreCase))
            {
                return _suppliers[Math.Min(3, _suppliers.Count - 1)];
            }
            if (normalizedCategoryName.Contains("Mì", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("gạo", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("khô", StringComparison.OrdinalIgnoreCase))
            {
                return _suppliers[Math.Min(4, _suppliers.Count - 1)];
            }
            if (normalizedCategoryName.Contains("Gia", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("dầu", StringComparison.OrdinalIgnoreCase))
            {
                return _suppliers[Math.Min(5, _suppliers.Count - 1)];
            }
            if (normalizedCategoryName.Contains("Bánh", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("kẹo", StringComparison.OrdinalIgnoreCase) || normalizedCategoryName.Contains("snack", StringComparison.OrdinalIgnoreCase))
            {
                return _suppliers[Math.Min(6, _suppliers.Count - 1)];
            }
            return _suppliers[index % _suppliers.Count];
        }

        private async Task LoadSupplierOptionsAsync()
        {
            try
            {
                var suppliers = await ApiClientService.GetAsync<List<SupplierDto>>("suppliers") ?? new List<SupplierDto>();
                if (suppliers.Count > 0)
                {
                    _suppliers = suppliers
                        .Where(s => !string.IsNullOrWhiteSpace(s.SupplierName))
                        .Select(s => s.SupplierName)
                        .Distinct()
                        .ToList();
                }
                else
                {
                    _suppliers = new List<string>
                    {
                        "Công ty TNHH Nông sản An Phước",
                        "Công ty CP Thực phẩm Sài Gòn",
                        "Công ty TNHH Sữa Quốc Tế",
                        "Công ty CP Nước giải khát Long Thành",
                        "Công ty TNHH Hàng tiêu dùng Miền Nam",
                        "Công ty CP Gia vị & Dầu ăn Việt",
                        "Công ty CP Bánh kẹo & Snack HCM",
                        "Công ty TNHH TM Mặt hàng tươi An Nam"
                    };
                }
            }
            catch
            {
                _suppliers = new List<string>
                {
                    "Công ty TNHH Nông sản An Phước",
                    "Công ty CP Thực phẩm Sài Gòn",
                    "Công ty TNHH Sữa Quốc Tế",
                    "Công ty CP Nước giải khát Long Thành",
                    "Công ty TNHH Hàng tiêu dùng Miền Nam",
                    "Công ty CP Gia vị & Dầu ăn Việt",
                    "Công ty CP Bánh kẹo & Snack HCM",
                    "Công ty TNHH TM Mặt hàng tươi An Nam"
                };
            }

            RefreshSupplierControls();
        }

        private void RefreshSupplierControls()
        {
            var items = _suppliers.Count > 0
                ? _suppliers
                : new List<string>
                {
                    "Công ty TNHH Nông sản An Phước",
                    "Công ty CP Thực phẩm Sài Gòn",
                    "Công ty TNHH Sữa Quốc Tế",
                    "Công ty CP Nước giải khát Long Thành",
                    "Công ty TNHH Hàng tiêu dùng Miền Nam",
                    "Công ty CP Gia vị & Dầu ăn Việt",
                    "Công ty CP Bánh kẹo & Snack HCM",
                    "Công ty TNHH TM Mặt hàng tươi An Nam"
                };

            if (_receiveSupplierCombo != null)
            {
                _receiveSupplierCombo.Items.Clear();
                _receiveSupplierCombo.Items.AddRange(items.ToArray());
                if (_receiveSupplierCombo.Items.Count > 0)
                {
                    _receiveSupplierCombo.SelectedIndex = 0;
                }
            }

            if (_supplierListBox != null)
            {
                _supplierListBox.Items.Clear();
                foreach (var supplier in items)
                {
                    _supplierListBox.Items.Add(supplier);
                }
            }
        }

        private static string GetInventoryStatus(int stock)
        {
            if (stock <= 5) return "Cận hạn";
            if (stock <= 15) return "Sắp hết";
            return "Bình thường";
        }

        private sealed class InventoryRow
        {
            public int Stt { get; set; }
            public string ProductCode { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Unit { get; set; } = string.Empty;
            public string SupplierName { get; set; } = string.Empty;
            public int Stock { get; set; }
            public decimal CostPrice { get; set; }
            public string Expiry { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
    }
}