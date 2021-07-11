#define RELEASE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Web;
using CefSharp;
using CefSharp.WinForms;
using DevExpress.DataAccess.DataFederation;
using DevExpress.XtraMap;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using RestSharp.Authenticators;
using DataColumn = System.Data.DataColumn;
using System.Xml;

namespace SDSClientAppV1
{
    public partial class DockInformationExplorer : DevExpress.XtraEditors.XtraUserControl
    {
#if RELEASE
        public string urlAddress = "http://maps.mvbg.org/";
#endif
#if DODEBUG
        public string urlAddress = @"https://localhost:51899/"; //"http://nfsservices1/";
#endif
        public ChromiumWebBrowser browserClass;
        private int counter;
        private int counter2;
        public HubConnection connection;
        public List<string> columnNames;
        public InstanceItem currentInstance;
        public DataTable tmpTable;
        public DockInformationExplorer()
        {
            InitializeComponent();
            columnNames = new List<string>();
            counter = 0;
            counter2 = 0;
        }

        private void traceAndDiagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockTrace.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockTrace.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
        }

        private void chatWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockChat.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockChat.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
        }

        public void init()
        {
            var settings = new CefSettings();
            columnNames = new List<string>();
            // Increase the log severity so CEF outputs detailed information, useful for debugging
            settings.LogSeverity = LogSeverity.Verbose;
            // By default CEF uses an in memory cache, to save cached data e.g. passwords you need to specify a cache path
            // NOTE: The executing user must have sufficient privileges to write to this folder.
            settings.CachePath = "cache";
            //  Cef.Initialize(settings);
            browserClass = new ChromiumWebBrowser("https://maps.mvbg.org/AgDisplay/Home/DisplayNodes");
            connection = new HubConnectionBuilder()
            .WithUrl("https://maps.mvbg.org/AgDisplay/ChatHub")
            .Build();

            var Initclient = new RestClient("https://localhost:59034/api/AllegroGraphInstance");

            OrgChart chart = new OrgChart();

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

            itemToAdd = new AllegroGraphRegistryEntry();
            Initclient = new RestClient("https://localhost:59034/api/AllegroGraphInstance/register/super/Show4time!/45.19.182.17/10035/importme/sdsokn");
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

            var client = new RestClient("https://localhost:59034/api/SDSClass/id/" + initID);
            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);
            browserClass.Dock = DockStyle.Fill;
            splitContainer3.Panel1.Controls.Add(browserClass);
            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            string[] firstSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
        }





        private void graphViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void mapViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockMaps.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockMaps.DockAsTab(panelContainer1);
        }

        private void resultsTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockTable.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockTable.DockAsTab(panelContainer1);
        }

        private void btnSubmitChat_Click(object sender, EventArgs e)
        {
            lstChatItems.Items.Add("User: " + txtChatRequest.Text);
            RestClient rep = new RestClient();
            rep.BaseUrl = new Uri("http://cici.lab.asu.edu:8999/");
            var assembly2a = Assembly.GetExecutingAssembly();
            string s = string.Empty;
            using (Stream binaryReader =
                assembly2a.GetManifestResourceStream("SDSClientAppV1.test.txt"))
            {
                using (StreamReader reader = new StreamReader(binaryReader))
                {
                    s= reader.ReadToEnd();
                }

             //   s = System.IO.File.ReadAllText(@"C:\t\test.txt");
            }

            rep.ClearHandlers();
            if (txtChatRequest.Text.ToLower().Contains("polarhub"))
            {
                DataTable tbl1 = new DataTable();
                string search1 = s.Replace("%TEST%", txtChatRequest.Text.Replace("from polarhub find ","").Trim());
                //    rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
                //    rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
                rep.AddDefaultHeader("Accept", "*/*");
                rep.AddDefaultHeader("Accept-Encoding", "gzip, deflate,br");
                rep.AddDefaultHeader("Content-Type", "application/xml");
                var request = new RestRequest("service=CSW&version=2.0.2");

                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/xml; charset=utf-8", search1, ParameterType.RequestBody);
                IRestResponse response = rep.Execute(request);
                string results = response.Content.ToString(); // + Environment.NewLine;
                lstCommands.Items.Add("Results from polar hub query: " + txtChatRequest.Text);
                
                lstCommands.Items.Add(results);
                tbl1.Columns.Add(new DataColumn("Title",System.Type.GetType("System.String")));
                tbl1.Columns.Add(new DataColumn("BoundingBox_Upper", System.Type.GetType("System.String")));
                tbl1.Columns.Add(new DataColumn("BoundingBox_Lower", System.Type.GetType("System.String")));
                tbl1.Columns.Add(new DataColumn("Abstract", System.Type.GetType("System.String")));
                GetRecordsResponse responses = new GetRecordsResponse();
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(GetRecordsResponse));
                using (System.IO.StringReader reader = new System.IO.StringReader(results))
                {
                    responses = (GetRecordsResponse) (serializer.Deserialize(reader));
                }

                Console.WriteLine(responses.SearchResults.Record.Count());
                lstChatItems.Items.Add("    From PolarHub, we have " +
                                       Convert.ToString(responses.SearchResults.numberOfRecordsMatched));
                int co1 = 1;
                foreach (var responseItem in responses.SearchResults.Record)
                {
                    lstChatItems.Items.Add("    " + co1 + " : result name is " + responseItem.title);
                    try
                    {
                        DataRow drNew = tbl1.NewRow();
                        drNew["Title"] = responseItem.title;
                        drNew["BoundingBox_Upper"] = responseItem.BoundingBox.UpperCorner;
                        drNew["BoundingBox_Lower"] = responseItem.BoundingBox.LowerCorner;
                        drNew["Abstract"] = responseItem.@abstract;
                        tbl1.Rows.Add(drNew);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    co1 += 1; 
                }

                if (mapControl1.Layers.Count > 1)
                {
                    mapControl1.Layers.RemoveAt(1);
                }
                VectorItemsLayer itemsLayer = new VectorItemsLayer();
                MapItemStorage storage = new MapItemStorage();
                grdChatResults.DataSource = tbl1; 
                co1 = 1;
                foreach (var responseItem in responses.SearchResults.Record)
                {
                    try
                    {
                        MapPolygon poly = new MapPolygon();
                        string[] lowerpoints = responseItem.BoundingBox.LowerCorner.Split(' ');
                        string[] upperpoints = responseItem.BoundingBox.UpperCorner.Split(' ');
                        double area = ((Convert.ToDouble(upperpoints[1]) - Convert.ToDouble(lowerpoints[1])) *
                                       (Convert.ToDouble(upperpoints[0]) - Convert.ToDouble(lowerpoints[0])));
                        var point1 = new GeoPoint(Convert.ToDouble(lowerpoints[0]), Convert.ToDouble(lowerpoints[1]));
                       var point2 = new GeoPoint(Convert.ToDouble(lowerpoints[0]), Convert.ToDouble(upperpoints[1]));
                        var point3 = new GeoPoint(Convert.ToDouble(upperpoints[0]), Convert.ToDouble(upperpoints[1]));
                        var point4 = new GeoPoint(Convert.ToDouble(upperpoints[0]), Convert.ToDouble(lowerpoints[1]));
                        GeoPoint[] points = new GeoPoint[]
                        {
                            point1, point2, point3, point4
                        };
                        poly = CreatePolygon(area, responseItem.title, points);
                        
                        storage.Items.Add(poly);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    co1 += 1;
                }

                itemsLayer.Data = storage;
                itemsLayer.Colorizer = CreateColorizer();
                mapControl1.Layers.Add(itemsLayer);
                mapControl1.Legends.Add(CreateLegend(itemsLayer));
                
            }
            else
            {
                string sparql = txtChatRequest.Text;
                DataTable tbl1 = new DataTable();
                if (txtChatRequest.Text.ToLower() ==
                    "what data is available for dissolved oxygen in the hood canal subwatershed of the puget sound")
                {
                    string querystring =
                        "SELECT ?dstitleString ?urlString WHERE {?dataset  ?pred ?dstitle . bind( str( ?dstitle ) as ?dstitleString)?pred rdfs:label ?mypred . FILTER (str(?mypred) = 'has title' ) . FILTER( regex(?dstitle, 'oxygen', 'i' ))  . ?dataset ?predhasurl ?url . bind( str( ?url ) as ?urlString) ?predhasurl rdfs:label ?xpred FILTER (str(?xpred) = 'has URL').}";

                    tbl1 = runQuery(querystring);
                    string tstring = string.Empty; 
                    foreach (DataColumn dc in tbl1.Columns)
                    {
                        tstring += "    " + dc.ColumnName;
                    }

                    lstChatItems.Items.Add(tstring);
                    foreach (DataRow dr in tbl1.Rows)
                    {
                        string tstring2 = string.Empty;
                        foreach (DataColumn dc in tbl1.Columns)
                        {
                            tstring2 += "    " + Convert.ToString(dr[dc.ColumnName]);
                        }
                        lstChatItems.Items.Add(tstring2);
                    }
                }
                if (txtChatRequest.Text.ToLower() ==
                    "who works on modeled dissolved oxygen")
                {
                    string querystring =
                        "SELECT ?person  ?personhomepage ?modelname ?datasettitle WHERE { ?dataset  ?pred ?dstitle . BIND( str(?dstitle)  as ?datasettitle ) ?pred rdfs:label ?mypred . FILTER (str(?mypred) = 'has title' ) . FILTER( regex(?dstitle, 'oxygen', 'i' ))  . ?dataset ?dspred ?_model . ?_model   ?predhsn ?_modelname . ?predhsn rdfs:label ?mypredhsn . FILTER (str(?mypredhsn) = 'has name' ) BIND( str(?_modelname)  as ?modelname ) ?predhsn rdfs:label ?mypredhsn . FILTER (str(?mypredhsn) = 'has name' ) ?dspred rdfs:label ?mydspred . FILTER (str(?mydspred) = 'has input' ) ?_person ?predwo ?_model . ?_person  rdfs:label ?personLabel . BIND( str(?personLabel)  as ?person ) ?predwo rdfs:label ?mypredwo . FILTER (str(?mypredwo) = 'works on' ) . ?_person  foaf:homepage  ?_homepage . BIND( str(?_homepage)  as ?personhomepage ) }";

                    tbl1 = runQuery(querystring);
                    string tstring = string.Empty;
                    foreach (DataColumn dc in tbl1.Columns)
                    {
                        tstring += "    " + dc.ColumnName;
                    }

                    lstChatItems.Items.Add(tstring);
                    foreach (DataRow dr in tbl1.Rows)
                    {
                        string tstring2= string.Empty;
                        foreach (DataColumn dc in tbl1.Columns)
                        {
                            tstring2 += "    " + Convert.ToString(dr[dc.ColumnName]);
                        }
                        lstChatItems.Items.Add(tstring2);
                    }
                }



                grdChatResults.DataSource = tbl1;
                // do sparql search
            }
            //if (counter == 0)
            //{

            //    lstChatItems.Items.Add("      The answer to who is directly addressing the concern - 'My concern is low dissolved oxygen levels in the Puget Sound fresh and marine waters':");
            //    lstChatItems.Items.Add("           Sheelagh Martin McCarthy from WA Ecology");
            //    lstChatItems.Items.Add("           Teizeen Mohamedali from WA Ecology");
            //    lstCommands.Items.Add("Generate sparql query for ntk questions");
            //    lstCommands.Items.Add("Generate sparql query for concerns of");
            //    lstCommands.Items.Add("Generate sparql query for people");
            //    lstCommands.Items.Add("Generate sparql query for organization");
            //}
            //else if (counter == 1)
            //{
            //    lstChatItems.Items.Add("      The answer to what NTK Questions address the concern - 'What are existing levels of DO in marine waters':");
            //    lstChatItems.Items.Add(".         How much are DO levels under natural conditions in rivers");
            //    lstChatItems.Items.Add(".         How much of DO issues are due to Human and water treatment");
            //    lstChatItems.Items.Add(".         What are causes of low DO in rivers");
            //    lstCommands.Items.Add("Generate sparql query for ntk questions");
            //    lstCommands.Items.Add("Generate sparql query for concerns of");
            //    lstCommands.Items.Add("Generate sparql query for addresses");

            //}
            //else if (counter == 2)
            //{
            //    lstChatItems.Items.Add("      The answer to who works on models that can answer the ntk question 'What are existing levels of DO in marine waters':");
            //    lstChatItems.Items.Add(".         Tarang Khangaonkarat - PNNL");
            //    lstChatItems.Items.Add(".         Parker McCready - University of Washington");
            //    lstCommands.Items.Add("Generate sparql query for ntk questions");
            //    lstCommands.Items.Add("Generate sparql query for person");
            //    lstCommands.Items.Add("Generate sparql query for organizations");



            //    picMapView1.Image = Image.FromFile(@"C:\maps\initialInput.png");
            //}
            ////else if (counter == 3)
            ////{
            ////    lstChatItems.Items.Add("      Opening associated workflow");
            ////    lstCommands.Items.Add("Opening Workflow Explorer");
            ////    Process.Start(
            ////        @"C:\Program Files\Microsoft Project Trident - A Scientific Workflow Workbench\WorkflowComposer.exe");

            ////}
            ////else if (counter == 4)
            ////{
            ////    lstCommands.Items.Add("Loading workflow file");
            ////    Application.DoEvents();
            ////    Thread.Sleep(2500);

            ////    lstCommands.Items.Add("Submitted for processing");
            ////    Application.DoEvents();
            ////    Thread.Sleep(3500);
            ////    Application.DoEvents();
            ////    lstCommands.Items.Add("Success");
            ////    lstChatItems.Items.Add("      Processed workflow, please a select a result map or all:");
            ////    lstChatItems.Items.Add("                                                               FireProportion");
            ////    lstChatItems.Items.Add("                                                               FireProportionIsHigh");
            ////    lstChatItems.Items.Add("                                                               VegDeparture");
            ////    lstChatItems.Items.Add("                                                               VegDepartureIsHigh");
            ////    lstChatItems.Items.Add("                                                               SuddenVegChangeIsHigh");



            ////}

            ////if (counter == 0)
            ////{

            ////    lstChatItems.Items.Add("      List of Workflow Models : 1) Potential For Sudden Vegetation Change");
            ////    lstCommands.Items.Add("Generate sparql query for registered models");
            ////}
            ////else if (counter == 1)
            ////{
            ////    lstChatItems.Items.Add("      Model: 'Potential For Sudden Vegetation Change' selected");
            ////    lstCommands.Items.Add("Model selected: Potential For Sudden Vegetation Change");

            ////}
            ////else if (counter == 2)
            ////{
            ////    lstChatItems.Items.Add("      The input map appears below");
            ////    lstCommands.Items.Add("Generated sparql query,now loading specified map associated with model");
            ////    picMapView1.Image = Image.FromFile(@"C:\maps\initialInput.png");
            ////}
            ////else if (counter == 3)
            ////{
            ////    lstChatItems.Items.Add("      Opening associated workflow");
            ////    lstCommands.Items.Add("Opening Workflow Explorer");
            ////    Process.Start(
            ////        @"C:\Program Files\Microsoft Project Trident - A Scientific Workflow Workbench\WorkflowComposer.exe");

            ////}
            ////else if (counter == 4)
            ////{
            ////    lstCommands.Items.Add("Loading workflow file");
            ////    Application.DoEvents();
            ////    Thread.Sleep(2500);

            ////    lstCommands.Items.Add("Submitted for processing");
            ////    Application.DoEvents();
            ////    Thread.Sleep(3500);
            ////    Application.DoEvents();
            ////    lstCommands.Items.Add("Success");
            ////    lstChatItems.Items.Add("      Processed workflow, please a select a result map or all:");
            ////    lstChatItems.Items.Add("                                                               FireProportion");
            ////    lstChatItems.Items.Add("                                                               FireProportionIsHigh");
            ////    lstChatItems.Items.Add("                                                               VegDeparture");
            ////    lstChatItems.Items.Add("                                                               VegDepartureIsHigh");
            ////    lstChatItems.Items.Add("                                                               SuddenVegChangeIsHigh");



            ////}
            ////else if (counter == 5)
            ////{
            ////    lstCommands.Items.Add("Displaying map image");

            ////    lstChatItems.Items.Add("      Map Displayed Below.");

            ////    picMapView1.Image = Image.FromFile(@"C:\mapsorig\SuddenVegChangePotentialIsHigh.png");
            ////}
            ////else if (counter == 6)
            ////{
            ////    lstCommands.Items.Add("Displaying map image");

            ////    lstChatItems.Items.Add("      Map Displayed Below.");
            ////    picMapView1.Image = Image.FromFile(@"C:\mapsorig\FireProportionIsHigh.png");
            ////}
            ////else if (counter == 7)
            ////{
            ////    lstCommands.Items.Add("Read Model for Parameters");
            ////    lstChatItems.Items.Add("      Model Parameters: True Threshold");
            ////    lstChatItems.Items.Add("      Model Parameters: False Threshold");
            ////}
            ////else if (counter == 8)
            ////{
            ////    lstCommands.Items.Add("Scenario Update Created");
            ////    lstChatItems.Items.Add("      Scenario 'Update' Created");
            ////}
            ////else if (counter == 9)
            ////{
            ////    lstCommands.Items.Add("Updating model parameter true threshold by 20%");
            ////    lstChatItems.Items.Add("      Model Parameters: True Threshold increased by 20%");
            ////}
            ////else if (counter == 10)
            ////{
            ////    lstCommands.Items.Add("Updating model parameter false threshold by 20%");
            ////    lstChatItems.Items.Add("      Model Parameters: False Threshold increased by 20%");
            ////}
            ////else if (counter == 11)
            ////{
            ////   lstCommands.Items.Add("Updating Workflow Script");
            ////    Application.DoEvents();
            ////    lstCommands.Items.Add("Saved to file PotentialForSuddenVegSuddenVegChange_Update.mpt");
            ////    lstChatItems.Items.Add("      Saved new model to: PotentialForSuddenVegSuddenVegChange_Update.mpt");

            ////}
            ////else if (counter == 12)
            ////{

            ////    lstChatItems.Items.Add("      List of Workflow Models : 1) Potential For Sudden Vegetation Change");
            ////    lstChatItems.Items.Add("                                2) Potential For Sudden Vegetation Change - Updated");
            ////    lstCommands.Items.Add("Generate sparql query for registered models");
            ////}
            ////else if (counter == 13)
            ////{
            ////    lstChatItems.Items.Add("      Model: 'Potential For Sudden Vegetation Change - Updated' selected");
            ////    lstCommands.Items.Add("Model selected: Potential For Sudden Vegetation Change - Updated");
            ////}
            ////else if (counter == 14)
            ////{
            ////    lstCommands.Items.Add("Loading workflow file");
            ////    Application.DoEvents();
            ////    Thread.Sleep(2500);

            ////    lstCommands.Items.Add("Submitted for processing");
            ////    Application.DoEvents();
            ////    Thread.Sleep(3500);
            ////    Application.DoEvents();
            ////    lstCommands.Items.Add("Success");
            ////    lstChatItems.Items.Add("      Processed workflow, please a select a result map or all:");
            ////    lstChatItems.Items.Add("                                                               FireProportion");
            ////    lstChatItems.Items.Add("                                                               FireProportionIsHigh");
            ////    lstChatItems.Items.Add("                                                               VegDeparture");
            ////    lstChatItems.Items.Add("                                                               VegDepartureIsHigh");
            ////    lstChatItems.Items.Add("                                                               SuddenVegChangeIsHigh");

            ////}
            ////else if (counter == 15)
            ////{
            ////    lstCommands.Items.Add("Displaying map image");

            ////    lstChatItems.Items.Add("      Map Displayed Below.");

            ////    picMapView1.Image = Image.FromFile(@"C:\maps\SuddenVegChangePotentialIsHigh.png");
            ////}
            ////else if (counter == 16)
            ////{
            ////    lstCommands.Items.Add("Displaying map image");

            ////    lstChatItems.Items.Add("      Map Displayed Below.");
            ////    picMapView1.Image = Image.FromFile(@"C:\maps\FireProportionIsHigh.png");
            ////}
            ////else if (counter == 17)
            ////{
            ////    lstCommands.Items.Add("Displaying map comparison image");

            ////    lstChatItems.Items.Add("      Map Displayed Below.");
            ////    picMapView1.Image = Image.FromFile(@"C:\mapsorig\SuddenVegChangePotentialIsHighCompare.png");
            ////}
            ////else if (counter == 18)
            ////{
            ////    lstCommands.Items.Add("Displaying map image");

            ////    lstChatItems.Items.Add("      Map Displayed Below.");

            ////    picMapView1.Image = Image.FromFile(@"C:\mapsorig\FireProportionIsHighCompare.png");
            ////}
            lstChatItems.SetSelected(lstChatItems.Items.Count - 1, true);

            //This deselects the last line
            lstChatItems.SetSelected(lstChatItems.Items.Count - 1, false);

            lstCommands.SetSelected(lstCommands.Items.Count - 1, true);

            //This deselects the last line
            lstCommands.SetSelected(lstCommands.Items.Count - 1, false);
            counter += 1;
            //if(txtChatRequest.Text.ToLower().Contains("who is working at wa ecology"))
            //{
            //    lstChatItems.Items.Add("Question: " + txtChatRequest.Text);
            //    lstChatItems.Items.Add("  Answer:");
            //    string uri = string.Empty;
            //    InstanceItem currentItem = new InstanceItem();

            //    lstCommands.Items.Add(@"First step is to look at instances of person: select distinct ?x ?pred ?object  where{ {?x ?pred ?y} .   OPTIONAL { ?x rdfs:label? label } . FILTER(?y IN(< " + HttpUtility.UrlDecode(uri) + " >)).}");
            //    lstCommands.Items.Add(@"Second step is to filter at properties works at for each person: select distinct ?x ?pred ?label  where{ {?x a nameToLookup} .   OPTIONAL { ?x rdfs:label? label } . FILTER(?y IN(< " + HttpUtility.UrlDecode(uri) + " >)).}");
            //    lstCommands.Items.Add(@"Final step is to filter at instances of organization 'WAEcology': select distinct ? x ? label  where{ {?x? pred ?y} .   OPTIONAL { ?x rdfs:label? label } . FILTER(?y IN(< " + HttpUtility.UrlDecode(uri) + " >)).}");
            //    List<string> itemsToview = new List<string>();
            //    itemsToview.Add("<http://www.sdsconsortium.org/schemas/sds-okn.owl#WQIntv000040005>");
            //    itemsToview.Add("<http://www.sdsconsortium.org/schemas/sds-okn.owl#WQIntv000040001>");
            //    lstCommands.Items.Add("Executing sparql query: " + @"select distinct ?x ?label  where{{?x ?pred ?y} .   OPTIONAL { ?x rdfs:label ?label } . FILTER (?y IN (<" + HttpUtility.UrlDecode(uri) + ">) ) .}");
            //    var client = new RestClient("http://45.19.182.24/agCaller/api/AgInstanceInfo/uri/" + HttpUtility.UrlEncode(uri.Trim()) + "/ " + Form1.agInstances[0].ID);
            //    var request = new RestRequest();

            //    var response = client.Get(request);
            //    string s = response.Content.ToString();
            //    System.Diagnostics.Debug.WriteLine(s);

            //    string[] args = { "**********" };
            //    string[] args2 = { ";;;;;;;;;;;" };
            //    string[] firstSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            //    DataTable tbl = new DataTable();
            //    List<string> columnNames = new List<string>();

            //    //foreach (string line in firstSplit)
            //    //{
            //    //    string[] items = line.Split(args2, StringSplitOptions.RemoveEmptyEntries);
            //    //    if (items.Length > 2)
            //    //    {
            //    //        if (items[2] != "----")
            //    //        {
            //    //            columnNames.Add(items[2]);
            //    //        }
            //    //    }

            //    //}
            //    tbl.Columns.Add("PropertyName", Type.GetType("System.String"));
            //    tbl.Columns.Add("Value", Type.GetType("System.String"));
            //    DataRow dr = tbl.NewRow();
            //    lstChatItems.Items.Add("         label :  Derek Day");
            //    lstChatItems.Items.Add("             lastName:  Day");
            //    lstChatItems.Items.Add("             homepage :  Derek Day");
            //}
            //if (txtChatRequest.Text.ToLower().Contains("who"))
            //{
            //    string searchItem = txtChatRequest.Text.Replace("who is"," ").Replace("Who is"," ").Trim();
            //    string uri = string.Empty;
            //    InstanceItem currentItem = new InstanceItem();
            //    foreach (var item in Form1.instanceList)
            //    {
            //        // get the uri to search
            //        if (searchItem == item.Name)
            //        {
            //            int offsetUri = 0;
            //            foreach (var propName in item.PropertyNames)
            //            {
            //                if (propName.ToLower().Trim() == "uri")
            //                {
            //                    uri = Convert.ToString(item.PropertyValues[offsetUri][0]);
            //                    break;
            //                }
            //                offsetUri += offsetUri;
            //            }
            //            currentItem = item;

            //            break;
            //        }
            //    }
            //    lstCommands.Items.Add("Executing sparql query: " + @"select distinct ?x ?label  where{{?x ?pred ?y} .   OPTIONAL { ?x rdfs:label ?label } . FILTER (?y IN (<" + HttpUtility.UrlDecode(uri) + ">) ) .}");
            //    var client = new RestClient("http://45.19.182.24/agCaller/api/AgInstanceInfo/uri/" + HttpUtility.UrlEncode(uri.Trim()) + "/ " + Form1.agInstances[0].ID);
            //    var request = new RestRequest();

            //    var response = client.Get(request);
            //    string s = response.Content.ToString();
            //    System.Diagnostics.Debug.WriteLine(s);

            //    string[] args = { "**********" };
            //    string[] args2 = { ";;;;;;;;;;;" };
            //    string[] firstSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            //    DataTable tbl = new DataTable();
            //    List<string> columnNames = new List<string>();

            //    foreach (string line in firstSplit)
            //    {
            //        string[] items = line.Split(args2, StringSplitOptions.RemoveEmptyEntries);
            //        if (items.Length > 2)
            //        {
            //            if (items[2] != "----")
            //            {
            //                columnNames.Add(items[2]);
            //            }
            //        }

            //    }
            //    tbl.Columns.Add("PropertyName", Type.GetType("System.String"));
            //    tbl.Columns.Add("Value", Type.GetType("System.String"));
            //    DataRow dr = tbl.NewRow();
            //    int offset = 0;
            //    Dictionary<string, string> FinalValues = new Dictionary<string, string>();
            //    foreach (string cName in columnNames)
            //    {

            //        string[] items = firstSplit[offset].Split(args2, StringSplitOptions.RemoveEmptyEntries);
            //        string nds = string.Empty;
            //        if (items.Length > 2)
            //        {
            //            if (items[2] != "----")
            //            {
            //                if (items[3] != "----")
            //                {
            //                    nds = items[3];
            //                }
            //            }
            //        }
            //        else if (items.Length == 2)
            //        {
            //            nds = items[1];
            //        }

            //        if (currentItem.PropertyNames.Contains(cName) == false)
            //        {
            //            currentItem.PropertyNames.Add(cName);
            //            List<string> tempValue = new List<string>();
            //            if (items.Length > 2)
            //                tempValue.Add(nds + " ------ " + items[1]);
            //            else
            //                tempValue.Add(nds + " ------ " + items[0]);
            //            currentItem.PropertyValues.Add(tempValue);
            //        }
            //        else
            //        {
            //            int myOffset = 0;
            //            foreach (string sName in currentItem.PropertyNames)
            //            {
            //                if (sName == cName)
            //                {

            //                    if (items.Length > 2)
            //                        currentItem.PropertyValues[myOffset].Add(nds + " ------ " + items[2]);
            //                    else
            //                        currentItem.PropertyValues[myOffset].Add(nds + " ------ " + items[0]);

            //                    break;
            //                }
            //                myOffset += 1;
            //            }
            //        }
            //        bool found = false;


            //        foreach (KeyValuePair<string, string> item in FinalValues)
            //        {
            //            if (item.Key == cName)
            //            {

            //                found = true;
            //                FinalValues[item.Key] = item.Value + Environment.NewLine + nds;
            //                break;
            //            }

            //        }
            //        if (!found)
            //        {
            //            FinalValues.Add(cName, nds);
            //        }
            //        offset += 1;
            //    }
            //    lstChatItems.Items.Add("  Answer:");
            //    foreach (KeyValuePair<string, string> item in FinalValues)
            //    {
            //        dr = tbl.NewRow();
            //        dr[0] = item.Key;
            //        dr[1] = item.Value;
            //        tbl.Rows.Add(dr);
            //        lstChatItems.Items.Add("    " + item.Key + ":" + item.Value);
            //    }

            //    grdChatResults.DataSource = tbl;
            //    dockGraph.Enabled = false;
            //    dockMaps.Enabled = false;
            //    dockTable.Enabled = true; 

            //}
            //else if (txtChatRequest.Text.ToLower().Contains("run"))
            //{
            //    lstCommands.Items.Add("Running script");

            //}
            //else if (txtChatRequest.Text.ToLower().Contains("model"))
            //{
            //    lstCommands.Items.Add("Looking up model information");

            //}
            txtChatRequest.Text = string.Empty;
        }

        public DataTable runQuery(string querystring)
        {
            DataTable dt = new DataTable();
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


                var item = Form1.agInstances[1];

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
                                              HttpUtility.UrlEncode(querystring));

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
                    //listBoxControl1.Items.Add(textBox1.Text + " - (" + totalNumberofRecords.ToString() + ")");

                    //gridControl1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in executing : " +  ' ' +
                                    " - with error at : " + resultsItems + " with the error message of " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the outer portion: " + resultsItems + " from the request  - with the error at : " + ex.Message);
            }

            return dt;
        }
       
        private MapPolygon CreatePolygon(double areaValue, string polygonName, GeoPoint[] points)
        {
            MapPolygon item = new MapPolygon();

            item.Attributes.Add(new MapItemAttribute()
            {
                Name = areaValueAttrName,
                Type = typeof(double),
                Value = areaValue
            });
            item.Attributes.Add(new MapItemAttribute()
            {
                Name = polygonNameAttrName,
                Type = typeof(string),
                Value = polygonName
            });

            item.ToolTipPattern = "{" + polygonNameAttrName + "}=<b>{" + areaValueAttrName + "}</b>";

            foreach (GeoPoint point in points)
            {
                item.Points.Add(point);
            }

            return item;
        }

const string areaValueAttrName = "AreaValue";
const string polygonNameAttrName = "PolygonName";
        private MapColorizer CreateColorizer()
        {
            ChoroplethColorizer colorizer = new ChoroplethColorizer();
            colorizer.ValueProvider = new ShapeAttributeValueProvider
            {
                AttributeName = areaValueAttrName
            };

            colorizer.RangeStops.AddRange(new List<double> { 0, 1000, 2000 });
            colorizer.ColorItems.AddRange(new List<ColorizerColorItem> {
                new ColorizerColorItem(Color.Yellow),
                new ColorizerColorItem(Color.Red)
            });

            return colorizer;
        }

        private MapLegendBase CreateLegend(MapItemsLayerBase layer)
        {
            ColorScaleLegend legend = new ColorScaleLegend();
            legend.Header = "Area";
            legend.Layer = layer;
            return legend;
        }

        private void cmbClassType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbInstances_SelectedValueChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            string node = ((DevExpress.XtraEditors.ComboBoxEdit)sender).Text;
            string uri = string.Empty;
            foreach (var item in Form1.masterClassList)
            {
                if (node == item.Name)
                {
                    uri = item.Uri.Trim();

                }
            }
            if (string.IsNullOrEmpty(uri))
                return;
            var client = new RestClient("https://localhost:59034/api/SDSInstance/uri/" + HttpUtility.UrlEncode(uri.Trim()) + "/" + Form1.agInstances[0].ID);
            var request = new RestRequest();

            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);
            cmbInstanceName.Properties.Items.Clear();

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            try
            {
                cmbInstanceName.SelectedIndexChanged -= cmbInstances_SelectedValueChanged;
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

            cmbInstanceName.SelectedIndexChanged += cmbInstances_SelectedValueChanged;
            this.Cursor = Cursors.Default;
            Application.DoEvents();
            if (cmbInstanceName.Properties.Items.Count > 0)
            {
                cmbInstances_SelectedValueChanged(sender, e);



            }
            this.Cursor = Cursors.Default;
        }

        private void cmbInstances_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            string node = ((DevExpress.XtraEditors.ComboBoxEdit)sender).Text;
            string uri = string.Empty;
            currentInstance = new InstanceItem();
            foreach (var item in Form1.instanceList)
            {
                // get the uri to search
                if (node == item.Name)
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
            var client = new RestClient("https://localhost:59034/api/AgInstanceInfo/uri/" + HttpUtility.UrlEncode(uri.Trim()) + "/ " + Form1.agInstances[0].ID);
            var request = new RestRequest();

            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };
            try
            {
                //  cmbInstances.SelectedIndexChanged -= cmbInstances_SelectedValueChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            string[] firstSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            DataTable tbl = new DataTable();
            columnNames = new List<string>();

            foreach (string line in firstSplit)
            {
                string[] items = line.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length > 2)
                {
                    if (items[2] != "----")
                    {
                        columnNames.Add(items[2]);
                    }
                }

            }
            tbl.Columns.Add("PropertyName", Type.GetType("System.String"));
            tbl.Columns.Add("Value", Type.GetType("System.String"));
            tmpTable = new DataTable();
            tmpTable.Columns.Add("PropertyName", Type.GetType("System.String"));
            tmpTable.Columns.Add("Value", Type.GetType("System.String"));
            Dictionary<string, string> FinalValues = new Dictionary<string, string>();
            DataRow dr = tbl.NewRow();
            int offset = 0;
            foreach (string cName in columnNames)
            {

                string[] items = firstSplit[offset].Split(args2, StringSplitOptions.RemoveEmptyEntries);
                string nds = string.Empty;
                if (items.Length > 2)
                {
                    if (items[2] != "----")
                    {
                        if (items[3] != "----")
                        {
                            nds = items[3];
                        }
                    }
                }
                else if (items.Length == 2)
                {
                    nds = items[1];
                }

                if (currentInstance.PropertyNames.Contains(cName) == false)
                {
                    currentInstance.PropertyNames.Add(cName);
                    List<string> tempValue = new List<string>();
                    if (items.Length > 2)
                        tempValue.Add(nds + " ------ " + items[1]);
                    else
                        tempValue.Add(nds + " ------ " + items[0]);
                    currentInstance.PropertyValues.Add(tempValue);
                }
                else
                {
                    int myOffset = 0;
                    foreach (string sName in currentInstance.PropertyNames)
                    {
                        if (sName == cName)
                        {

                            if (items.Length > 2)
                                currentInstance.PropertyValues[myOffset].Add(nds + " ------ " + items[2]);
                            else
                                currentInstance.PropertyValues[myOffset].Add(nds + " ------ " + items[0]);

                            break;
                        }
                        myOffset += 1;
                    }
                }
                bool found = false;

                foreach (KeyValuePair<string, string> item in FinalValues)
                {
                    if (item.Key == cName)
                    {

                        found = true;
                        FinalValues[item.Key] = item.Value + Environment.NewLine + nds;
                        break;
                    }

                }
                if (!found)
                {
                    FinalValues.Add(cName, nds);
                }
                offset += 1;
            }
            DataRow drCopy = tmpTable.NewRow();
            foreach (KeyValuePair<string, string> item in FinalValues)
            {
                dr = tbl.NewRow();
                dr[0] = item.Key;
                dr[1] = item.Value;
                tbl.Rows.Add(dr);
                drCopy = tmpTable.NewRow();
                drCopy[0] = item.Key;
                drCopy[1] = item.Value;
                tmpTable.Rows.Add(drCopy);

            }

            gridControl1.DataSource = tbl;

            grdInstanceDetails.OptionsView.RowAutoHeight = true;
            this.Cursor = Cursors.Default;
            Application.DoEvents();
            try
            {
                //cmbInstances.SelectedIndexChanged += cmbInstances_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            this.Cursor = Cursors.Default;

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string uri = string.Empty;
            if (counter2 == 0)
            {
                init();

            }
            else if (counter2 == 1)
            {
                uri = @"<http://www.sdsconsortium.org/schemas/sds-okn.owl#WQIntv000010004>";
            }
            else if (counter2 == 2)
            {
                uri = @"<http://www.sdsconsortium.org/schemas/sds-okn.owl#WQIntv000060004>";
            }
            else if (counter2 == 3)
            {
                uri = @"<http://www.sdsconsortium.org/schemas/sds-okn.owl#WQIntv000040005>";
            }

            if (counter2 == 0)
            {
                counter2 += 1;
                return;
            }
            var client = new RestClient("https://localhost:59034/api/AgInstanceInfo/uri/" + HttpUtility.UrlEncode(uri.Trim()) + "/ " + Form1.agInstances[0].ID);
            var request = new RestRequest();

            var response = client.Get(request);
            string s = response.Content.ToString();
            System.Diagnostics.Debug.WriteLine(s);

            string[] args = { "**********" };
            string[] args2 = { ";;;;;;;;;;;" };


            string[] firstSplit = s.Split(args, StringSplitOptions.RemoveEmptyEntries);
            DataTable tbl = new DataTable();
            columnNames = new List<string>();

            foreach (string line in firstSplit)
            {
                string[] items = line.Split(args2, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length > 2)
                {
                    if (items[2] != "----")
                    {
                        columnNames.Add(items[2]);
                    }
                }

            }
            tbl.Columns.Add("PropertyName", Type.GetType("System.String"));
            tbl.Columns.Add("Value", Type.GetType("System.String"));
            tmpTable = new DataTable();
            tmpTable.Columns.Add("PropertyName", Type.GetType("System.String"));
            tmpTable.Columns.Add("Value", Type.GetType("System.String"));
            Dictionary<string, string> FinalValues = new Dictionary<string, string>();
            DataRow dr = tbl.NewRow();
            int offset = 0;
            foreach (string cName in columnNames)
            {

                string[] items = firstSplit[offset].Split(args2, StringSplitOptions.RemoveEmptyEntries);
                string nds = string.Empty;
                if (items.Length > 2)
                {
                    if (items[2] != "----")
                    {
                        if (items[3] != "----")
                        {
                            nds = items[3];
                        }
                    }
                }
                else if (items.Length == 2)
                {
                    nds = items[1];
                }

                if (currentInstance.PropertyNames.Contains(cName) == false)
                {
                    currentInstance.PropertyNames.Add(cName);
                    List<string> tempValue = new List<string>();
                    if (items.Length > 2)
                        tempValue.Add(nds + " ------ " + items[1]);
                    else
                        tempValue.Add(nds + " ------ " + items[0]);
                    currentInstance.PropertyValues.Add(tempValue);
                }
                else
                {
                    int myOffset = 0;
                    foreach (string sName in currentInstance.PropertyNames)
                    {
                        if (sName == cName)
                        {

                            if (items.Length > 2)
                                currentInstance.PropertyValues[myOffset].Add(nds + " ------ " + items[2]);
                            else
                                currentInstance.PropertyValues[myOffset].Add(nds + " ------ " + items[0]);

                            break;
                        }
                        myOffset += 1;
                    }
                }
                bool found = false;

                foreach (KeyValuePair<string, string> item in FinalValues)
                {
                    if (item.Key == cName)
                    {

                        found = true;
                        FinalValues[item.Key] = item.Value + Environment.NewLine + nds;
                        break;
                    }

                }
                if (!found)
                {
                    FinalValues.Add(cName, nds);
                }
                offset += 1;
            }
            DataRow drCopy = tmpTable.NewRow();
            foreach (KeyValuePair<string, string> item in FinalValues)
            {
                dr = tbl.NewRow();
                dr[0] = item.Key;
                dr[1] = item.Value;
                tbl.Rows.Add(dr);
                drCopy = tmpTable.NewRow();
                drCopy[0] = item.Key;
                drCopy[1] = item.Value;
                tmpTable.Rows.Add(drCopy);

            }

            gridControl1.DataSource = tbl;

            grdInstanceDetails.OptionsView.RowAutoHeight = true;
            this.Cursor = Cursors.Default;
            Application.DoEvents();

            this.Cursor = Cursors.Default;
            counter2 += 1;
        }

        public string FormatXMLString(string sUnformattedXML)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(sUnformattedXML);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                xd.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/cat/csw/2.0.2")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.opengis.net/cat/csw/2.0.2", IsNullable = false)]
    public partial class GetRecordsResponse
    {

        private GetRecordsResponseSearchStatus searchStatusField;

        private GetRecordsResponseSearchResults searchResultsField;

        private string versionField;

        /// <remarks/>
        public GetRecordsResponseSearchStatus SearchStatus
        {
            get
            {
                return this.searchStatusField;
            }
            set
            {
                this.searchStatusField = value;
            }
        }

        /// <remarks/>
        public GetRecordsResponseSearchResults SearchResults
        {
            get
            {
                return this.searchResultsField;
            }
            set
            {
                this.searchResultsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/cat/csw/2.0.2")]
    public partial class GetRecordsResponseSearchStatus
    {

        private System.DateTime timestampField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/cat/csw/2.0.2")]
    public partial class GetRecordsResponseSearchResults
    {

        private GetRecordsResponseSearchResultsRecord[] recordField;

        private byte numberOfRecordsMatchedField;

        private byte numberOfRecordsReturnedField;

        private byte nextRecordField;

        private string recordSchemaField;

        private string elementSetField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Record")]
        public GetRecordsResponseSearchResultsRecord[] Record
        {
            get
            {
                return this.recordField;
            }
            set
            {
                this.recordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte numberOfRecordsMatched
        {
            get
            {
                return this.numberOfRecordsMatchedField;
            }
            set
            {
                this.numberOfRecordsMatchedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte numberOfRecordsReturned
        {
            get
            {
                return this.numberOfRecordsReturnedField;
            }
            set
            {
                this.numberOfRecordsReturnedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte nextRecord
        {
            get
            {
                return this.nextRecordField;
            }
            set
            {
                this.nextRecordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string recordSchema
        {
            get
            {
                return this.recordSchemaField;
            }
            set
            {
                this.recordSchemaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string elementSet
        {
            get
            {
                return this.elementSetField;
            }
            set
            {
                this.elementSetField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/cat/csw/2.0.2")]
    public partial class GetRecordsResponseSearchResultsRecord
    {

        private string identifierField;

        private string titleField;

        private string typeField;

        private subject[] subjectField;

        private references referencesField;

        private object relationField;

        private System.DateTime modifiedField;

        private string abstractField;

        private System.DateTime dateField;

        private string sourceField;

        private string languageField;

        private string rightsField;

        private string alternativeField;

        private BoundingBox boundingBoxField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string identifier
        {
            get
            {
                return this.identifierField;
            }
            set
            {
                this.identifierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subject", Namespace = "http://purl.org/dc/elements/1.1/")]
        public subject[] subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/terms/")]
        public references references
        {
            get
            {
                return this.referencesField;
            }
            set
            {
                this.referencesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public object relation
        {
            get
            {
                return this.relationField;
            }
            set
            {
                this.relationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/terms/", DataType = "date")]
        public System.DateTime modified
        {
            get
            {
                return this.modifiedField;
            }
            set
            {
                this.modifiedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/terms/")]
        public string @abstract
        {
            get
            {
                return this.abstractField;
            }
            set
            {
                this.abstractField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/", DataType = "date")]
        public System.DateTime date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string rights
        {
            get
            {
                return this.rightsField;
            }
            set
            {
                this.rightsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/terms/")]
        public string alternative
        {
            get
            {
                return this.alternativeField;
            }
            set
            {
                this.alternativeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.opengis.net/ows")]
        public BoundingBox BoundingBox
        {
            get
            {
                return this.boundingBoxField;
            }
            set
            {
                this.boundingBoxField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class subject
    {

        private string schemeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string scheme
        {
            get
            {
                return this.schemeField;
            }
            set
            {
                this.schemeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://purl.org/dc/terms/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://purl.org/dc/terms/", IsNullable = false)]
    public partial class references
    {

        private string schemeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string scheme
        {
            get
            {
                return this.schemeField;
            }
            set
            {
                this.schemeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/ows")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.opengis.net/ows", IsNullable = false)]
    public partial class BoundingBox
    {

        private string lowerCornerField;

        private string upperCornerField;

        private string crsField;

        private byte dimensionsField;

        /// <remarks/>
        public string LowerCorner
        {
            get
            {
                return this.lowerCornerField;
            }
            set
            {
                this.lowerCornerField = value;
            }
        }

        /// <remarks/>
        public string UpperCorner
        {
            get
            {
                return this.upperCornerField;
            }
            set
            {
                this.upperCornerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string crs
        {
            get
            {
                return this.crsField;
            }
            set
            {
                this.crsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte dimensions
        {
            get
            {
                return this.dimensionsField;
            }
            set
            {
                this.dimensionsField = value;
            }
        }
    }
}
