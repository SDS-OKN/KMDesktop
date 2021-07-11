using AllegroGraphNetCoreClient.OpenRDF.Query;
using AllegroGraphNetCoreClient.Util;
// ReSharper disable InvalidXmlDocComment


namespace AllegroGraphNetCoreClient.OpenRDF.RepositoryUtil
{
    public partial class RepositoryConnection
    {
        /// <summary>
        /// Add a sequence of one or more rules (in ASCII format).
        /// </summary>
        /// <param name="rules">
        /// rule declarations start with '<-' or '<--'. 
        /// The former appends a new rule; the latter overwrites any rule with the same predicate.
        /// </param>
        /// <param name="language">language defaults to QueryLanguage.PROLOG. </param>
        public void AddRules(string rules, string language = "PROLOG")
        {
            if (language.Equals(QueryLanguage.PROLOG))
                this.GetMiniRepository().DefinePrologFunctors(rules);
            else
                throw new AgRequestException("Cannot add a rule because the rule language has not been set.");
        }

        /// <summary>
        /// Load a file of rules.
        /// </summary>
        /// <param name="fileName">file is assumed to reside on the client machine.</param>
        /// <param name="language">defaults to QueryLanguage.PROLOG.</param>
        public void LoadRules(string fileName, string language = "PROLOG")
        {
#pragma warning disable IDE0063 // Use simple 'using' statement
            using (var sr = new System.IO.StreamReader(fileName))
#pragma warning restore IDE0063 // Use simple 'using' statement
            {
                string rules = sr.ReadToEnd();
                AddRules(rules, language);
                sr.Dispose();
            }
        }
    }
}
