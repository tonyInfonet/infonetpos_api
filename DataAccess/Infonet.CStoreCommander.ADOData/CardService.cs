using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Infonet.CStoreCommander.ADOData
{
    public class CardService : SqlDbService, ICardService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;


        #region CSCTills services

        /// <summary>
        /// Method to add card profile prompt
        /// </summary>
        /// <param name="cardProfilePrompt">Card profile promt</param>
        public void AddCardProfilePromptToDbTills(CardProfilePrompt cardProfilePrompt)
        {
            if (cardProfilePrompt != null)
            {
                var query = "SELECT * FROM CardProfilePrompts where Sale_No = " + Convert.ToString(cardProfilePrompt.SaleNumber) + " and TILL_NUM=" + cardProfilePrompt.TillNumber;

                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();

                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                DataRow fields = _dataTable.NewRow();
                fields["Till_Num"] = cardProfilePrompt.TillNumber;
                fields["Sale_No"] = cardProfilePrompt.SaleNumber;
                fields["CardNum"] = cardProfilePrompt.CardNumber;
                fields["ProfileID"] = cardProfilePrompt.ProfileID;
                fields["PromptAnswer"] = cardProfilePrompt.PromptAnswer;
                fields["PromptID"] = cardProfilePrompt.PromptID;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to add card sale
        /// </summary>
        /// <param name="cardSale">Card sale</param>
        public void AddCardSaleToDbTills(CardSale cardSale)
        {
            if (cardSale != null)
            {
                var query = "select * from CardSales where TILL_NUM=" + cardSale.TillNumber;
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();

                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                DataRow fields = _dataTable.NewRow();
                fields["TILL_NUM"] = cardSale.TillNumber;
                fields["SaleAmount"] = cardSale.SaleAmount;
                fields["CardBalance"] = cardSale.CardBalance;
                fields["CardNum"] = cardSale.CardNum;
                fields["CardType"] = cardSale.CardType;
                fields["ExpDate"] = cardSale.ExpiryDate;
                fields["LINE_NUM"] = cardSale.LineNumber;
                fields["PointBalance"] = cardSale.PointBalance;
                fields["RefNum"] = cardSale.ReferenceNumber;
                fields["SALE_NO"] = cardSale.SaleNumber;
                fields["SaleType"] = cardSale.SaleType;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to delete card process
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        public void DeleteCardProcessFromDbTill(int tillNumber)
        {
            Execute("Delete  FROM CardProcess where TILL_NUM=" + tillNumber, DataSource.CSCTills);
        }

        /// <summary>
        /// Method to get list of card process
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of card process</returns>
        public List<CardProcess> GetCardProcessFromDbTIll(int tillNumber)
        {
            var cardProcess = new List<CardProcess>();
            var dt = GetRecords("select * from CardProcess where TILL_NUM=" + tillNumber, DataSource.CSCTills);
            foreach (DataRow dr in dt.Rows)
            {
                cardProcess.Add(new CardProcess
                {
                    TILL_NUM = CommonUtility.GetByteValue(dr["TILL_NUM"]),
                    Amount = CommonUtility.GetDecimalValue(dr["Amount"]),
                    CardProductCode = CommonUtility.GetStringValue(dr["CardProductCode"]),
                    Discount = CommonUtility.GetDoubleValue(dr["Discount"]),
                    Fuel = CommonUtility.GetBooleanValue(dr["Fuel"]),
                    FuelMeasure = CommonUtility.GetStringValue(dr["FuelMeasure"]),
                    FuelServiceType = CommonUtility.GetStringValue(dr["FuelServiceType"]),
                    Qty = CommonUtility.GetFloatValue(dr["Oty"]),
                    SaleTax = CommonUtility.GetDecimalValue(dr["SaleTax"])
                });
            }
            return cardProcess;
        }

        /// <summary>
        /// Method to add card process
        /// </summary>
        /// <param name="cardProcess">Card process</param>
        public void AddCardProcess(CardProcess cardProcess)
        {
            var query = "select * from CardProcess";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            DataRow fields = _dataTable.NewRow();

            fields["TILL_NUM"] = cardProcess.TILL_NUM;
            fields["FuelMeasure"] = cardProcess.FuelMeasure;
            fields["FuelServiceType"] = cardProcess.FuelServiceType;
            fields["CardProductCode"] = cardProcess.CardProductCode;
            fields["Qty"] = cardProcess.Qty;
            fields["Amount"] = cardProcess.Amount;
            fields["Discount"] = cardProcess.Discount;
            fields["SaleTax"] = cardProcess.SaleTax;
            fields["Fuel"] = cardProcess.Fuel;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to update card process
        /// </summary>
        /// <param name="cardProcess">Card process</param>
        public void UpdateCardProcess(CardProcess cardProcess)
        {
            var query = "select * from CardProcess where CardProductCode = " + cardProcess.CardProductCode;
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            DataRow fields = _dataTable.NewRow();
            fields["Qty"] = cardProcess.Qty;
            fields["Amount"] = cardProcess.Amount;
            fields["Discount"] = cardProcess.Discount;
            fields["SaleTax"] = cardProcess.SaleTax;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        #endregion

        #region CSCCurSale services

        /// <summary>
        /// Method to get card profile prompts
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of card profile promts</returns>
        public List<CardProfilePrompt> GetCardProfilePromptFromDbTemp(int tillNumber, int saleNumber)
        {
            var cardProfilePrompts = new List<CardProfilePrompt>();
            var dt = GetRecords("select * from CardProfilePrompts where TILL_NUM=" + tillNumber + " AND SALE_NO = " + Convert.ToString(saleNumber), DataSource.CSCCurSale);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cardProfilePrompts.Add(new CardProfilePrompt
                    {
                        TillNumber = CommonUtility.GetIntergerValue(dr["Till_Num"]),
                        SaleNumber = CommonUtility.GetIntergerValue(dr["Sale_No"]),
                        CardNumber = CommonUtility.GetStringValue(dr["CardNum"]),
                        ProfileID = CommonUtility.GetStringValue(dr["ProfileID"]),
                        PromptAnswer = CommonUtility.GetStringValue(dr["PromptAnswer"]),
                        PromptID = CommonUtility.GetShortValue(dr["PromptID"])
                    });
                }
            }
            return cardProfilePrompts;
        }

        /// <summary>
        /// Method to get card sales
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of card sales</returns>
        public List<CardSale> GetCardSalesFromDbTemp(int tillNumber, int saleNumber)
        {
            var cardSales = new List<CardSale>();
            var dt = GetRecords("select * from CardSales where TILL_NUM=" +
                tillNumber + " AND SALE_NO = " + Convert.ToString(saleNumber),
                DataSource.CSCCurSale);
            foreach (DataRow dr in dt.Rows)
            {
                cardSales.Add(new CardSale
                {
                    TillNumber = CommonUtility.GetIntergerValue(dr["TILL_NUM"]),
                    SaleAmount = CommonUtility.GetDecimalValue(dr["SaleAmount"]),
                    CardBalance = CommonUtility.GetDecimalValue(dr["CardBalance"]),
                    CardNum = CommonUtility.GetStringValue(dr["CardNum"]),
                    CardType = CommonUtility.GetStringValue(dr["CardType"]),
                    ExpiryDate = CommonUtility.GetStringValue(dr["ExpDate"]),
                    LineNumber = CommonUtility.GetIntergerValue(dr["LINE_NUM"]),
                    PointBalance = CommonUtility.GetDecimalValue(dr["PointBalance"]),
                    ReferenceNumber = CommonUtility.GetStringValue(dr["RefNum"]),
                    SaleNumber = CommonUtility.GetIntergerValue(dr["SALE_NO"]),
                    SaleType = CommonUtility.GetStringValue(dr["SaleType"])
                });
            }
            return cardSales;
        }

        #endregion

        #region CSCMaster

        /// <summary>
        /// Method to get maximum fuel id from opt data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Maximum fuel ID</returns>
        public short GetMaxFuelId(string optDataProfileId)
        {
            var dt = GetRecords("SELECT ID AS MaxIDFuel FROM OptData WHERE ProfileID=\'" + optDataProfileId
                + "\' AND OrderNO= (SELECT MAX(OrderNo) FROM OptData WHERE ProfileID=\'" + optDataProfileId
                + "\' AND left(ID,1)=\'1\' )", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count != 0)
            {
                return CommonUtility.GetShortValue(dt.Rows[0]["MaxIDFuel"]);
            }

            return 0;
        }

        /// <summary>
        /// Method to get maximum non fuel id from opt data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Maximum non fuel ID</returns>
        public short GetMaxNonFuelId(string optDataProfileId)
        {
            var dt = GetRecords("SELECT ID AS MaxIDNonFuel FROM OptData WHERE ProfileID=\'" + optDataProfileId
                + "\' AND OrderNO= (SELECT MAX(OrderNo) FROM OptData WHERE ProfileID=\'" + optDataProfileId
                + "\' AND left(ID,1)=\'3\' )", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count != 0)
            {
                return CommonUtility.GetShortValue(dt.Rows[0]["MaxIDNonFuel"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get optional code
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Data code</returns>
        public string GetOptionalDataCode(string optDataProfileId)
        {
            var dt = GetRecords("SELECT Code FROM OptData WHERE (ID=\'020\' OR ID=\'022\') AND ProfileID=\'"
                + optDataProfileId + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count != 0)
            {
                return CommonUtility.GetStringValue(dt.Rows[0]["Code"]);
            }

            return string.Empty;
        }

        /// <summary>
        /// Method to get optional datas
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile id</param>
        /// <returns>List of optional data</returns>
        public List<OptData> GetOptionalDatas(string optDataProfileId)
        {
            var optionalDatas = new List<OptData>();
            var dt = GetRecords("SELECT * FROM OptData WHERE ProfileID=\'" + optDataProfileId + "\' ORDER BY OrderNo",
                DataSource.CSCMaster);
            foreach (DataRow dr in dt.Rows)
            {
                optionalDatas.Add(new OptData
                {
                    Alignment = CommonUtility.GetStringValue(dr["Alignment"]),
                    CharPad = CommonUtility.GetStringValue(dr["CharPad"]),
                    Code = CommonUtility.GetStringValue(dr["Code"]),
                    Decimals = CommonUtility.GetIntergerValue(dr["Decimals"]),
                    Description = CommonUtility.GetStringValue(dr["Description"]),
                    EndPos = CommonUtility.GetIntergerValue(dr["EndPos"]),
                    Format = CommonUtility.GetStringValue(dr["Format"]),
                    ID = CommonUtility.GetStringValue(dr["ID"]),
                    Length = CommonUtility.GetIntergerValue(dr["Length"]),
                    OrderNo = CommonUtility.GetIntergerValue(dr["OrderNo"]),
                    Padded = CommonUtility.GetBooleanValue(dr["Padded"]),
                    ProfileID = CommonUtility.GetStringValue(dr["ProfileID"]),
                    StartPos = CommonUtility.GetIntergerValue(dr["StartPos"])
                });
            }
            return optionalDatas;
        }

        /// <summary>
        /// Method to get prompt sequence
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile Id</param>
        /// <returns>Prompt sequence</returns>
        public short? GetPromptSeq(string optDataProfileId)
        {
            var dt = GetRecords("SELECT PromptSeq FROM CardProfilePrompts AS A INNER JOIN OptData  AS B "
                + "ON A.OptDataID= B.ID WHERE B.ProfileID=\'" + optDataProfileId + "\' AND ID=\'003\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count != 0)
            {
                return CommonUtility.GetShortValue(dt.Rows[0]["PromptSeq"]);
            }
            return null;
        }

        /// <summary>
        /// Nethod to get count of non fuel optional data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Non fuel data count</returns>
        public short GetTotalNonFuelOptionalData(string optDataProfileId)
        {
            var dt = GetRecords("SELECT COUNT(*) AS Tot FROM OptData WHERE (ID=\'121\' OR ID=\'301\') AND ProfileID=\'"
                + optDataProfileId + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count != 0)
            {
                return CommonUtility.GetShortValue(dt.Rows[0]["Tot"]);
            }

            return 0;
        }

        /// <summary>
        /// Method to get count of total optional data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Total optional data</returns>
        public short GetTotalOptionalData(string optDataProfileId)
        {
            var dt = GetRecords("SELECT COUNT(*) AS Tot FROM OptData WHERE ID=\'101\' AND ProfileID=\'"
               + optDataProfileId + "\'", DataSource.CSCMaster);

            if (dt != null && dt.Rows.Count != 0)
            {
                return CommonUtility.GetShortValue(dt.Rows[0]["Tot"]);
            }

            return 0;
        }

        /// <summary>
        /// Method to get product code
        /// </summary>
        /// <param name="profileId">Profile Id</param>
        /// <param name="stockCode">Stock code></param>
        /// <returns>Product code</returns>
        public string Get_ProductCode(string profileId, string stockCode)
        {
            string returnValue;

            var dt = GetRecords("SELECT ExtractionCode FROM ProductExtract WHERE ProfileID=\'" + profileId + "\' AND StockCode=\'"
                                      + stockCode + "\'", DataSource.CSCMaster);

            if (dt == null || dt.Rows.Count == 0)
            {
                returnValue = "99";
            }
            else
            {
                returnValue = CommonUtility.GetStringValue(dt.Rows[0]["ExtractionCode"]);
            }

            return returnValue;
        }

        /// <summary>
        /// Method to check whether this is existing card
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>True or false</returns>
        public bool IsExistingClientCard(string cardNumber)
        {
            bool returnValue = false;

            var dt = GetRecords("SELECT [CardNum] from [dbo].[ClientCard] WHERE CardNum=\'" + cardNumber + "\'", DataSource.CSCMaster);

            if (dt != null && dt.Rows.Count != 0)
            {
                returnValue = true;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get message code
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Message code</returns>
        public string GetMessageCode(string cardNumber)
        {
            var dt = GetRecords("SELECT *  FROM negcardstbl  WHERE negcardstbl.Cardnumber = \'" +
                cardNumber + "\'", DataSource.CSCMaster);

            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Empty;
            }
            return CommonUtility.GetStringValue(dt.Rows[0]["messagecode"]);
        }

        /// <summary>
        /// Method to get message
        /// </summary>
        /// <param name="messageCode">Message code</param>
        /// <returns>Message</returns>
        public string GetMessage(string messageCode)
        {

            var dt = GetRecords("SELECT message FROM cardmsgtbl WHERE  cardmsgtbl.code = " +
                 messageCode, DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return messageCode;
            }
            return CommonUtility.GetStringValue(dt.Rows[0]["Message"]);
        }

        /// <summary>
        /// Method to get list of cards
        /// </summary>
        /// <returns>List of cards</returns>
        public List<Card> GetCards()
        {
            var dt = GetRecords("SELECT * FROM Cards ORDER BY CardCode", DataSource.CSCMaster);
            List<Card> cards = new List<Card>();
            foreach (DataRow dr in dt.Rows)
            {
                var myCard = new Card();
                if (!DBNull.Value.Equals(dr["CardCode"]))
                {
                    myCard.AllowPayment = CommonUtility.GetBooleanValue(dr["AllowPayment"]);
                    myCard.CardID = CommonUtility.GetIntergerValue(dr["CardCode"]);
                    myCard.CardType = CommonUtility.GetStringValue(dr["CardType"]);
                    myCard.CheckDigitMask = CommonUtility.GetStringValue(dr["CheckMask"]);
                    myCard.CheckEncoding = CommonUtility.GetBooleanValue(dr["Encode"]);
                    myCard.CheckExpiryDate = CommonUtility.GetBooleanValue(dr["Expiry"]);
                    myCard.LanguageDigitPosition = CommonUtility.GetShortValue(dr["LanguageDigit"]);
                    myCard.MaxLength = CommonUtility.GetShortValue(dr["MaxLength"]);
                    myCard.MinLength = CommonUtility.GetShortValue(dr["MinLength"]);
                    myCard.Name = CommonUtility.GetStringValue(dr["Description"]);
                    myCard.SearchNegativeCardFile = CommonUtility.GetBooleanValue(dr["Negative"]);
                    myCard.SearchPositiveCardFile = CommonUtility.GetBooleanValue(dr["Positive"]);
                    myCard.VerifyCheckDigit = CommonUtility.GetBooleanValue(dr["VerifyCheckDigit"]);
                    myCard.PromptIDlength = CommonUtility.GetShortValue(dr["PromptIDlength"]);
                    myCard.PromptIDPosition = CommonUtility.GetShortValue(dr["PromptIDPosition"]);
                    myCard.UsageIDLength = CommonUtility.GetShortValue(dr["UsageIDLength"]);
                    myCard.UsageIDPosition = CommonUtility.GetShortValue(dr["UsageIDPosition"]);
                    myCard.DefaultPromptCode = CommonUtility.GetStringValue(dr["DefaultPromptCode"]);
                    myCard.DefaultUsageCode = CommonUtility.GetStringValue(dr["DefaultUsageCode"]);
                    myCard.UserEnteredRestriction = Convert.ToBoolean(dr["UserEnteredRestriction"]);
                    myCard.FuelServiceType = CommonUtility.GetStringValue(dr["ServiceTypeCode"]);
                    myCard.ProductPerTransaction = CommonUtility.GetShortValue(dr["ProducttPerTransaction"]);
                    myCard.VerifyThirdPartySoftware = CommonUtility.GetBooleanValue(dr["VerifyThirdPartySW"]);
                    myCard.GiftType = CommonUtility.GetStringValue(dr["GiftType"]);
                    myCard.EncryptCard = CommonUtility.GetBooleanValue(dr["EncryptCardNumber"]); //                                                                                                             // into the CardCodes collection in the CardType object MyCard
                    myCard.VerifyCardNumber = CommonUtility.GetBooleanValue(dr["VerifyCardNumber"]); //  
                }
                cards.Add(myCard);
            }

            foreach (var card in cards)
            {
                if (card.CardType == "F" && card.GiftType == "W")
                {
                    card.SearchNegativeCardFile = false;
                    card.SearchPositiveCardFile = false;
                    card.VerifyCardNumber = false;
                }

            }

            return cards;
        }

        /// <summary>
        /// Method to get list of card codes
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <returns>List of card code</returns>
        public List<CardCode> GetCardCodes(int cardCode)
        {
            var dt = GetRecords("SELECT *  FROM CardCodes  WHERE CardCodes.CardCode = " + cardCode + " ORDER BY CardCodes.LowerLimit ", DataSource.CSCMaster);
            var cardCodes = new List<CardCode>();
            foreach (DataRow dr in dt.Rows)
            {
                cardCodes.Add(new CardCode
                {
                    LowerLimit = CommonUtility.GetStringValue(dr["LowerLimit"]),
                    UpperLimit = CommonUtility.GetStringValue(dr["UpperLimit"])
                });
            }
            return cardCodes;
        }

        /// <summary>
        /// Method to get card profile restrications
        /// </summary>
        /// <param name="profileId">Profile Id</param>
        /// <returns>Card profile</returns>
        public CardProfile GetCardRestrProfiles(string profileId)
        {
            var dt = GetRecords("Select * from CardRestrProfiles where ProfileID = \'" + profileId + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                var cardResProfiles = new CardProfile();
                cardResProfiles.ProfileName = CommonUtility.GetStringValue(dt.Rows[0]["ProfileName"]);
                cardResProfiles.SngleTransaction = CommonUtility.GetDoubleValue(dt.Rows[0]["SngleTransaction"]);
                cardResProfiles.DailyTransaction = CommonUtility.GetDoubleValue(dt.Rows[0]["DailyTransaction"]);
                cardResProfiles.MonthlyTransaction = CommonUtility.GetDoubleValue(dt.Rows[0]["MonthlyTransaction"]);
                cardResProfiles.TransactionsPerDay = CommonUtility.GetIntergerValue(dt.Rows[0]["TransactionsPerDay"]);
                cardResProfiles.AskForPO = CommonUtility.GetBooleanValue(dt.Rows[0]["AskForPO"]);
                cardResProfiles.LimitTimeofPurchase = CommonUtility.GetBooleanValue(dt.Rows[0]["LimitTimeofPurchase"]);
                cardResProfiles.RestrictProducts = CommonUtility.GetBooleanValue(dt.Rows[0]["RestrictProducts"]);

                return cardResProfiles;
            }
            return null;
        }

        /// <summary>
        /// Method to get list of card prompt
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <returns>List of card prompt</returns>
        public List<CardPrompt> GetCardPrompts(string profileId)
        {
            var dt = GetRecords("SELECT MaxLength, MinLength,PromptMessage, PromptSeq, CardProfilePrompts.PromptID FROM  CardProfilePrompts INNER JOIN cardfuelprompts ON CardProfilePrompts.PromptID = cardfuelprompts.PromptID AND CardProfilePrompts.Type = cardfuelprompts.Type WHERE CardProfilePrompts.Type=\'F\' AND profileid = \'" + profileId + "\' ORDER BY PromptSeq ", DataSource.CSCMaster);
            var cardProfiles = new List<CardPrompt>();
            foreach (DataRow dr in dt.Rows)
            {
                cardProfiles.Add(new CardPrompt
                {
                    MaxLength = CommonUtility.GetShortValue(dr["MaxLength"]),
                    MinLength = CommonUtility.GetShortValue(dr["MinLength"]),
                    PromptMessage = CommonUtility.GetStringValue(dr["PromptMessage"]),
                    PromptSeq = CommonUtility.GetByteValue(dr["PromptSeq"]),
                    PromptID = CommonUtility.GetShortValue(dr["PromptID"])
                });
            }
            return cardProfiles;
        }

        /// <summary>
        /// Method to get card profile limit
        /// </summary>
        /// <param name="profileId">profile id</param>
        /// <param name="dayOfWeek">Day of week</param>
        /// <returns>Card profile time limit</returns>
        public CardProfileTimeLimit GetCardProfileTimeLimit(string profileId, byte dayOfWeek)
        {
            var dt = GetRecords("Select * from CardProfileTimeLimit where  profileID =\'" + profileId + "\' and dayofweek = " + Convert.ToString(dayOfWeek), DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return new CardProfileTimeLimit
                {
                    AllowPurchase = CommonUtility.GetBooleanValue(dt.Rows[0]["AllowPurchase"]),
                    TimeRestriction = CommonUtility.GetBooleanValue(dt.Rows[0]["Timerestriction"]),
                    EndTime = CommonUtility.GetDateTimeValue(dt.Rows[0]["EndTime"]),
                    StartTime = CommonUtility.GetDateTimeValue(dt.Rows[0]["starttime"])
                };

            }
            return null;
        }

        /// <summary>
        /// Method to find if card product restriction exists
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <param name="all">Constant value</param>
        /// <param name="saleLine">Sale line</param>
        /// <returns>True or false</returns>
        public bool IsCardProductRestriction(string profileId, string all, Sale_Line saleLine)
        {
            var dt = GetRecords("SELECT * FROM CardProductRestriction Where ProfileID =\'" + profileId + "\' " +
                " and ((dept = \'" + all + "\' and subdept =\'" + all + "\' and subdetail = \'" +
                all + "\' and stockcode = \'" + saleLine.Stock_Code + "\') or " +
                " ( dept = \'" + saleLine.Dept + "\'  and subdept = \'" + saleLine.Sub_Dept + "\' and subdetail = \'" +
                saleLine.Sub_Detail + "\') or  ( dept = \'" + saleLine.Dept + "\'  and subdept = \'" + saleLine.Sub_Dept +
                "\' and subdetail = \'" + all + "\') or  ( dept = \'" + saleLine.Dept + "\'  and subdept = \'"
                + all + "\' and subdetail = \'" + all + "\'))", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to get amount tendered
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="encryptCardNumber">Encrypt card number</param>
        /// <param name="criteria">Criteria</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Amount entered</returns>
        public double GetAmountTendered(string cardNumber, string encryptCardNumber, string criteria, DataSource dataSource)
        {
            var dt = GetRecords("select sum(amttend)  as saleamt from saletend INNER JOIN SALEHEAD ON Saletend.sale_no = salehead.sale_no and saletend.till_num= salehead.till Where (CCARD_NUM = \'" + cardNumber + "\'  or ccard_num = \'" + encryptCardNumber + "\')" + criteria, dataSource);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetDoubleValue(dt.Rows[0]["saleamt"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get sales count
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="encryptionCardNumber">Encrypted card number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sales count</returns>
        public int GetSalesCount(string cardNumber, string encryptionCardNumber, DataSource dataSource)
        {
            var dt = GetRecords("select count(*)  as salecnt from saletend INNER JOIN SALEHEAD ON Saletend.sale_no = salehead.sale_no and saletend.till_num= salehead.till Where (CCARD_NUM = \'" + cardNumber + "\'  or ccard_num = \'" + encryptionCardNumber + "\') and (T_Type = \'SALE\'  or t_type = \'PATP_APP\') and sale_date = \'" + DateTime.Today.ToString("yyyyMMdd") + "\'", dataSource);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetIntergerValue(dt.Rows[0]["salecnt"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get returns count
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="encryptionCardNumber">Encrypted card number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Returns count</returns>
        public int GetReturnsCount(string cardNumber, string encryptionCardNumber, DataSource dataSource)
        {
            var dt = GetRecords("select count(*)  as Returns from saletend INNER JOIN SALEHEAD ON Saletend.sale_no = salehead.sale_no and saletend.till_num= salehead.till Where (CCARD_NUM = \'" + cardNumber + "\'   or ccard_num = \'" + encryptionCardNumber + "\') and T_Type = \'REFUND\'  and sale_date = \'" + DateTime.Today.ToString("yyyyMMdd") + "\'", dataSource);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetIntergerValue(dt.Rows[0]["Returns"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get postal card pin
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Pin</returns>
        public string GetPostalCardPin(string cardNumber)
        {
            var dt = GetRecords("SELECT * FROM poscardstbl WHERE poscardstbl.Active = 1 AND poscardstbl.Cardnumber = \'" + cardNumber + "\'", DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            return CommonUtility.GetStringValue(dt.Rows[0]["pin"]);
        }

        /// <summary>
        /// Method to get usage type
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <param name="usageCode">Usage code</param>
        /// <returns>Usage type</returns>
        public string GetUsageType(int cardCode, string usageCode)
        {
            var dt = GetRecords("SELECT * FROM CardUsage WHERE  CardUsage.Cardcode =  " + cardCode + "    and UsageID = \'" + usageCode + "\' ", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetStringValue(dt.Rows[0]["usageType"]);
            }

            return string.Empty;
        }

        /// <summary>
        /// Method to get usage type by bank id
        /// </summary>
        /// <param name="bankCardId">Bank card Id</param>
        /// <param name="usageCode">Usage code</param>
        /// <returns>Usage type</returns>
        public string GetUsageType(string bankCardId, string usageCode)
        {
            var dt = GetRecords("Select * from Cardusage where BankCardId=\'" + bankCardId + "\' and UsageID=\'" + usageCode + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetStringValue(dt.Rows[0]["usageType"]);
            }

            return string.Empty;
        }

        /// <summary>
        /// Method to get card prompt
        /// </summary>
        /// <param name="cardId">Card id</param>
        /// <param name="entryMode">Entry mode</param>
        /// <param name="promtCode">Prompt code</param>
        /// <returns>Card</returns>
        public Card GetCardPromptByEntryMode(int cardId, string entryMode, string promtCode)
        {
            var query = $@" SELECT * FROM CardPrompts Where  CardPrompts.CardCode =  '{cardId}'
                 AND (EntryMode = '{entryMode}' OR EntryMode = 'B') AND PromptID = '{promtCode}'";
            var dt = GetRecords(query,
                DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return new Card
                {
                    AskDriverNo = CommonUtility.GetBooleanValue(dt.Rows[0]["DriverNo"]),
                    AskIdentificationNo = CommonUtility.GetBooleanValue(dt.Rows[0]["IdentificationNo"]),
                    AskOdometer = CommonUtility.GetBooleanValue(dt.Rows[0]["Odometer"]),
                    AskVehicle = CommonUtility.GetBooleanValue(dt.Rows[0]["Vechicle"])
                };
            }
            return null;
        }

        /// <summary>
        /// Method to get format answers
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <param name="fieldName">Field name</param>
        /// <returns>Format answers</returns>
        public FormatAnswers GetFormatAnswers(string cardCode, string fieldName)
        {
            var dt = GetRecords("SELECT * FROM FormatAnswers Where FormatAnswers.CardCode = " + cardCode + " AND FieldName=\'" + fieldName + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return new FormatAnswers
                {
                    Justified = CommonUtility.GetStringValue(dt.Rows[0]["Justified"]),
                    CharFilled = CommonUtility.GetCharValue(dt.Rows[0]["CharFilled"]),
                    Length = CommonUtility.GetIntergerValue(dt.Rows[0]["Length"])
                };
            }
            return null;
        }

        /// <summary>
        /// Method to get tend class by card code
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <returns>Tend class</returns>
        public string GetTendClass(int cardCode)
        {
            var dt = GetRecords("SELECT TendClass FROM Cards AS A INNER JOIN TendMast AS B ON A.Description=B.TendDesc WHERE A.CardCode=" +
                Convert.ToString(cardCode), DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Empty;
            }
            return CommonUtility.GetStringValue(dt.Rows[0]["TENDCLASS"]);
        }

        /// <summary>
        /// Method to get tender class by tender code
        /// </summary>
        /// <param name="tendCode">Tender code</param>
        /// <returns>Tender class</returns>
        public string GetTendClassByTendCode(string tendCode)
        {

            var dt = GetRecords("SELECT TendClass FROM TendMast WHERE TendCode=\'" + tendCode + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetStringValue(dt.Rows[0]["TENDCLASS"]);
            }
            return string.Empty;
        }

        /// <summary>
        /// Method to get card code by card type
        /// </summary>
        /// <param name="cardType">Card type</param>
        /// <returns>Credit card</returns>
        public Credit_Card GetCardCode(string cardType)
        {
            var dt = GetRecords("SELECT CardCode, Description FROM Cards WHERE CardType=\'" + cardType + "\'",
                DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            return new Credit_Card
            {
                CardCode = CommonUtility.GetIntergerValue(dt.Rows[0]["CardCode"]),
                Name = CommonUtility.GetStringValue(dt.Rows[0]["Description"])
            };
        }

        /// <summary>
        /// Method to get card product restriction
        /// </summary>
        /// <param name="bankCardId">Bank card id</param>
        /// <param name="dept">Dept</param>
        /// <param name="subDept">Sub dept</param>
        /// <param name="subDetail">Sub detail</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Card product restriction</returns>
        public CardProductRestriction GetCardProductRestriction(string bankCardId, string dept, string subDept,
            string subDetail, string stockCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM CardProductRestriction WHERE BankCardID=\'" + bankCardId + "\'");
            if (!string.IsNullOrEmpty(dept))
            {
                strSql.Append(" AND Dept=\'" + dept + "\'");
            }
            if (!string.IsNullOrEmpty(subDept))
            {
                strSql.Append(" AND SubDept=\'" + subDept + "\'");
            }
            if (!string.IsNullOrEmpty(subDetail))
            {
                strSql.Append(" AND SubDetail=\'" + subDetail + "\'");
            }
            if (!string.IsNullOrEmpty(stockCode))
            {
                strSql.Append(" AND StockCode=\'" + stockCode + "\'");
            }
            var query = strSql.ToString();
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return new CardProductRestriction
                {
                    CardProductCode = CommonUtility.GetStringValue(dt.Rows[0]["CardProductCode"]),
                    Amount = CommonUtility.GetDecimalValue(dt.Rows[0]["Amount"])
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Method to get card product link code
        /// </summary>
        /// <param name="bankCardId">Bank card id</param>
        /// <param name="dept">Dept</param>
        /// <param name="subDept">Sub dept</param>
        /// <param name="subDetail">Sub detail</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Card product link code</returns>
        public string GetCardProductLinkCode(string bankCardId, string dept, string subDept,
           string subDetail, string stockCode)
        {
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append("SELECT * FROM CardProductLink WHERE BankCardID=\'" + bankCardId + "\'");
            if (!string.IsNullOrEmpty(dept))
            {
                stringSql.Append(" AND Dept=\'" + dept + "\'");
            }
            if (!string.IsNullOrEmpty(subDept))
            {
                stringSql.Append(" AND SubDept=\'" + subDept + "\'");
            }
            if (!string.IsNullOrEmpty(subDetail))
            {
                stringSql.Append(" AND SubDetail=\'" + subDetail + "\'");
            }
            if (!string.IsNullOrEmpty(stockCode))
            {
                stringSql.Append(" AND StockCode=\'" + stockCode + "\'");
            }
            var query = stringSql.ToString();
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetStringValue(dt.Rows[0]["CardProductCode"]);
            }
            return null;
        }

        /// <summary>
        /// Method to get card format
        /// </summary>
        /// <returns>Card format</returns>
        public string GetCardFormat()
        {
            var dt = GetRecords("SELECT * FROM CardFormat WHERE CardFormat.[Group]=1", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return true.ToString();
            }
            return "";
        }

        /// <summary>
        /// Method to get list of card products
        /// </summary>
        /// <returns>List of card products</returns>
        public List<CardProduct> GetCardProducts()
        {
            var cardProducts = new List<CardProduct>();
            var dt = GetRecords("select * from CardProducts", DataSource.CSCMaster);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cardProducts.Add(new CardProduct
                    {
                        BankCardID = CommonUtility.GetStringValue(dr["BankCardID"]),
                        CardDescription = CommonUtility.GetStringValue(dr["CardDescription"]),
                        CardProductCode = CommonUtility.GetStringValue(dr["CardProductCode"]),
                        Description = CommonUtility.GetStringValue(dr["Description"]),
                        UsageType = CommonUtility.GetStringValue(dr["UsageType"])
                    });
                }
            }
            return cardProducts;
        }

        /// <summary>
        /// Method to get tender card
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <returns>Tender card</returns>
        public TenderCard LoadTenderCard(int cardCode)
        {
            var dt = GetRecords("Select * From   TendCard Where  TendCard.cardcode =" + Convert.ToString(cardCode), DataSource.CSCMaster);

            if (dt != null && dt.Rows.Count > 0)
            {
                var tenderCard = new TenderCard();
                tenderCard.CardCode = cardCode.ToString();
                tenderCard.TenderCode = Convert.ToString(dt.Rows[0]["TendCode"]);
                tenderCard.ReportGroup = Convert.ToInt16(Information.IsDBNull(dt.Rows[0]["ReportGroup"]) ? 0 : dt.Rows[0]["ReportGroup"]);
                tenderCard.BankCardID = Convert.ToString(dt.Rows[0]["BankCardID"]);
                tenderCard.CallTheBank = Convert.ToBoolean(dt.Rows[0]["Call"]);
                tenderCard.SignatureLine = Convert.ToBoolean(dt.Rows[0]["Signature"]);
                tenderCard.DiscountRate = Convert.ToSingle(dt.Rows[0]["DiscountRate"]);
                tenderCard.DiscountType = Convert.ToString(dt.Rows[0]["DiscountType"]);
                tenderCard.RefundAllowed = Convert.ToBoolean(dt.Rows[0]["RefundAllowed"]);
                tenderCard.PurchaseLimit = Convert.ToSingle(dt.Rows[0]["PurchaseLimit"]);
                tenderCard.FloorLimit = Convert.ToSingle(dt.Rows[0]["FloorLimit"]);
                tenderCard.PrintCopies = Convert.ToInt16(dt.Rows[0]["PrintCopies"]);
                tenderCard.ReceiptTotalText = Convert.ToString(dt.Rows[0]["ReceiptTotalText"]);
                tenderCard.LimitToSale = Convert.ToBoolean(dt.Rows[0]["LimitToSale"]);
                tenderCard.MaxCashBack = Convert.ToSingle(dt.Rows[0]["MaxCashBack"]);
                tenderCard.AllowPayPump = Convert.ToBoolean(dt.Rows[0]["AllowPayPump"]);
                tenderCard.CardProductRestrict = Convert.ToBoolean(dt.Rows[0]["CardProductRestrict"]);
                tenderCard.PrintDriver = Convert.ToBoolean(dt.Rows[0]["PrintDriverNo"]);
                tenderCard.PrintIdentification = Convert.ToBoolean(dt.Rows[0]["PrintIdentificationNo"]);
                tenderCard.PrintOdometer = Convert.ToBoolean(dt.Rows[0]["PrintOdometer"]);
                tenderCard.PrintVechicle = Convert.ToBoolean(dt.Rows[0]["PrintVechicleNo"]);
                tenderCard.PrintUsage = Convert.ToBoolean(dt.Rows[0]["PrintUsage"]);
                tenderCard.OptDataProfileID = Convert.ToString(Information.IsDBNull(dt.Rows[0]["OptDataProfileID"]) ? "" : dt.Rows[0]["OptDataProfileID"]);
                return tenderCard;
            }
            return new TenderCard();
        }

        /// <summary>
        /// Method to get tender card
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Tender card</returns>
        public TenderCard GetTenderCardByTenderCode(string tenderCode)
        {
            var dt = GetRecords("Select * From   TendCard Where  TendCode = '" + Convert.ToString(tenderCode) + "'", DataSource.CSCMaster);

            if (dt != null && dt.Rows.Count > 0)
            {
                var tenderCard = new TenderCard();
                //        Me.TenderCode = CardCode
                tenderCard.CardCode = CommonUtility.GetStringValue(dt.Rows[0]["CardCode"]);
                tenderCard.TenderCode = Convert.ToString(dt.Rows[0]["TendCode"]);
                tenderCard.ReportGroup = Convert.ToInt16(Information.IsDBNull(dt.Rows[0]["ReportGroup"]) ? 0 : dt.Rows[0]["ReportGroup"]);
                tenderCard.BankCardID = Convert.ToString(dt.Rows[0]["BankCardID"]);
                tenderCard.CallTheBank = Convert.ToBoolean(dt.Rows[0]["Call"]);
                tenderCard.SignatureLine = Convert.ToBoolean(dt.Rows[0]["Signature"]);
                tenderCard.DiscountRate = Convert.ToSingle(dt.Rows[0]["DiscountRate"]);
                tenderCard.DiscountType = Convert.ToString(dt.Rows[0]["DiscountType"]);
                tenderCard.RefundAllowed = Convert.ToBoolean(dt.Rows[0]["RefundAllowed"]);
                tenderCard.PurchaseLimit = Convert.ToSingle(dt.Rows[0]["PurchaseLimit"]);
                tenderCard.FloorLimit = Convert.ToSingle(dt.Rows[0]["FloorLimit"]);
                tenderCard.PrintCopies = Convert.ToInt16(dt.Rows[0]["PrintCopies"]);
                tenderCard.ReceiptTotalText = Convert.ToString(dt.Rows[0]["ReceiptTotalText"]);
                tenderCard.LimitToSale = Convert.ToBoolean(dt.Rows[0]["LimitToSale"]);
                tenderCard.MaxCashBack = Convert.ToSingle(dt.Rows[0]["MaxCashBack"]);
                tenderCard.AllowPayPump = Convert.ToBoolean(dt.Rows[0]["AllowPayPump"]);
                tenderCard.CardProductRestrict = Convert.ToBoolean(dt.Rows[0]["CardProductRestrict"]);
                tenderCard.PrintDriver = Convert.ToBoolean(dt.Rows[0]["PrintDriverNo"]);
                tenderCard.PrintIdentification = Convert.ToBoolean(dt.Rows[0]["PrintIdentificationNo"]);
                tenderCard.PrintOdometer = Convert.ToBoolean(dt.Rows[0]["PrintOdometer"]);
                tenderCard.PrintVechicle = Convert.ToBoolean(dt.Rows[0]["PrintVechicleNo"]);
                tenderCard.PrintUsage = Convert.ToBoolean(dt.Rows[0]["PrintUsage"]);
                tenderCard.OptDataProfileID = Convert.ToString(Information.IsDBNull(dt.Rows[0]["OptDataProfileID"]) ? "" : dt.Rows[0]["OptDataProfileID"]);
                return tenderCard;
            }
            return new TenderCard();
        }

        /// <summary>
        /// Method to get list of card prompt
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <param name="profPromptLinkClause">Profile prompt link clause</param>
        /// <returns>List of card prompts</returns>
        public List<CardPrompt> LoadCardPrompts(string profileId)
        {
            var dt = GetRecords("SELECT MaxLength, MinLength, PromptMessage, PromptSeq, A.PromptID  FROM CardProfilePrompts AS A INNER JOIN CardFuelPrompts AS B ON A.PromptID = B.PromptID AND A.Type=B.Type  WHERE A.Type =\'O\' AND ProfileID = \'" + profileId + "\' " + "ORDER BY PromptSeq ", DataSource.CSCMaster);
            //2013 11 08 - Reji - Wex Fleet Card Integration - End
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
            return cardPrompts;
        }

        /// <summary>
        /// Method to get prompt ids
        /// </summary>
        /// <param name="promptCodeStr">Prompt code</param>
        /// <param name="profileId">Profile id</param>
        /// <returns>Prompt id</returns>
        public string GetPromptIds(string promptCodeStr, string profileId)
        {
            string promptIds = string.Empty;
            var dt = GetRecords("Select PromptID from CardProfilePrompts  where PromptID in (select [PromptID] from  CardProfilePromptLink where CardPromptID=\'" + promptCodeStr + "\'  and ProfileID = \'" + profileId + "\')  and ProfileID = \'" + profileId + "\'", DataSource.CSCMaster);
            foreach (DataRow dr in dt.Rows)
            {
                promptIds = "," + Convert.ToString(dr["PromptID"]);
            }
            return promptIds;
        }

        /// <summary>
        /// Method to get map fields
        /// </summary>
        /// <returns>List of mapfields</returns>
        private List<string> GetMapFields()
        {
            var rsTf = GetRecords("SELECT * from CardFormat where Sort=1 order by SortNumber", DataSource.CSCMaster);
            return (from DataRow row in rsTf.Rows select CommonUtility.GetStringValue(row["MapField"])).ToList();
        }

        /// <summary>
        /// Method to create table index
        /// </summary>
        public void CreateTableIndex()
        {
            // Create the indexes based on sort fields in CardFormat table
            var mapFields = GetMapFields();
            var columns = string.Empty;
            Execute("DROP INDEX CardProcess.Sort", DataSource.CSCTills);
            if (mapFields.Count == 0) return;
            columns = mapFields.Aggregate(columns, (current, mapField) => current + (mapField + ","));
            var query = $"CREATE INDEX {"Sort"} ON {"CardProcess"}({columns.Remove(columns.Length - 1, 1)})";
            Execute(query, DataSource.CSCTills);
        }

        /// <summary>
        /// Method to get card process fields
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List if card process fields</returns>
        public List<Field> CardProcessFields(int tillNumber)
        {
            var rsProcess = GetRecords("select * from CardProcess where TILL_NUM=" + tillNumber, DataSource.CSCTills);

            return (from DataColumn column in rsProcess.Columns
                    select new Field
                    {
                        Name = column.ColumnName,
                        Value = column.DataType,
                    }).ToList();
        }

        /// <summary>
        /// Method to get list of string formats
        /// </summary>
        /// <returns>List of string formats</returns>
        public List<StringFormat> GetStringFormat()
        {
            var rsFormat = GetRecords("select * from StringFormat", DataSource.CSCMaster);

            return (from DataRow row in rsFormat.Rows
                    select new StringFormat
                    {
                        MapField = CommonUtility.GetStringValue(row["MapField"]),
                        Format = CommonUtility.GetStringValue(row["Format"]),
                        Sum = CommonUtility.GetBooleanValue(row["Sum"]),
                        NoField = CommonUtility.GetShortValue(row["nofield"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get card process sum
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Card process sum</returns>
        public int GetCardProcessSum(string fieldName, int tillNumber)
        {
            var rsTemp = GetRecords("SELECT SUM( CardProcess." + fieldName + ") as [Suma] FROM CardProcess where TILL_NUM=" + tillNumber, DataSource.CSCTills);
            var sum = CommonUtility.GetIntergerValue(rsTemp.Rows[0]["Suma"]);
            return sum;
        }

        /// <summary>
        /// method to get the card gidt type using the card code
        /// </summary>
        /// <param name="cardCode"></param>
        /// <returns></returns>
        public string GetCradGiftType(int cardCode)
        {
            var rsFormat = GetRecords("select GiftType from Cards where CardCode = "+ cardCode, DataSource.CSCMaster);
            var data = (from DataRow row in rsFormat.Rows select  CommonUtility.GetStringValue(row["GiftType"]));

            if (data != null)
            {
                return data.FirstOrDefault();
            }
            return null;
        }

        #endregion
    }
}
