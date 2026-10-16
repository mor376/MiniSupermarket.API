using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class FormCategoryManagement : Form
    {
        // DataGridView
        private DataGridView dgvCategories = null!;

        // Controls nhập liệu
        private TextBox txtId = null!;
        private TextBox txtCategoryName = null!;
        private TextBox txtDescription = null!;
        private TextBox txtKeyword = null!;

        // Controls nút bấm
        private Button btnLoad = null!;
        private Button btnAdd = null!;
        private Button btnUpdate = null!;
        private Button btnDelete = null!;
        private Button btnClear = null!;
        private Button btnSearch = null!;

        public FormCategoryManagement()
        {
            InitializeComponent();
            Load += FormCategoryManagement_Load;
        }

        private async void FormCategoryManagement_Load(object? sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        // =========================================================
        // KHỞI TẠO GIAO DIỆN
        // =========================================================
        private void InitializeComponent()
        {
            // Form Setup
            Text = "Quản lý Danh mục hàng - FormCategoryManagement";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 560);
            MinimumSize = new Size(850, 500);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            // -----------------------------------------------------
            // GROUPBOX TÌM KIẾM
            // -----------------------------------------------------
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
                PlaceholderText = "Nhập từ khóa danh mục..."
            };
            txtKeyword.KeyDown += TxtKeyword_KeyDown;

            btnSearch = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(375, 26),
                Size = new Size(90, 28)
            };
            btnSearch.Click += BtnSearch_Click;

            btnLoad = new Button
            {
                Text = "Tải lại",
                Location = new Point(475, 26),
                Size = new Size(80, 28)
            };
            btnLoad.Click += BtnLoad_Click;

            grpSearch.Controls.Add(txtKeyword);
            grpSearch.Controls.Add(btnSearch);
            grpSearch.Controls.Add(btnLoad);

            // -----------------------------------------------------
            // PANEL CHÍNH
            // -----------------------------------------------------
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            // -----------------------------------------------------
            // GROUPBOX THÔNG TIN NHÓM HÀNG (BÊN PHẢI)
            // -----------------------------------------------------
            GroupBox grpInfo = new GroupBox
            {
                Text = "Thông tin Nhóm hàng",
                Dock = DockStyle.Right,
                Width = 270,
                Padding = new Padding(10)
            };

            Label lblId = new Label { Text = "Mã ID", AutoSize = true, Location = new Point(12, 30) };
            txtId = new TextBox
            {
                Location = new Point(12, 50),
                Width = 240,
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            Label lblName = new Label { Text = "Tên Nhóm hàng (*)", AutoSize = true, Location = new Point(12, 85) };
            txtCategoryName = new TextBox
            {
                Location = new Point(12, 105),
                Width = 240
            };

            Label lblDescription = new Label { Text = "Mô tả", AutoSize = true, Location = new Point(12, 140) };
            txtDescription = new TextBox
            {
                Location = new Point(12, 160),
                Width = 240,
                Height = 85,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons thao tác
            btnAdd = new Button { Text = "Thêm mới", Size = new Size(75, 30), Location = new Point(12, 260) };
            btnUpdate = new Button { Text = "Cập nhật", Size = new Size(75, 30), Location = new Point(95, 260), Enabled = false };
            btnDelete = new Button { Text = "Xóa", Size = new Size(75, 30), Location = new Point(178, 260), Enabled = false };
            btnClear = new Button { Text = "Làm mới", Size = new Size(241, 28), Location = new Point(12, 300) };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearInputs();

            grpInfo.Controls.AddRange(new Control[] {
                lblId, txtId,
                lblName, txtCategoryName,
                lblDescription, txtDescription,
                btnAdd, btnUpdate, btnDelete, btnClear
            });

            // -----------------------------------------------------
            // GROUPBOX DANH SÁCH NHÓM HÀNG (BÊN TRÁI)
            // -----------------------------------------------------
            GroupBox grpList = new GroupBox
            {
                Text = "Danh sách Nhóm hàng",
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

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
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Cột DataGridView
            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryId",
                HeaderText = "Mã ID",
                DataPropertyName = "CategoryId",
                FillWeight = 20
            });

            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Tên Nhóm hàng",
                DataPropertyName = "CategoryName",
                FillWeight = 40
            });

            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Mô tả",
                DataPropertyName = "Description",
                FillWeight = 40
            });

            dgvCategories.CellClick += DgvCategories_CellClick;
            var categoryMenu = new ContextMenuStrip();
            categoryMenu.Items.Add("Sửa chi tiết", null, (_, _) =>
            {
                if (dgvCategories.CurrentRow != null)
                    DgvCategories_CellClick(dgvCategories, new DataGridViewCellEventArgs(0, dgvCategories.CurrentRow.Index));
            });
            categoryMenu.Items.Add("Xóa nhóm hàng", null, (_, _) => BtnDelete_Click(this, EventArgs.Empty));
            dgvCategories.ContextMenuStrip = categoryMenu;
            dgvCategories.CellMouseDown += (_, e) =>
            {
                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {
                    dgvCategories.ClearSelection();
                    dgvCategories.Rows[e.RowIndex].Selected = true;
                    dgvCategories.CurrentCell = dgvCategories.Rows[e.RowIndex].Cells[0];
                    DgvCategories_CellClick(dgvCategories, new DataGridViewCellEventArgs(0, e.RowIndex));
                }
            };

            grpList.Controls.Add(dgvCategories);

            // Ghép Panel
            mainPanel.Controls.Add(grpList);
            mainPanel.Controls.Add(grpInfo);

            Controls.Add(mainPanel);
            Controls.Add(grpSearch);
        }

        // =========================================================
        // TƯƠNG TÁC GIAO DIỆN & EVENT HANDLERS
        // =========================================================
        private void DgvCategories_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCategories.Rows.Count)
                return;

            DataGridViewRow row = dgvCategories.Rows[e.RowIndex];

            txtId.Text = row.Cells["CategoryId"].Value?.ToString() ?? string.Empty;
            txtCategoryName.Text = row.Cells["CategoryName"].Value?.ToString() ?? string.Empty;
            txtDescription.Text = row.Cells["Description"].Value?.ToString() ?? string.Empty;

            // Bật trạng thái sửa/xóa khi đã chọn bản ghi
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
            btnAdd.Enabled = false;
        }

        private async void TxtKeyword_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Chặn tiếng beep hệ thống
                await SearchAsync();
            }
        }

        private async void BtnSearch_Click(object? sender, EventArgs e) => await SearchAsync();
        private async void BtnLoad_Click(object? sender, EventArgs e) => await LoadDataAsync();

        // =========================================================
        // CÁC TÁC VỤ DỮ LIỆU (CRUD & API)
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
                ShowErrorMessage("Lỗi kết nối Server:\n\n" + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task SearchAsync()
        {
            string keyword = txtKeyword.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                var result = await ApiClientService.GetAsync<List<CategoryDto>>($"categories/search?keyword={Uri.EscapeDataString(keyword)}")
                             ?? new List<CategoryDto>();

                dgvCategories.DataSource = null;
                dgvCategories.DataSource = result;

                if (result.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy nhóm hàng phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ClearInputs();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Lỗi tìm kiếm:\n\n" + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput()) return;

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
                    MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Thêm mới thất bại!\n\n" + error, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private async void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput()) return;

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
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Cập nhật thất bại!\n\n" + error, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa nhóm hàng ID = {id}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                var response = await ApiClientService.DeleteAsync($"categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Xóa thất bại!\n\n" + error, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        // =========================================================
        // PHƯƠNG THỨC BỔ TRỢ
        // =========================================================
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F2)
            {
                ClearInputs();
                txtCategoryName.Focus();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhóm hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();

            dgvCategories.ClearSelection();

            // Trạng thái Form ban đầu
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show("Lỗi:\n\n" + message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // =============================================================
    // DTO
    // =============================================================
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}