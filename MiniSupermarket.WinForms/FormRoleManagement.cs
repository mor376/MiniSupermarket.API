using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class FormRoleManagement : Form
    {
        // Use ApiClientService which attaches JWT token automatically

        private DataGridView dgvRoles;
        private TextBox txtId, txtRoleName, txtDescription;
        private Button btnLoad, btnAdd, btnUpdate, btnDelete;

        public FormRoleManagement()
        {
            Text = "Quản lý vai trò";
            Width = 800;
            Height = 520;
            InitializeComponent();
            Load += async (s, e) => await LoadRolesAsync();
        }

        private void InitializeComponent()
        {
            dgvRoles = new DataGridView { Dock = DockStyle.Top, Height = 280, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoles.CellClick += DgvRoles_CellClick;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };

            var lblId = new Label { Text = "ID", Width = 80 };
            txtId = new TextBox { Width = 200, ReadOnly = true };
            var lblRole = new Label { Text = "Tên vai trò", Width = 80 };
            txtRoleName = new TextBox { Width = 300 };
            var lblDesc = new Label { Text = "Mô tả", Width = 80 };
            txtDescription = new TextBox { Width = 300 };

            btnLoad = new Button { Text = "Tải", Width = 100 };
            btnLoad.Click += async (s, e) => await LoadRolesAsync();
            btnAdd = new Button { Text = "Thêm", Width = 100 };
            btnAdd.Click += async (s, e) => await BtnAdd_Click(s, e);
            btnUpdate = new Button { Text = "Cập nhật", Width = 100 };
            btnUpdate.Click += async (s, e) => await BtnUpdate_Click(s, e);
            btnDelete = new Button { Text = "Xóa", Width = 100 };
            btnDelete.Click += async (s, e) => await BtnDelete_Click(s, e);

            var flowTop = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 120, AutoSize = true };
            flowTop.Controls.Add(lblId);
            flowTop.Controls.Add(txtId);
            flowTop.Controls.Add(lblRole);
            flowTop.Controls.Add(txtRoleName);
            flowTop.Controls.Add(lblDesc);
            flowTop.Controls.Add(txtDescription);

            var flowButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            flowButtons.Controls.Add(btnLoad);
            flowButtons.Controls.Add(btnAdd);
            flowButtons.Controls.Add(btnUpdate);
            flowButtons.Controls.Add(btnDelete);

            panel.Controls.Add(flowButtons);
            panel.Controls.Add(flowTop);

            Controls.Add(dgvRoles);
            Controls.Add(panel);
        }

        private void DgvRoles_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvRoles.Rows[e.RowIndex];
                txtId.Text = row.Cells["Id"].Value?.ToString() ?? string.Empty;
                txtRoleName.Text = row.Cells["RoleName"].Value?.ToString() ?? string.Empty;
                txtDescription.Text = row.Cells["Description"].Value?.ToString() ?? string.Empty;
            }
        }

        private async Task LoadRolesAsync()
        {
            try
            {
                var roles = await ApiClientService.GetAsync<List<RoleDto>>("roles");
                dgvRoles.DataSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BtnAdd_Click(object? sender, EventArgs e)
        {
            var newRole = new { RoleName = txtRoleName.Text, Description = txtDescription.Text };
            try
            {
                var response = await ApiClientService.PostAsJsonAsync("roles", newRole);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm vai trò mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRolesAsync();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Thêm mới thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Vui lòng chọn vai trò cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = int.Parse(txtId.Text);
            var updateRole = new { Id = id, RoleName = txtRoleName.Text, Description = txtDescription.Text };
            try
            {
                var response = await ApiClientService.PutAsJsonAsync($"roles/{id}", updateRole);
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    MessageBox.Show("Cập nhật vai trò thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRolesAsync();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BtnDelete_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Vui lòng chọn vai trò cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = int.Parse(txtId.Text);
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa vai trò ID = {id}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    var response = await ApiClientService.DeleteAsync($"roles/{id}");
                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        MessageBox.Show("Xóa vai trò thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadRolesAsync();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputs()
        {
            txtId.Text = "";
            txtRoleName.Text = "";
            txtDescription.Text = "";
        }
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
