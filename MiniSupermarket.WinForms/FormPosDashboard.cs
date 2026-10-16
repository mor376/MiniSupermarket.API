using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSupermarket.WinForms.Models;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormPosDashboard : Form
    {
        private readonly TextBox _barcode = new();
        private readonly TextBox _customerId = new();
        private readonly TextBox _cashReceived = new();
        private readonly DataGridView _cartGrid = new();
        private readonly Label _cartCount = new();
        private readonly Label _total = new();
        private readonly Label _change = new();
        private readonly BindingList<CartLine> _cart = new();
        private readonly Button _checkout = new();
        private readonly ToolTip _paymentToolTip = new();
        private bool _isCheckingOut;

        public FormPosDashboard()
        {
            Text = "Quầy thu ngân POS";
            BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            Padding = new Padding(18);
            BuildLayout();
            _cart.ListChanged += (_, _) => UpdateSummary();
            _cashReceived.TextChanged += (_, _) => UpdateSummary();
            UpdateSummary();
        }

        private void BuildLayout()
        {
            var header = new System.Windows.Forms.Panel { Dock = DockStyle.Top, Height = 76 };
            header.Controls.Add(new Label
            {
                Dock = DockStyle.Top,
                Height = 38,
                Text = "QUẦY THU NGÂN · BÁN HÀNG POS",
                Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(31, 41, 55)
            });
            header.Controls.Add(new Label { Dock = DockStyle.Top, Height = 28, Text = "Quét mã vạch hoặc nhập mã sản phẩm để bắt đầu hóa đơn.", ForeColor = System.Drawing.Color.FromArgb(107, 114, 128) });

            var left = new System.Windows.Forms.Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.White, Padding = new Padding(14) };
            var cartTitle = new System.Windows.Forms.Panel { Dock = DockStyle.Top, Height = 42 };
            cartTitle.Controls.Add(new Label { Dock = DockStyle.Left, Width = 190, Text = "GIỎ HÀNG", Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold), TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
            _cartCount.Dock = DockStyle.Right;
            _cartCount.Width = 120;
            _cartCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            _cartCount.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            cartTitle.Controls.Add(_cartCount);
            var searchBar = new System.Windows.Forms.FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, WrapContents = false, Padding = new Padding(0, 4, 0, 5) };
            _barcode.Width = 350;
            _barcode.PlaceholderText = "Quét hoặc nhập mã vạch rồi nhấn Enter";
            _barcode.KeyDown += Barcode_KeyDown;
            var removeLine = new Button { Text = "XÓA DÒNG", Width = 105, Height = 30 };
            StyleButton(removeLine, System.Drawing.Color.FromArgb(254, 226, 226), System.Drawing.Color.FromArgb(185, 28, 28));
            removeLine.Click += (_, _) => RemoveSelectedLine();
            searchBar.Controls.AddRange(new Control[] { _barcode, removeLine });
            ConfigureCartGrid();
            left.Controls.Add(_cartGrid);
            left.Controls.Add(searchBar);
            left.Controls.Add(cartTitle);

            var payment = BuildPaymentPanel();
            var split = new SplitContainer { Dock = DockStyle.Fill, Size = new System.Drawing.Size(1000, 500), Orientation = Orientation.Vertical, SplitterWidth = 10, Panel1MinSize = 450, Panel2MinSize = 270, BackColor = System.Drawing.Color.FromArgb(244, 246, 249) };
            split.Panel1.Padding = new Padding(0, 0, 8, 0);
            split.Panel2.Padding = new Padding(8, 0, 0, 0);
            split.Panel1.Controls.Add(left);
            split.Panel2.Controls.Add(payment);
            split.SizeChanged += (_, _) => SetSplitDistance(split);
            split.HandleCreated += (_, _) => SetSplitDistance(split);

            Controls.Add(split);
            Controls.Add(header);
        }

        private static void SetSplitDistance(SplitContainer split)
        {
            var available = split.Width - split.SplitterWidth;
            if (available > split.Panel1MinSize + split.Panel2MinSize)
            {
                var desired = available * 70 / 100;
                split.SplitterDistance = Math.Max(split.Panel1MinSize, Math.Min(available - split.Panel2MinSize, desired));
            }
        }

        private void ConfigureCartGrid()
        {
            _cartGrid.Dock = DockStyle.Fill;
            _cartGrid.AutoGenerateColumns = false;
            _cartGrid.AllowUserToAddRows = false;
            _cartGrid.AllowUserToDeleteRows = false;
            _cartGrid.ReadOnly = true;
            _cartGrid.RowHeadersVisible = false;
            _cartGrid.MultiSelect = false;
            _cartGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _cartGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _cartGrid.BackgroundColor = System.Drawing.Color.White;
            _cartGrid.BorderStyle = BorderStyle.None;
            _cartGrid.EnableHeadersVisualStyles = false;
            _cartGrid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(243, 246, 250);
            _cartGrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _cartGrid.RowTemplate.Height = 36;
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.ProductId), HeaderText = "Mã SP", FillWeight = 12 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.Barcode), HeaderText = "Mã vạch", FillWeight = 18 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.ProductName), HeaderText = "Tên sản phẩm", FillWeight = 34 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.UnitPrice), HeaderText = "Đơn giá", FillWeight = 16, DefaultCellStyle = { Format = "N0" } });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.Quantity), HeaderText = "SL", FillWeight = 8 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.LineTotal), HeaderText = "Thành tiền", FillWeight = 18, DefaultCellStyle = { Format = "N0" } });
            _cartGrid.DataSource = _cart;
        }

        private Control BuildPaymentPanel()
        {
            var panel = new System.Windows.Forms.Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.White, Padding = new Padding(18) };
            var title = new Label { Dock = DockStyle.Top, Height = 40, Text = "THANH TOÁN", Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(31, 41, 55) };
            var customerLabel = new Label { Dock = DockStyle.Top, Height = 22, Text = "MÃ KHÁCH HÀNG", ForeColor = System.Drawing.Color.FromArgb(107, 114, 128) };
            _customerId.Dock = DockStyle.Top;
            _customerId.Height = 32;
            _customerId.Text = "1";
            var cashLabel = new Label { Dock = DockStyle.Top, Height = 28, Text = "TIỀN KHÁCH ĐƯA", ForeColor = System.Drawing.Color.FromArgb(107, 114, 128), Padding = new Padding(0, 8, 0, 0) };
            _cashReceived.Dock = DockStyle.Top;
            _cashReceived.Height = 34;
            _cashReceived.Font = new System.Drawing.Font("Segoe UI", 12F);
            var totalCard = new System.Windows.Forms.Panel { Dock = DockStyle.Top, Height = 112, BackColor = System.Drawing.Color.FromArgb(240, 253, 244), Padding = new Padding(14), Margin = new Padding(0, 18, 0, 0) };
            totalCard.Controls.Add(_total);
            _total.Dock = DockStyle.Fill;
            _total.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _total.Font = new System.Drawing.Font("Segoe UI", 21F, System.Drawing.FontStyle.Bold);
            _total.ForeColor = System.Drawing.Color.FromArgb(21, 128, 61);
            totalCard.Controls.Add(new Label { Dock = DockStyle.Top, Height = 25, Text = "TỔNG CỘNG", ForeColor = System.Drawing.Color.FromArgb(107, 114, 128) });
            var changePanel = new System.Windows.Forms.Panel { Dock = DockStyle.Top, Height = 44 };
            _change.Dock = DockStyle.Fill;
            _change.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _change.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            changePanel.Controls.Add(_change);
            var methodLabel = new Label { Dock = DockStyle.Top, Height = 30, Text = "PHƯƠNG THỨC · TIỀN MẶT (F8)", ForeColor = System.Drawing.Color.FromArgb(107, 114, 128), Padding = new Padding(0, 8, 0, 0) };
            var methods = new System.Windows.Forms.FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, WrapContents = false };
            var cash = new Button { Text = "Tiền mặt · F8", Width = 120, Height = 34, Enabled = false };
            StyleButton(cash, System.Drawing.Color.FromArgb(22, 163, 74), System.Drawing.Color.White);
            var bank = new Button { Text = "Chuyển khoản · F9", Width = 135, Height = 34, Enabled = false };
            StyleButton(bank, System.Drawing.Color.FromArgb(229, 231, 235), System.Drawing.Color.FromArgb(107, 114, 128));
            var card = new Button { Text = "Thẻ POS", Width = 80, Height = 34, Enabled = false };
            StyleButton(card, System.Drawing.Color.FromArgb(229, 231, 235), System.Drawing.Color.FromArgb(107, 114, 128));
            _paymentToolTip.SetToolTip(bank, "Phương thức này chưa được lưu/xử lý bởi API thanh toán.");
            _paymentToolTip.SetToolTip(card, "Phương thức này chưa được lưu/xử lý bởi API thanh toán.");
            methods.Controls.AddRange(new Control[] { cash, bank, card });
            _checkout.Text = "THANH TOÁN · F12";
            _checkout.Dock = DockStyle.Bottom;
            _checkout.Height = 50;
            StyleButton(_checkout, System.Drawing.Color.FromArgb(22, 163, 74), System.Drawing.Color.White);
            _checkout.Click += async (_, _) => await CheckoutAsync();
            panel.Controls.Add(_checkout);
            panel.Controls.Add(methods);
            panel.Controls.Add(methodLabel);
            panel.Controls.Add(changePanel);
            panel.Controls.Add(totalCard);
            panel.Controls.Add(_cashReceived);
            panel.Controls.Add(cashLabel);
            panel.Controls.Add(_customerId);
            panel.Controls.Add(customerLabel);
            panel.Controls.Add(title);
            return panel;
        }

        private static void StyleButton(Button button, System.Drawing.Color background, System.Drawing.Color foreground)
        {
            button.BackColor = background;
            button.ForeColor = foreground;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            button.Margin = new Padding(0, 0, 6, 0);
        }

        private async void Barcode_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            var barcode = _barcode.Text.Trim();
            if (barcode.Length == 0) return;
            _barcode.Clear();
            try
            {
                var product = await ApiClientService.GetAsync<ProductCatalogDto>($"products/barcode/{Uri.EscapeDataString(barcode)}");
                if (product == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm có mã vạch này.", "Tra cứu POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var line = _cart.FirstOrDefault(x => x.ProductId == product.ProductId);
                if (line == null)
                {
                    _cart.Add(new CartLine { ProductId = product.ProductId, Barcode = product.Barcode, ProductName = product.ProductName, UnitPrice = product.SellingPrice > 0 ? product.SellingPrice : product.Price, Quantity = 1 });
                }
                else
                {
                    line.Quantity++;
                    _cart.ResetBindings();
                }
                _barcode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không thể tra cứu sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveSelectedLine()
        {
            if (_cartGrid.CurrentRow?.DataBoundItem is CartLine line) _cart.Remove(line);
        }

        private void UpdateSummary()
        {
            var total = _cart.Sum(item => item.LineTotal);
            _cartCount.Text = $"{_cart.Sum(item => item.Quantity)} sản phẩm";
            _total.Text = $"{total:N0} đ";
            _change.Text = decimal.TryParse(_cashReceived.Text, out var cash) && cash >= total
                ? $"TIỀN THỪA     {cash - total:N0} đ"
                : "TIỀN THỪA     0 đ";
            _checkout.Enabled = _cart.Count > 0;
        }

        private async Task CheckoutAsync()
        {
            if (_isCheckingOut) return;
            if (_cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(_customerId.Text, out var customerId) || customerId <= 0)
            {
                MessageBox.Show("Mã khách hàng phải là số nguyên dương.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var total = _cart.Sum(item => item.LineTotal);
            if (!decimal.TryParse(_cashReceived.Text, out var cash) || cash < total)
            {
                MessageBox.Show("Tiền khách đưa chưa hợp lệ hoặc chưa đủ thanh toán.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var request = new { CustomerId = customerId, Items = _cart.Select(line => new { line.ProductId, line.Quantity, line.UnitPrice }).ToList() };
            _isCheckingOut = true;
            _checkout.Enabled = false;
            try
            {
                using var response = await ApiClientService.PostAsJsonAsync("checkout", request);
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(ReadMessage(body), "Thanh toán thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using var json = JsonDocument.Parse(body);
                var root = json.RootElement;
                var message = root.TryGetProperty("message", out var messageNode) ? messageNode.GetString() : "Thanh toán thành công.";
                var code = root.TryGetProperty("orderCode", out var codeNode) ? codeNode.GetString() : string.Empty;
                MessageBox.Show($"{message}\nMã hóa đơn: {code}\nTổng thanh toán: {total:N0} đ", "Giao dịch hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _cart.Clear();
                _cashReceived.Clear();
                _barcode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isCheckingOut = false;
                _checkout.Enabled = _cart.Count > 0;
            }
        }

        private static string ReadMessage(string body)
        {
            try
            {
                using var json = JsonDocument.Parse(body);
                if (json.RootElement.TryGetProperty("message", out var message)) return message.GetString() ?? body;
            }
            catch (JsonException) { }
            return body;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F8) { _cashReceived.Focus(); return true; }
            if (keyData == Keys.F9)
            {
                MessageBox.Show("API hiện chỉ hỗ trợ thanh toán tiền mặt; chuyển khoản chưa được triển khai.", "Phương thức thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            if (keyData == Keys.F12) { _ = CheckoutAsync(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private sealed class CartLine : INotifyPropertyChanged
        {
            private int _quantity;
            public int ProductId { get; init; }
            public string Barcode { get; init; } = string.Empty;
            public string ProductName { get; init; } = string.Empty;
            public decimal UnitPrice { get; init; }
            public int Quantity
            {
                get => _quantity;
                set
                {
                    _quantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineTotal)));
                }
            }
            public decimal LineTotal => UnitPrice * Quantity;
            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
}
