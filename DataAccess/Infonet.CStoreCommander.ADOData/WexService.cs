using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADODB;
using System.Data.SqlClient;
using System.Data;
using Microsoft.VisualBasic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    // Wex service to perform db related actions for WEX FLEET payment
    public class WexService : SqlDbService ,IWexService
    {
        /// <summary>
        /// method to get the extraction code
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public string[] GetExtractionCode(string stockCode, string profileId)
        {
            var extractedStockCodes = new string[2];

            var query = " select  " + " PE.ExtractionCode as ExtractionCode1, PE2.ExtractionCode as ExtractionCode2 "
                                 + " ,PE3.ExtractionCode as ExtractionCode3, PE4.ExtractionCode as ExtractionCode4 "
                                 + " ,Stock_Code, SM.sub_dept, SM.sub_detail " + " ,PE.StockCode from  dbo.STOCKMST SM " 
                                 + " left  join ProductExtract PE on PE.StockCode = SM.Stock_Code AND ProfileID=\'" + profileId + "\'" 
                                 + " left  join (select * from ProductExtract where ProfileID=\'" 
                                 + profileId + "\' AND StockCode = null or StockCode is null or StockCode= \'<NONE>\') PE2 on PE2.subdetail = SM.sub_detail" 
                                 + " left  join (select * from ProductExtract where ProfileID=\'" 
                                 + profileId + "\' AND  subdetail= null or StockCode is null or subdetail= \'<NONE>\') PE3 on PE3.subdept = SM.sub_dept" 
                                 + " left  join (select * from ProductExtract where ProfileID=\'" 
                                 + profileId + "\' AND  subdept= null or StockCode is null or subdept= \'<NONE>\') PE4 on PE4.dept = SM.dept" 
                                 + " where Stock_Code in (" + stockCode + ")";
            
            var data = GetRecords(query, DataSource.CSCMaster);

            foreach (DataRow dr in data.Rows)
                {
                    if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode1"])))))
                    {
                        extractedStockCodes[0] = System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode1"]));
                    }
                    else if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode2"])))))
                    {
                        extractedStockCodes[0] = System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode1"]));
                    }
                    else if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode3"])))))
                    {
                        extractedStockCodes[0] = System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode3"]));
                    }
                    else if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode4"])))))
                    {
                        extractedStockCodes[0] = System.Convert.ToString(CommonUtility.GetStringValue(dr["ExtractionCode4"]));
                    }
                    else
                    {
                        extractedStockCodes[0] = "400";
                    }
                    extractedStockCodes[1] = System.Convert.ToString(CommonUtility.GetStringValue(dr["Stock_Code"]));
                }
           
            return extractedStockCodes;
        }

        /// <summary>
        /// method to get the wex profile ID
        /// </summary>
        /// <returns></returns>
        public string  GetWexProfileId()
        {
            var profileId = "";
            var query = " select OptDataProfileID from dbo.TendCard CC"
                            + " join dbo.Cards CD ON CC.CardCode = CD.CardCode "
                            + " WHERE CD.CardType=\'F\' AND CD.GiftType = \'W\' ";
            var data =  GetRecords(query, DataSource.CSCMaster);
            foreach (DataRow dt in data.Rows)
            {
                profileId = CommonUtility.GetStringValue(dt["OptDataProfileID"]);
            }
            return profileId;
        }

        /// <summary>
        /// method to get the WEX profile prompts
        /// </summary>
        /// <returns></returns>
        public List<string> GetWexProfilePrompts()
        {
            var profilePrompts = new List<string>();
            var profileId = GetWexProfileId();
            var query = "select PromptID from dbo.CardProfilePrompts where ProfileID =" + profileId;

            var data = GetRecords(query, DataSource.CSCMaster);

            foreach (DataRow dt in data.Rows)
            {
               profilePrompts.Add(CommonUtility.GetStringValue(dt["ProfileID"]));
            }
            return profilePrompts; 
        }

        /// <summary>
        /// method to get the prompts associated with the cards
        /// </summary>
        /// <param name="prompts"></param>
        /// <param name="promptString"></param>
        /// <param name="profileId"></param>
        public void GetCardProfilePrompts(ref CardPrompts prompts, string promptString, string profileId)
        {

            var query = "SELECT MaxLength, MinLength, PromptMessage, PromptSeq, A.PromptID  FROM CardProfilePrompts AS A INNER JOIN CardFuelPrompts AS B ON A.PromptID = B.PromptID AND A.Type=B.Type  WHERE A.Type =\'O\' AND ProfileID = \'" + profileId + "\' " + "AND A.PromptID in ( Select PromptID from CardProfilePrompts " + " where PromptID in (select[PromptID] from " + " CardProfilePromptLink where CardPromptID =\'" + promptString + "\' " + " and ProfileID = \'" + profileId + "\') " + " and ProfileID = \'" + profileId + "\')" + "ORDER BY PromptSeq ";
            
            var dt = GetRecords(query, DataSource.CSCMaster);
            
            var cardPrompts = new List<CardPrompt>();
            foreach (DataRow dr in dt.Rows)
            {
                cardPrompts.Add(new CardPrompt
                {
                    MaxLength = CommonUtility.GetShortValue(dr["MaxLength"]),
                    MinLength = CommonUtility.GetShortValue(dr["MinLength"]),

                    PromptMessage = CommonUtility.GetStringValue(dr["PromptMessage"]),
                    PromptSeq = CommonUtility.GetByteValue(dr["PromptSeq"]),
                    PromptID = CommonUtility.GetShortValue(dr["PromptID"])
                });
            }
            prompts = cardPrompts;
        }
    }
}
