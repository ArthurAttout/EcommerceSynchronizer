using System;
using System.Collections.Generic;
using System.Data;
using Hangfire.Dashboard;
using MySql.Data.MySqlClient;

namespace EcommerceSynchronizer.Model
{
    public class EcommerceDatabase : IEcommerceDatabase
    {
        private MySqlConnection _mySqlConnection;

        public MySqlConnection MySqlConnection
        {
            get
            {
                if ((_mySqlConnection.State & ConnectionState.Open) == 0) //if not already opened
                    _mySqlConnection.Open();
                return _mySqlConnection;
            }
            set => _mySqlConnection = value;
        }

        public string TableStockName { get; }

        public string ColumnID { get; }
        public string ColumnIDPos { get; }
        public string ColumnIDEcommerce { get; }
        public string ColumnAccountID { get; }
        public string ColumnQuantity { get; }
        public string ColumnDescription { get; set; }
        

        public EcommerceDatabase(string connectionString)
        {
            MySqlConnection = new MySqlConnection(connectionString);
            TableStockName = "stock";
            ColumnIDPos = "idProductPOS";
            ColumnIDEcommerce = "idProductEcommerce";
            ColumnID = "id";
            ColumnQuantity = "quantity";
            ColumnAccountID = "accountID";
            ColumnDescription = "description";
        }

        public void UpdateAllProducts(IList<Object> objects)
        {
            foreach (var obj in objects)
            {
                //Retrieve original object from database
                var objFromDatabase = GetObjectByAccountIDAndID(obj);
                if (objFromDatabase == null) continue;

                var cmd = new MySqlCommand($"INSERT INTO {TableStockName} ({ColumnID}, {ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID},{ColumnDescription}) " +
                                            "VALUES (@ID,@IDPos,@IDEcommerce,@Quantity,@AccountID,@Description) " +
                                            $"ON DUPLICATE KEY UPDATE {ColumnQuantity}=@QuantityUpdate", MySqlConnection);

                cmd.Parameters.AddWithValue("@ID", objFromDatabase.ID);
                cmd.Parameters.AddWithValue("@IDPos", objFromDatabase.PosID);
                cmd.Parameters.AddWithValue("@IDEcommerce", objFromDatabase.EcommerceID);
                cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
                cmd.Parameters.AddWithValue("@QuantityUpdate", obj.Quantity);
                cmd.Parameters.AddWithValue("@AccountID", obj.POS.AccountID);
                cmd.Parameters.AddWithValue("@Description", obj.Name);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            MySqlConnection.Close();
        }

        public bool AddNewProduct(Object obj)
        {
            var cmd = new MySqlCommand(
                $"INSERT INTO {TableStockName} ({ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID},{ColumnDescription}) " +
                "VALUES (@IDPos,@IDEcommerce,@Quantity,@AccountID,@Description) ", MySqlConnection);

            cmd.Parameters.AddWithValue("@IDPos", obj.PosID);
            cmd.Parameters.AddWithValue("@IDEcommerce", obj.EcommerceID);
            cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
            cmd.Parameters.AddWithValue("@AccountID", obj.POS.AccountID);
            cmd.Parameters.AddWithValue("@Description", obj.Name);
            cmd.Prepare();
            return cmd.ExecuteNonQuery() > 0;   
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