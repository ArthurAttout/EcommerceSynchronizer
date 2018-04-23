using System.Collections.Generic;
using EcommerceSynchronizer.Controllers;

namespace EcommerceSynchronizer.Model.POSInterfaces
{
    public class LightspeedPOS : IPOSInterface
    {
        public string AccesssToken { get; set; }
        public string RefreshToken { get; set; }

        public LightspeedPOS(string accessToken, string refreshToken)
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
            throw new System.NotImplementedException();
        }
    }
}