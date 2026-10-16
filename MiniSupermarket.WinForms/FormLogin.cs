using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class FormLogin : Form
    {
        private TextBox txtUser, txtPass;
        private Button btnLogin;

        public FormLogin()
        {
            Text = "Đăng nhập";
            Width = 400;
            Height = 220;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblUser = new Label { Text = "Tài khoản", Left = 20, Top = 20, Width = 80 };
            txtUser = new TextBox { Left = 120, Top = 20, Width = 220 };

            var lblPass = new Label { Text = "Mật khẩu", Left = 20, Top = 60, Width = 80 };
            txtPass = new TextBox { Left = 120, Top = 60, Width = 220, UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "Đăng nhập", Left = 120, Top = 100, Width = 120 };
            btnLogin.Click += async (s, e) => await BtnLogin_Click(s, e);

            Controls.Add(lblUser);
            Controls.Add(txtUser);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);
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
