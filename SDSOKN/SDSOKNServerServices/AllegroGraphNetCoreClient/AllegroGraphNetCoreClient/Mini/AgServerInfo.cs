namespace AllegroGraphNetCoreClient.Mini
{
    public class AgServerInfo : IAgUrl
    {
        public AgServerInfo(string baseUrl, string username, string password)
        {
            this.Url = baseUrl;
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; private set; }
    }
}
