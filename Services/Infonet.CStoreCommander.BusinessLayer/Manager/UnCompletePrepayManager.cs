using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class UnCompletePrepayManager : ManagerBase, IUnCompletePrepayManager
    {
        private readonly ISaleLineManager _saleLineManager;
        private readonly IPolicyManager _policyManager;
        private readonly ISaleManager _saleManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleService _saleService;
        private readonly IPrepayManager _prepayManager;
        private readonly IFuelPumpService _fuelPumpService;
        private readonly ITillService _tillService;
        private readonly IFuelPrepayManager _fuelPrepayManager;

        public UnCompletePrepayManager(
            ISaleLineManager saleLineManager,
            IPolicyManager policyManager,
            ISaleManager saleManager,
            IApiResourceManager resourceManager,
            ISaleService saleService,
            IPrepayManager prepayManager,
            IFuelPumpService fuelPumpService,
            ITillService tillService,
            IFuelPrepayManager fuelPrepayManager)
        {
            _saleLineManager = saleLineManager;
            _policyManager = policyManager;
            _saleManager = saleManager;
            _resourceManager = resourceManager;
            _saleService = saleService;
            _prepayManager = prepayManager;
            _fuelPumpService = fuelPumpService;
            _tillService = tillService;
            _fuelPrepayManager = fuelPrepayManager;
        }


        /// <summary>
        /// Method to delete uncomplete prepay
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <returns>Sale</returns>
        public Sale DeleteUnCompletePrepay(int pumpId, int saleNumber, int tillNumber, out ErrorMessage error)
        {
            var sale = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (sale?.Sale_Lines?.Count > 0)
            {
                
                //Chaps_Main.DisplayMessage(this, (short)60, temp_VbStyle, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,36, 60, null, CriticalOkMessageType)
                };
                return null;
            }

            sale = _saleService.GetSaleBySaleNoFromDbTill(ref sale, tillNumber, saleNumber);
            var saleLine = _saleService.GetPrepaySaleLine(saleNumber, tillNumber);

            if (sale == null || saleLine == null)
            {
                
                //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)61, temp_VbStyle2, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,36, 61, null, CriticalOkMessageType)
                };
                return null;
            }
            _saleService.UpdateDiscountTender(ref sale);



            var sl = new Sale_Line
            {
                No_Loading = true,
                Dept = saleLine.Dept,
                Sub_Dept = saleLine.Sub_Dept,
                Stock_Code = saleLine.Stock_Code,
                Amount = saleLine.Amount
            };

            _saleLineManager.SetSubDetail(ref sl, saleLine.Sub_Detail);
            _saleLineManager.SetPluCode(ref sale, ref sl, saleLine.PLU_Code, out error);
            _saleLineManager.SetPrice(ref sl, saleLine.price);
            _saleLineManager.SetQuantity(ref sl, saleLine.Quantity * -1);
            sl.Regular_Price = saleLine.Regular_Price;


            if (sale.Customer.GroupID != "" && _policyManager.FuelLoyalty)
            {
                if (sale.Customer.DiscountType == "%")
                {
                    _saleLineManager.SetAmount(ref sl, (saleLine.Amount - Convert.ToDecimal(saleLine.Amount * Convert.ToDecimal(sale.Customer.DiscountRate)) / 100) * (-1));
                }
                else if (sale.Customer.DiscountType == "$")
                {
                    _saleLineManager.SetAmount(ref sl, saleLine.Amount - Convert.ToDecimal(saleLine.Quantity * sale.Customer.DiscountRate) * (-1));
                }
                else
                {
                    _saleLineManager.SetAmount(ref sl, saleLine.Amount * (-1));
                }
            }
            else
            {
                _saleLineManager.SetAmount(ref sl, saleLine.Amount * (-1));
            }

            sl.pumpID = (byte)pumpId;
            sl.PositionID = saleLine.PositionID;
            sl.GradeID = saleLine.GradeID;

            sl.User = UserCode;
            sl.Description = saleLine.Description;
            sale.Sale_Type = "Delete Prepay";

            _saleManager.Add_a_Line(ref sale, sl, "", tillNumber, out error, true);

            return sale;
        }

        /// <summary>
        /// Method to finish overpayment uncomplete prepay
        /// </summary>
        /// <param name="ipumpId">Pump Id</param>
        /// <param name="lngSaleNum">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="finishAmount">Finish amount</param>
        /// <param name="finishQty">Finish quantity</param>
        /// <param name="finishPrice">Finish price</param>
        /// <param name="prepayAmount">Prepay amount</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="openDrawer">Open drawer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax exmept report</returns>
        public Report OverpaymentUncompletePrepay(int ipumpId, int lngSaleNum, int tillNumber, float finishAmount,
            float finishQty, float finishPrice, float prepayAmount, short iPositionId, short iGradeId,
             out bool openDrawer, out ErrorMessage error)
        {
            error = new ErrorMessage();
            openDrawer = false;
            if (ipumpId == 0)
            {
                return null; //nothing selected
            }

            if (_fuelPrepayManager.RemovePrepayBasket("P" + Strings.Right("00" + Convert.ToString(ipumpId), 2), (lngSaleNum).ToString().Trim()))
            {
                string changeDue;
                Report fs;
                _fuelPrepayManager.UpdatePrepaySale(lngSaleNum, finishAmount, finishQty, finishPrice,
                    (float)(modGlobalFunctions.Round(prepayAmount - finishAmount, 2)), true,
                    _policyManager.PRT_OVERPAY, iPositionId, iGradeId, tillNumber,
                    out changeDue, out openDrawer, out fs);
                return fs;
            }
            // Added to display the message here.
            
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage
            {
                MessageStyle = _resourceManager.CreateMessage(offSet,36, 61, null, CriticalOkMessageType)
            };
            return null;
        }

        /// <summary>
        /// Method to change uncomplete prepay
        /// </summary>
        /// <param name="ipumpId">Pump Id</param>
        /// <param name="lngSaleNum">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="finishAmount">Finish amount</param>
        /// <param name="finishQty">Finish quantity</param>
        /// <param name="finishPrice">Finish price</param>
        /// <param name="prepayAmount">Prepay amount</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="openDrawer">Open drawer</param>
        /// <param name="error">Error message</param>
        /// <returns>Tax exmept voucher</returns>
        public Report ChangeUncompletePrepay(int ipumpId, int lngSaleNum, int tillNumber, float finishAmount,
            float finishQty, float finishPrice, float prepayAmount, short iPositionId, short iGradeId, out string changeDue,
             out bool openDrawer, out ErrorMessage error)
        {
            error = new ErrorMessage();
            changeDue = "0.00";
            openDrawer = false;
            if (ipumpId == 0)
            {
                return null;
            }

            if (_fuelPrepayManager.RemovePrepayBasket("P" + Strings.Right("00" + Convert.ToString(ipumpId), 2), (lngSaleNum).ToString().Trim()))
            {
                Report fs;
                _fuelPrepayManager.UpdatePrepaySale(lngSaleNum, finishAmount, finishQty, finishPrice,
                    (float)(modGlobalFunctions.Round(prepayAmount - finishAmount, 2)), false, true, iPositionId,
                    iGradeId, tillNumber, out changeDue, out openDrawer, out fs);
                return fs;
            }
            // Added to display the message here.
            
            //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Information + MsgBoxStyle.OkOnly;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage
            {
                MessageStyle = _resourceManager.CreateMessage(offSet,36, 62, null, CriticalOkMessageType)
            };
            return null;
        }


        /// <summary>
        /// Method to load uncomplete grid
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <returns>Uncomplete prepay response</returns>
        public UnCompletePrepayResponse LoadUncompleteGrid(int tillNumber, out ErrorMessage error)
        {
            var result = new UnCompletePrepayResponse();
            error = new ErrorMessage();
            string[] prepayBasket = null;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (TCPAgent.Instance.IsConnected)
            {
                result.UnCompleteSale = new List<UnCompleteSale>();
                result.Caption = _resourceManager.GetResString(offSet,Convert.ToInt16(36 * 100 + 4));
                
                if (!GetPrepayBasket(ref prepayBasket))
                {
                    result.IsChangeEnabled = false;
                    result.IsOverPaymentEnabled = false;
                    return result;
                }

                short i;
                for (i = 0; i < prepayBasket.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(prepayBasket[i]))
                    {
                        var uncompleteSale = LoadUncompletePrepay(prepayBasket[i], tillNumber, out error);
                        if (uncompleteSale != null)
                        {
                            result.UnCompleteSale.Add(uncompleteSale);
                        }
                    }
                }
                if (result.UnCompleteSale.Count > 0)
                {
                    result.IsChangeEnabled = true;
                    result.IsOverPaymentEnabled = true;
                }
                result.IsDeleteVisible = false;
            }
            else 
            {
                result.UnCompleteSale = LoadMyUnlockedPrepay(tillNumber, out error);
                //Click on the  sale to select it , then click Delete button.
                result.Caption = _resourceManager.GetResString(offSet,Convert.ToInt16(36 * 100 + 5));

                result.IsDeleteVisible = true;
                result.IsDeleteEnabled = result.UnCompleteSale?.Count != 1;
                result.IsChangeEnabled = false;
                result.IsOverPaymentEnabled = false;
            }

            return result;
        }

        /// <summary>
        /// Method to load uncomplete prepay
        /// </summary>
        /// <param name="strBasket">Basket</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <returns>Uncomplete sale</returns>
        private UnCompleteSale LoadUncompletePrepay(string strBasket, int tillNumber, out ErrorMessage error)
        {
            UnCompleteSale unCompleteSale = new UnCompleteSale();
            error = new ErrorMessage();

            float unitPrice = 0;

            var invoiceId = _prepayManager.IsMyPrepayBasket(strBasket + ";", tillNumber);
            
            if (invoiceId == 0)
            {
                return null;
            }

            var strFinish = strBasket.Substring(4);
            var pumpId = (short)(Conversion.Val(strBasket.Substring(4, 2)));
            var mop = (byte)(Conversion.Val(strFinish.Substring(2, 1)));
            var positionId = (short)(Conversion.Val(strFinish.Substring(3, 1)));
            var volume = (float)(Conversion.Val(strFinish.Substring(12, 8)) / 1000);
            var amount = (float)(Conversion.Val(strFinish.Substring(4, 8)) / 1000);

            if (pumpId == 0 | positionId == 0)
            {
                return null;
            }

            var gradeId = Variables.gPumpPositionGrade[pumpId, positionId];
            if (mop == 2) //credit price in effect
            {
                unitPrice = Convert.ToSingle(Variables.Pump[pumpId].creditUP[positionId]);
            }
            else if (mop == 1) //cash pricing
            {
                if (Variables.Pump[pumpId].cashUP != null)
                {
                    unitPrice = Convert.ToSingle(Variables.Pump[pumpId].cashUP[positionId]);
                }
            }
            if ((unitPrice * volume).ToString("##0.00") != (amount.ToString("##0.00")))
            {
                unitPrice = (float)(modGlobalFunctions.Round(Conversion.Val(strFinish.Substring(4, 8)) / Conversion.Val(strFinish.Substring(12, 8)), 3));
            }

            var sale = new Sale();
            sale = _saleService.GetSaleBySaleNoFromDbTill(ref sale, tillNumber, invoiceId);
            var saleLine = _saleService.GetPrepaySaleLine(invoiceId, tillNumber);

            if (sale == null || saleLine == null)
            {
                return null;
            }


            unCompleteSale.PumpId = pumpId;
            unCompleteSale.PositionId = positionId;
            unCompleteSale.SaleNumber = invoiceId;

            
            
            if (saleLine.TaxForTaxExempt)
            {
                unCompleteSale.PrepayAmount = Helper.Round(Convert.ToDouble(saleLine.Quantity * saleLine.Regular_Price),2);
            }
            else
            {
                unCompleteSale.PrepayAmount = Helper.Round(Convert.ToDouble(saleLine.Amount),2);
            }

            unCompleteSale.PrepayVolume = saleLine.Quantity;
            unCompleteSale.UsedAmount = amount;
            unCompleteSale.UsedVolume = volume;
            unCompleteSale.Grade = gradeId;
            unCompleteSale.UnitPrice = unitPrice;

            unCompleteSale.SalePosition = saleLine.PositionID;
            unCompleteSale.SaleGrade = saleLine.GradeID;
            unCompleteSale.RegPrice = saleLine.Regular_Price;
            return unCompleteSale;
        }

        /// <summary>
        /// Method to check if prepay basket
        /// </summary>
        /// <param name="prepayBasket">Prepat baskets</param>
        /// <returns></returns>
        private bool GetPrepayBasket(ref string[] prepayBasket)
        {
            bool returnValue = false;
            if (string.IsNullOrEmpty(Variables.MyPrepayBaskets))
            {
                return false;
            }

            prepayBasket = Strings.Split(Variables.MyPrepayBaskets, ";", -1, CompareMethod.Text);

            if ((prepayBasket.ToString().Length - 1) > 0)
            {
                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Method to load unlocked prepay
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <returns>Uncomplete sales</returns>
        private List<UnCompleteSale> LoadMyUnlockedPrepay(int tillNumber, out ErrorMessage error)
        {
            List<UnCompleteSale> result = new List<UnCompleteSale>();
            error = new ErrorMessage();
            var till = _tillService.GetTill(tillNumber);

            if (till == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Ivalid Request",
                        MessageType = OkMessageType
                    }
                };
                return null;
            }
            var prepays = _fuelPumpService.LoadMyUnlockedPrepay(tillNumber);
            float volume = 0;
            short gradeId = 0;
            float unitPrice = 0;
            
            foreach (UnCompleteSale unCompleteSale in prepays)
            {
                var prepay = unCompleteSale;
                if (prepay.PositionId > 0)
                {
                    gradeId = Variables.gPumpPositionGrade[prepay.PumpId, prepay.PositionId];
                    if (prepay.Mop == 2) //credit price in effect
                    {
                        unitPrice = Convert.ToSingle(Variables.Pump[prepay.PumpId].creditUP[prepay.PositionId]);
                    }
                    else if (prepay.Mop == 1) //cash pricing
                    {
                        unitPrice = Convert.ToSingle(Variables.Pump[prepay.PumpId].cashUP[prepay.PositionId]);
                    }
                    if (unitPrice > 0)
                    {
                        volume = float.Parse((modGlobalFunctions.Round(Convert.ToDouble(Convert.ToDecimal(prepay.PrepayAmount) / Convert.ToDecimal(unitPrice)), 3)).ToString("##0.000"));
                    }
                }
                prepay.PrepayVolume = volume;
                prepay.UsedAmount = 0;
                prepay.UsedVolume = 0;
                prepay.Grade = gradeId;
                prepay.UnitPrice = unitPrice;

                result.Add(prepay);
            }
            return result;
        }

    }
}
