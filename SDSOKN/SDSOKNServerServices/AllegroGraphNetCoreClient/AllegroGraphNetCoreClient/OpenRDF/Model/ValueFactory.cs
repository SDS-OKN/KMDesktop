using System;

namespace AllegroGraphNetCoreClient.OpenRDF.Model
{
    public static class ValueFactory
    {
        public static BNode GetBNode(string nodeId = null)
        {
            if (string.IsNullOrEmpty(nodeId))
                throw new ArgumentException("Value cannot be null or empty.", nameof(nodeId));
            return new BNode(nodeId);
        }

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        ///     Create a new statement with the supplied subject, predicate and object
        //      and associated context. 
        // ReSharper disable once InvalidXmlDocComment
        /// </summary>
        /// <param name="subj">subject</param>
        /// <param name="pred">predicate</param>
        /// <param name="obj">object</param>
        /// <param name="context">context</param>
        /// <returns></returns>
        public static Statement CreateStatement(string subj, string pred, string obj, string context = null)
        {
            return new Statement(subj, pred, obj, context);
        }
        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        ///     Creates a new URI from the supplied string-representation(s).
        //      If two non-keyword arguments are passed, assumes they represent a
        //      namespace/localname pair.
        // ReSharper disable once InvalidXmlDocComment
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="nameSpace"></param>
        /// <param name="localname"></param>
        /// <returns></returns>
        public static URI CreateUri(string uri,string nameSpace,string localname)
        {
            return new URI(uri, nameSpace, localname);  
        }

        public static string CreateGeoLiteral(string literal, string literalType)
        {
            return string.Format("\"{0}\"^^{1}",literal,literalType);
        }
    }
}
