namespace EcommerceSynchronizer.Model.POSInterfaces.HiboutikBindingModel
{
    public class ItemStockBindingModel
    {
        public int product_id { get; set; }
        public string product_size { get; set; }
        public string warehouse_id { get; set; }
        public int stock_available { get; set; }
        public string inventory_alert { get; set; }
    }
}