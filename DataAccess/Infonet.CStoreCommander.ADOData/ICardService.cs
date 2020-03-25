using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ICardService
    {
        #region CSCCurSale services

        /// <summary>
        /// Method to get card sales
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of card sales</returns>
        List<CardSale> GetCardSalesFromDbTemp(int tillNumber, int saleNumber);

        /// <summary>
        /// Method to get card profile prompts
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of card profile promts</returns>
        List<CardProfilePrompt> GetCardProfilePromptFromDbTemp(int tillNumber, int saleNumber);

        #endregion

        #region CSCTills services

        /// <summary>
        /// Method to add card sale
        /// </summary>
        /// <param name="cardSale">Card sale</param>
        void AddCardSaleToDbTills(CardSale cardSale);

        /// <summary>
        /// Method to add card profile prompt
        /// </summary>
        /// <param name="cardProfilePrompt">Card profile promt</param>
        void AddCardProfilePromptToDbTills(CardProfilePrompt cardProfilePrompt);


        /// <summary>
        /// Method to delete card process
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        void DeleteCardProcessFromDbTill(int tillNumber);


        /// <summary>
        /// Method to get list of card process
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of card process</returns>
        List<CardProcess> GetCardProcessFromDbTIll(int tillNumber);

        #endregion


        #region CSCMaster

        /// <summary>
        /// Method to get maximum fuel id from opt data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Maximum fuel ID</returns>
        short GetMaxFuelId(string optDataProfileId);

        /// <summary>
        /// Method to get maximum non fuel id from opt data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Maximum non fuel ID</returns>
        short GetMaxNonFuelId(string optDataProfileId);

        /// <summary>
        /// Method to get count of total optional data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Total optional data</returns>
        short GetTotalOptionalData(string optDataProfileId);

        /// <summary>
        /// Method to get optional code
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Data code</returns>
        string GetOptionalDataCode(string optDataProfileId);


        /// <summary>
        /// Nethod to get count of non fuel optional data
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile ID</param>
        /// <returns>Non fuel data count</returns>
        short GetTotalNonFuelOptionalData(string optDataProfileId);

        /// <summary>
        /// Method to get optional datas
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile id</param>
        /// <returns>List of optional data</returns>
        List<OptData> GetOptionalDatas(string optDataProfileId);

        /// <summary>
        /// Method to get product code
        /// </summary>
        /// <param name="profileId">Profile Id</param>
        /// <param name="stockCode">Stock code></param>
        /// <returns>Product code</returns>
        string Get_ProductCode(string profileId, string stockCode);

        /// <summary>
        /// Method to get prompt sequence
        /// </summary>
        /// <param name="optDataProfileId">Optional data profile Id</param>
        /// <returns>Prompt sequence</returns>
        short? GetPromptSeq(string optDataProfileId);

        /// <summary>
        /// Method to check whether this is existing card
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>True or false</returns>
        bool IsExistingClientCard(string cardNumber);

        /// <summary>
        /// Method to get message code
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Message code</returns>
        string GetMessageCode(string cardNumber);

        /// <summary>
        /// Method to get message
        /// </summary>
        /// <param name="messageCode">Message code</param>
        /// <returns>Message</returns>
        string GetMessage(string messageCode);

        /// <summary>
        /// Method to get list of cards
        /// </summary>
        /// <returns>List of cards</returns>
        List<Card> GetCards();

        /// <summary>
        /// Method to get list of card codes
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <returns>List of card code</returns>
        List<CardCode> GetCardCodes(int cardCode);

        /// <summary>
        /// Method to get card profile restrications
        /// </summary>
        /// <param name="profileId">Profile Id</param>
        /// <returns>Card profile</returns>
        CardProfile GetCardRestrProfiles(string profileId);

        /// <summary>
        /// Method to get list of card prompt
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <returns>List of card prompt</returns>
        List<CardPrompt> GetCardPrompts(string profileId);

        /// <summary>
        /// Method to get card profile limit
        /// </summary>
        /// <param name="profileId">profile id</param>
        /// <param name="dayOfWeek">Day of week</param>
        /// <returns>Card profile time limit</returns>
        CardProfileTimeLimit GetCardProfileTimeLimit(string profileId, byte dayOfWeek);

        /// <summary>
        /// Method to find if card product restriction exists
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <param name="all">Constant value</param>
        /// <param name="saleLine">Sale line</param>
        /// <returns>True or false</returns>
        bool IsCardProductRestriction(string profileId, string all, Sale_Line saleLine);

        /// <summary>
        /// Method to get amount tendered
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="encryptCardNumber">Encrypt card number</param>
        /// <param name="criteria">Criteria</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Amount entered</returns>
        double GetAmountTendered(string cardNumber, string encryptCardNumber, string criteria, DataSource dataSource);

        /// <summary>
        /// Method to get sales count
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="encryptionCardNumber">Encrypted card number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sales count</returns>
        int GetSalesCount(string cardNumber, string encryptionCardNumber, DataSource dataSource);

        /// <summary>
        /// Method to get returns count
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="encryptionCardNumber">Encrypted card number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Returns count</returns>
        int GetReturnsCount(string cardNumber, string encryptionCardNumber, DataSource dataSource);

        /// <summary>
        /// Method to get postal card pin
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Pin</returns>
        string GetPostalCardPin(string cardNumber);

        /// <summary>
        /// Method to get usage type
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <param name="usageCode">Usage code</param>
        /// <returns>Usage type</returns>
        string GetUsageType(int cardCode, string usageCode);

        /// <summary>
        /// Method to get card prompt
        /// </summary>
        /// <param name="cardId">Card id</param>
        /// <param name="entryMode">Entry mode</param>
        /// <param name="promtCode">Prompt code</param>
        /// <returns>Card</returns>
        Card GetCardPromptByEntryMode(int cardId, string entryMode, string promtCode);

        /// <summary>
        /// Method to get format answers
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <param name="fieldName">Field name</param>
        /// <returns>Format answers</returns>
        FormatAnswers GetFormatAnswers(string cardCode, string fieldName);

        /// <summary>
        /// Method to get tend class by card code
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <returns>Tend class</returns>
        string GetTendClass(int cardCode);

        /// <summary>
        /// Method to get tender class by tender code
        /// </summary>
        /// <param name="tendCode">Tender code</param>
        /// <returns>Tender class</returns>
        string GetTendClassByTendCode(string tendCode);

        /// <summary>
        /// Method to get card code by card type
        /// </summary>
        /// <param name="cardType">Card type</param>
        /// <returns>Credit card</returns>
        Credit_Card GetCardCode(string cardType);

        /// <summary>
        /// Method to get card product restriction
        /// </summary>
        /// <param name="bankCardId">Bank card id</param>
        /// <param name="dept">Dept</param>
        /// <param name="subDept">Sub dept</param>
        /// <param name="subDetail">Sub detail</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Card product restriction</returns>
        CardProductRestriction GetCardProductRestriction(string bankCardId, string dept, string subDept,
            string subDetail, string stockCode);

        /// <summary>
        /// Method to get card product link code
        /// </summary>
        /// <param name="bankCardId">Bank card id</param>
        /// <param name="dept">Dept</param>
        /// <param name="subDept">Sub dept</param>
        /// <param name="subDetail">Sub detail</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Card product link code</returns>
        string GetCardProductLinkCode(string bankCardId, string dept, string subDept,
            string subDetail, string stockCode);

        /// <summary>
        /// Method to get card format
        /// </summary>
        /// <returns>Card format</returns>
        string GetCardFormat();

        /// <summary>
        /// Method to get list of card products
        /// </summary>
        /// <returns>List of card products</returns>
        List<CardProduct> GetCardProducts();

        /// <summary>
        /// Method to add card process
        /// </summary>
        /// <param name="cardProcess">Card process</param>
        void AddCardProcess(CardProcess cardProcess);

        /// <summary>
        /// Method to update card process
        /// </summary>
        /// <param name="cardProcess">Card process</param>
        void UpdateCardProcess(CardProcess cardProcess);

        /// <summary>
        /// Method to get tender card
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <returns>Tender card</returns>
        TenderCard LoadTenderCard(int cardCode);

        /// <summary>
        /// Method to get tender card
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Tender card</returns>
        TenderCard GetTenderCardByTenderCode(string tenderCode);

        /// <summary>
        /// Method to get list of card prompt
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <param name="profPromptLinkClause">Profile prompt link clause</param>
        /// <returns>List of card prompts</returns>
        List<CardPrompt> LoadCardPrompts(string profileId);

        /// <summary>
        /// Method to get prompt ids
        /// </summary>
        /// <param name="promptCodeStr">Prompt code</param>
        /// <param name="profileId">Profile id</param>
        /// <returns>Prompt id</returns>
        string GetPromptIds(string promptCodeStr, string profileId);

        /// <summary>
        /// Method to get usage type by bank id
        /// </summary>
        /// <param name="bankCardId">Bank card Id</param>
        /// <param name="usageCode">Usage code</param>
        /// <returns>Usage type</returns>
        string GetUsageType(string bankCardId, string usageCode);

        /// <summary>
        /// Method to create table index
        /// </summary>
        void CreateTableIndex();

        /// <summary>
        /// Method to get card process fields
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List if card process fields</returns>
        List<Field> CardProcessFields(int tillNumber);

        /// <summary>
        /// Method to get list of string formats
        /// </summary>
        /// <returns>List of string formats</returns>
        List<StringFormat> GetStringFormat();

        /// <summary>
        /// Method to get card process sum
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Card process sum</returns>
        int GetCardProcessSum(string fieldName, int tillNumber);

        /// <summary>
        /// method to get the card gift type using the card code
        /// </summary>
        /// <param name="cardCode"></param>
        /// <returns></returns>
        string GetCradGiftType(int cardCode);

        #endregion
    }
}
