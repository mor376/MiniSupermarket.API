#nullable enable
namespace MiniSupermarket.WinForms
{   
    partial class FormMainShell
    {
        private System.ComponentModel.IContainer components = null!;

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
            this.panelUserProfile = new System.Windows.Forms.Panel();
            this.pictureAvatar = new System.Windows.Forms.PictureBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserRole = new System.Windows.Forms.Label();
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
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panelSidebar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelSidebar.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.panelSidebar.Controls.Add(this.btnUserManage);
            this.panelSidebar.Controls.Add(this.btnReports);
            this.panelSidebar.Controls.Add(this.btnCustomer);
            this.panelSidebar.Controls.Add(this.btnProduct);
            this.panelSidebar.Controls.Add(this.btnCategory);
            this.panelSidebar.Controls.Add(this.btnPOS);
            this.panelSidebar.Controls.Add(this.panelUserProfile);
            this.panelSidebar.Controls.Add(this.lblLogo);
            this.panelSidebar.Controls.Add(this.panelUserFooter);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(240, 820);
            this.panelSidebar.TabIndex = 0;

            // lblLogo
            this.lblLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblLogo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.lblLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblLogo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblLogo.Location = new System.Drawing.Point(0, 0);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(220, 62);
            this.lblLogo.TabIndex = 0;
            this.lblLogo.Text = "MiniSupermarket";
            this.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // panelUserProfile
            this.panelUserProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.panelUserProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUserProfile.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserProfile.Location = new System.Drawing.Point(0, 62);
            this.panelUserProfile.Name = "panelUserProfile";
            this.panelUserProfile.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.panelUserProfile.Size = new System.Drawing.Size(220, 82);
            this.panelUserProfile.TabIndex = 1;

            this.pictureAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.pictureAvatar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pictureAvatar.Image = null;
            this.pictureAvatar.Location = new System.Drawing.Point(14, 12);
            this.pictureAvatar.Name = "pictureAvatar";
            this.pictureAvatar.Size = new System.Drawing.Size(44, 44);
            this.pictureAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureAvatar.TabIndex = 0;
            this.pictureAvatar.TabStop = false;

            this.lblUserName.AutoSize = false;
            this.lblUserName.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.White;
            this.lblUserName.Location = new System.Drawing.Point(66, 12);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(130, 20);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "Người dùng";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblUserRole.AutoSize = false;
            this.lblUserRole.BackColor = System.Drawing.Color.Transparent;
            this.lblUserRole.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblUserRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.lblUserRole.Location = new System.Drawing.Point(66, 36);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(130, 18);
            this.lblUserRole.TabIndex = 2;
            this.lblUserRole.Text = "Quản trị";
            this.lblUserRole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.panelUserProfile.Controls.Add(this.lblUserRole);
            this.panelUserProfile.Controls.Add(this.lblUserName);
            this.panelUserProfile.Controls.Add(this.pictureAvatar);

            // Sidebar Buttons
            this.SetupButton(this.btnPOS, "🛒  Thu ngân (POS)", 150, this.btnPOS_Click);
            this.SetupButton(this.btnCategory, "📦  Quản lý Kho & Nhập hàng", 195, this.btnCategory_Click);
            this.SetupButton(this.btnProduct, "📁  Danh mục Sản phẩm & Giá", 240, this.btnProduct_Click);
            this.SetupButton(this.btnCustomer, "👥  Khách hàng", 285, this.btnCustomer_Click);
            this.SetupButton(this.btnReports, "📊  Báo cáo", 330, this.btnReports_Click);
            this.SetupButton(this.btnUserManage, "⚙  Cài đặt", 375, this.btnUserManage_Click);

            // panelUserFooter
            this.panelUserFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panelUserFooter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelUserFooter.Controls.Add(this.btnLogout);
            this.panelUserFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelUserFooter.Location = new System.Drawing.Point(0, 740);
            this.panelUserFooter.Name = "panelUserFooter";
            this.panelUserFooter.Size = new System.Drawing.Size(220, 60);
            this.panelUserFooter.TabIndex = 1;

            // btnLogout
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.btnLogout.Location = new System.Drawing.Point(0, 0);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(220, 60);
            this.btnLogout.TabIndex = 0;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnLogout.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.btnLogout.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            // panelTopHeader
            this.panelTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panelTopHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelTopHeader.Controls.Add(this.lblUserInfo);
            this.panelTopHeader.Controls.Add(this.lblTitle);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(240, 0);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(1160, 72);
            this.panelTopHeader.TabIndex = 1;
            this.panelTopHeader.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTitle.Location = new System.Drawing.Point(22, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(236, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "BẢNG TỔNG QUAN HỆ THỐNG";

            // lblUserInfo
            this.lblUserInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUserInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblUserInfo.Location = new System.Drawing.Point(692, 24);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(440, 25);
            this.lblUserInfo.TabIndex = 1;
            this.lblUserInfo.Text = "Xin chào: ...";
            this.lblUserInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // panelMainContent
            this.panelMainContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.panelMainContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContent.Location = new System.Drawing.Point(240, 72);
            this.panelMainContent.Name = "panelMainContent";
            this.panelMainContent.Size = new System.Drawing.Size(1160, 748);
            this.panelMainContent.TabIndex = 2;

            // FormMainShell
            this.ClientSize = new System.Drawing.Size(1400, 820);
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
            btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btn.ForeColor = System.Drawing.Color.White;
            btn.BackColor = System.Drawing.Color.FromArgb(24, 30, 48);
            btn.Location = new System.Drawing.Point(8, top);
            btn.Name = btn.Name;
            btn.Size = new System.Drawing.Size(204, 42);
            btn.TabIndex = 1;
            btn.Text = text;
            btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btn.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            btn.UseVisualStyleBackColor = false;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(30, 64, 175);
            btn.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            btn.Click += onClick;
        }


        #endregion

        private System.Windows.Forms.Panel panelSidebar = null!;
        private System.Windows.Forms.Label lblLogo = null!;
        private System.Windows.Forms.Panel panelUserProfile = null!;
        private System.Windows.Forms.PictureBox pictureAvatar = null!;
        private System.Windows.Forms.Label lblUserName = null!;
        private System.Windows.Forms.Label lblUserRole = null!;
        private System.Windows.Forms.Button btnPOS = null!;
        private System.Windows.Forms.Button btnCategory = null!;
        private System.Windows.Forms.Button btnProduct = null!;
        private System.Windows.Forms.Button btnCustomer = null!;
        private System.Windows.Forms.Button btnReports = null!;
        private System.Windows.Forms.Button btnUserManage = null!;
        private System.Windows.Forms.Panel panelUserFooter = null!;
        private System.Windows.Forms.Button btnLogout = null!;
        private System.Windows.Forms.Panel panelTopHeader = null!;
        private System.Windows.Forms.Label lblTitle = null!;
        private System.Windows.Forms.Label lblUserInfo = null!;
        private System.Windows.Forms.Panel panelMainContent = null!;
    }
}