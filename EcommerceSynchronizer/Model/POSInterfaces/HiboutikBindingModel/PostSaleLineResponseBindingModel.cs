using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel
{
    public class PostSaleLineResponseBindingModel
    {
        [JsonProperty("id_sale_product_detail")]
        public int IDSaleProductDetail { get; set; }
    }
}