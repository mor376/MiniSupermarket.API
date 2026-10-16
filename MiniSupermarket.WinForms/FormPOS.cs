using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSupermarket.WinForms.Models;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormPOS : Form
    {
        private readonly TextBox _barcodeInput = new();
        private readonly TextBox _customerIdInput = new();
        private readonly TextBox _cashReceivedInput = new();
        private readonly DataGridView _cartGrid = new();
        private readonly Label _totalLabel = new();
        private readonly Label _changeLabel = new();
        private readonly BindingList<CartLine> _cart = new();

        public FormPOS()
        {
            Text = "Quầy bán hàng POS";
            BackColor = System.Drawing.Color.FromArgb(244, 245, 247);
            Padding = new Padding(24);

            var heading = new Label
            {
                Dock = DockStyle.Top,
                Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold),
                Height = 44,
                Text = "BÁN HÀNG POS"
            };

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 42,
                WrapContents = false
            };
            toolbar.Controls.Add(new Label { Text = "Mã vạch:", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            _barcodeInput.Width = 220;
            _barcodeInput.KeyDown += BarcodeInput_KeyDown;
            toolbar.Controls.Add(_barcodeInput);
            toolbar.Controls.Add(new Label { Text = "Mã khách hàng:", AutoSize = true, Padding = new Padding(12, 8, 0, 0) });
            _customerIdInput.Width = 80;
            _customerIdInput.Text = "1";
            toolbar.Controls.Add(_customerIdInput);

            _cartGrid.Dock = DockStyle.Fill;
            _cartGrid.AutoGenerateColumns = false;
            _cartGrid.AllowUserToAddRows = false;
            _cartGrid.ReadOnly = true;
            _cartGrid.RowHeadersVisible = false;
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.ProductId), HeaderText = "Mã SP", Width = 70 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.ProductName), HeaderText = "Sản phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.UnitPrice), HeaderText = "Đơn giá", Width = 120, DefaultCellStyle = { Format = "N0" } });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.Quantity), HeaderText = "SL", Width = 60 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CartLine.LineTotal), HeaderText = "Thành tiền", Width = 140, DefaultCellStyle = { Format = "N0" } });
            _cartGrid.DataSource = _cart;

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 54,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            var checkoutButton = new Button { Text = "THANH TOÁN", Width = 140, Height = 38 };
            checkoutButton.Click += CheckoutButton_Click;
            var clearButton = new Button { Text = "Hủy giỏ hàng", Width = 120, Height = 38 };
            clearButton.Click += (_, _) => _cart.Clear();
            _cashReceivedInput.Width = 120;
            _cashReceivedInput.TextChanged += (_, _) => UpdateTotal();
            _totalLabel.AutoSize = true;
            _totalLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            _totalLabel.Padding = new Padding(0, 5, 12, 0);
            _changeLabel.AutoSize = true;
            _changeLabel.Padding = new Padding(0, 9, 12, 0);
            footer.Controls.Add(checkoutButton);
            footer.Controls.Add(clearButton);
            footer.Controls.Add(_totalLabel);
            footer.Controls.Add(_changeLabel);
            footer.Controls.Add(_cashReceivedInput);
            footer.Controls.Add(new Label { Text = "Tiền khách đưa:", AutoSize = true, Padding = new Padding(0, 10, 0, 0) });

            Controls.Add(_cartGrid);
            Controls.Add(footer);
            Controls.Add(toolbar);
            Controls.Add(heading);
            _cart.ListChanged += (_, _) => UpdateTotal();
            UpdateTotal();
        }

        private async void BarcodeInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            var barcode = _barcodeInput.Text.Trim();
            _barcodeInput.Clear();
            if (barcode.Length == 0)
            {
                return;
            }

            try
            {
                var product = await ApiClientService.GetAsync<ProductDto>($"products/barcode/{Uri.EscapeDataString(barcode)}");
                if (product == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var existing = _cart.FirstOrDefault(line => line.ProductId == product.ProductId);
                if (existing == null)
                {
                    _cart.Add(new CartLine { ProductId = product.ProductId, ProductName = product.ProductName, UnitPrice = product.Price, Quantity = 1 });
                }
                else
                {
                    existing.Quantity++;
                    _cart.ResetBindings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tra cứu sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CheckoutButton_Click(object? sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(_customerIdInput.Text, out var customerId) || customerId <= 0)
            {
                MessageBox.Show("Mã khách hàng phải là số nguyên dương.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var total = _cart.Sum(line => line.LineTotal);
            if (!decimal.TryParse(_cashReceivedInput.Text, out var cashReceived) || cashReceived < total)
            {
                MessageBox.Show("Tiền khách đưa chưa hợp lệ hoặc chưa đủ thanh toán.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var request = new
            {
                CustomerId = customerId,
                Items = _cart.Select(line => new { line.ProductId, line.Quantity, line.UnitPrice }).ToList()
            };

            try
            {
                using var response = await ApiClientService.PostAsJsonAsync("checkout", request);
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(ReadResponseMessage(body), "Thanh toán thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(body, "Thanh toán thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _cart.Clear();
                _cashReceivedInput.Clear();
                _barcodeInput.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotal()
        {
            var total = _cart.Sum(line => line.LineTotal);
            _totalLabel.Text = $"Tổng: {total:N0} đ";
            _changeLabel.Text = decimal.TryParse(_cashReceivedInput.Text, out var cashReceived)
                ? $"Tiền thừa: {Math.Max(0, cashReceived - total):N0} đ"
                : "Tiền thừa: 0 đ";
        }

        private static string ReadResponseMessage(string body)
        {
            try
            {
                using var json = JsonDocument.Parse(body);
                if (json.RootElement.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? body;
                }
            }
            catch (JsonException)
            {
            }

            return body;
        }

        private sealed class CartLine : INotifyPropertyChanged
        {
            private int _quantity;

            public int ProductId { get; init; }
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
