using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgInstanceController : ControllerBase
    {
        [HttpGet("id/{id}")]
        public string Get(string id)
        {
            string basequery = @"select distinct ?x ?label  where{{?x ?pred ?y} .   OPTIONAL { ?x rdfs:label ?label } . FILTER (?y NOT IN (owl:Class) ) .}";

            string results = string.Empty;
            OntologyLoading loading = new OntologyLoading();
            results = loading.RetrieveInstanceList(basequery, id.Trim());


            return results;


        }



    }
}