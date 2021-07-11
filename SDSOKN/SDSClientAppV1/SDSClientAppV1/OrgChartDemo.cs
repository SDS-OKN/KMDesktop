using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDSClientAppV1
{
    public class OrgChartItem
    {
        public int ID { get; set; }

        public int ParentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Email { get; set;  }
    }

    public class OrgChart
    {
        public List<OrgChartItem> Data { get; set; }
        public OrgChart()
        {
            Data = new List<OrgChartItem>();
            Data.Add(new OrgChartItem() { ID = 0, Name = "Person:Person", Email = "Person:Email", Description = "This Is A person Object" });
            Data.Add(new OrgChartItem() { ID = 1, ParentID = 0, Name = "Jordan Henk", Email = "jordan_henk@redlands.edu", Description = "memberOf: Organization:RI" });
            Data.Add(new OrgChartItem() { ID = 2, ParentID = 0, Name = "Catherine Darst", Email = "cat_darst@fws.gov", Description = "memberOf: Organization:USFishAndWildlifeService" });
        }
    }


    public class Connection
    {
        public int To {get;set;}
        public int From { get; set; }
    }
}
