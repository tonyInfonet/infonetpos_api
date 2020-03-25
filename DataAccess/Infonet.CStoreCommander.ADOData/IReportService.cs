using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.IO;
namespace Infonet.CStoreCommander.ADOData
{
    public interface IReportService
    {
        /// <summary>
        /// Method to delete previous count report
        /// </summary>
        void DeletePreviousCountReport();

        /// <summary>
        /// Method to add count report
        /// </summary>
        /// <param name="countReport">Count report</param>
        void AddCountReport(CountReport countReport);

        /// <summary>
        /// Method to get sales count detail
        /// </summary>
        /// <param name="selectClause">Select clause</param>
        /// <param name="department">Department</param>
        /// <param name="shiftDate">ShiftDate</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="groupByClause">Group by clause</param>
        /// <param name="detailLevel">Detail level</param>
        /// <returns>Sale count detail</returns>
        List<CountReport> GetSalesCountDetail(string selectClause, string department, DateTime shiftDate,
            string whereClause, string groupByClause, byte detailLevel);


        /// <summary>
        /// Method to get existing count reports
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="detailLevel">Detail level</param>
        /// <returns>Count reports</returns>
        List<CountReport> GetCountReports(string query, byte detailLevel);

        /// <summary>
        /// Method to get flash report details for till number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        FlashReportTotals GetFlashReportDetails(int tillNumber);

        /// <summary>
        /// Method to get total sales without discount
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Line total</returns>
        decimal GetLineTotal(int tillNumber);

        /// <summary>
        /// Method to get sales by department for flash report
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Departments</returns>
        List<Department> GetDepartmentsForFlashReport(int tillNumber);

        /// <summary>
        /// Method to get fuel sales report
        /// </summary>
        /// <param name="fuelDepartment">Fuel department name</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <returns>Fuel sales</returns>
        List<FuelSale> GetFuelSalesReport(string fuelDepartment, int tillNumber);

        /// <summary>
        /// Method to get change value
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Change</returns>
        decimal GetChangeValue(int tillNumber);

        /// <summary>
        /// Method to get cash draws
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        Till GetCashDraws(int tillNumber);

        /// <summary>
        /// Method to get payouts
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        decimal GetPayouts(int tillNumber);

        /// <summary>
        /// Method to get bonus
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        decimal GetBonusGiveAway(int tillNumber);

        /// <summary>
        /// Method to get drop values
        /// </summary>
        /// <param name="tillNumber">Tillnumber</param>
        /// <returns>List of tenders</returns>
        List<Tender> GetDropValues(int tillNumber);

        /// <summary>
        /// Method to get cash sale values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        List<Tender> GetCashSaleValues(int tillNumber);

        /// <summary>
        /// Method to get payment values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        List<Tender> GetPaymentValues(int tillNumber);

        /// <summary>
        /// Method to get AR payment values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        List<Tender> GetArPaymentValues(int tillNumber);

        /// <summary>
        /// Method to get bottle return values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        List<Tender> GetBottleReturnValues(int tillNumber);

        /// <summary>
        /// Method to get sale totals
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale totals</returns>
        SaleTend GetSaleTotals(int tillNumber); 

        /// <summary>
        /// Method to get non cash currency tender by tender name
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <param name="tenderDescription">Tender description</param>
        /// <returns>Non cash currency names</returns>
        List<string> GetNonCashCurrencyTenders(string currency, string tenderDescription);

        /// <summary>
        /// Method to get non cash tenders
        /// </summary>
        /// <returns>Non cash tenders</returns>
        List<string> GetNonCashTenders();

        /// <summary>
        /// Method to get cash drop by tender name
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Cash drop</returns>
        float? GetCashDropByTenderName(int tillNumber, string tenderName);

        /// <summary>
        /// Method to get sale tender by tender name
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Sale tender</returns>
        float? GetSaleTenderByTenderName(int tillNumber, string tenderName);

        /// <summary>
        /// Method to get list of gift cards
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="reReprint">Is reprint or not</param>
        /// <returns></returns>
        List<GiftCardTender> GetListOfGiftcards(int saleNo, bool reReprint);

        /// <summary>
        /// Method to get card profile prompts
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>CArd profile prompts</returns>
        List<CardProfilePrompt> GetCardProfilePrompts(int saleNumber, string cardNumber, byte tillNumber);

        /// <summary>
        /// Method to get signature
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Image</returns>
        Stream GetSignature(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to verify if sale is available for till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        bool IsSaleAvailableForTill(int tillNumber);

        /// <summary>
        /// Method to find if reader usage is available
        /// </summary>
        /// <returns>True or false</returns>
        bool IsReaderUsageAvailable();

        /// <summary>
        /// Method to get pay at pump credits
        /// </summary>
        /// <returns>List of pump credits</returns>
        List<PayAtPumpCredit> GetPayAtPumpCredits();

        /// <summary>
        /// Method to find if pay at pump sales is available or not
        /// </summary>
        /// <returns>True or false</returns>
        bool IsPayAtPumpSalesAvailable();

        /// <summary>
        /// Method to get payment sales
        /// </summary>
        /// <param name="selectQuery">selct query</param>
        /// <param name="saleType">Sale type</param>
        /// <param name="date">Date</param>
        /// <returns>List of payment type sales</returns>
        List<Sale> GetPaymentSales(string selectQuery, string saleType, DateTime date);

        /// <summary>
        /// Method to get pay inside sales
        /// </summary>
        /// <param name="selectQuery">Select query</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of pay inside sales</returns>
        List<Sale> GetPayInsideSales(string selectQuery, DataSource dataSource);

        /// <summary>
        /// Method to get pay at pump sales
        /// </summary>
        /// <param name="selectQuery">Select query</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of pay at pump sales</returns>
        List<PayAtPumpSale> GetPayAtPumpSales(string selectQuery, DataSource dataSource);

        /// <summary>
        /// Method to get terminal ids
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>List of terminal ids</returns>
        List<string> GetTerminalIds(int posId);

        /// <summary>
        /// Method to get close batch reports
        /// </summary>
        /// <param name="terminalIds">Terminal ids</param>
        /// <param name="dt">Close batch date</param>
        /// <returns>List of close batch</returns>
        List<CloseBatch> GetCloseBatchReports(List<string> terminalIds,
             DateTime dt);

        /// <summary>
        /// Method to get data source of sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="sale">Sale</param>
        /// <returns>Data source</returns>
        DataSource GetSale(int saleNumber, out Sale sale);

        /// <summary>
        /// Method to get list of tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of Tenders</returns>
        List<Tender> GetTenders(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get card number
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>Card number</returns>
        string GetCardNumber(int saleNumber);

        /// <summary>
        /// Method to get vendor code
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Vendor code</returns>
        string GetVendorCode(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get sale taxes
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale taxes</returns>
        List<Sale_Tax> GetSaleTaxes(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get bottle ereturn
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>Bottle return payment</returns>
        BR_Payment GetBottleReturn(int saleNumber);

        /// <summary>
        /// Method to get list of card tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data ource</param>
        /// <returns>List of card tenders</returns>
        List<CardTender> GetCardTenders(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get history message
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>History message</returns>
        string GetHistoryMessage(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get receipt of pay at pump
        /// </summary>
        /// <param name="invoiceNumber">Invoice number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Receipt</returns>
        string GetReceipt(int invoiceNumber, DataSource dataSource);

        /// <summary>
        /// Method to get sale for pay at pump
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="payAtPumpTill">Pay at pump till</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>pay at pump</returns>
        PayAtPump GetSaleForPayAtPump(int saleNumber, int payAtPumpTill,
            DataSource dataSource);

        /// <summary>
        /// Method to get sale lines for pay at pump sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="payAtPumpTill">Pay at pump sale</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        List<Sale_Line> GetSaleLinesForPayAtPump(int saleNumber,
            int payAtPumpTill, DataSource dataSource);

        /// <summary>
        /// Method to get card tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="payAtPumpTill">Pay at pump till</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Card reprint</returns>
        Card_Reprint GetCardTender(int saleNumber, int payAtPumpTill,
            DataSource dataSource);

        /// <summary>
        /// Method to get grade description
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <returns>Grade description</returns>
        string GetGradeDescription(int gradeId);

        /// <summary>
        /// Method to get sale tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale tenders</returns>
        List<SaleTend> GetSaleTenders(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to find whether card tender is available
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tenderName">Tender name</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>True or false</returns>
        bool IsCardTenderAvailable(int saleNumber, string tenderName, DataSource dataSource);

        /// <summary>
        /// Method to find if tender is available
        /// </summary>
        /// <param name="tenderName">Tender name</param>
        /// <returns>True or false</returns>
        bool IsTenderAvailable(string tenderName);

        /// <summary>
        /// Method to get sale head
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale</returns>
        Sale GetSaleHead(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get sale lines
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale lines</returns>
        List<Sale_Line> GetSaleLines(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get line taxes
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Line taxes</returns>
        Line_Taxes GetLineTaxes(int saleNumber, int lineNumber, DataSource dataSource);

        /// <summary>
        /// Method to get charges
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charges</returns>
        Charges GetCharges(int saleNumber, int lineNumber, DataSource dataSource);

        /// <summary>
        /// Method get tax saved
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Tax saved</returns>
        float GetTaxSaved(int saleNumber, int lineNumber, DataSource dataSource);


        /// <summary>
        /// Method to get total exempted tax
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Exempted tax</returns>
        float GetTotalExemptedTax(int saleNumber, int tillNumber,
            DataSource dataSource);

        /// <summary>
        /// Method to get void number
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Void number</returns>
        int GetVoidNo(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to load card sales
        /// </summary>
        /// <param name="db">Data source</param>
        /// <param name="tillId">Till number</param>
        /// <param name="saleNo">Sale number</param>
        /// <param name="lineNo">Line number</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>True or false</returns>
        bool Load_CardSales(DataSource db, short tillId, int saleNo,
            short lineNo, out GiveXReceiptType givexReceipt);

        /// <summary>
        /// Method to get line kits
        /// </summary>
        /// <param name="sn">Serial number</param>
        /// <param name="ln">Line number</param>
        /// <param name="db">Data source</param>
        /// <returns>Line kits</returns>
        Line_Kits Get_Line_Kit(int sn, int ln, DataSource db);

        /// <summary>
        /// Method to get tax exempt sale
        /// </summary>
        /// <param name="sn">Sale number</param>
        /// <param name="tillId">TIll number</param>
        /// <param name="db">Data source</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="checkQuota">Check for quota</param>
        /// <returns>Tax exempt sale</returns>
        TaxExemptSale LoadTaxExempt(int sn, byte tillId, DataSource db,
            string teType, bool checkQuota = true);

        /// <summary>
        /// Method to load gst exempt
        /// </summary>
        /// <param name="sn">Serial number</param>
        /// <param name="tillId">Till number</param>
        /// <param name="db">Data source</param>
        /// <param name="oteTax">Tax exempt sale</param>
        /// <returns>True or false</returns>
        bool LoadGstExempt(int sn, byte tillId, DataSource db,
            ref TaxExemptSale oteTax);

        /// <summary>
        /// Method to check whether close batch data is available
        /// </summary>
        /// <param name="cashoutId">Cash out id</param>
        /// <returns></returns>
        CloseBatch GetGivexCloseAvailableForCashOutId(string cashoutId);

        /// <summary>
        /// Method to get temp close batch for givex
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CloseBatch GetTempCloseBatchById(string id);

        /// <summary>
        /// method to get the report to be printed corresponding to a card sale
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <returns></returns>
        string GetWexStringBySaleNumber(int saleNumber);
    }
}
