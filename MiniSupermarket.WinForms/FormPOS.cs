using System;
using System.ComponentModel;
using System.Drawing;
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
        private readonly TextBox _voucherInput = new();
        private readonly DataGridView _cartGrid = new();
        private readonly BindingList<CartLine> _cart = new();

        private readonly Label _totalLabel = new();
        private readonly Label _changeLabel = new();
        private readonly Label _clockLabel = new();
        private readonly Label _todayRevenueValue = new();
        private readonly Label _todayOrdersValue = new();
        private readonly Label _itemsSoldValue = new();
        private readonly Label _lowStockValue = new();

        private readonly Color navy = Color.FromArgb(25, 29, 46);
        private readonly Color blue = Color.FromArgb(35, 113, 205);
        private readonly Color green = Color.FromArgb(35, 145, 75);
        private readonly Color background = Color.FromArgb(244, 246, 250);
        private readonly Color borderColor = Color.FromArgb(225, 229, 236);

        public FormPOS()
        {
            Text = "MiniMart POS & Inventory - v1.2";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1320, 780);
            MinimumSize = new Size(1050, 650);
            BackColor = background;
            Font = new Font("Segoe UI", 9F);

            BuildInterface();
            Load += async (_, _) => await LoadPosSummaryAsync();
            _cart.ListChanged += (_, _) => UpdateTotal();
            UpdateTotal();

            var timer = new Timer { Interval = 1000 };
            timer.Tick += (_, _) =>
                _clockLabel.Text = DateTime.Now.ToString("dddd, dd MMM yyyy, HH:mm:ss");
            timer.Start();
            FormClosed += (_, _) => timer.Dispose();
        }

        private void BuildInterface()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12, 0, 12, 12),
                Margin = Padding.Empty,
                BackColor = background
            };

            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            content.Controls.Add(BuildTopBar(), 0, 0);
            content.Controls.Add(BuildStatistics(), 0, 1);
            content.Controls.Add(BuildSalesArea(), 0, 2);

            root.Controls.Add(content, 0, 0);
        }

        private Control BuildTopBar()
        {
            var bar = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 8, 0),
                Margin = Padding.Empty
            };

            bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));

            bar.Controls.Add(new Label
            {
                Text = "Trang chủ  ›  POS",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(100, 105, 115)
            }, 0, 0);

            var right = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(0, 8, 0, 0)
            };

            right.Controls.Add(new Label
            {
                Text = "⇥",
                AutoSize = true,
                Font = new Font("Segoe UI", 14F),
                ForeColor = navy,
                Cursor = Cursors.Hand
            });

            _clockLabel.Text = DateTime.Now.ToString("dddd, dd MMM yyyy, HH:mm:ss");
            _clockLabel.AutoSize = true;
            _clockLabel.Padding = new Padding(0, 5, 8, 0);
            right.Controls.Add(_clockLabel);

            bar.Controls.Add(right, 1, 0);
            return bar;
        }

        private Control BuildStatistics()
        {
            var area = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0, 5, 0, 8),
                BackColor = background
            };

            for (int i = 0; i < 4; i++)
            {
                area.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            }

            area.Controls.Add(StatCard("TỔNG ĐƠN THU HÔM NAY", "Đang tải...", "+0%", blue, _todayRevenueValue), 0, 0);
            area.Controls.Add(StatCard("HÓA ĐƠN ĐÃ LẬP", "0", "", green, _todayOrdersValue), 1, 0);
            area.Controls.Add(StatCard("MẶT HÀNG ĐÃ BÁN", "0", "", blue, _itemsSoldValue), 2, 0);
            area.Controls.Add(StatCard("TỒN KHO THẤP", "0 SP", "màu vàng", Color.DarkGoldenrod, _lowStockValue), 3, 0);

            return area;
        }

        private async Task LoadPosSummaryAsync()
        {
            try
            {
                var summaryTask = ApiClientService.GetAsync<JsonElement>("reports/dashboard-summary?range=1");
                var productsTask = ApiClientService.GetAsync<JsonElement>("products");
                var topProductsTask = ApiClientService.GetAsync<JsonElement>("reports/top-products?range=1");
                await Task.WhenAll(summaryTask, productsTask, topProductsTask);

                var summary = summaryTask.Result;
                var revenue = TryGetDecimal(summary, "totalRevenue");
                var orders = SumTotalOrders(summary);
                var soldItems = SumUnitsSold(topProductsTask.Result);
                var lowStock = CountLowStock(productsTask.Result);

                _todayRevenueValue.Text = $"{revenue:N0} VNĐ";
                _todayOrdersValue.Text = orders.ToString("N0");
                _itemsSoldValue.Text = soldItems.ToString("N0");
                _lowStockValue.Text = $"{lowStock} SP";
            }
            catch (Exception)
            {
                _todayRevenueValue.Text = "0 VNĐ";
                _todayOrdersValue.Text = "0";
                _itemsSoldValue.Text = "0";
                _lowStockValue.Text = "0 SP";
            }
        }

        private static decimal TryGetDecimal(JsonElement element, string propertyName)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out var prop))
            {
                return prop.TryGetDecimal(out var value) ? value : 0m;
            }

            return 0m;
        }

        private static int SumTotalOrders(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                return 0;
            }

            var total = 0;
            foreach (var item in data.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty("totalOrders", out var countProp) && countProp.TryGetInt32(out var count))
                {
                    total += count;
                }
            }

            return total;
        }

        private static int SumUnitsSold(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                return 0;
            }

            var total = 0;
            foreach (var item in data.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty("unitsSold", out var countProp) && countProp.TryGetInt32(out var count))
                {
                    total += count;
                }
            }

            return total;
        }

        private static int CountLowStock(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                return element.EnumerateArray().Count(item => item.ValueKind == JsonValueKind.Object && item.TryGetProperty("stockQuantity", out var prop) && prop.TryGetInt32(out var quantity) && quantity <= 10);
            }

            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            {
                return data.EnumerateArray().Count(item => item.ValueKind == JsonValueKind.Object && item.TryGetProperty("stockQuantity", out var prop) && prop.TryGetInt32(out var quantity) && quantity <= 10);
            }

            return 0;
        }

        private Control StatCard(string title, string value, string note, Color accent, Label? valueLabel = null)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 8, 0),
                Padding = new Padding(12, 7, 8, 5)
            };

            card.Paint += (_, e) =>
            {
                using var pen = new Pen(borderColor);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            card.Controls.Add(new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = Color.FromArgb(75, 80, 90),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            });

            var valueControl = valueLabel ?? new Label
            {
                Text = value,
                Dock = DockStyle.Top,
                Height = 32,
                ForeColor = accent,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            };
            valueControl.Text = value;
            valueControl.Dock = DockStyle.Top;
            valueControl.Height = 32;
            valueControl.ForeColor = accent;
            valueControl.Font = new Font("Segoe UI", 16F, FontStyle.Bold);

            card.Controls.Add(valueControl);

            card.Controls.Add(new Label
            {
                Text = note,
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = accent,
                Font = new Font("Segoe UI", 8F)
            });

            return card;
        }

        private Control BuildSalesArea()
        {
            var area = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = Padding.Empty,
                BackColor = background
            };

            area.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
            area.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));

            area.Controls.Add(BuildCartPanel(), 0, 0);
            area.Controls.Add(BuildInvoicePanel(), 1, 0);

            return area;
        }

        private Control BuildCartPanel()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 8, 0)
            };

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            panel.Controls.Add(new Label
            {
                Text = "GIỎ HÀNG",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = navy
            }, 0, 0);

            var searchBar = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            searchBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            searchBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

            _barcodeInput.Dock = DockStyle.Fill;
            _barcodeInput.Font = new Font("Segoe UI", 10F);
            _barcodeInput.PlaceholderText = "⌕  Quét mã hoặc nhập tên sản phẩm...";
            _barcodeInput.KeyDown += BarcodeInput_KeyDown;

            var searchButton = new Button
            {
                Text = "Tìm kiếm",
                Dock = DockStyle.Fill,
                BackColor = blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5, 0, 0, 0)
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.Click += async (_, _) => await SearchProductAsync();

            searchBar.Controls.Add(_barcodeInput, 0, 0);
            searchBar.Controls.Add(searchButton, 1, 0);
            panel.Controls.Add(searchBar, 0, 1);

            ConfigureCartGrid();
            panel.Controls.Add(_cartGrid, 0, 2);

            return panel;
        }

        private void ConfigureCartGrid()
        {
            _cartGrid.Dock = DockStyle.Fill;
            _cartGrid.AutoGenerateColumns = false;
            _cartGrid.AllowUserToAddRows = false;
            _cartGrid.AllowUserToResizeRows = false;
            _cartGrid.ReadOnly = true;
            _cartGrid.RowHeadersVisible = false;
            _cartGrid.BackgroundColor = Color.White;
            _cartGrid.BorderStyle = BorderStyle.FixedSingle;
            _cartGrid.GridColor = borderColor;
            _cartGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _cartGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _cartGrid.RowTemplate.Height = 32;
            _cartGrid.EnableHeadersVisualStyles = false;

            _cartGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 245, 249);
            _cartGrid.ColumnHeadersDefaultCellStyle.ForeColor = navy;
            _cartGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            _cartGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            _cartGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            _cartGrid.DefaultCellStyle.SelectionForeColor = navy;

            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "STT", FillWeight = 35, DataPropertyName = nameof(CartLine.ProductId) });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã SP", FillWeight = 65, DataPropertyName = nameof(CartLine.ProductId) });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên hàng hóa", FillWeight = 150, DataPropertyName = nameof(CartLine.ProductName) });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ĐVT", FillWeight = 45, DataPropertyName = nameof(CartLine.Unit) });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", FillWeight = 75, DataPropertyName = nameof(CartLine.UnitPrice), DefaultCellStyle = { Format = "N0" } });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", FillWeight = 55, DataPropertyName = nameof(CartLine.Quantity) });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành tiền", FillWeight = 85, DataPropertyName = nameof(CartLine.LineTotal), DefaultCellStyle = { Format = "N0" } });
            _cartGrid.Columns.Add(new DataGridViewButtonColumn { Name = "Delete", HeaderText = "Xóa", Text = "X", UseColumnTextForButtonValue = true, FillWeight = 35 });

            _cartGrid.DataSource = _cart;

            _cartGrid.CellContentClick += (_, e) =>
            {
                if (e.RowIndex < 0 || _cartGrid.Columns[e.ColumnIndex].Name != "Delete")
                    return;

                if (_cartGrid.Rows[e.RowIndex].DataBoundItem is CartLine line)
                {
                    _cart.Remove(line);
                }
            };

            _cart.ListChanged += (_, _) =>
            {
                for (int i = 0; i < _cartGrid.Rows.Count; i++)
                {
                    _cartGrid.Rows[i].Cells[0].Value = i + 1;
                }
            };
        }

        private Control BuildInvoicePanel()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(10),
                Margin = Padding.Empty
            };

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            panel.Controls.Add(new Label
            {
                Text = "THÔNG TIN HÓA ĐƠN",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = navy
            }, 0, 0);

            var details = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 12,
                Padding = new Padding(0, 5, 0, 0)
            };

            details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43));
            details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57));

            for (int i = 0; i < 12; i++)
            {
                details.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 10 ? 43 : i == 11 ? 48 : 35));
            }

            AddInvoiceRow(details, 0, "Khách hàng:");
            _customerIdInput.Text = "1";
            _customerIdInput.Dock = DockStyle.Fill;
            details.Controls.Add(_customerIdInput, 1, 0);

            AddInvoiceRow(details, 1, "SĐT:");
            var phone = new TextBox { Text = "", PlaceholderText = "Nhập SĐT", Dock = DockStyle.Fill };
            details.Controls.Add(phone, 1, 1);

            AddInvoiceRow(details, 2, "Điểm tích lũy:");
            var points = new NumericUpDown { Minimum = 0, Maximum = 1000000, Value = 0, Dock = DockStyle.Fill };
            details.Controls.Add(points, 1, 2);

            AddInvoiceRow(details, 3, "Voucher:");
            _voucherInput.Dock = DockStyle.Fill;
            _voucherInput.PlaceholderText = "Nhập mã";
            details.Controls.Add(_voucherInput, 1, 3);

            AddInvoiceRow(details, 4, "Tổng tiền hàng:");
            AddInvoiceRow(details, 5, "Giảm giá:");
            AddInvoiceRow(details, 6, "VAT:");

            var subtotalLabel = ValueLabel("0 VND");
            var discountLabel = ValueLabel("0 VND");
            var vatLabel = ValueLabel("0 VND");

            details.Controls.Add(subtotalLabel, 1, 4);
            details.Controls.Add(discountLabel, 1, 5);
            details.Controls.Add(vatLabel, 1, 6);

            var separator = new Label
            {
                Text = "────────────────────",
                Dock = DockStyle.Fill,
                ForeColor = borderColor,
                TextAlign = ContentAlignment.MiddleCenter
            };
            details.Controls.Add(separator, 0, 7);
            details.SetColumnSpan(separator, 2);

            var totalTitle = new Label
            {
                Text = "TỔNG CỘNG:",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            details.Controls.Add(totalTitle, 0, 8);

            _totalLabel.Dock = DockStyle.Fill;
            _totalLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _totalLabel.ForeColor = green;
            _totalLabel.TextAlign = ContentAlignment.MiddleRight;
            details.Controls.Add(_totalLabel, 1, 8);

            details.Controls.Add(new Label { Text = "Tiền khách đưa:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 9);
            _cashReceivedInput.Dock = DockStyle.Fill;
            _cashReceivedInput.PlaceholderText = "Nhập tiền";
            _cashReceivedInput.TextChanged += (_, _) => UpdateTotal();
            details.Controls.Add(_cashReceivedInput, 1, 9);

            details.Controls.Add(new Label { Text = "Tiền thừa:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 10);
            _changeLabel.Dock = DockStyle.Fill;
            _changeLabel.ForeColor = green;
            _changeLabel.TextAlign = ContentAlignment.MiddleRight;
            details.Controls.Add(_changeLabel, 1, 10);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 3, 0, 0)
            };

            var cashButton = PaymentButton("Tiền mặt\n(F8)");
            cashButton.Click += (_, _) => _cashReceivedInput.Focus();

            var transferButton = PaymentButton("Chuyển khoản\n(F9)");
            transferButton.Click += (_, _) => MessageBox.Show("Vui lòng xác nhận chuyển khoản trước khi thanh toán.", "Chuyển khoản");

            var clearButton = PaymentButton("Hủy POS");
            clearButton.Click += (_, _) =>
            {
                _cart.Clear();
                _cashReceivedInput.Clear();
                _barcodeInput.Focus();
            };

            buttons.Controls.Add(cashButton);
            buttons.Controls.Add(transferButton);
            buttons.Controls.Add(clearButton);

            details.Controls.Add(buttons, 0, 11);
            details.SetColumnSpan(buttons, 2);

            var payButton = new Button
            {
                Text = "THANH TOÁN & IN HÓA ĐƠN (F12)",
                Dock = DockStyle.Bottom,
                Height = 42,
                BackColor = Color.FromArgb(28, 31, 46),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 0, 0)
            };
            payButton.FlatAppearance.BorderSize = 0;
            payButton.Click += CheckoutButton_Click;

            var invoiceContent = new Panel { Dock = DockStyle.Fill };
            invoiceContent.Controls.Add(details);
            invoiceContent.Controls.Add(payButton);

            panel.Controls.Add(invoiceContent, 0, 1);

            return panel;
        }

        private void AddInvoiceRow(TableLayoutPanel table, int row, string title)
        {
            table.Controls.Add(new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(65, 70, 80)
            }, 0, row);
        }

        private Label ValueLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };
        }

        private Button PaymentButton(string text)
        {
            var button = new Button
            {
                Text = text,
                Width = 76,
                Height = 39,
                BackColor = Color.FromArgb(238, 240, 244),
                ForeColor = navy,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F),
                Margin = new Padding(0, 0, 4, 0),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderColor = borderColor;
            return button;
        }

        private async Task SearchProductAsync()
        {
            var barcode = _barcodeInput.Text.Trim();
            if (barcode.Length == 0)
                return;

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
                    _cart.Add(new CartLine
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        UnitPrice = product.Price,
                        Quantity = 1,
                        Unit = product.Unit
                    });
                }
                else
                {
                    existing.Quantity++;
                    _cart.ResetBindings();
                }

                _barcodeInput.Clear();
                _barcodeInput.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tra cứu sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BarcodeInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;
            await SearchProductAsync();
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
                UpdateTotal();
                await LoadPosSummaryAsync();
                SessionManager.TriggerDashboardRefresh();
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
            _totalLabel.Text = $"{total:N0} VND";
            _changeLabel.Text = decimal.TryParse(_cashReceivedInput.Text, out var cashReceived)
                ? $"{Math.Max(0, cashReceived - total):N0} VND"
                : "0 VND";
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
            public string Unit { get; init; } = "";
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
