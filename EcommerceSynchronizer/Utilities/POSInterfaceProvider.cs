﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model.Interfaces;
using EcommerceSynchronizer.Model.POSInterfaces;
using EcommerceSynchronizer.Utilities;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using Microsoft.Extensions.Configuration;
using IConfigurationProvider = EcommerceSynchronizer.Utilities.IConfigurationProvider;

namespace EcommerceSynchronizer.Model
{
    public class POSInterfaceProvider : IPOSInterfaceProvider
    {
        private IList<IPOSInterface> _posInterfaces;

        public POSInterfaceProvider(POSConfigurationList configuration, IConfigurationProvider configProvider)
        {
            
            _posInterfaces = new List<IPOSInterface>();
            foreach(var posCfg in configuration.PosConfigurations)
            {
                switch (posCfg.Type)
                {
                    case EnumPOSTypes.SQUAREPOS:
                        _posInterfaces.Add(new SquarePOS(posCfg.AccessToken, 
                            posCfg.AccountID));
                        break;

                    case EnumPOSTypes.HIBOUTIK:
                        _posInterfaces.Add(new HiboutikPOS(posCfg.AccessToken,
                            posCfg.EmailAddress,
                            posCfg.AccountName,
                            posCfg.StoreID,
                            posCfg.HiboutikCustomerID,
                            posCfg.MaximumRequests));
                        break;
                        
                    case EnumPOSTypes.LIGHTSPEED:
                        _posInterfaces.Add(new LightspeedPOS(posCfg.AccessToken,
                            posCfg.RefreshToken,
                            posCfg.ClientID,
                            posCfg.ClientSecret,
                            posCfg.LightspeedCustomerID,
                            posCfg.AccountID,
                            posCfg.EmployeeID,
                            posCfg.RegisterID,
                            posCfg.ShopID));
                        break;

                    case EnumPOSTypes.MOBILE_CLIENT:
                        _posInterfaces.Add(new MobileClient(posCfg.AccountID, configProvider.GetFirebaseServerToken(), posCfg.FirebaseFCMID));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Unrecognized POS type");
                }
            }
        }

        public IList<IPOSInterface> GetAllInterfaces()
        {
            return _posInterfaces;
        }
    }
}