using System.Collections;
using System.Collections.Generic;
using EcommerceSynchronizer.Controllers;

namespace EcommerceSynchronizer.Model
{
    public interface IEcommerceDatabase
    {
        void UpdateAllProducts(IList<Object> objects);
        bool UpdateProduct(Object obj);
        bool AddNewProduct(Object obj);
        Object GetObjectByAccountIDAndID(string posid, string accountID);
        Object GetObjectByEcommerceID(string id);
        List<Object> GetAllObjectsOfAccountID(string id);
    }
}