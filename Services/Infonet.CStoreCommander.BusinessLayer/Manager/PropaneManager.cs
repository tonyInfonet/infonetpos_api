using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PropaneManager : ManagerBase, IPropaneManager
    {
        private readonly IFuelPumpService _fuelPumpService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ISaleManager _saleManager;
        private readonly IPolicyManager _policyManager;
        private const double MaxAmt = 999.99;
        private const double MaxVolumn = 999.999;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fuelPumpService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="policyManager"></param>
        public PropaneManager(IFuelPumpService fuelPumpService,
            IApiResourceManager resourceManager,
            ISaleLineManager saleLineManager,
            ISaleManager saleManager,
            IPolicyManager policyManager)
        {
            _fuelPumpService = fuelPumpService;
            _resourceManager = resourceManager;
            _saleLineManager = saleLineManager;
            _saleManager = saleManager;
            _policyManager = policyManager;
        }

        /// <summary>
        /// Method to get propane grades
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        public List<PropaneGrade> GetPropaneGrades(out ErrorMessage error)
        {
            var propaneGrades = _fuelPumpService.GetPropaneGrades();
            error = new ErrorMessage();
            if (propaneGrades.Count == 0)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 76, null)
                };
                return null;
            }

            CacheManager.AddPropaneGrades(propaneGrades);
            return propaneGrades;
        }

        /// <summary>
        /// Method to get propane pumps by grade Id
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="error">Error message</param>
        /// <returns>Propane pumps</returns>
        public List<PropanePump> GetPropanePumpsByGradeId(int gradeId, out ErrorMessage error)
        {
            var grades = CacheManager.GetPropaneGrades();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (grades == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 76, null)
                };
                return null;
            }
            var firstOrdDefault = grades.FirstOrDefault(p => p.Id == gradeId);
            if (firstOrdDefault == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Request",
                        MessageType = MessageType.Critical
                    }
                };
                return null;
            }

            var stockCode = firstOrdDefault.StockCode;

            var pumps = _fuelPumpService.GetPumpsByPropaneGradeId(gradeId);
            error = new ErrorMessage();
            if (pumps.Count == 0)
            {
                // "There's no Pump set for " & Stock_Code(index) & ". Please set it in Fuel Control at first.", vbinformation+vbokonly
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 12, stockCode, InformationOkMessageType)
                };
                return null;
            }

            foreach (PropanePump propanePump in pumps)
            {
                propanePump.Name = _resourceManager.GetResString(offSet, (short)323) + " " + propanePump.Id;
            }
            return pumps;
        }

        /// <summary>
        /// Method to add propane sale
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="isAmount">Is amount</param>
        /// <param name="propaneValue">Propane value</param>
        /// <param name="error">Error message</param>
        /// <returns>Sale</returns>
        public Sale AddPropaneSale(int gradeId, int pumpId, int saleNumber, int tillNumber, byte registerNumber,
            bool isAmount, decimal propaneValue, out ErrorMessage error)
        {
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, UserCode, out error);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }

            var grades = CacheManager.GetPropaneGrades();
            if (grades == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 76, null)
                };
                return null;
            }
            var firstOrdDefault = grades.FirstOrDefault(p => p.Id == gradeId);

            if (firstOrdDefault == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Request",
                        MessageType = MessageType.Critical
                    }
                };
                return null;
            }
            var pumps = _fuelPumpService.GetPumpsByPropaneGradeId(gradeId);
            if (pumps == null || !pumps.Any(p => p.Id == pumpId))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Request",
                        MessageType = MessageType.Critical
                    }
                };
                return null;
            }

            var stockCode = firstOrdDefault.StockCode;


            if (sale.DeletePrepay)
            {
                // "There's no Pump set for " & Stock_Code(index) & ". Please set it in Fuel Control at first.", vbinformation+vbokonly
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1150, null, InformationOkMessageType)
                };
                return null;
            }

            var slProp = new Sale_Line();

            _saleLineManager.SetPluCode(ref sale, ref slProp, stockCode, out error);
            slProp.GradeID = (byte)gradeId;
            if (slProp.Regular_Price == 0)
            {
                // "There's no price for " & Stock_Code(Index) & ". Please set it in BackOffice at first.", vbInformation + vbOKOnly
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Information + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)11, temp_VbStyle, Stock_Code[Index], (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 48, 11, stockCode, InformationOkMessageType)
                };
                return null;
            }

            slProp.pumpID = (byte)pumpId;
            slProp.PositionID = (byte)_fuelPumpService.GetPositionId(pumpId, gradeId);

            SetQuantity(ref slProp, isAmount, propaneValue, out error);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }

            if (_policyManager.USE_CUST)
            {
                if (_policyManager.USE_LOYALTY && Strings.UCase(System.Convert.ToString(_policyManager.LOYAL_TYPE)) == "PRICES" && sale.Customer.Loyalty_Code.Length > 0 && sale.Customer.CL_Status == "A" && (!slProp.LOY_EXCLUDE))
                {

                    var loyalPricecode = System.Convert.ToInt16(_policyManager.LOYAL_PRICE);
                    if (loyalPricecode > 0)
                    {
                        _saleLineManager.SetPriceNumber(ref slProp, loyalPricecode);
                    }
                    else
                    {
                        _saleLineManager.SetPriceNumber(ref slProp, sale.Customer.Price_Code != 0 ? sale.Customer.Price_Code : (short)1);
                    }
                }
                else
                {
                    _saleLineManager.SetPriceNumber(ref slProp, sale.Customer.Price_Code != 0 ? sale.Customer.Price_Code : (short)1);
                }
            }
            else
            {
                //SL_Prop.Price_Number = (short)1;
                _saleLineManager.SetPriceNumber(ref slProp, (short)1);
            }
            _saleManager.Add_a_Line(ref sale, slProp, UserCode, sale.TillNumber, out error, true);

            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
            return sale;
        }

        /// <summary>
        /// Method to get volume value
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="propaneValue">Propane value</param>
        /// <param name="error">Error message</param>
        /// <returns>Volume value</returns>
        public string GetVolumeValue(int gradeId, int pumpId, int saleNumber, int tillNumber, byte registerNumber,
        decimal propaneValue, out ErrorMessage error)
        {
            string quantity = "";
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, UserCode, out error);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }

            var grades = CacheManager.GetPropaneGrades();
            if (grades == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 76, null)
                };
                return null;
            }
            var firstOrdDefault = grades.FirstOrDefault(p => p.Id == gradeId);

            if (firstOrdDefault == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Request",
                        MessageType = MessageType.Critical
                    }
                };
                return null;
            }
            var pumps = _fuelPumpService.GetPumpsByPropaneGradeId(gradeId);
            if (pumps == null || !pumps.Any(p => p.Id == pumpId))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Request",
                        MessageType = MessageType.Critical
                    }
                };
                return null;
            }

            var stockCode = firstOrdDefault.StockCode;


            if (sale.DeletePrepay)
            {
                // "There's no Pump set for " & Stock_Code(index) & ". Please set it in Fuel Control at first.", vbinformation+vbokonly
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1150, null, InformationOkMessageType)
                };
                return null;
            }

            var slProp = new Sale_Line();

            _saleLineManager.SetPluCode(ref sale, ref slProp, stockCode, out error);
            slProp.GradeID = (byte)gradeId;
            if (slProp.Regular_Price == 0)
            {
                // "There's no price for " & Stock_Code(Index) & ". Please set it in BackOffice at first.", vbInformation + vbOKOnly
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Information + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)11, temp_VbStyle, Stock_Code[Index], (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 48, 11, stockCode, InformationOkMessageType)
                };
                return null;
            }

            if (slProp.Regular_Price > 0)
            {
                quantity = (Conversion.Val(propaneValue) / slProp.Regular_Price).ToString("#####0.000");
            }
            return quantity;
        }

        #region Private methods

        /// <summary>
        /// Method to set quantity
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="isAmount">Is amount</param>
        /// <param name="propaneValue">propane value</param>
        /// <param name="error">Error message</param>
        private void SetQuantity(ref Sale_Line saleLine, bool isAmount, decimal propaneValue, out ErrorMessage
            error)
        {
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (Conversion.Val(propaneValue) != 0)
            {
                if (!isAmount)
                {
                    if (Conversion.Val(propaneValue) > MaxVolumn)
                    {
                        //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation;
                        //Chaps_Main.DisplayMessage(this, (short)14, temp_VbStyle, null, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 48, 14, null, ExclamationOkMessageType)
                        };
                        return;
                    }
                    _saleLineManager.SetQuantity(ref saleLine, (float)(Conversion.Val(propaneValue)));

                    if (saleLine.Regular_Price > 0)
                    {
                        decimal amount = System.Convert.ToDecimal(saleLine.Regular_Price) * propaneValue;
                        _saleLineManager.SetAmount(ref saleLine, amount);
                    }

                }
                else //um="$"
                {
                    if (Conversion.Val(propaneValue) > MaxAmt)
                    {
                        //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation;
                        //Chaps_Main.DisplayMessage(this, (short)13, temp_VbStyle2, null, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 48, 13, null, ExclamationOkMessageType)
                        };
                        return;
                    }
                    else if (saleLine.Regular_Price > 0)
                    {
                        // SL_Prop.Quantity = float.Parse((Conversion.Val(txtValue.Text) / SL_Prop.Regular_Price).ToString("#####0.000"));
                        _saleLineManager.SetQuantity(ref saleLine, float.Parse((Conversion.Val(propaneValue) / saleLine.Regular_Price).ToString("#####0.000")));
                        _saleLineManager.SetAmount(ref saleLine, (decimal)(Conversion.Val(propaneValue)));

                        //string temp_Policy_Name = "FUEL_UM";
                        //lbQuantity.Text = "(" + System.Convert.ToString(SL_Prop.Quantity) + System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name, SL_Prop)) + ")";

                    }
                }
            }
            else //if change to 0
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please Set Amount/Volume for Prepay Sale",
                        MessageType = MessageType.Critical
                    }
                };
            }
        }

        #endregion


    }
}

