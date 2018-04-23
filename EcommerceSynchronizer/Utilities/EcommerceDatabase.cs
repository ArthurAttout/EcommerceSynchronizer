using System;
using System.Collections.Generic;
using System.Data;
using Hangfire.Dashboard;
using MySql.Data.MySqlClient;

namespace EcommerceSynchronizer.Model
{
    public class EcommerceDatabase : IEcommerceDatabase
    {
        public MySqlConnection MySqlConnection { get; set; }

        public string TableStockName { get; set; }
        public string ColumnID { get; set; }
        public string ColumnIDPos { get; set; }
        public string ColumnIDEcommerce { get; set; }
        public string ColumnQuantity { get; set; }

        public EcommerceDatabase(string connectionString)
        {
            MySqlConnection = new MySqlConnection(connectionString);
            TableStockName = "stock";
            ColumnIDPos = "idProductPOS";
            ColumnIDEcommerce = "idProductEcommerce";
            ColumnID = "id";
            ColumnQuantity = "quantity";

        }

        public void UpdateAllProducts(IList<Object> objects)
        {
            try
            {
                if((MySqlConnection.State & ConnectionState.Open) == 0) //if not already opened
                    MySqlConnection.Open();

                foreach (var obj in objects)
                {
                    var cmd = new MySqlCommand("UPDATE @TableName WHERE @ColumnIDPos=@IDPos SET @ColumnQuantity=@Quantity", MySqlConnection);

                    cmd.Parameters.AddWithValue("@TableName", TableStockName);
                    cmd.Parameters.AddWithValue("@ColumnIDPos", ColumnIDPos);
                    cmd.Parameters.AddWithValue("@IDPos",obj.PosID);
                    cmd.Parameters.AddWithValue("@ColumnQuantity", ColumnQuantity);
                    cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
                    cmd.Prepare();

                    cmd.ExecuteNonQuery();
                }
                
                MySqlConnection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}