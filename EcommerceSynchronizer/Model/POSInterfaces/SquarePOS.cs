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
        private List<string> _locations;
        private int _maximumRequest = 1000;
        private int _requests;

        public SquarePOS(string accessToken,List<string> locations)
        {
            _accessToken = accessToken;
            _locations = locations;
        }

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
            foreach (var location in _locations)
            {
                var request = WebRequest.Create($"http://connect.squareup.com/v1/{location}/inventory");

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
                    var list = JsonConvert.DeserializeObject<List<ItemInventory>>(responseJSON);
                    var returnList = new List<Object>();
                    foreach (var itemInventory in list)
                    {
                        returnList.Add(new Object()
                        {
                            PosID = itemInventory.ID,
                            Quantity = itemInventory.Quantity
                        });
                    }
                }
            }

            return null;
        }

        public bool CanMakeRequest()
        {
            return _maximumRequest > _requests;
        }
    }
}