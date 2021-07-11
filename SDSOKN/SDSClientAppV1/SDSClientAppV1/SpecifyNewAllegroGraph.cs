using RestSharp;
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
    public partial class SpecifyNewAllegroGraph : DevExpress.XtraEditors.XtraForm
    {
        public SpecifyNewAllegroGraph()
        {
            InitializeComponent();
        }

        private void labelControl6_Click(object sender, EventArgs e)
        {

        }

        private void btnLstCatalogs_Click(object sender, EventArgs e)
        {
            ListItems li = new ListItems();
            li.labelDescription.Text = "Select Catalog from list:";
            li.cmbItemsToView.Properties.Items.Clear();
            var Initclient = new RestSharp.RestClient("http://" + txtServerUrl.Text + ":" + txtPortNumber.Text + "/agCaller/api/ListRepositories/catalogs/"+txtServerUrl.Text + "/" + txtPortNumber.Text + "/" + txtUserName.Text + "/" + txtPassword.Text);
          
          
            var request = new RestRequest();
            var response1 = Initclient.Get(request);
            string[] args = { Environment.NewLine };
            string[] items = response1.Content.Split(args, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in items)
            {
                li.cmbItemsToView.Properties.Items.Add(item);
            }
            if (li.cmbItemsToView.Properties.Items.Count > 0)
            {
                li.cmbItemsToView.SelectedIndex = 0;
            }
            if (li.ShowDialog() == DialogResult.OK)
            {
                txtCatalog.Text = li.cmbItemsToView.SelectedText;
            }

        }

        private void btnListRepositories_Click(object sender, EventArgs e)
        {
            ListItems li = new ListItems();
            li.labelDescription.Text = "Select Repository from list:";
            li.cmbItemsToView.Properties.Items.Clear();
            var Initclient = new RestSharp.RestClient("http://" + txtServerUrl.Text + ":" + txtPortNumber.Text + "/agCaller/api/ListRepositories/repos/" + txtServerUrl.Text + "/" + txtPortNumber.Text + "/" + txtUserName.Text + "/" + txtPassword.Text+"/" + txtCatalog.Text);


            var request = new RestRequest();
            var response1 = Initclient.Get(request);
            string[] args = { Environment.NewLine };
            string[] items = response1.Content.Split(args,StringSplitOptions.RemoveEmptyEntries);
            foreach(string item in items)
            {
                li.cmbItemsToView.Properties.Items.Add(item);
            }
            if(li.cmbItemsToView.Properties.Items.Count > 0)
            {
                li.cmbItemsToView.SelectedIndex = 0;
            }
            if(li.ShowDialog() == DialogResult.OK)
            {
                txtRepository.Text = li.cmbItemsToView.SelectedText;
            }

        }

        private void btnRegisterAG_Click(object sender, EventArgs e)
        {
            Microsoft.Data.Sqlite.SqliteConnectionStringBuilder connectionStringBuilder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "registeredinfo.sqlite";
            Microsoft.Data.Sqlite.SqliteConnection sqliteConnection = new Microsoft.Data.Sqlite.SqliteConnection(connectionStringBuilder.ConnectionString);
            sqliteConnection.Open();
            try
            {
                Microsoft.Data.Sqlite.SqliteCommand command = new Microsoft.Data.Sqlite.SqliteCommand();
                command.Connection = sqliteConnection;

                command.CommandText = "Insert into RegisteredAllegroGraphSources (id, name, description, serverUrl, port, username, encryptedPassword, catalogName, repositoryName) VALUES ('" + Guid.NewGuid().ToString() + "','" + txtAgInstanceName.Text + "','" + txtServerUrl.Text + "','" + txtPortNumber.Text + "','" + txtUserName.Text + "','" + txtPassword.Text + "','" + txtCatalog.Text + "','" + txtRepository.Text + "')";
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            if(sqliteConnection.State == ConnectionState.Open)
                sqliteConnection.Close();
            this.DialogResult = DialogResult.OK;
            this.Close(); 
        }
    }
}
