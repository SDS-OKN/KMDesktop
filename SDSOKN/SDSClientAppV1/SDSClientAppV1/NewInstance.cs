using System;
using System.Linq;
using System.Windows.Forms;

namespace SDSClientAppV1
{
    public partial class NewInstance : DevExpress.XtraEditors.XtraForm
    {
        public NewInstance()
        {
            InitializeComponent();
        }

        private void btnSaveNewInstance_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}