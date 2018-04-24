using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using EcommerceSynchronizer.Model.POSInterfaces.LightspeedPOSBindingModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var request = WebRequest.Create("https://cloud.lightspeedapp.com/API/Account/168750/Sale.json");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var headers = new WebHeaderCollection
            {
                {HttpRequestHeader.ContentType, "application/json"},
                {HttpRequestHeader.Authorization, "Bearer d9ea38f275d21bf9176d88dfe7c92d659046ba69"},
                {HttpRequestHeader.Accept, "application/json"}
            };
            request.Headers = headers;

            var sale = new SalesBindingModel()
            {
                shopID = 1,
                registerID = 1,
                employeeID = 1,
                customerID = 0,
                completed = true,
                SaleLines = new SaleLines()
                {
                    SaleLine = new List<SaleLine>(new[]
                    {
                        new SaleLine()
                        {
                            itemID = int.Parse("1"),
                            unitQuantity = 2
                        }
                    })
                }
            };

            var json = JsonConvert.SerializeObject(sale);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
}
