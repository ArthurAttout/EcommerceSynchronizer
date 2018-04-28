using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel
{
    public class CloseSaleBindingModel
    {
        [JsonProperty("sale_id")]
        public int SaleID { get; set; }
    }
}