using Infonet.CStoreCommander.Entities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class SaleVendorCouponService : SqlDbService, ISaleVendorCouponService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to add sale vendor coupon line
        /// </summary>
        /// <param name="oLine">Sale vendor coupon line</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        public void AddSaleVendorCouponLine(SaleVendorCouponLine oLine, int saleNumber, int tillNumber)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCCurSale));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from SaleVendorCoupon where TILL_NUM=" + tillNumber
                + " AND SALE_NO=" + Convert.ToString(saleNumber) + " AND LINE_NUM="
                + Convert.ToString(oLine.Line_Num) + "AND SeqNumber=" + Convert.ToString(oLine.SeqNum)
                , _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["Till_Num"] = tillNumber;
                fields["Sale_No"] = saleNumber;
                fields["Line_Num"] = oLine.Line_Num;
                fields["CouponCode"] = oLine.CouponCode;
                fields["CouponName"] = oLine.CouponName;
                fields["UnitValue"] = oLine.UnitValue;
                fields["Quantity"] = oLine.Quantity;
                fields["TotalValue"] = oLine.TotalValue;
                fields["SerialNumber"] = oLine.SerialNumber;
                fields["SeqNumber"] = oLine.SeqNum;
                fields["TendDesc"] = oLine.TendDesc;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to update sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="seqNumber">Sequence number</param>
        /// <param name="serialNumber"></param>
        public void UpdateSaleVendorCoupon(int saleNumber, int tillNumber, int lineNumber, short seqNumber, string serialNumber)
        {
            var query = "select * from SaleVendorCoupon where TILL_NUM=" + tillNumber
                 + " AND SALE_NO=" + Convert.ToString(saleNumber) + " AND LINE_NUM="
                 + Convert.ToString(lineNumber) + "AND SeqNumber=" + Convert.ToString(seqNumber);
            
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCCurSale));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                _dataTable.Rows[0]["SerialNumber"] = serialNumber;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to save sale vendor coupon service
        /// </summary>
        /// <param name="sv">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        public void SaveSaleVendorCoupon(SaleVendorCoupon sv, int tillNumber, DataSource dataSource)
        {
            _connection = new SqlConnection(GetConnectionString(dataSource));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            foreach (SaleVendorCouponLine tempLoopVarSvc in sv.SVC_Lines)
            {
                var svc = tempLoopVarSvc;
                bool addNew = false;

                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("select * from SaleVendorCoupon where TILL_NUM="
                    + tillNumber + " AND SALE_NO=" + Convert.ToString(sv.Sale_Num)
                    + " AND LINE_NUM=" + Convert.ToString(svc.Line_Num) + " AND SeqNumber="
                    + Convert.ToString(svc.SeqNum), _connection);
                _adapter.Fill(_dataTable);
                DataRow fields;
                if (_dataTable.Rows.Count != 0)
                {
                    addNew = true;
                    fields = _dataTable.NewRow();
                    fields["Till_Num"] = tillNumber;
                    fields["Sale_No"] = sv.Sale_Num;
                    fields["Line_Num"] = svc.Line_Num;
                    fields["SeqNumber"] = svc.SeqNum;
                }
                else
                {
                    fields = _dataTable.Rows[0];
                }
                fields["CouponCode"] = svc.CouponCode;
                fields["CouponName"] = svc.CouponName;
                fields["UnitValue"] = svc.UnitValue;
                fields["Quantity"] = svc.Quantity;
                fields["TotalValue"] = svc.TotalValue;
                fields["SerialNumber"] = svc.SerialNumber;
                fields["TendDesc"] = svc.TendDesc;
                if (addNew)
                {
                    _dataTable.Rows.Add(fields);
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
        }
    }
}
