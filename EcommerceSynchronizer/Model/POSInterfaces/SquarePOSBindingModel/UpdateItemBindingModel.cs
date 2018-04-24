using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EcommerceSynchronizer.Model.POSInterfaces.SquarePOSBindingModel
{
    public class UpdateItemBindingModel
    {
        [JsonProperty("adjustment_type")]
        public string AdjustmentType { get; set; }

        [JsonProperty("quantity_delta")]
        public int QuantityDelta { get; set; }
    }
}