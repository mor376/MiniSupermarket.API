using System;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormHomeDashboard : Form
    {
        private static readonly Color Ink = Color.FromArgb(31, 41, 55);
        private static readonly Color Muted = Color.FromArgb(107, 114, 128);

        private readonly Label _totalProductsLabel = new();
        private readonly Label _totalCustomersLabel = new();
        private readonly Label _totalOrdersLabel = new();
        private readonly Label _totalRevenueLabel = new();

        public FormHomeDashboard()
        {
            Text = "Tổng quan hệ thống";
            BackColor = Color.FromArgb(244, 246, 249);
            Font = new Font("Segoe UI", 9F);
            Padding = new Padding(18);
            BuildLayout();
            Load += async (_, _) => await LoadDashboardAsync();
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
                    BeginInvoke(new MethodInvoker(async () => await LoadDashboardAsync()));
                    return;
                }

                await LoadDashboardAsync();
            }
            catch (Exception)
            {
            }
        }

        private void BuildLayout()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 78 };
            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                Text = "TỔNG QUAN HỆ THỐNG",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Ink
            };
            var subtitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                Text = "Theo dõi hoạt động bán hàng, kho và khách hàng trong thời gian thực.",
                ForeColor = Muted,
                Font = new Font("Segoe UI", 10F)
            };
            header.Controls.Add(subtitle);
            header.Controls.Add(title);

            var kpis = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                ColumnCount = 4,
                Padding = new Padding(0, 4, 0, 8)
            };
            for (var i = 0; i < 4; i++)
            {
                kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            AddKpiCard(kpis, 0, "TỔNG SẢN PHẨM", "Đang tải...", Color.FromArgb(59, 130, 246), _totalProductsLabel);
            AddKpiCard(kpis, 1, "KHÁCH HÀNG", "Đang tải...", Color.FromArgb(22, 163, 74), _totalCustomersLabel);
            AddKpiCard(kpis, 2, "HÓA ĐƠN", "Đang tải...", Color.FromArgb(168, 85, 247), _totalOrdersLabel);
            AddKpiCard(kpis, 3, "DOANH THU", "Đang tải...", Color.FromArgb(234, 88, 12), _totalRevenueLabel);

            var bottom = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 12, 0, 0)
            };
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));

            var tasks = CreateTaskPanel();
            var status = CreateStatusPanel();

            bottom.Controls.Add(tasks, 0, 0);
            bottom.Controls.Add(status, 1, 0);

            Controls.Add(bottom);
            Controls.Add(kpis);
            Controls.Add(header);
        }

        private static void AddKpiCard(TableLayoutPanel container, int column, string label, string value, Color color, Label? targetLabel = null)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(14, 10, 14, 10),
                Margin = new Padding(0, 0, 10, 0)
            };

            var valueLabel = targetLabel ?? new Label
            {
                Dock = DockStyle.Fill,
                Text = value,
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleLeft
            };

            if (targetLabel != null)
            {
                valueLabel.Dock = DockStyle.Fill;
                valueLabel.Text = value;
                valueLabel.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
                valueLabel.ForeColor = color;
                valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            }

            var labelName = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 22,
                Text = label,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Muted,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(labelName);
            container.Controls.Add(card, column, 0);
        }

        private async Task LoadDashboardAsync()
        {
            try
            {
                var productsTask = ApiClientService.GetAsync<JsonElement>("products");
                var customersTask = ApiClientService.GetAsync<JsonElement>("customers");
                var summaryTask = ApiClientService.GetAsync<JsonElement>("reports/dashboard-summary?range=30");
                await Task.WhenAll(productsTask, customersTask, summaryTask);

                var productCount = ParseArrayLength(productsTask.Result);
                var customerCount = ParseArrayLength(customersTask.Result);
                var summary = summaryTask.Result;
                var totalRevenue = summary.ValueKind == JsonValueKind.Object && summary.TryGetProperty("totalRevenue", out var revenueProp)
                    ? revenueProp.GetDecimal() : 0m;
                var totalOrders = 0;
                if (summary.ValueKind == JsonValueKind.Object && summary.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in dataProp.EnumerateArray())
                    {
                        if (item.TryGetProperty("totalOrders", out var totalOrdersProp))
                        {
                            totalOrders += totalOrdersProp.GetInt32();
                        }
                    }
                }

                _totalProductsLabel.Text = productCount.ToString("N0");
                _totalCustomersLabel.Text = customerCount.ToString("N0");
                _totalOrdersLabel.Text = totalOrders.ToString("D2");
                _totalRevenueLabel.Text = $"{totalRevenue:N0}đ";
            }
            catch (Exception)
            {
                _totalProductsLabel.Text = "0";
                _totalCustomersLabel.Text = "0";
                _totalOrdersLabel.Text = "0";
                _totalRevenueLabel.Text = "0đ";
            }
        }

        private static int ParseArrayLength(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                return element.GetArrayLength();
            }

            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array)
            {
                return dataProp.GetArrayLength();
            }

            return 0;
        }

        private static Panel CreateTaskPanel()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                Text = "NHIỆM VỤ TRONG NGÀY",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Ink
            };

            var list = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0)
            };

            AddTask(list, "✅ Kiểm tra tồn kho các mặt hàng có nguy cơ hết hàng.");
            AddTask(list, "✅ Duyệt hóa đơn bán lẻ cuối ngày và đối soát.");
            AddTask(list, "✅ Cập nhật chương trình khuyến mãi mới cho khách hàng VIP.");
            AddTask(list, "✅ Kiểm tra doanh thu theo thu ngân và báo cáo.");

            card.Controls.Add(list);
            card.Controls.Add(title);
            return card;
        }

        private static void AddTask(FlowLayoutPanel list, string text)
        {
            var item = new Label
            {
                Text = text,
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(55, 65, 81),
                Margin = new Padding(0, 0, 0, 10)
            };
            list.Controls.Add(item);
        }

        private static Panel CreateStatusPanel()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 32,
                Text = "TRẠNG THÁI CỬA HÀNG",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Ink
            };

            var statusList = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(0, 10, 0, 0)
            };
            statusList.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            statusList.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            AddStatus(statusList, "Phòng kho", "Ổn định", 0);
            AddStatus(statusList, "Quầy POS", "Đang mở", 1);
            AddStatus(statusList, "Khách hàng VIP", "17", 2);
            AddStatus(statusList, "Hệ thống", "Bình thường", 3);

            card.Controls.Add(statusList);
            card.Controls.Add(title);
            return card;
        }

        private static void AddStatus(TableLayoutPanel list, string label, string value, int row)
        {
            var name = new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                ForeColor = Muted,
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var valueLabel = new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(22, 163, 74),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };
            list.Controls.Add(name, 0, row);
            list.Controls.Add(valueLabel, 1, row);
        }
    }
}
