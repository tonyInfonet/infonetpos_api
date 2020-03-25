using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class PaymentSourceService : SqlDbService, IPaymentSourceService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        
        public bool DownloadFile()
        {
            
            bool bDownloadSuccess = false;
            var dateStart = DateTime.Now;
            XElement xe = null;
            XDocument doc = null;
            string GroupNumber = null;
            string ProductVersion = null;
            string EffectiveDate = null;
            _performancelog.Debug($"Start,PaymentSourceService,DownloadFile,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            try
            {
                PSProfile pspf = GetPSProfile();
                doc = new XDocument(
                       new XDeclaration("1.0", "UTF-8", "yes"),
                       new XElement(
                         "RequestXml",
                         new XElement("Type", "FileDownload"),
                         new XElement("TerminalId", pspf.TerminalId),
                         new XElement("Password", pspf.PSpwd),
                         new XElement("Language", "eng"),
                         new XElement("FileType", "1")

                             )
                );
                HttpWebRequest req;
                req = (HttpWebRequest)WebRequest.Create(pspf.URL+doc.ToString());
                req.ContentType = "application/xml";
                req.Method = "GET";
                HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                req.CachePolicy = noCachePolicy;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebResponse resp;
                resp = (HttpWebResponse)req.GetResponse();

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    _performancelog.Error("PaymentSourceService.DownloadFile(): Web request failed.");
                    return false;
                }
                    
                    Stream responseStream = resp.GetResponseStream();
                    doc = XDocument.Load(responseStream);
                    
                    xe = GetElement(doc, "string");
                    string prods = xe.Value.Replace("&amp;lt;", "<");
                    prods = prods.Replace("&amp;gt;", ">");
                    prods = prods.Replace("&lt;", "<");
                    prods = prods.Replace("&gt;", ">");
                    doc = XDocument.Parse(prods);
                    xe = GetElement(doc, "FileData");
                    doc = XDocument.Parse(xe.Value);
                    xe = GetElement(doc, "GroupNumber");
                    if (xe != null)
                        GroupNumber = xe.Value;
                    xe = GetElement(doc, "ProductVersion");
                    if (xe != null)
                        ProductVersion = xe.Value;
                    xe = GetElement(doc, "EffectiveDate");

                    if (xe != null)
                        EffectiveDate = xe.Value;

                    EffectiveDate = EffectiveDate.Replace(".", "/");



                

                

                //read version and groupnumber from db
                var dt = GetRecords("select * from [dbo].[PSProfile]", DataSource.CSCMaster);

                
                
               
                
                

                if (GroupNumber!=dt.Rows[0]["GroupNumber"].ToString() || ProductVersion!=dt.Rows[0]["ProductVersion"].ToString())
                {
                    RepopulatePSProducts(doc);
                    RepopulatePSLogos(doc);
                    LinkLogoToVoucher();
                    Execute("update PSProfile set GroupNumber='" 
                           + GroupNumber 
                           + "', ProductVersion='" 
                           + ProductVersion 
                           + "', EffectiveDate='"
                           + EffectiveDate+"'", DataSource.CSCMaster);
                }

                //read products to populate stock tables
                string sSQL = "select v1.* "
                           + "from [dbo].[PSProducts] v1 "
                           + "left join [dbo].[STOCKMST] v2 on v2.STOCK_CODE=v1.UpcNumber "
                           + "where v2.STOCK_CODE is null ";
                var dtProducts = GetRecords(sSQL, DataSource.CSCMaster);

                PopulateStock(dtProducts);

                bDownloadSuccess = true;

            }
            catch(Exception ex)
            {
                _performancelog.Error("PaymentSourceService.DownloadFile(): "+ex.Message);
            }
            _performancelog.Debug($"End,PaymentSourceService,DownloadFile,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return bDownloadSuccess;
        }
        private void PopulateStock(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return;
            string sTableName;
            
            SqlDataAdapter oDA;
            
            DataRow tdr = null;
            DateTime oDatetime = DateTime.Now;

            using (SqlConnection oCon = new SqlConnection(GetConnectionString(DataSource.CSCMaster)))
            {
                DataTable dtSchema_STOCKMST = SqlDbService.GetTableSchema(oCon.ConnectionString, "CSCMaster.dbo.STOCKMST");
                DataTable oDT_STOCKMST = SqlDbService.GetTableTemplate("CSCMaster.dbo.STOCKMST", oCon.ConnectionString);
                DataTable dtSchema_StockPrice = SqlDbService.GetTableSchema(oCon.ConnectionString, "CSCMaster.dbo.Stock_Prices");
                DataTable oDT_StockPrice = SqlDbService.GetTableTemplate("CSCMaster.dbo.Stock_Prices", oCon.ConnectionString);
                DataTable dtSchema_STOCK_BR = SqlDbService.GetTableSchema(oCon.ConnectionString, "CSCMaster.dbo.STOCK_BR");
                DataTable oDT_STOCK_BR = SqlDbService.GetTableTemplate("CSCMaster.dbo.STOCK_BR", oCon.ConnectionString);
                DataTable dtSchema_PLUMAST = SqlDbService.GetTableSchema(oCon.ConnectionString, "CSCMaster.dbo.PLUMAST");
                DataTable oDT_PLUMAST = SqlDbService.GetTableTemplate("CSCMaster.dbo.PLUMAST", oCon.ConnectionString);
                oCon.Open();
                SqlTransaction trans = oCon.BeginTransaction();
                try
                {
                    //CSCMaster.dbo.STOCKMST
                    sTableName = "CSCMaster.dbo.STOCKMST";
                    oDA = SqlDbService.CreateInsertSqlDataAdapter(sTableName, oCon, dtSchema_STOCKMST);
                    oDA.InsertCommand.Transaction = trans;
                    //dbSchema = SqlDbService.GetTableSchema(oCon.ConnectionString, sTableName);
                    //oDT = SqlDbService.GetTableTemplate(sTableName, oCon.ConnectionString);

                    foreach(DataRow sdr in dt.Rows)
                    {
                        tdr = oDT_STOCKMST.NewRow();
                        SqlDbService.InitializeRow(tdr, dtSchema_STOCKMST);
                        tdr["STOCK_CODE"] = sdr["UpcNumber"];
                        tdr["DESCRIPT"] = sdr["Name"];
                        tdr["STOCK_TYPE"] = "V";
                        tdr["Vendor"] = "ALL";
                        tdr["DEPT"] = "40";
                        tdr["PR_TYPE"] = "R";
                        tdr["PR_UNITS"] = "$";
                        tdr["Availability"] = 1;
                        tdr["CreationDate"] = oDatetime;
                        tdr["UpdateDate"] = oDatetime;
                        tdr["Packing"] = 1;
                        tdr["ElgLoyalty"] = 1;
                        tdr["PROD_DISC"] = 0;
                        tdr["STD_COST"] = 0;
                        tdr["AVG_COST"] = 0;
                        tdr["AVG_UNITS"] = 0;
                        tdr["LATE_COST"] = 0;
                        tdr["SUB_DEPT"] = "";
                        tdr["Sub_Detail"] = "";
                        tdr["SERIAL"] = 0;
                        tdr["S_BY_WGHT"] = 0;
                        tdr["QualTaxRebate"] = 0;
                        tdr["ElgTaxRebate"] = 0;
                        tdr["ElgTaxExemption"] = 0;
                        oDT_STOCKMST.Rows.Add(tdr);
                    }
                    oDA.Update(oDT_STOCKMST);

                    //CSCMaster.dbo.Stock_Prices
                    sTableName = "CSCMaster.dbo.Stock_Prices";
                    oDA = SqlDbService.CreateInsertSqlDataAdapter(sTableName, trans.Connection, dtSchema_StockPrice);
                    oDA.InsertCommand.Transaction = trans;
                   
                    foreach (DataRow sdr in dt.Rows)
                    {
                        tdr = oDT_StockPrice.NewRow();
                        SqlDbService.InitializeRow(tdr, dtSchema_StockPrice);
                        tdr["Stock_Code"] = sdr["UpcNumber"];
                        tdr["Price_Number"] = 1;
                        tdr["Price"] = double.Parse(sdr["Amount"].ToString());
                        tdr["VendorID"] = "ALL";
                        tdr["OrderNo"] = 0;
                        tdr["Margin"] = 0;
                        tdr["UpdateDate"] = oDatetime;
                        oDT_StockPrice.Rows.Add(tdr);
                    }
                    oDA.Update(oDT_StockPrice);

                    //CSCMaster.dbo.STOCK_BR

                    sTableName = "CSCMaster.dbo.STOCK_BR";
                    oDA = SqlDbService.CreateInsertSqlDataAdapter(sTableName, trans.Connection, dtSchema_STOCK_BR);
                    oDA.InsertCommand.Transaction = trans;
                   
                    foreach (DataRow sdr in dt.Rows)
                    {
                        tdr = oDT_STOCK_BR.NewRow();
                        SqlDbService.InitializeRow(tdr, dtSchema_STOCK_BR);
                        tdr["STOCK_CODE"] = sdr["UpcNumber"];
                        tdr["BRANCH"] = "01";
                        tdr["IN_STOCK"] = -7;
                        tdr["AVAIL"] = -7;
                        tdr["ONORDER_IC"] = 0;
                        tdr["ORDCOST_IC"] = 0;
                        tdr["ONORDER_PO"] = 0;
                        tdr["ALLOC_LAY"] = 0;
                        tdr["ALLOC_OE"] = 0;
                        tdr["RETURNED"] = 0;
                        tdr["HOLD"] = 0;
                        tdr["WASTE"] = 0;
                        tdr["STOCK_TAKE"] = 0;
                        tdr["TAKE_ENTER"] = 0;
                        tdr["MIN_QTY"] = 0;
                        tdr["MAX_QTY"] = 0;
                        tdr["REORD_QTY"] = 0;
                        tdr["LAST_SALE"] = DateTime.Now.ToShortDateString();
                        tdr["HOLD"] = 0;


                        oDT_STOCK_BR.Rows.Add(tdr);
                    }
                    oDA.Update(oDT_STOCK_BR);


                    //CSCMaster.dbo.Stock_Prices
                    sTableName = "CSCMaster.dbo.PLUMAST";
                    oDA = SqlDbService.CreateInsertSqlDataAdapter(sTableName, trans.Connection,dtSchema_PLUMAST);
                    oDA.InsertCommand.Transaction = trans;
                   
                    foreach (DataRow sdr in dt.Rows)
                    {
                        tdr = oDT_PLUMAST.NewRow();
                        SqlDbService.InitializeRow(tdr, dtSchema_PLUMAST);
                        tdr["PLU_CODE"] = sdr["UpcNumber"];
                        tdr["PLU_PRIM"] = sdr["UpcNumber"];
                        tdr["PLU_TYPE"] = "S";
                        oDT_PLUMAST.Rows.Add(tdr);
                    }
                    oDA.Update(oDT_PLUMAST);

                    trans.Commit();
                    oDA.Dispose();
                    oCon.Close();
                    oCon.Dispose();

                }
                catch(Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }
        private void RepopulatePSLogos(XDocument doc)
        {
            try
            {
                //truncate logos
                Execute("truncate table [dbo].[PSLogos]", DataSource.CSCMaster);
                //Get PSLogo schema
                DataTable dt = GetDBRecords("select * from PSLogos where 0=1", DataSource.CSCMaster);

                var images = from c in doc.Root.Descendants("Graphics")
                             select c;
                string filename = string.Empty;
                string index = string.Empty;
                foreach (var c in images.Elements())
                {
                    if (c.Name.ToString().ToUpper() == "BMAP")
                    {
                        filename = Path.GetFileName(c.Value);
                        index = c.Attribute("index").Value;
                        //DataRow[] drs = dt.Select("logoFileName='" + filename + "'");
                        //if (drs.Length > 0)
                        //{
                        //    filename = null;
                        //    continue;
                        //}
                            
                    }
                    else if(c.Name.ToString().ToUpper() == "PICTURE" && filename!=null)
                    {

                        DataRow dr = dt.NewRow();
                        dr["BMAP"] = int.Parse(index);
                        dr["logoFileName"] = filename;
                        dr["imagestring"] = c.Value;
                        dt.Rows.Add(dr);

                    }
                }
                BatchInsert("select * from PSLogos where 0=1", dt, DataSource.CSCMaster);
            }
            catch(Exception ex)
            {
                _performancelog.Error("PaymentSourceService.RepopulatePSLogos(): " + ex.Message);
            }
        }
        private void LinkLogoToVoucher()
        {
            try
            {

                Execute("truncate table [dbo].[PSVoucherLogo]", DataSource.CSCMaster);
                DataTable dt = GetDBRecords("select * from PSVoucherLogo where 0=1", DataSource.CSCMaster);
                DataTable dtLogos = GetDBRecords("select BMAP from PSLogos", DataSource.CSCMaster);
                DataTable dtVouchers = GetDBRecords("select * from PSVouchers where Voucher like '%BMAP=%'", DataSource.CSCMaster);
                if (dtLogos.Rows.Count == 0 || dtVouchers.Rows.Count == 0)
                    return;
                List<int> indexes = new List<int>();
                foreach (DataRow logodr in dtLogos.Rows)
                {
                    indexes.Add((int)logodr[0]);
                }
                foreach (DataRow vdr in dtVouchers.Rows)
                {
                    string voucher = vdr["Voucher"].ToString();
                    List<int> myindexes = GetBMAPIndexes(indexes, voucher);
                    foreach(int i in myindexes)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ProdName"] = vdr["ProdName"];
                        dr["BMAP"] = i;
                        dt.Rows.Add(dr);
                    }
                }
                if(dt.Rows.Count>0)
                    BatchInsert("select * from PSVoucherLogo where 0=1", dt, DataSource.CSCMaster);

            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.LinkLogoToVoucher(): " + ex.Message);
            }
        }
        private void RepopulatePSProducts(XDocument doc)
        {
            try
            {
                
                //Get PSProducts schema
                DataTable dt = GetDBRecords("select * from PSProducts where 0=1", DataSource.CSCMaster);
                //Get PSVouchers schema
                DataTable dtVouchers = GetDBRecords("select * from PSVouchers where 0=1", DataSource.CSCMaster);
                string x = string.Empty;
                var categorys = from c in doc.Root.Descendants("Category")
                                select c;
                foreach(var ct in categorys)
                {
                    foreach (var prod in ct.Elements())
                    {
                        
                        foreach (var pd in prod.Elements())
                        {
                            if (pd.Name.LocalName != "Voucher" && pd.Attribute("UpcNumber").Value != "")
                            {
                                DataRow dr = dt.NewRow();
                                dr["CategoryName"] = ct.Attribute("Name").Value;
                                dr["Name"] = prod.Attribute("Name").Value;
                                dr["ProductCode"] = prod.Attribute("ProductCode").Value;
                                dr["AmountLimit"] = prod.Attribute("AmountLimit").Value;
                                dr["StoreGUI"] = prod.Attribute("StoreGUI").Value;
                                dr["Description"] = prod.Attribute("Description").Value;
                                dr["Amount"] = pd.Attribute("Value").Value;
                                dr["AmtDisplay"] = pd.Attribute("Display").Value;
                                dr["UpcNumber"] = pd.Attribute("UpcNumber").Value;
                                DataRow[] drtest = dt.Select("UpcNumber='" + pd.Attribute("UpcNumber").Value + "'");
                                if (drtest.Length == 0)
                                    dt.Rows.Add(dr);
                                else
                                     x = "Tell me.";
                            }
                            else
                            {
                                if(pd.Name.LocalName == "Voucher")
                                {
                                    
                                    DataRow drv = dtVouchers.NewRow();
                                    drv["ProdName"] = prod.Attribute("Name").Value;
                                    drv["Lines"] = int.Parse(pd.Attribute("Lines").Value);
                                    drv["Voucher"] = pd.Value;
                                    DataRow[] drtest1 = dtVouchers.Select("ProdName='" + prod.Attribute("Name").Value + "'");
                                    if(drtest1.Length==0)
                                      dtVouchers.Rows.Add(drv);
                                    else
                                        x = "Tell me.";
                                }
                            }
                        }
                    }
                }
                //truncate products
                Execute("truncate table [dbo].[PSProducts]", DataSource.CSCMaster);
                //truncate PSVouchers
                Execute("truncate table [dbo].[PSVouchers]", DataSource.CSCMaster);
                BatchInsert("select * from PSProducts where 0=1", dt, DataSource.CSCMaster);
                BatchInsert("select * from PSVouchers where 0=1", dtVouchers, DataSource.CSCMaster);
            }
            catch(Exception ex)
            {
                _performancelog.Error("PaymentSourceService.RepopulatePSProducts(): " + ex.Message);
            }
            

        }
        private List<int> GetBMAPIndexes(List<int> BMAPs, string Voucher)
        {

            List<int> olist = new List<int>();
            List<string> clist = new List<string>();
            var bmaps = from c in BMAPs
                        select new { BMAP = "{BMAP=" + c + "}", index = c };

            foreach(var c in bmaps)
            {
                if(Voucher.IndexOf(c.BMAP)>=0)
                {
                    olist.Add(c.index);
                }
            }

            
            
            return olist;
        }
        private XElement GetElement(XDocument doc, string elementName)
        {
            foreach (XNode node in doc.DescendantNodes())
            {
                if (node is XElement)
                {
                    XElement element = (XElement)node;
                    if (element.Name.LocalName.Equals(elementName))
                        return element;
                }
            }
            return null;
        }

        public List<PSProduct> GetPSProducts()
        {
            List<PSProduct> olist = null;
            try
            {
                //read PSproducts from db
                string sSQL = "select v2.* "
                            + "from [dbo].[PSProducts] v2 "
                            + "inner join [dbo].[STOCKMST] v1 on v2.UpcNumber=v1.STOCK_CODE ";
                var dt = GetRecords(sSQL, DataSource.CSCMaster);
                if (dt.Rows.Count == 0)
                    return olist;
                olist = new List<PSProduct>();
                PSProduct pd = null;
                foreach(DataRow dr in dt.Rows)
                {
                    pd = new PSProduct();
                    pd.UpcNumber = dr["UpcNumber"].ToString();
                    pd.Name = dr["Name"].ToString();
                    pd.ProductCode = dr["ProductCode"].ToString();
                    pd.AmountLimit = dr["AmountLimit"].ToString();
                    pd.StoreGUI = dr["StoreGUI"].ToString();
                    pd.Description = dr["Description"].ToString();
                    pd.Amount = dr["Amount"].ToString();
                    pd.AmtDisplay = dr["AmtDisplay"].ToString();
                    pd.CategoryName = dr["CategoryName"].ToString();
                    olist.Add(pd);
                }
            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSProducts(): " + ex.Message);
            }
            return olist;
        }

        public PSProfile GetPSProfile()
        {
            PSProfile pspf = null;
            try
            {
                var dt = GetRecords("select * from [dbo].[PSProfile]", DataSource.CSCMaster);
                if (dt.Rows.Count == 0)
                    return null;
                pspf = new PSProfile();
                pspf.GroupNumber = dt.Rows[0]["GroupNumber"].ToString();
                pspf.ProductVersion = dt.Rows[0]["ProductVersion"].ToString();
                pspf.TerminalId = dt.Rows[0]["TerminalId"].ToString();
                pspf.EffectiveDate = (DateTime)dt.Rows[0]["EffectiveDate"];
                pspf.PSpwd = dt.Rows[0]["PSpwd"].ToString();
                pspf.MID = dt.Rows[0]["MID"].ToString();
                pspf.URL = dt.Rows[0]["URL"].ToString();

            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSProfile(): " + ex.Message);
            }
            return pspf;
        }

        public PSVoucher GetPSVoucherInfo(string ProdName)
        {
            PSVoucher psvc = null;
            try
            {
                var dt = GetRecords("select * from [dbo].[PSVouchers] where ProdName='"+ProdName+"'", DataSource.CSCMaster);
                if (dt.Rows.Count == 0)
                    return psvc;
                psvc = new PSVoucher();
                psvc.ProdName = dt.Rows[0]["ProdName"].ToString();
                psvc.Voucher = dt.Rows[0]["Voucher"].ToString();
                psvc.Lines = (int)dt.Rows[0]["Lines"];

            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSVoucherInfo(): " + ex.Message);
            }
            return psvc;
        }

        public List<PSLogo> GetPSLogos(string ProdName)
        {
            List<PSLogo> pslg = null;
            try
            {
                string sSQL = "select v1.* "
                            + "from [dbo].[PSLogos] v1 "
                            + "inner join [dbo].[PSVoucherLogo] v2 on v1.BMAP=v2.BMAP "
                            + "where v2.ProdName='" + ProdName + "'";
                var dt = GetRecords(sSQL, DataSource.CSCMaster);
                if (dt.Rows.Count == 0)
                    return null;
                pslg = new List<PSLogo>();
                foreach(DataRow dr in dt.Rows)
                {
                    PSLogo o = new PSLogo();
                    o.BMAP = (int)dr["BMAP"];
                    o.ImageString = dr["ImageString"].ToString();
                    o.ImageFileName = dr["logoFileName"].ToString();
                    pslg.Add(o);
                }
                
            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSLogos(): " + ex.Message);
            }
            return pslg;
        }
        public List<PSTransaction> GetPSTransactions(int PastDays)
        {
            List<PSTransaction> olist = new List<PSTransaction>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select v1.SALE_DATE ");
                sb.Append(", v2.SERIAL_NO as TransactionID ");
                sb.Append(", v2.STOCK_CODE, v2.DESCRIPT ");
                sb.Append(", v2.AMOUNT ");
                sb.Append(" from SALEHEAD v1 ");
                sb.Append(" inner join SALELINE v2 on v1.SALE_NO=v2.SALE_NO ");
                sb.Append(" where DATEDIFF(DAY,v1.SD, GETDATE())<=" + PastDays);
                sb.Append(" and v2.SERIAL_NO<>''");

                DataTable dt = GetRecords(sb.ToString(), DataSource.CSCTills);
                if (dt.Rows.Count == 0)
                    return null;
                PSTransaction pt;

                foreach (DataRow dr in dt.Rows)
                {
                    pt = new PSTransaction();
                    pt.TransactionID = dr["TransactionID"].ToString();
                    pt.SALE_DATE = string.Format("{0:MM-dd-yyyy}", (DateTime)dr["SALE_DATE"]);
                    pt.STOCK_CODE = dr["STOCK_CODE"].ToString();
                    pt.DESCRIPT = dr["DESCRIPT"].ToString();
                    pt.Amount = string.Format("{0:0.00}", (Single)dr["AMOUNT"]);
                    olist.Add(pt);
                }
            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSTransactions(): " + ex.Message);
            }
            return olist;
        }
        public string GetPSTransactionID()
        {
            string TransID = string.Empty;
            try
            {
                TransID = CommonUtility.GetPSTransactionID();
            }
            catch(Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSTransactionID(): " + ex.Message);
            }
            return TransID;
        }

        public List<PSLogo> GetPSLogos()
        {
            List<PSLogo> pslg = null;
            try
            {
                string sSQL = "select v1.* "
                            + "from [dbo].[PSLogos] v1 ";
                           
                var dt = GetRecords(sSQL, DataSource.CSCMaster);
                if (dt.Rows.Count == 0)
                    return null;
                pslg = new List<PSLogo>();
                foreach (DataRow dr in dt.Rows)
                {
                    PSLogo o = new PSLogo();
                    o.BMAP = (int)dr["BMAP"];
                    o.ImageString = dr["ImageString"].ToString();
                    o.ImageFileName = dr["logoFileName"].ToString();
                    pslg.Add(o);
                }

            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSLogos(): " + ex.Message);
            }
            return pslg;
        }

        public PSRefund GetPSRefund(string TransactionID)
        {
            PSRefund psr = null;
            try
            {
                string sSQL = "select * from [dbo].[SALELINE] "
                           + "where SERIAL_NO='" + TransactionID + "'";
                var dt = GetRecords(sSQL, DataSource.CSCTills);
                if(dt.Rows.Count>0)
                {
                    psr = new PSRefund();
                    psr.Amount = string.Format("{0:0.00}", dt.Rows[0]["Amount"]);
                    psr.Name = dt.Rows[0]["DESCRIPT"].ToString();
                    psr.UpcNumber = dt.Rows[0]["STOCK_CODE"].ToString();
                }

            }
            catch (Exception ex)
            {
                _performancelog.Error("PaymentSourceService.GetPSRefund(): " + ex.Message);
            }
            return psr;
        }
    }
}
