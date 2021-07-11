using System.Collections.Generic;
using System.Text;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers

namespace AllegroGraphNetCoreClient.OpenRDF.Query
{
    public class AgDataSet
    {
#pragma warning disable IDE0044 // Add readonly modifier
        List<string> _defaultGraphs;
        List<string> _namedGraphs;
#pragma warning restore IDE0044 // Add readonly modifier
        public AgDataSet(List<string> contexts = null)
        {
            _defaultGraphs = new List<string>();
            _namedGraphs = new List<string>();
            if (contexts != null && contexts.Count > 0)
            {
                _namedGraphs.AddRange(contexts);
            }
        }

        public List<string> DefaultGraphs { get { return this._defaultGraphs; } }
        public List<string> NamedGraphs { get { return this._namedGraphs; } }

        public void AddDefaultGraph(string uri)
        {
            _defaultGraphs.Add(uri);
        }

        public void RemoveDefaultGraph(string uri)
        {
            _defaultGraphs.Remove(uri);
        }

        public void AddNamedGraph(string uri)
        {
            _namedGraphs.Add(uri);
        }

        public void RemoveNamedGraph(string uri)
        {
            _namedGraphs.Remove(uri);
        }

        public void Clear()
        {
            _namedGraphs.Clear();
            _defaultGraphs.Clear();
        }

        public string AsQuery(bool excludeNullContext)
        {
            if (_defaultGraphs.Count == 0 && _namedGraphs.Count == 0)
            {
                if (excludeNullContext)
                    return "";
                else
                    return "## empty dataset ##";
            }
            StringBuilder sb = new StringBuilder();
            foreach (string uri in _defaultGraphs)
            {
                if (uri == null && excludeNullContext) continue;//null context should not appear here
                sb.Append("FROM ");
                sb.Append(uri);
                sb.Append(" ");
            }
            foreach (string uri in _namedGraphs)
            {
                if (uri == null && excludeNullContext) continue; //null context should not appear here
                sb.Append("FROM NAMED ");
                sb.Append(uri);
                sb.Append(" ");
            }
            return sb.ToString();
        }

    }
}
