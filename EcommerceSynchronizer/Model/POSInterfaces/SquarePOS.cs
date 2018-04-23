using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model.POSInterfaces.SquarePOSBindingModel;
using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces
{
    public class SquarePOS : IPOSInterface
    {
        private string _accessToken;
        private int _maximumRequest = 1000;
        private int _requests;

        public SquarePOS(string accessToken, string location)
        {
            _accessToken = accessToken;
            AccountID = location;
        }

        public string AccountID { get; set; }

        public bool AdjustQuantityOfProduct(string productId, int delta)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAllObjects(IList<Object> objects)
        {
            throw new System.NotImplementedException();
        }

        public IList<Object> GetAllProducts()
        {
            var returnList = new List<Object>();
            
            var request = WebRequest.Create($"http://connect.squareup.com/v1/{AccountID}/inventory");
            var headers = new WebHeaderCollection
            {
                {HttpRequestHeader.ContentType, "application/json"},
                {HttpRequestHeader.Authorization, $"Bearer {_accessToken}"},
                {HttpRequestHeader.Accept, "application/json"}
            };
            request.Headers = headers;
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);
                var responseJSON = reader.ReadToEnd();
                var list = JsonConvert.DeserializeObject<List<ItemInventoryBindingModel>>(responseJSON);
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
                dataStream.Close();
            }
            

            return returnList;
        }

        private string GetNameOfItemByVariationID(string variationID)
        {
            var request = WebRequest.Create($"https://connect.squareup.com/v2/catalog/object/{variationID}");
            var headers = new WebHeaderCollection
            {
                {HttpRequestHeader.ContentType, "application/json"},
                {HttpRequestHeader.Authorization, $"Bearer {_accessToken}"},
                {HttpRequestHeader.Accept, "application/json"}
            };
            request.Headers = headers;
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);
                var responseJSON = reader.ReadToEnd();
                var itemVariation = JsonConvert.DeserializeObject<ItemVariationBindingModel>(responseJSON);
                var basicItemID = itemVariation.obj.item_variation_data.item_id;

                request = WebRequest.Create($"https://connect.squareup.com/v2/catalog/object/{variationID}");
                request.Headers = headers;
                response = request.GetResponse();
                dataStream = response.GetResponseStream();

                if (dataStream != null)
                {
                    reader = new StreamReader(dataStream);
                    responseJSON = reader.ReadToEnd();
                    var basicItem = JsonConvert.DeserializeObject<ItemBindingModel>(responseJSON);

                    return $"{basicItem.obj.item_data.name} - {itemVariation.obj.item_variation_data.name}";
                }
            }

            return null;
        }

        public bool CanMakeRequest()
        {
            return _maximumRequest > _requests;
        }

        public void RefreshToken()
        {
            //No need to refresh a token on SquarePOS
            return; 
        }
    }
}