using System.Collections.Generic;
// ReSharper disable NotAccessedField.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable RedundantArgumentDefaultValue

namespace AllegroGraphNetCoreClient.OpenRDF.Rio
{
    public class RdfFormat
    {
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0044 // Remove unread private members

#pragma warning restore IDE0044 // Remove unread private members
#pragma warning restore IDE0052 // Remove unread private members
        public string Name { get; }
        public List<string> MimeTypes { get; }
        public string CharSet { get; }
        public List<string> FileExtensions { get; }
        public bool SupportsNamespaces { get; }
        public bool SupportsContexts { get; }

        public RdfFormat(string formatName, List<string> mimeTypes, string charset = "UTF-8", List<string> fileExtensions = null,
                         bool supportsNamespaces = false, bool supportsContexts = false)
        {
            this.Name = formatName;
            this.MimeTypes = mimeTypes;
            this.CharSet = charset;
            this.FileExtensions = fileExtensions;
            this.SupportsNamespaces = supportsNamespaces;
            this.SupportsContexts = supportsContexts;        }
        //The RDF/XML file format.
        public static readonly RdfFormat Rdfxml = new RdfFormat("RDF/XML",
                                                        new List<string> { "application/rdf+xml", "application/xml" },
                                                        "UTF-ASCII",
                                                        new List<string> { "rdf", "rdfs", "owl", "xml" },
                                                        true,
                                                        false);
        //The N-Triples file format.
        public static readonly RdfFormat Ntriples = new RdfFormat("NTRIPLES",
                                                          new List<string> { "text/plain" },
                                                          "UTF-8",
                                                          new List<string> { "nt" },
                                                          false,
                                                          false);
    }
}
