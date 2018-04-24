using System;
using System.Linq;
using System.Net;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Model.Interfaces;
using EcommerceSynchronizer.Synchronizers;
using EcommerceSynchronizer.Utilities;
using Hangfire;
using Hangfire.Storage;
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

        [Route("api/items/{jobID}")]
        [HttpGet("api/items/{jobID}")]
        public string GetStatus(string jobid)
        {
            var connection = JobStorage.Current.GetConnection();
            var jobData = connection.GetJobData(jobid);
            var stateName = jobData.State;

            return "Job status : " + stateName;
        }

        // Endpoint to insert a new item in th e stock database 
        // After it is added, the item will automatically be synchronized
        [Route("api/items")]
        [HttpPost]
        public string PostStart([FromBody] PostItemBindingModel item)
        {

            if (item?.item_name == null && item?.item_pos_id == null || item?.account_id == null)
            {
                Response.StatusCode = 400;
                return "invalid request";
            }

            var jobID = BackgroundJob.Enqueue(() => AddNewItem(item));
            Response.StatusCode = 202;
            return "pending added. Location : items/" + jobID;
        }

        [AutomaticRetry(Attempts = 0)]
        public void AddNewItem(PostItemBindingModel item)
        {
            try
            {
                var posInterface = _posProvider.GetAllInterfaces()
                    .FirstOrDefault(i => i.AccountID.Equals(item.account_id));

                if (posInterface == null)
                    throw new ArgumentException("The POS account with ID " + item.account_id + " could not be found."); ;

                if (!posInterface.CanMakeRequest())
                {
                    posInterface.RefreshToken();
                    if (!posInterface.CanMakeRequest()) return;
                }
                var allProducts = posInterface.GetAllProducts();
                Object product;

                if (item.item_pos_id != null)
                    product = allProducts.FirstOrDefault(i => i.PosID.Equals(item.item_pos_id));
                else
                    product = allProducts.FirstOrDefault(i => i.Name.Equals(item.item_name));
                
                if (product == null) //The name could not be found
                    throw new ArgumentException("The product with name " + item.item_name + " could not be found in the specified POS system register.\n" +
                                                "Valid products include : [" + string.Join(",", allProducts.Select(p => p.Name).ToArray()) + "]");

                var itemToInsert = new Object()
                {
                    EcommerceID = item.item_ecommerce_id,
                    POS = posInterface,
                    PosID = product.PosID,
                    Quantity = product.Quantity,
                    Name = product.Name
                };
                _ecommerceDatabase.AddNewProduct(itemToInsert);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
        }
    }

    public class PostItemBindingModel
    {
        [JsonProperty("item_name")]
        public string item_name { get; set; }

        [JsonProperty("item_pos_id")]
        public string item_pos_id { get; set; }

        [JsonProperty("item_ecommerce_id")]
        public string item_ecommerce_id { get; set; }

        [JsonProperty("account_id")]
        public string account_id { get; set; }
    }
}