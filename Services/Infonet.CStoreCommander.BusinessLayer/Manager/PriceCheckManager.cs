using System;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PriceCheckManager : ManagerBase, IPriceCheckManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ISaleManager _saleManager;
        private readonly ITaxService _taxService;
        private readonly IStockService _stockService;
        private readonly IMainManager _mainManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="taxService"></param>
        /// <param name="stockService"></param>
        /// <param name="saleManager"></param>
        /// <param name="mainManager"></param>
        public PriceCheckManager(
            IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ISaleLineManager saleLineManager,
            ITaxService taxService,
            IStockService stockService,
            ISaleManager saleManager,
            IMainManager mainManager)
        {
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _saleLineManager = saleLineManager;
            _taxService = taxService;
            _stockService = stockService;
            _saleManager = saleManager;
            _mainManager = mainManager;
        }


        /// <summary>
        /// Get Stock Price details
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Stock price</returns>
        public StockPriceCheck GetStockPriceDetails(string stockCode, int tillNumber, int saleNumber, byte registerNumber, string userCode, out ErrorMessage error)
        {
            StockPriceCheck result = new StockPriceCheck { SpecialPriceTypes = GetSpecialPriceTypes() };
            Sale_Line slPrices = new Sale_Line();
            string txtCode = stockCode;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }

            slPrices.PriceCheck = true;
            //Stock_Br.Requery '  - To have the latest numbers; form
            // SLPrices.PLU_Code = cStock;
            //updated PLU setter
            _saleLineManager.SetPluCode(ref sale, ref slPrices, stockCode, out error);

            //  - For alternate codes change to the real stock code
            stockCode = slPrices.Stock_Code;

            float tePrice = 0;
            if (!string.IsNullOrEmpty(slPrices.Stock_Code))
            {
                var pd = slPrices.PRICE_DEC;
                result.IsPriceVisible = true;
                if ((slPrices.Stock_Type == 'V' || slPrices.Stock_Type == 'O') && !slPrices.ProductIsFuel)
                {
                    result.IsAvQtyVisible = true;
                    result.AvailableQuantity = slPrices.AvailItems.ToString(CultureInfo.InvariantCulture);
                }

                
                var productTaxExempt = _stockService.GetProductTaxExemptByProductCode(stockCode);
                if (productTaxExempt != null)
                {
                    result.TaxExemptPrice = GetTaxFreePrice(stockCode, (float)slPrices.Regular_Price, slPrices.ProductIsFuel, ref tePrice) ? tePrice.ToString(CultureInfo.InvariantCulture) : "";
                    // 
                    //                    If Policy.TE_Type = "AITE" Then ' Shiny added the if condition we are tracking separate inventory for TE product only for AITE
                    if (_policyManager.TAX_EXEMPT && _policyManager.TAX_EXEMPT && (_policyManager.TE_Type == "AITE" || (_policyManager.TE_Type != "AITE" && _policyManager.TRACK_TEINV))) //  - Only for AITE we need to track inventory separately for TE products 'If TaxExempt Then
                    {

                        if ((slPrices.Stock_Type == 'V' || slPrices.Stock_Type == 'O') && !slPrices.ProductIsFuel)
                        {
                            result.TaxExemptAvailable = productTaxExempt.Available.ToString();
                        }
                    }
                    result.IsTaxExemptVisible = true;
                }
                else
                {
                    result.IsTaxExemptVisible = false;
                }

                result.StockCode = stockCode;
                result.Description = slPrices.Description;
                result.RegularPriceText = slPrices.Regular_Price.ToString("#,##0." + new string('0', pd));
                var intSpPrices = (short)slPrices.SP_Prices.Count;
                if (intSpPrices != 0)
                {
                    result.FromDate = (slPrices.SP_Prices[1].FromDate).ToString("MM/dd/yyyy");
                    if (slPrices.SP_Prices[1].ToDate != System.DateTime.FromOADate(0))
                    {
                        result.IsToDateVisible = true;
                        result.ToDate = (slPrices.SP_Prices[1].ToDate).ToString("MM/dd/yyyy");
                        result.IsEndDateChecked = false;
                    }
                    else
                    {
                        result.IsToDateVisible = false;
                        result.IsEndDateChecked = true;
                    }
                    if (slPrices.Price_Units == '$')
                    {
                        result.IsPerDollarChecked = true;
                    }
                    else
                    {
                        result.IsPerPercentageChecked = true;
                    }
                }
                // Get the active vendor used to pick the prices and the price type
                result.VendorId = slPrices.Vendor;

                result.IsActiveVendorPrice = _stockService.IsActiveVendorPrice(stockCode, result.VendorId);
                var type = slPrices.Price_Units == '$' ? 1 : 2;

                SP_Price sp;
                switch (slPrices.Price_Type)
                {
                    case 'R':
                        result.PriceTypeText = _resourceManager.CreateCaption(offSet, 3, 39, null, 2); // "R - Regular Price"
                        break;
                    case 'S':
                        result.SalePrice = new SalePriceType { Columns = 1, SalePrices = new List<PriceGrid>() };
                        result.PriceTypeText = _resourceManager.CreateCaption(offSet, 3, 39, null, 3);// "S - Sale Price"
                        result.SalePrice.ColumnText = _resourceManager.CreateCaption(offSet, 4, 39, null, (short)type);
                        foreach (SP_Price tempLoopVarSp in slPrices.SP_Prices)
                        {
                            sp = tempLoopVarSp;
                            result.SalePrice.SalePrices.Add(new PriceGrid { Column1 = sp.Price.ToString("#,##0." + new string('0', pd)) });
                        }
                        result.IsAddButtonVisible = false;
                        result.IsRemoveButtonVisible = false;
                        break;

                    case 'F':
                        result.FirstUnitPrice = new FirstUnitPriceType { Columns = 2, FirstUnitPriceGrids = new List<PriceGrid>() };
                        result.PriceTypeText = _resourceManager.CreateCaption(offSet, 3, 39, null, 4); // "F - First Unit Price"
                        result.FirstUnitPrice.ColumnText = _resourceManager.GetResString(offSet, 230); // Quantity
                        result.FirstUnitPrice.ColumnText2 = _resourceManager.CreateCaption(offSet, 4, 39, null, (short)type); // Prices


                        foreach (SP_Price tempLoopVarSp in slPrices.SP_Prices)
                        {
                            sp = tempLoopVarSp;
                            result.FirstUnitPrice.FirstUnitPriceGrids.Add(new PriceGrid { Column1 = sp.From_Quantity.ToString(CultureInfo.InvariantCulture), Column2 = sp.Price.ToString("#,##0." + new string('0', pd)) });
                        }
                        result.IsAddButtonVisible = true;
                        result.IsRemoveButtonVisible = true;
                        break;

                    case 'I':
                        result.IncrementalPrice = new IncrementalPriceType { Columns = 3, IncrementalPriceGrids = new List<PriceGrid>() };

                        result.PriceTypeText = _resourceManager.CreateCaption(offSet, 3, 39, null, 5); // "I - Incremental Price"
                        result.IncrementalPrice.ColumnText = _resourceManager.CreateCaption(offSet, 4, 39, null, 3); //From
                        result.IncrementalPrice.ColumnText2 = _resourceManager.CreateCaption(offSet, 4, 39, null, 4); // To
                        result.IncrementalPrice.ColumnText3 = _resourceManager.CreateCaption(offSet, 4, 39, null, (short)type); // Prices


                        foreach (SP_Price tempLoopVarSp in slPrices.SP_Prices)
                        {
                            sp = tempLoopVarSp;
                            result.IncrementalPrice.IncrementalPriceGrids.Add(new PriceGrid { Column1 = sp.From_Quantity.ToString(CultureInfo.InvariantCulture), Column2 = sp.To_Quantity.ToString(CultureInfo.InvariantCulture), Column3 = sp.Price.ToString("#,##0." + new string('0', pd)) });
                        }
                        result.IsAddButtonVisible = true;
                        result.IsRemoveButtonVisible = true;
                        break;

                    case 'X':
                        result.XForPrice = new XForPriceType { Columns = 2, XForPriceGrids = new List<PriceGrid>() };
                        result.PriceTypeText = _resourceManager.CreateCaption(offSet, 3, 39, null, 6); // "X - 'X' for Price"
                        result.XForPrice.ColumnText = _resourceManager.GetResString(offSet, 230); // Quantity
                        result.XForPrice.ColumnText2 = _resourceManager.CreateCaption(offSet, 4, 39, null, (short)type); // Prices


                        foreach (SP_Price tempLoopVarSp in slPrices.SP_Prices)
                        {
                            sp = tempLoopVarSp;
                            result.XForPrice.XForPriceGrids.Add(new PriceGrid { Column1 = sp.From_Quantity.ToString(CultureInfo.InvariantCulture), Column2 = sp.Price.ToString("#,##0." + new string('0', pd)) });
                        }
                        result.IsAddButtonVisible = true;
                        result.IsRemoveButtonVisible = true;
                        break;
                }

                if (slPrices.Group_Price == false && _policyManager.U_CHGPRICE)
                {
                    result.IsChangePriceEnable = true;
                }
                else
                {
                    result.IsChangePriceEnable = false;
                }
                
                
                result.RegularPriceText = string.Format(result.RegularPriceText, "#,##0." + new string('0', pd));
                Register register = new Register();
                _mainManager.SetRegister(ref register, registerNumber);
                if (register.Customer_Display)
                {
                    result.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                        _mainManager.FormatLcdString(register, result.Description,
                        result.RegularPriceText), System.Convert.ToString(intSpPrices != 0 ?
                        _mainManager.FormatLcdString(register, _resourceManager.GetResString(offSet, 362), "") : ""));
                }

                //  - If  sell inactive product policy is true , even if the product is inactive we can sell the same. It is only for inactive  for Purchase and ordering)
                var days = _stockService.IsStockByDayAvailable(slPrices.Stock_Code);
                if (days.Count != 0)
                {
                    // processing
                    var dayOfWeek =
                        Convert.ToString(DateAndTime.Weekday(DateAndTime.Today));
                    if (!days.Contains(dayOfWeek))
                    {
                        slPrices.Active_StockCode = true;
                        // to avoid inactive stock item message in the main screen, saleLine item is not added to the sale anyway
                        slPrices.Active_DayOfWeek = false;
                        //PM to fix the issue related to Hot Buttons on August 27, 2012
                        var msg = _resourceManager.CreateMessage(offSet, 0, 8890, txtCode.Trim());
                        result.Message = msg.Message;
                        result.IsChangePriceEnable = false;
                        // no Else required, if it is found the product can be sold today, continue processing
                    }
                }
                else
                {
                    slPrices.Active_DayOfWeek = true; //PM on August 27, 2012
                }
                if (!slPrices.Active_StockCode || (!slPrices.Active_StockCode && !_policyManager.Sell_Inactive))
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1141, txtCode.Trim());
                    error.StatusCode = HttpStatusCode.NotFound;
                    return null;
                }
                
                if (slPrices.Price_Type == 'R')
                {
                    result.IsSpecialPricingVisible = false;
                    result.IsAddButtonVisible = false;
                    result.IsRemoveButtonVisible = false;
                    return result;
                }
                result.IsSpecialPricingVisible = true;
            }
            else // not a valid stock code
            {
                //Chaps_Main.DisplayMessage(this, (short)91, temp_VbStyle2, txtCode.Trim(), (byte)0);
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 39, 91, txtCode.Trim());
                error.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            return result;
        }

        /// <summary>
        /// Apply Regular Price
        /// </summary>
        /// <param name="priceCheck"></param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Stock price check</returns>
        public StockPriceCheck ApplyRegularPrice(RegularPriceCheck priceCheck,
            string userCode, out ErrorMessage error)
        {
            if (priceCheck.RegularPrice > 9999.99)
            {
                priceCheck.RegularPrice = 9.99;
            }
            double price = 0;
            var stockItem = _stockService.GetStockItem(priceCheck.StockCode);
            if (stockItem == null)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 39, 91, priceCheck.StockCode)
                };
                return null;
            }
            var vendorId = stockItem.Vendor;
            bool activeVendorPrice = _stockService.IsActiveVendorPrice(priceCheck.StockCode, vendorId);

            _stockService.DeletePreviousPrices(priceCheck.StockCode, "R", activeVendorPrice, vendorId);

            _stockService.AddUpdateRegularPrice(priceCheck.StockCode, ref vendorId, ref price, priceCheck.RegularPrice);

            _stockService.TrackPriceChange(priceCheck.StockCode, price, priceCheck.RegularPrice, "PC", 1, userCode, vendorId);

            var result = GetStockPriceDetails(priceCheck.StockCode, priceCheck.TillNumber, priceCheck.SaleNumber, priceCheck.RegisterNumber, userCode, out error);
            Register register = new Register();
            _mainManager.SetRegister(ref register, priceCheck.RegisterNumber);
            if (register.Customer_Display)
            {
                result.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                    _mainManager.FormatLcdString(register, result.Description,
                    result.RegularPriceText), "");
            }
            return result;
        }

        /// <summary>
        /// Apply Special Price
        /// </summary>
        /// <param name="priceCheck">Price check</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Stock price check</returns>
        public StockPriceCheck ApplySpecialPrice(SpecialPriceCheck priceCheck, string userCode, out ErrorMessage error)
        {
            if (priceCheck.RegularPrice > 9999.99)
            {
                priceCheck.RegularPrice = 9.99;
            }
            var stockItem = _stockService.GetStockItem(priceCheck.StockCode);
            if (stockItem == null)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 39, 91, priceCheck.StockCode)
                };
                return null;
            }
            var vendorId = stockItem.Vendor;
            bool activeVendorPrice = _stockService.IsActiveVendorPrice(priceCheck.StockCode, vendorId);

            _stockService.DeletePreviousPrices(priceCheck.StockCode, priceCheck.PriceType, activeVendorPrice, vendorId);

            if (!IsValidPricingInput(priceCheck.PriceType, priceCheck.GridPrices, out error))
            {
                return null;
            }


            foreach (PriceGrid priceGrid in priceCheck.GridPrices)
            {
                switch (priceCheck.PriceType[0].ToString())
                {
                    case "S":
                        if (Convert.ToDouble(priceGrid.Column1) > 9999.99)
                        {
                            priceGrid.Column1 = "9.99";
                        }
                        break;
                    case "F":
                        if (Convert.ToDouble(priceGrid.Column2) > 9999.99)
                        {
                            priceGrid.Column2 = "9.99";
                        }
                        break;
                    case "I":
                        if (Convert.ToDouble(priceGrid.Column3) > 9999.99)
                        {
                            priceGrid.Column3 = "9.99";
                        }
                        break;
                    case "X":
                        if (Convert.ToDouble(priceGrid.Column2) > 9999.99)
                        {
                            priceGrid.Column2 = "9.99";
                        }
                        break;
                    default:
                        if (Convert.ToDouble(priceGrid.Column1) > 9999.99)
                        {
                            priceGrid.Column1 = "9.99";
                        };
                        break;
                }
            }






            _stockService.AddUpdateSpecialPrice(priceCheck.StockCode, activeVendorPrice, ref vendorId, priceCheck.PriceType[0].ToString(),
                priceCheck.GridPrices, priceCheck.Fromdate, priceCheck.Todate, priceCheck.PerDollarChecked, priceCheck.IsEndDate);

            double price = 0;
            _stockService.AddUpdateRegularPrice(priceCheck.StockCode, ref vendorId, ref price, priceCheck.RegularPrice);

            _stockService.TrackPriceChange(priceCheck.StockCode, price, priceCheck.RegularPrice, "PC", 1, userCode, vendorId);

            var result = GetStockPriceDetails(priceCheck.StockCode, priceCheck.TillNumber, priceCheck.SaleNumber, priceCheck.RegisterNumber, userCode, out error);
            Register register = new Register();
            _mainManager.SetRegister(ref register, priceCheck.RegisterNumber);
            if (register.Customer_Display)
            {
                result.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                    _mainManager.FormatLcdString(register, result.Description,
                    result.RegularPriceText), "");
            }
            return result;
        }

        #region Private methods

        /// <summary>
        /// Checks whether Input is valid
        /// </summary>
        /// <param name="priceType">Price type</param>
        /// <param name="prices">Price grid</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        private bool IsValidPricingInput(string priceType, List<PriceGrid> prices, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            foreach (PriceGrid priceGrid in prices)
            {
                switch (priceType[0].ToString())
                {
                    case "S":
                        if (string.IsNullOrEmpty(priceGrid.Column1))
                        {
                            //MsgBox "You should enter a value", vbCritical + vbOK
                            //Chaps_Main.DisplayMessage(39, 93, temp_VbStyle, null, 0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 0, 93, null, CriticalOkMessageType)
                            };
                            return false;
                        }
                        break;
                    case "F":
                        if (string.IsNullOrEmpty(priceGrid.Column1))
                        {
                            //MsgBox "You should enter a value", vbCritical + vbOK
                            //Chaps_Main.DisplayMessage(39, 93, temp_VbStyle, null, 0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 0, 93, null, CriticalOkMessageType)
                            };
                            return false;
                        }
                        break;
                    case "I":
                        if (string.IsNullOrEmpty(priceGrid.Column1))
                        {
                            //MsgBox "You should enter a value", vbCritical + vbOK
                            //Chaps_Main.DisplayMessage(39, 93, temp_VbStyle, null, 0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 0, 93, null, CriticalOkMessageType)
                            };
                            return false;
                        }
                        break;
                    case "X":
                        if (string.IsNullOrEmpty(priceGrid.Column1))
                        {
                            //MsgBox "You should enter a value", vbCritical + vbOK
                            //Chaps_Main.DisplayMessage(39, 93, temp_VbStyle, null, 0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 0, 93, null, CriticalOkMessageType)
                            };
                            return false;
                        }
                        break;
                    default:
                        if (priceType[0].ToString() != "S" || priceType[0].ToString() != "F" || priceType[0].ToString() != "I" || priceType[0].ToString() != "X")
                        {
                            //MsgBox "You should enter a value", vbCritical + vbOK
                            //Chaps_Main.DisplayMessage(39, 93, temp_VbStyle, null, 0);
                            error = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    MessageType = 0,
                                    Message = "Please select Valid Price Type"
                                }
                            };
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Get Tax Free Price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="regPrice">Regular price</param>
        /// <param name="isFuelItem">Fuel item or not</param>
        /// <param name="taxExemptPrice">Tax exempt price</param>
        /// <returns>True or false</returns>
        private bool GetTaxFreePrice(string stockCode, float regPrice, bool isFuelItem, ref float taxExemptPrice)
        {
            short taxExemptTaxCode = 0;
            taxExemptPrice = 0;
            var productTaxEempt = _stockService.GetProductTaxExemptByProductCode(stockCode);

            if (productTaxEempt == null)
            {
                return false;
            }

            if (!_policyManager.TE_ByRate)
            {
                taxExemptPrice = productTaxEempt.TaxFreePrice;
                return true;
            }

            taxExemptTaxCode = productTaxEempt.TaxCode;
            float taxRate;
            string rateType;
            var result = _taxService.GetTaxExemptRate(taxExemptTaxCode.ToString(), out taxRate, out rateType);
            if (result)
            {
                taxExemptPrice = regPrice;
            }
            else
            {
                if (rateType == "$")
                {
                    taxExemptPrice = regPrice - taxRate;
                }
                else if (rateType == "%")
                {
                    taxExemptPrice = regPrice * (1 - taxRate / 100);
                }
                else
                {
                    taxExemptPrice = regPrice;
                }
                if (isFuelItem)
                {
                    taxExemptPrice = float.Parse(taxExemptPrice.ToString("#0.000"));
                }
                else
                {
                    taxExemptPrice = float.Parse(taxExemptPrice.ToString("#0.00"));
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the List of Special Price Types
        /// </summary>
        /// <returns>Special prices</returns>
        private List<string> GetSpecialPriceTypes()
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            List<string> items = new List<string>
            {
                _resourceManager.CreateCaption(offSet,16, 39, null, 2),//"R - Regular Price"
                _resourceManager.CreateCaption(offSet,16, 39, null, 3),//"S - Sale Pricing"
                _resourceManager.CreateCaption(offSet,16, 39, null, 4),//"F - First Unit Pricing"
                _resourceManager.CreateCaption(offSet,16, 39, null, 5),//"I - Incremental Pricing"
                _resourceManager.CreateCaption(offSet,16, 39, null, 6) //"X - 'X for' Pricing"
            };
            return items;
        }

        #endregion
    }
}
