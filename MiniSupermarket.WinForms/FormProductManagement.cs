using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSupermarket.WinForms.Models;

namespace MiniSupermarket.WinForms
{
    public partial class FormProductManagement : Form
    {
        private readonly HttpClient _httpClient;
        private const string ApiProductsUrl = "https://localhost:7035/api/products";
        private const string ApiCategoriesUrl = "https://localhost:7035/api/categories";

        public FormProductManagement()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void FormProductManagement_Load(object sender, EventArgs e)
        {
            await LoadCategoriesToComboAsync();
            await LoadProductsAsync();
        }

        public class CategoryComboDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }

        private async Task LoadCategoriesToComboAsync()
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryComboDto>>(ApiCategoriesUrl);
                cboCategory.DataSource = categories;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>(ApiProductsUrl);
                dgvProducts.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvProducts.CurrentRow != null)
            {
                var row = dgvProducts.CurrentRow;
                txtId.Text = row.Cells["ProductId"].Value?.ToString();
                txtBarcode.Text = row.Cells["Barcode"].Value?.ToString();
                txtProductName.Text = row.Cells["ProductName"].Value?.ToString();
                nudPrice.Value = Convert.ToDecimal(row.Cells["Price"].Value ?? 0);
                nudStock.Value = Convert.ToInt32(row.Cells["StockQuantity"].Value ?? 0);

                if (row.Cells["CategoryId"].Value != null)
                {
                    cboCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryId"].Value);
                }
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            var newProd = new
            {
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = (int)cboCategory.SelectedValue
            };

            var response = await _httpClient.PostAsJsonAsync(ApiProductsUrl, newProd);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Thêm mới sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadProductsAsync();
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id)) return;

            var updateProd = new
            {
                ProductId = id,
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = (int)cboCategory.SelectedValue
            };

            var response = await _httpClient.PutAsJsonAsync($"{ApiProductsUrl}/{id}", updateProd);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                await LoadProductsAsync();
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id)) return;

            if (MessageBox.Show($"Xóa sản phẩm [ID: {id}]?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var response = await _httpClient.DeleteAsync($"{ApiProductsUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadProductsAsync();
                }
            }
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadProductsAsync();
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStock.Value = 0;
            cboCategory.SelectedIndex = -1;
        }
    }
}