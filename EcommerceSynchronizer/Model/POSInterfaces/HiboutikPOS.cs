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
            throw new System.NotImplementedException();
        }

        public bool CanMakeRequest()
        {
            return _maximumRequest > requests;
        }
    }
}