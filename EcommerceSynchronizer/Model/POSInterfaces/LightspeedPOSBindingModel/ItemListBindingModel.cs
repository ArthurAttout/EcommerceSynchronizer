using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.LightspeedPOSBindingModel
{
    public class Attributes
    {
        public string count { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }

    public class ItemShop
    {
        public string itemShopID { get; set; }
        public string qoh { get; set; }
        public string backorder { get; set; }
        public string componentQoh { get; set; }
        public string componentBackorder { get; set; }
        public string reorderPoint { get; set; }
        public string reorderLevel { get; set; }
        public DateTime timeStamp { get; set; }
        public string itemID { get; set; }
        public string shopID { get; set; }
    }

    public class ItemShops
    {
        public List<ItemShop> ItemShop { get; set; }
    }

    public class ItemPrice
    {
        public string amount { get; set; }
        public string useTypeID { get; set; }
        public string useType { get; set; }
    }

    public class Prices
    {
        public List<ItemPrice> ItemPrice { get; set; }
    }

    public class Item
    {
        public string itemID { get; set; }
        public string systemSku { get; set; }
        public string defaultCost { get; set; }
        public string avgCost { get; set; }
        public string discountable { get; set; }
        public string tax { get; set; }
        public string archived { get; set; }
        public string itemType { get; set; }
        public string serialized { get; set; }
        public string description { get; set; }
        public string modelYear { get; set; }
        public string upc { get; set; }
        public string ean { get; set; }
        public string customSku { get; set; }
        public string manufacturerSku { get; set; }
        public DateTime createTime { get; set; }
        public DateTime timeStamp { get; set; }
        public string publishToEcom { get; set; }
        public string categoryID { get; set; }
        public string taxClassID { get; set; }
        public string departmentID { get; set; }
        public string itemMatrixID { get; set; }
        public string manufacturerID { get; set; }
        public string seasonID { get; set; }
        public string defaultVendorID { get; set; }
        public ItemShops ItemShops { get; set; }
        public Prices Prices { get; set; }
    }

    public class ItemListBindingModel
    {
        public Attributes Attributes { get; set; }
        public List<Item> Item { get; set; }
    }
}