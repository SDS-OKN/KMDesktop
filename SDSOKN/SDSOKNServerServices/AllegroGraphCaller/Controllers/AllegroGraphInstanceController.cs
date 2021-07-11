using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestSharp;
using RestSharp.Authenticators;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllegroGraphInstanceController : ControllerBase
    {
        [HttpGet()]
        public string CreateDefaultSDSOntology()
        {

            string SERVER_URL = "http://localhost";
            string PORT = "10035";
            string CATALOG_ID = "root";
            string REPOSITORY_ID = "SDS";
            string USERNAME = "super";
            string PASSWORD = "Show4time!";

            return Register(USERNAME, PASSWORD, SERVER_URL, PORT, CATALOG_ID, REPOSITORY_ID);
        }

        [HttpGet("register/{name}/{password}/{url}/{port}/{catalog}/{repository}")]
        public string Register(string name, string password, string url, string port, string catalog, string repository)
        {
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if ((name == item.Name) && (password == item.Password) && (url == item.Url) && (port == Convert.ToString(item.Port)) && (catalog == item.Catalog) && (repository == item.Repository))
                {
                    return item.ID;
                }
            }
            Program.AllegroGraphRegistry.Add(new AllegroGraphRegistryEntry(name, password, url, port, catalog, repository));
            return Program.AllegroGraphRegistry[Program.AllegroGraphRegistry.Count - 1].ID;

        }
    }

    public class OntologyLoading
    {

        int levelnumber;
        static List<ListItems> hierarchyTree;
        RestClient rep = new RestClient();

        public string RetrieveClassList(string query, string id)
        {
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (id.Trim() == item.ID.Trim())
                {
                    SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                    CATALOG_ID = item.Catalog.Trim();
                    REPOSITORY_ID = item.Repository;
                    USERNAME = item.Name;
                    PASSWORD = item.Password;
                    break;
                }
            }
            if (SERVER_URL.Contains("http") == false)
                SERVER_URL = "http://" + SERVER_URL;
            string toolName = string.Empty;
            string userName = USERNAME;
            string password = PASSWORD;
            string repository = REPOSITORY_ID;
            string serverName = SERVER_URL;

            rep.BaseUrl = new Uri(SERVER_URL);
            rep.Authenticator = new HttpBasicAuthenticator(userName, password);
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
            rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            var request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            IRestResponse response = rep.Execute(request);
            string results = response.Content;
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(results);
            XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            XmlNodeList xTest2 = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            List<Item> OriginalItemList = new List<Item>();
            List<string> unsortedList = new List<string>();

            for (int i = 0; i < xTest2.Count; i++)
            {
                int num = 0;
                string currentNode = xTest2[i].ChildNodes[0].InnerText;
                try
                {
                    if ((currentNode.StartsWith("urn:")) || (xTest2[i].ChildNodes.Count < 2))
                    {

                    }
                    else
                    {
                        string currentLabel = xTest2[i].ChildNodes[1].InnerText;
                        Item item = new Item();
                        if (currentLabel.StartsWith("urn:"))
                        {
                        }
                        else
                        {
                            item.id = OriginalItemList.Count;
                            item.label = currentLabel;
                            item.uri = currentNode;
                            OriginalItemList.Add(item);
                            unsortedList.Add(currentLabel);
                        }
                        bool NotFound = true;
                        System.Diagnostics.Debug.WriteLine(currentNode);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            List<string> ListaServizi = new List<string>();
            ListaServizi = unsortedList.OrderBy(q => q).ToList();
            List<Item> FinalList = new List<Item>();
            foreach (string s in ListaServizi)
            {
                Item newItem = new Item();
                newItem.id = FinalList.Count;
                newItem.label = s;
                foreach (var item in OriginalItemList)
                {
                    if (item.label == s)
                    {
                        newItem.uri = item.uri;
                        break;
                    }
                }
                FinalList.Add(newItem);
            }
            string finalstring = string.Empty;
            foreach (var item in FinalList)
            {
                finalstring += item.label + ";;;;;;;;;;;" + item.uri + "**********";
            }
            return finalstring;
        }
        public string RetrieveInstanceInfo(string query, string classquery, string id)
        {
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (id == item.ID)
                {
                    SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                    CATALOG_ID = item.Catalog.Trim();
                    REPOSITORY_ID = item.Repository;
                    USERNAME = item.Name;
                    PASSWORD = item.Password;
                    break;
                }
            }
            if (SERVER_URL.Contains("http") == false)
                SERVER_URL = "http://" + SERVER_URL;
            string toolName = string.Empty;
            string userName = USERNAME;
            string password = PASSWORD;
            string repository = REPOSITORY_ID;
            string serverName = SERVER_URL;
            rep = new RestClient();
            rep.BaseUrl = new Uri(SERVER_URL);
            rep.Authenticator = new HttpBasicAuthenticator(userName, password);
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/x-quints+json");
            rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            //rep.AddDefaultHeader("Accept", "application/x-quints+json");
            var request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(classquery);
            IRestResponse response = rep.Execute(request);
            string results = response.Content;
            XmlDocument xDoc = new XmlDocument();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("InstanceID", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("PropertyURI", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("PropertyLabel", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("InstanceValue", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("InstanceURI", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("OriginalInstanceURI", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("OriginalInstanceValue", System.Type.GetType("System.String")));
            xDoc.LoadXml(results);
            // build the class query information - has property and label
            XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            XmlNodeList xTest2 = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            List<Item> OriginalItemList = new List<Item>();
            List<string> unsortedList = new List<string>();
            string finalstring = string.Empty;
            for (int i = 0; i < xTest2.Count; i++)
            {
                string currentNodeUri = xTest2[i].ChildNodes[0].InnerText;
                string currentNodeName = string.Empty;
                if (xTest2[i].ChildNodes.Count == 2)
                {
                    currentNodeName = xTest2[i].ChildNodes[1].InnerText;
                }
                else
                {
                    currentNodeName = xTest2[i].ChildNodes[0].InnerText;
                }

                DataRow dr = dt.NewRow();
                dr[1] = currentNodeUri;
                dr[2] = currentNodeName;
                dt.Rows.Add(dr);
            }
            request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"/statements?infer=false&offset=0&limit=100&subj=" + System.Web.HttpUtility.UrlEncode(query);
            //request.Resource = "catalogs/importme/repositories/sdsokn/statements?infer=false&offset=0&limit=100&subj=%3Chttp%3A%2F%2Fwww.sdsconsortium.org%2Fschemas%2Fsds-okn.owl%23WQIntv000040013%3E";
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/x-quints+json");
            rep.AddDefaultHeader("Accept", @"application/x-quints+json");
            response = rep.Execute(request);
            results = response.Content;
            xDoc = new XmlDocument();
            //xDoc.LoadXml(results);
            //// build the class query information - has property, value, label, and value label,
            //// take only if have prop or value label
            //XmlNamespaceManager nsm2 = new XmlNamespaceManager(xDoc.NameTable);
            //nsm2.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            //XmlNodeList xTest3 = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm2);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            jArray = Newtonsoft.Json.Linq.JArray.Parse(results);
            Console.WriteLine(jArray.Count);
            foreach (var item in jArray)
            {
                string uniqueid = string.Empty;
                string subject = string.Empty;
                string objectname = string.Empty;
                string objecturi = string.Empty;
                string predicate = string.Empty;
                if (item.Count() == 4)
                {
                    int myCounter = 0;
                    foreach (var nitem in item)
                    {
                        if (myCounter == 0)
                            uniqueid = nitem.ToString();
                        if (myCounter == 1)
                            subject = nitem.ToString();
                        if (myCounter == 2)
                            predicate = nitem.ToString();
                        if (myCounter == 3)
                        {
                            objectname = nitem.ToString().Replace("^^<http://www.w3.org/2001/XMLSchema#string>", "")
                                .Trim();
                            objecturi = nitem.ToString().Trim();
                        }

                        myCounter += 1;


                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        string itemToView = Convert.ToString(dr[1]).ToLower();
                        if (itemToView.StartsWith("<") == false)
                        {
                            itemToView = "<" + itemToView + ">";
                        }
                        if (predicate.ToLower() == itemToView.ToLower())
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(dr["InstanceValue"])))
                            {
                                string[] finalName = objectname.Split('#');

                                if (objectname.StartsWith("\""))
                                {
                                    objectname = objectname.Remove(0, 1);
                                    objectname = objectname.Remove(objectname.Length - 1);
                                }
                                //if (itemToView.StartsWith("<") == true)
                                //{
                                //    objectname = finalName[finalName.Length - 1].Replace('>', ' ').Trim();
                                //}


                                dr["InstanceID"] = uniqueid;
                                dr["InstanceValue"] = objectname;
                                dr["InstanceUri"] = objecturi;
                                dr["OriginalInstanceValue"] = objectname;
                                dr["OriginalInstanceUri"] = objecturi;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            //for (int i = 0; i < xTest3.Count; i++)
            //{
            //    int num = 0;
            //    string currentNode = xTest3[i].ChildNodes[0].InnerText;
            //    try
            //    {
            //        try
            //        {
            //            string currentValue = xTest3[i].ChildNodes[1].InnerText;
            //            string propLabel = string.Empty;
            //            string valueLabel = string.Empty;
            //            if (xTest3[i].ChildNodes.Count == 4)
            //            {
            //                propLabel = xTest3[i].ChildNodes[2].InnerText;
            //                valueLabel += xTest3[i].ChildNodes[3].InnerText;
            //            }
            //            else if (xTest3[i].ChildNodes.Count == 3)
            //            {
            //                propLabel += xTest3[i].ChildNodes[2].InnerText;


            //            }

            //            if ((string.IsNullOrEmpty(propLabel) == false) || (string.IsNullOrEmpty(valueLabel) == false))
            //            {
            //                foreach (DataRow dr in dt.Rows)
            //                {
            //                    if (currentNode.ToLower() == Convert.ToString(dr["PropertyUri"]).ToLower())
            //                    {
            //                        dr["InstanceURI"] = currentValue;
            //                        if(string.IsNullOrEmpty(valueLabel))
            //                            dr["InstanceValue"] = currentValue;
            //                        else
            //                        {
            //                            dr["InstanceValue"] = valueLabel;

            //                        }
            //                        dr["OriginalInstanceURI"] = currentValue;
            //                        if (string.IsNullOrEmpty(valueLabel))
            //                        {
            //                            dr["OriginalInstanceValue"] = currentValue;
            //                        }
            //                        else
            //                        {
            //                            dr["OriginalInstanceValue"] = valueLabel;
            //                        }

            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            //
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine(ex.Message);
            //    }
            //}

            var writer = new StringWriter();
            if (string.IsNullOrEmpty(dt.TableName))
            {
                dt.TableName = "rSchema";
            }
            dt.WriteXml(writer, XmlWriteMode.WriteSchema, true);


            return writer.ToString();
        }
        public string RetrieveInstanceList(string query, string id)
        {
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (id == item.ID)
                {
                    SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                    CATALOG_ID = item.Catalog.Trim();
                    REPOSITORY_ID = item.Repository;
                    USERNAME = item.Name;
                    PASSWORD = item.Password;
                    break;
                }
            }
            if (SERVER_URL.Contains("http") == false)
                SERVER_URL = "http://" + SERVER_URL;
            string toolName = string.Empty;
            string userName = USERNAME;
            string password = PASSWORD;
            string repository = REPOSITORY_ID;
            string serverName = SERVER_URL;
            rep.BaseUrl = new Uri(SERVER_URL);
            rep.Authenticator = new HttpBasicAuthenticator(userName, password);
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
            rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            var request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            IRestResponse response = rep.Execute(request);
            string results = response.Content;
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(results);
            XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            XmlNodeList xTest2 = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            List<Item> OriginalItemList = new List<Item>();
            List<string> unsortedList = new List<string>();

            for (int i = 0; i < xTest2.Count; i++)
            {
                int num = 0;
                string currentNode = xTest2[i].ChildNodes[0].InnerText;
                try
                {
                    if ((currentNode.StartsWith("urn:")) || (xTest2[i].ChildNodes.Count < 2))
                    {
                        if (xTest2[i].ChildNodes.Count == 1)
                        {
                            string[] currentLabel = xTest2[i].ChildNodes[0].InnerText.Split('#');
                            string labels = currentLabel[currentLabel.Length - 1].Replace('>', ' ').Trim();
                            Item item = new Item();
                            item.id = OriginalItemList.Count;
                            item.label = labels;
                            item.uri = currentNode;
                            OriginalItemList.Add(item);
                            unsortedList.Add(labels);
                        }
                    }
                    else
                    {
                        string currentLabel = xTest2[i].ChildNodes[1].InnerText;
                        Item item = new Item();
                        if (currentLabel.StartsWith("urn:"))
                        {
                        }
                        else
                        {
                            item.id = OriginalItemList.Count;
                            item.label = currentLabel;
                            item.uri = currentNode;
                            OriginalItemList.Add(item);
                            unsortedList.Add(currentLabel);
                        }
                        bool NotFound = true;
                        System.Diagnostics.Debug.WriteLine(currentNode);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            List<string> ListaServizi = new List<string>();
            ListaServizi = unsortedList.OrderBy(q => q).ToList();
            List<Item> FinalList = new List<Item>();
            foreach (string s in ListaServizi)
            {
                Item newItem = new Item();
                newItem.id = FinalList.Count;
                newItem.label = s;
                foreach (var item in OriginalItemList)
                {
                    if (item.label == s)
                    {
                        newItem.uri = item.uri;
                        break;
                    }
                }
                FinalList.Add(newItem);
            }
            string finalstring = string.Empty;
            foreach (var item in FinalList)
            {
                finalstring += item.label + ";;;;;;;;;;;" + item.uri + "**********";
            }
            return finalstring;
        }
        public string RetrieveOntologyList(string id)
        {
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (id == item.ID)
                {
                    SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                    CATALOG_ID = item.Catalog.Trim();
                    REPOSITORY_ID = item.Repository;
                    USERNAME = item.Name;
                    PASSWORD = item.Password;
                    break;
                }
            }
            string toolName = string.Empty;
            string userName = USERNAME;
            string password = PASSWORD;
            string repository = REPOSITORY_ID;
            string serverName = SERVER_URL;

            List<Ordering> OntologyList = new List<Ordering>();

            hierarchyTree = new List<ListItems>();
            rep.BaseUrl = new Uri(SERVER_URL);
            //rep = new Repository(serverName + @"/repositories/" + repository);
            BuildOntologyOrderList(ref OntologyList, rep, id);

            List<NodeItem> nodes = new List<NodeItem>();
            List<Link> edges = new List<Link>();
            string finalstring = "var nodes = [";
            StringBuilder sbNodes = new StringBuilder();
            StringBuilder sbEdges = new StringBuilder();
            foreach (var item in OntologyList[0].Nodes)
            {
                sbNodes.Append("{id: " + item.id.ToString() + ", label: '" + item.label.Replace("'", " ") + "', title: '" + item.label.Replace("'", " ") + "', uri: '" + item.uri + "'},");
            }
            foreach (var item in OntologyList[0].Edges)
            {
                sbEdges.Append("{from: " + item.from.ToString() + ", to: " + item.to.ToString() + "},");
            }
            finalstring += sbNodes.ToString().Remove(sbNodes.ToString().Length - 1) + "];";

            finalstring += "var edges = [";
            finalstring += sbEdges.ToString().Remove(sbEdges.ToString().Length - 1) + "];";
            //foreach(var node in OntologyList[0].Nodes)
            //{
            //    nodes.Add("id: " + node.id + ", label: '" + node.label +"'");
            //}
            //foreach(var link in OntologyList[0].Edges)
            //{
            //    edges.Add("from: " + link.from + ", to: " + link.to + "");
            //}

            //FinalMerge fm = new FinalMerge();
            //fm.nodes = new List<string>();
            //fm.nodes.AddRange(nodes);
            //fm.edges = new List<string>();
            //fm.edges.AddRange(edges); 
            //string query = @"select * WHERE {?subject rdfs:subClassOf SDSKnowledgePortalOnto:SDSOnto .  ?subject rdfs:label ?label . 	{ select * where { ?child rdfs:subClassOf ?subject . { select * where { ?gchild rdfs:subClassOf ?child . } } } } } "; // @"select * WHERE {?subject rdfs:subClassOf SDSKnowledgePortalOnto:SDSOnto .  ?subject rdfs:label ?label . }";
            string resultsList = Newtonsoft.Json.JsonConvert.SerializeObject(finalstring, Newtonsoft.Json.Formatting.Indented);
            System.Diagnostics.Debug.WriteLine(resultsList);

            ////string query = @"select ?predicate ?object ?description ?label where {" + toolName + " ?predicate ?object . ?object rdfs:label ?description .  }";
            ////   List<Results> results = rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            //string results = string.Empty;
            //var request = new RestRequest();
            //request.Resource = @"catalogs/system/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            //IRestResponse response = rep.Execute(request);
            //results = response.Content;
            //XmlDocument xDoc = new XmlDocument();
            //xDoc.LoadXml(results);
            //XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            //nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            //XmlNodeList xNodesSubjectUrl = xDoc.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[2]/def:uri", nsm);  //user innertext to get the info
            //XmlNodeList xTest = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            //XmlNodeList xNodesSubjectName = xDoc.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[1]", nsm);
            //List<string> nodesName = new List<string>();
            //List<string> edges = new List<string>();
            //List<string> nodesNameUrl = new List<string>();
            //List<int> nodeIds = new List<int>();

            //for (int i = 0; i < xNodesSubjectUrl.Count; i++)
            //{
            //    ListItems info = new ListItems();
            //    info.ParentName = string.Empty;
            //    info.ParentUrl = string.Empty;
            //    info.NodeName = "SDSOnto";
            //    info.NodeUrl = @"<http://www.institute.redlands.edu/Ontologies/SDSS/SDSKnowledgePortalOnto.owl#SDSOnto>";
            //    info.ChildName = xNodesSubjectName[i].InnerText;
            //    info.ChildUrl = xNodesSubjectUrl[i].InnerText;
            //    hierarchyTree.Add(info);
            //}

            //query = @"select * WHERE {?subject rdfs:subClassOf SDSKnowledgePortalOnto:SDSOnto .  ?subject rdfs:label ?label .  ?subClassOnto rdfs:subClassOf ?subject . ?subClassOnto rdfs:label ?SubClassOntoName . }";

            ////string query = @"select ?predicate ?object ?description ?label where {" + toolName + " ?predicate ?object . ?object rdfs:label ?description .  }";
            //string results2 = string.Empty; //rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            //request = new RestRequest();
            //request.Resource = @"catalogs/system/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            //response = rep.Execute(request);
            //results2 = response.Content;
            //XmlDocument xDoc2 = new XmlDocument();
            //xDoc2.LoadXml(results2);
            //XmlNamespaceManager nsm2 = new XmlNamespaceManager(xDoc2.NameTable);
            //nsm2.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            //XmlNodeList xNodesSubjectUrl2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[4]/def:uri", nsm2);  //user innertext to get the info
            //XmlNodeList xTest2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result", nsm2);
            //XmlNodeList xNodesSubjectName2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[2]/def:literal", nsm2);
            //XmlNodeList xNodesChildUrl2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[3]/def:uri", nsm2);
            //XmlNodeList xNodesChildName2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[1]/def:literal", nsm2);
            //;
            //List<string> currentItems = new List<string>();
            //List<string> currentUrls = new List<string>();

            //for (int i = 0; i < xNodesSubjectUrl2.Count; i++)
            //{
            //    ListItems info = new ListItems();
            //    info.ParentName = "SDSOnto";
            //    info.ParentUrl = @"<http://www.institute.redlands.edu/Ontologies/SDSS/SDSKnowledgePortalOnto.owl#SDSOnto>";
            //    info.ChildName = xNodesChildName2[i].InnerText;
            //    info.ChildUrl = xNodesChildUrl2[i].InnerText;
            //    info.NodeName = xNodesSubjectName2[i].InnerText;
            //    info.NodeUrl = xNodesSubjectUrl2[i].InnerText;
            //    hierarchyTree.Add(info);
            //    if (currentItems.Contains(info.NodeName) == false)
            //    {
            //        currentItems.Add(info.NodeName);
            //        currentUrls.Add(info.NodeUrl);
            //    }
            //}
            //levelnumber = 2;
            //for (int i = 0; i < currentItems.Count; i++)
            //{
            //    buildTree(currentItems[i], currentUrls[i], levelnumber);

            //}
            ////for (int i = 0; i < xNodesChildUrl2.Count; i++)
            ////{
            ////    if (string.IsNullOrEmpty(xNodesChildUrl2[i].InnerText) == false)
            ////    {
            ////    // Original	buildTree(xNodesChildName2[i].InnerText, xNodesChildUrl2[i].InnerText, levelnumber);
            ////        buildTree(xNodesChildName2[i].InnerText, xNodesChildUrl2[i].InnerText, levelnumber);
            ////    }
            ////}


            //// combine items
            //FillOutHierarchyTreeWithChildren(hierarchyTree);
            //int uniqueID = 1;
            //int sourceID = 0;
            //int destId = 0;
            //for (int i = 0; i < hierarchyTree.Count; i++)
            //{
            //    if (nodesName.Contains(hierarchyTree[i].NodeName) == false)
            //    {
            //        if (string.IsNullOrEmpty(hierarchyTree[i].NodeName) == false)
            //        {
            //            nodeIds.Add(uniqueID);
            //            uniqueID += 1;
            //            nodesName.Add(hierarchyTree[i].NodeName);
            //            nodesNameUrl.Add(hierarchyTree[i].NodeUrl);
            //        }

            //    }
            //    if (nodesName.Contains(hierarchyTree[i].ChildName) == false)
            //    {
            //        if (string.IsNullOrEmpty(hierarchyTree[i].ChildName) == false)
            //        {
            //            nodeIds.Add(uniqueID);
            //            uniqueID += 1;
            //            nodesName.Add(hierarchyTree[i].ChildName);
            //            nodesNameUrl.Add(hierarchyTree[i].ChildUrl);
            //        }

            //    }
            //    for (int t = 0; t < nodesName.Count; t++)
            //    {
            //        if (nodesName[t] == hierarchyTree[i].NodeName)
            //        {
            //            sourceID = t;
            //        }
            //        if (nodesName[t] == hierarchyTree[i].ChildName)
            //        {
            //            destId = t;
            //        }
            //    }
            //    if (edges.Contains(sourceID.ToString() + "___" + destId.ToString()) == false)
            //    {
            //        edges.Add(sourceID.ToString() + "___" + destId.ToString());

            //    }
            //}



            ////for (int i = 0; i < hierarchyTree.Count; i++)
            ////{
            ////    if (nodesName.Contains(hierarchyTree[i].NodeName) == false)
            ////    {
            ////        nodesName.Add(hierarchyTree[i].NodeName);
            ////        nodesNameUrl.Add(hierarchyTree[i].NodeUrl);

            ////    }
            ////    if (nodesName.Contains(hierarchyTree[i].ChildName) == false)
            ////    {
            ////        nodesName.Add(hierarchyTree[i].ChildName);
            ////        nodesNameUrl.Add(hierarchyTree[i].ChildUrl);

            ////    }
            ////    if (edges.Contains(hierarchyTree[i].NodeName + "___" + hierarchyTree[i].ChildName) == false)
            ////    {
            ////        if ((string.IsNullOrEmpty(hierarchyTree[i].NodeName) == false) && (string.IsNullOrEmpty(hierarchyTree[i].ChildName) == false))
            ////            edges.Add(hierarchyTree[i].NodeName + "___" + hierarchyTree[i].ChildName);

            ////    }
            ////}


            ////  List<ListItems> items = RetrurnUnifiedSet(hierarchyTree);
            ////ArrayList finalList = new ArrayList();
            ////for (int q = 0; q < items.Count; q++)
            ////{
            ////   finalList.Add(items[q].ParentName + " * " + items[q].ParentUrl + " * " + items[q].NodeName + " * " + items[q].NodeUrl + " * " + items[q].ChildName + " * " + items[1].ChildUrl);
            ////}
            ////string foundResults = JSON.JsonEncode(finalList);

            ////generate the json for the response by first enumerating all the nodes to gurrantee unique entries
            ////Hashtable allNodes = new Hashtable();
            ////for (int i = 0; i < items.Count; i++)
            ////{
            ////    string thisNodeKey = FixName(items[i].NodeUrl.Replace("<", "").Replace(">", ""));
            ////    if (!allNodes.ContainsKey(thisNodeKey))
            ////    {
            ////        Hashtable thisNode = new Hashtable();
            ////        allNodes.Add(thisNodeKey, thisNode);
            ////        thisNode.Add("n", Util.ToTitleCase(items[i].NodeName));
            ////        bool foundinfo = false; 
            ////        for (int q = 0; q < OntologyList.Count; q++)
            ////        {

            ////            if (OntologyList[q].ChildName.ToLower().Contains(items[i].NodeUrl.ToLower()) && OntologyList[q].OrderID>0 )
            ////            {
            ////                thisNode.Add("order", OntologyList[q].OrderID);
            ////                thisNode.Add("ParentOrderName", OntologyList[q].ParentName);
            ////                foundinfo = true;
            ////                break;
            ////            }
            ////        }
            ////        if (!foundinfo) 
            ////        {
            ////            thisNode.Add("order",-1);
            ////            thisNode.Add("ParentOrderName", "NoOrder");
            ////        }
            ////            thisNode.Add("c", new Hashtable());
            ////    }
            ////}
            /////// STEVE : DO NOT FIX NAMES AS SHOWN BELOW
            //////now using the structure go ahead and begin building the hirearchy assigning the first node (SDSOnto) to be "root"
            ////Hashtable root = (Hashtable)allNodes["SDSKnowledgePortalOnto:SDSOnto"];

            ////for (int i = 0; i < items.Count; i++)
            ////{
            ////    //check to see if this node has a parent, if it does add the node as a child checking for duplicates
            ////    //and that the parent exists!
            ////    if (!string.IsNullOrEmpty(items[i].ParentUrl))
            ////    {
            ////        string thisNodeKey = FixName(items[i].NodeUrl.Replace("<", "").Replace(">", ""));
            ////        string parentKey = FixName(items[i].ParentUrl.Replace("<", "").Replace(">", ""));
            ////        if (allNodes.ContainsKey(parentKey))
            ////        {
            ////            Hashtable children = (Hashtable)((Hashtable)allNodes[parentKey])["c"];
            ////            if (!children.ContainsKey(thisNodeKey))
            ////            {
            ////                children.Add(thisNodeKey, allNodes[thisNodeKey]);
            ////            }
            ////        }
            ////        else
            ////        {
            ////            //Hashtable children = (Hashtable)((Hashtable)allNodes[parentKey])["c"];
            ////            //if (!children.ContainsKey(thisNodeKey))
            ////            //{
            ////            //    children.Add(thisNodeKey, allNodes[thisNodeKey]);
            ////            //}
            ////            //Response.Write("Missing Parent - " + parentKey + "\n");
            ////        }
            ////    }
            ////}

            ////now render the tree from root

            //List<ComboResults> resultsList = new List<ComboResults>();
            //ComboResults comboResults = new ComboResults(nodesName, nodesNameUrl, edges);
            //comboResults.nodeIds = nodeIds;
            //resultsList.Add(comboResults);
            return resultsList;
        }

        public string RetrieveOntologyList2(string id)
        {
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (id == item.ID)
                {
                    SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                    CATALOG_ID = item.Catalog.Trim();
                    REPOSITORY_ID = item.Repository;
                    USERNAME = item.Name;
                    PASSWORD = item.Password;
                    break;
                }
            }

            string toolName = string.Empty;
            string userName = USERNAME;
            string password = PASSWORD;
            string repository = REPOSITORY_ID;
            string serverName = SERVER_URL;

            List<Ordering> OntologyList = new List<Ordering>();

            hierarchyTree = new List<ListItems>();
            rep.BaseUrl = new Uri(SERVER_URL);
            //rep = new Repository(serverName + @"/repositories/" + repository);
            BuildOntologyOrderList(ref OntologyList, rep, id);

            //List<string> nodes = new List<string>();
            //List<string> edges = new List<string>();

            //foreach (var node in OntologyList[0].Nodes)
            //{
            //    nodes.Add("id: " + node.id + ", label: '" + node.label + "'");
            //}
            //foreach (var link in OntologyList[0].Edges)
            //{
            //    edges.Add("from: " + link.from + ", to: " + link.to + "");
            //}

            //FinalMerge fm = new FinalMerge();
            //fm.nodes = new List<string>();
            //fm.nodes.AddRange(nodes);
            //fm.edges = new List<string>();
            //fm.edges.AddRange(edges);
            List<NodeItem> nodes = new List<NodeItem>();
            List<Link> edges = new List<Link>();
            FinalMerge fm = new FinalMerge();
            fm.nodes = new List<NodeItem>();
            fm.edges = new List<Link>();

            fm.edges.AddRange(OntologyList[0].Edges);

            foreach (var item in OntologyList[0].Nodes)
            {
                NodeItem nodeItem = new NodeItem();
                nodeItem.id = Convert.ToString(item.id);
                nodeItem.label = item.label;
                nodeItem.uri = item.uri;

                fm.nodes.Add(nodeItem);
            }
            //string query = @"select * WHERE {?subject rdfs:subClassOf SDSKnowledgePortalOnto:SDSOnto .  ?subject rdfs:label ?label . 	{ select * where { ?child rdfs:subClassOf ?subject . { select * where { ?gchild rdfs:subClassOf ?child . } } } } } "; // @"select * WHERE {?subject rdfs:subClassOf SDSKnowledgePortalOnto:SDSOnto .  ?subject rdfs:label ?label . }";
            string resultsList = Newtonsoft.Json.JsonConvert.SerializeObject(fm, Newtonsoft.Json.Formatting.Indented);
            System.Diagnostics.Debug.WriteLine(resultsList);


            ////string query = @"select ?predicate ?object ?description ?label where {" + toolName + " ?predicate ?object . ?object rdfs:label ?description .  }";
            ////   List<Results> results = rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            //string results = string.Empty;
            //var request = new RestRequest();
            //request.Resource = @"catalogs/system/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            //IRestResponse response = rep.Execute(request);
            //results = response.Content;
            //XmlDocument xDoc = new XmlDocument();
            //xDoc.LoadXml(results);
            //XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            //nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            //XmlNodeList xNodesSubjectUrl = xDoc.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[2]/def:uri", nsm);  //user innertext to get the info
            //XmlNodeList xTest = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            //XmlNodeList xNodesSubjectName = xDoc.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[1]", nsm);
            //List<string> nodesName = new List<string>();
            //List<string> edges = new List<string>();
            //List<string> nodesNameUrl = new List<string>();
            //List<int> nodeIds = new List<int>();

            //for (int i = 0; i < xNodesSubjectUrl.Count; i++)
            //{
            //    ListItems info = new ListItems();
            //    info.ParentName = string.Empty;
            //    info.ParentUrl = string.Empty;
            //    info.NodeName = "SDSOnto";
            //    info.NodeUrl = @"<http://www.institute.redlands.edu/Ontologies/SDSS/SDSKnowledgePortalOnto.owl#SDSOnto>";
            //    info.ChildName = xNodesSubjectName[i].InnerText;
            //    info.ChildUrl = xNodesSubjectUrl[i].InnerText;
            //    hierarchyTree.Add(info);
            //}

            //query = @"select * WHERE {?subject rdfs:subClassOf SDSKnowledgePortalOnto:SDSOnto .  ?subject rdfs:label ?label .  ?subClassOnto rdfs:subClassOf ?subject . ?subClassOnto rdfs:label ?SubClassOntoName . }";

            ////string query = @"select ?predicate ?object ?description ?label where {" + toolName + " ?predicate ?object . ?object rdfs:label ?description .  }";
            //string results2 = string.Empty; //rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            //request = new RestRequest();
            //request.Resource = @"catalogs/~system/~repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            //response = rep.Execute(request);
            //results2 = response.Content;
            //XmlDocument xDoc2 = new XmlDocument();
            //xDoc2.LoadXml(results2);
            //XmlNamespaceManager nsm2 = new XmlNamespaceManager(xDoc2.NameTable);
            //nsm2.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            //XmlNodeList xNodesSubjectUrl2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[4]/def:uri", nsm2);  //user innertext to get the info
            //XmlNodeList xTest2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result", nsm2);
            //XmlNodeList xNodesSubjectName2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[2]/def:literal", nsm2);
            //XmlNodeList xNodesChildUrl2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[3]/def:uri", nsm2);
            //XmlNodeList xNodesChildName2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[1]/def:literal", nsm2);
            //;
            //List<string> currentItems = new List<string>();
            //List<string> currentUrls = new List<string>();

            //for (int i = 0; i < xNodesSubjectUrl2.Count; i++)
            //{
            //    ListItems info = new ListItems();
            //    info.ParentName = "SDSOnto";
            //    info.ParentUrl = @"<http://www.institute.redlands.edu/Ontologies/SDSS/SDSKnowledgePortalOnto.owl#SDSOnto>";
            //    info.ChildName = xNodesChildName2[i].InnerText;
            //    info.ChildUrl = xNodesChildUrl2[i].InnerText;
            //    info.NodeName = xNodesSubjectName2[i].InnerText;
            //    info.NodeUrl = xNodesSubjectUrl2[i].InnerText;
            //    hierarchyTree.Add(info);
            //    if (currentItems.Contains(info.NodeName) == false)
            //    {
            //        currentItems.Add(info.NodeName);
            //        currentUrls.Add(info.NodeUrl);
            //    }
            //}
            //levelnumber = 2;
            //for (int i = 0; i < currentItems.Count; i++)
            //{
            //    buildTree(currentItems[i], currentUrls[i], levelnumber);

            //}
            ////for (int i = 0; i < xNodesChildUrl2.Count; i++)
            ////{
            ////    if (string.IsNullOrEmpty(xNodesChildUrl2[i].InnerText) == false)
            ////    {
            ////    // Original	buildTree(xNodesChildName2[i].InnerText, xNodesChildUrl2[i].InnerText, levelnumber);
            ////        buildTree(xNodesChildName2[i].InnerText, xNodesChildUrl2[i].InnerText, levelnumber);
            ////    }
            ////}


            //// combine items
            //FillOutHierarchyTreeWithChildren(hierarchyTree);
            //int uniqueID = 1;
            //int sourceID = 0;
            //int destId = 0;
            //for (int i = 0; i < hierarchyTree.Count; i++)
            //{
            //    if (nodesName.Contains(hierarchyTree[i].NodeName) == false)
            //    {
            //        if (string.IsNullOrEmpty(hierarchyTree[i].NodeName) == false)
            //        {
            //            nodeIds.Add(uniqueID);
            //            uniqueID += 1;
            //            nodesName.Add(hierarchyTree[i].NodeName);
            //            nodesNameUrl.Add(hierarchyTree[i].NodeUrl);
            //        }

            //    }
            //    if (nodesName.Contains(hierarchyTree[i].ChildName) == false)
            //    {
            //        if (string.IsNullOrEmpty(hierarchyTree[i].ChildName) == false)
            //        {
            //            nodeIds.Add(uniqueID);
            //            uniqueID += 1;
            //            nodesName.Add(hierarchyTree[i].ChildName);
            //            nodesNameUrl.Add(hierarchyTree[i].ChildUrl);
            //        }

            //    }
            //    for (int t = 0; t < nodesName.Count; t++)
            //    {
            //        if (nodesName[t] == hierarchyTree[i].NodeName)
            //        {
            //            sourceID = t;
            //        }
            //        if (nodesName[t] == hierarchyTree[i].ChildName)
            //        {
            //            destId = t;
            //        }
            //    }
            //    if (edges.Contains(sourceID.ToString() + "___" + destId.ToString()) == false)
            //    {
            //        edges.Add(sourceID.ToString() + "___" + destId.ToString());

            //    }
            //}



            ////for (int i = 0; i < hierarchyTree.Count; i++)
            ////{
            ////    if (nodesName.Contains(hierarchyTree[i].NodeName) == false)
            ////    {
            ////        nodesName.Add(hierarchyTree[i].NodeName);
            ////        nodesNameUrl.Add(hierarchyTree[i].NodeUrl);

            ////    }
            ////    if (nodesName.Contains(hierarchyTree[i].ChildName) == false)
            ////    {
            ////        nodesName.Add(hierarchyTree[i].ChildName);
            ////        nodesNameUrl.Add(hierarchyTree[i].ChildUrl);

            ////    }
            ////    if (edges.Contains(hierarchyTree[i].NodeName + "___" + hierarchyTree[i].ChildName) == false)
            ////    {
            ////        if ((string.IsNullOrEmpty(hierarchyTree[i].NodeName) == false) && (string.IsNullOrEmpty(hierarchyTree[i].ChildName) == false))
            ////            edges.Add(hierarchyTree[i].NodeName + "___" + hierarchyTree[i].ChildName);

            ////    }
            ////}


            ////  List<ListItems> items = RetrurnUnifiedSet(hierarchyTree);
            ////ArrayList finalList = new ArrayList();
            ////for (int q = 0; q < items.Count; q++)
            ////{
            ////   finalList.Add(items[q].ParentName + " * " + items[q].ParentUrl + " * " + items[q].NodeName + " * " + items[q].NodeUrl + " * " + items[q].ChildName + " * " + items[1].ChildUrl);
            ////}
            ////string foundResults = JSON.JsonEncode(finalList);

            ////generate the json for the response by first enumerating all the nodes to gurrantee unique entries
            ////Hashtable allNodes = new Hashtable();
            ////for (int i = 0; i < items.Count; i++)
            ////{
            ////    string thisNodeKey = FixName(items[i].NodeUrl.Replace("<", "").Replace(">", ""));
            ////    if (!allNodes.ContainsKey(thisNodeKey))
            ////    {
            ////        Hashtable thisNode = new Hashtable();
            ////        allNodes.Add(thisNodeKey, thisNode);
            ////        thisNode.Add("n", Util.ToTitleCase(items[i].NodeName));
            ////        bool foundinfo = false; 
            ////        for (int q = 0; q < OntologyList.Count; q++)
            ////        {

            ////            if (OntologyList[q].ChildName.ToLower().Contains(items[i].NodeUrl.ToLower()) && OntologyList[q].OrderID>0 )
            ////            {
            ////                thisNode.Add("order", OntologyList[q].OrderID);
            ////                thisNode.Add("ParentOrderName", OntologyList[q].ParentName);
            ////                foundinfo = true;
            ////                break;
            ////            }
            ////        }
            ////        if (!foundinfo) 
            ////        {
            ////            thisNode.Add("order",-1);
            ////            thisNode.Add("ParentOrderName", "NoOrder");
            ////        }
            ////            thisNode.Add("c", new Hashtable());
            ////    }
            ////}
            /////// STEVE : DO NOT FIX NAMES AS SHOWN BELOW
            //////now using the structure go ahead and begin building the hirearchy assigning the first node (SDSOnto) to be "root"
            ////Hashtable root = (Hashtable)allNodes["SDSKnowledgePortalOnto:SDSOnto"];

            ////for (int i = 0; i < items.Count; i++)
            ////{
            ////    //check to see if this node has a parent, if it does add the node as a child checking for duplicates
            ////    //and that the parent exists!
            ////    if (!string.IsNullOrEmpty(items[i].ParentUrl))
            ////    {
            ////        string thisNodeKey = FixName(items[i].NodeUrl.Replace("<", "").Replace(">", ""));
            ////        string parentKey = FixName(items[i].ParentUrl.Replace("<", "").Replace(">", ""));
            ////        if (allNodes.ContainsKey(parentKey))
            ////        {
            ////            Hashtable children = (Hashtable)((Hashtable)allNodes[parentKey])["c"];
            ////            if (!children.ContainsKey(thisNodeKey))
            ////            {
            ////                children.Add(thisNodeKey, allNodes[thisNodeKey]);
            ////            }
            ////        }
            ////        else
            ////        {
            ////            //Hashtable children = (Hashtable)((Hashtable)allNodes[parentKey])["c"];
            ////            //if (!children.ContainsKey(thisNodeKey))
            ////            //{
            ////            //    children.Add(thisNodeKey, allNodes[thisNodeKey]);
            ////            //}
            ////            //Response.Write("Missing Parent - " + parentKey + "\n");
            ////        }
            ////    }
            ////}

            ////now render the tree from root

            //List<ComboResults> resultsList = new List<ComboResults>();
            //ComboResults comboResults = new ComboResults(nodesName, nodesNameUrl, edges);
            //comboResults.nodeIds = nodeIds;
            //resultsList.Add(comboResults);
            return resultsList;
        }
        /// <summary>
        ///  build list of all the items with the order, with the first at the top with zero, and then each with its own owner
        /// </summary>
        /// <param label="OntologyList"></param>
        /// <param label="rep"></param>
        /// <param label="userName"></param>
        /// <param label="password"></param>
        private void BuildOntologyOrderList(ref List<Ordering> OntologyList, RestClient rep, string id)
        {
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (id == item.ID)
                {
                    SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                    CATALOG_ID = item.Catalog.Trim();
                    REPOSITORY_ID = item.Repository;
                    USERNAME = item.Name;
                    PASSWORD = item.Password;
                    break;
                }
            }
            if (SERVER_URL.Contains("http") == false)
                SERVER_URL = "http://" + SERVER_URL;
            List<Item> NodeList = new List<Item>();
            List<Link> EdgesList = new List<Link>();
            string query = @"select * WHERE {?subject rdfs:subClassOf SDSKnowledgePortalOnto:SDSOnto .  ?subject rdfs:label ?label . 	{ select * where { ?child rdfs:subClassOf ?subject .  ?child rdfs:label ?childname . { select * where { ?gchild rdfs:subClassOf ?child . ?gchild rdfs:label ?gchildname } } } } } ";
            //string results = rep.evalSparqlQueryREST(rep.getName(), query, "application /sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            rep.Authenticator = new HttpBasicAuthenticator(USERNAME, PASSWORD);
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
            rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            var request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            IRestResponse response = rep.Execute(request);
            string results = response.Content;
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(results);
            XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            XmlNodeList xTest2 = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            for (int i = 0; i < xTest2.Count; i++)
            {
                int num = 0;
                string currentNode = xTest2[i].ChildNodes[0].InnerText;
                bool NotFound = true;
                foreach (var item in NodeList)
                {

                    if (item.uri == currentNode)
                    {
                        NotFound = false;
                        break;
                    }

                }
                if (NotFound)
                {
                    var newItem = new Item();
                    newItem.id = NodeList.Count;
                    newItem.label = xTest2[i].ChildNodes[1].InnerText;
                    newItem.uri = currentNode;
                    NodeList.Add(newItem);
                }
                currentNode = xTest2[i].ChildNodes[2].InnerText;
                NotFound = true;
                foreach (var item in NodeList)
                {

                    if (item.uri == currentNode)
                    {
                        NotFound = false;
                        break;
                    }

                }
                if (NotFound)
                {
                    var newItem = new Item();
                    newItem.id = NodeList.Count;
                    newItem.label = xTest2[i].ChildNodes[3].InnerText;
                    newItem.uri = currentNode;
                    NodeList.Add(newItem);
                }
                currentNode = xTest2[i].ChildNodes[4].InnerText;
                NotFound = true;
                foreach (var item in NodeList)
                {

                    if (item.uri == currentNode)
                    {
                        NotFound = false;
                        break;
                    }

                }
                if (NotFound)
                {
                    var newItem = new Item();
                    newItem.id = NodeList.Count;
                    newItem.label = xTest2[i].ChildNodes[5].InnerText;
                    newItem.uri = currentNode;
                    NodeList.Add(newItem);
                }

            }
            // now handle building the edges
            for (int i = 0; i < xTest2.Count; i++)
            {
                try
                {
                    string ChildNode = xTest2[i].ChildNodes[0].InnerText;
                    string GrandChildNode = xTest2[i].ChildNodes[2].InnerText;
                    string ParentNode = xTest2[i].ChildNodes[4].InnerText;

                    int childId = 0;
                    int grandChildId = 0;
                    int parentId = 0;
                    foreach (var item in NodeList)
                    {
                        if (item.uri == ChildNode)
                        {
                            childId = item.id;
                        }
                        if (item.uri == GrandChildNode)
                        {
                            grandChildId = item.id;
                        }
                        if (item.uri == ParentNode)
                        {
                            parentId = item.id;
                        }
                    }
                    var newEdge = new Link();
                    newEdge.from = parentId;
                    newEdge.to = childId;
                    bool Found = false;
                    foreach (var test in EdgesList)
                    {

                        if ((Convert.ToInt32(test.from) == Convert.ToInt32(newEdge.from)) && (Convert.ToInt32(test.to) == Convert.ToInt32(newEdge.to)))
                        {
                            Found = true;
                            break;
                        }

                    }
                    if (!Found)
                        EdgesList.Add(newEdge);
                    newEdge = new Link();
                    newEdge.from = childId;
                    newEdge.to = grandChildId;
                    Found = false;
                    foreach (var test in EdgesList)
                    {

                        if ((Convert.ToInt32(test.from) == Convert.ToInt32(newEdge.from)) && (Convert.ToInt32(test.to) == Convert.ToInt32(newEdge.to)))
                        {
                            Found = true;
                            break;
                        }

                    }
                    if (!Found)
                        EdgesList.Add(newEdge);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            OntologyList.Add(new Ordering(NodeList, EdgesList));
        }

        //function to build unified item list
        private List<ListItems> RetrurnUnifiedSet(List<ListItems> hierarchyTree)
        {
            List<ListItems> finalList = new List<ListItems>();
            finalList.Add(hierarchyTree[0]);
            if (string.IsNullOrEmpty(finalList[0].ChildName) == false)
            {
                finalList[0].ChildName += ";";
                finalList[0].ChildUrl += ";";
            }
            // step through list, adding items if the node does not exist, otherwise, appends the children
            for (int i = 1; i < hierarchyTree.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < finalList.Count; j++)
                {

                    //      if (finalList[j].NodeUrl == hierarchyTree[i].NodeUrl)
                    if ((finalList[j].NodeUrl == hierarchyTree[i].NodeUrl) && (finalList[j].ParentUrl == hierarchyTree[i].ParentUrl))
                    {
                        if (finalList[j].ChildName.Contains(hierarchyTree[i].ChildName) == false)
                        {
                            finalList[j].ChildName += hierarchyTree[i].ChildName + ";";
                            finalList[j].ChildUrl += hierarchyTree[i].ChildUrl + ";";
                        }
                        found = true;
                        break;
                    }
                }
                // if new item create
                if (!found)
                {
                    finalList.Add(hierarchyTree[i]);
                    if (string.IsNullOrEmpty(finalList[finalList.Count - 1].ChildName) == false)
                    {
                        finalList[finalList.Count - 1].ChildName += ";";
                        finalList[finalList.Count - 1].ChildUrl += ";";
                    }
                }
            }

            return finalList;
        }
        public void buildTree(string NodeName, string NodeUrl, int levelNumber, string id)
        {
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (id == item.ID)
                {
                    SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
                    CATALOG_ID = item.Catalog.Trim();
                    REPOSITORY_ID = item.Repository;
                    USERNAME = item.Name;
                    PASSWORD = item.Password;
                    break;
                }
            }
            if (SERVER_URL.Contains("http") == false)
                SERVER_URL = "http://" + SERVER_URL;
            string toolName = string.Empty;
            string userName = USERNAME;
            string password = PASSWORD;
            string repository = REPOSITORY_ID;
            string serverName = SERVER_URL;
            levelNumber++;
            System.Diagnostics.Debug.WriteLine(levelNumber + ": " + NodeName + "    " + NodeUrl);

            if (NodeUrl == "http://www.institute.redlands.edu/Ontologies/SDSS/Software.owl#SDSS")
            {
                System.Diagnostics.Debug.WriteLine("Found it");
            }
            /// original string query = @"select * WHERE {?subject rdfs:subClassOf <" + NodeUrl + "> .  ?subject rdfs:label ?label .  ?subClassOnto rdfs:subClassOf ?subject . ?subClassOnto rdfs:label ?SubClassOntoName . OPTIONAL {?SubClassOnto General:hasGeoDesignLabel ?gdname  } . OPTIONAL {?subject General:hasGeoDesignLabel ?gdNodename  } }";

            //string query = @"select ?predicate ?object ?description ?label where {" + toolName + " ?predicate ?object . ?object rdfs:label ?description .  }";

            string query = @"select * WHERE {?subject rdfs:subClassOf <" + NodeUrl + "> .  ?subject rdfs:label ?label .  }  ";
            //List<Results> results = rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            var request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            IRestResponse response = rep.Execute(request);
            string results = response.Content;

            query = @"select * WHERE {?subject rdfs:subClassOf <" + NodeUrl + "> .  ?subject rdfs:label ?label .  OPTIONAL{ ?subject General:hasSynonym ?synonym  }  }  ";
            //List<Results> results2 = rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            response = rep.Execute(request);
            string results2 = response.Content;

            query = @"select * WHERE {?subject rdfs:subClassOf <" + NodeUrl + "> .  ?subject rdfs:label ?label .OPTIONAL {?subject General:hasAbbreviation ?abbreviation} }  ";
            //List<Results> results3 = rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);
            request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(query);
            response = rep.Execute(request);
            string results3 = response.Content;

            XmlDocument xDoc2 = new XmlDocument();
            if (string.IsNullOrEmpty(results))
                return;
            else
                try
                {
                    xDoc2.LoadXml(results);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception : " + ex.Message + " \r\nError: " + results);
                    return;
                }

            XmlNamespaceManager nsm2 = new XmlNamespaceManager(xDoc2.NameTable);
            nsm2.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");

            XmlDocument xDoc3 = new XmlDocument();
            xDoc3.LoadXml(results2);
            XmlNamespaceManager nsm3 = new XmlNamespaceManager(xDoc3.NameTable);
            nsm3.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");

            XmlDocument xDoc4 = new XmlDocument();
            xDoc4.LoadXml(results3);
            XmlNamespaceManager nsm4 = new XmlNamespaceManager(xDoc4.NameTable);
            nsm4.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");

            XmlNodeList resultNodes = xDoc2.SelectNodes(@"def:sparql/def:results/def:result", nsm2);
            XmlNodeList resultNodes3 = xDoc3.SelectNodes(@"def:sparql/def:results/def:result", nsm3);
            XmlNodeList resultNodes4 = xDoc4.SelectNodes(@"def:sparql/def:results/def:result", nsm4);
            List<string> currentItems = new List<string>();
            List<string> currentUrls = new List<string>();
            List<string> currentGeoNames = new List<string>();
            for (int i = 0; i < resultNodes.Count; i++)
            {
                XmlNode resultNodes2 = resultNodes[i];
                XmlNode resultNodes5 = resultNodes3[i];
                XmlNode resultNodes6 = resultNodes4[i];
                ListItems info = new ListItems();
                info.ParentName = NodeName;
                info.ParentUrl = NodeUrl;
                info.NodeName = resultNodes2.ChildNodes[0].InnerText;
                info.NodeUrl = resultNodes2.ChildNodes[1].InnerText;

                if (resultNodes5.ChildNodes.Count > 2)
                    info.NodeSynonym = resultNodes5.ChildNodes[2].InnerText;
                if (resultNodes6.ChildNodes.Count > 2)
                {
                    info.NodeAbbreviation = resultNodes6.ChildNodes[2].InnerText;
                }

                hierarchyTree.Add(info);
                if (currentItems.Contains(info.NodeName) == false)
                {
                    currentItems.Add(info.NodeName);
                    currentUrls.Add(info.NodeUrl);
                }


            }


            if (currentItems.Count > 0)
            {
                levelnumber = 2;
                for (int i = 0; i < currentItems.Count; i++)
                {
                    buildTree(currentItems[i], currentUrls[i], levelnumber, id);

                }
                // need to add code if there are no entries to check the items for instance info

                return;
            }

            //string toolName = string.Empty;
            //string userName = USERNAME;
            //string password = PASSWORD;
            //string repository = REPOSITORY_ID;
            //string serverName = SERVER_URL;
            //levelNumber++;
            //System.Diagnostics.Debug.WriteLine(levelNumber);
            //if (NodeUrl == "http://www.institute.redlands.edu/Ontologies/SDSS/Software.owl#SDSS")
            //{
            //    System.Diagnostics.Debug.WriteLine("Found it");
            //}
            //string query = @"select * WHERE {?subject rdfs:subClassOf <" + NodeUrl + "> .  ?subject rdfs:label ?label .  ?subClassOnto rdfs:subClassOf ?subject . ?subClassOnto rdfs:label ?SubClassOntoName . }";
            //List<Results> results = rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);

            //XmlDocument xDoc2 = new XmlDocument();
            //xDoc2.LoadXml(results[0].Result.ToString());
            //XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc2.NameTable);
            //nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            //XmlNodeList xNodesSubjectUrl2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[1]/def:uri", nsm);  //user innertext to get the info
            //XmlNodeList xTest2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            //XmlNodeList xNodesSubjectName2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[2]", nsm);
            //XmlNodeList xNodesChildUrl2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[3]", nsm);
            //XmlNodeList xNodesChildName2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[4]", nsm);
            //if (xNodesSubjectName2.Count != xNodesChildName2.Count)
            //{
            //    System.Diagnostics.Debug.WriteLine("There is a problem");
            //}
            //List<string> currentItems = new List<string>();
            //List<string> currentUrls = new List<string>(); 
            //for (int i = 0; i < xNodesSubjectUrl2.Count; i++)
            //{
            //    ListItems info = new ListItems();
            //    info.ParentName = NodeName;
            //    info.ParentUrl = NodeUrl;
            //    info.ChildName = xNodesChildName2[i].InnerText;
            //    info.ChildUrl = xNodesChildUrl2[i].InnerText;
            //    info.NodeName = xNodesSubjectName2[i].InnerText;
            //    info.NodeUrl = xNodesSubjectUrl2[i].InnerText;
            //    hierarchyTree.Add(info);
            //    if (currentItems.Contains(info.NodeName) == false)
            //    {
            //        currentItems.Add(info.NodeName);
            //        currentUrls.Add(info.NodeUrl);
            //    }
            //}
            //// need to add code if there are no entries to check the items for instance info
            //if (xNodesChildName2.Count > 0)
            //{
            //    //for (int i = 0; i < xNodesChildUrl2.Count; i++)
            //    //{
            //    //    if (string.IsNullOrEmpty(xNodesChildUrl2[i].InnerText) == false)
            //    //    {
            //    //    //	buildTree(xNodesChildName2[i].InnerText, xNodesChildUrl2[i].InnerText, levelNumber);

            //    //    }
            //    //    else
            //    //    {
            //    //        levelNumber--;
            //    //        return;
            //    //    }
            //    //}
            //    for (int i = 0; i < currentItems.Count; i++)
            //    {
            //        buildTree(currentItems[i], currentUrls[i], levelnumber);
            //    }
            //    return;
            //}
            //else
            //{
            //    query = @"SELECT * WHERE {?subject a <" + NodeUrl + ">  . ?subject rdfs:label ?label .  }";
            //    List<Results> resultsInstance = rep.evalSparqlQuery(rep.getName(), query, "application/sparql-results+xml", false, null, null, null, string.Empty, userName, password);

            //    XmlDocument xDocInstance = new XmlDocument();
            //    xDocInstance.LoadXml(resultsInstance[0].Result.ToString());
            //    XmlNamespaceManager nsmInstance = new XmlNamespaceManager(xDocInstance.NameTable);
            //    nsmInstance.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            //    XmlNodeList xNodesSubjectUrlInstance = xDocInstance.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[1]/def:uri", nsm);  //user innertext to get the info
            //    XmlNodeList xNodesSubjectNameInstance = xDocInstance.SelectNodes(@"/def:sparql/def:results/def:result/def:binding[2]", nsm);
            //    ListItems info = new ListItems();
            //    if (xNodesSubjectName2.Count == 0)
            //    {

            //        info.ParentName = NodeName;
            //        info.ParentUrl = NodeUrl;
            //        info.ChildName = string.Empty;
            //        info.ChildUrl = string.Empty;
            //        //info.NodeName = xNodesSubjectName2[0].InnerText;
            //        //info.NodeUrl = xNodesSubjectUrl2[0].InnerText;
            //        //hierarchyTree.Add(info);
            //    }
            //    else
            //    {
            //        info.ParentName = NodeName;
            //        info.ParentUrl = NodeUrl;
            //        for (int i = 0; i < xNodesSubjectNameInstance.Count; i++)
            //        {
            //            if (i == xNodesSubjectNameInstance.Count - 1)
            //            {
            //                info.NodeUrl += xNodesSubjectUrlInstance[i].InnerText;
            //                info.NodeName += xNodesSubjectNameInstance[i].InnerText;
            //            }
            //            else
            //            {
            //                info.NodeUrl += xNodesSubjectUrlInstance[i].InnerText + ";";
            //                info.NodeName += xNodesSubjectNameInstance[i].InnerText + ";";
            //            }

            //        }
            //        info.ChildName = string.Empty;
            //        info.ChildUrl = string.Empty;
            //        //info.ChildName = xNodesChildName2[i].InnerText;
            //        //info.ChildUrl = xNodesChildUrl2[i].InnerText;
            //        //info.NodeName = xNodesSubjectName2[i].InnerText;
            //        //info.NodeUrl = xNodesSubjectUrl2[i].InnerText;
            //        hierarchyTree.Add(info);
            //    }





            //}


        }

        /// <summary>
        /// Clean the illegal characters out of a string for use in json
        /// </summary>
        /// <param label="original">Original possibly polluted string</param>
        /// <returns>Cleaned string valid for json</returns>
        string CleanCharacters(string original)
        {
            return FixName(original.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\""));
        }

        /// <summary>
        /// Fix the label to be in the expected namespace:label format
        /// </summary>
        /// <param label="originalName">original label</param>
        /// <returns>label in the proper format</returns>
        string FixName(string originalName)
        {
            if (originalName.StartsWith("http://") && originalName.Contains("#"))
            {
                string label = originalName.Substring(originalName.LastIndexOf("#") + 1);

                string inprogressName = originalName;
                inprogressName = originalName.Substring(0, originalName.LastIndexOf("#"));
                inprogressName = inprogressName.Replace(".owl", "");

                string[] spaceParts = inprogressName.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                string space = spaceParts[spaceParts.Length - 1];

                return string.Format("{0}:{1}", space, label);
            }
            else
            {
                return originalName;
            }
        }
        private void FillOutHierarchyTreeWithChildren(List<ListItems> hierarchyTree)
        {

            for (int currentNodeIndex = 0; currentNodeIndex < hierarchyTree.Count; currentNodeIndex++)
            {
                // grab the current item
                ListItems currentNode = hierarchyTree[currentNodeIndex];
                bool AlreadyHasChildren = false;
                if (string.IsNullOrEmpty(hierarchyTree[currentNodeIndex].ChildName))
                    AlreadyHasChildren = false;
                else
                    AlreadyHasChildren = true;
                // now have to loop through hiearchyTree to see if there are any matches
                for (int indexer = 0; indexer < hierarchyTree.Count; indexer++)
                {
                    if (AlreadyHasChildren)   // for the first two levels,. information is already filled out
                        break;

                    if (indexer != currentNodeIndex)  //ignore the current Node in the evaluation
                    {
                        if (hierarchyTree[indexer].ParentName == currentNode.NodeName)
                        {
                            if (hierarchyTree[currentNodeIndex].ChildUrl.Contains(hierarchyTree[indexer].NodeUrl) == false)
                            {
                                hierarchyTree[currentNodeIndex].ChildName += hierarchyTree[indexer].NodeName + ";";
                                hierarchyTree[currentNodeIndex].ChildUrl += hierarchyTree[indexer].NodeUrl + ";";
                            }
                        }

                    }
                }
            }
        }
    }
    public class ComboResults
    {
        public List<string> nodeNames { get; set; }
        public List<int> nodeIds { get; set; }
        public List<string> nodeUrls { get; set; }
        public List<string> edgeLists { get; set; }
        public ComboResults(List<string> NodeNames, List<string> NodeUrls, List<string> EdgeLists)
        {
            nodeNames = new List<string>();
            nodeUrls = new List<string>();
            edgeLists = new List<string>();
            nodeIds = new List<int>();
            nodeNames = NodeNames;
            nodeUrls = NodeUrls;
            edgeLists = EdgeLists;
        }
    }
    class ListItems
    {
        public string NodeName { get; set; }
        public string NodeUrl { get; set; }
        public string NodeSynonym { get; set; }
        public string NodeAbbreviation { get; set; }
        public string ChildName { get; set; }
        public string ChildUrl { get; set; }
        public string ParentName { get; set; }
        public string ParentUrl { get; set; }
        public ListItems()
        {
            NodeName = string.Empty;
            NodeUrl = string.Empty;
            NodeSynonym = string.Empty;
            NodeAbbreviation = string.Empty;
            ChildUrl = string.Empty;
            ChildName = string.Empty;
            ParentUrl = string.Empty;
            ParentName = string.Empty;
        }

    }

    class Ordering
    {
        public List<Item> Nodes { get; set; }
        public List<Link> Edges { get; set; }
        //public string ParentName { get; set; }
        //public int OrderID { get; set; }
        //public string ChildName { get; set; }

        public Ordering()
        {
            Nodes = new List<Item>();
            Edges = new List<Link>();
        }

        public Ordering(List<Item> nodesList, List<Link> edgeList)
        {
            Nodes = new List<Item>();
            Nodes.AddRange(nodesList);
            Edges = new List<Link>();
            Edges.AddRange(edgeList);
        }
        //public Ordering(string Parent, int OrderNumber, string Child)
        //{
        //    ParentName = Parent;
        //    OrderID = OrderNumber;
        //    ChildName = Child;
        //}
    }

    public class FinalMerge
    {
        public List<NodeItem> nodes { get; set; }
        public List<Link> edges { get; set; }
    }
    public class Item
    {
        public int id { get; set; }
        public string uri { get; set; }
        public string label { get; set; }
    }
    public class NodeItem
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string label { get; set; }
    }

    public class Link
    {
        public object from { get; set; }
        public object to { get; set; }
        public string linkDescription { get; set; }
    }


}