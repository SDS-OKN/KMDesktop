using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Web;
using AllegroGraphNetCoreClient.OpenRDF.Model;
// ReSharper disable MethodOverloadWithOptionalParameter
// ReSharper disable InvalidXmlDocComment

namespace AllegroGraphNetCoreClient.Mini
{
    public class AgRepository :IAgUrl
    {
#pragma warning disable IDE0044 // Add readonly modifier

        private string _repoUrl;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private AgCatalog _catalog;

                               // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string _name;
#pragma warning restore IDE0044 // Add readonly modifier
        /// <summary>
        /// Construct a repository from the catalog it belongs to
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="name"></param>
        public AgRepository(AgCatalog catalog, string name)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            _repoUrl = catalog.Url + "/repositories/" + name;
            this._name = name;
            this._catalog = catalog;
        }

        /// <summary>
        /// Constructor for OpenSession
        /// </summary>
        /// <param name="repoUrl">Session URL</param>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        public AgRepository(string repoUrl, string userName, string password)
        {
            this._repoUrl = repoUrl;
            this._catalog = new AgCatalog(new AgServerInfo(repoUrl, userName, password), null);
        }

        /// <summary>
        /// Repository Url
        /// </summary>
        public string Url
        {
            get { return _repoUrl; }
            set { _repoUrl = value; }
        }
        public string Username { get { return _catalog.Username; } }
        public string Password { get { return _catalog.Password; } }
        public string DatabaseName { get { return this._name; } }

        /// <summary>
        /// Get the size of the repository
        /// </summary>
        /// <param name="context">Can specify a named graph as context</param>
        /// <returns>The size of repository</returns>
        public int GetSize(string context = null)
        {
            Dictionary<string, object> parameters = null;
            if (context != null)
            {
                parameters = new Dictionary<string, object> {{"context", context}};
            }
            return AgRequestService.DoReqAndGet<int>(this, "GET", "/size", parameters);
        }

        /// <summary>
        /// Returns the catalog it belongs to
        /// </summary>
        /// <returns>The catalog it belongs to</returns>
        public AgCatalog GetCatalog()
        {
            return this._catalog;
        }

        /// <summary>
        /// List the contexts in this repository
        /// </summary>
        /// <returns>The names of contexts</returns>
        public List<string> ListContexts()
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/contexts");
        }

        /// <summary>
        /// Get all blank nodes in this repository
        /// </summary>
        /// <param name="amount">The amount</param>
        /// <returns>The URIs of blank nodes</returns>
        public List<string> GetBlankNodes(int amount = 1)
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "POST", $"/blankNodes?amount={amount}");
        }

        /// <summary>
        /// Add a new statement
        /// </summary>
        /// <param name="quads">A list of quadtuple, which consists of subject, predicate, object, and context</param>
        public void AddStatements(List<List<string>> quads)
        {
            AgRequestService.DoReq(this, "POST", "/statements", quads);
        }

        /// <summary>
        /// Delete the matching statements
        /// </summary>
        /// <param name="subj">Subject</param>
        /// <param name="pred">Predicate</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>The number of tuples deleted</returns>
        public int DeleteMatchingStatements(string subj, string pred, string obj, string context)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            if (subj != null)
                body.Add("subj", subj);
            if (pred != null)
                body.Add("pred", pred);
            if (obj != null)
                body.Add("obj", obj);
            if (context != null)
                body.Add("context", context);
            return AgRequestService.DoReqAndGet<int>(this, "DELETE", "/statements", body);
        }

        /// <summary>
        /// Remove the given statements
        /// </summary>
        /// <param name="quads">Statements to be deleted</param>
        public void DeleteStatements(List<List<string>> quads)
        {
            AgRequestService.DoReq(this, "POST", "/statements/delete", quads);
        }

        /// <summary>
        /// Execute SPARQL Query
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="infer">Infer option, can be "false","rdfs++","restriction"</param>
        /// <param name="context">Context</param>
        /// <param name="namedContext">Named Context</param>
        /// <param name="bindings">Local bindings for variables</param>
        /// <param name="checkVariables">Whether to check the non-existing variable</param>
        /// <param name="limit">The size limit of result</param>
        /// <param name="offset">Skip some of the results at the start</param>
        /// <returns>A raw string representing the result, encoded in JSON format</returns>
        public string EvalSparqlQuery(string query, string infer = "false", string context = null, string namedContext = null,
           Dictionary<string, string> bindings = null, bool checkVariables = false, int limit = -1, int offset = -1)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"query", query}, {"queryLn", "sparql"}, {"infer", infer}
            };
            if (context != null) parameters.Add("context", context);
            if (namedContext != null) parameters.Add("namedContext", namedContext);
            if (bindings != null)
            {
                foreach (string vari in bindings.Keys)
                    parameters.Add("$" + vari, HttpUtility.UrlEncode(bindings[vari]));
            }
            parameters.Add("checkVariables", checkVariables.ToString());
            if (limit >= 0) parameters.Add("limit", limit.ToString());
            if (offset >= 0) parameters.Add("offset", offset.ToString());
            return AgRequestService.DoReqAndGet(this, "GET", "", parameters);
        }

        /// <summary>
        /// Execute prolog query
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="infer">Infer type <see cref="EvalSparqlQuery"/></param>
        /// <param name="limit">The size limit of result</param>
        /// <param name="count">If true, counting number of the result will be returned</param>
        /// <param name="accept">Accept header in HTTP request</param>
        /// <returns>A raw result, either the query result, or the count of it</returns>
        public string EvalPrologQuery(string query, string infer = "false", int limit = -1, bool count = false, string accept = null)
        {
            if (accept == null)
            {
                if (count) accept = "text/integer";
                else accept = "application/json";
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"query", query}, {"queryLn", "prolog"}, {"infer", infer}, {"accept", accept}
            };
            if (limit >= 0) parameters.Add("limit", limit.ToString());

            // ReSharper disable once RedundantArgumentDefaultValue
            return AgRequestService.DoReqAndGet(this, "POST", null, parameters, needsAuth: true);
        }


        /// <summary>
        /// Translate result to Data.DataTable
        /// </summary>
        /// <param name="result">The raw result of query</param>
        /// <returns>A datatable</returns>
        // ReSharper disable once UnusedMember.Global
#pragma warning disable CA1822 // Mark members as static
        public DataTable QueryResultToDataTable(string result)
#pragma warning restore CA1822 // Mark members as static
        {
            JsonDocument rawResult = JsonDocument.Parse(result);
            JsonElement root = rawResult.RootElement;
            var headers = root.GetProperty("names");
            var contents = root.GetProperty("values");
            //JsonElement[] headers = rawResult.RootElement.EnumerateArray()["names"];
            //JsonElement[] contents = rawResult["values"];

          // [headers.EnumerateArray().Count()][];

            DataTable resultTable = new DataTable();
            foreach (var columnName in headers.EnumerateArray())
                resultTable.Columns.Add(new DataColumn(columnName.GetString()));

            foreach (var rowObj in contents.EnumerateArray())
            {
                DataRow aRow = resultTable.NewRow();
                int index = 0;
                foreach (var cell in rowObj.EnumerateArray())
                    aRow[index++] = cell.GetString();
                resultTable.Rows.Add(aRow);
            }
            return resultTable;
        }

        /// <summary>
        /// Translate result to string array,the first row is names,the rests are values
        /// </summary>
        /// <param name="result">The raw result</param>
        /// <returns>The translated array</returns>
        // ReSharper disable once UnusedMember.Global
#pragma warning disable CA1822 // Mark members as static
        public List<List<string>> QueryResultToArray(string result)
#pragma warning restore CA1822 // Mark members as static
        {
            JsonDocument rawResult = JsonDocument.Parse(result);
            JsonElement root = rawResult.RootElement;
            var headers = root.GetProperty("names");
            var contents = root.GetProperty("values");
            //JsonElement[] headers = rawResult.RootElement.EnumerateArray()["names"];
            //JsonElement[] contents = rawResult["values"];

            List<List<string>> resultArray = new List<List<string>>(); //string[headers.EnumerateArray().Count()][];
            int index = 0;
            resultArray.Add(new List<string>()); //new string[headers.EnumerateArray().Count()];
            foreach (var columnName in headers.EnumerateArray())
            {
                resultArray[0][index++] = columnName.GetString();
            }
            int row = 1;
            foreach (var rowObj in contents.EnumerateArray())
            {
                resultArray[row++] = new List<string>();//new string[rowObj.GetArrayLength()];
                foreach (var cell in rowObj.EnumerateArray())
                    resultArray[row].Add(cell.GetString()); 
            }
            return resultArray;
        }

        /// <summary>
        /// Get the specified statements
        /// </summary>
        /// <param name="subj">Subject constraints, can be given multiple times</param>
        /// <param name="pred">Predicate constraints, can be given multiple times</param>
        /// <param name="obj">Object constraints, can be given multiple times</param>
        /// <param name="context">Context constraints, can be given multiple times</param>
        /// <param name="infer">Infer type, default set to "False"</param>
        /// <param name="limit">The size limit of result</param>
        /// <param name="offset">Skip some of the results at the start</param>
        /// <returns>Found statements with tripleId</returns>
        public List<List<string>> GetStatements(List<string> subj, List<string> pred, List<string> obj, List<string> context, string infer = "false",
            int limit = -1, int offset = -1)
        {
            if (infer == null) throw new ArgumentNullException(nameof(infer));

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            void AddArrayParam(string key, List<string> param)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (param != null && param.Count > 0)
                {
                    if (param.Count == 1)
                        parameters.Add(key, param[0]);
                    else
                        parameters.Add(key, param);
                }
            }

            AddArrayParam("subj", subj);
            AddArrayParam("pred", pred);
            AddArrayParam("obj", obj);
            AddArrayParam("context", context);
            if (limit >= 0) parameters.Add("limit", limit.ToString());
            if (offset >= 0) parameters.Add("offset", offset.ToString());
            return AgRequestService.DoReqAndGet<List<List<string>>>(this, "GET", "/statements", "application/x-quints+json", parameters);
        }

        /// <summary>
        /// Return all statements whose triple ID matches an ID in the set 'ids'.
        /// </summary>
        /// <param name="ids">Id constraints</param>
        /// <param name="returnIDs">Whether to return ids</param>
        /// <returns>A raw result, eithor the statements or their ids</returns>
        public List<List<string>> GetStatementsById(string ids, bool returnIDs = true)
        {
            string accept;
            if (returnIDs)
            {
                accept = "application/x-quints+json";
            }
            else
            {
                accept = "application/json";
            }
            return AgRequestService.DoReqAndGet<List<List<string>>>(this, "GET", "/statements/id?id=" + ids, accept);
        }


        public List<string> GetStatementIDs(List<string> subj, List<string> pred, List<string> obj, List<string> context,
#pragma warning disable IDE0060 // Remove unused parameter
                                        string infer = "false", int limit = -1, int offset = -1)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            List<List<string>> results = this.GetStatements(subj, pred, obj, context);
            List<string> ids = new List<string>();
            for (int i = 0; i < results.Count; i++)
            {
                ids[i] = results[i][0];
            }
            return ids;
        }

        public List<string> GetStatementIDs()
        {
            List<List<string>> results = this.GetStatements(null, null, null, null);
            List<string> ids = new List<string>();
            for (int i = 0; i < results.Count; i++)
            {
                ids[i] = results[i][0];
            }
            return ids;
        }

        /// <summary>
        /// Delete statements by their ids
        /// </summary>
        /// <param name="ids">The ids of the statements</param>
        public void DeleteStatementsById(List<string> ids)
        {
            AgRequestService.DoReq(this, "POST", "/statements/delete?ids=true", JsonSerializer.Serialize(ids));
        }

        /// <summary>
        /// Deletes all duplicate statements that are currently present in the store
        /// </summary>
        /// <param name="indexMode">The indexmode can be either spog (the default) or spo to indicate</param>
        public void DeleteDuplicateStatements(string indexMode = "spog")
        {
            AgRequestService.DoReq(this, "DELETE", "/statements/duplicates?mode=" + indexMode);
        }

        /// <summary>
        /// List the namespaces of the current repository
        /// </summary>
        /// <returns>A list of namespace</returns>
        public List<Namespace> ListNamespaces()
        {
            return AgRequestService.DoReqAndGet<List<Namespace>>(this, "GET", "/namespaces");
        }

        /// <summary>
        /// Returns the namespace URI defined for the given prefix. 
        /// </summary>
        /// <param name="prefix">The prefix of the namespace</param>
        /// <returns>The namespace's name</returns>
        public string GetNamespaces(string prefix)
        {
            return AgRequestService.DoReqAndGet<string>(this, "GET", "/namespaces/" + prefix);
        }

        /// <summary>
        /// Add a namespace
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="nsUrl">Namespace's URL</param>
        public void AddNamespace(string prefix, string nsUrl)
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            AgRequestService.DoReq(this, "POST", "/namespaces/" + prefix, "text/plain", nsUrl, true);
        }

        /// <summary>
        /// Delete all the namespaces with the prefix
        /// </summary>
        /// <param name="prefix">Given prefix</param>
        public void DeleteNamespace(string prefix)
        {
            AgRequestService.DoReq(this, "DELETE", "/namespaces/" + prefix);
        }

        /// <summary>
        /// Deletes all namespaces in this repository for the current user.
        /// </summary>
        /// <param name="reset">
        /// If a reset argument of true is passed, the user's namespaces are reset to the default set of namespaces. 
        ///</param>
        public void ClearNamespaces(bool reset = true)
        {
            AgRequestService.DoReq(this, "DELETE", $"/namespaces/reset={reset}");
        }

        /// <summary>
        /// Load a given file into AG Server
        /// </summary>
        /// <param name="filePath">Path</param>
        /// <param name="format">File format, can be "nttriples","rdf/xml"</param>
        /// <param name="baseUrl">Base URL</param>
        /// <param name="context">Context, default set to null</param>
        /// <param name="serverSide">Whether this request is on server side</param>
        public void LoadFile(string filePath, string format, string baseUrl = null, string context = null, bool serverSide = false)
        {
            string contentType = string.Empty;
            if (format == "ntriples")
            {
                contentType = "text/plain";
            }
            else if (format == "rdf/xml")
            {
                contentType = "application/rdf+xml";
            }
            string fileContent = string.Empty;
            if (!serverSide)
            {
                StreamReader sr = new StreamReader(filePath);
                fileContent = sr.ReadToEnd();
                sr.Close();
                filePath = null;
            }
            string vars =
                $"file={HttpUtility.UrlEncode(filePath)}&context={HttpUtility.UrlEncode(context)}&baseUrl={HttpUtility.UrlEncode(baseUrl)}";
            string relativeUrl = "/statements?" + vars;
            AgRequestService.DoReq(this, "POST", relativeUrl, contentType, fileContent);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        ///Type mapping
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Fetches a result set of currently specified mappings. 
        /// </summary>
        /// <returns>List<DataType></returns>
        public List<DataType> ListTypeMapping()
        {
            return AgRequestService.DoReqAndGet<List<DataType>>(this, "GET", "/mapping");
        }

        /// <summary>
        /// Clear type mappings for this repository. 
        /// </summary>
        /// <param name="isAll">
        ///      if true Clear all type mappings for this repository including the automatic ones.
        ///      else Clear all non-automatic type mappings for this repository. 
        /// </param>
        public void ClearTypeMapping(bool isAll = false)
        {
            if (isAll)
            {
                AgRequestService.DoReq(this, "DELETE", "/mapping/all");
            }
            else
            {
                AgRequestService.DoReq(this, "DELETE", "/mapping");
            }
        }

        /// <summary>
        /// Yields a list of literal types for which datatype mappings have been defined in this store.
        /// </summary>
        /// <returns>The set of type</returns>
        public List<string> ListMappedTypes()
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/mapping/type");
        }

        /// <summary>
        /// Takes two arguments, type (the RDF literal type) and encoding, 
        /// and defines a datatype mapping from the first to the second
        /// </summary>
        /// <param name="type">the RDF literal type</param>
        /// <param name="encoding">Encoding</param>
        public void AddMappedType(string type, string encoding)
        {
            AgRequestService.DoReq(this, "PUT", $"/mapping/type?type={type}&encoding={encoding}");
        }

        /// <summary>
        /// Deletes a datatype mapping
        /// </summary>
        /// <param name="type">type should be an RDF resource</param>
        public void DeleteMappedType(string type)
        {
            AgRequestService.DoReq(this, "DELETE", $"/mapping/type?type={type}");
        }

        /// <summary>
        /// Yields a list of literal types for which predicate mappings have been defined in this store. 
        /// </summary>
        /// <returns></returns>
        public List<string> ListMappedPredicates()
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/mapping/predicate");
        }

        /// <summary>
        /// Takes two arguments, predicate and encoding, and defines a predicate mapping on them. 
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <param name="encoding">encoding</param>
        public void AddMappedPredicate(string predicate, string encoding)
        {
            AgRequestService.DoReq(this, "POST", $"/mapping/predicate?predicate={predicate}&encoding={encoding}");
        }

        /// <summary>
        /// Deletes a predicate mapping. Takes one parameter, predicate. 
        /// </summary>
        /// <param name="predicate">predicate</param>
        public void DeleteMappedPredicate(string predicate)
        {
            AgRequestService.DoReq(this, "DELETE", $"/mapping/predicate?predicate={predicate}");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        ///triple index
        /////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// List all the indices
        /// </summary>
        public List<string> ListIndices()
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/indices");
        }


        /// <summary>
        /// List the valid indices
        /// </summary>
        /// <returns>set of index</returns>
        public List<string> ListValidIndices()
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/indices?listValid=true");
        }

        /// <summary>
        /// Add an index with specific type
        /// </summary>
        /// <param name="indexType">Index type</param>
        public void AddIndex(string indexType)
        {
            AgRequestService.DoReq(this, "PUT", "/indices/" + indexType);
        }

        /// <summary>
        /// Drop an index with type
        /// </summary>
        /// <param name="indexType">index type to drop</param>
        public void DropIndex(string indexType)
        {
            AgRequestService.DoReq(this, "DELETE", "/indices/" + indexType);
        }

        /// <summary>
        /// Tells the server to try and optimize the indices for this store
        /// </summary>
        /// <param name="wait">
        ///  Defaulting to false,Indicates whether the HTTP request should return right away or
        ///  whether it should wait for the operation to complete.
        /// </param>
        /// <param name="level">Level determines how much work should be done. </param>
        public void OptimizeIndex(bool wait = false, string level = null)
        {
            AgRequestService.DoReq(this, "POST", $"/indices/optimize?wait={wait}&level={level}");
        }

        /// <summary>
        /// Open a new session
        /// </summary>
        /// <returns></returns>
        public string OpenSession(string spec, bool autocommit = false, int lifetime = -1, bool loadinitfile = false)
        {
            string relativeUrl;
            if (lifetime == -1)
            {
                relativeUrl = $"/session?autoCommit={autocommit},loadInitFile={loadinitfile},store={spec}";
            }
            else
            {
                relativeUrl =
                    $"/session?autoCommit={autocommit}, lifetime={lifetime},loadInitFile={loadinitfile}, store={spec}";
            }
            return AgRequestService.DoReqAndGet<string>(this, "POST", relativeUrl);
            //return new AgRepository(sessionUrl, this.Username, this.Password);
        }

        /// <summary>
        /// Close the current session
        /// </summary>
        public void CloseSession()
        {
            try
            {
                AgRequestService.DoReq(this, "POST", "/session/close");
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                // ignored
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Prepares a query,Preparing queries is only supported in a dedicated session,
        /// and the prepared queries will only be available in that session. 
        /// </summary>
        /// <param name="pQueryId">prepares query id</param>
        /// <param name="query">Query string</param>
        /// <param name="infer">Infer option, can be "false","rdfs++","restriction"</param>
        /// <param name="context">Context</param>
        /// <param name="namedContext">Named Context</param>
        /// <param name="bindings">Local bindings for variables</param>
        /// <param name="checkVariables">Whether to check the non-existing variable</param>
        /// <param name="limit">The size limit of result</param>
        /// <param name="offset">Skip some of the results at the start</param>
        /// <returns>prepares query and saves query under id.</returns>
        public void PreparingQueries(string pQueryId, string query, string infer = "false", string context = null, string namedContext = null,
           Dictionary<string, string> bindings = null, bool checkVariables = false, int limit = -1, int offset = -1)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"query", query}, {"queryLn", "sparql"}, {"infer", infer}
            };
            if (context != null) parameters.Add("context", context);
            if (namedContext != null) parameters.Add("namedContext", namedContext);

            if (bindings != null)
            {
                foreach (string vari in bindings.Keys)
                    parameters.Add("$" + vari, HttpUtility.UrlEncode(bindings[vari]));
            }
            parameters.Add("checkVariables", checkVariables.ToString());
            if (limit >= 0) parameters.Add("limit", limit.ToString());
            if (offset >= 0) parameters.Add("offset", offset.ToString());
            AgRequestService.DoReq(this, "PUT", "/queries/" + pQueryId, parameters);
        }

        /// <summary>
        ///  Executes a prepared query stored under the name id 
        /// </summary>
        /// <param name="pQueryId">prepared query id</param>
        public string ExecutePreparingQueries(string pQueryId)
        {
            //AgRequestService.DoReq(this, "GET", "/queries/" + PQueryID);
            return AgRequestService.DoReqAndGet(this, "GET", "/queries/" + pQueryId);
        }

        /// <summary>
        /// Executes a prepared query stored under the name id with some parameters
        /// </summary>
        /// <param name="pQueryId">prepared query id</param>
        /// <param name="bindings">Local bindings for variables</param>
        /// <param name="limit">The size limit of result</param>
        /// <param name="offset">Skip some of the results at the start</param>
        public string ExecutePreparingQueries(string pQueryId, Dictionary<string, string> bindings = null, int limit = -1, int offset = -1)
        {
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));
            if (bindings.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(bindings));
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (offset <= 0) throw new ArgumentOutOfRangeException(nameof(offset));
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            return AgRequestService.DoReqAndGet(this, "POST", "/queries/" + pQueryId, parameters);
        }

        /// <summary>
        /// Deletes the prepared query stored under id
        /// </summary>
        /// <param name="pQueryId">prepared query id</param>
        public void DeletePreparingQueries(string pQueryId)
        {
            AgRequestService.DoReq(this, "DELETE", "/queries/" + pQueryId);
        }

        /// <summary>
        /// Define Prolog functors, 
        /// which can be used in Prolog queries. 
        /// This is only allowed when accessing a dedicated session. 
        /// </summary>
        /// <param name="prologFunction">prolog function content</param>
        public void DefinePrologFunction(string prologFunction)
        {
            AgRequestService.DoReq(this, "POST", "/functor", prologFunction);
        }

        /// <summary>
        /// Commit the current session
        /// </summary>
        public void Commit()
        {
            AgRequestService.DoReq(this, "POST", "/commit");
        }

        /// <summary>
        /// Rollback the current session
        /// </summary>
        public void Rollback()
        {
            AgRequestService.DoReq(this, "POST", "/rollback");
        }

        /// <summary>
        ///     Enable the spogi cache in this repository. 
        ///     Takes an optional size argument to set the size of the cache.
        /// </summary>
        /// <param name="size">Triple cache size</param>
        public void EnableTripleCache(int size = -1)
        {
            string queryUrl;
            if (size == -1)
            {
                queryUrl = "/tripleCache";
            }
            else
            {
                queryUrl = $"/tripleCache?size={size}";
            }
            AgRequestService.DoReq(this, "PUT", queryUrl);
        }

        /// <summary>
        /// Disable the spogi cache for this repository. 
        /// </summary>
        public void DisableTripleCache()
        {
            AgRequestService.DoReq(this, "DELETE", "/tripleCache");
        }

        /// <summary>
        /// Find out whether the 'SPOGI cache' is enabled, 
        /// and what size it has. Returns an integer 
        /// 0 when the cache is disabled, the size of the cache otherwise. 
        /// </summary>
        /// <returns>An integer denoting the triple cache size</returns>
        public int GetTripleCacheSize()
        {
            return AgRequestService.DoReqAndGet<int>(this, "GET", "/tripleCache");
        }


        ///////////////////////////////////////////////////////////////////////////////////
        //  FreeText
        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Use free-text indices to search for the given pattern.
        /// </summary>
        /// <param name="pattern">The text to search for</param>
        /// <param name="expression">An S-expression combining search strings using and, or, phrase, match, and fuzzy. </param>
        /// <param name="index">
        ///   An optional parameter that restricts the search to a specific free-text index.
        ///   If not given, all available indexes are used
        /// </param>
        /// <param name="sorted"> 
        ///     indicating whether the results should be sorted by relevance. Default is false. 
        /// </param>
        /// <param name="limit">An integer limiting the amount of results that can be returned.</param>
        /// <param name="offset">An integer telling the server to skip the first few results</param>
        /// <returns>an array of statements</returns>
        public List<List<string>> EvalFreeTextIndex(string pattern, string expression = null, string index = null, bool sorted = false, int limit = -1, int offset = -1)
        {
            StringBuilder sbParameter = new StringBuilder($"?pattern={pattern}");
            if (expression != null)
            {
                sbParameter.Append($"&expression={expression}");
            }
            if (index != null)
            {
                sbParameter.Append($"&index={index}");
            }
            sbParameter.Append($"&sorted={sorted}");
            if (limit != -1)
            {
                sbParameter.Append($"&limit={limit}");
            }
            if (offset != -1)
            {
                sbParameter.Append($"&offset={offset}");
            }
            return AgRequestService.DoReqAndGet<List<List<string>>>(this, "GET", $"/freetext{sbParameter}");
        }

        /// <summary>
        /// Create a free-text index with the given parameters
        /// </summary>
        /// <param name="method">http method</param>
        /// <param name="name">string identifying the new index </param>
        /// <param name="predicates">If no predicates are given, triples are indexed regardless of predicate</param>
        /// <param name="indexLiterals">
        ///     IndexLiterals determines which literals to index.
        ///     It can be True (the default), False, or a list of resources, 
        ///     indicating the literal types that should be indexed
        /// </param>
        /// <param name="indexResources">
        ///     IndexResources determines which resources are indexed. 
        ///     It can be True, False (the default), or "short", 
        ///     to index only the part of resources after the last slash or hash character.
        /// </param>
        /// <param name="indexFields">
        ///     IndexFields can be a list containing any combination of the elements
        ///     "subject", "predicate", "object", and "graph".The default is ["object"]. 
        /// </param>
        /// <param name="minimumWordSize"> 
        ///     Determines the minimum size a word must have to be indexed.
        ///     The default is 3
        /// </param>
        /// <param name="stopWords">
        ///     StopWords should hold a list of words that should not be indexed. 
        ///     When not given, a list of common English words is used. 
        /// </param>
        /// <param name="wordFilters">
        ///     WordFilters can be used to apply some normalizing filters to words as they are indexed or queried.
        ///     Can be a list of filter names.  Currently, only "drop-accents" and "stem.english" are supported. 
        /// </param>
        /// <param name="innerChars">The character set to be used as the constituent characters of a word</param>
        /// <param name="borderChars"> The character set to be used as the border characters of indexed words. </param>
        /// <param name="tokenizer">An optional string. Can be either default or japanese.</param>
        public void ManipulateFreeTextIndex(string method, string name, List<string> predicates = null,
                                            object indexLiterals = null,
                                            string indexResources = "true", List<string> indexFields = null,
                                            int minimumWordSize = 3, List<string> stopWords = null,
                                            List<string> wordFilters = null, char[] innerChars = null,
                                            char[] borderChars = null, string tokenizer = null)
        {
            StringBuilder paramsBuilder = new StringBuilder();

            void AddParam(string paramName, string paramValue)
            {
                if (paramsBuilder.Length > 0)
                    paramsBuilder.Append($"&{paramName}={paramValue}");
                else
                    paramsBuilder.Append($"{paramName}={paramValue}");
            }

            if (predicates != null && predicates.Count > 0)
            {
                for (int i = 0; i < predicates.Count; i++)
                {
                    AddParam("predicate", predicates[i]);
                }
            }
            else AddParam("predicate", "");

            if (indexLiterals != null)
            {
                if (indexLiterals is string) AddParam(nameof(indexLiterals), indexLiterals.ToString());
                else if (indexLiterals is List<string>)
                {
                    // ReSharper disable once InconsistentNaming
                    List<string> index_Literals = indexLiterals as List<string>;
                    for (int i = 0; i < index_Literals.Count; i++)
                        AddParam("indexLiteralType", index_Literals[i]); //if indexLiteralType is an array of literal types to index,
                }
            }
            else AddParam(nameof(indexLiterals), "");

            if (indexResources != null) AddParam(nameof(indexResources), indexResources);
            else AddParam(nameof(indexResources), "");

            if (indexFields != null && indexFields.Count > 0)
            {
                for (int i = 0; i < indexFields.Count; i++)
                    AddParam("indexField", indexFields[i]);
            }
            else AddParam("indexField", "");

            if (minimumWordSize != -1)
            {
                AddParam(nameof(minimumWordSize), minimumWordSize.ToString());
            }

            if (stopWords != null && stopWords.Count > 0)
            {
                for (int i = 0; i < stopWords.Count; i++)
                {
                    AddParam("stopword", stopWords[i]);
                }
            }
            else AddParam("stopword", "");

            if (wordFilters != null && wordFilters.Count > 0)
            {
                for (int i = 0; i < wordFilters.Count; i++)
                    AddParam("wordFilter", wordFilters[i]);
            }
            else AddParam("wordFilter", "");

            if (innerChars != null && innerChars.Length > 0)
            {
                for (int i = 0; i < innerChars.Length; i++)
                {
                    AddParam(nameof(innerChars), innerChars[i].ToString());
                }
            }

            if (borderChars != null && borderChars.Length > 0)
            {
                for (int i = 0; i < borderChars.Length; i++)
                {
                    AddParam(nameof(borderChars), borderChars[i].ToString());
                }
            }
            if (tokenizer != null) AddParam(nameof(tokenizer), tokenizer);
            else AddParam(nameof(tokenizer), "");
            //Console.WriteLine(paramsBuilder.ToString());
            AgRequestService.DoReq(this, method, "/freetext/indices/" + name, paramsBuilder.ToString());
        }

        /// <summary>
        /// Crate a free text index
        /// </summary>
        /// <seealso cref="ManipulateFreeTextIndex"/>
        public void CreateFreeTextIndex(string name, List<string> predicates = null, object indexLiterals = null,
                                        string indexResources = "true", List<string> indexFields = null,
                                        int minimumWordSize = -1, List<string> stopWords = null,
                                        List<string> wordFilters = null, char[] innerChars = null,
                                        char[] borderChars = null, string tokenizer = null)
        {
            ManipulateFreeTextIndex("PUT", name, predicates, indexLiterals, indexResources, indexFields, minimumWordSize, stopWords, wordFilters, innerChars, borderChars, tokenizer);
        }

        /// <summary>
        /// Modify a free text index
        /// </summary>
        /// <seealso cref="ManipulateFreeTextIndex"/>
        public void ModifyFreeTextIndex(string name, List<string> predicates = null, object indexLiterals = null,
                                        string indexResources = "true", List<string> indexFields = null,
                                        int minimumWordSize = -1, List<string> stopWords = null,
                                        List<string> wordFilters = null, char[] innerChars = null,
                                        char[] borderChars = null, string tokenizer = null)
        {
            ManipulateFreeTextIndex("POST", name, predicates, indexLiterals, indexResources, indexFields, minimumWordSize, stopWords, wordFilters, innerChars, borderChars, tokenizer);
        }

        /// <summary>
        /// Returns a list of names of free-text indices defined in this repository. 
        /// </summary>
        /// <returns></returns>
        public List<string> ListFreeTextIndices()
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/freetext/indices");
        }

        /// <summary>
        /// Delete the named free-text index
        /// </summary>
        /// <param name="indexName"></param>
        public void DeleteFreeTextIndex(string name)
        {
            AgRequestService.DoReq(this, "DELETE", "/freetext/indices/" + name);
        }

        /// <summary>
        /// List all the free text predicates
        /// </summary>
        /// <returns>The URIs of the free text predicates</returns>
        //public List<string> ListFreeTextPredicates()
        //{
        //    return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/freetext/predicates");
        //}

        /// <summary>
        /// Register a new free text predicate
        /// </summary>
        /// <param name="predicate">the URI of predicate</param>
        //public void RegisterFreeTextPredicate(string predicate)
        //{
        //    AgRequestService.DoReq(this, "POST", "/freetext/predicates", "predicate=" + predicate);
        //}

        /// <summary>
        /// Returns the configuration parameters of the named free-text index
        /// </summary>
        /// <param name="indexName">Free text index name</param>
        /// <returns></returns>
        public FreeTextIndex GetFreeTextIndexConfiguration(string indexName)
        {
            return AgRequestService.DoReqAndGet<FreeTextIndex>(this, "GET", "/freetext/indices/" + indexName);
        }

        /// <summary>
        /// Returns the configuration parameter of the named free-text index
        /// </summary>
        /// <param name="indexName">Free text index name</param>
        /// <param name="paramName">parameter name</param>
        /// <returns></returns>
        public string GetFreeTextIndexConfiguration(string indexName, string paramName)
        {
            return AgRequestService.DoReqAndGet(this, "GET", $"/freetext/indices/{indexName}/{paramName}");
        }

        /// <summary>
        /// Evaluate a free text search
        /// </summary>
        /// <param name="pattern">The search pattern</param>
        /// <param name="infer">Whether to perform infer</param>
        /// <param name="limit">The size limit of result</param>
        /// <param name="indexs">The indices involved</param>
        /// <returns></returns>

        //public List<string> EvalFreeTextSearch(string pattern, bool infer = false, int limit = -1, List<string> indexs = null)
        //{
        //    string urlParam = "";
        //    if (indexs == null)
        //    {
        //        urlParam = string.Format("/freetext/pattern={0}&infer={1}&limit={2}&index={3}", pattern, infer, limit, "");
        //    }
        //    else
        //    {
        //        if (indexs.Length > 0)
        //        {
        //            StringBuilder indexParam = new StringBuilder(string.Format("index={0}", indexs[0]));
        //            for (int i = 1; i < indexs.Length; i++)
        //            {
        //                indexParam.Append(string.Format("&index={0}", indexs[i]));
        //            }
        //            urlParam = string.Format("/freetext/pattern={0}&infer={1}&limit={2}&{3}", pattern, infer, limit, indexParam.ToString());
        //        }
        //    }
        //    //return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/freetext/indices/" + urlParam);
        //    return AgRequestService.DoReqAndGet<List<string>>(this, "GET", urlParam);
        //}

        /// <summary>
        /// Define a prolog functor
        /// </summary>
        public void DefinePrologFunctors(string rules)
        {
            AgRequestService.DoReq(this, "POST", "/functor", rules);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //Geo-spatial               
        //////////////////////////////////////////////////////////////////////////////////////////////////// 

        /// <summary>
        /// List the geo-spatial types registered in the store.
        /// </summary>
        /// <returns></returns>
        public List<string> ListGeoTypes()
        {
            return AgRequestService.DoReqAndGet<List<string>>(this, "GET", "/geo/types");
        }

        /// <summary>
        /// Define a new Cartesian geospatial type. Returns the type resource
        /// </summary>
        /// <param name="stripWidth">
        /// A floating-point number that determines the granularity of the type
        /// </param>
        /// <param name="xmin">
        /// Floating-point numbers that determine the min x size of the Cartesian plane that is modelled by this type
        /// </param>
        /// <param name="xmax">
        /// Floating-point numbers that determine the man x size of the Cartesian plane that is modelled by this type
        /// </param>
        /// <param name="ymin">
        /// Floating-point numbers that determine the min y size of the Cartesian plane that is modelled by this type
        /// </param>
        /// <param name="ymax">
        /// Floating-point numbers that determine the max y size of the Cartesian plane that is modelled by this type
        /// </param>
        public string SetCartesianGeoType(float stripWidth, float xmin = -1, float xmax = -1, float ymin = -1, float ymax = -1)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append($"stripWidth={stripWidth}");
            parameters.Append($"&xmin={xmin}");
            parameters.Append($"&xmax={xmax}");
            parameters.Append($"&ymin={ymin}");
            parameters.Append($"&ymax={ymax}");
            return AgRequestService.DoReqAndGet(this, "POST", "/geo/types/cartesian?" + parameters);
        }

        /// <summary>
        /// Add a spherical geospatial type. Returns the type resource.
        /// </summary>
        /// <param name="stripWidth">
        /// A floating-point number that determines the granularity of the type
        /// </param>
        /// <param name="unit">
        ///  Can be degree, radian, km, or mile. Determines the unit in which the stripWidth argument is given
        /// </param>
        /// <param name="latmin">
        /// Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere. 
        /// </param>
        /// <param name="latmax">
        ///  Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere. 
        /// </param>
        /// <param name="longmin">
        /// Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere.  
        /// </param>
        /// <param name="longmax">
        /// Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere.  
        /// </param>
        /// <returns></returns>
        public string SetSphericalGeoType(float stripWidth, string unit = "degree", float latmin = 361, float latmax = 361, float longmin = 361, float longmax = 361)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append($"stripWidth={stripWidth}");
            parameters.Append($"&unit={unit}");
            if (latmin < 360) parameters.Append($"&latmin={latmin}");
            if (longmin < 360) parameters.Append($"&longmin={longmin}");
            if (latmax < 360) parameters.Append($"&latmax={latmax}");
            if (longmax < 360) parameters.Append($"&longmax={longmax}");
            return AgRequestService.DoReqAndGet(this, "POST", $"/geo/types/spherical?{parameters}");
        }

        /// <summary>
        /// Fetch all triples with a given predicate whose object is a geospatial value inside the given box. 
        /// </summary>
        /// <param name="type">The geospatial type of the object field.</param>
        /// <param name="predicate">The predicate to look for</param>
        /// <param name="xMin">The bounding box</param>
        /// <param name="xMax">The bounding box</param>
        /// <param name="yMin">The bounding box</param>
        /// <param name="yMax">The bounding box</param>
        /// <param name="limit">Optional. Used to limit the amount of returned triples</param>
        /// <param name="offset">Optional. Used to skip a number of returned triples.</param>
        /// <returns></returns>
        public List<Statement> GetStatementsInsideBox(string type, string predicate,
                                           float xMin, float xMax, float yMin, float yMax,
                                           float limit = -1, float offset = -1)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append($"type={type}");
            parameters.Append($"&predicate={predicate}");
            parameters.Append($"&xMin={xMin}");
            parameters.Append($"&xMax={xMax}");
            parameters.Append($"&yMin={yMin}");
            parameters.Append($"&yMax={yMax}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (limit != -1) parameters.Append($"&limit={limit}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (offset != -1) parameters.Append($"&offset={offset}");

            return AgRequestService.DoReqAndGet<List<Statement>>(this, "GET", $"/geo/box?{parameters}");
        }

        /// <summary>
        /// Retrieve triples within a circle.parameters reference GetStatementsInsideBox(...)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="predicate"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public List<Statement> GetStatementsInsideCircle(string type, string predicate,
                                                         float x, float y, float radius,
                                                         float limit = -1, float offset = -1)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append($"type={type}");
            parameters.Append($"&predicate={predicate}");
            parameters.Append($"&x={x}");
            parameters.Append($"&y={y}");
            parameters.Append($"&radius={radius}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (limit != -1) parameters.Append($"&limit={limit}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (offset != -1) parameters.Append($"&offset={offset}");

            return AgRequestService.DoReqAndGet<List<Statement>>(this, "GET", $"/geo/circle?{parameters}");
        }
        /// <summary>
        /// Get all the triples with a given predicate whose object lies within radius units 
        /// from the given latitude/longitude.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="predicate"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <param name="unit"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public List<Statement> GetStatementsHaversine(string type, string predicate,
                                                         float latitude, float longitude, float radius,
                                                         string unit = "km", float limit = -1, float offset = -1)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append($"type={type}");
            parameters.Append($"&predicate={predicate}");
            parameters.Append($"&lat={latitude}");
            parameters.Append($"&long={longitude}");
            parameters.Append($"&unit={unit}");
            parameters.Append($"&radius={radius}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (limit != -1) parameters.Append($"&limit={limit}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (offset != -1) parameters.Append($"&offset={offset}");
            return AgRequestService.DoReqAndGet<List<Statement>>(this, "GET", "/geo/haversine", parameters.ToString());
        }

        public List<Statement> GetStatementsInsidePolygon(string type, string predicate, object polygon,
                                                          float limit = -1, float offset = -1)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append($"type={type}");
            parameters.Append($"&predicate={predicate}");
            parameters.Append($"&polygon={polygon}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (limit != -1) parameters.Append($"&limit={limit}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (offset != -1) parameters.Append($"&offset={offset}");

            return AgRequestService.DoReqAndGet<List<Statement>>(this, "GET", "/geo/polygon", parameters.ToString());
        }


        /////////////////////////////////////////////////////////////////////////////////////////////
        // SNA   Social Network Analysis Methods
        /////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// subjectOf, objectOf, and undirected can be either a single predicate or a list of predicates.
        /// query should be a prolog query in the form (select ?x (q- ?node !<mypredicate> ?x)),
        /// where ?node always returns to the argument passed to the generator.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subjectOf"></param>
        /// <param name="objectOf"></param>
        /// <param name="undirected"></param>
        /// <param name="query"></param>
        public void RegisterSnaGenerator(string name, List<string> subjectOf = null, List<string> objectOf = null, List<string> undirected = null, string query = null)
        {
            StringBuilder parameters = new StringBuilder($"/snaGenerators/{name}?");
            if (subjectOf != null)
            {
                foreach (string pred in subjectOf) parameters = AddParams(nameof(subjectOf), pred, parameters);
            }
            if (objectOf != null)
            {
                foreach (string pred in objectOf) parameters = AddParams(nameof(objectOf), pred, parameters);
            }
            if (undirected != null)
            {
                foreach (string pred in undirected) parameters = AddParams(nameof(undirected), pred, parameters);
            }
            if (query != null) parameters = AddParams(nameof(query), query, parameters);
            AgRequestService.DoReq(this, "PUT", parameters.ToString());
        }

        /// <summary>
        ///     Create a neighbor-matrix, which is a pre-computed generator
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group">
        ///     A set of N-Triples terms (can be passed multiple times)
        ///     which serve as start nodes for building up the matrix. 
        ///</param>
        /// <param name="generator">The generator to use, by name</param>
        /// <param name="depth">
        /// An integer specifying the maximum depth to which to compute the matrix. Defaults to 1
        /// </param>
        public void RegisterNeighborMatrix(string name, List<string> group, string generator, int depth = 1)
        {
            StringBuilder parameters = new StringBuilder($"/neighborMatrices/{name}?");
            if (group != null)
            {
                foreach (string s in group) parameters = AddParams(nameof(group), s, parameters);
            }
            if (generator != null) parameters = AddParams(nameof(generator), generator, parameters);
            AddParams(nameof(depth), depth.ToString(), parameters);
            AgRequestService.DoReq(this, "PUT", parameters.ToString());
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Tools
        //////////////////////////////////////////////////////////////////////////////////////////////////
        public static StringBuilder AddParams(string paramName, string paramValue, StringBuilder parameters)
        {
            if (parameters.Length > 0 && parameters.ToString().Contains("&"))
                parameters.Append($"&{paramName}={paramValue}");
            else
                parameters.Append($"{paramName}={paramValue}");
            return parameters;
        }
    }
}
