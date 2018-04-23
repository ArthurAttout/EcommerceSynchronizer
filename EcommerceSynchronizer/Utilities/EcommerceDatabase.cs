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

        public string TableStockName { get; }

        public string ColumnID { get; }
        public string ColumnIDPos { get; }
        public string ColumnIDEcommerce { get; }
        public string ColumnAccountID { get; }
        public string ColumnQuantity { get; }
        

        public EcommerceDatabase(string connectionString)
        {
            MySqlConnection = new MySqlConnection(connectionString);
            TableStockName = "stock";
            ColumnIDPos = "idProductPOS";
            ColumnIDEcommerce = "idProductEcommerce";
            ColumnID = "id";
            ColumnQuantity = "quantity";
            ColumnAccountID = "accountID";
        }

        public void UpdateAllProducts(IList<Object> objects)
        {
            try
            {
                if((MySqlConnection.State & ConnectionState.Open) == 0) //if not already opened
                    MySqlConnection.Open();

                foreach (var obj in objects)
                {
                    //Retrieve original object from database
                    var objFromDatabase = GetObjectByAccountIDAndID(obj);
                    if (objFromDatabase == null) continue;

                    var cmd = new MySqlCommand($"INSERT INTO {TableStockName} ({ColumnID}, {ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID}) " +
                                               "VALUES (@ID,@IDPos,@IDEcommerce,@Quantity,@AccountID) " +
                                               $"ON DUPLICATE KEY UPDATE {ColumnQuantity}=@QuantityUpdate", MySqlConnection);

                    cmd.Parameters.AddWithValue("@ID", objFromDatabase.ID);
                    cmd.Parameters.AddWithValue("@IDPos", objFromDatabase.PosID);
                    cmd.Parameters.AddWithValue("@IDEcommerce", objFromDatabase.EcommerceID);
                    cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
                    cmd.Parameters.AddWithValue("@QuantityUpdate", obj.Quantity);
                    cmd.Parameters.AddWithValue("@AccountID", obj.POS.AccountID);
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

        public bool AddNewProduct(Object obj)
        {
            throw new NotImplementedException();
        }

        private Object GetObjectByAccountIDAndID(Object obj)
        {
            var sql = $"SELECT {ColumnID},{ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID} " +
                      $"FROM {TableStockName} " +
                      $"WHERE {ColumnIDPos}=@ObjectID AND {ColumnAccountID}=@AccountID";
            var cmd = new MySqlCommand(sql, MySqlConnection);

            cmd.Parameters.AddWithValue("@ObjectID", obj.PosID);
            cmd.Parameters.AddWithValue("@AccountID", obj.POS.AccountID);

            var rdr = cmd.ExecuteReader();
            Object objFound = null;

            if (rdr.Read())
            {
                //Object found
                objFound = new Object()
                {
                    ID = rdr[0].ToString(),
                    PosID = rdr[1].ToString(),
                    EcommerceID = rdr[2].ToString(),
                    Quantity = int.Parse(rdr[3].ToString()),
                };
            }
            rdr.Close();
            return objFound;
        }
    }
}