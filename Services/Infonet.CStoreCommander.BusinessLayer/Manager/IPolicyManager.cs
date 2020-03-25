using System;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Policy Manager Interface
    /// </summary>
    public interface IPolicyManager
    {
        //for Ackroo Loyalty and Gift Card. Done by Tony 03/19/2019
        string REWARDS_Carwash { get; }
        string REWARDS_CWGIFT { get; }

        string REWARDS_CWPKG { get; }
        bool REWARDS_DefaultLoyal { get; }
        //for Ackroo Loyalty and Gift Card. ----End

        //For fue discount. done by Tony 03/19/2019
        bool TDRS_FUELDISC { get; }
        string TDRS_EXEPTGID { get; }
        //bool CADB_FUELDISC { get; }
        //For fue discount. ----End

        //Payment Source 03/19/2019 by Tony
        bool PSInet { get; }
        string PSINet_Type { get; }
        //Payment Source ----- End
        //Receipt style type 07/29/2019 by Tony
        string RECEIPT_TYPE { get; }
        //Receipt style type 07/29/2019 by Tony----End
        bool ACCEPT_RET { get; }
        short ADV_LINES { get; }
        short AgeRestrict { get; }
        bool AllowManual { get; }
        bool AllowMin { get; }
        bool AllowPostPay { get; }
        bool AllowPrepay { get; set; }
        bool AllowStack { get; }
        bool ALLOW_CUR_PT { get; }
        bool ALLOW_MARKDO { get; }
        short AmountDigit { get; }
        short ArpayReceiptCopies { get;}
        string ARTender { get; }
        bool ASK_DIPREAD { get; }
        bool AuthPumpPOS { get; }
        bool AuthPump_Eod { get; set; }
        bool AutoShftPick { get; }
        bool AUTO_SALE { get; }
        string BANDMEMBER { get; }
        string BankSystem { get; }
        short BankTimeOutSec { get; }
        string BASECURR { get; }
        short BottleReturnReceiptCopies { get; }
        bool CADB_FUELDISC { get; }
        bool CallBankOnly { get; }
        bool CashAuthPP { get; }
        short CashDrawReceiptCopies { get;}
        short CashDropReceiptCopies { get; }
        bool CBonusDraw { get; }
        bool CBonusFloat { get; }
        string CBonusName { get; }
        string CBonusTend { get; }
        string CC_MODE { get; set; }
        bool Charge_Acct { get; }
        bool CheckUpsell { get; }
        int CLEAR_TILL { get; }
        short CLOSEB_DAYS { get; }
        bool CLOSE_INCLUD { get; }
        bool CLOSE_SUSP { get; }
        bool COMBINECR { get; }
        bool COMBINEFLEET { get; }
        bool COMBINE_LINE { get; }
        bool CONFIRM_DEL { get; }
        string COST_TYPE { get; }
        bool COUNT_CASH { get; set; }
        string COUNT_TYPE { get; }
        bool CouponMSG { get; }
        string CouponTend { get; }
        string CouponType { get; }
        bool CpnSerialID { get; }
        bool CreditMsg { get; }
        bool CREDTERM { get; }
        string CRED_CHANGE { get; }
        short CupnThrehld { get; }
        bool CUST_DISC { get; }
        bool CUST_EXPDATE { get; }
        bool CUST_SCAN { get; }
        bool CUST_SWP { get; }
        bool DebitSwipe { get; }
        bool DefaultCust { get; set; }
        string DEF_CUST_CODE { get; }
        short DEF_VALUE { get; }
        bool DftRdTankDip { get; }
        bool DftRdTotal { get; }
        DateTime DipInputTime { get; }
        bool Dip_Input { get; }
        bool DISC_REASON { get; }
        bool DO_PAYOUTS { get; }
        bool DRAW_REASON { get; }
        bool DropEnv { get; }
        bool ELG_LOY { get; }
        bool EMVVersion { get; }
        bool EMVVERSION_PATP { get; }
        bool ENABLE_BANDACCT { get; }
        bool EOD_CLOSE { get; }
        short EOD_COPIES { get; }
        bool EOD_GROUP { get; }
        string EXC_CASHBONUS { get; }
        bool EXPAND_KITS { get; }
        short EXPIRE_DAYS { get; }
        bool EziDept { get; }
        short FMShift { get; }
        string FORCE_DEF { get; }
        bool FORCE_ID { get; set; }
        short FPR_NOTE_CNT { get; }
        int FPR_TIME { get; }
        bool FPR_USER { get; }
        bool FSGC_ARTENDER { get; }
        string FSGC_CalcType { get; }
        double FSGC_CREDIT { get; }
        bool FSGC_ENABLE { get; }
        short FSGC_EXP { get; }
        double FSGC_OTHER { get; }
        string FSGC_PLU { get; }
        bool FuelLoyalty { get; set; }
        bool FUELONLY { get; set; }
        string FUELONLY_IP { get; }
        short FUELONLY_PRT { get; }
        short FUELONLY_TMT { get; }
        bool FuelPriceChg { get; }
        bool FUELPR_HO { get; }
        bool FuelRebate { get; }
        bool FUEL_CP { get; }
        bool FUEL_GP { get; }
        double FUEL_MAXTH { get; }
        string FUEL_UM { get; }
        string GC_CHANGE { get; }
        bool GC_DISCOUNT { get; }
        short GC_EXPIRE { get; }
        bool GC_FORCE { get; }
        bool GC_NUMBERS { get; }
        string GC_REPT { get; }
        bool GIFTCERT { get; }
        string GiftType { get; }
        bool GIVE_POINTS { get; }
        bool GROUP_PRTY { get; }
        bool Hour24Store { get; }
        bool IDENTIFY_MEMBER { get; }
        int INV_LEFT { get; }
        short ItemCodeDgt { get; }
        string KICKBACK_IP { get; }
        string KICKBACK_PRT { get; }
        string KICKBACK_TMT { get; }
        bool KITS_IN_PO { get; }
        string KP_LOGIN { get; }
        short LastShift { get; }
        bool LockTill { get; }
        short LockTime { get; }
        bool LogUnlimit { get; set; }
        string LoyaltyMesg { get; }
        short LOYAL_DISC { get; }
        int LOYAL_LIMIT { get; }
        string LOYAL_NAME { get; }
        short LOYAL_PPD { get; }
        short LOYAL_PRICE { get; }
        string LOYAL_TYPE { get; }
        double L_RedeemPnts { get; }
        bool ManualCCard { get; }
        bool MASK_CARDNO { get; }
        short MaxDBSwipe { get; }
        bool MEMBER_CODE { get; }
        string MEMBER_IDENTITY { get; }
        bool Msg_Input { get; }
        string NONBANDMEMBER { get; }
        string NUMERIC_TYPE { get; }
        short NUM_PRICE { get; }
        bool OCD_REASON { get; }
        bool OPEN_BUTTON { get; }
        string OPEN_DRAWER { get; }
        bool OR_USER_DISC { get; }
        short PaymentReceiptCopies { get; }
        short PayoutReceiptCopies { get; }
        bool PAY_INSIDE { get; }
        bool PENNY_ADJ { get; }
        string PinpadType { get; }
        bool PO_REASON { get; }
        bool PRICEDISPLAY { get; }
        bool PRICE_TRACK { get; }
        bool PrintLogo { get; }
        bool PRINT_CrdHld { get; }
        bool PRINT_REC { get; }
        bool PRINT_VOID { get; }
        bool PRN_CO_ADDR { get; }
        bool PRN_CO_CODE { get; }
        bool PRN_CO_NAME { get; }
        bool PRN_CO_PHONE { get; }
        bool PRN_SgnRtn { get; }
        bool PRN_UName { get; }
        bool PROD_DISC { get; }
        bool PROMO_SALE { get; }
        bool PrtExmptTax { get; }
        short PRT_ALLSHIFTS { get; }
        bool PRT_CPN { get; }
        bool PRT_OVERPAY { get; }
        string REC_JUSTIFY { get; }
        short RefundReceiptCopies { get; }
        short RES_DAYS { get; }
        string REWARDS_Caption { get; }
        bool REWARDS_Enabled { get; }
        string REWARDS_Gift { get; }
        string REWARDS_Message { get; }
        bool REWARDS_NoRedmpt { get; }
        short REWARDS_Timeout { get; }
        string REWARDS_TpsIp { get; }
        int REWARDS_TpsPort { get; }
        bool RSTR_PROFILE { get; }
        bool SAFEATMDROP { get; }
        short SALE_DELAY { get; }
        bool Scale_Item { get; }
        bool ScanGiftCard { get; }
        bool ScanLoyCard { get; }
        bool SC_CHECK { get; }
        bool Sell_Inactive { get; }
        bool SHARE_SUSP { get; }
        string SHIFT_DAY { get; }
        bool ShowAccBal { get; }
        bool ShowCardCustomers { get; }
        bool SHOW_CODE { get; }
        bool SHOW_CODES { get; }
        bool SHOW_KP { get; }
        bool SHOW_POINT { get; }
        bool SHOW_PRICE { get; }
        bool SITE_RTVAL { get; }
        short SOUND_ALARM { get; }
        bool SOUND_DEV { get; }
        bool SOUND_PUMP { get; }
        bool SOUND_SYS { get; }
        bool StopMsg { get; }
        bool STORE_CREDIT { get; }
        bool SUPPORTEZI { get; }
        bool Support_SAF { get; }
        bool SUSPEND_MT { get; }
        bool SWIPE_CARD { get; }
        bool TankGauge { get; }
        short TaxExemptVoucherCopies { get; }
        bool TAX_COMP { get; }
        bool TAX_EXEMPT { get; }
        bool TAX_EXEMPT_FNGTR { get; }
        bool TAX_EXEMPT_GA { get; }
        bool Tax_Rebate { get; }
        bool TE_AUTOMATE { get; }
        bool TE_ByRate { get; }
        string TE_COLLECTTAX { get; }
        bool TE_GETNAME { get; }
        bool TE_SIGNATURE { get; }
        string TE_SIGNMODE { get; }
        string TE_Type { get; }
        bool ThirdParty { get; set; }
        bool TidelSafe { get; }
        bool TILL_FLOAT { get; }
        bool TILL_NUM { get; }
        bool TRACK_TEINV { get; }
        short TreatyNumDgt { get; }
        decimal TX_RB_AMT { get; }
        string UPC_Format { get; }
        bool UseWeight { get; }
        bool USE_ARCUST { get; }
        bool CashBonus { get; }
        bool USE_CL_TILL { get; }
        bool USE_CUST { get; }
        bool USE_FUEL { get; }
        bool Use_KickBack { get; }
        bool USE_LOYALTY { get; set; }
        bool USE_OVERRIDE { get; }
        bool USE_PINPAD { get; }
        bool USE_PROPANE { get; }
        bool USE_SHIFTS { get; set; }
        bool U_AddStock { get; }
        bool U_AD_PI { get; }
        bool U_AllowFlPay { get; }
        bool U_ARSALES { get; }
        short U_AUTH_LEVEL { get; }
        bool U_BANK_CL { get; }
        bool U_BOTTLERTN { get; }
        float U_BR_LIMIT { get; }
        bool U_CAN_VOID { get; }
        bool U_CHGFPRICE { get; }
        bool U_CHGPASS { get; }
        bool U_CHGPRICE { get; }
        bool U_CHGQTY { get; }
        bool U_DipRead { get; }
        bool U_DISCOUNTS { get; }
        string U_EOD_DISP { get; }
        bool U_EXACTCHG { get; }
        bool U_FM { get; }
        bool U_FUELGP { get; }
        bool U_GIVEREF { get; }
        bool U_INITDEBIT { get; }
        bool U_ManualF { get; }
        bool U_ManuFPrice { get; }
        bool U_OPENDRW { get; }
        bool U_OR_LIMIT { get; }
        bool U_OR_TEQUOTA { get; }
        bool U_OVERPLIMIT { get; }
        bool U_PUMPTEST { get; }
        bool U_REQ_PW { get; }
        bool U_RUNAWAY { get; }
        bool U_SELL { get; }
        bool U_SUSP { get; }
        bool U_TILLAUDIT { get; }
        bool U_TILLCLOSE { get; }
        bool U_TILLDRAW { get; }
        bool U_TILLDROP { get; }
        bool U_TOTREAD { get; }
        string Version { get; }
        bool VOID_AUTH { get; }
        bool VOID_REASON { get; }
        bool WEXEnabled { get; }
        string WexTpsIP { get; }
        int WexTpsPort { get; }
        bool WINDOWS_LOGIN { get; }
        bool X_RIGOR { get; }
        string GIVEX_IP { get; }
        string GIVEX_PORT { get; }
        int GIVETIMEOUT { get; }
        string GIVEX_USER { get; }
        string GIVEX_PASS { get; }
        string GiveXMerchID { get; }
        bool AlwAdjGiveX { get; }
        string TIMEFORMAT { get; }
        string CarwashDepartment { get; }
        bool IsCarwashIntegrated { get; }
        bool SupportCarwashAtPump { get; }
        string CarwashIP { get; }
        int CarwashPort { get; }
        int CarwashTout { get; }
        bool IsCarwashSupported { get; }
        bool IsWexEnable { get; }
        string WexIp { get; }
        int WexPort { get;}
        bool Store_Credit { get; }

        /// <summary>
        /// Get all Policies
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        dynamic GetAllPolicies(string userCode);

        /// <summary>
        /// Get login Policies
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="posId"></param>
        /// <returns></returns>
        object GetLoginPolicies(string ipAddress, int posId);


        /// <summary>
        /// Load store information
        /// </summary>
        /// <returns></returns>
        Store LoadStoreInfo();

        void SetUpPolicy(Security security);

        dynamic GetPol(string policyName, dynamic obj);
        Security LoadSecurityInfo();
        dynamic RefreshPolicies(string userCode);

    }
}