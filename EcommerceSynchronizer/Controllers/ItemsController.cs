using System;
using System.Linq;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Model.Interfaces;
using EcommerceSynchronizer.Synchronizers;
using EcommerceSynchronizer.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Object = EcommerceSynchronizer.Model.Object;

namespace EcommerceSynchronizer.Controllers
{
    [Produces("application/json")]
    public class ItemsController : Controller
    {
        private readonly ApplicationState _state;
        private readonly IPOSInterfaceProvider _posProvider;
        private readonly IEcommerceDatabase _ecommerceDatabase;

        public ItemsController(ApplicationState state, IPOSInterfaceProvider provider, IEcommerceDatabase database)
        {
            _state = state;
            _ecommerceDatabase = database;
            _posProvider = provider;
        }


        // Endpoint to insert a new item in the stock database 
        // After it is added, the item will automatically be synchronized
        [Route("api/items")]
        [HttpPost]
        public string PostStart([FromBody]string item)
        {
            var obj = JsonConvert.DeserializeObject<PostItemBindingModel>(item);
            BackgroundJob.Enqueue(() => AddNewItem(obj));
            return "started";
        }

        private bool AddNewItem(PostItemBindingModel item)
        {
            if (item.ItemPOSID != null)
            {
                foreach (var posInterface in _posProvider.GetAllInterfaces())
                {
                    var allProducts = posInterface.GetAllProducts();
                    var product = allProducts.First(i => i.PosID.Equals(item.ItemPOSID));
                    if (product == null) //The ID could not be found
                        return false;

                    var itemToInsert = new Object()
                    {
                        EcommerceID = item.ItemEcommerceID,
                        POS = posInterface,
                        PosID = item.ItemPOSID,
                        Quantity = product.Quantity,
                        Name = product.Name
                    };
                    _ecommerceDatabase.AddNewProduct(itemToInsert);
                }
            }
            else
            {
                foreach (var posInterface in _posProvider.GetAllInterfaces())
                {
                    var allProducts = posInterface.GetAllProducts();
                    var product = allProducts.First(i => i.Name.Equals(item.ItemName));
                    if (product == null) //The name could not be found
                        return false;

                    var itemToInsert = new Object()
                    {
                        EcommerceID = item.ItemEcommerceID,
                        POS = posInterface,
                        PosID = item.ItemPOSID,
                        Quantity = product.Quantity
                    };
                }
            }

            return false;
        }

    }

    public class PostItemBindingModel
    {
        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        [JsonProperty("item_pos_id")]
        public string ItemPOSID { get; set; }

        [JsonProperty("item_ecommerce_id")]
        public string ItemEcommerceID { get; set; }

        [JsonProperty("account_id")]
        public string AccountID { get; set; }
    }
}