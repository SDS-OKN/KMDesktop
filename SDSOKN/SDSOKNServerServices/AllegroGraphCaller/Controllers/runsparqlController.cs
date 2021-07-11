using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AllegroGraphNetCoreClient.Mini;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;

namespace AllegroGraphCaller.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class runsparqlController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            string finalItem = string.Empty;
            foreach (var item in Program.AllegroGraphRegistry)
            {
                finalItem += "ServerUrl = " + item.Url + " with Item ID = " + item.ID + Environment.NewLine;
            }
            return finalItem;
        }

        //[HttpGet("id/{id}")]
        //public string runInfo(string id)
        //{
        //    string SERVER_URL = string.Empty;
        //    string PORT = "0";
        //    string CATALOG_ID = string.Empty;
        //    string REPOSITORY_ID = string.Empty;
        //    string USERNAME = string.Empty;
        //    string PASSWORD = string.Empty;
        //    foreach (var item in Program.AllegroGraphRegistry)
        //    {
        //        if (id.Trim() == item.ID.Trim())
        //        {
        //            SERVER_URL = item.Url + ":" + Convert.ToString(item.Port);
        //            CATALOG_ID = item.Catalog.Trim();
        //            REPOSITORY_ID = item.Repository;
        //            USERNAME = item.Name;
        //            PASSWORD = item.Password;
        //            break;
        //        }
        //    }

        //    return SERVER_URL + "/" + CATALOG_ID + "/" + REPOSITORY_ID;
        //}

        [HttpGet("executesparql/{id}/{statement}")]
        public string executesparql(string id, string statement)
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
                                              HttpUtility.UrlEncode(statement.Replace('+', ' ')));

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
            return results;
        }
    }
}
