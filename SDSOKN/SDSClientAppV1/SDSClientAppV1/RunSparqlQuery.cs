#define DODEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace SDSClientAppV1
{
    public partial class RunSparqlQuery : Form
    {


#if RELEASE
        public string urlAddress = "http://maps.mvbg.org/";
#endif
#if DODEBUG
        public string urlAddress = @"https://localhost:51899/"; //"http://nfsservices1/";
#endif
        private DataTable dt;
        public string instanceID;
        public RunSparqlQuery()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string results = string.Empty;
            try
            {


                string SERVER_URL = string.Empty;
                string PORT = "0";
                string CATALOG_ID = string.Empty;
                string REPOSITORY_ID = string.Empty;
                string USERNAME = string.Empty;
                string PASSWORD = string.Empty;
             //   foreach (var item in Form1.agInstances)
             
                 
                     var item = Form1.agInstances[Form1.cmbRepoIndex];

                     SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                     CATALOG_ID = item.Catalog.Trim();
                     REPOSITORY_ID = item.Repository;
                     USERNAME = item.Name;
                     PASSWORD = item.Password;
               
             

                if (SERVER_URL.Contains("http") == false)
                    SERVER_URL = "http://" + SERVER_URL;
                string toolName = string.Empty;
                string userName = USERNAME;
                string password = PASSWORD;
                string repository = REPOSITORY_ID;
                string serverName = SERVER_URL;
                RestClient rep = new RestClient();
                rep.BaseUrl = new Uri(SERVER_URL);
                rep.Authenticator = new HttpBasicAuthenticator(userName, password);
                rep.ClearHandlers();
                //    rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
                //    rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
                rep.AddDefaultHeader("Accept", "application/json");
                rep.AddDefaultHeader("Accept-Encoding", "gzip, deflate");
                rep.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded");
                var request = new RestRequest("catalogs/" + CATALOG_ID + "/repositories/" + repository +
                                              "?queryLn=SPARQL&infer=true&returnQueryMetadata=true&checkVariables=false&query=" +
                                              HttpUtility.UrlEncode(textBox1.Text.Replace('+', ' ')));

                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;

                IRestResponse response = rep.Execute(request);
                results = response.Content.ToString() + Environment.NewLine;
                //results += "Failed with url " + rep.BaseUrl + "/" + "catalogs/" + CATALOG_ID + "/repositories/" +
                //          repository +
                //          "?queryLn=SPARQL&infer=true&returnQueryMetadata=true&checkVariables=false&query=" +
                //          HttpUtility.UrlEncode(statement.Replace('+', ' ')) + " with statement = " + statement;
            }
            catch (Exception ex)
            {
                results = ex.Message;
            }
            
            //var request1 = new RestRequest();
            //var client2 = new RestClient(urlAddress + "api/runsparql");
            //var response2 = client2.Get(request1);
            //var info2 = response2.Content.ToString();
            //Console.WriteLine(info2);
            //var client1 = new RestClient(urlAddress + @"api/runsparql/executesparql/" + instanceID +"/" + HttpUtility.UrlEncode(textBox1.Text).Replace('+',' '));
            //var request2 = new RestRequest();
            //var response1 = client1.Get(request2);
            //var information = response1.Content.ToString();
            var resultsItems = string.Empty;
            try
            {
                //var resultsItems1 = JsonConvert.DeserializeObject(response.Content.ToString());
                resultsItems = results.ToString();
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(results.ToString());
                
                var offset = 0;
                int totalNumberofRecords = 0;
                dt = new DataTable();
                try
                {
                    foreach (var item in json.Children())
                    {
                        if (offset == 0)
                        {
                            int numberofColumns = item.Children().Count();
                            var columnList = item.First;
                            foreach (var colName in columnList.Children())
                            {
                                Console.WriteLine(colName.ToString());
                                dt.Columns.Add(new DataColumn(colName.ToString(), typeof(string)));

                            }
                        }
                        else if (offset == 1)
                        {
                            totalNumberofRecords = item.Children().First().Children().Count();
                            var columnList = item.Children().First();
                            foreach (var row in columnList.Children())
                            {
                                int columnOffset = 0;
                                DataRow dr = dt.NewRow();
                                foreach (var record in row.Children())
                                {
                                    dr[columnOffset] = record.ToString();
                                    columnOffset += 1;
                                }

                                dt.Rows.Add(dr);
                            }
                        }

                        offset += 1;
                    }

                    //int totalNumberofRecords = json.
                    listBoxControl1.Items.Add(textBox1.Text + " - (" + totalNumberofRecords.ToString() + ")");

                    gridControl1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in executing : " + HttpUtility.UrlEncode(textBox1.Text).Replace('+', ' ') +
                                    " - with error at : " + resultsItems + " with the error message of "+ ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the outer portion: " + resultsItems + " from the request " +
                                HttpUtility.UrlEncode(textBox1.Text).Replace('+', ' ') +
                                " - with the error at : " + ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            gridControl1.ShowRibbonPrintPreview();
        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBoxControl1_DoubleClick(object sender, EventArgs e)
        {
            string[] args = {" - ("};
            string statement = listBoxControl1.SelectedValue.ToString()
                .Split(args, StringSplitOptions.RemoveEmptyEntries)[0];
            var request1 = new RestRequest();

            var client1 = new RestClient(urlAddress + "api/runsparql/id/" + instanceID + "/" + HttpUtility.UrlEncode(statement).Replace('+',' '));
            var response1 = client1.Get(request1);
            var information = response1.Content.ToString();
            var resultsItems = JsonConvert.DeserializeObject(response1.Content.ToString());
            Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(resultsItems.ToString());
            var offset = 0;
            int totalNumberofRecords = 0;
           dt = new DataTable();

            foreach (var item in json.Children())
            {
                if (item.Count() == 3)
                {
                    if (offset == 0)
                    {
                        int numberofColumns = item.Children().Count();
                        var columnList = item.First;
                        foreach (var colName in columnList.Children())
                        {
                            Console.WriteLine(colName.ToString());
                            dt.Columns.Add(new DataColumn(colName.ToString(), typeof(string)));

                        }
                    }
                    else if (offset == 1)
                    {
                        totalNumberofRecords = item.Children().First().Children().Count();
                        var columnList = item.Children().First();
                        foreach (var row in columnList.Children())
                        {
                            int columnOffset = 0;
                            DataRow dr = dt.NewRow();
                            foreach (var record in row.Children())
                            {
                                dr[columnOffset] = record.ToString();
                                columnOffset += 1;
                            }

                            dt.Rows.Add(dr);
                        }
                    }

                    offset += 1;
                }
            }
            //int totalNumberofRecords = json.
            

            gridControl1.DataSource = dt;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "xlsx";
            ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName; 
                gridControl1.ExportToXlsx(path);
            }
        }

        private void btnViewNameSpaces_Click(object sender, EventArgs e)
        {
            PrefixViewer pfx = new PrefixViewer();
            var request1 = new RestRequest();

            var client1 = new RestClient(urlAddress + "api/prefixes/retrieve/id/" + instanceID );
            
            var response1 = client1.Get(request1);
            string tstr = response1.Content.ToString().Remove(0, 1);
            tstr = tstr.Remove(tstr.Length - 1);
            string t = Compressor.Decompress(tstr);
            pfx.dt =  Form1.ConvertXmlStringToDataTable(t);
            
            pfx.ShowDialog(); 
        }
    }
}
