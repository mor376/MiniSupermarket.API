using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormAnalyticsDashboard : Form
    {
        private static readonly Color Ink = Color.FromArgb(31, 41, 55);
        private static readonly Color Muted = Color.FromArgb(107, 114, 128);
        private readonly Label _revenue = new();
        private readonly Label _orderCount = new();
        private readonly Label _cashierCount = new();
        private readonly DataGridView _grid = new();
        private readonly RevenueBars _chart = new();
        private readonly Label _updated = new();
        private readonly Button _refresh = new();
        private List<CashierRevenueDto> _data = new();

        public FormAnalyticsDashboard()
        {
            Text = "Báo cáo doanh thu";
            BackColor = Color.FromArgb(244, 246, 249);
            Font = new Font("Segoe UI", 9F);
            Padding = new Padding(20);
            BuildLayout();
            Load += async (_, _) => await LoadReportAsync();
        }

        private void BuildLayout()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 72 };
            header.Controls.Add(_refresh);
            _refresh.Text = "↻  LÀM MỚI";
            _refresh.Dock = DockStyle.Right;
            _refresh.Width = 130;
            StyleButton(_refresh, Color.FromArgb(37, 99, 235), Color.White);
            _refresh.Click += async (_, _) => await LoadReportAsync();
            header.Controls.Add(_updated);
            _updated.Dock = DockStyle.Right;
            _updated.Width = 200;
            _updated.TextAlign = ContentAlignment.MiddleRight;
            _updated.ForeColor = Muted;
            header.Controls.Add(new Label { Dock = DockStyle.Top, Height = 38, Text = "BÁO CÁO & THỐNG KÊ DOANH THU", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Ink });
            header.Controls.Add(new Label { Dock = DockStyle.Top, Height = 27, Text = "Tổng hợp số liệu bán hàng theo nhân viên thu ngân.", ForeColor = Muted });

            var kpis = new TableLayoutPanel { Dock = DockStyle.Top, Height = 94, ColumnCount = 3, Padding = new Padding(0, 4, 0, 12) };
            for (var i = 0; i < 3; i++) kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            AddKpi(kpis, 0, "TỔNG DOANH THU", _revenue, Color.FromArgb(22, 163, 74));
            AddKpi(kpis, 1, "TỔNG HÓA ĐƠN", _orderCount, Color.FromArgb(37, 99, 235));
            AddKpi(kpis, 2, "THU NGÂN CÓ GIAO DỊCH", _cashierCount, Color.FromArgb(234, 88, 12));

            ConfigureGrid();
            var chartCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14) };
            chartCard.Controls.Add(_chart);
            chartCard.Controls.Add(new Label { Dock = DockStyle.Top, Height = 32, Text = "DOANH THU THEO THU NGÂN", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Ink });
            var tableCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14) };
            tableCard.Controls.Add(_grid);
            tableCard.Controls.Add(new Label { Dock = DockStyle.Top, Height = 32, Text = "CHI TIẾT HIỆU SUẤT", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Ink });
            var split = new SplitContainer { Dock = DockStyle.Fill, Size = new Size(1000, 500), Orientation = Orientation.Vertical, SplitterWidth = 10, Panel1MinSize = 330, Panel2MinSize = 360, BackColor = Color.FromArgb(244, 246, 249) };
            split.Panel1.Padding = new Padding(0, 0, 8, 0);
            split.Panel2.Padding = new Padding(8, 0, 0, 0);
            split.Panel1.Controls.Add(chartCard);
            split.Panel2.Controls.Add(tableCard);
            split.SizeChanged += (_, _) => SetSplitDistance(split);
            split.HandleCreated += (_, _) => SetSplitDistance(split);

            Controls.Add(split);
            Controls.Add(kpis);
            Controls.Add(header);
        }

        private static void SetSplitDistance(SplitContainer split)
        {
            var available = split.Width - split.SplitterWidth;
            if (available > split.Panel1MinSize + split.Panel2MinSize)
                split.SplitterDistance = Math.Max(split.Panel1MinSize, Math.Min(available - split.Panel2MinSize, available / 2));
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
            _grid.AutoGenerateColumns = false;
            _grid.AllowUserToAddRows = false;
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.None;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 246, 250);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CashierRevenueDto.CashierUsername), HeaderText = "Thu ngân", FillWeight = 35 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CashierRevenueDto.TotalOrders), HeaderText = "Hóa đơn", FillWeight = 20 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CashierRevenueDto.TotalRevenue), HeaderText = "Doanh thu", FillWeight = 28, DefaultCellStyle = { Format = "N0" } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CashierRevenueDto.TotalPointsAwarded), HeaderText = "Điểm thưởng", FillWeight = 20 });
        }

        private async Task LoadReportAsync()
        {
            _refresh.Enabled = false;
            try
            {
                var result = await ApiClientService.GetAsync<ReportResponse>("reports/revenue-by-cashier");
                _data = result?.Data ?? new List<CashierRevenueDto>();
                _revenue.Text = (result?.GrandTotalRevenue ?? 0).ToString("N0") + " đ";
                _orderCount.Text = _data.Sum(item => item.TotalOrders).ToString("N0");
                _cashierCount.Text = (result?.TotalCashiers ?? _data.Count).ToString("N0");
                _grid.DataSource = null;
                _grid.DataSource = _data;
                _chart.Data = _data;
                _chart.Invalidate();
                _updated.Text = $"Cập nhật {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải báo cáo doanh thu.\n" + ex.Message, "Lỗi báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _refresh.Enabled = true; }
        }

        private static void StyleButton(Button button, Color back, Color fore)
        {
            button.BackColor = back;
            button.ForeColor = fore;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        }

        private sealed class ReportResponse
        {
            public ReportResponse() { }
            public decimal GrandTotalRevenue { get; set; }
            public int TotalCashiers { get; set; }
            public List<CashierRevenueDto> Data { get; set; } = new();
        }

        private sealed class CashierRevenueDto
        {
            public CashierRevenueDto() { }
            public string CashierUsername { get; set; } = string.Empty;
            public int TotalOrders { get; set; }
            public decimal TotalRevenue { get; set; }
            public int TotalPointsAwarded { get; set; }
        }

        private sealed class RevenueBars : Panel
        {
            public IReadOnlyList<CashierRevenueDto> Data { get; set; } = Array.Empty<CashierRevenueDto>();

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.Clear(Color.White);
                if (Data.Count == 0)
                {
                    TextRenderer.DrawText(e.Graphics, "Chưa có dữ liệu doanh thu.", Font, ClientRectangle, Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    return;
                }
                var maximum = Math.Max(1m, Data.Max(item => item.TotalRevenue));
                var rowHeight = Math.Max(42, (ClientSize.Height - 16) / Data.Count);
                using var barBrush = new SolidBrush(Color.FromArgb(59, 130, 246));
                using var trackBrush = new SolidBrush(Color.FromArgb(239, 246, 255));
                using var textBrush = new SolidBrush(Ink);
                using var mutedBrush = new SolidBrush(Muted);
                for (var i = 0; i < Data.Count; i++)
                {
                    var item = Data[i];
                    var y = 12 + i * rowHeight;
                    e.Graphics.DrawString(item.CashierUsername, Font, textBrush, 2, y);
                    var barY = y + 21;
                    var barWidth = Math.Max(4, ClientSize.Width - 8);
                    e.Graphics.FillRectangle(trackBrush, 2, barY, barWidth, 12);
                    var valueWidth = (int)(barWidth * item.TotalRevenue / maximum);
                    e.Graphics.FillRectangle(barBrush, 2, barY, Math.Max(2, valueWidth), 12);
                    e.Graphics.DrawString(item.TotalRevenue.ToString("N0") + " đ", Font, mutedBrush, 2, barY + 14);
                }
            }
        }
    }
}
