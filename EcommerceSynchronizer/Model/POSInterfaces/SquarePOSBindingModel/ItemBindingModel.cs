using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcommerceSynchronizer.Model.POSInterfaces.SquarePOSBindingModel
{
   
    public class Variation
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

    public class ItemData
    {
        public string name { get; set; }
        public string visibility { get; set; }
        public string category_id { get; set; }
        public List<Variation> variations { get; set; }
        public string product_type { get; set; }
        public bool skip_modifier_screen { get; set; }
    }

    public class ObjectItem
    {
        public string type { get; set; }
        public string id { get; set; }
        public DateTime updated_at { get; set; }
        public long version { get; set; }
        public bool is_deleted { get; set; }
        public bool present_at_all_locations { get; set; }
        public List<string> present_at_location_ids { get; set; }
        public List<string> absent_at_location_ids { get; set; }
        public ItemData item_data { get; set; }
    }

    public class ItemBindingModel
    {
        [JsonProperty("object")]
        public ObjectItem ObjectItem { get; set; }
    }
}