using System;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Infonet.CStoreCommander.ADOData
{

    public class KickBackService : SqlDbService, IKickBackService
    {

        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        public KickBackService()
        {


        }


        public void Set_Customer_KickBack_Data(string InputValue, bool isPhoneNumber, string PointCardNumber,
            ref Sale sale, bool doSwipeCheck = false)
        {
            var customer = new Customer();
            //  ADODB.Recordset rsTemp = default(ADODB.Recordset);

            if (isPhoneNumber)
            {

                var query = GetRecords("SELECT CustomerCardNum, PointCardNum FROM KickBack " + "WHERE PhoneNum=\'" + InputValue.Trim() + "\'", DataSource.CSCMaster);


                if (query.Rows.Count > 0)

                {
                    var row = query.Rows[0];
                    sale.Customer.CustomerCardNum = CommonUtility.GetStringValue(row["CustomerCardNum"]);
                    sale.Customer.PointCardNum = CommonUtility.GetStringValue(row["PointCardNum"]);
                    sale.Customer.PointCardPhone = InputValue.Trim();
                    sale.Customer.PointCardSwipe = doSwipeCheck ? (isPhoneNumber ? "1" : "2") : "1"; //0-from database based on a swiped GK card, 1-from phone number, 2-swiped
                    WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside setkickback 45 cardno value" + sale.Customer.PointCardNum);
                }
                else
                {
                    // if the phone number is not in the database, send to Kick Back to validate
                    sale.Customer.CustomerCardNum = null;
                    sale.Customer.PointCardNum = null;
                    sale.Customer.PointCardPhone = InputValue.Trim();
                    sale.Customer.PointCardSwipe = doSwipeCheck ? (isPhoneNumber ? "1" : "2") : "1"; //0-from database based on a swiped GK card, 1-from phone number, 2-swiped
                }
                sale.Customer.PointsCard_AllowRedemption = false; // no redemption for phone number
            }
            else
            {
                var query = GetRecords("SELECT CustomerCardNum, PhoneNum FROM KickBack " + "WHERE PointCardNum=\'" 
                    + PointCardNumber.Trim() + "\'", DataSource.CSCMaster);

                if (query.Rows.Count > 0)
                {
                    var row = query.Rows[0];
                    sale.Customer.CustomerCardNum = CommonUtility.GetStringValue(row["CustomerCardNum"]);
                    sale.Customer.PointCardNum = PointCardNumber.Trim();
                    sale.Customer.PointCardPhone = CommonUtility.GetStringValue(row["phonenum"]);
                    sale.Customer.PointCardSwipe = doSwipeCheck ? (isPhoneNumber ? "1" : "2") : "1"; //0-from database, 1-from phone number, 2-swiped
                    WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside setkickback 68 cardno value" + sale.Customer.PointCardNum);
                }
                else
                {
                    // if the phone number is not in the database, send to Kick Back to validate
                    sale.Customer.CustomerCardNum = null;
                    sale.Customer.PointCardNum = PointCardNumber.Trim();
                    sale.Customer.PointCardPhone = null;
                    sale.Customer.PointCardSwipe = doSwipeCheck ? (isPhoneNumber ? "1" : "2") : "1";//0-from database, 1-from phone number, 2-swiped
                }

                // Nicolette May 12, 2009: if Kick Back card is swiped then the system should allow redemption
                // redemption is also based on the KickBack response (card should be registered, otherwise no redemption allowed)
                sale.Customer.PointsCard_AllowRedemption = true;
                WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside setkickback 82 cardno value" + sale.Customer.PointCardNum);
            }

        }




        // Nicolette March 04, 2009: Kick Back cards or phone numbers do not identify the customer, the system sends to Kick Back to validate the entry or not
        // Only Gas King cards sets a customer, otherwise is the generic customer or cash sale
        ///    Set rsTemp = Get_Records("SELECT CL_Code FROM ClientCard WHERE CardNum='" & _
        ///'        CardNumber & "' ", dbMaster, adOpenForwardOnly, adLockReadOnly)
        ///    If Not rsTemp.EOF Then
        ///        Code = rsTemp![cl_code]  ' this cause the Customer_Change function to set correct prices, discounts and labels on the form for selected customer
        ///    Else
        ///        Code = ""
        ///    End If
        ///
        ///    Get_CustomerCode = Code
      




        /// <summary>
        /// To get kickback card linked to Gasking card
        /// </summary>
        /// 
        public DataTable GaskingKickback(Sale sale)
        {

            var rsKick = GetRecords("SELECT * FROM KickBack " + " WHERE CustomerCardNum=\'" + sale.Customer.LoyaltyCard + "\'", DataSource.CSCMaster);
            return rsKick;
        }

        public DataTable GetKickbackQueue()
        {

            var rsKick = GetRecords("SELECT * FROM KickBackQueue", DataSource.CSCMaster);
            return rsKick;
        }

        public void DeleteKickbackQueue()
        {

            var affected = Execute("DELETE FROM KickBackQueue", DataSource.CSCMaster);

        }

        public void InsertKickbackQueue(string source)
        {
            var dateStart = DateTime.Now;
            //  Performancelog.Debug($"Start,KickbackService,InsertKickbackQueue,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            var records = Execute("INSERT INTO KickBackQueue VALUES(\'" + source + "\')", DataSource.CSCMaster);

        }



        public void WriteUDPData(string msgStr)
        {
            try
            {
                var logPath = @"C:\APILog\";
                var fileName = logPath + "PosLog_" + DateTime.Today.ToString("MM/dd/yyyy") + ".txt";

                using (StreamWriter fileWriter = new StreamWriter(fileName, true))
                {
                    fileWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + Microsoft.VisualBasic.Strings.Space(3) + msgStr);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
