using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Bottle Return Service
    /// </summary>
    public class BottleReturnService : SqlDbService, IBottleReturnService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;


        #region CSCMater services

        /// <summary>
        /// Get Bottle Returns to display on Main Grid
        /// </summary>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>List of bottles</returns>
        public List<BottleReturn> GetBottlesFromDbMaster(int startIndex, int endIndex)
        {
            var dt = GetRecords("SELECT *  FROM   HotButtons  WHERE  HotButtons.Button > " + (startIndex - 1) + " " + " AND  HotButtons.Button <" + (endIndex + 1) + "ORDER BY HotButtons.Button ", DataSource.CSCMaster);
            var bottleReturns = new List<BottleReturn>();
            foreach (DataRow dr in dt.Rows)
            {
                var bottleReturn = new BottleReturn
                {
                    Product = CommonUtility.GetStringValue(dr["Product"]),
                    Image_Url = CommonUtility.GetStringValue(dr["Image_Url"]),
                    Description = $"{CommonUtility.GetStringValue(dr["Description_1"])} {CommonUtility.GetStringValue(dr["Description_2"])}",
                    Price = CommonUtility.GetFloatValue(dr["BottlePrice"]),
                    Quantity = CommonUtility.GetFloatValue(dr["DefaultQty"])

                };
                bottleReturns.Add(bottleReturn);
            }
            return bottleReturns;
        }

        #endregion

        #region CSCTrans services

        /// <summary>
        /// Method to save bottle return
        /// </summary>
        /// <param name="brPayment">Bottle return payment</param>
        public void SaveBottleReturnsToDbTrans(BR_Payment brPayment)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
          
            foreach (BottleReturn bottleReturn in brPayment.Br_Lines)
            {
                _dataTable = new DataTable();
                var query = "select * from BottleReturn where TILL_NUM=" + brPayment.TillNumber + " AND SALE_NO=" + brPayment.Sale_Num + " AND LINE_NUM=" + bottleReturn.LineNumber;
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count == 0)
                {
                    DataRow fields = _dataTable.NewRow();
                    fields["TILL_NUM"] = brPayment.TillNumber;
                    fields["sale_no"] = brPayment.Sale_Num;
                    fields["Line_Num"] = bottleReturn.LineNumber;
                    fields["Product"] = bottleReturn.Product;
                    fields["Quantity"] = bottleReturn.Quantity;
                    fields["price"] = bottleReturn.Price;
                    fields["Amount"] = bottleReturn.Amount;
                    _dataTable.Rows.Add(fields);
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    _dataTable.Rows[0]["Product"] = bottleReturn.Product;
                    _dataTable.Rows[0]["Quantity"] = bottleReturn.Quantity;
                    _dataTable.Rows[0]["price"] = bottleReturn.Price;
                    _dataTable.Rows[0]["Amount"] = bottleReturn.Amount;
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }                
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }

        }

        #endregion
    }
}
