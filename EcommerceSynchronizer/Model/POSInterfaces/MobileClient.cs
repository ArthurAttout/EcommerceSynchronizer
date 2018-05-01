using System.Collections.Generic;
using EcommerceSynchronizer.Model.Interfaces;
using Flurl.Http;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Twitter;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EcommerceSynchronizer.Model.POSInterfaces
{
    public class MobileClient : IPOSInterface
    {
        public string AccountID { get; set; }
        public string FirebaseToken { get; set; }
        public string FirebaseClientToken { get; set; }

        public MobileClient(string AccountID, string firebaseFCM, string firebaseClientFCM)
        {
            this.FirebaseToken = firebaseFCM;
            this.FirebaseClientToken = firebaseClientFCM;
            this.AccountID = AccountID;
        }

        public bool AdjustQuantityOfProduct(Object objectSold, int quantitySold, int balanceInCents)
        {
            

            SendNotificationBody body = new SendNotificationBody()
            {
                DesinationToken = FirebaseClientToken,
                notification = new Notification()
                {
                    Title = "Une commande a été passée en ligne",
                    Body = $"{objectSold.Name} - Quantité vendue : {quantitySold}"
                }
            };

            //Send push notification
            var response = $"https://fcm.googleapis.com/fcm/send"
                .WithHeader("Authorization", $"key={FirebaseToken}")
                .PostJsonAsync(body).Result;

            return response.IsSuccessStatusCode;
        }

        public void UpdateAllObjects(IList<Object> objects)
        {
            
        }

        public IList<Object> GetAllProducts()
        {
            return new List<Object>();
        }

        public bool CanMakeRequest()
        {
            return true;
        }

        public void RefreshToken()
        {
            
        }

        public class SendNotificationBody
        {
            [JsonProperty("to")]
            public string DesinationToken { get; set; }

            [JsonProperty("notification")]
            public Notification notification { get; set; }
        }

        public class Notification
        {
            [JsonProperty("body")]
            public string Body { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }
    }
}