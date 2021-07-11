using System.Collections.Generic;

namespace AllegroGraphNetCoreClient.OpenRDF.Model
{
    public class FreeTextIndex
    {

        /// <summary>
        ///An array of strings. Empty if the index indexes all predicates, containing only the predicates that are indexed otherwise. 
        /// </summary>
        public List<string> Predicates { get; set; }

        /// <summary>
        /// Can be true (index all literals), false (no literals), or an array of literal types to index. 
        /// </summary>
        public string IndexLiterals { get; set; }

        /// <summary>
        ///Can be true (index resources fully), false (don't index resources), or the string "short" to index only the part after the last # or / in the resource. 
        /// </summary>
        public string IndexResources { get; set; }

        /// <summary>
        ///An array containing any of the strings "subject", "predicate", "object", and "graph". This indicates which fields of a triple are indexed. 
        /// </summary>
        public List<string> IndexFields { get; set; }

        /// <summary>
        ///An integer, indicating the minimum size a word must have to be indexed. 
        /// </summary>
        public int MinimumWordSize { get; set; }

        /// <summary>
        ///A list of words, indicating the words that count as stop-words, and should not be indexed. 
        /// </summary>
        public List<string> StopWords { get; set; }

        /// <summary>
        /// A list of word filters configured for this index (see below).
        /// </summary>
        public List<string> WordFilters { get; set; }

        /// <summary>
        ///A  list of character specifiers configured for this index (see below).
        /// </summary>
        public List<string> InnerChars { get; set; }

        /// <summary>
        /// A list of character specifiers configured for this index.
        /// </summary>
        public List<string> BorderChars { get; set; }

        /// <summary>
        /// The name of the tokenizer being used (currently either default or japanese). 
        /// </summary>
        public string Tokenizer { get; set; }
    }
}
