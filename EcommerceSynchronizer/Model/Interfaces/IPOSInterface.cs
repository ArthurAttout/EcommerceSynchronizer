using System.Collections;
using System.Collections.Generic;
using EcommerceSynchronizer.Model;

namespace EcommerceSynchronizer.Controllers
{
    public interface IPOSInterface
    {
        bool AdjustQuantityOfProduct(string productId, int delta);
        void UpdateAllObjects(IList<Object> objects);
        IList<Object> GetAllProducts();
        bool CanMakeRequest();
    }
}