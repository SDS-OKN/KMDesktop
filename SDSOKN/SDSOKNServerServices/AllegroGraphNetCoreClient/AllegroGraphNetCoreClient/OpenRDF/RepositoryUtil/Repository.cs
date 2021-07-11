using AllegroGraphNetCoreClient.Mini;
using AllegroGraphNetCoreClient.OpenRDF.Sail;
// ReSharper disable InconsistentNaming

namespace AllegroGraphNetCoreClient.OpenRDF.RepositoryUtil
{
    public enum AccessVerb { RENEW, ACCESS, OPEN, CREATE, REPLACE };
    public class Repository
    {
        private AgRepository _agRepository;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
#pragma warning disable IDE0044 // Add readonly modifier
        private Catalog _catalog;
#pragma warning restore IDE0044 // Add readonly modifier


        // ReSharper disable once ConvertToAutoProperty
#pragma warning disable IDE1006 // Naming Styles
        public Catalog catalog { get { return this._catalog; } }
#pragma warning restore IDE1006 // Naming Styles
        public string OldUrl { get; set; }
        public string Url
        {
            get { return _agRepository.Url; }
            set { _agRepository.Url = value; }
        }

        public Repository(Catalog catalog, string name)
        {
            _catalog = catalog;
            _agRepository = new AgRepository(catalog.AgCatalog, name);
        }


        // ReSharper disable once UnusedParameter.Local
#pragma warning disable IDE0060 // Remove unused parameter
        public Repository(AgRepository agRepository, string name = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _agRepository = agRepository;
        }



        /// <summary>
        /// Get RepositoryConnection
        /// </summary>
        /// <returns>RepositroyConnection object</returns>
        public RepositoryConnection GetConnection()
        {
            return new RepositoryConnection(this);
        }

        /// <summary>
        /// Get AGRepository Object
        /// </summary>
        /// <returns>_agRepository</returns>
        public AgRepository GetMiniRepository()
        {
            return this._agRepository;
        }

        /// <summary>
        ///  Return the name of the database (remote triple store) that this repository is interfacing with.
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
        {
            return _agRepository.DatabaseName;
        }

        /// <summary>
        /// Returns a string composed of the catalog name concatenated with the repository name.
        /// </summary>
        /// <returns></returns>
        public string GetSpec()
        {
            string catName = this.catalog.GetName();
            if (catName == null || catName == "/")
            {
                return Spec.Local(this.GetDatabaseName());
            }
            else
            {
                return Spec.Local(this.GetDatabaseName(), catName);
            }
        }
        /// <summary>
        ///   Initializes this repository. A repository needs to be initialized before
        ///   it can be used.  Return 'this' (so that we can chain this call if we like).
        /// </summary>
        /// <returns></returns>
        public Repository Initialize()
        {
            return this;
        }

        /// <summary>
        ///   Shuts the store down, releasing any resources that it keeps hold of.
        ///   Once shut down, the store can no longer be used.
        /// </summary>
        public void ShutDown()
        {
            this._agRepository = null;
        }
    }
}
