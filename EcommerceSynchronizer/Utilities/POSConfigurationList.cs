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
    }
}