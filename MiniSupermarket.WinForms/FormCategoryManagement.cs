using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class FormCategoryManagement : Form
    {
        // Use ApiClientService which attaches JWT token automatically

        // DataGridView
        private DataGridView dgvCategories;

        // TextBox
        private TextBox txtId;
        private TextBox txtCategoryName;
        private TextBox txtDescription;
        private TextBox txtKeyword;

        // Button
        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;

        public FormCategoryManagement()
        {
            InitializeComponent();

            Load += async (s, e) =>
            {
                await LoadDataAsync();
            };
        }

        // =========================================================
        // KHỞI TẠO GIAO DIỆN
        // =========================================================
        private void InitializeComponent()
        {
            // =====================================================
            // FORM
            // =====================================================
            Text = "Quản lý Danh mục hàng - FormCategoryManagement";
            StartPosition = FormStartPosition.CenterScreen;

            ClientSize = new Size(900, 560);
            MinimumSize = new Size(850, 500);

            Font = new Font("Segoe UI", 9F);

            // =====================================================
            // GROUPBOX TÌM KIẾM
            // =====================================================
            GroupBox grpSearch = new GroupBox
            {
                Text = "Tìm kiếm",
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(10)
            };

            txtKeyword = new TextBox
            {
                Location = new Point(15, 28),
                Size = new Size(350, 25),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            // Placeholder
            txtKeyword.PlaceholderText = "Nhập từ khóa...";

            btnSearch = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(375, 26),
                Size = new Size(90, 28)
            };

            btnLoad = new Button
            {
                Text = "Tải lại",
                Location = new Point(475, 26),
                Size = new Size(80, 28)
            };

            btnSearch.Click += async (s, e) =>
            {
                await BtnSearch_Click(s, e);
            };

            btnLoad.Click += async (s, e) =>
            {
                await LoadDataAsync();
            };

            grpSearch.Controls.Add(txtKeyword);
            grpSearch.Controls.Add(btnSearch);
            grpSearch.Controls.Add(btnLoad);

            // =====================================================
            // PANEL CHÍNH
            // =====================================================
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            // =====================================================
            // GROUPBOX THÔNG TIN NHÓM HÀNG - BÊN PHẢI
            // =====================================================
            GroupBox grpInfo = new GroupBox
            {
                Text = "Thông tin Nhóm hàng",
                Dock = DockStyle.Right,
                Width = 260,
                Padding = new Padding(10)
            };

            // -----------------------------
            // MÃ ID
            // -----------------------------
            Label lblId = new Label
            {
                Text = "Mã ID",
                AutoSize = true,
                Location = new Point(12, 30)
            };

            txtId = new TextBox
            {
                Location = new Point(12, 50),
                Width = 225,
                ReadOnly = true,
                BackColor = Color.White
            };

            // -----------------------------
            // TÊN NHÓM HÀNG
            // -----------------------------
            Label lblName = new Label
            {
                Text = "Tên Nhóm hàng",
                AutoSize = true,
                Location = new Point(12, 85)
            };

            txtCategoryName = new TextBox
            {
                Location = new Point(12, 105),
                Width = 225
            };

            // -----------------------------
            // MÔ TẢ
            // -----------------------------
            Label lblDescription = new Label
            {
                Text = "Mô tả",
                AutoSize = true,
                Location = new Point(12, 140)
            };

            txtDescription = new TextBox
            {
                Location = new Point(12, 160),
                Width = 225,
                Height = 75,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // -----------------------------
            // BUTTON
            // -----------------------------
            btnAdd = new Button
            {
                Text = "Thêm mới",
                Size = new Size(70, 30),
                Location = new Point(12, 250)
            };

            btnUpdate = new Button
            {
                Text = "Cập nhật",
                Size = new Size(70, 30),
                Location = new Point(88, 250)
            };

            btnDelete = new Button
            {
                Text = "Xóa",
                Size = new Size(70, 30),
                Location = new Point(164, 250)
            };

            btnAdd.Click += async (s, e) =>
            {
                await BtnAdd_Click(s, e);
            };

            btnUpdate.Click += async (s, e) =>
            {
                await BtnUpdate_Click(s, e);
            };

            btnDelete.Click += async (s, e) =>
            {
                await BtnDelete_Click(s, e);
            };

            grpInfo.Controls.Add(lblId);
            grpInfo.Controls.Add(txtId);

            grpInfo.Controls.Add(lblName);
            grpInfo.Controls.Add(txtCategoryName);

            grpInfo.Controls.Add(lblDescription);
            grpInfo.Controls.Add(txtDescription);

            grpInfo.Controls.Add(btnAdd);
            grpInfo.Controls.Add(btnUpdate);
            grpInfo.Controls.Add(btnDelete);

            // =====================================================
            // GROUPBOX DANH SÁCH NHÓM HÀNG - BÊN TRÁI
            // =====================================================
            GroupBox grpList = new GroupBox
            {
                Text = "Danh sách Nhóm hàng",
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            // =====================================================
            // DATAGRIDVIEW
            // =====================================================
            dgvCategories = new DataGridView
            {
                Dock = DockStyle.Fill,

                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,

                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,

                AutoGenerateColumns = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,

                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,

                RowHeadersVisible = true,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill
            };

            // =====================================================
            // CỘT MÃ ID
            // =====================================================
            DataGridViewTextBoxColumn colId =
                new DataGridViewTextBoxColumn
                {
                    Name = "CategoryId",
                    HeaderText = "Mã ID",
                    DataPropertyName = "CategoryId",
                    FillWeight = 20
                };

            // =====================================================
            // CỘT TÊN NHÓM HÀNG
            // =====================================================
            DataGridViewTextBoxColumn colName =
                new DataGridViewTextBoxColumn
                {
                    Name = "CategoryName",
                    HeaderText = "Tên Nhóm hàng",
                    DataPropertyName = "CategoryName",
                    FillWeight = 40
                };

            // =====================================================
            // CỘT MÔ TẢ
            // =====================================================
            DataGridViewTextBoxColumn colDescription =
                new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Mô tả",
                    DataPropertyName = "Description",
                    FillWeight = 40
                };

            dgvCategories.Columns.Add(colId);
            dgvCategories.Columns.Add(colName);
            dgvCategories.Columns.Add(colDescription);

            // Click vào dòng
            dgvCategories.CellClick += DgvCategories_CellClick;

            // Double click cũng chọn
            dgvCategories.CellDoubleClick += DgvCategories_CellClick;

            grpList.Controls.Add(dgvCategories);

            // =====================================================
            // THÊM VÀO PANEL CHÍNH
            // =====================================================
            mainPanel.Controls.Add(grpList);
            mainPanel.Controls.Add(grpInfo);

            // =====================================================
            // THÊM CONTROL VÀO FORM
            // =====================================================
            Controls.Add(mainPanel);
            Controls.Add(grpSearch);
        }

        // =========================================================
        // CLICK DÒNG TRONG DATAGRIDVIEW
        // =========================================================
        private void DgvCategories_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvCategories.Rows.Count <= e.RowIndex)
                return;

            DataGridViewRow row =
                dgvCategories.Rows[e.RowIndex];

            txtId.Text =
                row.Cells["CategoryId"].Value?.ToString()
                ?? string.Empty;

            txtCategoryName.Text =
                row.Cells["CategoryName"].Value?.ToString()
                ?? string.Empty;

            txtDescription.Text =
                row.Cells["Description"].Value?.ToString()
                ?? string.Empty;
        }

        // =========================================================
        // LOAD DATA
        // =========================================================
        private async Task LoadDataAsync()
        {
            try
            {

                Cursor = Cursors.WaitCursor;

                var categories = await ApiClientService.GetAsync<List<CategoryDto>>("categories") ?? new List<CategoryDto>();

                dgvCategories.DataSource = null;
                dgvCategories.DataSource = categories;

                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi kết nối Server:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // =========================================================
        // THÊM
        // =========================================================
        private async Task BtnAdd_Click(
            object? sender,
            EventArgs e)
        {
            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên nhóm hàng!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCategoryName.Focus();
                return;
            }

            var newCat = new
            {
                CategoryName = txtCategoryName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            try
            {
                var response = await ApiClientService.PostAsJsonAsync("categories", newCat);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Thêm mới thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Thêm mới thất bại!\n\n" + error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // CẬP NHẬT
        // =========================================================
        private async Task BtnUpdate_Click(
            object? sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show(
                    "Vui lòng chọn nhóm hàng cần sửa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Mã nhóm hàng không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên nhóm hàng!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var updateCat = new
            {
                CategoryId = id,
                CategoryName = txtCategoryName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            try
            {
                var response = await ApiClientService.PutAsJsonAsync($"categories/{id}", updateCat);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Cập nhật thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Cập nhật thất bại!\n\n" + error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // XÓA
        // =========================================================
        private async Task BtnDelete_Click(
            object? sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show(
                    "Vui lòng chọn nhóm hàng cần xóa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Mã nhóm hàng không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            var confirm =
                MessageBox.Show(
                    $"Bạn có chắc muốn xóa nhóm hàng ID = {id}?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                var response = await ApiClientService.DeleteAsync($"categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Xóa thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Xóa thất bại!\n\n" + error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // TÌM KIẾM
        // =========================================================
        private async Task BtnSearch_Click(
            object? sender,
            EventArgs e)
        {
            string keyword =
                txtKeyword.Text.Trim();

            // Nếu không nhập từ khóa -> load toàn bộ
            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                var result = await ApiClientService.GetAsync<List<CategoryDto>>($"categories/search?keyword={Uri.EscapeDataString(keyword)}") ?? new List<CategoryDto>();

                dgvCategories.DataSource = null;
                dgvCategories.DataSource = result;

                if (result == null || result.Count == 0)
                {
                    MessageBox.Show(
                        "Không tìm thấy nhóm hàng phù hợp!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // =========================================================
        // XÓA INPUT
        // =========================================================
        private void ClearInputs()
        {
            txtId.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();

            dgvCategories.ClearSelection();
        }
    }

    // =============================================================
    // DTO
    // =============================================================
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
            = string.Empty;

        public string? Description { get; set; }
    }
}
