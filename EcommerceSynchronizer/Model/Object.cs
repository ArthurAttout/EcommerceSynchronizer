﻿using EcommerceSynchronizer.Controllers;
using EcommerceSynchronizer.Model.Interfaces;

namespace EcommerceSynchronizer.Model
{
    public class Object
    {
        //The ID in the general database         
        public string ID { get; set; }

        //The ID that is local to the POS
        public string PosID { get; set; }

        //The ID that is local to the ecommerce website
        public string EcommerceID { get; set; }

        public int Quantity { get; set; }

        //If the quantity is lower than this limit, the item won't be sold anymore
        public int LimitQuantitySale { get; set; }

        public IPOSInterface POS { get; set; }

        public string Name { get; set; }
    }
}