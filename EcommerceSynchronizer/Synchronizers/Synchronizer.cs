using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Model.Interfaces;

namespace EcommerceSynchronizer.Synchronizers
{
    public class Synchronizer
    {
        
        private readonly IEcommerceDatabase _ecommerceDatabase;
        private readonly IPOSInterfaceProvider _posInterfaceProvider;

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
                if (posInterface.CanMakeRequest())
                {
                    _ecommerceDatabase.UpdateAllProducts(posInterface.GetAllProducts());
                }
                else
                {
                    posInterface.RefreshToken();
                    if(posInterface.CanMakeRequest())
                        _ecommerceDatabase.UpdateAllProducts(posInterface.GetAllProducts());
                }
            }
        }

        // Triggered when a sale is made on the website
        // -> Update database of POS system based on the sale info
        // -> Update database of website

        public void UpdateFromSale(Sale sale)
        {
            var objectFromDatabase = _ecommerceDatabase.GetObjectByEcommerceID(sale.Object.EcommerceID);
            var posToUpdate = objectFromDatabase.POS;

            if (!posToUpdate.CanMakeRequest())
            {
                posToUpdate.RefreshToken();
                if (!posToUpdate.CanMakeRequest()) throw new UnauthorizedAccessException("Could not perform update on pos " + posToUpdate.AccountID);
            }
            posToUpdate.AdjustQuantityOfProduct(objectFromDatabase.PosID, sale.QuantitySold, sale.BalanceInCents);
            objectFromDatabase.Quantity -= sale.QuantitySold;

            _ecommerceDatabase.UpdateProduct(objectFromDatabase);
        }       
    }
}