using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System.Linq;
using System;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class FuelPrepayManager : ManagerBase, IFuelPrepayManager
    {

        private readonly ISaleManager _saleManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly IPolicyManager _policyManager;
        private readonly IPrepayManager _prepayManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly IPrepayService _prepayService;
        private readonly ICustomerManager _customerManager;
        private readonly ITaxManager _taxManager;
        private readonly ISaleService _saleService;
        private readonly ITreatyService _treatyService;
        private readonly ISiteMessageService _siteMessageService;
        private readonly ITaxExemptService _taxExemptService;
        private readonly ITaxExemptSaleManager _taxExemptManager;
        private readonly IPurchaseItemManager _purchaseItemManager;
        private readonly IPurchaseListManager _purchaseListManager;
        private readonly IReceiptManager _receiptManager;
        private readonly ITreatyManager _treatyManager;
        private readonly IEncryptDecryptUtilityManager _encryptDecryptManager;
        private readonly ITillService _tillService;

        byte[] _pumpStLocal = new byte[33];


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="saleManager">Sale manager</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <param name="policyManager"></param>
        /// <param name="prepayManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="prepayService"></param>
        /// <param name="customerManager"></param>
        /// <param name="taxManager"></param>
        /// <param name="saleService"></param>
        /// <param name="treatyService"></param>
        /// <param name="siteMessageService"></param>
        /// <param name="taxExemptSaleManager"></param>
        /// <param name="taxExemptService"></param>
        /// <param name="purchaseListManager"></param>
        /// <param name="purchaseItemManager"></param>
        /// <param name="receiptManager"></param>
        /// <param name="treatyManager"></param>
        /// <param name="encryptDecryptManager"></param>
        /// <param name="tillService"></param>
        public FuelPrepayManager(ISaleManager saleManager,
            IApiResourceManager resourceManager,
            IPolicyManager policyManager,
            IPrepayManager prepayManager,
            ISaleLineManager saleLineManager,
            IPrepayService prepayService,
            ICustomerManager customerManager,
            ITaxManager taxManager,
            ISaleService saleService,
            ITreatyService treatyService,
            ISiteMessageService siteMessageService,
            ITaxExemptSaleManager taxExemptSaleManager,
            ITaxExemptService taxExemptService,
            IPurchaseListManager purchaseListManager,
            IPurchaseItemManager purchaseItemManager,
            IReceiptManager receiptManager,
            ITreatyManager treatyManager,
            IEncryptDecryptUtilityManager encryptDecryptManager,
            ITillService tillService)
        {
            _saleManager = saleManager;
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _prepayManager = prepayManager;
            _saleLineManager = saleLineManager;
            _prepayService = prepayService;
            _customerManager = customerManager;
            _taxManager = taxManager;
            _saleService = saleService;
            _treatyService = treatyService;
            _siteMessageService = siteMessageService;
            _taxExemptManager = taxExemptSaleManager;
            _taxExemptService = taxExemptService;
            _purchaseItemManager = purchaseItemManager;
            _purchaseListManager = purchaseListManager;
            _receiptManager = receiptManager;
            _treatyManager = treatyManager;
            _encryptDecryptManager = encryptDecryptManager;
            _tillService = tillService;
        }


        //public void RefreshPrepayPump(short activePump)
        //{
        //    short I = 0;

        //    for (I = 1; I <= Variables.iPumpCount; I++)
        //    {
        //        if (Variables.Pump[I].IsHoldPrepay && !Variables.Pump[I].IsPrepay)
        //        {
        //            _pumpStLocal[I] = (byte)5;
        //        }
        //        else if (Variables.Pump[I].IsPrepay)
        //        {
        //            _pumpStLocal[I] = (byte)4;
        //        }
        //        else 
        //        {
        //            if (I == activePump && (_pumpStLocal[I] == 2)) 
        //            {

        //            }
        //            else
        //            {
        //                _pumpStLocal[I] = (byte)1;
        //            }
        //        }
        //    }

        //}

        //public void RefreshPrepayForPump(short pumpID, short activePump)
        //{

        //    if (Variables.Pump[pumpID].IsHoldPrepay && !Variables.Pump[pumpID].IsPrepay)
        //    {
        //        _pumpStLocal[pumpID] = (byte)5;
        //    }
        //    else if (Variables.Pump[pumpID].IsPrepay)
        //    {
        //        _pumpStLocal[pumpID] = (byte)4;
        //        
        //        
        //    }
        //    else 
        //    {
        //        if (pumpID == activePump && (_pumpStLocal[pumpID] == 2)) 
        //        {
        //        }
        //        else
        //        {
        //            _pumpStLocal[pumpID] = (byte)1;
        //        }
        //    }

        //}

        /// <summary>
        /// Method to add prepay
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="activePump">Active pump</param>
        /// <param name="amountOn">Amount</param>
        /// <param name="cobGradeText">Grade text</param>
        /// <param name="isAmountTypeCash">Amount type</param>
        /// <param name="error">Error message</param>
        /// <returns>Sale</returns>
        public Sale AddPrepay(int saleNumber, int tillNumber, short activePump, float
            amountOn, string cobGradeText, bool isAmountTypeCash, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var sale = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            //dynamic Prepay_Renamed = default(dynamic);
            ////dynamic TCPAgent.Instance = default(dynamic);
            short positionId;
            short loyalPricecode;
            float price;
            float taxExemptPrice;
            short gradeId;
            string response = "";
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (sale.DeletePrepay)
            {
                //Please complete delete prepay first!~Comlete current transaction.
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Information;
                //Chaps_Main.DisplayMessage(0, (short)1150, temp_VbStyle, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1150, null, InformationOkMessageType)
                };
                return null;
            }

            if (_prepayManager.PrepayItemId(ref sale) > 0)
            {

                //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)59, temp_VbStyle2, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, (short)59, null, CriticalOkMessageType)
                };
                return null;
            }


            if (Variables.Pump[activePump].IsPrepay || Variables.Pump[activePump].IsHoldPrepay)
            {

                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, (short)80, activePump, CriticalOkMessageType)
                };
                return null;
            }

            if (amountOn == 0)
            {
                //Enter Prepayment value first !
                //Chaps_Main.DisplayMessage(this, (short)91, MsgBoxStyle.Critical, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 91, null, CriticalOkMessageType)
                };
                //CompleteFinished = true;
                return null;
            }


            if (amountOn > 999.99)
            {
                //Maximum amount for prepay is 999.99~Prepay
                //Chaps_Main.DisplayMessage(this, (short)90, MsgBoxStyle.Critical, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 90, null, CriticalOkMessageType)
                };
                //CompleteFinished = true;
                return null;
            }


            if (cobGradeText.Trim() == "")
            {

                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 66, null, CriticalOkMessageType)
                };
                //CompleteFinished = true;
                return null;
            }
            var I = (short)(cobGradeText.Trim().IndexOf("-", StringComparison.Ordinal) + 1);
            if (I > 1)
            {
                positionId = (short)Conversion.Val(Strings.Left(cobGradeText.Trim(), I - 1));

                gradeId = Convert.ToInt16(Variables.gPumps.get_Assignment((byte)activePump, (byte)positionId).GradeID);
            }
            else
            {

                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 66, null, CriticalOkMessageType)
                };
                //CompleteFinished = true;
                return null;
            }

            if (isAmountTypeCash)
            {
                if (Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CashPrice == 0)
                {

                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 25, 67, null, CriticalOkMessageType)
                    };
                    //CompleteFinished = true;
                    return null;
                }
            }
            else
            {
                if (Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CreditPrice == 0)
                {

                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 25, 67, null, CriticalOkMessageType)
                    };
                    //CompleteFinished = true;
                    return null;
                }
            }

            //this.Enabled = false;


            if (!TCPAgent.Instance.IsConnected)
            {
                //this.Enabled = true;
                //        MsgBox ("Communication problem, cannot set prepay!~Set Prepayment Error!")
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 93, null, CriticalOkMessageType)
                };
                //CompleteFinished = true;
                return null;
            }

            //IsWaiting = true;
            var strPumpId = Strings.Right("00" + Convert.ToString(activePump), 2);


            string tempCommandRenamed = "HPP" + strPumpId;
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
            //Response = "";
            var strRemain = "";
            var timeIN = (float)DateAndTime.Timer;
            while (!(DateAndTime.Timer - timeIN > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting HPP" + strPumpId);
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "HPP" + strPumpId, ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify PortReading from holding prepay: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIN)
                {
                    timeIN = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            if (Strings.Left(response, 8) == "HPP" + strPumpId + "ERR")
            {
                //this.Enabled = true;
                //        MsgBox ("Cannot set prepay for this pump!~Set Prepayment Error!")
                //Chaps_Main.DisplayMessage(this, (short)68, MsgBoxStyle.Critical, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 68, null, CriticalOkMessageType)
                };
                //CompleteFinished = true;
                return null;
            }

            if (Strings.Left(response, 7) != "HPP" + strPumpId + "OK")
            {
                //this.Enabled = true;
                //        MsgBox ("Invalid response from FuelControl, cannot set prepay!~Set Prepayment Error!")
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 69, null, CriticalOkMessageType)
                };
                //CompleteFinished = true;
                return null;
            }

            _prepayManager.HoldPrepayment(activePump);


            object prepayAmount = amountOn;

            prepayAmount = string.Format(prepayAmount.ToString(), "####.00");

            prepayAmount = Strings.Replace(Convert.ToString(prepayAmount), ".", "", 1, 1, CompareMethod.Text);

            prepayAmount = Strings.Right("000000" + Convert.ToString(prepayAmount), 6);


            Variables.PrepAmount = prepayAmount;

            Prepay prepay = new Prepay
            {
                PrepAmount = amountOn,
                Position = (short)Conversion.Val(Strings.Left(cobGradeText.Trim(), 1))
            };


            Variables.Pump[activePump].PrepayAmount = amountOn;
            Variables.Pump[activePump].PrepayInvoiceID = sale.Sale_Num;
            Variables.Pump[activePump].PrepayPosition = (short)Conversion.Val(Strings.Left(cobGradeText.Trim(), 1));

            //if (!isAmountTypeCash)
            //{
            //    Chaps_Main.PrepayPriceType = '2';
            //}
            //else
            //{
            //    Chaps_Main.PrepayPriceType = '1';
            //}

            //Chaps_Main.Transaction_Type = "Prepay";

            var sl = new Sale_Line();
            //updated plu code setter
            //SL.PLU_Code = System.Convert.ToString(Variables.gPumps.get_Grade((byte)GradeID).Stock_Code);
            _saleLineManager.SetPluCode(ref sale, ref sl, Convert.ToString(Variables.gPumps.get_Grade((byte)gradeId).Stock_Code), out error);

            if (isAmountTypeCash)
            {
                price = float.Parse(Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CashPrice.ToString("##0.000"));
                taxExemptPrice = float.Parse(Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).teCashPrice.ToString("##0.000")); // 
            }
            else
            {
                price = float.Parse(Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CreditPrice.ToString("##0.000"));
                taxExemptPrice = float.Parse(Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).teCreditPrice.ToString("##0.000")); // 
            }
            var dblPrice = double.Parse(price.ToString("##0.000"));
            var quantity = float.Parse((amountOn / price).ToString("##0.000"));
            //SL.Quantity = float.Parse(Quantity.ToString("##0.000"));
            _saleLineManager.SetQuantity(ref sl, float.Parse(quantity.ToString("##0.000")));
            sl.Regular_Price = double.Parse(price.ToString("##0.000"));
            sl.TEPrice = double.Parse(taxExemptPrice.ToString("##0.000")); // 
            sl.pumpID = (byte)activePump;
            sl.PositionID = (byte)positionId;
            sl.GradeID = (byte)gradeId;
            sl.MOP = Convert.ToByte(isAmountTypeCash ? 1 : 2);
            sl.Total_Amount = (decimal)amountOn; //  - to keep the real amount showing in pump
            sl.Prepay = true;

            _saleManager.Add_a_Line(ref sale, sl, UserCode, sale.TillNumber, out error, true);
            _saleManager.Line_Price(ref sale, ref sl, dblPrice);

            // sale.Line_Price(SL, dblPrice);
            //Chaps_Main.SC = SL.Stock_Code;


            //SaleMain.Default.Refresh_Lines();

            //if (_policyManager.SOUND_SYS)
            //{
            //    MDIfrmPump.Default.PlaySound((byte)3, (byte)3);
            //}
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
            return sale;
        }

        /// <summary>
        /// Method to switch prepay
        /// </summary>
        /// <param name="activePump">Active pump id</param>
        /// <param name="newPumpId">New pump id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public bool SwitchPrepay(short activePump, short newPumpId, int saleNumber, int tillNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            ////dynamic TCPAgent.Instance = default(dynamic);
            float timeIn = 0;

            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!TCPAgent.Instance.IsConnected)
            {

                //Chaps_Main.DisplayMessage(this, (short)22, MsgBoxStyle.Critical, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 22, null, CriticalOkMessageType)
                };
                return false;
            }

            if (newPumpId != 0)
            {
                if (activePump != newPumpId)
                {
                    if (Variables.PumpStatus[activePump] != 1)
                    {

                        //Chaps_Main.DisplayMessage(this, (short)23, MsgBoxStyle.Critical, activePump, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 25, 23, activePump, CriticalOkMessageType)
                        };
                        return false;
                    }
                    if (Variables.PumpStatus[newPumpId] != 1)
                    {

                        //Chaps_Main.DisplayMessage(this, (short)24, MsgBoxStyle.Critical, newPumpId, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 25, 24, newPumpId, CriticalOkMessageType)
                        };
                        return false;
                    }
                    if (Variables.gBasket[newPumpId].CurrentFilled && Variables.gBasket[newPumpId].StackFilled)
                    {

                        //Chaps_Main.DisplayMessage(this, (short)25, MsgBoxStyle.Critical, newPumpId, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 25, 25, newPumpId, CriticalOkMessageType)
                        };
                        return false;
                    }
                    if (Variables.Pump[newPumpId].IsPrepay || Variables.Pump[newPumpId].IsHoldPrepay)
                    {

                        //Chaps_Main.DisplayMessage(this, (short)26, MsgBoxStyle.Critical, newPumpId, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 25, 26, newPumpId, CriticalOkMessageType)
                        };
                        return false;
                    }

                    //this.Enabled = false;
                    //cmdExit_.Enabled = false;
                    //cmdSwitch.Enabled = false;
                    //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    //CompleteFinished = false;
                    //IsWaiting = true;

                    //if (timeIN > DateAndTime.Timer)
                    //{
                    //    timeIN = 0; //reset on midnight
                    //}
                    //else
                    //{
                    //    timeIN = (float)DateAndTime.Timer;
                    //}

                    saleNumber = Variables.Pump[activePump].PrepayInvoiceID;

                    string tempCommandRenamed = "SPR" + Strings.Right("00" + Convert.ToString(activePump), 2) + Strings.Right("00" + Convert.ToString(newPumpId), 2);
                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

                    string response = "";
                    string strRemain = "";
                    var timeIN = (float)DateAndTime.Timer;
                    while (!(DateAndTime.Timer - timeIN > Variables.gPumps.CommunicationTimeOut))
                    {
                        var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                        if (!string.IsNullOrEmpty(strBuffer))
                        {
                            modStringPad.SplitResponse(strBuffer, "SPR" + Strings.Right("00" + Convert.ToString(activePump), 2) + Strings.Right("00" + Convert.ToString(newPumpId), 2), ref response, ref strRemain);
                            if (!string.IsNullOrEmpty(response)) //got what we are waiting
                            {
                                TCPAgent.Instance.PortReading = strRemain;
                                break;
                            }
                        }
                        if (DateAndTime.Timer < timeIN)
                        {
                            timeIN = (float)DateAndTime.Timer;
                        }
                        Variables.Sleep(100);
                    }

                    if (Strings.Left(response, 9) != "SPR" + Strings.Right("00" + Convert.ToString(activePump), 2) + Strings.Right("00" + Convert.ToString(newPumpId), 2) + "OK")
                    {
                        if (Strings.Left(response, 10) == "SPR" + Strings.Right("00" + Convert.ToString(activePump), 2) + Strings.Right("00" + Convert.ToString(newPumpId), 2) + "ERR")
                        {

                            //Chaps_Main.DisplayMessage(this, (short)27, MsgBoxStyle.Critical, null, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 25, 27, null, CriticalOkMessageType)
                            };
                        }
                        else
                        {

                            //Chaps_Main.DisplayMessage(this, (short)28, MsgBoxStyle.Critical, null, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 25, 28, null, CriticalOkMessageType)
                            };
                        }
                        // this.Enabled = true;

                        //this.Cursor = System.Windows.Forms.Cursors.Arrow;
                        //CompleteFinished = true;
                        //cmdExit_.Enabled = true;
                        //IsWaiting = false;

                        //this.Close();

                        return false;
                    }
                    //else
                    //{

                    _prepayService.UpdatePrepayPumpIdForSale(saleNumber, tillNumber, newPumpId);
                    _prepayManager.SwitchPrepayment(activePump, newPumpId);

                    //}
                }
                //IsWaiting = false;
                //this.Enabled = true;
                //this.Cursor = System.Windows.Forms.Cursors.Arrow;

                //this.Close();

                return true;
            }
            return true;
        }

        /// <summary>
        /// Method to delete prepay
        /// </summary>
        /// <param name="activePump">Active pump id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="error">Error message</param>
        /// <returns>Sale summary response</returns>
        public SaleSummaryResponse DeletePrepay(short activePump, int saleNumber, int tillNumber, int shiftNumber, byte registerNumber, out ErrorMessage error)
        {
            Store storeRenamed = _policyManager.LoadStoreInfo();
            var sale = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            var oTeSale = CacheManager.GetTaxExemptSaleForTill(tillNumber, saleNumber);
            var offSet = storeRenamed.OffSet;
            if (!Variables.Pump[activePump].IsPrepay)// || !Variables.Pump[activePump].IsHoldPrepay)
            {

                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)20, temp_VbStyle, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 20, null, CriticalOkMessageType)
                };
                return null;
            }

            if (sale?.Sale_Lines.Count > 0)
            {

                //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)21, temp_VbStyle2, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 21, null, CriticalOkMessageType)
                };
                return null;
            }

            sale = _saleService.GetSaleBySaleNoFromDbTill(ref sale, tillNumber, Variables.Pump[activePump].PrepayInvoiceID);
            var saleLine = _saleService.GetPrepaySaleLine(Variables.Pump[activePump].PrepayInvoiceID, tillNumber);

            if (sale == null || saleLine == null)
            {

                //Chaps_Main.DisplayMessage(this, (short)20, temp_VbStyle3, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 20, null, CriticalOkMessageType)
                };

                return null;
            }

            if (!HoldDeletingPrepay(activePump, out error))
            {
                //this.Enabled = true;

                //MsgBoxStyle temp_VbStyle4 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)20, temp_VbStyle4, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 20, null, CriticalOkMessageType)
                };
                return null;
            }

            sale.Customer = _customerManager.LoadCustomer(sale.Customer.Code);
            sale.Void_Num = Variables.Pump[activePump].PrepayInvoiceID;
            sale.Register = registerNumber;

            if (_policyManager.TE_Type != "AITE" && _policyManager.TE_Type != "QITE" && _policyManager.TE_GETNAME)
            {
                sale.TreatyName = _treatyService.GetTreatyName(sale.TreatyNumber);
            }
            //shiny end


            //Added to load the original Loyalty Card
            _saleService.UpdateDiscountTender(ref sale);


            var sl = new Sale_Line
            {
                Dept = saleLine.Dept,
                Sub_Dept = saleLine.Sub_Dept,
                Stock_Code = saleLine.Stock_Code
            };

            _saleLineManager.SetSubDetail(ref sl, saleLine.Sub_Detail);
            _saleLineManager.SetPluCode(ref sale, ref sl, saleLine.PLU_Code, out error);
            _saleLineManager.SetPrice(ref sl, saleLine.price);
            _saleLineManager.SetQuantity(ref sl, saleLine.Quantity * -1);
            sl.Regular_Price = saleLine.Regular_Price;

            sl.pumpID = (byte)activePump;
            sl.PositionID = saleLine.PositionID;
            sl.GradeID = saleLine.GradeID;

            sl.User = UserCode;
            sl.Description = saleLine.Description;
            sl.Prepay = true;

            // Initialize the dll communication and create the object RTVPService
            short ret;
            if (_policyManager.SITE_RTVAL)
            {

                Variables.RTVPService = new RTVP.POSService.Transaction();

                ret = Convert.ToInt16(Variables.RTVPService.SetRetailer(Convert.ToInt32(storeRenamed.RetailerID), sale.TillNumber, sale.Sale_Num));

                WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetRetailer sent with parameters " + storeRenamed.RetailerID + "," + Convert.ToString(sale.TillNumber) + "," + Convert.ToString(sale.Sale_Num));

                short tillMode;
                if (CacheManager.GetUser(UserCode).User_Group.Code == Entities.Constants.Trainer)
                {
                    tillMode = 2;
                }
                else
                {
                    tillMode = 1;
                }
                ret = Convert.ToInt16(Variables.RTVPService.SetCustomer(sale.TreatyNumber, 1));
                WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetCustomer sent with parameters " + sale.TreatyNumber + ",1");
                ret = Convert.ToInt16(Variables.RTVPService.SetTillMode(tillMode));
                WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetTillMode sent with parameter " + Convert.ToString(tillMode));
            }
            var oPurchaseList = new tePurchaseList();
            var oTreatyNo = new teTreatyNo();

            if (_taxExemptManager.LoadTaxExemptPrepay(Variables.Pump[activePump].PrepayInvoiceID, sl, saleLine.Line_Num, ref sale, ref oPurchaseList, ref oTreatyNo))
            {
                sl.IsTaxExemptItem = true;


                _saleManager.ApplyTaxes(ref sale, false);
            }
            else if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type == "AITE")
            {
                if (oTeSale.teCardholder.GstExempt)
                {
                    oTeSale.TillNumber = Convert.ToByte(tillNumber);
                    oTeSale.Sale_Num = sale.Sale_Num;
                    oTeSale.Shift = Convert.ToInt16(shiftNumber);
                    oTeSale.UserCode = UserCode;
                    _saleManager.ApplyTaxes(ref sale, false);
                }
            }


            sl.Line_Discount = saleLine.Line_Discount;

            sl.FuelRebateEligible = saleLine.FuelRebateEligible;

            sl.FuelRebate = saleLine.FuelRebate;
            if (sl.FuelRebateEligible && sl.FuelRebate > 0 && sale.Customer.UseFuelRebate && sale.Customer.UseFuelRebateDiscount)
            {
                _saleLineManager.ApplyFuelRebate(ref sl);

                _saleLineManager.SetAmount(ref sl, decimal.Parse((-1 * saleLine.Amount).ToString("##0.00")));
            }
            else
            {




                if (sale.Customer.GroupID != "" && _policyManager.FuelLoyalty)
                {
                    //  - For site prepay don't reduce discounts, since we are giving more gas during sale for the amount of discount + tax exempt
                    if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type == "SITE" && sl.Prepay && sl.IsTaxExemptItem)
                    {
                        //SL.Amount = System.Convert.ToDecimal(System.Convert.ToInt32(rsDetail.Fields["Amount"].Value) * (-1));
                        _saleLineManager.SetAmount(ref sl, Convert.ToDecimal(saleLine.Amount * -1));
                    }
                    else
                    {
                        _saleLineManager.SetAmount(ref sl, Convert.ToDecimal(saleLine.Amount * -1));
                    }
                }
                else
                {
                    _saleLineManager.SetAmount(ref sl, Convert.ToDecimal(saleLine.Amount * -1));
                    _saleLineManager.SetDiscountRate(ref sl, 0);
                }

            }
            sl.Total_Amount = Convert.ToDecimal(-1 * saleLine.Total_Amount);

            Chaps_Main.Transaction_Type = "Delete Prepay";

            _saleManager.Add_a_Line(ref sale, sl, UserCode, sale.TillNumber, out error, true);

            Chaps_Main.SC = sl.Stock_Code;
            //var oPurchaseList = CacheManager.GetPurchaseListSaleForTill(tillNumber, saleNumber);
            //   real time validation SITE
            if (_policyManager.SITE_RTVAL)
            {
                if (oPurchaseList?.Count() > 0)
                {
                    // GetCustomerStatus call is not necessary (based on email from SITE, it is not mandatory)
                    // LimitRequest call returns the status of the customer
                    ret = 999; // set to a value that is not a possible response from RTVP function call
                    ret = Convert.ToInt16(Variables.RTVPService.LimitRequest());


                    WriteToLogFile("Response is " + Convert.ToString(ret) + " from LimitRequest sent with no parameters");
                    if (ret == 0 | ret == 4)
                    {
                        // valid treaty number, no message to display, complete the transaction
                        oTreatyNo.ValidTreatyNo = true;
                    }
                    else
                    {
                        var strMessage = _siteMessageService.GetSiteMessage(ret);
                        if (!string.IsNullOrEmpty(strMessage))
                        {
                            error = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    MessageType = CriticalOkMessageType,
                                    Message = strMessage
                                }
                            };
                        }
                        else
                        {
                            //Chaps_Main.DisplayMsgForm(Chaps_Main.GetResString((short)1118), (short)99, null, (byte)0, (byte)0, "", "", "", "");
                            error = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    MessageType = CriticalOkMessageType,
                                    Message = _resourceManager.GetResString(offSet, 1118)
                                }
                            };
                        }

                        if ((ret >= 5 && ret <= 18) || (ret >= 22 && ret <= 25))
                        {
                            oTreatyNo.ValidTreatyNo = false;
                        } // over the limit
                        else if ((ret >= 1 && ret <= 3) || (ret >= 19 && ret <= 21))
                        {
                            oTreatyNo.ValidTreatyNo = true;
                        }
                        return null;
                    }
                }
                else
                {
                    oTreatyNo.ValidTreatyNo = false;
                }
            }
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);

            var result = _taxManager.GetSaleSummary(new SaleSummaryInput
            {
                TillNumber = tillNumber,
                IsSiteValidated = false,
                IsAiteValidated = false,
                UserCode = UserCode,
                RegisterNumber = registerNumber,
                SaleNumber = saleNumber
            }, out error);
            return result;
        }

        //This function is created by   - to move the switch prepay basket handling 
        //from Readudp -to solve POS freezing - "CHANGE PREPAY GRADE"
        public void Finish_SwitchPrepayBaskets(int tillNumber, out string changeDue,
            out bool openDrawer, out Report report)
        {
            changeDue = "0.00";
            openDrawer = false;
            report = null;
            if (string.IsNullOrEmpty(Variables.SwitchPrepayBaskets))
            {
                return; // No switch basket
            }
            WriteToLogFile("Processing SwitchPrepayBaskets");

            var swPrepayBasket = Strings.Split(Variables.SwitchPrepayBaskets, ";", -1, CompareMethod.Text);
            var boolError = false;

            if (swPrepayBasket.Length - 1 > 0) // we splitted the switch prepay baskets
            {
                short i;
                for (i = 0; i <= swPrepayBasket.Length - 1 - 1; i++)
                {

                    var ubasking = Convert.ToString(swPrepayBasket[i]);
                    WriteToLogFile("Switch Prepay Basket " + ubasking);
                    //Following section is copied from READUDP-  
                    var basketVal = ubasking.Split('.');
                    if (RemovePrepayBasket("D" + ubasking.Substring(4, 2), basketVal[1].Trim()))
                    {
                        // Get rid of ubasking-
                        UpdatePrepaySale(int.Parse(basketVal[1]),
                            float.Parse((double.Parse(ubasking.Substring(8, 8)) / 1000).ToString("##0.00")),
                            float.Parse((double.Parse(ubasking.Substring(16, 8)) / 1000).ToString("##0.000")),
                            float.Parse((double.Parse(ubasking.Substring(8, 8)) / (double.Parse(ubasking.Substring(16, 8)))).ToString("##0.000")),
                            0, false, false, short.Parse(ubasking.Substring(7, 1)),
                            Variables.gPumpPositionGrade[int.Parse(ubasking.Substring(4, 2)),
                            int.Parse(ubasking.Substring(7, 1))], tillNumber, out changeDue, out openDrawer, out report);
                        WriteToLogFile("Finished the Switch prepay basket " + ubasking);
                    }
                    else
                    {
                        boolError = true;
                    }
                    //This is copied from READUDP-   end
                }
                WriteToLogFile(" Finish the Processing of current SwitchPrepayBaskets");
                if (!boolError)
                {
                    Variables.SwitchPrepayBaskets = ""; // processed
                }
            }
        }

        /// <summary>
        /// Method to remove prepay basket
        /// </summary>
        /// <param name="baskId">Basket Id</param>
        /// <param name="prepayInvoice">Prepay invoice</param>
        /// <returns>True ror false</returns>
        public bool RemovePrepayBasket(string baskId, string prepayInvoice)
        {
            //dynamic TCPAgent.Instance = default(dynamic);






            if (baskId.Substring(0, 1) != "P" && baskId.Substring(0, 1) != "D" || !TCPAgent.Instance.IsConnected)
            {


                return false;
            }

            var response = "";
            var strRemain = "";
            Variables.IsWaiting = true;
            var timeIn = (float)DateAndTime.Timer;

            string tempCommandRenamed = "Rmv" + baskId + "," + prepayInvoice.Trim() + ".";
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
            {

                var strBuffer = System.Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Rmv" + baskId + "," + prepayInvoice.Trim() + ".");
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "Rmv" + baskId, ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {

                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify TCPAgent.PortReading from remove Prepay Basket: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            Variables.IsWaiting = false;

            if (response != null && response.Contains("Rmv" + baskId + "ERR"))
            {
                return false;
            }

            if (response != null && response.IndexOf("Rmv" + baskId + "OK", StringComparison.Ordinal) + 1 < 1)
            {
                string tempCommandRenamed2 = "ENDPOS";
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed2, true);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to update prepay sale
        /// </summary>
        /// <param name="invoiceId">Sale number</param>
        /// <param name="saleAmount">Sale amount</param>
        /// <param name="saleQuantity">Sale quantity</param>
        /// <param name="unitPrice">Unit price</param>
        /// <param name="lessAmount">Less amount</param>
        /// <param name="overPayment">Over payment</param>
        /// <param name="goToTender">Go to tender</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="openDrawer">Open drawer</param>
        /// <param name="fs">File stream</param>
        /// <param name="taxExemptReport">Tax exempt voucher</param>
        /// <returns>True or false</returns>
        public bool UpdatePrepaySale(int invoiceId, float saleAmount, float saleQuantity, float unitPrice,
            float lessAmount, bool overPayment, bool goToTender, short iPositionId, short iGradeId, int tillNumber,
            out string changeDue, out bool openDrawer, out Report taxExemptReport)
        {
            bool returnValue = false;
            changeDue = "0.00";
            openDrawer = false;
            taxExemptReport = null;
            WriteToLogFile("UpdatepreparySale LessAmount value is " + Convert.ToString(lessAmount));


            Sale_Line sl = default(Sale_Line);
            DataSource db;
            double change;
            float lessQuantity = 0;


            var ppTeSale = default(TaxExemptSale);
            float newTotalAmount = 0;
            Sale salTmp = default(Sale);
            Sale_Line slTmp = default(Sale_Line);
            short orgPosition = 0;
            string newDescription = "";
            //End - SV
            Sale_Line sp = default(Sale_Line); //  - to fix the prepay screw up of products in switch prepay and with fuel loyalty. ( fuel loyalty reused the SL and changed the setting)
            decimal pennyAdj = new decimal();
            var oPurchaseList = CacheManager.GetPurchaseListSaleForTill(tillNumber, invoiceId);


            var saleHead = _prepayService.GetOriginalSaleHead(invoiceId, tillNumber, out db);
            if (saleHead == null)
            {
                return false;
            }
            var saleLines = _prepayService.GetOrginalSaleLines(invoiceId, tillNumber, db);

            if (saleLines.Count == 0)
            {
                return false;
            }

            // Need to get the original positionId
            var positionId = _prepayService.GetOrginalPositionId(invoiceId, tillNumber, db);
            if (positionId == null)
            {
                return false;
            }
            orgPosition = positionId.Value; //


            if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type != "SITE")
            {
                ppTeSale = _taxExemptService.LoadTaxExempt(_policyManager.TE_Type, invoiceId, (byte)saleHead.TillNumber, db, false);
            }

            //  - reload the Purchaseitem list. If a sale happend in before finishing a partial\over prepay this list need to be reloaded
            if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type == "SITE")
            {

                ReloadTaxexempt_SITE(invoiceId, (short)saleHead.TillNumber, db);
            }
            // 


            var sx = new Sale();
            string strLoyaltyCard;
            string strCouponId;
            sx.LoadingTemp = true;
            sx.Sale_Num = saleHead.SaleNumber;
            sx.TillNumber = (byte)saleHead.TillNumber;

            sx.Customer.Code = saleHead.Client;
            Chaps_Main.CL = sx.Customer.Code; //   CL has to keep Customer code
            sx.Customer = _customerManager.LoadCustomer(sx.Customer.Code);
            sx.Sale_Type = saleHead.TType;
            sx.Void_Num = invoiceId;
            sx.Sale_Deposit = saleHead.Deposit;

            sx.TreatyNumber = saleHead.TreatyNumber;
            // 
            if ((_policyManager.TE_Type != "AITE" && _policyManager.TE_Type != "QITE") && _policyManager.TE_GETNAME)
            {
                sx.TreatyName = _treatyService.GetTreatyName(sx.TreatyNumber);
            }
            //shiny end

            //   added penny adjustments to the change

            sx.Sale_Change = Convert.ToDecimal(Convert.ToDouble(saleHead.Change) + Convert.ToDouble(saleHead.PennyAdjust)); //  - This is getting the original change for the prepay

            WriteToLogFile("Loaded penny adjustment is " + Convert.ToString(saleHead.PennyAdjust));


            if (_policyManager.FuelLoyalty && !string.IsNullOrEmpty(sx.Customer.GroupID))
            {


                //                        InvoiceID & " AND TILL_NUM=" & Till.Number, _
                //                        db, adOpenForwardOnly, adLockReadOnly)
                var discountTender = _prepayService.GetDiscountTender(invoiceId, tillNumber, db);

                strLoyaltyCard = discountTender.CardNumber;
                strCouponId = discountTender.CouponId;

            }
            else
            {
                strLoyaltyCard = "";
                strCouponId = "";
            }
            sx.Customer.LoyaltyCard = strLoyaltyCard;
            sx.CouponID = strCouponId;




            bool fullSwitch = false;
            //   - to handle the overpumping also should be considered as full switch
            foreach (var saleLine in saleLines)
            {

                sl = new Sale_Line
                {
                    No_Loading = true,
                    Dept = saleLine.Dept,
                    Sub_Dept = saleLine.Sub_Dept,
                    Stock_Code = saleLine.Stock_Code
                };






                _saleLineManager.SetSubDetail(ref sl, saleLine.Sub_Detail);


                ErrorMessage error;
                // _saleLineManager.SetPluCode(ref SX, ref SL, saleLine.PLU_Code, out error);
                sl.Line_Num = saleLine.Line_Num;
                sl.Price_Type = saleLine.Price_Type;
                sl.Quantity = saleLine.Quantity;
                sl.Amount = saleLine.Amount;
                sl.Discount_Adjust = saleLine.Discount_Adjust;
                sl.Line_Discount = saleLine.Line_Discount;
                sl.Discount_Type = saleLine.Discount_Type;
                sl.Discount_Code = saleLine.Discount_Code;
                _saleLineManager.SetDiscountRate(ref sl, saleLine.Discount_Rate);
                sl.DiscountName = saleLine.DiscountName; // 
                sl.Associate_Amount = saleLine.Associate_Amount;
                sl.User = saleLine.User;
                sl.Description = saleLine.Description;
                sl.Loyalty_Save = saleLine.Loyalty_Save;
                sl.Units = saleLine.Units;
                sl.Serial_No = saleLine.Serial_No;
                sl.Prepay = saleLine.Prepay;
                sl.pumpID = saleLine.pumpID;
                sl.GradeID = saleLine.GradeID;
                sl.PositionID = saleLine.PositionID;
                sl.IsTaxExemptItem = saleLine.IsTaxExemptItem; //  - to identify taxexempt items on partial prepay



                sl.Total_Amount = saleLine.Total_Amount;
                // Added Prepay Condition since it was updating all items not just prepay
                if (iPositionId != orgPosition && saleLine.Prepay)
                {
                    // Added to update position and grade id's
                    sl.GradeID = (byte)iGradeId;
                    sl.PositionID = (byte)iPositionId;
                    //End - SV

                    // _saleLineManager.SetPluCode(ref SX, ref SL, System.Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeID).Stock_Code), out error);
                    sl.Stock_Code = Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeId).Stock_Code);
                    //  - to identify full switch
                    if (saleAmount >= (float)sl.Total_Amount && sl.IsTaxExemptItem || (saleAmount >= (float)sl.Amount && sl.IsTaxExemptItem == false))
                    {
                        fullSwitch = true;
                    }
                    else
                    {
                        fullSwitch = false;
                    }
                    // 
                    var description = _prepayService.GetStockDescription(sl.Stock_Code);
                    newDescription = description ??
                                     Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeId).Stock_Code);
                    sl.Description = newDescription;
                    if (_policyManager.TAX_EXEMPT && !string.IsNullOrEmpty(sx.TreatyNumber))
                    {
                        sl.Regular_Price = unitPrice;
                        if (_policyManager.TE_Type == "AITE" || _policyManager.TE_Type == "QITE") // 
                        {


                            sl.price = unitPrice - (saleLine.Regular_Price - saleLine.price);
                        }
                        else
                        {
                            sl.price = Variables.gPumps.get_FuelPrice((byte)iGradeId, Convert.ToByte(Variables.gPumps.get_Pump(Convert.ToByte(saleLine.pumpID)).TierID), System.Convert.ToByte(Variables.gPumps.get_Pump(System.Convert.ToByte(saleLine.pumpID)).LevelID)).teCashPrice;
                        }
                    }
                    else
                    {
                        sl.price = unitPrice;
                        sl.Regular_Price = unitPrice;
                    }
                }
                else
                {
                    sl.price = saleLine.price;
                    sl.Regular_Price = saleLine.Regular_Price;
                    sl.Regular_Price = Math.Round(sl.Regular_Price, 3); // To format the double amount -  
                }




                if (iPositionId != orgPosition && saleLine.Prepay)
                {
                    sl.PLU_Code = Variables.gPumps.get_Grade((byte)iGradeId).Stock_Code;
                    //SL.PLU_Code = System.Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeID).Stock_Code);
                    //_saleLineManager.SetPluCode(ref SX, ref SL, System.Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeID).Stock_Code), out error);
                    sl.Stock_Code = Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeId).Stock_Code);
                    sl.Description = newDescription;

                    sp = sl; //  - Even if somebody used the SL object update saleline shouldn't get screwd up, so keeping the info in another object for update

                }




                sl.FuelRebateEligible = saleLine.FuelRebateEligible;

                sl.FuelRebate = saleLine.FuelRebate;




                if (_policyManager.TAX_EXEMPT && !string.IsNullOrEmpty(sx.TreatyNumber))
                {
                    sl.Total_Amount = (decimal)saleAmount;
                    newTotalAmount = (float)sl.Total_Amount;
                }



                if (sl.Prepay)
                {
                    // Pick up the taxes that were applied to the line.

                    sl.Line_Taxes = null;
                    var lineTaxes = _prepayService.GetLineTaxes(invoiceId, saleLine.Line_Num, db);
                    CalculateTax(sx, sl, saleAmount, saleQuantity, ref salTmp, ref slTmp);

                    foreach (var lineTax in lineTaxes)
                    {
                        sl.Line_Taxes.Add(lineTax.Tax_Name, lineTax.Tax_Code, lineTax.Tax_Rate, lineTax.Tax_Included,
                            lineTax.Tax_Rebate_Rate, lineTax.Tax_Rebate, "");
                    }


                    float taxCreditAmount = 0;
                    float taxIncldAmount = 0;
                    float includedTax = 0;
                    TaxExemptSaleLine ppTeLine;
                    if (_policyManager.TAX_EXEMPT && !string.IsNullOrEmpty(sx.TreatyNumber)
                        && (_policyManager.TE_Type == "AITE" || _policyManager.TE_Type == "QITE")) // 
                    {

                        ppTeSale.TaxCreditLines = new TaxCreditLines();
                        var tcl = new TaxCreditLine { Line_Num = sl.Line_Num };
                        foreach (Line_Tax tempLoopVarLtx in slTmp.Line_Taxes)
                        {
                            var ltx = tempLoopVarLtx;
                            taxIncldAmount = taxIncldAmount + ltx.Tax_Added_Amount;


                            taxCreditAmount = taxCreditAmount + ltx.Tax_Added_Amount + ltx.Tax_Incl_Amount;

                            includedTax = includedTax + ltx.Tax_Incl_Amount;

                            var lt = new Line_Tax
                            {
                                Tax_Added_Amount = ltx.Tax_Added_Amount,
                                Tax_Code = ltx.Tax_Code,
                                Tax_Hidden = ltx.Tax_Hidden,
                                Tax_Hidden_Total = ltx.Tax_Hidden_Total,
                                Tax_Incl_Amount = ltx.Tax_Incl_Amount,
                                Tax_Incl_Total = ltx.Tax_Incl_Total,
                                Tax_Included = ltx.Tax_Included,
                                Tax_Name = ltx.Tax_Name,
                                Tax_Rate = ltx.Tax_Rate,
                                Taxable_Amount = ltx.Taxable_Amount
                            };
                            tcl.Line_Taxes.AddTaxLine(lt, "");
                        }

                        if (tcl.Line_Taxes.Count > 0)
                        {


                            ppTeSale.TaxCreditLines.AddLine(tcl.Line_Num, tcl, "");

                        }
                        taxIncldAmount = taxIncldAmount + saleAmount;
                        foreach (TaxExemptSaleLine tempLoopVarPpTeLine in ppTeSale.Te_Sale_Lines)
                        {
                            ppTeLine = tempLoopVarPpTeLine;
                            if (ppTeLine.Line_Num == sl.Line_Num)
                            {
                                ppTeLine.TaxInclPrice = taxIncldAmount;
                                ppTeLine.TaxCreditAmount = taxCreditAmount;
                                break;
                            }
                        }
                    }

                    if (_policyManager.TAX_EXEMPT && !string.IsNullOrEmpty(sx.TreatyNumber))
                    {
                        if (_policyManager.TE_Type == "SITE")
                        {
                            saleAmount = float.Parse((saleQuantity * sl.price - includedTax).ToString("##0.00"));
                            lessAmount = float.Parse((sl.Amount - (decimal)sl.Line_Discount - (decimal)saleAmount).ToString("#0.00")); //  
                        }
                        else
                        {
                            float exemptAmount;
                            if (_policyManager.TE_Type == "QITE")
                            {
                                exemptAmount = float.Parse((saleQuantity * sl.Discount_Rate).ToString("##0.00"));
                                saleAmount = saleAmount - includedTax; // - ExemptAmount
                                lessAmount = float.Parse(((float)sl.Amount - (float)sl.Line_Discount - saleAmount).ToString("#0.00"));
                            }
                            else
                            {
                                exemptAmount = mPrivateGlobals.theSystem.RoundToHighCent((float)((sl.Regular_Price - sl.price) * saleQuantity));
                                saleAmount = saleAmount - exemptAmount - includedTax;
                                lessAmount = float.Parse(((float)sl.Amount - saleAmount).ToString("#0.00")); //  
                            }
                        }
                        sl.Amount = (decimal)saleAmount;
                    }
                    else
                    {

                        sl.price = unitPrice;
                        sl.Amount = (decimal)saleAmount;
                    }

                    lessQuantity = float.Parse((sl.Quantity - saleQuantity).ToString("##0.000"));

                    sl.Quantity = saleQuantity;


                    if (_policyManager.TAX_EXEMPT)
                    {


                        if (_policyManager.TE_Type == "SITE")
                        {

                            if (oPurchaseList?.Count() > 0)
                            {
                                short i;
                                for (i = 1; i <= oPurchaseList.Count(); i++)
                                {
                                    if (oPurchaseList.Item(i).GetRowInSalesMain() == sl.Line_Num)
                                    {
                                        //  added the following - If there is a sale in between finishing the prepay , system picking up wrong price and amount
                                        oPurchaseList.Item(i).SetOriginalPrice((float)sl.Regular_Price); // resettiing original price
                                        oPurchaseList.Item(i).SetTaxFreePrice((float)sl.price); // resetting Tax exempt price
                                        //shiny end
                                        oPurchaseList.Item(i).SetQuantity(sl.Quantity);




                                        if (iPositionId != orgPosition)
                                        {
                                            _purchaseItemManager.UpdateQuantityInDb(oPurchaseList.Item(i), invoiceId, sl.Line_Num, lessQuantity, sx.TreatyNumber, Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeId).Stock_Code), (float)(sl.price), (float)(sl.Regular_Price));
                                        }
                                        else
                                        {
                                            _purchaseItemManager.UpdateQuantityInDb(oPurchaseList.Item(i), invoiceId, sl.Line_Num, lessQuantity, sx.TreatyNumber);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (TaxExemptSaleLine tempLoopVarPpTeLine in ppTeSale.Te_Sale_Lines)
                            {
                                ppTeLine = tempLoopVarPpTeLine;
                                if (ppTeLine.Line_Num == sl.Line_Num)
                                {
                                    ppTeLine.ToBeUpdated = true;
                                    ppTeLine.NewQuantity = sl.Quantity;


                                    if (iPositionId != orgPosition)
                                    {

                                        ppTeLine.StockCode = Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeId).Stock_Code);
                                        ppTeLine.Description = newDescription;
                                        var productTaxExempt = _prepayService.GetProductTaxExemptForStock(ppTeLine.StockCode);
                                        if (productTaxExempt != null)
                                        {
                                            ppTeLine.ProductCode = productTaxExempt.TEVendor;
                                            ppTeLine.ProductType = (mPrivateGlobals.teProductEnum)productTaxExempt.CategoryFK.Value;
                                        }

                                        ppTeLine.TaxFreePrice = (float)sl.price;
                                        ppTeLine.OriginalPrice = (float)sl.Regular_Price;
                                        ppTeLine.StockIsChanged = true;
                                    }


                                    ppTeSale.HasUpdatedLine = true;
                                    break;
                                }
                            }
                        }
                    }

                }


                if (sl.ProductIsFuel && sl.FuelRebateEligible && sl.FuelRebate > 0 && sx.Customer.UseFuelRebate
                    && sx.Customer.UseFuelRebateDiscount)
                {
                    _saleLineManager.ApplyFuelRebate(ref sl);
                    sl.Line_Discount = double.Parse((saleQuantity * sl.Discount_Rate).ToString("#0.00"));
                }
                else
                {


                    if (sl.ProductIsFuel && !string.IsNullOrEmpty(sx.Customer.GroupID) && _policyManager.FuelLoyalty)
                    {
                        if (sx.Customer.DiscountType == "%")
                        {
                            sl.Line_Discount = double.Parse(((float)sl.Amount * sx.Customer.DiscountRate / 100).ToString("##0.00"));
                        }
                        else if (sx.Customer.DiscountType == "$")
                        {
                            sl.Line_Discount = double.Parse((sl.Quantity * sx.Customer.DiscountRate).ToString("##0.00"));
                        }
                        else if (sx.Customer.DiscountType == "D") //   added missing code for tax exempt QITE that uses discount type "D"
                        {
                            sl.Line_Discount = double.Parse((sl.Quantity * sl.Discount_Rate).ToString("##0.00"));
                        }

                    }
                    else if (sl.ProductIsFuel)
                    {
                        if (sl.Discount_Type == "%")
                        {
                            sl.Line_Discount = double.Parse((saleAmount * sl.Discount_Rate / 100).ToString("##0.00"));
                        }
                        else if (sl.Discount_Type == "D") //  : was "$"; $ discount is not based on updated quantity or amount, so it should not be recalculated
                        {
                            sl.Line_Discount = double.Parse((saleQuantity * sl.Discount_Rate).ToString("##0.00"));
                        }
                    }
                }

                var strPromo = saleLine.PromoID;
                if (!string.IsNullOrEmpty(strPromo))
                {
                    sl.PromoID = strPromo; //  
                }

                // Similarly, pick up the charges associated with the line.

                sl.Charges = _prepayService.GetSaleLineCharges(invoiceId, saleLine.Line_Num, db);

                sl.Line_Kits = _prepayService.Get_Line_Kit(db, invoiceId, saleLine.Line_Num);

                _saleManager.Add_a_Line(ref sx, sl, sl.User, sx.TillNumber, out error, false, false);
            }


            if (_policyManager.TAX_EXEMPT && !string.IsNullOrEmpty(sx.TreatyNumber) && (_policyManager.TE_Type == "AITE" || _policyManager.TE_Type == "QITE")) //  
            {
                ppTeSale.TaxCredit = new Sale_Taxes();
                foreach (Sale_Tax tempLoopVarSt in salTmp.Sale_Totals.Sale_Taxes)
                {
                    var st = tempLoopVarSt;
                    if (st.Taxable_Amount != 0 | st.Tax_Included_Amount != 0)
                    {
                        ppTeSale.TaxCredit.Add(st.Tax_Name, st.Tax_Code, st.Tax_Rate, st.Taxable_Amount, st.Tax_Added_Amount, st.Tax_Included_Amount, st.Tax_Included_Total, st.Tax_Rebate_Rate, st.Tax_Rebate, st.Tax_Name + st.Tax_Code); //   - gave mismatch type error for AITE
                    }
                }
            }



            if (_policyManager.TAX_EXEMPT)
            {


                if (_policyManager.TE_Type == "SITE")
                {

                    //
                    if (oPurchaseList?.Count() > 0)
                    {
                        var user = CacheManager.GetUser(UserCode);
                        var till = _tillService.GetTill(tillNumber);


                        _purchaseListManager.SaveAndAssignToQuotas(ref oPurchaseList,
                            user, till, (false));

                    }


                }
                else
                {

                    _taxExemptManager.UpdateSale(ref ppTeSale);

                    // Chaps_Main.oTeSale = ppTeSale; 


                    sx.TotalTaxSaved = ppTeSale.TotalExemptedTax;

                    taxExemptReport = _receiptManager.PrintTaxExemptVoucher(ppTeSale);
                }

            }



            if (overPayment)
            {


                if (_policyManager.FuelLoyalty && sx.Customer.GroupID != "")
                {
                    //  - if partial prpay and site
                    if (_policyManager.TE_Type == "SITE" && _policyManager.TAX_EXEMPT && sl.IsTaxExemptItem)
                    {
                        if (sx.Customer.DiscountType == "%")
                        {
                            sx.OverPayment = decimal.Parse((lessAmount + sl.Line_Discount).ToString("##0.00"));
                        }
                        else if (sx.Customer.DiscountType == "$")
                        {
                            sx.OverPayment = decimal.Parse((lessAmount + (sl.Quantity * sx.Customer.DiscountRate)).ToString("##0.00"));
                        }
                        else
                        {
                            sx.OverPayment = (decimal)lessAmount;
                        }
                    }
                    else
                    {
                        if (sx.Customer.DiscountType == "%")
                        {
                            sx.OverPayment = decimal.Parse((lessAmount * (1 - sx.Customer.DiscountRate / 100)).ToString("##0.00"));
                        }
                        else if (sx.Customer.DiscountType == "$")
                        {
                            sx.OverPayment = decimal.Parse((lessAmount - lessQuantity * sx.Customer.DiscountRate).ToString("##0.00"));
                        }
                        else if (sx.Customer.DiscountType == "D")
                        {
                            sx.OverPayment = decimal.Parse((lessAmount + sl.Line_Discount).ToString("##0.00"));
                        }
                        else
                        {
                            sx.OverPayment = (decimal)lessAmount;
                        }
                    }
                }
                else
                {

                    foreach (Sale_Line tempLoopVarSl in sx.Sale_Lines)
                    {
                        sl = tempLoopVarSl;
                        if (sl.Prepay)
                        {
                            break;
                        }
                    }
                    if (sl.Discount_Type == "%")
                    {
                        sx.OverPayment = decimal.Parse((lessAmount * (1 - sl.Discount_Rate / 100)).ToString("##0.00"));
                    }
                    else if (sl.Discount_Type == "$")
                    {
                        sx.OverPayment = decimal.Parse((lessAmount - lessQuantity * sl.Discount_Rate).ToString("##0.00"));
                    }
                    else
                    {

                        sx.OverPayment = (decimal)lessAmount;
                    }
                }


                change = 0; //  when there is original chnage and then do an overpayment, system doubling the change and recording the correct overpayment ''rsHead!Change  ''equal to the original change value
            }
            else
            {
                sx.OverPayment = 0;



                if (_policyManager.FuelLoyalty && !string.IsNullOrEmpty(sx.Customer.GroupID))
                {

                    if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type == "SITE" && sl.IsTaxExemptItem)
                    {
                        if (sx.Customer.DiscountType == "%")
                        {
                            //                T_Change = Round(rsHead![Change] - (LessAmount + SL.Line_Discount), 2) '  added discount adj
                            change = Math.Round(-(lessAmount + sl.Line_Discount), 2); //  added discount adj

                        }
                        else if (sx.Customer.DiscountType == "$")
                        {
                            //                T_Change = Round(rsHead![Change] - (LessAmount + (SL.Quantity * SX.Customer.DiscountRate)), 2)
                            change = Math.Round(-(lessAmount + sl.Quantity * sx.Customer.DiscountRate), 2);

                        }
                        else
                        {
                            //                T_Change = Round(rsHead![Change] - LessAmount, 2)
                            change = Math.Round(Convert.ToDouble(-lessAmount), 2);
                        }
                        // 
                    }
                    else
                    {
                        if (sx.Customer.DiscountType == "%")
                        {
                            //                T_Change = Round(rsHead![Change] - LessAmount * (1 - SX.Customer.DiscountRate / 100), 2)
                            change = Math.Round(-lessAmount * (1 - sx.Customer.DiscountRate / 100), 2);
                        }
                        else if (sx.Customer.DiscountType == "$")
                        {
                            change = Math.Round(-(lessAmount - lessQuantity * sx.Customer.DiscountRate), 2);

                        }
                        else if (sx.Customer.DiscountType == "D")
                        {
                            change = Math.Round(-(lessAmount + sl.Line_Discount), 2);
                        }
                        else
                        {
                            //                T_Change = Round(rsHead![Change] - LessAmount, 2)
                            change = Math.Round(Convert.ToDouble(-lessAmount), 2);
                        }

                    }

                }
                else
                {

                    foreach (Sale_Line tempLoopVarSl in sx.Sale_Lines)
                    {
                        sl = tempLoopVarSl;
                        if (sl.Prepay)
                        {
                            break;
                        }
                    }
                    if (sl.Discount_Type == "%")
                    {
                        //            T_Change = Round(rsHead![Change] - LessAmount * (1 - SL.Discount_Rate / 100), 2)
                        change = Math.Round(-lessAmount * (1 - sl.Discount_Rate / 100), 2);
                    }
                    else if (sl.Discount_Type == "D") //   "$" discount does not require recalculation, it is not based on amount or quantity, so it should not affect the total
                    {
                        //            T_Change = Round(rsHead![Change] - (LessAmount - LessQuantity * SL.Discount_Rate), 2)
                        change = Math.Round(-(lessAmount - lessQuantity * sl.Discount_Rate), 2);
                    }
                    else
                    {

                        //            T_Change = Round(rsHead![Change] - LessAmount, 2)
                        change = Math.Round(Convert.ToDouble(-lessAmount), 2);
                    }
                }


            }
            //  - To adjust the switch prepay with Tax exempt and fuel loyalty
            if (fullSwitch) // 
            {
                sp.Discount_Adjust = sp.Discount_Adjust + change;
                change = 0;
            }
            //shiny end

            if (_policyManager.FuelLoyalty && !string.IsNullOrEmpty(sx.Customer.GroupID))
            {
                if (sx.Customer.DiscountType == "%")
                {
                    var discountTender = new DiscountTender
                    {
                        SaleAmount = Convert.ToDecimal((saleAmount * (1 - sx.Customer.DiscountRate / 100)).ToString("##0.00")),
                        DiscountAmount = Convert.ToDecimal((saleAmount * sx.Customer.DiscountRate / 100).ToString("##0.00"))
                    };
                    _prepayService.UpdateDiscountTender(invoiceId, tillNumber, db, discountTender);
                }
                else if (sx.Customer.DiscountType == "$")
                {
                    var discountTender = new DiscountTender
                    {
                        SaleAmount = Convert.ToDecimal((saleAmount - (saleQuantity * sx.Customer.DiscountRate)).ToString("##0.00")),
                        DiscountAmount = Convert.ToDecimal((saleQuantity * sx.Customer.DiscountRate).ToString("##0.00"))
                    };
                    _prepayService.UpdateDiscountTender(invoiceId, tillNumber, db, discountTender);
                }
                else if (sx.Customer.DiscountType == "C")
                {


                    if (!overPayment)
                    {
                        var discountTender = new DiscountTender
                        {
                            SaleAmount = Convert.ToDecimal(saleAmount),
                            DiscountAmount = Convert.ToDecimal((saleQuantity * sx.Customer.DiscountRate).ToString("##0.00"))
                        };
                        _prepayService.UpdateDiscountTender(invoiceId, tillNumber, db, discountTender);
                        _prepayService.UpdateCoupon(sx.CouponID, (saleQuantity * sx.Customer.DiscountRate).ToString("##0.00"));
                    }
                }
            }


            sx.Sale_Totals.Invoice_Discount_Type = saleHead.DiscountType;
            sx.Sale_Totals.Invoice_Discount = saleHead.INVCDiscount;
            _saleManager.ReCompute_Totals(ref sx);

            // change
            // This section repeats the code in the class Tend_Totals class (Tend_Used Let) due to the incorrect design and codding of UpdatePrepaySale function
            if (_policyManager.PENNY_ADJ && change != 0)
            {
                pennyAdj = modGlobalFunctions.Calculate_Penny_Adj((decimal)change);
                change = change + (double)pennyAdj;
                sx.Sale_Totals.Penny_Adj = pennyAdj;
            }
            else
            {
                sx.Sale_Totals.Penny_Adj = 0;
            }
            //  


            _prepayService.UpdateSaleData(sx, sl, pennyAdj, change, saleQuantity, _policyManager.TAX_EXEMPT,
                saleAmount, unitPrice, iPositionId, orgPosition, iGradeId, _policyManager.FuelLoyalty, newTotalAmount,
                fullSwitch, db);



            if (goToTender)
            {
                var tenders = new Tenders();

                var saleTenders = _saleService.LoadTendersInSale(invoiceId, db);
                // Load only those tenders that were actually used in the sale. Just dummy out
                // the attributes of the tenders since we don't need the correct values to
                // print a receipt.
                foreach (var saleTend in saleTenders)
                {
                    if (_policyManager.COMBINEFLEET && saleTend.TenderClass == "FLEET" || _policyManager.COMBINECR && (saleTend.TenderClass == "CRCARD"))
                    {
                        tenders.Add(saleTend.ClassDescription, saleTend.TenderClass,
                            Convert.ToDouble(saleTend.Exchange), true, true, true, 1,
                            "A", false, 99, 99, 99, true, Convert.ToDouble(saleTend.AmountTend),
                            1, true, false, "",
                            saleTend.TenderName + saleTend.SequenceNumber);
                    }
                    else
                    {
                        tenders.Add(saleTend.TenderName, saleTend.TenderClass,
                            Convert.ToDouble(saleTend.Exchange), true, true, true, 1,
                            "A", false, 99, 99, 99, true, Convert.ToDouble(saleTend.AmountTend),
                            1, true, false, "",
                            saleTend.TenderName + saleTend.SequenceNumber);

                    }
                    tenders[saleTend.TenderName + saleTend.SequenceNumber].Amount_Entered = saleTend.AmountTend.Value;

                    tenders[saleTend.TenderName + saleTend.SequenceNumber].Amount_Used = saleTend.AmountUsed.Value;



                    tenders.Tend_Totals.Tend_Used = tenders.Tend_Totals.Tend_Used + saleTend.AmountUsed.Value;


                    if (Strings.Trim(saleTend.TenderClass).ToUpper() == "ACCOUNT")
                    {
                        if (saleTend.SerialNumber != "")
                        {
                            sx.AR_PO = saleTend.SerialNumber;
                        }
                    }


                    if (Strings.Trim(saleTend.CCardNumber) != "")
                    {
                        var cardTender = _saleService.GetCardTender(saleTend.SaleNumber, saleTend.TillNumber, saleTend.TenderName, db);
                        if (cardTender != null)
                        {

                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Name = cardTender.Name;
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Crd_Type = cardTender.Crd_Type;
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Cardnumber = _encryptDecryptManager.Decrypt(cardTender.Cardnumber);
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Card_Swiped = cardTender.Card_Swiped;
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Authorization_Number = cardTender.Authorization_Number;
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Language = cardTender.Language;
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Expiry_Date = cardTender.Expiry_Date;
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Customer_Name = cardTender.Customer_Name;
                            tenders[saleTend.TenderName + saleTend.SequenceNumber].Credit_Card.Result = cardTender.Result;
                        }

                    }
                }

                tenders.Tend_Totals.Change = Convert.ToDecimal(change);
            }
            // 



            Chaps_Main.Transaction_Type = "Complete Prepay";
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            changeDue = string.Format(Utilities.Constants.ChangeDue, Helper.Round(Convert.ToDouble(-change), 2), _resourceManager.GetResString(offSet, 166));

            //  only we need to open the cash drawer, when finishing  partial prepay with a change
            if (!overPayment && goToTender)
            {
                if (_policyManager.OPEN_DRAWER == "Every Sale")
                {
                    openDrawer = true;
                }
            }
            return returnValue;
        }

        #region Private methods

        /// <summary>
        /// Method to hold deleting prepay
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        private bool HoldDeletingPrepay(short pumpId, out ErrorMessage error)
        {
            error = new ErrorMessage();

            if (!TCPAgent.Instance.IsConnected)
            {
                //        MsgBox ("Communication problem, Cannot delete prepay!~Delete Prepay Error!")
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)19, temp_VbStyle, null, (byte)0);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 25, 19, null, CriticalOkMessageType)
                };
                return false;
            }

            var response = "";
            var strRemain = "";

            string tempCommandRenamed = "HRP" + Strings.Right("0" + Convert.ToString(pumpId), 2);
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

            var timeIN = (float)DateAndTime.Timer;
            while (!(DateAndTime.Timer - timeIN > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting HRP" + Strings.Right("00" + Convert.ToString(pumpId), 2));
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "HRP" + Strings.Right("00" + Convert.ToString(pumpId), 2), ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {

                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify PortReading from Hold Deleting prepay: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIN)
                {
                    timeIN = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            if (Strings.Left(response, 8) == "HRP" + Strings.Right("00" + Convert.ToString(pumpId), 2) + "ERR")
            {
                //IsWaiting = false;
                return false;
            }

            if (Strings.Left(response, 7) != "HRP" + Strings.Right("00" + Convert.ToString(pumpId), 2) + "OK") //response is not HRPERR or HRPOK
            {

                string tempCommandRenamed2 = "ENDPOS";
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed2, true);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to reload tax exempt site
        /// </summary>
        /// <param name="saleNo"></param>
        /// <param name="tillId"></param>
        /// <param name="db"></param>
        private void ReloadTaxexempt_SITE(int saleNo, short tillId, DataSource db)
        {
            var purchaseItems = _prepayService.GetPurchaseItems(db, saleNo);
            var oPurchaseList = new tePurchaseList();
            oPurchaseList.NoRTVP = true; //  
            if (purchaseItems.Count != 0)
            {
                var oTreatyNo = new teTreatyNo();

                _treatyManager.Init(ref oTreatyNo, purchaseItems.FirstOrDefault().TreatyNo, false);
                oPurchaseList.Init(oTreatyNo, saleNo, tillId);
                foreach (var purchaseItem in purchaseItems)
                {

                    var tierId = purchaseItem.PsTierID;

                    var levelId = purchaseItem.PsLevelID;

                    var gradeId = purchaseItem.PsGradeIDpsTreatyNo;

                    double orgPrice = purchaseItem.PdOriginalPrice;

                    var taxFreePrice = purchaseItem.TaxFreePrice;

                    var taxIncldPrice = double.Parse((-1 * Convert.ToInt32(taxFreePrice + purchaseItem.pdTaxFreePrice)).ToString("#0.00"));
                    if (gradeId > 0)
                    {
                        if (_policyManager.USE_FUEL)
                        {
                            string tempSProductKey = mPrivateGlobals.theSystem.teMakeFuelKey(gradeId, tierId, levelId);
                            double tempDQuantity = purchaseItem.Quantity;
                            short tempIRowNumberInSalesMainForm = purchaseItem.LineItem;
                            string tempStockcode = purchaseItem.stockcode;
                            bool tempIsFuelItem = true;
                            var tempSale = new Sale { Sale_Num = saleNo, TillNumber = (byte)tillId };
                            _purchaseListManager.AddItem(ref oPurchaseList, ref tempSale, ref oTreatyNo, ref tempSProductKey, ref tempDQuantity, ref orgPrice, ref tempIRowNumberInSalesMainForm, ref tempStockcode, ref taxIncldPrice, ref tempIsFuelItem);

                        }
                        else
                        {
                            string tempSProductKey2 = purchaseItem.stockcode;
                            double tempDQuantity2 = purchaseItem.Quantity;
                            short tempIRowNumberInSalesMainForm2 = purchaseItem.LineItem;
                            string tempStockcode2 = purchaseItem.stockcode;
                            bool tempIsFuelItem2 = false;
                            var tempSale = new Sale { Sale_Num = saleNo, TillNumber = (byte)tillId };
                            _purchaseListManager.AddItem(ref oPurchaseList, ref tempSale, ref oTreatyNo, ref tempSProductKey2, ref tempDQuantity2, ref orgPrice, ref tempIRowNumberInSalesMainForm2, ref tempStockcode2, ref taxIncldPrice, ref tempIsFuelItem2);
                        }
                    }
                    else
                    {
                        string tempSProductKey3 = purchaseItem.stockcode;
                        double tempDQuantity3 = purchaseItem.Quantity;
                        short tempIRowNumberInSalesMainForm3 = purchaseItem.LineItem;
                        string tempStockcode3 = purchaseItem.stockcode;
                        bool tempIsFuelItem3 = false;
                        var tempSale = new Sale { Sale_Num = saleNo, TillNumber = (byte)tillId };
                        _purchaseListManager.AddItem(ref oPurchaseList, ref tempSale, ref oTreatyNo, ref tempSProductKey3, ref tempDQuantity3, ref orgPrice, ref tempIRowNumberInSalesMainForm3, ref tempStockcode3, ref taxIncldPrice, ref tempIsFuelItem3);
                    }
                }
            }
            oPurchaseList.NoRTVP = false; //  
        }

        /// <summary>
        /// Method to calculate tax
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sl"></param>
        /// <param name="saleAmount"></param>
        /// <param name="saleQuantity"></param>
        /// <param name="salTmp"></param>
        /// <param name="slTmp"></param>
        private void CalculateTax(Sale sx, Sale_Line sl, float saleAmount, float saleQuantity,
            ref Sale salTmp, ref Sale_Line slTmp)
        {
            salTmp = new Sale { Customer = { Code = sx.Customer.Code } };
            salTmp.Customer = _customerManager.LoadCustomer(salTmp.Customer.Code);

            slTmp = new Sale_Line
            {
                No_Loading = true,
                Dept = sl.Dept,
                Sub_Dept = sl.Sub_Dept,
                User = sl.User,
                Stock_Code = sl.Stock_Code
            };
            //SlTmp.Sub_Detail = SL.Sub_Detail;
            _saleLineManager.SetSubDetail(ref slTmp, sl.Sub_Detail);
            slTmp.PLU_Code = sl.PLU_Code;
            ErrorMessage error;
            //_saleLineManager.SetStockCode(ref SalTmp, ref SlTmp, SlTmp.Stock_Code, SlTmp.User, out error);
            //_saleLineManager.SetPluCode(ref SalTmp, ref SlTmp, SL.PLU_Code, out error);
            slTmp.Line_Num = sl.Line_Num;
            slTmp.Price_Type = sl.Price_Type;
            slTmp.Quantity = saleQuantity;
            slTmp.price = sl.price;
            slTmp.Regular_Price = sl.Regular_Price;
            slTmp.Amount = (decimal)saleAmount;
            _saleManager.Add_a_Line(ref salTmp, slTmp, slTmp.User, salTmp.TillNumber, out error, false, false);

        }
        #endregion

    }
}
