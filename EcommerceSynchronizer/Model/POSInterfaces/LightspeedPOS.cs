using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model.POSInterfaces.LightspeedPOSBindingModel;
using EcommerceSynchronizer.Model.POSInterfaces.SquarePOSBindingModel;
using Flurl.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces
{
    public class LightspeedPOS : IPOSInterface
    {
        public int EmployeeId { get; }
        public int RegisterId { get; }
        public int ShopId { get; }
        public string AccesssToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DateTokenExpiration { get; set; }
        public string ClientID { get; set; }
        public string AccountID { get; set; }
        public string ClientSecret { get; set; }

        public LightspeedPOS(string accessToken, string refreshToken, string clientID, string clientSecret, string accountId, int employeeId, int registerId, int shopId)
        {
            EmployeeId = employeeId;
            RegisterId = registerId;
            ShopId = shopId;
            AccesssToken = accessToken;
            RefreshToken = refreshToken;
            AccountID = accountId;
            ClientID = clientID;
            ClientSecret = clientSecret;
        }

        public bool AdjustQuantityOfProduct(string productId, int delta, int balanceInCents)
        {
            var sale = new SalesBindingModel()
            {
                shopID = ShopId,
                registerID = RegisterId,
                employeeID = EmployeeId,
                customerID = 0,
                completed = true,
                SaleLines = new SaleLines()
                {
                    SaleLine = new List<SaleLine>(new[]
                    {
                        new SaleLine()
                        {
                            itemID = int.Parse(productId),
                            unitQuantity = delta
                        }
                    })
                },
                SalePayments = new SalePayments()
                {
                    SalePayment = new SalePayment()
                    {    
                        amount = (double) balanceInCents/100,
                        paymentTypeID = 3 //Stands for credit card type      
                    }
                }
            };

            try
            {
                var result = $"https://api.lightspeedapp.com/API/Account/{AccountID}/Sale.json"
                    .WithHeader("content-type", "application/json")
                    .WithHeader("Authorization", $"Bearer {AccesssToken}")
                    .PostJsonAsync(sale)
                    .ReceiveString().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return true;
        }

        public void UpdateAllObjects(IList<Object> objects)
        {
            throw new System.NotImplementedException();
        }

        public IList<Object> GetAllProducts()
        {
            var returnList = new List<Object>();
            
            var request = WebRequest.Create($"https://api.lightspeedapp.com/API/Account/{AccountID}/Item.json?load_relations=[\"ItemShops\"]");
            var headers = new WebHeaderCollection
            {
                {HttpRequestHeader.ContentType, "application/json"},
                {HttpRequestHeader.Authorization, $"Bearer {AccesssToken}"},
                {HttpRequestHeader.Accept, "application/json"}
            };
            request.Headers = headers;
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);
                var responseJson = reader.ReadToEnd();
                var list = JsonConvert.DeserializeObject<ItemListBindingModel>(responseJson);
                foreach (var itemInventory in list.Item)
                {
                    returnList.Add(new Object()
                    {
                        PosID = itemInventory.itemID,
                        Quantity = int.Parse(itemInventory.ItemShops.ItemShop.ElementAt(0).qoh), //TODO handle multiple shops
                        POS = this,
                        Name = itemInventory.description
                    });
                }
            }
            

            return returnList;
        }

        public bool CanMakeRequest()
        {
            // Gives a tolerance of 10 seconds to make all requests
            return (DateTokenExpiration > DateTime.Now.AddSeconds(10));
        }

        void IPOSInterface.RefreshToken()
        {
            var request = WebRequest.Create("https://cloud.lightspeedapp.com/oauth/access_token.php");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add("refresh_token", RefreshToken);
            outgoingQueryString.Add("client_id", ClientID);
            outgoingQueryString.Add("client_secret", ClientSecret);
            outgoingQueryString.Add("grant_type", "refresh_token");

            var postdata = outgoingQueryString.ToString();

            var byteArray = Encoding.UTF8.GetBytes(postdata);
            request.ContentLength = byteArray.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var response = request.GetResponse();
            dataStream = response.GetResponseStream();
            if (dataStream == null)
                throw new SecurityTokenException("Could not refresh token for client " + ClientID);

            var reader = new StreamReader(dataStream);
            var responseJSON = reader.ReadToEnd();
            var accessTokenModel = JsonConvert.DeserializeObject<AccessTokenBindingModel>(responseJSON);
            DateTokenExpiration = DateTime.Now.AddSeconds(accessTokenModel.ExpiresIn);
            AccesssToken = accessTokenModel.AccessToken;
        }
    }
}