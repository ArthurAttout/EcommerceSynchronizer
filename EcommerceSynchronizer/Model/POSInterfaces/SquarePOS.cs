using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model.Interfaces;
using EcommerceSynchronizer.Model.POSInterfaces.LightspeedPOSBindingModel;
using EcommerceSynchronizer.Model.POSInterfaces.SquarePOSBindingModel;
using Flurl.Http;
using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces
{
    public class SquarePOS : IPOSInterface
    {
        public string AccessToken { get; }
        private int _maximumRequest = 1000;
        public int Requests { get; }

        public SquarePOS(string accessToken, string location)
        {
            AccessToken = accessToken;
            AccountID = location;
        }

        public string AccountID { get; set; }

        public bool AdjustQuantityOfProduct(string variationID, int quantitySold, int balance)
        {
            var updateBindingModel = new UpdateItemBindingModel()
            {
                QuantityDelta = -quantitySold,
                AdjustmentType = "SALE"
            };

            var response = $"https://connect.squareup.com/v1/{AccountID}/inventory/{variationID}"
                .WithHeader("Authorization", $"Bearer {AccessToken}")
                .PostJsonAsync(updateBindingModel).Result;

            return response.IsSuccessStatusCode;
        }

        public void UpdateAllObjects(IList<Object> objects)
        {
            throw new System.NotImplementedException();
        }

        public IList<Object> GetAllProducts()
        {
            var list = $"http://connect.squareup.com/v1/{AccountID}/inventory"
                .WithHeader("accept", "application/json")
                .WithHeader("Authorization", $"Bearer {AccessToken}")
                .GetJsonAsync<List<ItemInventoryBindingModel>>().Result;
                

            var returnList = new List<Object>();
            foreach (var itemInventory in list)
            {
                var name = GetNameOfItemByVariationID(itemInventory.ID);
                returnList.Add(new Object()
                {
                    PosID = itemInventory.ID,
                    Quantity = itemInventory.Quantity,
                    POS = this,
                    Name = name
                });
            }
            return returnList;
        }

        private string GetNameOfItemByVariationID(string variationID)
        {

            var itemVariation = $"https://connect.squareup.com/v2/catalog/object/{variationID}"
                .WithHeader("accept", "application/json")
                .WithHeader("Authorization", $"Bearer {AccessToken}")
                .GetAsync()
                .ReceiveJson<ItemVariationBindingModel>().Result;
            
            var basicItemID = itemVariation.ObjectVariation.item_variation_data.item_id;
            var basicItem = $"https://connect.squareup.com/v2/catalog/object/{basicItemID}"
                .WithHeader("accept", "application/json")
                .WithHeader("Authorization", $"Bearer {AccessToken}")
                .GetAsync()
                .ReceiveJson<ItemBindingModel>().Result;

            if(basicItem == null)
                throw new ArgumentNullException("Could not retrieve basic item from variation item (square POS)");
            
            return $"{basicItem.ObjectItem?.item_data.name} - {itemVariation.ObjectVariation?.item_variation_data.name}";
        }

        public bool CanMakeRequest()
        {
            return _maximumRequest > Requests;
        }

        public void RefreshToken()
        {
            //No need to refresh a token on SquarePOS
            return; 
        }
    }
}