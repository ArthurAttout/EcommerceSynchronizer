using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel
{
    public class PostSaleLineBindingModel
    {
        [JsonProperty("sale_id")]
        public int SaleID { get; set; }

        [JsonProperty("product_id")]
        public int ProductID { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("stock_withdrawal")]
        public int StockWithdrawal { get; set; }
    }
}