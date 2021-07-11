using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDSClientAppV1
{
    public partial class DataEntry : Form
    {
        private DataTable tbl;
        public DataEntry()
        {
            InitializeComponent();
            tbl = new DataTable();
            tbl.Columns.Add("FieldName", System.Type.GetType("System.String"));
            tbl.Columns.Add("Values", System.Type.GetType("System.String"));
            DataRow dr = tbl.NewRow();
            dr[0] = "Produces";
            tbl.Rows.Add(dr);
            dr = tbl.NewRow();
            dr[0] = "HasName";
            tbl.Rows.Add(dr);
            dr = tbl.NewRow();
            dr[0] = "HasDescription";
            tbl.Rows.Add(dr);
            dr = tbl.NewRow();
            dr[0] = "HasUrl";
            tbl.Rows.Add(dr);
            gridControl1.DataSource = tbl;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
