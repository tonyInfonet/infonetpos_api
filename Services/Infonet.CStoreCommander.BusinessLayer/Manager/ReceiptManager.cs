using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class ReceiptManager : ManagerBase, IReceiptManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly IUtilityService _utilityService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ITillService _tillService;
        private readonly IReportService _reportService;
        private readonly IStockService _stockService;
        private readonly ITenderService _tenderService;
        private readonly IShiftService _shiftService;
        private readonly IPromoManager _promoManager;
        private readonly ISaleManager _saleManager;
        private readonly ISaleService _saleService;
        private readonly ILoginManager _loginManager;
        private readonly ICreditCardManager _creditCardManager;
        private readonly ICardService _cardService;
        private readonly ITaxService _taxService;
        private readonly ICustomerManager _customerManager;
        private readonly IReasonService _reasonService;
        private readonly IPayAtPumpManager _payAtPumpManager;
        private readonly ISaleHeadManager _saleHeadManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ITreatyService _treatyService;
        private readonly ISaleVendorCouponManager _svcManager;
        private readonly ITaxExemptService _taxExemptService;
        private readonly ITeSystemManager _teSystemManager;
        private readonly IEncryptDecryptUtilityManager _encryptManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="utilityService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="tillService"></param>
        /// <param name="reportService"></param>
        /// <param name="stockService"></param>
        /// <param name="tenderService"></param>
        /// <param name="shiftService"></param>
        /// <param name="promoManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="saleService"></param>
        /// <param name="loginManager"></param>
        /// <param name="creditCardManager"></param>
        /// <param name="cardService"></param>
        /// <param name="taxService"></param>
        /// <param name="customerManager"></param>
        /// <param name="reasonService"></param>
        /// <param name="payAtpumpManager"></param>
        /// <param name="saleHeadManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="treatyService"></param>
        /// <param name="svcManager"></param>
        /// <param name="taxExemptService"></param>
        /// <param name="teSystemManager"></param>
        /// <param name="encryptManager"></param>
        public ReceiptManager(IPolicyManager policyManager,
            IUtilityService utilityService,
            IApiResourceManager resourceManager,
            ITillService tillService,
            IReportService reportService,
            IStockService stockService,
            ITenderService tenderService,
            IShiftService shiftService,
            IPromoManager promoManager,
            ISaleManager saleManager,
            ISaleService saleService,
            ILoginManager loginManager,
            ICreditCardManager creditCardManager,
            ICardService cardService,
            ITaxService taxService,
            ICustomerManager customerManager,
            IReasonService reasonService,
            IPayAtPumpManager payAtpumpManager,
            ISaleHeadManager saleHeadManager,
            ISaleLineManager saleLineManager,
            ITreatyService treatyService,
            ISaleVendorCouponManager svcManager,
            ITaxExemptService taxExemptService,
            ITeSystemManager teSystemManager,
            IEncryptDecryptUtilityManager encryptManager)
        {
            _policyManager = policyManager;
            _utilityService = utilityService;
            _resourceManager = resourceManager;
            _tillService = tillService;
            _reportService = reportService;
            _stockService = stockService;
            _tenderService = tenderService;
            _shiftService = shiftService;
            _promoManager = promoManager;
            _saleManager = saleManager;
            _saleService = saleService;
            _loginManager = loginManager;
            _creditCardManager = creditCardManager;
            _cardService = cardService;
            _taxService = taxService;
            _customerManager = customerManager;
            _reasonService = reasonService;
            _payAtPumpManager = payAtpumpManager;
            _saleHeadManager = saleHeadManager;
            _saleLineManager = saleLineManager;
            _treatyService = treatyService;
            _taxExemptService = taxExemptService;
            _svcManager = svcManager;
            _teSystemManager = teSystemManager;
            _encryptManager = encryptManager;
        }
        //Tony 03/19/2019
        public List<string> GetReceiptHeader()
        {
            List<string> olist = new List<string>();
            var just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper();
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short hWidth = 40;


            if (_policyManager.PRN_CO_NAME)
            {
                olist.Add(modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
            }
            if (_policyManager.PRN_CO_ADDR)
            {
                olist.Add(modStringPad.PadIt(just, Convert.ToString(store.Address.Street1), hWidth));
                if (store.Address.Street2 != "")
                {
                    olist.Add(modStringPad.PadIt(just, Convert.ToString(store.Address.Street2), hWidth));
                }
                olist.Add(modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, Convert.ToString(store.Address.PostalCode), hWidth));
            }
            if (_policyManager.PRN_CO_PHONE)
            {
                foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                {
                    var phoneRenamed = tempLoopVarPhoneRenamed;
                    if (phoneRenamed.Number.Trim() != "")
                    {
                        olist.Add(modStringPad.PadC(phoneRenamed.PhoneName + " " + phoneRenamed.Number, hWidth));
                    }
                }
            }

            olist.Add(modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth));

            if (store.SecRegName != "")
            {
                olist.Add(modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth) + "\r\n");
            }

            return olist;
        }
        //end
        /// <summary>
        /// Print Bottle Return
        /// </summary>
        /// <param name="brPay"></param>
        /// <param name="userName"></param>
        /// <param name="saleDate"></param>
        /// <param name="saleTime"></param>
        /// <param name="registerNum"></param>
        /// <param name="tillNum"></param>
        /// <param name="tillShift"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        public Report Print_BottleReturn(BR_Payment brPay, string userName, DateTime saleDate,
            DateTime saleTime, short registerNum, short tillNum, short tillShift, bool reprint = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Print_BottleReturn,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var report = new Report
            {
                ReportName = Utilities.Constants.BottleReturnFile,
                Copies = _policyManager.BottleReturnReceiptCopies
            };
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            short dWidth = 16;
            short aWidth = 10;
            short qWidth = 8;
            short pWidth = 6;
            var nH = (short)FileSystem.FreeFile();
            var fileName = Path.GetTempPath() + "\\BottleReturn" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";
            try
            {
                FileSystem.FileOpen(nH, fileName, OpenMode.Output);

                var just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper();
                short hWidth = 40;
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street1), hWidth));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, Convert.ToString(store.Address.PostalCode), hWidth));
                }

                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        var phoneRenamed = tempLoopVarPhoneRenamed;
                        if (phoneRenamed.Number.Trim() != "")
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phoneRenamed.PhoneName + " " + phoneRenamed.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth)); //& vbCrLf

                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth) + "\r\n");
                }
                else
                {
                    FileSystem.PrintLine(nH);
                }
                FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, 1248).ToUpper() + " #" + Convert.ToString(brPay.Sale_Num), hWidth)); //"Bottle Return # "

                if (reprint)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, 225) + ": " + userName + " (" + _resourceManager.GetResString(offSet, 132).Substring(0, 1) + Convert.ToString(registerNum) + "/" + _resourceManager.GetResString(offSet, 131).Substring(0, 1) + Convert.ToString(tillNum) + "/" + _resourceManager.GetResString(offSet, 346).Substring(0, 1) + Convert.ToString(tillShift) + ")", hWidth)); //Cashier
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, saleDate.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, 208) + saleTime.ToString(timeFormatHm), hWidth) + "\r\n"); //  
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, 234) + ": " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, 208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //Reprinted on  '  
                }
                else
                {
                    if (_policyManager.PRN_UName)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, 225) + ": " + userName + " (" + _resourceManager.GetResString(offSet, 132).Substring(0, 1) + Convert.ToString(registerNum) + "/" + _resourceManager.GetResString(offSet, 131).Substring(0, 1) + Convert.ToString(tillNum) + "/" + _resourceManager.GetResString(offSet, 346).Substring(0, 1) + Convert.ToString(tillShift) + ")", hWidth)); //Cashier
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, 225) + ": " + UserCode + " (" + _resourceManager.GetResString(offSet, 132).Substring(0, 1) + Convert.ToString(registerNum) + "/" + _resourceManager.GetResString(offSet, 131).Substring(0, 1) + Convert.ToString(tillNum) + "/" + _resourceManager.GetResString(offSet, 346).Substring(0, 1) + Convert.ToString(tillShift) + ")", hWidth)); //Cashier
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, 208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //" at "  '  
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, 119), dWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, 230), qWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, 122), pWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, 106), aWidth)); //"Description"'"Quantity","Price","Amount"
                FileSystem.PrintLine(nH, new string('-', hWidth));

                foreach (BottleReturn tempLoopVarBr in brPay.Br_Lines)
                {
                    var br = tempLoopVarBr;
                    if (br.Amount != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(br.Product, (short)(dWidth - 1)) + " " + modStringPad.PadL(Math.Abs((short)br.Quantity).ToString("#0"), qWidth) + modStringPad.PadL(Math.Abs((short)br.Price).ToString("#0.00"), pWidth) + modStringPad.PadL(Math.Abs(br.Amount).ToString("#0.00"), aWidth));
                    }
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, 210), 30) + modStringPad.PadL(brPay.Amount.ToString("0.00"), 10)); //"Total" '  PadL(Format(Abs(BrPay.Amount), "0.00"), 10)    '"Total"
                if (brPay.Penny_Adj != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, 486), (short)(hWidth - aWidth)) + modStringPad.PadL(brPay.Penny_Adj.ToString("###,##0.00"), aWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadL(new string('_', 10), hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, 210), (short)(hWidth - aWidth)) + modStringPad.PadL((brPay.Amount + brPay.Penny_Adj).ToString("###,##0.00"), aWidth));
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.FileClose(nH);

                Chaps_Main.Last_Printed = "BottleReturn.txt";
                var stream = File.OpenRead(fileName);
                Performancelog.Debug($"End,ReceiptManager,Print_BottleReturn,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,Print_BottleReturn,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(nH);
                FileSystem.Kill(fileName);
            }
        }

        /// <summary>
        /// Method to get all departments
        /// </summary>
        /// <returns>List of departments</returns>
        public Dictionary<string, string> GetAllDepartmets()
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,GetAllDepartmets,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var departments = new Dictionary<string, string>();
            string all = _resourceManager.GetResString(offSet, 407);
            departments.Add("0", all);
            var availableDepartments = _utilityService.GetAllDepartments();
            foreach (var department in availableDepartments)
            {
                departments.Add(department.Dept, $"{department.Dept} - {department.DeptName}");
            }
            Performancelog.Debug($"End,ReceiptManager,GetAllDepartmets,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return departments;
        }

        /// <summary>
        /// Method to print sales count summary report
        /// </summary>
        /// <param name="department">Department id</param>
        /// <param name="tillNumber">TIll number</param>
        /// <param name="shiftNumber">Shiftnumber</param>
        /// <param name="loggedTill">LoggedTill</param>
        public FileStream PrintSaleCountReport(string department, int tillNumber, int shiftNumber, Till loggedTill)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintSaleCountReport,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string whereClause = "";
            string dept;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            string filterRenamed;

            if (tillNumber > 0)
            {
                whereClause = " AND A.Till=" + tillNumber;
                filterRenamed = _resourceManager.GetResString(offSet, (short)8329) + " " + tillNumber;
            }
            else
            {
                filterRenamed = _resourceManager.GetResString(offSet, 408); // all tills
            }
            if (shiftNumber > 0)
            {
                if (whereClause.Length == 0)
                {
                    whereClause = "AND A.Shift=" + shiftNumber;
                }
                else
                {
                    whereClause = whereClause + " AND A.Shift=" + shiftNumber;
                }
                filterRenamed = filterRenamed + " " + _resourceManager.GetResString(offSet, (short)405) + " " + shiftNumber;
            }
            else
            {
                filterRenamed = filterRenamed + " " + _resourceManager.GetResString(offSet, (short)409);
            }

            if (!string.IsNullOrEmpty(department) && department != "0")
            {
                dept = department;
            }
            else
            {
                dept = "";
            }
            Performancelog.Debug($"End,ReceiptManager,PrintSaleCountReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Run_Report(dept, whereClause, filterRenamed, loggedTill);

        }

        /// <summary>
        /// Method to get totals for flash report
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Byte</returns>
        public FlashReportTotals GetTotalsForFlashReport(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,GetTotalsForFlashReport,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var flashDetails = _reportService.GetFlashReportDetails(tillNumber);

            flashDetails.NetTotal = flashDetails.Totals - flashDetails.Tax - flashDetails.Charge +
                flashDetails.LineDiscount + flashDetails.InvoiceDiscount - flashDetails.Refund;
            flashDetails.ProductSales = flashDetails.Totals - flashDetails.Tax - flashDetails.Charge +
                 flashDetails.LineDiscount + flashDetails.InvoiceDiscount;
            flashDetails.SalesAfterDiscount = flashDetails.NetTotal - flashDetails.LineDiscount - flashDetails.InvoiceDiscount + flashDetails.Refund;
            flashDetails.TotalReceipt = flashDetails.Totals + flashDetails.Refund;
            Performancelog.Debug($"End,ReceiptManager,GetTotalsForFlashReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return flashDetails;

        }

        /// <summary>
        /// Get list of departments and the net sales
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Bytes</returns>
        public List<Department> GetDepartmentDetailsForFlashReport(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,GetDepartmentDetailsForFlashReport,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            //var lineTotal = _reportService.GetLineTotal(tillNumber);
            Performancelog.Debug($"End,ReceiptManager,GetDepartmentDetailsForFlashReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return _reportService.GetDepartmentsForFlashReport(tillNumber);
        }

        /// <summary>
        /// Method to print flash report
        /// </summary>
        /// <param name="till"></param>
        /// <param name="totals"></param>
        /// <param name="departments"></param>
        /// <returns></returns>
        public FileStream PrintFlashReport(Till till, FlashReportTotals totals, List<Department> departments)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintFlashReport,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var store = _policyManager.LoadStoreInfo();
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            short nH = 0;
            string strFileName = "";
            var hWidth = 40;
            string fuelDept = "";
            string dName = "";
            decimal sSales = new decimal();
            decimal fSales = new decimal();
            decimal fVol = new decimal();
            decimal avgPrice = new decimal();
            string dept = "";
            fuelDept = _utilityService.GetFuelDepartmentId();
            strFileName = Path.GetTempPath() + "\\FlashRep" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";
            nH = (short)(FileSystem.FreeFile());
            var offSet = store.OffSet;
            try
            {
                FileSystem.FileOpen(nH, strFileName, OpenMode.Output);

                FileSystem.PrintLine(nH, modStringPad.PadC(store.Code + "  " + store.Name, (short)40)); //  
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8134) + till.Number, (short)40)); //"TILL FLASH REPORT FOR TILL #"                                                                                                                            //    Print #nH, PadC(GetResString(199) & Format(Now, "d-mmm-yyyy") & " at " & Format(Now, "h:nn AMPM"), 40)     '"Printed on "
                                                                                                                                          ///    Print #nH, PadC(GetResString(199) & Format(Now, "d-mmm-yyyy") & " " & GetResString(208) & " " & Format(Now, "h:nn AMPM"), 40)     '"Printed on "
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)199) + DateTime.Now.ToString("d-MMM-yyyy") + " " + _resourceManager.GetResString(offSet, (short)208) + " " + DateTime.Now.ToString(timeFormatHm), (short)40)); //"Printed on "   '  
                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)149), (short)30) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8380), (short)10)); //"Department""Sales"

                FileSystem.PrintLine(nH, modStringPad.PadL("-", (short)40, "-"));
                foreach (var department in departments)
                {
                    dept = department.Dept;
                    if (dept != fuelDept)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(dept, (short)5) + modStringPad.PadR(Convert.ToString(string.IsNullOrEmpty(department.Dept) ? (_resourceManager.GetResString(offSet, (short)147) + " " + _resourceManager.GetResString(offSet, (short)149)) : department.DeptName), (short)20) + modStringPad.PadL(department.Sales.ToString("0.00"), (short)15));
                    }

                }

                var fuelSales = _reportService.GetFuelSalesReport(fuelDept, till.Number);
                if (fuelSales.Count > 0)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, _resourceManager.CreateCaption(offSet, (short)62, 29, null, (short)0));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)332), (short)10) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)63, 29, null, (short)0), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)334), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8380), (short)10));
                    FileSystem.PrintLine(nH, new string('_', hWidth));
                    foreach (var fuelSale in fuelSales)
                    {

                        dName = modStringPad.PadR(fuelSale.Description, (short)10);
                        sSales = Convert.ToDecimal(sSales + fuelSale.Sales);
                        fSales = Convert.ToDecimal(fSales + fuelSale.Sales);
                        fVol = Convert.ToDecimal(fVol + fuelSale.Volume);
                        avgPrice = Convert.ToDecimal(Convert.ToDouble(fuelSale.Sales) / Convert.ToDouble(fuelSale.Volume));
                        FileSystem.PrintLine(nH, dName + modStringPad.PadL(avgPrice.ToString("#,##0.000"), (short)10) + modStringPad.PadL(fuelSale.Volume.ToString("#,##0.000"), (short)10) + modStringPad.PadL(fuelSale.Sales.ToString("#,##0.00"), (short)10));

                    }
                    FileSystem.PrintLine(nH, new string('_', hWidth));
                    avgPrice = fSales / fVol;
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.CreateCaption(offSet, (short)64, 29, null, (short)0), (short)10) + modStringPad.PadL(avgPrice.ToString("#,##0.000"), (short)10) + modStringPad.PadL((fVol.ToString("#,##0.000")), (short)10) + modStringPad.PadL(fSales.ToString("#,##0.00"), (short)10));

                    FileSystem.PrintLine(nH);
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadL("-", (short)40, "-"));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8380), (short)30) + modStringPad.PadL((totals.NetTotal + totals.Refund).ToString("0.00"), (short)10)); //"Sales"

                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)202), (short)30) + modStringPad.PadL(totals.LineDiscount.ToString("0.00"), (short)10)); //"Line Discounts"
                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)203), (short)30) + modStringPad.PadL(totals.InvoiceDiscount.ToString("0.00"), (short)10)); //"Invoice Discounts"
                FileSystem.PrintLine(nH, Strings.Space(30) + modStringPad.PadL("_", (short)10, "_"));
                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)204), (short)30) + modStringPad.PadL(totals.SalesAfterDiscount.ToString("0.00"), (short)10)); //binal Remove 'val' from the expression   '"Sales after Discounts"
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)137), (short)30) + modStringPad.PadL((totals.Tax.ToString("0.00")), (short)10)); //"Taxes"
                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)138), (short)30) + modStringPad.PadL((totals.Charge.ToString("0.00")), (short)10)); //"Charges"

                FileSystem.PrintLine(nH, Strings.Space(30) + modStringPad.PadL("_", (short)10, "_"));
                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)243), (short)30) + modStringPad.PadL(totals.Refund.ToString("0.00"), (short)10)); // Refund
                                                                                                                                                                                          //    Print #nH, PadL(GetResString(205), 30) & PadL(Format(Val(lblTotal.Caption), "#,##0.00"), 10)     '"Total Receipts"
                                                                                                                                                                                          // binal commented to remove the format from caption property
                FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)205), (short)30) + modStringPad.PadL(totals.TotalReceipt.ToString("#,##0.00"), (short)10)); //binal Remove 'val' from the expression'"Total Receipts"
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.FileClose();
                var stream = File.OpenRead(strFileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,PrintFlashReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return stream;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,PrintFlashReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(nH);
            }
        }

        /// <summary>
        /// Method to print till audit report
        /// </summary>
        /// <param name="till">Till</param>
        /// <returns>File stream</returns>
        public FileStream PrintTillAuditReport(Till till)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintTillAuditReport,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var auditTill = new Till();
            auditTill.Number = Convert.ToInt16(till.Number);
            auditTill.Date_Open = Convert.ToDateTime(till.Date_Open);
            auditTill.Time_Open = Convert.ToDateTime(till.Time_Open);
            auditTill.Float = till.Float;
            auditTill.BonusFloat = till.BonusFloat; //  Cash Bonus implementation
            var cbt = _tenderService.GetTenderName(Convert.ToString(_policyManager.CBonusTend)); //  - Cash bonus
            if (string.IsNullOrEmpty(cbt))
            {
                cbt = Convert.ToString(_policyManager.CBonusTend);
            }
            Collect_Data(ref auditTill, cbt);

            var strFileName = Path.GetTempPath() + "\\TillAudit" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            Performancelog.Debug($"End,ReceiptManager,PrintTillAuditReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Make_Report(strFileName, auditTill, cbt);
        }

        /// <summary>
        /// Method to validate critera to get a report
        /// </summary>
        /// <param name="department"></param>
        /// <param name="tillNumber"></param>
        /// <param name="shiftNumber"></param>
        /// <returns></returns>
        public MessageStyle IsValidateReportCriteria(string department, int tillNumber, int shiftNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,IsValidateReportCriteria,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var errorMessage = new MessageStyle();
            var flag = true;
            if (_utilityService.GetTotalDepartments() == 0)
            {
                MessageType tempVbStyle14 = (int)MessageType.Exclamation + MessageType.OkOnly;
                errorMessage = _resourceManager.CreateMessage(offSet, 0, (short)5689, null, tempVbStyle14);
            }
            if (!string.IsNullOrEmpty(department) && !GetAllDepartmets().ContainsKey(department))
            {
                flag = false;
            }
            if (tillNumber != 0 && !_tillService.GetAllTills().Contains(tillNumber))
            {
                flag = false;
            }
            if (shiftNumber != 0 && _shiftService.GetShifts(null).FirstOrDefault(s => s.ShiftNumber == shiftNumber) == null)
            {
                flag = false;
            }
            if (!flag)
            {
                errorMessage.Message = "Request is invalid";
            }
            Performancelog.Debug($"End,ReceiptManager,IsValidateReportCriteria,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return errorMessage;
        }


        /// <summary>
        /// Method to print receipt
        /// </summary>
        /// <param name="loggedTill"></param>
        /// <param name="storeCredit"></param>
        /// <param name="sale"></param>
        /// <param name="tenders"></param>
        /// <param name="printIt"></param>
        /// <param name="fileName"></param>
        /// <param name="reprint"></param>
        /// <param name="signature"></param>
        /// <param name="userName"></param>
        /// <param name="completePrepay</">param>
        /// <param name="reprintCards"></param>
        /// <returns></returns>
        public Report Print_Receipt(int loggedTill, Store_Credit storeCredit, ref Sale sale, ref Tenders tenders, bool printIt, ref string
            fileName, ref bool reprint, out Stream signature, string userName, bool completePrepay = false, Reprint_Cards reprintCards = null)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Print_Receipt,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var till = _tillService.GetTill(loggedTill);
            var security = _policyManager.LoadSecurityInfo();
            var promos = _promoManager.Load_Promos("");
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            signature = new MemoryStream();
            int nH = 0;
            Sale_Line sl;
            Line_Kit kit;
            Charge charge;
            Charge_Tax ctx;
            K_Charge kitChg;
            Sale_Tax stx;
            Line_Tax ltx;
            Tender tender;
            double factor = 0;
            decimal sumPrice;
            decimal sumTender;
            decimal sumQa;
            decimal subTotal;
            decimal pointsUsed = new decimal();
            string taxSymbol = string.Empty;
            short n = 0;
            short dWidth = 0;
            short aWidth = 0;
            short qWidth = 0;
            short pWidth = 0;
            short rWidth = 0;
            short hWidth = 0;
            short ldWidth = 0;
            bool showCode = false;
            bool showPrice = false;
            string just = "";
            string qi = "";
            string pi = "";
            string ri = "";
            string pf = "";
            bool sigLine = false;
            bool printSignForRefund;
            bool markDown;
            bool Void;
            bool runAway;
            bool pumpTest;
            string fs;
            short qd;
            short pd;
            string totalLineCaption;
            string promoName = "";
            bool fuelLine;
            bool hasFuelSale;
            bool hasPrepay;
            string longDescription;
            string description;
            string pumpNo;
            short copies;
            byte advLines;
            string strArPo;
            bool boolExistingPromo;
            Promo pr;
            bool carwashProduct = false;

            bool emvVersion = _policyManager.EMVVersion;
            bool taxExempt = _policyManager.TAX_EXEMPT;
            string teType = _policyManager.TE_Type;

            double dPointEarned = 0; //Info.
            double dBalance = 0; //Info.
            string sCardNumber = ""; //Info.

            var user = _loginManager.GetExistingUser(userName);
            TaxExemptSaleLine tesl = default(TaxExemptSaleLine);
            TaxCreditLine tcl = default(TaxCreditLine);
            Line_Tax lt = default(Line_Tax);
            Sale_Tax tc = default(Sale_Tax);

            bool printCardHolderAgree = false;
            string fleetLanguage = "";
            bool maskCard = false;

            bool affdaCustomerUsed = false;
            bool electronicSignature = false;

            string discountName = "";
            decimal fuelLoyaltyDiscount = new decimal();
            bool boolPrintSubTotal = false;
            bool breakSwitchPrepay = false;
            short noLines = 0;
            short k = 0;
            short pos1 = 0;
            short pos2 = 0;
            double totalExemptedTax = 0; //  

            bool validGrCouSa;
            double fuelTotal = 0;
            double creTotal = 0;
            double othTotal;
            decimal totalTaxExemptGa = new decimal(); //  
            decimal totalTaxExemptGaAdd = new decimal(); //  
            bool boolArTenderUsed = false; //  
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            validGrCouSa = false;

            strArPo = "";
            affdaCustomerUsed = false;
            printCardHolderAgree = false;
            markDown = sale.Sale_Type == "MARKDOWN";
            runAway = sale.Sale_Type == "RUNAWAY";
            pumpTest = sale.Sale_Type == "PUMPTEST";
            @Void = sale.Sale_Type == "VOID";
            printSignForRefund = false;
            advLines = Convert.ToByte(_policyManager.ADV_LINES);
            boolArTenderUsed = false;

            totalLineCaption = _resourceManager.GetResString(offSet, (short)210);
            if (tenders != null)
            {
                foreach (Tender tempLoopVarTenderRenamed in tenders)
                {
                    tender = tempLoopVarTenderRenamed;
                    var creditCard = tender.Credit_Card;
                    if (!string.IsNullOrEmpty(tender.Credit_Card.Cardnumber))
                    {
                        totalLineCaption = _creditCardManager.ReceiptTotalText(ref creditCard);
                        break;
                    }
                }
            }
            string reportName = "";
            if (string.IsNullOrEmpty(fileName))
            {

                fileName = Path.GetTempPath() + "\\" + "Receipt" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" +
                            GenerateRandomNo() + ".txt";
                reportName = Utilities.Constants.ReceiptFile;
            }
            else
            {
                reportName = fileName + ".txt";
                fileName = Path.GetTempPath() + "\\" + fileName + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" +
                         GenerateRandomNo() + ".txt";
            }
            var report = new Report
            {
                ReportName = reportName,
                Copies = 1
            };

            dWidth = (short)1; // Description Width
            aWidth = (short)10; // Amount Width
            qWidth = (short)7; // Quantity Width
            pWidth = (short)6; // Price Width
            hWidth = (short)40; // Total Receipt Width

            showCode = Convert.ToBoolean(_policyManager.SHOW_CODE); // Show the Product Code
            showPrice = Convert.ToBoolean(_policyManager.SHOW_PRICE); // Show the Item Price
            just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification
            var loyalName = _policyManager.LOYAL_NAME;
            // Force the Description width to fit the total receipt width.
            dWidth = Convert.ToInt16(hWidth - Convert.ToInt32(Convert.ToInt32(qWidth + Convert.ToInt32(showPrice ? pWidth : 0)) + aWidth));

            sumPrice = 0;
            subTotal = 0;
            sumTender = 0;
            if (sale.Sale_Time == DateTime.MinValue)
            {
                sale.Sale_Time = DateAndTime.TimeOfDay;
            }
            try
            {
                nH = FileSystem.FreeFile();
                FileSystem.FileOpen(nH, fileName, OpenMode.Output, OpenAccess.Write);

                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }

                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street1), hWidth));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, Convert.ToString(store.Address.PostalCode), hWidth));
                }

                Phone phone = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhone in store.Address.Phones)
                    {
                        phone = tempLoopVarPhone;

                        if (!string.IsNullOrEmpty(phone.Number))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phone.PhoneName + " " + phone.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth)); //& vbCrLf

                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth));
                }
                FileSystem.PrintLine(nH);

                if (@Void)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)8381).ToUpper() + " # " + Convert.ToString(sale.Sale_Num), hWidth)); //"VOID RECEIPT # "
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, sale.Return_Reason.Description, hWidth));
                }
                else if (markDown)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)226).ToUpper() + Convert.ToString(sale.Sale_Num), hWidth)); //"STOCK WRITE OFF # "
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, sale.Return_Reason.Description, hWidth));
                }
                else if (runAway)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)282).ToUpper() + " # " + Convert.ToString(sale.Sale_Num), hWidth));
                }
                else if (pumpTest)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)454).ToUpper() + " # " + Convert.ToString(sale.Sale_Num), hWidth));
                }
                else
                {
                    if (sale.Sale_Totals.Gross >= 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)229).ToUpper() + Convert.ToString(sale.Sale_Num), hWidth)); //"SALE RECEIPT # "
                        validGrCouSa = true;
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)227).ToUpper() + Convert.ToString(sale.Sale_Num), hWidth)); //"REFUND RECEIPT # "

                        if (_policyManager.PRN_SgnRtn)
                        {
                            printSignForRefund = true;
                        }
                    }

                    if (_policyManager.USE_CUST)
                    {
                        if (!string.IsNullOrEmpty(sale.Customer.Name) && sale.Customer.Name != _resourceManager.GetResString(offSet, (short)400))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)110) + ": " + sale.Customer.Code + " - " + sale.Customer.Name, hWidth));
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)110) + ": " + _resourceManager.GetResString(offSet, (short)400), hWidth)); //Cash Sale Customer
                        }
                    }
                    if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + _resourceManager.GetResString(offSet, (short)231), hWidth)); //" Member(Active)"
                    }
                    else if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "F")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + _resourceManager.GetResString(offSet, (short)233), hWidth)); //" Member(Frozen)"
                    }
                    else if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "I")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + _resourceManager.GetResString(offSet, (short)232), hWidth)); //" Member(Inactive)"
                    }
                }
                if (sale.EligibleTaxEx && !string.IsNullOrEmpty(sale.ReferenceNumber))
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)1714) + " " + sale.ReferenceNumber, hWidth));
                }

                if (reprint)
                {
                    validGrCouSa = false;
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + userName + " (" + _resourceManager.GetResString(offSet, (short)132).Substring(0, 1) + Convert.ToString(sale.Register) + "/" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1) + Convert.ToString(sale.TillNumber) + "/" + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1) + Convert.ToString(sale.Shift) + ")", hWidth)); //Cashier '  reprint should show original shift - not current shiftTill.Shift & ")", H_Width) 'Cashier
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               ///        Print #nH, PadIt(Just, Format(Sale.Sale_Date, "dd-MMM-yyyy") & GetResString(208) & Format(Sale.Sale_Time, "h:nn AMPM"), H_Width) & vbCrLf
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, sale.Sale_Date.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) +
                        sale.Sale_Time.ToString(timeFormatHm), hWidth) + "\r\n");
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)234) + ": " +
                        DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) +
                        DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //Reprinted on '  
                }
                else
                {
                    if (_policyManager.PRN_UName)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Name + " (" + _resourceManager.GetResString(offSet, (short)132).Substring(0, 1) + Convert.ToString(sale.Register) + "/" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1) + till.Number + "/" + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1) + till.Shift + ")", hWidth)); //Cashier
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Code + " (" + _resourceManager.GetResString(offSet, (short)132).Substring(0, 1) + Convert.ToString(sale.Register) + "/" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1) + till.Number + "/" + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1) + till.Shift + ")", hWidth)); //Cashier
                    }

                    FileSystem.PrintLine(nH, modStringPad.PadIt(just,
                        DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) +
                        DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //" at "  '  
                }

                TaxExemptSale oTeSale = null;
                if (!reprint)
                    oTeSale = CacheManager.GetTaxExemptSaleForTill(till.Number, sale.Sale_Num);
                else
                {
                    oTeSale = _taxExemptService.LoadTaxExempt(teType, sale.Sale_Num, sale.TillNumber,
                                  DataSource.CSCTills) ??
                              _taxExemptService.LoadTaxExempt(teType, sale.Sale_Num, sale.TillNumber,
                                  DataSource.CSCTrans);
                }
                if (taxExempt && (teType == "AITE"))
                {
                    if (oTeSale?.teCardholder != null)
                    {
                        if (!string.IsNullOrEmpty(oTeSale.teCardholder.CardNumber))
                        {
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5466) + ": " + MaskedAiteNumber(oTeSale.teCardholder.CardNumber)); //& vbCrLf    'AITE Card
                        }
                    }
                }

                if (taxExempt && !string.IsNullOrEmpty(sale.TreatyNumber))
                {
                    if (teType == "QITE")
                    {
                        if (oTeSale?.teCardholder != null)
                        {
                            if (!string.IsNullOrEmpty(oTeSale.teCardholder.CardNumber))
                            {
                                FileSystem.PrintLine(nH,
                                    _resourceManager.GetResString(offSet, (short)422) + " " + oTeSale.teCardholder.CardNumber);
                                FileSystem.PrintLine(nH,
                                    _resourceManager.GetResString(offSet, (short)448) + " " + oTeSale.teCardholder.Name);
                                FileSystem.PrintLine(nH, oTeSale.teCardholder.Address);
                                FileSystem.PrintLine(nH,
                                    oTeSale.teCardholder.City + " " + oTeSale.teCardholder.PostalCode);
                                FileSystem.PrintLine(nH,
                                    _resourceManager.GetResString(offSet, (short)421) + " " + oTeSale.teCardholder.PlateNumber);
                                if (!string.IsNullOrEmpty(oTeSale.teCardholder.Note))
                                {
                                    noLines =
                                        (short)
                                        (Math.Round((double)oTeSale.teCardholder.Note.Length / hWidth, 0));
                                    if (oTeSale.teCardholder.Note.Length < 20)
                                    {
                                        noLines = (short)1;
                                    }
                                    pos1 = (short)1;
                                    for (k = 1; k <= noLines; k++)
                                    {
                                        pos2 =
                                            (short)
                                            (oTeSale.teCardholder.Note.Substring(pos1 - 1, hWidth).Length + pos1 - 1);
                                        FileSystem.PrintLine(nH, oTeSale.teCardholder.Note.Substring(pos1 - 1, hWidth));
                                        pos1 = (short)(pos2 + 1);
                                    }
                                }
                            }
                        }
                    }
                    else //For AITE and SITE
                    {
                        FileSystem.PrintLine(nH, _resourceManager.CreateCaption(offSet, (short)5, (short)51, null, (short)1) + ": " + sale.TreatyNumber); //Treaty Number
                                                                                                                                                          // 
                        if (_policyManager.TE_GETNAME)
                        {
                            if (!string.IsNullOrEmpty(sale.TreatyName))
                            {
                                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5119) + ": " + sale.TreatyName);
                            }
                        }
                        FileSystem.PrintLine(nH);
                    }
                }

                FileSystem.PrintLine(nH);
                if (showCode)
                {

                    qWidth = (short)10; // Quantity Width
                    rWidth = (short)10; // Regular Price Width
                    pWidth = (short)10; // Price Width
                    aWidth = (short)10; // Amount Width

                    if (showPrice)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)230), qWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)235), rWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)122), pWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)106), aWidth)); // "Quantity", "Reg Price", "Price", "Amount"
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)230), qWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)106), (short)(pWidth + aWidth))); //"Quantity","Amount"
                    }
                }
                else
                {
                    if (showPrice)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)119), (short)(dWidth - 6)) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)230), (short)(qWidth + 6)) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)122), pWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)106), aWidth)); //"Description"'"Quantity","Price","Amount"
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)119), (short)(dWidth - 7)) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)230), (short)(qWidth + 7)) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)106), aWidth)); //"Description","Quantity","Amount"
                    }
                }
                FileSystem.PrintLine(nH, modStringPad.PadL("=", hWidth, "=") + "\r\n");

                hasFuelSale = false;
                hasPrepay = false;
                bool breakAiteExempt = false;
                double exemptedTax = 0;
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {

                    sl = tempLoopVarSl;

                    if (sl.IsCarwashProduct)
                    {
                        carwashProduct = true;
                    }

                    if (sl.FuelRebateEligible)
                    {
                        affdaCustomerUsed = true;
                    }

                    fuelLine = sl.ProductIsFuel;

                    if ((sl.Prepay == false || completePrepay == true) && validGrCouSa == true && _policyManager.FSGC_ENABLE == true)
                    {
                        validGrCouSa = true;
                    }
                    else
                    {
                        validGrCouSa = false;
                    }
                    var fuelDept = _utilityService.GetFuelDepartmentId();
                    if (sl.Dept == fuelDept && validGrCouSa == true)
                    {
                        if (_policyManager.FSGC_CalcType == "AMOUNT")
                        {
                            fuelTotal = fuelTotal + (double)sl.Amount;
                        }
                        else
                        {
                            fuelTotal = fuelTotal + sl.Quantity;
                        }
                    }

                    if (fuelLine)
                    {
                        hasFuelSale = true;
                    }
                    if (sl.Prepay)
                    {
                        hasPrepay = true;
                    }
                    if (hasPrepay)
                    {
                        tenders.Tend_Totals.ChangePaid = sale.Sale_Change;
                    }
                    string tempPolicyName = "FUEL_UM";
                    Variables.UnitMeasurement = Convert.ToString(_policyManager.GetPol(tempPolicyName, sl));
                    taxSymbol = "E";
                    foreach (Line_Tax tempLoopVarLtx in sl.Line_Taxes)
                    {
                        ltx = tempLoopVarLtx;
                        if (ltx.Tax_Rate != 0 && !(sale.EligibleTaxEx && sl.EligibleTaxEx))
                        {
                            taxSymbol = Get_Tax_Symbol(taxSymbol, ltx.Tax_Included);
                        }
                    }

                    qd = sl.QUANT_DEC; // Quantity Decimals
                    pd = sl.PRICE_DEC; // Price Decimals
                    if (Strings.UCase(Convert.ToString(security.BackOfficeVersion)) == "FULL" || _policyManager.PROMO_SALE)
                    {
                        if (sl.PromoID != "" && sl.price != sl.Regular_Price)
                        {
                            boolExistingPromo = false;
                            foreach (Promo tempLoopVarPr in promos)
                            {
                                pr = tempLoopVarPr;
                                if (pr.PromoID == sl.PromoID)
                                {
                                    boolExistingPromo = true;
                                    break;
                                }
                            }
                            if (!boolExistingPromo)
                            {
                                promoName = " " + _resourceManager.GetResString(offSet, (short)413) + " " + sl.PromoID;
                            }
                            else
                            {
                                promoName = " " + promos.get_Item(sl.PromoID).Description;
                            }
                        }
                        else
                        {
                            promoName = "";
                        }
                    }
                    if (fuelLine)
                    {
                        if (!_policyManager.USE_FUEL)
                        {
                            pumpNo = "";
                        }
                        else
                        {
                            if (sl.IsPropane)
                            {
                                pumpNo = " " + _resourceManager.GetResString(offSet, (short)371);
                            }
                            else
                            {
                                pumpNo = " " + _resourceManager.GetResString(offSet, (short)333) + "-" + (sl.pumpID).ToString().Trim();
                            }
                        }
                    }
                    else
                    {
                        pumpNo = "";
                    }

                    description = sl.Description;
                    if (showCode)
                    {
                        if (sl.Stock_Code == sl.PLU_Code)
                        {
                            ldWidth = (short)(hWidth - sl.Stock_Code.Length - " - ".Length);
                        }
                        else
                        {
                            ldWidth = hWidth; // Print description on a new line
                        }
                    }
                    else
                    {
                        ldWidth = dWidth;
                    }

                    if (Strings.Len(description + promoName + pumpNo) > ldWidth)
                    {
                        if (sl.IsPropane)
                        {
                            longDescription = (description + promoName + pumpNo).Substring(0, ldWidth);
                        }
                        else
                        {
                            if (Strings.Len(promoName + pumpNo) > ldWidth)
                            {
                                if (pumpNo.Length > ldWidth)
                                {
                                    longDescription = pumpNo.Substring(0, ldWidth);
                                }
                                else
                                {
                                    longDescription = promoName.Substring(0, ldWidth - pumpNo.Length) + pumpNo;
                                }
                            }
                            else
                            {
                                if (!_policyManager.USE_FUEL && pumpNo.Trim().ToUpper() == _resourceManager.GetResString(offSet, (short)371).ToUpper())
                                {
                                    longDescription = description + (promoName + pumpNo).Substring(0, ldWidth - description.Length - 2);
                                }
                                else
                                {
                                    longDescription = description.Substring(0, ldWidth - Strings.Len(promoName + pumpNo)) + promoName + pumpNo;
                                }
                            }
                        }
                    }
                    else
                    {
                        longDescription = description + promoName + pumpNo;
                    }

                    if (showCode)
                    {
                        if (sl.Stock_Code.Trim() == sl.PLU_Code.Trim())
                        {
                            FileSystem.PrintLine(nH, sl.Stock_Code + " - " + longDescription);
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, sl.Stock_Code + " (" + sl.PLU_Code + ") ");
                            FileSystem.PrintLine(nH, longDescription);
                        }
                    }

                    fs = Convert.ToString(qd == 0 ? "#,##0" : ("#,##0." + new string('0', qd)));
                    if (!fuelLine)
                    {
                        if (sl.Stock_BY_Weight && showCode)
                        {
                            qi = sl.Quantity.ToString(fs) + sl.UM;
                        }
                        else
                        {
                            qi = sl.Quantity.ToString(fs);
                        }
                    }
                    else
                    {
                        qi = sl.Quantity.ToString(fs) + Variables.UnitMeasurement;
                    }

                    breakAiteExempt = false;
                    if (taxExempt && (teType == "AITE" || teType == "QITE")) // 
                    {
                        if (sl.IsTaxExemptItem)
                        {
                            breakAiteExempt = true;
                        }
                        else if (!(oTeSale == null))
                        {
                            if (oTeSale.teCardholder.GstExempt || sale.TreatyNumber != "")
                            {
                                foreach (TaxCreditLine tempLoopVarTcl in oTeSale.TaxCreditLines)
                                {
                                    tcl = tempLoopVarTcl;
                                    if (tcl.Line_Num == sl.Line_Num)
                                    {
                                        foreach (Line_Tax tempLoopVarLt in tcl.Line_Taxes)
                                        {
                                            lt = tempLoopVarLt;
                                            if (lt.Tax_Included)
                                            {
                                                breakAiteExempt = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    breakSwitchPrepay = false;
                    if (taxExempt && (teType == "SITE") && sl.IsTaxExemptItem && sl.ProductIsFuel) //  not only prepay but for all fuel lines'and SL.Prepay = True
                    {
                        breakSwitchPrepay = true;
                    }
                    if (showCode)
                    {
                        if (showPrice)
                        {
                            if (breakAiteExempt || breakSwitchPrepay)
                            {
                                pi = "$" + Conversion.Str(sl.Regular_Price).Trim();
                                fs = Convert.ToString(pd == 0 ? "#,##0" : ("#,##0." + new string('0', pd)));
                                if (!fuelLine)
                                {
                                    if (sl.Stock_BY_Weight && showCode) //  added the if part to show unit of measure for Scalable product
                                    {
                                        pf = Convert.ToString("$" +
                                            sl.Regular_Price.ToString(fs) + Convert.ToString(!string.IsNullOrEmpty(sl.UM) ? ("/" + sl.UM.Trim()) : ""));
                                    }
                                    else
                                    {
                                        pf = "$" + sl.Regular_Price.ToString(fs);
                                    }
                                }
                                else
                                {
                                    pf = "$" + sl.Regular_Price.ToString(fs) + "/" + Variables.UnitMeasurement.Trim();
                                }
                                pi = Convert.ToString((pf.Length > pi.Length) ? pf : pi);
                                ri = " ";
                                FileSystem.PrintLine(nH, modStringPad.PadC(qi, qWidth) + modStringPad.PadL(ri, rWidth) + modStringPad.PadL(pi, pWidth) +
                                    modStringPad.PadL(sl.Total_Amount.ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));

                            }
                            else
                            {
                                pi = "$" + Conversion.Str(sl.price).Trim();
                                fs = Convert.ToString(pd == 0 ? "#,##0" : ("#,##0." + new string('0', pd)));
                                if (!fuelLine)
                                {
                                    if (sl.Stock_BY_Weight && showCode) //  added the if part to show unit of measure for Scalable product
                                    {
                                        pf = Convert.ToString("$" + sl.price.ToString(fs) + Convert.ToString(!string.IsNullOrEmpty(sl.UM) ? ("/" + sl.UM.Trim()) : ""));
                                    }
                                    else
                                    {
                                        pf = "$" + sl.price.ToString(fs);
                                    }
                                }
                                else
                                {
                                    pf = "$" + sl.price.ToString(fs) + "/" + Variables.UnitMeasurement.Trim();
                                }
                                pi = Convert.ToString((pf.Length > pi.Length) ? pf : pi);
                                if (!fuelLine)
                                {
                                    if (sl.Stock_BY_Weight && showCode) //  added the if part to show unit of measure for Scalable product
                                    {
                                        ri = Convert.ToString("$" + sl.Regular_Price.ToString(fs) + Convert.ToString(!string.IsNullOrEmpty(sl.UM) ? ("/" + sl.UM.Trim()) : ""));
                                    }
                                    else
                                    {
                                        ri = "$" + sl.Regular_Price.ToString(fs);
                                    }
                                }
                                else
                                {
                                    ri = "$" + sl.Regular_Price.ToString(fs) + "/" + Variables.UnitMeasurement.Trim();
                                }

                                if (sl.price == sl.Regular_Price)
                                {
                                    ri = " ";
                                }
                                if (sl.NoPriceFormat)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadC(qi, qWidth) + modStringPad.PadL(ri, rWidth) + modStringPad.PadL(pi, pWidth) + modStringPad.PadL("$" + Convert.ToString(sl.Amount), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));

                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadC(qi, qWidth) + modStringPad.PadL(ri, rWidth) + modStringPad.PadL(pi, pWidth) + modStringPad.PadL(sl.Amount.ToString("$###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                }
                            }

                        }
                        else
                        {
                            if (breakAiteExempt || breakSwitchPrepay)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadC(qi, qWidth) + modStringPad.PadL(sl.Total_Amount.ToString("###,##0.00"), (short)(pWidth + aWidth)) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                            }
                            else
                            {
                                if (sl.NoPriceFormat)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadC(qi, qWidth) + modStringPad.PadL("$" + Convert.ToString(sl.Amount), (short)(pWidth + aWidth)) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));

                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadC(qi, qWidth) + modStringPad.PadL(sl.Amount.ToString("$###,##0.00"), (short)(pWidth + aWidth)) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (showPrice)
                        {
                            if (breakAiteExempt || breakSwitchPrepay)
                            {
                                pi = "$" + Conversion.Str(sl.Regular_Price).Trim();
                                fs = Convert.ToString(pd == 0 ? "#,##0" : ("#,##0." + new string('0', pd)));
                                if (sl.Stock_BY_Weight && showCode) //  added the if part to show unit of measure for Scalable product
                                {
                                    pf = Convert.ToString("$" + sl.Regular_Price.ToString(fs) + Convert.ToString((!string.IsNullOrEmpty(sl.UM)) ? ("/" + sl.UM.Trim()) : ""));
                                }
                                else
                                {
                                    pf = Convert.ToString(fuelLine ? ("$" + sl.Regular_Price.ToString(fs) + "/" + Variables.UnitMeasurement.Trim()) : ("$" + sl.Regular_Price.ToString(fs)));
                                }
                                pi = Convert.ToString((pf.Length > pi.Length) ? pf : pi);

                                if (fuelLine) //for fuel we need more then 6 charactres for price eg.: 0.902/L
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, (short)(dWidth - 1)) + modStringPad.PadR(qi, (short)(qWidth + 1)) + modStringPad.PadL(pi, (short)(pWidth + 2)) + modStringPad.PadL(sl.Total_Amount.ToString("###,##0.00"), (short)(aWidth - 2)) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, dWidth) + modStringPad.PadC(qi, qWidth) + modStringPad.PadL(pi, pWidth) + modStringPad.PadL(sl.Total_Amount.ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                }
                            }
                            else
                            {
                                pi = "$" + Conversion.Str(sl.price).Trim();
                                fs = Convert.ToString(pd == 0 ? "#,##0" : ("#,##0." + new string('0', pd)));
                                if (sl.Stock_BY_Weight && showCode) //  added the if part to show unit of measure for Scalable product
                                {
                                    pf = Convert.ToString("$" + sl.Regular_Price.ToString(fs) + Convert.ToString(!string.IsNullOrEmpty(sl.UM) ? ("/" + sl.UM.Trim()) : ""));
                                }
                                else
                                {
                                    pf = Convert.ToString(fuelLine ? ("$" + sl.price.ToString(fs) + "/" + Variables.UnitMeasurement.Trim()) : ("$" + sl.price.ToString(fs)));
                                }
                                pi = Convert.ToString((pf.Length > pi.Length) ? pf : pi);

                                if (fuelLine)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, (short)(dWidth - 2)) + modStringPad.PadC(qi, (short)(qWidth + 2)) + modStringPad.PadL(pi, (short)(pWidth + 2)) + modStringPad.PadL(sl.Amount.ToString("$###,##0.00"), (short)(aWidth - 2)) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                }
                                else
                                {
                                    if (sl.NoPriceFormat)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, dWidth) + modStringPad.PadC(qi, qWidth) + modStringPad.PadL(pi, pWidth) + modStringPad.PadL("$" + Convert.ToString(sl.Amount), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                    }
                                    else
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, dWidth) + modStringPad.PadC(qi, qWidth) + modStringPad.PadL(pi, pWidth) + modStringPad.PadL(sl.Amount.ToString("$###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (breakAiteExempt || breakSwitchPrepay)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, dWidth) + modStringPad.PadC(qi, qWidth) + modStringPad.PadL(sl.Total_Amount.ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                            }
                            else
                            {
                                if (sl.NoPriceFormat)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, dWidth) + modStringPad.PadC(qi, qWidth) + modStringPad.PadL("$" + Convert.ToString(sl.Amount), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(longDescription, dWidth) + modStringPad.PadC(qi, qWidth) + modStringPad.PadL(sl.Amount.ToString("$###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                }
                            }
                        }
                    }

                    if (breakAiteExempt)
                    {
                        FileSystem.PrintLine(nH, new string('-', hWidth));
                        foreach (TaxExemptSaleLine tempLoopVarTesl in oTeSale.Te_Sale_Lines)
                        {
                            tesl = tempLoopVarTesl;
                            if (tesl.Line_Num == sl.Line_Num)
                            {
                                if (((tesl.ProductType == mPrivateGlobals.teProductEnum.eCigarette) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eCigar)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
                                {
                                    if (tesl.ExemptedTax != 0)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR("   " + _resourceManager.GetResString(offSet, (short)8393), (short)(hWidth - aWidth)) + modStringPad.PadL((tesl.ExemptedTax * -1).ToString("###,##0.00"), aWidth));
                                    }
                                }
                                else
                                {
                                    if (tesl.ExemptedTax != 0)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR("   " + _resourceManager.GetResString(offSet, (short)8394), (short)(hWidth - aWidth)) + modStringPad.PadL((tesl.ExemptedTax * -1).ToString("###,##0.00"), aWidth));
                                    }
                                }
                                break;
                            }
                        }
                        foreach (TaxCreditLine tempLoopVarTcl in oTeSale.TaxCreditLines)
                        {
                            tcl = tempLoopVarTcl;
                            if (tcl.Line_Num == sl.Line_Num)
                            {
                                foreach (Line_Tax tempLoopVarLt in tcl.Line_Taxes)
                                {
                                    lt = tempLoopVarLt;
                                    if (lt.Tax_Included)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR("   " + lt.Tax_Name + _resourceManager.GetResString(offSet, (short)8397), (short)(hWidth - aWidth)) + modStringPad.PadL((lt.Tax_Incl_Amount * -1).ToString("#0.00"), aWidth)); //"(Incl.) exempt"
                                    }
                                }
                                break;
                            }
                        }
                    }

                    if (sl.NoPriceFormat)
                    {
                        subTotal = subTotal + sl.Amount;
                    }
                    else
                    {
                        subTotal = (decimal)(modGlobalFunctions.Round((double)(subTotal + sl.Amount), 2));
                    }
                    var t = BitConverter.GetBytes(decimal.GetBits((decimal)sl.price)[3])[2];
                    var multiplier = Math.Pow(10, t);
                    var tempPrice = (int)(sl.Regular_Price * multiplier);
                    sl.Regular_Price = (double)tempPrice / multiplier;
                    if (breakSwitchPrepay)
                    {
                        exemptedTax = modGlobalFunctions.Round(sl.Quantity * sl.Regular_Price, 2) - modGlobalFunctions.Round(sl.Quantity * sl.price, 2);
                        if (exemptedTax != 0)
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8394), (short)(hWidth - aWidth - 10)) + modStringPad.PadL("$" + sl.price.ToString("#0.000") + "/" + Variables.UnitMeasurement.Trim(), (short)10) + modStringPad.PadL((exemptedTax * -1).ToString("###,##0.00"), aWidth));
                        }
                        else
                        {
                            breakSwitchPrepay = false;
                        }
                    }
                    if (taxExempt && (teType == "SITE") && sl.IsTaxExemptItem && _policyManager.TE_ByRate)
                    {
                        totalExemptedTax = totalExemptedTax + modGlobalFunctions.Round(sl.Quantity * sl.Regular_Price, 2) - modGlobalFunctions.Round(sl.Quantity * sl.price, 2); //  
                    }

                    if (sl.Line_Discount != 0.0D)
                    {
                        fs = Convert.ToString(pd == 0 ? "#,##0" : ("#,##0." + new string('0', pd)));
                        if (showCode)
                        {
                            if (sl.Discount_Type == "%")
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString("   " + Convert.ToString(string.IsNullOrEmpty(sl.DiscountName) ? (_resourceManager.GetResString(offSet, (short)107)) : sl.DiscountName) + "(" + sl.Discount_Rate.ToString("##0.0") + "%)"), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Line_Discount * -1).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol)); //" Discount ("

                            }
                            else
                            {
                                if (breakAiteExempt)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString("   " + Convert.ToString(string.IsNullOrEmpty(sl.DiscountName) ? (_resourceManager.GetResString(offSet, (short)107)) : sl.DiscountName)), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Line_Discount * -1).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol)); //"  Discount"

                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString(string.IsNullOrEmpty(sl.DiscountName) ? (_resourceManager.GetResString(offSet, (short)107)) : sl.DiscountName), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Line_Discount * -1).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol)); //"  Discount"
                                }
                            }
                        }
                        else
                        {
                            if (sl.Discount_Type == "%")
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString("   " + Convert.ToString(string.IsNullOrEmpty(sl.DiscountName) ? (_resourceManager.GetResString(offSet, (short)107)) : sl.DiscountName) + "(" + sl.Discount_Rate.ToString("##0.0") + "%)"), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Line_Discount * -1).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));

                            }
                            else
                            {
                                if (breakAiteExempt)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString("   " + Convert.ToString(string.IsNullOrEmpty(sl.DiscountName) ? (_resourceManager.GetResString(offSet, (short)107)) : sl.DiscountName)), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Line_Discount * -1).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol)); //"  Discount"
                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString(string.IsNullOrEmpty(sl.DiscountName) ? (_resourceManager.GetResString(offSet, (short)107)) : sl.DiscountName), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Line_Discount * -1).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol)); //"  Discount"
                                }
                            }
                        }
                        if (sl.Discount_Adjust != 0)
                        {
                            if (breakAiteExempt)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR("   " + _resourceManager.GetResString(offSet, (short)3209), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Discount_Adjust * -1).ToString("###,##0.00"), aWidth));
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3209), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Discount_Adjust * -1).ToString("###,##0.00"), aWidth));
                            }

                            subTotal = (decimal)(modGlobalFunctions.Round((double)subTotal - sl.Line_Discount - sl.Discount_Adjust, 2));
                        }
                        else
                        {
                            subTotal = (decimal)(modGlobalFunctions.Round((double)subTotal - sl.Line_Discount, 2));
                        }

                        discountName = Convert.ToString(string.IsNullOrEmpty(sl.DiscountName) ? (_resourceManager.GetResString(offSet, (short)107)) : sl.DiscountName);
                        fuelLoyaltyDiscount = fuelLoyaltyDiscount + (decimal)sl.Line_Discount;
                    }
                    else
                    {
                        if (sl.Discount_Adjust != 0)
                        {
                            if (breakAiteExempt)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR("   " + _resourceManager.GetResString(offSet, (short)3209), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Discount_Adjust * -1).ToString("###,##0.00"), aWidth));
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3209), (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Discount_Adjust * -1).ToString("###,##0.00"), aWidth));
                            }

                            subTotal = (decimal)(Math.Round((double)subTotal - sl.Line_Discount - sl.Discount_Adjust, 2));
                        }
                        else
                        {
                            subTotal = (decimal)(Math.Round((double)subTotal - sl.Line_Discount, 2));
                        }
                    }

                    if (breakAiteExempt)
                    {
                        FileSystem.PrintLine(nH, new string('-', hWidth));
                    }
                    else if (breakSwitchPrepay)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadL(new string('-', aWidth), hWidth));
                        FileSystem.PrintLine(nH, modStringPad.PadL("Fuel Total", (short)(hWidth - aWidth)) + modStringPad.PadL(((double)sl.Amount - sl.Line_Discount - sl.Discount_Adjust).ToString("###,##0.00"), aWidth));
                    }

                    if (sl.Charges.Count > 0 && !markDown && !@Void)
                    {
                        foreach (Charge tempLoopVarChargeRenamed in sl.Charges)
                        {
                            charge = tempLoopVarChargeRenamed;
                            taxSymbol = "E";
                            foreach (Charge_Tax tempLoopVarCtx in charge.Charge_Taxes)
                            {
                                ctx = tempLoopVarCtx;
                                if (ctx.Tax_Rate != 0)
                                {
                                    taxSymbol = Get_Tax_Symbol(taxSymbol, ctx.Tax_Included);
                                }
                            }
                            if (showCode)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR("  " + charge.Charge_Desc, (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Quantity * charge.Charge_Price).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR("  " + charge.Charge_Desc, (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Quantity * charge.Charge_Price).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                            }
                            subTotal = (decimal)(Math.Round((double)((float)subTotal + sl.Quantity * charge.Charge_Price), 2));
                        }
                    }

                    if (sl.Line_Kits.Count > 0)
                    {
                        foreach (Line_Kit tempLoopVarKit in sl.Line_Kits)
                        {
                            kit = tempLoopVarKit;
                            sumPrice = sumPrice + (decimal)(kit.Kit_Item_Base * kit.Kit_Item_Qty);
                        }

                        if (sumPrice != 0 & sl.Quantity != 0)
                        {
                            factor = (double)((float)sl.Amount / (sl.Quantity * (float)sumPrice));
                        }

                        foreach (Line_Kit tempLoopVarKit in sl.Line_Kits)
                        {
                            kit = tempLoopVarKit;

                            kit.Kit_Item_Allocate = (float)(factor * kit.Kit_Item_Base * kit.Kit_Item_Qty);

                            if (_policyManager.EXPAND_KITS)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR("   " + kit.Kit_Item_Qty.ToString("##0") + " - " + kit.Kit_Item_Desc.Trim(), hWidth));
                            }

                            if (kit.K_Charges.Count > 0 && !markDown && !@Void)
                            {
                                foreach (K_Charge tempLoopVarKitChg in kit.K_Charges)
                                {
                                    kitChg = tempLoopVarKitChg;
                                    taxSymbol = "E";
                                    foreach (Charge_Tax tempLoopVarCtx in kitChg.Charge_Taxes)
                                    {
                                        ctx = tempLoopVarCtx;
                                        if (ctx.Tax_Rate != 0)
                                        {
                                            taxSymbol = Get_Tax_Symbol(taxSymbol, ctx.Tax_Included);
                                        }
                                    }

                                    if (showCode)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR("  " + kitChg.Charge_Desc, (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Quantity * kit.Kit_Item_Qty * kitChg.Charge_Price).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));
                                    }
                                    else
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR("  " + kitChg.Charge_Desc, (short)(hWidth - aWidth)) + modStringPad.PadL((sl.Quantity * kit.Kit_Item_Qty * kitChg.Charge_Price).ToString("###,##0.00"), aWidth) + Convert.ToString(taxSymbol == "E" ? " " : taxSymbol));

                                    }
                                    subTotal = subTotal + (decimal)modGlobalFunctions.Round(sl.Quantity * kit.Kit_Item_Qty * kitChg.Charge_Price, 2);
                                }
                            }
                        }

                    }
                }

                aWidth = (short)10; // Amount Width
                qWidth = (short)7; // Quantity Width
                pWidth = (short)6; // Price Width
                hWidth = (short)40; // Total Receipt Width

                dWidth = Convert.ToInt16(hWidth - Convert.ToInt32(Convert.ToInt32(qWidth + Convert.ToInt32(showPrice ? pWidth : 0)) + aWidth));

                if (_policyManager.TAX_EXEMPT_GA)
                {
                    foreach (Sale_Tax tempLoopVarStx in sale.Sale_Totals.Sale_Taxes)
                    {
                        stx = tempLoopVarStx;
                        if (stx.Tax_Exemption_GA_Incl != 0)
                        {
                            totalTaxExemptGa = totalTaxExemptGa + stx.Tax_Exemption_GA_Incl;
                        }
                    }
                    if (totalTaxExemptGa != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)1712).Trim(), dWidth) + modStringPad.PadL(totalTaxExemptGa.ToString("###,##0.00"), (short)(hWidth - dWidth)));
                        subTotal = (decimal)(modGlobalFunctions.Round((double)(subTotal - totalTaxExemptGa), 2));
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadR(" ", dWidth) + modStringPad.PadL(" ", Convert.ToInt16(qWidth + Convert.ToInt32(showPrice ? pWidth : 0))) + modStringPad.PadR("_", aWidth, "_"));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)237), dWidth) + modStringPad.PadL(" ", Convert.ToInt16(qWidth + Convert.ToInt32(showPrice ? pWidth : 0))) + modStringPad.PadL(subTotal.ToString("###,##0.00"), aWidth)); //"Sub Total"

                if (!markDown && !@Void)
                {
                    foreach (Sale_Tax tempLoopVarStx in sale.Sale_Totals.Sale_Taxes)
                    {
                        stx = tempLoopVarStx;
                        if (stx.Tax_Added_Amount != 0)
                        {
                            if (Conversion.Int(stx.Tax_Rate * 10) != stx.Tax_Rate * 10)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(stx.Tax_Name.Trim() + " (" + stx.Tax_Rate.ToString("0.000") + "%)" + _resourceManager.GetResString(offSet, (short)236) + stx.Taxable_Amount.ToString("###,##0.00"), (short)(hWidth - aWidth)) + modStringPad.PadL(stx.Tax_Added_Amount.ToString("###,##0.00"), aWidth)); //" on $"
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(stx.Tax_Name.Trim() + " (" + stx.Tax_Rate.ToString("0.0") + "%)" + _resourceManager.GetResString(offSet, (short)236) + stx.Taxable_Amount.ToString("###,##0.00"), (short)(hWidth - aWidth)) + modStringPad.PadL(stx.Tax_Added_Amount.ToString("###,##0.00"), aWidth)); //" on $"
                            }
                            subTotal = (decimal)(modGlobalFunctions.Round((double)(subTotal + stx.Tax_Added_Amount), 2));
                        }
                    }
                }

                if (_policyManager.Tax_Rebate)
                {
                    boolPrintSubTotal = true;
                    foreach (Sale_Tax tempLoopVarStx in sale.Sale_Totals.Sale_Taxes)
                    {
                        stx = tempLoopVarStx;
                        if (stx.Tax_Rebate != 0)
                        {
                            if (boolPrintSubTotal)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)237), dWidth) + modStringPad.PadL(" ", Convert.ToInt16(qWidth + Convert.ToInt32(showPrice ? pWidth : 0))) + modStringPad.PadL(subTotal.ToString("###,##0.00"), aWidth)); //"Sub Total"
                            }
                            boolPrintSubTotal = false;
                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)474) + " " + stx.Tax_Name.Trim() + " (" + stx.Tax_Rebate_Rate.ToString("0.0") + "%)" + _resourceManager.GetResString(offSet, (short)236) + stx.Rebatable_Amount.ToString("###,##0.00"), (short)(hWidth - aWidth)) + modStringPad.PadL(stx.Tax_Rebate.ToString("###,##0.00"), aWidth)); //" on $"
                            subTotal = (decimal)(Math.Round((double)(subTotal - stx.Tax_Rebate), 2));
                        }
                    }
                }

                if (sale.Sale_Totals.Invoice_Discount != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)238), dWidth) + modStringPad.PadL(" ", Convert.ToInt16(qWidth + Convert.ToInt32(showPrice ? pWidth : 0))) + modStringPad.PadL((sale.Sale_Totals.Invoice_Discount * -1).ToString("###,##0.00"), aWidth)); //"Sale Discount"
                }

                if (sale.Sale_Totals.Penny_Adj != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)486), (short)(hWidth - aWidth)) + modStringPad.PadL(sale.Sale_Totals.Penny_Adj.ToString("###,##0.00"), aWidth));
                }

                FileSystem.PrintLine(nH, modStringPad.PadR(" ", dWidth) + modStringPad.PadL(" ", Convert.ToInt16(qWidth + Convert.ToInt32(showPrice ? pWidth : 0))) + modStringPad.PadR("_", aWidth, "_"));


                FileSystem.PrintLine(nH, modStringPad.PadR(totalLineCaption, (short)(hWidth - aWidth)) + modStringPad.PadL((markDown || @Void ? sale.Sale_Totals.Net : sale.Sale_Totals.Gross + sale.Sale_Totals.Penny_Adj).ToString("###,##0.00"), aWidth));

                if (!markDown && !@Void)
                {
                    FileSystem.PrintLine(nH, " ");
                    foreach (Sale_Tax tempLoopVarStx in sale.Sale_Totals.Sale_Taxes)
                    {
                        stx = tempLoopVarStx;
                        if (stx.Tax_Included_Amount != 0.0M)
                        {
                            if (Conversion.Int(stx.Tax_Rate * 10) != stx.Tax_Rate * 10)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(stx.Tax_Name.Trim() + " (" + stx.Tax_Rate.ToString("0.000") + "%)" + _resourceManager.GetResString(offSet, (short)239) + stx.Tax_Included_Total.ToString("###,##0.00"), (short)(hWidth - aWidth)) + modStringPad.PadL(stx.Tax_Included_Amount.ToString("###,##0.00"), aWidth)); // " included in $"
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(stx.Tax_Name.Trim() + " (" + stx.Tax_Rate.ToString("0.0") + "%)" + _resourceManager.GetResString(offSet, (short)239) + stx.Tax_Included_Total.ToString("###,##0.00"), (short)(hWidth - aWidth)) + modStringPad.PadL(stx.Tax_Included_Amount.ToString("###,##0.00"), aWidth)); // " included in $"
                            }
                        }
                    }
                }

                FileSystem.PrintLine(nH, " ");
                FileSystem.PrintLine(nH, " ");
                sumTender = 0;

                creTotal = 0;


                if (tenders == null || markDown || runAway || @Void || pumpTest) //Shiny  added the PumpTest criteria
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadC("=", hWidth, "="));

                    if (@Void)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8382).ToUpper(), hWidth)); //"Transaction Voided"
                    }
                    else
                    {

                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)240).ToUpper(), hWidth)); //"NO TENDERS"

                    }
                    FileSystem.PrintLine(nH, modStringPad.PadC("=", hWidth, "="));
                    FileSystem.PrintLine(nH);
                }
                else
                {
                    foreach (Tender tempLoopVarTenderRenamed in tenders)
                    {
                        tender = tempLoopVarTenderRenamed;
                        if (tender.Amount_Used != 0)
                        {

                            if (tender.Tender_Class == "CRCARD")
                            {
                                creTotal = creTotal + (double)tender.Amount_Used;
                            }

                            if (tender.Tender_Class == "POINTS")
                            {
                                pointsUsed = tender.Amount_Entered; // Print 'Entered' and 'Used' if they are different. Just print
                            }

                            if (tender.Tender_Class == "COUPON")
                            {

                                SaleVendorCoupon saleVendorCoupons = new SaleVendorCoupon();
                                if (!reprint)
                                    saleVendorCoupons = CacheManager.GetSaleVendorCoupon(sale.Sale_Num, tender.Tender_Code);
                                else
                                {
                                    var saleVendorCouponLines = _saleService.GetSaleVendorCouponsForReprint(sale.Sale_Num, till.Number);
                                    if (saleVendorCouponLines == null)
                                        saleVendorCoupons = null;
                                    else
                                    {

                                        saleVendorCoupons.Sale_Num = sale.Sale_Num;
                                        foreach (var line in saleVendorCouponLines)
                                        {
                                            _svcManager.Add_a_Line(ref saleVendorCoupons, till.Number, line, false);
                                        }
                                    }
                                }
                                if (saleVendorCoupons != null)
                                {
                                    FileSystem.PrintLine(nH);
                                    FileSystem.PrintLine(nH, modStringPad.PadR(tender.Tender_Name, hWidth));
                                    foreach (SaleVendorCouponLine saleVendorCoupon in saleVendorCoupons.SVC_Lines)
                                    {
                                        FileSystem.PrintLine(nH, saleVendorCoupon.CouponCode + " - " + saleVendorCoupon.CouponName);
                                        FileSystem.PrintLine(nH, modStringPad.PadC(saleVendorCoupon.SerialNumber, (short)(hWidth - aWidth)) + modStringPad.PadL(saleVendorCoupon.TotalValue.ToString("$#0.00"), aWidth));
                                    }
                                    FileSystem.PrintLine(nH);
                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(tender.Tender_Name, (short)(hWidth - aWidth)) + modStringPad.PadL(tender.Amount_Used.ToString("###,##0.00"), aWidth));
                                }
                                sumTender = sumTender + (decimal)modGlobalFunctions.Round((double)tender.Amount_Used, 2);
                            }
                            else
                            {

                                if (tender.Amount_Entered != tender.Amount_Used)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(tender.Tender_Name + "  (" +
                                        tender.Amount_Entered.ToString("###,##0.00") + ")", (short)(hWidth - aWidth)) + modStringPad.PadL(tender.Amount_Used.ToString("###,##0.00"), aWidth));
                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(tender.Tender_Name, (short)(hWidth - aWidth)) + modStringPad.PadL(tender.Amount_Used.ToString("###,##0.00"), aWidth));
                                }
                                sumTender = sumTender + (decimal)modGlobalFunctions.Round((double)tender.Amount_Used, 2);

                                if (tender.Tender_Class == "ACCOUNT")
                                {
                                    if (!string.IsNullOrEmpty(sale.AR_PO))
                                    {
                                        strArPo = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)393), (short)(hWidth - 15)) + modStringPad.PadL(sale.AR_PO.Trim(), (short)15);
                                    }
                                }
                            }
                            if (_policyManager.FSGC_ENABLE && tender.Tender_Code == _policyManager.ARTender && !boolArTenderUsed)
                            {
                                boolArTenderUsed = true;
                            }
                        }
                    }

                    FileSystem.PrintLine(nH, modStringPad.PadR(" ", (short)(hWidth - aWidth)) + modStringPad.PadR("_", aWidth, "_"));

                    if (sale.Sale_Totals.Gross >= 0.0M)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)241), (short)(hWidth - aWidth)) + modStringPad.PadL(sumTender.ToString("###,##0.00"), aWidth)); //"Total Tendered"
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)242), (short)(hWidth - aWidth)) + modStringPad.PadL(sumTender.ToString("###,##0.00"), aWidth)); //"Total Refunded"
                    }

                    if (tenders.Tend_Totals.ChangePaid != 0)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)279), (short)(hWidth - aWidth)) + modStringPad.PadL((System.Math.Abs(tenders.Tend_Totals.ChangePaid)).ToString("###,##0.00"), aWidth)); //"Change "
                    }

                    FileSystem.PrintLine(nH, " ");

                    if (Math.Round((double)tenders.Tend_Totals.Change, 2) > 0.0D)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)243), (short)(hWidth - aWidth)) + modStringPad.PadL((System.Math.Abs(tenders.Tend_Totals.Change)).ToString("###,##0.00"), aWidth)); //"Refunded "

                    }
                    else if (Math.Round((double)tenders.Tend_Totals.Change, 2) < 0.0D)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)220), (short)(hWidth - aWidth)) + modStringPad.PadL((System.Math.Abs(tenders.Tend_Totals.Change)).ToString("###,##0.00"), aWidth)); //"Change "

                    }


                    if (sale.OverPayment > 0)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)280), (short)(hWidth - aWidth)) + modStringPad.PadL((System.Math.Abs(sale.OverPayment)).ToString("#,##0.00"), aWidth));
                    }
                    else if (sale.OverPayment < 0) // Print cash short
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8353), (short)(hWidth - aWidth)) + modStringPad.PadL((System.Math.Abs(sale.OverPayment)).ToString("#,##0.00"), aWidth));
                    }

                    if (!string.IsNullOrEmpty(strArPo))
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, strArPo);
                    }
                }

                if (_policyManager.TAX_EXEMPT_GA)
                {
                    foreach (Sale_Tax tempLoopVarStx in sale.Sale_Totals.Sale_Taxes)
                    {
                        stx = tempLoopVarStx;
                        if (stx.Tax_Exemption_GA_Added != 0)
                        {
                            totalTaxExemptGaAdd = totalTaxExemptGaAdd + stx.Tax_Exemption_GA_Added;
                        }
                    }
                    if (totalTaxExemptGaAdd != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)1712).Trim(), dWidth) + modStringPad.PadL(totalTaxExemptGaAdd.ToString("###,##0.00"), (short)(hWidth - dWidth)));
                        FileSystem.PrintLine(nH);
                    }
                }

                if (storeCredit != null)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)244) + storeCredit.Number + _resourceManager.GetResString(offSet, (short)245), (short)(hWidth - aWidth)) + modStringPad.PadL(storeCredit.Amount.ToString("0.00"), aWidth)); //"Store Credit #"," issued"
                }

                string lType = "";
                decimal lSave = new decimal();
                float ppd;
                short lPriceCode = 0;
                if (!markDown && !@Void)
                {
                    if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && _policyManager.USE_LOYALTY)
                    {

                        lType = Convert.ToString(_policyManager.LOYAL_TYPE);

                        switch (lType)
                        {

                            case "Prices":
                                lPriceCode = Convert.ToInt16(_policyManager.LOYAL_PRICE);
                                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                                {
                                    sl = tempLoopVarSl;
                                    if (sl.Price_Number == lPriceCode)
                                    {
                                        sl.Loyalty_Save = (float)(modGlobalFunctions.Round((sl.Regular_Price * sl.Quantity) - (double)sl.Amount, 2)); // Nicolette added round to 2, otherwise loyalty save will be round to 0
                                                                                                                                                      //shiny end
                                        lSave = lSave + (decimal)sl.Loyalty_Save;
                                    }
                                }
                                if (lSave != 0)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + " " + _resourceManager.GetResString(offSet, (short)246) + lSave.ToString("0.00"), hWidth));
                                }
                                break;

                            case "Points":
                                if (!reprint)
                                {
                                    sumQa = _saleManager.ComputePoints(sale);

                                    if (!_policyManager.GIVE_POINTS && sumQa > 0)
                                    {
                                        sumQa = sumQa - pointsUsed;
                                        if (sumQa < 0)
                                        {
                                            sumQa = 0;
                                        }
                                    }
                                    if (!_policyManager.GIVE_POINTS && sumQa < 0)
                                    {
                                        sumQa = sumQa - pointsUsed;
                                        if (sumQa > 0)
                                        {
                                            sumQa = 0;
                                        }
                                    }

                                    sale.Customer.PointsAwarded = (decimal)(modGlobalFunctions.Round((double)sumQa, 2));
                                    sale.Customer.Loyalty_Points = sale.Customer.Loyalty_Points + (double)sale.Customer.PointsAwarded - (double)pointsUsed;
                                    if (sale.Customer.Loyalty_Points >= 0 & sale.Customer.PointsAwarded != 0)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + " " + _resourceManager.GetResString(offSet, (short)222) + " : " + sale.Customer.PointsAwarded.ToString("0.00"), hWidth));
                                    }
                                    if (sale.Customer.Loyalty_Points >= 0)
                                    {
                                        if (_policyManager.SHOW_POINT)
                                        {
                                            FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8301) + " " + sale.Customer.Loyalty_Points.ToString("0.00"), hWidth));
                                        }
                                    }
                                }
                                else
                                {
                                    if (sale.LoyaltyPoints != 0)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + " " + _resourceManager.GetResString(offSet, (short)222) + " : " + sale.LoyaltyPoints.ToString("0.00"), hWidth));
                                    }
                                }
                                break;

                            case "Discounts":
                                lPriceCode = Convert.ToInt16(_policyManager.LOYAL_PRICE);
                                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                                {
                                    sl = tempLoopVarSl;
                                    if (sl.Discount_Rate > 0 && sl.LoyaltyDiscount)
                                    {
                                        sl.Loyalty_Save = (float)(modGlobalFunctions.Round(sl.Line_Discount, 2));
                                        lSave = lSave + (decimal)sl.Loyalty_Save;
                                    }
                                }
                                if (lSave != 0)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + " " + _resourceManager.GetResString(offSet, (short)246) + lSave.ToString("0.00"), hWidth));
                                }
                                break;
                        }
                        FileSystem.PrintLine(nH);
                    }
                }
                maskCard = Convert.ToBoolean(_policyManager.MASK_CARDNO);
                copies = (short)1;
                if (!reprint)
                {
                    if (sale.Sale_Totals.Gross < 0)
                    {
                        // Copies = Convert.ToInt16(_policyManager.RefundReceiptCopies);
                    }

                    sigLine = false;
                    if (!(tenders == null))
                    {
                        foreach (Tender tempLoopVarTenderRenamed in tenders)
                        {
                            tender = tempLoopVarTenderRenamed;
                            if (!string.IsNullOrEmpty(tender.Credit_Card.Cardnumber) && (tender.Credit_Card.Result == "0" || tender.Credit_Card.GiftType == "G"))
                            {
                                if ((emvVersion == false) || (emvVersion == true && tender.Tender_Class != "DBCARD" && tender.Tender_Class != "CRCARD"))
                                {
                                    Print_Credit_Card(tender.Credit_Card, maskCard, (short)nH);
                                }

                                if (!(tender == null))
                                {
                                    if (_policyManager.RSTR_PROFILE)
                                    {
                                        if (!string.IsNullOrEmpty(tender.Credit_Card.PONumber))
                                        {
                                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)472), (short)20) + modStringPad.PadL(tender.Credit_Card.PONumber, (short)20));
                                        }
                                        PrintCardProfilePrompts(tender.Credit_Card.Cardnumber, sale.Sale_Num, sale.TillNumber, nH);
                                    }
                                }
                                FileSystem.PrintLine(nH);

                                if (tender.Tender_Class == "FLEET" && _policyManager.COMBINEFLEET)
                                {
                                    if (tender.Credit_Card.PrintCopies > copies)
                                    {
                                        copies = tender.Credit_Card.PrintCopies;
                                    }
                                }

                            }
                            if ((emvVersion == false) || (emvVersion == true && tender.Tender_Class != "DBCARD" && tender.Tender_Class != "CRCARD"))
                            {
                                if (tender.Amount_Used != 0 && tender.SignatureLine)
                                {
                                    sigLine = true;
                                }
                            }
                            if ((tender.PrintCopies > copies) && (tender.Amount_Used != 0))
                            {
                                copies = tender.PrintCopies;
                            }
                        }
                    }


                }
                else
                {
                    RePrint_Credit_Card(till, reprintCards, maskCard, (short)nH, sale);
                    FileSystem.PrintLine(nH);
                }

                if (!(tenders == null))
                {
                    foreach (Tender tempLoopVarTenderRenamed in tenders)
                    {
                        tender = tempLoopVarTenderRenamed;
                        var creditCard = tender.Credit_Card;
                        if (tender.Amount_Used != 0 && tender.Tender_Class == "ACCOUNT" && tender.SignatureLine && _policyManager.PRINT_CrdHld && tender.Credit_Card.Crd_Type == "F")
                        {
                            fleetLanguage = _creditCardManager.Language(ref creditCard).Substring(0, 1).ToUpper() == "F" ? "F" : "E";
                            printCardHolderAgree = true;
                        }
                        if ((emvVersion == false) || (emvVersion == true && tender.Tender_Class != "DBCARD" && tender.Tender_Class != "CRCARD"))
                        {
                            if (tender.Amount_Used != 0 && tender.SignatureLine)
                            {
                                sigLine = true;
                            }
                        }
                    }
                }


                if (_policyManager.REWARDS_Enabled)
                {
                    sCardNumber = sale?.SaleHead?.LoyaltyCard;
                    if (!string.IsNullOrEmpty(sCardNumber))
                    {
                        dPointEarned = Convert.ToDouble(sale?.SaleHead?.LoyalPoint);
                        dBalance = Convert.ToDouble(sale?.SaleHead?.LoyaltyBalance);

                        if (!string.IsNullOrEmpty(sCardNumber))
                        {
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, modStringPad.PadC("-", (short)40, "-"));
                            FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)502), (short)22) + modStringPad.PadL(GetMaskedCardNum(sCardNumber, "L"), (short)18));
                            FileSystem.PrintLine(nH, modStringPad.PadL(_policyManager.REWARDS_Caption + ": ", (short)22) + modStringPad.PadL(dPointEarned.ToString("#0.00"), (short)18));
                            FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)503), (short)22) + modStringPad.PadL(dBalance.ToString("#0.00"), (short)18));
                            FileSystem.PrintLine(nH, modStringPad.PadC("=", (short)40, "="));
                        }
                    }
                }


                if (printCardHolderAgree)
                {
                    if (fleetLanguage == "F")
                    {
                        FileSystem.PrintLine(nH, "    Le Titulaire versera ce montant a  ");
                        FileSystem.PrintLine(nH, "        L\'emetteur conformement au    ");
                        FileSystem.PrintLine(nH, "             contrat adherent  ");
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, " Cardholder will pay card issuer above ");
                        FileSystem.PrintLine(nH, "amount pursuant to Cardholder Agreement");
                    }
                }
                if (affdaCustomerUsed)
                {
                    sigLine = true;
                    if (copies < 2)
                    {
                        copies = (short)2;
                    }
                }
                double taxsaved = 0;
                bool excludedExempt = false;
                if (!runAway)
                {

                    if (taxExempt && _policyManager.PrtExmptTax)
                    {
                        //var saleTemp = _saleService.GetSale(sale.TillNumber, sale.Sale_Num);

                        foreach(Sale_Line saleLine in sale.Sale_Lines)
                        {
                            taxsaved = taxsaved + (modGlobalFunctions.Round(saleLine.Quantity * saleLine.Regular_Price, 2) - modGlobalFunctions.Round(saleLine.Quantity * saleLine.price, 2));
                        }


                        if ((!reprint) && teType == "SITE")
                        {
                            //  var oPurchaseList = CacheManager.GetPurchaseListSaleForTill(till.Number, sale.Sale_Num);
                            //  var oPurchaseList = Chaps_Main.oPurchaseList;

                            //if (oTeSale == null)
                            //{
                            //    oTeSale = _taxExemptService.LoadTaxExempt(teType, sale.Sale_Num, sale.TillNumber,
                            //     DataSource.CSCTills) ??
                            // _taxExemptService.LoadTaxExempt(teType, sale.Sale_Num, sale.TillNumber,
                            //     DataSource.CSCTrans);
                            //}

                            //if (!(oTeSale == null))
                            //{
                            //if (oPurchaseList.Count() > 0)
                            //{

                            
                                    if (taxsaved != 0 || totalExemptedTax !=0)
                                    {
                                        FileSystem.PrintLine(nH);
                                        if (_policyManager.TE_ByRate)
                                        {
                                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)385), (short)(hWidth - aWidth)) + modStringPad.PadL(totalExemptedTax.ToString("###,##0.00"), aWidth));
                                        }
                                        else
                                        {
                                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)385), (short)(hWidth - aWidth)) + modStringPad.PadL(taxsaved.ToString("###,##0.00"), aWidth));
                                        }
                                        if (_policyManager.TE_ByRate)
                                        {
                                            if (fuelLoyaltyDiscount != 0)
                                            {
                                                FileSystem.PrintLine(nH, modStringPad.PadR(discountName, (short)(hWidth - aWidth)) + modStringPad.PadL(fuelLoyaltyDiscount.ToString("###,##0.00"), aWidth));
                                            }
                                            if (_policyManager.TE_ByRate)
                                            {
                                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)475) + " ", (short)(hWidth - aWidth)) + modStringPad.PadL(((decimal)totalExemptedTax + fuelLoyaltyDiscount).ToString("###,##0.00"), aWidth));
                                            }
                                            else
                                            {
                                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)475) + " ", (short)(hWidth - aWidth)) + modStringPad.PadL((taxsaved + (float)fuelLoyaltyDiscount).ToString("###,##0.00"), aWidth));
                                            }
                                        }
                                        FileSystem.PrintLine(nH);
                                    }
                                //}
                            //}
                        }
                        else
                        {
                            if (teType == "SITE" && taxsaved != 0)
                            {

                                FileSystem.PrintLine(nH);
                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)385), (short)(hWidth - aWidth)) + modStringPad.PadL(taxsaved.ToString("###,##0.00"), aWidth));

                                if (_policyManager.TE_ByRate)
                                {
                                    if (fuelLoyaltyDiscount != 0)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR(discountName, (short)(hWidth - aWidth)) + modStringPad.PadL(fuelLoyaltyDiscount.ToString("###,##0.00"), aWidth));
                                    }
                                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)475) + " ", (short)(hWidth - aWidth)) + modStringPad.PadL((taxsaved + (float)fuelLoyaltyDiscount).ToString("###,##0.00"), aWidth));
                                }

                                FileSystem.PrintLine(nH);

                            }
                            else
                            {
                                excludedExempt = false;
                                if (!(oTeSale == null))
                                {
                                    foreach (Sale_Tax tempLoopVarTc in oTeSale.TaxCredit)
                                    {
                                        tc = tempLoopVarTc;
                                        if (tc.Tax_Added_Amount != 0)
                                        {
                                            excludedExempt = true;
                                            break;
                                        }
                                    }
                                }
                                if (excludedExempt)
                                {
                                    FileSystem.PrintLine(nH, new string('-', hWidth));
                                    foreach (Sale_Tax tempLoopVarTc in oTeSale.TaxCredit)
                                    {
                                        tc = tempLoopVarTc;
                                        if (tc.Tax_Added_Amount != 0)
                                        {
                                            FileSystem.PrintLine(nH, modStringPad.PadR(tc.Tax_Name + _resourceManager.GetResString(offSet, (short)8396), (short)(hWidth - aWidth)) + modStringPad.PadL(tc.Tax_Added_Amount.ToString("#0.00"), aWidth)); //"(Excl.) exempt"
                                        }
                                    }
                                    FileSystem.PrintLine(nH, new string('-', hWidth));
                                    FileSystem.PrintLine(nH);
                                }
                            }
                        }
                    }
                    if (((!_policyManager.PRT_CPN && sale.Customer.DiscountType == "C") || sale.Customer.DiscountType != "C") && hasFuelSale && _policyManager.FuelLoyalty && sale.Customer.GroupID != "" && sale.Customer.DiscountType != "B")
                    {
                        Print_FuelLoyalty(sale, nH, reprint, completePrepay, runAway, hasPrepay, pumpTest); // 
                    }

                    if (hasFuelSale && _policyManager.CashBonus && sale.Customer.GroupID != "" && sale.Customer.DiscountType == "B" && sale.CBonusTotal != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_policyManager.CBonusName + ": ", (short)(hWidth - aWidth)) + modStringPad.PadL(sale.CBonusTotal.ToString("$###.00"), aWidth));
                    }

                    if (hasFuelSale && _policyManager.CashBonus && sale.Customer.GroupID != "" && sale.Customer.DiscountType == "B" && sale.CBonusTotal != 0)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, sale.Customer.Footer);
                    }

                    if (!reprint)
                    {
                        if (!(tenders == null))
                        {
                            foreach (Tender tempLoopVarTenderRenamed in tenders)
                            {
                                tender = tempLoopVarTenderRenamed;
                                if (modTPS.cc != null)
                                {
                                    if (!(string.IsNullOrEmpty(modTPS.cc.Report)) && tender.Tender_Class == "FLEET" && tender.Credit_Card.GiftType == "W")
                                    {
                                        FileSystem.PrintLine(nH, modTPS.cc.Report);
                                        report.Copies = tender.PrintCopies;
                                        taxExempt = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var reciept = _reportService.GetWexStringBySaleNumber(sale.Sale_Num);
                        FileSystem.PrintLine(nH, reciept);
                    }
                    if (taxExempt && _policyManager.TE_SIGNATURE && Strings.UCase(Convert.ToString(_policyManager.TE_SIGNMODE)) == "MANUAL" && sale.TreatyNumber != "") // if treaty sale and require signatureline we are checking whether there
                    {
                        electronicSignature = false;
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)247) + ":  ", hWidth, "_")); //Signature
                        FileSystem.PrintLine(nH);
                    }
                    else
                    {
                        if (sigLine || printSignForRefund || (taxExempt && _policyManager.TE_SIGNATURE && Strings.UCase(Convert.ToString(_policyManager.TE_SIGNMODE)) == "READER" && sale.TreatyNumber != ""))
                        {
                            //if (LoadElectronicSignature(sale.Sale_Num, sale.TillNumber, out signature))
                            //{
                            //    electronicSignature = true;
                            //}
                            //else
                            {
                                electronicSignature = false;
                                FileSystem.PrintLine(nH);
                                FileSystem.PrintLine(nH);
                                FileSystem.PrintLine(nH);
                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)247) + ":  ", hWidth, "_")); //Signature
                                FileSystem.PrintLine(nH);
                            }
                        }
                    }
                    if (teType == "QITE" && !string.IsNullOrEmpty(sale.TreatyNumber))
                    {
                        FileSystem.PrintLine(nH, store.TaxExempt_Footer);
                    }
                    else
                    {
                        if (sale.Sale_Totals.Gross >= 0)
                        {
                            FileSystem.PrintLine(nH, store.Sale_Footer);
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, store.Refund_Footer);
                        }
                    }
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)283) + ": ", hWidth, "_"));
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)284) + ": ", hWidth, "_"));
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)285) + ": ", hWidth, "_"));
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)119) + ": ", hWidth, "_"));
                }



               

                if (_policyManager.PRT_CPN && hasFuelSale && _policyManager.FuelLoyalty && sale.Customer.GroupID != "" && sale.Customer.DiscountType == "C")
                {
                    FileSystem.PrintLine(nH, Strings.ChrW(169)); // this line sends a paper cut command to the printer
                    Print_FuelLoyalty(sale, nH, reprint, completePrepay, runAway, hasPrepay);
                }
                Chaps_Main.FSGDCouponReceipt = "";
                DateTime cDateExp = default(DateTime);
                string couString = "";
                object crePercentage = null;
                object othPercentage = null;
                double discountAmt = 0;
                if (validGrCouSa == true && fuelTotal > 0 && !string.IsNullOrEmpty(Chaps_Main.FSGDCouponStr) && !boolArTenderUsed) //   added And not boolARTenderUsed condition
                {
                    cDateExp = DateAndTime.DateAdd(DateInterval.Day, Convert.ToDouble(_policyManager.FSGC_EXP), DateAndTime.Today);

                    crePercentage = ((decimal)creTotal / (sale.Sale_Totals.Gross)) * 100; //  Sum_Tender
                    if (Convert.ToInt32(crePercentage) > 100)
                    {
                        crePercentage = 100;
                    }
                    othPercentage = 100 - Convert.ToInt32(crePercentage);

                    discountAmt = Convert.ToDouble(((fuelTotal / 100) * Convert.ToDouble(crePercentage)) * _policyManager.FSGC_CREDIT);
                    discountAmt = discountAmt + (((fuelTotal / 100) * Convert.ToDouble(othPercentage)) * _policyManager.FSGC_OTHER);

                    discountAmt = discountAmt / 100;

                    couString = Chaps_Main.FSGDCouponStr;

                    couString = couString.Replace("@AMT@", discountAmt.ToString("###,##0.00"));
                    couString = couString.Replace("@DATE@", cDateExp.ToString("MM/dd/yyyy"));
                    couString = couString.Replace("@PLU@", _policyManager.FSGC_PLU);

                    Chaps_Main.FSGDCouponReceipt = Chaps_Main.FSGDCouponReceipt + couString;
                }

                if (carwashProduct)
                {
                    if (_policyManager.IsCarwashSupported && _policyManager.IsCarwashIntegrated)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, Chaps_Main.SA.CarwashReceipt);
                    }
                }

                for (n = 1; n <= advLines; n++) // GetPol("ADV_LINES")
                {
                    FileSystem.PrintLine(nH);
                }

                Chaps_Main.Last_Printed = fileName;
                FileSystem.FileClose(nH);
                if (_policyManager.Use_KickBack && sale.Customer.PointCardNum != "" && !reprint)
                {

                   // Print_KickBack(sale, reprint);
                   // modPrint.Dump_To_Printer(Path.GetTempPath() + "\\" + "KickBack_Receipt" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".txt", (short)1, true, true, false);
                }
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,Print_Receipt,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                report.ReportContent = GetReportContent(stream);
                report.Copies = copies > 1 ? copies : 1;
                return report;
                //todo
               //add the code here
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,Print_Receipt,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(nH);
                FileSystem.Kill(fileName);
            }
        }



        /// <summary>
        /// Method to get masked card number
        /// </summary>
        /// <param name="cardNum">Card number</param>
        /// <param name="cardType">Card type</param>
        public string GetMaskedCardNum(string cardNum, string cardType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,GetMaskedCardNum,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string returnValue = "";
            string st = "";
            if (cardType == "F" || cardType == "G" || cardType == "K") //shiny added the K - kickback
            {
                if (cardNum.Length > 8)
                {
                    st = cardNum.Substring(0, 4);
                    st = st + new string('*', cardNum.Length - 8);
                    st = st + cardNum.Substring(cardNum.Length - 4, 4);
                    returnValue = st;
                }
                else
                {
                    returnValue = cardNum;
                }
                return returnValue;
            }
            if (cardType == "T" || cardType == "L")
            {
                if (cardNum.Length > 9)
                {
                    returnValue = cardNum.Substring(0, 6) + new string('*', cardNum.Length - 9) + cardNum.Substring(cardNum.Length - 3, 3);
                }
                else
                {
                    returnValue = cardNum;
                }
                return returnValue;
            }
            if (_policyManager.BankSystem != "Moneris")
            {
                if (cardNum.Length > 4)
                {
                    st = new string('*', cardNum.Length - 4);
                    st = st + cardNum.Substring(cardNum.Length - 4, 4);
                    returnValue = st;
                }
                else
                {
                    returnValue = cardNum;
                }
            }
            else
            {
                if (cardType.ToUpper() == "D")
                {
                    if (cardNum.Length > 10)
                    {
                        st = cardNum.Substring(0, 10);
                        st = st + new string('*', cardNum.Length - 10);
                        returnValue = st;
                    }
                    else
                    {
                        returnValue = cardNum;
                    }
                }
                else
                {
                    if (cardNum.Length > 4)
                    {
                        st = new string('*', cardNum.Length - 4);
                        st = st + cardNum.Substring(cardNum.Length - 4, 4);
                        returnValue = st;
                    }
                    else
                    {
                        returnValue = cardNum;
                    }
                }
            }
            Performancelog.Debug($"End,ReceiptManager,GetMaskedCardNum,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to load electronic signature
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="tillNo">Till number</param>
        /// <param name="stream">Stream</param>
        /// <returns>True or false</returns>
        public bool LoadElectronicSignature(int saleNo, byte tillNo, out Stream stream)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,LoadElectronicSignature,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            bool returnValue = false;

            stream = _reportService.GetSignature(saleNo, tillNo);
            if (stream.Length > 0)
            {
                returnValue = true;
            }
            Performancelog.Debug($"End,ReceiptManager,LoadElectronicSignature,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to find if any flas report is available or not
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        public bool IsFlashReportAvailable(int tillNumber)
        {
            return _reportService.IsSaleAvailableForTill(tillNumber);
        }

        /// <summary>
        /// Method to find if a user can audit till or not
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>True or false</returns>
        public bool UserCanAuditTill(string userCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,UserCanAuditTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var user = _loginManager.GetExistingUser(userCode);
            Performancelog.Debug($"End,ReceiptManager,UserCanAuditTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Convert.ToBoolean(_policyManager.GetPol("U_TILLAUDIT", user));
        }

        /// <summary>
        /// Method to print cash draw
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="registerNumber">Register Number</param>
        /// <param name="userCode">User</param>
        /// <param name="coins">Coins</param>
        /// <param name="bills">Bills</param>
        /// <param name="returnReason">Return Reason</param>
        /// <param name="totalAmount">Total amount</param>
        /// <returns></returns>
        public FileStream Print_Draw(Till till, short registerNumber, string userCode, List<Cash> coins, List<Cash> bills,
            Return_Reason returnReason, decimal totalAmount)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Print_Draw,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            User user = _loginManager.GetExistingUser(userCode);
            Store store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short nH = 0;
            string fileName = "";
            short n = 0;
            decimal sTotal = new decimal();
            decimal cbTotal = new decimal();
            int bv = 0;
            string just = "";
            if (totalAmount == 0)
            {
                return null;
            }
            short hWidth = 40;
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);

            fileName = Path.GetTempPath() + "CashDraw" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            sTotal = 0;
            try
            {
                nH = (short)(FileSystem.FreeFile());
                just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification

                FileSystem.FileOpen(nH, fileName, OpenMode.Output);
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street1), hWidth));

                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, Convert.ToString(store.Address.PostalCode), hWidth));
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8135), (short)40));
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)213) + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateTime.Now.ToString(timeFormatHm), (short)40));

                if (_policyManager.PRN_UName)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)214) + ": " + user.Code + " - " + user.Name, (short)40)); //"Draw By: "
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)214) + ": " + user.Code, (short)40)); //"Draw By: "
                }

                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)132) + ": " + Convert.ToString(registerNumber), (short)40)); //"Register: "
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)131) + ": " + till.Number, (short)40)); //"Till    : "
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)389) + ": " + till.ShiftDate.ToString("dd/MM/yyyy"), hWidth)); //Shiftdate
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)346) + ": " + till.Shift, hWidth)); //Shift id
                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)115), (short)20) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)209), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)106), (short)10)); //"Tender","Qty","Amount"
                FileSystem.PrintLine(nH, modStringPad.PadR("_", (short)20, "_") + modStringPad.PadL("_", (short)10, "_") + modStringPad.PadL("_", (short)10, "_"));

                FileSystem.PrintLine(nH);
                if (totalAmount != 0)
                {

                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)212));
                    n = (short)0;
                    foreach (var coin in coins)
                    {
                        bv = coin.Quantity;
                        if (bv > 0 && Strings.Trim(coin.CurrencyName) != _resourceManager.GetResString(offSet, (short)336))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(coin.CurrencyName, (short)20) + modStringPad.PadL((bv).ToString(), (short)8) + modStringPad.PadL((bv * coin.Value).ToString("#,##0.00"), (short)12));
                            sTotal = sTotal + (bv * Convert.ToInt32(coin.Value));
                        }
                        n++;
                    }

                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)211)); //"BILLS"
                    n = (short)0;
                    foreach (var bill in bills)
                    {
                        bv = bill.Quantity;
                        if (bv > 0 && bill.CurrencyName != _resourceManager.GetResString(offSet, (short)336))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(bill.CurrencyName, (short)20) + modStringPad.PadL((bv).ToString(), (short)8) + modStringPad.PadL((bv * Convert.ToInt32(bill.Value)).ToString("#,##0.00"), (short)12));
                            sTotal = sTotal + (bv * Convert.ToInt32(bill.Value));
                        }
                        n++;
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadR("_", (short)20, "_") + modStringPad.PadL("_", (short)10, "_") + modStringPad.PadL("_", (short)10, "_"));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)210), (short)20) + modStringPad.PadL(sTotal.ToString("#,##0.00"), (short)20)); //"TOTAL"
                }

                FileSystem.PrintLine(nH);
                if (returnReason != null)
                {
                    if (!string.IsNullOrEmpty(returnReason.Description))
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)196) + " " + returnReason.Description, (short)40));
                    }
                }
                FileSystem.PrintLine(nH);
                FileSystem.FileClose(nH);
                Chaps_Main.Last_Printed = "CashDraw.txt";
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,Print_Draw,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return stream;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,Print_Draw,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(nH);
            }
        }

        /// <summary>
        /// Method to print cash drop
        /// </summary>
        /// <param name="tenders">List of tenders</param>
        /// <param name="till">Till</param>
        /// <param name="user">User</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="dropId">Drop id</param>
        /// <param name="cntDrop"> Count drop</param>
        /// <returns>Stream</returns>
        public FileStream PrintDrop(Tenders tenders, Till till, User user, byte registerNumber,
            CashDrop cashDrop, short cntDrop)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintDrop,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short nH = 0;
            string fileName = "";
            short hWidth = 0;
            decimal sAmount = new decimal();
            decimal sValue = new decimal();
            string just = "";
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            fileName = Path.GetTempPath() + "CashDrop" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            try
            {
                nH = (short)(FileSystem.FreeFile());
                hWidth = (short)40;
                sAmount = 0;
                sValue = 0;
                just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification

                FileSystem.FileOpen(nH, fileName, OpenMode.Output);
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street1), hWidth));
                    if (string.IsNullOrEmpty(store.Address.Street2))
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, Convert.ToString(store.Address.PostalCode), hWidth));
                }
                FileSystem.PrintLine(nH);
                if (_policyManager.SAFEATMDROP)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)258), hWidth)); //"CASH DROP RECEIPT"
                    if (Strings.Trim(Convert.ToString(cashDrop.ReasonCode)) == "ATM")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)1272) + "(" + _resourceManager.GetResString(offSet, (short)1204) + ":" + cashDrop.DropID + ")", hWidth));
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)1273) + "(" + _resourceManager.GetResString(offSet, (short)1204) + ":" + cashDrop.DropID + ")", hWidth));
                    }
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)258), hWidth)); //"CASH DROP RECEIPT"
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)1204) + ":" + cashDrop.DropID, hWidth));
                }
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)259) + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateTime.Now.ToString(timeFormatHm), hWidth));

                if (_policyManager.PRN_UName)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)260) + ": " + user.Code + " - " + user.Name, hWidth)); //"Dropped By
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)260) + ": " + user.Code, hWidth)); //"Dropped By
                }
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)132) + ": " + Convert.ToString(registerNumber), hWidth)); //Register
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)131) + ": " + till.Number, hWidth)); //Till
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)389) + ": " + till.ShiftDate.ToString("dd/MM/yyyy"), hWidth)); //Shiftdate
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)346) + ": " + till.Shift, hWidth)); //Shift id
                                                                                                                                            //
                if (_policyManager.DropEnv == true)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)4703) + " " + cashDrop.Envelope_No, hWidth)); //Envelope Number
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)390) + ": " + Convert.ToString(cntDrop)); //Cntdrop
                FileSystem.PrintLine(nH);

                Tender T = default(Tender);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)115), (short)20) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)106), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)114), (short)10)); //"Tender","Amount","Value"
                FileSystem.PrintLine(nH, modStringPad.PadR("_", (short)20, "_") + modStringPad.PadL("_", (short)10, "_") + modStringPad.PadL("_", (short)10, "_"));
                foreach (Tender tempLoopVarT in tenders)
                {
                    T = tempLoopVarT;
                    if (T.Amount_Entered != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(T.Tender_Name, (short)20) + modStringPad.PadL(T.Amount_Entered.ToString("#,##0.00"), (short)10) + modStringPad.PadL(T.Amount_Used.ToString("#,##0.00"), (short)10));
                        sAmount = sAmount + T.Amount_Entered;
                        sValue = sValue + T.Amount_Used;
                    }
                }
                FileSystem.PrintLine(nH, modStringPad.PadR("_", (short)20, "_") + modStringPad.PadL("_", (short)10, "_") + modStringPad.PadL("_", (short)10, "_"));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)136).ToUpper(), (short)20) + modStringPad.PadL(sAmount.ToString("#,##0.00"), (short)10) + modStringPad.PadL(sValue.ToString("#,##0.00"), (short)10)); //"TOTALS"
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.FileClose(nH);
                Chaps_Main.Last_Printed = "CashDrop.txt";
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,PrintDrop,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return stream;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,PrintDrop,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(nH);
            }
        }

        /// <summary>
        /// Method to print gift card
        /// </summary>
        /// <param name="gcPayment">Gift card payment</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint or not</param>
        /// <param name="copies">Copies</param>
        /// <param name="sameSale">Same sale or not</param>
        /// <returns>Report content</returns>
        public FileStream PrintGiftCard(GCPayment gcPayment, bool printIt = true, bool reprint = false, short copies = 1, bool sameSale = false)
        {
            var store = _policyManager.LoadStoreInfo();
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintGiftCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            int nH = 0;
            string fileName = "";
            short hWidth = 0;
            short lWidth = 0;
            string strTranType;
            short nCopies = 0;
            GCTender oGc = default(GCTender);
            string strMessage = "";
            if (gcPayment.GC_Lines.Count == 0)
            {
                return null;
            }

            if (copies > 0)
            {
                nCopies = copies;
            }
            else
            {
                nCopies = (short)1;
            }

            hWidth = (short)40; // Total Receipt Width
            lWidth = (short)20;

            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            fileName = Path.GetTempPath() + "\\" + "MillipleinReceipt" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            try
            {
                nH = FileSystem.FreeFile();
                FileSystem.FileOpen(nH, fileName, OpenMode.Output, OpenAccess.Write);

                FileSystem.PrintLine(nH, modStringPad.PadC(store.Code + "  " + store.Name, hWidth));
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(Convert.ToString(store.Address.Street1), hWidth));
                    if (!string.IsNullOrEmpty(store.Address.Street2))
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadC(Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadC(Convert.ToString(store.Address.PostalCode), hWidth));
                }
                Phone phone = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        phone = tempLoopVarPhoneRenamed;
                        if (!string.IsNullOrEmpty(phone.Number))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phone.PhoneName + " " + phone.Number, hWidth));
                        }
                    }
                }
                FileSystem.PrintLine(nH, modStringPad.PadC(Strings.Trim(Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth));

                if (!string.IsNullOrEmpty(store.SecRegName))
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(Strings.Trim(Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth));
                }
                FileSystem.PrintLine(nH, "\r\n");

                if (reprint)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC("** " + _resourceManager.GetResString(offSet, (short)135).ToUpper() + " **", hWidth)); // "          ** REPRINT **"
                                                                                                                                                      ///        Print #nH, PadC(GetResString(234) & ": " & Format(Date, "dd-MMM-yyyy") & GetResString(208) & Format(Time, "h:nn AMPM"), H_Width)        'Reprinted on
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)234) + ": " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth)); //Reprinted on   '  
                    FileSystem.PrintLine(nH);
                }

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)305) + ": " + gcPayment.GC_Lines[1].TransactionTime.ToString("dd-MMM-yyyy"), lWidth) + modStringPad.PadR(_resourceManager.GetResString(offSet, (short)197) + ": " + gcPayment.GC_Lines[1].TransactionTime.ToString(timeFormatHms), lWidth)); //Date Time
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC("RELEVE DE PAIEMENT", hWidth));
                FileSystem.PrintLine(nH, modStringPad.PadC("DE CERTIFICAT-CADEAU", hWidth));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC("FACTURE NO: " + Convert.ToString(gcPayment.Sale_Num), hWidth));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, "AUTORISATION: " + gcPayment.GC_Lines[1].RefNum);
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, "CERTIFICAT VALIDE");
                foreach (GCTender tempLoopVar_OGc in gcPayment.GC_Lines)
                {
                    oGc = tempLoopVar_OGc;
                    FileSystem.PrintLine(nH, modStringPad.PadR(oGc.CertificateNum, (short)20) + modStringPad.PadL(oGc.SaleAmount.ToString("#0.00"), (short)(hWidth - 20)));
                    if (oGc.Message != "")
                    {
                        strMessage = oGc.Message;
                    }
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR("VALEUR TOTALE: ", (short)20) + modStringPad.PadL(gcPayment.Amount.ToString("#0.00"), (short)(hWidth - 20)));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR("SIGN:" + new string('_', hWidth - 5), hWidth));
                FileSystem.PrintLine(nH);
                string[] arrMsg = null;
                short iArr = 0;
                if (!string.IsNullOrEmpty(strMessage))
                {
                    arrMsg = Strings.Split(Expression: strMessage, Delimiter: "\r\n", Compare: CompareMethod.Text);
                    for (iArr = 0; iArr <= (arrMsg.Length - 1); iArr++)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(arrMsg[iArr], hWidth));
                    }
                }
                FileSystem.PrintLine(nH);
                FileSystem.FileClose(nH);
                Chaps_Main.Last_Printed = fileName;
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,PrintGiftCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return stream;
            }
            finally
            {
                FileSystem.FileClose(nH);
            }
        }

        /// <summary>
        /// Method to issue store credit receipt
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="customerCode">Customer code</param>
        /// <param name="user">User</param>
        /// <param name="tender">Tender</param>
        /// <param name="amount">Amount</param>
        /// <param name="storeCredit">Store credit</param>
        /// <returns>Report content</returns>
        public Report Issue_Store_Credit(int saleNumber, string customerCode,
            User user, Tender tender, float amount, out Store_Credit storeCredit)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Issue_Store_Credit,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Store store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            var report = new Report
            {
                ReportName = Utilities.Constants.StoreCreditFile,
                Copies = 1
            };

            string fileName = "";
            storeCredit = new Store_Credit();
            storeCredit.Amount = (decimal)amount;
            storeCredit.SC_Date = DateTime.Now;
            storeCredit.Sale_Number = saleNumber;
            storeCredit.Customer = customerCode;
            storeCredit.EXPIRE_DAYS = _policyManager.EXPIRE_DAYS;
            storeCredit.Expires_On = DateTime.Today.AddDays(_policyManager.EXPIRE_DAYS);
            _tenderService.SaveCredit(storeCredit);
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            // Print the store credit receipt.
            short nH = 0;
            try
            {
                nH = (short)(FileSystem.FreeFile());
                fileName = Path.GetTempPath() + "\\" + "\\StoreCred" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
                FileSystem.FileOpen(nH, fileName, OpenMode.Output);
                FileSystem.PrintLine(nH, modStringPad.PadC(store.Code + "  " + store.Name, (short)40));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)173) + storeCredit.Number, (short)40)); //"store Credit #"
                                                                                                                                                ///    Print #nH, PadC(GetResString(171) & " " & Format(Now, "dd-mmm-yyyy hh:nn AMPM"), 40)     '"Issued On: "
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)171) + " " + DateAndTime.Today.ToString("dd-mmm-yyyy") + " " + DateAndTime.TimeOfDay.ToString(timeFormatHm), (short)40)); //"Issued On: "   '  
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)174) + " " + System.Convert.ToString(saleNumber), (short)40)); //"Sale Number: "
                                                                                                                                                                       // 
                                                                                                                                                                       //    Print #nH, PadC(GetResString(172) & " " & User.Name, 40)     '"Issued By  : "
                if (_policyManager.PRN_UName)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)172) + " " + user.Name, (short)40)); //"Issued By  : "
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)172) + " " + user.Code, (short)40)); //"Issued By  : "
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)175), (short)40)); //"C R E D I T   A M O U N T"
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)15) + amount.ToString("0.00"), (short)40)); //"$"
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);

                if (storeCredit.EXPIRE_DAYS > 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)176), (short)40)); //"This store credit must be"
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)177) + " " + storeCredit.EXPIRE_DAYS + _resourceManager.GetResString(offSet, (short)180), (short)40)); //redeemed within , " days"
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)178) + DateAndTime.DateAdd(Microsoft.VisualBasic.DateInterval.Day, System.Convert.ToDouble(storeCredit.EXPIRE_DAYS), DateAndTime.Today).ToString("dd-MMM-yyyy"), (short)40)); //"It expires on "
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)179), (short)40)); //"This store credit does not expire"
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)181), (short)40)); //"It is valid until redeemed"
                }
                FileSystem.FileClose(nH);
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,Issue_Store_Credit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                FileSystem.FileClose(nH);
                FileSystem.Kill(fileName);
            }
        }


        /// <summary>
        /// Method to print givex receipt
        /// </summary>
        /// <param name="gxReceipt">Givex receipt</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="copies">copies</param>
        /// <param name="sameSale">Same sale or not</param>
        /// <returns>Report content</returns>
        public Report Print_GiveX_Receipt(GiveXReceiptType gxReceipt, bool printIt = true, bool reprint = false,
            short copies = 1,
            bool sameSale = false)
        {
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            int nH = 0;
            string fileName = "";
            short hWidth = 0;
            short lWidth = 0;
            string strTranType = "";

            hWidth = (short)40; // Total Receipt Width
            lWidth = (short)20;
            var report = new Report
            {
                ReportName = Utilities.Constants.GivexFile,
                Copies = copies
            };
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            fileName = Path.GetTempPath() + "\\" + "GivexReceipt" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            try
            {

                nH = FileSystem.FreeFile();
                FileSystem.FileOpen(nH, fileName, OpenMode.Output, OpenAccess.Write);
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC((_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(System.Convert.ToString(store.Address.Street1), hWidth));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(System.Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadC(Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadC(System.Convert.ToString(store.Address.PostalCode), hWidth));
                }
                Phone phoneRenamed = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        phoneRenamed = tempLoopVarPhoneRenamed;
                        if (phoneRenamed.Number.Trim() != "")
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phoneRenamed.PhoneName + " " + phoneRenamed.Number, hWidth));
                        }
                    }
                }
                FileSystem.PrintLine(nH, modStringPad.PadC(Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth)); //& vbCrLf & vbCrLf

                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(Strings.Trim(System.Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth));
                }
                FileSystem.PrintLine(nH, "\r\n");

                if (reprint)
                {
                    FileSystem.PrintLine(nH, "          ** REPRINT **");
                    FileSystem.PrintLine(nH, modStringPad.PadC("Reprint At: " + DateAndTime.Today.ToString("mmm-dd-yy") + "  " + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth)); //  
                    FileSystem.PrintLine(nH);
                }

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)305) + ": " + string.Format(gxReceipt.Date, "MM/dd/yyyy"), lWidth) + modStringPad.PadR(_resourceManager.GetResString(offSet, (short)197) + ": " + string.Format(gxReceipt.Time, timeFormatHms), lWidth));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3278) + ": " + gxReceipt.UserID, (short)20) + modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3279) + ": ", (short)20));
                FileSystem.PrintLine(nH);
                switch (gxReceipt.TranType)
                {
                    case (short)1:
                        strTranType = _resourceManager.GetResString(offSet, (short)3206); //Activation
                        break;
                    case (short)2:
                        strTranType = _resourceManager.GetResString(offSet, (short)102); //Cancel
                        break;
                    case (short)3:
                        strTranType = _resourceManager.GetResString(offSet, (short)3209); //Adjustment
                        break;
                    case (short)4:
                        strTranType = _resourceManager.GetResString(offSet, (short)3281); //BalanceInquiry
                        break;
                    case (short)5:
                        strTranType = _resourceManager.GetResString(offSet, (short)3208); //Increment
                        break;
                    case (short)6:
                        strTranType = _resourceManager.GetResString(offSet, (short)3214); //Redemption
                        break;
                    default:
                        strTranType = "";
                        break;
                }
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3280) + ": " + strTranType, hWidth));
                FileSystem.PrintLine(nH);
                if (gxReceipt.SaleNum != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3282) + ": " + System.Convert.ToString(gxReceipt.SaleNum), hWidth));
                }
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3283) + ": " + gxReceipt.SeqNum, hWidth));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3284) + ": " + GetGiveXMaskNum(gxReceipt.CardNum), hWidth));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3277) + gxReceipt.ExpDate, hWidth));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3285) + ": " + gxReceipt.Balance.ToString("#0.00"), hWidth));
                if (gxReceipt.PointBalance != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3286) + ": " + System.Convert.ToString(gxReceipt.PointBalance), hWidth));
                }
                if (gxReceipt.SaleAmount != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(strTranType + " " + _resourceManager.GetResString(offSet, (short)106) + ": " + gxReceipt.SaleAmount.ToString("#0.00"), hWidth));
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3287), hWidth));
                FileSystem.PrintLine(nH, modStringPad.PadC(gxReceipt.ResponseCode.Trim(), hWidth));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3288) + "!", hWidth));
                FileSystem.FileClose(nH);
                Chaps_Main.Last_Printed = fileName;
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                FileSystem.FileClose(nH);
                FileSystem.Kill(fileName);
            }
        }

        /// <summary>
        /// Method to print AR pay
        /// </summary>
        /// <param name="arPay">AR payment</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleTime">Sale time</param>
        /// <param name="reprintCards">Reprint cards</param>
        /// <returns>Report content</returns>
        public Report Print_ARPay(AR_Payment arPay, string userCode, Till till, Tenders tenders = null,
            bool printIt = false, bool reprint = false, DateTime saleDate = default(DateTime),
            DateTime saleTime = default(DateTime), Reprint_Cards reprintCards = null)
        {
            int nH = 0;
            Tender tenderRenamed = default(Tender);
            decimal sumTender = new decimal();
            short n = 0;
            short aWidth = 0;
            string pad = "";
            string fileName = "";
            bool sigLine = false;
            var report = new Report
            {
                ReportName = Utilities.Constants.ArPayFile,
                Copies = _policyManager.ArpayReceiptCopies
            };
            aWidth = (short)10; // Amount Width
            short hWidth = (short)40;
            pad = Strings.Left(System.Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification
            var user = _loginManager.GetExistingUser(userCode);
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            sumTender = 0;
            try
            {
                nH = FileSystem.FreeFile();
                fileName = Path.GetTempPath() + "\\ARPay" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";

                var timeFormatHm = string.Empty;
                var timeFormatHms = string.Empty;
                GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
                FileSystem.FileOpen(nH, fileName, OpenMode.Output, OpenAccess.Write);
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") +
                        store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, System.Convert.ToString(store.Address.Street1), hWidth));
                    if (!string.IsNullOrEmpty(store.Address.Street2))
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(pad, System.Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, Strings.Trim(System.Convert.ToString(store.Address.City))
                        + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(pad, System.Convert.ToString(store.Address.PostalCode), hWidth));
                }
                Phone phoneRenamed = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        phoneRenamed = tempLoopVarPhoneRenamed;
                        if (!string.IsNullOrEmpty(phoneRenamed.Number))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phoneRenamed.PhoneName + " "
                                + phoneRenamed.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(pad, Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth));
                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, Strings.Trim(System.Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth) + "\r\n");
                }
                else
                {
                    FileSystem.PrintLine(nH);
                }
                FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)8302) + " # " + System.Convert.ToString(arPay.Sale_Num), hWidth));
                if (reprint)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Name, hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, saleDate.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + saleTime.ToString(timeFormatHm), hWidth) + "\r\n"); //  
                                                                                                                                                                                                                ///        Print #nH, PadIt(Pad, GetResString(234) & ": " & Format(Date, "dd-MMM-yyyy") & GetResString(208) & Format(Time, "h:nn AMPM"), H_Width) & vbCrLf         'Reprinted on
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)234) + ": " + DateAndTime.Today.ToString("dd-MMM-yyyy") +
                        _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth)
                        + "\r\n");
                }
                else
                {
                    if (_policyManager.PRN_UName)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Name, hWidth)); //"Cashier"
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Code, hWidth)); //"Cashier"
                    }

                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, DateAndTime.Today.ToString("dd-MMM-yyyy") + " "
                        + _resourceManager.GetResString(offSet, (short)208) + " " + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //  
                }
                FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)110) + " " + arPay.Customer.Code + " - " + arPay.Customer.Name, hWidth));

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)249), (short)(hWidth - aWidth)) + modStringPad.PadL("$" + arPay.Amount.ToString("0.00"), aWidth));
                if (arPay.Penny_Adj != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)486), (short)(hWidth - aWidth)) + modStringPad.PadL(arPay.Penny_Adj.ToString("###,##0.00"), aWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadL(new string('_', 10), hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)210), (short)(hWidth - aWidth)) + modStringPad.PadL((arPay.Amount + arPay.Penny_Adj).ToString("###,##0.00"), aWidth));
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)252) + ":");
                // Print the tenders
                FileSystem.PrintLine(nH, " ");
                sumTender = 0;
                if (tenders == null)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadC("=", hWidth, "="));
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)240), hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadC("=", hWidth, "="));
                    FileSystem.PrintLine(nH);
                }
                else
                {
                    foreach (Tender tempLoopVarTenderRenamed in tenders)
                    {
                        tenderRenamed = tempLoopVarTenderRenamed;
                        if (tenderRenamed.Amount_Used != 0)
                        {
                            if (tenderRenamed.Amount_Entered != tenderRenamed.Amount_Used)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(tenderRenamed.Tender_Name + "  (" + tenderRenamed.Amount_Entered.ToString("###,##0.00") + ")", (short)(hWidth - aWidth)) + modStringPad.PadL(tenderRenamed.Amount_Used.ToString("###,##0.00"), aWidth));
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(tenderRenamed.Tender_Name, (short)(hWidth - aWidth)) + modStringPad.PadL(tenderRenamed.Amount_Used.ToString("###,##0.00"), aWidth));
                            }
                            sumTender = sumTender + tenderRenamed.Amount_Used;
                        }
                    }

                    FileSystem.PrintLine(nH, modStringPad.PadR(" ", (short)(hWidth - aWidth)) + modStringPad.PadR("_", aWidth, "_"));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)241), (short)(hWidth - aWidth)) + modStringPad.PadL(sumTender.ToString("###,##0.00"), aWidth));
                    // Print the change.
                    if (Math.Round((double)tenders.Tend_Totals.Change, 2) < 0.0D)
                    {
                        FileSystem.PrintLine(nH, " ");
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)220) + " ", (short)(hWidth - aWidth)) + modStringPad.PadL(System.Math.Abs(tenders.Tend_Totals.Change).ToString("###,##0.00"), aWidth));

                    }
                }

                FileSystem.PrintLine(nH);
                if (_policyManager.ShowAccBal)
                {
                    if (reprint)
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8303), (short)30) +
                            modStringPad.PadL(((decimal)arPay.Customer.Current_Balance - arPay.Amount).ToString("$#,##0.00"),
                            (short)10));
                    else
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8303), (short)30) +
                        modStringPad.PadL(((decimal)arPay.Customer.Current_Balance).ToString("$#,##0.00"),
                       (short)10));
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                sigLine = false;
                bool maskCard = false;
                maskCard = System.Convert.ToBoolean(_policyManager.MASK_CARDNO);
                if (reprint)
                {
                    RePrint_Credit_Card(till, reprintCards, System.Convert.ToBoolean(_policyManager.MASK_CARDNO), (short)nH);
                    FileSystem.PrintLine(nH);
                }
                else
                {
                    if (tenders != null)
                    {
                        foreach (Tender tempLoopVarTenderRenamed in tenders)
                        {
                            tenderRenamed = tempLoopVarTenderRenamed;
                            if (!string.IsNullOrEmpty(tenderRenamed.Credit_Card.Cardnumber))
                            {
                                Print_Credit_Card(tenderRenamed.Credit_Card, maskCard, (short)nH);
                            }
                            if (tenderRenamed.Amount_Used != 0 && tenderRenamed.SignatureLine)
                            {
                                sigLine = true;
                            }
                        }
                    }

                    if (sigLine)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH);
                        //        Print #nH, PadR("Signature:  ", H_Width, "_")
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)247) + ":  ", hWidth, "_"));
                        FileSystem.PrintLine(nH);
                    }

                }
                for (n = 1; n <= _policyManager.ADV_LINES; n++)
                {
                    FileSystem.PrintLine(nH);
                }

                FileSystem.FileClose();

                Chaps_Main.Last_Printed = "ARPay.txt";
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                FileSystem.FileClose();
                FileSystem.Kill(fileName);
            }
        }


        /// <summary>
        /// Method to print transaction record in french
        /// </summary>
        /// <param name="tc">Credit card</param>
        /// <param name="maskCard">Mask card number</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="merchantCopy">Merchant copy or not</param>
        /// <returns>Report content</returns>
        public Report PrintTransRecordEnglish(Credit_Card tc, bool maskCard, bool printIt, bool reprint, bool merchantCopy)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintTransRecordFrench,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            var report = new Report
            {
                ReportName = Utilities.Constants.EnglishCopy,
                Copies = 1
            };
            string vstr = "";
            short fileNumber = 0;
            string fileName = "";
            string ccl;
            string ccr;
            short ccc;
            try
            {
                var store = _policyManager.LoadStoreInfo();
                if (_policyManager.EMVVersion && tc.Report.IndexOf("<END-MERCHANT>") + 1 <= 0)
                {
                    return null;
                }

                fileNumber = (short)(FileSystem.FreeFile());
                fileName = Path.GetTempPath() + "\\" + "TEnglish" +
                           string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";

                FileSystem.FileOpen(fileNumber, fileName, OpenMode.Output);
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(fileNumber, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name);
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(fileNumber, store.Address.Street1);
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(fileNumber, store.Address.Street2);
                    }
                    FileSystem.PrintLine(fileNumber,
                        Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState +
                        "\r\n" + store.Address.PostalCode);
                }
                FileSystem.PrintLine(fileNumber);
                if (reprint)
                {
                    FileSystem.PrintLine(fileNumber, "          ** REPRINT **");
                }
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber,
                        (DateAndTime.Today.ToString("MMM-dd-yy") + "   " +
                         DateAndTime.TimeOfDay.ToString(timeFormatHm) + Strings.Space(25)).Substring(0, 25) +
                        (Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Substring(
                            (Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Length - 15, 15));
                if (!_policyManager.EMVVersion)
                {
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, "           TRANSACTION RECORD ");
                }
                if (_policyManager.EMVVersion && !tc.Card_Swiped)
                {
                    if (merchantCopy)
                    {
                        if (tc.Report.IndexOf("<END-MERCHANT>") + 1 > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                    tc.Report.Substring(0, tc.Report.IndexOf("<END-MERCHANT>") + 0));
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, tc.Report);
                        }
                    }
                    else
                    {
                        if (tc.Report.IndexOf("<END-MERCHANT>") + 1 > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                tc.Report.Substring(
                                    tc.Report.Length -
                                    ((tc.Report.Length - (tc.Report.IndexOf("<END-MERCHANT>") + 1)) - 13),
                                    (tc.Report.Length - (tc.Report.IndexOf("<END-MERCHANT>") + 1)) - 13));
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, tc.Report);
                        }
                    }
                }
                else
                {
                    if (tc.Crd_Type == "D")
                    {
                        FileSystem.PrintLine(fileNumber, "         INTERAC DIRECT PAYMENT");
                    }
                    FileSystem.PrintLine(fileNumber);
                    if (maskCard)
                    {
                        FileSystem.PrintLine(fileNumber,
                               ("Card Number: " + Strings.Space(20)).Substring(0, 20) +
                               (Strings.Space(20) + GetMaskedCardNum(tc.Cardnumber ?? string.Empty, tc.Crd_Type)).Substring(
                                   (Strings.Space(20) + GetMaskedCardNum(tc.Cardnumber ?? string.Empty, tc.Crd_Type)).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Card Number: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Cardnumber ?? string.Empty).Substring(
                                (Strings.Space(20) + tc.Cardnumber ?? string.Empty).Length - 20, 20));
                    }
                    if (tc.Crd_Type == "C")
                    {
                        if (_policyManager.BankSystem != "Moneris")
                        {
                            FileSystem.PrintLine(fileNumber,
                                ("Exp:         " + Strings.Space(20)).Substring(0, 20) +
                                (Strings.Space(20) + "**/**").Substring((Strings.Space(20) + "**/**").Length - 20, 20));
                        }
                    }
                    FileSystem.PrintLine(fileNumber, tc.Customer_Name);
                    if (tc.StoreAndForward && _policyManager.BankSystem == "Moneris")
                    {
                        vstr = "KEYED";
                        tc.Card_Swiped = false;
                    }
                    else
                    {
                        if (tc.Card_Swiped)
                        {
                            vstr = "SWIPED";
                        }
                        else
                        {
                            vstr = "KEYED";
                        }
                    }
                    FileSystem.PrintLine(fileNumber,
                        ("Card Entry: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                    if (tc.Crd_Type == "D")
                    {
                        vstr = "DEBIT";
                        FileSystem.PrintLine(fileNumber,
                            ("Account: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                        FileSystem.PrintLine(fileNumber,
                            ("Account Type: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.DebitAccount.ToUpper()).Substring(
                                (Strings.Space(20) + tc.DebitAccount.ToUpper()).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Account: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Name.ToUpper()).Substring(
                                (Strings.Space(20) + tc.Name.ToUpper()).Length - 20, 20));
                    }
                    if (!_policyManager.USE_PINPAD)
                    {
                        if (tc.Print_VechicleNo)
                        {
                            if (tc.Vechicle_Number.Trim().Length > 0)
                            {
                                FileSystem.PrintLine(fileNumber,
                                    ("VEHICLE: " + Strings.Space(20)).Substring(0, 20) +
                                    (Strings.Space(20) + tc.Vechicle_Number.Trim()).Substring(
                                        (Strings.Space(20) + tc.Vechicle_Number.Trim()).Length - 20, 20));
                            }
                        }
                        if (tc.Print_DriverNo)
                        {
                            if (tc.Driver_Number.Trim().Length > 0)
                            {
                                FileSystem.PrintLine(fileNumber,
                                    ("DRIVER #: " + Strings.Space(20)).Substring(0, 20) +
                                    (Strings.Space(20) + tc.Driver_Number.Trim()).Substring(
                                        (Strings.Space(20) + tc.Driver_Number.Trim()).Length - 20, 20));
                            }
                        }
                        if (tc.Print_IdentificationNo)
                        {
                            if (tc.ID_Number.Trim().Length > 0)
                            {
                                FileSystem.PrintLine(fileNumber,
                                    ("IDENTIFICATION #: " + Strings.Space(20)).Substring(0, 20) +
                                    (Strings.Space(20) + tc.ID_Number.Trim()).Substring(
                                        (Strings.Space(20) + tc.ID_Number.Trim()).Length - 20, 20));
                            }
                        }
                    }
                    if (tc.Print_Odometer)
                    {
                        if (tc.Odometer_Number.Trim().Length > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                ("ODOMETER : " + Strings.Space(20)).Substring(0, 20) +
                                (Strings.Space(20) + tc.Odometer_Number.Trim()).Substring(
                                    (Strings.Space(20) + tc.Odometer_Number.Trim()).Length - 20, 20));
                        }
                    }
                    if (tc.Print_Usage)
                    {
                        if (tc.usageType.Trim().Length > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                ("CARD USAGE: " + Strings.Space(20)).Substring(0, 20) +
                                (Strings.Space(20) + tc.usageType.Trim()).Substring(
                                    (Strings.Space(20) + tc.usageType.Trim()).Length - 20, 20));
                        }
                    }

                    if (_policyManager.BankSystem == "Moneris" && !string.IsNullOrEmpty(tc.Trans_Type))
                    {
                        if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                        {
                            vstr = "REFUND";
                        }
                        else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                        {
                            if (tc.Trans_Amount < 0)
                            {
                                vstr = "PURCHASE CORRECTION";
                            }
                            else
                            {
                                vstr = "REFUND CORRECTION";
                            }
                        }
                        else
                        {

                            if (tc.Trans_Type.ToUpper() == "SAFSALEINSIDE" &&
                                System.Math.Abs((short)tc.Trans_Amount) > tc.FloorLimit)
                            {
                                vstr = "PRE-AUTH ADVICE";
                            }
                            else
                            {
                                vstr = "PURCHASE";
                            }
                        }
                    }
                    else
                    {
                        if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                        {
                            vstr = "RETURN";
                        }
                        else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                        {
                            if (tc.Trans_Amount < 0)
                            {
                                vstr = "PURCHASE CORRECTION";
                            }
                            else
                            {
                                vstr = "RETURN CORRECTION";
                            }
                        }
                        else
                        {
                            vstr = "PURCHASE";
                        }
                    }
                    FileSystem.PrintLine(fileNumber,
                        ("Trans Type: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("Amount: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Substring(
                            (Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Length -
                            20, 20));

                    if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                    {
                        if (tc.Void_Num > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                ("Reference #: " + Strings.Space(20)).Substring(0, 20) +
                                (Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Substring(
                                    (Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Length - 20, 20));
                        }
                    }

                    FileSystem.PrintLine(fileNumber,
                        ("Response Code : " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Substring(
                            (Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("Auth #: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.Authorization_Number).Substring(
                            (Strings.Space(20) + tc.Authorization_Number).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("Sequence #: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.Sequence_Number).Substring(
                            (Strings.Space(20) + tc.Sequence_Number).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("Terminal #: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.TerminalID).Substring((Strings.Space(20) + tc.TerminalID).Length - 20,
                            20));


                    if (!Information.IsDBNull(tc.Trans_Date))
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Date: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Substring(
                                (Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Date: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Substring(
                                (Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Length - 20, 20));
                    }
                    if (tc.Trans_Time == DateTime.Parse("00:00:00"))
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Time: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Substring(
                                (Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Time: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Substring(
                                (Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Length - 20, 20));
                    }
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, tc.Receipt_Display);
                    FileSystem.PrintLine(fileNumber);
                    if (tc.Print_Signature && tc.Crd_Type != "D" && tc.Trans_Type.ToUpper() != "REFUNDINSIDE" &&
                        tc.Trans_Type.ToUpper() != "SAFREFUNDINSIDE" && tc.Trans_Type.ToUpper() != "VOIDINSIDE" &&
                        tc.Trans_Type.ToUpper() != "SAFVOIDINSIDE")
                    {
                        if (_policyManager.BankSystem == "Moneris")
                        {
                            if (_creditCardManager.Language(ref tc).Substring(0, 1).ToUpper() == "F")
                            {
                                FileSystem.PrintLine(fileNumber, "    Le Titulaire versera ce montant a  ");
                                FileSystem.PrintLine(fileNumber, "        L\'emetteur conformement au    ");
                                FileSystem.PrintLine(fileNumber, "             contrat adherent  ");
                            }
                            else
                            {
                                FileSystem.PrintLine(fileNumber, " Cardholder will pay card issuer above ");
                                FileSystem.PrintLine(fileNumber, "amount pursuant to Cardholder Agreement");
                            }
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, "CUSTOMER AGREES TO PAY THE ABOVE AMOUNT");
                            FileSystem.PrintLine(fileNumber, " ACCORDING TO THE CARD ISSUER AGREEMENT");
                        }
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "Signature");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "_________________________________________");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                    }
                    else
                    {
                        if ((tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE") &&
                            tc.Trans_Amount > 0 && _policyManager.BankSystem == "Moneris" && tc.Crd_Type != "D")
                        {
                            FileSystem.PrintLine(fileNumber, " Cardholder will pay card issuer above ");
                            FileSystem.PrintLine(fileNumber, "amount pursuant to Cardholder Agreement");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, "Signature");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, "_________________________________________");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, ",");
                        }
                    }
                }
                FileSystem.FileClose();
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(fileNumber);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,PrintTransRecordEnglish,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(fileNumber);
                FileSystem.Kill(fileName);
            }

        }

        /// <summary>
        /// Method to print transaction record in english
        /// </summary>
        /// <param name="tc">Credit card</param>
        /// <param name="maskCard">Mask card number</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="merchantCopy">Merchant copy or not</param>
        /// <returns>Report content</returns>
        public Report PrintTransRecordFrench(Credit_Card tc, bool maskCard, bool printIt, bool reprint, bool merchantCopy)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintTransRecordFrench,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string vstr = "";
            short fileNumber = 0;
            string fileName = "";
            string ccl;
            string ccr;
            short ccc;
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            var report = new Report
            {
                ReportName = Utilities.Constants.FrenchCopy,
                Copies = 1
            };
            try
            {
                var store = _policyManager.LoadStoreInfo();
                //In emv version need to print only if message came from stps
                if (_policyManager.EMVVersion && tc.Report.IndexOf("<END-MERCHANT>") + 1 <= 0)
                {
                    return null;
                }

                fileNumber = (short)(FileSystem.FreeFile());
                fileName = Path.GetTempPath() + "\\" + "TFrench" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";

                FileSystem.FileOpen(fileNumber, fileName, OpenMode.Output);
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(fileNumber,
                        (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name);
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(fileNumber, store.Address.Street1);
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(fileNumber, store.Address.Street2);
                    }
                    FileSystem.PrintLine(fileNumber,
                        Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState +
                        "\r\n" + store.Address.PostalCode);
                }
                FileSystem.PrintLine(fileNumber);
                if (reprint)
                {
                    FileSystem.PrintLine(fileNumber, "          ** RÉIMPRESSION **");
                }
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber,
                        (DateAndTime.Today.ToString("MMM-dd-yy") + "   " +
                         DateAndTime.TimeOfDay.ToString(timeFormatHm) + Strings.Space(25)).Substring(0, 25) +
                        (Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Substring(
                            (Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Length - 15, 15));
                FileSystem.PrintLine(fileNumber,
                        (DateAndTime.Today.ToString("yyyy-MM-dd") + "   " +
                         DateAndTime.TimeOfDay.ToString(timeFormatHm) + Strings.Space(25)).Substring(0, 25) +
                        (Strings.Space(10) + "Trans# " + tc.Trans_Number.Trim()).Substring(
                            (Strings.Space(10) + "Trans# " + tc.Trans_Number.Trim()).Length - 20, 20));
                if (!_policyManager.EMVVersion)
                {
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, "           RELEVE DE TRANSACTION");
                }
                if (_policyManager.EMVVersion && !tc.Card_Swiped)
                {
                    if (merchantCopy)
                    {
                        if (tc.Report.IndexOf("<END-MERCHANT>") + 1 > 0)
                        {
                            FileSystem.PrintLine(fileNumber, tc.Report.Substring(0, tc.Report.IndexOf("<END-MERCHANT>") + 0));
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, tc.Report);
                        }
                    }
                    else
                    {
                        if (tc.Report.IndexOf("<END-MERCHANT>") + 1 > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                tc.Report.Substring(
                                    tc.Report.Length - ((tc.Report.Length - (tc.Report.IndexOf("<END-MERCHANT>") + 1)) - 13),
                                    (tc.Report.Length - (tc.Report.IndexOf("<END-MERCHANT>") + 1)) - 13));
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, tc.Report);
                        }
                    }
                }
                else
                {
                    if (tc.Crd_Type == "D")
                    {
                        FileSystem.PrintLine(fileNumber, "           INTERAC DIRECT PAYMENT");
                    }
                    FileSystem.PrintLine(fileNumber);
                    if (maskCard)
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Numero Carte: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + GetMaskedCardNum(tc.Cardnumber, tc.Crd_Type)).Substring(
                                (Strings.Space(20) + GetMaskedCardNum(tc.Cardnumber, tc.Crd_Type)).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Numero Carte: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Cardnumber).Substring((Strings.Space(20) + tc.Cardnumber).Length - 20,
                                20));
                    }
                    if (tc.Crd_Type == "C")
                    {
                        if (_policyManager.BankSystem != "Moneris")
                        {
                            FileSystem.PrintLine(fileNumber,
                                ("Exp:         " + Strings.Space(20)).Substring(0, 20) +
                                (Strings.Space(20) + "**/**").Substring((Strings.Space(20) + "**/**").Length - 20, 20));
                        }
                    }
                    FileSystem.PrintLine(fileNumber, tc.Customer_Name);
                    if (tc.StoreAndForward && _policyManager.BankSystem == "Moneris")
                    {
                        vstr = "Manuel"; //"M"
                        tc.Card_Swiped = false;
                    }
                    else
                    {
                        if (tc.Card_Swiped)
                        {
                            vstr = "GLISSER";
                        }
                        else
                        {
                            vstr = "Manuel"; //"M"
                        }

                    }
                    FileSystem.PrintLine(fileNumber,
                        ("Mode Entree: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));

                    if (tc.Crd_Type == "D")
                    {
                        vstr = "DEBIT";
                        FileSystem.PrintLine(fileNumber,
                            ("Type de carte: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                        FileSystem.PrintLine(fileNumber,
                            ("Type Compte: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.DebitAccount.ToUpper()).Substring(
                                (Strings.Space(20) + tc.DebitAccount.ToUpper()).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Type de carte: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Name.ToUpper()).Substring(
                                (Strings.Space(20) + tc.Name.ToUpper()).Length - 20, 20));
                    }
                    if (!_policyManager.USE_PINPAD)
                    {
                        if (tc.Print_VechicleNo)
                        {
                            if (tc.Vechicle_Number.Trim().Length > 0)
                            {
                                FileSystem.PrintLine(fileNumber,
                                    ("VÉHICULE: " + Strings.Space(20)).Substring(0, 20) +
                                    (Strings.Space(20) + tc.Vechicle_Number.Trim()).Substring(
                                        (Strings.Space(20) + tc.Vechicle_Number.Trim()).Length - 20, 20));
                            }
                        }
                        if (tc.Print_DriverNo)
                        {
                            if (tc.Driver_Number.Trim().Length > 0)
                            {
                                FileSystem.PrintLine(fileNumber,
                                    ("GESTIONNAIRE #: " + Strings.Space(20)).Substring(0, 20) +
                                    (Strings.Space(20) + tc.Driver_Number.Trim()).Substring(
                                        (Strings.Space(20) + tc.Driver_Number.Trim()).Length - 20, 20));
                            }
                        }
                        if (tc.Print_IdentificationNo)
                        {
                            if (tc.ID_Number.Trim().Length > 0)
                            {
                                FileSystem.PrintLine(fileNumber,
                                    ("IDENTIFICATION #: " + Strings.Space(20)).Substring(0, 20) +
                                    (Strings.Space(20) + tc.ID_Number.Trim()).Substring(
                                        (Strings.Space(20) + tc.ID_Number.Trim()).Length - 20, 20));
                            }
                        }
                    }
                    if (tc.Print_Odometer)
                    {
                        if (tc.Odometer_Number.Trim().Length > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                ("ODOMÈTRE : " + Strings.Space(20)).Substring(0, 20) +
                                (Strings.Space(20) + tc.Odometer_Number.Trim()).Substring(
                                    (Strings.Space(20) + tc.Odometer_Number.Trim()).Length - 20, 20));
                        }
                    }
                    if (tc.Print_Usage)
                    {
                        if (tc.usageType.Trim().Length > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                ("UTILISATION DE CARTE : " + Strings.Space(20)).Substring(0, 20) +
                                (Strings.Space(20) + tc.usageType.Trim()).Substring(
                                    (Strings.Space(20) + tc.usageType.Trim()).Length - 20, 20));
                        }
                    }

                    if (_policyManager.BankSystem == "Moneris")
                    {
                        if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                        {
                            vstr = "REMISE D\'ACHAT";
                        }
                        else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                        {
                            if (tc.Trans_Amount < 0)
                            {
                                vstr = "CORRECTION D\'ACHAT";
                            }
                            else
                            {
                                vstr = "CORRECTION DE REMISE";
                            }
                        }
                        else
                        {
                            if (tc.Trans_Type.ToUpper() == "SAFSALEINSIDE" &&
                                System.Math.Abs((short)tc.Trans_Amount) > tc.FloorLimit)
                            {
                                vstr = "AVIS D\'ACHAT";
                            }
                            else
                            {
                                vstr = "ACHAT";
                            }
                        }
                    }
                    else
                    {
                        if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                        {
                            vstr = "RETOUR";
                        }
                        else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                        {
                            if (tc.Trans_Amount < 0)
                            {
                                vstr = "CORRECTION D\'ACHAT";
                            }
                            else
                            {
                                vstr = "RETOUR ANNULE";
                            }
                        }
                        else
                        {
                            vstr = "ACHAT";
                        }

                    }

                    FileSystem.PrintLine(fileNumber,
                            Strings.Space(20).Substring(0, 20) +
                            (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("Montant: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Substring(
                            (Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Length - 20,
                            20));

                    if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                    {
                        if (tc.Void_Num > 0)
                        {
                            FileSystem.PrintLine(fileNumber,
                                    ("# Reference: " + Strings.Space(20)).Substring(0, 20) +
                                    (Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Substring(
                                        (Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Length - 20, 20));
                        }
                    }

                    FileSystem.PrintLine(fileNumber,
                        ("Code de Reponse : " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Substring(
                            (Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("# Autor: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.Authorization_Number).Substring(
                            (Strings.Space(20) + tc.Authorization_Number).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("# Seq: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.Sequence_Number).Substring(
                            (Strings.Space(20) + tc.Sequence_Number).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber,
                        ("# Term: " + Strings.Space(20)).Substring(0, 20) +
                        (Strings.Space(20) + tc.TerminalID).Substring((Strings.Space(20) + tc.TerminalID).Length - 20, 20));
                    if (!Information.IsDBNull(tc.Trans_Date))
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Date: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Substring(
                                (Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Date: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Substring(
                                (Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Length - 20, 20));
                    }
                    if (tc.Trans_Time == DateTime.Parse("00:00:00"))
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Heure: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Substring(
                                (Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber,
                            ("Heure: " + Strings.Space(20)).Substring(0, 20) +
                            (Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Substring(
                                (Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Length - 20, 20));
                    }
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, tc.Receipt_Display);
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);

                    if (tc.Print_Signature && tc.Crd_Type != "D" && tc.Trans_Type.ToUpper() != "REFUNDINSIDE" &&
                       tc.Trans_Type.ToUpper() != "SAFREFUNDINSIDE" && tc.Trans_Type.ToUpper() != "VOIDINSIDE" &&
                       tc.Trans_Type.ToUpper() != "SAFVOIDINSIDE")
                    {
                        FileSystem.PrintLine(fileNumber, "    Le Titulaire versera ce montant a  ");
                        FileSystem.PrintLine(fileNumber, "        L\'emetteur conformement au    ");
                        FileSystem.PrintLine(fileNumber, "             contrat adherent  ");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "Signature");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "_________________________________________");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                    }
                    else
                    {
                        if ((tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE") &&
                            tc.Trans_Amount > 0 && _policyManager.BankSystem == "Moneris" && tc.Crd_Type != "D")
                        {
                            FileSystem.PrintLine(fileNumber, "    Le Titulaire versera ce montant a  ");
                            FileSystem.PrintLine(fileNumber, "        L\'emetteur conformement au    ");
                            FileSystem.PrintLine(fileNumber, "             contrat adherent  ");

                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, "Signature");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, "_________________________________________");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, ",");
                        }
                    }
                } // Only Endif -EMVVERSION
                FileSystem.FileClose(fileNumber);
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(fileNumber);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,PrintTransRecordFrench,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(fileNumber);
                FileSystem.Kill(fileName);
            }
        }

        /// <summary>
        /// Method to print prepay sales
        /// </summary>
        /// <param name="prepay"></param>
        /// <param name="user"></param>
        /// <param name="storeCredit"></param>
        /// <param name="prepayTenders"></param>
        /// <returns></returns>
        public FileStream PrintPrepay(Prepay prepay, User user, Store_Credit storeCredit, Tenders prepayTenders)
        {
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short nH = 0;
            string fileName = "";
            short hWidth = 0;
            decimal sumTender = new decimal();
            string just = "";
            Tender T = default(Tender);
            short aWidth = 0;
            short n = 0;
            bool sigLine = false;

            aWidth = (short)10;
            hWidth = (short)40;
            int hWidthSubAWidth = hWidth - aWidth;
            just = Strings.Left(System.Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper();
            try
            {
                var timeFormatHm = string.Empty;
                var timeFormatHms = string.Empty;
                GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
                fileName = Path.GetTempPath() + "\\" + "Prepay" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
                nH = (short)(FileSystem.FreeFile());
                sumTender = 0;

                FileSystem.FileOpen(nH, fileName, OpenMode.Output);

                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, System.Convert.ToString(store.Address.Street1), hWidth));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, System.Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, System.Convert.ToString(store.Address.PostalCode), hWidth));
                }
                Phone phone = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhone in store.Address.Phones)
                    {
                        phone = tempLoopVarPhone;
                        if (!string.IsNullOrEmpty(phone.Number))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phone.PhoneName + " " + phone.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth)); //& vbCrLf

                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth));
                }
                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)277).ToUpper() + System.Convert.ToString(prepay.SaleNo), hWidth));

                if (!string.IsNullOrEmpty(prepay.Customer.Name) && prepay.Customer.Name != _resourceManager.GetResString(offSet, (short)400))
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)110) + ": " + prepay.Customer.Code + " - " + prepay.Customer.Name.Trim(), hWidth));
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)110) + ": " + _resourceManager.GetResString(offSet, (short)400), hWidth));
                }

                if (_policyManager.PRN_UName)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Name, hWidth));
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Code, hWidth));
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth)); //  

                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH, modStringPad.PadC("=", hWidth, "="));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)281) + " " + System.Convert.ToString(prepay.Pump) + ": " + _resourceManager.GetResString(offSet, (short)106), (short)hWidthSubAWidth) + modStringPad.PadL(prepay.PrepAmount.ToString("#,##0.00"), aWidth));
                FileSystem.PrintLine(nH, modStringPad.PadC("=", hWidth, "="));
                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)126), (short)hWidthSubAWidth));
                FileSystem.PrintLine(nH);
                foreach (Tender tempLoopVarT in prepayTenders)
                {
                    T = tempLoopVarT;
                    if (T.Amount_Entered != 0)
                    {
                        if (T.Amount_Entered != T.Amount_Used)
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(T.Tender_Name + "  (" + T.Amount_Entered.ToString("###,##0.00") + ")", (short)hWidthSubAWidth) + modStringPad.PadL(T.Amount_Used.ToString("###,##0.00"), aWidth));
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(T.Tender_Name, (short)hWidthSubAWidth) + modStringPad.PadL(T.Amount_Used.ToString("###,##0.00"), aWidth));
                        }
                        sumTender = sumTender + T.Amount_Used;
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadR(" ", (short)hWidthSubAWidth) + modStringPad.PadR("_", aWidth, "_"));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)241), (short)hWidthSubAWidth) + modStringPad.PadL(sumTender.ToString("###,##0.00"), aWidth)); //"Total Tendered"
                FileSystem.PrintLine(nH);

                // Print the change
                if (Math.Round((double)prepayTenders.Tend_Totals.Change, 2) > 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)243), (short)hWidthSubAWidth) + modStringPad.PadL(System.Math.Abs(prepayTenders.Tend_Totals.Change).ToString("###,##0.00"), aWidth));
                }
                else if (Math.Round((double)prepayTenders.Tend_Totals.Change, 2) < 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)220), (short)hWidthSubAWidth) + modStringPad.PadL(System.Math.Abs(prepayTenders.Tend_Totals.Change).ToString("###,##0.00"), aWidth));
                }
                if (!(storeCredit == null))
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)244) + storeCredit.Number + _resourceManager.GetResString(offSet, (short)245), (short)hWidthSubAWidth) + modStringPad.PadL(storeCredit.Amount.ToString("0.00"), aWidth)); //"Store Credit #"," issued"
                }

                bool maskCard;
                //maskCard = System.Convert.ToBoolean(_policyManager.MASK_CARDNO);

                // Now Print Card Details
                sigLine = false;
                if (!(prepayTenders == null))
                {
                    foreach (Tender tempLoopVarT in prepayTenders)
                    {
                        T = tempLoopVarT;
                        if (T.Credit_Card.Cardnumber.Length > 0)
                        {
                            //       Sig_Line = Print_Credit_Card(T.Credit_Card, Mask_Card, nH)
                        }
                        if (T.Amount_Used != 0 && T.SignatureLine)
                        {
                            sigLine = true;
                        }
                    }
                }
                // Print a signature line if required.
                if (sigLine)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)247) + ":  ", hWidth, "_")); //Signature
                    FileSystem.PrintLine(nH);
                }

                // Print the receipt footers
                FileSystem.PrintLine(nH, store.Sale_Footer);
                for (n = 1; n <= _policyManager.ADV_LINES; n++)
                {
                    FileSystem.PrintLine(nH);
                }

                FileSystem.FileClose(nH);
                Chaps_Main.Last_Printed = "Prepay.txt";
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                return stream;
            }
            finally
            {
                FileSystem.FileClose(nH);
            }
        }

        /// <summary>
        /// Method to reprint pay at pump
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="cCard">Credit card</param>
        /// <param name="fileName">File name</param>
        /// <returns>Report</returns>
        public FileStream RePrintPayAtPump(PayAtPump payPump, Card_Reprint cCard, string fileName)
        {
            Sale_Tax papStx = default(Sale_Tax);
            Sale_Line papRepSl = default(Sale_Line);
            short nH = 0;
            string just = "";
            short hWidth = 0;
            decimal subTotal = new decimal();
            string sGrade = "";
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            string strRenamed = "";
            string strCouponTotal = "";
            var store = _policyManager.LoadStoreInfo();
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            hWidth = (short)40;
            nH = (short)(FileSystem.FreeFile());
            var filePath = Path.GetTempPath() + "\\" + fileName + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            try
            {
                FileSystem.FileOpen(nH, filePath, OpenMode.Output, OpenAccess.Write);
                Variables.UnitMeasurement = Convert.ToString(_policyManager.FUEL_UM);
                if (cCard.Language.Substring(0, 1).ToUpper() == "E") //english receipt
                {

                    FileSystem.PrintLine(nH, payPump.Header);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)234) + " " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth)); //  
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, GetMaskedCardNum(cCard.CardNumber, cCard.Card_Type));

                    if (cCard.Card_Type == "C" || cCard.Card_Type == "F")
                    {
                        if (_policyManager.BankSystem != "Moneris" && cCard.Expiry_Date != "")
                        {
                            FileSystem.PrintLine(nH, cCard.Name.ToUpper() + " " + "**/**");
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, cCard.Name.ToUpper());
                        }
                    }
                    else if (cCard.Card_Type == "T")
                    {
                        FileSystem.PrintLine(nH, cCard.Name.ToUpper());
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, "Debit" + "    " + cCard.DebitAccount);
                    }

                    if (_policyManager.BankSystem == "DataPac" || _policyManager.BankSystem == "Global")
                    {
                        FileSystem.PrintLine(nH, "Appr # " + cCard.ApprovalCode);
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, "AUTH # " + cCard.ApprovalCode);
                    }
                    if (cCard.Card_Type != "F")
                    {
                        FileSystem.PrintLine(nH, "Seq # " + cCard.Sequence_Number);
                        FileSystem.PrintLine(nH, "Terminal # " + cCard.TerminalID);
                        FileSystem.PrintLine(nH, "Trans : Purchase");
                        FileSystem.PrintLine(nH, "Res Code : " + cCard.ResponseCode);
                    }

                    FileSystem.PrintLine(nH, cCard.Receipt_Display);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, "Inv #" + System.Convert.ToString(payPump.Sale_Num));
                    FileSystem.PrintLine(nH, payPump.Sale_Date.ToString("MM/dd/yyyy") + " " + payPump.Sale_Time.ToString(timeFormatHms));
                    FileSystem.PrintLine(nH);

                    foreach (Sale_Line tempLoopVarPapRepSl in payPump.Sale_Lines)
                    {
                        papRepSl = tempLoopVarPapRepSl;
                        sGrade = _reportService.GetGradeDescription(papRepSl.GradeID);
                        string tempPolicyName = "FUEL_UM";
                        Variables.UnitMeasurement = System.Convert.ToString(_policyManager.GetPol(tempPolicyName, papRepSl));
                        FileSystem.PrintLine(nH, "Pump # " + System.Convert.ToString(papRepSl.pumpID) + "-" + sGrade);
                        FileSystem.PrintLine(nH, "Vol : " + papRepSl.Quantity.ToString("##0.000") + Variables.UnitMeasurement.Trim());
                        FileSystem.PrintLine(nH, "Price/" + Variables.UnitMeasurement.Trim() + ":" + papRepSl.price.ToString("$0.000"));
                    }
                    if (payPump.Sale_Lines.Count > 0)
                    {
                        if (payPump.Sale_Lines[1].Line_Discount > 0)
                        {
                            FileSystem.PrintLine(nH, "Sub Total: " + payPump.Sale_Lines[1].Amount.ToString("$#0.00"));
                            switch (payPump.Sale_Lines[1].Discount_Type)
                            {
                                case "%":
                                    strRenamed = "Discount(" + payPump.Sale_Lines[1].Discount_Rate.ToString("#0.0#") + "%): ";
                                    break;
                                case "$":
                                    strRenamed = "Discount($" + payPump.Customer.DiscountRate.ToString("#0.0#") + "/" +
                                        Variables.UnitMeasurement.Trim() + "): ";
                                    break;
                                default:
                                    strRenamed = strRenamed + "Discount : ";
                                    break;
                            }
                            FileSystem.PrintLine(nH, strRenamed + payPump.Sale_Lines[1].Line_Discount.ToString("$#0.00"));
                            FileSystem.PrintLine(nH, "Total: " + payPump.Sale_Totals.Gross.ToString("$#0.00"));
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, "Total: " + payPump.Sale_Amount.ToString("$#0.00"));
                        }
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, "Total: " + payPump.Sale_Amount.ToString("$#0.00"));
                    }

                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, "Fuel Includes:");

                    foreach (Sale_Tax tempLoopVarPapStx in payPump.Sale_Totals.Sale_Taxes)
                    {
                        papStx = tempLoopVarPapStx;
                        if (papStx.Tax_Included_Amount != 0)
                        {
                            FileSystem.PrintLine(nH, papStx.Tax_Name.Trim() + "(" + papStx.Tax_Rate.ToString("0.0") + "%):" + "  " + papStx.Tax_Included_Amount.ToString("$###,##0.00"));
                            subTotal = (decimal)(modGlobalFunctions.Round((double)(subTotal + papStx.Tax_Included_Amount), 2));
                        }
                        else
                        {
                            if (papStx.Tax_Added_Amount != 0)
                            {
                                FileSystem.PrintLine(nH, papStx.Tax_Name.Trim() + "(" + papStx.Tax_Rate.ToString("0.0") + "%):" + "  " + papStx.Tax_Added_Amount.ToString("$###,##0.00"));
                                subTotal = (decimal)(modGlobalFunctions.Round((double)(subTotal + papStx.Tax_Added_Amount), 2));
                            }
                        }
                    }
                    FileSystem.PrintLine(nH, "Tax Total : " + subTotal.ToString("$#0.00"));

                    FileSystem.PrintLine(nH, Strings.Trim(System.Convert.ToString(store.RegName)) + " # " + Strings.Trim(System.Convert.ToString(store.RegNum)));

                    if (store.SecRegName != "")
                    {
                        FileSystem.PrintLine(nH, Strings.Trim(System.Convert.ToString(store.SecRegName)) + " # " + Strings.Trim(System.Convert.ToString(store.SecRegNum)));
                    }
                    if (!string.IsNullOrEmpty(payPump.Customer.GroupID) && _policyManager.FuelLoyalty && payPump.Customer.DiscountType == "C")
                    {
                        if (payPump.CouponTotal > 0)
                        {
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, Strings.Space(6) + "Coupon Code");
                            FileSystem.PrintLine(nH, Strings.Space(5) + payPump.CouponID);
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, Strings.Space(6) + "Coupon Value");
                            strCouponTotal = payPump.CouponTotal.ToString("$#0.00").Trim();
                            FileSystem.PrintLine(nH, Strings.Space(Convert.ToInt32((25 - strCouponTotal.Length) / 2)) + strCouponTotal);
                        }
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, payPump.Customer.Footer);
                    }
                }
                else // french receipt
                {

                    FileSystem.PrintLine(nH, payPump.Header);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, "REIMPRESSION" + " " + DateAndTime.Today.ToString("dd-MMM-yyyy") + " a " +
                        DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth)); //  
                    FileSystem.PrintLine(nH);

                    FileSystem.PrintLine(nH, GetMaskedCardNum(cCard.CardNumber, cCard.Card_Type));
                    if (cCard.Card_Type == "C" || cCard.Card_Type == "F")
                    {
                        if (_policyManager.BankSystem != "Moneris" && cCard.Expiry_Date != "")
                        {
                            FileSystem.PrintLine(nH, cCard.Name.ToUpper() + " " + "**/**");
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, cCard.Name.ToUpper());
                        }
                    }
                    else if (cCard.Card_Type == "T")
                    {
                        FileSystem.PrintLine(nH, cCard.Name.ToUpper());
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, "Debit" + "    " + cCard.DebitAccount);
                    }
                    if (_policyManager.BankSystem == "DataPac" || _policyManager.BankSystem == "Global")
                    {
                        FileSystem.PrintLine(nH, "# Appr " + cCard.ApprovalCode);
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, "# Autor : " + cCard.ApprovalCode);
                    }
                    FileSystem.PrintLine(nH, "# Seq : " + cCard.Sequence_Number);
                    FileSystem.PrintLine(nH, "# Term : " + cCard.TerminalID);
                    FileSystem.PrintLine(nH, "Trans : Achat");
                    FileSystem.PrintLine(nH, "Code de Response: " + cCard.ResponseCode);
                    FileSystem.PrintLine(nH, cCard.Receipt_Display);
                    FileSystem.PrintLine(nH);

                    FileSystem.PrintLine(nH, "# Fact : " + Convert.ToString(payPump.Sale_Num));
                    FileSystem.PrintLine(nH, payPump.Sale_Date.ToString("MM/dd/yyyy") + " " + System.Convert.ToString(payPump.Sale_Time));
                    FileSystem.PrintLine(nH);

                    foreach (Sale_Line tempLoopVarPapRepSl in payPump.Sale_Lines)
                    {
                        papRepSl = tempLoopVarPapRepSl;
                        sGrade = _reportService.GetGradeDescription(papRepSl.GradeID);
                        string tempPolicyName2 = "FUEL_UM";
                        Variables.UnitMeasurement = System.Convert.ToString(_policyManager.GetPol(tempPolicyName2, papRepSl));
                        FileSystem.PrintLine(nH, "Pompe # " + Convert.ToString(papRepSl.pumpID) + "-" + sGrade);
                        FileSystem.PrintLine(nH, "Vol : " + papRepSl.Quantity.ToString("##0.000") + Variables.UnitMeasurement.Trim());
                        FileSystem.PrintLine(nH, "Prix/" + Variables.UnitMeasurement.Trim() + " : " +
                            papRepSl.price.ToString("$0.000"));
                    }

                    if (payPump.Sale_Lines.Count > 0)
                    {
                        if (payPump.Sale_Lines[1].Line_Discount > 0)
                        {
                            FileSystem.PrintLine(nH, "Sub Total: " + payPump.Sale_Lines[1].Amount.ToString("$#0.00"));
                            switch (payPump.Sale_Lines[1].Discount_Type)
                            {
                                case "%":
                                    strRenamed = "Discount(" + payPump.Sale_Lines[1].Discount_Rate.ToString("#0.0#") + "%): ";
                                    break;
                                case "$":
                                    strRenamed = "Discount($" + payPump.Customer.DiscountRate.ToString("#0.0#") + "/" + Variables.UnitMeasurement.Trim() + "): ";
                                    break;
                                default:
                                    strRenamed = strRenamed + "Discount : ";
                                    break;
                            }
                            FileSystem.PrintLine(nH, strRenamed + payPump.Sale_Lines[1].Line_Discount.ToString("$#0.00"));
                            FileSystem.PrintLine(nH, "Total: " + payPump.Sale_Totals.Gross.ToString("$#0.00"));
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, "Total: " + payPump.Sale_Amount.ToString("$#0.00"));
                        }
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, "Total: " + payPump.Sale_Amount.ToString("$#0.00"));
                    }

                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, "Essence Inclus:");

                    foreach (Sale_Tax tempLoopVarPapStx in payPump.Sale_Totals.Sale_Taxes)
                    {
                        papStx = tempLoopVarPapStx;
                        if (papStx.Tax_Included_Amount != 0)
                        {
                            FileSystem.PrintLine(nH, papStx.Tax_Name.Trim() + "(" + papStx.Tax_Rate.ToString("0.0") + "%):" + "  " +
                                papStx.Tax_Included_Amount.ToString("$###,##0.00"));
                            subTotal = (decimal)(Math.Round((double)(subTotal + papStx.Tax_Included_Amount), 2));
                        }
                        else
                        {
                            if (papStx.Tax_Added_Amount != 0)
                            {
                                FileSystem.PrintLine(nH, papStx.Tax_Name.Trim() + "(" + papStx.Tax_Rate.ToString("0.0") + "%):" + "  " +
                                    papStx.Tax_Added_Amount.ToString("$###,##0.00"));
                                subTotal = (decimal)(Math.Round((double)(subTotal + papStx.Tax_Added_Amount), 2));
                            }
                        }
                    }
                    FileSystem.PrintLine(nH, "Total impôt: " + subTotal.ToString("$#0.00"));

                    FileSystem.PrintLine(nH, store.RegName + " # " + Strings.Trim(System.Convert.ToString(store.RegNum)));
                    if (store.SecRegName != "")
                    {


                        FileSystem.PrintLine(nH, store.SecRegName + " # " + Strings.Trim(System.Convert.ToString(store.SecRegNum)));
                    }

                    if (!string.IsNullOrEmpty(payPump.Customer.GroupID) && _policyManager.FuelLoyalty && payPump.Customer.DiscountType == "C")
                    {
                        if (payPump.CouponTotal > 0)
                        {
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, Strings.Space(6) + "Coupon Code");
                            FileSystem.PrintLine(nH, Strings.Space(5) + payPump.CouponID);
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, Strings.Space(6) + "Coupon Value");
                            strCouponTotal = payPump.CouponTotal.ToString("$#0.00").Trim();
                            FileSystem.PrintLine(nH, Strings.Space(System.Convert.ToInt32((25 - strCouponTotal.Length) / 2)) + strCouponTotal);
                        }
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, payPump.Customer.Footer);
                    }
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, payPump.Footer);
                FileSystem.FileClose(nH);
                var stream = File.OpenRead(filePath);
                FileSystem.FileClose(nH);
                return stream;
            }
            finally
            {
                FileSystem.FileClose(nH);
            }
        }

        /// <summary>
        /// Method to print payout
        /// </summary>
        /// <param name="po">Payout</param>
        /// <param name="userCode">User code</param>
        /// <param name="userName">User name</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleTime">Sale time</param>
        /// <param name="registerNum">Register number</param>
        /// <param name="till">Till number</param>
        /// <param name="rePrint">Reprint</param>
        /// <returns>Report</returns>
        public Report Print_Payout(Payout po, string userCode, string userName, DateTime saleDate, DateTime saleTime, short registerNum, Till till, bool rePrint = false)
        {
            if (saleDate == default(DateTime))
                saleDate = DateTime.Parse("12:00:00 AM");

            if (saleTime == default(DateTime))
                saleTime = DateTime.Parse("12:00:00 AM");
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short nH = 0;
            short hWidth = 0;
            string just = "";
            short aWidth = 0;
            Payout_Tax poTax = default(Payout_Tax);
            var report = new Report
            {
                ReportName = Utilities.Constants.PayoutFile,
                Copies = _policyManager.PayoutReceiptCopies
            };
            string fileName = Path.GetTempPath() + "\\" + "Payout" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";
            try
            {
                nH = (short)(FileSystem.FreeFile());
                FileSystem.FileOpen(nH, fileName, OpenMode.Output);
                just = Strings.Left(System.Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper();
                hWidth = (short)40;
                aWidth = (short)10; if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, System.Convert.ToString(store.Address.Street1), hWidth));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, System.Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, System.Convert.ToString(store.Address.PostalCode), hWidth));
                }

                Phone phoneRenamed = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        phoneRenamed = tempLoopVarPhoneRenamed;
                        if (phoneRenamed.Number.Trim() != "")
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phoneRenamed.PhoneName + " " + phoneRenamed.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth)); //& vbCrLf

                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth) + "\r\n");
                }
                else
                {
                    FileSystem.PrintLine(nH);
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)261).ToUpper() + " #" + System.Convert.ToString(po.Sale_Num), hWidth)); //"STOCK WRITE OFF # "
                if (rePrint)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + userName + " (" + _resourceManager.GetResString(offSet, (short)132).Substring(0, 1) + System.Convert.ToString(registerNum) + "/" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1) + Convert.ToString(till.Number) + "/" + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1) + System.Convert.ToString(till.Shift) + ")", hWidth)); //Cashier
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, saleDate.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + saleTime.ToString(timeFormatHm), hWidth) + "\r\n");
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)234) + ": " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n");
                }
                else
                {
                    if (_policyManager.PRN_UName)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + userName + " (" + _resourceManager.GetResString(offSet, (short)132).Substring(0, 1) + System.Convert.ToString(registerNum) + "/" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1) + Convert.ToString(till.Number) + "/" + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1) + System.Convert.ToString(till.Shift) + ")", hWidth)); //Cashier
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + userCode + " (" + _resourceManager.GetResString(offSet, (short)132).Substring(0, 1) + System.Convert.ToString(registerNum) + "/" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1) + Convert.ToString(till.Number) + "/" + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1) + System.Convert.ToString(till.Shift) + ")", hWidth)); //Cashier
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, DateTime.Now.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateTime.Now.ToString(timeFormatHm), hWidth) + "\r\n"); //" at "  '  

                }
                if (!(po.Vendor == null))
                {
                    if (!string.IsNullOrEmpty(po.Vendor.Name))
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)2304) + " " + po.Vendor.Code + " - " + po.Vendor.Name, hWidth));
                        FileSystem.PrintLine(nH);
                    }
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)106), (short)30) + modStringPad.PadL(po.Gross.ToString("0.00"), (short)10)); //"Amount"
                                                                                                                                                                                     //   penny adjustment
                if (po.Penny_Adj != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)486), (short)(hWidth - aWidth)) + modStringPad.PadL(po.Penny_Adj.ToString("###,##0.00"), aWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadL(new string('_', 10), hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)210), (short)(hWidth - aWidth)) + modStringPad.PadL((po.Gross + po.Penny_Adj).ToString("###,##0.00"), aWidth));
                }
                FileSystem.PrintLine(nH);

                foreach (Payout_Tax tempLoopVarPoTax in po.Payout_Taxes)
                {
                    poTax = tempLoopVarPoTax;
                    if (poTax.Tax_Amount != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(poTax.Tax_Name + "   " + _resourceManager.GetResString(offSet, (short)194), (short)30) + modStringPad.PadL(poTax.Tax_Amount.ToString("0.00"), (short)10)); //" included"
                    }
                }
                FileSystem.PrintLine(nH);
                if (_policyManager.PO_REASON)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)196) + ": " + modStringPad.Proper_Case(po.Return_Reason.Description), hWidth)); //Reason
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.FileClose(nH);

                Chaps_Main.Last_Printed = "Payout.txt";
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                FileSystem.FileClose(nH);
                FileSystem.Kill(fileName);
            }
        }

        /// <summary>
        /// Print Payment
        /// </summary>
        /// <param name="till"></param>
        /// <param name="payment"></param>
        /// <param name="tenders"></param>
        /// <param name="user"></param>
        /// <param name="saleDate"></param>
        /// <param name="saleTime"></param>
        /// <param name="reprint"></param>
        /// <param name="reprintCards"></param>
        /// <returns></returns>
        public Report Print_Payment(Till till, Payment payment, Tenders tenders,
            User user, DateTime saleDate, DateTime saleTime,
            bool reprint = false, Reprint_Cards reprintCards = null)
        {
            if (saleDate == default(DateTime))
                saleDate = DateTime.Parse("12:00:00 AM");

            if (saleTime == default(DateTime))
                saleTime = DateTime.Parse("12:00:00 AM");

            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;

            int nH = 0;
            Tender tenderRenamed = default(Tender);
            decimal sumTender = new decimal();
            short n = 0;
            short aWidth = 0;
            short hWidth = 0;
            string pad = "";
            string fileName = "";
            bool sigLine = false;
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            aWidth = (short)10; // Amount Width
            hWidth = (short)40; // Total Receipt Width
            pad = Strings.Left(System.Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper();
            var report = new Report
            {
                ReportName = Utilities.Constants.PaymentFile,
                Copies = _policyManager.PaymentReceiptCopies
            };
            sumTender = 0;
            try
            {

                nH = (short)(FileSystem.FreeFile());
                fileName = Path.GetTempPath() + "\\" + "Payment" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";
                FileSystem.FileOpen(nH, fileName, OpenMode.Output, OpenAccess.Write);
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, System.Convert.ToString(store.Address.Street1), hWidth));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(pad, System.Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(pad, System.Convert.ToString(store.Address.PostalCode), hWidth));
                }

                Phone phone = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        phone = tempLoopVarPhoneRenamed;

                        if (!string.IsNullOrEmpty(phone.Number))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phone.PhoneName + " " + phone.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(pad, Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth));
                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, Strings.Trim(System.Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth));
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)248).ToUpper(), hWidth)); //"PAYMENT ON ACCOUNT"
                if (reprint)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Name, hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, saleDate.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + saleTime.ToString(timeFormatHm), hWidth) + "\r\n"); //  
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)234) + ": " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //Reprinted on '  
                }
                else
                {
                    if (_policyManager.PRN_UName)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Name, hWidth)); //"Cashier"
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)225) + ": " + user.Code, hWidth)); //"Cashier"
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //" at " '  

                }
                if (payment.Card == null)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)250) + payment.Account, hWidth)); //"Account #"
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(pad, _resourceManager.GetResString(offSet, (short)251) + payment.Card.Cardnumber, hWidth)); //"Card #"
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)249), (short)(hWidth - aWidth)) + modStringPad.PadL("$" + payment.Amount.ToString("0.00"), aWidth)); //"Amount Paid"
                                                                                                                                                                                                             //   penny adjustment
                if (payment.Penny_Adj != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)486), (short)(hWidth - aWidth)) + modStringPad.PadL(payment.Penny_Adj.ToString("###,##0.00"), aWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadL(new string('_', 10), hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)210), (short)(hWidth - aWidth)) + modStringPad.PadL((payment.Amount + payment.Penny_Adj).ToString("###,##0.00"), aWidth));
                }
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)252) + ":"); // "Paid By:"
                // Print the tenders
                FileSystem.PrintLine(nH, " ");
                sumTender = 0;
                if (tenders == null)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadC("=", (short)40, "="));
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)240), (short)40)); //"NO TENDERS"
                    FileSystem.PrintLine(nH, modStringPad.PadC("=", (short)40, "="));
                    FileSystem.PrintLine(nH);
                }
                else
                {
                    foreach (Tender tempLoopVarTenderRenamed in tenders)
                    {
                        tenderRenamed = tempLoopVarTenderRenamed;
                        if (tenderRenamed.Amount_Used != 0)
                        {
                            if (tenderRenamed.Amount_Entered != tenderRenamed.Amount_Used)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(tenderRenamed.Tender_Name + "  (" + tenderRenamed.Amount_Entered.ToString("###,##0.00") + ")", (short)(hWidth - aWidth)) + modStringPad.PadL(tenderRenamed.Amount_Used.ToString("###,##0.00"), aWidth));
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(tenderRenamed.Tender_Name, (short)(hWidth - aWidth)) + modStringPad.PadL(tenderRenamed.Amount_Used.ToString("###,##0.00"), aWidth));
                            }
                            sumTender = sumTender + tenderRenamed.Amount_Used;
                        }
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadR(" ", (short)(hWidth - aWidth)) + modStringPad.PadR("_", aWidth, "_"));
                    // Print total paid.
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)241), (short)(hWidth - aWidth)) + modStringPad.PadL(sumTender.ToString("###,##0.00"), aWidth)); //"Total Tendered"
                    // Print the change.
                    if (modGlobalFunctions.Round((double)tenders.Tend_Totals.Change, 2) < 0.0D)
                    {
                        FileSystem.PrintLine(nH, " ");
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)220), (short)(hWidth - aWidth)) + modStringPad.PadL(System.Math.Abs(tenders.Tend_Totals.Change).ToString("###,##0.00"), aWidth));

                    }
                }
                bool maskCard = false;
                if (reprint)
                {
                    RePrint_Credit_Card(till, reprintCards, Convert.ToBoolean(_policyManager.MASK_CARDNO), (short)nH);
                    FileSystem.PrintLine(nH);
                }
                else
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH);
                    sigLine = false;
                    maskCard = System.Convert.ToBoolean(_policyManager.MASK_CARDNO);
                    if (!(tenders == null))
                    {
                        foreach (Tender tempLoopVarTenderRenamed in tenders)
                        {
                            tenderRenamed = tempLoopVarTenderRenamed;

                            if (!string.IsNullOrEmpty(tenderRenamed.Credit_Card.Cardnumber))
                            {
                                Print_Credit_Card(tenderRenamed.Credit_Card, maskCard, (short)nH);
                            }
                            if (tenderRenamed.Amount_Used != 0 && tenderRenamed.SignatureLine)
                            {
                                sigLine = true;
                            }
                        }
                    }

                    if (sigLine)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)247) + " :  ", (short)40, "_")); //Signature
                        FileSystem.PrintLine(nH);
                    }

                }
                for (n = 1; n <= _policyManager.ADV_LINES; n++)
                {
                    FileSystem.PrintLine(nH);
                }
                FileSystem.FileClose(nH);
                Chaps_Main.Last_Printed = fileName;
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                FileSystem.FileClose(nH);
                FileSystem.Kill(fileName);
            }
        }

        /// <summary>
        /// Method to get list of all reprint reports
        /// </summary>
        /// <returns>List opf reprint reports</returns>
        public List<ReprintReport> GetReprintReports()
        {
            var reports = new List<ReprintReport>();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var currentPayInsideSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1254) + "_" + _resourceManager.GetResString(offSet, 1210),
                ReportName = ReportType.PayInside_CurrentSale.ToString(),
                DateEnabled = false,
                IsEnabled = true
            };
            var historyPayInsideSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1254) + "_" + _resourceManager.GetResString(offSet, 1211),
                ReportName = ReportType.PayInside_HistoricalSale.ToString(),
                DateEnabled = true,
                IsEnabled = true
            };
            var currentPayAtPumpSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1222) + "_" + _resourceManager.GetResString(offSet, 1210),
                ReportName = ReportType.PayAtPump_CurrentSale.ToString(),
                DateEnabled = false,
                IsEnabled = true
            };
            var historicalPayAtPumpSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1222) + "_" + _resourceManager.GetResString(offSet, 1211),
                ReportName = ReportType.PayAtPump_HistoricalSale.ToString(),
                DateEnabled = true,
                IsEnabled = true
            };
            var arPaySale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1244) + "_" + _resourceManager.GetResString(offSet, 1211),
                ReportName = ReportType.Payments_ArPay.ToString(),
                DateEnabled = true,
                IsEnabled = true
            };
            var fleetCardSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1202) + "_" + _resourceManager.GetResString(offSet, 1211),
                ReportName = ReportType.Payments_FleetCard.ToString(),
                DateEnabled = true,
                IsEnabled = true
            };
            var payoutSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1203) + "_" + _resourceManager.GetResString(offSet, 1211),
                ReportName = ReportType.Payments_Payout.ToString(),
                DateEnabled = true,
                IsEnabled = true
            };
            var bottleReturnSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 1248) + "_" + _resourceManager.GetResString(offSet, 1211),
                ReportName = ReportType.Payments_BottleReturn.ToString(),
                DateEnabled = true,
                IsEnabled = true
            };
            var closeBatchSale = new ReprintReport
            {
                ReportType = _resourceManager.GetResString(offSet, 5805),
                ReportName = ReportType.CloseBatch.ToString(),
                DateEnabled = true,
                IsEnabled = true
            };
            bool blnUsePatP = false;
            bool nocreditDebitpayPump = false;
            nocreditDebitpayPump = false;
            if (_reportService.IsReaderUsageAvailable())
            {
                nocreditDebitpayPump = true;
            }
            var payAtPumpCredits = _reportService.GetPayAtPumpCredits();
            var security = _policyManager.LoadSecurityInfo();
            blnUsePatP = false;
            foreach (var payAtPumpCredit in payAtPumpCredits)
            {
                if (!blnUsePatP)
                {

                    blnUsePatP = Convert.ToBoolean((payAtPumpCredit.PayPumpCredit
                        && security.Pay_Pump_Credit)
                        || (payAtPumpCredit.PayPumpCreditDebit && security.Pay_Pump_Debit
                        && security.Pay_Pump_Credit)
                    || nocreditDebitpayPump);
                }
            }
            if (blnUsePatP)
            {
                blnUsePatP = _reportService.IsPayAtPumpSalesAvailable() && blnUsePatP;
            }

            currentPayAtPumpSale.IsEnabled = blnUsePatP;

            arPaySale.IsEnabled = _policyManager.CREDTERM && _policyManager.USE_CUST;

            payoutSale.IsEnabled = Convert.ToBoolean(_policyManager.DO_PAYOUTS);

            if (security.BackOfficeVersion == "Lite")
            {
                arPaySale.IsEnabled = false;
            }

            if (security.Fleet_Card == true)
            {
                fleetCardSale.IsEnabled = true;
            }

            if (_policyManager.CC_MODE == "Cross-Ring")
            {
                closeBatchSale.IsEnabled = false;
            }
            reports.Add(currentPayInsideSale);
            reports.Add(historyPayInsideSale);
            reports.Add(currentPayAtPumpSale);
            reports.Add(historicalPayAtPumpSale);
            reports.Add(arPaySale);
            reports.Add(fleetCardSale);
            reports.Add(payoutSale);
            reports.Add(bottleReturnSale);
            reports.Add(closeBatchSale);
            return reports;
        }

        /// <summary>
        /// Method to get reprint sales
        /// </summary>
        /// <param name="reportName">Report name</param>
        /// <param name="date">Date</param>
        /// <param name="error">Error</param>
        /// <returns>Reprint sale</returns>
        public ReprintSale GetReprintSales(string reportName, DateTime? date,
            out ErrorMessage error)
        {
            error = new ErrorMessage();
            var reprintReports = GetReprintReports();
            ReportType reportType;
            var selectedReport = reprintReports.FirstOrDefault(r => r.ReportType == reportName && r.IsEnabled);
            if (selectedReport == null)
            {
                error.MessageStyle.Message = "Request is invalid";
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }
            else
            {
                Enum.TryParse(selectedReport.ReportName, out reportType);
                if (!selectedReport.DateEnabled)
                    date = DateTime.Today;
                if (selectedReport.DateEnabled && (date == null || (date.HasValue && date.Value < new DateTime(1900, 1, 1))))
                {
                    error.MessageStyle.Message = "Request is invalid";
                    error.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return null;
                }
            }
            switch (reportType)
            {
                case ReportType.Payments_BottleReturn: return GetPaymentSales('B', date.Value);
                case ReportType.PayInside_CurrentSale: return GetPayInsideSales('C', date.Value);
                case ReportType.PayInside_HistoricalSale: return GetPayInsideSales('H', date.Value);
                case ReportType.Payments_ArPay: return GetPaymentSales('A', date.Value);
                case ReportType.Payments_FleetCard: return GetPaymentSales('F', date.Value);
                case ReportType.Payments_Payout: return GetPaymentSales('P', date.Value);
                case ReportType.PayAtPump_CurrentSale: return GetPayAtPumpSales('C', date.Value);
                case ReportType.PayAtPump_HistoricalSale: return GetPayAtPumpSales('H', date.Value);
                case ReportType.CloseBatch: return GetCloseBatchSale(out error);
            }
            return null;
        }

        /// <summary>
        /// Method to get sale report
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="date">Sale date</param>
        /// <param name="reportName">Report type</param>
        /// <param name="fileName">File name</param>
        /// <param name="error">Error</param>
        /// <returns>Report</returns>
        public List<Report> GetReport(int saleNumber, DateTime? date,
            string reportName, out string fileName, out ErrorMessage error)
        {
            fileName = string.Empty;
            error = new ErrorMessage();
            ReportType reportType;
            var reprintReports = GetReprintReports();
            var selectedReport = reprintReports.FirstOrDefault(r => r.ReportType == reportName && r.IsEnabled);
            if (selectedReport == null)
            {
                error.MessageStyle.Message = "Request is invalid";
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }
            else
            {
                Enum.TryParse(selectedReport.ReportName, out reportType);
                if (!selectedReport.DateEnabled)
                    date = DateTime.Today;
                if (selectedReport.DateEnabled && (date == null || (date.HasValue && date.Value < new DateTime(1900, 1, 1))))
                {
                    error.MessageStyle.Message = "Request is invalid";
                    error.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return null;
                }
            }
            if (!IsSaleAvailable(saleNumber, date, reportName))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 28, 92, saleNumber, MessageType.OkOnly);
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }
            switch (reportType)
            {
                case ReportType.Payments_BottleReturn: return ReprintPaymentReport('B', saleNumber, date.Value, out error);
                case ReportType.PayInside_CurrentSale: return ReprintPayInsideSale('C', saleNumber, date.Value, out error);
                case ReportType.PayInside_HistoricalSale: return ReprintPayInsideSale('H', saleNumber, date.Value, out error);
                case ReportType.Payments_ArPay: return ReprintPaymentReport('A', saleNumber, date.Value, out error);
                case ReportType.Payments_FleetCard: return ReprintPaymentReport('F', saleNumber, date.Value, out error);
                case ReportType.Payments_Payout: return ReprintPaymentReport('P', saleNumber, date.Value, out error);
                case ReportType.PayAtPump_CurrentSale: return ReprintPayAtPumpSale('C', saleNumber, date.Value, out error);
                case ReportType.PayAtPump_HistoricalSale: return ReprintPayAtPumpSale('H', saleNumber, date.Value, out error);
            }
            return null;
        }

        /// <summary>
        /// Method to reprint transcation record in english
        /// </summary>
        /// <param name="tc">Card reprint</param>
        /// <param name="maskCard">Mask card</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint or not</param>
        /// <param name="merchantCopy">Merchant copy</param>
        /// <returns>Report</returns>
        public Report RePrintTransRecordEnglish(Card_Reprint tc, bool maskCard,
            bool printIt, bool reprint, bool merchantCopy)
        {
            var cardTypes = _creditCardManager.Load_CardTypes();
            var store = _policyManager.LoadStoreInfo();
            string vstr = "";
            short fileNumber = 0;
            string fileName = "";
            var report = new Report
            {
                ReportName = Utilities.Constants.TransactionReprintCopy
            };
            if (_policyManager.EMVVersion && tc.Message.IndexOf("<END-MERCHANT>") + 1 <= 0)
            {
                return null;
            }
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            fileNumber = (short)(FileSystem.FreeFile());
            fileName = Path.GetTempPath() + "\\" + "TReprint" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";
            try
            {
                FileSystem.FileOpen(fileNumber, fileName, OpenMode.Append); if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(fileNumber, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name);
                }
                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(fileNumber, store.Address.Street1);
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(fileNumber, store.Address.Street2);
                    }
                    FileSystem.PrintLine(fileNumber, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState + "\r\n" + store.Address.PostalCode);
                }
                FileSystem.PrintLine(fileNumber);
                if (reprint)
                {
                    FileSystem.PrintLine(fileNumber, "          ** REPRINT **");
                }
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber, (DateAndTime.Today.ToString("MMM-dd-yy") + "   " + DateAndTime.TimeOfDay.ToString(timeFormatHm) + Strings.Space(25)).Substring(0, 25) + (Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Substring((Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Length - 15, 15)); //  
                if (!_policyManager.EMVVersion)
                {
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, "           TRANSACTION RECORD ");
                }
                TenderCard mTendCard = default(TenderCard);
                Card cd = default(Card);
                bool bFoundCard = false;
                if (_policyManager.EMVVersion && !tc.Card_Swiped)
                {
                    if (merchantCopy)
                    {
                        if (tc.Message.IndexOf("<END-MERCHANT>") + 1 > 0)
                        {
                            FileSystem.PrintLine(fileNumber, tc.Message.Substring(0, tc.Message.IndexOf("<END-MERCHANT>") + 0)); // this for printing merchant copy
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, tc.Message);
                        }
                    }
                    else
                    {
                        if (tc.Message.IndexOf("<END-MERCHANT>") + 1 > 0)
                        {
                            FileSystem.PrintLine(fileNumber, tc.Message.Substring(tc.Message.Length - ((tc.Message.Length - (tc.Message.IndexOf("<END-MERCHANT>") + 1)) - 13), (tc.Message.Length - (tc.Message.IndexOf("<END-MERCHANT>") + 1)) - 13));
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, tc.Message);
                        }

                    }
                }
                else
                {
                    if (tc.Card_Type == "D")
                    {
                        FileSystem.PrintLine(fileNumber, "         INTERAC DIRECT PAYMENT");
                    }
                    FileSystem.PrintLine(fileNumber);
                    if (maskCard)
                    {
                        FileSystem.PrintLine(fileNumber, ("Card Number: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + GetMaskedCardNum(tc.CardNumber, tc.Card_Type)).Substring((Strings.Space(20) + GetMaskedCardNum(tc.CardNumber, tc.Card_Type)).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber, ("Card Number: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.CardNumber).Substring((Strings.Space(20) + tc.CardNumber).Length - 20, 20));
                    }
                    if (tc.Card_Type == "C")
                    {
                        if (_policyManager.BankSystem != "Moneris")
                        {
                            FileSystem.PrintLine(fileNumber, ("Exp:         " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + "**/**").Substring((Strings.Space(20) + "**/**").Length - 20, 20));
                        }
                    }
                    FileSystem.PrintLine(fileNumber, tc.Customer_Name);
                    if (tc.Store_Forward && _policyManager.BankSystem == "Moneris")
                    {
                        vstr = "KEYED";
                    }
                    else
                    {
                        if (tc.Card_Swiped)
                        {
                            vstr = "SWIPED";
                        }
                        else
                        {
                            vstr = "KEYED";
                        }

                    }
                    FileSystem.PrintLine(fileNumber, ("Card Entry: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                    if (tc.Card_Type == "D")
                    {
                        vstr = "DEBIT";
                        FileSystem.PrintLine(fileNumber, ("Account: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                        FileSystem.PrintLine(fileNumber, ("Account Type: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.DebitAccount.ToUpper()).Substring((Strings.Space(20) + tc.DebitAccount.ToUpper()).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber, ("Account: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Name.ToUpper()).Substring((Strings.Space(20) + tc.Name.ToUpper()).Length - 20, 20));
                    }
                    if (string.IsNullOrEmpty(tc.Trans_Type))
                        tc.Trans_Type = string.Empty;
                    if (!string.IsNullOrEmpty(tc.Trans_Type))
                    {
                        if (_policyManager.BankSystem == "Moneris")
                        {
                            if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                            {
                                vstr = "REFUND";
                            }
                            else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                            {
                                if (tc.Trans_Amount < 0)
                                {
                                    vstr = "PURCHASE CORRECTION";
                                }
                                else
                                {
                                    vstr = "REFUND CORRECTION";
                                }
                            }
                            else
                            {
                                if (tc.Store_Forward)
                                {
                                    mTendCard = new TenderCard();
                                    bFoundCard = false;
                                    foreach (Card tempLoopVarCd in cardTypes)
                                    {
                                        cd = tempLoopVarCd;
                                        if (cd.CardType == tc.Card_Type)
                                        {
                                            bFoundCard = true;
                                            mTendCard = _cardService.LoadTenderCard(cd.CardID);
                                            if (System.Math.Abs((short)tc.Trans_Amount) > mTendCard.FloorLimit)
                                            {
                                                vstr = "PRE-AUTH ADVICE";
                                            }
                                            else
                                            {
                                                vstr = "PURCHASE";
                                            }
                                        }
                                    }
                                    if (!bFoundCard)
                                    {
                                        vstr = "ACHAT";
                                    }
                                    mTendCard = null;
                                    cd = null;
                                }
                                else
                                {
                                    vstr = "PURCHASE";
                                }
                            }
                        }
                        else
                        {
                            if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                            {
                                vstr = "RETURN";
                            }
                            else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                            {
                                if (tc.Trans_Amount < 0)
                                {
                                    vstr = "PURCHASE CORRECTION";
                                }
                                else
                                {
                                    vstr = "RETURN CORRECTION";
                                }
                            }
                            else
                            {
                                vstr = "PURCHASE";
                            }

                        }
                    }
                    else
                    {
                        vstr = "PURCHASE";
                    }

                    FileSystem.PrintLine(fileNumber, ("Trans Type: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber, ("Amount: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Substring((Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Length - 20, 20));

                    if (!string.IsNullOrEmpty(tc.Trans_Type) && (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE"))
                    {
                        if (tc.Void_Num > 0)
                        {
                            FileSystem.PrintLine(fileNumber, ("Reference #: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Substring((Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Length - 20, 20));
                        }
                    }

                    FileSystem.PrintLine(fileNumber, ("Response Code : " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Substring((Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber, ("Auth #: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Authorization_Number).Substring((Strings.Space(20) + tc.Authorization_Number).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber, ("Sequence #: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Sequence_Number).Substring((Strings.Space(20) + tc.Sequence_Number).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber, ("Terminal #: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.TerminalID).Substring((Strings.Space(20) + tc.TerminalID).Length - 20, 20));


                    if (!Information.IsDBNull(tc.Trans_Date))
                    {
                        FileSystem.PrintLine(fileNumber, ("Date: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Substring((Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber, ("Date: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Substring((Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Length - 20, 20));
                    }
                    if (tc.Trans_Time == DateTime.Parse("00:00:00"))
                    {
                        FileSystem.PrintLine(fileNumber, ("Time: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Substring((Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Length - 20, 20));
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber, ("Time: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Substring((Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Length - 20, 20));
                    }
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, tc.Receipt_Display);
                    FileSystem.PrintLine(fileNumber);

                    if (tc.Print_Signature && tc.Card_Type != "D" && tc.Trans_Type.ToUpper() != "REFUNDINSIDE" && tc.Trans_Type.ToUpper() != "SAFREFUNDINSIDE" && tc.Trans_Type.ToUpper() != "VOIDINSIDE" && tc.Trans_Type.ToUpper() != "SAFVOIDINSIDE")
                    {
                        if (_policyManager.BankSystem == "Moneris")
                        {
                            if (tc.Language.Substring(0, 1).ToUpper() == "F")
                            {
                                FileSystem.PrintLine(fileNumber, "    Le Titulaire versera ce montant a  ");
                                FileSystem.PrintLine(fileNumber, "        L\'emetteur conformement au    ");
                                FileSystem.PrintLine(fileNumber, "             contrat adherent  ");
                            }
                            else
                            {
                                FileSystem.PrintLine(fileNumber, " Cardholder will pay card issuer above ");
                                FileSystem.PrintLine(fileNumber, "amount pursuant to Cardholder Agreement");
                            }
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber, "CUSTOMER AGREES TO PAY THE ABOVE AMOUNT");
                            FileSystem.PrintLine(fileNumber, " ACCORDING TO THE CARD ISSUER AGREEMENT");
                        }
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "Signature");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "_________________________________________");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                    }
                    else
                    {
                        if ((tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE") && tc.Trans_Amount > 0 && _policyManager.BankSystem == "Moneris" && tc.Card_Type != "D")
                        {
                            FileSystem.PrintLine(fileNumber, " Cardholder will pay card issuer above ");
                            FileSystem.PrintLine(fileNumber, "amount pursuant to Cardholder Agreement");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, "Signature");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, "_________________________________________");
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                        }
                        else
                        {
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber);
                            FileSystem.PrintLine(fileNumber, ",");

                        }
                    }

                }
                if (_policyManager.EMVVersion)
                {
                    if (merchantCopy)
                    {
                        FileSystem.PrintLine(fileNumber, Strings.ChrW(169));
                    }
                }
                FileSystem.FileClose(fileNumber);
                var stream = File.OpenRead(fileName);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                FileSystem.FileClose(fileNumber);
                FileSystem.Kill(fileName);
            }
        }

        /// <summary>
        /// Method to reprint transcation record in French
        /// </summary>
        /// <param name="tc">Card reprint</param>
        /// <param name="maskCard">Mask card</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint or not</param>
        /// <param name="merchantCopy">Merchant copy</param>
        /// <returns>Report</returns>
        public Report RePrintTransRecordFrench(Card_Reprint tc, bool maskCard,
            bool printIt, bool reprint, bool merchantCopy)
        {
            var cardTypes = _creditCardManager.Load_CardTypes();
            var store = _policyManager.LoadStoreInfo();
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            string vstr = "";
            short fileNumber = 0;
            string fileName = "";
            string ccl;
            string ccr;
            short ccc;
            if (_policyManager.EMVVersion && tc.Message.IndexOf("<END-MERCHANT>") + 1 <= 0)
            {
                return null;
            }
            var report = new Report
            {
                ReportName = Utilities.Constants.TransactionReprintCopy,

            };
            fileNumber = (short)(FileSystem.FreeFile());
            fileName = Path.GetTempPath() + "\\" + "TReprint" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";

            FileSystem.FileOpen(fileNumber, fileName, OpenMode.Append);
            if (_policyManager.PRN_CO_NAME)
            {
                FileSystem.PrintLine(fileNumber, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name);
            }
            if (_policyManager.PRN_CO_ADDR)
            {
                FileSystem.PrintLine(fileNumber, store.Address.Street1);
                if (store.Address.Street2 != "")
                {
                    FileSystem.PrintLine(fileNumber, store.Address.Street2);
                }
                FileSystem.PrintLine(fileNumber, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState + "\r\n" + store.Address.PostalCode);
            }


            FileSystem.PrintLine(fileNumber);
            if (reprint)
            {
                FileSystem.PrintLine(fileNumber, "          ** RÉIMPRESSION **");
            }
            FileSystem.PrintLine(fileNumber);
            FileSystem.PrintLine(fileNumber, (DateAndTime.Today.ToString("mmm-dd-yy") + "   " + DateAndTime.TimeOfDay.ToString(timeFormatHm) + Strings.Space(25)).Substring(0, 25) + (Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Substring((Strings.Space(15) + "Trans# " + tc.Trans_Number.Trim()).Length - 15, 15)); //  

            if (!_policyManager.EMVVersion)
            {
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber, "           RELEVE DE TRANSACTION");
            }
            TenderCard mTendCard = default(TenderCard);
            Card cd = default(Card);
            bool bFoundCard = false;
            if (_policyManager.EMVVersion && !tc.Card_Swiped)
            {
                if (merchantCopy)
                {
                    FileSystem.PrintLine(fileNumber,
                        tc.Message.IndexOf("<END-MERCHANT>") + 1 > 0
                            ? tc.Message.Substring(0, tc.Message.IndexOf("<END-MERCHANT>") + 0)
                            : tc.Message);
                }
                else
                {
                    FileSystem.PrintLine(fileNumber,
                        tc.Message.IndexOf("<END-MERCHANT>") + 1 > 0
                            ? tc.Message.Substring(
                                tc.Message.Length -
                                ((tc.Message.Length - (tc.Message.IndexOf("<END-MERCHANT>") + 1)) - 13),
                                (tc.Message.Length - (tc.Message.IndexOf("<END-MERCHANT>") + 1)) - 13)
                            : tc.Message);
                }
            }
            else
            {
                if (tc.Card_Type == "D")
                {
                    FileSystem.PrintLine(fileNumber, "           INTERAC DIRECT PAYMENT");
                }
                FileSystem.PrintLine(fileNumber);
                if (maskCard)
                {
                    FileSystem.PrintLine(fileNumber, ("Numero Carte: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + GetMaskedCardNum(tc.CardNumber, tc.Card_Type)).Substring((Strings.Space(20) + GetMaskedCardNum(tc.CardNumber, tc.Card_Type)).Length - 20, 20));
                }
                else
                {
                    FileSystem.PrintLine(fileNumber, ("Numero Carte: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.CardNumber).Substring((Strings.Space(20) + tc.CardNumber).Length - 20, 20));
                }
                if (tc.Card_Type == "C")
                {
                    if (_policyManager.BankSystem != "Moneris")
                    {
                        FileSystem.PrintLine(fileNumber, ("Exp:         " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + "**/**").Substring((Strings.Space(20) + "**/**").Length - 20, 20));
                    }
                }
                FileSystem.PrintLine(fileNumber, tc.Customer_Name);
                if (tc.Store_Forward && _policyManager.BankSystem == "Moneris")
                {
                    vstr = "Manuel"; //"M"
                }
                else
                {
                    if (tc.Card_Swiped)
                    {
                        vstr = "GLISSER";
                    }
                    else
                    {
                        vstr = "Manuel";
                    }
                }
                FileSystem.PrintLine(fileNumber, ("Mode Entree: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));

                if (tc.Card_Type == "D")
                {
                    vstr = "DEBIT";
                    FileSystem.PrintLine(fileNumber, ("Type de carte: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                    FileSystem.PrintLine(fileNumber, ("Type Compte: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.DebitAccount.ToUpper()).Substring((Strings.Space(20) + tc.DebitAccount.ToUpper()).Length - 20, 20));
                }
                else
                {
                    FileSystem.PrintLine(fileNumber, ("Type de carte: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Name.ToUpper()).Substring((Strings.Space(20) + tc.Name.ToUpper()).Length - 20, 20));
                }

                if (_policyManager.BankSystem == "Moneris")
                {
                    if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                    {
                        vstr = "REMISE D\'ACHAT";
                    }
                    else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                    {
                        if (tc.Trans_Amount < 0)
                        {
                            vstr = "CORRECTION D\'ACHAT";
                        }
                        else
                        {
                            vstr = "CORRECTION DE REMISE";
                        }
                    }
                    else
                    {
                        if (tc.Store_Forward)
                        {
                            mTendCard = new TenderCard();
                            bFoundCard = false;
                            foreach (Card tempLoopVarCd in cardTypes)
                            {
                                cd = tempLoopVarCd;
                                if (cd.CardType == tc.Card_Type)
                                {
                                    bFoundCard = true;
                                    mTendCard = _cardService.LoadTenderCard(cd.CardID);
                                    vstr = System.Math.Abs((short)tc.Trans_Amount) > mTendCard.FloorLimit ? "AVIS D\'ACHAT" : "ACHAT";
                                }
                            }
                            if (!bFoundCard)
                            {
                                vstr = "ACHAT";
                            }
                            mTendCard = null;
                            cd = null;
                        }
                        else
                        {
                            vstr = "ACHAT";
                        }

                    }
                }
                else
                {
                    if (tc.Trans_Type.ToUpper() == "REFUNDINSIDE" || tc.Trans_Type.ToUpper() == "SAFREFUNDINSIDE")
                    {
                        vstr = "RETOUR";
                    }
                    else if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                    {
                        if (tc.Trans_Amount < 0)
                        {
                            vstr = "CORRECTION D\'ACHAT";
                        }
                        else
                        {
                            vstr = "RETOUR ANNULE";
                        }
                    }
                    else
                    {
                        vstr = "ACHAT";
                    }

                }

                FileSystem.PrintLine(fileNumber, Strings.Space(20).Substring(0, 20) + (Strings.Space(20) + vstr).Substring((Strings.Space(20) + vstr).Length - 20, 20));
                FileSystem.PrintLine(fileNumber, ("Montant: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Substring((Strings.Space(20) + System.Math.Abs((short)tc.Trans_Amount).ToString("$###0.00")).Length - 20, 20));

                if (tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE")
                {
                    if (tc.Void_Num > 0)
                    {
                        FileSystem.PrintLine(fileNumber, ("# Reference: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Substring((Strings.Space(20) + System.Convert.ToString(tc.Void_Num)).Length - 20, 20)); //add by Mars
                    }
                }

                FileSystem.PrintLine(fileNumber, ("Code de Reponse : " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Substring((Strings.Space(20) + tc.ApprovalCode + " - " + tc.ResponseCode).Length - 20, 20));
                FileSystem.PrintLine(fileNumber, ("# Autor: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Authorization_Number).Substring((Strings.Space(20) + tc.Authorization_Number).Length - 20, 20));
                FileSystem.PrintLine(fileNumber, ("# Seq: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Sequence_Number).Substring((Strings.Space(20) + tc.Sequence_Number).Length - 20, 20));
                FileSystem.PrintLine(fileNumber, ("# Term: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.TerminalID).Substring((Strings.Space(20) + tc.TerminalID).Length - 20, 20));
                if (!Information.IsDBNull(tc.Trans_Date))
                {
                    FileSystem.PrintLine(fileNumber, ("Date: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Substring((Strings.Space(20) + tc.Trans_Date.ToString("MM/dd/yyyy")).Length - 20, 20));
                }
                else
                {
                    FileSystem.PrintLine(fileNumber, ("Date: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Substring((Strings.Space(20) + DateAndTime.Today.ToString("MM/dd/yy")).Length - 20, 20));
                }
                if (tc.Trans_Time == DateTime.Parse("00:00:00"))
                {
                    FileSystem.PrintLine(fileNumber, ("Heure: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Substring((Strings.Space(20) + DateAndTime.TimeOfDay.ToString("hh:mm:ss")).Length - 20, 20));
                }
                else
                {
                    FileSystem.PrintLine(fileNumber, ("Heure: " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Substring((Strings.Space(20) + tc.Trans_Time.ToString("hh:mm:ss")).Length - 20, 20));
                }
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber, tc.Receipt_Display);
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber);

                if (tc.Print_Signature && tc.Card_Type != "D" && tc.Trans_Type.ToUpper() != "REFUNDINSIDE" && tc.Trans_Type.ToUpper() != "SAFREFUNDINSIDE" && tc.Trans_Type.ToUpper() != "VOIDINSIDE" && tc.Trans_Type.ToUpper() != "SAFVOIDINSIDE")
                {
                    FileSystem.PrintLine(fileNumber, "    Le Titulaire versera ce montant a  ");
                    FileSystem.PrintLine(fileNumber, "        L\'emetteur conformement au    ");
                    FileSystem.PrintLine(fileNumber, "             contrat adherent  ");
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, "Signature");
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, "_________________________________________");
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                }
                else
                {
                    if ((tc.Trans_Type.ToUpper() == "VOIDINSIDE" || tc.Trans_Type.ToUpper() == "SAFVOIDINSIDE") && tc.Trans_Amount > 0 && _policyManager.BankSystem == "Moneris" && tc.Card_Type != "D")
                    {
                        FileSystem.PrintLine(fileNumber, "    Le Titulaire versera ce montant a  ");
                        FileSystem.PrintLine(fileNumber, "        L\'emetteur conformement au    ");
                        FileSystem.PrintLine(fileNumber, "             contrat adherent  ");

                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "Signature");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, "_________________________________________");
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                    }
                    else
                    {
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber);
                        FileSystem.PrintLine(fileNumber, ",");

                    }
                }
            }
            FileSystem.FileClose(fileNumber);
            var stream = File.OpenRead(fileName);
            FileSystem.FileClose(fileNumber);
            return report;

        }

        /// <summary>
        /// Method to reprint pay inside sale
        /// </summary>
        /// <param name="reprintType">Reprint type</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="FileName">File name</param>
        /// <param name="error">Error</param>
        /// <returns>Report</returns>
        public List<Report> ReprintPayInsideSale(char reprintType, int sn,
            DateTime saleDate, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var reports = new List<Report>();
            var fileName = "Reprint.txt";
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            string userName = "";
            Sale_Line sl = default(Sale_Line);
            Sale salRep = new Sale();
            bool maskCard = false;
            float vTaxSaved = 0;
            bool boolTaxForTaxExempt = false;
            var giveReceipt = new GiveXReceiptType();
            string po = null;
            if (sn <= 0 || !Information.IsNumeric(sn))
            {
                return null;
            }
            Tenders tend = new Tenders();
            var taxEmempt = _policyManager.TAX_EXEMPT;
            var teType = _policyManager.TE_Type;
            maskCard = Convert.ToBoolean(_policyManager.MASK_CARDNO);
            string strFileName = Path.GetTempPath() + "\\GiveXReceipt" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";
            var db = reprintType == 'C' ? DataSource.CSCTills : DataSource.CSCTrans;
            var saleTenders = _reportService.GetSaleTenders(sn, db);
            foreach (var saleTender in saleTenders)
            {
                if ((_policyManager.COMBINEFLEET && saleTender.TenderClass == "FLEET") || (_policyManager.COMBINECR && saleTender.TenderClass == "CRCARD"))
                {
                    if (_reportService.IsTenderAvailable(saleTender.TenderName))
                    {
                        tend.Add(saleTender.TenderName, saleTender.TenderClass, Convert.ToDouble(saleTender.Exchange),
                            true, true, true, (short)1, "A", false, 99, 99, 99, true, Convert.ToDouble(false),
                            Convert.ToInt16(saleTender.AmountTend), true, false, "",
                            saleTender.TenderName + saleTender.SequenceNumber);
                    }
                    else
                    {
                        tend.Add(saleTender.ClassDescription, saleTender.TenderClass, Convert.ToDouble(saleTender.Exchange),
                            true, true, true, (short)1, "A", false, 99, 99, 99, true, Convert.ToDouble(false),
                            Convert.ToInt16(saleTender.AmountTend), true, false, "",
                            saleTender.TenderName + saleTender.SequenceNumber);
                    }
                }
                else
                {
                    if (_reportService.IsTenderAvailable(saleTender.TenderName))
                    {
                        tend.Add(saleTender.TenderName, saleTender.TenderClass, Convert.ToDouble(saleTender.Exchange),
                              true, true, true, (short)1, "A", false, 99, 99, 99, true, Convert.ToDouble(false),
                              Convert.ToInt16(saleTender.AmountTend), true, false, "",
                              saleTender.TenderName + saleTender.SequenceNumber);
                    }
                    else
                    {
                        tend.Add(saleTender.ClassDescription, saleTender.TenderClass, Convert.ToDouble(saleTender.Exchange),
                            true, true, true, (short)1, "A", false, 99, 99, 99, true, Convert.ToDouble(false),
                            Convert.ToInt16(saleTender.AmountTend), true, false, "",
                            saleTender.TenderName + saleTender.SequenceNumber);
                    }
                    if (saleTender.TenderClass == "ACCOUNT" && _policyManager.PRINT_CrdHld)
                    {
                        if (_reportService.IsCardTenderAvailable(sn, saleTender.TenderName, db))
                        {
                            tend[saleTender.TenderName + saleTender.SequenceNumber].Credit_Card.Crd_Type = "F";
                        }
                    }
                }
                tend[saleTender.TenderName + saleTender.SequenceNumber].Amount_Entered = Convert.ToDecimal(saleTender.AmountTend);
                tend[saleTender.TenderName + saleTender.SequenceNumber].Amount_Used = Convert.ToDecimal(saleTender.AmountUsed);

                if (Strings.Trim(saleTender.TenderClass).ToUpper() == "ACCOUNT")
                {
                    if (!string.IsNullOrEmpty(saleTender.SerialNumber))
                    {
                        po = saleTender.SerialNumber;
                    }
                }
            }

            salRep = _reportService.GetSaleHead(sn, db);
            salRep.AR_PO = po;
            _saleHeadManager.Load_Taxes(ref salRep);
            salRep.LoadingTemp = true;
            salRep.Customer = _customerManager.LoadCustomer(salRep.Customer.Code);
            if (salRep.ReferenceNumber != "" && _policyManager.TAX_EXEMPT_GA)
            {
                salRep.EligibleTaxEx = true;
            }
            if ((teType != "AITE" && teType != "QITE") && _policyManager.TE_GETNAME)
            {
                salRep.TreatyName = _treatyService.GetTreatyName(salRep.TreatyNumber);
            }
            tend.Tend_Totals.Change = salRep.Sale_Change;
            var saleLines = _reportService.GetSaleLines(sn, db);
            vTaxSaved = 0;
            foreach (var saleLine in saleLines)
            {
                sl = saleLine;
                sl.No_Loading = true;
                _saleLineManager.SetSubDetail(ref sl, sl.Sub_Detail);
                sl.PRICE_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("PRICE_DEC", sl));
                sl.QUANT_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("QUANT_DEC", sl));
                error = new ErrorMessage();
                sl.price = Math.Round(sl.price, 3);
                sl.Regular_Price = Math.Round(sl.Regular_Price, 3);
                _saleLineManager.SetDiscountRate(ref sl, sl.Discount_Rate);
                var lineTaxes = _reportService.GetLineTaxes(sn, saleLine.Line_Num, db);
                foreach (Line_Tax lineTax in lineTaxes)
                {

                    sl.Line_Taxes.Add(lineTax.Tax_Name,
                        lineTax.Tax_Code,
                        lineTax.Tax_Rate,
                        lineTax.Tax_Included,
                        lineTax.Tax_Rebate_Rate,
                        lineTax.Tax_Rebate, "");
                    if (!string.IsNullOrEmpty(sl.TE_COLLECTTAX))
                    {
                        if (sl.TE_COLLECTTAX.Trim() == lineTax.Tax_Name)
                        {
                            boolTaxForTaxExempt = true;
                        }
                    }
                    if (boolTaxForTaxExempt && !string.IsNullOrEmpty(salRep.TreatyNumber))
                    {
                        _saleManager.ApplyTaxes(ref salRep, false);
                    }
                }
                sl.Charges = _reportService.GetCharges(sn, saleLine.Line_Num, db);
                sl.Line_Kits = _reportService.Get_Line_Kit(sn, saleLine.Line_Num, db);
                if (sl.IsTaxExemptItem && taxEmempt)
                {
                    if (teType == "SITE")
                    {
                        vTaxSaved = _reportService.GetTaxSaved(sn, saleLine.Line_Num, db);
                    }
                }
                _saleManager.Add_a_Line(ref salRep, sl, sl.User, salRep.TillNumber, out error, false, false, true, salRep.Void_Num == 0 ? true : false, true);
            }

            if (taxEmempt && teType != "SITE")
            {
                var oTeSale = new TaxExemptSale();
                var taxExempt = _reportService.GetTotalExemptedTax(sn, salRep.TillNumber, db);
                if (taxExempt != 0)
                {
                    vTaxSaved = taxExempt;
                    oTeSale = _reportService.LoadTaxExempt(sn, salRep.TillNumber, db, teType);
                    if (teType == "QITE")
                    {
                        if (oTeSale == null)
                        {
                            // return;
                        }
                    }
                    else // 
                    {
                        if (oTeSale == null)
                        {
                            // return;
                        }
                    }
                    var taxExemptFile = PrintTaxExemptVoucher(oTeSale, true, false);
                    reports.Add(taxExemptFile);
                }
                else
                {
                    vTaxSaved = 0;
                    if (_reportService.LoadGstExempt(sn, salRep.TillNumber, db, ref oTeSale))
                    {
                        oTeSale.teCardholder.GstExempt = true;
                    }
                }
                if (oTeSale != null)
                    CacheManager.AddTaxExemptSaleForTill(salRep.TillNumber, sn, oTeSale);
            }
            salRep.TotalTaxSaved = vTaxSaved;
            var voidNo = _reportService.GetVoidNo(sn, db);
            Reprint_Cards reprintCards = new Reprint_Cards();
            Card_Reprint cardReprint = default(Card_Reprint);
            var cardTenders = _reportService.GetCardTenders(sn, db);
            foreach (var cardTender in cardTenders)
            {
                cardReprint = new Card_Reprint();
                cardReprint.Card_Type = cardTender.CardType;
                cardReprint.CardNumber = System.Convert.ToString(_encryptManager.Decrypt(cardTender.CardNum));
                cardReprint.Name = cardTender.CardName;
                cardReprint.Expiry_Month = Strings.Right(cardTender.ExpiryDate, 2);
                cardReprint.Expiry_Year = Strings.Left(cardTender.ExpiryDate, 2);
                cardReprint.Card_Swiped = cardTender.Swiped;
                cardReprint.Authorization_Number = cardTender.ApprovalCode;
                cardReprint.Print_Signature = true;
                cardReprint.Language = cardTender.Language;
                cardReprint.Expiry_Date = cardTender.ExpiryDate;
                cardReprint.Customer_Name = cardTender.CustomerName;
                cardReprint.TerminalID = cardTender.TerminalID;
                cardReprint.Trans_Date = cardTender.TransactionDate;
                cardReprint.Trans_Time = cardTender.TransactionTime;
                cardReprint.Trans_Amount = Convert.ToSingle(cardTender.Amount);
                cardReprint.Trans_Number = cardTender.SaleNumber.ToString();
                cardReprint.ResponseCode = cardTender.ResponseCode;
                cardReprint.ApprovalCode = cardTender.ISOCode;
                cardReprint.Sequence_Number = cardTender.SequenceNumber;
                cardReprint.DebitAccount = cardTender.DebitAccount;
                cardReprint.Receipt_Display = cardTender.ReceiptDisplay;
                cardReprint.Vechicle_Number = cardTender.VechicleNo;
                cardReprint.Driver_Number = cardTender.DriverNo;
                cardReprint.ID_Number = cardTender.IdentificationNo;
                cardReprint.Odometer_Number = cardTender.Odometer;
                cardReprint.UsageType = cardTender.CardUsage;
                cardReprint.PrintDriver = cardTender.PrintDriverNo;
                cardReprint.PrintIdentification = cardTender.PrintIdentificationNo;
                cardReprint.PrintOdometer = cardTender.PrintOdometer;
                cardReprint.PrintUsage = cardTender.PrintUsage;
                cardReprint.PrintVechicle = cardTender.PrintVechicleNo;
                cardReprint.CardprofileID = cardTender.CardProfileID;
                cardReprint.TillNumber = Convert.ToByte(cardTender.TillNumber);
                cardReprint.PONumber = cardTender.PONumber;
                cardReprint.Store_Forward = cardTender.StoreForward;
                if (voidNo != 0)
                {
                    cardReprint.Void_Num = voidNo;
                }
                cardReprint.Balance = cardTender.Balance;
                cardReprint.Result = cardTender.Result;
                cardReprint.Quantity = cardTender.Quantity;
                cardReprint.Message = cardTender.Message;
                reprintCards.Add(cardReprint, "");
            }
            var user = _loginManager.GetExistingUser(salRep.Vendor);
            if (user == null)
            {
                userName = salRep.Vendor;
            }
            else
            {
                if (_policyManager.PRN_UName)
                {
                    userName = user.Name;
                }
                else
                {
                    userName = user.Code;
                }
            }
            salRep.Vendor = "";
            _saleManager.ReCompute_Totals(ref salRep);

            if (reprintCards != null)
            {

                foreach (Card_Reprint tempLoopVarCardReprintRenamed in reprintCards)
                {
                    cardReprint = tempLoopVarCardReprintRenamed;
                    if (salRep.Sale_Totals.Gross < 0)
                    {
                        if (cardReprint.Void_Num > 0)
                        {
                            cardReprint.Trans_Type = "VOIDINSIDE";
                        }
                        else
                        {
                            cardReprint.Trans_Type = "REFUNDINSIDE";
                        }
                    }
                    else
                    {
                        if (cardReprint.Void_Num > 0)
                        {
                            cardReprint.Trans_Type = "VOIDINSIDE";
                        }
                    }

                    if (cardReprint.Card_Type == "G")
                    {
                        giveReceipt.Date = cardReprint.Trans_Date.ToString("MM/dd/yyyy");
                        giveReceipt.Time = cardReprint.Trans_Time.ToString("hh:mm:ss");

                        giveReceipt.UserID = Convert.ToString(_policyManager.GIVEX_USER);
                        giveReceipt.TranType = (short)(Conversion.Val(cardReprint.Result));
                        giveReceipt.SeqNum = cardReprint.Trans_Number;
                        giveReceipt.CardNum = cardReprint.CardNumber;
                        giveReceipt.ExpDate = cardReprint.Expiry_Date;
                        giveReceipt.Balance = (float)cardReprint.Balance;
                        if (giveReceipt.TranType == 3)
                        {
                            giveReceipt.SaleAmount = 0 - cardReprint.Trans_Amount;
                        }
                        else
                        {
                            giveReceipt.SaleAmount = cardReprint.Trans_Amount;
                        }
                        giveReceipt.ResponseCode = cardReprint.Receipt_Display;
                        var givexReceipt = Print_GiveX_Receipt(giveReceipt, false, true, (short)1, true);
                        reports.Add(givexReceipt);
                    }
                    else
                    {
                        if (cardReprint.Card_Type != "F")
                        {
                            Report transactionFile = null;
                            if (cardReprint.Language == "French")
                            {
                                transactionFile = RePrintTransRecordFrench(cardReprint, maskCard, false, true, false);
                            }
                            else
                            {
                                transactionFile = RePrintTransRecordEnglish(cardReprint, maskCard, false, true, false);
                            }
                            reports.Add(transactionFile);
                        }
                    }
                }
            }

            string strLineNumbers = "";
            string[] ln = null;
            short i = 0;
            if (modGlobalFunctions.HasGivexSale(salRep, ref strLineNumbers))
            {
                ln = Strings.Split(Expression: strLineNumbers, Delimiter: ",", Compare: CompareMethod.Text);
                for (i = 0; i <= (ln.Length - 1); i++)
                {
                    if (reprintType == 'C')
                    {
                        if (_reportService.Load_CardSales(DataSource.CSCTills, salRep.TillNumber, salRep.Sale_Num,
                            (short)(Conversion.Val(ln[i])), out giveReceipt))
                        {
                            var givexReceipt = Print_GiveX_Receipt(giveReceipt, false, true, (short)1, true);
                            reports.Add(givexReceipt);
                        }
                    }
                    else
                    {
                        if (_reportService.Load_CardSales(DataSource.CSCTrans, (short)0,
                            salRep.Sale_Num, (short)(Conversion.Val(ln[i])), out giveReceipt))
                        {
                            var givexReceipt = Print_GiveX_Receipt(giveReceipt, false, true, (short)1, true);
                            reports.Add(givexReceipt);
                        }
                    }
                }
            }

            string tempFileName = "";
            bool tempReprint = true;
            Stream signature;
            var reprint = Print_Receipt(salRep.TillNumber, null, ref salRep, ref tend, false,
                ref tempFileName, ref tempReprint, out signature, user.Code, false, reprintCards);
            reports.Add(reprint);
            return reports;
        }

        /// <summary>
        /// Method to pay at pump sale
        /// </summary>
        /// <param name="reprintType"></param>
        /// <param name="sn"></param>
        /// <param name="saleDate"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public List<Report> ReprintPayAtPumpSale(char reprintType, int sn,
           DateTime saleDate, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var fName = "PayAtPump.txt";
            var report = new Report
            {
                ReportName = Utilities.Constants.PayAtPumpFile,

            };
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            PayAtPump papRep = new PayAtPump();
            Sale_Line papRepSl = default(Sale_Line);
            Card_Reprint papRepCard = default(Card_Reprint);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (sn == 0 || !Information.IsNumeric(sn))
            {
                return null;
            }
            string strHeader = string.Empty;
            short nH = 0;
            string strReceipt = "";


            strReceipt = "";
            if (_policyManager.EMVVERSION_PATP)
            {
                strReceipt = _reportService.GetHistoryMessage(sn, reprintType == 'C' ? DataSource.CSCTills : DataSource.CSCTrans);
            }
            string receipt = string.Empty;
            if (!_policyManager.EMVVERSION_PATP || string.IsNullOrEmpty(strReceipt))
            {
                receipt = _reportService.GetReceipt(sn, reprintType == 'C' ? DataSource.CSCPayPump : DataSource.CSCPayPumpHist);
            }
            if (!string.IsNullOrEmpty(receipt))
            {
                papRep.Sale_Num = sn;
                if (_policyManager.EMVVERSION_PATP && !string.IsNullOrEmpty(strReceipt))
                {
                    strReceipt = strHeader + "\r\n" + "\r\n" + strReceipt;
                }
                else
                {
                    strReceipt = receipt;
                }
            }
            string fileName = Path.GetTempPath() + "\\" + fName + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}" + GenerateRandomNo() + ".txt";
            if (!string.IsNullOrEmpty(strReceipt))
            {
                strReceipt = strReceipt + "\r\n" + _resourceManager.GetResString(offSet, (short)234) + " " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormatHm);
                nH = (short)(FileSystem.FreeFile());
                FileSystem.FileOpen(nH, fileName, OpenMode.Output, OpenAccess.Write);
                FileSystem.PrintLine(nH, strReceipt);
                FileSystem.FileClose(nH);
                var content = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                report.ReportContent = GetReportContent(content);
            }
            var dataSouce = reprintType == 'C' ? DataSource.CSCTills : DataSource.CSCTrans;
            papRep = _reportService.GetSaleForPayAtPump(sn, 100, dataSouce);
            var sale = new Sale();
            _saleHeadManager.Load_Taxes(ref sale);
            papRep.Sale_Totals.Sale_Taxes = sale.Sale_Totals.Sale_Taxes;
            var loyaltyCard = papRep.Customer.LoyaltyCard;
            papRep.Customer = _customerManager.LoadCustomer(papRep.Customer.Code);
            papRep.Customer.LoyaltyCard = loyaltyCard;
            var saleLines = _reportService.GetSaleLinesForPayAtPump(sn, 100, dataSouce);

            foreach (var saleLine in saleLines)
            {
                papRepSl = new Sale_Line();
                papRepSl.No_Loading = true;
                papRepSl.Dept = saleLine.Dept;
                papRepSl.Sub_Dept = saleLine.Sub_Dept;
                papRepSl.Sub_Detail = saleLine.Sub_Detail;
                papRepSl.Stock_Code = saleLine.Stock_Code;
                papRepSl.PLU_Code = saleLine.PLU_Code;
                papRepSl.Line_Taxes = _saleLineManager.Make_Taxes(papRepSl);
                papRepSl.pumpID = saleLine.pumpID;
                papRepSl.PositionID = saleLine.PositionID;
                papRepSl.GradeID = saleLine.GradeID;
                papRepSl.Quantity = saleLine.Quantity;
                papRepSl.price = saleLine.price;
                papRepSl.Amount = saleLine.Amount;
                papRepSl.Discount_Adjust = saleLine.Discount_Adjust;
                papRepSl.Line_Discount = saleLine.Line_Discount;
                papRepSl.Discount_Type = saleLine.Discount_Type;
                papRepSl.Discount_Code = saleLine.Discount_Code;
                papRepSl.Discount_Rate = saleLine.Discount_Rate;
                papRepSl.DiscountName = saleLine.DiscountName;
                var user = _loginManager.GetExistingUser(UserCode);
                _payAtPumpManager.Add_a_Line(ref papRep, user, papRepSl, true, false, true);
            }

            papRepCard = _reportService.GetCardTender(sn, 100, dataSouce);
            papRepCard.CardNumber = System.Convert.ToString(_encryptManager.Decrypt(papRepCard.CardNumber));
            _payAtPumpManager.SetLanguage(ref papRep, papRepCard.Language);
            papRepCard.Expiry_Month = Strings.Right(papRepCard.Expiry_Date, 2);
            papRepCard.Expiry_Year = Strings.Left(papRepCard.Expiry_Date, 2);

            if (!string.IsNullOrEmpty(papRepCard.Sequence_Number))
            {
                if (_policyManager.BankSystem != "Moneris" && papRepCard.Card_Type != "T")
                {
                    papRepCard.Sequence_Number = Strings.Left(Strings.Trim(papRepCard.Sequence_Number), Strings.Trim(papRepCard.Sequence_Number).Length - 1);
                }
                else //Moneris
                {
                    papRepCard.Sequence_Number = Strings.Trim(papRepCard.Sequence_Number) + " S"; // Card has Swiped
                }
            }
            papRep.Sale_Date = papRepCard.Trans_Date;
            papRep.Sale_Time = papRepCard.Trans_Time;

            var stream = RePrintPayAtPump(papRep, papRepCard, "PayAtPump");
            report.ReportContent = GetReportContent(stream);
            var reports = new List<Report> { report };
            return reports;
        }

        /// <summary>
        /// Method to print tax exempt voucher
        /// </summary>
        /// <param name="oteSale">Tax exempt sale</param>
        /// <param name="reprint">Reprint</param>
        /// <param name="printIt">Print or not</param>
        /// <returns>Report content</returns>
        public Report PrintTaxExemptVoucher(TaxExemptSale oteSale, bool reprint = false, bool printIt = true)
        {
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short hWidth = 0;
            short dWidth = 0;
            short aWidth = 0;
            short qWidth = 0;
            string fileName = "";
            int nH = 0;
            string just = "";
            TaxExemptSaleLine tesl = default(TaxExemptSaleLine);
            short I = 0;
            float totalOverrideExemptTax = 0;
            var report = new Report
            {
                ReportName = Utilities.Constants.TaxExemptionFile,
                Copies = _policyManager.TaxExemptVoucherCopies
            };

            if ((!oteSale.GasOverLimit) && (!oteSale.TobaccoOverLimit) && (!oteSale.PropaneOverLimit))
            {
                return null;
            }
            var teSystem = new teSystem();
            _teSystemManager.GetAllReasons(ref teSystem);
            _teSystemManager.GetAllLimits(ref teSystem);
            var timeFormatHm = string.Empty;
            var timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            fileName = Path.GetTempPath() + "\\TaxExemptVoucher" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";

            dWidth = (short)19; // Description Width
            aWidth = (short)13; // Amount Width
            qWidth = (short)8; // Quantity Width
            hWidth = (short)40; // Total Receipt Width

            just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification

            nH = FileSystem.FreeFile();
            FileSystem.FileOpen(nH, fileName, OpenMode.Output, OpenAccess.Write);
            try
            {
                //ALBERTA INDIAN TAX EXEMPTION VOUCHER
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)5464), hWidth) + "\r\n");

                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, store.Code + "  " + store.Name, hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, System.Convert.ToString(store.Address.Street1), hWidth));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, System.Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, System.Convert.ToString(store.Address.PostalCode), hWidth));
                }
                Phone phoneRenamed = default(Phone);
                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        phoneRenamed = tempLoopVarPhoneRenamed;
                        if (phoneRenamed.Number.Trim() != "")
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phoneRenamed.PhoneName + " " + phoneRenamed.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth));
                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(System.Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth));
                }
                FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)5465) + ": " + teSystem.Retailer, hWidth) + "\r\n"); //"Retailer"
                FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)8384) + ": " + System.Convert.ToString(oteSale.Sale_Num), hWidth)); //"Transaction Number: "
                if (reprint)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just,
                        _resourceManager.GetResString(offSet, (short)225) + ": " + oteSale.UserCode
                        + " (" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1)
                        + Convert.ToString(oteSale.TillNumber) + "/"
                        + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1)
                        + Convert.ToString(oteSale.Shift) + ")", hWidth)); FileSystem.PrintLine(nH, modStringPad.PadIt(just, oteSale.Sale_Time.ToString("dd-MMM-yyyy") +
                        _resourceManager.GetResString(offSet, (short)208) +
                        oteSale.Sale_Time.ToString(timeFormatHm), hWidth) + "\r\n");
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)234) + ": " +
                        DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) +
                        DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n");
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)225) + ": " + oteSale.UserCode + " (" + _resourceManager.GetResString(offSet, (short)131).Substring(0, 1) + System.Convert.ToString(oteSale.TillNumber) + "/" + _resourceManager.GetResString(offSet, (short)346).Substring(0, 1) + System.Convert.ToString(oteSale.Shift) + ")", hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, oteSale.Sale_Time.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) +
                        oteSale.Sale_Time.ToString(timeFormatHm), hWidth) + "\r\n");
                }
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5466) + ":" + oteSale.teCardholder.CardNumber); //AITE Card
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)189) + ":" + (oteSale.teCardholder.Name.Length > 35 ? oteSale.teCardholder.Name.Substring(0, 35) : oteSale.teCardholder.Name)); //Name
                FileSystem.PrintLine(nH, (oteSale.teCardholder.Name.Length > 35 ? oteSale.teCardholder.Name.Substring(35) : "") + "\r\n");
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)5467), dWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)230), qWidth) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)5468), aWidth));
                FileSystem.PrintLine(nH, new string('_', hWidth));

                totalOverrideExemptTax = 0;
                foreach (TaxExemptSaleLine tempLoopVarTesl in oteSale.Te_Sale_Lines)
                {
                    tesl = tempLoopVarTesl;
                    if (tesl.OverLimit)
                    {
                        if (((tesl.ProductType == mPrivateGlobals.teProductEnum.eCigar) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eCigarette)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(tesl.StockCode + "-" + tesl.Description, hWidth));
                            FileSystem.PrintLine(nH, modStringPad.PadR(new string(' ', dWidth), dWidth) + modStringPad.PadL(tesl.Quantity.ToString("#0.00"), qWidth) +
                                modStringPad.PadL(tesl.ExemptedTax.ToString("#0.00"), aWidth));
                            totalOverrideExemptTax = totalOverrideExemptTax + tesl.ExemptedTax;
                        }
                        else if (((((tesl.ProductType == mPrivateGlobals.teProductEnum.eGasoline) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eDiesel)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.ePropane)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedGas)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel))
                        {

                            FileSystem.PrintLine(nH, modStringPad.PadR(tesl.StockCode, dWidth) +
                                modStringPad.PadL(tesl.Quantity.ToString("#0.000"), qWidth) +
                                modStringPad.PadL(tesl.ExemptedTax.ToString("#0.00"), aWidth));
                            totalOverrideExemptTax = totalOverrideExemptTax + tesl.ExemptedTax;
                        }
                    }
                }
                FileSystem.PrintLine(nH, modStringPad.PadL(new string('_', aWidth), hWidth));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)210), (short)(dWidth + qWidth)) +
                    modStringPad.PadL(totalOverrideExemptTax.ToString("#0.00"), aWidth) + "\r\n");
                if (oteSale.TobaccoOverLimit)
                {
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5469) + ": " + oteSale.TobaccoReasonDesp); //"Tobacco Reason"
                    if (oteSale.TobaccoReasonDetail != "")
                    {
                        I = (short)(oteSale.TobaccoReasonDetail.IndexOf("####") + 1);
                        if (I <= 0)
                        {
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5470) + ": " + oteSale.TobaccoReasonDetail + "\r\n"); //"Explanation"
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)305) + ": ", (short)(Strings.Len(_resourceManager.GetResString(offSet, (short)5471) + ": "))) + modStringPad.PadR(oteSale.TobaccoReasonDetail.Substring(0, I - 1), (short)25)); //"Date:"
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5471) + ": " + modStringPad.PadR(oteSale.TobaccoReasonDetail.Substring(I + 4 - 1), (short)25) + "\r\n"); //"Location:"
                        }
                    }
                    else
                    {
                        FileSystem.PrintLine(nH);
                    }
                }
                if (oteSale.GasOverLimit)
                {
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5472) ?? "" + ": " + oteSale?.GasReasonDesp ?? ""); //"Fuel Reason"
                    if (oteSale.GasReasonDetail != "" && oteSale.GasReasonDetail != null)
                    {
                        I = (short)(oteSale.GasReasonDetail.IndexOf("####") + 1);
                        if (I <= 0)
                        {
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5470) + ": " + oteSale.GasReasonDetail + "\r\n"); //"Explanation"
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)305) + ": ", (short)(Strings.Len(_resourceManager.GetResString(offSet, (short)5471) + ": "))) + modStringPad.PadR(oteSale.GasReasonDetail.Substring(0, I - 1), (short)25)); //"Date:"
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5471) + ": " + modStringPad.PadR(oteSale.GasReasonDetail.Substring(I + 4 - 1), (short)25) + "\r\n"); //"Location:"
                        }
                    }
                    else
                    {
                        FileSystem.PrintLine(nH);
                    }
                }
                if (oteSale.PropaneOverLimit)
                {
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5473) + ": " + oteSale.PropaneReasonDesp); //"Propane Reason"
                    if (oteSale.PropaneReasonDetail != "")
                    {
                        I = (short)(oteSale.PropaneReasonDetail.IndexOf("####") + 1);
                        if (I <= 0)
                        {
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5470) + ": " + oteSale.PropaneReasonDetail + "\r\n"); //"Explanation"
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)305) + ": ", (short)(Strings.Len(_resourceManager.GetResString(offSet, (short)5471) + ": "))) + modStringPad.PadR(oteSale.PropaneReasonDetail.Substring(0, I - 1), (short)25)); // "Date"
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)5471) + ": " + modStringPad.PadR(oteSale.PropaneReasonDetail.Substring(I + 4 - 1), (short)25) + "\r\n"); //"Location"
                        }
                    }
                    else
                    {
                        FileSystem.PrintLine(nH);
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)5470) + ": ", hWidth, "_") + "\r\n"); //"Explanation"
                FileSystem.PrintLine(nH, teSystem.VoucherFooter + "\r\n");

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)247) + ": ", hWidth, "_")); //Signature
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);

                FileSystem.FileClose(nH);
                Chaps_Main.Last_Printed = "TaxExemptVoucher.txt";
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                report.ReportContent = GetReportContent(stream);
                return report;
            }
            finally
            {
                FileSystem.FileClose(nH);
                FileSystem.Kill(fileName);
            }
        }

        /// <summary>
        /// Method to print givex close report
        /// </summary>
        /// <param name="cashoutId">Cash out Id</param>
        /// <param name="printIt">Print it</param>
        /// <param name="reprint">Reprint</param>
        /// <returns></returns>
        public Report PrintGiveXClose(string cashoutId, bool printIt, ref bool reprint)
        {

            var report = new Report
            {
                ReportName = Utilities.Constants.GivexCloseFile,
                Copies = 1
            };

            var stream = MakeGiveReport(cashoutId, reprint);
            if (stream != null)
            {
                report.ReportContent = GetReportContent(stream);
                return report;

            }
            return null;
        }

        /// <summary>
        /// Method to mask givex report
        /// </summary>
        /// <param name="cashoutId">Cash out Id</param>
        /// <param name="reprint">Reprint</param>
        /// <returns></returns>
        public FileStream MakeGiveReport(string cashoutId, bool reprint)
        {
            var storeRenamed = _policyManager.LoadStoreInfo();
            var offSet = storeRenamed.OffSet;
            short fileNumber = 0;
            string fileName = "";
            string strSql = "";
            string[] arrRpt = null;
            short i = 0;
            string strTmp = "";










            var batch = _reportService.GetGivexCloseAvailableForCashOutId(cashoutId);
            if (batch == null) return null;
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            fileNumber = (short)(FileSystem.FreeFile());
            fileName = Path.GetTempPath() + "\\" + "GiveXClose" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            if (FileSystem.Dir(fileName) != "")
            {
                Variables.DeleteFile(fileName);
            }
            FileSystem.FileOpen(fileNumber, fileName, OpenMode.Output);
            if (_policyManager.PRN_CO_NAME)
            {
                FileSystem.PrintLine(fileNumber, modStringPad.PadC((_policyManager.PRN_CO_CODE ? storeRenamed.Code + "  " : "") + storeRenamed.Name, (short)41));
            }
            FileSystem.PrintLine(fileNumber, modStringPad.PadC(System.Convert.ToString(storeRenamed.Address.Street1), (short)41));
            FileSystem.PrintLine(fileNumber, modStringPad.PadC(storeRenamed.Address.City + "," + storeRenamed.Address.ProvState, (short)41));
            FileSystem.PrintLine(fileNumber);
            FileSystem.PrintLine(fileNumber);
            string timeFormat;
            if (_policyManager.TIMEFORMAT == "24 HOURS")
            {
                timeFormat = "hh:mm";
            }
            else
            {
                timeFormat = "hh:mm tt";
            }
            FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3216), (short)41)); //"GiveX CLOSE BATCH REPORT"
            FileSystem.PrintLine(fileNumber);
            FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3217) + " " + Variables.GiveX_Renamed.MerchantID, (short)41)); //"Merchant #:"
            if (reprint)
            {
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)234) + ": " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormat), (short)41)); //Reprinted on   '  
                FileSystem.PrintLine(fileNumber);
            }
            var tempBatch = _reportService.GetTempCloseBatchById(batch.BatchNumber);
            if (tempBatch != null)
            {
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3218) + ":   " + tempBatch.Date.ToString("dd-MMM-yyyy") + " " + tempBatch.Time.ToString(timeFormatHms), (short)41)); //"From: Date Time "   '  
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3219) + ":   " + batch.Date.ToString("dd-MMM-yyyy") + " " + batch.Time.ToString(timeFormatHms), (short)41)); //"To: Date Time "  '  
            }
            else
            {
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)305) + ": " + batch.Date.ToString("dd-MMM-yyyy") + "       " + _resourceManager.GetResString(offSet, (short)197) + ": " + batch.Time.ToString(timeFormatHms), (short)41)); //"Date: " ,"Time: "  '  
            }

            FileSystem.PrintLine(fileNumber);
            FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)3220) + " : " + Strings.Trim(System.Convert.ToString(cashoutId)), (short)41)); //CashOut # :
            FileSystem.PrintLine(fileNumber);

            FileSystem.PrintLine(fileNumber, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3221), (short)22) + modStringPad.PadR(_resourceManager.GetResString(offSet, (short)303), (short)7) + modStringPad.PadR(_resourceManager.GetResString(offSet, (short)106), (short)10));
            FileSystem.PrintLine(fileNumber, modStringPad.PadL("_", (short)41, "_"));

            strTmp = Strings.Replace(batch.Report, "  ", " ", 1);
            arrRpt = Strings.Split(strTmp, " ", -1, CompareMethod.Text);

            for (i = 0; i <= (arrRpt.Length - 1); i++)
            {
                switch (i % 4)
                {
                    case (short)0:
                        strTmp = "";
                        switch (arrRpt[i].ToUpper())
                        {
                            case "A": // -Activation
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3206), (short)21);
                                break;
                            case "F": // - Forced Redemption
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3207), (short)21);
                                break;
                            case "I": // -Increment"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3208), (short)21);
                                break;
                            case "J": // -Adjustment"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3209), (short)21);
                                break;
                            case "K": // - Cash Back"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3210), (short)21);
                                break;
                            case "R": // -Register"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3211), (short)21);
                                break;
                            case "S": // -Sale"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3212), (short)21);
                                break;
                            case "T": // - Balance Transfer to"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3213), (short)21);
                                break;
                            case "V": // - Redemption"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3214), (short)21);
                                break;
                            case "Z": // - Balance Transfer from"
                                strTmp = modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3215), (short)21);
                                break;
                        }
                        strTmp = strTmp + " ";
                        break;
                    case (short)1:
                        strTmp = strTmp + modStringPad.PadR(arrRpt[i], (short)6) + " ";
                        break;
                    case (short)2:
                        strTmp = strTmp + modStringPad.PadR(arrRpt[i], (short)10);
                        FileSystem.PrintLine(fileNumber, modStringPad.PadR(strTmp, (short)41));
                        break;
                }
            }
            FileSystem.PrintLine(fileNumber);
            FileSystem.PrintLine(fileNumber);
            FileSystem.PrintLine(fileNumber);
            FileSystem.FileClose(fileNumber);
            var stream = File.OpenRead(fileName);
            FileSystem.FileClose(fileNumber);
            return stream;
        }

        #region Private methods

        /// <summary>
        /// Method to search if sale is available for reprint
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="date">Date</param>
        /// <param name="reportName">Report name</param>
        /// <returns>True or false</returns>
        private bool IsSaleAvailable(int saleNumber, DateTime? date, string reportName)
        {
            ErrorMessage error;
            var sales = GetReprintSales(reportName, date, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                return false;
            if (sales.PayAtPumpSales != null)
                return sales.PayAtPumpSales.Any(s => s.SaleNumber == saleNumber);
            if (sales.PayInsideSales != null)
                return sales.PayInsideSales.Any(s => s.SaleNumber == saleNumber);
            if (sales.PaymentSales != null)
                return sales.PaymentSales.Any(s => s.SaleNumber == saleNumber);
            return false;
        }

        /// <summary>
        /// Method to reprint payment reports
        /// </summary>
        /// <param name="paymentType">Payment type</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="error">Error</param>
        /// <returns>Reports</returns>
        private List<Report> ReprintPaymentReport(char paymentType, int saleNumber,
            DateTime saleDate, out ErrorMessage error)
        {

            error = new ErrorMessage();
            var reports = new List<Report>();
            AR_Payment arRep = null;
            Payment pmtRep = null;
            Payout poRep = null;
            BR_Payment brRep = null;
            switch (paymentType)
            {
                case 'A':
                    arRep = new AR_Payment();
                    break;
                case 'F':
                    pmtRep = new Payment();
                    break;
                case 'P':
                    poRep = new Payout();
                    break;
                case 'B':
                    brRep = new BR_Payment();
                    break;
            }

            Sale sale;
            var db = _reportService.GetSale(saleNumber, out sale);
            if (db == DataSource.CSCMaster)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                MessageType tempVbStyle = (int)MessageType.Information + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 28, (short)92, saleNumber, tempVbStyle);
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }
            Tenders tend = new Tenders();
            var tenders = _reportService.GetTenders(saleNumber, db);
            foreach (var tender in tenders)
            {
                tend.Add(tender.Tender_Name,
                    tender.Tender_Class,
                    tender.Exchange_Rate,
                    true, true, true, (short)1, "A", false, 99, 99, 99, true, Convert.ToDouble(false),
                    tender.PrintCopies, true, false, tender.Image, tender.Tender_Name + tender.Sequence_Number);
                tend[tender.Tender_Name + tender.Sequence_Number].Amount_Entered = Convert.ToDecimal(tender.Amount_Entered);
                tend[tender.Tender_Name + tender.Sequence_Number].Amount_Used = Convert.ToDecimal(tender.Amount_Used);
            }

            // Get the primary (Header) attributes of the sale.
            switch (paymentType)
            {
                case 'A':
                    arRep.Sale_Num = saleNumber;

                    arRep.Amount = sale.Sale_Payment;

                    arRep.Customer.Code = sale.Customer.Code;
                    arRep.Customer = _customerManager.LoadCustomer(arRep.Customer.Code);
                    break;
                case 'F':

                    pmtRep.Amount = sale.Sale_Payment;
                    var cardNumber = _reportService.GetCardNumber(sale.Sale_Num);
                    if (!string.IsNullOrEmpty(cardNumber))
                    {
                        var cardRenamed = new Credit_Card();
                        _creditCardManager.SetCardnumber(ref cardRenamed, Convert.ToString(_encryptManager.Decrypt(cardNumber)));
                        cardRenamed.Expiry_Date = DateAndTime.Month(DateTime.Now) + System.Convert.ToString(DateAndTime.Year(DateTime.Now));
                        pmtRep.Card = cardRenamed;
                    }
                    break;
                case 'P':
                    poRep.Sale_Num = sale.Sale_Num;
                    poRep.Gross = sale.PointsAmount;
                    poRep.Return_Reason = _reasonService.GetReturnReason(sale.Return_Reason.Reason, Convert.ToChar(sale.Return_Reason.RType));
                    var vendorCode = _reportService.GetVendorCode(saleNumber, db);
                    if (!string.IsNullOrEmpty(vendorCode))
                    {
                        poRep.Vendor = _stockService.GetVendorByCode(vendorCode);
                    }
                    var saleTaxes = _reportService.GetSaleTaxes(saleNumber, db);
                    var taxes = _taxService.GetAllTaxes();

                    foreach (var tax in taxes)
                    {
                        poRep.Payout_Taxes.Add(tax.TaxName, tax.TaxDescription, 0, tax.Active.Value, tax.TaxName); //  - added taxactive for HST change -Me.Add rs!Tax_Name, rs!Tax_Desc, 0, rs!Tax_Name
                    }
                    foreach (var saleTax in saleTaxes)
                    {
                        poRep.Payout_Taxes[saleTax.Tax_Name].Tax_Amount = saleTax.Tax_Included_Amount;
                    }
                    break;
                case 'B':
                    brRep = _reportService.GetBottleReturn(saleNumber);
                    break;
            }
            var SaleDate = sale.Sale_Date;
            var saleTime = sale.Sale_Time;
            var registerNum = sale.Register;
            var tillNum = sale.TillNumber;
            var tillShift = sale.Shift;
            string userName = string.Empty;

            var user = _loginManager.GetExistingUser(sale.Sale_Client);

            if (_policyManager.PRN_UName)
            {
                userName = user.Name;
            }
            else
            {
                userName = user.Code;
            }

            tend.Tend_Totals.Change = sale.Sale_Change;

            var reprintCards = new Reprint_Cards();
            Card_Reprint cardReprint = default(Card_Reprint);
            var cardTenders = _reportService.GetCardTenders(saleNumber, db);
            foreach (var cardTender in cardTenders)
            {
                cardReprint = new Card_Reprint();
                cardReprint.Card_Type = cardTender.CardType;
                cardReprint.CardNumber = System.Convert.ToString(_encryptManager.Decrypt(cardTender.CardNum));
                cardReprint.Name = cardTender.CardName;
                cardReprint.Expiry_Month = Strings.Right(cardTender.ExpiryDate, 2);
                cardReprint.Expiry_Year = Strings.Left(cardTender.ExpiryDate, 2);
                cardReprint.Card_Swiped = cardTender.Swiped;
                cardReprint.Authorization_Number = cardTender.ApprovalCode;
                cardReprint.Print_Signature = true;
                cardReprint.Language = cardTender.Language;
                cardReprint.Expiry_Date = cardTender.ExpiryDate;
                cardReprint.Customer_Name = cardTender.CustomerName;
                cardReprint.TerminalID = cardTender.TerminalID;
                cardReprint.Trans_Date = cardTender.TransactionDate;
                cardReprint.Trans_Time = cardTender.TransactionTime;
                cardReprint.Trans_Amount = Convert.ToSingle(cardTender.Amount);
                cardReprint.Trans_Number = cardTender.SaleNumber.ToString();
                cardReprint.ResponseCode = cardTender.ResponseCode;
                cardReprint.ApprovalCode = cardTender.ISOCode;
                cardReprint.Sequence_Number = cardTender.SequenceNumber;
                cardReprint.DebitAccount = cardTender.DebitAccount;
                cardReprint.Receipt_Display = cardTender.ReceiptDisplay;
                cardReprint.Vechicle_Number = cardTender.VechicleNo;
                cardReprint.Driver_Number = cardTender.DriverNo;
                cardReprint.ID_Number = cardTender.IdentificationNo;
                cardReprint.Odometer_Number = cardTender.Odometer;
                cardReprint.UsageType = cardTender.CardUsage;
                cardReprint.PrintDriver = cardTender.PrintDriverNo;
                cardReprint.PrintIdentification = cardTender.PrintIdentificationNo;
                cardReprint.PrintOdometer = cardTender.PrintOdometer;
                cardReprint.PrintUsage = cardTender.PrintUsage;
                cardReprint.PrintVechicle = cardTender.PrintVechicleNo;
                cardReprint.Store_Forward = cardTender.StoreForward;
                cardReprint.Balance = cardTender.Balance;
                cardReprint.Result = cardTender.Result;
                cardReprint.Message = cardTender.Message;
                reprintCards.Add(cardReprint, cardReprint.ID_Number);
            }

            if (reprintCards != null)
            {

                foreach (Card_Reprint tempLoopVarCardReprintRenamed in reprintCards)
                {
                    cardReprint = tempLoopVarCardReprintRenamed;


                    if (cardReprint.Card_Type == "G")
                    {
                        var gxReceipt = new GiveXReceiptType();
                        gxReceipt.Date = cardReprint.Trans_Date.ToString("MM/dd/yyyy");
                        gxReceipt.Time = cardReprint.Trans_Time.ToString("hh:mm:ss");

                        gxReceipt.UserID = System.Convert.ToString(sale.Sale_Client);
                        gxReceipt.TranType = (short)(Conversion.Val(cardReprint.Result));
                        gxReceipt.SeqNum = cardReprint.Trans_Number;
                        gxReceipt.CardNum = cardReprint.CardNumber;
                        gxReceipt.ExpDate = cardReprint.Expiry_Date;
                        gxReceipt.Balance = (float)cardReprint.Balance;
                        if (gxReceipt.TranType == 3)
                        {
                            gxReceipt.SaleAmount = 0 - cardReprint.Trans_Amount;
                        }
                        else
                        {
                            gxReceipt.SaleAmount = cardReprint.Trans_Amount;
                        }
                        gxReceipt.ResponseCode = cardReprint.Receipt_Display;
                        var givexReport = Print_GiveX_Receipt(gxReceipt, false, true, (short)1, true);
                        reports.Add(givexReport);
                    }
                    else
                    {
                        if (cardReprint.Card_Type != "F")
                        {

                            if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "TD")
                            {
                                if (cardReprint.Language == "French")
                                {

                                    var transactionFile = RePrintTransRecordFrench(cardReprint, System.Convert.ToBoolean(_policyManager.MASK_CARDNO), false, true, false);
                                }
                                else
                                {
                                    var transactionFile = RePrintTransRecordEnglish(cardReprint, System.Convert.ToBoolean(_policyManager.MASK_CARDNO), false, true, false);
                                }
                            }
                        }
                    }
                }
            }
            var till = _tillService.GetTill(sale.TillNumber);
            Report paymentReport = null;
            switch (paymentType)
            {
                case 'A':
                    paymentReport = Print_ARPay(arRep, sale.Sale_Client, till, tend, false, true, SaleDate, saleTime, reprintCards);
                    break;
                case 'F':
                    paymentReport = Print_Payment(till, pmtRep, tend, user, SaleDate, saleTime, true, reprintCards);
                    break;
                case 'P':
                    paymentReport = Print_Payout(poRep, sale.Sale_Client, userName, SaleDate, saleTime, registerNum, till, true);
                    break;
                case 'B':
                    paymentReport = Print_BottleReturn(brRep, sale.Sale_Client, SaleDate, saleTime, registerNum, tillNum, tillShift, true);
                    break;
            }

            reports.Add(paymentReport);
            return reports;
        }

        /// <summary>
        /// Method to get close batch sale
        /// </summary>
        /// <param name="error">Error</param>
        /// <returns>Reprint sale</returns>
        private ReprintSale GetCloseBatchSale(out ErrorMessage error)
        {
            error = new ErrorMessage();
            var reprintSale = new ReprintSale();
            reprintSale.IsCloseBatchSale = true;
            reprintSale.CloseBatchSales = new List<CloseBatchSale>();
            var terminalIds = _reportService.GetTerminalIds(PosId);
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            var dt = DateTime.FromOADate(DateAndTime.Today.ToOADate() - _policyManager.CLOSEB_DAYS);
            var closeBatchSales = _reportService.GetCloseBatchReports(terminalIds, dt);
            if (closeBatchSales.Count != 0)
            {
                foreach (var closeBatchSale in closeBatchSales)
                {
                    reprintSale.CloseBatchSales.Add(new CloseBatchSale
                    {
                        BatchNumber = closeBatchSale.BatchNumber,
                        TerminalId = closeBatchSale.TerminalId,
                        Date = closeBatchSale.Date.ToString("MM/dd/yyyy"),
                        Time = closeBatchSale.Time.ToString(timeFormatHm),
                        Report = Convert.ToBase64String(Encoding.UTF8.GetBytes(closeBatchSale.Report))
                    });
                }
            }
            else
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                MessageType tempVbStyle = (int)MessageType.Information + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)3791, null, tempVbStyle);
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
            }

            return reprintSale;
        }

        /// <summary>
        /// Method to get pay at pump sales
        /// </summary>
        /// <param name="rePrintType">Reprint type</param>
        /// <param name="date">Date</param>
        /// <returns>Reprint sale</returns>
        private ReprintSale GetPayAtPumpSales(char rePrintType, DateTime date)
        {
            string strSql = "SELECT SaleHead.Sale_No As [Sale Number], SaleLine.Quantity AS [Volume], " + "SaleHead.Sale_Amt AS [Amount], SaleLine.PumpID AS [Pump], SaleLine.GradeID AS [Grade], " + "SaleHead.Sale_Date AS [Date], SaleHead.Sale_Time AS [Time]  FROM SaleHead , SaleLine WHERE SaleHead.Sale_No=SaleLine.Sale_No AND " + "(SaleHead.T_Type = \'PATP_APP\' OR SaleHead.T_Type = \'PATP_C\')  AND SaleHead.TILL = " + Chaps_Main.PayAtPumpTill + " AND   SaleHead.Sale_Date=\'" + date.ToString("yyyyMMdd") + "\' " + " and saleline.pumpid > 0 ORDER BY SaleHead.Sale_No DESC";
            var reprintSale = new ReprintSale();
            reprintSale.IsPayAtPumpSale = true;
            DataSource dataSource = DataSource.CSCTills;
            switch (rePrintType)
            {
                case 'C':
                    {
                        dataSource = DataSource.CSCTills; break;
                    }
                case 'H':
                    {
                        dataSource = DataSource.CSCTrans; break;
                    }
            }
            var sales = _reportService.GetPayAtPumpSales(strSql, dataSource);
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            reprintSale.PayAtPumpSales = new List<Entities.PayAtPumpSale>();
            foreach (var sale in sales)
            {
                reprintSale.PayAtPumpSales.Add(new Entities.PayAtPumpSale
                {
                    SaleNumber = sale.SaleNumber,
                    Volume = sale.Volume.ToString("#,##0.000"),
                    Amount = sale.SaleAmount.ToString("#,##0.00"),
                    Pump = sale.PumpId.ToString(),
                    Grade = sale.GradeId.ToString(),
                    Date = sale.SaleDate.ToString("MM/dd/yyyy"),
                    Time = sale.SaleTime.ToString(timeFormatHm),
                });
            }
            return reprintSale;
        }

        /// <summary>
        /// Method to get pay inside sales
        /// </summary>
        /// <param name="rePrintType">Reprint type</param>
        /// <param name="date">Date</param>
        /// <returns>Reprint sale</returns>
        private ReprintSale GetPayInsideSales(char rePrintType, DateTime date)
        {
            var reprintSale = new ReprintSale();
            reprintSale.IsPayInsideSale = true;
            string query = string.Empty;
            DataSource dataSource = DataSource.CSCTills;
            switch (rePrintType)
            {
                case 'C':
                    {
                        query = "Select SaleHead.Sale_No as [Sale], " + "       SaleHead.Sale_Date as [Sold On], " + "       SaleHead.Sale_Time as [Time], " + "       SaleHead.Sale_Amt as [Amount], " + "       SaleHead.Client as [Customer]  FROM   SaleHead  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'RUNAWAY\', \'PUMPTEST\') AND " + "       SaleHead.Till <> " + Chaps_Main.PayAtPumpTill + " " + "ORDER BY SaleHead.Sale_No DESC ";
                        dataSource = DataSource.CSCTills; break;
                    }
                case 'H':
                    {
                        query = "Select SaleHead.Sale_No as [Sale], " + "       SaleHead.Sale_Date as [Sold On], " + "       SaleHead.Sale_Time as [Time], " + "       SaleHead.Sale_Amt as [Amount], " + "       SaleHead.Client as [Customer]  FROM   SaleHead  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'RUNAWAY\', \'PUMPTEST\') AND " + "       SaleHead.Till <> " + Chaps_Main.PayAtPumpTill + " " + " AND   SaleHead.Sale_Date=\'" + date.ToString("yyyyMMdd") + "\' " + "ORDER BY SaleHead.Sale_No DESC ";
                        dataSource = DataSource.CSCTrans; break;
                    }
            }
            var sales = _reportService.GetPayInsideSales(query, dataSource);
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            reprintSale.PayInsideSales = new List<PayInsideSale>();
            foreach (var sale in sales)
            {
                reprintSale.PayInsideSales.Add(new PayInsideSale
                {
                    SaleNumber = sale.Sale_Num,
                    SoldOn = sale.Sale_Date.ToString("MM/dd/yyyy"),
                    Time = sale.Sale_Time.ToString(timeFormatHm),
                    Amount = sale.Sale_Amount.ToString("#,##0.00"),
                    Customer = sale.Sale_Client
                });
            }
            return reprintSale;
        }

        /// <summary>
        /// Method to get paymnet sales list
        /// </summary>
        /// <param name="paymentType">Payment type</param>
        /// <param name="date">Date</param>
        /// <returns>Reprint sale</returns>
        private ReprintSale GetPaymentSales(char paymentType, DateTime date)
        {
            var reprintSale = new ReprintSale();
            reprintSale.IsPaymentSale = true;
            string strSaleType = "";
            string strSelect = "Select Sale_No as [Sale], " + "       Sale_Date as [Sold On], " + "       Sale_Time as [Time], " + "       PAYMENT as [Amount] ";
            switch (paymentType)
            {
                case 'A':
                    strSaleType = "ARPAY";
                    break;
                case 'F':
                    strSaleType = "PAYMENT";
                    break;
                case 'P':
                    strSaleType = "PAYOUT";
                    strSelect = "Select Sale_No as [Sale], " + "       Sale_Date as [Sold On], " + "       Sale_Time as [Time], " + "       Sale_Amt as [Amount] ";
                    break;
                case 'B':
                    strSaleType = "BTL RTN";
                    strSelect = "Select Sale_No as [Sale], " + "       Sale_Date as [Sold On], " + "       Sale_Time as [Time], " + "       (-1) * Sale_Amt as [Amount] ";
                    break;
            }
            if (string.IsNullOrEmpty(strSaleType))
            {
                return null;
            }
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            reprintSale.PaymentSales = new List<PaymentSale>();
            var sales = _reportService.GetPaymentSales(strSelect, strSaleType, date);
            foreach (var sale in sales)
            {
                reprintSale.PaymentSales.Add(new PaymentSale
                {
                    SaleNumber = sale.Sale_Num,
                    SoldOn = sale.Sale_Date.ToString("MM/dd/yyyy"),
                    Time = sale.Sale_Time.ToString(timeFormatHm),
                    Amount = sale.Sale_Amount.ToString("#,##0.00")
                });
            }
            return reprintSale;
        }

        /// <summary>
        /// Method to get givex masked number
        /// </summary>
        /// <param name="cardNum">Card number</param>
        /// <returns>Masked number</returns>
        private string GetGiveXMaskNum(string cardNum)
        {
            string returnValue = "";
            string strNum = "";

            if (cardNum.Length <= 6)
            {
                strNum = cardNum;
            }
            else if (cardNum.Length <= 12)
            {
                strNum = cardNum.Substring(0, 6) + new string('*', cardNum.Length - 6);
            }
            else
            {
                strNum = cardNum.Substring(0, 6) + new string('*', 5) + cardNum.Substring(11, cardNum.Length - 12) + "*";
            }
            returnValue = strNum;
            return returnValue;
        }

        /// <summary>
        /// Method to generate four digit random number
        /// </summary>
        /// <returns>True or false</returns>
        private int GenerateRandomNo()
        {
            int min = 1000;
            int max = 9999;
            Random rdm = new Random();
            return rdm.Next(min, max);
        }

        /// <summary>
        /// Method to mask Aite number
        /// </summary>
        /// <param name="strNumber">aite number</param>
        /// <returns>Masked number</returns>
        private string MaskedAiteNumber(string strNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,MaskedAiteNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string returnValue = "";

            string strTmp = "";

            if (strNumber.Length <= 4)
            {
                returnValue = strNumber;
                return returnValue;
            }

            strTmp = strNumber.Substring(0, 2) + new string('*', strNumber.Length - 4) + strNumber.Substring(strNumber.Length - 2, 2);
            returnValue = strTmp;
            Performancelog.Debug($"End,ReceiptManager,MaskedAiteNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to get tax symbol
        /// </summary>
        /// <param name="oldSymbol">Old tax symbol</param>
        /// <param name="included">Tax included or not</param>
        /// <returns>Tax symbol</returns>
        private string Get_Tax_Symbol(string oldSymbol, bool included)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Get_Tax_Symbol,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string returnValue = "";

            string newSymbol = "";
            newSymbol = Convert.ToString(included ? "I" : "T");
            if (oldSymbol == "E" || oldSymbol == newSymbol)
            {
                returnValue = newSymbol;
            }
            else
            {
                returnValue = "M";
            }
            Performancelog.Debug($"End,ReceiptManager,Get_Tax_Symbol,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to print fuel loyalty
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="nH"></param>
        /// <param name="reprint"></param>
        /// <param name="completePrepay"></param>
        /// <param name="runAway"></param>
        /// <param name="hasPrepay"></param>
        /// <param name="pumpTest"></param>
        private void Print_FuelLoyalty(Sale sale, int nH, bool reprint, bool completePrepay = false, bool runAway = false, bool hasPrepay = false, bool pumpTest = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Print_FuelLoyalty,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var store = _policyManager.LoadStoreInfo();
            var loyalName = _policyManager.LOYAL_NAME;
            var offSet = store.OffSet;
            short hWidth = 0;
            string just = "";

            hWidth = (short)40;
            just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper();

            if (_policyManager.PRT_CPN && sale.Customer.DiscountType == "C")
            {
                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }

                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH,
                    sale.Sale_Totals.Gross >= 0
                        ? modStringPad.PadIt(just,
                            _resourceManager.GetResString(offSet, (short)229).ToUpper() + Convert.ToString(sale.Sale_Num),
                            hWidth)
                        : modStringPad.PadIt(just,
                            _resourceManager.GetResString(offSet, (short)227).ToUpper() + Convert.ToString(sale.Sale_Num),
                            hWidth));

                if (_policyManager.USE_CUST)
                {
                    if (!string.IsNullOrEmpty(sale.Customer.Name) && sale.Customer.Name != _resourceManager.GetResString(offSet, (short)400))
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)110) + ": " + sale.Customer.Code + " - " + sale.Customer.Name, hWidth));
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, (short)110) + ": " + _resourceManager.GetResString(offSet, (short)400), hWidth)); //Cash Sale Customer
                    }
                }

                if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + _resourceManager.GetResString(offSet, (short)231), hWidth)); //" Member(Active)"
                }
                else if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "F")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + _resourceManager.GetResString(offSet, (short)233), hWidth)); //" Member(Frozen)"
                }
                else if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "I")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(loyalName + _resourceManager.GetResString(offSet, (short)232), hWidth)); //" Member(Inactive)"
                }
            }


            if (sale.CouponTotal > 0)
            {
                if ((!runAway) && (!pumpTest) && ((!hasPrepay) || (hasPrepay && reprint) || completePrepay))
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8369), hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadC(sale.CouponID, hWidth));
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8370), hWidth));
                    FileSystem.PrintLine(nH, modStringPad.PadC((sale.CouponTotal).ToString(), hWidth));
                    FileSystem.PrintLine(nH);
                }
            }
            if ((!runAway) && (!pumpTest) && sale.Sale_Totals.Gross != 0)
            {
                if ((hasPrepay && sale.Customer.DiscountType != "C") || (hasPrepay && reprint) || completePrepay)
                {
                    FileSystem.PrintLine(nH, sale.Customer.Footer);
                    FileSystem.PrintLine(nH);
                }
            }
            Performancelog.Debug($"End,ReceiptManager,Print_FuelLoyalty,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to print card profile prompts
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="saleNo">Sale number</param>
        /// <param name="tillNo">Till no</param>
        /// <param name="nH">File number</param>
        private void PrintCardProfilePrompts(string cardNumber, int saleNo, byte tillNo, object nH)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintCardProfilePrompts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var cardSales = _reportService.GetCardProfilePrompts(saleNo, cardNumber, tillNo);
            foreach (var cardSale in cardSales)
            {
                FileSystem.PrintLine(Convert.ToInt32(nH), modStringPad.PadR(Strings.Trim(cardSale.DisplayText), (short)20) + modStringPad.PadL(cardSale.PromptAnswer, (short)20));
            }
            Performancelog.Debug($"End,ReceiptManager,PrintCardProfilePrompts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Method to print credit card
        /// </summary>
        /// <param name="cCard">Credit card</param>
        /// <param name="maskCard">Mask card or not</param>
        /// <param name="nH">File number</param>
        private void Print_Credit_Card(Credit_Card cCard, bool maskCard, short nH)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Print_Credit_Card,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            string ccl;
            string ccr;
            short ccc;

            short i = 0;

            FileSystem.PrintLine(nH);
            if (!string.IsNullOrEmpty(cCard.Customer_Name))
            {
                FileSystem.PrintLine(nH, cCard.Customer_Name);
            }

            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)501)); // "Tendered Card(s)"

            if (cCard.TendCode != "ACK")
            {
                FileSystem.PrintLine(nH, modStringPad.PadR(cCard.Name, (short)20) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)253) + cCard.Authorization_Number, (short)20)); //"Auth: "
            }

            if (maskCard)
            {

                if (cCard.StoreAndForward && _policyManager.BankSystem == "Moneris")
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, GetMaskedCardNum(cCard.Cardnumber, cCard.Crd_Type) + modStringPad.PadL("(" + _resourceManager.GetResString(offSet, (short)254) + ")", (short)11)); //Keyed
                }
                else
                {
                    if (cCard.TendCode != "ACK")
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, GetMaskedCardNum(cCard.Cardnumber, cCard.Crd_Type) + modStringPad.PadL(Convert.ToString(cCard.Card_Swiped ? ("(" + _resourceManager.GetResString(offSet, (short)255) + ")") : ("(" + _resourceManager.GetResString(offSet, (short)254) + ")")), (short)11)); //Swiped,Keyed"
                    }
                    else
                    {
                        FileSystem.PrintLine(nH);
                        if (cCard.TendCode == "ACK")
                        {
                            if (cCard.Trans_Number != "")
                            {
                                var cards = _reportService.GetListOfGiftcards(int.Parse(cCard.Trans_Number), false);
                                foreach (var card in cards)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString(card.TenderName), (short)30));
                                    FileSystem.PrintLine(nH, modStringPad.PadL(GetMaskedCardNum(Convert.ToString(card.CardNumber), "L"), (short)30) + modStringPad.PadL(card.Amount.ToString("#0.00"), (short)10));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (cCard.StoreAndForward && _policyManager.BankSystem == "Moneris")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(cCard.Cardnumber, (short)30) + modStringPad.PadL("(" + _resourceManager.GetResString(offSet, (short)254) + ")", (short)10));
                }
                else
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(cCard.Cardnumber, (short)30) + modStringPad.PadL(Convert.ToString(cCard.Card_Swiped ? ("(" + _resourceManager.GetResString(offSet, (short)255) + ")") : ("(" + _resourceManager.GetResString(offSet, (short)254) + ")")), (short)11));
                }
            }

            if (cCard.Crd_Type != "D")
            {
                if (_policyManager.BankSystem != "Moneris")
                {
                    if (cCard.CheckExpiryDate)
                    {
                        FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)256) + ": " + "**/**"); //Expires
                    }
                }
            }
            if (!_policyManager.USE_PINPAD) // donot print the information entered through a pin pad/pump- print only the one entered throug pos
            {
                if (cCard.Print_VechicleNo)
                {
                    if (!string.IsNullOrEmpty(cCard.Vechicle_Number))
                    {
                        FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)352) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cCard.Vechicle_Number.Trim()).Substring((Strings.Space(20) + cCard.Vechicle_Number.Trim()).Length - 20, 20));
                    }
                }

                if (cCard.Print_DriverNo)
                {
                    if (!string.IsNullOrEmpty(cCard.Driver_Number))
                    {
                        FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)355) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cCard.Driver_Number.Trim()).Substring((Strings.Space(20) + cCard.Driver_Number.Trim()).Length - 20, 20));
                    }
                }
                if (cCard.Print_IdentificationNo)
                {
                    if (!string.IsNullOrEmpty(cCard.ID_Number))
                    {
                        FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)354) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cCard.ID_Number.Trim()).Substring((Strings.Space(20) + cCard.ID_Number.Trim()).Length - 20, 20));
                    }
                }
            }
            if (cCard.Print_Odometer)
            {
                if (!string.IsNullOrEmpty(cCard.Odometer_Number))
                {
                    FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)353) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cCard.Odometer_Number.Trim()).Substring((Strings.Space(20) + cCard.Odometer_Number.Trim()).Length - 20, 20));
                }
            }
            if (cCard.Print_Usage)
            {
                if (!string.IsNullOrEmpty(cCard.usageType))
                {
                    FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)356) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cCard.usageType.Trim()).Substring((Strings.Space(20) + cCard.usageType.Trim()).Length - 20, 20));
                }
            }

            Performancelog.Debug($"End,ReceiptManager,Print_Credit_Card,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to reprint credit card
        /// </summary>
        /// <param name="till"></param>
        /// <param name="reprintCards"></param>
        /// <param name="maskCard"></param>
        /// <param name="nH"></param>
        /// <param name="sale"></param>
        private void RePrint_Credit_Card(Till till, Reprint_Cards reprintCards, bool maskCard, short nH, Sale sale = null)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,RePrint_Credit_Card,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string ccl;
            string ccr;
            short ccc;
            Card_Reprint cardReprintRenamed = default(Card_Reprint);
            short i;

            double dPointEarned = 0;
            double dBalance = 0;
            double dDeduct;
            string sCardNumber = "";
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)501)); // "Tendered Card(s)"
            foreach (Card_Reprint tempLoopVarCardReprintRenamed in reprintCards)
            {
                cardReprintRenamed = tempLoopVarCardReprintRenamed;

                if ((_policyManager.EMVVersion == false)
                    || (_policyManager.EMVVersion == true
                    && cardReprintRenamed.Card_Type != "D"
                    && cardReprintRenamed.Card_Type != "C"))
                {
                    FileSystem.PrintLine(nH);
                    if (!string.IsNullOrEmpty(cardReprintRenamed.Customer_Name))
                    {
                        FileSystem.PrintLine(nH, cardReprintRenamed.Customer_Name);
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadR(cardReprintRenamed.Name, (short)30));

                    if (maskCard)
                    {

                        if (cardReprintRenamed.Store_Forward && _policyManager.BankSystem == "Moneris")
                        {
                            FileSystem.PrintLine(nH, GetMaskedCardNum(cardReprintRenamed.CardNumber, cardReprintRenamed.Card_Type) + modStringPad.PadL("(" + _resourceManager.GetResString(offSet, (short)254) + ")", (short)11)); //Keyed
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadL(GetMaskedCardNum(cardReprintRenamed.CardNumber, cardReprintRenamed.Card_Type), (short)30) + modStringPad.PadL(cardReprintRenamed.Trans_Amount.ToString("#0.00"), (short)10));
                        }
                    }
                    else
                    {
                        if (cardReprintRenamed.Store_Forward && _policyManager.BankSystem == "Moneris")
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(cardReprintRenamed.CardNumber, (short)30) + modStringPad.PadL("(" + _resourceManager.GetResString(offSet, (short)254) + ")", (short)10)); //Keyed
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(cardReprintRenamed.CardNumber, (short)30) + modStringPad.PadL(Convert.ToString(cardReprintRenamed.Card_Swiped ? ("(" + _resourceManager.GetResString(offSet, (short)255) + ")") : ("(" + _resourceManager.GetResString(offSet, (short)254) + ")")), (short)11)); //Swiped,Keyed
                        }
                    }

                    if (_policyManager.BankSystem != "Moneris")
                    {

                    }
                    if (!(cardReprintRenamed == null))
                    {
                        if (_policyManager.RSTR_PROFILE) //And Card_Reprint.CardProfileID <> "") Then
                        {
                            if (!string.IsNullOrEmpty(cardReprintRenamed.PONumber))
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)472), (short)20) + modStringPad.PadL(cardReprintRenamed.PONumber, (short)20));
                            }
                            PrintCardProfilePrompts(cardReprintRenamed.CardNumber, (int)(Conversion.Val(cardReprintRenamed.Trans_Number)), cardReprintRenamed.TillNumber, nH);
                        }
                    }

                    if (cardReprintRenamed.PrintVechicle)
                    {
                        if (!string.IsNullOrEmpty(cardReprintRenamed.Vechicle_Number))
                        {
                            FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)352) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cardReprintRenamed.Vechicle_Number.Trim()).Substring((Strings.Space(20) + cardReprintRenamed.Vechicle_Number.Trim()).Length - 20, 20));
                        }
                    }

                    if (cardReprintRenamed.PrintDriver)
                    {
                        if (!string.IsNullOrEmpty(cardReprintRenamed.Driver_Number))
                        {
                            FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)355) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cardReprintRenamed.Driver_Number.Trim()).Substring((Strings.Space(20) + cardReprintRenamed.Driver_Number.Trim()).Length - 20, 20));
                        }
                    }
                    if (cardReprintRenamed.PrintIdentification)
                    {
                        if (!string.IsNullOrEmpty(cardReprintRenamed.ID_Number))
                        {
                            FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)354) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cardReprintRenamed.ID_Number.Trim()).Substring((Strings.Space(20) + cardReprintRenamed.ID_Number.Trim()).Length - 20, 20));
                        }
                    }
                    if (cardReprintRenamed.PrintOdometer)
                    {
                        if (!string.IsNullOrEmpty(cardReprintRenamed.Odometer_Number))
                        {
                            FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)353) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cardReprintRenamed.Odometer_Number.Trim()).Substring((Strings.Space(20) + cardReprintRenamed.Odometer_Number.Trim()).Length - 20, 20));
                        }
                    }
                    if (cardReprintRenamed.PrintUsage)
                    {
                        if (!string.IsNullOrEmpty(cardReprintRenamed.UsageType))
                        {
                            FileSystem.PrintLine(nH, (_resourceManager.GetResString(offSet, (short)356) + ": " + Strings.Space(20)).Substring(0, 20) + (Strings.Space(20) + cardReprintRenamed.UsageType.Trim()).Substring((Strings.Space(20) + cardReprintRenamed.UsageType.Trim()).Length - 20, 20));
                        }
                    }
                } //EMV-endif only
            }

            if (_policyManager.REWARDS_Enabled)
            {
                if (sale != null)
                {
                    sCardNumber = sale?.SaleHead?.LoyaltyCard;
                    if (!string.IsNullOrEmpty(sCardNumber))
                    {
                        dPointEarned = Convert.ToDouble(sale?.SaleHead?.LoyalPoint);
                        dBalance = Convert.ToDouble(sale?.SaleHead?.LoyaltyBalance);

                        if (!string.IsNullOrEmpty(sCardNumber))
                        {
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, modStringPad.PadC("-", (short)40, "-"));
                            FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)502), (short)22) + modStringPad.PadL(GetMaskedCardNum(sCardNumber, "L"), (short)18));
                            FileSystem.PrintLine(nH, modStringPad.PadL(_policyManager.REWARDS_Caption + ": ", (short)22) + modStringPad.PadL(dPointEarned.ToString("#0.00"), (short)18));
                            FileSystem.PrintLine(nH, modStringPad.PadL(_resourceManager.GetResString(offSet, (short)503), (short)22) + modStringPad.PadL(dBalance.ToString("#0.00"), (short)18));
                            FileSystem.PrintLine(nH, modStringPad.PadC("=", (short)40, "="));
                        }
                    }
                }
            }
            Chaps_Main.Reprint_Cards = null;
            Performancelog.Debug($"End,ReceiptManager,RePrint_Credit_Card,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to collect till audit data
        /// </summary>
        /// <param name="auditTill"></param>
        private void Collect_Data(ref Till auditTill, string cbt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Collect_Data,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            auditTill.Change = _reportService.GetChangeValue(auditTill.Number);
            auditTill.OverPayment = 0;
            var cashDraws = _reportService.GetCashDraws(auditTill.Number);
            auditTill.Draws = cashDraws.Draws;
            auditTill.BonusDraw = cashDraws.BonusDraw;
            auditTill.Payouts = _reportService.GetPayouts(auditTill.Number);
            if (_policyManager.CashBonus && !string.IsNullOrEmpty(cbt))
            {
                //Bonus giveaways
                auditTill.BonusGiveAway = _reportService.GetBonusGiveAway(auditTill.Number);

            }
            //shiny end
            Performancelog.Debug($"End,ReceiptManager,Collect_Data,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to make report for till
        /// </summary>
        /// <param name="strFileName">File name</param>
        /// <param name="auditTill">Till</param>
        /// <param name="cbt">Cash bonus tender</param>
        /// <returns>Streamm</returns>
        private FileStream Make_Report(string strFileName, Till auditTill, string cbt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Make_Report,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short intFile = 0;

            string strRenamed = "";
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            intFile = (short)(FileSystem.FreeFile());
            decimal curCashBonus = 0;
            try
            {
                FileSystem.FileOpen(intFile, strFileName, OpenMode.Output);


                FileSystem.PrintLine(intFile, modStringPad.PadC(store.Code + "  " + store.Name, (short)40)); //  
                FileSystem.PrintLine(intFile);
                strRenamed = _resourceManager.GetResString(offSet, (short)8136) + " " + Convert.ToString(auditTill.Number);
                if (strRenamed.Length > 40)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadC(Strings.Left(strRenamed, 27), (short)40)); //"Till Audit for till number "
                    FileSystem.PrintLine(intFile, modStringPad.PadC(strRenamed.Substring(27), (short)40)); //"Till Audit for till number "
                }
                else
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadC(strRenamed, (short)40)); //"Till Audit for till number "
                }
                FileSystem.PrintLine(intFile, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)217) + auditTill.Date_Open.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + (auditTill.Time_Open.ToString(timeFormatHms)), (short)40));
                FileSystem.PrintLine(intFile, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)199) + " " + DateTime.Now.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + (DateTime.Now.ToString(timeFormatHms)), (short)40));
                FileSystem.PrintLine(intFile, modStringPad.PadC(new string('_', 40), (short)40));
                FileSystem.PrintLine(intFile);
                FileSystem.PrintLine(intFile, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3464) + ":", (short)27) + modStringPad.PadL((NetSaleTotal(auditTill.Number).ToString("#,##0.00")), (short)13));
                FileSystem.PrintLine(intFile);
                FileSystem.PrintLine(intFile, _resourceManager.GetResString(offSet, (short)3465) + ":");
                FileSystem.PrintLine(intFile);
                FileSystem.PrintLine(intFile, _policyManager.BASECURR + ":"); //"Cash:"

                if (auditTill.Float != 0)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadR(" + " + _resourceManager.GetResString(offSet, (short)221) + ": ", (short)27) + modStringPad.PadL((auditTill.Float.ToString("#,##0.00")), (short)13)); //Float
                }
                if (auditTill.Payouts != 0)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadR(" - " + _resourceManager.GetResString(offSet, (short)216) + ":", (short)27) + modStringPad.PadL((auditTill.Payouts.ToString("#,##0.00")), (short)13)); //Payouts
                }
                if (auditTill.Change != 0)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadR(" - " + _resourceManager.GetResString(offSet, (short)220) + ":", (short)27) + modStringPad.PadL((auditTill.Change.ToString("#,##0.00")), (short)13)); //Change
                }
                if (auditTill.Draws != 0)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadR(" + " + _resourceManager.GetResString(offSet, (short)218) + ": ", (short)27) + modStringPad.PadL((auditTill.Draws.ToString("#,##0.00")), (short)13)); //Draws
                }
                if (auditTill.OverPayment != 0)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadR(" + " + _resourceManager.GetResString(offSet, (short)280) + ": ", (short)27) + modStringPad.PadL((auditTill.OverPayment.ToString("#,##0.00")), (short)13)); // Over Payment
                }
                var curCash = auditTill.Float - auditTill.Payouts + auditTill.Change + auditTill.Draws + auditTill.OverPayment;


                var rsSales = _reportService.GetCashSaleValues(auditTill.Number);

                PrintLine(ref curCash, curCashBonus, cbt, intFile, rsSales, true, " + " + _resourceManager.GetResString(offSet, (short)3404) + ":", "ADD"); //Tendered

                var rsDrops = _reportService.GetDropValues(auditTill.Number);

                PrintLine(ref curCash, curCashBonus, cbt, intFile, rsDrops, true, " - " + _resourceManager.GetResString(offSet, (short)219) + ":", "SUB"); //Drops

                var rsPayments = _reportService.GetPaymentValues(auditTill.Number);
                PrintLine(ref curCash, curCashBonus, cbt, intFile, rsPayments, true, " + " + _resourceManager.GetResString(offSet, (short)215) + ":", "ADD"); //Payments

                var rsARpayments = _reportService.GetArPaymentValues(auditTill.Number);
                PrintLine(ref curCash, curCashBonus, cbt, intFile, rsARpayments, true, " + " + _resourceManager.GetResString(offSet, (short)345) + ":", "ADD"); //AR Payments

                var rsBottleReturn = _reportService.GetBottleReturnValues(auditTill.Number);
                PrintLine(ref curCash, curCashBonus, cbt, intFile, rsBottleReturn, true, " - " + _resourceManager.GetResString(offSet, (short)1248) + ":", "ADD");

                FileSystem.PrintLine(intFile);

                FileSystem.PrintLine(intFile, modStringPad.PadL("_________________", (short)40));
                FileSystem.PrintLine(intFile, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)224) + ":", (short)27) + modStringPad.PadL((curCash.ToString("#,##0.00")), (short)13)); //Current cash

                FileSystem.PrintLine(intFile);

                if (_policyManager.CashBonus && !string.IsNullOrEmpty(cbt))
                {
                    FileSystem.PrintLine(intFile, _policyManager.CBonusName + ":"); //"Cash Bonus:"

                    if (auditTill.BonusFloat != 0)
                    {
                        FileSystem.PrintLine(intFile, modStringPad.PadR(" + " + _resourceManager.GetResString(offSet, (short)432) + ": ", (short)27) + modStringPad.PadL((auditTill.BonusFloat.ToString("#,##0.00")), (short)13)); // Cash Bonus Float
                    }
                    if (auditTill.BonusDraw != 0)
                    {
                        FileSystem.PrintLine(intFile, modStringPad.PadR(" + " + _resourceManager.GetResString(offSet, (short)433) + ": ", (short)27) + modStringPad.PadL((auditTill.BonusDraw.ToString("#,##0.00")), (short)13)); //Cash bonus Draws
                    }

                    curCashBonus = auditTill.BonusFloat + auditTill.BonusDraw;

                    PrintLine(ref curCash, curCashBonus, cbt, intFile, rsSales, false, " + " + _resourceManager.GetResString(offSet, (short)3404) + ":", "ADD"); //Tendered

                    PrintLine(ref curCash, curCashBonus, cbt, intFile, rsDrops, false, " - " + _resourceManager.GetResString(offSet, (short)219) + ":", "SUB"); //Drops

                    if (auditTill.BonusGiveAway != 0)
                    {
                        FileSystem.PrintLine(intFile, modStringPad.PadR(" - " + _resourceManager.GetResString(offSet, (short)431) + ": ", (short)27) + modStringPad.PadL((auditTill.BonusGiveAway.ToString("#,##0.00")), (short)13)); //Cash bonus Draws
                    }
                    curCashBonus = curCashBonus - auditTill.BonusGiveAway;

                    FileSystem.PrintLine(intFile);

                    FileSystem.PrintLine(intFile, modStringPad.PadL("_________________", (short)40));
                    FileSystem.PrintLine(intFile, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)434) + ":", (short)27) + modStringPad.PadL((curCashBonus.ToString("#,##0.00")), (short)13)); //Current cash bonus
                    FileSystem.PrintLine(intFile);

                }
                FileSystem.PrintLine(intFile, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)3463) + ":", (short)20)); //Other Tenders
                FileSystem.PrintLine(intFile);
                PrintNonBaseCurrTenders(cbt, auditTill.Number, intFile);

                FileSystem.PrintLine(intFile);
                FileSystem.PrintLine(intFile, modStringPad.PadC("===========================", (short)40));

                FileSystem.FileClose(intFile);
                var stream = File.OpenRead(strFileName);
                FileSystem.FileClose(intFile);
                Performancelog.Debug($"End,ReceiptManager,Make_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return stream;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,Make_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                FileSystem.FileClose(intFile);
            }
        }

        /// <summary>
        /// Method to get net sale totals
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Net sale total</returns>
        private float NetSaleTotal(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,NetSaleTotal,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            float returnValue = 0;
            float totals = 0;
            float totOverP = 0;

            var rsTotals = _reportService.GetSaleTotals(tillNumber);

            totOverP = Convert.ToSingle(rsTotals.AmountUsed);

            totals = Convert.ToSingle(rsTotals.AmountTend);
            totals = totals + totOverP; // add the overpayments value
            returnValue = totals;
            Performancelog.Debug($"End,ReceiptManager,NetSaleTotal,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }


        /// <summary>
        /// Method to print line for each tender
        /// </summary>
        /// <param name="curCash">Current cash</param>
        /// <param name="curCashBonus">Current cash bonus</param>
        /// <param name="cbt">Cash bonus tender</param>
        /// <param name="intFileNo">File number</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="blnBaseCurr">Base currency</param>
        /// <param name="strPrint">Print</param>
        /// <param name="strOpt">Option</param>
        private void PrintLine(ref decimal curCash, decimal curCashBonus, string cbt, short intFileNo, List<Tender> tenders, bool blnBaseCurr, string strPrint, string strOpt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintLine,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            bool blnAlreadyPrint = false;
            blnAlreadyPrint = true;
            var strBaseCurr = Convert.ToString(_policyManager.BASECURR);
            foreach (var tender in tenders)
            {
                if (blnBaseCurr) // print only lines for base tender
                {
                    if (Strings.LCase(tender.Tender_Name) == strBaseCurr.ToLower())
                    {
                        FileSystem.PrintLine(intFileNo, modStringPad.PadR(strPrint, (short)27) + modStringPad.PadL((System.Math.Abs(tender.Amount_Used)).ToString("#,##0.00"), (short)13));
                        if (strOpt == "ADD")
                        {
                            curCash = Convert.ToDecimal(curCash + tender.Amount_Used);
                        }
                        else
                        {
                            curCash = Convert.ToDecimal(curCash - tender.Amount_Used);
                        }
                    }
                }
                else
                {
                    if (Strings.UCase(tender.Tender_Name) == cbt.ToUpper())
                    {
                        FileSystem.PrintLine(intFileNo, modStringPad.PadR(strPrint, (short)27) + modStringPad.PadL((System.Math.Abs(tender.Amount_Used).ToString("#,##0.00")), (short)13));
                        if (strOpt == "ADD")
                        {
                            curCashBonus = Convert.ToDecimal(curCashBonus + tender.Amount_Used);
                        }
                        else
                        {
                            curCashBonus = Convert.ToDecimal(curCashBonus - tender.Amount_Used);
                        }
                    }
                    else // print other tenders
                    {
                        if (Strings.LCase(tender.Tender_Name) != strBaseCurr.ToLower() && Strings.LCase(tender.Tender_Name) != cbt.ToLower()) // shiny added the extra criteria for Cash bonus
                        {
                            if (blnAlreadyPrint)
                            {
                                FileSystem.PrintLine(intFileNo, modStringPad.PadR(strPrint, (short)15));
                            }
                            blnAlreadyPrint = false;
                            FileSystem.PrintLine(intFileNo, modStringPad.PadR(tender.Tender_Name, (short)27) + modStringPad.PadL((System.Math.Abs(tender.Amount_Used).ToString("#,##0.00")), (short)13));
                        }
                    }
                }
            }
            Performancelog.Debug($"End,ReceiptManager,PrintLine,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");


        }


        /// <summary>
        /// Method to print non cash based tenders
        /// </summary>
        /// <param name="cbt">Cash bonus tender</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="intFileNo">File number</param>
        private void PrintNonBaseCurrTenders(string cbt, int tillNumber, short intFileNo)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintNonBaseCurrTenders,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            float? totTended = 0;
            float? totDrop = 0;
            float mTotal = 0;

            var tenderNames = _reportService.GetNonCashCurrencyTenders(_policyManager.BASECURR, cbt.ToUpper());
            foreach (var tenderName in tenderNames)
            {
                totTended = _reportService.GetSaleTenderByTenderName(tillNumber, tenderName);
                totDrop = _reportService.GetCashDropByTenderName(tillNumber, tenderName);
                if (totTended != 0 || totDrop != 0)
                {
                    mTotal = totTended.Value - totDrop.Value;
                    FileSystem.PrintLine(intFileNo, modStringPad.PadR(tenderName, (short)27) + modStringPad.PadL((mTotal.ToString("#,##0.00")), (short)13));
                }
            }
            var tenders = _reportService.GetNonCashTenders();
            foreach (var tender in tenders)
            {
                totTended = _reportService.GetSaleTenderByTenderName(tillNumber, tender);
                if (totTended != 0)
                {
                    FileSystem.PrintLine(intFileNo, modStringPad.PadR(tender, (short)27) + modStringPad.PadL(totTended.Value.ToString("#,##0.00"), (short)13));
                }
            }
            Performancelog.Debug($"End,ReceiptManager,PrintNonBaseCurrTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to run report
        /// </summary>
        /// <param name="selDept">Department</param>
        /// <param name="whereClause">Till and shift conditions</param>
        /// <param name="filterRenamed">Filters</param>
        private FileStream Run_Report(string selDept, string whereClause, string filterRenamed, Till till)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Run_Report,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var store = _policyManager.LoadStoreInfo();
            var departments = new List<Department>();
            byte detailLevel = 0;
            short nH = 0;
            string none = "";
            string groupByClause = "";
            string selectClause = "";
            string deptName = "";
            string subDeptName = "";
            string subDetailName = "";
            string dept = "";
            string subDept = "";
            string subDetail;
            double grandTotal = 0;
            double grandTotalAmount = 0;
            var offSet = store.OffSet;
            none = _resourceManager.GetResString(offSet, (short)347);

            if (string.IsNullOrEmpty(selDept))
            {
                departments = _utilityService.GetAllDepartments();
            }
            else
            {
                departments = _utilityService.GetDepartmentById(selDept);
                //dept = selDept;
            }
            _reportService.DeletePreviousCountReport();
            var fileName = Path.GetTempPath() + "SaleCountReport" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            try
            {
                nH = (short)(FileSystem.FreeFile());
                FileSystem.FileOpen(nH, fileName, OpenMode.Output);

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8889), (short)40));
                FileSystem.PrintLine(nH);


                FileSystem.PrintLine(nH, modStringPad.PadC(store.Code + "  " + store.Name, (short)40)); //  

                FileSystem.PrintLine(nH, modStringPad.PadC(Strings.Left(store.Address.Street1 + " " + store.Address.City, 40), (short)40));
                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)406) + ":" + till.ShiftDate.ToString("MM/dd/yyyy"));
                FileSystem.PrintLine(nH, filterRenamed);
                if (selDept.Length > 0)
                {
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)149) + " " + selDept);
                }
                else
                {
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)410)); // all departments
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)119), FileSystem.TAB((short)22), _resourceManager.GetResString(offSet, (short)105), FileSystem.TAB((short)32), _resourceManager.GetResString(offSet, (short)106));
                FileSystem.PrintLine(nH, "__________________________________________");
                FileSystem.PrintLine(nH);

                foreach (var department in departments)
                {

                    detailLevel = Convert.ToByte(department.CountDetail);

                    dept = string.IsNullOrEmpty(department.Dept) ? none : department.Dept;

                    if (detailLevel == ((byte)1))
                    {
                        groupByClause = "B.Dept, B.Stock_Code, C.Stock_Code , C.Quantity ";
                        selectClause = "B.Dept AS Dept, B.Stock_Code, C.Stock_Code AS KStock_Code, C.Quantity , SUM(B.Quantity) As ItemCount, SUM(Amount) As Amount  ";
                    }
                    else if (detailLevel == ((byte)2))
                    {
                        groupByClause = "B.Dept, B.Sub_Dept, B.Stock_Code, C.Stock_Code, C.Quantity ";
                        selectClause = "B.Dept AS Dept, B.Sub_Dept AS Sub_Dept, B.Stock_Code, C.Stock_Code AS KStock_Code , C.Quantity, SUM(b.Quantity) As ItemCount, SUM(Amount) As Amount  ";
                    }
                    else if (detailLevel == ((byte)3))
                    {
                        groupByClause = "B.Dept, B.Sub_Dept, B.Sub_Detail, B.Stock_Code, C.Stock_Code, C.Quantity ";
                        selectClause = "B.Dept AS Dept, B.Sub_Dept AS Sub_Dept, B.Sub_Detail AS Sub_Detail, B.Stock_Code, C.Stock_Code AS KStock_Code, C.Quantity, SUM(b.Quantity) As ItemCount, SUM(Amount) As Amount  ";
                    }
                    else if (detailLevel == ((byte)4))
                    {
                        groupByClause = "B.Dept, B.Sub_Dept, B.Sub_Detail, B.Stock_Code, C.Stock_Code , C.Quantity, B.Descript  ";
                        selectClause = "B.Dept AS Dept, B.Sub_Dept AS Sub_Dept, B.Sub_Detail AS Sub_Detail, B.Stock_Code AS Stock_Code, B.Descript, C.Stock_Code AS KStock_Code, C.Quantity, SUM(b.Quantity) As ItemCount, SUM(Amount) As Amount  ";
                    }


                    var countReportDetails = _reportService.GetSalesCountDetail(selectClause, dept, till.ShiftDate, whereClause,
                       groupByClause, detailLevel);
                    foreach (var countReportDetail in countReportDetails)
                    {
                        _reportService.AddCountReport(countReportDetail);
                    }
                }

                foreach (var department in departments)
                {

                    detailLevel = Convert.ToByte(department.CountDetail);
                    //departmentName = department.DeptName;
                    dept = string.IsNullOrEmpty(department.Dept) ? none : department.Dept;

                    deptName = string.IsNullOrEmpty(department.DeptName) ? none : department.DeptName;
                    string query = string.Empty;
                    if (detailLevel == ((byte)1))
                    {
                        query = "SELECT Dept, SUM(ItemCount) AS ItemCount, SUM(Amount) As Amount  FROM CountReport WHERE  Dept=\'" + dept + "\' GROUP BY Dept";
                    }
                    else if (detailLevel == ((byte)2))
                    {
                        query = "SELECT Dept, Sub_Dept, SUM(ItemCount) AS ItemCount, SUM(Amount) As Amount  FROM CountReport WHERE  Dept=\'" + dept + "\' GROUP BY Dept, Sub_Dept";

                    }
                    else if (detailLevel == ((byte)3))
                    {
                        query = "SELECT Dept, Sub_Dept, Sub_Detail, SUM(ItemCount) AS ItemCount, SUM(Amount) As Amount  FROM CountReport WHERE  Dept=\'" + dept + "\' GROUP BY Dept, Sub_Dept, Sub_Detail";

                    }
                    else if (detailLevel == ((byte)4))
                    {
                        query = "SELECT Dept, Sub_Dept, Sub_Detail, Stock_Code, SUM(ItemCount) AS ItemCount, SUM(Amount) As Amount  FROM CountReport WHERE  Dept=\'" + dept + "\' GROUP BY Dept, Sub_Dept, Sub_Detail, Stock_Code ";

                    }
                    var countReports = _reportService.GetCountReports(query, detailLevel);
                    if (countReports.Count != 0)
                    {
                        FileSystem.Print(nH, dept, FileSystem.TAB((short)10), deptName);
                    }
                    int index = 0;
                    while (index < countReports.Count)
                    {
                        if (detailLevel == ((byte)1))
                        {
                            FileSystem.PrintLine(nH, FileSystem.TAB((short)22), modStringPad.PadL(Convert.ToString(countReports[index].ItemCount), (short)5) + string.Format(modStringPad.PadL(countReports[index].Amount.ToString("$#,##0.00"), (short)10)));
                            grandTotal = Convert.ToDouble(grandTotal + Convert.ToDouble(countReports[index].ItemCount));

                            grandTotalAmount = Convert.ToDouble(grandTotalAmount + countReports[index].Amount);
                        }
                        else if (detailLevel == ((byte)2))
                        {

                            var subName = _utilityService.GetSubDepartmentName(dept, countReports[index].SubDepartment);

                            if (subName != null)
                            {

                                subDeptName = subName.Length == 0 ? none : subName;
                            }
                            else
                            {
                                subDeptName = "";
                            }

                            FileSystem.Print(nH, FileSystem.TAB((short)3), countReports[index].SubDepartment, FileSystem.TAB((short)13), Strings.Left(subDeptName, 25));
                            FileSystem.PrintLine(nH, FileSystem.TAB((short)22), modStringPad.PadL(Convert.ToString(countReports[index].ItemCount), (short)5) + modStringPad.PadL((countReports[index].Amount.ToString("$#,##0.00")), (short)10));
                            grandTotal = Convert.ToDouble(grandTotal + Convert.ToDouble(countReports[index].ItemCount));

                            grandTotalAmount = Convert.ToDouble(grandTotalAmount + Convert.ToDouble(countReports[index].Amount));

                        }
                        else if (detailLevel == ((byte)3))
                        {

                            var subName = _utilityService.GetSubDepartmentName(dept, countReports[index].SubDepartment);

                            if (subName != null)
                            {
                                subDeptName = subName.Length == 0 ? none : subName;
                            }
                            else
                            {
                                subDeptName = "";
                            }

                            FileSystem.PrintLine(nH, FileSystem.TAB((short)3), countReports[index].SubDepartment, FileSystem.TAB((short)13), Strings.Left(subDeptName, 25));
                            subDept = Convert.ToString(countReports[index].SubDepartment);

                            while (subDept == (countReports[index].SubDepartment))
                            {

                                subDetailName = _utilityService.GetSubDetailName(dept, countReports[index].SubDepartment,
                                     countReports[index].SubDetail);

                                FileSystem.Print(nH, FileSystem.TAB((short)6), countReports[index].SubDetail, FileSystem.TAB((short)16), Strings.Left(subDetailName, 22));
                                FileSystem.PrintLine(nH, FileSystem.TAB((short)22), modStringPad.PadL(Convert.ToString(countReports[index].ItemCount), (short)5) + modStringPad.PadL((countReports[index].Amount.ToString("$#,##0.00")), (short)10));
                                grandTotal = Convert.ToDouble(grandTotal + Convert.ToDouble(countReports[index].ItemCount));

                                grandTotalAmount = Convert.ToDouble(grandTotalAmount + Convert.ToDouble(countReports[index].Amount));

                                index++;
                                if (index >= countReports.Count)
                                {
                                    break;
                                }

                            }
                        }
                        else if (detailLevel == ((byte)4))
                        {

                            var subName = _utilityService.GetSubDepartmentName(dept, countReports[index].SubDepartment);

                            if (subName != null)
                            {
                                subDeptName = subName.Length == 0 ? none : subName;
                            }
                            else
                            {
                                subDeptName = "";
                            }

                            FileSystem.PrintLine(nH, FileSystem.TAB((short)3), countReports[index].SubDepartment, FileSystem.TAB((short)13), subDeptName);
                            subDept = countReports[index].SubDepartment;
                            subDetailName = _utilityService.GetSubDetailName(dept, countReports[index].SubDepartment,
                                                           countReports[index].SubDetail);

                            FileSystem.PrintLine(nH, FileSystem.TAB((short)6), countReports[index].SubDetail, FileSystem.TAB((short)16), subDetailName);
                            subDetail = countReports[index].SubDetail;

                            while (countReports[index].SubDepartment == subDept && subDetail == countReports[index].SubDetail)
                            {
                                var kit = _stockService.GetKitDescription(countReports[index].StockCode);

                                FileSystem.Print(nH, countReports[index].StockCode, FileSystem.TAB((short)17), Strings.Left(kit, 22));
                                FileSystem.PrintLine(nH, FileSystem.TAB((short)22), modStringPad.PadL(Convert.ToString(countReports[index].ItemCount), (short)5) + modStringPad.PadL((countReports[index].Amount.ToString("$#,##0.00")), (short)10));
                                grandTotal = Convert.ToDouble(grandTotal + Convert.ToDouble(countReports[index].ItemCount));

                                grandTotalAmount = Convert.ToDouble(grandTotalAmount + Convert.ToDouble(countReports[index].Amount));

                                index++;
                                if (index >= countReports.Count)
                                {
                                    break;
                                }
                            }
                        }

                        if (detailLevel < 3)
                        {
                            index++;
                        }
                        FileSystem.PrintLine(nH);
                    }
                }

                if (grandTotal != 0)
                {
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)210), FileSystem.TAB((short)19), modStringPad.PadL(grandTotal.ToString("#####0"), (short)6) + modStringPad.PadL((grandTotalAmount.ToString("$###,##0.00")), (short)12));
                }

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, 199) + Strings.Space(5) + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), (short)40)); //  
                FileSystem.PrintLine(nH);
                FileSystem.FileClose(nH);
                var stream = File.OpenRead(fileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,Run_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return stream;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,Run_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                FileSystem.FileClose(nH);
            }
        }

        /// <summary>
        /// Method to get time formats
        /// </summary>
        /// <param name="timeFormatHm">Hour minute</param>
        /// <param name="timeFormatHms">Hour minute second </param>
        private void GetTimeFormats(ref string timeFormatHm, ref string timeFormatHms)
        {
            if (_policyManager.TIMEFORMAT == "24 HOURS")
            {
                timeFormatHm = "hh:mm";
                timeFormatHms = "hh:mm:ss";
            }
            else
            {
                timeFormatHm = "hh:mm tt";
                timeFormatHms = "hh:mm:ss tt";
            }
        }



        /// <summary>
        /// Method to print Kickback Points
        /// </summary>
        /// <param name="points">Points</param>
        /// <returns>File stream</returns>
        public FileStream PrintKickbackPoints(int points)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintKickbackPoints,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

           
       
           

            var strFileName = Path.GetTempPath() + "\\KickBack_Balance" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            Performancelog.Debug($"End,ReceiptManager,PrintKickbackPoints,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Kickback_Report(strFileName, points);
        }


        public Report Print_Kickback(Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,PrintKickbackReceipt,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");





            var strFileName = Path.GetTempPath() + "\\KickBack_Receipt" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + GenerateRandomNo() + ".txt";
            var fs= Print_KickbackReceipt(strFileName, sale);
            var content = GetReportContent(fs);
            var kickBackReceipt = new Report
            {
                ReportName = Utilities.Constants.KickbackReceipt,
                ReportContent = content,
                Copies = 1
            };
            return kickBackReceipt;
            Performancelog.Debug($"End,ReceiptManager,PrintKickbackReceipt,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

          
        }



        /// <summary>
        /// Method to make report for Kickback
        /// </summary>
        /// <param name="strFileName">File name</param>
        /// <param name="points">Kickback Points</param>
        /// <returns>Streamm</returns>
        public FileStream Kickback_Report(string strFileName, int points)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Kickback_Report,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            short intFile = 0;
            short H_Width = 0;
            string Just = "";
            string strRenamed = "";
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            intFile = (short)(FileSystem.FreeFile());
            var Phone_Renamed = new Phone();
            Just = Strings.Left(System.Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification
            H_Width = (short)40; // Total Receipt Width

            try
            {
                FileSystem.FileOpen(intFile, strFileName, OpenMode.Output);



                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, H_Width));
                }

                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, System.Convert.ToString(store.Address.Street1), H_Width));
                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, System.Convert.ToString(store.Address.Street2), H_Width));
                    }
                    FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, H_Width) + "\r\n" + modStringPad.PadIt(Just, System.Convert.ToString(store.Address.PostalCode), H_Width));
                }



                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVar_Phone_Renamed in store.Address.Phones)
                    {
                        Phone_Renamed = tempLoopVar_Phone_Renamed;
                        if (Phone_Renamed.Number.Trim() != "")
                        {
                            FileSystem.PrintLine(intFile, modStringPad.PadC(Phone_Renamed.PhoneName + " " + Phone_Renamed.Number, H_Width));
                        }
                    }
                }

                FileSystem.PrintLine(intFile);
                FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, "Date: " + DateAndTime.Today.ToString("dd-MMM-yyyy") + " " + DateAndTime.TimeOfDay.ToString(Chaps_Main.TimeFormatHM), H_Width) + "\r\n");
                FileSystem.PrintLine(intFile);
                FileSystem.PrintLine(intFile, "<B>" + modStringPad.PadC("Kickback Points Available", H_Width) + "</B>");
                FileSystem.PrintLine(intFile);
    
                FileSystem.PrintLine(intFile, modStringPad.PadC("Current Balance: " + System.Convert.ToString(points), H_Width));
                FileSystem.PrintLine(intFile);
                FileSystem.PrintLine(intFile, store.Sale_Footer);
                FileSystem.PrintLine(intFile);

             

                FileSystem.FileClose(intFile);
                var stream = File.OpenRead(strFileName);
                FileSystem.FileClose(intFile);
                Performancelog.Debug($"End,ReceiptManager,Kickback_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return stream;
            }
            finally
            {
                Performancelog.Debug($"End,ReceiptManager,Kickback_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                FileSystem.FileClose(intFile);
            }
        }


        public FileStream Print_KickbackReceipt(string strFileName, Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReceiptManager,Print_KickbackReceipt,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            short nH = 0;
            short n;
            short A_Width;
            short H_Width = 0;
            string Pad;
            string File_Name = "";
            bool Sig_Line;
            string Just = "";
            var Phone_Renamed =new Phone();
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            A_Width = (short)10; // Amount Width
            H_Width = (short)40; // Total Receipt Width
            nH = (short)(FileSystem.FreeFile());
            Just = Strings.Left(System.Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification

            if (Information.IsDBNull(sale.Sale_Time))
            {
                sale.Sale_Time = DateAndTime.TimeOfDay;
            }
            try { 
          
            FileSystem.FileOpen(nH, strFileName, OpenMode.Output);

            FileSystem.PrintLine(nH);
            //    If Policy.PRN_CO_NAME Then Print #nH, PadIt(Just, Store.Code & "  " & Store_Renamed.Name, H_Width)
            //shiny changed on june1,2010 - store code printing should be based on policy - Gasking will enter store code as part of store name - so they don't want to see store code in the beginning
            //        If Policy.PRN_CO_NAME Then Print #nH, PadIt(Pad, Store.Code & "  " & Store_Renamed.Name, H_Width)
            if (_policyManager.PRN_CO_NAME)
            {
                FileSystem.PrintLine(nH, modStringPad.PadIt(Just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, H_Width));
            }
            //shiny end - June1, 2010

            if (_policyManager.PRN_CO_ADDR)
            {
                FileSystem.PrintLine(nH, modStringPad.PadIt(Just, System.Convert.ToString(store.Address.Street1), H_Width));
                if (store.Address.Street2 != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(Just, System.Convert.ToString(store.Address.Street2), H_Width));
                }
                FileSystem.PrintLine(nH, modStringPad.PadIt(Just, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, H_Width));
            }

            if (_policyManager.PRN_CO_PHONE)
            {
                foreach (Phone tempLoopVar_Phone_Renamed in store.Address.Phones)
                {
                    Phone_Renamed = tempLoopVar_Phone_Renamed;
                    if (Phone_Renamed.Number.Trim() != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(Phone_Renamed.PhoneName + " " + Phone_Renamed.Number, H_Width));
                    }
                }
            }

            FileSystem.PrintLine(nH, modStringPad.PadIt(Just, Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, H_Width)); //& vbCrLf
            FileSystem.PrintLine(nH, modStringPad.PadIt(Just, _resourceManager.GetResString(offSet,(short)229).ToUpper() + Convert.ToString(sale.Sale_Num), H_Width)); //"SALE RECEIPT # "
                                                                                                                                                                 //shiny chnaged on march 10,2010- sale_time is coming as 12:00
                                                                                                                                                                 //    Print #nH, PadIt(Just, Format(Sale.Sale_Date, "dd-MMM-yyyy") & GetResString(208) & Format(Sale.Sale_Time, TimeFormatHM), H_Width) & vbCrLf 'shiny added jan22,2010- gasking request
          //FileSystem.PrintLine(nH, modStringPad.PadIt(Just, DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet,(short)208) + DateAndTime.TimeOfDay.ToString(_policyManager.TIMEFORMAT), H_Width) + "\r\n"); //shiny added march10,2010- gasking request
          //      FileSystem.PrintLine(nH, modStringPad.PadIt(Just, DateAndTime.Today.ToString("dd-MMM-yyyy")+ DateAndTime.TimeOfDay.ToString(_policyManager.TIMEFORMAT), H_Width) + "\r\n");
          //      var a = DateAndTime.Today.ToString("dd-MMM-yyyy");

          //      var b = DateAndTime.TimeOfDay.ToString(_policyManager.TIMEFORMAT);
                //shiny end

      FileSystem.PrintLine(nH, modStringPad.PadIt(Just,DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) +
        DateAndTime.TimeOfDay.ToString(timeFormatHm), H_Width) + "\r\n");


                FileSystem.PrintLine(nH);
            FileSystem.PrintLine(nH);
            FileSystem.PrintLine(nH, GetMaskedCardNum(sale.Customer.PointCardNum, "K"));
                //shiny end

              //  FileSystem.PrintLine(intFile, modStringPad.PadC("Current Balance: " + System.Convert.ToString(points), H_Width));


                FileSystem.PrintLine(nH, XML.GetCustomerMessageData);
            //Shiny added on Sept16, 2009 - to show
            FileSystem.PrintLine(nH);
            if (sale.Sale_Totals.Gross >= 0)
            {
                FileSystem.PrintLine(nH, store.Sale_Footer);
            }
            else
            {
                FileSystem.PrintLine(nH, store.Refund_Footer);
            }
            FileSystem.PrintLine(nH);

            FileSystem.FileClose(nH);
             var stream = File.OpenRead(strFileName);
                FileSystem.FileClose(nH);
                Performancelog.Debug($"End,ReceiptManager,Kickback_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return stream;
            }
            finally 
            {
                Performancelog.Debug($"End,ReceiptManager,Kickback_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                FileSystem.FileClose(nH);
            }
        }


        /// <summary>
        /// Method to make report for Kickback
        /// </summary>
        /// <param name="strFileName">File name</param>
        /// <param name="points">Kickback Points</param>
        /// <returns>Streamm</returns>
        //public FileStream Kickback_Receipt(string strFileName, Sale Sale)
        //{
        //    var dateStart = DateTime.Now;
        //    Performancelog.Debug($"Start,ReceiptManager,Kickback_Report,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");



        //    int nH = 0;
        //    short n;
        //    short A_Width;
        //    short H_Width = 0;
        //    string Pad;
        //    string File_Name = "";
        //    bool Sig_Line;
        //    string Just = "";

        //    A_Width = (short)10; // Amount Width
        //    H_Width = (short)40;


        //    var store = _policyManager.LoadStoreInfo();
        //    var offSet = store.OffSet;
        //    short intFile = 0;

        //    string strRenamed = "";
        //    string timeFormatHm = string.Empty;
        //    string timeFormatHms = string.Empty;
        //    GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
        //    intFile = (short)(FileSystem.FreeFile());
        //    var Phone_Renamed = new Phone();



        //    try
        //    {
        //        Just = Strings.Left(System.Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper(); // Header Justification

        //        if (Information.IsDBNull(Sale.Sale_Time))
        //        {
        //            Sale.Sale_Time = DateAndTime.TimeOfDay;
        //        }

        //        FileSystem.FileOpen(intFile, strFileName, OpenMode.Output);


        //        FileSystem.PrintLine(intFile);
        //        //    If Policy.PRN_CO_NAME Then Print #nH, PadIt(Just, Store.Code & "  " & Store_Renamed.Name, H_Width)
        //        //shiny changed on june1,2010 - store code printing should be based on policy - Gasking will enter store code as part of store name - so they don't want to see store code in the beginning
        //        //        If Policy.PRN_CO_NAME Then Print #nH, PadIt(Pad, Store.Code & "  " & Store_Renamed.Name, H_Width)
        //        if (_policyManager.PRN_CO_NAME)
        //        {
        //            FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, H_Width));
        //        }
        //        //shiny end - June1, 2010

        //        if (_policyManager.PRN_CO_ADDR)
        //        {
        //            FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, System.Convert.ToString(store.Address.Street1), H_Width));
        //            if (store.Address.Street2 != "")
        //            {
        //                FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, System.Convert.ToString(store.Address.Street2), H_Width));
        //            }
        //            FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, Strings.Trim(System.Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, H_Width));
        //        }

        //        if (_policyManager.PRN_CO_PHONE)
        //        {
        //            foreach (Phone tempLoopVar_Phone_Renamed in store.Address.Phones)
        //            {
        //                Phone_Renamed = tempLoopVar_Phone_Renamed;
        //                if (Phone_Renamed.Number.Trim() != "")
        //                {
        //                    FileSystem.PrintLine(nH, modStringPad.PadC(Phone_Renamed.PhoneName + " " + Phone_Renamed.Number, H_Width));
        //                }
        //            }
        //        }

        //        FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, Strings.Trim(System.Convert.ToString(store.RegName)) + " " + store.RegNum, H_Width)); //& vbCrLf
        //        FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, Chaps_Main.GetResString((short)229).ToUpper() + System.Convert.ToString(Sale.Sale_Num), H_Width)); //"SALE RECEIPT # "
        //                                                                                                                                                                  //shiny chnaged on march 10,2010- sale_time is coming as 12:00
        //                                                                                                                                                                  //    Print #nH, PadIt(Just, Format(Sale.Sale_Date, "dd-MMM-yyyy") & GetResString(208) & Format(Sale.Sale_Time, TimeFormatHM), H_Width) & vbCrLf 'shiny added jan22,2010- gasking request
        //        FileSystem.PrintLine(intFile, modStringPad.PadIt(Just, DateAndTime.Today.ToString("dd-MMM-yyyy") + Chaps_Main.GetResString((short)208) + DateAndTime.TimeOfDay.ToString(Chaps_Main.TimeFormatHM), H_Width) + "\r\n"); //shiny added march10,2010- gasking request
        //                                                                                                                                                                                                                              //shiny end
        //        FileSystem.PrintLine(intFile);
        //        FileSystem.PrintLine(intFile);
        //        FileSystem.PrintLine(intFile, GetMaskedCardNum(Sale.Customer.PointCardNum, "K"));
        //        //shiny end

        //        FileSystem.PrintLine(intFile, Variables.KickBack.GetCustomerMessageData);
        //        //Shiny added on Sept16, 2009 - to show
        //        FileSystem.PrintLine(intFile);
        //        if (Sale.Sale_Totals.Gross >= 0)
        //        {
        //            FileSystem.PrintLine(intFile, store.Sale_Footer);
        //        }
        //        else
        //        {
        //            FileSystem.PrintLine(intFile, store.Refund_Footer);
        //        }
        //        FileSystem.PrintLine(intFile);

        //        FileSystem.FileClose(intFile);

        //        var stream = File.OpenRead(strFileName);
        //        FileSystem.FileClose(intFile);
        //        return stream;
        //    }
        //    finally
        //    {
        //        Performancelog.Debug($"End,ReceiptManager,Kickback_Report,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        //        FileSystem.FileClose(intFile);
        //    }
        //}


        #endregion

    }//end class
}//end namespace
