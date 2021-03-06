﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Model.Interfaces;
using Hangfire;

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
        [AutomaticRetry(Attempts = 0)]
        public bool UpdateFromSale(Sale sale)
        {
            var objectFromDatabase = _ecommerceDatabase.GetObjectByEcommerceID(sale.Object.EcommerceID);
            var posToUpdate = objectFromDatabase.POS;

            if (!posToUpdate.CanMakeRequest())
            {
                posToUpdate.RefreshToken();
                if (!posToUpdate.CanMakeRequest())
                {
                    throw new UnauthorizedAccessException("Could not update POS " + posToUpdate.AccountID);
                }
            }

            if (objectFromDatabase.Quantity - sale.QuantitySold > objectFromDatabase.LimitQuantitySale)
            {
                posToUpdate.AdjustQuantityOfProduct(objectFromDatabase, sale.QuantitySold, sale.BalanceInCents);
                objectFromDatabase.Quantity -= sale.QuantitySold;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Quantity limit has been reached");
            }
            _ecommerceDatabase.UpdateProduct(objectFromDatabase);
            return true;
        }       
    }
}