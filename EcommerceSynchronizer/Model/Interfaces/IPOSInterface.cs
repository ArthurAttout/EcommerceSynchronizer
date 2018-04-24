using System.Collections;
using System.Collections.Generic;
using EcommerceSynchronizer.Model;

namespace EcommerceSynchronizer.Controllers
{
    public interface IPOSInterface
    {
        string AccountID { get; set; }

        bool AdjustQuantityOfProduct(string productId, int delta, int balanceInCents);
        void UpdateAllObjects(IList<Object> objects);
        IList<Object> GetAllProducts();
        bool CanMakeRequest();
        void RefreshToken();
    }
}