using AllegroGraphNetCoreClient.OpenRDF.RepositoryUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllegroGraphNetCoreClient.OpenRDF.Query
{
    public abstract class AbstractQuery
    {
        public string Querylanguage { get; set; }
        public string QueryString { get; set; }
        //public string BaseURI { get; set; }
        public string Contexts { get; set; }
        public string NamedContexts { get; set; }
        public bool IncludeInferred { get; set; }
        public Dictionary<string, string> Bindings { get; set; }
        public RepositoryConnection Connection { get; set; }
        public bool CheckVariables { get; set; }


        /// <summary>
        /// Binds the named key to the supplied value. 
        /// Any value that was previously bound to the specified attribute will be overwritten. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBindings(string key, string value)
        {
            if (Bindings == null)
            {
                Bindings = new Dictionary<string, string>();
            }
            Bindings[key] = value;
        }

        /// <summary>
        /// Sets multiple bindings using a dictionary of attribute keys and values.
        /// </summary>
        /// <param name="dictionary"></param>
        public void SetBindings(Dictionary<string, string> dictionary)
        {
            if (Bindings == null)
            {
                Bindings = new Dictionary<string, string>();
            }
            foreach (string key in dictionary.Keys)
            {
                Bindings[key] = dictionary[key];
            }
        }

        /// <summary>
        /// Removes the named binding so that it has no value.
        /// </summary>
        /// <param name="key">binding key</param>
        public void RemoveBinding(string key)
        {
            if (Bindings != null && Bindings.ContainsKey(key))
            {
                Bindings.Remove(key);
            }
        }

        /// <summary>
        /// Assert a set of contexts (named graphs) that filter all triples.        
        /// </summary>
        /// <param name="contexts"></param>
        public void SetContext(string contexts)
        {
            this.Contexts = contexts;
        }

        /// <summary>
        ///     Determine whether evaluation results of this query should include inferred statements 
        ///     (if any inferred statements are present in the repository). 
        ///     The default setting is 'true'.
        /// </summary>
        /// <param name="includeInferred"></param>
        public void SetIncludeInferred(bool includeInferred)
        {
            this.IncludeInferred = includeInferred;
        }

        /// <summary>
        /// If true, the presence of variables in the select clause not referenced in a triple are flagged.
        /// </summary>
        /// <param name="setting"></param>
        public void SetCheckVariables(bool setting)
        {
            this.CheckVariables = setting;
        }

        /// <summary>
        ///  Evaluate a SPARQL or PROLOG query.
        /// </summary>
        /// <param name="infer">Infer option, can be "false","rdfs++","restriction"</param>
        /// <param name="limit">The size limit of result<</param>
        /// <param name="offset">Skip some of the results at the start</param>
        /// <returns></returns>
        public string Evaluate_generic_query(string infer = "false", int limit = -1, int offset = -1)
        {
            RepositoryConnection conn = this.Connection;
            string queryResult;
            if (this.Querylanguage == QueryLanguage.SPARQL)
            {
                queryResult = conn.GetMiniRepository().EvalSparqlQuery(this.QueryString,
                                                                       infer,
                                                                       this.Contexts,
                                                                       this.NamedContexts,
                                                                       this.Bindings,
                                                                       this.CheckVariables,
                                                                       limit,
                                                                       offset);
            }
            else
            {
                queryResult = conn.GetMiniRepository().EvalPrologQuery(this.QueryString, infer, limit);
            }
            return queryResult;
        }
    }

    public class BooleanQuery : AbstractQuery
    {
        public bool Evaluate(string infer, int limit, int offset)
        {
            bool result = false;
            try
            {
                bool.Parse(this.Evaluate_generic_query(infer, limit, offset));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return result;
        }
    }

    public class StringArrayQuery : AbstractQuery
    {
        public List<List<string>> Evaluate(string infer, int limit, int offset)
        {
            List<List<string>> result = null;
            try
            {
                result = Connection.GetMiniRepository().QueryResultToArray(Evaluate_generic_query(infer, limit, offset));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                // ignored
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return result;
        }
    }

    public static class QueryLanguage
    {
        public static string SPARQL { get { return "SPARQL"; } }
        public static string PROLOG { get { return "PROLOG"; } }
    }
}
