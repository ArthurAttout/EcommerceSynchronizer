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
    }
}