using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.SquarePOSBindingModel
{
    public class ItemInventory
    {
        [JsonProperty("variation_id")]
        public string ID { get; set; }

        [JsonProperty("quantity_on_hand")]
        public int Quantity { get; set; }
        
    }
}