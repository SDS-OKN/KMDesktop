using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllegroGraphNetCoreClient.Mini;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListRepositoriesController : ControllerBase
    {
        [HttpGet("repos/{serverName}/{port}/{username}/{password}/{catalogName}")]
        public string GetRepositories(string serverName, string port, string username, string password,string catalogName)
        {
            AgServerInfo info = new AgServerInfo("http://" + serverName + ":" + port, username, password);
            AgCatalog catalog = new AgCatalog(info, catalogName);
//            AgRepository repository = new AgRepository(catalog,catalogName);
            List<string> items2 = catalog.ListRepositories();
            ////TestCase-1
            //List<string> subj = new List<string>() { "<http://www.institute.redlands.edu/Ontologies/SDSS/Person.owl#Person>" };
            //List<string> pred = null;
            //List<string> obj = null;
            //List<string> context = null;
            //List<List<string>> result = repository.GetStatements(subj, pred, obj, context);

            string finalResponse = string.Empty;
            foreach (var s in items2)
            {
               
                finalResponse += s + Environment.NewLine;
            }
            return finalResponse;
        }
        [HttpGet("catalogs/{serverName}/{port}/{username}/{password}")]
        public string GetCatalogs(string serverName, string port, string username, string password)
        {
            AgServerInfo info = new AgServerInfo("http://" + serverName + ":" + port, username, password);
            AgCatalog catalog = new AgCatalog(info, "system");
            AgClient client = new AgClient(info);
            //AgRepository repository = new AgRepository(catalog, "SDSS");
            List<string> items2 = client.ListCatalogs();
            //TestCase-1
            
            string finalResponse = string.Empty;
            for(int i = 1; i < items2.Count;i++)
            {                
                finalResponse += items2[i] + Environment.NewLine;
            }
            return finalResponse;
        }
    }
}