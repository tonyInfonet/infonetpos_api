namespace Infonet.CStoreCommander.BusinessLayer.Utilities
{
    public static class Constants
    {
        public const string Trainer = "Trainer"; //Behrooz Jan-12-06
        public const int TrainFirstTill = 91; //Behrooz Jan-12-06
        public const string TrainLastTill = "99"; //Behrooz Jan-12-06
        public const string TrainerRecipt = "(( Training Mode ))"; //Behrooz Jan-12-06
        public const string CashSaleClient = "Cash Sale";
        public const string CurrencyFormat = "##0.00";
        public const string DiscountFormat = "0.00";
        public const string ChangeFormat = "###,##0.00";

        public const string NoPumpsDefined = "There are no Pump grades defined!";
        public const string ChangeDue = "${0}~{1}";
        //File names
        public const string BottleReturnFile = "BottleReturn.txt";
        public const string CashDrawFile = "CashDraw.txt";
        public const string SalesCountFile = "SalesCountReport.txt";
        public const string FlasReportFile = "FlashRep.txt";
        public const string TillAuditFile = "TillAudit.txt";

        /* Added by sonali */
        public const string KickbackPoints = "KickBack_Balance.txt";
        public const string KickbackReceipt = "KickBack_Receipt.txt";
        /*ended*/
        public const string CashDropFile = "CashDrop.txt";
        public const string PriceFile = "Price.txt";
        public const string FuelPriceFile = "FuelPrice.txt";
        public const string ReceiptFile = "Receipt.txt";
        public const string RunAwayFile = "RunAway.txt";
        public const string PayoutFile = "Payout.txt";
        public const string GivexFile = "GivexReceipt.txt";
        public const string PumpTestFile = "PumpTest.txt";
        public const string BankEodFile = "BankEod.txt";
        public const string TaxExemptionFile = "TaxExemptVoucher.txt";
        public const string TillCloseFile = "TillClose.txt";
        public const string ErrorFile = "ErrorLog";
        public const string EnglishCopy = "TEnglish.txt";
        public const string FrenchCopy = "TFrench.txt";
        public const string TransactionReprintCopy = "TReprint.txt";
        public const string PayAtPumpFile = "PayAtPump.txt";
        public const string ArPayFile = "ARPay.txt";
        public const string ReprintFile = "Reprint.txt";
        public const string PaymentFile = "Payment.txt";
        public const string StoreCreditFile = "StoreCred.txt";
        public const string GivexCloseFile = "GiveXClose.txt";
        public const string EodDetailsFile = "EodDetails.txt";
        public const string TankDipFile = "Dip.txt";

        //Error
        public const string InvalidRequest = "Request is Invalid";
        public const string InvalidRegister = "Register number does not exists";
        public const string PumpTestNotAllowed = "You cannot use this option.~PumpTest.";
        public const string MaxLimitExceeded = "Cannot use more value, Maximum limit exceeded";
        public const string CheckKickbackBalance = "Invalid Loyalty Card";
    }
}
