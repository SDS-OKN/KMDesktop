using System.ComponentModel;

namespace AllegroGraphNetCoreClient.OpenRDF.Model
{
    public class URI
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
#pragma warning disable IDE0044 // Add readonly modifier
        private string _uri;
#pragma warning restore IDE0044 // Add readonly modifier
        public string Uri => _uri;

        public URI(string uri,string nameSpace=null,string localName=null)
        {
            var startIndex = 1; 
            if (!string.IsNullOrEmpty(uri))
            {
                if (uri[0] == '<' && uri[^1] == '>')
                {
                    if (uri.Length > 1) 
                        _uri = uri.Substring(startIndex,uri.Length-2);
                }
            }
            else if (!string.IsNullOrEmpty(nameSpace) && !string.IsNullOrEmpty(localName))
            {
                _uri = nameSpace + localName;
            }
        }
        public string ToNTriples()
        {
            return string.Format(@"<{0}>",Uri);
        }
        public override string ToString()
        {
            return _uri;
        }
    }

    public class BNode
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once ArrangeTypeMemberModifiers
        public string Id { get; }

        public string ToNTriples()
        {
            return string.Format("_:{0}", Id);
        }
        public BNode(string id)
        {
            this.Id = id;
        }
    }


   public class Namespace
   {
       string _prefix;
       string _name;
       public string Prefix{
           get { return _prefix; }
           set { this._prefix = value; }
       }
       public string NameSpace
       {
           get { return _name; }
           set { this._name = value; }
       }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="p">Prefix</param>
       /// <param name="n">Namespace</param>
       public Namespace(string p, string n)
       {
           this._name = n;
           this._prefix = p;
       }

       public override string ToString()
       {
           return string.Format("{0} :: {1}",_prefix,_name);
       }
       public bool Equals(Namespace ns)
       {
           if (ns.Prefix == this.Prefix && ns.NameSpace == this.NameSpace)
               return true;
           else
               return false;
       }
   }
}
