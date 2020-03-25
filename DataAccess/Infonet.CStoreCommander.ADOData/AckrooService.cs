using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.ADOData
{
    public class AckrooService : SqlDbService, IAckrooService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
       public string GetLoyaltyNo(int Sale_No)
        {
            string sVal = null;
            int OldSaleNo;
            DataTable dt;
            try
            {
                string sSQL = "Select Void_No From SALEHEAD WHERE Sale_No = "
                           + Sale_No;

                 dt = GetRecords(sSQL, DataSource.CSCCurSale);
                if (dt.Rows.Count == 0)
                    OldSaleNo = 0;
                else
                    OldSaleNo = int.Parse(dt.Rows[0][0].ToString());

                if(OldSaleNo>0)
                {
                    sSQL = "Select [LoyaltyCard] From SALEHEAD Where Sale_No =" + OldSaleNo;
                    dt = GetRecords(sSQL, DataSource.CSCTills);
                    if (dt.Rows.Count == 0)
                    {
                        dt = GetRecords(sSQL, DataSource.CSCTrans);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        sVal = "";
                    }
                    else
                    {
                        sVal = dt.Rows[0][0].ToString();
                    }

                }
                else
                {
                    sSQL = "Select [LoyaltyCard] From SALEHEAD Where Sale_No =" + Sale_No;
                    dt = GetRecords(sSQL, DataSource.CSCCurSale);
                    if (dt.Rows.Count == 0)
                        sVal = "";
                    else
                        sVal = dt.Rows[0][0].ToString();
                }



            }
            catch (Exception ex)
            {
                _performancelog.Error("AckrooService.GetLoyaltyNo(): " + ex.Message);
            }
            return sVal;
        }
       public string GetAckrooCarwashStockCode(string sDesc)
        {
            string sVal = null;
            try
            {
                string sSQL = "SELECT [P_VALUE] "
                           + "From [CSCAdmin].[dbo].[P_SET] "
                           + " WHERE P_NAME='REWARDS_CARWASH' "
                           + "  AND P_SET = '" + sDesc + "'";
                var dt = GetRecords(sSQL, DataSource.CSCAdmin);

                if (dt == null || dt.Rows.Count == 0)
                    return sVal;

                sVal = dt.Rows[0][0].ToString();
                return sVal;

            }
            catch(Exception ex)
            {
                _performancelog.Error("AckrooService.GetAckrooCarwashStockCode(): " + ex.Message);
            }
            return sVal;
        }
       public List<Carwash> GetCarwashCategories()
        {
            List<Carwash> olist = new List<Carwash>();
            Carwash cw;
            try
            {
                string sSQL = "SELECT UnitId, [Category] FROM [CSCAdmin].[dbo].[Carwash]  "
                           + "order by  UnitId";
                var dt = GetRecords(sSQL, DataSource.CSCAdmin);
                if (dt == null || dt.Rows.Count == 0)
                    return olist;
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    cw = new Carwash();
                    cw.UnitId = int.Parse(dt.Rows[i][0].ToString());
                    cw.Category= dt.Rows[i][1].ToString();
                    olist.Add(cw);
                }

            }
            catch(Exception ex)
            {
                _performancelog.Error("AckrooService.GetCarwashCategories(): " + ex.Message);
            }
            return olist;
        }

        public string GetValidAckrooStock()
        {
            string sVal = "";
            string strResult = "";
            try
            {
                string sSQL = "select * From [CSCAdmin].[dbo].[P_SET] "
                           + "where P_NAME = 'REWARDS_Gift' and P_SET='AckrooGift' ";
                           
                var dt = GetRecords(sSQL, DataSource.CSCAdmin);
                if (dt == null || dt.Rows.Count == 0)
                    return sVal;

                string sNewSQL = sSQL + " and P_LEVEL='STOCK'";
                dt = GetRecords(sSQL, DataSource.CSCAdmin);
                if (dt!=null && dt.Rows.Count>0)
                {
                    sVal= Strings.Trim(Convert.ToString(dt.Rows[0]["P_VALUE"]));
                }
                else
                {
                    sNewSQL = sSQL + " and P_LEVEL=\'SUBDETAIL\'";
                    dt = GetRecords(sSQL, DataSource.CSCAdmin);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        strResult = Strings.Trim(Convert.ToString(dt.Rows[0]["P_VALUE"]));
                    }
                    else
                    {
                        sNewSQL = sSQL + " and P_LEVEL=\'SUBDEPT\'";
                        dt = GetRecords(sSQL, DataSource.CSCAdmin);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            strResult = Strings.Trim(Convert.ToString(dt.Rows[0]["P_VALUE"]));
                        }
                        else
                        {
                            sNewSQL = sSQL + " and P_LEVEL=\'DEPT\'";
                            dt = GetRecords(sSQL, DataSource.CSCAdmin);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                strResult = Strings.Trim(Convert.ToString(dt.Rows[0]["P_VALUE"]));
                            }
                        }
                    }
                }
                if(strResult.Trim()!="")
                {
                    var arr = Strings.Split(Expression: strResult, Delimiter: Convert.ToString(Strings.Chr(255)), Compare: CompareMethod.Text);
                    string strSql = "";
                    var n = (short)(arr.Length - 1);
                    short i;
                    for (i = 0; i <= n; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                var strDept = arr[0];
                                strSql = "select * from STOCKMST where DEPT=\'" + strDept + "\'";
                                break;
                            case 1:
                                var strSubDept = arr[1];
                                strSql = strSql + " and SUB_DEPT=\'" + strSubDept + "\'";
                                break;
                            case 2:
                                var strSubDetail = arr[2];
                                strSql = strSql + " and Sub_Detail=\'" + strSubDetail + "\'";
                                break;
                        }
                    }
                    if (strSql.Trim() != "")
                    {
                        var rsStock = GetRecords(strSql, DataSource.CSCMaster);
                        if (rsStock != null && rsStock.Rows.Count > 0)
                        {
                            sVal = Strings.Trim(Convert.ToString(rsStock.Rows[0]["Stock_Code"]));
                        }
                    }
                }
                

            }
            catch (Exception ex)
            {
                _performancelog.Error("AckrooService.GetValidAckrooStock(): " + ex.Message);
            }
            return sVal;
        }
    }
}
