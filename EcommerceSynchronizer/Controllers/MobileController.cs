using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EcommerceSynchronizer.Controllers
{
    public class MobileController : Controller
    {
        // POST
        [Route("api/mobile/registerToken")]
        [HttpPost]
        public void GetAllObjectsOfAccountId([FromBody]RegisterFCMTokenBody body)
        {
            Console.WriteLine("*************");
            Console.WriteLine(body.AccountID);
            Console.WriteLine(body.Token);
            Console.WriteLine("*************");
        }
    }


    public class RegisterFCMTokenBody
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("account_id")]
        public string AccountID { get; set; }
    }
}