using System.Collections;
using System.Collections.Generic;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Model.Interfaces;

namespace EcommerceSynchronizer.Synchronizers
{
    public class Synchronizer
    {
        
        private IEcommerceDatabase _ecommerceDatabase;
        private IPOSInterfaceProvider _posInterfaceProvider;

        public Synchronizer(IPOSInterfaceProvider posInterfaceProvider, IEcommerceDatabase ecommerceDatabase)
        {
            _posInterfaceProvider = posInterfaceProvider;
            _ecommerceDatabase = ecommerceDatabase;
        }

        // Triggered when the timeout is reached
        // -> Retrieve stock information from POS 
        // -> Update ecommerce database appropriately 

        public void UpdateFromTimeout()
        {
            foreach(var posInterface in _posInterfaceProvider.GetAllInterfaces())
            {
                if(posInterface.CanMakeRequest())
                    _ecommerceDatabase.UpdateAllProducts(posInterface.GetAllProducts());
            }
        }

        // Triggered when a sale is made on the website
        // -> Update database of POS system based on the sale info
        // -> Update database of website

        public void UpdateFromSale(Sale sale)
        {

        }
    }
}