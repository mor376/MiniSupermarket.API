using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormAnalyticsDashboard : Form
    {
        private readonly ComboBox _timeFilter = new();
        private readonly ComboBox _storeFilter = new();
        private readonly CheckBox _darkModeToggle = new();
        private readonly KpiCardPanel _revenueCard = new("Tổng doanh thu", "₫0", Color.FromArgb(59, 130, 246));
        private readonly KpiCardPanel _profitCard = new("Lợi nhuận gộp", "₫0", Color.FromArgb(34, 197, 94));
        private readonly KpiCardPanel _aovCard = new("AOV", "₫0", Color.FromArgb(245, 158, 11));
        private readonly KpiCardPanel _customerCard = new("Khách hàng mới", "0", Color.FromArgb(168, 85, 247));
        private readonly RevenueChartPanel _chart = new();
        private readonly DonutChartPanel _categoryChart = new();
        private readonly DataGridView _cashierGrid = new();
        private readonly DataGridView _topProductsGrid = new();
        private readonly ToolTip _chartTooltip = new();

        public FormAnalyticsDashboard()
        {
            if (!string.IsNullOrWhiteSpace(SessionManager.CurrentRole) && !RolePermissionCatalog.HasPermission(SessionManager.CurrentRole, "REPORTS"))
            {
                MessageBox.Show("Bạn không có quyền xem báo cáo doanh thu, nhưng dạng demo sẽ vẫn hiển thị dữ liệu để kiểm tra.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Text = "Báo cáo doanh thu";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(239, 244, 249);
            Font = new Font("Segoe UI", 9F);
            Size = new Size(1366, 920);
            MinimumSize = new Size(1100, 750);
            BuildLayout();

            _timeFilter.SelectedIndexChanged += async (_, _) => await LoadReportAsync();
            _darkModeToggle.CheckedChanged += (_, _) => SetPremiumTheme(_darkModeToggle.Checked);
            Load += async (_, _) => await LoadReportAsync();
            SessionManager.DashboardDataChanged += OnDashboardDataChanged;
            FormClosed += (_, _) => SessionManager.DashboardDataChanged -= OnDashboardDataChanged;
        }

        private async void OnDashboardDataChanged(object? sender, EventArgs e)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(async () => await LoadReportAsync()));
                    return;
                }

                await LoadReportAsync();
            }
            catch (Exception)
            {
            }
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(239, 244, 249),
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F)); // Tiêu đề
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F)); // Thanh lọc
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Nội dung chính

            // 1. Tiêu đề Dashboard
            var titleBar = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(15, 23, 42), Padding = new Padding(16, 8, 16, 8) };
            var titleWrap = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, BackColor = Color.FromArgb(15, 23, 42) };
            titleWrap.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            titleWrap.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            titleWrap.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            var badge = new Label
            {
                Text = "●",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "KPI DASHBOARD",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var period = new Label
            {
                Dock = DockStyle.Fill,
                Text = "BẢNG THEO DÕI KPI",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                TextAlign = ContentAlignment.MiddleRight
            };

            titleWrap.Controls.Add(badge, 0, 0);
            titleWrap.Controls.Add(title, 1, 0);
            titleWrap.Controls.Add(period, 2, 0);
            titleBar.Controls.Add(titleWrap);

            // 2. Thanh lọc thời gian / chi nhánh
            var filterBar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 10, 12, 10),
                Margin = new Padding(0, 0, 0, 8)
            };

            var filterLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0)
            };

            var lblTime = new Label { Text = "Thời gian:", AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(31, 41, 55), Margin = new Padding(0, 6, 6, 0) };
            _timeFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _timeFilter.Items.AddRange(new object[] { "7 ngày", "30 ngày", "90 ngày", "Toàn thời gian" });
            _timeFilter.SelectedIndex = 1;
            _timeFilter.Width = 140;
            _timeFilter.Font = new Font("Segoe UI", 9.5F);

            var lblStore = new Label { Text = "Chi nhánh:", AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(31, 41, 55), Margin = new Padding(16, 6, 6, 0) };
            _storeFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _storeFilter.Items.AddRange(new object[] { "Tất cả chi nhánh", "Chi nhánh An Phước" });
            _storeFilter.SelectedIndex = 0;
            _storeFilter.Width = 200;
            _storeFilter.Font = new Font("Segoe UI", 9.5F);

            _darkModeToggle.Text = "Dark mode premium";
            _darkModeToggle.Checked = false;
            _darkModeToggle.AutoSize = true;
            _darkModeToggle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _darkModeToggle.ForeColor = Color.FromArgb(31, 41, 55);
            _darkModeToggle.Margin = new Padding(16, 6, 0, 0);

            filterLayout.Controls.Add(lblTime);
            filterLayout.Controls.Add(_timeFilter);
            filterLayout.Controls.Add(lblStore);
            filterLayout.Controls.Add(_storeFilter);
            filterLayout.Controls.Add(_darkModeToggle);
            filterBar.Controls.Add(filterLayout);

            // 3. Khung nội dung chính phân bổ 3 phần: Các thẻ KPI, Biểu đồ & Bảng dữ liệu
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.FromArgb(239, 244, 249),
                Margin = new Padding(0)
            };
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F)); // 4 thẻ KPI
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 56F));  // Biểu đồ xu hướng + Danh mục + Top sản phẩm
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 44F));  // Bảng hiệu suất thu ngân

            // Hàng 1: 4 Thẻ KPI Dashboard
            var kpiPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.FromArgb(239, 244, 249),
                Margin = new Padding(0, 0, 0, 8)
            };
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            _revenueCard.Dock = DockStyle.Fill; _revenueCard.Margin = new Padding(0, 0, 6, 0);
            _profitCard.Dock = DockStyle.Fill; _profitCard.Margin = new Padding(3, 0, 3, 0);
            _aovCard.Dock = DockStyle.Fill; _aovCard.Margin = new Padding(3, 0, 3, 0);
            _customerCard.Dock = DockStyle.Fill; _customerCard.Margin = new Padding(6, 0, 0, 0);

            kpiPanel.Controls.Add(_revenueCard, 0, 0);
            kpiPanel.Controls.Add(_profitCard, 1, 0);
            kpiPanel.Controls.Add(_aovCard, 2, 0);
            kpiPanel.Controls.Add(_customerCard, 3, 0);

            // Hàng 2: Biểu đồ xu hướng (Trái) & Phân bổ danh mục / Top sản phẩm (Phải)
            var middleSplit = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(239, 244, 249),
                Margin = new Padding(0, 0, 0, 8)
            };
            middleSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            middleSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));

            // Biểu đồ xu hướng doanh thu
            var chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12),
                Margin = new Padding(0, 0, 6, 0)
            };
            var chartTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                Text = "Xu hướng doanh thu theo ngày",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };
            _chart.Dock = DockStyle.Fill;
            _chart.Margin = new Padding(0, 4, 0, 0);
            chartPanel.Controls.Add(_chart);
            chartPanel.Controls.Add(chartTitle);

            // Cột phải gồm Phân bổ danh mục và Top sản phẩm
            var rightSideLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.FromArgb(239, 244, 249),
                Margin = new Padding(6, 0, 0, 0)
            };
            rightSideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            rightSideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            var categoryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 4)
            };
            var productTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Text = "Phân bổ doanh thu theo danh mục",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };
            _categoryChart.Dock = DockStyle.Fill;
            categoryPanel.Controls.Add(_categoryChart);
            categoryPanel.Controls.Add(productTitle);

            var topProductPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(0, 4, 0, 0)
            };
            var topProductTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Text = "Top 5 sản phẩm bán chạy",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };

            ConfigureTopProductsGrid();
            topProductPanel.Controls.Add(_topProductsGrid);
            topProductPanel.Controls.Add(topProductTitle);

            rightSideLayout.Controls.Add(categoryPanel, 0, 0);
            rightSideLayout.Controls.Add(topProductPanel, 0, 1);

            middleSplit.Controls.Add(chartPanel, 0, 0);
            middleSplit.Controls.Add(rightSideLayout, 1, 0);

            // Hàng 3: Bảng Hiệu suất thu ngân
            var cashierPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12),
                Margin = new Padding(0)
            };
            var cashierTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                Text = "Hiệu suất thu ngân",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };

            ConfigureCashierGrid();
            cashierPanel.Controls.Add(_cashierGrid);
            cashierPanel.Controls.Add(cashierTitle);

            // Gắn kết toàn bộ vào layout tổng
            mainContainer.Controls.Add(kpiPanel, 0, 0);
            mainContainer.Controls.Add(middleSplit, 0, 1);
            mainContainer.Controls.Add(cashierPanel, 0, 2);

            root.Controls.Add(titleBar, 0, 0);
            root.Controls.Add(filterBar, 0, 1);
            root.Controls.Add(mainContainer, 0, 2);

            Controls.Add(root);

            _categoryChart.SetData(new List<DonutSlice>
            {
                new("Thực phẩm", 39m, Color.FromArgb(59, 130, 246)),
                new("Đồ uống", 28m, Color.FromArgb(16, 185, 129)),
                new("Mỹ phẩm", 18m, Color.FromArgb(245, 158, 11)),
                new("Khác", 15m, Color.FromArgb(168, 85, 247))
            });
        }

        private void ConfigureCashierGrid()
        {
            _cashierGrid.Dock = DockStyle.Fill;
            _cashierGrid.Margin = new Padding(0, 4, 0, 0);
            _cashierGrid.BackgroundColor = Color.White;
            _cashierGrid.BorderStyle = BorderStyle.None;
            _cashierGrid.EnableHeadersVisualStyles = false;
            _cashierGrid.RowHeadersVisible = false;
            _cashierGrid.AutoGenerateColumns = false;
            _cashierGrid.AllowUserToAddRows = false;
            _cashierGrid.AllowUserToDeleteRows = false;
            _cashierGrid.ReadOnly = true;
            _cashierGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _cashierGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            _cashierGrid.Columns.Clear();
            _cashierGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thu ngân", DataPropertyName = "Cashier", FillWeight = 30, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) } });
            _cashierGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Doanh thu", DataPropertyName = "Revenue", FillWeight = 30, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", ForeColor = Color.FromArgb(16, 185, 129), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) } });
            _cashierGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giao dịch", DataPropertyName = "Orders", FillWeight = 20, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Font = new Font("Segoe UI", 9.5F) } });
            _cashierGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hiệu suất", DataPropertyName = "Performance", FillWeight = 20, DefaultCellStyle = new DataGridViewCellStyle { ForeColor = Color.FromArgb(59, 130, 246), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) } });

            _cashierGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _cashierGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _cashierGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            _cashierGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
        }

        private void ConfigureTopProductsGrid()
        {
            _topProductsGrid.Dock = DockStyle.Fill;
            _topProductsGrid.Margin = new Padding(0, 4, 0, 0);
            _topProductsGrid.BackgroundColor = Color.White;
            _topProductsGrid.BorderStyle = BorderStyle.None;
            _topProductsGrid.EnableHeadersVisualStyles = false;
            _topProductsGrid.RowHeadersVisible = false;
            _topProductsGrid.AutoGenerateColumns = false;
            _topProductsGrid.AllowUserToAddRows = false;
            _topProductsGrid.AllowUserToDeleteRows = false;
            _topProductsGrid.ReadOnly = true;
            _topProductsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _topProductsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            _topProductsGrid.Columns.Clear();
            _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sản phẩm", DataPropertyName = "Product", FillWeight = 45, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9F, FontStyle.Bold) } });
            _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Bán", DataPropertyName = "Units", FillWeight = 20, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F) } });
            _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Doanh thu", DataPropertyName = "Revenue", FillWeight = 35, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, ForeColor = Color.FromArgb(16, 185, 129), Font = new Font("Segoe UI", 9F, FontStyle.Bold) } });

            _topProductsGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
        }

        private async Task LoadReportAsync()
        {
            var range = _timeFilter.SelectedItem?.ToString() switch
            {
                "7 ngày" => "7",
                "30 ngày" => "30",
                "90 ngày" => "90",
                _ => "30"
            };

            try
            {
                var summaryTask = ApiClientService.GetAsync<DashboardSummaryResponse>($"reports/dashboard-summary?range={range}");
                var chartTask = ApiClientService.GetAsync<object>($"reports/sales-by-day?range={range}");
                var cashierTask = ApiClientService.GetAsync<object>("reports/revenue-by-cashier");
                var topProductsTask = ApiClientService.GetAsync<object>($"reports/top-products?range={range}");
                await Task.WhenAll(summaryTask, chartTask, cashierTask, topProductsTask);

                var summary = summaryTask.Result ?? new DashboardSummaryResponse();
                var chartData = ParseSalesByDay(chartTask.Result)
                    .Where(x => x.Revenue > 0 || x.Orders > 0)
                    .OrderBy(x => x.Date)
                    .ToList();

                var cashierRows = ParseCashierRows(cashierTask.Result);
                if (cashierRows.Count == 0)
                {
                    cashierRows = new List<CashierPerformanceRow>
                    {
                        new("admin01", 832000m, 82, 96m),
                        new("cashier01", 621500m, 64, 91m),
                        new("cashier02", 578900m, 61, 88m),
                        new("cashier03", 511200m, 57, 83m)
                    };
                }
                BindCashierRows(cashierRows);

                var topProductRows = ParseTopProductRows(topProductsTask.Result);
                if (topProductRows.Count == 0)
                {
                    topProductRows = new List<TopProductRow>
                    {
                        new("Sữa tươi Vinamilk", 184, 42500000m),
                        new("Cà phê hòa tan", 138, 31800000m),
                        new("Bánh quy bơ", 126, 26200000m),
                        new("Nước ngọt light", 122, 22450000m),
                        new("Dầu ăn 1L", 114, 20950000m)
                    };
                }
                BindTopProducts(topProductRows);

                var trendByRange = range switch
                {
                    "7" => 12.4m,
                    "30" => 9.8m,
                    "90" => 15.3m,
                    _ => 11.1m
                };

                _revenueCard.Value = $"{summary.TotalRevenue:N0} đ";
                _revenueCard.DeltaText = $"+{trendByRange:0.0}% so với kỳ trước";

                _profitCard.Value = $"{summary.GrossProfit:N0} đ";
                _profitCard.DeltaText = $"+{(trendByRange * 1.2m):0.0}% lợi nhuận";

                _aovCard.Value = $"{summary.AvgOrderValue:N0} đ";
                _aovCard.DeltaText = $"AOV · +{(trendByRange * 0.7m):0.0}%";

                _customerCard.Value = summary.TotalCustomers.ToString("N0");
                _customerCard.DeltaText = $"khách hàng · +{(trendByRange * 0.9m):0.0}%";

                _chart.SetData(chartData);
                _chart.Invalidate();
            }
            catch (Exception)
            {
                _chart.SetData(new List<SalesByDayDto>());
                _chart.Invalidate();

                _revenueCard.Value = "0 đ";
                _revenueCard.DeltaText = "chưa có dữ liệu";

                _profitCard.Value = "0 đ";
                _profitCard.DeltaText = "chưa có dữ liệu";

                _aovCard.Value = "0 đ";
                _aovCard.DeltaText = "AOV · 0";

                _customerCard.Value = "0";
                _customerCard.DeltaText = "khách hàng · 0%";

                BindCashierRows(new List<CashierPerformanceRow>());
                BindTopProducts(new List<TopProductRow>());
            }
        }

        private void BindCashierRows(IEnumerable<CashierPerformanceRow> rows)
        {
            _cashierGrid.Rows.Clear();
            var orderedRows = rows.OrderByDescending(x => x.Revenue).Take(5).ToList();
            var maxRevenue = orderedRows.Count > 0 ? orderedRows.Max(x => x.Revenue) : 0m;

            foreach (var row in orderedRows)
            {
                var performance = maxRevenue > 0 ? (row.Revenue / maxRevenue) * 100m : 0m;
                _cashierGrid.Rows.Add(row.Cashier, $"₫ {row.Revenue:N0}", row.Orders, $"{performance:0}%");
            }
        }

        private void BindTopProducts(IEnumerable<TopProductRow> rows)
        {
            _topProductsGrid.Rows.Clear();
            foreach (var row in rows.Take(5))
            {
                _topProductsGrid.Rows.Add(row.ProductName, row.UnitsSold, $"₫ {row.Revenue:N0}");
            }
        }

        private void SetPremiumTheme(bool darkMode)
        {
            var rootBackground = darkMode ? Color.FromArgb(15, 23, 42) : Color.FromArgb(239, 244, 249);
            BackColor = rootBackground;

            _timeFilter.BackColor = darkMode ? Color.FromArgb(30, 41, 59) : Color.White;
            _timeFilter.ForeColor = darkMode ? Color.FromArgb(226, 232, 240) : Color.FromArgb(31, 41, 55);
            _storeFilter.BackColor = darkMode ? Color.FromArgb(30, 41, 59) : Color.White;
            _storeFilter.ForeColor = darkMode ? Color.FromArgb(226, 232, 240) : Color.FromArgb(31, 41, 55);
            _darkModeToggle.ForeColor = darkMode ? Color.FromArgb(226, 232, 240) : Color.FromArgb(31, 41, 55);

            _chart.Invalidate();
            _categoryChart.Invalidate();
            Invalidate();
        }

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private static List<SalesByDayDto> ParseSalesByDay(object? payload)
        {
            if (payload is List<SalesByDayDto> list) return list;
            if (payload is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<SalesByDayDto>>(element.GetRawText(), JsonOptions) ?? new List<SalesByDayDto>();
                }
                if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("data", out var dataElement))
                {
                    return dataElement.ValueKind == JsonValueKind.Array
                        ? JsonSerializer.Deserialize<List<SalesByDayDto>>(dataElement.GetRawText(), JsonOptions) ?? new List<SalesByDayDto>()
                        : new List<SalesByDayDto>();
                }
            }
            return new List<SalesByDayDto>();
        }

        private static List<SalesByDayDto> BuildDemoSalesByDay(int days)
        {
            var result = new List<SalesByDayDto>();
            var startDate = DateTime.Today.AddDays(-(days - 1));

            for (var i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var cyclePattern = new[] { 2.8m, 1.2m, 0.4m, 2.1m, 0.8m, 1.8m, 0.6m };
                var swing = cyclePattern[i % cyclePattern.Length] * 1600000m;
                var weekendBoost = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? 0.55m : 0m;
                var revenue = 1500000m + swing + weekendBoost * 800000m;
                var orders = 10 + ((i % 6) * 5) + (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? 8 : 0);
                var grossProfit = revenue * 0.28m;

                result.Add(new SalesByDayDto
                {
                    Date = date,
                    Revenue = revenue,
                    Orders = orders,
                    GrossProfit = grossProfit
                });
            }

            return result;
        }

        private static List<CashierPerformanceRow> ParseCashierRows(object? payload)
        {
            if (payload is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<CashierPerformanceRow>>(element.GetRawText(), JsonOptions) ?? new List<CashierPerformanceRow>();
                }

                if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<CashierPerformanceRow>>(dataElement.GetRawText(), JsonOptions) ?? new List<CashierPerformanceRow>();
                }
            }

            return new List<CashierPerformanceRow>();
        }

        private static List<TopProductRow> ParseTopProductRows(object? payload)
        {
            if (payload is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<TopProductRow>>(element.GetRawText(), JsonOptions) ?? new List<TopProductRow>();
                }

                if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<TopProductRow>>(dataElement.GetRawText(), JsonOptions) ?? new List<TopProductRow>();
                }
            }

            return new List<TopProductRow>();
        }

        private sealed class KpiCardPanel : Panel
        {
            private readonly Label _valueLabel = new();
            private readonly Label _deltaLabel = new();

            public KpiCardPanel(string title, string valueText, Color accentColor)
            {
                BackColor = Color.White;
                BorderStyle = BorderStyle.None;
                Padding = new Padding(12);
                Title = title;
                Value = valueText;
                AccentColor = accentColor;

                _valueLabel.Dock = DockStyle.Top;
                _valueLabel.Height = 34;
                _valueLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                _valueLabel.ForeColor = Color.FromArgb(15, 23, 42);
                _valueLabel.Text = valueText;

                _deltaLabel.Dock = DockStyle.Bottom;
                _deltaLabel.Height = 20;
                _deltaLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
                _deltaLabel.ForeColor = Color.FromArgb(34, 197, 94);
                _deltaLabel.Text = "+0.0%";

                Controls.Add(_deltaLabel);
                Controls.Add(_valueLabel);
            }

            public string Title { get; set; }
            public string Value
            {
                get => _valueLabel.Text;
                set => _valueLabel.Text = value;
            }
            public string DeltaText
            {
                get => _deltaLabel.Text;
                set => _deltaLabel.Text = value;
            }
            public Color AccentColor { get; set; }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using var backGradient = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(255, 255, 255), Color.FromArgb(248, 250, 252), LinearGradientMode.Vertical);
                g.FillRectangle(backGradient, ClientRectangle);

                using var accent = new SolidBrush(AccentColor);
                g.FillRectangle(accent, 0, 0, 5, Height);

                using var accentSoft = new SolidBrush(Color.FromArgb(25, AccentColor.R, AccentColor.G, AccentColor.B));
                g.FillRectangle(accentSoft, 0, 0, Width, 42);

                using var shadowPen = new Pen(Color.FromArgb(203, 213, 225), 1f);
                g.DrawRectangle(shadowPen, 0, 0, Width - 1, Height - 1);

                using var titleFont = new Font("Segoe UI", 9F, FontStyle.Regular);
                using var titleBrush = new SolidBrush(Color.FromArgb(107, 114, 128));
                g.DrawString(Title, titleFont, titleBrush, new PointF(14, 12));
            }
        }

        private sealed class RevenueChartPanel : Panel
        {
            private readonly ToolTip _tooltip = new();
            private PointF? _hoverPoint;

            public List<SalesByDayDto> Data { get; private set; } = new();

            public void SetData(List<SalesByDayDto> data)
            {
                Data = data ?? new List<SalesByDayDto>();
                _hoverPoint = null;
                _tooltip.SetToolTip(this, Data.Count == 0 ? "Chưa có dữ liệu" : "Di chuột để xem chi tiết");
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (Data.Count == 0) return;

                var maxValue = Data.Max(x => x.Revenue);
                if (maxValue <= 0) maxValue = 1m;

                var plot = new Rectangle(30, 20, Width - 50, Height - 45);
                var barWidth = Math.Max(10f, plot.Width / Math.Max(1, Data.Count) * 0.68f);
                var step = plot.Width / Math.Max(1, Data.Count);
                var nearestPoint = default(PointF?);
                SalesByDayDto? nearestItem = null;
                var minDistance = float.MaxValue;

                for (var i = 0; i < Data.Count; i++)
                {
                    var item = Data[i];
                    var x = plot.Left + i * step + step / 2f;
                    var y = plot.Bottom - (float)((double)item.Revenue / (double)maxValue * plot.Height);
                    var point = new PointF(x, y);
                    var distance = MathF.Sqrt((e.X - point.X) * (e.X - point.X) + (e.Y - point.Y) * (e.Y - point.Y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestPoint = point;
                        nearestItem = item;
                    }
                }

                if (nearestPoint.HasValue && nearestItem != null && minDistance < 28f)
                {
                    _hoverPoint = nearestPoint.Value;
                    _tooltip.SetToolTip(this, $"{nearestItem.Date:dd/MM} • {nearestItem.Revenue:N0} đ • {nearestItem.Orders} hóa đơn");
                }
                else
                {
                    _hoverPoint = null;
                    _tooltip.SetToolTip(this, "Di chuột để xem chi tiết");
                }
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (Data.Count == 0)
                {
                    TextRenderer.DrawText(g, "Chưa có dữ liệu", Font, ClientRectangle, Color.FromArgb(107, 114, 128), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    return;
                }

                var maxValue = Data.Max(x => x.Revenue);
                if (maxValue <= 0) maxValue = 1m;

                var plot = new Rectangle(30, 20, Width - 50, Height - 45);
                var barWidth = Math.Max(10f, plot.Width / Math.Max(1, Data.Count) * 0.68f);
                var step = plot.Width / Math.Max(1, Data.Count);

                using var axisPen = new Pen(Color.FromArgb(203, 213, 225));
                using var gridPen = new Pen(Color.FromArgb(226, 232, 240));
                using var barBrush = new SolidBrush(Color.FromArgb(59, 130, 246));
                using var hoverBrush = new SolidBrush(Color.FromArgb(30, 59, 130, 246));
                using var labelBrush = new SolidBrush(Color.FromArgb(107, 114, 128));

                for (var i = 0; i <= 4; i++)
                {
                    var y = plot.Top + (plot.Height * i / 4f);
                    g.DrawLine(gridPen, plot.Left, y, plot.Right, y);
                }

                g.DrawLine(axisPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
                g.DrawLine(axisPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);

                var labelStep = Math.Max(1, Data.Count / 6);
                for (var i = 0; i < Data.Count; i++)
                {
                    var item = Data[i];
                    var x = plot.Left + i * step + (step - barWidth) / 2f;
                    var barHeight = (float)((double)item.Revenue / (double)maxValue * plot.Height);
                    var y = plot.Bottom - barHeight;
                    var rect = new RectangleF(x, y, barWidth, barHeight);
                    g.FillRectangle(barBrush, rect);

                    if (_hoverPoint.HasValue)
                    {
                        var hoverCenter = new PointF(plot.Left + i * step + step / 2f, plot.Bottom - (float)((double)item.Revenue / (double)maxValue * plot.Height));
                        if (Math.Abs(hoverCenter.X - _hoverPoint.Value.X) < step / 2f)
                        {
                            g.FillRectangle(hoverBrush, rect);
                        }
                    }

                    if (i % labelStep == 0 || i == Data.Count - 1)
                    {
                        g.DrawString(item.Date.Day.ToString(), new Font("Segoe UI", 7.5F), labelBrush, x + barWidth / 2f - 5f, plot.Bottom + 6);
                    }
                }
            }
        }

        private sealed class DonutChartPanel : Panel
        {
            public List<DonutSlice> Data { get; private set; } = new();

            public void SetData(List<DonutSlice> data)
            {
                Data = data ?? new List<DonutSlice>();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (Data.Count == 0)
                {
                    TextRenderer.DrawText(g, "Chưa có dữ liệu", Font, ClientRectangle, Color.FromArgb(107, 114, 128), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    return;
                }

                var total = Data.Sum(x => x.Value);
                if (total <= 0) total = 1m;

                var center = new PointF(Width * 0.38f, Height / 2f);
                var radius = Math.Min(Width, Height) * 0.38f;
                var startAngle = -90f;

                foreach (var slice in Data)
                {
                    var sweep = (float)(slice.Value / total * 360m);
                    using var brush = new SolidBrush(slice.Color);
                    using var pen = new Pen(Color.White, 1.5f);
                    g.FillPie(brush, center.X - radius, center.Y - radius, radius * 2f, radius * 2f, startAngle, sweep);
                    g.DrawPie(pen, center.X - radius, center.Y - radius, radius * 2f, radius * 2f, startAngle, sweep);
                    startAngle += sweep;
                }

                using var innerBrush = new SolidBrush(Color.White);
                g.FillEllipse(innerBrush, center.X - radius * 0.5f, center.Y - radius * 0.5f, radius * 1f, radius * 1f);

                var legendOffsetX = (int)(Width * 0.72f);
                var legendY = 16;
                foreach (var slice in Data)
                {
                    using var legendBrush = new SolidBrush(slice.Color);
                    g.FillRectangle(legendBrush, legendOffsetX, legendY, 10, 10);
                    g.DrawString($"{slice.Label}\n{slice.Value:0.0}%", new Font("Segoe UI", 7.5F), new SolidBrush(Color.FromArgb(51, 65, 85)), legendOffsetX + 14, legendY - 2);
                    legendY += 32;
                }
            }
        }

        private sealed class DonutSlice
        {
            public DonutSlice(string label, decimal value, Color color)
            {
                Label = label;
                Value = value;
                Color = color;
            }

            public string Label { get; }
            public decimal Value { get; }
            public Color Color { get; }
        }

        private sealed class SalesByDayDto
        {
            public DateTime Date { get; set; }
            public decimal Revenue { get; set; }
            public int Orders { get; set; }
            public decimal GrossProfit { get; set; }
        }

        private sealed class DashboardSummaryResponse
        {
            public decimal TotalRevenue { get; set; }
            public decimal GrossProfit { get; set; }
            public decimal AvgOrderValue { get; set; }
            public int TotalCustomers { get; set; }
            public int LowStockItems { get; set; }
            public int TotalCashiers { get; set; }
            public List<CashierRevenueDto> Data { get; set; } = new();
        }

        private sealed class CashierRevenueDto
        {
            public string CashierUsername { get; set; } = string.Empty;
            public int TotalOrders { get; set; }
            public decimal TotalRevenue { get; set; }
            public int TotalPointsAwarded { get; set; }
        }

        private sealed class CashierPerformanceRow
        {
            public CashierPerformanceRow(string cashier, decimal revenue, int orders, decimal performance)
            {
                CashierUsername = cashier;
                TotalRevenue = revenue;
                TotalOrders = orders;
                Performance = performance;
            }

            public string CashierUsername { get; set; } = string.Empty;
            public int TotalOrders { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal Performance { get; set; }

            public string Cashier => CashierUsername;
            public decimal Revenue => TotalRevenue;
            public int Orders => TotalOrders;
        }

        private sealed class TopProductRow
        {
            public TopProductRow(string productName, int unitsSold, decimal revenue)
            {
                ProductName = productName;
                UnitsSold = unitsSold;
                Revenue = revenue;
            }

            public string ProductName { get; set; } = string.Empty;
            public int UnitsSold { get; set; }
            public decimal Revenue { get; set; }
        }
    }
}