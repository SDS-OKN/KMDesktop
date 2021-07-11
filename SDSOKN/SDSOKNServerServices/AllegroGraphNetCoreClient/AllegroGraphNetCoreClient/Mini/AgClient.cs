using System;
using System.Collections.Generic;
using System.Text.Json;
// ReSharper disable InvalidXmlDocComment

namespace AllegroGraphNetCoreClient.Mini
{
    public class AgClient
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
#pragma warning disable IDE0044 // Add readonly modifier
        private AgServerInfo _server;
#pragma warning restore IDE0044 // Add readonly modifier

        public AgClient(AgServerInfo server)
        {
            this._server = server;
        }

        /// <summary>
        /// Get the version of allegrograph server
        /// </summary>
        /// <returns>allegrograph server version</returns>
        public string GetVersion()
        {
            return AgRequestService.DoReqAndGet<string>(_server, "GET", "/version", null, false);
        }

        /// <summary>
        /// Get the date on which the server was built
        /// </summary>
        /// <returns>allegrograph server built date</returns>
        public string GetBuiltDate()
        {
            return AgRequestService.DoReqAndGet<string>(_server, "GET", "/version/date", null, false);
        }

        /// <summary>
        /// Re-read configuration file
        /// </summary>
        public void ReConfigure()
        {
            AgRequestService.DoReq(_server, "POST", "/reconfigure ");
        }

        /// <summary>
        /// Re-open log file,need administrator previlege
        /// </summary>
        public void ReopenLog()
        {
            AgRequestService.DoReq(_server, "POST", "/reopenLog");
        }

        /// <summary>
        /// List all of the Catalog's name
        /// </summary>
        /// <returns></returns>
        public List<string> ListCatalogs()
        {
            string result = AgRequestService.DoReqAndGet(_server, "GET", "/catalogs", null, false);
            JsonDocument arr = JsonDocument.Parse(result);
            List<string> catalogs = new List<string>(); //string[arr.RootElement.EnumerateArray().Count()];
            //for (int i = 0; i < catalogs.Length; ++i)
            //    catalogs[i] = (string)arr[i]["id"];
            foreach (var item in arr.RootElement.EnumerateArray())
            {
                catalogs.Add(item.GetProperty("id").ToString());
            }
            return catalogs;
        }

        /// <summary>
        /// Create catalog,if exist return null else return new catalog,need enable dynamic catalogs for the server.
        /// </summary>
        /// <param name="name">catalog name</param>
        /// <returns></returns>
        public AgCatalog CreateCatalog(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (ListCatalogs().Contains(name))
            {
                return null;
            }
            else
            {
                AgRequestService.DoReq(_server, "PUT", "/catalogs/" + name);
                return new AgCatalog(_server, name);
            }
        }

        /// <summary>
        /// Open Catalog
        /// </summary>
        /// <param name="name">Catalog name</param>
        /// <returns>Catalog</returns>
        public AgCatalog OpenCatalog(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new AgCatalog(_server, name);
        }

        /// <summary>
        /// Delete catalog,need enable dynamic catalogs for the server. 
        /// </summary>
        /// <param name="name">catalog name</param>
        public void DeleteCatalog(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            AgRequestService.DoReq(_server, "DELETE", "/Catalogs/" + name);
        }

        /// <summary>
        /// Open a session on a federated, reasoning, or filtered store.
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="autoCommit"></param>
        /// <param name="lifetime"></param>
        /// <param name="loadInitFile"></param>
        /// <returns>AgRepository</returns>        
        public AgRepository OpenSession(string spec, bool autoCommit = false, int lifetime = -1, bool loadInitFile = false)
        {
            string param;
            if (lifetime == -1)
            {
                param = string.Format("/session?autoCommit={0}&loadInitFile={1}&store={2}", autoCommit, loadInitFile, spec);
            }
            else
            {
                param = string.Format("/session?autoCommit={0}&loadInitFile={1}&store={2}&lifetime={3}", autoCommit, loadInitFile, spec, lifetime);
            }
            string sessionUrl = AgRequestService.DoReqAndGet(this._server, "POST", param);
            return new AgRepository(sessionUrl, _server.Username, _server.Password);
        }

        /// <summary>
        /// Get the initialization file contents
        /// </summary>
        /// <param name="Name">Catalog name</param>
        /// <returns>return opened Catalog</returns>
        public string GetInitFile()
        {
            return AgRequestService.DoReqAndGet(_server, "GET", "/initfile", null, false);
        }

        /// <summary>
        ///  Replace the current initialization file contents with the
        /// 'content' string or remove if null. 
        /// </summary>
        /// <param name="content">init file content</param>
        /// <param name="restart">
        ///     defaults to true, specifies whether any running shared back-ends should
        ///     be shut down, so that subsequent requests will be handled by
        ///     back-ends that include the new code.
        /// </param>
        public void SetInitFile(string content = null, bool restart = true)
        {
            if (string.IsNullOrEmpty(content))
            {
                DeleteInitFile();
            }
            else
            {
                AgRequestService.DoReq(_server, "PUT", string.Format("/initfile?restart={0}", restart), content, true);
            }
        }

        public void DeleteInitFile()
        {
            AgRequestService.DoReq(_server, "DELETE", "/initfile");
        }
    }
}
