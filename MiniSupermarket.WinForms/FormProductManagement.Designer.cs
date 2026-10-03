namespace MiniSupermarket.WinForms
{
    partial class FormProductManagement
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
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.grpInfo = new System.Windows.Forms.GroupBox();
            this.lblId = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblProductName = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.nudPrice = new System.Windows.Forms.NumericUpDown();
            this.lblStock = new System.Windows.Forms.Label();
            this.nudStock = new System.Windows.Forms.NumericUpDown();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.grpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStock)).BeginInit();
            this.SuspendLayout();

            // dgvProducts
            this.dgvProducts.AllowUserToAddRows = false;
            this.dgvProducts.AllowUserToDeleteRows = false;
            this.dgvProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProducts.Location = new System.Drawing.Point(12, 12);
            this.dgvProducts.Name = "dgvProducts";
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.Size = new System.Drawing.Size(640, 600);
            this.dgvProducts.TabIndex = 0;
            this.dgvProducts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProducts_CellClick);

            // grpInfo
            this.grpInfo.Controls.Add(this.lblId);
            this.grpInfo.Controls.Add(this.txtId);
            this.grpInfo.Controls.Add(this.lblBarcode);
            this.grpInfo.Controls.Add(this.txtBarcode);
            this.grpInfo.Controls.Add(this.lblProductName);
            this.grpInfo.Controls.Add(this.txtProductName);
            this.grpInfo.Controls.Add(this.lblPrice);
            this.grpInfo.Controls.Add(this.nudPrice);
            this.grpInfo.Controls.Add(this.lblStock);
            this.grpInfo.Controls.Add(this.nudStock);
            this.grpInfo.Controls.Add(this.lblCategory);
            this.grpInfo.Controls.Add(this.cboCategory);
            this.grpInfo.Controls.Add(this.btnAdd);
            this.grpInfo.Controls.Add(this.btnUpdate);
            this.grpInfo.Controls.Add(this.btnDelete);
            this.grpInfo.Controls.Add(this.btnLoad);
            this.grpInfo.Location = new System.Drawing.Point(665, 12);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(350, 600);
            this.grpInfo.TabIndex = 1;
            this.grpInfo.TabStop = false;
            this.grpInfo.Text = "Chi Tiết Sản Phẩm";

            // Controls inside grpInfo
            this.lblId.Location = new System.Drawing.Point(15, 35);
            this.lblId.Text = "Mã SP:";
            this.txtId.Location = new System.Drawing.Point(110, 32);
            this.txtId.Size = new System.Drawing.Size(220, 22);
            this.txtId.ReadOnly = true;

            this.lblBarcode.Location = new System.Drawing.Point(15, 75);
            this.lblBarcode.Text = "Mã vạch:";
            this.txtBarcode.Location = new System.Drawing.Point(110, 72);
            this.txtBarcode.Size = new System.Drawing.Size(220, 22);

            this.lblProductName.Location = new System.Drawing.Point(15, 115);
            this.lblProductName.Text = "Tên SP:";
            this.txtProductName.Location = new System.Drawing.Point(110, 112);
            this.txtProductName.Size = new System.Drawing.Size(220, 22);

            this.lblPrice.Location = new System.Drawing.Point(15, 155);
            this.lblPrice.Text = "Đơn giá:";
            this.nudPrice.Location = new System.Drawing.Point(110, 152);
            this.nudPrice.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            this.nudPrice.Size = new System.Drawing.Size(220, 22);

            this.lblStock.Location = new System.Drawing.Point(15, 195);
            this.lblStock.Text = "Tồn kho:";
            this.nudStock.Location = new System.Drawing.Point(110, 192);
            this.nudStock.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudStock.Size = new System.Drawing.Size(220, 22);

            this.lblCategory.Location = new System.Drawing.Point(15, 235);
            this.lblCategory.Text = "Danh mục:";
            this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategory.Location = new System.Drawing.Point(110, 232);
            this.cboCategory.Size = new System.Drawing.Size(220, 24);

            // Buttons inside grpInfo
            this.btnAdd.Location = new System.Drawing.Point(15, 290);
            this.btnAdd.Size = new System.Drawing.Size(150, 35);
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            this.btnUpdate.Location = new System.Drawing.Point(180, 290);
            this.btnUpdate.Size = new System.Drawing.Size(150, 35);
            this.btnUpdate.Text = "Cập nhật";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);

            this.btnDelete.Location = new System.Drawing.Point(15, 340);
            this.btnDelete.Size = new System.Drawing.Size(150, 35);
            this.btnDelete.Text = "Xóa";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.btnLoad.Location = new System.Drawing.Point(180, 340);
            this.btnLoad.Size = new System.Drawing.Size(150, 35);
            this.btnLoad.Text = "Tải lại";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);

            // FormProductManagement
            this.ClientSize = new System.Drawing.Size(1030, 630);
            this.Controls.Add(this.grpInfo);
            this.Controls.Add(this.dgvProducts);
            this.Name = "FormProductManagement";
            this.Text = "Quản Lý Sản Phẩm & Kho Hàng";
            this.Load += new System.EventHandler(this.FormProductManagement_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStock)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.GroupBox grpInfo;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.NumericUpDown nudPrice;
        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.NumericUpDown nudStock;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnLoad;
    }
}