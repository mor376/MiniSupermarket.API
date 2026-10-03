using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormCustomerManagement : Form
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7035/api/customers";

        public FormCustomerManagement()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void FormCustomerManagement_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            await LoadDataAsync();
        }

        private void SetupDataGridView()
        {
            dgvCustomers.AutoGenerateColumns = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;
        }

        public class CustomerDto
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public int RewardPoints { get; set; }
            public string MembershipRank { get; set; } = "Chuẩn";
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<CustomerDto>>(ApiBaseUrl);
                dgvCustomers.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCustomers.CurrentRow != null)
            {
                var row = dgvCustomers.CurrentRow;
                txtCustomerId.Text = row.Cells["CustomerId"].Value?.ToString();
                txtCustomerName.Text = row.Cells["CustomerName"].Value?.ToString();
                txtPhoneNumber.Text = row.Cells["PhoneNumber"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();
                txtRewardPoints.Text = row.Cells["RewardPoints"].Value?.ToString();
                cboMembershipRank.Text = row.Cells["MembershipRank"].Value?.ToString();
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) || string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Tên và Số điện thoại là bắt buộc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newCust = new CustomerDto
            {
                CustomerName = txtCustomerName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                RewardPoints = int.TryParse(txtRewardPoints.Text, out int points) ? points : 0,
                MembershipRank = string.IsNullOrWhiteSpace(cboMembershipRank.Text) ? "Chuẩn" : cboMembershipRank.Text
            };

            var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, newCust);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show("Thêm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var updateCust = new CustomerDto
            {
                CustomerId = id,
                CustomerName = txtCustomerName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                RewardPoints = int.TryParse(txtRewardPoints.Text, out int points) ? points : 0,
                MembershipRank = cboMembershipRank.Text
            };

            var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{id}", updateCust);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xóa khách hàng [ID: {id}]?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadDataAsync();
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchKey.Text.Trim();
            var list = await _httpClient.GetFromJsonAsync<List<CustomerDto>>($"{ApiBaseUrl}/search?keyword={Uri.EscapeDataString(keyword)}");
            dgvCustomers.DataSource = list;
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadDataAsync();
        }

        private void ClearInputs()
        {
            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtPhoneNumber.Clear();
            txtAddress.Clear();
            txtRewardPoints.Text = "0";
            cboMembershipRank.SelectedIndex = -1;
            txtSearchKey.Clear();
        }
    }
}