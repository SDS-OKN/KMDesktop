namespace SDSClientAppV1
{
    partial class NewInstance
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
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnSaveNewInstance = new DevExpress.XtraBars.BarButtonItem();
            this.btnClose = new DevExpress.XtraBars.BarButtonItem();
            this.rbEdit = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.grdNewItem = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.propertyName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Values = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
            this.repositoryItemRichTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdNewItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoExEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichTextEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.btnSaveNewInstance,
            this.btnClose});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.ribbonControl1.MaxItemId = 3;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.rbEdit});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.OfficeUniversal;
            this.ribbonControl1.Size = new System.Drawing.Size(621, 58);
            // 
            // btnSaveNewInstance
            // 
            this.btnSaveNewInstance.Caption = "Save and Close";
            this.btnSaveNewInstance.Id = 1;
            this.btnSaveNewInstance.Name = "btnSaveNewInstance";
            this.btnSaveNewInstance.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSaveNewInstance_ItemClick);
            // 
            // btnClose
            // 
            this.btnClose.Caption = "Cancel";
            this.btnClose.Id = 2;
            this.btnClose.Name = "btnClose";
            this.btnClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnClose_ItemClick);
            // 
            // rbEdit
            // 
            this.rbEdit.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.rbEdit.Name = "rbEdit";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSaveNewInstance);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnClose);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // grdNewItem
            // 
            this.grdNewItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdNewItem.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grdNewItem.Location = new System.Drawing.Point(0, 58);
            this.grdNewItem.MainView = this.gridView1;
            this.grdNewItem.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grdNewItem.Name = "grdNewItem";
            this.grdNewItem.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoExEdit1,
            this.repositoryItemRichTextEdit1});
            this.grdNewItem.Size = new System.Drawing.Size(621, 366);
            this.grdNewItem.TabIndex = 1;
            this.grdNewItem.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.propertyName,
            this.Values});
            this.gridView1.DetailHeight = 295;
            this.gridView1.GridControl = this.grdNewItem;
            this.gridView1.Name = "gridView1";
            // 
            // propertyName
            // 
            this.propertyName.Caption = "Property Name";
            this.propertyName.FieldName = "propertyName";
            this.propertyName.MinWidth = 16;
            this.propertyName.Name = "propertyName";
            this.propertyName.Visible = true;
            this.propertyName.VisibleIndex = 0;
            this.propertyName.Width = 58;
            // 
            // Values
            // 
            this.Values.Caption = "Values";
            this.Values.ColumnEdit = this.repositoryItemMemoExEdit1;
            this.Values.FieldName = "values";
            this.Values.MinWidth = 16;
            this.Values.Name = "Values";
            this.Values.Visible = true;
            this.Values.VisibleIndex = 1;
            this.Values.Width = 58;
            // 
            // repositoryItemMemoExEdit1
            // 
            this.repositoryItemMemoExEdit1.AutoHeight = false;
            this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
            // 
            // repositoryItemRichTextEdit1
            // 
            this.repositoryItemRichTextEdit1.Name = "repositoryItemRichTextEdit1";
            this.repositoryItemRichTextEdit1.OptionsBehavior.Printing = DevExpress.XtraRichEdit.DocumentCapability.Enabled;
            this.repositoryItemRichTextEdit1.OptionsBehavior.SaveAs = DevExpress.XtraRichEdit.DocumentCapability.Default;
            this.repositoryItemRichTextEdit1.ShowCaretInReadOnly = false;
            // 
            // NewInstance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 424);
            this.Controls.Add(this.grdNewItem);
            this.Controls.Add(this.ribbonControl1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "NewInstance";
            this.Text = "NewInstance";
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdNewItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoExEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichTextEdit1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage rbEdit;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem btnSaveNewInstance;
        private DevExpress.XtraBars.BarButtonItem btnClose;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn propertyName;
        private DevExpress.XtraGrid.Columns.GridColumn Values;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit repositoryItemMemoExEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit repositoryItemRichTextEdit1;
        public DevExpress.XtraGrid.GridControl grdNewItem;
    }
}