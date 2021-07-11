using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllegroGraphNetCoreClient.Mini;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddAgDataController : ControllerBase
    {
        [HttpGet("addTriple/{id}/{subject}/{predicate}/{objectText}")]
        public bool addTriple(string id, string subject, string predicate, string objectText)
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
            rep.ClearHandlers(); rep.AddDefaultHeader("Content-Type", @"application/sparql-results+xml");
            rep.AddDefaultHeader("Accept", @"application/sparql-results+xml");
            var request = new RestRequest();
            AgServerInfo agServer = new AgServerInfo(SERVER_URL, userName, password);
            AgCatalog agCatalog = new AgCatalog(agServer, CATALOG_ID);
            AgRepository agRepo = new AgRepository(agCatalog, REPOSITORY_ID);
            List<string> quad = new List<string>();
            quad.Add(subject + "," + predicate + "," + objectText + ",\"\"");
            List<List<string>> quads = new List<List<string>>();
            quads.Add(quad);
            bool success = false;
            try
            {
                agRepo.AddStatements(quads);
                success = true;
                ;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return true;
        }

    }
}