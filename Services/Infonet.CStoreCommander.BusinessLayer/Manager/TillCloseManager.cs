using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Data.SqlClient;
using System.Data;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TillCloseManager : ManagerBase, ITillCloseManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ILoginManager _loginManager;
        private readonly ISaleManager _saleManager;
        private readonly ITillCloseService _tillCloseService;
        private readonly ITillService _tillService;
        private readonly ITenderService _tenderService;
        private readonly IGetPropertyManager _getPropertyManager;
        private readonly IFuelPumpManager _fuelPumpManager;
        private readonly IUtilityService _utilityService;
        private readonly IMainManager _mainManager;
        private readonly ICardManager _cardManager;
        private readonly ITenderManager _tenderManager;
        private readonly IFuelPrepayManager _fuelPrepayManager;
        private readonly ISaleService _saleService;
        private readonly ICreditCardManager _creditCardManager;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IKickBackService _kickBackService;
        private readonly IWexManager _wexManager;

        public TillCloseManager(IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ILoginManager loginManager,
            ISaleManager saleManager,
            ITillCloseService tillCloseService,
            ITillService tillService,
            IGetPropertyManager getPropertyManager,
            ITenderService tenderService,
            IFuelPumpManager fuelPumpManager,
            IUtilityService utilityService,
            IMainManager mainManager,
            ICardManager cardManager,
            ITenderManager tenderManager,
            IFuelPrepayManager fuelPrepayManager,
            ISaleService saleService,
            ICreditCardManager creditCardManager,
            IMaintenanceService maintenanceService,
            IKickBackService kickBackService,
            IWexManager wexManager)
        {
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _loginManager = loginManager;
            _saleManager = saleManager;
            _tillCloseService = tillCloseService;
            _tillService = tillService;
            _getPropertyManager = getPropertyManager;
            _fuelPumpManager = fuelPumpManager;
            _tenderService = tenderService;
            _utilityService = utilityService;
            _mainManager = mainManager;
            _cardManager = cardManager;
            _tenderManager = tenderManager;
            _fuelPrepayManager = fuelPrepayManager;
            _saleService = saleService;
            _creditCardManager = creditCardManager;
            _maintenanceService = maintenanceService;
            _kickBackService = kickBackService;
            _wexManager = wexManager;
        }

        /// <summary>
        /// Method to set till recordser
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <param name="tillCloseTenders">Till close tenders</param>
        public void SetTill_Recordset(ref Till_Close tillClose, List<TillClose> tillCloseTenders)
        {
            tillClose.Tenders = tillCloseTenders;
            Close_Line cl;



            if (_policyManager.COUNT_TYPE == "Each Tender")
            {

                ErrorMessage error;
                var tenders = _tenderManager.Load(null, "CloseCurrentTill", true, "", out error);
                var n = (short)0;
                if (tillClose.Tenders == null)
                {
                    tillClose.Tenders = new List<TillClose>();
                }
                foreach (var tender in tillClose.Tenders)
                {
                    cl = new Close_Line();
                    n++;
                    cl.Sequence = n;
                    if (tender.Tender == "Other Tenders")
                    {
                        cl.Short_Name = "Other";
                        cl.Tender_Class = "OTHER";
                        cl.Exchange_Rate = 1;
                    }
                    else
                    {
                        //                CL.Short_Name = Tenders.Item(![Tender]).Short_Name

                        if ((_policyManager.COMBINECR && tender.Tender?.ToUpper() == "CRCARD"
                            && Strings.UCase(Convert.ToString(_policyManager.CC_MODE)) != "VALIDATE")
                            || (_policyManager.COMBINEFLEET && tender.Tender?.ToUpper() == "FLEET"
                            && Strings.UCase(Convert.ToString(_policyManager.CC_MODE)) != "VALIDATE"))
                        {

                            cl.Tender_Class = tender.Tender?.ToUpper();
                            cl.Exchange_Rate = 1;

                            //""
                        }
                        else if (string.IsNullOrEmpty(tender.Tender))
                        {
                            cl.Tender_Class = "No_TENDCLS";
                            cl.Exchange_Rate = 1;
                            //end
                        }
                        else
                        {
                            var selectedTender = tenders.FirstOrDefault(t => t.Tender_Code?.ToUpper() == tender.Tender?.ToUpper());
                            if (selectedTender != null)
                            {
                                cl.Tender_Class = selectedTender.Tender_Class;
                                cl.Exchange_Rate = selectedTender.Exchange_Rate;
                            }
                            else
                            {
                                cl.Tender_Class = "No_TENDCLS";
                                cl.Exchange_Rate = 1;
                            }
                        }
                    }
                    //""
                    cl.Tender_Name = string.IsNullOrEmpty(tender.Tender) ? "No_Tend_Name" : tender.Tender.ToUpper();
                    //end

                    cl.Tender_Count = (short)tender.Count;
                    cl.Entered = Convert.ToDecimal(tender.Entered);
                    cl.System = Convert.ToDecimal(tender.System);
                    cl.Balance = Convert.ToDecimal(tender.Difference);

                    cl.Converted_System = Convert.ToDecimal(tender.System * cl.Exchange_Rate);
                    cl.Converted_Entered = Convert.ToDecimal(tender.Entered * cl.Exchange_Rate);
                    cl.Converted_Balance = Convert.ToDecimal(tender.Difference * cl.Exchange_Rate);
                    tillClose.Add(cl, "");
                }
            }
            else
            {
                var allTender = tillCloseTenders.FirstOrDefault() ?? new TillClose();
                cl = new Close_Line
                {
                    Sequence = 1,
                    Tender_Name = "All Tenders",
                    Short_Name = "All",
                    Tender_Class = "ALL",
                    Tender_Count = 1,
                    Entered = Convert.ToDecimal(allTender.Entered),
                    System = Convert.ToDecimal(allTender.System),
                    Balance = Convert.ToDecimal(allTender.Difference),
                    Exchange_Rate = 1,
                    Converted_System = Convert.ToDecimal(allTender.System),
                    Converted_Entered = Convert.ToDecimal(allTender.Entered),
                    Converted_Balance = Convert.ToDecimal(allTender.Difference)
                };
                tillClose.Add(cl, "");
            }
        }

        /// <summary>
        /// Method to set trans database
        /// </summary>
        /// <param name="tillClose">Till close</param>
        public void SetTrans_Database(ref Till_Close tillClose)
        {


            // Need to set date and time since till_database is not used
            tillClose.Close_Date = DateAndTime.Today;
            tillClose.Close_Time = new DateTime(1899, 12, 30, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            //End - SV
            tillClose.Close_Num = _tillCloseService.GetMaxCloseHead();
        }


        /// <summary>
        /// Method to close till
        /// </summary>
        /// <param name="tillClose">till close</param>
        /// <param name="till">Till</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="allShifts">All shifts or not</param>
        public void Close_Till(ref Till_Close tillClose, Till till, string whereClause, DateTime shiftDate,
            bool allShifts)
        {
            if (shiftDate == default(DateTime))
                shiftDate = DateTime.Parse("12:00:00 AM");

            short nH = 0;
            int n = 0;
            bool countCash = false;
            decimal ent = new decimal();
            decimal sys = new decimal();
            decimal dif = new decimal();
            decimal excAdjE = new decimal();
            decimal excAdjS = new decimal();
            decimal excAdjD = new decimal();
            Close_Line cl = default(Close_Line);
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
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
            DataSource dataSource = DataSource.CSCTills;
            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            //   print till close for all shifts
            if (allShifts)
            {
                dataSource = DataSource.CSCTrans;
            }
            //   end

            countCash = Convert.ToBoolean(_policyManager.COUNT_CASH);
            if (allShifts)
            {
                countCash = false; //   reprint all shifts don't require cash count
            }

            // Pick up sale credits issued.
            tillClose.Credits_Issued = _tillCloseService.GetTotalCredits(till.Number, whereClause, dataSource);
            var filePath = Path.GetTempPath() + "\\TillClose_" + PosId + ".txt";
            nH = (short)(FileSystem.FreeFile());
            try
            {
                if (allShifts)
                {
                    FileSystem.FileOpen(nH, filePath, OpenMode.Append);
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, new string('=', modPrint.PRINT_WIDTH));
                    FileSystem.PrintLine(nH);
                }
                else
                {
                    FileSystem.FileOpen(nH, filePath, OpenMode.Append);
                }
                if (!allShifts)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(store.Code + "  " + store.Name, modPrint.PRINT_WIDTH)); //  
                    FileSystem.PrintLine(nH, modStringPad.PadC(Convert.ToString(store.Address.Street1), modPrint.PRINT_WIDTH)); //  
                    FileSystem.PrintLine(nH, modStringPad.PadC(store.Address.City + "," + store.Address.ProvState, modPrint.PRINT_WIDTH)); //  

                    FileSystem.PrintLine(nH);
                    //    Print #nH, PadC("TILL CLOSE #" & Me.Close_Num & " FOR TILL #" & Me.Till_Number, PRINT_WIDTH)
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8310) + " #" + Convert.ToString(tillClose.Close_Num) + " " + _resourceManager.GetResString(offSet, (short)8311) + " #" + Convert.ToString(tillClose.Till_Number), modPrint.PRINT_WIDTH));
                    //    Print #nH, PadC("Opened on " & Format(Me.Open_Date, "dd-MMM-yyyy") & " at " & Format(Me.Open_Time, "hh:mm:ss tt"), PRINT_WIDTH)
                    //    Print #nH, PadC(GetResString(8312) & " " & Format(Me.Open_Date, "dd-MMM-yyyy") & " " & GetResString(208) & " " & Format(Me.Open_Time, "hh:mm:ss tt"), PRINT_WIDTH)
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8312) + " " + tillClose.Open_Date.ToString("dd-MMM-yyyy") + " " + _resourceManager.GetResString(offSet, (short)208) + " " + tillClose.Open_Time.ToString(timeFormatHms), modPrint.PRINT_WIDTH)); //  
                                                                                                                                                                                                                                                                                                             //    Print #nH, PadC("Closed on " & Format(Me.Close_Date, "dd-MMM-yyyy") & " at " & Format(Me.Close_Time, "hh:mm:ss tt"), PRINT_WIDTH)
                                                                                                                                                                                                                                                                                                             ///    Print #nH, PadC(GetResString(8313) & " " & Format(Me.Close_Date, "dd-MMM-yyyy") & " " & GetResString(208) & " " & Format(Me.Close_Time, "hh:mm:ss tt"), PRINT_WIDTH)
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8313) + " " + tillClose.Close_Date.ToString("dd-MMM-yyyy") + " " + _resourceManager.GetResString(offSet, (short)208) + " " + tillClose.Close_Time.ToString(timeFormatHms), modPrint.PRINT_WIDTH)); //  


                    if (_policyManager.USE_SHIFTS && whereClause.Length == 0)
                    {

                        //        Print #nH, PadC("Shift " & Me.ShiftNumber & " on " & Format(Me.ShiftDate, "dd-MMM-yyyy"), PRINT_WIDTH)
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)346) + " " + Convert.ToString(tillClose.ShiftNumber) + " " + _resourceManager.GetResString(offSet, (short)262) + " " + tillClose.ShiftDate.ToString("dd-MMM-yyyy"), modPrint.PRINT_WIDTH));
                    }

                    //  - To show User name in Till close report
                    var tillUser = _loginManager.GetUser(Convert.ToString(till.UserLoggedOn));
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)225) + ": " + tillUser.Code + " - " + tillUser.Name, modPrint.PRINT_WIDTH));
                }

                // Added to print ezipin terminal id
                if (_policyManager.SUPPORTEZI)
                {
                    var terminalId = _tillCloseService.GetEzipinTerminalId();
                    if (!string.IsNullOrEmpty(terminalId))
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)1255) + " " + _resourceManager.GetResString(offSet, (short)306) + ": " + terminalId, modPrint.PRINT_WIDTH)); //  
                    }
                }
                //End - SV

                // 
                if (allShifts)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)600) + ": " + shiftDate.ToString("dd-MMM-yyyy"), modPrint.PRINT_WIDTH));
                    //        Print #nH, PadC("REPRINT ON " & Format(Now, "dd-MMM-yyyy") & " AT " & Format(Now, "hh:mm:ss tt"), PRINT_WIDTH)
                    FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)234) + " " + DateAndTime.Today.ToString("dd-MMM-yyyy") + " " + _resourceManager.GetResString(offSet, (short)208) + " " + DateAndTime.TimeOfDay.ToString(timeFormatHms), modPrint.PRINT_WIDTH)); //  
                }

                FileSystem.PrintLine(nH);

                if (tillClose.Till_Number >= double.Parse(Entities.Constants.TrainFirstTill) && tillClose.Till_Number <= double.Parse(Entities.Constants.TrainLastTill) && !allShifts)
                {
                    var saleNumbers = _tillCloseService.GetMaxMinSaleNumber(till.Number, dataSource);
                    if (saleNumbers.Count != 0)
                    {
                        n = saleNumbers[0] - saleNumbers[1];


                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8366), (short)30) + modStringPad.PadL(saleNumbers[1].ToString("0"), (short)8));
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8367), (short)30) + modStringPad.PadL(saleNumbers[0].ToString("0"), (short)8));
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8324), (short)30) + modStringPad.PadL(n.ToString("0"), (short)8));
                    }
                }


                ent = 0;
                sys = 0;
                dif = 0;
                excAdjE = 0;
                excAdjS = 0;
                excAdjD = 0;
                n = (short)0;

                if (countCash)
                {
                    //        Print #nH, PadR("Tender", 7) & PadC("Cnt", 3) & _
                    //PadL("Entered", 10) & _
                    //PadL("System", 10) & _
                    //PadL("Diff", 10)
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)402), (short)7) + modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8314), (short)3) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8315), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8316), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8317), (short)10));
                }
                else
                {
                    //        Print #nH, PadR("Tender", 7) & PadC("Cnt", 3) & _
                    //Space(10) & _
                    //PadL("System", 10)
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)402), (short)7) + modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8314), (short)3) + Strings.Space(10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8316), (short)10));
                }
                FileSystem.PrintLine(nH, modStringPad.PadL("_", modPrint.PRINT_WIDTH, "_"));

                // Compute the exchange adjustments for each tender
                foreach (Close_Line tempLoopVarCl in tillClose)
                {
                    cl = tempLoopVarCl;
                    if (countCash)
                    {

                        if (cl.Tender_Name.ToUpper() == "COMPTANT")
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR("COMP.", (short)7) + modStringPad.PadL((cl.Tender_Count).ToString(), (short)3) + modStringPad.PadL(cl.Entered.ToString("#,##0.00"), (short)10) +
                                modStringPad.PadL(cl.System.ToString("#,##0.00"), (short)10) +
                                modStringPad.PadL(cl.Balance.ToString("#,##0.00"), (short)10));
                        }
                        else
                        {

                            FileSystem.PrintLine(nH, modStringPad.PadR(cl.Tender_Name, (short)7) + modStringPad.PadL((cl.Tender_Count).ToString(), (short)3) +
                                modStringPad.PadL(cl.Entered.ToString("#,##0.00"), (short)10) +
                                modStringPad.PadL(cl.System.ToString("#,##0.00"), (short)10) +
                                modStringPad.PadL(cl.Balance.ToString("#,##0.00"), (short)10));
                        }
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(cl.Tender_Name, (short)7) + modStringPad.PadL((cl.Tender_Count).ToString(), (short)3) + Strings.Space(10) +
                            modStringPad.PadL(cl.System.ToString("#,##0.00"), (short)10));
                    }

                    ent = ent + cl.Entered;
                    sys = sys + cl.System;
                    dif = dif + cl.Balance;

                    excAdjE = excAdjE - cl.Entered * (decimal)(1 - cl.Exchange_Rate);
                    excAdjS = excAdjS - cl.System * (decimal)(1 - cl.Exchange_Rate);
                    excAdjD = excAdjD - cl.Balance * (decimal)(1 - cl.Exchange_Rate);
                }


                // These are the raw totals of the face values of the tenders.
                FileSystem.PrintLine(nH, modStringPad.PadL("_", modPrint.PRINT_WIDTH, "_"));
                if (countCash)
                {
                    //        Print #nH, PadR("TOTALS", 10) & _
                    //PadL(Format(T_Ent, "#,##0.00"), 10) & _
                    //PadL(Format(T_Sys, "#,##0.00"), 10) & _
                    //PadL(Format(T_Dif, "#,##0.00"), 10)
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)136), (short)10) +
                        modStringPad.PadL(ent.ToString("#,##0.00"), (short)10) +
                        modStringPad.PadL(sys.ToString("#,##0.00"), (short)10) +
                        modStringPad.PadL(dif.ToString("#,##0.00"), (short)10));

                    // Show Exchange Adjustments (may be positive or negative)
                    if (excAdjE != 0 | excAdjS != 0 | excAdjD != 0)
                    {
                        //            Print #nH, PadR("+ Exch Adj", 10) & _
                        //PadL(Format(Exc_Adj_E, "#,##0.00"), 10) & _
                        //PadL(Format(Exc_Adj_S, "#,##0.00"), 10) & _
                        //PadL(Format(Exc_Adj_D, "#,##0.00"), 10)

                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)8318), (short)10) +
                            modStringPad.PadL(excAdjE.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(excAdjS.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(excAdjD.ToString("#,##0.00"), (short)10));
                    }

                    // Subtract the float
                    if (tillClose.Float != 0)
                    {
                        //            Print #nH, PadR("- Float", 10) & _
                        //PadL(Format(-Me.Float, "#,##0.00"), 10) & _
                        //PadL(Format(-Me.Float, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)221), (short)10) +
                            modStringPad.PadL((-tillClose.Float).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.Float).ToString("#,##0.00"), (short)10));
                    }

                    // Remove Store Credits issued
                    if (tillClose.Credits_Issued != 0)
                    {
                        //            Print #nH, PadR("- Credits", 10) & _
                        //PadL(Format(Me.Credits_Issued, "#,##0.00"), 10) & _
                        //PadL(Format(Me.Credits_Issued, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)8319), (short)10) +
                            modStringPad.PadL(tillClose.Credits_Issued.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(tillClose.Credits_Issued.ToString("#,##0.00"), (short)10));
                    }

                    // Subtract Payments Received
                    if (tillClose.Payments != 0)
                    {
                        //            Print #nH, PadR("- Payments", 10) & _
                        //PadL(Format(-Me.Payments, "#,##0.00"), 10) & _
                        //PadL(Format(-Me.Payments, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)215), (short)10) +
                            modStringPad.PadL((-tillClose.Payments).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.Payments).ToString("#,##0.00"), (short)10));
                    }

                    // Subtract AR Payments Received
                    if (tillClose.ARPay != 0)
                    {
                        //            Print #nH, PadR("- AR Pay ", 10) & _
                        //PadL(Format(-Me.ARPay, "#,##0.00"), 10) & _
                        //PadL(Format(-Me.ARPay, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)8320) + " ", (short)10) +
                            modStringPad.PadL((-tillClose.ARPay).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.ARPay).ToString("#,##0.00"), (short)10));
                    }


                    // Subtract Over Payments on Prepaid Sales
                    if (tillClose.OverPay != 0)
                    {
                        //            Print #nH, PadR("- OverPaid", 10) & _
                        //PadL(Format(-Me.OverPay, "#,##0.00"), 10) & _
                        //PadL(Format(-Me.OverPay, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)8321), (short)10) +
                            modStringPad.PadL((-tillClose.OverPay).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.OverPay).ToString("#,##0.00"), (short)10));
                    }

                    // Remove the till Draws
                    if (tillClose.Draw != 0)
                    {
                        //            Print #nH, PadR("- Draws", 10) & _
                        //PadL(Format(-Me.Draw, "#,##0.00"), 10) & _
                        //PadL(Format(-Me.Draw, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)218), (short)10) +
                            modStringPad.PadL((-tillClose.Draw).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.Draw).ToString("#,##0.00"), (short)10));
                    }
                    // Add the Till Drops to the Safe
                    if (tillClose.Drop != 0)
                    {
                        //            Print #nH, PadR("+ Drops", 10) & _
                        //PadL(Format(Me.Drop, "#,##0.00"), 10) & _
                        //PadL(Format(Me.Drop, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)219), (short)10) +
                            modStringPad.PadL(tillClose.Drop.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(tillClose.Drop.ToString("#,##0.00"), (short)10));
                    }
                    // Add PayOuts Made
                    if (tillClose.Payouts != 0)
                    {
                        //            Print #nH, PadR("+ Payouts", 10) & _
                        //PadL(Format(Me.Payouts, "#,##0.00"), 10) & _
                        //PadL(Format(Me.Payouts, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)216), (short)10) +
                            modStringPad.PadL(tillClose.Payouts.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(tillClose.Payouts.ToString("#,##0.00"), (short)10));
                    }

                    //   display total penny adjustments
                    if (tillClose.Penny_Adj != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("+/-" + _resourceManager.GetResString(offSet, (short)485), (short)10) +
                            modStringPad.PadL((-tillClose.Penny_Adj).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.Penny_Adj).ToString("#,##0.00"), (short)10));
                    }
                    //   end

                    if (tillClose.BonusFloat != 0 | tillClose.BonusDraw != 0 | tillClose.BonusDrop != 0 | tillClose.BonusGiveAway != 0)
                    {
                        FileSystem.PrintLine(nH, Strings.UCase(Convert.ToString(_policyManager.CBonusName)) + ":");
                    }
                    //  For Cash bonus Float
                    if (tillClose.BonusFloat != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)221), (short)10) +
                            modStringPad.PadL((-tillClose.BonusFloat).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.BonusFloat).ToString("#,##0.00"), (short)10));
                    }
                    //shiny end

                    //Shiny Mar2,2009 - Remove till Cash Bonus draws
                    if (tillClose.BonusDraw != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)218), (short)10) +
                            modStringPad.PadL((-tillClose.BonusDraw).ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL((-tillClose.BonusDraw).ToString("#,##0.00"), (short)10));
                    }
                    //shiny end

                    //Shiny Mar2,2009 - Add the Till Cash Bonus Drops to the Safe
                    if (tillClose.BonusDrop != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)219), (short)10) +
                            modStringPad.PadL(tillClose.BonusDrop.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(tillClose.BonusDrop.ToString("#,##0.00"), (short)10));
                    }
                    if (tillClose.BonusGiveAway != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)431), (short)10) +
                            modStringPad.PadL(tillClose.BonusGiveAway.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(tillClose.BonusGiveAway.ToString("#,##0.00"), (short)10));
                    }
                    //shiny end
                }
                else
                {
                    //        Print #nH, PadR("TOTALS", 10) & _
                    //Space(10) & _
                    //PadL(Format(T_Sys, "#,##0.00"), 10)
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)136), (short)10) + Strings.Space(10) +
                        modStringPad.PadL(sys.ToString("#,##0.00"), (short)10));

                    // Show Exchange Adjustments (may be positive or negative)
                    if (excAdjE != 0 | excAdjS != 0 | excAdjD != 0)
                    {
                        //            Print #nH, PadR("+ Exch Adj", 10) & _
                        //Space(10) & _
                        //PadL(Format(Exc_Adj_S, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)8318), (short)10) + Strings.Space(10) +
                            modStringPad.PadL(excAdjS.ToString("#,##0.00"), (short)10));
                    }

                    // Subtract the float
                    if (tillClose.Float != 0)
                    {
                        //            Print #nH, PadR("- Float", 10) & _
                        //Space(10) & _
                        //PadL(Format(-Me.Float, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)221), (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.Float).ToString("#,##0.00"), (short)10));
                    }
                    // Remove Store Credits issued
                    if (tillClose.Credits_Issued != 0)
                    {
                        //            Print #nH, PadR("- Credits", 10) & _
                        //Space(10) & _
                        //PadL(Format(Me.Credits_Issued, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)8319), (short)10) + Strings.Space(10) +
                            modStringPad.PadL(tillClose.Credits_Issued.ToString("#,##0.00"), (short)10));
                    }

                    // Subtract Payments Received
                    if (tillClose.Payments != 0)
                    {
                        //            Print #nH, PadR("- Payments", 10) & _
                        //Space(10) & _
                        //PadL(Format(-Me.Payments, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)215), (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.Payments).ToString("#,##0.00"), (short)10));
                    }

                    // Subtract AR Payments Received
                    if (tillClose.ARPay != 0)
                    {
                        //            Print #nH, PadR("- AR Pay ", 10) & _
                        //Space(10) & _
                        //PadL(Format(-Me.ARPay, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)215) + " ", (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.ARPay).ToString("#,##0.00"), (short)10));
                    }

                    // Subtract Over Payments on Prepaid Sales
                    if (tillClose.OverPay != 0)
                    {
                        //            Print #nH, PadR("- OP Adj.", 10) & _
                        //Space(10) & _
                        //PadL(Format(-(Me.OverPay), "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)8322), (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.OverPay).ToString("#,##0.00"), (short)10));
                    }

                    // Remove the till Draws
                    if (tillClose.Draw != 0)
                    {
                        //            Print #nH, PadR("- Draws", 10) & _
                        //Space(10) & _
                        //PadL(Format(-Me.Draw, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)218), (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.Draw).ToString("#,##0.00"), (short)10));
                    }

                    // Add the Till Drops to the Safe
                    if (tillClose.Drop != 0)
                    {
                        //            Print #nH, PadR("+ Drops", 10) & _
                        //Space(10) & _
                        //PadL(Format(Me.Drop, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)219), (short)10) + Strings.Space(10) +
                            modStringPad.PadL(tillClose.Drop.ToString("#,##0.00"), (short)10));
                    }

                    // Add PayOuts Made
                    if (tillClose.Payouts != 0)
                    {
                        //            Print #nH, PadR("+ Payouts", 10) & _
                        //Space(10) & _
                        //PadL(Format(Me.Payouts, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)216), (short)10) + Strings.Space(10) +
                            modStringPad.PadL(tillClose.Payouts.ToString("#,##0.00"), (short)10));
                    }

                    //   display total penny adjustments
                    if (tillClose.Penny_Adj != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("+/-" + _resourceManager.GetResString(offSet, (short)485), (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.Penny_Adj).ToString("#,##0.00"), (short)10));
                    }
                    //   end

                    if (tillClose.BonusFloat != 0 | tillClose.BonusDraw != 0 | tillClose.BonusDrop != 0)
                    {
                        FileSystem.PrintLine(nH, Strings.UCase(Convert.ToString(_policyManager.CBonusName)) + ":");
                    }
                    //  For Cash bonus Float
                    if (tillClose.BonusFloat != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)221), (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.BonusFloat).ToString("#,##0.00"), (short)10));
                    }
                    //shiny end

                    //Shiny Mar2,2009 - Remove till Cash Bonus draws
                    if (tillClose.BonusDraw != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("-" + _resourceManager.GetResString(offSet, (short)218), (short)10) + Strings.Space(10) +
                            modStringPad.PadL((-tillClose.BonusDraw).ToString("#,##0.00"), (short)10));
                    }
                    //shiny end
                    //Shiny Mar2,2009 - Add the Till Cash Bonus Drops to the Safe
                    if (tillClose.BonusDrop != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)219), (short)10) + Strings.Space(10) +
                            modStringPad.PadL(tillClose.BonusDrop.ToString("#,##0.00"), (short)10));
                    }
                    //shiny end
                    if (tillClose.BonusGiveAway != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR("+" + _resourceManager.GetResString(offSet, (short)219), (short)10) + Strings.Space(10) +
                            modStringPad.PadL(tillClose.BonusGiveAway.ToString("#,##0.00"), (short)10));
                    }

                }

                //  - to include cash Bonus
                ent = ent + excAdjE + tillClose.Credits_Issued - tillClose.Float - tillClose.BonusFloat - tillClose.Draw - tillClose.BonusDraw + tillClose.Drop + tillClose.BonusDrop + tillClose.BonusGiveAway - tillClose.Payments - tillClose.ARPay + tillClose.Payouts - tillClose.OverPay - tillClose.Penny_Adj;

                sys = sys + excAdjS + tillClose.Credits_Issued - tillClose.Float - tillClose.BonusFloat - tillClose.Draw - tillClose.BonusDraw + tillClose.Drop + tillClose.BonusDrop + tillClose.BonusGiveAway - tillClose.Payments - tillClose.ARPay + tillClose.Payouts - tillClose.OverPay - tillClose.Penny_Adj;
                //Shiny end
                dif = dif + excAdjD;

                // Totals after all adjustments
                if (excAdjE != 0 | excAdjS != 0 | excAdjD != 0 | tillClose.Credits_Issued != 0 | tillClose.OverPay != 0 | tillClose.Float != 0 | tillClose.Draw != 0 | tillClose.Drop != 0 | tillClose.BonusFloat != 0 | tillClose.BonusDraw != 0 | tillClose.BonusDrop != 0 | tillClose.BonusGiveAway != 0) // 
                {
                    FileSystem.PrintLine(nH, modStringPad.PadL("_", modPrint.PRINT_WIDTH, "_"));
                    if (countCash)
                    {
                        //            Print #nH, PadR("TOTALS", 10) & _
                        //PadL(Format(T_Ent, "#,##0.00"), 10) & _
                        //PadL(Format(T_Sys, "#,##0.00"), 10) & _
                        //PadL(Format(T_Dif, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)136), (short)10) +
                            modStringPad.PadL(ent.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(sys.ToString("#,##0.00"), (short)10) +
                            modStringPad.PadL(dif.ToString("#,##0.00"), (short)10));
                    }
                    else
                    {
                        //            Print #nH, PadR("TOTALS", 10) & _
                        //Space(10) & _
                        //PadL(Format(T_Sys, "#,##0.00"), 10)
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)136), (short)10) + Strings.Space(10) +
                            modStringPad.PadL(sys.ToString("#,##0.00"), (short)10));
                    }
                }

                // Summarize the transactions by type
                FileSystem.PrintLine(nH);
                //    Print #nH, "Transaction Summary"
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8323));

                FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));

                var saleHeads = _tillCloseService.GetSaleHeads(till.Number, whereClause, dataSource);

                foreach (var saleHead in saleHeads)
                {



                    //                   PadL(Format(rs![Transactions], "0"), 8) & _
                    //                   PadL(Format(rs![Amount] + rs![Pays], "#,##0.00"), 12)
                    if (Strings.UCase(saleHead.TType) == "REFUND")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8362), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));
                    }
                    else if (Strings.UCase(saleHead.TType) == "SALE")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8363), (short)20) + modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));
                    }
                    else if (Strings.UCase(saleHead.TType) == "VOID")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)121), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));
                    }
                    else if (Strings.UCase(saleHead.TType) == "ARPAY")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8320), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));
                    }
                    else if (Strings.UCase(saleHead.TType) == "PAYOUT")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)261), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));

                    }
                    else if (Strings.UCase(saleHead.TType) == "BTL RTN")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)1248), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));



                    }
                    else if (Strings.UCase(saleHead.TType) == "RUNAWAY")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8405), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));

                        //  - Pump test
                    }
                    else if (Strings.UCase(saleHead.TType) == "PUMPTEST")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)454), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));
                        //shiny end
                    }
                    else
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(modStringPad.Proper_Case(saleHead.TType), (short)20) +
                            modStringPad.PadL(saleHead.CloseNum.ToString("0"), (short)8) +
                            modStringPad.PadL((Convert.ToDouble(saleHead.SaleAmount) + Convert.ToDouble(saleHead.Payment)).ToString("#,##0.00"), (short)12));
                    }

                }
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                var transactions = _tillCloseService.GetTotalTransactions(till.Number, whereClause, dataSource);
                //    Print #nH, PadR("Total Transactions", 20) & PadL(Format(rs![Transactions], "0"), 8)
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8324), (short)20) +
                    modStringPad.PadL(transactions.ToString("0"), (short)8));


                string strTranType = "";

                if (_tillCloseService.AreCardSalesAvailable(till.Number, whereClause, dataSource))
                {
                    // Summarize the GiveX transactions by type
                    FileSystem.PrintLine(nH);
                    //"GiveX Transaction Summary"
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8371));
                    FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                    var givexCardSales = _tillCloseService.GetCardSales(till.Number, whereClause, dataSource);
                    foreach (var cardSale in givexCardSales)
                    {
                        strTranType = "";
                        if (cardSale.SaleType == "1")
                        {
                            strTranType = _resourceManager.GetResString(offSet, (short)3206); //Activation
                        }
                        else if (cardSale.SaleType == "2")
                        {
                            strTranType = _resourceManager.GetResString(offSet, (short)102); //Cancel
                        }
                        else if (cardSale.SaleType == "3")
                        {
                            strTranType = _resourceManager.GetResString(offSet, (short)3209); //Adjustment
                        }
                        else if (cardSale.SaleType == "4")
                        {
                            strTranType = _resourceManager.GetResString(offSet, (short)3281); //BalanceInquiry
                        }
                        else if (cardSale.SaleType == "5")
                        {
                            strTranType = _resourceManager.GetResString(offSet, (short)3208); //Increment
                        }
                        else if (cardSale.SaleType == "6")
                        {
                            strTranType = _resourceManager.GetResString(offSet, (short)3214); //Redemption
                        }
                        else
                        {
                        }
                        FileSystem.PrintLine(nH, modStringPad.PadR(strTranType, (short)20) +
                            modStringPad.PadL(cardSale.LineNumber.ToString("0"), (short)8) +
                            modStringPad.PadL(cardSale.SaleAmount.Value.ToString("#,##0.00"), (short)12));
                    }
                    FileSystem.PrintLine(nH);
                }


                //   New cash balance section - we are considering only base currency
                string BT = "";
                string CBT = ""; // Cash bonus tender name
                decimal dbltotalCash = new decimal(); //Double
                decimal dbltotalChange = new decimal(); //Double
                decimal dbltotalPayout = new decimal(); //Double
                decimal dbltotalDrop = new decimal(); //Double
                decimal dblTotalBottleReturn = new decimal(); //As Double  ''12/21/06 Nancy
                decimal dblComputedBalance = new decimal(); //Double
                decimal dblactualbalance = new decimal(); //Double
                decimal dbltotalPayment = new decimal(); //Double
                decimal dbltotalARPay = new decimal(); //Double
                decimal dbltotalDraw = new decimal(); //Double
                                                      // 
                decimal dblBonusDraw; //Currency
                decimal dblBonusDrop; //Currency
                decimal dblBonusCash = new decimal(); //Double
                decimal dblActualBonusBalance = new decimal();
                decimal dblComputedBonusBalance = new decimal();
                // 

                BT = Convert.ToString(_policyManager.BASECURR);
                CBT = _tenderService.GetTenderName(_policyManager.CBonusTend); // shiny Mar3,2009
                FileSystem.PrintLine(nH);



                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8357));


                FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                if (tillClose.Float != 0)
                {


                    ///                       PadL(Format(Me.Float, "#,##0.00"), 10)
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8358), (short)30) +
                        modStringPad.PadL(tillClose.Float.ToString("#,##0.00"), (short)10));

                }
                dbltotalCash = _tillCloseService.GetTotalCash(till.Number, whereClause, BT, dataSource);
                dbltotalChange = _tillCloseService.GetTotalChange(till.Number, whereClause, dataSource);

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8344), (short)30) +
                    modStringPad.PadL((dbltotalCash + dbltotalChange).ToString("#,##0.00"), (short)10));
                //Cash Payment
                dbltotalPayment = _tillCloseService.GetTotalPayment(till.Number, whereClause, BT, dataSource);
                if (dbltotalPayment != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8345), (short)30) +
                        modStringPad.PadL(dbltotalPayment.ToString("#,##0.00"), (short)10));
                }
                // Cash AR Payment
                dbltotalARPay = _tillCloseService.GetTotalArPayment(till.Number, whereClause, BT, dataSource);
                if (dbltotalARPay != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8346), (short)30) +
                        modStringPad.PadL(dbltotalARPay.ToString("#,##0.00"), (short)10));
                }
                //Cash Draw
                dbltotalDraw = _tillCloseService.GetTotalDraw(till.Number, shiftDate, whereClause, dataSource);
                if (dbltotalDraw != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8347), (short)30) +
                        modStringPad.PadL(dbltotalDraw.ToString("#,##0.00"), (short)10));
                }

                //Cash Payout
                dbltotalPayout = _tillCloseService.GetTotalPayout(till.Number, whereClause, dataSource);
                if (dbltotalPayout != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8349), (short)30) +
                        modStringPad.PadL(dbltotalPayout.ToString("#,##0.00"), (short)10));
                }
                //Cash drop
                dbltotalDrop = _tillCloseService.GetTotalDrop(till.Number, whereClause, shiftDate, BT, dataSource);
                if (dbltotalDrop != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8350), (short)30) +
                        modStringPad.PadL(dbltotalDrop.ToString("#,##0.00"), (short)10));
                }


                dblTotalBottleReturn = _tillCloseService.GetTotalBottleReturn(till.Number, whereClause, dataSource);
                if (dblTotalBottleReturn != 0)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)1248) + ": (-)", (short)30) +
                        modStringPad.PadL(System.Math.Abs(dblTotalBottleReturn).ToString("#,##0.00"), (short)10));
                }




                dblComputedBalance = tillClose.Float + (dbltotalCash + dbltotalChange) + dbltotalPayment + dbltotalARPay + dbltotalDraw - dbltotalPayout - dbltotalDrop + dblTotalBottleReturn;


                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8351), (short)30) +
                    modStringPad.PadL(dblComputedBalance.ToString("#,##0.00"), (short)10));
                dblactualbalance = 0;
                cl = new Close_Line();
                foreach (Close_Line tempLoopVar_CL in tillClose)
                {
                    cl = tempLoopVar_CL;
                    if (countCash)
                    {
                        if (cl.Tender_Name.ToUpper() == BT.ToUpper())
                        {
                            dblactualbalance = cl.Entered;
                            break;
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8352), (short)30) +
                    modStringPad.PadL(dblactualbalance.ToString("#,##0.00"), (short)10));
                if ((dblComputedBalance - dblactualbalance) > 0)
                {


                    ///                       PadL(Format((Abs(dblComputedBalance - dblactualbalance)), "(#,##0.00)"), 10)
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8353), (short)30) +
                        modStringPad.PadL(System.Math.Abs(dblComputedBalance - dblactualbalance).ToString("(#,##0.00)"), (short)10));

                }
                else if ((dblComputedBalance - dblactualbalance) < 0)
                {


                    ///                       PadL(Format(Abs(dblComputedBalance - dblactualbalance), "#,##0.00"), 10)
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8354), (short)30) +
                        modStringPad.PadL(System.Math.Abs(dblComputedBalance - dblactualbalance).ToString("#,##0.00"), (short)10));

                }
                FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                //shiny end

                //Shiny Mar2,2009 - For cash Bonus Summary section
                if (_policyManager.CashBonus)
                {
                    FileSystem.PrintLine(nH);

                    FileSystem.PrintLine(nH, _policyManager.CBonusName + " " + _resourceManager.GetResString(offSet, (short)425));
                    FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                    if (tillClose.BonusFloat != 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)426), (short)30) +
                            modStringPad.PadL(tillClose.BonusFloat.ToString("#,##0.00"), (short)10));
                    }
                    dblBonusCash = _tillCloseService.GetBonusCash(CBT, till.Number, dataSource);

                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)427), (short)30) + modStringPad.PadL(dblBonusCash.ToString("#,##0.00"), (short)10));


                    if (tillClose.BonusDraw != 0) // dblBonusDraw <> 0 Then
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)428), (short)30) + modStringPad.PadL(tillClose.BonusDraw.ToString("#,##0.00"), (short)10));
                    }

                    if (tillClose.BonusDrop != 0) //dblBonusDrop <> 0 Then
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)429), (short)30) + modStringPad.PadL(tillClose.BonusDrop.ToString("#,##0.00"), (short)10));
                    }

                    if (tillClose.BonusGiveAway != 0) //dblBonusDrop <> 0 Then
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)430), (short)30) + modStringPad.PadL(tillClose.BonusGiveAway.ToString("#,##0.00"), (short)10));
                    }

                    dblComputedBonusBalance = tillClose.BonusFloat + dblBonusCash + tillClose.BonusDraw - tillClose.BonusDrop - tillClose.BonusGiveAway;

                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8351), (short)30) + modStringPad.PadL(dblComputedBonusBalance.ToString("#,##0.00"), (short)10));
                    dblActualBonusBalance = 0;
                    cl = new Close_Line();
                    foreach (Close_Line tempLoopVar_CL in tillClose)
                    {
                        cl = tempLoopVar_CL;
                        if (countCash)
                        {
                            if (cl.Tender_Name.ToUpper() == CBT.ToUpper())
                            {
                                dblActualBonusBalance = Convert.ToDecimal((Information.IsDBNull(cl.Entered)) ? 0 : cl.Entered);
                                break;
                            }
                        }
                    }

                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8352), (short)30) + modStringPad.PadL(dblActualBonusBalance.ToString("#,##0.00"), (short)10));
                    if ((dblComputedBonusBalance - dblActualBonusBalance) > 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8353), (short)30) + modStringPad.PadL(System.Math.Abs(dblComputedBonusBalance - dblActualBonusBalance).ToString("(#,##0.00)"), (short)10));
                    }
                    else if ((dblActualBonusBalance - dblActualBonusBalance) < 0)
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8354), (short)30) + modStringPad.PadL(System.Math.Abs(dblComputedBonusBalance - dblActualBonusBalance).ToString("#,##0.00"), (short)10));
                    }

                    FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                }
                // 

                //  for payout summary
                // Get the payouts value - can be only in the base tender

                double dblPayout = 0;
                double dblTendPayout = 0;
                short intTendCount = 0;
                short intPayoutCount = 0;
                double PayoutTotal = 0;
                short PayoutCount = 0;

                short cnt = 0;
                cnt = 0;
                var payouts = _tillCloseService.GetPayoutReasons();
                if (payouts.Count != 0)
                {
                    //   Print #nH,
                    //        Print #nH, GetResString(8342) '"Payout Summary"
                    //        Print #nH, String(PRINT_WIDTH, "_")
                    dblPayout = 0;
                    dblTendPayout = 0;
                    PayoutTotal = 0;
                    intTendCount = (short)0;
                    intPayoutCount = (short)0;
                    foreach (var payout in payouts)
                    {
                        var saleHead = _tillCloseService.GetPayoutSaleHead(payout.Reason, till.Number, whereClause, dataSource);
                        dblPayout = Convert.ToDouble(saleHead.SaleAmount);
                        intPayoutCount = Convert.ToInt16(saleHead.CloseNum);
                        if (intPayoutCount > 0)
                        {
                            cnt++;
                        }

                        dblTendPayout = 0;
                        intTendCount = (short)0;
                        if (cnt == 1)
                        {
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8342)); //"Payout Summary"
                            FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                            cnt++;
                        }
                        var links = _tillCloseService.GetTenderDescriptions(payout.Reason);
                        foreach (var link in links)
                        {
                            var saleTend = _tillCloseService.GetPayoutSaleTend(link, till.Number, whereClause, dataSource);
                            dblTendPayout = Convert.ToDouble(dblTendPayout + Convert.ToDouble(saleTend.AmountUsed));
                            intTendCount = Convert.ToInt16(intTendCount + Convert.ToInt32(saleTend.AmountTend));
                            if (intTendCount > 0)
                            {
                                cnt++;
                            }
                            if (cnt == 1)
                            {
                                FileSystem.PrintLine(nH);
                                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8342)); //"Payout Summary"
                                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                                cnt++;
                            }
                        }
                        PayoutTotal = PayoutTotal + (dblPayout + dblTendPayout);
                        PayoutCount = (short)(PayoutCount + (short)(intPayoutCount + intTendCount));
                        if ((intPayoutCount + intTendCount) != 0)
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(payout.Description, (short)25) + modStringPad.PadL((intPayoutCount + intTendCount).ToString("###0"), (short)5) + modStringPad.PadL((dblPayout + dblTendPayout).ToString("###0.00"), (short)10));
                        }
                    }
                    if (cnt > 0)
                    {
                        FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8343), (short)25) + modStringPad.PadL(PayoutCount.ToString("###0"), (short)5) + modStringPad.PadL(PayoutTotal.ToString("###0.00"), (short)10));
                        FileSystem.PrintLine(nH);
                    }
                }
                // 

                // Summarize Sales Voided and Individual Lines Deleted.
                if (!string.IsNullOrEmpty(whereClause))
                {
                    var deletedLines = _tillCloseService.GetDeletedLines(shiftDate, dataSource);
                    if (deletedLines.Count != 0)
                    {
                        //            Print #nH,
                        FileSystem.PrintLine(nH);
                        //            Print #nH, "Void & Deleted Lines"
                        FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8325));

                        FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                        foreach (var deletedLine in deletedLines)
                        {
                            //                Print #nH, PadR(IIf(rs![Action] = "D", "Line Deleted", "Voided Lines"), 30) & _
                            //PadL(rs![Transactions], 10)
                            FileSystem.PrintLine(nH, modStringPad.PadR((deletedLine.LINE_TYPE == "D") ? (_resourceManager.GetResString(offSet, (short)8326)) : (_resourceManager.GetResString(offSet, (short)8327)), (short)30) + modStringPad.PadL(Convert.ToString(deletedLine.AvailItems), (short)10));
                        }
                    }
                }
                else
                {
                    //added by binal 22 / 2 / 2002 since voided and deleted sales are not printing in Print Till Close
                    var deletedLines = _tillCloseService.GetDeletedLinesForTill(till.Number, dataSource);
                    if (deletedLines.Count != 0)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH);
                        //            Print #nH, "Void & Deleted Lines"
                        FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8325));

                        FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                        foreach (var deletedLine in deletedLines)
                        {
                            //                Print #nH, PadR(IIf(rs![Action] = "D", "Line Deleted", "Voided Lines"), 30) & _
                            //PadL(rs![Transactions], 10)
                            FileSystem.PrintLine(nH, modStringPad.PadR((deletedLine.LINE_TYPE == "D") ? (_resourceManager.GetResString(offSet, (short)8326)) : (_resourceManager.GetResString(offSet, (short)8327)), (short)30) + modStringPad.PadL(Convert.ToString(deletedLine.AvailItems), (short)10));
                        }
                    }
                    //       binal 22 / 2 / 2002
                }
                if (allShifts)
                {
                    Product_Sales_Report(till, tillClose, nH, whereClause, true);
                }
                else
                {
                    Product_Sales_Report(till, tillClose, nH, whereClause, false);
                }
                string[] stringArr = null;
                if (!string.IsNullOrEmpty(modTPS.cc.Report))
                {
                    if (modTPS.cc.Report.Contains("HOST RESPONSE : "))
                    {
                       stringArr = modTPS.cc.Report.Split((new string[] { "HOST RESPONSE : " }), StringSplitOptions.None);
                       stringArr[0] =  stringArr[0] + "HOST RESPONSE : " ;
                    }
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH);
                    FileSystem.Print(nH, new string(' ', modPrint.PRINT_WIDTH/3));
                    FileSystem.PrintLine(nH,"WEX RECIEPT");
                    FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, stringArr[0]);
                    FileSystem.PrintLine(nH, stringArr[1]);
                    FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                }

            }
            finally
            {
                FileSystem.FileClose(nH);
            }

        }

        /// <summary>
        /// Method to validate till close
        /// </summary>
        /// <param name="tillNumber">Till close</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        public TillCloseResponse ValidateTillClose(int tillNumber, int saleNumber, string userCode, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var till = _tillService.GetTill(tillNumber);
            var tillCloseResponse = new TillCloseResponse();
            // short Response;
            bool boolPrepay = false;


            WriteToLogFile("cmdExit click event"); //  


            if (sale.DeletePrepay)
            {
                //Please complete delete prepay first!~Comlete current transaction.
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)50, null);
                return null;
            }




            //  - Including the switch Prepay baskets too - "CHANGE PREPAY GRADE"
            if (!string.IsNullOrEmpty(Variables.SwitchPrepayBaskets))
            {
                //  - "CHANGE PREPAY GRADE" - Need to process any outstanding switch Prepay baskets - PM suggested to do it here( need to do it here and before exiting out from POS)
                string changeDue;
                bool openDrawer;
                Report fs;
                _fuelPrepayManager.Finish_SwitchPrepayBaskets(tillNumber, out changeDue,
                    out openDrawer, out fs);
            }
            // 
            //  - Give warning if still it is outstanding - somehow can't communicate to FC
            //    If (Not rs_Prepay.EOF) Or Trim(MyPrepayBaskets) <> "" Then
            if ((_tillCloseService.IsPrepayGlobalsPresent(tillNumber)) || !string.IsNullOrEmpty(Variables.MyPrepayBaskets) || !string.IsNullOrEmpty(Variables.SwitchPrepayBaskets))
            {
                // 



                boolPrepay = true;




                tillCloseResponse.PrepayMessage = _resourceManager.CreateMessage(offSet, 11, 71, null, MessageType.YesNo);




                //if (ans == (int)MsgBoxResult.No)
                //{
                //    

                //    rs_Prepay = null;
                //    return;
                //}
            }

            if (sale.Sale_Lines.Count > 0)
            {
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 67, null);
                return null;
            }


            //Binal Sept10,2002 for TillClose
            // Nicolette added to check if we have to close databases here or in TillClose form.


            MessageStyle suspendMessage = null;
            var isSuspendedSale = Suspended_Sales(tillNumber, out suspendMessage);
            if (_policyManager.U_TILLCLOSE && !boolPrepay && !isSuspendedSale)
            {

                //        Ans = MsgBox("Do You Want to Close the Current Till ?", vbQuestion + vbYesNo, "Closing Current Till")





                if (_policyManager.U_TOTREAD && _policyManager.USE_FUEL)
                {



                    if (_policyManager.LastShift == 0)
                    {




                        tillCloseResponse.CloseTillMessage = _resourceManager.CreateMessage(offSet, 11, 72, null, MessageType.YesNoCancel);
                        Variables.blCloseTill = true; // down
                                                      //Do You Want to process totalizer reading? ~ Read totalizer.
                                                      //MsgBoxStyle temp_VbStyle6 = (int)MsgBoxStyle.Question + MsgBoxStyle.YesNo;
                                                      //AnsTotalizer = (short)(Chaps_Main.DisplayMessage(this, (short)49, temp_VbStyle6, null, (byte)0));
                        tillCloseResponse.ReadTotalizerMessage = _resourceManager.CreateMessage(offSet, 11, 49, null, MessageType.YesNo);



                        //                Me.MousePointer = vbHourglass

                        // False  June 05, 2009 Nicolette based on Mr. Gas requirement to authorize pumps while reading totalizer




                        if (_policyManager.TankGauge && _policyManager.U_DipRead)
                        {
                            //   added Next If; if policy is Yes, don't ask question but process the dip reading
                            if (_policyManager.ASK_DIPREAD)
                            {

                                tillCloseResponse.TankDipMessage = _resourceManager.CreateMessage(offSet, 11, 24, null, MessageType.YesNo);
                            }
                            else
                            {
                                tillCloseResponse.ProcessTankDip = true;
                            }

                        }

                    }
                    else
                    {

                        if (till.Shift == _policyManager.LastShift)
                        {
                            //  - made the last till for last shift checking as a function
                            if (_tillCloseService.IsLastTill(till))
                            {
                                tillCloseResponse.ReadTotalizerMessage = _resourceManager.CreateMessage(offSet, 11, (short)66, null, MessageType.YesNoCancel);
                                Variables.blCloseTill = true; // down
                                                              //                Me.MousePointer = vbHourglass




                                if (_policyManager.TankGauge && _policyManager.U_DipRead)
                                {
                                    //   Next If; if policy is Yes, don't ask question but process the dip reading
                                    if (_policyManager.ASK_DIPREAD)
                                    {

                                        tillCloseResponse.TankDipMessage = _resourceManager.CreateMessage(offSet, 11, (short)24, null, MessageType.YesNo);
                                    }
                                    else
                                    {
                                        tillCloseResponse.ProcessTankDip = true;
                                    }
                                }

                            }
                            else
                            {
                                tillCloseResponse.CloseTillMessage = _resourceManager.CreateMessage(offSet, 11, 72, null, MessageType.YesNoCancel);
                            }
                        }
                        else
                        {
                            tillCloseResponse.CloseTillMessage = _resourceManager.CreateMessage(offSet, 11, 72, null, MessageType.YesNoCancel);
                        }
                    }

                }
                else
                {
                    tillCloseResponse.CloseTillMessage = _resourceManager.CreateMessage(offSet, 11, 72, null, MessageType.YesNoCancel);
                }

            }
            else
            {
                tillCloseResponse.EndSaleSessionMessage = _resourceManager.CreateMessage(offSet, 11, (short)98, null, MessageType.YesNo);


            }
            tillCloseResponse.SuspendSaleMessage = suspendMessage;

            return tillCloseResponse;

        }

        /// <summary>
        /// Method to save sale and close till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Close current till response</returns>
        public CloseCurrentTillResponseModel TillClose(int tillNumber, int saleNumber, string userCode, out ErrorMessage error)
        {
            CacheManager.DeleteTillCloseModel(tillNumber);
            var closeTillResponse = new CloseCurrentTillResponseModel();
            //Variables.IsChildForm = false; //Nancy

            //blCloseOPOS = false;

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                return null;
            var till = _tillService.GetTill(tillNumber);
            // 'We can only authorize the pump
            if (!_policyManager.AuthPump_Eod)
            {
                //regular till close
                //  - When Authpump policy is false  Customer display is crashing because all forms except frmTillclose is exiting out after showing( since it was not showing modal it was continuing the code) - Control for customer display is in MDI form. Keeping Salemain and MDI form  until we finish till lose and then exiting out after the till close
                //call frmTillClose.Show
                //  closebatch not getting connection
                if (_policyManager.EOD_CLOSE)
                {
                    // this.wskTPS.Close(); //   Close Batch didn't work in Till Close form. Close Batch in TillClose form uses another winsock control, so we have to disconect this one
                }
                if (_policyManager.EOD_CLOSE)
                {
                    // this.WexTPS.Close(); // 2013 12 12 - Reji for WEX Fleet card
                }
                if (_policyManager.ThirdParty)
                {
                    // MDIfrmPump.Default.wtcpMilliplein.Close(); //   see above comment line
                }
                // 
                //  frmTillClose.Default.ShowDialog();
            }
            //Do a till close and user can authorize pumps
            Tenders tdrs = null;
            //Before we can close the till, we need to add CANCEL sale and clear cursale
            sale.Sale_Type = "CANCEL";
            _saleManager.SaveSale(sale, userCode, ref tdrs, null);

            // Chaps_Main.SA = null;

            // Tbx = null;


            //Binal Sept10,2002
            till.Processing = false;
            _tillService.UpdateTill(till);





            //    If mscCustDisplay.PortOpen Then mscCustDisplay.PortOpen = False

            _tillCloseService.DeleteCurrentSale(tillNumber);

            //if (_policyManager.EOD_CLOSE)
            //{
            //    // this.wskTPS.Close(); //   Close Batch didn't work in Till Close form. Close Batch in TillClose form uses another winsock control, so we have to disconect this one
            //}
            //if (_policyManager.EOD_CLOSE)
            //{
            //    // this.WexTPS.Close(); // 2013 12 12 - Reji for WEX Fleet card
            //}
            //if (_policyManager.ThirdParty)
            //{
            //    // MDIfrmPump.Default.wtcpMilliplein.Close(); //   see above comment line
            //}

            Close_A_Till((short)tillNumber, ref closeTillResponse, out error);
            if (closeTillResponse.ShowBillCoins)
                closeTillResponse.BillCoins = _tillCloseService.GetBillCoins();
            // 
            Register register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);
            if (register.Customer_Display)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                //        Call DisplayMsgLcd(GetResString(451), "")   ' Closing the Till.........
                closeTillResponse.CustomerDisplay = _mainManager.DisplayMsgLcd(register, _mainManager.FormatLcdString(register,
                       _resourceManager.GetResString(offSet, (short)451), ""), "");
            }
            // 

            CacheManager.AddTillCloseModel(tillNumber, closeTillResponse);

            return closeTillResponse;
        }

        /// <summary>
        /// Method to end a sale session
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public bool EndSale(int tillNumber, int saleNumber, string userCode, out ErrorMessage error)
        {
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                return false;
            var till = _tillService.GetTill(tillNumber);
            sale.Sale_Type = "CANCEL";
            Tenders tdrs = null;
            _saleManager.SaveSale(sale, userCode, ref tdrs, null);

            //Tbx = null;


            //Binal Sept10,2002
            till.Processing = false;
            _tillService.UpdateTill(till);





            //    If mscCustDisplay.PortOpen Then mscCustDisplay.PortOpen = False


            _tillCloseService.DeleteCurrentSale(tillNumber);
            Variables.closeMDI = true;
            //for testing
            return true;
        }

        /// <summary>
        /// Method to read tank dip
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public bool ReadTankDip(int tillNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                error.MessageStyle = new MessageStyle { Message = "Till does not exists" };
                return false;
            }
            float timeIn = 0;
            string response = "";
            string strRemain = "";
            bool boolNegTemp = false;

            response = "";
            strRemain = "";

            string tempCommandRenamed = "DIP";
            try
            {
                _fuelPumpManager.LoadPumps(tillNumber);
            }
            catch
            {
                //error.MessageStyle = new MessageStyle { Message = "Unable to connect to server" };
                return false;
            }
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

            if (timeIn > DateAndTime.Timer)
            {
                timeIn = 0; //reset on midnight
            }
            else
            {
                timeIn = (float)DateAndTime.Timer;
            }
            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
            {

                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting DIP");
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "DIP", ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {

                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify TCPAgent.PortReading from reading tank dip: " + strRemain);
                        break;
                    }
                }
                Variables.Sleep(100);
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
            }

            if (response != null && ((response.Length <= 16) || (response.Substring(0, 3) != "DIP") || (response.Substring(response.Length - 2, 2) != "OK")))
            {
                goto Error1;
            }


            if (response != null)
            {
                response = response.Substring(3, response.Length - 5);
                var readTime = DateTime.Parse(response.Substring(0, 2) + "/" + response.Substring(2, 2) + "/" + Convert.ToString(DateAndTime.Year(DateAndTime.Today)) + " " + response.Substring(4, 2) + ":" + response.Substring(6, 2) + ":00");
                response = response.Substring(11);

                //   to consider negative temperatures
                boolNegTemp = response.IndexOf("-") + 1 > 0;
                var arrMinus = new short[1]; //   to consider negative amounts for temperature
                var arrTanks = new short[1]; // positive
                var originalResponse = response;
                short k = 0;
                if (boolNegTemp)
                {
                    k = 1;
                    while (k <= response.Length)
                    {
                        var pos = (short)(k.ToString().IndexOf(response) + 1);
                        if (pos > 0)
                        {
                            var j = (short)((arrMinus.Length - 1) + 1);
                            Array.Resize(ref arrMinus, j + 1);
                            arrMinus[j] = pos;
                            k = (short)(pos + 1);
                        }
                        else
                        {
                            k++;
                        }
                    }
                    //            Response = Replace(Response, "-", "", 1)    ' eliminate the minus sign, Arr_Minus contains the positions of minus sign in the original string
                    response = Strings.Replace(response, "-", "0", 1); // true
                }
                //   end




                //------------------------------------------------
                //Added by Dmitry to diffenciate between Incon and Veeder-Root
                var systemType = _tillCloseService.GetSystemType();
                short numchars = 0; //added by Dmitry to save number of bytes for TankDIP device response
                short intBlockSize = 0;
                if (systemType == "Incon")
                {
                    if (response != null && response.Length % 31 != 0)
                    {
                        goto Error1;
                    }
                    numchars = 31;
                    intBlockSize = 31;
                }
                else
                {
                    if (response != null && response.Length % 32 != 0)
                    {
                        goto Error1;
                    }
                    numchars = 32;
                    intBlockSize = 32;
                }

                //End Dmitry

                //   for negative temperatures; set the negative tanks sign
                if (boolNegTemp)
                {
                    arrTanks = new short[originalResponse.Length / intBlockSize + 1];
                    for (k = 1; k <= (double)originalResponse.Length / intBlockSize; k++)
                    {
                        if (k <= (arrTanks.Length - 1) && k <= (arrMinus.Length - 1))
                        {
                            arrTanks[k] = (short)((arrMinus[k] > (short)(k - 1) * numchars) && (arrMinus[k] < k * numchars) ? 1 : (short)0);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                //   end


                var totMg = (short)_tillCloseService.GetMaximumDipNumber();
                //Added by Dmitry to save data in DipEvents table
                _tillCloseService.AddDipEvent(totMg, till.ShiftDate);
                //End Dmitry
                var posStart = (short)1;
                k = (short)0; // data
                while (response != null && posStart < response.Length)
                {
                    var strReading = response.Substring(posStart - 1, numchars);
                    k++; // data

                    var i = (short)(Conversion.Val(strReading.Substring(0, 1)));
                    var fuelDip = float.Parse((Conversion.Val(strReading.Substring(6, 5)) / 100).ToString("#0.00"));
                    var volume = int.Parse(strReading.Substring(11, 6));
                    var temp = float.Parse((Conversion.Val(strReading.Substring(17, 5)) / 10).ToString("#0.0"));
                    // temperature
                    if (boolNegTemp)
                    {
                        if (k <= (arrTanks.Length - 1))
                        {
                            if (arrTanks[k] == 1)
                            {
                                temp = (-1) * temp;
                            }
                        }
                    }
                    //   end
                    var vllage = int.Parse(strReading.Substring(22, 6));
                    var waterDip = float.Parse((Conversion.Val(strReading.Substring(28, 3)) / 10).ToString("#0.0"));
                    var tankDip = new TankDip
                    {
                        DipNumber = totMg,
                        TankId = i,
                        FuelDip = fuelDip,
                        WaterDip = waterDip,
                        Temperature = temp,
                        Date = DateTime.Today,
                        ShiftDate = till.ShiftDate,
                        ReadTime = readTime,
                        GradeId = _getPropertyManager.get_TankInfo((byte)i).GradeID,
                        Volume = volume,
                        Vllage = vllage
                    };

                    _tillCloseService.SaveTankDip(tankDip);


                    posStart = (short)(posStart + numchars);
                }
            }
            return true;

        Error1:
            return false;
        }

        /// <summary>
        /// Method to update till close
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <param name="entered">Entered amount</param>
        /// <param name="description">Bill coin</param>
        /// <param name="amount">Amount</param>
        /// <param name="error">Error message</param>
        /// <returns>Close current till response</returns>
        public CloseCurrentTillResponseModel UpdateTillClose(int tillNumber, string tenderName, decimal entered, string description,
            decimal amount, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var closeTillResponseModel = CacheManager.GetTillCloseModel(tillNumber);
            if (closeTillResponseModel == null)
            {
                error.MessageStyle = new MessageStyle { Message = Utilities.Constants.InvalidRequest };
                return null;
            }
            var tillClose = _tillCloseService.GetTillCloseByTillNumber(tillNumber);
            if (!closeTillResponseModel.ShowBillCoins && !string.IsNullOrEmpty(description))
            {
                error.MessageStyle = new MessageStyle { Message = Utilities.Constants.InvalidRequest };
                return null;
            }

            if ((tillClose.Count == 0 || !closeTillResponseModel.ShowEnteredField) && !string.IsNullOrEmpty(tenderName))
            {
                error.MessageStyle = new MessageStyle { Message = Utilities.Constants.InvalidRequest };
                return null;
            }

            if (!string.IsNullOrEmpty(tenderName))
            {
                if (entered > 10000000000)
                {
                    error.MessageStyle = new MessageStyle { Message = Utilities.Constants.MaxLimitExceeded };
                    return null;
                }
                var selectedTender = tillClose.FirstOrDefault(t => t.Tender == tenderName);
                if (selectedTender != null)
                {
                    selectedTender.Entered = Convert.ToDouble(entered);
                    selectedTender.Difference = selectedTender.System - Convert.ToDouble(entered);
                    _tillCloseService.UpdateTillClose(selectedTender);
                }
                else
                {
                    error.MessageStyle = new MessageStyle { Message = Utilities.Constants.InvalidRequest };
                    return null;
                }
            }

            else if (!string.IsNullOrEmpty(description))
            {
                if (amount > 9999)
                {
                    error.MessageStyle = new MessageStyle { Message = Utilities.Constants.MaxLimitExceeded };
                    return null;
                }
                var billCoins = closeTillResponseModel.BillCoins;
                var selectedBillCoin = billCoins.FirstOrDefault(b => b.Description == description);
                if (selectedBillCoin != null)
                {
                    var originalBillCoins = _tillCloseService.GetBillCoins();
                    var coin = originalBillCoins.FirstOrDefault(b => b.Description == description);
                    if (coin != null)
                    {
                        var value = coin.Value;
                        selectedBillCoin.Amount = amount.ToString();
                        selectedBillCoin.Value = (Conversion.Val(amount) * Conversion.Val(value)).ToString("0.00");
                    }
                    double total = 0;
                    foreach (var bill in billCoins)
                    {
                        if (bill.Amount == "0.00")
                        {
                            bill.Amount = "";
                        }
                        if (!string.IsNullOrEmpty(bill.Amount))
                        {
                            total += Conversion.Val(bill.Value);
                        }
                    }
                    closeTillResponseModel.Total = total.ToString("0.00");
                    closeTillResponseModel.BillCoins = billCoins;
                    var baseTender = tillClose.FirstOrDefault(t => t.Tender.ToLower() == _policyManager.BASECURR.ToLower());
                    if (baseTender != null)
                    {
                        baseTender.Entered = total;
                        baseTender.Difference = baseTender.Entered - baseTender.System;
                        _tillCloseService.UpdateTillClose(baseTender);
                    }
                }
                else
                {
                    error.MessageStyle = new MessageStyle { Message = Utilities.Constants.InvalidRequest };
                    return null;
                }
            }
            else
            {
                // click bill coin counter button
                // update tender
                closeTillResponseModel.Total = "0.00";
                closeTillResponseModel.BillCoins = _tillCloseService.GetBillCoins();
                var baseTender = tillClose.FirstOrDefault(t => t.Tender.ToLower() == _policyManager.BASECURR.ToLower());
                baseTender.Entered = 0;
                baseTender.Difference = baseTender.Entered - baseTender.System;
                _tillCloseService.UpdateTillClose(baseTender);
            }

            tillClose = _tillCloseService.GetTillCloseByTillNumber(tillNumber);
            closeTillResponseModel.Tenders = GetTenders(tillClose);
            CacheManager.AddTillCloseModel(tillNumber, closeTillResponseModel);
            return closeTillResponseModel;
        }

        /// <summary>
        /// Method to finish till close and print till close report
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="readTankDipSuccess">Read tank dip response</param>
        /// <param name="readTotalizerSuccess">Read totaliser response</param>
        /// <param name="customerDisplay">Customer display</param>
        /// <param name="error">Error message</param>
        /// <returns>Till close report</returns>
        public List<Report> FinishTillClose(int tillNumber, string userCode, int registerNumber, bool? readTankDipSuccess,
            bool? readTotalizerSuccess, out CustomerDisplay customerDisplay, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var reports = new List<Report>();
            string reportContent = string.Empty;
            var till = _tillService.GetTill(tillNumber);
            var user = _loginManager.GetExistingUser(userCode);
            var security = _policyManager.LoadSecurityInfo();
            customerDisplay = new CustomerDisplay();
            string message = "";
            modTPS.cc = new Credit_Card();
            modTPS.cc.Report = ""; 
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //
            if (_policyManager.AuthPump_Eod)
            {
                //go back to salemain form to be able to authorize pumps
                //this.Visible = false;
            }
            //End - SV
            string lblMessage;

            var closeBatchSuccess = false;
            if (_policyManager.CC_MODE == "Validate") //  - if cross-ring don't do closebatch
            {
                if (_policyManager.EOD_CLOSE)
                {


                    lblMessage = _resourceManager.CreateCaption(offSet, (short)91, Convert.ToInt16(46), null, (short)0);

                    // lblMessage.Visible = true;

                    ClearCloseBatchReport();
                    Report bankEod;
                    Report eodReport;
                    closeBatchSuccess = CloseBatch(till.Number, out error, out bankEod, out eodReport);
                    if (closeBatchSuccess)
                    {
                        if (bankEod != null)
                            reports.Add(bankEod);
                        if (eodReport != null)
                            reports.Add(eodReport);
                    }
                    else
                    {
                        message = error.MessageStyle.Message;
                    }
                }
            }

            if (_policyManager.Use_KickBack)
            {
                lblMessage = _resourceManager.CreateCaption(offSet, (short)96, Convert.ToInt16(46), null, (short)0);
                if (ProcessKickBackQueue(out error))
                {
                    //Kickback has succesfully processed outstanding points.
                    lblMessage = _resourceManager.CreateCaption(offSet, (short)94, Convert.ToInt16(46), null, (short)0);
                }
                else
                {
                    //Kickback has not succesfully processed outstanding points.
                    lblMessage = _resourceManager.CreateCaption(offSet, (short)95, Convert.ToInt16(46), null, (short)0);
                }
            }

            lblMessage = _resourceManager.CreateCaption(offSet, (short)92, Convert.ToInt16(46), null, (short)0);


            Finish_The_Close(till, readTotalizerSuccess, readTankDipSuccess, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                return null;

            if (_policyManager.TE_AUTOMATE && _policyManager.TAX_EXEMPT && _policyManager.TE_Type == "AITE") // if it's AITE  tax exempt
            {
                if (_policyManager.LastShift == 0 || (_policyManager.LastShift == till.Shift && _tillCloseService.IsLastTill(till)))
                {
                    var dailyTransactionFile = ExtractDailyTransactionFile();
                    reports.Add(dailyTransactionFile);
                }
            }
            // 

            if (_policyManager.EOD_CLOSE) //And CloseBatchSuccess Then
            {
                reportContent = reportContent + "\r\n" + GetCloseBatchReport();
            }


            if (_policyManager.PRT_ALLSHIFTS == till.Shift && _tillCloseService.IsLastTill(till))
            {
                Gen_AllShiftsReport(till);
            }
            // Nicolette, February 2016 end


            lblMessage = "";


            //

            if (_policyManager.AuthPump_Eod)
            {
                //Go back to salemain to be able to authorize pumps
                //If cmdPrint.Enabled = True Then
                // cmdFinish.Enabled = false;
                //End If
                //this.Visible = true;
            }
            //End - SV
            // 
            Register register = new Register();
            _mainManager.SetRegister(ref register, (short)registerNumber);
            if (register.Customer_Display)
            {

                //        Call DisplayMsgLcd(GetResString(452), "")   ' Till closed
                customerDisplay = _mainManager.DisplayMsgLcd(register, _mainManager.FormatLcdString(register,
                    _resourceManager.GetResString(offSet, (short)452), ""), "");
            }

            string content = string.Empty;
            var filePath = Path.GetTempPath() + "\\TillClose_" + PosId + ".txt";
            using (FileStream stream = File.OpenRead(filePath))
            {
                try
                {
                    //var stream = File.OpenRead(fileName);
                    //FileSystem.FileClose(fileNumber);
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
                    content = Convert.ToBase64String(bytes);
                }
                catch (Exception)
                {
                    //
                }
                finally
                {
                    stream.Close();
                    File.Delete(filePath);
                }
            }
            reports.Add(new Report
            {
                ReportName = "TillClose.txt",
                ReportContent = content,
                Copies = _policyManager.EOD_COPIES
            });
            if (string.IsNullOrEmpty(message)) return reports;
            error.MessageStyle.Message = message;
            error.StatusCode = HttpStatusCode.OK;
            return reports;
        }

        // 
        /// <summary>
        /// This function to extract the daily transaction file for AITE Tax Exempt
        /// </summary>
        public Report ExtractDailyTransactionFile()
        {
            var downLoadInfo = _tillCloseService.GetDownloadInfo();
            if (downLoadInfo == null)
            {
                mPrivateGlobals.gintTRA_CLOSENO = (short)0;
                mPrivateGlobals.gdateTRA_TRANSTIME = DateTime.Parse("01/01/2000 12:00:00");
                mPrivateGlobals.glngTRA_AHRNo = 0;
                mPrivateGlobals.glngTRA_RegistryNo = 0;
            }
            else
            {

                mPrivateGlobals.gintTRA_CLOSENO = downLoadInfo.Tra_Close_Num;

                mPrivateGlobals.gdateTRA_TRANSTIME = downLoadInfo.Tra_Trans_Time == DateTime.MinValue ? DateTime.Parse("01/01/2000 12:00:00") : downLoadInfo.Tra_Trans_Time;

                mPrivateGlobals.glngTRA_AHRNo = downLoadInfo.Tra_AHR_TransNo;

                mPrivateGlobals.glngTRA_RegistryNo = downLoadInfo.Tra_Registry_TransNo;
            }
            mPrivateGlobals.gdateCurrentTime = DateTime.Now;

            return CreateDailyTransactionFile();
        }

        /// <summary>
        /// Method to write AITE logs
        /// </summary>
        /// <param name="strMsg">Message</param>
        public void WriteAITELog(string strMsg)
        {

            short nH = 0;
            string FileName = "";
            string NewFileName = "";
            // On Error GoTo 0 ' prevent crashing if file is already open VBConversions Note: Statement had no effect. // prevent crashing if file is already open


            if (FileSystem.Dir((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\Logs", FileAttribute.Directory) == "")
            {
                FileSystem.MkDir((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\Logs");
            }
            FileName = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\Logs\\" + "AITE DIRECT.log";
            nH = (short)(FileSystem.FreeFile());


            if (FileSystem.Dir(FileName) != "")
            {
                if (FileSystem.FileLen(FileName) > 100000)
                {
                    NewFileName = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\Logs\\" + "AITE DIRECT." + DateAndTime.Today.ToString("mmddyyyy") + ".log";
                    Variables.CopyFile(FileName, NewFileName, 0);
                    Variables.DeleteFile(FileName);
                }
            }

            FileSystem.FileOpen(nH, FileName, OpenMode.Append);
            FileSystem.PrintLine(nH);
            FileSystem.PrintLine(nH, "=== " + Convert.ToString(DateTime.Now) + "===" + strMsg);
            FileSystem.FileClose(nH);

        }

        /// <summary>
        /// Method to finish till close
        /// </summary>
        /// <param name="till">Till close</param>
        /// <param name="readTotalizerInTillClose">Read totaliser</param>
        /// <param name="readTankDipInTillClose">Read tank dip</param>
        /// <param name="error">Error message</param>
        public void Finish_The_Close(Till till, bool? readTotalizerInTillClose, bool? readTankDipInTillClose,
            out ErrorMessage error)
        {
            Till_Close TC = new Till_Close();
            int? lngGroupNumber = 0;
            int lngDipNumber = 0;
            error = new ErrorMessage();
            var security = _policyManager.LoadSecurityInfo();
            //   to link TillClose to TotalizerHist
            // If totalizer was not read before Till Close or failed, then GroupType is
            // NULL for this Till Close; GroupNumber has to be saved anyway in Till Close
            // because ShiftRec application is using it to calculate FuelSales
            // For sites without pump control don't look for GroupNumber (there are no records in TotalizerHist table)
            if (security.Pump_Control)
            {

                lngGroupNumber = _tillCloseService.GetMaximumGroupNumber();

                TC.GroupNumber = lngGroupNumber.HasValue ? lngGroupNumber.Value : 0;
                if (readTotalizerInTillClose != null)
                {
                    TC.GroupType = "R"; // to reprint Totalizer reading in Till Close reprint
                }
            }
            else
            {
                TC.GroupNumber = 0;
            }
            //   end

            //   to record Dip_Number in Close_Head (used in DSR)

            lngDipNumber = _tillCloseService.GetMaxDipNumber();

            TC.Dip_Number = lngDipNumber == -1 ? 0 : lngDipNumber;
            //   end

            // Open the till record
            TC.Till_Number = (short)till.Number;
            TC.ShiftNumber = till.Shift;
            TC.Open_Date = till.Date_Open;
            TC.Open_Time = till.Time_Open;
            TC.ShiftDate = till.ShiftDate;
            var savedData = CacheManager.GetTillCloseData(till.Number);
            if (savedData == null)
                savedData = new Till_Close();
            TC.Float = savedData.Float;
            TC.Draw = savedData.Draw;
            TC.Drop = savedData.Drop; //  took out the bonus drop from the total drop ( bonus drop is separate)
            TC.Payments = savedData.Payments;
            TC.ARPay = savedData.ARPay;
            TC.Payouts = savedData.Payouts;
            TC.Penny_Adj = savedData.Penny_Adj; //  
            TC.OverPay = savedData.OverPay;

            TC.User = Convert.ToString(till.UserLoggedOn); //  - we were not saving user info in close_head (till close)
                                                           // 
            TC.BonusFloat = savedData.BonusFloat;
            TC.BonusDraw = savedData.BonusDraw;
            TC.BonusDrop = savedData.BonusDrop;
            TC.BonusGiveAway = savedData.BonusGiveAway;
            //Shiny end
            //Set TC.Till_Database = dbTill ' What does it do that Trans_Database doesn't
            SetTrans_Database(ref TC);
            var dtaTenders = _tillCloseService.GetTillCloseByTillNumber(till.Number);


            // code added to sent the close batch request to the WEX server in case there is a payment made in the till using wex card
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse(_policyManager.WexTpsIP), _policyManager.WexTpsPort);
            var card = new Credit_Card();
            var retry = 0;
            var flag = false;
            foreach (var tender in dtaTenders)
            {
                var cardNumbars = _tillCloseService.GetCardNumbersByTenderName(tender.Tender);

                if (cardNumbars != null)
                {
                    foreach (var cardNumber in cardNumbars)
                    {
                       
                        if (!(string.IsNullOrEmpty(cardNumber)) && cardNumber.Length < 25)
                        {
                            _creditCardManager.SetCardnumber(ref card, cardNumber);
                            if (card.GiftType == "W")
                            {
                                var wexCloseBatchString =  _wexManager.GetWexCloseBatchString();
                                try
                                {
                                    socket.Connect(remoteEndPoint);
                                   
                                        SendToTPS(wexCloseBatchString, ref socket, ref card);
                                        while (retry < 5) 
                                        {
                                            var bytes = new byte[2048];
                                            var bytesRec = socket.Receive(bytes);
                                            var strBuffer = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                                            _wexManager.AnalyseWexResponse(strBuffer, ref card);
                                            if (card.Response.Length > 0)
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }
                                }
                                catch
                                {
                                    continue;
                                }
                               break;
                            }
                        }
                    }
                }
                if (flag)
                {
                    break;
                }
            }

            modTPS.cc.Report = card.Report;
           //wex closbatch request part ends

            Chaps_Main.Transaction_Type = "CloseCurrentTill";
            SetTill_Recordset(ref TC, dtaTenders);
            Close_Till(ref TC, till, "", till.ShiftDate, false);
            Save(TC, till, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                return;
            if (TC.Complete)
            {
                if (_policyManager.U_TOTREAD && _policyManager.USE_FUEL)
                {


                    if (readTotalizerInTillClose != null)
                    {
                        Process_Totalizer(readTotalizerInTillClose);
                    }

                    //Added to print tank dip reading if processed
                    if (readTankDipInTillClose != null)
                    {
                        ProcessTankDipReport(readTankDipInTillClose);
                    }
                }
            }

        }


        /// <summary>
        /// Method to print EOD sales
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cbDate">Close batch date</param>
        /// <param name="cbTime">Close batch time</param>
        /// <param name="crTermId">Credit Terminal Id</param>
        /// <param name="crBatchNum">Credit Batch number</param>
        /// <param name="dbTermId">Debit terminal Id</param>
        /// <param name="dbBatchNum">Debit batch number</param>
        /// <param name="reprint">Reprint or not</param>
        /// <param name="batchDate">Batch date</param>
        public Report PrintEodDetails(int tillNumber, ref DateTime cbDate, ref DateTime cbTime, string crTermId,
            string crBatchNum, string dbTermId, string dbBatchNum, bool reprint,
            DateTime batchDate)
        {
            if (cbDate == default(DateTime))
                cbDate = DateTime.Parse("12:00:00 AM");

            if (cbTime == default(DateTime))
                cbTime = DateTime.Parse("12:00:00 AM");

            if (batchDate == default(DateTime))
                batchDate = DateTime.Parse("12:00:00 AM");

            var store = _policyManager.LoadStoreInfo();
            var offSet = store.OffSet;
            string fileName = "";

            double totSale = 0;
            double totReturn = 0;
            int totRtnCount = 0;
            int totSaleCount = 0;



            var fileNumber = (short)(FileSystem.FreeFile());

            try
            {
                fileName = Path.GetTempPath() + "\\" + "EodDetails_" + PosId + ".txt";
                FileSystem.FileOpen(fileNumber, fileName, OpenMode.Output);
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(store.Code + "  " + store.Name, (short)41));
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(Convert.ToString(store.Address.Street1), (short)41));
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(store.Address.City + "," + store.Address.ProvState, (short)41));
                //Print #FileNumber, Store_Renamed.Address.Phones(1)
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber);
                string strSql = "";

                strSql = "Select CardTenders.Card_Name,CardTenders.Card_Type," + " Sum(CardTenders.amount) as [Sale]," + " Count(CardTenders.amount) as [TransNos]" + " From CardTenders " + " Where ((CardTenders.TerminalID = \'" + crTermId + "\'" + " AND CardTenders.BatchNumber = \'" + crBatchNum + "\')" + " OR (CardTenders.TerminalID = \'" + dbTermId + "\' " + " AND CardTenders.BatchNumber = \'" + dbBatchNum + "\'))" + " AND CardTenders.BatchDate = \'" + batchDate.ToString("yyyyMMdd") + "\'" + " AND CardTenders.amount > 0 AND CardTenders.Result = \'0\'" + " Group By CardTenders.Card_Name,CardTenders.Card_Type "; //& |''        " Order BY  CardTenders.Card_Type "
                strSql = strSql + " UNION ALL " + " Select CardTenders.Card_Name,CardTenders.Card_Type," + " Sum(CardTenders.amount) as [Sale]," + " Count(cardtenders.amount) as [TransNos]" + " From CardTenders " + " Where ((CardTenders.TerminalID = \'" + crTermId + "\'" + " AND CardTenders.BatchNumber = \'" + crBatchNum + "\')" + " OR (CardTenders.TerminalID = \'" + dbTermId + "\' " + " AND CardTenders.BatchNumber = \'" + dbBatchNum + "\'))" + " AND CardTenders.BatchDate = \'" + batchDate.ToString("yyyyMMdd") + "\'" + " AND CardTenders.amount < 0 AND CardTenders.Result = \'0\'" + " Group By CardTenders.Card_Name,CardTenders.Card_Type " + " Order BY  CardTenders.Card_Type ";

                _tillCloseService.GetTotals(strSql, tillNumber, DataSource.CSCTills);
                _tillCloseService.GetTotals(strSql, tillNumber, DataSource.CSCTrans);
                string timeFormat;
                string timeFormats;
                if (_policyManager.TIMEFORMAT == "24 HOURS")
                {
                    timeFormat = "hh:mm";
                    timeFormats = "hh:mm:ss";
                }
                else
                {
                    timeFormat = "hh:mm tt";
                    timeFormats = "hh:mm:ss tt";
                }
                // "Group By BatchTotal.Cardname,BatchTotal.CardType " &
                var batchTotals = _tillCloseService.GetBatchTotals();
                FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)308), (short)41)); //"STORE CLOSE BATCH REPORT"
                FileSystem.PrintLine(fileNumber);
                if (reprint)
                {
                    //        Print #FileNumber, PadC(GetResString(234) & ": " & Format(Date, "dd-MMM-yyyy") & GetResString(208) & Format(Time, "h:nn AMPM"), 41) 'Reprinted on
                    FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, (short)234) + ": " + DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, (short)208) + DateAndTime.TimeOfDay.ToString(timeFormat), (short)41)); //Reprinted on  '  
                    FileSystem.PrintLine(fileNumber);
                }
                if (cbDate == DateTime.Parse("12:00:00 AM"))
                {
                    cbDate = DateTime.Now;
                    cbTime = DateTime.Now;
                }
                //    Print #FileNumber, PadR(GetResString(305) & ": " & Format(CBDate, "dd-mmm-yyyy") & "       " & GetResString(197) & ": " & Format(CBTime, "hh:mm:ss tt"), 41) '"Date: " ,"Time: "
                FileSystem.PrintLine(fileNumber, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)305) + ": " + cbDate.ToString("dd-mmm-yyyy") + "       " + _resourceManager.GetResString(offSet, (short)197) + ": " + cbTime.ToString(timeFormats), (short)41)); //"Date: " ,"Time: " '  
                FileSystem.PrintLine(fileNumber);

                FileSystem.PrintLine(fileNumber, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)306), (short)20) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)307), (short)18)); //"Terminal ID","Batch Number"
                FileSystem.PrintLine(fileNumber, modStringPad.PadR("------------", (short)20) + modStringPad.PadL("--------------", (short)19));
                if (!string.IsNullOrEmpty(crBatchNum))
                {
                    FileSystem.PrintLine(fileNumber, modStringPad.PadR(crTermId.Trim(), (short)20) + modStringPad.PadL(crBatchNum.Trim(), (short)18)); //"Credit"'PadR(GetResString(301),6) &
                }
                if (!string.IsNullOrEmpty(dbBatchNum))
                {
                    FileSystem.PrintLine(fileNumber, modStringPad.PadR(dbTermId.Trim(), (short)20) + modStringPad.PadL(dbBatchNum.Trim(), (short)18)); //"Debit"PadR(GetResString(302), 6) &

                }
                FileSystem.PrintLine(fileNumber);
                FileSystem.PrintLine(fileNumber, modStringPad.PadL("_", 41, "_"));
                FileSystem.PrintLine(fileNumber, modStringPad.PadR(_resourceManager.GetResString(offSet, 310), 5) + modStringPad.PadC(_resourceManager.GetResString(offSet, 116), 12) + modStringPad.PadC(_resourceManager.GetResString(offSet, 298), 14) + modStringPad.PadC(_resourceManager.GetResString(offSet, 304), (short)14)); //"Card","Sale","Return","Store"
                FileSystem.PrintLine(fileNumber, modStringPad.PadR(_resourceManager.GetResString(offSet, 189), 5) + modStringPad.PadC(_resourceManager.GetResString(offSet, 303), 5) + modStringPad.PadL(_resourceManager.GetResString(offSet, 106), 8) + modStringPad.PadL(_resourceManager.GetResString(offSet, 303), (short)6) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)106), (short)7) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)299), (short)10));
                FileSystem.PrintLine(fileNumber, modStringPad.PadL("_", 41, "_"));
                if (batchTotals.Count > 0)
                {
                    //int i = 0;
                    foreach (BatchTotal btotal in batchTotals)
                    {
                        //var crdType = batchTotals[i].CardType;
                        //while (crdType == batchTotals[i].CardType)
                        //{
                        FileSystem.PrintLine(fileNumber, modStringPad.PadR(btotal.CardName, 5) + modStringPad.PadL(Convert.ToString(btotal.SaleCount), 3) + modStringPad.PadL(btotal.SaleAmount.ToString("#,##0.00"), 10) + modStringPad.PadL(Convert.ToString(btotal.ReturnCount), 3) + modStringPad.PadL(btotal.ReturnAmount.ToString("#,##0.00"), 10) + modStringPad.PadL((Convert.ToDouble(btotal.SaleAmount) + Convert.ToDouble(btotal.ReturnAmount)).ToString("#,##0.00"), 10));
                        totSale = Convert.ToDouble(totSale + btotal.SaleAmount);
                        totReturn = Convert.ToDouble(totReturn + btotal.ReturnAmount);
                        totSaleCount = Convert.ToInt32(totSaleCount + btotal.SaleCount);
                        totRtnCount = Convert.ToInt32(totRtnCount + btotal.ReturnCount);
                        //}
                        FileSystem.PrintLine(fileNumber, modStringPad.PadL("-", 41, "-"));
                        FileSystem.PrintLine(fileNumber, modStringPad.PadR(_resourceManager.GetResString(offSet, 309), 4) + modStringPad.PadL(totSaleCount.ToString("####"), 4) + modStringPad.PadL(totSale.ToString("##,##0.00"), 10) + modStringPad.PadL(totRtnCount.ToString("###"), 3) + modStringPad.PadL(totReturn.ToString("##,##0.00"), 10) + modStringPad.PadL((totSale + totReturn).ToString("##,##0.00"), 10)); //Tot                        
                    }
                    FileSystem.PrintLine(fileNumber, modStringPad.PadL("=", 41, "="));
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                }
                else
                {
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber, modStringPad.PadC(_resourceManager.GetResString(offSet, 317), 41));
                    FileSystem.PrintLine(fileNumber);
                    FileSystem.PrintLine(fileNumber);
                }
                _tillCloseService.DeleteBatchTotal();
                FileSystem.FileClose(fileNumber);
                string content = string.Empty;
                using (FileStream stream = File.OpenRead(fileName))
                {
                    try
                    {
                        //var stream = File.OpenRead(fileName);
                        //FileSystem.FileClose(fileNumber);
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
                        content = Convert.ToBase64String(bytes);
                    }
                    catch (Exception)
                    {
                        //
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
                var report = new Report
                {
                    ReportName = Utilities.Constants.EodDetailsFile,
                    ReportContent = content,
                    Copies = 1
                };

                return report;
            }
            finally
            {
                FileSystem.FileClose(fileNumber);
            }

        }

        #region Private methods

        /// <summary>
        /// Method to send request to TPS
        /// </summary>
        /// <param name="strRequest">Request</param>
        /// <param name="socket">Socket</param>
        /// <param name="cc">Credit card</param>
        private void SendToTPS(string strRequest, ref Socket socket, ref Credit_Card cc)
        {
            short retry = 0;
            var isWex = false;

            while (retry < 3) //From Table
            {
                try
                {
                    if (socket.Connected)
                    {
                        object sendStringSuf = "," + "END-DATA";
                        if (cc != null)
                        {
                            if ((cc.Crd_Type == "F" && cc.GiftType.ToUpper() == "W") || cc.Crd_Type == "WEX")
                            {
                                isWex = true;
                            }
                        }

                        if (isWex)
                        {

                            object startHeader = 0x1;
                            object sequenceNumber = 0x1;
                            const byte endTransmit = (byte)(0x4);

                            strRequest = startHeader + Convert.ToString(sequenceNumber) + (strRequest.Length.ToString("0000") + strRequest); // For WEX TPS Specific
                            sendStringSuf = endTransmit;
                        }
                        WriteToLogFile("Send to STPS: " + strRequest + Convert.ToString(sendStringSuf));
                        var msg = Encoding.ASCII.GetBytes(strRequest + Convert.ToString(sendStringSuf));
                        socket.Send(msg);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Thread.Sleep(200);
                    retry++;
                }
            }
        }

        /// <summary>
        /// Method to collect close data
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="shiftDate">Shift date</param>
        private void Collect_Close_Data(Till till, string whereClause, DateTime shiftDate)
        {

            string baseC = "";
            decimal sumOther = new decimal();
            short cntOther = 0;
            float cntTotals = 0;
            bool includInClose = false;
            string temp_Policy_Name = "COUNT_TYPE";
            string countType = Convert.ToString(_policyManager.GetPol(temp_Policy_Name, null));
            decimal drawAmt = 0;
            decimal bonusDraw = 0;
            decimal bonusGiveAway = 0;
            string temp_Policy_Name2 = "CLOSE_INCLUD";
            includInClose = Convert.ToBoolean(_policyManager.GetPol(temp_Policy_Name2, null));
            // 
            string CBT = "";
            decimal Drop_Amt = 0;
            string temp_Policy_Name3 = "CBONUSTEND";
            var PolCBonusTend = Convert.ToString(_policyManager.GetPol(temp_Policy_Name3, null));
            // shiny Mar4,2009

            var Loading = true;
            // Add up the tenders received
            // NOTE: We want to select Store Credits with POSITIVE amounts but ignore
            //       Store Credits with NEGATIVE amounts (i.e. Ignore issued credits)

            var saleTenders = _tillCloseService.GetSaleTendAmountForTrans(whereClause);
            // Get the list of included tenders
            var includedTenders = _tillCloseService.GetIncludedTenders();
            // Total the OverPayments on Prepaid Sales


            var overPay = _tillCloseService.GetOverPaymentForTrans(whereClause);

            // Total the Payments Received


            var payments = _tillCloseService.GetPaymentsForTrans(whereClause);
            // Total the AR Payments Received

            var arPay = _tillCloseService.GetArPaymentForTrans(whereClause);
            //    Debug.Print ARPay

            // Total the PayOuts Made

            var payouts = _tillCloseService.GetPayoutsForTrans(whereClause);
            //   total the penny adjustments


            var pennyAdj = _tillCloseService.GetPennyAdjustmentForTrans(whereClause);
            //   end

            // Total the change paid out.

            var sumChange = _tillCloseService.GetChangeSumForTrans(whereClause);
            // Total the Cash Draws
            //  To add Cash Bonus draw section
            var drawBonus = _tillCloseService.GetDrawAndBonusForTrans(shiftDate);

            drawAmt = drawBonus[0];
            bonusDraw = drawBonus[1]; //shiny Mar2,2009


            //  -Cash Bonus

            if (_policyManager.CashBonus && !string.IsNullOrEmpty(CBT))
            {
                //Bonus giveaways
                bonusGiveAway = _tillCloseService.GetBonusGiveAwayForTrans(whereClause);
                _tillCloseService.GetBonusDropForTrans(whereClause, CBT);

            }
            // 

            var dropLines = _tillCloseService.GetDropLinesForTrans(shiftDate);
            //  for cash bonus

            var bonusFloat = Convert.ToDecimal(till.BonusFloat); //Shiny Mar3,2009



            var Float = till.Float;
            //shiny end
            // Clear out the TillClose file before reloading.
            _tillCloseService.ClearPreviousTillCloseForTrans();
            // var tillClose = _tillCloseService.GetTillCloseForTrans();


            string temp_Policy_Name5 = "BASECURR";
            baseC = Convert.ToString(_policyManager.GetPol(temp_Policy_Name5, null));
            // 

            var totalTenders = Convert.ToDecimal(Convert.ToDecimal(sumChange) + drawAmt + Float);

            decimal dropConv = 0;
            foreach (var dropLine in dropLines)
            {
                if (countType == "Each Tender")
                {
                    var newTillClose = new TillClose
                    {
                        Tender = dropLine.TenderName,
                        Entered = 0
                    };
                    //  cash bonus & cash we are adding float and drawamount and giveaway here only for cahs bonus- it is not necessary to have reemption- so it maynot goto rs_tenders section
                    //For other tenders like  us dollar we don't have to worry about balancing the drops with out sales(it is not realistic)
                    if (newTillClose.Tender.ToLower() == baseC.ToLower())
                    {

                        newTillClose.System = Convert.ToDouble(-dropLine.Amount + Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt);

                    }
                    else if (newTillClose.Tender.ToLower() == CBT.ToLower() && _policyManager.CashBonus)
                    {

                        newTillClose.System = Convert.ToDouble(-dropLine.Amount) + Convert.ToDouble(bonusFloat) + Convert.ToDouble(bonusDraw) - Convert.ToDouble(bonusGiveAway);
                        //shiny end
                    }
                    else
                    {

                        newTillClose.System = Convert.ToDouble(-dropLine.Amount);
                    }
                    newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                    _tillCloseService.AddTillCloseForTrans(newTillClose);
                }
                else
                {

                    totalTenders = Convert.ToDecimal(totalTenders + (dropLine.ConvAmount));
                }
                dropConv = Convert.ToDecimal(dropConv + dropLine.ConvAmount);
            }

            var tillClose = _tillCloseService.GetTillCloseForTrans();

            foreach (var saleTend in saleTenders) // This is for the tenders with sales
            {
                // If we are counting Each Tender then show then individually.
                var selectedTillClose = tillClose.FirstOrDefault(t => t.Tender == saleTend.Tender);
                if (selectedTillClose == null)
                {
                    if (countType == "Each Tender")
                    {
                        if (includInClose)
                        {
                            var includedTender = includedTenders.FirstOrDefault(t => t.TendDescription == saleTend.Tender);
                            if (includedTender == null && (saleTend.Tender != CBT)) //  For Cash bonus even if the include in till close is not checked display it as separate line
                            {
                                sumOther = Convert.ToDecimal(sumOther + saleTend.Used);
                                cntOther = Convert.ToInt16(cntOther + saleTend.Count);
                            }
                            else
                            {
                                var newTillClose = new TillClose { Tender = saleTend.Tender };
                                if (newTillClose.Tender.ToLower() == baseC.ToLower())
                                {

                                    newTillClose.System = Convert.ToDouble(saleTend.Amount) + Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt);
                                    newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                                    //  for cash bonus

                                }
                                else if (newTillClose.Tender.ToLower() == CBT.ToLower() && _policyManager.CashBonus)
                                {
                                    newTillClose.System = Convert.ToDouble(saleTend.Amount + bonusFloat + bonusDraw - bonusGiveAway);
                                    newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                                    //shiny end
                                }
                                else
                                {
                                    newTillClose.System = Convert.ToDouble(saleTend.Amount - Drop_Amt);
                                    newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                                }
                                newTillClose.Count = saleTend.Count;
                                newTillClose.Entered = 0;
                                _tillCloseService.AddTillCloseForTrans(newTillClose);
                            }
                        }
                        else
                        {
                            var newTillClose = new TillClose { Tender = saleTend.Tender };
                            if (newTillClose.Tender.ToLower() == baseC.ToLower())
                            {

                                newTillClose.System = Convert.ToDouble(saleTend.Amount) + Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt);
                                newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);

                                Float = till.Float;
                                //Shiny mar2,2009- Cash bonus

                            }
                            else if (String.Equals(newTillClose.Tender, CBT, StringComparison.CurrentCultureIgnoreCase) && _policyManager.CashBonus)
                            {
                                newTillClose.System = Convert.ToDouble(saleTend.Amount + bonusFloat + bonusDraw - bonusGiveAway);
                                newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                                //Shiny end
                            }
                            else
                            {
                                newTillClose.System = Convert.ToDouble(saleTend.Amount - Drop_Amt);
                                newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                            }
                            newTillClose.Count = saleTend.Count;
                            newTillClose.Entered = 0;
                            _tillCloseService.AddTillCloseForTrans(newTillClose);
                        }

                    }
                    else
                    {
                        totalTenders = Convert.ToDecimal(totalTenders + saleTend.Used);
                        cntTotals = Convert.ToSingle(cntTotals + saleTend.Count);
                    }
                }
                else //already added because there was a drop
                {
                    if (countType == "Each Tender")
                    {
                        if (includInClose)
                        {
                            var includedTender = includedTenders.FirstOrDefault(t => t.TendDescription == saleTend.Tender);
                            if (includedTender == null && (saleTend.Tender != CBT)) //  For Cash bonus even if the include in till close is not checked display it as separate line 'Then
                            {
                                sumOther = Convert.ToDecimal(sumOther + saleTend.Used);
                                cntOther = Convert.ToInt16(cntOther + saleTend.Count);
                            }
                            else
                            {
                                selectedTillClose.System = Convert.ToDouble(selectedTillClose.System) + Convert.ToDouble(saleTend.Amount);
                                selectedTillClose.Difference = -1 * Convert.ToInt32(selectedTillClose.System);
                                selectedTillClose.Count = saleTend.Count;
                                selectedTillClose.Entered = 0;
                                _tillCloseService.UpdateTillCloseForTrans(selectedTillClose);
                            }
                        }
                        else
                        {
                            selectedTillClose.System = Convert.ToDouble(selectedTillClose.System) + Convert.ToDouble(saleTend.Amount);
                            selectedTillClose.Difference = -1 * Convert.ToInt32(selectedTillClose.System);
                            selectedTillClose.Count = saleTend.Count;
                            selectedTillClose.Entered = 0;
                            _tillCloseService.UpdateTillCloseForTrans(selectedTillClose);
                        }
                    }
                    else
                    {
                        totalTenders = Convert.ToDecimal(totalTenders + saleTend.Used);
                        cntTotals = Convert.ToSingle(cntTotals + saleTend.Count);
                    }
                }
            }
            //  Sometimes they can cash bonus without any

            tillClose = _tillCloseService.GetTillCloseForTrans();

            // Just in case the base tender wasn't in the list, add it if there was a float,
            // a draw, a payout or change was issued.
            if (countType == "Each Tender")
            {
                var selectedTillClose = tillClose.FirstOrDefault(t => t.Tender.ToLower() == baseC.ToLower());
                if (selectedTillClose == null && Convert.ToInt32(Math.Abs(till.Float) + Math.Abs(Convert.ToDecimal(sumChange)) + Math.Abs(drawAmt) + Math.Abs(payouts)) > 0)
                {

                    var newTillClose = new TillClose
                    {
                        Tender = baseC,
                        System = Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt)
                    };
                    Float = till.Float;
                    newTillClose.Difference = -1 * Convert.ToInt32(Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt));
                    newTillClose.Count = 0;
                    newTillClose.Entered = 0;
                    _tillCloseService.AddTillCloseForTrans(newTillClose);
                    tillClose = _tillCloseService.GetTillCloseForTrans();
                }
                // 

                if (_policyManager.CashBonus && CBT.Length != 0)
                {
                    selectedTillClose = tillClose.FirstOrDefault(t => t.Tender.ToLower() == CBT.ToLower());
                    if (selectedTillClose == null && (System.Math.Abs(bonusFloat) + System.Math.Abs(bonusDraw) + System.Math.Abs(bonusGiveAway)) > 0)
                    {

                        var newTillClose = new TillClose();
                        newTillClose.Tender = CBT;
                        newTillClose.System = Convert.ToDouble(bonusFloat + bonusDraw - bonusGiveAway);
                        newTillClose.Difference = Convert.ToDouble(-1 * (bonusFloat + bonusDraw - bonusGiveAway));
                        newTillClose.Count = 0;
                        newTillClose.Entered = 0;
                        _tillCloseService.AddTillCloseForTrans(newTillClose);
                        tillClose = _tillCloseService.GetTillCloseForTrans();
                    }
                }
                //shiny end
            }

            // If we are just counting All Tenders then lump everything together.
            if (countType == "All Tenders")
            {
                var newTillClose = new TillClose
                {
                    Tender = "All Tenders",
                    System = Convert.ToDouble(totalTenders),
                    Entered = 0
                };
                newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                newTillClose.Count = Convert.ToInt32(cntTotals);
                _tillCloseService.AddTillCloseForTrans(newTillClose);
            }
            else if (includInClose && sumOther != 0)
            {
                var newTillClose = new TillClose
                {
                    Tender = "Other Tenders",
                    System = Convert.ToDouble(sumOther),
                    Entered = 0
                };
                newTillClose.Difference = -1 * Convert.ToInt32(newTillClose.System);
                newTillClose.Count = Convert.ToInt32(cntOther);
                _tillCloseService.AddTillCloseForTrans(newTillClose);
            }
            // 

            Loading = false;
            var TC = new Till_Close
            {
                Till_Number = till.Number,
                Draw = drawAmt,
                Drop = dropConv,
                Payments = payments,
                ARPay = arPay,
                Payouts = payouts,
                OverPay = overPay,
                Penny_Adj = pennyAdj,
                BonusFloat = bonusFloat,
                BonusDraw = bonusDraw,
                Float = till.Float
            };
            //  took out the bonus drop from the total drop ( bonus drop is separate)
            CacheManager.AddTillCloseDataForTill(till.Number, TC);
        }

        /// <summary>
        /// Method to get response
        /// </summary>
        /// <param name="strResponse">Response</param>
        /// <param name="cc">Credit card</param>
        private void GetResponse(string strResponse, ref Credit_Card cc)
        {
            WriteToLogFile("GetResponse procedure response is " + cc.Response);

            cc.Response = GetStrPosition(strResponse, 15).Trim().ToUpper();
            if (_policyManager.EMVVersion) //EMVVERSION 'Added May4,2010
            {
                cc.Card_Swiped = cc.ManualCardProcess == false;
            }
            cc.Result = GetStrPosition(strResponse, 16).Trim();
            cc.Authorization_Number = GetStrPosition(strResponse, 17).Trim().ToUpper();
            cc.ResponseCode = GetStrPosition(strResponse, 29).Trim().ToUpper();
            //  EMVVERSION
            if (_policyManager.EMVVersion) //EMVVERSION
            {
                cc.Crd_Type = GetStrPosition(strResponse, 2).Trim().Substring(0, 1);
                // _creditCardManager.SetTrack2(ref cc, GetStrPosition(strResponse, (short)12).Trim().ToUpper());
                // cc.Swipe_String = cc.Track2;
            }
            //shiny end-EMVVERSION



            var strSeq = GetStrPosition(strResponse, 5).Trim();
            if (_policyManager.BankSystem != "Moneris")
            {
                cc.Sequence_Number = string.IsNullOrEmpty(strSeq) ? "" : strSeq.Substring(0, strSeq.Length - 1);
            }
            else //Moneris
            {
                cc.Sequence_Number = strSeq;
            }


            cc.TerminalID = GetStrPosition(strResponse, 8).Trim();
            cc.DebitAccount = GetStrPosition(strResponse, 11).Trim();
            var strDate = GetStrPosition(strResponse, 21).Trim();
            //Nancy changed,10/21/02
            if (string.IsNullOrEmpty(strDate))
            {
                cc.Trans_Date = DateTime.Today;
            }
            else
            {
                DateTime date;
                cc.Trans_Date = DateTime.TryParseExact(strDate, "MM/dd/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date) ? date : DateTime.Today;
            }
            var strTime = GetStrPosition(strResponse, 22).Trim();

            if (string.IsNullOrEmpty(strTime))
            {
                cc.Trans_Time = DateTime.Now;

            }
            else
            {
                DateTime time;
                cc.Trans_Time = DateTime.TryParseExact(strTime, "hh:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out time) ? new DateTime(1899, 12, 30, time.Hour, time.Minute, time.Second) : DateTime.Now;
            }
            //    cc.Trans_Date = Trim(GetStrPosition(strResponse, 21))
            //    cc.Trans_Time = Trim(GetStrPosition(strResponse, 22))
            cc.ApprovalCode = GetStrPosition(strResponse, 18).Trim();
            cc.Receipt_Display = GetStrPosition(strResponse, 23).Trim();

            //    cc.Report = Trim(GetStrPosition(strResponse, 30))
            if (_policyManager.EMVVersion) //EMVVERSION
            {
                cc.Report = GetStrPosition(strResponse, 31).Trim();
                cc.BankMessage = GetStrPosition(strResponse, 30).Trim();
            }
            else
            {
                cc.Report = GetStrPosition(strResponse, 30).Trim();
            }

            // Nicolette added next lines
            if (cc.AskVechicle)
            {
                _creditCardManager.SetVehicleNumber(ref cc, GetStrPosition(strResponse, 33).Trim());
            }
            if (cc.AskIdentificationNo)
            {
                _creditCardManager.SetIdNumber(ref cc, GetStrPosition(strResponse, 34).Trim());
            }
            if (cc.AskDriverNo)
            {
                _creditCardManager.SetDriverNumber(ref cc, GetStrPosition(strResponse, 34).Trim());
            }
            if (cc.AskOdometer)
            {
                _creditCardManager.SetOdoMeter(ref cc, GetStrPosition(strResponse, 35).Trim());
            }
            // Nicolette end
            //Nancy add for PinPad






            //if (!_policyManager.EMVVersion) //  this is for pinpad swipe
            //{
            //    if (cc.Track2 == "" && GetStrPosition(strResponse, (short)12) != "")
            //    {
            //        //        12/20/06 end
            //        _creditCardManager.SetTrack2(ref cc, GetStrPosition(strResponse, (short)12).Trim());
            //    }
            //}
            //
            //to set Language again, Nov.20th,2002, Nancy
            //    cc.language = Trim(GetStrPosition(strResponse, 10))

            //    cc.Swipe_String = cc.Track2
            _creditCardManager.SetIdNumber(ref cc, GetStrPosition(strResponse, 3).Trim());
            //  -even if manual entry , it was changing that to swiped, actually stps is returning whether swiped or not
            //    cc.Ca  rd_Swiped = True
            if (!_policyManager.EMVVersion) //EMVVERSION ' 
            {
                if (GetStrPosition(strResponse, 1).Trim().ToUpper() == "SWIPEINSIDE")
                {
                    cc.Card_Swiped = (GetStrPosition(strResponse, 15).Trim().ToUpper() == "SWIPED");
                }
            }
            //SHINY END
            // EMVVERSION
            if (_policyManager.EMVVersion) // 31 position is card name
            {
                //  because luba changed to 33 and response is giving cardname at 33 position - Shecan't remember
                cc.Name = GetStrPosition(strResponse, 33).Trim(); // Trim(GetStrPosition(strResponse, 32))
            }
        }

        /// <summary>
        /// Method to get string position
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="lo"></param>
        /// <returns>Position</returns>
        private string GetStrPosition(string str, short lo)
        {
            var strTemp = "";

            var strT = str;
            var returnValue = "";
            var i = (short)0;
            if (!string.IsNullOrEmpty(str))
            {
                while (i < lo)
                {
                    var intTemp = (short)(strT.IndexOf(",", StringComparison.Ordinal) + 1);
                    if ((intTemp == 0) && (!string.IsNullOrEmpty(strT)))
                    {
                        strTemp = strT;
                        strT = "";
                    }
                    else
                    {
                        if (intTemp > 0) //  added to prevent from occurring runtime error
                        {
                            strTemp = strT.Substring(0, intTemp - 1);
                            strT = strT.Substring(intTemp + 1 - 1);
                        }
                    }
                    i++;
                }
                returnValue = strTemp;

            }
            return returnValue;
        }

        /// <summary>
        /// Method to process totaliser
        /// </summary>
        /// <param name="readTotalizerSuccess">Read totaliser</param>
        private void Process_Totalizer(bool? readTotalizerSuccess)
        {
            DateTime LastDate = default(DateTime);
            DateTime HL_LastDate = default(DateTime);
            double TV = 0;
            double TA = 0;
            double TVH = 0;
            double TVL = 0;
            string fileName = "";
            short MG = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            fileName = Path.GetTempPath() + "\\TillClose_" + PosId + ".txt";
            //var nH = FileSystem.FreeFile();
            //FileSystem.FileOpen(nH, File_Name, OpenMode.Append);

            using (StreamWriter fileWriter = new StreamWriter(fileName, true))
            {

                short GradeID = 0;
                if (readTotalizerSuccess != null && readTotalizerSuccess.Value)
                {
                    // 'LastDate' is the date of the last totalizer readings that were used.
                    var mg = _tillCloseService.GetMaximumGroupNumber();
                    if (mg.HasValue)
                    {
                        MG = Convert.ToInt16(mg.Value);
                    }
                    else
                    {
                        fileWriter.WriteLine();
                        fileWriter.WriteLine(modStringPad.PadL(_resourceManager.GetResString(offSet, (short)366), (short)40));
                        fileWriter.WriteLine();
                        return;
                    }
                    var totalizers = _tillCloseService.GetTotalizerHistByGroupNumber(MG);
                    LastDate = totalizers.FirstOrDefault().Date;
                    string timeFormat;
                    if (_policyManager.TIMEFORMAT == "24 HOURS")
                    {
                        timeFormat = "hh:mm";
                    }
                    else
                    {
                        timeFormat = "hh:mm tt";
                    }
                    fileWriter.WriteLine();
                    fileWriter.WriteLine();
                    fileWriter.WriteLine(modStringPad.PadC(_resourceManager.CreateCaption(offSet, (short)61, Convert.ToInt16(46), null, (short)0), modPrint.PRINT_WIDTH));
                    fileWriter.WriteLine(modStringPad.PadC(DateAndTime.Today.ToString("dd-MMM-yyyy") + " " + (DateAndTime.TimeOfDay.ToString(timeFormat)), modPrint.PRINT_WIDTH)); //  
                    fileWriter.WriteLine();
                    fileWriter.WriteLine(modStringPad.PadC(_resourceManager.CreateCaption(offSet, (short)63, Convert.ToInt16(46), null, (short)0), (short)10) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)64, Convert.ToInt16(46), null, (short)0), (short)15) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)65, Convert.ToInt16(46), null, (short)0), (short)15));

                    fileWriter.WriteLine(modStringPad.PadC("_", modPrint.PRINT_WIDTH, "_"));
                    fileWriter.WriteLine();

                    foreach (var tot in totalizers)
                    {
                        //var tot = totalizers[k];
                        var str = modStringPad.PadR(tot.PumpId + "/" + Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(Variables.gPumps.get_Assignment(Convert.ToByte(tot.PumpId), Convert.ToByte(tot.PositionId)).GradeID)).FullName), (short)10)
                                + modStringPad.PadL(tot.Volume.ToString("0.000"), (short)15)
                                + modStringPad.PadL(tot.Dollars.ToString("0.000"), (short)15);
                        fileWriter.WriteLine(str);

                        TV = Convert.ToDouble(TV + Convert.ToDouble(tot.Volume));
                        TA = Convert.ToDouble(TA + Convert.ToDouble(tot.Dollars));
                    }


                    fileWriter.WriteLine(modStringPad.PadC("_", modPrint.PRINT_WIDTH, "_"));

                    fileWriter.WriteLine(modStringPad.PadR(_resourceManager.CreateCaption(offSet, (short)66, Convert.ToInt16(46), null, (short)0), (short)10) + " " + modStringPad.PadL((TV.ToString("0.000")), (short)15) + modStringPad.PadL((TA.ToString("0.000")), (short)15));

                    var totalReadings = _tillCloseService.GetTotalReading(MG);

                    fileWriter.WriteLine();
                    fileWriter.WriteLine(modStringPad.PadC(_resourceManager.GetResString(offSet, (short)332), (short)10) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)64, Convert.ToInt16(46), null, (short)0), (short)15) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)65, Convert.ToInt16(46), null, (short)0), (short)15));
                    fileWriter.WriteLine(modStringPad.PadC("_", modPrint.PRINT_WIDTH, "_"));
                    foreach (var totalReading in totalReadings)
                    {

                        GradeID = Convert.ToInt16(totalReading.Grade);
                        if (GradeID > 0)
                        {
                            fileWriter.WriteLine(modStringPad.PadR(Convert.ToString(Variables.gPumps.get_Grade((byte)GradeID).FullName), (short)10) + modStringPad.PadL(totalReading.Volume.ToString("0.000"), (short)15) + modStringPad.PadL(totalReading.Dollars.ToString("0.000"), (short)15));
                        };
                    }


                    mg = _tillCloseService.GetMaximumHighLow();

                    if (mg.HasValue)
                    {
                        MG = Convert.ToInt16(mg.Value);
                    }
                    else
                    {
                        fileWriter.WriteLine();
                        fileWriter.WriteLine(modStringPad.PadL(_resourceManager.GetResString(offSet, (short)367), (short)40));
                        fileWriter.WriteLine();
                        return;
                    }
                    var totalHighLows = _tillCloseService.GetTotalHighLows(MG);
                    HL_LastDate = totalHighLows.FirstOrDefault().Date;

                    fileWriter.WriteLine();
                    fileWriter.WriteLine();
                    fileWriter.WriteLine(modStringPad.PadC(_resourceManager.CreateCaption(offSet, (short)67, Convert.ToInt16(46), null, (short)0), modPrint.PRINT_WIDTH));
                    fileWriter.WriteLine(modStringPad.PadC(DateAndTime.Today.ToString("dd-MMM-yyyy") + " " + (DateAndTime.TimeOfDay.ToString(timeFormat)), modPrint.PRINT_WIDTH)); //  
                                                                                                                                                                                   //    Print #nH,

                    fileWriter.WriteLine(modStringPad.PadC(_resourceManager.CreateCaption(offSet, (short)62, Convert.ToInt16(46), null, (short)0) + " " + (HL_LastDate.ToString("dd-MMM-yyyy")) + " " + (HL_LastDate.ToString(timeFormat)), (short)40)); //  

                    fileWriter.WriteLine();
                    fileWriter.WriteLine(modStringPad.PadC(_resourceManager.GetResString(offSet, (short)333), (short)6) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)68, Convert.ToInt16(46), null, (short)0), (short)17) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)69, Convert.ToInt16(46), null, (short)0), (short)17));
                    fileWriter.WriteLine(modStringPad.PadC("_", modPrint.PRINT_WIDTH, "_"));
                    fileWriter.WriteLine();
                    foreach (var tot1 in totalHighLows)
                    {
                        fileWriter.WriteLine();
                        fileWriter.WriteLine(modStringPad.PadC(Convert.ToString(tot1.PumpId), (short)6) + modStringPad.PadL(tot1.HighVolume.ToString("0.000"), (short)17) + modStringPad.PadL(tot1.LowVolume.ToString("0.000"), (short)17));
                        TVH = Convert.ToDouble(TVH + tot1.HighVolume);
                        TVL = Convert.ToDouble(TVL + tot1.LowVolume);
                    }

                    fileWriter.WriteLine(modStringPad.PadC("_", modPrint.PRINT_WIDTH, "_"));
                    fileWriter.WriteLine(modStringPad.PadC(_resourceManager.CreateCaption(offSet, (short)66, Convert.ToInt16(46), null, (short)0), (short)6) + modStringPad.PadL(TVH.ToString("0.000"), (short)17) + modStringPad.PadL((TVL.ToString("0.000")), (short)17));

                }
                else
                {
                    fileWriter.WriteLine();
                    fileWriter.WriteLine();
                    fileWriter.WriteLine(modStringPad.PadR(_resourceManager.GetResString(offSet, (short)366), (short)40));
                }
            }

        }

        /// <summary>
        /// Method to process tank dip report
        /// </summary>
        /// <param name="readTankDipInTillClose">Read tank dip report</param>
        private void ProcessTankDipReport(bool? readTankDipInTillClose)
        {
            short nH = 0;
            string fileName = "";

            try
            {
                fileName = Path.GetTempPath() + "\\TillClose_" + PosId + ".txt";
                nH = (short)(FileSystem.FreeFile());
                FileSystem.FileOpen(nH, fileName, OpenMode.Append);

                FileSystem.PrintLine(nH, _fuelPumpManager.GetTankDipReport(readTankDipInTillClose));

            }
            finally
            {
                FileSystem.FileClose(nH);
            }

        }

        /// <summary>
        /// Method to run FMPOS.EXE
        /// </summary>
        private void Fuel_Management() // This process will run the FMPOS.EXE(this name is very important)  and will finish the FManagement for the current shiftdate (If they didn't close the till properly, there may be an issue with the shift date- FM uses the correct shift date)
        {
            short ret = 0;
            if (FileSystem.Dir((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\" + "FMPOS.exe", FileAttribute.Normal) != "")
            {
                WriteToLogFile(" Find " + (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\" + "FMPOS.exe");

                ret = (short)(Interaction.Shell((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\FMPOS.exe", AppWinStyle.NormalFocus));
                if (ret == 0)
                {
                    Interaction.MsgBox("Fuel Management Can\'t be launched", (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Error");
                    WriteToLogFile("Fuel Management Can\'t be launched" + Convert.ToString(ret));
                }
                else
                {
                    WriteToLogFile("SUCCESSFUL launch of FMPOS" + Convert.ToString(ret));
                }
            }
            else
            {
                WriteToLogFile("Missing the Fuel Management EXE " + (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\" + "FMPOS.exe\"");
                return;
            }
        }

        /// <summary>
        /// Method to generate all shifts reports
        /// </summary>
        /// <param name="till">Till</param>
        private void Gen_AllShiftsReport(Till till)
        {

            int shift = 0;
            string whereClause = "";
            Till_Close tc = new Till_Close();

            shift = 1;
            tc.AllShifts = true;

            whereClause = " " + "SaleHead.ShiftDate = \'" + till.ShiftDate.ToString("yyyyMMdd") + "\'";

            Collect_Close_Data(till, whereClause, till.ShiftDate);


            tc.Till_Number = 0;
            tc.ShiftNumber = shift;
            tc.Close_Num = 0;
            var tillClose = _tillCloseService.GetTillCloseByTillNumber(till.Number);
            SetTill_Recordset(ref tc, tillClose);
            var savedData = CacheManager.GetTillCloseData(till.Number) ?? new Till_Close();
            tc.Float = 0;
            tc.Draw = savedData.Draw;
            tc.Drop = savedData.Drop; //  took out the bonus drop from the total drop ( bonus drop is separate)
            tc.Payments = savedData.Payments;
            tc.ARPay = savedData.ARPay;
            tc.Payouts = savedData.Payouts;
            tc.OverPay = savedData.OverPay;
            tc.Penny_Adj = savedData.Penny_Adj;
            tc.BonusFloat = savedData.BonusFloat;
            tc.BonusDraw = savedData.BonusDraw;
            tc.BonusDrop = savedData.BonusDrop;
            tc.BonusGiveAway = savedData.BonusGiveAway;

            //TC.Credits_Issued = SC_Issued;
            Close_Till(ref tc, till, whereClause, till.ShiftDate, true);

        }

        /// <summary>
        /// Method to get connection with TPS
        /// </summary>
        /// <param name="socket">Socket</param>
        /// <returns>True or false</returns>
        private bool GetConnectionWithStps(ref Socket socket)
        {
            var ip = _utilityService.GetPosAddress((byte)PosId);
            if (ip != null)
            {
                var ipAddress = IPAddress.Parse(ip);
                var remoteEndPoint = new IPEndPoint(ipAddress, 8888);
                try
                {
                    socket.Connect(remoteEndPoint);

                    if (socket.Connected)
                    {
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;

        }

        /// <summary>
        /// Method to get records count
        /// </summary>
        /// <param name="astrTransType"></param>
        /// <returns></returns>
        private short GetRecordCounts(string astrTransType) // This function is to get the count of rows in each type of file based on the process type
        {
            short returnValue = 0;
            short RecordCount = 0;
            short intCnt = 0;
            switch (astrTransType)
            {
                case "TRANSACTION":
                    //( 1 for TIR+ 1 for TSHR+ # saleheader+ # of saledetails+ # of purchaseheader+# of purchase detail+ 1 for AHR + 1 for ADR +  1 for trailer record)
                    RecordCount = (short)2; //1 for TIR+ 1 for TSHR
                                            // For saleheader ( SHR)
                    intCnt = _tillCloseService.GetMaxTaxExemptSale(mPrivateGlobals.gintTRA_CLOSENO, mPrivateGlobals.gintLastCloseNo);

                    RecordCount = (short)(RecordCount + intCnt);

                    //For sale detail (SDR)
                    intCnt = _tillCloseService.GetMaxTaxExemptSaleLine(mPrivateGlobals.gintTRA_CLOSENO, mPrivateGlobals.gintLastCloseNo);
                    RecordCount = (short)(RecordCount + intCnt);

                    //For Purchase header(PHR)
                    intCnt = _tillCloseService.GetMaxPurchaseHeader(mPrivateGlobals.gdateTRA_TRANSTIME);
                    RecordCount = (short)(RecordCount + intCnt);

                    //For Purchase Detail(PDR)
                    intCnt = _tillCloseService.GetMaxPurchaseLine(mPrivateGlobals.gdateTRA_TRANSTIME);
                    RecordCount = (short)(RecordCount + intCnt);
                    intCnt = (short)0;

                    RecordCount = (short)(RecordCount + 2); //( '1 for AHR, 1 for ADR)

                    // 
                    RecordCount++; // Last 1 for Trailer record
                    break;
                case "REGISTRY":
                    //( 1 for TIR+ 1 for TSHR+ # RHR+ # of RDD+   1 for trailer record)
                    RecordCount = (short)2; //1 for TIR+ 1 for TSHR
                                            // For Cardholder Header ( SHR)
                    intCnt = _tillCloseService.GetMaxTaxExemptCardRegistry();
                    RecordCount = (short)(RecordCount + intCnt); // 1 header record for each new cardholder
                                                                 // For Cardholder Header ( SHR)
                    RecordCount = (short)(RecordCount + intCnt); // 1 detail record for each new cardholder
                    RecordCount++; // Last 1 for Trailer record
                    break;
            }

            if (RecordCount > 0)
            {
                returnValue = RecordCount;
            }
            else
            {
                returnValue = (short)0;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get product type
        /// </summary>
        /// <param name="productCategory">Product caegory</param>
        /// <returns>Product type</returns>
        private string GetProductType(short productCategory) // This is to get product type to identify from the taxexempt category of the product- discussion with nancy ( currently support, only 6 taxexempt categories , which will be static- if change in future, need to change in evrywhere)
        {
            string returnValue = "";
            switch (productCategory)
            {
                case 1:
                case 2:
                case 3:
                    returnValue = "TSD"; //Tobacco
                    break;
                case 4:
                case 5:
                    returnValue = "FSD"; //Fuel
                    break;
                case 6:
                    returnValue = "PSD"; // Propane
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to save till close
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <param name="till">Till</param>
        /// <param name="error">Error message</param>
        private void Save(Till_Close tillClose, Till till, out ErrorMessage error)
        {
            var store = _policyManager.LoadStoreInfo();
            error = new ErrorMessage();
            short n = 0;

            bool boolShiftRec;


            //    On Error GoTo Err_Hdl

            //Behrooz Jan-12-06
            if (tillClose.Till_Number >= double.Parse(Entities.Constants.TrainFirstTill) && tillClose.Till_Number <= double.Parse(Entities.Constants.TrainLastTill))
            {
                _tillCloseService.SaveTrainerTill(ref tillClose, till);
                // Behrooz Jan-12-06  NRGT
                _saleService.Update_TillClose_NoneResettableGrantTotal(tillClose.Till_Number, tillClose.ShiftDate);
                return;
            }
            string temp_Policy_Name = "SHIFTREC";
            boolShiftRec = _policyManager.GetPol(temp_Policy_Name, null);
            int transRec, tillRec, errorNum;
            string errDescription, lastTable;
            double lastSaleNo;
            _tillCloseService.Save(ref tillClose, till, store.Code, boolShiftRec, _policyManager.TAX_EXEMPT, _policyManager.TE_Type,
              out transRec, out tillRec, out errorNum, out errDescription, out lastTable, out lastSaleNo);
            if (tillClose.Complete)
            {
                // application
                if (boolShiftRec && tillClose.Till_Number == 1)
                {
                    WriteToLogFile(transRec + " records set with close number " + Convert.ToString(tillClose.Close_Num) + " in CSCTrans database. Till closed is " + Convert.ToString(tillClose.Till_Number));
                    WriteToLogFile(tillRec + " records set with close number " + Convert.ToString(tillClose.Close_Num) + " in CSCTills database. Till closed is " + Convert.ToString(tillClose.Till_Number));
                }
                //   end

                // Reset the till to not active and not processing
                till.Active = false;
                till.Processing = false;
                _tillService.UpdateTill(till);

                // Behrooz Jan-12-06  NRGT
                _saleService.Update_TillClose_NoneResettableGrantTotal(tillClose.Till_Number, tillClose.ShiftDate);

                return;
            }
            //
            //Moved rollback to the end
            error.MessageStyle = Strings.Left(Convert.ToString(store.Language), 1).ToUpper() == "E" ? new MessageStyle { Message = "Couldn\'t complete till close. Please check TillCloseErr.log." } : new MessageStyle { Message = "La fermeture n\'a pu être complété normalement. Vérifier le journal des erreurs de fermeture de caisse." };

            var offSet = _policyManager.LoadStoreInfo().OffSet;
            WriteInFile(errorNum + " " + errDescription + "\r\n" + _resourceManager.GetResString(offSet, (short)8329)
                        + ": " + Convert.ToString(tillClose.Till_Number) + " " + lastTable + " "
                        + _resourceManager.GetResString(offSet, (short)8330) + ":  " + Convert.ToString(lastSaleNo));
            // Nicolette end
        }

        // ========================================================================
        // PRODUCT SALES REPORT
        // ========================================================================
        /// <summary>
        /// Method to create product sales report
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="tillClose">Till close</param>
        /// <param name="nH"></param>
        /// <param name="whereClause"></param>
        /// <param name="allShifts"></param>
        private void Product_Sales_Report(Till till, Till_Close tillClose, short nH, string whereClause,
            bool allShifts)
        {
            decimal sSales = new decimal();
            int sVolume = 0;
            decimal sDepos = new decimal();
            decimal taxRebate = new decimal();
            string dName = "";
            bool showCodes = false;
            int GroupTotalVolume = 0;
            decimal GroupTotalSales = new decimal();
            decimal F_Sales = new decimal();
            decimal F_Vol = new decimal();
            decimal Avg_Price = new decimal();
            decimal Sales_Only = new decimal();
            int Volume_Only = 0;
            float TotDiscount = 0;
            float AddedTax = 0;
            float InclTax = 0;
            decimal Ftsale = new decimal();
            decimal Ftvol = new decimal();
            DataSource dataSource = DataSource.CSCTills;
            F_Sales = 0;
            F_Vol = 0;
            Ftsale = 0;
            Ftvol = 0;
            showCodes = Convert.ToBoolean(_policyManager.SHOW_CODES);
            //   print till close for all shifts
            if (allShifts)
            {
                dataSource = DataSource.CSCTrans;
            }
            //   end
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var depts = _tillCloseService.GetDepartment();
            FileSystem.PrintLine(nH);
            FileSystem.PrintLine(nH);
            //    Print #nH, "PRODUCT SALES"
            FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)206));
            //    Print #nH, PadR("Category", 30) & PadL("Sales", 10) and volume
            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8331), (short)27) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8359), (short)5) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)8));
            FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
            var fuelDept = _tillCloseService.GetFuelDepartmentId();
            List<Sale_Line> saleLines;
            bool Dept_Printed = false;
            foreach (var dept in depts)
            {
                if (dept.DeptCode != fuelDept)
                {
                    if (dept.EODDetail <= 1 || dept.EODDetail > 4)
                    {

                        // Department Level Reporting
                        var saleLine = _tillCloseService.GetSaleLineByDept(dept.DeptCode, till.Number, whereClause, dataSource);
                        if (saleLine != null)
                        {
                            if (saleLine.Amount != 0)
                            {
                                if (showCodes)
                                {
                                    dName = Convert.ToString(modStringPad.PadR(saleLine.Dept, (short)5) + (string.IsNullOrEmpty(saleLine.Description) ? (" - " + _resourceManager.GetResString(offSet, (short)8364)) : saleLine.Description));
                                }
                                else
                                {
                                    dName = string.IsNullOrEmpty(saleLine.Description) ? (_resourceManager.GetResString(offSet, (short)8364)) : saleLine.Description;
                                }
                                FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)27) + modStringPad.PadL(saleLine.Quantity.ToString("###0"), (short)5) + modStringPad.PadL(saleLine.Amount.ToString("###0.00"), (short)8));
                                sSales = Convert.ToDecimal(sSales + saleLine.Amount);
                                sVolume = Convert.ToInt32(sVolume + saleLine.Quantity);
                            }
                        }

                    }
                    else if (dept.EODDetail == 2)
                    {

                        // Report for each sub-department in the department
                        saleLines = _tillCloseService.GetSaleLinesBySubDept(dept.DeptCode, till.Number, whereClause, dataSource);
                        if (saleLines.Count != 0)
                        {
                            if (showCodes)
                            {
                                FileSystem.PrintLine(nH, modStringPad.PadR(dept.DeptName, (short)5) + " - " + dept.DeptName);
                            }
                            else
                            {
                                FileSystem.PrintLine(nH, dept.DeptName);
                            }
                        }

                        foreach (var saleLine in saleLines)
                        {

                            if (saleLine.Amount != 0)
                            {
                                if (showCodes)
                                {

                                    dName = Convert.ToString(modStringPad.PadR(saleLine.Sub_Dept, (short)5) + (string.IsNullOrEmpty(saleLine.Description) ? "<No Sub Dept>" : saleLine.Description));
                                }
                                else
                                {

                                    dName = string.IsNullOrEmpty(saleLine.Description) ? "<No Sub Dept>" : saleLine.Description;
                                }
                                FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)27) + modStringPad.PadL(saleLine.Quantity.ToString("###0"), (short)5) +
                                    modStringPad.PadL(saleLine.Amount.ToString("###0.00"), (short)8));
                                sSales = Convert.ToDecimal(sSales + saleLine.Amount);
                                sVolume = Convert.ToInt32(sVolume + Convert.ToInt32(saleLine.Quantity));
                            }
                        }

                    }
                    else if (dept.EODDetail == 3)
                    {

                        // Report for the sub-details in each sub-department
                        var subDepts = _tillCloseService.GetSubDeptByDept(dept.DeptCode);
                        Dept_Printed = false;
                        foreach (var subDept in subDepts)
                        {
                            saleLines = _tillCloseService.GetSaleLinesBySubDetail(dept.DeptCode, subDept.Sub_Dept, till.Number, whereClause, dataSource);

                            if (saleLines.Count != 0)
                            {
                                if (!Dept_Printed)
                                {
                                    if (showCodes)
                                    {
                                        FileSystem.PrintLine(nH, modStringPad.PadR(dept.DeptName, (short)5) + " - " + dept.DeptName);
                                    }
                                    else
                                    {
                                        FileSystem.PrintLine(nH, dept.DeptName);
                                    }
                                    Dept_Printed = true;
                                }
                                if (showCodes)
                                {
                                    FileSystem.PrintLine(nH, Strings.Space(3) + modStringPad.PadR(subDept.Sub_Name, (short)5) + " - " + subDept.Sub_Name);
                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, Strings.Space(3) + subDept.Sub_Name);
                                }

                                foreach (var saleLine in saleLines)
                                {

                                    if (saleLine.Amount != 0)
                                    {
                                        if (showCodes)
                                        {

                                            dName = Convert.ToString(modStringPad.PadR(saleLine.Sub_Detail, (short)6) + (string.IsNullOrEmpty(saleLine.Description) ? " - <No Detail>" : saleLine.Description));
                                        }
                                        else
                                        {

                                            dName = (string.IsNullOrEmpty(saleLine.Description) ? " - <No Detail>" : saleLine.Description);
                                        }
                                        FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)27) + modStringPad.PadL(saleLine.Quantity.ToString("###0"), (short)5) + modStringPad.PadL(saleLine.Amount.ToString("###0.00"), (short)8));
                                        sSales = Convert.ToDecimal(sSales + saleLine.Amount);
                                        sVolume = Convert.ToInt32(sVolume + Convert.ToInt32(saleLine.Quantity));
                                    }
                                }
                            }
                        }

                    }
                    else if (dept.EODDetail == 4)
                    {

                        // Report individual items sold.
                        saleLines = _tillCloseService.GetSaleLinesByStockCode(dept.DeptCode, till.Number, whereClause, dataSource);

                        if (saleLines.Count != 0)
                        {
                            if (showCodes)
                            {

                                dName = modStringPad.PadR(dept.DeptCode, (short)5) + (string.IsNullOrEmpty(dept.DeptName) ? (" - " + _resourceManager.GetResString(offSet, (short)8364)) : dept.DeptName);
                            }
                            else
                            {
                                dName = string.IsNullOrEmpty(dept.DeptName) ? _resourceManager.GetResString(offSet, (short)8364) : dept.DeptName;
                            }
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH, modStringPad.PadR(dName, modPrint.PRINT_WIDTH));
                            foreach (var saleLine in saleLines)
                            {

                                if (saleLine.Amount != 0)
                                {
                                    dName = modStringPad.PadR("** " + saleLine.Description, (short)30);
                                    FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)27) + modStringPad.PadL(saleLine.Quantity.ToString("###0"), (short)5) + modStringPad.PadL(saleLine.Amount.ToString("###0.00"), (short)8));
                                    sSales = Convert.ToDecimal(sSales + saleLine.Amount);
                                    sVolume = Convert.ToInt32(sVolume + Convert.ToInt32(saleLine.Quantity));
                                }
                            }
                            FileSystem.PrintLine(nH);
                        }
                    }
                }
            }

            // Report for sales that do not have any department coding
            saleLines = _tillCloseService.GetSaleLines(till.Number, whereClause, dataSource);

            foreach (var saleLine in saleLines)
            {
                dName = _resourceManager.GetResString(offSet, (short)8364);
                FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)27) + modStringPad.PadL(saleLine.Quantity.ToString("###0"), (short)5) + modStringPad.PadL(saleLine.Amount.ToString("###0.00"), (short)8));
                sSales = Convert.ToDecimal(sSales + saleLine.Amount);
                sVolume = Convert.ToInt32(sVolume + Convert.ToInt32(saleLine.Quantity));
            }

            FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8332), (short)30) + modStringPad.PadL(sSales.ToString("#,##0.00"), (short)10)); // Total product Sales

            var fuelSales = _tillCloseService.GetFuelSaleLines(till.Number, whereClause, fuelDept, dataSource);
            var taxSales = _tillCloseService.GetTaxableSaleLines(till.Number, whereClause, fuelDept, dataSource);


            if (fuelSales.Count != 0)
            {
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)2962)); // "FUEL SALES"
                                                                                              //        Print #nH, PadR("Grade", 10) & PadL("Avg $", 10) & PadL("Volume", 10) & PadL("Sales", 10)
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)332), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)2963), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)334), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)10));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                TotDiscount = 0;
                foreach (var saleLine in fuelSales)
                {

                    if (saleLine.Amount != 0)
                    {
                        dName = modStringPad.PadR(saleLine.Description, (short)10);
                        sSales = Convert.ToDecimal(Convert.ToDouble(sSales + saleLine.Amount) + Convert.ToDouble(saleLine.Discount_Adjust));
                        sVolume = Convert.ToInt32(sVolume + Convert.ToInt32(saleLine.Quantity));
                        F_Sales = Convert.ToDecimal(Convert.ToDouble(F_Sales + saleLine.Amount) + Convert.ToDouble(saleLine.Discount_Adjust));
                        F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(saleLine.Quantity));
                        if (saleLine.Quantity != 0)
                        {
                            Avg_Price = Convert.ToDecimal(Convert.ToDouble(saleLine.Amount) / Convert.ToDouble(saleLine.Quantity));
                        }
                        else
                        {
                            Avg_Price = 0;
                        }
                        FileSystem.PrintLine(nH, dName + modStringPad.PadL(Avg_Price.ToString("#,##0.000"), (short)10) + modStringPad.PadL(saleLine.Quantity.ToString("#,##0.000"), (short)10) + modStringPad.PadL(saleLine.Amount.ToString("#,##0.00"), (short)10));

                        if (saleLine.Discount_Adjust != 0)
                        {
                            FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("" + _resourceManager.GetResString(offSet, (short)8281) + "", (short)22) + modStringPad.PadL(((-1) * Convert.ToInt32(saleLine.Discount_Adjust)).ToString("#,##0.00"), (short)10)); //"(Discount Included)"
                            TotDiscount = TotDiscount + Convert.ToSingle(saleLine.Discount_Adjust.ToString("#0.00"));
                        }

                        // amount
                        if (saleLine.TE_Amount_Incl != 0)
                        {
                            FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("" + _resourceManager.GetResString(offSet, (short)1712) + "", (short)22) + modStringPad.PadL(((-1) * Convert.ToInt32(saleLine.TE_Amount_Incl)).ToString("#,##0.00"), (short)10));
                            F_Sales = Convert.ToDecimal(F_Sales - saleLine.TE_Amount_Incl);
                            sSales = Convert.ToDecimal(sSales - saleLine.TE_Amount_Incl);
                        }
                        //   end
                    }
                }
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                if (F_Vol != 0)
                {
                    Avg_Price = F_Sales / F_Vol;
                }
                else
                {
                    Avg_Price = 0;
                }
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)2964), (short)10) + modStringPad.PadL(Avg_Price.ToString("#,##0.000"), (short)10) + modStringPad.PadL(F_Vol.ToString("#,##0.000"), (short)10) + modStringPad.PadL(F_Sales.ToString("#,##0.00"), (short)10));

                Ftsale = F_Sales;
                Ftvol = F_Vol;
                if (TotDiscount != 0)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)2965), (short)25) + modStringPad.PadL(System.Math.Abs((short)TotDiscount).ToString("#,##0.00"), (short)10)); //"Fuel Discount Total"
                }
                FileSystem.PrintLine(nH);
            }

            FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)388), (short)30) + modStringPad.PadL(sSales.ToString("#,##0.00"), (short)10)); // Total Sales
            FileSystem.PrintLine(nH);

            //shiny moved this TaxExempt section further down-nov 3 2009- PM's instruction
            // Report associated charges


            sDepos = _tillCloseService.GetChargeDeposit(till.Number, whereClause, dataSource);

            if (sDepos != 0)
            {
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)381), (short)30)); //& PadL(Format(S_Depos, "#,##0.00"), 10)
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8331), (short)27) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8359), (short)5) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)8));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                var saleCharges = _tillCloseService.GetSaleCharges(till.Number, dataSource);
                var saleKitCharges = _tillCloseService.GetSaleKitCharge(till.Number, dataSource);
                if (saleCharges.Count != 0)
                {

                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)382));
                    foreach (var charge in saleCharges)
                    {
                        var sumCharge = _tillCloseService.GetTotalCharge(charge.AsCode, till.Number, whereClause, dataSource);
                        if (sumCharge.Amount != 0)
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(charge.Description, (short)27) + modStringPad.PadL(sumCharge.Quantity.ToString("###0"), (short)5) + modStringPad.PadL(sumCharge.Amount.ToString("###0.00"), (short)8));
                        }
                    }
                    FileSystem.PrintLine(nH);
                }

                if (saleKitCharges.Count != 0)
                {

                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)383));
                    foreach (var charge in saleKitCharges)
                    {
                        var sumCharge = _tillCloseService.GetTotalKitCharge(charge.AsCode, till.Number, whereClause, dataSource);
                        if (sumCharge.Amount != 0)
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(charge.Description, (short)27) + modStringPad.PadL(sumCharge.Quantity.ToString("###0"), (short)5) + modStringPad.PadL(sumCharge.Amount.ToString("###0.00"), (short)8));
                        }
                    }

                }
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)384), (short)30) + modStringPad.PadL(sDepos.ToString("#,##0.00"), (short)10));
                FileSystem.PrintLine(nH);
            }


            Sales_Only = sSales;
            Volume_Only = sVolume;
            sSales = sSales + sDepos;

            // Report the total taxes of each type added.
            // Nicolette changed SQL statements for rs_TaxF recordset and
            // rolled back other SQL statements (till_close was given wrong amounts
            // in taxes and totals and, also, was unbalanced !!!), Dec 19, 2002
            var saleTaxes = _tillCloseService.GetSaleTaxes(till.Number, whereClause, dataSource);
            var fuelLineTaxes = _tillCloseService.GetLineTaxesForFuel(till.Number, whereClause, fuelDept, dataSource);
            var saleLineCharges = _tillCloseService.GetKitCharges(till.Number, whereClause, dataSource);
            if (saleTaxes.Count != 0)
            {
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)137).ToUpper()); //" TAXES"
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)189), (short)7) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)194), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8338), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)210), (short)13));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                AddedTax = 0;
                InclTax = 0;
                foreach (var saleTax in saleTaxes)
                {

                    if ((saleTax.Tax_Added_Amount != 0) || (saleTax.Tax_Included_Amount != 0))
                    {

                        FileSystem.PrintLine(nH, modStringPad.PadR(saleTax.Tax_Name, (short)7) + modStringPad.PadL((saleTax.Tax_Included_Amount.ToString("###,##0.00")), (short)10) + modStringPad.PadL((saleTax.Tax_Added_Amount.ToString("###,##0.00")), (short)10) +
                            modStringPad.PadL((Convert.ToDouble(saleTax.Tax_Added_Amount) + Convert.ToDouble(saleTax.Tax_Included_Amount)).ToString("##,###,##0.00"), (short)13));
                        // rebate

                        string temp_Policy_Name = "TAX_REBATE";
                        if (_policyManager.GetPol(temp_Policy_Name, null))
                        {

                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)474), (short)10) + modStringPad.PadL(saleTax.Tax_Rebate.ToString("###,##0.00"), (short)30));

                            taxRebate = Convert.ToDecimal(taxRebate + saleTax.Tax_Rebate);
                        }
                        else
                        {
                            taxRebate = 0;
                        }
                        //   end

                        AddedTax = AddedTax + Convert.ToSingle(saleTax.Tax_Added_Amount);

                        InclTax = InclTax + Convert.ToSingle(saleTax.Tax_Included_Amount);

                        sSales = sSales + Convert.ToDecimal(saleTax.Tax_Added_Amount) - taxRebate;
                    }
                }
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)210), (short)7) + modStringPad.PadL(((Information.IsDBNull(InclTax)) ? 0.ToString() : InclTax.ToString("###,##0.00")), (short)10) + modStringPad.PadL(((Information.IsDBNull(AddedTax)) ? 0.ToString() : AddedTax.ToString("###,##0.00")), (short)10) + modStringPad.PadL((Convert.ToDouble((Information.IsDBNull(InclTax)) ? 0 : InclTax) + Convert.ToDouble((decimal)((Information.IsDBNull(AddedTax)) ? 0 : AddedTax) - taxRebate)).ToString("##,###,##0.00"), (short)13));
            }

            if (saleLineCharges.Count != 0)
            {
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8125).ToUpper()); //" TAXES"
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)189), (short)7) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)194), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8338), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)210), (short)13));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                AddedTax = 0;
                InclTax = 0;
                foreach (var kitCharge in saleLineCharges)
                {

                    if ((kitCharge.Tax_Added_Amount != 0) || (kitCharge.Tax_Incl_Amount != 0))
                    {

                        FileSystem.PrintLine(nH, modStringPad.PadL(kitCharge.Tax_Name, (short)7) + modStringPad.PadL((kitCharge.Tax_Incl_Amount.ToString("###,##0.00")), (short)10) + modStringPad.PadL(kitCharge.Tax_Added_Amount.ToString("###,##0.00"), (short)10) + modStringPad.PadL((Convert.ToDouble(kitCharge.Tax_Added_Amount) + Convert.ToDouble(kitCharge.Tax_Incl_Amount)).ToString("##,###,##0.00"), (short)13));

                        AddedTax = Convert.ToSingle(AddedTax + kitCharge.Tax_Added_Amount);

                        InclTax = Convert.ToSingle(InclTax + kitCharge.Tax_Incl_Amount);

                        sSales = Convert.ToDecimal(sSales + Convert.ToDecimal(kitCharge.Tax_Added_Amount));
                    }
                }
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)210), (short)7) + modStringPad.PadL(((Information.IsDBNull(InclTax)) ? 0.ToString() : InclTax.ToString("###,##0.00")), (short)10) + modStringPad.PadL(((Information.IsDBNull(AddedTax)) ? 0.ToString() : AddedTax.ToString("###,##0.00")), (short)10) + modStringPad.PadL((Convert.ToDouble((Information.IsDBNull(InclTax)) ? 0 : InclTax) + Convert.ToDouble((Information.IsDBNull(AddedTax)) ? 0 : AddedTax)).ToString("##,###,##0.00"), (short)13));
            }

            FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
            //    Print #nH, PadR("TOTALS", 30) & PadL(Format(S_Sales, "#,##0.00"), 10)
            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)136), (short)30) + modStringPad.PadL(sSales.ToString("#,##0.00"), (short)10));

            //DETAIL SECTION STARTS HERE
            //shiny moved this section here-nov 3 2009- PM's instruction

            float vAmount = 0;
            float vTotalTaxSaved = 0;
            float vQty = 0;
            bool HeadPrn;
            if (_policyManager.TAX_EXEMPT)
            {

                //  specifically using the TE type ( we can;t use <> SITE, now we are adding more Tax Exempt like QITE

                if (_policyManager.TE_Type == "AITE") //Policy.TE_Type <> "SITE" Then
                {
                    Print_AITE_Report(tillClose, nH, whereClause);

                }
                else if (_policyManager.TE_Type == "SITE")
                {

                    var categories = _tillCloseService.GetCategories();
                    if (categories.Count != 0)
                    {
                        HeadPrn = false;
                        FileSystem.PrintLine(nH);
                        vAmount = 0;
                        vTotalTaxSaved = 0;
                        foreach (var category in categories)
                        {
                            var taxExempt = _tillCloseService.GetPurchaseItem(category.Key.ToString(), whereClause, tillClose.Till_Number, dataSource);
                            if (taxExempt != null)
                            {
                                if (HeadPrn == false) // Nov3, 2009 print only if any data
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)386), (short)20));

                                    FileSystem.PrintLine(nH, modStringPad.PadR("", (short)33) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)387), (short)8));
                                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8331), (short)13) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8359), (short)9) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)404), (short)8));
                                    FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                                    HeadPrn = true;
                                }


                                vQty = Convert.ToSingle(taxExempt.Quantity);
                                FileSystem.PrintLine(nH, modStringPad.PadR(category.Value, (short)13) + Convert.ToString(modStringPad.PadL(Convert.ToString((vQty == modGlobalFunctions.Round(vQty, 0)) ? (vQty.ToString("#,###,##0")) : (vQty.ToString("#,##0.000"))), (short)9)) + modStringPad.PadL(taxExempt.Amount.ToString("#,##0.00"), (short)10) + modStringPad.PadL(taxExempt.AddedTax.ToString("#,##0.00"), (short)8));

                                vAmount = Convert.ToSingle(vAmount + Convert.ToSingle(taxExempt.Amount));

                                vTotalTaxSaved = Convert.ToSingle(vTotalTaxSaved + Convert.ToSingle(taxExempt.AddedTax));
                            }
                        }

                        if (HeadPrn == true) // Nov3, 2009 print only if any data
                        {
                            FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)388), (short)22) + modStringPad.PadL(vAmount.ToString("#,##0.00"), (short)10) + modStringPad.PadL(vTotalTaxSaved.ToString("#,##0.00"), (short)8));
                            FileSystem.PrintLine(nH);
                            FileSystem.PrintLine(nH);
                        }
                    }

                }
            }



            var nonTaxableSale = _tillCloseService.GetNonTaxableItems(whereClause, till.Number, dataSource);
            string strDept = "";
            if (nonTaxableSale != null)
            {
                if (nonTaxableSale.Line_Num > 0)
                {
                    FileSystem.PrintLine(nH);

                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)391));
                    FileSystem.PrintLine(nH, Strings.Space(20) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8359), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)10));


                    var nonTaxableSaleLines = _tillCloseService.GetNonTaxableSaleLines(whereClause, till.Number, dataSource);
                    foreach (var saleLine in nonTaxableSaleLines)
                    {
                        var deptName = _tillCloseService.GetDepartmentById(saleLine.Dept);
                        if (deptName != null)
                        {

                            strDept = Convert.ToString(modStringPad.PadR(saleLine.Dept, (short)5) + (deptName.Length == 0 ? (_resourceManager.GetResString(offSet, (short)8364)) : deptName));
                        }
                        else //  
                        {
                            strDept = saleLine.Dept + " " + "<" + _resourceManager.GetResString(offSet, (short)8364) + ">";
                        }

                        if (saleLine.Dept == fuelDept)
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(strDept, (short)20) + modStringPad.PadL(saleLine.Quantity.ToString("####0.000"), (short)10) + modStringPad.PadL(saleLine.Amount.ToString("####0.00"), (short)10));
                        }
                        else
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadR(strDept, (short)20) + modStringPad.PadL(saleLine.Quantity.ToString("#######.00"), (short)10) + modStringPad.PadL(saleLine.Amount.ToString("####0.00"), (short)10));
                        }
                    }

                    FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)392), (short)20) + modStringPad.PadL(nonTaxableSale.Quantity.ToString("####0.000"), (short)10) + modStringPad.PadL(nonTaxableSale.Amount.ToString("####0.00"), (short)10));
                    FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                }
            }


            //  added Special section to show Tax exempted Taxes summary( This is only for AITE and QITE)

            if (_policyManager.TE_Type == "QITE" || _policyManager.TE_Type == "AITE") // Added the QITE section to show added and excluded taxes
            {
                FileSystem.PrintLine(nH);
                Print_TE_TAXES_Report(tillClose, nH, whereClause);
            }
            //shiny end

            // 

            bool blGroupSummary = false;
            if (_policyManager.EOD_GROUP)
            {
                blGroupSummary = true;

                var eodGroups = _tillCloseService.GetEodGroups();
                if (eodGroups.Count != 0)
                {
                    GroupTotalVolume = 0;
                    GroupTotalSales = 0;
                    foreach (var eod in eodGroups)
                    {
                        var eodSaleLine = _tillCloseService.GetEodSaleLine(eod.GroupId, whereClause, till.Number, dataSource);
                        if (eodSaleLine != null)
                        {

                            if (eodSaleLine.Quantity > 0)
                            {
                                if (blGroupSummary)
                                {
                                    FileSystem.PrintLine(nH);
                                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)377)); // GROUP SUMMARY

                                    FileSystem.PrintLine(nH, new string('-', modPrint.PRINT_WIDTH));
                                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)378), (short)20) + modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8359), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)10));
                                    FileSystem.PrintLine(nH);
                                    blGroupSummary = false;
                                }
                                if (eodSaleLine.Dept == fuelDept)
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString(eod.GroupName), (short)20) + modStringPad.PadR(eodSaleLine.Quantity.ToString("####0.000"), (short)10) + modStringPad.PadL(eodSaleLine.Amount.ToString("###0.00"), (short)10));
                                }
                                else
                                {
                                    FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString(eod.GroupName), (short)20) + modStringPad.PadR(eodSaleLine.Quantity.ToString("####0"), (short)10) + modStringPad.PadL(eodSaleLine.Amount.ToString("###0.00"), (short)10));
                                }
                                GroupTotalVolume = Convert.ToInt32(GroupTotalVolume + Convert.ToInt32(eodSaleLine.Quantity));
                                GroupTotalSales = Convert.ToDecimal(GroupTotalSales + eodSaleLine.Amount);
                            }
                        }
                    }
                    //for remainder items
                    if (GroupTotalVolume > 0 | GroupTotalSales > 0)
                    {
                        if ((Sales_Only - GroupTotalSales) > 0 || (Volume_Only - GroupTotalVolume) > 0)
                        {

                            FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8365), (short)20) + modStringPad.PadR((Volume_Only - GroupTotalVolume).ToString("####0"), (short)10) + modStringPad.PadL((Sales_Only - GroupTotalSales).ToString("###0.00"), (short)10));
                        }
                        FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));


                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8361), (short)20) + modStringPad.PadL("", (short)10) + modStringPad.PadL(Sales_Only.ToString("####0.00"), (short)10));

                    }
                }
            }
            // 
            //TODO: Tidel_Removed
            //Tidel report
            // 

            //ADODB.Recordset rsTidel = default(ADODB.Recordset);
            //if (_policyManager.TidelSafe)
            //{
            //    //    If Report_Type = "S" Then   'this is for summary with only successful transactions
            //    if (Where_Clause.Length == 0)
            //    {

            //        rsTidel = Chaps_Main.Get_Records("Select distinct ProcessType, sum(CashValue) as totalCash,  " + " sum(CashCount)as cashcnt,  Count(processtype)  as cnt  " + " from TidelTrans where  Till_Num= " + till.Number + " and Response in (\'OK\', \'TRUE\') Group by Processtype", Chaps_Main.dbTill, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
            //    }
            //    else
            //    {
            //        rsTidel = Chaps_Main.Get_Records("Select distinct ProcessType, sum(CashValue) as totalCash,  " + " sum(CashCount)as cashcnt,  Count(processtype)  as cnt  " + " from TidelTrans where " + " Response in (\'OK\', \'TRUE\') AND " + Where_Clause + " Group by Processtype", Chaps_Main.dbTill, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
            //    }
            //    if (!rsTidel.EOF)
            //    {
            //        FileSystem.PrintLine(nH);
            //        FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet,(short)437)); // "TIDEL SAFE TRANSACTIONS"
            //        FileSystem.PrintLine(nH, new string('-', 40));
            //        //            Print #nH, PadR("Type", 11) & PadL("Cash Value", 11) & PadL("Cash Cnt", 11); PadL("# Trans", 7)
            //        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet,(short)5414), (short)11) + modStringPad.PadL(_resourceManager.GetResString(offSet,(short)1430), (short)11) + modStringPad.PadL(_resourceManager.GetResString(offSet,(short)438), (short)11) + modStringPad.PadL(_resourceManager.GetResString(offSet,(short)439), (short)7));
            //        FileSystem.PrintLine(nH, new string('-', 40));

            //        while (!rsTidel.EOF)
            //        {
            //            FileSystem.PrintLine(nH, modStringPad.PadR(Convert.ToString(rsTidel.Fields["ProcessType"].Value), (short)11) + modStringPad.PadL(rsTidel.Fields["TotalCash"].Value.ToString("####0.00"), (short)11) + Convert.ToString((Convert.ToInt32(rsTidel.Fields["CashCnt"].Value) > 0) ? (modStringPad.PadL(rsTidel.Fields["CashCnt"].Value.ToString("####0"), (short)11)) : (Strings.Space(11))) + modStringPad.PadL(rsTidel.Fields["cnt"].Value.ToString("####0"), (short)7));
            //            if (!rsTidel.EOF)
            //            {
            //                rsTidel.MoveNext();
            //            }
            //        }

            //        rsTidel = null;
            // }
            //  }
            // 
            //Tidel end
            //END Tidel_REmoved
            //  - separate  section for Taxexempt Fuel sales by product
            List<Sale_Line> teSaleLines = new List<Sale_Line>();
            if (_policyManager.TAX_EXEMPT)
            {

                teSaleLines = _tillCloseService.GetTaxExemptSaleLines(till.Number, fuelDept, whereClause, _policyManager.TE_Type, dataSource);
            }

            if (_policyManager.TAX_EXEMPT)
            {

                if (fuelSales.Count != 0 && teSaleLines.Count != 0)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)457).ToUpper(), modPrint.PRINT_WIDTH)); // Fuel Sales - Detail
                }
            }
            else
            {
                if (fuelSales.Count != 0)
                {
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)457).ToUpper(), modPrint.PRINT_WIDTH));
                }
            }

            if (_policyManager.TAX_EXEMPT)
            {
                if (teSaleLines.Count != 0)
                {
                    F_Sales = 0;
                    F_Vol = 0;
                    FileSystem.PrintLine(nH);
                    FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)456)); //"Taxexempt fuel Sale"
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)332), (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)334), (short)12) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)13));
                    FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                    foreach (var teSaleLine in teSaleLines)
                    {


                        if (teSaleLine.Amount != 0)
                        {
                            dName = modStringPad.PadR(teSaleLine.Description, (short)12);
                            F_Sales = Convert.ToDecimal(Convert.ToDouble(F_Sales + teSaleLine.Amount) + teSaleLine.Discount_Adjust);
                            F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(teSaleLine.Quantity));
                            FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL(teSaleLine.Quantity.ToString("#######0.000"), (short)12) + modStringPad.PadL(teSaleLine.Amount.ToString("##,###,##0.00"), (short)13));

                            //shiny -aug9,2010 - To include fuel discount too
                            if (teSaleLine.Discount_Adjust != 0)
                            {
                                FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL(teSaleLine.Discount_Adjust.ToString("######0.00"), (short)10)); //"(Discount Included)"
                            }
                            // 
                        }
                    }
                    FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)2964), (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL(F_Vol.ToString("#######0.000"), (short)12) + modStringPad.PadL(F_Sales.ToString("##,###,##0.00"), (short)13));
                    FileSystem.PrintLine(nH);
                }
            }
            // 

            //  - Need a section for Fuel after excluding the included tax and also after reducing taxexempt sales

            if (_policyManager.TAX_EXEMPT)
            {

            }
            if (fuelSales.Count != 0)
            {
                F_Sales = 0;
                F_Vol = 0;
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)455)); //"taxable fuel sales (Excluding taxes)
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)332), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)2963), (short)5) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)334), (short)12) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)13));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                foreach (var saleLine in fuelSales)
                {

                    if (saleLine.Amount != 0)
                    {
                        dName = modStringPad.PadR(saleLine.Description, (short)12);
                        var includedLine = taxSales.FirstOrDefault(t => t.Stock_Code == saleLine.Stock_Code);

                        if (_policyManager.TAX_EXEMPT)
                        {
                            var teSaleLine = teSaleLines.FirstOrDefault(t => t.Stock_Code == saleLine.Stock_Code);

                            if ((_policyManager.TE_Type == "AITE" || _policyManager.TE_Type == "SITE" || _policyManager.TE_Type == "QITE") && teSaleLine != null)
                            {
                                if (includedLine == null)
                                {

                                    F_Sales = F_Sales + saleLine.Amount - teSaleLine.Amount + Convert.ToDecimal(saleLine.Discount_Adjust - teSaleLine.Discount_Adjust);

                                    F_Vol = F_Vol + Convert.ToDecimal(saleLine.Quantity) - Convert.ToDecimal(teSaleLine.Quantity);


                                    FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL((Convert.ToDouble(saleLine.Quantity - teSaleLine.Discount_Adjust).ToString("#######0.000")), (short)12) + modStringPad.PadL((saleLine.Amount - teSaleLine.Amount).ToString("##,###,##0.00"), (short)13));
                                    //shiny -aug9,2010 - To include fuel discount too

                                    if ((Convert.ToDouble(saleLine.Discount_Adjust) - Convert.ToDouble(teSaleLine.Discount_Adjust)) != 0 && (Convert.ToDouble(saleLine.Amount) - Convert.ToDouble(saleLine.Amount)) != 0)
                                    {

                                        FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL((saleLine.Discount_Adjust - saleLine.Discount_Adjust).ToString("######0.00"), (short)10)); //"(Discount Included)"
                                    }
                                    // 
                                }
                                else
                                {

                                    F_Sales = Convert.ToDecimal(F_Sales + saleLine.Amount - includedLine.AddedTax - teSaleLine.Amount + Convert.ToDecimal(saleLine.Discount_Adjust - teSaleLine.Discount_Adjust));

                                    F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(saleLine.Quantity)) - Convert.ToDecimal(teSaleLine.Quantity);


                                    FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL((saleLine.Quantity - teSaleLine.Quantity).ToString("#######0.000"), (short)12) + modStringPad.PadL((saleLine.Amount - teSaleLine.Amount - Convert.ToDecimal(includedLine.AddedTax)).ToString("##,###,##0.00"), (short)13));
                                    //shiny -aug9,2010 - To include fuel discount too

                                    if ((saleLine.Discount_Adjust - teSaleLine.Discount_Adjust) != 0 && (saleLine.Amount - teSaleLine.Amount - Convert.ToDecimal(includedLine.AddedTax)) != 0)
                                    {

                                        FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL((saleLine.Discount_Adjust - teSaleLine.Discount_Adjust).ToString("######0.00"), (short)10)); //"(Discount Included)"
                                    }
                                    // 
                                }
                            }
                            else
                            {
                                // 
                                if (includedLine == null)
                                {
                                    F_Sales = Convert.ToDecimal(Convert.ToDouble(F_Sales + saleLine.Amount) + Convert.ToDouble(saleLine.Discount_Adjust));
                                    F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(saleLine.Quantity));
                                    FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL(saleLine.Quantity.ToString("#######0.000"), (short)12) + modStringPad.PadL(saleLine.Amount.ToString("##,###,##0.00"), (short)13));

                                    //shiny -aug9,2010 - To include fuel discount too
                                    if (saleLine.Discount_Adjust != 0)
                                    {
                                        FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL(saleLine.Discount_Adjust.ToString("######0.00"), (short)10)); //"(Discount Included)"
                                    }
                                    // 
                                }
                                else
                                {

                                    F_Sales = Convert.ToDecimal(Convert.ToDouble(F_Sales + saleLine.Amount) - Convert.ToDouble(includedLine.AddedTax) + saleLine.Discount_Adjust);
                                    F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(saleLine.Quantity));


                                    FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL(saleLine.Quantity.ToString("#######0.000"), (short)12) + modStringPad.PadL((Convert.ToDouble(saleLine.Amount) - Convert.ToDouble(includedLine.AddedTax)).ToString("##,###,##0.00"), (short)13));
                                    //shiny -aug9,2010 - To include fuel discount too

                                    if (saleLine.Discount_Adjust != 0 && (Convert.ToDouble(saleLine.Amount) - Convert.ToDouble(includedLine.AddedTax)) != 0)
                                    {
                                        FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL(saleLine.Discount_Adjust.ToString("######0.00"), (short)10)); //"(Discount Included)"
                                    }
                                    // 
                                }
                                // 
                            }
                        }
                        else
                        {
                            if (includedLine == null)
                            {
                                F_Sales = Convert.ToDecimal(Convert.ToDouble(F_Sales + saleLine.Amount) + Convert.ToDouble(saleLine.Discount_Adjust) - Convert.ToDouble(saleLine.TE_Amount_Incl));
                                F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(saleLine.Quantity));
                                // 
                                FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)10) + Convert.ToString(modStringPad.PadL((Convert.ToSingle(saleLine.Amount) / (((int)(saleLine.Quantity) == 0) ? 1 : (saleLine.Quantity))).ToString("#0.000"), (short)5) + modStringPad.PadL(saleLine.Quantity.ToString("#######0.000"), (short)12) + modStringPad.PadL((Convert.ToDouble(saleLine.Amount) - Convert.ToDouble(saleLine.TE_Amount_Incl)).ToString("##,###,##0.00"), (short)13)));
                                //shiny -aug9,2010 - To include fuel discount too
                                if (saleLine.Discount_Adjust != 0)
                                {
                                    FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL(saleLine.Discount_Adjust.ToString("######0.00"), (short)10)); //"(Discount Included)"
                                }
                                // 
                            }
                            else
                            {

                                F_Sales = Convert.ToDecimal(Convert.ToDouble(F_Sales + saleLine.Amount) - Convert.ToDouble(includedLine.AddedTax) + saleLine.Discount_Adjust - Convert.ToDouble(saleLine.TE_Amount_Incl));
                                F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(saleLine.Quantity));
                                // 

                                FileSystem.PrintLine(nH, modStringPad.PadR(dName,
                                    (short)10) + modStringPad.PadL(
                                        (Convert.ToDouble(Convert.ToDouble(saleLine.Amount) - Convert.ToDouble(saleLine.TE_Amount_Incl) - Convert.ToDouble(includedLine.AddedTax) / ((int)(saleLine.Quantity) == 0 ? 1 : (saleLine.Quantity))).ToString("#0.000")),
                                        (short)5) + modStringPad.PadL(
                                                   saleLine.Quantity.ToString("#######0.000"), (short)12) +
                                            modStringPad.PadL(
                                                Convert.ToDouble(Convert.ToDouble(saleLine.Amount) - Convert.ToDouble(saleLine.TE_Amount_Incl) - Convert.ToDouble(includedLine.AddedTax)).ToString("##,###,##0.00"),
                                            (short)13));
                                //shiny -aug9,2010 - To include fuel discount too
                                if (saleLine.Discount_Adjust != 0 && (Convert.ToDouble(saleLine.Amount) - Convert.ToDouble(includedLine.AddedTax)) != 0)
                                {
                                    FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL(saleLine.Discount_Adjust.ToString("######0.00"), (short)10)); //"(Discount Included)"
                                }
                                // 
                            }
                        }
                    }
                }
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)2964), (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL(F_Vol.ToString("#######0.000"), (short)12) + modStringPad.PadL(F_Sales.ToString("##,###,##0.00"), (short)13));

                FileSystem.PrintLine(nH);
            }
            // 

            // report taxes for each fuel product,  
            var fuelSaleLines = _tillCloseService.GetFuelLines(till.Number, fuelDept, whereClause, dataSource);

            if (fuelSaleLines.Count != 0)
            {
                FileSystem.PrintLine(nH);
                //        Print #nH, "FUEL TAXES"
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8337));
                //        Print #nH, PadR("Grade", 12) & PadL("Name", 8) & PadL("Added", 10) & PadL("Included", 10)
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)332), (short)12) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)189), (short)8) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8338), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)194), (short)10));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                foreach (var saleLine in fuelSaleLines)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(saleLine.Description, (short)12) + modStringPad.PadL(saleLine.DiscountName, (short)8) + modStringPad.PadL(saleLine.AddedTax.ToString("#,##0.00"), (short)10) + modStringPad.PadL(saleLine.Amount.ToString("#,##0.00"), (short)10));
                }
            }

            if (fuelLineTaxes.Count != 0)
            {
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                //       Print #nH, "Total Fuel Taxes"
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)8339));
                foreach (var lineTax in fuelLineTaxes)
                {
                    FileSystem.PrintLine(nH, Strings.Space(12) + modStringPad.PadL(lineTax.Tax_Name, (short)8) + modStringPad.PadL(lineTax.Tax_Added_Amount.ToString("#,##0.00"), (short)10) + modStringPad.PadL(lineTax.Tax_Incl_Amount.ToString("#,##0.00"), (short)10));
                }
            }
            // Nicolette end

            if (Ftsale != 0)
            {
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)2964), (short)10) + modStringPad.PadL("", (short)5) + modStringPad.PadL(Ftvol.ToString("#######0.000"), (short)12) + modStringPad.PadL(Ftsale.ToString("##,###,##0.00"), (short)13));
            }
            //Shiny adding a separate section for duncan- They want to see the actual fuel sale without exemption, even if they are giving exemption( and govt refunding the same back). this is for reporting to the government.
            //finally we decided to show the same for SITE customers.
            if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type == "SITE")
            {
                if (teSaleLines.Count != 0) // Only show if there is any tax exempt fuel sale
                {

                    if (fuelSales.Count != 0)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)473).ToUpper()); //"FUEL SALES (WITHOUT EXEMPTION)"
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)9125), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)612), (short)5) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)613), (short)12) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)9214), (short)13));

                        FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                        TotDiscount = 0;
                        F_Sales = 0;
                        F_Vol = 0;
                        Ftsale = 0;
                        Ftvol = 0;
                        foreach (var saleLine in fuelSales)
                        {

                            if (saleLine.price != 0)
                            {
                                dName = modStringPad.PadR(saleLine.Description, (short)12);
                                F_Sales = Convert.ToDecimal(Convert.ToDouble(F_Sales + Convert.ToDecimal(saleLine.price)) + Convert.ToDouble(saleLine.Discount_Adjust));
                                F_Vol = Convert.ToDecimal(F_Vol + Convert.ToDecimal(saleLine.Quantity));
                                if (saleLine.Quantity != 0)
                                {
                                    Avg_Price = Convert.ToDecimal(Convert.ToDouble(saleLine.price) / Convert.ToDouble(saleLine.Quantity));
                                }
                                else
                                {
                                    Avg_Price = 0;
                                }
                                FileSystem.PrintLine(nH, modStringPad.PadR(dName, (short)10) + modStringPad.PadL(Avg_Price.ToString("0.000"), (short)5) + modStringPad.PadL(saleLine.Quantity.ToString("#######0.000"), (short)12) + modStringPad.PadL(saleLine.price.ToString("##,###,##0.00"), (short)13));
                                if (saleLine.Discount_Adjust != 0)
                                {
                                    FileSystem.PrintLine(nH, Strings.Space(8) + modStringPad.PadC("(" + _resourceManager.GetResString(offSet, (short)8281) + ")", (short)22) + modStringPad.PadL(saleLine.Discount_Adjust.ToString("######0.00"), (short)10)); //"(Discount Included)"
                                    TotDiscount = float.Parse((TotDiscount + saleLine.Discount_Adjust).ToString("######0.00"));
                                }
                            }
                        }
                        FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                        if (F_Vol != 0)
                        {
                            Avg_Price = F_Sales / F_Vol;
                        }
                        else
                        {
                            Avg_Price = 0;
                        }
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)9215), (short)10) + modStringPad.PadL(Avg_Price.ToString("0.000"), (short)5) + modStringPad.PadL(F_Vol.ToString("#######0.000"), (short)12) + modStringPad.PadL(F_Sales.ToString("##,###,##0.00"), (short)13));
                    Ftsale = F_Sales;
                    Ftvol = F_Vol;
                    if (TotDiscount != 0)
                    {
                        FileSystem.PrintLine(nH);
                        FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)2965), (short)27) + modStringPad.PadL(System.Math.Abs((short)TotDiscount).ToString("##,###,##0.00"), (short)13)); //"Fuel Discount Total"
                    }

                    FileSystem.PrintLine(nH);
                }
            }
            //shiny end - april23
            //   print till close for all shifts, restore the till connection
            //if (AllShifts)
            //{
            //    Chaps_Main.dbTill = dbSavedTill;
            //}
            //   end


        }


        /// <summary>
        /// Method to print AITE report
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <param name="nH">File number</param>
        /// <param name="whereClause">Where clause</param>
        private void Print_AITE_Report(Till_Close tillClose, short nH, string whereClause = "")
        {
            var categories = _tillCloseService.GetCategories();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (categories.Count != 0)
            {

                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)386), (short)20));

                FileSystem.PrintLine(nH, modStringPad.PadR("", (short)33) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)387), (short)8));
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)8331), (short)13) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8359), (short)9) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8360), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)404), (short)8));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));
                float vAmount = 0;
                float vTotalTaxSaved = 0;
                foreach (var category in categories)
                {
                    var taxExenptLine = _tillCloseService.GetTaxExemptSaleLine(whereClause, category.Key, tillClose.Till_Number);
                    if (taxExenptLine != null)
                    {
                        var vQty = taxExenptLine.Quantity;
                        FileSystem.PrintLine(nH, modStringPad.PadR(category.Value, (short)13) + Convert.ToString(modStringPad.PadL(Convert.ToString((vQty == modGlobalFunctions.Round(vQty, 0)) ? (vQty.ToString("#,###,##0")) : (vQty.ToString("#,##0.000"))), (short)9)) + modStringPad.PadL(taxExenptLine.Amount.ToString("#,##0.00"), (short)10) + modStringPad.PadL(taxExenptLine.ExemptedTax.ToString("#,##0.00"), (short)8));
                        vAmount = Convert.ToSingle(vAmount + taxExenptLine.Amount);
                        vTotalTaxSaved = Convert.ToSingle(vTotalTaxSaved + taxExenptLine.ExemptedTax);
                    }
                }

                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)388), (short)22) + modStringPad.PadL(vAmount.ToString("#,##0.00"), (short)10) + modStringPad.PadL(vTotalTaxSaved.ToString("#,##0.00"), (short)8));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH);
            }

        }

        /// <summary>
        /// Method to write in AITE file
        /// </summary>
        /// <param name="strString">Content</param>
        private void WriteInFile(string strString)
        {

            short nH = 0;
            nH = (short)(FileSystem.FreeFile());
            var logPath = @"C:\APILog\";


            FileSystem.FileOpen(nH, logPath + "TillCloseErr.log", OpenMode.Append, OpenAccess.Write);


            FileSystem.PrintLine(nH, strString);
            FileSystem.FileClose(nH);

        }

        //  To show summary of Exempted tax (Added and Included Tax only) for TE customers - Not applicable for SITE
        /// <summary>
        /// Method to print TE taxes report
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <param name="nH">File number</param>
        /// <param name="whereClause">Where clause</param>
        private void Print_TE_TAXES_Report(Till_Close tillClose, short nH, string whereClause = "")
        {

            double totAdded = 0;
            double totIncluded = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var taxCredits = _tillCloseService.GetTaxCredits(whereClause, tillClose.Till_Number);
            if (taxCredits.Count != 0)
            {
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)423), (short)40));
                //        Print #nH,  PadL("Name", 20) & PadL("Added", 10) & PadL("Included", 10)
                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)189), (short)20) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8338), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)194), (short)10));
                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                foreach (var taxCredit in taxCredits)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadR(taxCredit.Tax_Name, (short)20) + modStringPad.PadL(taxCredit.Tax_Added_Amount.ToString("#####0.00"), (short)10) + modStringPad.PadL(taxCredit.Tax_Included_Amount.ToString("#####0.00"), (short)10));
                    totAdded = Convert.ToDouble(totAdded + Convert.ToDouble(taxCredit.Tax_Added_Amount));
                    totIncluded = Convert.ToDouble(totIncluded + Convert.ToDouble(taxCredit.Tax_Included_Amount));

                }

                FileSystem.PrintLine(nH, new string('_', modPrint.PRINT_WIDTH));

                FileSystem.PrintLine(nH, modStringPad.PadR(_resourceManager.GetResString(offSet, (short)424), (short)20) + modStringPad.PadL(totAdded.ToString("#####0.00"), (short)10) + modStringPad.PadL(totIncluded.ToString("#####0.00"), (short)10));
                FileSystem.PrintLine(nH);

            }
        }

        /// <summary>
        /// Method to create daily transactions
        /// </summary>
        private Report CreateDailyTransactionFile() // This function Create daily transaction file in the format specified by TRA
        {
            string TFileName = "";
            string RegistryFilename = "";
            string strBackupFileName = "";
            string FileName = "";
            short nH = 0;
            Report dailyTransactionFile = null;
            //Getting the last close number from the Tax exempt sale table
            mPrivateGlobals.gintLastCloseNo = _tillCloseService.GetMaxCloseNumber();

            if (mPrivateGlobals.gintTRA_CLOSENO == mPrivateGlobals.gintLastCloseNo)
            {
                WriteAITELog("No Tax Exempt sale data available ! Last close number " + Convert.ToString(mPrivateGlobals.gintLastCloseNo) + " was already processed.");
            }
            else if (mPrivateGlobals.gintLastCloseNo == 0)
            {
                WriteAITELog("No Tax Exempt Sale data available ito transfer to TRA. Close active Tills before processing data ");
            }

            TFileName = "T" + DateAndTime.Today.ToString("yyMMdd") + ".TXT";
            FileName = Path.GetTempPath() + mPrivateGlobals.gstrTRA_FILEOUTPATH + TFileName;
            //RegistryFilename = "C" & Format(Date, "YYMMDD") & ".TXT"
            //  - refer the last registry update file from TRA
            RegistryFilename = _tillCloseService.GetLastRegistryFileName();
            if (FileSystem.Dir(FileName) != "")
            {
                Variables.DeleteFile(FileName);
            }

            nH = (short)(FileSystem.FreeFile());
            FileSystem.FileOpen(nH, FileName, OpenMode.Output);

            // Transmitter Identification Record
            FileSystem.PrintLine(nH, "TIR" + "~" + mPrivateGlobals.theSystem.Retailer + "~" + mPrivateGlobals.theSystem.TaxCertifyCode + "~" + mPrivateGlobals.theSystem.TaxProgram + "~" + TFileName + "~");

            //Transaction Set Header record
            FileSystem.PrintLine(nH, "TSHR" + "~" + mPrivateGlobals.theSystem.Retailer + "~" + mPrivateGlobals.gdateCurrentTime.ToString("yyyyMMdd") + "~" + (mPrivateGlobals.gdateCurrentTime.ToString("HHMMSS") + "~" + Convert.ToString(GetRecordCounts("TRANSACTION")) + "~"));

            // Transaction Header and detail Record(Sales)
            // we are considering only those records  existed when started the process, if store is 24 hours, new records can come during the process time, we weon't consider the same.
            var taxExemptSaleHeads = _tillCloseService.GetTaxExemptSaleHeads(mPrivateGlobals.gintTRA_CLOSENO, mPrivateGlobals.gintLastCloseNo);
            foreach (var saleHead in taxExemptSaleHeads)
            {
                FileSystem.Print(nH, "SHR" + "~" + Convert.ToString(saleHead.Sale_Num) + "~" + "SHR" + "~" + (saleHead.Sale_Time.ToString("yyyyMMdd") + "~"));
                FileSystem.Print(nH, (saleHead.Sale_Time.ToString("hhmmss") + "~" + saleHead.teCardholder.CardNumber + "~" + saleHead.teCardholder.Barcode + "~"));
                FileSystem.PrintLine(nH, saleHead.GasReason + "~" + saleHead.PropaneReason + "~" + saleHead.TobaccoReason + "~");
                var taxExemptSaleLines = _tillCloseService.GetTaxExemptSaleLines(saleHead.Sale_Num);
                foreach (var teSaleLine in taxExemptSaleLines)
                {
                    //  - For tobacco we need to send upc code , for others  fuel, propane and cigars send product code
                    if (((int)teSaleLine.ProductType == 1) || ((int)teSaleLine.ProductType == 2) || ((int)teSaleLine.ProductType == 3)) // 1-Cigarette, 2-Losse Tobacco,3- cigar
                    {
                        FileSystem.Print(nH, "SDR" + "~" + saleHead.Sale_Num + "~" + GetProductType(Convert.ToInt16(teSaleLine.ProductType)) + "~" + Strings.Trim(teSaleLine.StockCode) + "~");
                        //   instead of calculation we are saving the tax rate at pos side(rounding issue) Print #nh, rsDetail![QUANTITY] & "~" & rsDetail![OriginalPrice] & "~" & Round((rsDetail![ExemptedTax] / rsDetail![QUANTITY]), 3) & "~" ' getting original price as price after disount, but before tax exempt calculation
                        FileSystem.PrintLine(nH, teSaleLine.Quantity + "~" + teSaleLine.OriginalPrice + "~" + teSaleLine.TaxExemptRate + "~"); // getting original price as price after disount, but before tax exempt calculation
                    } //, 4- Gasoline, 5-Diesel, 6- Propane
                    else if (((int)teSaleLine.ProductType == 4) || ((int)teSaleLine.ProductType == 5) || ((int)teSaleLine.ProductType == 6))
                    {
                        FileSystem.Print(nH, "SDR" + "~" + Convert.ToString(saleHead.Sale_Num) + "~" + GetProductType(Convert.ToInt16(teSaleLine.ProductType)) + "~" + teSaleLine.ProductCode + "~");
                        //   Print #nh, rsDetail![QUANTITY] & "~" & rsDetail![OriginalPrice] & "~" & Round((rsDetail![ExemptedTax] / rsDetail![QUANTITY]), 3) & "~" ' getting original price as price after disount, but before tax exempt calculation
                        FileSystem.PrintLine(nH, teSaleLine.Quantity + "~" + teSaleLine.OriginalPrice + "~" + teSaleLine.TaxExemptRate + "~"); // getting original price as price after disount, but before tax exempt calculation
                    }
                }
            }


            // Transaction Header and detail Record(Purchases)
            var purchaseHeaders = _tillCloseService.GetTaxExemptPurchaseHeads(mPrivateGlobals.gdateTRA_TRANSTIME);
            foreach (var purchaseHeader in purchaseHeaders)
            {
                FileSystem.Print(nH, "PHR" + "~" + purchaseHeader.PurchaseNumber + "~" + "PHR" + "~" + (purchaseHeader.TransDate.ToString("yyyyMMdd") + "~"));
                FileSystem.Print(nH, (purchaseHeader.TransDate.ToString("HHMMSS") + "~" + purchaseHeader.WholeSaleNumber + "~"));
                FileSystem.PrintLine(nH, purchaseHeader.InvoiceNumber + "~" + (purchaseHeader.PurchaseDate.ToString("yyyyMMdd") + "~"));
                var purchaseLines = _tillCloseService.GetTaxExemptPurchaseDetails(purchaseHeader.PurchaseNumber);
                foreach (var purchaseLine in purchaseLines)
                {
                    if (((Strings.Trim(Convert.ToString(purchaseLine.ProductType)) == (1).ToString()) || (Strings.Trim(Convert.ToString(purchaseLine.ProductType)) == (2).ToString())) || (Strings.Trim(Convert.ToString(purchaseLine.ProductType)) == (3).ToString())) // 1-Cigarette, 2-Losse Tobacco , 3- Cigars
                    {
                        FileSystem.Print(nH, "PDR" + "~" + Convert.ToString(purchaseLine.PurchaseNumber) + "~" + purchaseLine.PurchaseItem + "~");
                    }
                    else if (((Strings.Trim(Convert.ToString(purchaseLine.ProductType)) == (4).ToString()) || (Strings.Trim(Convert.ToString(purchaseLine.ProductType)) == (5).ToString())) || (Strings.Trim(Convert.ToString(purchaseLine.ProductType)) == (6).ToString()))
                    {
                        FileSystem.Print(nH, "PDR" + "~" + purchaseLine.PurchaseNumber + "~" + Convert.ToString(!string.IsNullOrEmpty(purchaseLine.ProductCode) ? purchaseLine.ProductCode : purchaseLine.PurchaseItem) + "~");
                    }
                    FileSystem.PrintLine(nH, purchaseLine.PurchaseQuantity + "~");

                }
            }
            //   - they don't need separate file for customer barcode updates they will grab from the  file  used for last cardholder load
            // Transaction Header and detail Record(Acknowledgments)

            ///              " WHERE Updated = 1 ", dbMaster)



            mPrivateGlobals.glngTRA_AHRNo++;
            FileSystem.PrintLine(nH, "AHR" + "~" + Convert.ToString(mPrivateGlobals.glngTRA_AHRNo) + "~" + "AHR" + "~" + (mPrivateGlobals.gdateCurrentTime.ToString("yyyyMMdd") + "~" + mPrivateGlobals.gdateCurrentTime.ToString("HHMMSS") + "~"));
            FileSystem.PrintLine(nH, "ADR" + "~" + Convert.ToString(mPrivateGlobals.glngTRA_AHRNo) + "~" + RegistryFilename + "~");




            //Trailer Record
            FileSystem.PrintLine(nH, "EOF" + "~");
            FileSystem.FileClose(nH);
            var stream = File.OpenRead(FileName);
            FileSystem.FileClose(nH);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
            stream.Close();
            var content = Convert.ToBase64String(bytes);
            dailyTransactionFile = new Report
            {
                ReportName = TFileName,
                ReportContent = content
            };

            //kept a backup and deleted the file from the original folder
            strBackupFileName = mPrivateGlobals.gstrTRA_FILEOUTPATH + "Backup\\" + TFileName;

            if (FileSystem.Dir(mPrivateGlobals.gstrTRA_FILEOUTPATH + "Backup\\", FileAttribute.Directory) == "")
            {
                FileSystem.MkDir(mPrivateGlobals.gstrTRA_FILEOUTPATH + "Backup\\");
            }
            Variables.CopyFile(FileName, strBackupFileName, 0);

            // Update the download info with last closeno, last ahr transaction number, last registry transno and transdate to use in the next process
            var query = "UPDATE DownloadInfo set Tra_Close_Num =" + Convert.ToString(mPrivateGlobals.gintLastCloseNo) + ", Tra_Trans_Time = \'" + Convert.ToString(mPrivateGlobals.gdateCurrentTime) + "\', TRA_AHR_TransNo = " + Convert.ToString(mPrivateGlobals.glngTRA_AHRNo) + ", Tra_Registry_TransNo = " + Convert.ToString(mPrivateGlobals.glngTRA_RegistryNo);
            _tillCloseService.UpdateDownloadInfo(query);
            return dailyTransactionFile;
        }

        /// <summary>
        /// Method to close a till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="closeTill">Close till</param>
        /// <param name="error">Error message</param>
        private void Close_A_Till(short tillNumber, ref CloseCurrentTillResponseModel closeTill, out ErrorMessage error)
        {
            error = new ErrorMessage();
            string baseC = "";
            string cbt = "";
            bool includInClose = false;
            decimal sumOther = new decimal();
            float cntOther = 0;
            float cntTotals = 0;
            decimal bonusDrop = 0;
            decimal bonusGiveAway = 0;
            decimal dropConv = 0;
            decimal dropAmt = 0;
            bool countCash = _policyManager.COUNT_CASH;
            string blindConfirm = _policyManager.U_EOD_DISP;
            //    boolSuspended = False
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var countType = Convert.ToString(_policyManager.COUNT_TYPE);

            includInClose = Convert.ToBoolean(_policyManager.CLOSE_INCLUD);

            baseC = Convert.ToString(_policyManager.BASECURR);

            cbt = _tenderService.GetTenderName(_policyManager.CBonusTend); //  - cash Bonus tender

            if (string.IsNullOrEmpty(baseC))
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = _resourceManager.CreateCaption(offSet, 4690, 0, null, 0)
                };
                error.ShutDownPos = true;
                return;
            }
            //  Cash Bonus

            if (_policyManager.CashBonus)
            {

                if (string.IsNullOrEmpty(_policyManager.CBonusTend))
                {
                    error.MessageStyle = new MessageStyle
                    {
                        Message = _resourceManager.CreateCaption(offSet, 4693, 0, null, 0)
                    };
                    error.ShutDownPos = true;
                    return;
                }
            }
            //shiny end

            // Add up the tenders received
            // NOTE: We want to select Store Credits with POSITIVE amounts but ignore
            //       Store Credits with NEGATIVE amounts (i.e. Ignore issued credits)

            var saleTenders = _tillCloseService.GetSaleTendAmountForTill(tillNumber);
            // Get the list of included tenders
            var includedTenders = _tillCloseService.GetIncludedTenders();
            // Total the OverPayments on Prepaid Sales


            var overPay = _tillCloseService.GetOverPayment(tillNumber);

            // Total the Payments Received


            var payments = _tillCloseService.GetPayments(tillNumber);
            // Total the AR Payments Received

            var arPay = _tillCloseService.GetArPayment(tillNumber);
            //    Debug.Print ARPay

            // Total the PayOuts Made

            var payouts = _tillCloseService.GetPayouts(tillNumber);
            //   total the penny adjustments


            var pennyAdj = _tillCloseService.GetPennyAdjustment(tillNumber);
            //   end

            // Total the change paid out.

            var sumChange = _tillCloseService.GetChangeSum(tillNumber);
            // Open the till recordset
            var till = _tillService.GetTill(tillNumber);
            // Total the Cash Draws
            //  To add Cash Bonus draw section
            decimal drawAmt = 0;
            var drawBonus = _tillCloseService.GetDrawAndBonus(tillNumber);

            drawAmt = drawBonus[0];
            var bonusDraw = drawBonus[1];

            // Clear out the TillClose file before reloading.

            _tillCloseService.ClearPreviousTillClose(tillNumber);


            //  -Cash Bonus

            if (_policyManager.CashBonus && !string.IsNullOrEmpty(cbt))
            {
                //Bonus giveaways
                bonusGiveAway = _tillCloseService.GetBonusGiveAway(tillNumber);
                bonusDrop = _tillCloseService.GetBonusDrop(tillNumber, cbt);

            }
            // 

            var dropLines = _tillCloseService.GetDropLinesForTill(tillNumber);
            //  for cash bonus

            var bonusFloat = Convert.ToDecimal(till.BonusFloat);


            var totalTenders = Convert.ToDecimal(till.Float + bonusFloat + Convert.ToDecimal(sumChange) + drawAmt + bonusDraw - bonusGiveAway);

            var Float = till.Float;
            //shiny end

            foreach (var dropLine in dropLines)
            {
                if (countType == "Each Tender")
                {
                    var newTillClose = new TillClose
                    {
                        TillNumber = tillNumber,
                        Tender = dropLine.TenderName
                    };

                    newTillClose.Entered = countCash ? 0 : newTillClose.System;
                    //  cash bonus & cash we are adding float and drawamount and giveaway here only for cahs bonus- it is not necessary to have reemption- so it maynot goto rs_tenders section
                    //For other tenders like  us dollar we don't have to worry about balancing the drops with out sales(it is not realistic)
                    if (newTillClose.Tender.ToLower() == baseC.ToLower())
                    {

                        newTillClose.System = Convert.ToDouble(-dropLine.Amount + Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt);

                    }
                    else if (newTillClose.Tender.ToLower() == cbt.ToLower() && _policyManager.CashBonus)
                    {

                        newTillClose.System = Convert.ToDouble(-dropLine.Amount) + Convert.ToDouble(bonusFloat) + Convert.ToDouble(bonusDraw) - Convert.ToDouble(bonusGiveAway);
                        //shiny end
                    }
                    else
                    {

                        newTillClose.System = Convert.ToDouble(-dropLine.Amount);
                    }
                    newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                    _tillCloseService.AddTillClose(newTillClose);
                }
                else
                {

                    totalTenders = Convert.ToDecimal(totalTenders + (dropLine.ConvAmount));
                }
                dropConv = Convert.ToDecimal(dropConv + dropLine.ConvAmount);
            }

            var tillClose = _tillCloseService.GetTillCloseByTillNumber(tillNumber);

            foreach (var saleTend in saleTenders) // This is for the tenders with sales
            {
                // If we are counting Each Tender then show then individually.
                var selectedTillClose = tillClose.FirstOrDefault(t => t.Tender == saleTend.Tender);
                if (selectedTillClose == null)
                {
                    if (countType == "Each Tender")
                    {
                        if (includInClose)
                        {
                            var includedTender = includedTenders.FirstOrDefault(t => t.TendDescription == saleTend.Tender);
                            if (includedTender == null && (saleTend.Tender != cbt)) //  For Cash bonus even if the include in till close is not checked display it as separate line
                            {
                                sumOther = Convert.ToDecimal(sumOther + saleTend.Used);
                                cntOther = Convert.ToSingle(cntOther + saleTend.Count);
                            }
                            else
                            {
                                var newTillClose = new TillClose
                                {
                                    TillNumber = tillNumber,
                                    Tender = saleTend.Tender
                                };

                                if (newTillClose.Tender.ToLower() == baseC.ToLower())
                                {

                                    newTillClose.System = Convert.ToDouble(saleTend.Amount) + Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt);
                                    newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                                    //  for cash bonus

                                }
                                else if (newTillClose.Tender.ToLower() == cbt.ToLower() && _policyManager.CashBonus)
                                {
                                    newTillClose.System = Convert.ToDouble(saleTend.Amount + bonusFloat + bonusDraw - bonusGiveAway);
                                    newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                                    //shiny end
                                }
                                else
                                {
                                    newTillClose.System = Convert.ToDouble(saleTend.Amount - dropAmt);
                                    newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                                }
                                newTillClose.Count = saleTend.Count;
                                newTillClose.Entered = countCash ? 0 : newTillClose.System;
                                _tillCloseService.AddTillClose(newTillClose);
                            }
                        }
                        else
                        {
                            var newTillClose = new TillClose
                            {
                                TillNumber = tillNumber,
                                Tender = saleTend.Tender
                            };

                            if (newTillClose.Tender.ToLower() == baseC.ToLower())
                            {

                                newTillClose.System = Convert.ToDouble(saleTend.Amount) + Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt);
                                newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;

                                Float = till.Float;
                                //Shiny mar2,2009- Cash bonus

                            }
                            else if (newTillClose.Tender.ToLower() == cbt.ToLower() && _policyManager.CashBonus)
                            {
                                newTillClose.System = Convert.ToDouble(saleTend.Amount + bonusFloat + bonusDraw - bonusGiveAway);
                                newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                                bonusFloat = bonusFloat;
                                //Shiny end
                            }
                            else
                            {
                                newTillClose.System = Convert.ToDouble(saleTend.Amount - dropAmt);
                                newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                            }
                            newTillClose.Count = saleTend.Count;
                            newTillClose.Entered = countCash ? 0 : (newTillClose.System);
                            _tillCloseService.AddTillClose(newTillClose);
                        }

                    }
                    else
                    {
                        totalTenders = Convert.ToDecimal(totalTenders + saleTend.Used);
                        cntTotals = Convert.ToSingle(cntTotals + saleTend.Count);
                    }
                }
                else //already added because there was a drop
                {
                    if (countType == "Each Tender")
                    {
                        if (includInClose)
                        {
                            var includedTender = includedTenders.FirstOrDefault(t => t.TendDescription == saleTend.Tender);
                            if (includedTender == null && (saleTend.Tender != cbt)) //  For Cash bonus even if the include in till close is not checked display it as separate line 'Then
                            {
                                sumOther = Convert.ToDecimal(sumOther + saleTend.Used);
                                cntOther = Convert.ToSingle(cntOther + saleTend.Count);
                            }
                            else
                            {
                                selectedTillClose.System = Convert.ToDouble(selectedTillClose.System) + Convert.ToDouble(saleTend.Amount);
                                selectedTillClose.Difference = countCash ? (-1 * Convert.ToInt32(selectedTillClose.System)) : 0;
                                selectedTillClose.Count = saleTend.Count;
                                selectedTillClose.Entered = countCash ? 0 : selectedTillClose.System;
                                _tillCloseService.UpdateTillClose(selectedTillClose);
                            }
                        }
                        else
                        {
                            selectedTillClose.System = Convert.ToDouble(selectedTillClose.System) + Convert.ToDouble(saleTend.Amount);
                            selectedTillClose.Difference = countCash ? (-1 * Convert.ToInt32(selectedTillClose.System)) : 0;
                            selectedTillClose.Count = saleTend.Count;
                            selectedTillClose.Entered = countCash ? 0 : (selectedTillClose.System);
                            _tillCloseService.UpdateTillClose(selectedTillClose);
                        }
                    }
                    else
                    {
                        totalTenders = Convert.ToDecimal(totalTenders + saleTend.Used);
                        cntTotals = Convert.ToSingle(cntTotals + saleTend.Count);
                    }
                }
            }
            //  Sometimes they can cash bonus without any

            tillClose = _tillCloseService.GetTillCloseByTillNumber(tillNumber);

            // Just in case the base tender wasn't in the list, add it if there was a float,
            // a draw, a payout or change was issued.
            if (countType == "Each Tender")
            {
                var selectedTillClose = tillClose.FirstOrDefault(t => t.Tender.ToLower() == baseC.ToLower());
                if (selectedTillClose == null && Convert.ToInt32(Math.Abs(till.Float) + Math.Abs(Convert.ToDecimal(sumChange)) + Math.Abs(drawAmt) + Math.Abs(payouts)) > 0)
                {

                    var newTillClose = new TillClose
                    {
                        TillNumber = tillNumber,
                        Tender = baseC,
                        System = Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt)
                    };
                    Float = till.Float;
                    newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(Convert.ToDouble(till.Float) + Convert.ToDouble(sumChange) + Convert.ToDouble(drawAmt))) : 0;
                    newTillClose.Count = 0;
                    newTillClose.Entered = countCash ? 0 : (newTillClose.System);
                    _tillCloseService.AddTillClose(newTillClose);
                    tillClose = _tillCloseService.GetTillCloseByTillNumber(tillNumber);
                }
                // 

                if (_policyManager.CashBonus && cbt.Length != 0)
                {
                    selectedTillClose = tillClose.FirstOrDefault(t => t.Tender.ToLower() == cbt.ToLower());
                    if (selectedTillClose == null && (System.Math.Abs(bonusFloat) + System.Math.Abs(bonusDraw) + System.Math.Abs(bonusGiveAway)) > 0)
                    {

                        var newTillClose = new TillClose
                        {
                            TillNumber = tillNumber,
                            Tender = cbt,
                            System = Convert.ToDouble(bonusFloat + bonusDraw - bonusGiveAway)
                        };
                        bonusFloat = bonusFloat;
                        newTillClose.Difference = Convert.ToDouble(countCash ? (-1 * (bonusFloat + bonusDraw - bonusGiveAway)) : 0);
                        newTillClose.Count = 0;
                        newTillClose.Entered = countCash ? 0 : (newTillClose.System);
                        _tillCloseService.AddTillClose(newTillClose);
                        tillClose = _tillCloseService.GetTillCloseByTillNumber(tillNumber);
                    }
                }
                //shiny end
            }

            // If we are just counting All Tenders then lump everything together.
            if (countType == "All Tenders")
            {
                var newTillClose = new TillClose();

                newTillClose.TillNumber = tillNumber;
                newTillClose.Tender = "All Tenders";
                newTillClose.System = Convert.ToDouble(totalTenders);
                newTillClose.Entered = countCash ? 0 : (newTillClose.System);
                newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                newTillClose.Count = Convert.ToInt32(cntTotals);
                _tillCloseService.AddTillClose(newTillClose);
            }
            else if (includInClose && sumOther != 0)
            {
                var newTillClose = new TillClose();

                newTillClose.TillNumber = tillNumber;
                newTillClose.Tender = "Other Tenders";
                newTillClose.System = Convert.ToDouble(sumOther);
                newTillClose.Entered = countCash ? 0 : (newTillClose.System);
                newTillClose.Difference = countCash ? (-1 * Convert.ToInt32(newTillClose.System)) : 0;
                newTillClose.Count = Convert.ToInt32(cntOther);
                _tillCloseService.AddTillClose(newTillClose);
            }

            tillClose = _tillCloseService.GetTillCloseByTillNumber(tillNumber);
            closeTill.Tenders = GetTenders(tillClose);

            if (!countCash)
            {
                closeTill.ShowEnteredField = false;
                closeTill.ShowDifferenceField = false;
            }
            if (blindConfirm == "Blind")
            {
                closeTill.ShowSystemField = false;
                closeTill.ShowDifferenceField = false;
            }
            closeTill.ShowBillCoins = tillClose.Count != 0 && countCash;
            var tc = new Till_Close
            {
                Till_Number = tillNumber,
                Draw = drawAmt,
                Drop = dropConv - bonusDrop,
                Payments = payments,
                ARPay = arPay,
                Payouts = payouts,
                OverPay = overPay,
                Penny_Adj = pennyAdj,
                BonusFloat = bonusFloat,
                BonusDraw = bonusDraw,
                BonusDrop = bonusDrop,
                BonusGiveAway = bonusGiveAway,
                Float = till.Float
            };
            //  took out the bonus drop from the total drop ( bonus drop is separate)
            CacheManager.AddTillCloseDataForTill(tillNumber, tc);
        }

        /// <summary>
        /// Method to get all till close tenders
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <returns>List of till close tenders</returns>
        private List<TillCloseTender> GetTenders(List<TillClose> tillClose)
        {
            var tenders = new List<TillCloseTender>();
            foreach (var till in tillClose)
            {
                tenders.Add(new TillCloseTender
                {
                    Tender = till.Tender,
                    Count = till.Count.ToString(),
                    Entered = till.Entered.ToString("#0.00"),
                    System = till.System.ToString("#0.00"),
                    Difference = till.Difference.ToString("#0.00")
                });
            }

            return tenders;
        }

        /// <summary>
        /// Method to get close batch report
        /// </summary>
        /// <returns>Content</returns>
        private string GetCloseBatchReport()
        {
            string returnValue = "";
            short FileNumber = 0;
            string FileName = "";
            string cBuf = "";
            string strReport = "";

            strReport = "";
            FileName = Path.GetTempPath() + "\\" + "BankEod_" + PosId + ".txt";

            if (FileSystem.Dir(FileName) != "")
            {
                FileNumber = (short)(FileSystem.FreeFile());
                FileSystem.FileOpen(FileNumber, FileName, OpenMode.Input);
                while (!(FileSystem.EOF(FileNumber)))
                {
                    cBuf = FileSystem.LineInput(FileNumber);
                    strReport = strReport + cBuf + "\r\n";
                }
                FileSystem.FileClose(FileNumber);
            }

            FileName = Path.GetTempPath() + "\\" + "EodDetails_" + PosId + ".txt";

            if (FileSystem.Dir(FileName) != "")
            {
                FileNumber = (short)(FileSystem.FreeFile());
                FileSystem.FileOpen(FileNumber, FileName, OpenMode.Output);
                while (!(FileSystem.EOF(FileNumber)))
                {
                    cBuf = FileSystem.LineInput(FileNumber);
                    strReport = strReport + cBuf + "\r\n";
                }
                FileSystem.FileClose(FileNumber);
            }

            returnValue = strReport;
            return returnValue;
        }




        /// <summary>
        /// Method to clear close batch report
        /// </summary>
        private void ClearCloseBatchReport()
        {
            string FileName = "";

            FileName = Path.GetTempPath() + "\\" + "BankEod_" + PosId + ".txt";

            if (FileSystem.Dir(FileName) != "")
            {
                FileSystem.Kill(FileName);
            }


            FileName = Path.GetTempPath() + "\\" + "EodDetails_" + PosId + ".txt";

            if (FileSystem.Dir(FileName) != "")
            {
                FileSystem.Kill(FileName);
            }
        }

        /// <summary>
        /// Method to close batch
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        private bool CloseBatch(int tillNumber, out ErrorMessage error, out Report bankEodReport, out Report eodReport)
        {
            bool returnValue = false;
            string date = Convert.ToString("12:00:00 AM");
            error = new ErrorMessage();
            short TermNos = 0;
            int TransNo = 0;
            bankEodReport = null;
            eodReport = null;
            string[,] Terminals = new string[3, 3];
            string RFileName = "";
            short i = 0;
            short RFileNumber = 0;
            DateTime BatchDate = default(DateTime); // 
            bool blnNoDebitTrans = false;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            returnValue = false;

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (GetConnectionWithStps(ref socket))
            {
                var terminals = _tillCloseService.GetTerminals(PosId);
                if (terminals.Count == 0)
                {

                    WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + _resourceManager.GetResString(offSet, (short)1287)); // Shiny Mar6, 2008 -EKO
                    MessageType temp_VbStyle4 = (int)MessageType.Critical + MessageType.OkOnly;
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1287, null, temp_VbStyle4);
                    return returnValue;
                }
                // this.Enabled = false;
                RFileNumber = (short)(FileSystem.FreeFile());
                RFileName = Path.GetTempPath() + "\\" + "BankEod_" + PosId + ".txt";
                FileSystem.FileOpen(RFileNumber, RFileName, OpenMode.Append);
                TermNos = (short)0;
                i = (short)0;
                foreach (var terminal in terminals)
                {

                    if (string.IsNullOrEmpty(terminal.TerminalType) || string.IsNullOrEmpty(terminal.TerminalId))
                    {

                        WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + _resourceManager.GetResString(offSet, (short)1287)); // Shiny Mar6, 2008 -EKO
                        error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1287, null);
                        // this.Enabled = true;
                        FileSystem.FileClose(RFileNumber);
                        return returnValue;
                    }

                    //   to check if there are any transactions for the terminalID

                    TransNo = _tillCloseService.GetTotalCardTransactions(terminal.TerminalId);
                    var cc = new Credit_Card();
                    if (TransNo > 0)
                    {
                        //   end


                        //Shiny - Dec1, 2010 -added this section to reset the flag for CloseBatch. Otherwise it is going to nonemv report section
                        if (_policyManager.EMVVersion)
                        {
                            Chaps_Main.EMVProcess = true;
                        }
                        else
                        {
                            Chaps_Main.EMVProcess = false;
                        }
                        //cc = new Credit_Card();
                        WriteToLogFile("EMVprocess:" + Convert.ToString(Chaps_Main.EMVProcess));
                        // 
                        SendToTPS(_cardManager.GetRequestString(ref cc, null, "EODTerminal", terminal.TerminalType, 0, terminal.TerminalId), ref socket, ref cc);

                        var EodTimer = DateAndTime.Timer;

                        while (Convert.ToInt32(DateAndTime.Timer - Convert.ToDouble(EodTimer)) < 120)
                        {
                            var bytes = new byte[2048];
                            var bytesRec = socket.Receive(bytes);
                            //int bytesRec = 0;
                            var strBuffer = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            WriteToLogFile("Received from STPS: " + strBuffer);
                            GetResponse(strBuffer, ref cc);
                            if (!string.IsNullOrEmpty(cc.Response))
                            {
                                break;
                            }
                            // System.Windows.Forms.Application.DoEvents();
                        }
                        if (!string.IsNullOrEmpty(cc.Response) && cc.Response.ToUpper() == "APPROVED")
                        {
                            TermNos++;
                            Terminals[TermNos, 1] = cc.TerminalID;
                            Terminals[TermNos, 2] = cc.Sequence_Number;
                            FileSystem.PrintLine(RFileNumber, cc.Report);
                            BatchDate = cc.Trans_Date; // 
                            WriteToLogFile("SUCCESS WITH CLOSE BATCH"); // Shiny Mar6, 2008 -EKO

                            //  - added as part of Datawire Integration (added by Mina)
                            //#:5:TPS does handshake after sending EOD for debit. If there weren’t any debit transactions between closes batches, POS has to send special indication that there weren’t any debit transactions. Having gotten that flag, TPS doesn’t do handshake.

                            if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "GLOBAL")
                            {
                                blnNoDebitTrans = cc.Report.IndexOf("No Transactions") + 1 > 0;
                            }
                            _maintenanceService.SetCloseBatchNumber(cc);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(cc.Receipt_Display))
                            {
                                WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + cc.Receipt_Display); // Shiny Mar6, 2008 -EKO
                                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1284, null); //shiny changed on July 2010' Call MsgBox(cc.Receipt_Display, vbCritical + vbOKOnly)
                                FileSystem.FileClose(RFileNumber);
                                // this.Enabled = true;
                                return returnValue;
                            }
                            else
                            {
                                WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + _resourceManager.GetResString(offSet, (short)1284)); // Shiny Mar6, 2008 -EKO
                                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1284, null);
                                FileSystem.FileClose(RFileNumber);
                                // this.Enabled = true;
                                return returnValue;
                            }
                        }
                        WriteToLogFile("CC object set to nothing in CloseBatch - frmTillClose"); // TEST ONLY - to be removed after we fix the CC issue
                        i++;
                    }
                    else
                    {
                        WriteToLogFile("No transaction for close batch for terminalID " + terminal.TerminalId);
                    }
                }
                socket.Disconnect(true);
            }
            else
            {
                WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + _resourceManager.GetResString(offSet, (short)1296)); // Shiny Mar6, 2008 -EKO
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1296, null);
            }
            FileSystem.FileClose(RFileNumber);
            if (!string.IsNullOrEmpty(RFileName))
            {
                var stream = File.OpenRead(RFileName);
                FileSystem.FileClose(RFileNumber);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
                stream.Close();
                var content = Convert.ToBase64String(bytes);
                bankEodReport = new Report
                {
                    ReportName = "BankEod.txt",
                    ReportContent = content
                };
            }
            if (TermNos == i)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (GetConnectionWithStps(ref socket))
                {
                    //  - Datawire Integration (Added by Mina)
                    //#5: TPS does handshake after sending EOD for debit. If there weren’t any debit transactions between closes batches, POS has to send special indication that there weren’t any debit transactions. Having gotten that flag, TPS doesn’t do handshake.
                    var cc = new Credit_Card();
                    if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "GLOBAL")
                    {
                        if (blnNoDebitTrans) //Added by Mina
                        {
                            SendToTPS(_cardManager.GetRequestString(ref cc, null, "CloseBatchInside", "NoDebit", 0, ""), ref socket, ref cc);
                        }
                        else
                        {
                            SendToTPS(_cardManager.GetRequestString(ref cc, null, "CloseBatchInside", "Credit", 0, ""), ref socket, ref cc);
                        }
                    }
                    else
                    {
                        SendToTPS(_cardManager.GetRequestString(ref cc, null, "CloseBatchInside", "Credit", 0, ""), ref socket, ref cc);
                    }
                    socket.Disconnect(false);
                }
                else
                {
                    return returnValue;
                }
            }
            if (TermNos > 0)
            {
                //  - passing batchdate too
                //         PrintEodDetails , , Terminals(1, 1), Terminals(1, 2), Terminals(2, 1), Terminals(2, 2), True
                var cbdate = DateTime.Parse("12:00:00 AM");
                var cbtime = DateTime.Parse("12:00:00 AM");
                eodReport = PrintEodDetails(tillNumber, ref cbdate, ref cbtime, Terminals[1, 1], Terminals[1, 2],
                     Terminals[2, 1], Terminals[2, 2], false, BatchDate);
                // 
            }
            //this.Enabled = true;
            returnValue = true;
            return returnValue;
        }

        /// <summary>
        /// Method to check if suspended sales exists
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="suspendaleMessage">Suspended sale message</param>
        /// <returns>True or false</returns>
        private bool Suspended_Sales(int tillNumber, out MessageStyle suspendaleMessage)
        {
            bool returnValue = false;
            suspendaleMessage = null;

            if (!_policyManager.CLOSE_SUSP)
            {


                if (_tillCloseService.AreSuspendedSales(tillNumber))
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    suspendaleMessage = _resourceManager.CreateMessage(offSet, 11, (short)64, null);
                    returnValue = true;
                }

            }

            // Nicolette end

            return returnValue;
        }


        private bool ProcessKickBackQueue(out ErrorMessage error)
        {
            bool returnValue = false;
            dynamic Policy_Renamed = default(dynamic);
            var xml = new XML(_policyManager);
            var timeout = _policyManager.KICKBACK_TMT;
            error = new ErrorMessage();
            
            float timeIN = 0;
            short TimeOut = 0;
            bool GotResponse = false;

            returnValue = false;

          var  rsKickBack = _kickBackService.GetKickbackQueue();
            if (rsKickBack.Rows.Count==0)
            {
                returnValue = true;
                return returnValue;
            }
            var i = 0;
            var request ="";
            //Svetlana - April 15, 2010 Initialize functon is generic now; added 3 parameter (system type)
           foreach(DataRow fields in rsKickBack.Rows)
            { 
                  request= CommonUtility.GetStringValue(fields["Request"]);
        
            string KickBackBuffer = "";
                SocketWrench socket = new SocketWrench();
            KickBackBuffer = socket.Connect(request, _policyManager.KICKBACK_IP, (short)Conversion.Val(_policyManager.KICKBACK_PRT), (short)Conversion.Val(timeout), out error);
            if (KickBackBuffer == "failed")
            {

                return false;

            }
 

                //Got Response from the Loyalty Server
                GotResponse = false;

                TimeOut = System.Convert.ToInt16(_policyManager.KICKBACK_TMT);
                timeIN = (float)DateAndTime.Timer;
                while (!(DateAndTime.Timer - timeIN > TimeOut))
                {
                   
                    if (!string.IsNullOrEmpty(KickBackBuffer))
                    {
                        GotResponse = true;
                        break;
                    }
                    if (DateAndTime.Timer < timeIN)
                    {
                        timeIN = (float)DateAndTime.Timer;
                    }
                }

                if (!GotResponse)
                {
                    
                    return false;
                }
                _kickBackService.DeleteKickbackQueue();
                
            }

            
            returnValue = true;

            rsKickBack = null;
           
            return returnValue;
        }

        #endregion
    }
}
