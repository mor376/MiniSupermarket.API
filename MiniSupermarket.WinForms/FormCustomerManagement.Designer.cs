namespace MiniSupermarket.WinForms
{
    partial class FormCustomerManagement
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
            this.grpInfo = new System.Windows.Forms.GroupBox();
            this.lblCustomerId = new System.Windows.Forms.Label();
            this.txtCustomerId = new System.Windows.Forms.TextBox();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblRewardPoints = new System.Windows.Forms.Label();
            this.txtRewardPoints = new System.Windows.Forms.TextBox();
            this.lblMembershipRank = new System.Windows.Forms.Label();
            this.cboMembershipRank = new System.Windows.Forms.ComboBox();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.txtSearchKey = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();

            this.grpInfo.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.SuspendLayout();

            // grpInfo
            this.grpInfo.Controls.Add(this.lblCustomerId);
            this.grpInfo.Controls.Add(this.txtCustomerId);
            this.grpInfo.Controls.Add(this.lblCustomerName);
            this.grpInfo.Controls.Add(this.txtCustomerName);
            this.grpInfo.Controls.Add(this.lblPhoneNumber);
            this.grpInfo.Controls.Add(this.txtPhoneNumber);
            this.grpInfo.Controls.Add(this.lblAddress);
            this.grpInfo.Controls.Add(this.txtAddress);
            this.grpInfo.Controls.Add(this.lblRewardPoints);
            this.grpInfo.Controls.Add(this.txtRewardPoints);
            this.grpInfo.Controls.Add(this.lblMembershipRank);
            this.grpInfo.Controls.Add(this.cboMembershipRank);
            this.grpInfo.Location = new System.Drawing.Point(12, 12);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(360, 260);
            this.grpInfo.TabIndex = 0;
            this.grpInfo.TabStop = false;
            this.grpInfo.Text = "Thông Tin Khách Hàng";

            // Control Details inside grpInfo
            this.lblCustomerId.Location = new System.Drawing.Point(15, 30);
            this.lblCustomerId.Size = new System.Drawing.Size(100, 20);
            this.lblCustomerId.Text = "Mã KH:";
            this.txtCustomerId.Location = new System.Drawing.Point(120, 27);
            this.txtCustomerId.Size = new System.Drawing.Size(220, 22);
            this.txtCustomerId.ReadOnly = true;

            this.lblCustomerName.Location = new System.Drawing.Point(15, 65);
            this.lblCustomerName.Size = new System.Drawing.Size(100, 20);
            this.lblCustomerName.Text = "Tên KH (*):";
            this.txtCustomerName.Location = new System.Drawing.Point(120, 62);
            this.txtCustomerName.Size = new System.Drawing.Size(220, 22);

            this.lblPhoneNumber.Location = new System.Drawing.Point(15, 100);
            this.lblPhoneNumber.Size = new System.Drawing.Size(100, 20);
            this.lblPhoneNumber.Text = "Số điện thoại (*):";
            this.txtPhoneNumber.Location = new System.Drawing.Point(120, 97);
            this.txtPhoneNumber.Size = new System.Drawing.Size(220, 22);

            this.lblAddress.Location = new System.Drawing.Point(15, 135);
            this.lblAddress.Size = new System.Drawing.Size(100, 20);
            this.lblAddress.Text = "Địa chỉ:";
            this.txtAddress.Location = new System.Drawing.Point(120, 132);
            this.txtAddress.Size = new System.Drawing.Size(220, 22);

            this.lblRewardPoints.Location = new System.Drawing.Point(15, 170);
            this.lblRewardPoints.Size = new System.Drawing.Size(100, 20);
            this.lblRewardPoints.Text = "Điểm tích lũy:";
            this.txtRewardPoints.Location = new System.Drawing.Point(120, 167);
            this.txtRewardPoints.Size = new System.Drawing.Size(220, 22);
            this.txtRewardPoints.Text = "0";

            this.lblMembershipRank.Location = new System.Drawing.Point(15, 205);
            this.lblMembershipRank.Size = new System.Drawing.Size(100, 20);
            this.lblMembershipRank.Text = "Hạng thẻ:";
            this.cboMembershipRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMembershipRank.FormattingEnabled = true;
            this.cboMembershipRank.Items.AddRange(new object[] { "Chuẩn", "Bạc", "Vàng", "Kim Cương" });
            this.cboMembershipRank.Location = new System.Drawing.Point(120, 202);
            this.cboMembershipRank.Size = new System.Drawing.Size(220, 24);

            // grpSearch
            this.grpSearch.Controls.Add(this.txtSearchKey);
            this.grpSearch.Controls.Add(this.btnSearch);
            this.grpSearch.Controls.Add(this.btnLoad);
            this.grpSearch.Location = new System.Drawing.Point(385, 12);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(580, 65);
            this.grpSearch.TabIndex = 1;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Tìm Kiếm (Tên / SĐT)";

            this.txtSearchKey.Location = new System.Drawing.Point(15, 25);
            this.txtSearchKey.Size = new System.Drawing.Size(330, 22);

            this.btnSearch.Location = new System.Drawing.Point(360, 23);
            this.btnSearch.Size = new System.Drawing.Size(95, 27);
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.btnLoad.Location = new System.Drawing.Point(465, 23);
            this.btnLoad.Size = new System.Drawing.Size(95, 27);
            this.btnLoad.Text = "Tải lại";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);

            // pnlButtons
            this.pnlButtons.Controls.Add(this.btnAdd);
            this.pnlButtons.Controls.Add(this.btnUpdate);
            this.pnlButtons.Controls.Add(this.btnDelete);
            this.pnlButtons.Location = new System.Drawing.Point(12, 280);
            this.pnlButtons.Size = new System.Drawing.Size(360, 45);

            this.btnAdd.Location = new System.Drawing.Point(10, 8);
            this.btnAdd.Size = new System.Drawing.Size(100, 30);
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            this.btnUpdate.Location = new System.Drawing.Point(130, 8);
            this.btnUpdate.Size = new System.Drawing.Size(100, 30);
            this.btnUpdate.Text = "Cập nhật";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);

            this.btnDelete.Location = new System.Drawing.Point(250, 8);
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // dgvCustomers
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            this.dgvCustomers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomers.Location = new System.Drawing.Point(385, 85);
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.RowHeadersWidth = 51;
            this.dgvCustomers.Size = new System.Drawing.Size(580, 240);
            this.dgvCustomers.TabIndex = 2;
            this.dgvCustomers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomers_CellClick);

            // FormCustomerManagement
            this.ClientSize = new System.Drawing.Size(980, 340);
            this.Controls.Add(this.dgvCustomers);
            this.Controls.Add(this.grpSearch);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.grpInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormCustomerManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản Lý Khách Hàng Thành Viên";
            this.Load += new System.EventHandler(this.FormCustomerManagement_Load);

            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpInfo;
        private System.Windows.Forms.Label lblCustomerId;
        private System.Windows.Forms.TextBox txtCustomerId;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblPhoneNumber;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblRewardPoints;
        private System.Windows.Forms.TextBox txtRewardPoints;
        private System.Windows.Forms.Label lblMembershipRank;
        private System.Windows.Forms.ComboBox cboMembershipRank;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.TextBox txtSearchKey;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridView dgvCustomers;
    }
}