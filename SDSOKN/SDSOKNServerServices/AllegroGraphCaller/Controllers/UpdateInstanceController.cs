using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AllegroGraphNetCoreClient.Mini;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateInstanceController : ControllerBase
    {

        [HttpGet("updateStatement/{originalID}/{originalstatement}/{updatestatement}/{id}")]
        public void updateStatement(string originalID, string originalstatement, string updatestatement, string id)
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
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/json");
            rep.AddDefaultHeader("Accept", @"application/json");


            string[] args = { ";;;;;;;;" };
            string[] statementParts = originalstatement.Split(args, StringSplitOptions.RemoveEmptyEntries);
            string[] newStatementParts = updatestatement.Split(args, StringSplitOptions.RemoveEmptyEntries);
            string t = string.Empty;
            JsonArray arr = new JsonArray();
            //if (statementParts.Length > 2)
            //{

            //    string subject = HttpUtility.UrlDecode(statementParts[0]);
            //    string predicate = HttpUtility.UrlDecode(statementParts[1]);
            //    string objectText = HttpUtility.UrlDecode(statementParts[2]);
            //    if (subject.StartsWith("http"))
            //    {
            //        subject = "<" + subject + ">";
            //    }
            //    else
            //    {
            //        if (subject.StartsWith("\"") == false)
            //            subject = "\"" + subject + "\"";
            //    }

            //    if (predicate.StartsWith("http"))
            //    {
            //        predicate = "<" + predicate + ">";
            //    }
            //    else
            //    {
            //        if (predicate.StartsWith("\"") == false)
            //            predicate = "\"" + predicate + "\"";
            //    }


            //    if (objectText.StartsWith("http"))
            //    {
            //        objectText = "<" + objectText + ">";
            //    }
            //    else
            //    {
            //        if (predicate.StartsWith("\"") == false)
            //            objectText = "\"" + objectText + "\"";
            //    }

            //    arr.Add(subject);
            //    arr.Add(predicate);
            //    arr.Add(objectText);
            //    //arr.Add(null);
            //    //arr.Add(null);
            //    t = "[\"" + subject + "\",\"" + predicate + "\"," + objectText + ",\"\"]";
            //}
            //List<List<string>> quads = new List<List<string>>();
            //quads.Add(quad);

            bool success = false;
            try
            {
                var request = new RestRequest("catalogs/" + CATALOG_ID + "/repositories/" + repository + "/statements/delete?ids=true");
                JsonArray json1 = new JsonArray();
                json1.Add(originalID);
                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json; charset=utf-8", Newtonsoft.Json.JsonConvert.SerializeObject(json1), ParameterType.RequestBody);
                IRestResponse response = rep.Execute(request);
                string results2 = response.Content.ToString() + Environment.NewLine;
                System.Diagnostics.Debug.WriteLine(results2);
                // agRepo.DeleteStatements(quads);
                success = true;
                ;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            JsonArray masterArray2 = new JsonArray();
            JsonArray arr1 = new JsonArray();
            if (newStatementParts.Length > 2)
            {

                string subject = HttpUtility.UrlDecode(newStatementParts[0]);
                if (subject.StartsWith("http"))
                {
                    subject = "<" + subject + ">";
                }
                else
                {
                    if (subject.StartsWith("\"") == false)
                        subject = "\"" + subject + "\"";
                }
                string predicate = HttpUtility.UrlDecode(newStatementParts[1]);
                if (predicate.StartsWith("http"))
                {
                    predicate = "<" + predicate + ">";
                }
                else
                {
                    if (predicate.StartsWith("\"") == false)
                        predicate = "\"" + predicate + "\"";
                }
                string objectText = HttpUtility.UrlDecode(newStatementParts[2]);

                if (objectText.StartsWith("http"))
                {
                    objectText = "<" + objectText + ">";
                }
                else
                {
                    if (predicate.StartsWith("\"") == false)
                        objectText = "\"" + objectText + "\"";
                }

                arr1.Add(subject);
                arr1.Add(predicate);
                arr1.Add(objectText);

            }


            try
            {
                JsonArray jr1 = new JsonArray();
                jr1.Add(arr1);
                var request = new RestRequest("catalogs/" + CATALOG_ID + "/repositories/" + repository + "/statements");

                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json; charset=utf-8", Newtonsoft.Json.JsonConvert.SerializeObject(jr1), ParameterType.RequestBody);
                IRestResponse response = rep.Execute(request);
                string results2 = response.Content.ToString() + Environment.NewLine;
                System.Diagnostics.Debug.WriteLine(results2);
                // agRepo.DeleteStatements(quads);
                success = true;
                success = true;
                ;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return;
        }

        [HttpGet("insertStatement/{statement}/{id}")]
        public void insertStatement(string statement, string id)
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
            //  rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
            // rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/json");
            rep.AddDefaultHeader("Accept", @"application/json");
            JsonArray arr = new JsonArray();
            string[] args = { ";;;;;;;;" };
            string[] statementParts = statement.Split(args, StringSplitOptions.RemoveEmptyEntries);
            JsonArray masterArray2 = new JsonArray();
            JsonArray arr1 = new JsonArray();
            if (statementParts.Length > 2)
            {

                string subject = HttpUtility.UrlDecode(statementParts[0]);
                if (subject.StartsWith("http"))
                {
                    subject = "<" + subject + ">";
                }
                else
                {
                    if (subject.StartsWith("\"") == false)
                        subject = "\"" + subject + "\"";
                }
                string predicate = HttpUtility.UrlDecode(statementParts[1]);
                if (predicate.StartsWith("http"))
                {
                    predicate = "<" + predicate + ">";
                }
                else
                {
                    if (predicate.StartsWith("\"") == false)
                        predicate = predicate;
                }
                string objectText = HttpUtility.UrlDecode(statementParts[2]);

                if (objectText.StartsWith("http"))
                {
                    objectText = "<" + objectText + ">";
                }
                else
                {
                    if (predicate.StartsWith("\"") == false)
                        objectText = "\"" + objectText + "\"";
                }

                arr1.Add(subject);
                arr1.Add(predicate);
                arr1.Add(objectText);

            }


            try
            {
                JsonArray jr1 = new JsonArray();
                jr1.Add(arr1);
                var request = new RestRequest("catalogs/" + CATALOG_ID + "/repositories/" + repository + "/statements");

                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json; charset=utf-8", Newtonsoft.Json.JsonConvert.SerializeObject(jr1), ParameterType.RequestBody);
                IRestResponse response = rep.Execute(request);
                string results2 = response.Content.ToString() + Environment.NewLine;
                System.Diagnostics.Debug.WriteLine(results2);
                // agRepo.DeleteStatements(quads);


                ;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return;
        }

    }

}