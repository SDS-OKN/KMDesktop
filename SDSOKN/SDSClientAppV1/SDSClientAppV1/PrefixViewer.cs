using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Commands;
using RestSharp;

namespace SDSClientAppV1
{
    public partial class PrefixViewer : Form
    {
        public DataTable dt;
        public string instanceID;
        public string urlAddress; 

        public PrefixViewer()
        {
            InitializeComponent();
            dt = new DataTable();
            dt.Columns.Add(new DataColumn("prefix", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("namespace", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("isActive", Type.GetType("System.Boolean")));
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            grdNamespaces.ShowPrintPreview();
        }

        private void btnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string path = string.Empty;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".xlsx";
            sfd.OverwritePrompt = true;
            sfd.Filter = "Excel|*.xlsx";
            grdNamespaces.ExportToXlsx(path);
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnSaveChanges_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var request1 = new RestRequest();
            var writer = new StringWriter();
            if (string.IsNullOrEmpty(dt.TableName))
            {
                dt.TableName = "rSchema";
            }
            dt.WriteXml(writer, XmlWriteMode.WriteSchema, true);
            
    
           string datatable = Compressor.Compress(writer.ToString());
           var client1 = new RestClient(urlAddress + "api/prefixes/save/id/" + instanceID + "/" + HttpUtility.UrlEncode(datatable));
           var response1 = client1.Get(request1);
                        
        }

        private void PrefixViewer_Shown(object sender, EventArgs e)
        {
            grdNamespaces.DataSource = dt; 
        }
    }
}
