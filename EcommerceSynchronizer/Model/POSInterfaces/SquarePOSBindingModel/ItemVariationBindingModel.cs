using System;
using System.Collections.Generic;

namespace EcommerceSynchronizer.Model.POSInterfaces.SquarePOSBindingModel
{
    public class PriceMoney
    {
        public int amount { get; set; }
        public string currency { get; set; }
    }

    public class LocationOverride
    {
        public string location_id { get; set; }
        public bool track_inventory { get; set; }
    }

    public class ItemVariationData
    {
        public string item_id { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public int ordinal { get; set; }
        public string pricing_type { get; set; }
        public PriceMoney price_money { get; set; }
        public List<LocationOverride> location_overrides { get; set; }
    }

    public class ObjectVariation
    {
        public string type { get; set; }
        public string id { get; set; }
        public DateTime updated_at { get; set; }
        public long version { get; set; }
        public bool is_deleted { get; set; }
        public bool present_at_all_locations { get; set; }
        public List<string> present_at_location_ids { get; set; }
        public List<string> absent_at_location_ids { get; set; }
        public ItemVariationData item_variation_data { get; set; }
    }

    public class ItemVariationBindingModel
    {
        public ObjectVariation obj { get; set; }
    }
}