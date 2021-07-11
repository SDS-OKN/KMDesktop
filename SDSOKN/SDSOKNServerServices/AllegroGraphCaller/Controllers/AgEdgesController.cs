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
    public class AgEdgesController : ControllerBase
    {
        public AgEdgesController()
        {

        }

        [HttpGet("id/{id}")]
        public string Get(string id)
        {
            OntologyLoading ontologyLoading = new OntologyLoading();
            return ontologyLoading.RetrieveOntologyList2(id.Trim());

        }
    }
}