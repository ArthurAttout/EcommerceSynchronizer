﻿using System;
using System.Collections.Generic;
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

        [Route("api/itemstask/{jobID}")]
        [HttpGet("api/itemstask/{jobID}")]
        public string GetStatus(string jobid)
        {
            //TODO enhance response
            var connection = JobStorage.Current.GetConnection();
            var jobData = connection.GetJobData(jobid);
            var stateName = jobData.State;
            
            return "Job status : " + stateName;
        }

        // GET
        [Route("api/items/{accountID}")]
        [HttpGet("api/items/{accountID}")]
        public List<Object> GetAllObjectsOfAccountId(string accountID)
        {
            return _ecommerceDatabase.GetAllObjectsOfAccountID(accountID);
        }

        // Endpoint to insert a new item in th e stock database 
        // After it is added, the item will automatically be synchronized
        [Route("api/items")]
        [HttpPost]
        public string PostStart([FromBody] PostItemBindingModel item)
        {

            if (item?.ItemName == null && item?.ItemPosId == null || item?.AccountId == null)
            {
                Response.StatusCode = 400;
                return "invalid request";
            }

            var jobID = BackgroundJob.Enqueue(() => AddNewItem(item));
            Response.StatusCode = 202;
            return "pending added. Location : itemstask/" + jobID;
        }

        [AutomaticRetry(Attempts = 0)]
        public void AddNewItem(PostItemBindingModel item)
        {
            try
            {
                if (item.IsMobileClient)
                {
                    AddItemForMobileClient(item);
                }
                else
                {
                    AddItemForPOS(item);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
        }

        private void AddItemForMobileClient(PostItemBindingModel item)
        {
            var posInterface = _posProvider.GetAllInterfaces()
                .FirstOrDefault(i => i.AccountID.Equals(item.AccountId));

            if (posInterface == null)
                throw new ArgumentException("The POS account with ID " + item.AccountId + " could not be found.");

            var itemToInsert = new Object()
            {
                EcommerceID = item.ItemEcommerceId,
                POS = posInterface,
                PosID = item.ItemPosId,
                Quantity = item.Quantity,
                Name = item.ItemName,
                LimitQuantitySale = item.LimitQuantity
            };
            _ecommerceDatabase.AddNewProduct(itemToInsert);
        }

        private void AddItemForPOS(PostItemBindingModel item)
        {
            var posInterface = _posProvider.GetAllInterfaces()
                .FirstOrDefault(i => i.AccountID.Equals(item.AccountId));

            if (posInterface == null)
                throw new ArgumentException("The POS account with ID " + item.AccountId + " could not be found.");
            

            if (!posInterface.CanMakeRequest())
            {
                posInterface.RefreshToken();
                if (!posInterface.CanMakeRequest()) return;
            }

            var allProducts = posInterface.GetAllProducts();
            Object product;

            if (item.ItemPosId != null)
                product = allProducts.FirstOrDefault(i => i.PosID.Equals(item.ItemPosId));
            else
                product = allProducts.FirstOrDefault(i => i.Name.Equals(item.ItemName));

            if (product == null) //The name could not be found
                throw new ArgumentException("The product with name " + item.ItemName +
                                            " could not be found in the specified POS system register.\n" +
                                            "Valid products include : [" +
                                            string.Join(",", allProducts.Select(p => p.Name).ToArray()) + "]");

            var itemToInsert = new Object()
            {
                EcommerceID = item.ItemEcommerceId,
                POS = posInterface,
                PosID = product.PosID,
                Quantity = product.Quantity,
                Name = product.Name,
                LimitQuantitySale = item.LimitQuantity
            };
            _ecommerceDatabase.AddNewProduct(itemToInsert);
        }
    }

    public class PostItemBindingModel
    {
        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        [JsonProperty("item_pos_id")]
        public string ItemPosId { get; set; }

        [JsonProperty("item_ecommerce_id")]
        public string ItemEcommerceId { get; set; }

        [JsonProperty("limit_quantity")]
        public int LimitQuantity { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("is_mobile_client")]
        public bool IsMobileClient { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}