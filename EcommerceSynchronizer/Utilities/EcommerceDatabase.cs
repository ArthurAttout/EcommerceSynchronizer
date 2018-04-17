using System;
using System.Collections.Generic;
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

        public EcommerceDatabase(string connectionString)
        {
            MySqlConnection = new MySqlConnection(connectionString);
        }

        public void UpdateAllProducts(IList<Object> objects)
        {
            try
            {
                MySqlConnection.Open();

                foreach (var obj in objects)
                {
                    var cmd = new MySqlCommand("UPDATE @TableName WHERE @ColumnIDPos=@IDPos", MySqlConnection);

                    cmd.Parameters.AddWithValue("@TableName", TableStockName);
                    cmd.Parameters.AddWithValue("@ColumnIDPos", ColumnIDPos);
                    cmd.Parameters.AddWithValue("@IDPos",obj.PosID);
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