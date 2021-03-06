﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Model.Interfaces;
using Hangfire.Dashboard;
using MySql.Data.MySqlClient;

namespace EcommerceSynchronizer.Utilities
{
    public class EcommerceDatabase : IEcommerceDatabase
    {
        private readonly IPOSInterfaceProvider _posInterfaceProvider;
        private MySqlConnection _mySqlConnection;

        public MySqlConnection MySqlConnection
        {
            get
            {
                if (_mySqlConnection.State != ConnectionState.Open) //if not already opened
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
        public string ColumnLimitQuantity { get; set; }
        
        public EcommerceDatabase(IConfigurationProvider configProvider, IPOSInterfaceProvider posInterfaceProvider)
        {
            _posInterfaceProvider = posInterfaceProvider;
            MySqlConnection = new MySqlConnection(configProvider.GetDatabaseConnectionString());
            TableStockName = "stock";
            ColumnIDPos = "idProductPOS";
            ColumnIDEcommerce = "idProductEcommerce";
            ColumnID = "id";
            ColumnQuantity = "quantity";
            ColumnAccountID = "accountID";
            ColumnDescription = "description";
            ColumnLimitQuantity = "limitQuantity";
        }

        public void UpdateAllProducts(IList<Object> objects)
        {
            foreach (var obj in objects)
            {
                //Retrieve original object from database
                var objFromDatabase = GetObjectByAccountIDAndID(obj.PosID,obj.POS.AccountID);
                if (objFromDatabase == null) continue;

                var cmd = new MySqlCommand($"INSERT INTO {TableStockName} ({ColumnID}, {ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID},{ColumnDescription},{ColumnLimitQuantity}) " +
                                            "VALUES (@ID,@IDPos,@IDEcommerce,@Quantity,@AccountID,@Description,@LimitQuantity) " +
                                            $"ON DUPLICATE KEY UPDATE {ColumnQuantity}=@QuantityUpdate", MySqlConnection);

                cmd.Parameters.AddWithValue("@ID", objFromDatabase.ID);
                cmd.Parameters.AddWithValue("@IDPos", objFromDatabase.PosID);
                cmd.Parameters.AddWithValue("@IDEcommerce", objFromDatabase.EcommerceID);
                cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
                cmd.Parameters.AddWithValue("@QuantityUpdate", obj.Quantity);
                cmd.Parameters.AddWithValue("@AccountID", obj.POS.AccountID);
                cmd.Parameters.AddWithValue("@Description", obj.Name);
                cmd.Parameters.AddWithValue("@LimitQuantity", obj.LimitQuantitySale);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            MySqlConnection.Close();
        }

        public bool UpdateProduct(Object obj)
        {
            var cmd = new MySqlCommand(
                $"UPDATE {TableStockName} SET {ColumnIDPos} = @IDPos ,{ColumnIDEcommerce}=@IDEcommerce ,{ColumnQuantity}=@Quantity,{ColumnAccountID}=@AccountID,{ColumnDescription}=@Description " +
                $"WHERE {ColumnID}=@ID ", MySqlConnection);

            cmd.Parameters.AddWithValue("@ID", obj.ID);
            cmd.Parameters.AddWithValue("@IDPos", obj.PosID);
            cmd.Parameters.AddWithValue("@IDEcommerce", obj.EcommerceID);
            cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
            cmd.Parameters.AddWithValue("@AccountID", obj.POS.AccountID);
            cmd.Parameters.AddWithValue("@Description", obj.Name);
            cmd.Prepare();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool AddNewProduct(Object obj)
        {
            var cmd = new MySqlCommand(
                $"INSERT INTO {TableStockName} ({ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID},{ColumnDescription},{ColumnLimitQuantity}) " +
                "VALUES (@IDPos,@IDEcommerce,@Quantity,@AccountID,@Description,@LimitQuantity) ", MySqlConnection);

            cmd.Parameters.AddWithValue("@IDPos", obj.PosID);
            cmd.Parameters.AddWithValue("@IDEcommerce", obj.EcommerceID);
            cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
            cmd.Parameters.AddWithValue("@AccountID", obj.POS.AccountID);
            cmd.Parameters.AddWithValue("@Description", obj.Name);
            cmd.Parameters.AddWithValue("@LimitQuantity", obj.LimitQuantitySale);
            cmd.Prepare();
            return cmd.ExecuteNonQuery() > 0;   
        }

        public Object GetObjectByAccountIDAndID(string posID, string accountID)
        {
            var sql = $"SELECT {ColumnID},{ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID},{ColumnDescription},{ColumnLimitQuantity} " +
                      $"FROM {TableStockName} " +
                      $"WHERE {ColumnIDPos}=@ObjectID AND {ColumnAccountID}=@AccountID";
            var cmd = new MySqlCommand(sql, MySqlConnection);

            cmd.Parameters.AddWithValue("@ObjectID", posID);
            cmd.Parameters.AddWithValue("@AccountID", accountID);

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
                    POS = _posInterfaceProvider.GetAllInterfaces().FirstOrDefault(i => i.AccountID.Equals(rdr[4])),
                    Name = rdr[5].ToString(),
                    LimitQuantitySale = int.Parse(rdr[6].ToString())
                };
            }
            rdr.Close();
            return objFound;
        }



        public Object GetObjectByEcommerceID(string id)
        {
            var sql = $"SELECT {ColumnID},{ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID},{ColumnDescription},{ColumnLimitQuantity} " +
                      $"FROM {TableStockName} " +
                      $"WHERE {ColumnIDEcommerce}=@IDEcommerce";
            var cmd = new MySqlCommand(sql, MySqlConnection);

            cmd.Parameters.AddWithValue("@IDEcommerce", id);

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
                    POS = _posInterfaceProvider.GetAllInterfaces().FirstOrDefault(i => i.AccountID.Equals(rdr[4])),
                    Name = rdr[5].ToString(),
                    LimitQuantitySale = int.Parse(rdr[6].ToString())
                };
            }
            rdr.Close();
            return objFound;
        }

        public List<Object> GetAllObjectsOfAccountID(string id)
        {
            var sql = $"SELECT {ColumnID},{ColumnIDPos},{ColumnIDEcommerce},{ColumnQuantity},{ColumnAccountID},{ColumnDescription},{ColumnLimitQuantity} " +
                      $"FROM {TableStockName} " +
                      $"WHERE {ColumnAccountID}=@AccountID";
            var cmd = new MySqlCommand(sql, MySqlConnection);

            cmd.Parameters.AddWithValue("@AccountID", id);

            var rdr = cmd.ExecuteReader();
            var arrayObjects = new List<Object>();

            while (rdr.Read())
            {
                //Object found
                var objFound = new Object()
                {
                    ID = rdr[0].ToString(),
                    PosID = rdr[1].ToString(),
                    EcommerceID = rdr[2].ToString(),
                    Quantity = int.Parse(rdr[3].ToString()),
                    POS = _posInterfaceProvider.GetAllInterfaces().FirstOrDefault(i => i.AccountID.Equals(rdr[4])),
                    Name = rdr[5].ToString(),
                    LimitQuantitySale = int.Parse(rdr[6].ToString())
                };
                arrayObjects.Add(objFound);
            }
            rdr.Close();
            return arrayObjects;
        }
    }
}