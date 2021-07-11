using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllegroGraphCaller
{
    
    public class Program
    {

        public static List<AllegroGraphRegistryEntry> AllegroGraphRegistry;
        public static void Main(string[] args)
        {
            AllegroGraphRegistry = new List<AllegroGraphRegistryEntry>(); 
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class AllegroGraphRegistryEntry
    {
        public string ID { get; }
        public string Name { get; set; }
        public string Password { get; set; }

        public string Url { get; set; }

        public int Port { get; set; }

        public string Catalog { get; set; }
        public string Repository { get; set; }

        public AllegroGraphRegistryEntry(string name, string password, string url, string port, string catalog, string repository)
        {
            ID = Convert.ToString(Guid.NewGuid());
            Name = name;
            Password = password;
            Url = url;
            Port = Convert.ToInt32(port);
            Catalog = catalog;
            Repository = repository;
        }


    }


}
