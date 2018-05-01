using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel
{
    public class PostSaleBindingModel
    {
        [JsonProperty("store_id")]
        public int StoreID { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("customer_id")]
        public int CustomerID { get; set; }
    }
}