using System.Collections;
using System.Collections.Generic;

namespace EcommerceSynchronizer.Model
{
    public interface IEcommerceDatabase
    {
        void UpdateAllProducts(IList<Object> objects);
        bool AddNewProduct(Object obj);
    }
}