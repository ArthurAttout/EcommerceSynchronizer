using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel
{
    public class PostSaleResponseBindingModel
    {
        [JsonProperty("sale_id")]
        public int SaleID { get; set; }
    }
}