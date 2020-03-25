using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class CashBonusService: SqlDbService, ICashBonusService
    {
        /// <summary>
        /// Method to get all coins
        /// </summary>
        /// <returns></returns>
        public List<CashBonus> GetCashBonusCoins()
        {
            var dt = GetRecords("Select *  FROM   CBonusDrawTypes", DataSource.CSCMaster);
            var coins = new List<CashBonus>();
            foreach (DataRow dr in dt.Rows)
            {
                coins.Add(new CashBonus
                {
                    CurrencyName = CommonUtility.GetStringValue(dr["Currency_Name"]),
                    Value = CommonUtility.GetDecimalValue(CommonUtility.GetDecimalValue(dr["Value"]).ToString("0.00")),
                    ButtonNumber = CommonUtility.GetIntergerValue(dr["Button_Number"])
                });
            }
            return coins;
        }

       public double CalculateCashBonus(string GroupID, float SaleLitre)
        {

            dynamic returnValue = default(dynamic);
            ADODB.Recordset rschart = new ADODB.Recordset(); // use using instead
            double BonusRate = 0;
            double PrevRate = 0;
            short PrevLitre = 0;
            bool blRangeFound = false;

            BonusRate = 0;
            blRangeFound = false;
            rschart = ConvertToRecordset(GetRecords("Select * from CashBonusChart where GroupID = \'" + GroupID + "\' and  IncrementalType = \'>=\' Order by IncrementalQty asc",DataSource.CSCMaster));
            rschart = ConvertToRecordset(GetRecords($"Select * from CashBonusChart where GroupID = \'" + GroupID + "\' and  IncrementalType = \'>=\' Order by IncrementalQty asc",DataSource.CSCMaster));

            rschart.MoveFirst();
            //rschart.AbsolutePosition=rschart.;
            if (!rschart.EOF)
            {
                PrevRate = 0;
                PrevLitre = (short)0;
                while (!rschart.EOF)
                {
                    if (System.Convert.ToDouble(rschart.Fields["IncrementalQty"].Value) <= System.Math.Abs(SaleLitre))
                    {
                        PrevLitre = System.Convert.ToInt16(rschart.Fields["IncrementalQty"].Value);
                        PrevRate = System.Convert.ToDouble(rschart.Fields["BonusRate"].Value);
                    }
                    else //we found a range
                    {
                        //we need to check that if this is greater than the minimum starting point- we shouldn't give bonus for sales < minimum
                         rschart = ConvertToRecordset(GetRecords("Select min(IncrementalQty) as minqty from CashBonusChart where GroupID = \'" + GroupID + "\' and  IncrementalType = \'>=\' ", DataSource.CSCMaster));
                        if (!rschart.EOF)
                        {
                            if (System.Math.Abs(SaleLitre) < System.Convert.ToDouble((Information.IsDBNull(rschart.Fields["minqty"].Value)) ? 0 : (rschart.Fields["minqty"].Value)))
                            {
                                BonusRate = 0;
                                blRangeFound = true;
                            }
                            else
                            {
                                BonusRate = PrevRate;
                                blRangeFound = true;
                            }
                        }
                    }
                    if (!rschart.EOF)
                    {
                        rschart.MoveNext();
                    }
                }
            }
            else // This is there is no range setup, only 1 setting for incremental value
            {
                blRangeFound = false;
            }
            if (blRangeFound)
            {
                returnValue = BonusRate;
            }
            else // Salelitre is more than the Range specified, need to calculate based on incremental value( use the max rang to claculate the first bonus rate and then gove the incremental rate for remainiing)- for eg 119- rate is4.25 , salelitre is 135 , give 4.25 for first 119 litre, then for the remaining 16 litre give .25 cent for each 7 litre increment ( 4.25 +( 16 mod7) *.25 ) ie total bonus = 4.25 + 2*.25 = 4.75
            {
                BonusRate = PrevRate; // This is for the range

                rschart = ConvertToRecordset(GetRecords("Select * from CashBonusChart where GroupID = \'" + GroupID + "\' and  IncrementalType = \'*\' ",DataSource.CSCMaster));
                if (!rschart.EOF)
                {

                   
                    BonusRate = BonusRate + System.Convert.ToDouble(System.Convert.ToDouble((System.Math.Abs(SaleLitre) - PrevLitre) / (rschart.Fields["IncrementalQty"].Value)) * System.Convert.ToDouble(rschart.Fields["BonusRate"].Value));
                }
                returnValue = BonusRate;
            }
            rschart = null;
            return returnValue;
        }

        /// <summary>
        /// Method to add cash bonus draw
        /// </summary>
        /// <param name="cashBonusDraw">Cash draw</param>
        public void AddCashBonusDraw(CashBonusDraw cashBonusDraw)
        {
           var _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
           var _dataTable = new DataTable();
           var _adapter = new SqlDataAdapter("select * from CashDraw where TILL=" + cashBonusDraw.TillNumber, _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Draw_Date"] = cashBonusDraw.DrawDate;
            fields["User"] = cashBonusDraw.User;
            fields["Till"] = cashBonusDraw.TillNumber;
            fields["Reason"] = cashBonusDraw.Reason;
            fields["CashBonus"] =cashBonusDraw.CashBonus;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }
    }
}
