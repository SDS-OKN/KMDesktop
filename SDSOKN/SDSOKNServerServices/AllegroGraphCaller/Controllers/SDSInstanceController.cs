using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SDSInstanceController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hi";
        }

        [HttpGet("uri/{uri}/{id}")]
        public string Get(string uri, string id)
        {

            string basequery = @"select distinct ?x ?label  where{{?x ?pred ?y} .   OPTIONAL { ?x rdfs:label ?label } . FILTER (?y IN (" + uri + ") ) .}";
            basequery = @"select distinct ?x ?label  where{{?x ?pred ?y} .   OPTIONAL { ?x rdfs:label ?label } . FILTER (?y IN (<" + HttpUtility.UrlDecode(uri) + ">) ) .}";
            // new basequery as of 6/23/2020
            basequery = "select distinct ?x ?label where {?x ?info <" + HttpUtility.UrlDecode(uri) +
                        "> .  OPTIONAL {?x rdfs:label ?label .} Filter (?info = <http://www.w3.org/1999/02/22-rdf-syntax-ns#type>)}";
            string results = string.Empty;
            OntologyLoading loading = new OntologyLoading();
            results = loading.RetrieveInstanceList(basequery, id.Trim());


            return results;

        }
    }
}