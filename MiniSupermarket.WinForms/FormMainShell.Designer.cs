#nullable enable
namespace MiniSupermarket.WinForms
{   
    partial class FormMainShell
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.lblLogo = new System.Windows.Forms.Label();
            this.btnPOS = new System.Windows.Forms.Button();
            this.btnCategory = new System.Windows.Forms.Button();
            this.btnProduct = new System.Windows.Forms.Button();
            this.btnCustomer = new System.Windows.Forms.Button();
            this.btnReports = new System.Windows.Forms.Button();
            this.btnUserManage = new System.Windows.Forms.Button();
            this.panelUserFooter = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.panelMainContent = new System.Windows.Forms.Panel();

            this.panelSidebar.SuspendLayout();
            this.panelUserFooter.SuspendLayout();
            this.panelTopHeader.SuspendLayout();
            this.SuspendLayout();

            // panelSidebar
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(30)))), ((int)(((byte)(48)))));
            this.panelSidebar.Controls.Add(this.btnUserManage);
            this.panelSidebar.Controls.Add(this.btnReports);
            this.panelSidebar.Controls.Add(this.btnCustomer);
            this.panelSidebar.Controls.Add(this.btnProduct);
            this.panelSidebar.Controls.Add(this.btnCategory);
            this.panelSidebar.Controls.Add(this.btnPOS);
            this.panelSidebar.Controls.Add(this.lblLogo);
            this.panelSidebar.Controls.Add(this.panelUserFooter);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(230, 720);
            this.panelSidebar.TabIndex = 0;

            // lblLogo
            this.lblLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblLogo.ForeColor = System.Drawing.Color.White;
            this.lblLogo.Location = new System.Drawing.Point(0, 0);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(230, 60);
            this.lblLogo.TabIndex = 0;
            this.lblLogo.Text = "🛒  MiniMart An Phước";
            this.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Sidebar Buttons
            this.SetupButton(this.btnPOS, "🛒  Bán hàng (POS)", 60, this.btnPOS_Click);
            this.SetupButton(this.btnCategory, "📁  Danh mục hàng hóa", 105, this.btnCategory_Click);
            this.SetupButton(this.btnProduct, "📦  Sản phẩm & Giá", 150, this.btnProduct_Click);
            this.SetupButton(this.btnCustomer, "👥  Quản lý Khách hàng", 195, this.btnCustomer_Click);
            this.SetupButton(this.btnReports, "📊  Báo cáo Doanh thu", 240, this.btnReports_Click);
            this.SetupButton(this.btnUserManage, "⚙  Cài đặt & Phân quyền", 285, this.btnUserManage_Click);

            // panelUserFooter
            this.panelUserFooter.Controls.Add(this.btnLogout);
            this.panelUserFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelUserFooter.Location = new System.Drawing.Point(0, 660);
            this.panelUserFooter.Name = "panelUserFooter";
            this.panelUserFooter.Size = new System.Drawing.Size(230, 60);
            this.panelUserFooter.TabIndex = 1;

            // btnLogout
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnLogout.Location = new System.Drawing.Point(0, 0);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(230, 60);
            this.btnLogout.TabIndex = 0;
            this.btnLogout.Text = "🚪  Đăng xuất";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            // panelTopHeader
            this.panelTopHeader.BackColor = System.Drawing.Color.White;
            this.panelTopHeader.Controls.Add(this.lblUserInfo);
            this.panelTopHeader.Controls.Add(this.lblTitle);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(230, 0);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(1050, 60);
            this.panelTopHeader.TabIndex = 1;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(30)))), ((int)(((byte)(48)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(235, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "BÀN LÀM VIỆC HỆ THỐNG";

            // lblUserInfo
            this.lblUserInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUserInfo.ForeColor = System.Drawing.Color.DimGray;
            this.lblUserInfo.Location = new System.Drawing.Point(600, 18);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(430, 25);
            this.lblUserInfo.TabIndex = 1;
            this.lblUserInfo.Text = "Xin chào: ...";
            this.lblUserInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // panelMainContent
            this.panelMainContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.panelMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContent.Location = new System.Drawing.Point(230, 60);
            this.panelMainContent.Name = "panelMainContent";
            this.panelMainContent.Size = new System.Drawing.Size(1050, 660);
            this.panelMainContent.TabIndex = 2;

            // FormMainShell
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.panelMainContent);
            this.Controls.Add(this.panelTopHeader);
            this.Controls.Add(this.panelSidebar);
            this.Name = "FormMainShell";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hệ thống Quản lý Bán lẻ & Tồn kho Siêu thị Mini";
            this.Load += new System.EventHandler(this.FormMainShell_Load);

            this.panelSidebar.ResumeLayout(false);
            this.panelUserFooter.ResumeLayout(false);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.ResumeLayout(false);
        }

        private void SetupButton(System.Windows.Forms.Button btn, string text, int top, System.EventHandler onClick)
        {
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            btn.ForeColor = System.Drawing.Color.White;
            btn.Location = new System.Drawing.Point(0, top);
            btn.Name = btn.Name;
            btn.Size = new System.Drawing.Size(230, 45);
            btn.TabIndex = 1;
            btn.Text = text;
            btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btn.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            btn.UseVisualStyleBackColor = true;
            btn.Click += onClick;
        }


        #endregion

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Button btnPOS;
        private System.Windows.Forms.Button btnCategory;
        private System.Windows.Forms.Button btnProduct;
        private System.Windows.Forms.Button btnCustomer;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.Button btnUserManage;
        private System.Windows.Forms.Panel panelUserFooter;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Panel panelMainContent;
    }
}