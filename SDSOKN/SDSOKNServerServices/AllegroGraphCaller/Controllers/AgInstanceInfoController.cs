using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
    public class AgInstanceInfoController : ControllerBase
    {

        [HttpGet()]
        public string Get()
        {
            string basequery = @"select distinct ?x ?label  where{{?x ?pred ?y} .   OPTIONAL { ?x rdfs:label ?label } . FILTER (?y NOT IN (owl:Class) ) .}";

            string results = string.Empty;
            OntologyLoading loading = new OntologyLoading();
            results = loading.RetrieveInstanceList(basequery, "0");


            return results;


        }
        //select distinct ?property ?value ?propertyLabel ?valueLabel  where {  <http://www.institute.redlands.edu/Ontologies/SDSS/Person.owl#StevenSchill> ?property ?value . OPTIONAL{ ?property rdfs:label ?propertyLabel} . OPTIONAL {?value rdfs:label ?valueLabel}. }

        [HttpGet("uri/{uri}/{id}/{classuri}")]
        public string Get(string uri, string id, string classuri)
        {

            string basequery = @"select distinct ?x ?label  where{{?x ?pred ?y} .   OPTIONAL { ?x rdfs:label ?label } . FILTER (?y IN (" + uri + ") ) .}";
            basequery =  "<" + HttpUtility.UrlDecode(uri) + ">" ;//@"select distinct ?property ?value ?propertyLabel ?valueLabel  where {<" + HttpUtility.UrlDecode(uri) + "> ?property ?value . OPTIONAL{ ?property rdfs:label ?propertyLabel} . OPTIONAL {?value rdfs:label ?valueLabel}. }";
            // new class as of 6/23/2020
            string classbasequery = @"SELECT DISTINCT ?property ?label WHERE { ?entity rdf:type ?General_characteristics . ?General_characteristics rdfs:subClassOf* <" + HttpUtility.UrlDecode(classuri) + "> . ?entity ?property ?object . OPTIONAL {?property rdfs:label ?label} }";

            string results = string.Empty;
            OntologyLoading loading = new OntologyLoading();

            results = loading.RetrieveInstanceInfo(basequery, classbasequery, id.Trim());
            results = Compressor.Compress(results);
            return results;

        }
    }

    public static class Compressor
    {

        /// &lt;summary&gt;
        /// Use this to compress UTF-8 string to Base-64 string.
        /// &lt;/summary&gt;
        /// &lt;param name="text"&gt;The string value to compress.&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public static string Compress(this string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var stream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            memoryStream.Position = 0;
            var compressed = new byte[memoryStream.Length];
            memoryStream.Read(compressed, 0, compressed.Length);
            var gZipBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gZipBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// &lt;summary&gt;
        /// Use this to decompress Base-64 string to UTF-8 string.
        /// &lt;/summary&gt;
        /// &lt;param name="compressedText"&gt;&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public static string Decompress(this string compressedText)
        {
            try
            {
                var gZipBuffer = Convert.FromBase64String(compressedText);
                using var memoryStream = new MemoryStream();
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                var buffer = new byte[dataLength];
                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return compressedText;
            }
        }
    }
}