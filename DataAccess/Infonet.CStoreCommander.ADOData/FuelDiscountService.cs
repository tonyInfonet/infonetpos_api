using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;

namespace Infonet.CStoreCommander.ADOData
{
    public class FuelDiscountService : SqlDbService, IFuelDiscountService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        public List<ClientGroup> GetClientGroups()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelDiscountService,GetCustomerGroups,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var dt = GetRecords("SELECT * FROM ClientGroup", DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                _performancelog.Debug($"End,CustomerService,GetCustomerGroups,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;
            }
            List<ClientGroup> cgs = new List<ClientGroup>();
            ClientGroup cg;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cg = new ClientGroup();
                cg.GroupName = CommonUtility.GetStringValue(dt.Rows[i]["GroupName"]);
                cg.DiscountType = CommonUtility.GetStringValue(dt.Rows[i]["DiscountType"]);
                cg.DiscountRate = CommonUtility.GetFloatValue(dt.Rows[i]["DiscountRate"]);
                cg.Footer = CommonUtility.GetStringValue(dt.Rows[i]["LoyaltyFooter"]);
                cg.DiscountName = CommonUtility.GetStringValue(dt.Rows[i]["DiscountName"]);
                cg.GroupId = CommonUtility.GetStringValue(dt.Rows[i]["GroupID"]);
                cgs.Add(cg);
            }

            _performancelog.Debug($"End,FuelDiscountService,GetCustomerGroups,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return cgs;
        }

        public string GetFuelCodes()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelDiscountService,GetFuelCodes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var rs = GetRecords("select STOCK_CODE from [dbo].[STOCKMST] where DEPT=1", DataSource.CSCMaster);
            if (rs == null || rs.Rows.Count == 0)
                return "";
            string sCodes = "";
            foreach (DataRow dr in rs.Rows)
            {
                sCodes += CommonUtility.GetStringValue(dr[0]) + ",";
            }
            if (sCodes.Length > 0)
                sCodes = sCodes.Substring(0, sCodes.Length - 1);
            _performancelog.Debug($"End,FuelDiscountService,GetFuelCodes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sCodes;
        }
    }
}
