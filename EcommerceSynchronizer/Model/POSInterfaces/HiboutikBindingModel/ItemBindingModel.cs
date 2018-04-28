using System.Collections.Generic;

namespace EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel
{

    public class ProductSizeDetail
    {
        public string size_id { get; set; }
        public string size_name { get; set; }
        public string barcode { get; set; }
    }

    public class ItemBindingModel
    {
        public string product_id { get; set; }
        public string product_model { get; set; }
        public string product_brand { get; set; }
        public string product_supplier { get; set; }
        public string product_price { get; set; }
        public string product_discount_price { get; set; }
        public string product_supply_price { get; set; }
        public string points_in { get; set; }
        public string points_out { get; set; }
        public string product_category { get; set; }
        public string product_size_type { get; set; }
        public string product_package { get; set; }
        public string product_stock_management { get; set; }
        public string product_supplier_reference { get; set; }
        public string product_vat { get; set; }
        public string product_display { get; set; }
        public string product_display_www { get; set; }
        public string product_arch { get; set; }
        public string products_desc { get; set; }
        public List<ProductSizeDetail> product_size_details { get; set; }
        public List<object> product_specific_rules { get; set; }
        public string product_barcode { get; set; }
    }
}