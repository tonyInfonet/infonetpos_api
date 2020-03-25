using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Policy Manager 
    /// </summary>
    public class PolicyManager : ManagerBase, IPolicyManager
    {
        private readonly IPolicyService _policyService;
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly IDipInputService _dipInputService;
        private readonly IPromoManager _promoManager;
        private List<Policy> _policies;
        private List<PolicyCanbe> _policiesCanbe;
        private List<PolicySet> _policiesSet;
        private readonly IFuelService _fuelService;
        private List<BackOfficePolicy> _allPolicies;

        /*added by sonali */
     //   private readonly ISaleManager _saleManager;
        /*ended by sonali*/

        public PolicyManager(IPolicyService policyService, ILoginService loginService,
            IUserService userService, IPromoManager promoManager,
            IDipInputService dipInputService, IFuelService fuelService)
        {
            _policyService = policyService;
            _loginService = loginService;
            _userService = userService;
            _promoManager = promoManager;
            _dipInputService = dipInputService;
            _fuelService = fuelService;
          //  _saleManager = saleManager;
            //InitializePolicyData(PosId);
        }

        /// <summary>
        /// Get login Policy 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="posId"></param>
        /// <returns></returns>
        public object GetLoginPolicies(string ipAddress, int posId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PolicyManager,GetLoginPolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            InitializePolicies();

            _promoManager.Load_Promos("");
            _promoManager.GetPromosForToday();
            var loginPolicies = new
            {
                PosId = posId,
                WindowsLogin = WINDOWS_LOGIN,
                UseShifts = USE_SHIFTS,
                ProvideTillFloat = TILL_FLOAT,
                UsePredefinedTillNumber = TILL_NUM,
                AutoShiftPick = AutoShftPick,
                PosLanguage = Language,
                KeypadFormat = KP_LOGIN
            };
            Performancelog.Debug($"End,PolicyManager,GetLoginPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return loginPolicies;
        }


        /// <summary>
        /// Get current user
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private User GetCurrentUser(string code)
        {
            //return _loginManager.GetUser(code);
            return CacheManager.GetUser(code) ?? _userService.GetUser(code);
        }

        /// <summary>
        /// Get All Policy 
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public dynamic GetAllPolicies(string userCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PolicyManager,GetAllPolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var service = _fuelService.LoadService();
            var dipInputs = _dipInputService.GetDipInputValues().Where(d => d.DipValue != "").ToList();
            dynamic allPolicies = new ExpandoObject();
            allPolicies.VERSION = Version; // Tony 09/05/2019
            allPolicies.RECEIPT_TYPE = RECEIPT_TYPE;//done by tony 07/29/2019
            allPolicies.FuelDept = _fuelService.LoadFuelDept(); //done by Tony 05/21/2019
            allPolicies.PSINet_Type = PSINet_Type; //done by tony 11/21/2018
            allPolicies.SupportPSInet = PSInet; // done by tony 08/23/2018
            allPolicies.isTDRS_FUELDISCSupported = TDRS_FUELDISC;  //done by tony 07/17/2018
            allPolicies.isFuelDiscountSupported = CADB_FUELDISC;  //done by tony 07/17/2018
            allPolicies.displayCustGrpID = TDRS_EXEPTGID;  //done by tony 07/17/2018
            allPolicies.REWARDS_Message = REWARDS_Message; //done by Tony 01/22/2019
            allPolicies.REWARDS_Enabled = REWARDS_Enabled; //done by Tony 12/19/2018
            allPolicies.REWARDS_Gift = REWARDS_Gift; //done by Tony 12/19/2018
            allPolicies.REWARDS_TpsIp = REWARDS_TpsIp; //done by Tony 12/19/2018
            allPolicies.REWARDS_TpsPort = REWARDS_TpsPort; //done by Tony 12/19/2018
            allPolicies.REWARDS_Timeout = REWARDS_Timeout; //done by Tony 12/19/2018
            allPolicies.REWARDS_Carwash = REWARDS_Carwash; //done by Tony 12/19/2018
            allPolicies.REWARDS_CWGIFT = REWARDS_CWGIFT; //done by Tony 12/19/2018
            allPolicies.REWARDS_CWPKG = REWARDS_CWPKG; //done by Tony 12/19/2018
            allPolicies.REWARDS_DefaultLoyal = REWARDS_DefaultLoyal; //done by Tony 12/19/2018

            allPolicies.SupportKickback = USE_KICKBACK;
            allPolicies.KickbackRedeemMsg= L_REDEEMPNTS;
            allPolicies.CustomKickbackmsg = LoyaltyMesg;
            allPolicies.OperatorCanUseCustomer = USE_CUST;
            allPolicies.OperatorCanUseARCustomer = USE_ARCUST;
            allPolicies.ShowCustomerNoteOnOverlimit = CreditMsg;
            allPolicies.DisplayCustomerDetails = ShowCardCustomers;
            allPolicies.DefaultCustomerCode = DEF_CUST_CODE;
            allPolicies.TenderNameforARAccount = ARTender;
            allPolicies.OperatorCanScanCustomerCard = CUST_SCAN;
            allPolicies.OperatorCanSwipeCustomerCard = CUST_SWP;
            allPolicies.OperatorCanSwipeMemberCodeAtPump = MEMBER_CODE;
            allPolicies.DefaultMemberCodeForNonMembers = NONBANDMEMBER;
            allPolicies.SupportKitsInPurchase = KITS_IN_PO;
            allPolicies.AddStockItemNotFoundInList = U_AddStock;
            allPolicies.ConfirmDeleteLineItem = CONFIRM_DEL;
            allPolicies.UseProductDiscount = PROD_DISC;
            allPolicies.PrintReceiptForVoidAndReturn = PRINT_VOID;
            allPolicies.SuspendEmptySales = SUSPEND_MT;
            allPolicies.ShareSuspendSale = SHARE_SUSP;
            allPolicies.AllowPayout = DO_PAYOUTS;
            allPolicies.ReasonForPayout = PO_REASON;
            allPolicies.UseCustomerDiscountCode = CUST_DISC;
            allPolicies.ForceAuthorizationOnVoid = VOID_AUTH;
            allPolicies.OperatorCanGiveDiscount = U_DISCOUNTS;
            allPolicies.OperatorCanChangePrice = U_CHGPRICE;
            allPolicies.OperatorCanChangeQantity = U_CHGQTY;
            allPolicies.OperatorCanVoidSale = U_CAN_VOID;
            allPolicies.UseReasonForVoid = VOID_REASON;
            allPolicies.OperatorCanSuspendOrUnsuspendSales = U_SUSP;
            allPolicies.OperatorCanReturnBottle = U_BOTTLERTN;
            allPolicies.OperatorBottleReturnLimit = U_BR_LIMIT;
            allPolicies.SupportDipInput = Dip_Input && U_DipRead;
            allPolicies.DipInputTime = dipInputs.Count == 0 ? DipInputTime : DipInputTime.AddDays(1);
            allPolicies.AllowPOSMinimize = AllowMin;
            allPolicies.UserCanChangePassword = U_CHGPASS;
            allPolicies.OperatorCanAddStock = U_AddStock;
            allPolicies.OperatorCanUseLoyalty = USE_LOYALTY;
            allPolicies.CertificateType = GiftType;
            allPolicies.AllowAdjustmentForGiveX = AlwAdjGiveX;
            allPolicies.RequirePasswordOnChangeUser = U_REQ_PW;
            allPolicies.OperatorCanReturnSale = U_GIVEREF;
            allPolicies.SupportsTaxExampt = TAX_EXEMPT;
            allPolicies.FreezeTillAutomatically = LockTill;
            allPolicies.IdleIntervalAfterAppFreezes = LockTime;
            allPolicies.OperatorCanDrawCash = U_TILLDRAW;
            allPolicies.OperatorCanDropCash = U_TILLDROP;
            allPolicies.UseReasonForCashDraw = DRAW_REASON;
            allPolicies.RefundReceiptCopies = RefundReceiptCopies;
            allPolicies.ArpayReceiptCopies = ArpayReceiptCopies;
            allPolicies.PayoutReceiptCopies = PayoutReceiptCopies;
            allPolicies.PaymentReceiptCopies = PaymentReceiptCopies;
            allPolicies.BottleReturnReceiptCopies = BottleReturnReceiptCopies;
            allPolicies.CashDropReceiptCopies = CashDropReceiptCopies;
            allPolicies.CashDrawReceiptCopies = CashDrawReceiptCopies;
            allPolicies.OperatorIsTrainer = GetCurrentUser(UserCode).User_Group.Code == "Trainer";
            allPolicies.AskForCashDropReason = SAFEATMDROP;
            allPolicies.RequireEnvelopNumber = DropEnv;
            allPolicies.OperatorCanOpenCashDrawer = GetCurrentUser(UserCode).User_Group.Code != "Trainer" && OPEN_BUTTON;
            allPolicies.UseReasonForCashDrawer = OCD_REASON;
            allPolicies.GiftTender = GiftType;
            allPolicies.ForceGiftCertificate = GC_FORCE;
            allPolicies.GiftCertificateNumbered = GC_NUMBERS;
            allPolicies.ForcePrintReceipt = PRINT_REC;
            allPolicies.DelayInNewSale = SALE_DELAY;
            allPolicies.EnableExactChange = U_EXACTCHG && !TAX_EXEMPT;
            allPolicies.CheckSC = SC_CHECK;
            allPolicies.BaseCurrency = BASECURR;
            allPolicies.EnableMsgInput = Msg_Input;
            allPolicies.CouponTender = CouponTend;
            allPolicies.TaxExemption = TAX_EXEMPT_GA;
            allPolicies.ShowPumpStopMessage = StopMsg;
            allPolicies.PrepayEnabled = service.PrepayEnabled;
            allPolicies.PostPayEnabled = service.PostPayEnabled;
            allPolicies.PayAtPumpEnabled = service.PayPumpEnabled;
            allPolicies.IsFuelOnlySystem = USE_FUEL && FUELONLY;
            allPolicies.IsPosOnlySystem = !USE_FUEL;
            allPolicies.AllowSwipeScan = CUST_SCAN || CUST_SWP;
            allPolicies.FuelPriceNotificationTimeInterval = FPR_TIME;
            allPolicies.SupportFuelPriceFromHO = FUELPR_HO;
            allPolicies.FuelPriceNotificationCount = FPR_NOTE_CNT;
            allPolicies.SupportCashCreditpricing = FUEL_CP;
            allPolicies.SwitchUserOnEachSale = GetPol("FORCE_ID", GetCurrentUser(UserCode));
            allPolicies.IsFuelPricingGrouped = FUEL_GP;
            allPolicies.IsPriceIncrementEnabled = FUEL_GP && U_FUELGP;
            allPolicies.IsTaxExemptionPricesEnabled = FUEL_GP && TAX_EXEMPT && (!TE_ByRate || (TE_ByRate && TE_Type == "SITE"));
            allPolicies.PumpSpace = _fuelService.GetPumpSpace();
            allPolicies.MinimizePOS = AllowMin;
            allPolicies.IsFuelPriceDisplayUsed = PRICEDISPLAY;
            allPolicies.UserCanManualFuelSale = U_ManualF;
            allPolicies.StayOnFuelPricePage = FuelPriceChg;
            allPolicies.RequireToGetCustomerName = TE_GETNAME;
            allPolicies.UserCanPerformManualSales = U_ManualF;
            allPolicies.IsFleetCardRequired = !CC_MODE.Equals("Cross-Ring");
            allPolicies.ClickDelayForPumps = _fuelService.GetClickDelayForPumps();
            allPolicies.IsCarwashIntegrated = IsCarwashIntegrated;
            allPolicies.IsCarwashSupported = IsCarwashSupported;
            allPolicies.IsWexEnable = IsWexEnable;
            Performancelog.Debug($"End,PolicyManager,GetAllPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!FUELONLY)
            {
                Variables.GiveX_Renamed = new GiveX
                {
                    IP = GIVEX_IP,
                    MerchantID = GiveXMerchID,
                    TcpPort = Convert.ToInt16(GIVEX_PORT),
                    AllowAdjustGiveX = Convert.ToString(AlwAdjGiveX),
                    CommunicationTimeOut = Convert.ToInt16(GIVETIMEOUT),
                    UserID = GIVEX_USER,
                    UserPassword = GIVEX_PASS
                };
            }

            return allPolicies;
        }

        public dynamic RefreshPolicies(string userCode)
        {
            _promoManager.Load_Promos("");
            _promoManager.GetPromosForToday();
            InitializePolicies();
            return GetAllPolicies(userCode);
        }


        #region Company level Policies
        
        //For Akroo car wash. Done by Tony 03/19/2019

        public string REWARDS_Carwash => Convert.ToString(GetPol("REWARDS_Carwash", null)).ToUpper();
        public string REWARDS_CWGIFT => Convert.ToString(GetPol("REWARDS_CWGIFT", null)).ToUpper();
        public string REWARDS_CWPKG => Convert.ToString(GetPol("REWARDS_CWPKG", null)).ToUpper();
        public bool REWARDS_DefaultLoyal => Convert.ToBoolean(GetPol("REWARDS_DefaultLoyal", null));
        //For Akroo car wash. ----End

        //For Payment Source 03/19/2019 by Tony
        public bool PSInet => Convert.ToBoolean(GetPol("SUPPORT_PSINet", null));
        public string PSINet_Type => Convert.ToString(GetPol("PSINet_Type", null));
        //For Payment Source ----End
        //Receipt style type  07/29/2019  by Tony
        public string RECEIPT_TYPE => Convert.ToString(GetPol("RECEIPT_TYPE", null));
        //Receipt style type  07/29/2019  by Tony----End
        //Support cash/debit fuel discount?
        //public bool CADB_FUELDISC => Convert.ToBoolean(GetPol("CADB_FUELDISC", null)); //done by Tony 03/19/2019

        //Support Display Fleet and Credit for Cash/Debit fuel discount 
        public bool TDRS_FUELDISC => Convert.ToBoolean(GetPol("TDRS_FUELDISC", null)); //done by Tony 03/19/2019

        public string TDRS_EXEPTGID => Convert.ToString(GetPol("TDRS_EXEPTGID", null));   //done by Tony 03/19/2019

        //Language
        public string Language => LoadStoreInfo().Language;

        //Version
        public string Version => Convert.ToString(GetPol("VERSION", null)).ToUpper();

        //This is the Company level's "Do you accept product returns?" {Yes,No} - Boolean
        //for it's other level's setting, we save it into Sale_Line class
        public bool ACCEPT_RET => Convert.ToBoolean(GetPol("ACCEPT_RET", null));

        //Only company level:  "Lines to advance after printing" RG(0,20) - Integer
        public short ADV_LINES => Convert.ToInt16(GetPol("ADV_LINES", null));

        //Shift Number for Automatic Fuel Management Reading TODO:Remove
        public short FMShift => Convert.ToInt16(GetPol("FMSHIFT", null));

        //Only company level:  "Allow use of Current Sale Points in Payment." {Yes,No} - Boolean
        public bool ALLOW_CUR_PT => Convert.ToBoolean(GetPol("ALLOW_CUR_PT", null));

        //Only company level:  "Give Automatic Discounts on Sale Items?" {Yes,No} - Boolean
        public bool AUTO_SALE => Convert.ToBoolean(GetPol("AUTO_SALE", null));

        //Allow adjustment to givex sale
        public bool AlwAdjGiveX => Convert.ToBoolean(GetPol("AlwAdjGiveX", null));

        //Only company level:  "Name of base currency" STAB(TendMast, TendDesc) - string
        public string BASECURR => Convert.ToString(GetPol("BASECURR", null));

        //Only company level:  "Credit Card Mode is" {Cross-Ring,Validate} - string
        public string CC_MODE
        {
            get
            {
                return Convert.ToString(GetPol("CC_MODE", null));
            }
            set { }
        }

        //Only company level:  "Clear Till if Currency is more than __dollars." RG(1,10000) - integer
        public int CLEAR_TILL => Convert.ToInt32(GetPol("CLEAR_TILL", null));

        //Only company level:  "Show only 'Include in CloseTenders?" {Yes,No} - Boolean
        public bool CLOSE_INCLUD => Convert.ToBoolean(GetPol("CLOSE_INCLUD", null));

        //Only company level:  "Allow close with suspended sales." {Yes,No} - Boolean
        public bool CLOSE_SUSP => Convert.ToBoolean(GetPol("CLOSE_SUSP", null));

        //Only company level:  "Show Reprint Close Batch___days" RG(1,365) - integer
        public short CLOSEB_DAYS => Convert.ToInt16(GetPol("CLOSEB_DAYS", null));



        public bool COMBINE_LINE => Convert.ToBoolean(GetPol("COMBINE_LINE", null));

        //Only company level:  "Tender Screen Display Only One Credit Card" {Yes,No} - Boolean
        public bool COMBINECR => Convert.ToBoolean(GetPol("COMBINECR", null));

        //Only company level:  "Tender Screen Display Only One Fleet Card" {Yes,No} - Boolean
        public bool COMBINEFLEET => Convert.ToBoolean(GetPol("COMBINEFLEET", null));

        //Only company level:  "Confirm Deletes of lines in a sale?" {Yes,No} - Boolean
        public bool CONFIRM_DEL => Convert.ToBoolean(GetPol("CONFIRM_DEL", null));

        //Only company level:  "Enter Cash Counts when Closing" {Yes,No} - Boolean
        public bool COUNT_CASH
        {
            get
            {
                return Convert.ToBoolean(GetPol("COUNT_CASH", null));
            }
            set { }
        }

        //Only company level:  "Enter Tender Amounts for" {Each Tender,All Tenders} - String
        public string COUNT_TYPE => Convert.ToString(GetPol("COUNT_TYPE", null));

        //Only company level:  "Give a credit as change on a credit." {Never,Always,Choice} - String
        public string CRED_CHANGE => Convert.ToString(GetPol("CRED_CHANGE", null));

        //Only company level:  "Do you offer credit terms?" {Yes,No} - Boolean
        public bool CREDTERM => Convert.ToBoolean(GetPol("CREDTERM", null));

        //Only company level:  "Use Customer Discount Codes" {Yes,No} - Boolean
        public bool CUST_DISC => Convert.ToBoolean(GetPol("CUST_DISC", null));

        //Only company level:  "Default Gift Certificate Amount" RG(0,1000) - integer
        public short DEF_VALUE => Convert.ToInt16(GetPol("DEF_VALUE", null));



        public bool DISC_REASON => Convert.ToBoolean(GetPol("DISC_REASON", null));

        //Only company level:  "Do you allow Payouts?" {Yes,No} - Boolean
        public bool DO_PAYOUTS => Convert.ToBoolean(GetPol("DO_PAYOUTS", null));

        //Only company level:  "Copies of End of Day to Print" RG(1,5) - integer
        public short EOD_COPIES => Convert.ToInt16(GetPol("EOD_COPIES", null));

        //Only company level:  "Expand Kits on receipts?" {Yes,No} - Boolean
        public bool EXPAND_KITS => Convert.ToBoolean(GetPol("EXPAND_KITS", null));

        //Only company level:  "Store Credits Expire in ____ days." RG(0,1000) - integer
        public short EXPIRE_DAYS => Convert.ToInt16(GetPol("EXPIRE_DAYS", null));

        //Only company level:  "Use reasons for cash draw ?" {Yes,No} - Boolean
        public bool DRAW_REASON => Convert.ToBoolean(GetPol("DRAW_REASON", null));

        //Only company level:  "Default User ID to" {Current,Blank} - String
        public string FORCE_DEF => Convert.ToString(GetPol("FORCE_DEF", null));

        //Only company level:  "Force User ID on Every Sale" {Yes,No} - Boolean
        public bool FORCE_ID
        {
            get
            {
                return Convert.ToBoolean(GetPol("FORCE_ID", null));
            }
            set { }
        }

        //Only company level:  "Support Cash/Credit Pricing" {Yes,No} - Boolean
        public bool FUEL_CP => Convert.ToBoolean(GetPol("FUEL_CP", null));

        //Only company level:  "Do you use fuel group price?" {Yes,No} - Boolean
        public bool FUEL_GP => Convert.ToBoolean(GetPol("FUEL_GP", null));



        public string FUEL_UM => Convert.ToString(GetPol("FUEL_UM", null));

        //Only company level:  "Give a credit as change on a Gift Cert" {Never,Always,Choice} - String
        public string GC_CHANGE => Convert.ToString(GetPol("GC_CHANGE", null));

        //Only company level:  "Allow discounts on Gift Certificates" {Yes,No} - Boolean
        public bool GC_DISCOUNT => Convert.ToBoolean(GetPol("GC_DISCOUNT", null));

        //Only company level:  "Gift Certificates expire in ___ days." RG(0,1000) - Integer
        public short GC_EXPIRE => Convert.ToInt16(GetPol("GC_EXPIRE", null));

        //Only company level:  "Force Gift Certificate Checkoff ?" {Yes,No} - Boolean
        public bool GC_FORCE => Convert.ToBoolean(GetPol("GC_FORCE", null));

        //Only company level:  "Are Gift Certificates Numbered?" {Yes,No} - Boolean
        public bool GC_NUMBERS => Convert.ToBoolean(GetPol("GC_NUMBERS", null));

        //Only company level:  "Report Gift Certificates in Department" STAB(Dept, Dept) - String
        public string GC_REPT => Convert.ToString(GetPol("GC_REPT", null));

        //Only company level:  "Do you sell Gift Certificates?" {Yes,No} - Boolean
        public bool GIFTCERT => Convert.ToBoolean(GetPol("GIFTCERT", null));

        //Only company level:  "Give points on points purchases." {Yes,No} - Boolean
        public bool GIVE_POINTS => Convert.ToBoolean(GetPol("GIVE_POINTS", null));

        //Only company level:  "Group pricing has priority." {Yes,No} - Boolean TODO:Remove
        public bool GROUP_PRTY => Convert.ToBoolean(GetPol("GROUP_PRTY", null));

        //Only company level:  "Warn user when only___ invoice numbers are left." RG(500,50000) - Integer
        public int INV_LEFT => Convert.ToInt32(GetPol("INV_LEFT", null));

        //Only company level:  "Loyalty Discount Code is" RG(0,99) - Integer
        public short LOYAL_DISC => Convert.ToInt16(GetPol("LOYAL_DISC", null));

        //Only company level:  "Number of Points can be Redeemed / Transaction( 0- No limit)" RG(0,100000) - Integer
        public int LOYAL_LIMIT => Convert.ToInt32(GetPol("LOYAL_LIMIT", null));

        //Only company level:  "Loyalty Program Name" {?} - String
        public string LOYAL_NAME => Convert.ToString(GetPol("LOYAL_NAME", null));

        //Only company level:  "Points per dollar purchased" RG(0,1000) - Integer
        public short LOYAL_PPD => Convert.ToInt16(GetPol("LOYAL_PPD", null));

        //Only company level:  "Points per Units purchased" RG(0,1000) - Integer
        //Public Property Let LOYAL_PPU(ByVal vData As Integer)
        //    mvarLOYAL_PPU = vData
        //End Property
        //
        //Public Property Get LOYAL_PPU() As Integer
        //    LOYAL_PPU = mvarLOYAL_PPU
        //End Property

        //Only company level:  "Loyalty Price is" RG(2,10) - Integer
        public short LOYAL_PRICE => Convert.ToInt16(GetPol("LOYAL_PRICE", null));

        //Only company level:  "Loyalty Type is" {Points,Prices,Discounts} - String
        public string LOYAL_TYPE => Convert.ToString(GetPol("LOYAL_TYPE", null));

        //Only company level:  "Mask the Credit Card Number" {Yes,No} - Boolean
        public bool MASK_CARDNO => Convert.ToBoolean(GetPol("MASK_CARDNO", null));

        // to keep "Number of pre-defined prices in use" RG(1,10) - Integer
        public short NUM_PRICE => Convert.ToInt16(GetPol("NUM_PRICE", null));

        // to keep "Numeric Entry Style" {Decimal,0,1,2,3} - string
        public string NUMERIC_TYPE => Convert.ToString(GetPol("NUMERIC_TYPE", null));

        // to keep "Use reasons for open cash drawer ?" {Yes,No} - Boolean
        public bool OCD_REASON => Convert.ToBoolean(GetPol("OCD_REASON", null));

        // to keep "Show a Drawer Open Button" {Yes,No} - Boolean
        public bool OPEN_BUTTON => Convert.ToBoolean(GetPol("OPEN_BUTTON", null));

        // to keep "Open Drawer on" {Every Sale,As Needed} - string
        public string OPEN_DRAWER => Convert.ToString(GetPol("OPEN_DRAWER", null));

        //Only company level:  "Over-ride User Discounts from Table" {Yes,No} - Boolean
        public bool OR_USER_DISC => Convert.ToBoolean(GetPol("OR_User_DISC", null));

        //Only company level:  "Pay Inside Credit/Debit" {Yes,No} - Boolean
        public bool PAY_INSIDE => Convert.ToBoolean(GetPol("PAY_INSIDE", null));

        //Only company level:  "Use reasons for payouts ?" {Yes,No} - Boolean
        public bool PO_REASON => Convert.ToBoolean(GetPol("PO_REASON", null));

        //Only company level:  "Use reasons for payouts ?" {Yes,No} - Boolean
        public bool PRICEDISPLAY => Convert.ToBoolean(GetPol("PRICEDISPLAY", null));

        //Only company level:  "Force Receipt Printing" {Yes,No} - Boolean
        public bool PRINT_REC => Convert.ToBoolean(GetPol("PRINT_REC", null));

        //Only company level:  "Print Company Address on Receipts" {Yes,No} - Boolean
        public bool PRN_CO_ADDR => Convert.ToBoolean(GetPol("PRN_CO_ADDR", null));

        //Only company level:  "Print Company Name on Receipts" {Yes,No} - Boolean
        public bool PRN_CO_NAME => Convert.ToBoolean(GetPol("PRN_CO_NAME", null));

        //Only company level:  "Print Phone & Fax Numbers on Receipt" {Yes,No} - Boolean
        public bool PRN_CO_PHONE => Convert.ToBoolean(GetPol("PRN_CO_PHONE", null));

        //Only company level:  "Use Product Discount Codes" {Yes,No} - Boolean
        public bool PROD_DISC => Convert.ToBoolean(GetPol("PROD_DISC", null));

        //Only company level:  "Supports Promotional Sale" {Yes,No} - Boolean
        public bool PROMO_SALE => Convert.ToBoolean(GetPol("PROMO_SALE", null));

        //Only company level:  "Receipt Header Justification is" {Left,Right,Centre} - String
        public string REC_JUSTIFY => Convert.ToString(GetPol("REC_JUSTIFY", null));

        // to keep "Keep data for Returning a Sale for ____Days." RG(0,365) - Integer
        public short RES_DAYS => Convert.ToInt16(GetPol("RES_DAYS", null));

        // to keep "Delay after printing receipt (seconds)" RG(0,60) - Integer
        public short SALE_DELAY => Convert.ToInt16(GetPol("SALE_DELAY", null));

        // to keep "Do you validate redeemed credits?" {Yes,No} - Boolean
        public bool SC_CHECK => Convert.ToBoolean(GetPol("SC_CHECK", null));

        // to keep "Share Suspended Sale" {Yes,No} - Boolean
        public bool SHARE_SUSP => Convert.ToBoolean(GetPol("SHARE_SUSP", null));

        //Only company level:  "Shift 1 Starts in the ----- Day" {Current,Next} - String
        public string SHIFT_DAY => Convert.ToString(GetPol("SHIFT_DAY", null));

        // to keep "Show Stock Codes on Receipts" {Yes,No} - Boolean
        public bool SHOW_CODE => Convert.ToBoolean(GetPol("SHOW_CODE", null));

        // to keep "Show Codes in Product Sales Report" {Yes,No} - Boolean
        public bool SHOW_CODES => Convert.ToBoolean(GetPol("SHOW_CODES", null));

        // to keep "Show KeyPads" {Yes,No} - Boolean
        public bool SHOW_KP => Convert.ToBoolean(GetPol("SHOW_KP", null));

        // to keep "Show total points on receipts" {Yes,No} - Boolean
        public bool SHOW_POINT => Convert.ToBoolean(GetPol("SHOW_POINT", null));

        // to keep "Show Prices on Receipts" {Yes,No} - Boolean
        public bool SHOW_PRICE => Convert.ToBoolean(GetPol("SHOW_PRICE", null));

        // to keep "Sound on Device event" {Yes,No} - Boolean
        public bool SOUND_DEV => Convert.ToBoolean(GetPol("SOUND_DEV", null));

        // to keep "Sound on Pump event" {Yes,No} - Boolean
        public bool SOUND_PUMP => Convert.ToBoolean(GetPol("SOUND_PUMP", null));

        // to keep "Sound on Different Application Events" {Yes,No} - Boolean
        public bool SOUND_SYS => Convert.ToBoolean(GetPol("SOUND_SYS", null));

        // to keep "Do you issue store credits?" {Yes,No} - Boolean
        public bool STORE_CREDIT => Convert.ToBoolean(GetPol("STORE_CREDIT", null));

        // to keep "Suspend empty sales" {Yes,No} - Boolean
        public bool SUSPEND_MT => Convert.ToBoolean(GetPol("SUSPEND_MT", null));

        // to keep "Swipe Credit Cards?" {Yes,No} - Boolean
        public bool SWIPE_CARD => Convert.ToBoolean(GetPol("SWIPE_CARD", null));

        // to keep "Supports Tax Exempt" {Yes,No} - Boolean
        public bool TAX_EXEMPT => Convert.ToBoolean(GetPol("TAX_EXEMPT", null));

        // to keep "Supports Third Party Card" {Yes,No} - Boolean
        public bool ThirdParty
        {
            get
            {
                return Convert.ToBoolean(GetPol("ThirdParty", null));
            }
            set { }
        }


        //"Eligible for loyalty set to True by default"
        public bool ELG_LOY => Convert.ToBoolean(GetPol("ELG_LOY", null));

        //Nancy added PRINT_VOID to print void receipt
        public bool PRINT_VOID => Convert.ToBoolean(GetPol("PRINT_VOID", null));


        public bool PRN_SgnRtn => Convert.ToBoolean(GetPol("PRN_SgnRtn", null));

        //Nancy added to support TaxExempt by tax rate
        public bool TE_ByRate => Convert.ToBoolean(GetPol("TE_ByRate", null));

        public string TE_Type => Convert.ToString(GetPol("TE_Type", null));

        // to keep "Do you provide a till float?" {Yes,No} - Boolean
        public bool TILL_FLOAT => Convert.ToBoolean(GetPol("TILL_FLOAT", null));

        //Only company level:  "End-of-day display method" {Confirm, Blind} - String
        public string U_EOD_DISP => Convert.ToString(GetPol("U_EOD_DISP", null));

        // to keep "Use Clear Till Warning" {Yes,No} - Boolean
        public bool USE_CL_TILL => Convert.ToBoolean(GetPol("USE_CL_TILL", null));

        // to keep "Use Customers" {Yes,No} - Boolean
        public bool USE_CUST => Convert.ToBoolean(GetPol("USE_CUST", null));



        public bool USE_KICKBACK => Convert.ToBoolean(GetPol("USE_KICKBACK", null));


        public short L_REDEEMPNTS => Convert.ToInt16(GetPol("L_REDEEMPNTS", null));

        // to keep "Do you use the fuel system?" {Yes,No} - Boolean
        public bool USE_FUEL => Convert.ToBoolean(GetPol("USE_FUEL", null));

        // to keep "Do you use the loyalty system?" {Yes,No} - Boolean
        public bool USE_LOYALTY
        {
            get
            {
                return Convert.ToBoolean(GetPol("USE_LOYALTY", null));
            }
            set { }
        }

        // to keep "Use Pin Pad entry." {Yes,No} - Boolean
        public bool USE_PINPAD => Convert.ToBoolean(GetPol("USE_PINPAD", null));

        // to keep "Do you sell propane?" {Yes,No} - Boolean
        public bool USE_PROPANE => Convert.ToBoolean(GetPol("USE_PROPANE", null));

        // to keep "Use Store Shifts" {Yes,No} - Boolean
        public bool USE_SHIFTS
        {
            get
            {
                return Convert.ToBoolean(GetPol("USE_SHIFTS", null));
            }
            set { }
        }

        // to keep "Use reasons for void sales ?" {Yes,No} - Boolean
        public bool VOID_REASON => Convert.ToBoolean(GetPol("VOID_REASON", null));

        // to keep "Strictly Enforce X-For Prices" {Yes,No} - Boolean
        public bool X_RIGOR => Convert.ToBoolean(GetPol("X_RIGOR", null));

        //Only company level:  "Do you want to track price change{Yes,No} - Boolean
        public bool PRICE_TRACK => Convert.ToBoolean(GetPol("PRICE_TRACK", null));

        //Only company level:  "Allow grouping by 'EOD Groupsin the Till Close Report?" {Yes,NO} - boolean
        public bool EOD_GROUP => Convert.ToBoolean(GetPol("EOD_GROUP", null));

        // to keep "Use OverRide code in Tax Exempt?" {Yes,No} - Boolean
        public bool USE_OVERRIDE => Convert.ToBoolean(GetPol("USE_OVERRIDE", null));

        //Only company level:  "Bank system" {Global Payment,Moneris,None} - String
        public string BankSystem => Convert.ToString(GetPol("BankSystem", null));

        //Allow Manual credit card processing & pinpad type
        public bool ManualCCard => Convert.ToBoolean(GetPol("ManualCCard", null));

        //PinpadType
        public string PinpadType => Convert.ToString(GetPol("PinpadType", null));

        //Only company level:  "Display Fuel Coupon Message?" {Yes,No} - boolean
        public bool CouponMSG => Convert.ToBoolean(GetPol("CouponMSG", null));

        //Only company level:  "Fuel Coupon message threshold value" {1-1000} - Integer
        public short CupnThrehld => Convert.ToInt16(GetPol("CupnThrehld", null));

        //Only company level:  "Fuel Coupon Type" {Fuel Coupon,Fuel Stamp,Litre Log} - String
        public string CouponType => Convert.ToString(GetPol("CouponType", null));

        // to keep for allow PostPay/Prepay "Allow Post Pay?" {Yes,No} - Boolean
        public bool AllowPostPay => Convert.ToBoolean(GetPol("AllowPostPay", null));

        //Allow Prepay?
        public bool AllowPrepay
        {
            get
            {
                return Convert.ToBoolean(GetPol("AllowPrepay", null));
            }
            set { }
        }


        public bool DebitSwipe => Convert.ToBoolean(GetPol("DebitSwipe", null));

        //Maximum times for Debit Swipe. TODO Remove
        public short MaxDBSwipe => Convert.ToInt16(GetPol("MaxDBSwipe", null));

        //Nancy added for Manual Fuel
        public bool AllowManual => Convert.ToBoolean(GetPol("AllowManual", null));

        //Nancy added for Close Batch when Til Close.
        public bool EOD_CLOSE => Convert.ToBoolean(GetPol("EOD_CLOSE", null));

        //Support Dip Input?
        public bool Dip_Input => Convert.ToBoolean(GetPol("Dip_Input", null));

        //Dip Input Time
        public DateTime DipInputTime => Convert.ToDateTime(GetPol("DipInputTime", null));

        //LastShift for ReadTotalizer in TillClose
        public short LastShift => Convert.ToInt16(GetPol("LastShift", null));

        //Support Fuel Loyalty?
        public bool FuelLoyalty
        {
            get
            {
                return Convert.ToBoolean(GetPol("FuelLoyalty", null));
            }
            set { }
        }

        //CouponTend for Fuel Loyalty coupon tender
        public string CouponTend => Convert.ToString(GetPol("CouponTend", null));

        //Take Loyalty Card from Scanner?
        public bool ScanLoyCard => Convert.ToBoolean(GetPol("ScanLoyCard", null));

        // to keep "What kind of Gift Certificate system do you support?" {NONE,LocalGift,GiveX,Milliplein} - string
        public string GiftType => Convert.ToString(GetPol("GiftType", null));

        //Take Gift Card from Scanner?
        public bool ScanGiftCard => Convert.ToBoolean(GetPol("ScanGiftCard", null));

        //User can log on more than one till
        public bool LogUnlimit
        {
            get
            {
                return Convert.ToBoolean(GetPol("Log_Unlimit", null));
            }
            set { }
        }

        //Till is locked automatically after inactivity?
        public bool LockTill => Convert.ToBoolean(GetPol("Lock_Till", null));

        //Use Envelope Number for Cash Drops?
        public bool DropEnv => Convert.ToBoolean(GetPol("DROP_ENV", null));

        //Show AR customer account balance in AR payment receit
        public bool ShowAccBal => Convert.ToBoolean(GetPol("ShowAccBal", null));

        //Added to print logo
        public bool PrintLogo => Convert.ToBoolean(GetPol("PRINT_LOGO", null));

        //Till/BackOffice is locked automatically after _____ minutes
        public short LockTime => Convert.ToInt16(GetPol("Lock_Time", null));

        //How many copies do you want to print Tax Exempt Voucher?
        public short TaxExemptVoucherCopies => Convert.ToInt16(GetPol("Vouch_Copy", null));

        //What's the age restriction for tobacco sale?
        public short AgeRestrict => Convert.ToInt16(GetPol("AgeRestrict", null));

        //Receipt copies for different sale types

        public short RefundReceiptCopies => Convert.ToInt16(GetPol("PRINT_COPIES", new Sale_Type { SaleType = "REFUND" }));

        //Number of copies for AR pay receipts
        public short ArpayReceiptCopies => Convert.ToInt16(GetPol("PRINT_COPIES", new Sale_Type { SaleType = "ARPAY" }));

        //Number of copies for Payout receipts
        public short PayoutReceiptCopies => Convert.ToInt16(GetPol("PRINT_COPIES", new Sale_Type { SaleType = "PAYOUT" }));

        //Number of copies for Payment receipts
        public short PaymentReceiptCopies => Convert.ToInt16(GetPol("PRINT_COPIES", new Sale_Type { SaleType = "PAYMENT" }));

        //Number of copies for Bottle return receipts
        public short BottleReturnReceiptCopies => Convert.ToInt16(GetPol("PRINT_COPIES", new Sale_Type { SaleType = "BOTTLERETURN" }));

        //Number of copies for CashDrop receipt copies
        public short CashDropReceiptCopies => Convert.ToInt16(GetPol("PRINT_COPIES", new Sale_Type { SaleType = "CASHDROP" }));

        //Number of copies for CashDraw receipt copies
        public short CashDrawReceiptCopies => Convert.ToInt16(GetPol("PRINT_COPIES", new Sale_Type { SaleType = "CASHDRAW" }));

        //Check up-sell sales? TODO:Remove
        public bool CheckUpsell => Convert.ToBoolean(GetPol("CheckUpsell", null));

        //Support input messages?
        public bool Msg_Input => Convert.ToBoolean(GetPol("Msg_Input", null));

        //Support cashier authorize for pay@pump?
        public bool CashAuthPP => Convert.ToBoolean(GetPol("CashAuthPP", null));

        //Display confirm message to stop pump?
        public bool StopMsg => Convert.ToBoolean(GetPol("StopMsg", null));

        //Allow stack fuel sale?
        public bool AllowStack => Convert.ToBoolean(GetPol("AllowStack", null));

        //Compute tax on taxes?
        public bool TAX_COMP => Convert.ToBoolean(GetPol("TAX_COMP", null));

        //Allow Minimize POS window?
        public bool AllowMin => Convert.ToBoolean(GetPol("AllowMin", null));

        //Print Card Holder Agreement for Fleet Card?
        public bool PRINT_CrdHld => Convert.ToBoolean(GetPol("PRINT_CrdHld", null));

        //Support Scale Item and Loto-Quebec?
        public bool Scale_Item => Convert.ToBoolean(GetPol("Scale_Item", null));

        //Support Customer Fuel Rebate?
        public bool FuelRebate => Convert.ToBoolean(GetPol("FuelRebate", null));

        //Input serial number for vendor coupon?
        public bool CpnSerialID => Convert.ToBoolean(GetPol("CpnSerialID", null));

        //Type of cost used_____ TODO: Not Used
        public string COST_TYPE => Convert.ToString(GetPol("COST_TYPE", null));

        //TODO: Not Used Use automation for TRA Files processing?
        public bool TE_AUTOMATE => Convert.ToBoolean(GetPol("TE_AUTOMATE", null));

        //How many digits of SITE treaty number card?
        public short TreatyNumDgt => Convert.ToInt16(GetPol("TreatyNumDgt", null));

        //Print exempted tax in receipt?
        public bool PrtExmptTax => Convert.ToBoolean(GetPol("PrtExmptTax", null));

        //What's the UPC format system?
        public string UPC_Format => Convert.ToString(GetPol("UPC_Format", null));

        //How many digits of the item code for scalable items?
        public short ItemCodeDgt => Convert.ToInt16(GetPol("ItemCodeDgt", null));

        //How many digits of Amount/Weight for scalable items?
        public short AmountDigit => Convert.ToInt16(GetPol("AmountDigit", null));

        //Do you print weight in scalable item UPC?
        public bool UseWeight => Convert.ToBoolean(GetPol("UseWeight", null));

        //Default to read totalizer while setting fuel price.
        public bool DftRdTotal => Convert.ToBoolean(GetPol("DftRdTotal", null));

        //Support Tank Gauge System integration?
        public bool TankGauge => Convert.ToBoolean(GetPol("TankGauge", null));

        //Default to process tank dip reading when setting fuel price.
        public bool DftRdTankDip => Convert.ToBoolean(GetPol("DftRdTankDip", null));

        //Alarm sound repeat interval (min)
        public short SOUND_ALARM => Convert.ToInt16(GetPol("SOUND_ALARM", null));

        //Remain in Fuel Price Screen until prices are set?
        public bool FuelPriceChg => Convert.ToBoolean(GetPol("FuelPriceChg", null));

        //Print User Name on Receipts?
        public bool PRN_UName => Convert.ToBoolean(GetPol("PRN_UName", null));

        //Is it 24 Hour Store?
        public bool Hour24Store => Convert.ToBoolean(GetPol("Hour24Store", null));

        //Automatically pick the next shift (it's not based on time)?
        public bool AutoShftPick => Convert.ToBoolean(GetPol("AutoShftPick", null));

        //Show customer note if the customer is over limit(Y\N)?
        public bool CreditMsg => Convert.ToBoolean(GetPol("CreditMsg", null));

        //Authorize pump by pos id?
        public bool AuthPumpPOS => Convert.ToBoolean(GetPol("AuthPumpPOS", null));

        //Display Parent Account if customer card is linked(Y\N)?
        public bool ShowCardCustomers => Convert.ToBoolean(GetPol("ShowCardCust", null));

        //Support KickBack Reward Points?
        public bool Use_KickBack => Convert.ToBoolean(GetPol("Use_KickBack", null));

        //Customized message
        public string LoyaltyMesg => Convert.ToString(GetPol("LoyaltyMesg", null));

        //IP Address of KickBack TPS
        public string KICKBACK_IP => Convert.ToString(GetPol("KICKBACK_IP", null));

        //TCP listening port of KickBack TPS
        public string KICKBACK_PRT => Convert.ToString(GetPol("KICKBACK_PRT", null));

        //KickBack TPS communication Timeout
        public string KICKBACK_TMT => Convert.ToString(GetPol("KICKBACK_TMT", null));

        //Be able to authorize pumps when closing the till?
        public bool AuthPump_Eod
        {
            get
            {
                return Convert.ToBoolean(GetPol("AuthPump_Eod", null));
            }
            set { }
        }

        //Use default customer
        public bool DefaultCust
        {
            get
            {
                return Convert.ToBoolean(GetPol("DefaultCust", null));
            }
            set { }
        }

        //Default customer is
        public string DEF_CUST_CODE => Convert.ToString(GetPol("DEFCUST_CODE", null));

        //Print coupon on a separate receipt?
        public bool PRT_CPN => Convert.ToBoolean(GetPol("PRT_CPN", null));

        //TODO: Not in scope
        public bool CashBonus => Convert.ToBoolean(GetPol("CASHBONUS", null));

        //Cash Bonus Name
        public string CBonusName => Convert.ToString(GetPol("CBonusName", null));

        //Do you provide till Cash Bonus float??
        public bool CBonusFloat => Convert.ToBoolean(GetPol("CBonusFloat", null));

        //Do you support till Cash Bonus Draw\Drop?
        public bool CBonusDraw => Convert.ToBoolean(GetPol("CBonusDraw", null));

        //Minimum num. of points needed to display redemption message TODO:Remove
        public double L_RedeemPnts => Convert.ToDouble(GetPol("L_RedeemPnts", null));

        //Tender Name for Cash Bonus Program:
        public string CBonusTend => Convert.ToString(GetPol("CBonusTend", null));

        //TODO: Need to Remove
        public bool TidelSafe => Convert.ToBoolean(GetPol("TidelSafe", null));

        //Sell Inactive Item?
        public bool Sell_Inactive => Convert.ToBoolean(GetPol("SELL_INACT", null));

        //Charge the Customer Account when using Customer Card?
        public bool Charge_Acct => Convert.ToBoolean(GetPol("Charge_Acct", null));

        //Support Store & Forward(SAF) Transactions?
        public bool Support_SAF => Convert.ToBoolean(GetPol("Support_SAF", null));

        //Allow only 'Call Bank Authorization' for SAF Transactions?
        public bool CallBankOnly => Convert.ToBoolean(GetPol("CallBankOnly", null));

        //Bank transaction timeout in seconds
        public short BankTimeOutSec => Convert.ToInt16(GetPol("BankTimeOutSec", null));

        //Tender Name for Customer AR Account:
        public string ARTender => Convert.ToString(GetPol("ARTender", null));

        //Required to Collect Tax Exempt Customer Name?
        public bool TE_GETNAME => Convert.ToBoolean(GetPol("TE_GETNAME", null));

        //Required to Collect Tax Exempt Customer Signature?
        public bool TE_SIGNATURE => Convert.ToBoolean(GetPol("TE_SIGNATURE", null));

        //Mode for collecting Tax Exempt Customer Signature:
        public string TE_SIGNMODE => Convert.ToString(GetPol("TE_SIGNMODE", null));

        //Track Tax Exempt Inventory Separately?
        public bool TRACK_TEINV => Convert.ToBoolean(GetPol("TRACK_TEINV", null));

        //Fuel Only System?
        public bool FUELONLY
        {
            get
            {
                return Convert.ToBoolean(GetPol("FUELONLY", null));
            }
            set { }
        }

        //IP address for Integrated System?
        public string FUELONLY_IP => Convert.ToString(GetPol("FUELONLY_IP", null));

        //TCP listening port for Integrated System
        public short FUELONLY_PRT => Convert.ToInt16(GetPol("FUELONLY_PRT", null));

        //Integrated System communication timeout
        public short FUELONLY_TMT => Convert.ToInt16(GetPol("FUELONLY_TMT", null));

        //Supports Real-Time Validation for SITE?
        public bool SITE_RTVAL => Convert.ToBoolean(GetPol("SITE_RTVAL", null));

        //Supports EMV Compliance?
        public bool EMVVersion => Convert.ToBoolean(GetPol("EMVVersion", null));

        //TODO: Remove
        public bool SUPPORTEZI => Convert.ToBoolean(GetPol("SUPPORTEZI", null));

        //TODO: Remove
        public bool EziDept => Convert.ToBoolean(GetPol("Ezi_Dept", null));

        //Supports Private Card Restriction Profiles? 
        public bool RSTR_PROFILE => Convert.ToBoolean(GetPol("RSTR_PROFILE", null));

        //Support tax rebate?
        public bool Tax_Rebate => Convert.ToBoolean(GetPol("Tax_Rebate", null));

        //Maximum amount to qualify for tax rebate?
        public decimal TX_RB_AMT => Convert.ToDecimal(GetPol("TX_RB_AMT", null));

        //Customer settings linked to Band Member:
        public string BANDMEMBER => Convert.ToString(GetPol("BANDMEMBER", null));

        //Customer settings linked to Non Band Member:
        public string NONBANDMEMBER => Convert.ToString(GetPol("NONBANDMEMBER", null));

        //Do you support Band Account for Tax Exempt Sales?
        public bool ENABLE_BANDACCT => Convert.ToBoolean(GetPol("ENABLE_BANDACCT", null));

        //Band Account Number to identify the band member:
        public string MEMBER_IDENTITY => Convert.ToString(GetPol("MEMBER_IDENTITY", null));

        //Automatically identify the Band Member using Band Account?
        public bool IDENTIFY_MEMBER => Convert.ToBoolean(GetPol("IDENTIFY_MEMBER", null));

        //Print Store Code on Receipts?
        public bool PRN_CO_CODE => Convert.ToBoolean(GetPol("PRN_CO_CODE", null));

        //TODO: Remove
        public bool SAFEATMDROP => Convert.ToBoolean(GetPol("SAFE&ATMDROP", null));

        //Keypad format in login screen
        public string KP_LOGIN => Convert.ToString(GetPol("KP_LOGIN", null));

        //Prompt user for dip readings in POS till close
        public bool ASK_DIPREAD => Convert.ToBoolean(GetPol("ASK_DIPREAD", null));

        //Support fuel price change from HeadOffice?
        public bool FUELPR_HO => Convert.ToBoolean(GetPol("FUELPR_HO", null));

        //Notify about fuel price changes from HeadOffice __times
        public short FPR_NOTE_CNT => Convert.ToInt16(GetPol("FPR_NOTE_CNT", null));

        //Notify about fuel price changes from HeadOffice every __ min
        public int FPR_TIME => Convert.ToInt32(GetPol("FPR_TIME", null));

        //Use predefined till number
        public bool TILL_NUM => Convert.ToBoolean(GetPol("TILL_NUM", null));

        //Supports EMV Compliance at the pump?
        public bool EMVVERSION_PATP => Convert.ToBoolean(GetPol("EMVVERSION_PATP", null));

        //Scan customer cards in POS to identify the customer?
        public bool CUST_SCAN => Convert.ToBoolean(GetPol("CUST_SCAN", null));

        //Swipe customer cards in POS to identify the customer?
        public bool CUST_SWP => Convert.ToBoolean(GetPol("CUST_SWP", null));

        //TODO: Remove
        public string EXC_CASHBONUS => Convert.ToString(GetPol("EXC_CASHBONUS", null));

        //Verify expiry date on customer cards?
        public bool CUST_EXPDATE => Convert.ToBoolean(GetPol("CUST_EXPDATE", null));

        //Use Penny Adjustment
        public bool PENNY_ADJ => Convert.ToBoolean(GetPol("PENNY_ADJ", null));

        //Login using Windows User
        public bool WINDOWS_LOGIN => Convert.ToBoolean(GetPol("WINDOWS_LOGIN", null));

        //Print receipt for prepay overpayment?
        public bool PRT_OVERPAY => Convert.ToBoolean(GetPol("PRT_OVERPAY", null));

        //Enable Wex Fleet Card
        public bool WEXEnabled => Convert.ToBoolean(GetPol("WEXEnabled", null));

        //Wex Fleet TPS Server IP
        public string WexTpsIP => Convert.ToString(GetPol("WexTpsIP", null));

        //Wex Fleet TPS Server Port
        public int WexTpsPort => Convert.ToInt32(GetPol("WexTpsPort", null));

        //TODO: Remove
        public bool FSGC_ENABLE => Convert.ToBoolean(GetPol("FSGC_ENABLE", null));

        //TODO: Remove
        public double FSGC_CREDIT => Convert.ToDouble(GetPol("FSGC_CREDIT", null));

        //TODO: Remove
        public double FSGC_OTHER => Convert.ToDouble(GetPol("FSGC_OTHER", null));

        //TODO: Remove
        public string FSGC_PLU => Convert.ToString(GetPol("FSGC_PLU", null));

        //TODO: Remove
        public short FSGC_EXP => Convert.ToInt16(GetPol("FSGC_EXP", null));

        //TODO: Remove
        public string FSGC_CalcType => Convert.ToString(GetPol("FSGC_CalcType", null));

        //Enable ACKROO Loyalty and Gift Card
        public bool REWARDS_Enabled => Convert.ToBoolean(GetPol("REWARDS_Enabled", null));

        //Customized message
        public string REWARDS_Message => Convert.ToString(GetPol("REWARDS_Message", null));

        //TODO: Remove
        public string REWARDS_Caption => Convert.ToString(GetPol("REWARDS_Caption", null));

        //TODO: Remove
        public bool REWARDS_NoRedmpt => Convert.ToBoolean(GetPol("REWARDS_NoRedmpt", null));

        //ACKROO TPS Server IP
        public string REWARDS_TpsIp => Convert.ToString(GetPol("REWARDS_TpsIp", null));

        //ACKROO TPS Server Port
        public int REWARDS_TpsPort => Convert.ToInt32(GetPol("REWARDS_TpsPort", null));

        //ACKROO GiftCard
        public string REWARDS_Gift => Convert.ToString(GetPol("REWARDS_Gift", null));

        //Ackroo TPS communication Timeout
        public short REWARDS_Timeout => Convert.ToInt16(GetPol("REWARDS_Timeout", null));

        //Support tax exemption for Government Agencies
        public bool TAX_EXEMPT_GA => Convert.ToBoolean(GetPol("TAX_EXEMPT_GA", null));

        //Maximum threshold for fuel price change is_____.
        public double FUEL_MAXTH => Convert.ToDouble(GetPol("FUEL_MAXTH", null));

        //Use Ontario Gas Tax Refunds E-Services
        public bool TAX_EXEMPT_FNGTR => Convert.ToBoolean(GetPol("TAX_EXEMPT_FNGTR", null));

        //TODO Removed
        public bool FSGC_ARTENDER => Convert.ToBoolean(GetPol("FSGC_ARTENDER", null));

        //Support cash/debit fuel discount?
        public bool CADB_FUELDISC => Convert.ToBoolean(GetPol("CADB_FUELDISC", null));

        //TODO: Remove
        public string TE_COLLECTTAX => Convert.ToString(GetPol("TE_COLLECTTAX", null));

        //Shift number to reprint till close report for the day
        public short PRT_ALLSHIFTS => Convert.ToInt16(GetPol("PRT_ALLSHIFTS", null));


        public string GIVEX_IP => Convert.ToString(GetPol("GIVEX_IP", null));

        public string GIVEX_PORT => Convert.ToString(GetPol("GIVEX_PORT", null));

        public int GIVETIMEOUT => Convert.ToInt16(GetPol("GIVETIMEOUT", null));

        public string GIVEX_USER => Convert.ToString(GetPol("GIVEX_USER", null));

        public string GIVEX_PASS => Convert.ToString(GetPol("GIVEX_PASS", null));

        public string GiveXMerchID => Convert.ToString(GetPol("GiveXMerchID", null));

        public string TIMEFORMAT => Convert.ToString(GetPol("TIMEFORMAT", null));

        #endregion

        #region Policies for Carwash
        public bool IsCarwashSupported => Convert.ToBoolean(GetPol("CarWash", null));
        public string CarwashDepartment => Convert.ToString(GetPol("Carwash_Dep", null));
        public bool IsCarwashIntegrated => Convert.ToBoolean(GetPol("Carwash_Int", null));
        public bool SupportCarwashAtPump => Convert.ToBoolean(GetPol("Carwash_PATP", null));
        public string CarwashIP => Convert.ToString(GetPol("CW_IP", null));
        public int CarwashPort => Convert.ToInt16(GetPol("CW_Port", null));
        public int CarwashTout => Convert.ToInt16(GetPol("CW_Tout", null));

        #endregion

        #region polices for wex

        public bool IsWexEnable => Convert.ToBoolean(GetPol("WEXEnabled",null));
        public string WexIp => Convert.ToString(GetPol("WEXTPSIP",null));
        public int WexPort => Convert.ToInt16(GetPol("WEXTPSPORT",null));
        #endregion


        #region User Level Policies


        public bool USE_ARCUST => Convert.ToBoolean(GetPol("USE_ARCUST", GetCurrentUser(UserCode)));

        public bool Store_Credit => Convert.ToBoolean(GetPol("Store_Credit", GetCurrentUser(UserCode)));

        public bool MEMBER_CODE => Convert.ToBoolean(GetPol("MEMBER_CODE", GetCurrentUser(UserCode)));

        public bool KITS_IN_PO => Convert.ToBoolean(GetPol("KITS_IN_PO", GetCurrentUser(UserCode)));

        public bool ALLOW_MARKDO => Convert.ToBoolean(GetPol("ALLOW_MARKDO", GetCurrentUser(UserCode)));

        public bool U_ARSALES => Convert.ToBoolean(GetPol("U_ARSALES", GetCurrentUser(UserCode)));

        public short U_AUTH_LEVEL => Convert.ToInt16(GetPol("U_AUTH_LEVEL", GetCurrentUser(UserCode)));

        public bool U_BANK_CL => Convert.ToBoolean(GetPol("U_BANK_CL", GetCurrentUser(UserCode)));

        public bool U_CAN_VOID => Convert.ToBoolean(GetPol("U_CAN_VOID", GetCurrentUser(UserCode)));

        public bool U_CHGFPRICE => Convert.ToBoolean(GetPol("U_CHGFPRICE", GetCurrentUser(UserCode)));

        public bool U_CHGPRICE => Convert.ToBoolean(GetPol("U_CHGPRICE", GetCurrentUser(UserCode)));

        public bool U_CHGQTY => Convert.ToBoolean(GetPol("U_CHGQTY", GetCurrentUser(UserCode)));

        public bool U_DISCOUNTS => Convert.ToBoolean(GetPol("U_DISCOUNTS", GetCurrentUser(UserCode)));

        public bool U_FUELGP => Convert.ToBoolean(GetPol("U_FUELGP", GetCurrentUser(UserCode)));

        public bool U_GIVEREF => Convert.ToBoolean(GetPol("U_GIVEREF", GetCurrentUser(UserCode)));

        public bool U_OR_LIMIT => Convert.ToBoolean(GetPol("U_OR_LIMIT", GetCurrentUser(UserCode)));

        public bool U_OVERPLIMIT => Convert.ToBoolean(GetPol("U_OVERPLIMIT", GetCurrentUser(UserCode)));

        public bool U_REQ_PW => Convert.ToBoolean(GetPol("U_REQ_PW", GetCurrentUser(UserCode)));

        public bool U_SELL => Convert.ToBoolean(GetPol("U_SELL", GetCurrentUser(UserCode)));

        public bool U_TILLAUDIT => Convert.ToBoolean(GetPol("U_TILLAUDIT", GetCurrentUser(UserCode)));

        public bool U_TILLCLOSE => Convert.ToBoolean(GetPol("U_TILLCLOSE", GetCurrentUser(UserCode)));

        public bool U_TILLDRAW => Convert.ToBoolean(GetPol("U_TILLDRAW", GetCurrentUser(UserCode)));

        public bool U_TILLDROP => Convert.ToBoolean(GetPol("U_TILLDROP", GetCurrentUser(UserCode)));

        public bool U_TOTREAD => Convert.ToBoolean(GetPol("U_TOTREAD", GetCurrentUser(UserCode)));

        public bool VOID_AUTH => Convert.ToBoolean(GetPol("VOID_AUTH", GetCurrentUser(UserCode)));

        public bool U_ManualF => Convert.ToBoolean(GetPol("U_ManualF", GetCurrentUser(UserCode)));

        public float U_BR_LIMIT => Convert.ToSingle(GetPol("U_BR_LIMIT", GetCurrentUser(UserCode)));

        public bool U_BOTTLERTN => Convert.ToBoolean(GetPol("U_BOTTLERTN", GetCurrentUser(UserCode)));

        public bool U_AddStock => Convert.ToBoolean(GetPol("U_AddStock", GetCurrentUser(UserCode)));

        public bool U_CHGPASS => Convert.ToBoolean(GetPol("U_CHGPASS", GetCurrentUser(UserCode)));

        public bool U_RUNAWAY => Convert.ToBoolean(GetPol("U_RUNAWAY", GetCurrentUser(UserCode)));

        public bool U_ManuFPrice => Convert.ToBoolean(GetPol("U_ManuFPrice", GetCurrentUser(UserCode)));

        public bool U_DipRead => Convert.ToBoolean(GetPol("U_DipRead", GetCurrentUser(UserCode)));

        public bool U_AllowFlPay => Convert.ToBoolean(GetPol("U_ALLOWFLPAY", GetCurrentUser(UserCode)));

        public bool U_OPENDRW => Convert.ToBoolean(GetPol("U_OPENDRW", GetCurrentUser(UserCode)));

        public bool U_SUSP => Convert.ToBoolean(GetPol("U_SUSP", GetCurrentUser(UserCode)));

        public bool U_EXACTCHG => Convert.ToBoolean(GetPol("EXACTCHG", GetCurrentUser(UserCode)));

        public bool U_FM => Convert.ToBoolean(GetPol("U_FUELMGMT", GetCurrentUser(UserCode)));

        public bool U_PUMPTEST => Convert.ToBoolean(GetPol("U_PUMPTEST", GetCurrentUser(UserCode)));

        public bool U_INITDEBIT => Convert.ToBoolean(GetPol("U_INITDEBIT", GetCurrentUser(UserCode)));

        public bool U_AD_PI => Convert.ToBoolean(GetPol("U_AD_PI", GetCurrentUser(UserCode)));

        public bool FPR_USER => Convert.ToBoolean(GetPol("FPR_USER", GetCurrentUser(UserCode)));

        public bool U_OR_TEQUOTA => Convert.ToBoolean(GetPol("U_OR_TEQUOTA", GetCurrentUser(UserCode)));

        #endregion

        public Store LoadStoreInfo()
        {
            var store = CacheManager.GetStoreInfo();
            if (store == null)
            {
                store = _loginService.LoadStoreInfo();
                CacheManager.AddStoreInfo(store);
            }
            return store;
        }

        public Security LoadSecurityInfo()
        {
            var security = CacheManager.GetSecurityInfo();
            if (security == null)
            {
                security = _loginService.LoadSecurityInfo();
                CacheManager.AddSecurityInfo(security);
            }
            return security;
        }

        public void SetUpPolicy(Security security)
        {
            _policyService.SetUpPolicy(security);
        }

        private void InitializePolicies()
        {
            CacheManager.RemovePromotionalItems("");
            CacheManager.DeletePromosForToday();
            CacheManager.DeleteStoreInfo();
            CacheManager.DeleteSecurityInfo();
            _policies = _policyService.LoadAllPolicies();
            _policiesCanbe = _policyService.LoadAllPolicyCanbe();
            _policiesSet = _policyService.LoadAllPolicySet();
            CacheManager.AddPoliciesForPos(_policies);
            CacheManager.AddPoliciesCanbeForPos(_policiesCanbe);
            CacheManager.AddPoliciesSetForPos(_policiesSet);

            _allPolicies = _policyService.GetAllPolicies();
            CacheManager.AddAllPoliciesForPos(_allPolicies);
        }

        private void InitializePolicyData()
        {
            //CacheManager.RemovePromotionalItems("");
            //CacheManager.DeletePromosForToday();
            //CacheManager.DeleteStoreInfo();
            //CacheManager.DeleteSecurityInfo();

            _policies = CacheManager.GetPoliciesForPos();
            if (_policies == null)
            {
                _policies = _policyService.LoadAllPolicies();
                CacheManager.AddPoliciesForPos(_policies);
            }

            _policiesCanbe = CacheManager.GetPoliciesCanbeForPos();
            if (_policiesCanbe == null)
            {
                _policiesCanbe = _policyService.LoadAllPolicyCanbe();
                CacheManager.AddPoliciesCanbeForPos(_policiesCanbe);
            }

            _policiesSet = CacheManager.GetPoliciesSetForPos();
            if (_policiesSet == null)
            {
                _policiesSet = _policyService.LoadAllPolicySet();
                CacheManager.AddPoliciesSetForPos(_policiesSet);
            }

            _allPolicies = CacheManager.GetAllPoliciesForPos();
            if (_allPolicies == null)
            {
                _allPolicies = _policyService.GetAllPolicies();
                CacheManager.AddAllPoliciesForPos(_allPolicies);
            }
        }

        /// <summary>
        /// Get Policy by Name
        /// </summary>
        /// <param name="policyName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public dynamic GetPol(string policyName, dynamic obj)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PolicyManager,GetPol,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            dynamic returnValue = default(dynamic);
            if (_allPolicies == null)
            {
                InitializePolicyData();
            }
            var policy = _allPolicies.Where(p => p.PolicyName.ToUpper() == policyName.ToUpper());

            var backOfficePolicies = policy as BackOfficePolicy[] ?? policy.ToArray();
            if (!backOfficePolicies.Any())
            {
                returnValue = _policyService.AddPolicy(policyName);
                _allPolicies = _policyService.GetAllPolicies();
                CacheManager.AddAllPoliciesForPos(_allPolicies);
                policy = _allPolicies.Where(p => p.PolicyName.ToUpper() == policyName.ToUpper());
                backOfficePolicies = policy as BackOfficePolicy[] ?? policy.ToArray();
            }
            if (!backOfficePolicies.Any())
            {
                return returnValue;
            }

            var companyLevelPolicy = backOfficePolicies[0];

            switch (companyLevelPolicy.VarType)
            {
                case "L":
                    returnValue = false;
                    if (Strings.Left(Convert.ToString(companyLevelPolicy.CompanyLevelSet), 1).ToUpper() == "Y")
                    {

                        if (companyLevelPolicy.Implemented)
                        {
                            returnValue = true;
                        }
                    }
                    break;
                case "N":
                    returnValue = Conversion.Val(companyLevelPolicy.CompanyLevelSet);
                    break;
                case "C":
                    returnValue = companyLevelPolicy.CompanyLevelSet;
                    break;
                case "T":
                    returnValue = Convert.ToDateTime(companyLevelPolicy.CompanyLevelSet).ToString("hh:mm:ss");
                    break;
            }

            if (obj != null)
            {
                var rsCanbe =
                    backOfficePolicies.Where(
                        p =>
                            p.PolicyName.ToUpper() == policyName.ToUpper() && p.PolicyLevel != "COMPANY" &&
                            p.OtherLevelSet != "" && p.PolicyLevel != "").OrderBy(p => p.Sequence);

                if (rsCanbe.Any())
                {
                    foreach (BackOfficePolicy policyCanbe in rsCanbe)
                    {
                        object parmVal = Constants.vbNullString;

                        switch (policyCanbe.PolicyLevel)
                        {
                            case "USER":
                                parmVal = obj.Code;
                                break;
                            case "UGROUP":
                                parmVal = obj.User_Group.Code;
                                break;
                            case "STOCK":
                                parmVal = obj.Stock_Code;
                                break;
                            case "SUBDETAIL":
                                parmVal = obj.Dept + Strings.Chr(255) + obj.Sub_Dept +
                                          Convert.ToString(Strings.Chr(255)) + obj.Sub_Detail;
                                break;
                            case "SUBDEPT":
                                parmVal = obj.Dept + Strings.Chr(255) + obj.Sub_Dept;
                                break;
                            case "DEPT":
                                parmVal = obj.Dept;
                                break;
                            case "SALETYPE":
                                parmVal = obj.SaleType;
                                break;
                            case "TENDER":
                                parmVal = obj.Tender_Code;
                                break;
                        }

                        if ((string)parmVal != Constants.vbNullString)
                        {
                            var rsSet = rsCanbe.FirstOrDefault(p => p.PolicyName.ToUpper() == policyName.ToUpper() && p.PolicyLevel == policyCanbe.PolicyLevel && p.PolicyValue.ToUpper() == Convert.ToString(parmVal).ToUpper());
                            if (rsSet != null)
                            {
                                switch (policyCanbe.VarType)
                                {
                                    case "L":
                                        returnValue =
                                            Strings.Left(Convert.ToString(rsSet.OtherLevelSet), 1).ToUpper() == "Y";
                                        break;
                                    case "N":
                                        returnValue = Conversion.Val(rsSet.OtherLevelSet);
                                        break;
                                    case "C":
                                        returnValue = rsSet.OtherLevelSet;
                                        break;
                                    case "T":
                                        returnValue = Convert.ToDateTime(rsSet.OtherLevelSet).ToString("hh:mm:ss");
                                        break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            Performancelog.Debug($"End,PolicyManager,GetPol,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnValue;

        }









        ///// <summary>
        ///// Get Policy by Name
        ///// </summary>
        ///// <param name="policyName"></param>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public dynamic GetPol(string policyName, dynamic obj)
        //{
        //    var dateStart = DateTime.Now;
        //    Performancelog.Debug($"Start,PolicyManager,GetPol,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

        //    dynamic returnValue = default(dynamic);
        //    if (_policies == null)
        //    {
        //        InitializePolicyData();
        //    }
        //    var policy = _policies.FirstOrDefault(p => p.PolicyName.ToUpper() == policyName.ToUpper());
        //    // Make sure that there is such a policy.
        //    //_rsComp.Find("P_NAME=\'" + policyName + "\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);

        //    if (policy == null) //If rsPComp.EOF Then
        //    {
        //        returnValue = _policyService.AddPolicy(policyName);
        //        _policies = _policyService.LoadAllPolicies();
        //        CacheManager.AddPoliciesForPos(_policies);
        //        policy = _policies.FirstOrDefault(p => p.PolicyName.ToUpper() == policyName.ToUpper());
        //    }
        //    if (policy == null)
        //    {
        //        return returnValue; // again if we can't find the policy just exit out -  
        //    }

        //    // Determine the company level setting.
        //    if (policy.VarType == "L") // Set as a Boolean
        //    {
        //        returnValue = false;
        //        if (Strings.Left(Convert.ToString(policy.Value), 1).ToUpper() == "Y")
        //        {

        //            if (policy.Implemented)
        //            {
        //                returnValue = true; 
        //            }
        //        }
        //    }
        //    else if (policy.VarType == "N")
        //    {
        //        returnValue = Conversion.Val(policy.Value);
        //    }
        //    else if (policy.VarType == "C")
        //    {
        //        returnValue = policy.Value;
        //    }
        //    else if (policy.VarType == "T")
        //    {
        //        returnValue = Convert.ToDateTime(policy.Value).ToString("hh:mm:ss");
        //    }

        //    // Don't bother to search below company level if no entity info was provided.
        //    if (obj != null)
        //    {
        //        // Retrieve all the settings at other levels
        //        var rsCanbe = _policiesCanbe.Where(p => p.PolicyName.ToUpper() == policyName.ToUpper() && p.CanBe != "COMPANY").OrderBy(p => p.Sequence);
        //        // No settings below company level. Use the company setting.
        //        //if (rsCanbe.Any())
        //        //{
        //        // Work your way up the policy heirarchy until you find a setting that
        //        // applies to the current object.
        //        foreach (PolicyCanbe policyCanbe in rsCanbe)
        //        {
        //            object parmVal = Constants.vbNullString;

        //            switch (policyCanbe.CanBe)
        //            {
        //                case "USER":
        //                    parmVal = obj.Code;
        //                    break;
        //                case "UGROUP":
        //                    parmVal = obj.User_Group.Code;
        //                    break;
        //                case "STOCK":
        //                    parmVal = obj.Stock_Code;
        //                    break;
        //                case "SUBDETAIL":
        //                    parmVal = obj.Dept + Strings.Chr(255) + obj.Sub_Dept + Convert.ToString(Strings.Chr(255)) + obj.Sub_Detail;
        //                    break;
        //                case "SUBDEPT":
        //                    parmVal = obj.Dept + Strings.Chr(255) + obj.Sub_Dept;
        //                    break;
        //                case "DEPT":
        //                    parmVal = obj.Dept;
        //                    break;
        //                case "SALETYPE":
        //                    parmVal = obj.SaleType;
        //                    break;
        //                case "TENDER":
        //                    parmVal = obj.Tender_Code;
        //                    break;
        //            }

        //            //if (policyCanbe.CanBe == "USER")
        //            //{
        //            //    parmVal = obj.Code;
        //            //}
        //            //else if (policyCanbe.CanBe == "UGROUP")
        //            //{
        //            //    parmVal = obj.User_Group.Code;
        //            //}
        //            //else if (policyCanbe.CanBe == "STOCK")
        //            //{
        //            //    parmVal = obj.Stock_Code;
        //            //}
        //            //else if (policyCanbe.CanBe == "SUBDETAIL")
        //            //{
        //            //    parmVal = obj.Dept + Strings.Chr(255) + obj.Sub_Dept + Convert.ToString(Strings.Chr(255)) + obj.Sub_Detail;
        //            //}
        //            //else if (policyCanbe.CanBe == "SUBDEPT")
        //            //{
        //            //    parmVal = obj.Dept + Strings.Chr(255) + obj.Sub_Dept;
        //            //}
        //            //else if (policyCanbe.CanBe == "DEPT")
        //            //{
        //            //    parmVal = obj.Dept;
        //            //}
        //            //else if (policyCanbe.CanBe == "SALETYPE")
        //            //{
        //            //    parmVal = obj.SaleType;
        //            //}
        //            //else if (policyCanbe.CanBe == "TENDER")
        //            //{
        //            //    parmVal = obj.Tender_Code;
        //            //}

        //            // If the level is defined then look it up in the table.
        //            if ((string)parmVal != Constants.vbNullString)
        //            {

        //                var rsSet = _policiesSet.FirstOrDefault(p => p.PolicyName.ToUpper() == policyName.ToUpper() && p.Level == policyCanbe.CanBe && p.Value == Convert.ToString(parmVal));
        //                //GetRecords("select * from P_Set where " + "P_NAME=\'" + policyName + "\' and P_LEVEL=\'" + Convert.ToString(rsCanbeFields["P_Canbe"].Value) + "\' and P_VALUE=\'" + Convert.ToString(parmVal) + "\'", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly);

        //                // If we found it then we have the correct policy. Grab it's setting
        //                // and return the value.
        //                if (rsSet != null)
        //                {

        //                    switch (policy.VarType)
        //                    {
        //                        case "L":
        //                            returnValue = Strings.Left(Convert.ToString(rsSet.Set), 1).ToUpper() == "Y";
        //                            break;
        //                        case "N":
        //                            returnValue = Conversion.Val(rsSet.Set);
        //                            break;
        //                        case "C":
        //                            returnValue = rsSet.Set;
        //                            break;
        //                        case "T":
        //                            returnValue = Convert.ToDateTime(rsSet.Set).ToString("hh:mm:ss");
        //                            break;
        //                    }

        //                    //if (policy.VarType == "L")
        //                    //{
        //                    //    returnValue = Strings.Left(Convert.ToString(rsSet.Set), 1).ToUpper() == "Y";
        //                    //}
        //                    //else if (policy.VarType == "N")
        //                    //{
        //                    //    returnValue = Conversion.Val(rsSet.Set);
        //                    //}
        //                    //else if (policy.VarType == "C")
        //                    //{
        //                    //    returnValue = rsSet.Set;
        //                    //}
        //                    //else if (policy.VarType == "T")
        //                    //{
        //                    //    returnValue = Convert.ToDateTime(rsSet.Set).ToString("hh:mm:ss");
        //                    //}
        //                    break;
        //                }
        //                //}
        //            }
        //        }
        //    }
        //    Performancelog.Debug($"End,PolicyManager,GetPol,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        //    return returnValue;

        //}

    }//end class
}//end namespace
