using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace AllegroGraphNetCoreClient.Mini
{
    public class AgCatalog : IAgUrl
    {
#pragma warning disable IDE0044 // Add readonly modifier
        private string _catalogUrl;
        private AgServerInfo _server;
        private string _name;
#pragma warning restore IDE0044 // Add readonly modifier
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server">Server Object</param>
        /// <param name="name">Catalog name, returns the root catalog if name is null</param>
        /// <returns></returns>
        public AgCatalog(AgServerInfo server, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _catalogUrl = server.Url + "/";
            }
            else
            {
                _catalogUrl = server.Url + "/catalogs/" + name;
            }
            this._name = name;
            this._server = server;
        }

        public string Url { get { return _catalogUrl; } }
        public string Username { get { return _server.Username; } }
        public string Password { get { return _server.Password; } }

        /// <summary>
        /// List the repositories in the current catalog
        /// </summary>
        public List<string> ListRepositories()
        {
            string result = AgRequestService.DoReqAndGet(this, "GET", "/repositories", false);
            JsonDocument arr = JsonDocument.Parse(result);
            List<string> repos = new List<string>();//new string[arr.RootElement.EnumerateArray().Count()];
            //for (int i = 0; i < repos.Length; ++i)
            //    repos[i] = (string)arr.RootElement.EnumerateArray().[i]["id"];
            foreach (var item in arr.RootElement.EnumerateArray())
            {
                repos.Add(item.GetString());
            }
            return repos;
        }

        /// <summary>
        /// Delete a repository
        /// </summary>
        /// <param name="name">Repository name</param>
        public void DeleteRepository(string name)
        {
            AgRequestService.DoReq(this, "DELETE", "/repositories/" + name);
        }

        /// <summary>
        /// Create a repository
        /// </summary>
        /// <param name="name">Repository name</param>
        public void CreateRepository(string name)
        {
            if (!ListRepositories().Contains(name))
                // ReSharper disable once RedundantArgumentDefaultValue
                AgRequestService.DoReq(this, "PUT", "/repositories/" + name, null, true);
        }

        /// <summary>
        /// Open a repository
        /// </summary>
        /// <param name="name">Repository name</param>
        /// <returns>The opened repository</returns>
        public AgRepository OpenRepository(string name)
        {
            return new AgRepository(this, name);
        }

        /// <summary>
        /// Get the name of current catalog
        /// </summary>
        /// <returns>catalog name</returns>
        public string GetName()
        {
            return this._name;
        }
    }
}
