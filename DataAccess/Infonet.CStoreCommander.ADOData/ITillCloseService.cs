using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ITillCloseService
    {
        /// <summary>
        /// Method to check if suspended sales are available
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        bool AreSuspendedSales(int tillNumber);

        /// <summary>
        /// Method to check if last till
        /// </summary>
        /// <param name="till">Till</param>
        /// <returns>True or false</returns>
        bool IsLastTill(Till till);

        /// <summary>
        /// Method to delete records from current sale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        void DeleteCurrentSale(int tillNumber);

        /// <summary>
        /// Method to get system type
        /// </summary>
        /// <returns>System type</returns>
        string GetSystemType();

        /// <summary>
        ///Method to get maximum dip number 
        /// </summary>
        /// <returns></returns>
        int GetMaximumDipNumber();

        /// <summary>
        /// Method to add dip event
        /// </summary>
        /// <param name="dipNumber">Dip number</param>
        /// <param name="shiftDate">Shift date</param>
        void AddDipEvent(short dipNumber, DateTime shiftDate);

        /// <summary>
        /// Method to save tank dip
        /// </summary>
        /// <param name="tankDip"></param>
        void SaveTankDip(TankDip tankDip);


        /// <summary>
        /// Method to check if prepay exists for prepay globals
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        bool IsPrepayGlobalsPresent(int tillNumber);

        /// <summary>
        /// Method to get all bill coins
        /// </summary>
        /// <returns></returns>
        List<BillCoin> GetBillCoins();

        /// <summary>
        /// Method to get total over payment
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Over payment</returns>
        decimal GetOverPayment(int tillNumber);

        /// <summary>
        /// Method to get total payments
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Payments</returns>
        decimal GetPayments(int tillNumber);

        /// <summary>
        /// Method to get total AR payments
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>AR pament</returns>
        decimal GetArPayment(int tillNumber);

        /// <summary>
        /// Method to get total payouts
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Payouts</returns>
        decimal GetPayouts(int tillNumber);

        /// <summary>
        /// Method to total penny adjustments
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Penny adjustment</returns>
        decimal GetPennyAdjustment(int tillNumber);

        /// <summary>
        /// Method to get total change
        /// </summary>
        /// <param name="tillNumber">till number</param>
        /// <returns>Change</returns>
        double GetChangeSum(int tillNumber);

        /// <summary>
        /// Method to get draw and bonus
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Draw and bonus</returns>
        List<decimal> GetDrawAndBonus(int tillNumber);

        /// <summary>
        /// Method to clear previous till close
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        void ClearPreviousTillClose(int tillNumber);

        /// <summary>
        /// Method to get included tender
        /// </summary>
        /// <returns>Tenders</returns>
        List<Tender> GetIncludedTenders();

        /// <summary>
        /// Method to get sale tend amount for till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale tenders</returns>
        List<SaleTendAmount> GetSaleTendAmountForTill(int tillNumber);

        /// <summary>
        /// Method to get bonus give away
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Bonus give away</returns>
        decimal GetBonusGiveAway(int tillNumber);

        /// <summary>
        /// Method to get bonus drop
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cbt">Cash bonus tender</param>
        /// <returns>Bonus drop</returns>
        decimal GetBonusDrop(int tillNumber, string cbt);

        /// <summary>
        /// Method to get drop lines for till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of drop lines</returns>
        List<DropLine> GetDropLinesForTill(int tillNumber);

        /// <summary>
        /// Method to get till close for till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of till close</returns>
        List<TillClose> GetTillCloseByTillNumber(int tillNumber);

        /// <summary>
        /// Method to add till close
        /// </summary>
        /// <param name="tillClose">Till close</param>
        void AddTillClose(TillClose tillClose);

        /// <summary>
        /// Method to update till close
        /// </summary>
        /// <param name="selectedTillClose">Till close</param>
        void UpdateTillClose(TillClose selectedTillClose);

        /// <summary>
        /// Method to get maximum close head
        /// </summary>
        /// <returns>Close head</returns>
        int GetMaxCloseHead();

        /// <summary>
        /// Method to get total credits
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Credits</returns>
        decimal GetTotalCredits(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get ezipin terminal Id
        /// </summary>
        /// <returns>Ezipin terminal Id</returns>
        string GetEzipinTerminalId();

        /// <summary>
        /// Method to get maximum and minimum sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale numbers</returns>
        List<int> GetMaxMinSaleNumber(int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get sale heads
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale heads</returns>
        List<SaleHead> GetSaleHeads(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get total transactions
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total transactions</returns>
        int GetTotalTransactions(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to check if card sales are available
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        bool AreCardSalesAvailable(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get card sales
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        List<CardSale> GetCardSales(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get total cash
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="bt"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalCash(int tillNumber, string whereClause, string bt, DataSource dataSource);

        /// <summary>
        /// Method to get total change
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalChange(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get total payment
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="bt"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalPayment(int tillNumber, string whereClause, string bt, DataSource dataSource);

        /// <summary>
        /// Method to get total AR paymnet
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="bt"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalArPayment(int tillNumber, string whereClause, string bt, DataSource dataSource);

        /// <summary>
        /// Method to get total draw
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="shiftDate"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalDraw(int tillNumber, DateTime shiftDate, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get total payout
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalPayout(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get total drop
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="shiftDate"></param>
        /// <param name="bt"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalDrop(int tillNumber, string whereClause, DateTime shiftDate, string bt, DataSource dataSource);

        /// <summary>
        /// Method to get total bottle return
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetTotalBottleReturn(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get bonus cash
        /// </summary>
        /// <param name="cbt"></param>
        /// <param name="tillNumber"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        decimal GetBonusCash(string cbt, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get payout reasons
        /// </summary>
        /// <returns></returns>
        List<Return_Reason> GetPayoutReasons();

        /// <summary>
        /// Method to get payout sale head
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        SaleHead GetPayoutSaleHead(string code, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get tender description
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        List<string> GetTenderDescriptions(string code);

        /// <summary>
        /// Method to get payout sale tend
        /// </summary>
        /// <param name="tenderName"></param>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        SaleTend GetPayoutSaleTend(string tenderName, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get deleted lines for shift
        /// </summary>
        /// <param name="shiftDate"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        List<Sale_Line> GetDeletedLines(DateTime shiftDate, DataSource dataSource);

        /// <summary>
        /// Method to get deleted lines for till
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        List<Sale_Line> GetDeletedLinesForTill(int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to save till close
        /// </summary>
        /// <param name="tillClose"></param>
        /// <param name="till"></param>
        /// <param name="storeCode"></param>
        /// <param name="boolShiftRec"></param>
        /// <param name="taxExempt"></param>
        /// <param name="teType"></param>
        /// <param name="dbTransRec"></param>
        /// <param name="dbTillRec"></param>
        /// <param name="errorNumber"></param>
        /// <param name="errorDescription"></param>
        /// <param name="lastTable"></param>
        /// <param name="lastSaleNo"></param>
        void Save(ref Till_Close tillClose, Till till, string storeCode, bool boolShiftRec,
            bool taxExempt, string teType, out int dbTransRec, out int dbTillRec, out int errorNumber,
            out string errorDescription, out string lastTable, out double lastSaleNo);

        /// <summary>
        /// Method to save trainer till
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <param name="till">Till</param>
        void SaveTrainerTill(ref Till_Close tillClose, Till till);

        /// <summary>
        /// Method to get all departments
        /// </summary>
        /// <returns></returns>
        List<Dept> GetDepartment();

        /// <summary>
        /// Method to get fuel department Id
        /// </summary>
        /// <returns></returns>
        string GetFuelDepartmentId();

        /// <summary>
        /// Method to get sale lines by department
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        Sale_Line GetSaleLineByDept(string dept, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get sale lines by sub department
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        List<Sale_Line> GetSaleLinesBySubDept(string dept, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get sale lines by sub detail
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="subDept"></param>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        List<Sale_Line> GetSaleLinesBySubDetail(string dept, string subDept, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get sale lines by stock code
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        List<Sale_Line> GetSaleLinesByStockCode(string dept, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get sale lines
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        List<Sale_Line> GetSaleLines(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get sub departments by department Id
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        List<SubDept> GetSubDeptByDept(string dept);

        /// <summary>
        /// Method to get categories
        /// </summary>
        /// <returns></returns>
        Dictionary<int, string> GetCategories();

        /// <summary>
        /// Method to get tax exempt sale line
        /// </summary>
        /// <param name="whereClause">Where clause</param>
        /// <param name="categoryId">Category Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>ATx exempt sale line</returns>
        TaxExemptSaleLine GetTaxExemptSaleLine(string whereClause, int categoryId, int tillNumber);

        /// <summary>
        /// Method to get tax credits
        /// </summary>
        /// <param name="whereClause">Where cluase</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        List<Sale_Tax> GetTaxCredits(string whereClause, int tillNumber);

        /// <summary>
        /// Method to get max dip number
        /// </summary>
        /// <returns></returns>
        short GetMaxDipNumber();

        /// <summary>
        /// Method to get tank dips by dip number
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        List<TankDip> GetTankDipByDipNumber(short dn);

        /// <summary>
        /// Method to get tank gauge set up
        /// </summary>
        /// <returns></returns>
        TankGaugeSetup GetTankGaugeSetUp();

        /// <summary>
        /// Method to get max tax exempt sale
        /// </summary>
        /// <param name="firstCloseNumber">First close number</param>
        /// <param name="lastCloseNumber">Last close number</param>
        /// <returns></returns>
        short GetMaxTaxExemptSale(int firstCloseNumber, int lastCloseNumber);

        /// <summary>
        /// Method to get maximum tax exempt sale line
        /// </summary>
        /// <param name="firstCloseNumber">First close number</param>
        /// <param name="lastCloseNumber">Last close number</param>
        /// <returns>Max tax exempt sale line</returns>
        short GetMaxTaxExemptSaleLine(int firstCloseNumber, int lastCloseNumber);

        /// <summary>
        /// Method to get maximum purchase header
        /// </summary>
        /// <param name="transTime">Transaction time</param>
        /// <returns></returns>
        short GetMaxPurchaseHeader(DateTime transTime);

        /// <summary>
        /// Method to get maximum purchase line
        /// </summary>
        /// <param name="transTime">Transaction time</param>
        /// <returns></returns>
        short GetMaxPurchaseLine(DateTime transTime);

        /// <summary>
        /// Method to get maximum tax exempt card registry
        /// </summary>
        /// <returns></returns>
        short GetMaxTaxExemptCardRegistry();

        /// <summary>
        /// Method to set close batch number
        /// </summary>
        /// <param name="cc"></param>
        void SetCloseBatchNumber(Credit_Card cc);

        /// <summary>
        /// Method to get maximum group number
        /// </summary>
        /// <returns></returns>
        int? GetMaximumGroupNumber();

        /// <summary>
        /// Method to get download Info
        /// </summary>
        /// <returns>Donwload info</returns>
        DownloadInfo GetDownloadInfo();

        /// <summary>
        /// Method to update download info
        /// </summary>
        /// <param name="query">Query string</param>
        void UpdateDownloadInfo(string query);

        /// <summary>
        /// Method to get terminal Ids
        /// </summary>
        /// <param name="posId">Pos Id</param>
        /// <returns>List of terminals</returns>
        List<Terminal> GetTerminals(int posId);

        /// <summary>
        /// Method to get total card transactions
        /// </summary>
        /// <param name="terminalId">Terminal Id</param>
        /// <returns>Card transactions</returns>
        int GetTotalCardTransactions(string terminalId);

        /// <summary>
        /// Method to get maximum close number
        /// </summary>
        /// <returns></returns>
        int GetMaxCloseNumber();

        /// <summary>
        /// Method to get last registry file name
        /// </summary>
        /// <returns>File name</returns>
        string GetLastRegistryFileName();

        /// <summary>
        /// Method to get tax exempt sale heads
        /// </summary>
        /// <param name="firstCloseNumber">Starting till close number</param>
        /// <param name="lastCloseNumber">Last til close number</param>
        /// <returns>Tax exempt sale heads</returns>
        List<TaxExemptSale> GetTaxExemptSaleHeads(int firstCloseNumber, int lastCloseNumber);

        /// <summary>
        /// Method to get tax exempt sale lines
        /// </summary>
        /// <param name="saleNunber">Sale number</param>
        /// <returns>List of tax exempt sale lines</returns>
        List<TaxExemptSaleLine> GetTaxExemptSaleLines(int saleNunber);

        /// <summary>
        /// Method to get tax exempt purchase heads by transaction time
        /// </summary>
        /// <param name="transactionTime">Transaction time</param>
        /// <returns>List of purchase header</returns>
        List<TePurchaseHeader> GetTaxExemptPurchaseHeads(DateTime transactionTime);

        /// <summary>
        /// Mehod to get tax exempt purchase details by purchase number
        /// </summary>
        /// <param name="purchaseNumber">Purchase number</param>
        /// <returns>List of purchase detail</returns>
        List<TePurchaseDetail> GetTaxExemptPurchaseDetails(int purchaseNumber);

        /// <summary>
        /// Method to get totals
        /// </summary>
        /// <param name="strSql">Query string</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        void GetTotals(string strSql, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get batch totals
        /// </summary>
        /// <returns>List of batch total</returns>
        List<BatchTotal> GetBatchTotals();

        /// <summary>
        /// Method to delete close batch
        /// </summary>
        void DeleteBatchTotal();

        /// <summary>
        /// Method to get sale lines for fuel product
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="fuelDept">Fuel department</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="dataSource">Data base</param>
        /// <returns>Sale lines</returns>
        List<Sale_Line> GetFuelLines(int tillNumber, string fuelDept, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get all EOD groups
        /// </summary>
        /// <returns></returns>
        List<EodGroup> GetEodGroups();

        /// <summary>
        /// Method to get EOD saleline
        /// </summary>
        /// <param name="groupId">EOD group Id</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>SaleLine</returns>
        Sale_Line GetEodSaleLine(string groupId, string whereClause, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get non taxable items
        /// </summary>
        /// <param name="whereClause">Where condition</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        Sale_Line GetNonTaxableItems(string whereClause, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get non taxable sale lines
        /// </summary>
        /// <param name="whereClause">Where condition</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        List<Sale_Line> GetNonTaxableSaleLines(string whereClause, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get department by department Id
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        string GetDepartmentById(string deptId);

        /// <summary>
        /// Method to get purchase item
        /// </summary>
        /// <param name="categoryId">Category Id</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        Sale_Line GetPurchaseItem(string categoryId, string whereClause, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get fuel sale lines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="fuelDept"></param>
        /// <param name="dataSource">Data source</param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        List<Sale_Line> GetFuelSaleLines(int tillNumber, string whereClause, string fuelDept, DataSource dataSource);

        /// <summary>
        /// Method to get taxable saleLines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="fuelDept">Fuel dept</param>
        /// <param name="dataSource">Data source</param>
        /// <param name="whereClause">Where clause</param>
        /// <returns></returns>
        List<Sale_Line> GetTaxableSaleLines(int tillNumber, string whereClause, string fuelDept, DataSource dataSource);

        /// <summary>
        /// Method to get total deposits
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Deposits</returns>
        decimal GetChargeDeposit(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get sale charges
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of charges</returns>
        List<Charge> GetSaleCharges(int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get sale kit charges
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of kit charges</returns>
        List<Charge> GetSaleKitCharge(int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get total charge
        /// </summary>
        /// <param name="asCode">As code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charge</returns>
        Charge GetTotalCharge(string asCode, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get total kit charge
        /// </summary>
        /// <param name="asCode">As code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charge</returns>
        Charge GetTotalKitCharge(string asCode, int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get tax exempt sale lines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="fuelDept">Fuel dept</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        List<Sale_Line> GetTaxExemptSaleLines(int tillNumber, string fuelDept, string whereClause, string teType, DataSource dataSource);

        /// <summary>
        /// Method to get sale taxes
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        List<Sale_Tax> GetSaleTaxes(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get line taxes for fuel dept
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="fuelDept">Fuel dept</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        List<Line_Tax> GetLineTaxesForFuel(int tillNumber, string whereClause, string fuelDept, DataSource dataSource);

        /// <summary>
        /// Method to get kit charges
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charges</returns>
        List<Charge_Tax> GetKitCharges(int tillNumber, string whereClause, DataSource dataSource);

        /// <summary>
        /// Method to get totalizer history by group number
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <returns></returns>
        List<TotalizerHist> GetTotalizerHistByGroupNumber(int groupNumber);

        /// <summary>
        /// Method to get total totalizer reading
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <returns></returns>
        List<TotalizerHist> GetTotalReading(int groupNumber);

        /// <summary>
        /// Method to get maximum high low
        /// </summary>
        /// <returns></returns>
        short? GetMaximumHighLow();

        /// <summary>
        /// Method to get total high lows
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <returns>High lows</returns>
        List<TotalHighLow> GetTotalHighLows(int groupNumber);

        /// <summary>
        /// Method to get till close 
        /// </summary>
        /// <returns></returns>
        List<TillClose> GetTillCloseForTrans();

        /// <summary>
        /// Method to add tll close
        /// </summary>
        /// <param name="tillClose"></param>
        void AddTillCloseForTrans(TillClose tillClose);

        /// <summary>
        /// Method to update till close
        /// </summary>
        /// <param name="tillClose">Till close</param>
        void UpdateTillCloseForTrans(TillClose tillClose);


        /// <summary>
        /// Method to get total over payment
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Over payment</returns>
        decimal GetOverPaymentForTrans(string whereClause);

        /// <summary>
        /// Method to get total payments
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Payments</returns>
        decimal GetPaymentsForTrans(string whereClause);

        /// <summary>
        /// Method to get total AR payments
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>AR pament</returns>
        decimal GetArPaymentForTrans(string whereClause);

        /// <summary>
        /// Method to get total payouts
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Payouts</returns>
        decimal GetPayoutsForTrans(string whereClause);

        /// <summary>
        /// Method to total penny adjustments
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Penny adjustment</returns>
        decimal GetPennyAdjustmentForTrans(string whereClause);

        /// <summary>
        /// Method to get total change
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Change</returns>
        double GetChangeSumForTrans(string whereClause);

        /// <summary>
        /// Method to get draw and bonus
        /// </summary>
        /// <param name="shiftDate"></param>
        /// <returns>Draw and bonus</returns>
        List<decimal> GetDrawAndBonusForTrans(DateTime shiftDate);

        /// <summary>
        /// Method to clear previous till close
        /// </summary>
        void ClearPreviousTillCloseForTrans();

        /// <summary>
        /// Method to get sale tend amount for till
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Sale tenders</returns>
        List<SaleTendAmount> GetSaleTendAmountForTrans(string whereClause);

        /// <summary>
        /// Method to get bonus give away
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Bonus give away</returns>
        decimal GetBonusGiveAwayForTrans(string whereClause);

        /// <summary>
        /// Method to get bonus drop
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="cbt">Cash bonus tender</param>
        /// <returns>Bonus drop</returns>
        decimal GetBonusDropForTrans(string whereClause, string cbt);

        /// <summary>
        /// Method to get drop lines for till
        /// </summary>
        /// <param name="shiftDate"></param>
        /// <returns>List of drop lines</returns>
        List<DropLine> GetDropLinesForTrans(DateTime shiftDate);

        /// <summary>
        /// method to get the card numbers by tender name 
        /// </summary>
        /// <param name="tenderName"></param>
        /// <returns></returns>
        List<string> GetCardNumbersByTenderName(string tenderName);


    }
}
