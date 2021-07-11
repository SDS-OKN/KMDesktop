namespace SDSClientAppV1
{
    partial class SpecifyNewAllegroGraph
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.formAssistant1 = new DevExpress.XtraBars.FormAssistant();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.btnRegisterAG = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnLstCatalogs = new DevExpress.XtraEditors.SimpleButton();
            this.btnListRepositories = new DevExpress.XtraEditors.SimpleButton();
            this.btnVerifyConnection = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new System.Windows.Forms.MaskedTextBox();
            this.txtAgInstanceName = new DevExpress.XtraEditors.TextEdit();
            this.txtServerUrl = new DevExpress.XtraEditors.TextEdit();
            this.txtPortNumber = new DevExpress.XtraEditors.TextEdit();
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.txtCatalog = new DevExpress.XtraEditors.TextEdit();
            this.txtRepository = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgInstanceName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPortNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCatalog.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepository.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 43);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(163, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "AllegroGraph Instance Name";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(112, 88);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(63, 16);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Server Url:";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(142, 125);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(23, 16);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Port";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(102, 160);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(63, 16);
            this.labelControl4.TabIndex = 3;
            this.labelControl4.Text = "User Name";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(110, 197);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(55, 16);
            this.labelControl5.TabIndex = 4;
            this.labelControl5.Text = "Password";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(122, 237);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(43, 16);
            this.labelControl6.TabIndex = 5;
            this.labelControl6.Text = "Catalog";
            this.labelControl6.Click += new System.EventHandler(this.labelControl6_Click);
            // 
            // btnRegisterAG
            // 
            this.btnRegisterAG.Location = new System.Drawing.Point(119, 376);
            this.btnRegisterAG.Name = "btnRegisterAG";
            this.btnRegisterAG.Size = new System.Drawing.Size(165, 56);
            this.btnRegisterAG.TabIndex = 6;
            this.btnRegisterAG.Text = "Register AllegroGraph\r\nInstance";
            this.btnRegisterAG.Click += new System.EventHandler(this.btnRegisterAG_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(336, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(165, 56);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            // 
            // btnLstCatalogs
            // 
            this.btnLstCatalogs.Location = new System.Drawing.Point(530, 237);
            this.btnLstCatalogs.Name = "btnLstCatalogs";
            this.btnLstCatalogs.Size = new System.Drawing.Size(113, 26);
            this.btnLstCatalogs.TabIndex = 8;
            this.btnLstCatalogs.Text = "Lookup Catalogs";
            this.btnLstCatalogs.Click += new System.EventHandler(this.btnLstCatalogs_Click);
            // 
            // btnListRepositories
            // 
            this.btnListRepositories.Location = new System.Drawing.Point(530, 276);
            this.btnListRepositories.Name = "btnListRepositories";
            this.btnListRepositories.Size = new System.Drawing.Size(113, 26);
            this.btnListRepositories.TabIndex = 9;
            this.btnListRepositories.Text = "List Repositories";
            this.btnListRepositories.Click += new System.EventHandler(this.btnListRepositories_Click);
            // 
            // btnVerifyConnection
            // 
            this.btnVerifyConnection.Location = new System.Drawing.Point(216, 325);
            this.btnVerifyConnection.Name = "btnVerifyConnection";
            this.btnVerifyConnection.Size = new System.Drawing.Size(223, 26);
            this.btnVerifyConnection.TabIndex = 10;
            this.btnVerifyConnection.Text = "Verify Connection";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(110, 276);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(60, 16);
            this.labelControl7.TabIndex = 11;
            this.labelControl7.Text = "Repository";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(195, 197);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(399, 26);
            this.txtPassword.TabIndex = 12;
            // 
            // txtAgInstanceName
            // 
            this.txtAgInstanceName.Location = new System.Drawing.Point(195, 49);
            this.txtAgInstanceName.Name = "txtAgInstanceName";
            this.txtAgInstanceName.Size = new System.Drawing.Size(399, 22);
            this.txtAgInstanceName.TabIndex = 13;
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Location = new System.Drawing.Point(195, 85);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(399, 22);
            this.txtServerUrl.TabIndex = 14;
            // 
            // txtPortNumber
            // 
            this.txtPortNumber.Location = new System.Drawing.Point(195, 122);
            this.txtPortNumber.Name = "txtPortNumber";
            this.txtPortNumber.Size = new System.Drawing.Size(94, 22);
            this.txtPortNumber.TabIndex = 15;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(195, 157);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(399, 22);
            this.txtUserName.TabIndex = 16;
            // 
            // txtCatalog
            // 
            this.txtCatalog.Location = new System.Drawing.Point(195, 239);
            this.txtCatalog.Name = "txtCatalog";
            this.txtCatalog.Size = new System.Drawing.Size(329, 22);
            this.txtCatalog.TabIndex = 17;
            // 
            // txtRepository
            // 
            this.txtRepository.Location = new System.Drawing.Point(195, 278);
            this.txtRepository.Name = "txtRepository";
            this.txtRepository.Size = new System.Drawing.Size(329, 22);
            this.txtRepository.TabIndex = 18;
            // 
            // SpecifyNewAllegroGraph
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 446);
            this.Controls.Add(this.txtRepository);
            this.Controls.Add(this.txtCatalog);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtPortNumber);
            this.Controls.Add(this.txtServerUrl);
            this.Controls.Add(this.txtAgInstanceName);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.btnVerifyConnection);
            this.Controls.Add(this.btnListRepositories);
            this.Controls.Add(this.btnLstCatalogs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRegisterAG);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SpecifyNewAllegroGraph";
            this.Text = "Create New AllegroGraph Connection";
            ((System.ComponentModel.ISupportInitialize)(this.txtAgInstanceName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPortNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCatalog.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepository.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.FormAssistant formAssistant1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.SimpleButton btnRegisterAG;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnLstCatalogs;
        private DevExpress.XtraEditors.SimpleButton btnListRepositories;
        private DevExpress.XtraEditors.SimpleButton btnVerifyConnection;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        public System.Windows.Forms.MaskedTextBox txtPassword;
        public DevExpress.XtraEditors.TextEdit txtAgInstanceName;
        public DevExpress.XtraEditors.TextEdit txtServerUrl;
        public DevExpress.XtraEditors.TextEdit txtPortNumber;
        public DevExpress.XtraEditors.TextEdit txtUserName;
        public DevExpress.XtraEditors.TextEdit txtCatalog;
        public DevExpress.XtraEditors.TextEdit txtRepository;
    }
}