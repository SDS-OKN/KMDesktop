using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class prefixesController : ControllerBase
    {

        [HttpGet("retrieve/id/{instanceID}")]
        public string retrieve(string instanceID)
        {
            RestClient rep = new RestClient();
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (instanceID == item.ID)
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
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/text/plain");
            rep.AddDefaultHeader("Accept", @"text/plain");
            var request = new RestRequest();
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"/data/wv.namespaces?allowEmpty=true";
            IRestResponse response = rep.Execute(request);
            string info = response.Content.ToString();
            string results = string.Empty;
            try
            {
                info = info.Replace("namespace", "namespaceName");
                var pf = JsonConvert.DeserializeObject<PrefixNamespace[]>(info);
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("prefix", Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("namespace", Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("isActive", Type.GetType("System.Boolean")));
                foreach (var item in pf)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.prefix;
                    dr[1] = item.namespaceName;
                    dr[2] = true;
                    dt.Rows.Add(dr);
                }
                var writer = new StringWriter();
                if (string.IsNullOrEmpty(dt.TableName))
                {
                    dt.TableName = "rSchema";
                }
                dt.WriteXml(writer, XmlWriteMode.WriteSchema, true);
                System.Text.StringBuilder hex = new System.Text.StringBuilder();
                results = Compressor.Compress(writer.ToString());
                //foreach (byte b in results)
                //{
                //    hex.AppendFormat("{0:x2}", b);
                //}

                return results;
                //results =   
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return results;
        }

        public DataTable ConvertXmlStringToDataTable(string s)
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
        [HttpGet("save/id/{instanceID}/{compresseddata}")]
        public void save(string instanceID, string compresseddata)
        {
            RestClient rep = new RestClient();
            string SERVER_URL = string.Empty;
            string PORT = "0";
            string CATALOG_ID = string.Empty;
            string REPOSITORY_ID = string.Empty;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                if (instanceID == item.ID)
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
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"text/plain");
            rep.AddDefaultHeader("Accept", @"text/plain");
            var request = new RestRequest();
            request.Method = Method.PUT;
            request.Resource = @"catalogs/" + CATALOG_ID + "/repositories/" + REPOSITORY_ID + @"/data/wv.namespaces?allowEmpty=true";
            string datatable1 = Compressor.Decompress(HttpUtility.UrlDecode((compresseddata)));
            DataTable tmpTable = ConvertXmlStringToDataTable(datatable1);
            string json1 = "[";
            foreach (DataRow dr in tmpTable.Rows)
            {
                json1 += "{\"prefix\":\"" + Convert.ToString(dr[0]) + "\",\"namespace\":\"" + Convert.ToString(dr[1]) + "\"},";
            }

            json1 = json1.Remove(json1.Length - 1);
            request.AddParameter("application/json; charset=utf-8", Newtonsoft.Json.JsonConvert.SerializeObject(json1), ParameterType.RequestBody);
            IRestResponse response = rep.Execute(request);

            string results = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return;
        }
    }


    public class Namespaces
    {
        public PrefixNamespace[] PrefixList { get; set; }
    }

    public class PrefixNamespace
    {
        public string prefix { get; set; }
        public string namespaceName { get; set; }
    }


}
