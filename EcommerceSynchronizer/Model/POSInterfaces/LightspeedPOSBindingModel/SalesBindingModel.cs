using System.Collections.Generic;

namespace EcommerceSynchronizer.Model.POSInterfaces.LightspeedPOSBindingModel
{
    public class SaleLine
    {
        public int itemID { get; set; }
        public int unitQuantity { get; set; }
    }

    public class SaleLines
    {
        public List<SaleLine> SaleLine { get; set; }
    }

    public class SalePayment
    {
        public double amount { get; set; }
        public int paymentTypeID { get; set; }
    }

    public class SalePayments
    {
        public SalePayment SalePayment { get; set; }
    }

    public class SalesBindingModel
    {
        public int employeeID { get; set; }
        public int registerID { get; set; }
        public int shopID { get; set; }
        public int customerID { get; set; }
        public bool completed { get; set; }
        public SaleLines SaleLines { get; set; }
        public SalePayments SalePayments { get; set; }
    }
}