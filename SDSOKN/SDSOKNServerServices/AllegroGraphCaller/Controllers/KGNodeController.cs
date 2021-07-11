using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KGNodeController : ControllerBase
    {
        /// <summary>
        /// Retrieve Top Nodes for the specified ontology/kg
        /// </summary>
        /// <param name="id">Index of loaded kg instance information</param>
        /// <returns></returns>
        [HttpGet("retrieveTopNodes/{id}")]
        public string RetrieveTopNodes(string id)
        {
            List<Ordering> OntologyList = new List<Ordering>();
            string initCall = "select distinct ?class ?name {?resource a ?class . ?class rdfs:label ?name} ";
            string results = string.Empty;
            string SERVER_URL = "http://64.183.179.218:10035";
            string PORT = "10035";
            string CATALOG_ID = "#";
            string REPOSITORY_ID = "SDSOKN";
            string USERNAME = "agadmin";
            string PASSWORD = "Show4time!";
            RestClient rep = new RestClient();
            rep.BaseUrl = new Uri(SERVER_URL);
            rep.Authenticator = new HttpBasicAuthenticator(USERNAME, PASSWORD);
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
            rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            var request = new RestRequest();
            request.Resource ="repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(initCall);
            IRestResponse response = rep.Execute(request);
            results = response.Content;
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(results);
            XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            List<Item> TopLevelNodes = new List<Item>();
            List<Item> TempItemList = new List<Item>();
            List<Item> OriginalItemList = new List<Item>();
            List<string> unsortedList = new List<string>();
            List<Link> EdgesList = new List<Link>();
            // if there are 4 childNodes, then it is className, label of className, relationship to child, childName
            // if there are 6 childNodes, then it is className, label of classname, relationship to child, pointer, ChildReference,  ChildName
            XmlNodeList nodeList = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            for (int i = 0; i < nodeList.Count; i++)
            {
                Item item = new Item();
                item.id = OriginalItemList.Count;
                item.uri = nodeList[i].ChildNodes[0].InnerText;
                item.label = nodeList[i].ChildNodes[1].InnerText;
                if (item.id == 4)
                {
                    System.Diagnostics.Debug.WriteLine("4 added at 1");
                }
                OriginalItemList.Add(item);
                TopLevelNodes.Add(item);
            }
            // now we have the top level items, we need to either the children or the instances at that level
            string countQuery = "select (COUNT(distinct ?entity) as ?items) where {?entity <http://www.w3.org/2000/01/rdf-schema#subClassOf>  ?ENTITYURI?}";
            GenerateNodes(ref results, CATALOG_ID, REPOSITORY_ID, rep, ref request, ref response, TopLevelNodes, OriginalItemList, EdgesList, countQuery);

            string finalstring = "var nodes = [";
            StringBuilder sbNodes = new StringBuilder();
            StringBuilder sbEdges = new StringBuilder();
            foreach (var item in OriginalItemList)
            {
                sbNodes.Append("{id: " + item.id.ToString() + ", label: '" + item.label.Replace("'", " ") + "', title: '" + item.label.Replace("'", " ") + "', uri: '" + item.uri + "'},");
            }
            foreach (var item in EdgesList)
            {
                sbEdges.Append("{from: " + item.from.ToString() + ", to: " + item.to.ToString() + "},");
            }
            finalstring += sbNodes.ToString().Remove(sbNodes.ToString().Length - 1) + "];";

            finalstring += "var edges = [";
            finalstring += sbEdges.ToString().Remove(sbEdges.ToString().Length - 1) + "];";
            results = Newtonsoft.Json.JsonConvert.SerializeObject(finalstring);
            return results;
        }
        
        [HttpGet("retrieveNodes/{id}/{nodeStartUri}")]
        public string RetrieveNodes(string id, string nodeStartUri)
        {
            //   nodeStartUri = "http://" + HttpUtility.UrlDecode(nodeStartUri);
            nodeStartUri = HttpUtility.UrlDecode(nodeStartUri);
            if(nodeStartUri.StartsWith('<') == false)
            {
                nodeStartUri = "<" + nodeStartUri.Trim() + ">";
            }
            List<Ordering> OntologyList = new List<Ordering>();
            string initCall = "select (COUNT(distinct ?entity) as ?items) where {?entity <http://www.w3.org/2000/01/rdf-schema#subClassOf>  " + nodeStartUri + " . } ";
            string results = string.Empty;
            string SERVER_URL = "http://64.183.179.218:10035";
            string PORT = "10035";
            string CATALOG_ID = "importme";
            string REPOSITORY_ID = "sdsokn";
            string USERNAME = "super";
            string PASSWORD = "Show4time!";
            RestClient rep = new RestClient();
            rep.BaseUrl = new Uri(SERVER_URL);
            rep.Authenticator = new HttpBasicAuthenticator(USERNAME, PASSWORD);
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
            rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            var request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(initCall);
            IRestResponse response = rep.Execute(request);
            results = response.Content;
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(results);
            XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            nsm.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
            List<Item> TopLevelNodes = new List<Item>();
            List<Item> TempItemList = new List<Item>();
            List<Item> OriginalItemList = new List<Item>();
            List<string> unsortedList = new List<string>();
            List<Link> EdgesList = new List<Link>();
            
            // if there are 4 childNodes, then it is className, label of className, relationship to child, childName
            // if there are 6 childNodes, then it is className, label of classname, relationship to child, pointer, ChildReference,  ChildName
            XmlNodeList nodeList = xDoc.SelectNodes(@"/def:sparql/def:results/def:result", nsm);
            int count = Convert.ToInt32(nodeList[0].ChildNodes[0].InnerText);
            if (count > 0)
            {
                string q1 = "select ?entity where {?entity <http://www.w3.org/2000/01/rdf-schema#subClassOf> " + nodeStartUri + "}";
                var request2 = new RestRequest();
                request2.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(q1);
                IRestResponse response2 = rep.Execute(request2);
                var results2 = response2.Content;
                XmlDocument xDoc2 = new XmlDocument();
                xDoc2.LoadXml(results2);
                XmlNamespaceManager nsm2 = new XmlNamespaceManager(xDoc2.NameTable);
                nsm2.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                XmlNodeList nodeList2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result", nsm2);
                for (int i = 0; i < nodeList2.Count; i++)
                {
                    Item item = new Item();
                    item.id = OriginalItemList.Count;
                    item.uri = nodeList2[i].ChildNodes[0].InnerText;
                    item.label = nodeList2[i].ChildNodes[1].InnerText;                 
                    OriginalItemList.Add(item);
                    TopLevelNodes.Add(item);
                }
                // now we have the top level items, we need to either the children or the instances at that level
                string countQuery = "select (COUNT(distinct ?entity) as ?items) where {?entity <http://www.w3.org/2000/01/rdf-schema#subClassOf>  ?ENTITYURI?}";
                GenerateNodes(ref results, CATALOG_ID, REPOSITORY_ID, rep, ref request, ref response, TopLevelNodes, OriginalItemList, EdgesList, countQuery);

            }
            else
            {
                // use this one to get the properites : string q1 = "select distinct ?object ?class ?objectname where { <" + nodeStartUri +"> ?object ?class .  OPTIONAL {?object rdfs:label ?objectname .} } ";
                // we need to see if this is an instance or a 
                string q1 = "select ?subject ?class ?info where{?subject  rdf:type  " +  nodeStartUri + " .  ?subject rdfs:label ?info}";
                var request2 = new RestRequest();
                request2.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(q1);
                IRestResponse response2 = rep.Execute(request2);
                var results2 = response2.Content;
                XmlDocument xDoc2 = new XmlDocument();
                xDoc2.LoadXml(results2);
                // this will determine if this is an instance or still a class
                XmlNamespaceManager nsm2 = new XmlNamespaceManager(xDoc2.NameTable);
                nsm2.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                XmlNodeList nodeList2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result", nsm2);
                string uriText = "select ?subject where{  " + nodeStartUri + " rdfs:label ?subject . } ";
                var request2a = new RestRequest();
                request2a.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(uriText);
                IRestResponse response2a = rep.Execute(request2a);
                var results2a = response2a.Content;
                XmlDocument xDoc2a = new XmlDocument();
                xDoc2a.LoadXml(results2a);
                XmlNamespaceManager nsm2a = new XmlNamespaceManager(xDoc2a.NameTable);
                nsm2a.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                XmlNodeList nodeList2a = xDoc2a.SelectNodes(@"/def:sparql/def:results/def:result", nsm2a);
                Item instanceItem = new Item();
                instanceItem.id = OriginalItemList.Count;
                instanceItem.label = nodeList2a[0].ChildNodes[0].InnerText;
                instanceItem.uri = nodeStartUri.Replace('<', ' ').Replace('>', ' ').Trim();
                OriginalItemList.Add(instanceItem);
                TopLevelNodes.Add(instanceItem);
                if (nodeList2.Count > 0)
                {
                    for (int i = 0; i < nodeList2.Count; i++)
                    {
                        Item item = new Item();
                        item.id = OriginalItemList.Count;
                        item.uri = nodeList2[i].ChildNodes[0].InnerText;
                        if (nodeList2[i].ChildNodes.Count > 2)
                            item.label = nodeList2[i].ChildNodes[2].InnerText;
                        else
                            item.label = nodeList2[i].ChildNodes[1].InnerText;
                        OriginalItemList.Add(item);
                        TopLevelNodes.Add(item);

                        EdgesList.Add(new Link() { from = item.id, to = instanceItem.id });
                    }
                }
                else
                {
                    //q1 = select ?subject ?class ?info where{<http://www.sdsconsortium.org/schemas/sds-okn.owl#WQIntv000020015> ?subject ?class . Optional{ ?class rdfs:label ?info }}
                    var request2b = new RestRequest();
                    q1 = "select ?subject ?class ?info where{" + nodeStartUri + "?subject ?class . Optional{ ?class rdfs:label ?info }}";
                    request2b.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(q1);
                    IRestResponse response2b = rep.Execute(request2b);
                    var results2b = response2b.Content;
                    XmlDocument xDoc2b = new XmlDocument();
                    xDoc2b.LoadXml(results2b);
                    // this will determine if this is an instance or still a class
                    XmlNamespaceManager nsm2b = new XmlNamespaceManager(xDoc2b.NameTable);
                    nsm2b.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                    XmlNodeList nodeList2b = xDoc2b.SelectNodes(@"/def:sparql/def:results/def:result", nsm2b);
                    EdgesList.Add(new Link() { from = instanceItem.id, to = instanceItem.id });
                    if (nodeList2b.Count > 0)
                    {
                        for (int i = 0; i < nodeList2b.Count; i++)
                        {

                            Item item = new Item();
                            item.id = OriginalItemList.Count;
                            item.uri = nodeList2b[i].ChildNodes[0].InnerText;
                            if (nodeList2b[i].ChildNodes.Count > 2)
                                item.label = nodeList2b[i].ChildNodes[2].InnerText;
                            else
                                item.label = nodeList2b[i].ChildNodes[1].InnerText;
                            OriginalItemList.Add(item);
                            TopLevelNodes.Add(item);
                        }
                    }
                }

            }
              
            
            string finalstring = "var nodes = [";
            StringBuilder sbNodes = new StringBuilder();
            StringBuilder sbEdges = new StringBuilder();
            foreach (var item in OriginalItemList)
            {
                sbNodes.Append("{id: " + item.id.ToString() + ", label: '" + item.label.Replace("'", " ") + "', title: '" + item.label.Replace("'", " ") + "', uri: '" + item.uri + "'},");
            }
            foreach (var item in EdgesList)
            {
                sbEdges.Append("{from: " + item.from.ToString() + ", to: " + item.to.ToString() + "},");
            }
            finalstring += sbNodes.ToString().Remove(sbNodes.ToString().Length - 1) + "];";

            finalstring += "var edges = [";
            finalstring += sbEdges.ToString().Remove(sbEdges.ToString().Length - 1) + "];";
            results = Newtonsoft.Json.JsonConvert.SerializeObject(finalstring);
            return results;
        }
        private static void GenerateNodes(ref string results, string CATALOG_ID, string REPOSITORY_ID, RestClient rep, ref RestRequest request, ref IRestResponse response, List<Item> TopLevelNodes, List<Item> OriginalItemList, List<Link> EdgesList, string countQuery)
        {
            foreach (var item in TopLevelNodes)
            {
                string countQueryInstance = countQuery.Replace("?ENTITYURI?", "<" + item.uri + ">");
                request = new RestRequest();
                request.Resource = @"repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(countQueryInstance);  // used to start with @"catalogs/" + CATALOG_ID + "/
                response = rep.Execute(request);
                results = response.Content;
                XmlDocument xDoc2 = new XmlDocument();
                xDoc2.LoadXml(results);
                XmlNamespaceManager nsm2 = new XmlNamespaceManager(xDoc2.NameTable);
                nsm2.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                XmlNodeList nodeList2 = xDoc2.SelectNodes(@"/def:sparql/def:results/def:result", nsm2);
                int count = Convert.ToInt32(nodeList2[0].ChildNodes[0].InnerText);
                if (count > 0)
                {
                    //this query string will return the next set of classes
                    string queryNextLevels = "select distinct ?entity ?fullName  where { ?entity <http://www.w3.org/2000/01/rdf-schema#subClassOf> ?ENTITYURI? . ?entity rdfs:label ?fullName}";
                    string queryNextLevelsInstance = queryNextLevels.Replace("?ENTITYURI?", "<" + item.uri + ">");
                    request = new RestRequest();
                    request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(queryNextLevelsInstance);
                    var responseA = rep.Execute(request);
                    string results2 = responseA.Content;
                    XmlDocument xDoc3 = new XmlDocument();
                    xDoc3.LoadXml(results2);
                    XmlNamespaceManager nsm3 = new XmlNamespaceManager(xDoc3.NameTable);
                    nsm3.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                    XmlNodeList nodeList3 = xDoc3.SelectNodes(@"/def:sparql/def:results/def:result", nsm3);
                    for (int i = 0; i < nodeList3.Count; i++)
                    {
                        bool foundMe = false;
                        foreach (Item sItem in OriginalItemList)
                        {
                            if ((nodeList3[i].ChildNodes[0].InnerText == sItem.uri) && (nodeList3[i].ChildNodes[1].InnerText == sItem.label))
                            {
                                foundMe = true;
                                break;
                            }
                        }
                        if (!foundMe)
                        {
                            Item itemNew = new Item();
                            itemNew.id = OriginalItemList.Count;
                            itemNew.uri = nodeList3[i].ChildNodes[0].InnerText;
                            itemNew.label = nodeList3[i].ChildNodes[1].InnerText;
                            if (itemNew.id == 4)
                            {
                                System.Diagnostics.Debug.WriteLine("4 added at 2");
                            }
                            OriginalItemList.Add(itemNew);
                            Link link = new Link();
                            link.from = item.id;
                            link.to = itemNew.id;
                            EdgesList.Add(link);
                            string entityListQuery = "SELECT ?entity ?fullName WHERE{   ?entity a ?ENTITYURI?. ?entity rdfs:label ?fullName}";
                            string entityListQueryInstance = entityListQuery.Replace("?ENTITYURI?", "<" + itemNew.uri + ">");
                            request = new RestRequest();
                            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(entityListQueryInstance);
                            var response3 = rep.Execute(request);
                            string results3 = response3.Content;
                            XmlDocument xDoc4 = new XmlDocument();
                            xDoc4.LoadXml(results3);
                            XmlNamespaceManager nsm4 = new XmlNamespaceManager(xDoc4.NameTable);
                            nsm4.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                            XmlNodeList nodeList4 = xDoc4.SelectNodes(@"/def:sparql/def:results/def:result", nsm4);
                            for (int j = 0; j < nodeList4.Count; j++)
                            {
                                bool found = false;
                                foreach (Item sItem in OriginalItemList)
                                {
                                    if ((nodeList4[i].ChildNodes[0].InnerText == sItem.uri) && (nodeList4[i].ChildNodes[1].InnerText == sItem.label))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    Item itemNew2 = new Item();
                                    itemNew2.id = OriginalItemList.Count;
                                    itemNew2.uri = nodeList4[i].ChildNodes[0].InnerText;
                                    itemNew2.label = nodeList4[i].ChildNodes[1].InnerText;
                                    OriginalItemList.Add(itemNew2);
                                    Link link2 = new Link();
                                    link2.from = itemNew.id;
                                    link2.to = itemNew2.id;
                                    EdgesList.Add(link2);
                                }
                            }
                        }
                    }
                    // this
                    string nextlevelQuery = "select distinct ?class ?name ?properties ?items ?propertiesURI ?propertiesLabel where { ?class <http://www.w3.org/2000/01/rdf-schema#subClassOf> ?ENTITYNAME? . ?class rdfs:label ?name. ?class ?properties ?items .  OPTIONAL { ?items owl:onProperty ?propertiesURI . ?propertiesURI rdfs:label ?propertiesLabel .} }";

                }
                else
                {
                    // add the entities to the list
                    string entityListQuery = "SELECT ?entity ?fullName WHERE{   ?entity a ?ENTITYURI?. ?entity rdfs:label ?fullName}";
                    string entityListQueryInstance = entityListQuery.Replace("?ENTITYURI?", "<" + item.uri + ">");
                    request = new RestRequest();
                    request.Resource = "repositories/" + REPOSITORY_ID + @"?query=" + System.Web.HttpUtility.UrlEncode(entityListQueryInstance);
                    response = rep.Execute(request);
                    string results2 = response.Content;
                    XmlDocument xDoc3 = new XmlDocument();
                    xDoc3.LoadXml(results2);
                    XmlNamespaceManager nsm3 = new XmlNamespaceManager(xDoc3.NameTable);
                    nsm3.AddNamespace("def", "http://www.w3.org/2005/sparql-results#");
                    XmlNodeList nodeList3 = xDoc3.SelectNodes(@"/def:sparql/def:results/def:result", nsm3);
                    bool found = false;
                    for (int i = 0; i < nodeList3.Count; i++)
                    {

                        foreach (Item sItem in OriginalItemList)
                        {
                            if ((nodeList3[i].ChildNodes[0].InnerText == sItem.uri) && (nodeList3[i].ChildNodes[1].InnerText == sItem.label))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            Item itemNew = new Item();
                            itemNew.id = OriginalItemList.Count;
                            itemNew.uri = nodeList3[i].ChildNodes[0].InnerText;
                            itemNew.label = nodeList3[i].ChildNodes[1].InnerText;
                            OriginalItemList.Add(itemNew);
                            Link link = new Link();
                            link.from = item.id;
                            link.to = itemNew.id;
                            EdgesList.Add(link);
                        }
                    }
                }
            }
        }
    }
}