using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AllegroGraphCaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SDSClassController : ControllerBase
    {
        [HttpGet("id/{id}")]
        public string Get(string id)
        {
            string basequery = @"SELECT DISTINCT ?subject ?label WHERE { { ?subject a owl:Class . } UNION { ?individual a ?subject . } .    OPTIONAL { ?subject rdfs:label ?label }} ORDER BY ?subject";

            string results = string.Empty;
            OntologyLoading loading = new OntologyLoading();
            results = loading.RetrieveClassList(basequery, id.Trim());


            return results;


        }


        [HttpGet("retrieveItems/id/{classTouse}/{id}")]
        public string GetItems(string classToUse, string id)
        {
            classToUse = HttpUtility.UrlDecode(classToUse);
            if (classToUse.StartsWith("<") == false)
            {
                classToUse = "<" + classToUse + ">";
            }
            //string basequery = @"SELECT DISTINCT ?subject ?label WHERE { { ?subject a owl:Class . } UNION { ?individual a ?subject . } .    OPTIONAL { ?subject rdfs:label ?label }} ORDER BY ?subject";
            //            string basequery = @"SELECT DISTINCT ?subject ?label WHERE {  ?subject rdf:type " + classToUse + " .    OPTIONAL { ?subject rdfs:label ?label }} ORDER BY ?subject";
            // updated 6/24/2020
            string basequery =
                @"SELECT DISTINCT ?property ?label WHERE { ?entity rdf:type ?General_characteristics.?General_characteristics rdfs:subClassOf* " + classToUse + "  . ?entity ?property ?object. OPTIONAL {?property rdfs:label ?label}}";
            string results = string.Empty;
            OntologyLoading loading = new OntologyLoading();
            bool worked = false;
            try
            {
                results = loading.RetrieveClassList(basequery, id.Trim());
                worked = true;
            }
            catch (Exception ex)
            {
            }
            if (!worked)
            {
                basequery = @"SELECT DISTINCT ?subject ?label WHERE { { ?subject a owl:Class . } UNION { ?individual a ?subject . } .    OPTIONAL { ?subject rdfs:label ?label }} ORDER BY ?subject";


                loading = new OntologyLoading();
                results = loading.RetrieveClassList(basequery, id.Trim());

            }

            return results;


        }
    }
}