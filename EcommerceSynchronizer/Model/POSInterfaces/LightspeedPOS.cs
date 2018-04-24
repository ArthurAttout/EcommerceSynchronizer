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

        public bool AdjustQuantityOfProduct(string productId, int quantitySold, int balanceInCents)
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
                            unitQuantity = quantitySold
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
                Console.WriteLine("Error while selling product : " + ex.ToString());
                throw;
            }

            return true;
        }

        public void UpdateAllObjects(IList<Object> objects)
        {
            throw new System.NotImplementedException();
        }

        public IList<Object> GetAllProducts()
        {

            var list = $"https://api.lightspeedapp.com/API/Account/{AccountID}/Item.json?load_relations=[\"ItemShops\"]"
                .WithHeader("accept", "application/json")
                .WithHeader("Authorization", $"Bearer {AccesssToken}")
                .GetAsync()
                .ReceiveJson<ItemListBindingModel>().Result;

            var returnList = new List<Object>();
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
            return returnList;
        }

        public bool CanMakeRequest()
        {
            // Gives a tolerance of 10 seconds to make all requests
            return (DateTokenExpiration > DateTime.Now.AddSeconds(10));
        }

        void IPOSInterface.RefreshToken()
        {
            var refreshTokenModel = new RefreshTokenBindingModel()
            {
                ClientID = ClientID,
                ClientSecret = ClientSecret,
                RefreshToken = RefreshToken,
                GrantType = "refresh_token"
            };
            var accessTokenModel = "https://cloud.lightspeedapp.com/oauth/access_token.php"
                .PostJsonAsync(refreshTokenModel)
                .ReceiveJson<AccessTokenBindingModel>().Result;

            DateTokenExpiration = DateTime.Now.AddSeconds(accessTokenModel.ExpiresIn);
            AccesssToken = accessTokenModel.AccessToken;
        }
    }
}