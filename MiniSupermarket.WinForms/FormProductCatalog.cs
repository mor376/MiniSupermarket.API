using System;
using System.Collections.Generic;
using System.Drawing;
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
            Text = "Danh mục sản phẩm & giá";
            BackColor = Color.FromArgb(244, 246, 249);
            Font = new Font("Segoe UI", 9F);
            Padding = new Padding(20);
            BuildLayout();
            Load += async (_, _) => await LoadAsync();
            _search.TextChanged += (_, _) => ApplyFilter();
            _categoryFilter.SelectedIndexChanged += (_, _) => ApplyFilter();
        }

        private void BuildLayout()
        {
            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 38,
                Text = "DANH MỤC SẢN PHẨM & GIÁ",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Ink
            };
            var subtitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Text = "Tra cứu, cập nhật thông tin hàng hóa và giá bán của cửa hàng An Phước.",
                ForeColor = Muted
            };
            var top = new Panel { Dock = DockStyle.Top, Height = 108, Padding = new Padding(0, 0, 0, 12) };
            var tools = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 38, WrapContents = false, Padding = new Padding(0, 3, 0, 0) };
            _search.Width = 310;
            _search.Height = 29;
            _search.PlaceholderText = "Tìm theo tên hoặc quét mã vạch...";
            _categoryFilter.Width = 190;
            _categoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _categoryFilter.Items.Add(new CategoryOption(0, "Tất cả ngành hàng"));
            _categoryFilter.SelectedIndex = 0;
            var refresh = MakeButton("Làm mới", Color.FromArgb(229, 231, 235), Ink);
            refresh.Click += async (_, _) => await LoadAsync();
            var addNew = MakeButton("＋  THÊM SẢN PHẨM MỚI (F2)", Green, Color.White);
            addNew.Click += (_, _) => StartNewProduct();
            tools.Controls.AddRange(new Control[] { _search, _categoryFilter, refresh, addNew });
            top.Controls.Add(tools);
            top.Controls.Add(subtitle);
            top.Controls.Add(title);

            ConfigureGrid();
            var listCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
            var listTitle = new Label { Dock = DockStyle.Top, Height = 32, Text = "BẢNG DANH MỤC SẢN PHẨM", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Ink };
            listCard.Controls.Add(_grid);
            listCard.Controls.Add(listTitle);

            var details = BuildDetailsPanel();
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Size = new Size(1000, 500),
                Orientation = Orientation.Vertical,
                SplitterWidth = 10,
                Panel1MinSize = 460,
                Panel2MinSize = 300,
                BackColor = Color.FromArgb(244, 246, 249)
            };
            split.Panel1.Padding = new Padding(0, 0, 8, 0);
            split.Panel2.Padding = new Padding(8, 0, 0, 0);
            split.Panel1.Controls.Add(listCard);
            split.Panel2.Controls.Add(details);
            split.SizeChanged += (_, _) => SetSplitDistance(split);
            split.HandleCreated += (_, _) => SetSplitDistance(split);

            Controls.Add(split);
            Controls.Add(top);
        }

        private static void SetSplitDistance(SplitContainer split)
        {
            var available = split.Width - split.SplitterWidth;
            if (available > split.Panel1MinSize + split.Panel2MinSize)
                split.SplitterDistance = Math.Max(split.Panel1MinSize, Math.Min(available - split.Panel2MinSize, available * 68 / 100));
        }

        private void ConfigureGrid()
        {
            _grid.Dock = DockStyle.Fill;
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.None;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.MultiSelect = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoGenerateColumns = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 246, 250);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Ink;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            _grid.DefaultCellStyle.SelectionForeColor = Ink;
            _grid.RowTemplate.Height = 34;
            AddColumn("ProductId", "Mã SP", 8);
            AddColumn("Barcode", "Mã vạch", 12);
            AddColumn("ProductName", "Tên sản phẩm", 23);
            AddColumn("CategoryId", "Ngành hàng", 10);
            AddColumn("CostPrice", "Giá nhập", 12, "N0");
            AddColumn("SellingPrice", "Giá bán lẻ", 12, "N0");
            AddColumn("StockQuantity", "Tồn kho", 10);
            AddColumn("Unit", "ĐVT", 8);
            _grid.SelectionChanged += (_, _) => SelectCurrentRow();

            var menu = new ContextMenuStrip();
            menu.Items.Add("Sửa sản phẩm", null, (_, _) => SelectCurrentRow());
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

        private void AddColumn(string property, string header, float weight, string? format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = property,
                DataPropertyName = property,
                HeaderText = header,
                FillWeight = weight,
                MinimumWidth = 55
            };
            if (format != null) column.DefaultCellStyle.Format = format;
            _grid.Columns.Add(column);
        }

        private Control BuildDetailsPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16) };
            var title = new Label { Dock = DockStyle.Top, Height = 32, Text = "CHI TIẾT & SỬA ĐỔI", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Ink };
            var fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 16, Padding = new Padding(0, 8, 0, 0) };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (var i = 0; i < 8; i++)
            {
                fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 19));
                fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 29));
            }
            ConfigureText(_id, true);
            ConfigureText(_barcode);
            ConfigureText(_name);
            ConfigureText(_unit);
            ConfigureMoney(_cost);
            ConfigureMoney(_price);
            _stock.Minimum = 0;
            _stock.Maximum = 10000000;
            _stock.ThousandsSeparator = true;
            _category.DropDownStyle = ComboBoxStyle.DropDownList;
            AddField(fields, 0, "Mã sản phẩm", _id);
            AddField(fields, 2, "Mã vạch", _barcode);
            AddField(fields, 4, "Tên sản phẩm", _name);
            AddField(fields, 6, "Ngành hàng", _category);
            AddField(fields, 8, "Giá vốn", _cost);
            AddField(fields, 10, "Giá bán lẻ", _price);
            AddField(fields, 12, "Tồn kho", _stock);
            AddField(fields, 14, "Đơn vị tính", _unit);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 84, Padding = new Padding(0, 12, 0, 0), WrapContents = true };
            _add.Text = "THÊM";
            StyleButton(_add, Color.FromArgb(229, 231, 235), Ink);
            _update.Text = "CẬP NHẬT";
            StyleButton(_update, Color.FromArgb(37, 99, 235), Color.White);
            _delete.Text = "XÓA";
            StyleButton(_delete, Color.FromArgb(220, 38, 38), Color.White);
            _update.Enabled = false;
            _delete.Enabled = false;
            var clear = MakeButton("Làm trống", Color.FromArgb(229, 231, 235), Ink);
            clear.Click += (_, _) => StartNewProduct();
            _add.Click += async (_, _) => await SaveAsync(false);
            _update.Click += async (_, _) => await SaveAsync(true);
            _delete.Click += async (_, _) => await DeleteAsync();
            actions.Controls.AddRange(new Control[] { _add, _update, _delete, clear });
            panel.Controls.Add(fields);
            panel.Controls.Add(actions);
            panel.Controls.Add(title);
            return panel;
        }

        private static void AddField(TableLayoutPanel panel, int row, string caption, Control input)
        {
            panel.Controls.Add(new Label { Text = caption, Dock = DockStyle.Fill, ForeColor = Muted, TextAlign = ContentAlignment.BottomLeft }, 0, row);
            input.Dock = DockStyle.Fill;
            panel.Controls.Add(input, 0, row + 1);
        }

        private static void ConfigureText(TextBox box, bool readOnly = false)
        {
            box.ReadOnly = readOnly;
            box.BorderStyle = BorderStyle.FixedSingle;
            box.BackColor = readOnly ? Color.FromArgb(243, 244, 246) : Color.White;
        }

        private static void ConfigureMoney(NumericUpDown input)
        {
            input.Maximum = 9999999999999999m;
            input.DecimalPlaces = 2;
            input.ThousandsSeparator = true;
            input.TextAlign = HorizontalAlignment.Right;
        }

        private static Button MakeButton(string text, Color backColor, Color foreColor)
        {
            var button = new Button { Text = text, Height = 32, AutoSize = true, Padding = new Padding(10, 0, 10, 0) };
            StyleButton(button, backColor, foreColor);
            return button;
        }

        private static void StyleButton(Button button, Color backColor, Color foreColor)
        {
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            button.Height = 34;
            button.Margin = new Padding(4, 0, 4, 0);
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
                _categoryFilter.Items.Add(new CategoryOption(0, "Tất cả ngành hàng"));
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

        private void ApplyFilter()
        {
            var keyword = _search.Text.Trim();
            var categoryId = (_categoryFilter.SelectedItem as CategoryOption)?.CategoryId ?? 0;
            var filtered = _products.Where(p =>
                (categoryId == 0 || p.CategoryId == categoryId)
                && (keyword.Length == 0 || p.ProductName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                    || p.Barcode.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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

        private async Task SaveAsync(bool update)
        {
            if (string.IsNullOrWhiteSpace(_barcode.Text) || string.IsNullOrWhiteSpace(_name.Text)
                || string.IsNullOrWhiteSpace(_unit.Text) || _category.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập mã vạch, tên sản phẩm, ngành hàng và đơn vị tính.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (update && !int.TryParse(_id.Text, out _))
            {
                MessageBox.Show("Hãy chọn sản phẩm cần cập nhật.", "Chưa chọn sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var product = new
            {
                ProductId = update ? int.Parse(_id.Text) : 0,
                Barcode = _barcode.Text.Trim(),
                ProductName = _name.Text.Trim(),
                CategoryId = Convert.ToInt32(_category.SelectedValue),
                CostPrice = _cost.Value,
                SellingPrice = _price.Value,
                StockQuantity = (int)_stock.Value,
                Unit = _unit.Text.Trim()
            };
            try
            {
                using var response = update
                    ? await ApiClientService.PutAsJsonAsync($"products/{product.ProductId}", product)
                    : await ApiClientService.PostAsJsonAsync("products", product);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Lưu sản phẩm thất bại ({(int)response.StatusCode}).\n{error}", "Lỗi lưu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                await LoadAsync();
                StartNewProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu sản phẩm.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            if (keyData == Keys.F2)
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
