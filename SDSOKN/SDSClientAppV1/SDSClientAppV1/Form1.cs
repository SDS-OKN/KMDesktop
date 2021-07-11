#define RELEASE
using CefSharp;
using CefSharp.Structs;
using CefSharp.WinForms;
using DevExpress.Spreadsheet;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using RestSharp.Authenticators;
using DataRow = System.Data.DataRow;

namespace SDSClientAppV1
{

    public partial class Form1 : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {

#if RELEASE
        public string urlAddress = "http://maps.mvbg.org/agCaller/";
#endif
#if DODEBUG
        public string urlAddress = @"https://localhost:51899/";
#endif
        private bool isNew;
        public static int cmbClassChoiceIndex;
        public static int cmbClassFilterIndex;
        public static int cmbRepoIndex;
        public static ChromiumWebBrowser browserClass;
        public static ChromiumWebBrowser browserEditor;
        public static List<AllegroGraphRegistryEntry> agInstances;
        public static List<ClassList> masterClassList;
        public static List<InstanceItem> instanceList;
        public static InstanceItem instanceItem;
        public static InstanceItem currentInstance;
        public static DataTable tmpTable;
        public static List<string> columnNames;
        public static DataTable tmpNewInstance;
        public static DockInformationExplorer dockInformationExplorer;
        public static HubConnection connection;
        public static RehostedDesignerControl.DesignerControl workflowControl;

        public static string LookupInformationUrl =
            "select distinct ?name  ?label ?object ?predicate where { ?name rdf:type <%%REPLACEME%%> . ?name rdfs:label ?label . OPTIONAL {?name ?object ?predicate .}}";
        public Form1()
        {
            InitializeComponent();
            isNew = false;
            drpOntologySource.SelectedIndexChanged -= drpOntologySource_SelectedIndexChanged;
            var settings = new CefSettings();
            columnNames = new List<string>();
            // Increase the log severity so CEF outputs detailed information, useful for debugging
            settings.LogSeverity = LogSeverity.Verbose;
            // By default CEF uses an in memory cache, to save cached data e.g. passwords you need to specify a cache path
            // NOTE: The executing user must have sufficient privileges to write to this folder.
            settings.CachePath = "cache";
            dockInformationExplorer = new DockInformationExplorer();
            dockInformationExplorer.Dock = DockStyle.Fill;
            tabInformationExplorer.Controls.Add(dockInformationExplorer);
            Cef.Initialize(settings);
            browserClass = new ChromiumWebBrowser("https://maps.mvbg.org/AgDisplay/Home/DisplayNodes");
            browserEditor = new ChromiumWebBrowser("https://maps.mvbg.org/AgDisplay/Home/DisplayNodes");
            //browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            //{
            //    var repo = e.ObjectRepository;
            //    if (e.ObjectName == "boundAsync")
            //    {
            //        BindingOptions bindingOptions = null; //Binding options is an optional param, defaults to null
            //        bindingOptions = BindingOptions.DefaultBinder; //Use the default binder to serialize values into complex objects, CamelCaseJavascriptNames = true is the default
            //        //No camelcase of names and specify a default binder
            //        repo.Register("boundAsync", new BoundObject(), isAsync: true, options: bindingOptions);
            //    }
            //};

            //browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
            //{
            //    var name = e.ObjectName;

            //    Debug.WriteLine($"Object {e.ObjectName} was bound successfully.");
            //};
            //splitContainer1.Panel1.Controls.Add(browser);
            //browser.Dock = DockStyle.Fill;
            //browser.Refresh();
            connection = new HubConnectionBuilder()
             .WithUrl("https://maps.mvbg.org/AgDisplay/ChatHub")
             .Build();
            InitConnection();
            var Initclient = new RestClient(urlAddress + "api/AllegroGraphInstance");
            agInstances = new List<AllegroGraphRegistryEntry>();
            OrgChart chart = new OrgChart();
            diagramOrgChartController1.DataSource = chart.Data;
            instanceList = new List<InstanceItem>();
            instanceItem = new InstanceItem();
            masterClassList = new List<ClassList>();
            var request = new RestRequest();
            var response1 = Initclient.Get(request);
            var itemToAdd = new AllegroGraphRegistryEntry();
            string initID = string.Empty;
            itemToAdd.ID = response1.Content.ToString();
            itemToAdd.ID = itemToAdd.ID.Replace("\"", "");
            itemToAdd.ID = itemToAdd.ID.Trim();
            initID = itemToAdd.ID;
            itemToAdd.Url = "http://45.19.182.17";
            itemToAdd.Port = 10035;
            itemToAdd.Catalog = "Data";
            itemToAdd.Repository = "SDSS";
            itemToAdd.Name = "super";
            itemToAdd.Password = "Show4time!";
            agInstances.Add(itemToAdd);
            itemToAdd = new AllegroGraphRegistryEntry();
            Initclient = new RestClient(urlAddress + "api/AllegroGraphInstance/register/super/Show4time!/45.19.182.17/10035/importme/sdsokn");
            response1 = Initclient.Get(request);
            itemToAdd.ID = response1.Content.ToString();
            itemToAdd.ID = itemToAdd.ID.Replace("\"", "");
            itemToAdd.ID = itemToAdd.ID.Trim();
            itemToAdd.Url = "http://45.19.182.17";
            itemToAdd.Port = 10035;
            itemToAdd.Catalog = "importme";
            itemToAdd.Repository = "sdsokn";
            itemToAdd.Name = "super";
            itemToAdd.Password = "Show4time!";
            initID = itemToAdd.ID;
            agInstances.Add(itemToAdd);
            drpOntologySource.Properties.Items.Add("SDSOKN Ontology");
            var client = new RestClient(urlAddress + "api/SDSClass/id/" + initID);
            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            string[] firstSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            cmbClassChoice.Properties.Items.Clear();
            cmbInstanceName.Properties.Items.Clear();
            cmbClassFilter.Properties.Items.Clear();
            foreach (string sInfo in firstSplit)
            {
                string[] t1 = sInfo.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (t1.Length > 1)
                {
                    string itemClass = t1[0].Replace("\"", "");
                    cmbClassChoice.Properties.Items.Add(itemClass.Trim());
                    masterClassList.Add(new ClassList(itemClass.Trim(), t1[1]));
                    cmbClassFilter.Properties.Items.Add(itemClass.Trim());
                }
            }

            drpOntologySource.SelectedIndex = 1;
            drpOntologySource.Text = drpOntologySource.SelectedText;
            elementHost1.Dock = DockStyle.Fill;
            workflowControl = new RehostedDesignerControl.DesignerControl();

            elementHost1.Child = workflowControl;

            cmbClassFilter.Text = "Select Class";
            cmbClassChoice.Text = "Select Class";
            cmbInstanceName.Text = "No Instances";
            cmbClassChoice.SelectedIndex = -1;
            client = new RestClient(urlAddress + "api/aginstance/id/" + initID);
            response = client.Get(request);
            s = response.Content.ToString();
            string[] secondSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sInfo in secondSplit)
            {
                string[] t1 = sInfo.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (t1.Length > 1)
                {
                    cmbInstanceName.Properties.Items.Add(t1[0]);
                    //instanceList.Add(new ClassList(t1[0], t1[1]));
                    List<string> instanceNames = new List<string>();
                    InstanceItem tempInstance = new InstanceItem();
                    List<List<string>> instanceProps = new List<List<string>>();
                    List<string> props = new List<string>();
                    tempInstance.Name = t1[0];
                    instanceNames.Add("Uri");
                    props.Add(t1[1]);
                    instanceProps.Add(props);
                    tempInstance.PropertyNames = instanceNames;
                    tempInstance.PropertyValues = instanceProps;
                    instanceList.Add(tempInstance);
                }
            }
            drpOntologySource.Text = "SDSOKN Ontology";
            cmbClassChoice.Text = "Select Class";
            cmbClassFilter.Text = "Select Class";

            cmbClassFilter.SelectedIndexChanged += CmbClassFilter_SelectedIndexChanged;
            drpOntologySource.SelectedIndexChanged += drpOntologySource_SelectedIndexChanged;
            //tabWorkflowEditor.Controls.Add(wd);
            browserClass.Dock = DockStyle.Fill;
            browserEditor.Dock = DockStyle.Fill;
            spltEditClass.Panel1.Controls.Add(browserClass);
            spltInstanceEditor.Panel1.Controls.Add(browserEditor);
            this.Cursor = Cursors.Default;
        }

        async public void InitConnection()
        {
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                // Retrieve class and update
                string[] args = { "----" };
                string[] results = message.Split(args, StringSplitOptions.RemoveEmptyEntries);
                int counter = 0;
                foreach (var item in cmbClassChoice.Properties.Items)
                {
                    if (results[1].ToLower() == item.ToString().ToLower())
                    {
                        cmbClassChoice.SelectedIndex = counter;
                        cmbClassChoice.Text = item.ToString();
                        break;
                    }
                    counter += 1;
                }

            });

            try
            {

                await connection.StartAsync();
                Console.WriteLine("connection state:" + connection.State.ToString());

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            this.Cursor = Cursors.Default;
        }
        private void CmbClassFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            string node = ((DevExpress.XtraEditors.ComboBoxEdit)sender).Text;
            string classuri = string.Empty;
            foreach (var item in masterClassList)
            {
                if (node == item.Name)
                {
                    classuri = item.Uri.Trim();

                }
            }
            if (string.IsNullOrEmpty(classuri))
                return;
            var client = new RestClient(urlAddress + "api/SDSInstance/uri/" + HttpUtility.UrlEncode(classuri.Trim()) + "/" + agInstances[drpOntologySource.SelectedIndex].ID);
            var request = new RestRequest();

            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);
            cmbInstanceName.Properties.Items.Clear();

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            try
            {
                cmbInstanceName.SelectedIndexChanged -= CmbInstanceName_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            string[] secondSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            cmbInstanceName.Properties.Items.Clear();

            foreach (string sInfo in secondSplit)
            {
                string[] t1 = sInfo.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (t1.Length > 1)
                {
                    string itemInstance = t1[0].Replace("\"", "");

                    cmbInstanceName.Properties.Items.Add(itemInstance.Trim());
                    //instanceList.Add(new ClassList(itemInstance.Trim(), t1[1]));

                }
            }
            if (cmbInstanceName.Properties.Items.Count > 0)
                cmbInstanceName.Text = cmbInstanceName.Properties.Items[0].ToString();
            else
                cmbInstanceName.Text = "No instances";

            cmbInstanceName.SelectedIndexChanged += CmbInstanceName_SelectedIndexChanged;
            this.Cursor = Cursors.Default;
            Application.DoEvents();
            if (cmbInstanceName.Properties.Items.Count > 0)
            {
                CmbInstanceName_SelectedIndexChanged(sender, e);


            }
            this.Cursor = Cursors.Default;
        }

        private void CmbInstanceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string classuri = string.Empty;
            string nodeClass = cmbClassFilter.Text;
            foreach (var item in masterClassList)
            {
                if (nodeClass == item.Name)
                {
                    classuri = item.Uri.Trim();

                }
            }
            if (string.IsNullOrEmpty(classuri))
                return;
            if (isNew)
            {
                tmpTable = new DataTable();
                tmpTable.Columns.Add("PropertyName", Type.GetType("System.String"));
                tmpTable.Columns.Add("Value", Type.GetType("System.String"));
                tmpTable.Columns.Add("OriginalValue", Type.GetType("System.String"));
                DataRow dr1 = tmpTable.NewRow();
                dr1[0] = "Produces";
                dr1[1] = "Watershed Boundary Dataset";
                tmpTable.Rows.Add(dr1);
                dr1 = tmpTable.NewRow();
                dr1[0] = "HasName";
                dr1[1] = "Watershed Boundary Dataset Program";
                tmpTable.Rows.Add(dr1);
                dr1 = tmpTable.NewRow();
                dr1[0] = "HasDescription";
                dr1[1] = "The Watershed Boundary Dataset (WBD) is a seamless, national hydrologic unit dataset.";
                tmpTable.Rows.Add(dr1);
                dr1 = tmpTable.NewRow();
                dr1[0] = "HasUrl";
                dr1[1] =
                    @" https://www.usgs.gov/core-science-systems/ngp/national-hydrography/watershed-boundary-dataset";
                tmpTable.Rows.Add(dr1);
                gridControl1.DataSource = tmpTable;
                return;

            }
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            string node = cmbInstanceName.Text;
            string uri = string.Empty;
            currentInstance = new InstanceItem();
            foreach (var item in instanceList)
            {
                // get the uri to search
                if ((node == item.Name) || (item.Name.ToLower().Contains(node.ToLower())))
                {
                    int offsetUri = 0;
                    foreach (var propName in item.PropertyNames)
                    {
                        if (propName.ToLower().Trim() == "uri")
                        {
                            uri = Convert.ToString(item.PropertyValues[offsetUri][0]);
                            break;
                        }
                        offsetUri += offsetUri;
                    }
                    currentInstance = item;

                    break;
                }
            }
            if (string.IsNullOrEmpty(uri))
                return;
            var client = new RestClient(urlAddress + "api/AgInstanceInfo/uri/" + HttpUtility.UrlEncode(uri.Trim()) + "/ " + agInstances[drpOntologySource.SelectedIndex].ID + "/" + HttpUtility.UrlEncode(classuri));
            var request = new RestRequest();

            var response = client.Get(request);
            string sTemp = response.Content.ToString();

            string s = Compressor.Decompress(sTemp.Replace("\"", "").Trim());
            System.Diagnostics.Debug.WriteLine(s);


            try
            {
                cmbInstanceName.SelectedIndexChanged -= CmbInstanceName_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


            Dictionary<string, string> FinalValues = new Dictionary<string, string>();

            DataTable tbl = ConvertXmlStringToDataTable(s);
            grdInstanceDetails.OptionsBehavior.AutoPopulateColumns = true;
            gridControl1.DataSource = tbl;
            Application.DoEvents();
            gridControl1.RefreshDataSource();
            Application.DoEvents();
            grdInstanceDetails.OptionsView.RowAutoHeight = true;
            try
            {
                if (grdInstanceDetails.Columns.Count > 4)
                {
                    grdInstanceDetails.Columns[0].Visible = false;
                    grdInstanceDetails.Columns[4].Visible = false;
                    grdInstanceDetails.Columns[5].Visible = false;
                    grdInstanceDetails.Columns[6].Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            grdInstanceDetails.OptionsView.ColumnAutoWidth = true;

            this.Cursor = Cursors.Default;
            Application.DoEvents();
            try
            {
                cmbInstanceName.SelectedIndexChanged += CmbInstanceName_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            this.Cursor = Cursors.Default;

        }

        private void acEditor_Click(object sender, EventArgs e)
        {
            mainTabl.SelectedTabPage = tabEditor;
        }

        private void acAdvancedEditor_Click(object sender, EventArgs e)
        {
            mainTabl.SelectedTabPage = tabAdvancedEditor;
        }

        private void acKnowledgeExplorer_Click(object sender, EventArgs e)
        {
            mainTabl.SelectedTabPage = tabInformationExplorer;
            dockInformationExplorer.Refresh();
        }

        private void acWorkflowEditor_Click(object sender, EventArgs e)
        {
            mainTabl.SelectedTabPage = tabWorkflowEditor;

        }

        private void acWorkflowsAdmin_Click(object sender, EventArgs e)
        {
            mainTabl.SelectedTabPage = tabWorkflowMonitor;
        }

        private void acSettings_Click(object sender, EventArgs e)
        {
            mainTabl.SelectedTabPage = tabSettings;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //save current tables
            DataTable dtUpdated = GetDataTable(grdInstanceDetails);
            string originalInfo = String.Empty;
            int offset = 0;
            foreach (var item in currentInstance.PropertyNames)
            {
                originalInfo += item + " ------- ";
                string tmpValue = string.Empty;
                foreach (var value in currentInstance.PropertyValues[offset])
                {
                    tmpValue += value + " ******* ";
                }
                originalInfo += tmpValue + " ;;;;;;;; ";

            }
            string updateInfo = string.Empty;
            foreach (DataRow drOrig in tmpTable.Rows)
            {
                bool Found = false;
                DataRow currentRow = dtUpdated.NewRow();
                foreach (DataRow dr in dtUpdated.Rows)
                {
                    if (Convert.ToString(dr[0]).Trim() == Convert.ToString(drOrig[0]).Trim())
                    {
                        currentRow[0] = dr[0];
                        currentRow[1] = dr[1];
                        Found = true;
                        break;

                    }
                }
                if (Found)
                {
                    if (Convert.ToString(currentRow[1]).Trim() == Convert.ToString(drOrig[1]).Trim())
                    {
                        var client = new RestClient(urlAddress + "api/UpdateInstance/updateStatement/" + HttpUtility.UrlEncode(originalInfo.Trim()) + "/" + HttpUtility.UrlEncode(updateInfo.Trim()) + "/ " + agInstances[drpOntologySource.SelectedIndex].ID);
                        var request = new RestRequest();

                        var response = client.Get(request);
                        string s = response.Content.ToString();
                        break;
                    }
                }
                //

            }
        }

        DataTable GetDataTable(GridView view)
        {
            DataTable dt = new DataTable();
            foreach (DevExpress.XtraGrid.Columns.GridColumn c in view.Columns)
                dt.Columns.Add(c.FieldName, c.ColumnType);
            for (int r = 0; r < view.RowCount; r++)
            {
                object[] rowValues = new object[dt.Columns.Count];
                for (int c = 0; c < dt.Columns.Count; c++)
                    rowValues[c] = view.GetRowCellValue(r, dt.Columns[c].ColumnName);
                dt.Rows.Add(rowValues);
            }
            return dt;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }
        public static DataTable ConvertXmlStringToDataTable(string s)
        {

            if (string.IsNullOrEmpty(s))
            {
                Debug.WriteLine("Big problem, no table");
                return new DataTable();
            }



            var xMlReader = XmlReader.Create(new StringReader(s));


            var newTable = new DataTable();

            try
            {
                newTable.ReadXml(xMlReader);
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }

            return newTable;
        }
        private void cmbClassChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            string node = ((DevExpress.XtraEditors.ComboBoxEdit)sender).Text;
            string uri = string.Empty;
            foreach (var item in masterClassList)
            {
                if (node.ToLower().Trim() == item.Name.ToLower().Trim())
                {
                    uri = item.Uri.Trim();

                }
            }
            if (string.IsNullOrEmpty(uri))
                return;
            var client = new RestClient(urlAddress + "api/SDSClass/retrieveItems/id/" + HttpUtility.UrlEncode(uri.Trim()) + "/" + agInstances[drpOntologySource.SelectedIndex].ID);
            var request = new RestRequest();

            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);
            cmbInstanceName.Properties.Items.Clear();

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            try
            {
                cmbInstanceName.SelectedIndexChanged -= CmbInstanceName_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            string[] secondSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            cmbInstanceName.Properties.Items.Clear();
            DataTable tmpTable = new DataTable();
            tmpTable.Columns.Add("Properties", System.Type.GetType("System.String"));
            tmpTable.Columns.Add("Values", System.Type.GetType("System.String"));
            foreach (string sInfo in secondSplit)
            {
                string[] t1 = sInfo.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (t1.Length > 1)
                {
                    string itemInstance = t1[0].Replace("\"", "");

                    DataRow dr = tmpTable.NewRow();
                    dr[0] = itemInstance.Trim();
                    try
                    {
                        string itemInstance2 = t1[1].Replace("\"", "");
                        dr[1] = itemInstance2;
                    }
                    catch (Exception ex) { }
                    tmpTable.Rows.Add(dr);

                    //instanceList.Add(new ClassList(itemInstance.Trim(), t1[1]));

                }

            }
            grdClassProperties.DataSource = tmpTable;
            this.Cursor = Cursors.Default;
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            string node = ((DevExpress.XtraEditors.ComboBoxEdit)sender).Text;
            string uri = string.Empty;
            foreach (var item in masterClassList)
            {
                if (node == item.Name)
                {
                    uri = item.Uri.Trim();

                }
            }
            if (string.IsNullOrEmpty(uri))
                return;
            var client = new RestClient(urlAddress + "api/SDSClass/uri/" + HttpUtility.UrlEncode(uri.Trim()) + "/" + agInstances[drpOntologySource.SelectedIndex].ID);
            var request = new RestRequest();

            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);
            cmbInstanceName.Properties.Items.Clear();

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            try
            {
                cmbInstanceName.SelectedIndexChanged -= CmbInstanceName_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            string[] secondSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            cmbInstanceName.Properties.Items.Clear();
            DataTable tmpTable = new DataTable();
            tmpTable.Columns.Add("Properties", System.Type.GetType("System.String"));
            tmpTable.Columns.Add("Values", System.Type.GetType("System.String"));
            foreach (string sInfo in secondSplit)
            {
                string[] t1 = sInfo.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (t1.Length > 1)
                {
                    string itemInstance = t1[0].Replace("\"", "");

                    DataRow dr = tmpTable.NewRow();
                    dr[0] = itemInstance.Trim();
                    try
                    {
                        string itemInstance2 = t1[1].Replace("\"", "");
                        dr[1] = itemInstance2;
                    }
                    catch (Exception ex) { }
                    tmpTable.Rows.Add(dr);

                    //instanceList.Add(new ClassList(itemInstance.Trim(), t1[1]));

                }

            }
            grdClassProperties.DataSource = tmpTable;
            this.Cursor = Cursors.Default;
        }

        private void btnCreateTemplate_Click(object sender, EventArgs e)
        {
            // export to excel
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "*.xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfd.FileName;
                Workbook workbook = new Workbook();

                // Access the first worksheet in the workbook.
                DevExpress.Spreadsheet.Worksheet worksheet = workbook.Worksheets[0];
                this.Cursor = Cursors.WaitCursor;
                Application.DoEvents();
                // Set the unit of measurement.
                workbook.Unit = DevExpress.Office.DocumentUnit.Point;

                NewInstance instance = new NewInstance();

                tmpNewInstance = new DataTable();
                tmpNewInstance.Columns.Add("PropertyName", Type.GetType("System.String"));
                tmpNewInstance.Columns.Add("Value", Type.GetType("System.String"));
                Dictionary<string, string> FinalValues = new Dictionary<string, string>();

                List<string> finalItems = new List<string>();
                foreach (string s in columnNames)
                {
                    if (finalItems.Contains(s) == false)
                    {
                        finalItems.Add(s);
                    }
                }
                foreach (string s in finalItems)
                {
                    DataRow dr = tmpNewInstance.NewRow();
                    dr[0] = s;
                    tmpNewInstance.Rows.Add(dr);
                }
                worksheet[0, 0].Value = "Properties";
                for (int i = 1; i < finalItems.Count + 1; i++)
                {
                    worksheet[0, i].Value = finalItems[i - 1];
                }
                workbook.SaveDocument(fileName, DocumentFormat.OpenXml);

                System.Diagnostics.Process.Start(fileName);
            }
            this.Cursor = Cursors.Default;
        }

        private void btnCreateNewInstance_Click(object sender, EventArgs e)
        {
            NewInstance instance = new NewInstance();

            tmpNewInstance = new DataTable();
            tmpNewInstance.Columns.Add("propertyName", Type.GetType("System.String"));
            tmpNewInstance.Columns.Add("values", Type.GetType("System.String"));
            Dictionary<string, string> FinalValues = new Dictionary<string, string>();

            List<string> finalItems = new List<string>();
            foreach (string s in columnNames)
            {
                if (finalItems.Contains(s) == false)
                {
                    finalItems.Add(s);
                }
            }
            foreach (string s in finalItems)
            {
                DataRow dr = tmpNewInstance.NewRow();
                dr[0] = s;
                tmpNewInstance.Rows.Add(dr);
            }
            instance.grdNewItem.DataSource = tmpNewInstance;
            if (instance.ShowDialog() == DialogResult.OK)
            {
                //save it

                string newInstanceString = string.Empty;
                DataTable tmp = new DataTable();

                var client = new RestClient(urlAddress + "api/UpdateInstance/add/" + HttpUtility.UrlEncode(newInstanceString.Trim()) + "/ " + agInstances[drpOntologySource.SelectedIndex].ID);
                var request = new RestRequest();

                var response = client.Get(request);
                string s = response.Content.ToString();
                System.Diagnostics.Debug.WriteLine(s);
            }
        }

        private void bnSaveChanges_Click(object sender, EventArgs e)
        {
            string node = cmbInstanceName.Text;
            string instanceUri = string.Empty;
            foreach (var item in instanceList)
            {
                // get the uri to search
                if (node == item.Name)
                {
                    int offsetUri = 0;
                    foreach (var propName in item.PropertyNames)
                    {
                        if (propName.ToLower().Trim() == "uri")
                        {
                            instanceUri = Convert.ToString(item.PropertyValues[offsetUri][0]);
                            break;
                        }
                        offsetUri += offsetUri;
                    }
                    currentInstance = item;

                    break;
                }
            }
            DataTable dtUpdated = GetDataTable(grdInstanceDetails);
            foreach (DataRow row in dtUpdated.Rows)
            {
                if (Convert.ToString(row["InstanceValue"]) != Convert.ToString(row["OriginalInstanceValue"]))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(row["OriginalInstanceValue"])))
                    {
                        var client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" + HttpUtility.UrlEncode(Convert.ToString(instanceUri) + ";;;;;;;;" + Convert.ToString(row["PropertyURI"]) + ";;;;;;;;" + Convert.ToString(row["InstanceValue"])) +
                                                        "/" +
                                                    agInstances[drpOntologySource.SelectedIndex].ID);
                        var request = new RestRequest();
                        client.Get(request);
                    }
                    else
                    {
                        var client = new RestClient(urlAddress + "api/UpdateInstance/updateStatement/"
                                                               + HttpUtility.UrlEncode(Convert.ToString(row["InstanceID"])) + "/" +
                                                               HttpUtility.UrlEncode(Convert.ToString(instanceUri) + ";;;;;;;;" + Convert.ToString(row["PropertyURI"]) + ";;;;;;;;" + Convert.ToString(row["OriginalInstanceValue"])) +
                                                               "/" + HttpUtility.UrlEncode(Convert.ToString(instanceUri) + ";;;;;;;;" + Convert.ToString(row["PropertyURI"]) + ";;;;;;;;" + Convert.ToString(row["InstanceValue"])) + "/"
                                                               + agInstances[drpOntologySource.SelectedIndex].ID);
                        var request = new RestRequest();
                        client.Get(request);
                    }
                }
            }

            MessageBox.Show("Updated information");
        }

        private void labelControl5_Click(object sender, EventArgs e)
        {

        }

        private void btnSelectNewAllegrograph_Click(object sender, EventArgs e)
        {

        }

        private void btnAskQuestion_Click(object sender, EventArgs e)
        {
            //  txtChatBox.Text += "User question: " + txtQuery.Text;
        }

        private void btnCreateNewInstance_Click_1(object sender, EventArgs e)
        {
            btnCreateNewInstance_Click(sender, e);
        }

        private void btnAddAllegrograph_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SpecifyNewAllegroGraph allegroGraph = new SpecifyNewAllegroGraph();
            if (allegroGraph.ShowDialog() == DialogResult.OK)
            {
                // do registering
            }
        }

        private void drpOntologySource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var request = new RestRequest();
            var client = new RestClient(urlAddress + "api/SDSClass/id/" + agInstances[drpOntologySource.SelectedIndex].ID);
            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            string[] firstSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            cmbClassChoice.Properties.Items.Clear();
            cmbInstanceName.Properties.Items.Clear();
            cmbClassFilter.Properties.Items.Clear();
            foreach (string sInfo in firstSplit)
            {
                string[] t1 = sInfo.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (t1.Length > 1)
                {
                    string itemClass = t1[0].Replace("\"", "");
                    cmbClassChoice.Properties.Items.Add(itemClass.Trim());
                    masterClassList.Add(new ClassList(itemClass.Trim(), t1[1]));
                    cmbClassFilter.Properties.Items.Add(itemClass.Trim());
                }
            }



            cmbClassFilter.Text = "Select Class";
            cmbClassChoice.Text = "Select Class";
            cmbInstanceName.Text = "No Instances";
            cmbClassChoice.SelectedIndex = -1;
            client = new RestClient(urlAddress + "api/aginstance/id/" + agInstances[drpOntologySource.SelectedIndex].ID);
            response = client.Get(request);
            s = response.Content.ToString();
            string[] secondSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sInfo in secondSplit)
            {
                string[] t1 = sInfo.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (t1.Length > 1)
                {
                    cmbInstanceName.Properties.Items.Add(t1[0]);
                    //instanceList.Add(new ClassList(t1[0], t1[1]));
                    List<string> instanceNames = new List<string>();
                    InstanceItem tempInstance = new InstanceItem();
                    List<List<string>> instanceProps = new List<List<string>>();
                    List<string> props = new List<string>();
                    tempInstance.Name = t1[0];
                    instanceNames.Add("Uri");
                    props.Add(t1[1]);
                    instanceProps.Add(props);
                    tempInstance.PropertyNames = instanceNames;
                    tempInstance.PropertyValues = instanceProps;
                    instanceList.Add(tempInstance);
                }
            }


            cmbClassFilter.SelectedIndexChanged += CmbClassFilter_SelectedIndexChanged;
            drpOntologySource.SelectedIndexChanged += drpOntologySource_SelectedIndexChanged;

        }

        private void grdInstanceDetails_DoubleClick(object sender, EventArgs e)
        {




        }

        private void btnCreateNewInstance_Click_2(object sender, EventArgs e)
        {
            DataEntry dataEntry = new DataEntry();
            if (dataEntry.ShowDialog() == DialogResult.OK)
            {
                cmbInstanceName.Properties.Items.Add("Wateshed Boundary Dataset Program");
                isNew = true;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".xlsx";
            ofd.Filter = "Excel Files (*.xlsx)|*.xlsx|Excel Macro Files (*.xlsm)|*.xlsm";
            ofd.Title = "Select file to import";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Workbook wb = new Workbook();
                wb.LoadDocument(ofd.FileName);
                bool ViewMultiplePages = false;
                if (wb.Worksheets.Count > 1)
                {
                    DialogResult drResult = MessageBox.Show(
                        "There are more than one worksheet in this file, do you wish to import all of them?",
                        "Multiple Tables Detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (drResult == DialogResult.Yes)
                    {
                        for (int i = 0; i < wb.Worksheets.Count; i++)
                        {
                            string worksheetName = wb.Worksheets[i].Name;
                            ProcessWorksheet(wb, worksheetName);
                        }
                    }
                    else if(drResult == DialogResult.No)
                    {
                        XtraInputBoxForm input = new XtraInputBoxForm();
                        //input.Text = "Specify the name of the worksheet to import.";
                        XtraInputBoxArgs args = new XtraInputBoxArgs();
                        args.Caption = "Worksheet to Import";
                        args.Prompt = "Specify the name of the worksheet to import";
                        var results = XtraInputBox.Show("Specify the name of the worksheet to import", "Worksheet to Import", "Person");
                        if (results != "") ProcessWorksheet(wb, results);

                    }
                }

                else
                {

                    Worksheet tbl = wb.Worksheets[0];
                    ProcessWorksheet(wb, wb.Worksheets[0].Name);

                }
            }
        }

        private void ProcessWorksheet(Workbook wb, string worksheetName)
        {
            Worksheet tbl = wb.Worksheets[worksheetName];

            // IF WORKSHEETNAME == INSTRUCTIONS, skip
            if ((tbl.Name.ToLower() == "instructions")|| (tbl.Name.ToLower() == "lookups") || (tbl.Name.ToLower() == "meta"))
                return;
            int rowOffset = 3;
            int ColumnOffset = 3;
            bool continueProcessing = true;
            DataTable NamesTable = RetrieveURI(tbl.Name);
            while (continueProcessing)
            {
                string identifier = string.Empty;
                identifier = Convert.ToString(tbl[rowOffset, 2].Value);
                if (string.IsNullOrEmpty(identifier))
                {
                    continueProcessing = false;
                }
                else
                {
                    //first build the names table
                    string filterString = "label ='\"" + identifier + "\"^^<http://www.w3.org/2001/XMLSchema#string>'";
                    DataTable dataView = NamesTable.Clone();
                    foreach (DataRow dr in NamesTable.Rows)
                    {
                        if (Convert.ToString(dr["label"]).Contains(identifier))
                        {
                            DataRow drNew = dataView.NewRow();
                            foreach (DataColumn dc in dataView.Columns)
                            {
                                drNew[dc.ColumnName] = dr[dc.ColumnName];
                            }

                            dataView.Rows.Add(drNew);
                        }
                    }
                    // <http://www.sdsconsortium.org/schemas/sds-okn.owl#WQIntv00009_NTKC1>
                    string columnHeader2 = string.Empty;
                    if (dataView.Rows.Count == 0) // there are no rows, so new item
                    {
                        //";;;;;;;;"
                        string newSrcURI = HttpUtility.UrlEncode("http://www.sdsconsortium.org/schemas/sds-okn.owl#" + Convert.ToString(tbl[rowOffset, 1].Value));
                        string inputArray = HttpUtility.UrlEncode(newSrcURI) + ";;;;;;;;" + HttpUtility.UrlEncode("http://www.w3.org/1999/02/22-rdf-syntax-ns#type")+ ";;;;;;;;" +
                                            HttpUtility.UrlEncode(Convert.ToString(tbl[0, 0].Value));

                        // first add to database
                        var client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" + inputArray +
                                                    "/" +
                                                    agInstances[drpOntologySource.SelectedIndex].ID);
                        var request = new RestRequest();
                        client.Get(request);
                        // now add the label
                        inputArray = HttpUtility.UrlEncode(newSrcURI) + ";;;;;;;;" + HttpUtility.UrlEncode("http://www.w3.org/2000/01/rdf-schema#label")+";;;;;;;;" +
                                            HttpUtility.UrlEncode(Convert.ToString(tbl[rowOffset, 2].Value));
                        client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" + inputArray +
                                                "/" +
                                                agInstances[drpOntologySource.SelectedIndex].ID);
                        request = new RestRequest();
                        client.Get(request);
                        // now add the other properties
                        do
                        {
                            columnHeader2 = Convert.ToString(tbl[1, ColumnOffset].Value);
                            if (string.IsNullOrEmpty(columnHeader2) == false)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(tbl[rowOffset, ColumnOffset].Value)))
                                {

                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        "Col " + ColumnOffset++ + " name is " + columnHeader2);
                                    // build insert string
                                    if (Convert.ToString(tbl[2, ColumnOffset].Value) == "'xsd:string'")
                                    {
                                        inputArray = HttpUtility.UrlEncode(newSrcURI) + ";;;;;;;;" +
                                                     HttpUtility.UrlEncode(
                                                         Convert.ToString(tbl[0, ColumnOffset].Value)) + ";;;;;;;;" +
                                                     HttpUtility.UrlEncode(
                                                         Convert.ToString(tbl[rowOffset, ColumnOffset].Value));
                                        client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" +
                                                                inputArray +
                                                                "/" +
                                                                agInstances[drpOntologySource.SelectedIndex].ID);
                                        request = new RestRequest();
                                        client.Get(request);
                                    }
                                    else
                                    {
                                        DataTable tblLookup = RetrieveURI(tbl.Name,
                                            Convert.ToString(tbl[0, ColumnOffset].Value));
                                        DataTable dataView2 = tblLookup.Clone();
                                        foreach (DataRow dr in tblLookup.Rows)
                                        {
                                            if (Convert.ToString(dr["label"]).Contains(identifier))
                                            {
                                                DataRow drNew = dataView2.NewRow();
                                                foreach (DataColumn dc in dataView2.Columns)
                                                {
                                                    drNew[dc.ColumnName] = dr[dc.ColumnName];
                                                }

                                                dataView2.Rows.Add(drNew);
                                            }
                                        }

                                        if (dataView2.Rows.Count == 0)
                                        {
                                            string newSrcURI2 = HttpUtility.UrlEncode(
                                                "http://www.sdsconsortium.org/schemas/sds-okn.owl#" +
                                                Convert.ToString(tbl[rowOffset, ColumnOffset].Value).Replace(' ', '_'));
                                            string inputArray1 =
                                                HttpUtility.UrlEncode(newSrcURI2) + ";;;;;;;;" +
                                                HttpUtility.UrlEncode(
                                                    "http://www.w3.org/1999/02/22-rdf-syntax-ns#type") + ";;;;;;;;" +
                                                HttpUtility.UrlEncode(Convert.ToString(tbl[0, ColumnOffset].Value));

                                            // add type instance
                                            client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" +
                                                                    inputArray +
                                                                    "/" +
                                                                    agInstances[drpOntologySource.SelectedIndex].ID);
                                            request = new RestRequest();
                                            client.Get(request);
                                            // now add the label
                                            inputArray =
                                                HttpUtility.UrlEncode(newSrcURI2) + ";;;;;;;;" +
                                                HttpUtility.UrlEncode("http://www.w3.org/2000/01/rdf-schema#label") +
                                                ";;;;;;;;" +
                                                HttpUtility.UrlEncode(
                                                    Convert.ToString(tbl[rowOffset, ColumnOffset].Value));
                                            client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" +
                                                                    inputArray +
                                                                    "/" +
                                                                    agInstances[drpOntologySource.SelectedIndex].ID);
                                            request = new RestRequest();
                                            client.Get(request);
                                            inputArray1 =
                                                HttpUtility.UrlEncode(newSrcURI) + ";;;;;;;;" + HttpUtility.UrlEncode(
                                                    Convert.ToString(tbl[0, ColumnOffset].Value) + ";;;;;;;;" +
                                                    HttpUtility.UrlEncode(
                                                        Convert.ToString(tbl[rowOffset, ColumnOffset].Value)));

                                            // add type instance
                                            client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" +
                                                                    inputArray +
                                                                    "/" +
                                                                    agInstances[drpOntologySource.SelectedIndex].ID);
                                            request = new RestRequest();
                                            client.Get(request);
                                        }
                                        else
                                        {

                                            string inputArray1 =
                                                HttpUtility.UrlEncode(newSrcURI) + ";;;;;;;;" + HttpUtility.UrlEncode(
                                                    Convert.ToString(tbl[0, ColumnOffset].Value) + ";;;;;;;;" +
                                                    HttpUtility.UrlEncode(
                                                        Convert.ToString(tbl[rowOffset, ColumnOffset].Value)));

                                            // add type instance
                                            client = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" +
                                                                    inputArray +
                                                                    "/" +
                                                                    agInstances[drpOntologySource.SelectedIndex].ID);
                                            request = new RestRequest();
                                            client.Get(request);
                                        }
                                    }

                                    // client.Get(request);

                                }
                            }
                            else
                                {

                                    break;
                                }
                            

                            ColumnOffset += 1;
                        } while (string.IsNullOrEmpty(columnHeader2) == false);
                    }
                    else
                    {
                        ColumnOffset = 3;
                        rowOffset = 4;
                        do
                        {
                            columnHeader2 = Convert.ToString(tbl[1, ColumnOffset].Value);
                            bool foundIt = false;
                            int offset1 = 0;
                            foreach (DataRow dr in dataView.Rows)
                            {
                                if (Convert.ToString(tbl[3, ColumnOffset]) == "'xsd:string'")
                                {
                                    if (Convert.ToString(dr["object"]).Replace('<',' ').Replace('>',' ').Trim() == Convert.ToString(tbl[1, ColumnOffset]))
                                    {
                                        foundIt = true;
                                        break;
                                    }

                                    offset1 += 1; 
                                }
                            }

                            var request1 = new RestRequest();
                            string origArray = HttpUtility.UrlEncode(Convert.ToString(dataView.Rows[offset1]["name"])) + ";;;;;;;;" + HttpUtility.UrlEncode(
                                Convert.ToString(tbl[0, ColumnOffset].Value) + ";;;;;;;;" +
                                HttpUtility.UrlEncode(
                                    Convert.ToString(dataView.Rows[offset1]["predicate"])));
                            string inputArray =
                                HttpUtility.UrlEncode(Convert.ToString(dataView.Rows[offset1]["name"])) + ";;;;;;;;" + HttpUtility.UrlEncode(
                                    Convert.ToString(tbl[0, ColumnOffset].Value) + ";;;;;;;;" +
                                    HttpUtility.UrlEncode(
                                        Convert.ToString(tbl[rowOffset, ColumnOffset].Value)));
                            var client1 = new RestClient();
                            if (foundIt)
                                client1 = new RestClient(urlAddress + "api/UpdateInstance/updateStatement/" + HttpUtility.UrlEncode(origArray.Trim()) + "/" + HttpUtility.UrlEncode(inputArray.Trim()) + "/ " + agInstances[drpOntologySource.SelectedIndex].ID);
                            if (!foundIt)
                                client1 = new RestClient(urlAddress + "api/UpdateInstance/insertStatement/" +
                                                                                 inputArray +
                                                                                 "/" +
                                                                                 agInstances[drpOntologySource.SelectedIndex].ID); 

                            var response1 = client1.Get(request1);


                            //string columnHeader = Convert.ToString(tbl[0, ColumnOffset].Value);

                            //while (string.IsNullOrEmpty(columnHeader) == false)
                            //{
                            //    // insert the value
                            //    string value = Convert.ToString(tbl[rowOffset, ColumnOffset].Value);
                            //    var request = new RestRequest();
                            //    var client = new RestClient(urlAddress + "api/AddAgData/id/" +
                            //                                agInstances[drpOntologySource.SelectedIndex].ID + "/" +
                            //                                HttpUtility.UrlEncode(identifier) + "/" +
                            //                                HttpUtility.UrlEncode(columnHeader) + "/" +
                            //                                HttpUtility.UrlEncode(value));
                            //    // var response = client.Get(request);
                            //    // string s = response.Content.ToString();
                            //    System.Diagnostics.Debug.WriteLine(columnHeader + ", " + columnHeader + "," + value);
                            //    columnHeader = Convert.ToString(tbl[1, ColumnOffset]);
                                ColumnOffset += 1;

                            //}
                        } while (string.IsNullOrEmpty(columnHeader2) == false);
                    }

                    rowOffset += 1;
                    identifier = string.Empty;
                    identifier = Convert.ToString(tbl[rowOffset, 1].Value);
                    if (string.IsNullOrEmpty(identifier))
                    {
                        continueProcessing = false;
                    }
                }
            }


        }

        private void bnSaveChanges_Click_1(object sender, EventArgs e)
        {
            bnSaveChanges_Click(sender, e);
        }

        private void btnRunSparqlQuery_Click(object sender, EventArgs e)
        {
            cmbClassChoiceIndex = cmbClassChoice.SelectedIndex;
            cmbClassFilterIndex = cmbClassFilter.SelectedIndex;
            cmbRepoIndex = drpOntologySource.SelectedIndex;
            RunSparqlQuery wsq = new RunSparqlQuery();
            wsq.instanceID = agInstances[drpOntologySource.SelectedIndex].ID;


            wsq.ShowDialog();
        }

        private void gridControl1_MouseUp(object sender, MouseEventArgs e)
        {


        }

        private void grdInstanceDetails_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void btnCreateTemplate_Click_1(object sender, EventArgs e)
        {
            string textInfo = grdInstanceDetails.GetFocusedValue().ToString();
            if (textInfo.StartsWith("<"))
                textInfo = textInfo.Remove(0, 1);
            if (textInfo.EndsWith(">"))
            {
                textInfo = textInfo.Remove(textInfo.Length - 1);
            }
            Process.Start(textInfo);
        }

        public DataTable RetrieveURI(string type, string item = "")
        {
            DataTable tbl = new DataTable();
            if (string.IsNullOrEmpty(item))
            {
                if (type.ToLower() == "person")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://xmlns.com/foaf/0.1/Person"));
                }
                else if (type.ToLower() == "organization")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://xmlns.com/foaf/0.1/Organization"));
                }
                else if (type.ToLower() == "needtoknowconcerns")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://www.sdsconsortium.org/schemas/sds-okn.owl#NeedToKnowConcern"));
                }
                else if (type.ToLower() == "needtoknowquestions")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://www.sdsconsortium.org/schemas/sds-okn.owl#NeedToKnowQuestion"));
                }
                else if (type.ToLower() == "interview")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://purl.org/ontology/bibo/Interview"));
                }

                if (type.ToLower() == "project")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://vivoweb.org/ontology/core#Project"));
                }
                else if (type.ToLower() == "dataset")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://vivoweb.org/ontology/core#Dataset"));
                }

                if (type.ToLower() == "model")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://semanticscience.org/resource/SIO_000510"));
                }
                else if (type.ToLower() == "region")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://purl.obolibrary.org/obo/GEO_000000372"));
                }
                else if (type.ToLower() == "report")
                {
                    tbl = RetrieveTableViaSparql(LookupInformationUrl.Replace("%%REPLACEME%%",
                        "http://purl.org/ontology/bibo/Report"));
                }
            }
            else
            {
                string typeToUse = string.Empty; 
                if (type.ToLower() == "person")
                {
                    typeToUse = 
                        "http://xmlns.com/foaf/0.1/Person";
                }
                else if (type.ToLower() == "organization")
                {
                    typeToUse = 
                        "http://xmlns.com/foaf/0.1/Organization";
                }
                else if (type.ToLower() == "needtoknowconcerns")
                {
                    typeToUse = 
                        "http://www.sdsconsortium.org/schemas/sds-okn.owl#NeedToKnowConcern";
                }
                else if (type.ToLower() == "needtoknowquestions")
                {
                    typeToUse = 
                        "http://www.sdsconsortium.org/schemas/sds-okn.owl#NeedToKnowQuestion";
                }
                else if (type.ToLower() == "interview")
                {
                    typeToUse = 
                        "http://purl.org/ontology/bibo/Interview";
                }
                if (type.ToLower() == "project")
                {
                    typeToUse = 
                        "http://vivoweb.org/ontology/core#Project";
                }
                else if (type.ToLower() == "dataset")
                {
                    typeToUse = 
                        "http://vivoweb.org/ontology/core#Dataset";
                }
                if (type.ToLower() == "model")
                {
                    typeToUse = 
                        "http://semanticscience.org/resource/SIO_000510";
                }
                else if (type.ToLower() == "region")
                {
                    typeToUse = 
                        "http://purl.obolibrary.org/obo/GEO_000000372";
                }
                else if (type.ToLower() == "report")
                {
                    typeToUse =
                        "http://purl.org/ontology/bibo/Report";
                }

                string newInformation =
                    "select distinct ?name  ?label ?object ?predicate where { ?name rdf:type <%%REPLACEME%%> . ?name rdfs:label ?label . ?name <%%REPLACEME2%%> ?predicate . OPTIONAL {?name ?object ?predicate . }}";
                tbl = RetrieveTableViaSparql(newInformation.Replace("%%REPLACEME%%",
                    typeToUse).Replace("%%REPLACEME2%%", item));
            }

            return tbl;
        }

        public DataTable RetrieveTableViaSparql(string sparql)
        {
            string results = string.Empty;
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            DataTable dt = new DataTable();
            string PASSWORD = string.Empty;
            //   foreach (var item in Form1.agInstances)
            try
            {

                var item = Form1.agInstances[drpOntologySource.SelectedIndex];

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
                                              HttpUtility.UrlEncode(sparql.Replace('+', ' ')));

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

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in executing : " + HttpUtility.UrlEncode(sparql).Replace('+', ' ') +
                                    " - with error at : " + resultsItems + " with the error message of " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the outer portion: " + resultsItems + " from the request " +
                                HttpUtility.UrlEncode(sparql).Replace('+', ' ') +
                                " - with the error at : " + ex.Message);
            }

            return dt;
        }
    }

    public class InstanceItem
    {
        public string Name { get; set; }
        public List<string> PropertyNames { get; set; }
        public List<List<string>> PropertyValues { get; set; }
        public List<List<string>> OriginalPropertyValues { get; set; }

        public InstanceItem()
        {
            PropertyNames = new List<string>();
            PropertyValues = new List<List<string>>();
            OriginalPropertyValues = new List<List<string>>();
        }

        public InstanceItem(string name, List<string> propertyNames, List<List<string>> propertyValues)
        {
            Name = name;
            PropertyNames = new List<string>();
            PropertyValues = new List<List<string>>();
            OriginalPropertyValues = new List<List<string>>();
            foreach (var propertyName in propertyNames)
                PropertyNames.Add(propertyName);
            foreach (var values in propertyValues)
            {
                List<string> initValues = new List<string>();
                foreach (var item in values)
                {
                    initValues.Add(item);
                }
                propertyValues.Add(initValues);
            }
        }
    }

    public class ClassList
    {
        public string Name { get; set; }

        public string Uri { get; set; }

        public ClassList(string name, string uri)
        {
            Name = name;
            Uri = uri;
        }
    }

    public class AllegroGraphRegistryEntry
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public string Url { get; set; }

        public int Port { get; set; }

        public string Catalog { get; set; }
        public string Repository { get; set; }

        public AllegroGraphRegistryEntry()
        {
            ID = new Guid().ToString();
        }

        public AllegroGraphRegistryEntry(string name, string password, string url, string port, string catalog, string repository)
        {
            ID = new Guid().ToString();
            Name = name;
            Password = password;
            Url = url;
            Port = Convert.ToInt32(port);
            Catalog = catalog;
            Repository = repository;
        }


    }

    public class BoundObject
    {
        public int[] ids { get; set; }
        public string selectUri { get; set; }
        public string selectLabel { get; set; }
        public string UriToUse { get; set; }
        public string RetrieveUrlForPath(string selectUri)
        {
            RestClient getItem = new RestClient();
            return UriToUse;
        }
    }
    public static class Compressor
    {

        /// &lt;summary&gt;
        /// Use this to compress UTF-8 string to Base-64 string.
        /// &lt;/summary&gt;
        /// &lt;param name="text"&gt;The string value to compress.&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public static string Compress(this string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var stream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            memoryStream.Position = 0;
            var compressed = new byte[memoryStream.Length];
            memoryStream.Read(compressed, 0, compressed.Length);
            var gZipBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gZipBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// &lt;summary&gt;
        /// Use this to decompress Base-64 string to UTF-8 string.
        /// &lt;/summary&gt;
        /// &lt;param name="compressedText"&gt;&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public static string Decompress(this string compressedText)
        {
            try
            {
                var gZipBuffer = Convert.FromBase64String(compressedText);
                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                    var buffer = new byte[dataLength];
                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }
                    return Encoding.UTF8.GetString(buffer);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return compressedText;
            }
        }

 
    }
}
