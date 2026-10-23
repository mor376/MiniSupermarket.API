using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class FormLogin : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;

        public FormLogin()
        {
            Text = "Đăng nhập hệ thống";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);
            Size = new Size(680, 420);
            MinimumSize = new Size(680, 420);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 52, 90)
            };
            var title = new Label
            {
                Text = "Cửa hàng Bách hóa Xanh An Phước",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            header.Controls.Add(title);

            var body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 22, 40, 20),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            var formTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(0, 8, 0, 0)
            };
            formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            formTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            formTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            formTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            formTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

            var lblUser = new Label
            {
                Text = "Tài khoản",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };

            txtUser = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Width = 330,
                Height = 36,
                Font = new Font("Segoe UI", 11F)
            };

            var lblPass = new Label
            {
                Text = "Mật khẩu",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };

            txtPass = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Width = 330,
                Height = 36,
                Font = new Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };

            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Anchor = AnchorStyles.Left,
                Width = 330,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += async (s, e) => await BtnLogin_Click(s, e);
            AcceptButton = btnLogin;

            var hint = new Label
            {
                Text = "Demo: admin01 / cashier01 / ware01 · mật khẩu 123456",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = Color.FromArgb(107, 114, 128),
                Font = new Font("Segoe UI", 10F)
            };

            formTable.Controls.Add(lblUser, 0, 0);
            formTable.Controls.Add(txtUser, 1, 0);
            formTable.Controls.Add(lblPass, 0, 1);
            formTable.Controls.Add(txtPass, 1, 1);
            formTable.Controls.Add(btnLogin, 1, 2);
            formTable.Controls.Add(hint, 1, 3);

            body.Controls.Add(formTable);
            mainLayout.Controls.Add(header, 0, 0);
            mainLayout.Controls.Add(body, 0, 1);

            Controls.Add(mainLayout);
        }

        private async Task BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var ok = await ApiClientService.LoginAsync(username, password);
                if (ok)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối đến Server: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}