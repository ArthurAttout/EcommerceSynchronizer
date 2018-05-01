using System.Collections;
using System.Collections.Generic;
using EcommerceSynchronizer.Model;

namespace EcommerceSynchronizer.Model.Interfaces
{
    public interface IPOSInterface
    {
        string AccountID { get; set; }
        bool AdjustQuantityOfProduct(Object product, int quantitySold, int balanceInCents);
        void UpdateAllObjects(IList<Object> objects);
        IList<Object> GetAllProducts();
        bool CanMakeRequest();
        void RefreshToken();
    }
}