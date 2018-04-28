using System.Collections;
using System.Collections.Generic;
using EcommerceSynchronizer.Model.POSInterfaces;

namespace EcommerceSynchronizer.Utilities
{
    public class POSConfigurationList
    {
        public List<PosConfiguration> PosConfigurations { get; set; }
    }

    public class PosConfiguration
    {
        public string AccessToken { get; set; }
        public EnumPOSTypes Type { get; set; }

        //SquarePOS
        public List<string> Locations { get; set; }

        //Hiboutik
        public string EmailAddress { get; set; }
        public string AccountName { get; set; }
        public int MaximumRequests { get; set; }
        public int StoreID { get; set; }

        //Lightspeed
        public string RefreshToken { get; set; }
        public string AccountID { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public int EmployeeID { get; set; }
        public int RegisterID { get; set; }
        public int ShopID { get; set; }
    }
}