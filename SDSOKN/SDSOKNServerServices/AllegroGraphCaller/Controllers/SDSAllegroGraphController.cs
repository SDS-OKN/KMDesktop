using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SDSAllegroGraphController : ControllerBase
    {
        public SDSAllegroGraphController()
        {
            
        }

        [HttpGet("id/{id}")]
        public string Get(string id)
        {
            OntologyLoading ontologyLoading = new OntologyLoading();
            return ontologyLoading.RetrieveOntologyList(id.Trim());
            
        }
    }

}