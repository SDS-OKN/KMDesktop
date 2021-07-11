namespace SDSClientAppV1
{
    partial class PrefixViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrefixViewer));
            this.rbLocals = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rbPrefixes = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.stackPanel1 = new DevExpress.Utils.Layout.StackPanel();
            this.grdNamespaces = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSaveChanges = new DevExpress.XtraBars.BarButtonItem();
            this.btnClose = new DevExpress.XtraBars.BarButtonItem();
            this.btnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.btnExport = new DevExpress.XtraBars.BarButtonItem();
            this.prefix = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prefixnamespace = new DevExpress.XtraGrid.Columns.GridColumn();
            this.IsActive = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLocals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stackPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdNamespaces)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // rbLocals
            // 
            this.rbLocals.ExpandCollapseItem.Id = 0;
            this.rbLocals.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.rbLocals.ExpandCollapseItem,
            this.btnSaveChanges,
            this.btnClose,
            this.btnPrint,
            this.btnExport});
            this.rbLocals.Location = new System.Drawing.Point(0, 0);
            this.rbLocals.MaxItemId = 5;
            this.rbLocals.Name = "rbLocals";
            this.rbLocals.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.rbPrefixes});
            this.rbLocals.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.OfficeUniversal;
            // 
            // 
            // 
            this.rbLocals.SearchEditItem.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.rbLocals.SearchEditItem.EditWidth = 150;
            this.rbLocals.SearchEditItem.Id = -5000;
            this.rbLocals.SearchEditItem.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.rbLocals.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.rbLocals.Size = new System.Drawing.Size(800, 58);
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnPrint);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnExport);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSaveChanges);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnClose);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // rbPrefixes
            // 
            this.rbPrefixes.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.rbPrefixes.Name = "rbPrefixes";
            this.rbPrefixes.Text = "Prefix Options";
            // 
            // stackPanel1
            // 
            this.stackPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stackPanel1.Location = new System.Drawing.Point(0, 437);
            this.stackPanel1.Name = "stackPanel1";
            this.stackPanel1.Size = new System.Drawing.Size(800, 13);
            this.stackPanel1.TabIndex = 1;
            // 
            // grdNamespaces
            // 
            this.grdNamespaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdNamespaces.Location = new System.Drawing.Point(0, 58);
            this.grdNamespaces.MainView = this.gridView1;
            this.grdNamespaces.Name = "grdNamespaces";
            this.grdNamespaces.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.grdNamespaces.Size = new System.Drawing.Size(800, 379);
            this.grdNamespaces.TabIndex = 2;
            this.grdNamespaces.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.prefix,
            this.prefixnamespace,
            this.IsActive});
            this.gridView1.GridControl = this.grdNamespaces;
            this.gridView1.Name = "gridView1";
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Caption = "Save Changes";
            this.btnSaveChanges.Id = 1;
            this.btnSaveChanges.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSaveChanges.ImageOptions.SvgImage")));
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSaveChanges_ItemClick);
            // 
            // btnClose
            // 
            this.btnClose.Caption = "Close";
            this.btnClose.Id = 2;
            this.btnClose.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.ImageOptions.SvgImage")));
            this.btnClose.Name = "btnClose";
            this.btnClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnClose_ItemClick);
            // 
            // btnPrint
            // 
            this.btnPrint.Caption = "Print";
            this.btnPrint.Id = 3;
            this.btnPrint.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnPrint.ImageOptions.SvgImage")));
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
            this.btnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnPrint_ItemClick);
            // 
            // btnExport
            // 
            this.btnExport.Caption = "Export";
            this.btnExport.Id = 4;
            this.btnExport.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnExport.ImageOptions.SvgImage")));
            this.btnExport.Name = "btnExport";
            this.btnExport.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
            this.btnExport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExport_ItemClick);
            // 
            // prefix
            // 
            this.prefix.Caption = "Prefix";
            this.prefix.FieldName = "prefix";
            this.prefix.Name = "prefix";
            this.prefix.Visible = true;
            this.prefix.VisibleIndex = 0;
            this.prefix.Width = 519;
            // 
            // prefixnamespace
            // 
            this.prefixnamespace.Caption = "Namespace";
            this.prefixnamespace.FieldName = "namespace";
            this.prefixnamespace.Name = "prefixnamespace";
            this.prefixnamespace.Visible = true;
            this.prefixnamespace.VisibleIndex = 1;
            this.prefixnamespace.Width = 350;
            // 
            // IsActive
            // 
            this.IsActive.Caption = "Active?";
            this.IsActive.ColumnEdit = this.repositoryItemCheckEdit1;
            this.IsActive.FieldName = "isActive";
            this.IsActive.Name = "IsActive";
            this.IsActive.Visible = true;
            this.IsActive.VisibleIndex = 2;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // PrefixViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.grdNamespaces);
            this.Controls.Add(this.stackPanel1);
            this.Controls.Add(this.rbLocals);
            this.Name = "PrefixViewer";
            this.Text = "Prefix/Namespace Viewer";
            this.Shown += new System.EventHandler(this.PrefixViewer_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.rbLocals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stackPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdNamespaces)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl rbLocals;
        private DevExpress.XtraBars.BarButtonItem btnSaveChanges;
        private DevExpress.XtraBars.BarButtonItem btnClose;
        private DevExpress.XtraBars.BarButtonItem btnPrint;
        private DevExpress.XtraBars.BarButtonItem btnExport;
        private DevExpress.XtraBars.Ribbon.RibbonPage rbPrefixes;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.Utils.Layout.StackPanel stackPanel1;
        private DevExpress.XtraGrid.GridControl grdNamespaces;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn prefix;
        private DevExpress.XtraGrid.Columns.GridColumn prefixnamespace;
        private DevExpress.XtraGrid.Columns.GridColumn IsActive;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
    }
}