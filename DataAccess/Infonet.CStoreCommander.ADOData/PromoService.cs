using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class PromoService : SqlDbService, IPromoService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Method to get distinct promoId for today
        /// </summary>
        /// <returns>Promo IDs</returns>
        public List<string> GetDistinctPromoIdsForToday()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PromoService,GetDistinctPromoIdsForToday,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rsHeader = GetRecords("SELECT DISTINCT PromoID FROM PromoHeader  WHERE [PromoHeader].[EndDate]>= \'" +
                                            DateTime.Now.ToString("yyMMdd") + "\' AND [PromoHeader].[StartDate]<= \'" +
                                            DateTime.Now.ToString("yyMMdd") + "\' ORDER BY  PromoID", DataSource.CSCMaster);
            var promoIds = (from DataRow fields in rsHeader.Rows select CommonUtility.GetStringValue(fields["PromoID"])).ToList();
            _performancelog.Debug($"End,PromoService,GetDistinctPromoIdsForToday,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return promoIds;
        }

        ///// <summary>
        ///// Method to get max link
        ///// </summary>
        ///// <param name="promoId"></param>
        ///// <returns>Max link</returns>
        //public short GetMaxLink(string promoId)
        //{
        //    var dateStart = DateTime.Now;
        //    _performancelog.Debug($"Start,PromoService,GetMaxLink,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

        //    var rsTemp = GetRecords("SELECT MAX(Link) AS MaxLink FROM PromoDetail WHERE PromoID=\'" + promoId + "\'", DataSource.CSCMaster);
        //    if (rsTemp.Rows.Count != 0)
        //    {
        //        var fields = rsTemp.Rows[0];
        //        _performancelog.Debug($"End,PromoService,GetMaxLink,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        //        return CommonUtility.GetShortValue(fields["MaxLink"]);
        //    }
        //    _performancelog.Debug($"End,PromoService,GetMaxLink,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        //    return 0;
        //}

        ///// <summary>
        ///// Method to get number of links
        ///// </summary>
        ///// <param name="promoId">Promo id</param>
        ///// <returns>Number of links</returns>
        //public List<int> GetNumberOfLinks(string promoId)
        //{
        //    var dateStart = DateTime.Now;
        //    _performancelog.Debug($"Start,PromoService,GetNumberOfLinks,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

        //    var rsTemp = GetRecords("SELECT SUM(Link)/Link AS NrOfLinks, Link FROM PromoDetail WHERE PromoID=\'" + promoId + "\' GROUP BY Link ORDER BY Link", DataSource.CSCMaster);
        //    var links = (from DataRow fields in rsTemp.Rows select CommonUtility.GetShortValue(fields["NrOfLinks"])).Select(dummy => (int)dummy).ToList();
        //    _performancelog.Debug($"End,PromoService,GetNumberOfLinks,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        //    return links;
        //}

        /// <summary>
        /// Method to get all promos for today
        /// </summary>
        /// <param name="optPromoId">Promo id</param>
        /// <returns>Promo headers</returns>
        public List<Promo> GetPromoHeadersForToday(string optPromoId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PromoService,GetPromoHeadersForToday,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var promos = new List<Promo>();           

            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            SqlCommand cmd = new SqlCommand("GetPromoHeaders", _connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@optPromoId", Value = optPromoId });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@dt", Value = DateTime.Now.ToString("yyyyMMdd") });
            var rdr = cmd.ExecuteReader();

            if (!rdr.HasRows)
            {
                return promos;
            }
            var stockInfo = new Sale_Line();
            while (rdr.Read())
            {
                Promo promo = new Promo();
                promo.PromoID = CommonUtility.GetStringValue(rdr["PromoID"]);
                promo.Description = CommonUtility.GetStringValue(rdr["Description"]);
                promo.DiscType = CommonUtility.GetStringValue(rdr["DiscType"]);
                promo.Amount = CommonUtility.GetDoubleValue(rdr["price"]);
                promo.PrType = CommonUtility.GetStringValue(rdr["PrType"]);
                promo.MaxLink = CommonUtility.GetShortValue(rdr["MaxLink"]);
                promo.MultiLink = CommonUtility.GetIntergerValue(rdr["NrOfLinks"]) > 1 ? true : false;
                promos.Add(promo);
            }

            _performancelog.Debug($"End,PromoService,GetPromoHeadersForToday,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return promos;
        }

        /// <summary>
        /// Method to get promo lines
        /// </summary>
        /// <param name="promoId">Promo id</param>
        /// <param name="none"></param>
        /// <returns>List of promo lines</returns>
        public List<Promo_Line> GetPromoLines(string promoId, string none)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PromoService,GetPromoLines,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var promoLines = new List<Promo_Line>();
            var rsDetail = GetRecords("SELECT * FROM PromoDetail WHERE PromoID=\'" + promoId + "\' ORDER BY Link, Stock_Code, Dept, Sub_Dept, SubDetail", DataSource.CSCMaster);
            foreach (DataRow fields in rsDetail.Rows)
            {
                var promoLine = new Promo_Line
                {
                    Amount = CommonUtility.GetFloatValue(fields["Amount"]),
                    Stock_Code = CommonUtility.GetStringValue(fields["Stock_Code"]),
                    Dept = CommonUtility.GetStringValue(fields["Dept"]),
                    Sub_Dept = CommonUtility.GetStringValue(fields["Sub_Dept"]),
                    Sub_Detail = CommonUtility.GetStringValue(fields["SubDetail"]),
                    Link = CommonUtility.GetByteValue(fields["Link"]),
                    Quantity = CommonUtility.GetFloatValue(fields["Qty"])
                };
                if (CommonUtility.GetStringValue(fields["Stock_Code"]) != none)
                {
                    promoLine.Level = 1;
                }
                else if (CommonUtility.GetStringValue(fields["Dept"]) != none
                    && CommonUtility.GetStringValue(fields["Sub_Dept"]) == none
                    && CommonUtility.GetStringValue(fields["SubDetail"]) == none)
                {
                    promoLine.Level = 2;
                }
                else if (CommonUtility.GetStringValue(fields["Dept"]) != none
                    && CommonUtility.GetStringValue(fields["Sub_Dept"]) != none
                    && CommonUtility.GetStringValue(fields["SubDetail"]) == none)
                {
                    promoLine.Level = 3;
                }
                else if (CommonUtility.GetStringValue(fields["Dept"]) != none
                    && CommonUtility.GetStringValue(fields["Sub_Dept"]) != none
                    && CommonUtility.GetStringValue(fields["SubDetail"]) != none)
                {
                    promoLine.Level = 4;
                }
                promoLines.Add(promoLine);
            }
            _performancelog.Debug($"End,PromoService,GetPromoLines,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return promoLines;
        }

        /// <summary>
        /// Method to get promos for today
        /// </summary>
        /// <returns>List of promos</returns>
        public List<Promo> GetPromosForToday()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PromoService,GetPromosForToday,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            // August 21, 2008 took out the quantity validation to implement mix and match promotions
            var strSql = "SELECT DISTINCT [PromoHeader].[PromoID] AS PromoID, [PromoHeader].[Day] AS Day,[PromoDetail].[Qty] AS Qty, [PromoDetail].[Link] AS Link, [PromoDetail].[Amount] AS Amount,[PromoDetail].[Stock_Code],[PromoDetail].[Dept],[PromoDetail].[Sub_Dept],[PromoDetail].[SubDetail] FROM PromoHeader LEFT JOIN PromoDetail ON [PromoHeader].[PromoID] =[PromoDetail].[PromoID]  WHERE (Day IS NULL OR Day=0 OR Day=" + Convert.ToString(DateAndTime.Weekday(DateAndTime.Today)) + ") AND [PromoHeader].[EndDate]>= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND [PromoHeader].[StartDate]<= \'" + DateTime.Now.ToString("yyyyMMdd") + "\'";
            var rsPromo = GetRecords(strSql, DataSource.CSCMaster);
            var promos = (from DataRow fields in rsPromo.Rows
                          select new Promo
                          {
                              StockCode = CommonUtility.GetStringValue(fields["Stock_Code"]),
                              Dept = CommonUtility.GetStringValue(fields["Dept"]),
                              SubDept = CommonUtility.GetStringValue(fields["Sub_Dept"]),
                              SubDetail = CommonUtility.GetStringValue(fields["SubDetail"]),
                              PromoID = CommonUtility.GetStringValue(fields["PromoID"]),
                              Day = CommonUtility.GetByteValue(fields["Day"]),
                              TotalQty = CommonUtility.GetShortValue(fields["Qty"]),
                              MaxLink = CommonUtility.GetShortValue(fields["Link"]),
                              Amount = CommonUtility.GetDoubleValue(fields["Amount"])
                          }).ToList();
            _performancelog.Debug($"End,PromoService,GetPromosForToday,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return promos;
        }
    }
}
