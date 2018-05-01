using System.Collections.Generic;
using System.Linq;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model.Interfaces;
using EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel;
using Flurl.Http;
using PostSaleBindingModel = EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel.PostSaleBindingModel;
using PostSaleResponseBindingModel = EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel.PostSaleResponseBindingModel;

namespace EcommerceSynchronizer.Model.POSInterfaces
{
    public class HiboutikPOS : IPOSInterface
    {
        private readonly int _maximumRequest;
        private int _requests;
        private int _storeId;
        private readonly string _accessToken;
        private readonly string _emailAddress;
        private readonly int _customerID;

        public HiboutikPOS(string accessToken, string emailAddress, string accountName, int storeId, int customerId, int maximumRequest)
        {
            AccountID = accountName;
            _requests = 0;
            _emailAddress = emailAddress;
            _storeId = storeId;
            _accessToken = accessToken;
            _maximumRequest = maximumRequest;
            _customerID = customerId;
        }

        public string AccountID { get; set; }

        public bool AdjustQuantityOfProduct(Object objectSold, int quantitySold, int balance)
        {
            
            var sale = new PostSaleBindingModel()
            {
                StoreID = _storeId,
                CurrencyCode = "EUR",
                CustomerID = _customerID
            };

            //Create the sale
            _requests++;
            var result = $"https://{AccountID}.hiboutik.com/apirest/sales/"
                .WithHeader("content-type", "application/json")
                .WithBasicAuth(_emailAddress, _accessToken)
                .PostJsonAsync(sale)
                .ReceiveJson<PostSaleResponseBindingModel>().Result;

            //Add a new line on the created sale

            var saleLine = new PostSaleLineBindingModel()
            {
                Quantity = quantitySold,
                ProductID = int.Parse(objectSold.PosID),
                SaleID = result.SaleID,
                StockWithdrawal = 1
            };
            _requests++;
            var lineResult = $"https://{AccountID}.hiboutik.com/apirest/sales/add_product/"
                .WithHeader("content-type", "application/json")
                .WithBasicAuth(_emailAddress, _accessToken)
                .PostJsonAsync(saleLine)
                .ReceiveJson<PostSaleLineResponseBindingModel>().Result;

            //Close the sale

            var closeSale = new CloseSaleBindingModel()
            {
                SaleID = result.SaleID
            };
            _requests++;
            var closeResult = $"https://{AccountID}.hiboutik.com/apirest/sales/close/"
                .WithHeader("content-type", "application/json")
                .WithBasicAuth(_emailAddress, _accessToken)
                .PostJsonAsync(closeSale)
                .ReceiveString().Result;

            return true;
        }

        public void UpdateAllObjects(IList<Object> objects)
        {
            _requests++;
            throw new System.NotImplementedException();
        }

        public IList<Object> GetAllProducts()
        {
            _requests++;
            var products = $"https://{AccountID}.hiboutik.com/apirest/products/"
                .WithHeader("Accept", "application/json")
                .WithBasicAuth(_emailAddress, _accessToken)
                .GetJsonAsync<List<ItemBindingModel>>().Result;

            var returnList = new List<Object>();
            foreach (var itemBindingModel in products)
            {
                var quantity = GetProductAvailableQuantity(itemBindingModel.product_id);
                if(quantity == -1) continue; //Object with no stock
                returnList.Add(new Object()
                {
                    POS = this,
                    Name = itemBindingModel.product_model,
                    PosID = itemBindingModel.product_id,
                    Quantity = quantity
                });
            }

            return returnList;
        }

        private int GetProductAvailableQuantity(string productId)
        {
            var product = $"https://{AccountID}.hiboutik.com/apirest/stock_available/product_id/{productId}"
                .WithHeader("Accept", "application/json")
                .WithBasicAuth(_emailAddress, _accessToken)
                .GetJsonAsync<List<ItemStockBindingModel>>().Result;

            if (product.Count == 0) return -1;
            return product.ElementAt(0).stock_available;
        }

        public bool CanMakeRequest()
        {
            return _maximumRequest > _requests;
        }

        public void RefreshToken()
        {
            //Nothing to do 
        }
    }
}