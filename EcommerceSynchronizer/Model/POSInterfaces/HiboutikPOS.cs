using System.Collections.Generic;
using EcommerceSynchronizer.Controllers;

namespace EcommerceSynchronizer.Model.POSInterfaces
{
    public class HiboutikPOS : IPOSInterface
    {
        private int _maximumRequest = 10000;
        private int requests;

        public HiboutikPOS(string accessToken, string emailAddress, string accountName)
        {
            AccountID = accountName;
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
            return new List<Object>();
        }

        public bool CanMakeRequest()
        {
            return _maximumRequest > requests;
        }

        public void RefreshToken()
        {
            throw new System.NotImplementedException();
        }
    }
}