using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Linq;
using System.Net;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Sale line manager
    /// </summary>
    public class SaleLineManager : ManagerBase, ISaleLineManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly IStockService _stockService;
        private readonly IFuelService _fuelService;
        private readonly IUtilityService _utilityService;
        private readonly ILoginManager _loginManager;
        private readonly IPromoManager _promoManager;
        private readonly IStockManager _stockManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="stockService"></param>
        /// <param name="fuelService"></param>
        /// <param name="utilityService"></param>
        /// <param name="loginManager"></param>
        /// <param name="promoManager"></param>
        /// <param name="stockManager"></param>
        public SaleLineManager(IApiResourceManager resourceManager,
            IPolicyManager policyManager,
            IStockService stockService,
            IFuelService fuelService,
            IUtilityService utilityService,
            ILoginManager loginManager,
            IPromoManager promoManager,
            IStockManager stockManager)
        {
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _stockService = stockService;
            _fuelService = fuelService;
            _utilityService = utilityService;
            _loginManager = loginManager;
            _promoManager = promoManager;
            _stockManager = stockManager;
        }

        /// <summary>
        /// Method to set sale line policies
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        public void SetSaleLinePolicy(ref Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetSaleLinePolicy,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            saleLine.GC_REPT = Convert.ToString(_policyManager.GC_REPT);
            saleLine.LOYAL_PPD = Convert.ToDecimal(_policyManager.LOYAL_PPD);
            saleLine.NUM_PRICE = Convert.ToInt16(_policyManager.NUM_PRICE);
            saleLine.GROUP_PRTY = Convert.ToBoolean(_policyManager.GROUP_PRTY);
            saleLine.GC_DISCOUNT = Convert.ToBoolean(_policyManager.GC_DISCOUNT);
            saleLine.LOYAL_DISC = Convert.ToInt16(_policyManager.LOYAL_DISC);

            Performancelog.Debug($"End,SaleLineManager,SetSaleLinePolicy,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to create a new saleLine
        /// </summary>
        /// <returns>Sale line</returns>
        public Sale_Line CreateNewSaleLine()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,CreateNewSaleLine,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var newSaleLine = new Sale_Line();
            SetSaleLinePolicy(ref newSaleLine);
            Performancelog.Debug($"End,SaleLineManager,CreateNewSaleLine,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return newSaleLine;
        }

        /// <summary>
        /// Method to set plu code
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="error">Error</param>
        /// <param name="isReturnMode">Return mode or not</param>
        public void SetPluCode(ref Sale sale, ref Sale_Line saleLine, string stockCode, out ErrorMessage error, bool isReturnMode = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetPluCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            error = new ErrorMessage();
            if (saleLine.No_Loading)
            {
                return;
            }
            string tempUserInputString = Convert.ToString(stockCode);
            string tempAvoidedValuesString = "";
            var code = Strings.UCase(Convert.ToString(Helper.SqlQueryCheck(ref tempUserInputString, ref tempAvoidedValuesString))); ////avoided string values Sample string = """,',--"

            saleLine.PLU_Code = code;
            var stockCache = _stockManager.GetStockDetails(code);

            if (stockCache == null)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 99, stockCode, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return;
            }
            MapSaleLine(ref saleLine, stockCache);
            SetStockCode(ref sale, ref saleLine, stockCache.StockCode, saleLine.User, out error, isReturnMode);

            Performancelog.Debug($"End,SaleLineManager,SetPluCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set sub detail
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="subDetail">Sub detail</param>
        public void SetSubDetail(ref Sale_Line saleLine, string subDetail)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetSubDetail,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //nancy 03/04/2002
            //checking Grade table's Stock_code to see saleLine product is Fuel or not
            saleLine.Sub_Detail = subDetail;

            var stockCode = saleLine.Stock_Code.Trim().ToUpper();
            var fuelType = _fuelService.GetFuelTypeFromDbPump(stockCode);
            if (!string.IsNullOrEmpty(fuelType))
            {
                saleLine.ProductIsFuel = true;
                saleLine.IsPropane = fuelType == "O";
            }
            else
            {
                saleLine.ProductIsFuel = false;
                saleLine.IsPropane = false;
            }
            //nancy end
            Performancelog.Debug($"End,SaleLineManager,SetSubDetail,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to adjust the discount rate
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="discountRate">Discount rate</param>
        public void SetDiscountRate(ref Sale_Line saleLine, float discountRate)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetDiscountRate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            saleLine.Discount_Rate = discountRate;
            if (!saleLine.No_Loading)
            {
                switch (saleLine.Discount_Type)
                {
                    case "%":
                        saleLine.Line_Discount = Convert.ToDouble(Convert.ToSingle(saleLine.Amount) * saleLine.Discount_Rate / 100);
                        break;
                    case "$":


                        if (saleLine.LINE_TYPE.ToUpper() == "LINE TOTAL")
                        {


                            saleLine.Line_Discount = saleLine.Discount_Rate;
                        }
                        else
                        {
                            saleLine.Line_Discount = saleLine.Quantity * saleLine.Discount_Rate;
                        }
                        break;
                    default:
                        saleLine.Line_Discount = 0;
                        break;
                }
            }

            saleLine.Net_Amount = decimal.Round(saleLine.Amount - (decimal)(saleLine.Line_Discount + saleLine.Discount_Adjust), 2);


            Performancelog.Debug($"End,SaleLineManager,SetDiscountRate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set the amount
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="amount">Amount</param>
        public void SetAmount(ref Sale_Line saleLine, decimal amount)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetAmount,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            if (saleLine.No_Loading)
            {

                //  
                if (saleLine.NoPriceFormat)
                {
                    saleLine.Amount = amount;
                }
                else
                {
                    saleLine.Amount = (decimal)modGlobalFunctions.Round((double)amount, 2);
                }
                //  - we need to have saleLine line for all preapy (irrespective of taxexemp)- oherwise regular prepay gross is same as original prepay set  and not updating th salehead.sale_amt for partial prepay
                //        '  - If site tax exempt for prepay use the discount to give extra gas ( even if they pumped for more gas - gross should be same as what they set)
                //        If Policy.TAX_EXEMPT And Policy.TE_Type = "SITE" And Me.Prepay And Me.IsTaxExemptItem Then
                //             mvarNetAmount = mvarAmount - Round(CDbl(mvarLine_Discount + mvarDiscount_Adjust), 2)
                //         End If
                if (saleLine.Prepay)
                {
                    saleLine.Net_Amount = saleLine.Amount - (decimal)modGlobalFunctions.Round(saleLine.Line_Discount + saleLine.Discount_Adjust, 2);
                }
            }
            else
            {
                if (saleLine.Amount != amount || saleLine.Net_Amount != amount)
                {
                    if (saleLine.NoPriceFormat)
                    {
                        saleLine.Amount = amount;
                    }
                    else
                    {
                        saleLine.Amount = (decimal)modGlobalFunctions.Round((double)amount, 2);
                    }

                    saleLine.Discount_Rate = saleLine.Discount_Rate;

                    saleLine.Net_Amount = saleLine.Amount - (decimal)modGlobalFunctions.Round(saleLine.Line_Discount + saleLine.Discount_Adjust, 2);
                }
            }
            Performancelog.Debug($"End,SaleLineManager,SetAmount,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set price
        /// </summary>
        /// <param name="saleLine">Saleline</param>
        /// <param name="price">Price</param>
        public void SetPrice(ref Sale_Line saleLine, double price)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            // Dec 18, 2006 Nicolette added to format field according to the policy before the control goes back to sale main, otherwise the user will be asked for a reason if the policy to use reasons is set to Yes
            var fs = Convert.ToString(saleLine.PRICE_DEC == 0 ? "0" : ("0." + new string('0', saleLine.PRICE_DEC)));
            // promotions
            saleLine.price = saleLine.NoPriceFormat ? price : double.Parse(price.ToString(fs));
            // End Dec 18, 2006

            // mvarPrice = vData



            if (saleLine.No_Loading || saleLine.LoadFuelAmount)
            {
                return;
            }




            if (saleLine.Price_Type == 'X' && saleLine.Price_Number == 1)
            {
                SetAmount(ref saleLine, (decimal)saleLine.price);
            }
            else
            {

                SetAmount(ref saleLine, (decimal)(saleLine.price * saleLine.Quantity));

            }
            Performancelog.Debug($"End,SaleLineManager,SetPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set quantity
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="quantity">Quantity</param>
        public void SetQuantity(ref Sale_Line saleLine, float quantity)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetQuantity,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (saleLine.PriceCheck)
            {
                return; //   we donot need saleLine setting in pricecheck
            }
            if (!saleLine.No_Loading)
            {


                if (saleLine.User == Entities.Constants.Trainer) //Behrooz Jan-12-06
                {
                    saleLine.Quantity = quantity;
                    return;
                }

                //if ((saleLine.Stock_Type == 'V' || saleLine.Stock_Type == 'O') && !saleLine.ProductIsFuel) // Only for Tracking type stocks
                //{

                //}
                saleLine.Quantity = quantity;
            }
            else
            {
                saleLine.Quantity = quantity;
            }
            Performancelog.Debug($"End,SaleLineManager,SetQuantity,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set price number
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="priceNumber">Price number</param>
        public void SetPriceNumber(ref Sale_Line saleLine, short priceNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetPriceNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            saleLine.Price_Number = priceNumber;
            if (saleLine.ManualFuel)
            {
                return; //  - for manual fuel sales with customers it is saving wrong price and discount(from stock tables) in the csccursale. But screen is correct. So in crash recovery wrong info
            }
            if (saleLine.No_Loading)
            {
                return;
            }

            if (priceNumber >= 1 & priceNumber <= _policyManager.NUM_PRICE)
            {
                string whereVendor;
                if (string.IsNullOrEmpty(saleLine.Vendor))
                {
                    whereVendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
                }
                else
                {
                    whereVendor = " AND VendorID=\'" + saleLine.Vendor + "\' AND OrderNo=0 ";
                }

                var price = _stockService.GetStockPriceForPriceNumber(saleLine.Stock_Code, priceNumber, whereVendor);
                if (price.HasValue)
                {
                    if ((int)(price.Value) == 0)
                    {
                        saleLine.price = saleLine.Regular_Price;
                    }
                    else
                    {
                        saleLine.price = price.Value;
                    }
                    if (priceNumber > 1)
                    {
                        saleLine.Price_Type = 'R'; //  
                    }
                }
                else
                {
                    whereVendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
                    price = _stockService.GetStockPriceForPriceNumber(saleLine.Stock_Code, priceNumber, whereVendor);
                    if (price.HasValue)
                    {
                        if ((int)(price.Value) == 0)
                        {
                            saleLine.price = saleLine.Regular_Price;
                        }
                        else
                        {
                            saleLine.price = price.Value;
                        }
                    }
                    else
                    {
                        saleLine.price = saleLine.Regular_Price;
                    }
                    if (priceNumber > 1)
                    {
                        saleLine.Price_Type = 'R'; //  
                    }
                }
            }
            else
            {
                saleLine.Price_Number = 1;
                saleLine.price = saleLine.Regular_Price;
            }
            Performancelog.Debug($"End,SaleLineManager,SetPriceNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Method to set stock code
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="userCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="isReturnMode"></param>
        public void SetStockCode(ref Sale sale, ref Sale_Line saleLine, string stockCode, string userCode, out ErrorMessage errorMessage, bool isReturnMode = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetStockCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (string.IsNullOrEmpty(stockCode))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 99, stockCode, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return;
            }

            //set discount
            saleLine.Discount_Type = " ";
            saleLine.Associate_Amount = 0;
            saleLine.Total_Amount = 0;
            SetDiscountRate(ref saleLine, 0);
            SetSubDetail(ref saleLine, saleLine.Sub_Detail);
            //set user
            saleLine.User = userCode;
            
            if (isReturnMode)
            {
                SetQuantity(ref saleLine, -1);
            }
            else
            {

                SetQuantity(ref saleLine, 1);
            }

            //Set the special price
            IsPriceSet(ref saleLine);
            //set available items
            SetAvailableItems(ref saleLine);
            SetPromotionalInformation(ref sale, ref saleLine, out errorMessage);
            SetLevelPolicies(ref saleLine);

            // Dec 18, 2006 Nicolette added to format field according to the policy before the control goes back to sale main, otherwise the user will be asked for a reason if the policy to use reasons is set to Yes
            string fs = Convert.ToString(saleLine.PRICE_DEC == 0 ? "0" : "0." + new string('0', saleLine.PRICE_DEC));
            saleLine.Regular_Price = double.Parse(saleLine.Regular_Price.ToString(fs));

            //   added the Me.Price_Type <> "R" condition to avoid the Price_Type property let to be processed again
            if (saleLine.Price_Type != 'R' && saleLine.SP_Prices == null)
            {
                saleLine.Price_Type = 'R';
            }
            else if (saleLine.Price_Type != 'R' && saleLine.SP_Prices.Count == 0) //  
            {
                saleLine.Price_Type = 'R';
            }

            Performancelog.Debug($"End,SaleLineManager,SetStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Method to make taxes
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <returns>Line taxes</returns>
        public Line_Taxes Make_Taxes(Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,Make_Taxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string cStock = saleLine.Stock_Code;

            var lt = new Line_Taxes();



            if (saleLine.Stock_Type == 'G')
            {
                return null;
            }
            var taxes = _stockService.GetStockTaxes(cStock);
            if (taxes != null && taxes.Count != 0)
            {
                foreach (var tax in taxes)
                {
                    var taxMaster = _stockService.GetTaxMast(tax.Name);
                    if (taxMaster?.Active == null || !taxMaster.Active.Value) continue;
                    var taxRate = _stockService.GetTaxRate(tax.Name, tax.Code);

                    if (taxRate != null)
                    {
                        lt.Add(tax.Name,
                            tax.Code,
                            taxRate.Rate ?? 0,
                            taxRate.Included ?? false,
                            Convert.ToSingle(taxRate.Rebate ?? 0), 0,
                            tax.Name);
                    }
                }
            }
            var returnValue = lt;
            Performancelog.Debug($"End,SaleLineManager,Make_Taxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }
                

        /// <summary>
        /// Method to apply table discount
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="pc">PC</param>
        /// <param name="cc">cc</param>
        /// <param name="errorMessage">Error message</param>
        public void Apply_Table_Discount(ref Sale_Line saleLine, short pc, short cc, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,Apply_Table_Discount,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();


            if (!_policyManager.OR_USER_DISC && saleLine.User_Discount)
            {
                return;
            }




            if (!_policyManager.AUTO_SALE && saleLine.Price_Type != 'R')
            {
                return;
            }




            string tempPolicyName = "CL_DISCOUNTS";
            if (_policyManager.GetPol(tempPolicyName, saleLine) && (!saleLine.Gift_Certificate || (saleLine.Gift_Certificate && _policyManager.GC_DISCOUNT)))
            {

                //       (Not Me.Gift_Certificate Or (Me.Gift_Certificate And GC_DISCOUNT)) Then




                string tempPolicyName2 = "MAX_DISC%";
                int maxDisc = Convert.ToInt32(_policyManager.GetPol(tempPolicyName2, saleLine));



                var discountPercentage = _utilityService.GetDiscountPercent(pc, cc);
                if (discountPercentage.HasValue)
                {
                    if (discountPercentage.Value <= maxDisc)
                    {
                        saleLine.Discount_Type = "%";
                        saleLine.Discount_Rate = discountPercentage.Value;
                    }
                    else
                    {
                        var offSet = _policyManager.LoadStoreInfo().OffSet;
                        MessageType tempVbStyle = (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly);
                        errorMessage = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8196, maxDisc, tempVbStyle),
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                }
                else
                {
                    saleLine.Discount_Type = " ";
                    saleLine.Discount_Rate = 0;
                }
            }
            else
            {
                saleLine.Discount_Type = "";
                saleLine.Discount_Rate = 0;
            }
            //  For discount loyalty, system was not showing the savings
            var loyaltyDiscountCode = _policyManager.LOYAL_DISC;
            saleLine.LoyaltyDiscount = loyaltyDiscountCode == cc;
            //shiny end
            Performancelog.Debug($"End,SaleLineManager,Apply_Table_Discount ,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to apply fuel loyalty 
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="discountType">Discount type</param>
        /// <param name="discountRate">Discount rate</param>
        /// <param name="discountName">Discount name</param>
        public void ApplyFuelLoyalty(ref Sale_Line saleLine, string discountType, float discountRate, string discountName = "")
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,ApplyFuelLoyalty,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (string.IsNullOrEmpty(discountType))
            {
                Performancelog.Debug($"End,SaleLineManager,ApplyFuelLoyalty,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return;
            }

            switch (discountType)
            {
                case "%":
                    saleLine.Discount_Type = "%";
                    saleLine.Discount_Rate = discountRate;
                    saleLine.DiscountName = discountName; // 
                    break;
                case "$":
                case "D":
                    //  - discount chart is entered in cents, we need to make it as $
                    if (discountType == "D")
                    {
                        if (Math.Abs(discountRate) > 0)
                        {
                            discountRate = discountRate / 100;
                        }
                    }
                    //shiny end
                    saleLine.Discount_Type = "$";
                    saleLine.DiscountName = discountName; // 







                    saleLine.LINE_TYPE = "Each Item";
                    //  (2.5C) to 3 c
                    //            Me.Discount_Rate = Format(DiscountRate, "#0.00")
                    //            fs = IIf(GetPol("PRICE_DEC", Me) = 0, "0", "0." & String$(GetPol("PRICE_DEC", Me), "0"))
                    saleLine.Discount_Rate = discountRate; //Format(DiscountRate, fs)
                    break;
                // 09

                default:
                    saleLine.Discount_Type = " ";
                    saleLine.Discount_Rate = 0;
                    saleLine.DiscountName = ""; // 
                    break;
            }
            SetDiscountRate(ref saleLine, saleLine.Discount_Rate);
            Performancelog.Debug($"End,SaleLineManager,ApplyFuelLoyalty,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to apply fuel rebate
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        public void ApplyFuelRebate(ref Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,ApplyFuelRebate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (saleLine.FuelRebate <= 0)
            {
                return;
            }

            saleLine.Discount_Type = "$";






            saleLine.LINE_TYPE = "Each Item";
            saleLine.Discount_Rate = float.Parse(saleLine.FuelRebate.ToString("#0.00"));
            SetDiscountRate(ref saleLine, saleLine.Discount_Rate);

            Performancelog.Debug($"End,SaleLineManager,ApplyFuelRebate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to make group prices
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="dept">Department</param>
        /// <param name="subDept">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns></returns>
        public SP_Prices MakeGroupPrice(ref Sale_Line saleLine, string dept, string subDept, string subDetail)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,MakeGroupPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            SP_Prices returnValue = default(SP_Prices);

            var groupPriceHead = _stockService.GetGroupPriceHead(dept, subDept, subDetail);
            var sp = new SP_Prices();

            if (groupPriceHead != null)
            {
                if (groupPriceHead.PrType != 'R' && groupPriceHead.PrFrom <= DateAndTime.Today && (groupPriceHead.PrTo == DateTime.MinValue || groupPriceHead.PrTo >= DateAndTime.Today))
                {
                    saleLine.Price_Type = groupPriceHead.PrType;
                    saleLine.Price_Units = groupPriceHead.PrUnit;
                    var prices = _stockService.GetGroupPriceLines(dept, subDept, subDetail);
                    foreach (var price in prices)
                    {
                        sp.Add(Convert.ToSingle(price.PrFQty), Convert.ToSingle(price.PrTQty), Convert.ToSingle(price.Price), groupPriceHead.PrFrom, groupPriceHead.PrTo, "");
                    }
                    returnValue = sp;
                }

            }
            Performancelog.Debug($"End,SaleLineManager,MakeGroupPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to load sale line policies
        /// </summary>
        /// <param name="saleLine"></param>
        public void LoadSaleLinePolicies(ref Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,LoadSaleLinePolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string tempPolicyName = "QUANT_DEC";
            saleLine.QUANT_DEC = Convert.ToInt16(_policyManager.GetPol(tempPolicyName, saleLine));
            string tempPolicyName2 = "PRICE_DEC";
            saleLine.PRICE_DEC = Convert.ToInt16(_policyManager.GetPol(tempPolicyName2, saleLine));
            string tempPolicyName3 = "LOY-EXCLUDE";
            saleLine.LOY_EXCLUDE = Convert.ToBoolean(_policyManager.GetPol(tempPolicyName3, saleLine));
            string tempPolicyName4 = "I_RIGOR";
            saleLine.I_RIGOR = Convert.ToBoolean(_policyManager.GetPol(tempPolicyName4, saleLine));
            string tempPolicyName5 = "VOL_POINTS";
            saleLine.VOL_POINTS = Convert.ToBoolean(_policyManager.GetPol(tempPolicyName5, saleLine));
            string tempPolicyName6 = "LOYAL_PPU";
            saleLine.LOYAL_PPU = Convert.ToInt16(_policyManager.GetPol(tempPolicyName6, saleLine));
            string tempPolicyName7 = "TE_COLLECTTAX";
            saleLine.TE_COLLECTTAX = Convert.ToString(_policyManager.GetPol(tempPolicyName7, saleLine));


            if (_policyManager.ThirdParty)
            {
                string tempPolicyName10 = "TrdPtyExt";
                saleLine.ThirdPartyExtractCode = Convert.ToString(_policyManager.GetPol(tempPolicyName10, saleLine)); //3,5,6,7,10
            }



            if (_policyManager.TAX_EXEMPT)
            {
                string tempPolicyName11 = "TE_AgeRstr";
                saleLine.TE_AgeRstr = Convert.ToString(_policyManager.GetPol(tempPolicyName11, saleLine));
            }
            else
            {
                saleLine.TE_AgeRstr = false.ToString();
            }
            Performancelog.Debug($"End,SaleLineManager,LoadSaleLinePolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");


        }

        /// <summary>
        /// Method to get fuel discount chart rate
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="groupId">Group Id</param>
        /// <param name="gradeId">Grade ID</param>
        /// <returns>Fuel discount</returns>
        public float GetFuelDiscountChartRate(ref Sale_Line saleLine, string groupId, byte gradeId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,GetFuelDiscountChartRate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            float returnValue = 0;
            var discountRate = _fuelService.GetDiscountRate(groupId, Convert.ToString(gradeId));
            if (discountRate.HasValue)
            {
                returnValue = discountRate.Value;
            }
            Performancelog.Debug($"End,SaleLineManager,GetFuelDiscountChartRate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }



        /// <summary>
        /// Method to  check restrictions on stock 
        /// </summary>
        /// <param name="isReturnMode">Return mode or not</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TillNumber</param>
        /// <param name="userCode"> User code</param>
        /// <param name="error">Error</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock message</returns>
        public StockMessage CheckStockConditions(int saleNumber, int tillNumber, string stockCode, string userCode,
            bool isReturnMode, float quantity, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,NewCheckStockConditions,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            error = new ErrorMessage();
            var stockMessage = new StockMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var user = _loginManager.GetExistingUser(userCode);
            string tempUserInputString = Convert.ToString(stockCode);
            string tempAvoidedValuesString = "";
            string code = Strings.UCase(Convert.ToString(Helper.SqlQueryCheck(ref tempUserInputString, ref tempAvoidedValuesString))); ////avoided string values Sample string = """,',--"

            var saleLine = _stockService.GetSaleLineInfo(code);

            if (!string.IsNullOrEmpty(saleLine?.Stock_Code))
            {
                saleLine.User = userCode;
                if (!saleLine.Active_DayOfWeek)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8890, saleLine.Stock_Code, MessageType.OkOnly),
                        StatusCode = HttpStatusCode.Conflict
                    };
                    error.StatusCode = HttpStatusCode.NotAcceptable;
                    Performancelog.Debug($"End,SaleLineManager,NewCheckStockConditions,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    return null;
                }

                if (saleLine.Stock_Type == 'G')
                {
                    var tempPolicyName8 = "GiftType";
                    saleLine.GiftType = Convert.ToString(_policyManager.GetPol(tempPolicyName8, saleLine)); //NONE,LocalGift,GiveX,Milliplein
                }
                else
                {
                    saleLine.GiftType = "NONE";
                }
                //check if it is a regular price prompt
                if (!saleLine.IsPriceSet)
                {
                    stockMessage.RegularPriceMessage = _resourceManager.CreateMessage(offSet, 0, 8112, stockCode, MessageType.OkOnly);
                }
                //check if stock is active
                //  - if not active stock code shouldn't allow to sell
                if (saleLine.Active_StockCode == false && _policyManager.Sell_Inactive == false) // shiny changed on aug24, added policy.sell_inactive. If policy is true, even if the product is inactive we can sell the product
                {
                    MessageType tempVbStyle2 = (int)MessageType.Information + MessageType.OkOnly;
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 41, stockCode, tempVbStyle2);
                    error.StatusCode = HttpStatusCode.NotAcceptable;
                    Performancelog.Debug($"End,SaleLineManager,NewCheckStockConditions,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    return null;
                }
                var enteredQuantity = isReturnMode ? -quantity : quantity;
                string temp_Policy = "ACCEPT_RET";

                if (enteredQuantity < 0 && !_policyManager.GetPol(temp_Policy, saleLine))
                {
                    // Product Returns are not accepted for this Product
                    MessageType temp_VbStyle11 = (int)MessageType.Critical + MessageType.OkOnly;
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8109, saleLine.Stock_Code + " " + saleLine.Description, temp_VbStyle11);
                    error.StatusCode = HttpStatusCode.NotAcceptable;
                    return null;
                }
                if (enteredQuantity < 0 && !_policyManager.GetPol("U_GIVEREF", user))
                {
                    // You are not authorized to give refunds
                    MessageType temp_VbStyle12 = (int)MessageType.Critical + MessageType.OkOnly;
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)8110, null, temp_VbStyle12);
                    error.StatusCode = HttpStatusCode.Unauthorized;
                    return null;
                }

                if ((saleLine.PluType == 'S' || saleLine.PluType == 'K') && saleLine.Stock_Type != 'G')
                {
                    stockMessage.RestrictionPage = VerifyRestriction(user, saleLine, saleNumber, tillNumber, out error);
                    string tempPolicyName = "ALLOW_ENT";
                    stockMessage.CanManuallyEnterProduct = _policyManager.GetPol(tempPolicyName, saleLine);
                    stockMessage.ManuallyEnterMessage =
                        _resourceManager.CreateMessage(offSet, 11, 85, stockCode, MessageType.OkOnly).Message;
                }
                else if (saleLine.GiftType == "LocalGift")
                {
                    //check for giftcard
                    stockMessage.GiftCertPage = VeriftGiftCertPage(isReturnMode, saleLine, out error);
                    stockMessage.CanManuallyEnterProduct = true;
                    stockMessage.ManuallyEnterMessage =
                        _resourceManager.CreateMessage(offSet, 11, 85, stockCode, MessageType.OkOnly).Message;

                }
                else if (saleLine.GiftType == "GiveX")
                {
                    stockMessage.GivexPage = VerifyGiftCardPage(isReturnMode, saleLine, out error);
                    stockMessage.CanManuallyEnterProduct = true;
                    stockMessage.ManuallyEnterMessage =
                    _resourceManager.CreateMessage(offSet, 11, 85, stockCode, MessageType.OkOnly).Message;
                }

                if (saleLine.PluType == 'S' && saleLine.Dept == "40")
                {
                    stockMessage.PSInetPage = VerifyPSInetPage(isReturnMode, saleLine, out error);
                    stockMessage.CanManuallyEnterProduct = true;
                    stockMessage.ManuallyEnterMessage =
                    _resourceManager.CreateMessage(offSet, 11, 85, stockCode, MessageType.OkOnly).Message;
                }

                //check for quantity prompt message
                stockMessage.QuantityMessage = VerifyQuantity(saleLine, quantity);
            }
            else
            {
                //check if stock code exists or can add stock    
                var userCanAddStock = _policyManager.GetPol("U_AddStock", user);
                if (string.IsNullOrEmpty(saleLine?.Stock_Code) && !userCanAddStock)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 11, 99, stockCode, MessageType.OkOnly),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
                if (string.IsNullOrEmpty(saleLine?.Stock_Code) && userCanAddStock)
                {
                    error.MessageStyle = new MessageStyle();
                    stockMessage.AddStockPage = new AddStockPage
                    {
                        OpenAddStockPage = true,
                        StockCode = stockCode
                    };
                    Performancelog.Debug($"End,SaleLineManager,NewCheckStockConditions,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                    return stockMessage;
                }
            }
            Performancelog.Debug($"End,SaleLineManager,NewCheckStockConditions,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockMessage;
        }

        /// <summary>
        /// Method to make promo
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="sale"></param>
        /// <param name="changeQuantity"></param>
        public void Make_Promo(ref Sale sale, ref Sale_Line saleLine, bool changeQuantity = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,Make_Promo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            bool boolItemInPromo = false;
            var security = _policyManager.LoadSecurityInfo();
            var allPromos = new Promos();
            if (security.BackOfficeVersion.ToUpper() == "FULL" || _policyManager.PROMO_SALE)
            {
                allPromos = _promoManager.Load_Promos("");
            }
            bool[] links;
            short i;
            short k;
            short j;
            short newIndex;
            double sumRegPrice;
            float qtyByStock;
            float totalQty = 0;
            Promo pr;
            Sale_Line sl;
            SP_Prices sp;
            short idxFound = 0;
            Promo_Line pl;
            bool boolAddItem;
            bool boolItemExists;
            short kk = 0;
            short[] arrQtySet;

            if (saleLine.No_Loading)
            {
                return;
            }
            if (saleLine.NoPromo)
            {
                return;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var none = _resourceManager.GetResString(offSet, 347);
            var arrPromos = new string[1];
            var tempArrPromo = new float[6, 1];
            if (saleLine.HotButton || saleLine.ProductIsFuel)
            {
                changeQuantity = true;
            }

            // added on August 20, 2009 to fix the issue for quantity with decimals
            // quantity has to be already formated when the promo is made
            var fs = Convert.ToString(saleLine.QUANT_DEC == 0 ? "0" : "0." + new string('0', saleLine.QUANT_DEC));
            SetQuantity(ref saleLine, float.Parse(saleLine.Quantity.ToString(fs)));
            // endif

            // 1. verify item is in a promo based on stock item code
            // set the mvarTotalQty_ByStock variable, is the same for all levels because is based on stock code
            if (changeQuantity)
            {
                saleLine.TotalQty_ByStock = (float)Get_TotalQuantity(ref saleLine, ref sale, allPromos, 1, saleLine.PromoID);
            }
            else
            {
                saleLine.TotalQty_ByStock =
                    (float)(saleLine.Quantity + Get_TotalQuantity(ref saleLine, ref sale, allPromos, 1, saleLine.PromoID));
            }
            var stockCode = saleLine.Stock_Code;
            var dept = saleLine.Dept;
            var subDept = saleLine.Sub_Dept;
            var subDetail = saleLine.Sub_Detail;
            var promoItems = new Promos();
            var promosForToday = _promoManager.GetPromosForToday();
            var promos = promosForToday.Where(p => p.StockCode == stockCode).ToList();
            if (promos == null || promos.Count == 0)
            {
                boolItemInPromo = false;
            }
            _promoManager.Clear_AllPromos(ref promoItems);

            foreach (var promo in promos)
            {
                promoItems.Add(allPromos.get_Item(promo.PromoID), promo.PromoID);
            }

            foreach (var promo in promos)
            {
                boolItemInPromo = true;

                k = (short)(Information.UBound(tempArrPromo, 2) + 1);
                tempArrPromo =
                    (float[,])
                    Utils.CopyArray(tempArrPromo, new float[6, k + 1]);
                tempArrPromo[1, k] = Convert.ToSingle(promo.MaxLink);
                tempArrPromo[2, k] = 1;
                tempArrPromo[3, k] = Convert.ToSingle(promo.TotalQty);

                // total quantity is based on promoID because tempArrPromo(4, k) is based on promo
                // so for different promo could be different totalqty based on link
                // TotalQty is based on link number July 03, 2009
                totalQty = 0;
                if (changeQuantity)
                {
                    totalQty =
                        (float)
                        Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                            Convert.ToString(promo.PromoID));
                }
                else
                {
                    totalQty =
                        (float)
                        (saleLine.Quantity +
                         Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                             Convert.ToString(promo.PromoID)));
                }

                tempArrPromo[4, k] = totalQty;
                tempArrPromo[5, k] = Convert.ToSingle(promo.Amount);

                Array.Resize(ref arrPromos, arrPromos.Length - 1 + 1 + 1);
                arrPromos[arrPromos.Length - 1] = Convert.ToString(promo.PromoID);
            }

            // 2. verify item is in promo based on dept
            promos = promosForToday.Where(p => p.Dept == dept && p.SubDept == none && p.SubDetail == none).ToList();
            if ((promos == null || promos.Count == 0) && !boolItemInPromo)
            {
                boolItemInPromo = false;
            }
            _promoManager.Clear_AllPromos(ref promoItems);
            foreach (var promo in promos)
            {
                promoItems.Add(allPromos.get_Item(promo.PromoID), promo.PromoID);
            }
            foreach (var promo in promos)
            {
                boolItemInPromo = true;

                k = (short)(Information.UBound(tempArrPromo, 2) + 1);
                tempArrPromo =
                    (float[,])
                    Utils.CopyArray(tempArrPromo, new float[6, k + 1]);
                tempArrPromo[1, k] = Convert.ToSingle(promo.MaxLink);
                tempArrPromo[2, k] = 2;
                tempArrPromo[3, k] = Convert.ToSingle(promo.TotalQty);

                totalQty = 0;
                if (changeQuantity)
                {
                    totalQty =
                        (float)
                        Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                            Convert.ToString(promo.PromoID));
                }
                else
                {
                    totalQty =
                        (float)
                        (saleLine.Quantity +
                         Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                             Convert.ToString(promo.PromoID)));
                }

                tempArrPromo[4, k] = totalQty;
                tempArrPromo[5, k] = Convert.ToSingle(promo.Amount);

                Array.Resize(ref arrPromos, arrPromos.Length - 1 + 1 + 1);
                arrPromos[arrPromos.Length - 1] = Convert.ToString(promo.PromoID);
            }

            // 3 verify that item is in a promo based on dept and subdept
            promos = promosForToday.Where(p => p.Dept == dept && p.SubDept == subDept && p.SubDetail == none).ToList();
            _promoManager.Clear_AllPromos(ref promoItems);
            foreach (var promo in promos)
            {
                promoItems.Add(allPromos.get_Item(promo.PromoID), promo.PromoID);
            }
            if ((promos == null || promos.Count == 0) && !boolItemInPromo)
            {
                boolItemInPromo = false;
            }

            foreach (var promo in promos)
            {
                boolItemInPromo = true;

                k = (short)(Information.UBound(tempArrPromo, 2) + 1);
                tempArrPromo =
                    (float[,])
                    Utils.CopyArray(tempArrPromo, new float[6, k + 1]);
                tempArrPromo[1, k] = Convert.ToSingle(promo.MaxLink);
                tempArrPromo[2, k] = 3;
                tempArrPromo[3, k] = Convert.ToSingle(promo.TotalQty);

                totalQty = 0;
                if (changeQuantity)
                {
                    totalQty =
                        (float)
                        Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                            Convert.ToString(promo.PromoID));
                }
                else
                {
                    totalQty =
                        (float)
                        (saleLine.Quantity +
                         Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                             Convert.ToString(promo.PromoID)));
                }

                tempArrPromo[4, k] = totalQty;
                tempArrPromo[5, k] = Convert.ToSingle(promo.Amount);

                Array.Resize(ref arrPromos, arrPromos.Length - 1 + 1 + 1);
                arrPromos[arrPromos.Length - 1] = Convert.ToString(promo.PromoID);
            }

            // 4 verify that item is in a promo based on dept, subdept and subdetail
            promosForToday.Where(p => p.Dept == dept && p.SubDept == subDept && p.SubDetail == subDetail).ToList();

            if ((promos == null || promos.Count == 0) && !boolItemInPromo)
            {
                boolItemInPromo = false;
            }

            _promoManager.Clear_AllPromos(ref promoItems);
            foreach (var promo in promos)
            {
                promoItems.Add(allPromos.get_Item(promo.PromoID), promo.PromoID);
            }
            foreach (var promo in promos)
            {
                boolItemInPromo = true;

                k = (short)(Information.UBound(tempArrPromo, 2) + 1);
                tempArrPromo =
                    (float[,])
                    Utils.CopyArray(tempArrPromo, new float[6, k + 1]);
                tempArrPromo[1, k] = Convert.ToSingle(promo.MaxLink);
                tempArrPromo[2, k] = 4;

                totalQty = 0;
                if (changeQuantity)
                {
                    totalQty =
                        (float)
                        Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                            Convert.ToString(promo.PromoID));
                }
                else
                {
                    totalQty =
                        (float)
                        (saleLine.Quantity +
                         Get_TotalQuantity(ref saleLine, ref sale, promoItems, (byte)tempArrPromo[1, k],
                             Convert.ToString(promo.PromoID)));
                }

                tempArrPromo[3, k] = Convert.ToSingle(promo.TotalQty);
                tempArrPromo[4, k] = totalQty;
                tempArrPromo[5, k] = Convert.ToSingle(promo.Amount);

                Array.Resize(ref arrPromos, arrPromos.Length - 1 + 1 + 1);
                arrPromos[arrPromos.Length - 1] = Convert.ToString(promo.PromoID);
            }

            // at this point we know the item is in a promotion and we know the level, but we have
            // to make sure all others items in the promotion are already in the sale lines
            // all others posible items in the promotion should be in the CSCCurSale DB
            // in the SaleLine table for this particular sale number
            // go throught all the posible PromoID, sorted by link. Once a link is valid,
            // (there are enought items for that link in the sale) move forward to the next link
            // in the SQL statement exclude the item and the link we alredy know is in the promo
            // A special case are promos with one link that are already valid at this point, so
            // we can set boolValidPromo= True and go further to assign the prices

            if (!boolItemInPromo)
            {
                var procPromoId = saleLine.PromoID;
                // reset the promoID and price for all other items belonging to same promo
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    sl = tempLoopVarSl;
                    if (sl.PromoID == procPromoId && !string.IsNullOrEmpty(sl.PromoID))
                    // added And SL.PromoID <> "" condition on August 06, 2008
                    {
                        sl.PromoID = "";
                        SetPrice(ref sl, sl.Regular_Price);
                        sl.NoPriceFormat = false;
                    }
                }
                if (!string.IsNullOrEmpty(saleLine.PromoID))
                {
                    saleLine.PromoID = "";
                    SetPrice(ref saleLine, saleLine.Regular_Price);
                    saleLine.NoPriceFormat = false;
                }
                return;
            }

            for (i = 1; i <= arrPromos.Length - 1; i++)
            {
                var arrStockCodeToUpdate = new string[1];
                var arrQtyInPromoLine = new float[1];
                var arrTotalQtyLine = new float[1]; // keeps total qty by link, for level 1 is equal with ArrTotalQuantity value
                var arrAmountPromoLine = new float[1];
                var arrTotalQuantity = new float[1]; // keeps total quantity by stock item
                var arrLinkNo = new float[1];
                var arrRegPrice = new float[1];
                // reset the array that keeps regular prices, otherwise it contains extra prices  ' July 25, 2008
                sale.NoOfPromo = 0; // reset the number of promos in the sale when we start to process a new promo
                sumRegPrice = 0; // reset the summary for regular prices of the items in this promo

                // Handle a promo being loaded after the POS was started
                var boolExistingPromo = false;
                foreach (Promo tempLoopVarPr in (IEnumerable)allPromos)
                {
                    pr = tempLoopVarPr;
                    if (pr.PromoID == arrPromos[i])
                    {
                        boolExistingPromo = true;
                        break;
                    }
                }
                if (!boolExistingPromo)
                {
                }
                var objPromo = allPromos.get_Item(arrPromos[i]);

                links = new bool[objPromo.MaxLink + 1];
                var remainingQtyToProcess = new float[objPromo.MaxLink + 1];
                arrQtyInPromoLine = new float[objPromo.MaxLink + 1]; // Apr 28, 2009: changed by link
                arrTotalQtyLine = new float[objPromo.MaxLink + 1]; // Apr 28, 2009: changed by link

                // July 06, 2009 start
                arrQtyInPromoLine[(int)tempArrPromo[1, i]] = tempArrPromo[3, i];
                arrTotalQtyLine[(int)tempArrPromo[1, i]] = tempArrPromo[4, i];
                if (arrTotalQtyLine[(int)tempArrPromo[1, i]] >= arrQtyInPromoLine[(int)tempArrPromo[1, i]])
                {
                    links[(int)tempArrPromo[1, i]] = true;
                }

                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    sl = tempLoopVarSl;
                    boolAddItem = false;
                    foreach (Promo_Line tempLoopVarPl in objPromo.Promo_Lines)
                    {
                        pl = tempLoopVarPl;
                        if (pl.Level == 1)
                        {
                            if (sl.Stock_Code == pl.Stock_Code && (string.IsNullOrEmpty(sl.PromoID) || sl.PromoID == objPromo.PromoID))
                            {
                                boolAddItem = true;
                            }
                        }
                        else if (pl.Level == 2)
                        {
                            if (sl.Dept == pl.Dept && (string.IsNullOrEmpty(sl.PromoID) || sl.PromoID == objPromo.PromoID))
                            {
                                boolAddItem = true;
                            }
                        }
                        else if (pl.Level == 3)
                        {
                            if (sl.Dept == pl.Dept && sl.Sub_Dept == pl.Sub_Dept &&
                                (string.IsNullOrEmpty(sl.PromoID) || sl.PromoID == objPromo.PromoID))
                            {
                                boolAddItem = true;
                            }
                        }
                        else if (pl.Level == 4)
                        {
                            if (sl.Dept == pl.Dept && sl.Sub_Dept == pl.Sub_Dept && sl.Sub_Detail == pl.Sub_Detail &&
                                (string.IsNullOrEmpty(sl.PromoID) || sl.PromoID == objPromo.PromoID))
                            {
                                boolAddItem = true;
                            }
                        }
                        if (boolAddItem)
                        {
                            boolItemExists = false;
                            for (kk = 1; kk <= arrStockCodeToUpdate.Length - 1; kk++)
                            {
                                if (arrStockCodeToUpdate[kk] == sl.Stock_Code)
                                {
                                    boolItemExists = true;
                                    break;
                                }
                            }

                            if (!boolItemExists)
                            {
                                newIndex = (short)(arrStockCodeToUpdate.Length - 1 + 1);
                                Array.Resize(ref arrStockCodeToUpdate, newIndex + 1);
                                arrStockCodeToUpdate[newIndex] = sl.Stock_Code; //  rsPrice!Stock_Code

                                // Regular price to calculate type "P" price promo
                                Array.Resize(ref arrRegPrice, newIndex + 1);
                                arrRegPrice[newIndex] = (float)sl.price; // rsPrice!price

                                // Link number for the sale line
                                Array.Resize(ref arrLinkNo, newIndex + 1);
                                arrLinkNo[newIndex] = pl.Link; // LinkNo ''' rsPromoDetail!Link

                                // Quantity required by this line in promo
                                arrQtyInPromoLine[pl.Link] = pl.Quantity; // rsPromoDetail!Qty

                                // Amount for this promo line for type "B" promos
                                Array.Resize(ref arrAmountPromoLine, newIndex + 1);
                                arrAmountPromoLine[newIndex] = pl.Amount;
                                // IIf(IsNull(rsPromoDetail!Amount), 0, rsPromoDetail!Amount)

                                if (pl.Link == tempArrPromo[1, i] && !changeQuantity)
                                {
                                    arrTotalQtyLine[pl.Link] =
                                        (float)
                                        (saleLine.Quantity +
                                         Get_TotalQuantity(ref saleLine, ref sale, promoItems, pl.Link, objPromo.PromoID));
                                }
                                else
                                {
                                    arrTotalQtyLine[pl.Link] =
                                        (float)Get_TotalQuantity(ref saleLine, ref sale, promoItems, pl.Link, objPromo.PromoID);
                                }
                            }

                            if (arrTotalQtyLine[pl.Link] < arrQtyInPromoLine[pl.Link])
                            {
                                // the link is not valid because there are not enough items in sale
                                // but need to check other links with same number
                                if (!links[pl.Link])
                                {
                                    links[pl.Link] = false;
                                }
                            }
                            else
                            {
                                if (arrTotalQtyLine[pl.Link] == 0 || arrQtyInPromoLine[pl.Link] == 0)
                                {
                                    if (!links[pl.Link])
                                    {
                                        links[pl.Link] = false;
                                    }
                                }
                                else
                                {
                                    links[pl.Link] = true;
                                }
                            }

                        }
                    }
                }
                // July 06, 2009 end

                // July 10, 2009 add the current item being processed
                boolItemExists = false;
                for (kk = 1; kk <= arrStockCodeToUpdate.Length - 1; kk++)
                {
                    if (arrStockCodeToUpdate[kk] == saleLine.Stock_Code)
                    {
                        boolItemExists = true;
                        break;
                    }
                }

                if (!boolItemExists)
                {
                    newIndex = (short)(arrStockCodeToUpdate.Length - 1 + 1);
                    Array.Resize(ref arrStockCodeToUpdate, newIndex + 1);
                    arrStockCodeToUpdate[newIndex] = saleLine.Stock_Code;

                    // Regular price to calculate type "P" price promo
                    Array.Resize(ref arrRegPrice, newIndex + 1);
                    arrRegPrice[newIndex] = (float)saleLine.Regular_Price; // price

                    // Link number for the sale line
                    Array.Resize(ref arrLinkNo, newIndex + 1);
                    arrLinkNo[newIndex] = tempArrPromo[1, i]; // LinkNo

                    // Amount for this promo line for type "B" promos
                    Array.Resize(ref arrAmountPromoLine, newIndex + 1);
                    arrAmountPromoLine[newIndex] = tempArrPromo[5, i];
                }
                // July 10, 2009 end add the current item being processed

                // Validate all the links
                var boolValidPromo = true;
                for (k = 1; k <= links.Length - 1; k++)
                {
                    if (links[k] == false)
                    {
                        boolValidPromo = false;
                    }
                }

                if (boolValidPromo)
                {
                    // Calculate TotalQty_ByStock property for each sale line; has to be done here because is not reliable from Adjust_Lines
                    for (k = 1; k <= arrStockCodeToUpdate.Length - 1; k++)
                    {
                        qtyByStock = 0;
                        Array.Resize(ref arrTotalQuantity, arrTotalQuantity.Length - 1 + 1 + 1);
                        // July 07, 2009; for processed stock code total is already calculated in mvarTotalQty_ByStock variable
                        if (arrStockCodeToUpdate[k] == saleLine.Stock_Code)
                        {
                            arrTotalQuantity[k] = saleLine.TotalQty_ByStock;
                        }
                        else
                        {
                            // July 07, 2009 end
                            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                            {
                                sl = tempLoopVarSl;
                                if (sl.Stock_Code == arrStockCodeToUpdate[k] &&
                                    (string.IsNullOrEmpty(sl.PromoID) || sl.PromoID == objPromo.PromoID) &&
                                    sl.Stock_Code != saleLine.Stock_Code)
                                {
                                    qtyByStock = qtyByStock + sl.Quantity;
                                }
                            }
                            arrTotalQuantity[k] = qtyByStock;
                        } // July 07, 2009
                    }
                    float tempNoOfPromo = 0;

                    // Apr 21, 2009 calculate the number of promos in the sale going through all the links
                    // Has to be done at the beginning of processing because for item one the number of
                    // promo can be 2 but for the second can be 1, so the system tries to assign 2 promos
                    // to the second item and this is wrong
                    // start with the item being processed by sale line (mvarStock_Code)

                    // check all links and keep the lowest number
                    // calculate number of promos in the sale for this PromoID
                    for (k = 1; k <= links.Length - 1; k++)
                    {
                        if (tempNoOfPromo == 0 | tempNoOfPromo > arrTotalQtyLine[k] / arrQtyInPromoLine[k])
                        {
                            tempNoOfPromo = Convert.ToInt32(Math.Floor(arrTotalQtyLine[k] / arrQtyInPromoLine[k]));
                        }
                    }
                    sale.NoOfPromo = tempNoOfPromo;

                    // mvarStock_Code has been included in ArrStockCodeToUpdate array and is processed there

                    // Apr 27, 2009: calculate the RemainingQtyToProcess by link number
                    for (j = 1; j <= arrStockCodeToUpdate.Length - 1; j++)
                    {
                        if (remainingQtyToProcess[(int)arrLinkNo[j]] == 0)
                        {
                            remainingQtyToProcess[(int)arrLinkNo[j]] = sale.NoOfPromo *
                                                                        arrQtyInPromoLine[(int)arrLinkNo[j]];
                        }
                    }
                    // Apr 27, 2009 end

                    var arrScSet = new string[1];
                    arrQtySet = new short[1];

                    // assign the promoID to all stock items that are in this promo
                    for (j = 1; j <= arrStockCodeToUpdate.Length - 1; j++)
                    {
                        if (remainingQtyToProcess[(int)arrLinkNo[j]] <= 0)
                        {
                            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                            {
                                sl = tempLoopVarSl;
                                if (sl.Stock_Code == arrStockCodeToUpdate[j] && sl.PromoID == arrPromos[i])
                                {
                                    sl.PromoID = "";
                                    sl.Price_Type = 'R';
                                    SetPrice(ref sl, arrRegPrice[j]);
                                    sl.NoPriceFormat = false;
                                    sl.QtyForPromo = 0;
                                }
                            }
                            //                    Exit For  ' ??
                        }
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            sl = tempLoopVarSl;
                            if (sl.Stock_Code == arrStockCodeToUpdate[j] &&
                                (string.IsNullOrEmpty(sl.PromoID) || sl.PromoID == arrPromos[i]))
                            {
                                sl.PromoID = arrPromos[i];
                                sl.QtyInPromoLine = arrQtyInPromoLine[(int)arrLinkNo[j]];
                                sl.AmountInPromoLine = arrAmountPromoLine[j];
                                sl.Price_Type = 'R';

                                // July 16, 2009 to keep quantity already set and to set it for all stock codes that are the same in the sale
                                boolItemExists = false;
                                for (kk = 1; kk <= arrScSet.Length - 1; kk++)
                                {
                                    if (arrScSet[kk] == arrStockCodeToUpdate[j])
                                    {
                                        boolItemExists = true;
                                        idxFound = j;
                                        break;
                                    }
                                }
                                if (boolItemExists)
                                {
                                    sl.QtyForPromo = arrQtySet[idxFound];
                                    sl.PromoID = arrPromos[i]; // Aug 18, 2009
                                    if (arrRegPrice[j] != sl.Regular_Price)
                                    {
                                        arrRegPrice[j] = (float)sl.Regular_Price;
                                    }
                                    // don't decrease RemainingQtyToProcess or add regular price to SumRegPrice, only mark items in the promo
                                    //                            SumRegPrice = SumRegPrice + ArrReg_Price(j) * sl.QtyForPromo
                                    //                            RemainingQtyToProcess(ArrLinkNo(j)) = RemainingQtyToProcess(ArrLinkNo(j)) - sl.QtyForPromo
                                }
                                else
                                {
                                    sl.QtyForPromo = Helper.MinThreeValues(remainingQtyToProcess[(int)arrLinkNo[j]],
                                        arrTotalQuantity[j], 999);
                                    if (arrRegPrice[j] != sl.Regular_Price)
                                    {
                                        arrRegPrice[j] = (float)sl.Regular_Price;
                                    }
                                    sumRegPrice = sumRegPrice + arrRegPrice[j] * sl.QtyForPromo;
                                    sl.PromoID = arrPromos[i]; // Aug 18, 2009
                                    remainingQtyToProcess[(int)arrLinkNo[j]] =
                                        remainingQtyToProcess[(int)arrLinkNo[j]] - sl.QtyForPromo;

                                    newIndex = (short)(arrScSet.Length - 1 + 1);
                                    Array.Resize(ref arrScSet, newIndex + 1);
                                    Array.Resize(ref arrQtySet, newIndex + 1);

                                    arrScSet[newIndex] = sl.Stock_Code;
                                    arrQtySet[newIndex] = (short)sl.QtyForPromo;
                                    boolItemExists = true;
                                }
                                // July 16, 2009 end
                                // August 12, 2009 added "If Not ChangeQuantity Then" condition to next line to fix EKO' issue with same item in the second line in the sale and quantity change was not getting the promotion
                                // commented on August 18, 2009 have to go through all the sale lines

                                // go through all items because the same stock item can be in
                                // another line if the policy to combine items is set to "No"
                            }
                        }
                        if (saleLine.Stock_Code == arrStockCodeToUpdate[j] &&
                            (string.IsNullOrEmpty(saleLine.PromoID) || saleLine.PromoID == arrPromos[i]) &&
                            !changeQuantity)
                        {

                            if (remainingQtyToProcess[(int)arrLinkNo[j]] > 0)
                            {
                                boolItemExists = false;
                                for (kk = 1; kk <= arrScSet.Length - 1; kk++)
                                {
                                    if (arrScSet[kk] == arrStockCodeToUpdate[j])
                                    {
                                        boolItemExists = true;
                                        idxFound = j;
                                        break;
                                    }
                                }
                                if (boolItemExists)
                                {
                                    saleLine.QtyForPromo = arrQtySet[idxFound];
                                    saleLine.PromoID = arrPromos[i]; // Aug 18, 2009
                                    saleLine.QtyInPromoLine = arrQtyInPromoLine[(int)arrLinkNo[j]]; // Aug 18, 2009
                                    saleLine.AmountInPromoLine = arrAmountPromoLine[j]; // Oct 14, 2009
                                    if (arrRegPrice[j] != saleLine.Regular_Price)
                                    {
                                        arrRegPrice[j] = (float)saleLine.Regular_Price;
                                    }
                                    saleLine.RefreshPrice = true;
                                }
                                else
                                {
                                    saleLine.PromoID = arrPromos[i];
                                    saleLine.Price_Type = 'R';
                                    saleLine.QtyInPromoLine = arrQtyInPromoLine[(int)arrLinkNo[j]];
                                    saleLine.QtyForPromo = Helper.MinThreeValues(remainingQtyToProcess[(int)arrLinkNo[j]],
                                        arrTotalQuantity[j], 999);
                                    saleLine.AmountInPromoLine = arrAmountPromoLine[j];
                                    if (arrRegPrice[j] != saleLine.Regular_Price)
                                    {
                                        arrRegPrice[j] = (float)saleLine.Regular_Price;
                                    }
                                    sumRegPrice = sumRegPrice + arrRegPrice[j] * saleLine.QtyForPromo;
                                    remainingQtyToProcess[(int)arrLinkNo[j]] =
                                        remainingQtyToProcess[(int)arrLinkNo[j]] - saleLine.QtyForPromo;
                                    // go through all items because the same stock item can be in
                                    // another line if the policy to combine items is set to "No"
                                }
                            }
                            else
                            {
                                // Aug 18, 2009 to search in the Arr_SC_Set for item being processed
                                boolItemExists = false;
                                for (kk = 1; kk <= arrScSet.Length - 1; kk++)
                                {
                                    if (arrScSet[kk] == arrStockCodeToUpdate[j])
                                    {
                                        boolItemExists = true;
                                        idxFound = j;
                                        break;
                                    }
                                }
                                if (boolItemExists)
                                {
                                    saleLine.QtyForPromo = arrQtySet[idxFound];
                                    saleLine.PromoID = arrPromos[i];
                                    saleLine.QtyInPromoLine = arrQtyInPromoLine[(int)arrLinkNo[j]];
                                    saleLine.AmountInPromoLine = arrAmountPromoLine[j];
                                    if (arrRegPrice[j] != saleLine.Regular_Price)
                                    {
                                        arrRegPrice[j] = (float)saleLine.Regular_Price;
                                    }
                                }
                                else
                                {
                                    // Aug 18, 2009 end
                                    if (!string.IsNullOrEmpty(saleLine.PromoID))
                                    {
                                        saleLine.PromoID = "";
                                        saleLine.Price_Type = 'R';
                                        SetPrice(ref saleLine, arrRegPrice[j]);
                                        saleLine.NoPriceFormat = false;
                                    }
                                }
                                //                        Exit For  ' ??
                                saleLine.RefreshPrice = true;
                            }
                        }
                    }

                    // July 22, 2009 this should not happen, but it will prevent crash in Adjust_Lines procedure
                    foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                    {
                        sl = tempLoopVarSl;
                        // SetLevels(ref sl);
                        if (sl.QtyForPromo == 0 && sl.PromoID == objPromo.PromoID)
                        {
                            sl.PromoID = "";
                            sl.Price_Type = 'R';
                            sl.NoPriceFormat = false;
                            SetPrice(ref sl, sl.Regular_Price);
                        }
                    }
                    if (saleLine.QtyForPromo == 0 && saleLine.PromoID == objPromo.PromoID)
                    {
                        saleLine.PromoID = "";
                        saleLine.Price_Type = 'R';
                        saleLine.NoPriceFormat = false;
                        SetPrice(ref saleLine, saleLine.Regular_Price);
                    }
                    // July 22, 2009 end

                    // calculate the price for other stock items that are in this promo
                    foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                    {
                        sl = tempLoopVarSl;
                        // Added the second condition to handle one link promos when sale lines are not combined
                        if (sl.PromoID == objPromo.PromoID)
                        {
                            switch (objPromo.DiscType)
                            {
                                case "$":
                                    sl.Promo_Price =
                                        double.Parse((sl.Regular_Price - objPromo.Amount).ToString("#,##0.00"));
                                    break;
                                case "%":
                                    sl.Promo_Price =
                                        double.Parse((sl.Regular_Price * (1 - objPromo.Amount / 100)).ToString("#,##0.00"));
                                    break;
                                case "P":
                                    if (!(objPromo.MaxLink == 1 && objPromo.MultiLink))
                                    {
                                        if (sumRegPrice != 0)
                                        {
                                            sl.Promo_Price = sl.Regular_Price * sl.QtyInPromoLine / sumRegPrice *
                                                         objPromo.Amount / sl.QtyInPromoLine;
                                            sl.Promo_Price = double.Parse((sl.Promo_Price * sale.NoOfPromo).ToString("#0.0000"));
                                        }
                                    }
                                    else
                                    {
                                        sl.Promo_Price = double.Parse((objPromo.Amount / sl.QtyInPromoLine).ToString("#0.0000"));
                                        // mvarQtyInPromoLine  ''' (SL.Regular_Price * SL.QtyForPromo / SumRegPrice) * objPromo.Amount / SL.QtyForPromo * SA.NoOfPromo
                                    }
                                    break;
                                case "B":
                                    if (objPromo.PrType == "%") // this should be only for new promo set as % type
                                    {
                                        sl.Promo_Price =
                                            double.Parse(
                                                (sl.Regular_Price * (100 - sl.AmountInPromoLine) / 100).ToString("#,##0.00"));
                                    }
                                    else // else handles dollar and NULL PrType records
                                    {
                                        sl.Promo_Price = sl.Regular_Price - sl.AmountInPromoLine;
                                        // took out the format on September 24, 2008
                                    }
                                    break;
                            }

                            if (!sl.ProductIsFuel)
                            {
                                sp = new SP_Prices();
                                //                        If objPromo.DiscType = "P" And objPromo.MaxLink = 1 Then

                                string tempPolicyName = "PRICE_DEC";
                                string tempPolicyName2 = "PRICE_DEC";
                                fs =
                                    Convert.ToString((int)_policyManager.GetPol(tempPolicyName, sl) == 0
                                        ? "0"
                                        : "0." +
                                          new string('0',
                                              Convert.ToInt32(_policyManager.GetPol(tempPolicyName2, sl))));
                                if (
                                    !(sl.QtyForPromo * sl.QtyInPromoLine * double.Parse(sl.Promo_Price.ToString(fs)) ==
                                      objPromo.Amount) &&
                                    (Conversion.Val(sl.Promo_Price.ToString()) !=
                                     Conversion.Val(sl.Promo_Price.ToString(fs))))
                                {
                                    sl.NoPriceFormat = true;
                                }
                                //                        End If
                                sp.Add(1, sl.QtyForPromo, (float)sl.Promo_Price, DateAndTime.Today,
                                    DateAndTime.DateAdd(DateInterval.Year, 1, DateAndTime.Today),
                                    "");
                                sl.SP_Prices = sp;
                            }
                        }
                        if (sl.Stock_Code == saleLine.Stock_Code)
                        {
                            sl.RefreshPrice = true; // Oct 14, 2009
                        }
                        if (sl.HotButton && objPromo.MaxLink == 1 && objPromo.MultiLink)
                        {
                            sl.NoPromo = true;
                            // added on September 12, 2008 to avoid promo being remade from HotButtons screen
                        }
                    }
                    // moved here July 10, 2009
                    // calculate the price for mvarStock_Code that is in promo
                    if (saleLine.PromoID == objPromo.PromoID && saleLine.QtyForPromo > 0)
                    {
                        switch (objPromo.DiscType)
                        {
                            case "$":
                                saleLine.Promo_Price =
                                    double.Parse((saleLine.Regular_Price - objPromo.Amount).ToString("#,##0.00"));
                                break;
                            case "%":
                                saleLine.Promo_Price =
                                    double.Parse(
                                        (saleLine.Regular_Price * (1 - objPromo.Amount / 100)).ToString("#,##0.00"));
                                break;
                            case "P":
                                if (!(objPromo.MaxLink == 1 && objPromo.MultiLink))
                                {
                                    if (sumRegPrice != 0)
                                    {
                                        saleLine.Promo_Price = saleLine.Regular_Price * saleLine.QtyInPromoLine / sumRegPrice *
                                                               objPromo.Amount / saleLine.QtyInPromoLine;
                                        saleLine.Promo_Price = double.Parse((saleLine.Promo_Price * sale.NoOfPromo).ToString("#0.0000"));
                                    }
                                }
                                else
                                {
                                    saleLine.Promo_Price = double.Parse((objPromo.Amount / saleLine.QtyInPromoLine).ToString("#0.0000"));

                                }
                                break;
                            case "B":
                                if (objPromo.PrType == "%") // this should be only for new promo set as % type
                                {
                                    saleLine.Promo_Price =
                                        double.Parse(
                                            (saleLine.Regular_Price * (100 - saleLine.AmountInPromoLine) / 100).ToString(
                                                "#,##0.00"));
                                }
                                else // else handles dollar and NULL PrType records
                                {
                                    saleLine.Promo_Price =
                                        double.Parse(
                                            (saleLine.Regular_Price - saleLine.AmountInPromoLine).ToString("#,##0.00"));
                                }
                                break;
                        }

                        string tempPolicyName3 = "PRICE_DEC";
                        string tempPolicyName4 = "PRICE_DEC";
                        fs =
                            Convert.ToString((int)_policyManager.GetPol(tempPolicyName3, saleLine) == 0
                                ? "0"
                                : "0." +
                                  new string('0',
                                      Convert.ToInt32(_policyManager.GetPol(tempPolicyName4, saleLine))));
                        if (
                            !(saleLine.QtyForPromo * saleLine.QtyInPromoLine *
                              double.Parse(saleLine.Promo_Price.ToString(fs)) == objPromo.Amount) &&
                            Conversion.Val(saleLine.Promo_Price.ToString()) !=
                            Conversion.Val(saleLine.Promo_Price.ToString(fs)))
                        {
                            saleLine.NoPriceFormat = true;
                        }
                        saleLine.RefreshPrice = true; // Oct 14, 2009
                    }
                    // moved here July 10, 2009
                    break;
                }
                else
                {
                    // it is an invalid promo
                    //  
                    if (!string.IsNullOrEmpty(saleLine.PromoID))
                    {
                        // reset the promoID and price for all other items belonging to same promo
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            sl = tempLoopVarSl;
                            if (!string.IsNullOrEmpty(sl.PromoID) && sl.PromoID == objPromo.PromoID)
                            {
                                sl.PromoID = "";
                                SetPrice(ref sl, sl.Regular_Price);
                                sl.NoPriceFormat = false;
                            }
                        }
                        saleLine.PromoID = "";
                        SetPrice(ref saleLine, saleLine.Regular_Price);
                        saleLine.NoPriceFormat = false;
                    }
                }
            }
            Performancelog.Debug($"End,SaleLineManager,Make_Promo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        

        #region Private methods

        /// <summary>
        /// Method to get total quantity
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="sale">Sale</param>
        /// <param name="allPromos">All promos</param>
        /// <param name="link">Link</param>
        /// <param name="promoId">Promo id</param>
        /// <returns>Quantity</returns>
        private double Get_TotalQuantity(ref Sale_Line saleLine, ref Sale sale, Promos allPromos, byte link,
            string promoId = "")
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,Get_TotalQuantity,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            Promo promoRenamed = new Promo();
            double returnValue = 0;
            double totQty = 0;
            if (!string.IsNullOrEmpty(promoId))
            {
                var boolExistingPromo = false;
                foreach (Promo tempLoopVarPro in (IEnumerable)allPromos)
                {
                    var pro = tempLoopVarPro;
                    if (pro.PromoID == promoId)
                    {
                        boolExistingPromo = true;
                        promoRenamed = tempLoopVarPro;
                        break;
                    }
                }
                if (!boolExistingPromo)
                {
                    allPromos = _promoManager.Load_Promos(promoId);
                    promoRenamed = allPromos.get_Item(promoId);
                }
            }

            // PromoID can be empty string only for stock code
            Sale_Line sl;
            if (string.IsNullOrEmpty(promoId))
            {
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    sl = tempLoopVarSl;
                    if (sl.Stock_Code == saleLine.Stock_Code)
                    {
                        totQty = totQty + sl.Quantity;
                    }
                }
                returnValue = totQty;
                return returnValue;
            }

            foreach (Promo_Line tempLoopVarPr in promoRenamed.Promo_Lines)
            {
                var pr = tempLoopVarPr;
                if (pr.Link == link)
                {

                    if (pr.Level == 1)
                    {
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            sl = tempLoopVarSl;
                            if (sl.Stock_Code == pr.Stock_Code && (sl.PromoID == promoId || string.IsNullOrEmpty(sl.PromoID)))
                            {
                                totQty = totQty + sl.Quantity;
                            }
                        }
                    }
                    else if (pr.Level == 2)
                    {
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            sl = tempLoopVarSl;
                            if (sl.Dept == pr.Dept && (sl.PromoID == promoId || string.IsNullOrEmpty(sl.PromoID)))
                            {
                                totQty = totQty + sl.Quantity;
                            }
                        }
                    }
                    else if (pr.Level == 3)
                    {
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            sl = tempLoopVarSl;
                            if (sl.Dept == pr.Dept && sl.Sub_Dept == pr.Sub_Dept &&
                                (sl.PromoID == promoId || string.IsNullOrEmpty(sl.PromoID)))
                            {
                                totQty = totQty + sl.Quantity;
                            }
                        }
                    }
                    else if (pr.Level == 4)
                    {
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            sl = tempLoopVarSl;
                            if (sl.Dept == pr.Dept && sl.Sub_Dept == pr.Sub_Dept && sl.Sub_Detail == pr.Sub_Detail &&
                                (sl.PromoID == promoId || string.IsNullOrEmpty(sl.PromoID)))
                            {
                                totQty = totQty + sl.Quantity;
                            }
                        }
                    }

                }
            }

            //    Select Case Level
            //
            //    Case 1
            //        For Each sl In SA.Sale_Lines
            //            If Len(Trim$(PromoID)) = 0 Then
            //                If sl.Stock_Code = mvarStock_Code Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            Else
            //                If sl.Stock_Code = mvarStock_Code And (sl.PromoID = PromoID Or sl.PromoID = "") Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            End If
            //        Next sl
            //    Case 2
            //        If Promo.MaxLink = 1 And Promo.MultiLink Then
            //            For Each sl In SA.Sale_Lines
            //                If (sl.PromoID = PromoID Or sl.PromoID = "") Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            Next sl
            //        Else
            //            For Each sl In SA.Sale_Lines
            //                If sl.Dept = Me.Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            Next sl
            //        End If
            //
            //    Case 3
            //        If Promo.MaxLink = 1 And Promo.MultiLink Then
            //            For Each sl In SA.Sale_Lines
            //                If sl.Dept = Me.Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            Next sl
            //        Else
            //            For Each sl In SA.Sale_Lines
            //                If sl.Dept = Me.Dept And sl.Sub_Dept = Me.Sub_Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            Next sl
            //        End If
            //
            //    Case 4
            //        If Promo.MaxLink = 1 And Promo.MultiLink Then
            //            For Each sl In SA.Sale_Lines
            //                If sl.Dept = Me.Dept And sl.Sub_Dept = Me.Sub_Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            Next sl
            //        Else
            //            For Each sl In SA.Sale_Lines
            //                If sl.Dept = Me.Dept And sl.Sub_Dept = Me.Sub_Dept And sl.Sub_Detail = Me.Sub_Detail And (sl.PromoID = PromoID Or sl.PromoID = "") Then
            //                    TotQty = TotQty + sl.Quantity
            //                End If
            //            Next sl
            //        End If
            //
            //    End Select

            returnValue = totQty;
            Performancelog.Debug($"End,SaleLineManager,Get_TotalQuantity,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to check whether the stock code is exiting or not
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        private bool IsExistingStockCode(ref string stockCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,IsExistingStockCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //check stock code
            var plu = _stockService.GetPluMast(stockCode);
            if (plu == null)
            {
                return false;
            }
            var stockItem = _stockService.GetStockItem(plu.PLUPrim);

            if (stockItem == null)
            {
                Performancelog.Debug($"End,SaleLineManager,IsExistingStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return false;
            }
            stockCode = plu.PLUPrim;
            Performancelog.Debug($"End,SaleLineManager,IsExistingStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

        /// <summary>
        /// Method to verify if restriction exists
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="tillNumber"></param>
        /// <param name="error">Error message</param>
        /// <param name="saleNumber"></param>
        /// <returns>Restriction page</returns>
        private RestrictionPage VerifyRestriction(User user, Sale_Line saleLine, int saleNumber, int tillNumber, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,VerifyRestriction,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //added for restricted products
            error = new ErrorMessage();
            var restrictionPage = new RestrictionPage
            {
                OpenRestrictionPage = false,
                Description = string.Empty
            };

            //verify restriction on stock code
            var intUAuth = Convert.ToInt16(_policyManager.GetPol("U_AUTH_LEVEL", user));
            var intRestr = Convert.ToInt16(_policyManager.GetPol("RESTR_SALE", saleLine));

            if (intRestr == 0)
            {
                restrictionPage.OpenRestrictionPage = false;
                return restrictionPage;
            }

            if (intRestr > intUAuth)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                // You are not authorized to sell that product.
                MessageType tempVbStyle = (int)MessageType.Information + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 84, null, tempVbStyle);
                error.StatusCode = HttpStatusCode.Unauthorized;
                return null;
            }
            var restriction = _utilityService.ExistsRestriction(intRestr);
            if (restriction.Exist_Restriction) // the code for this restriction was set in Restrictions table
            {
                //check if the same restrition has been previously checked for any other sale line item
                var saleLines = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber) == null ? new Sale_Lines() : CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber).Sale_Lines;
                foreach (Sale_Line line in saleLines)
                {
                    //  SetLevels(ref sl);
                    var code = Convert.ToInt16(_policyManager.GetPol("RESTR_SALE", line));
                    if (code != 0 && code == restriction.Code)
                    {
                        restrictionPage.OpenRestrictionPage = false;
                        return restrictionPage;
                    }
                }
                restrictionPage.Description = restriction.Description;
                restrictionPage.OpenRestrictionPage = true;
            }
            Performancelog.Debug($"End,SaleLineManager,VerifyRestriction,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return restrictionPage;
        }

        /// <summary>
        /// Method to verify gift certificate page
        /// </summary>
        /// <param name="isReturnMode">Return mode or not</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="error">Error</param>
        /// <returns>Gift cert page</returns>
        private GiftCertPage VeriftGiftCertPage(bool isReturnMode, Sale_Line saleLine, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,VeriftGiftCertPage,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            GiftCertPage giftCertPage = new GiftCertPage
            {
                OpenGiftCertPage = false,
                StockCode = string.Empty,
                RegularPrice = "0.00",
            };
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            if (isReturnMode)
            {

                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 55, null, MessageType.OkOnly);
                error.StatusCode = HttpStatusCode.NotAcceptable;
                Performancelog.Debug($"End,SaleLineManager,VeriftGiftCertPage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;
            }



            if (_policyManager.GIFTCERT)
            {
                if (_policyManager.GC_NUMBERS)
                {
                    giftCertPage.OpenGiftCertPage = true;
                    giftCertPage.StockCode = saleLine.Stock_Code;
                    giftCertPage.RegularPrice = saleLine.Regular_Price.ToString("0.00");
                    giftCertPage.GiftNumber = _stockService.GetMaximumGiftNumber();
                }
                Performancelog.Debug($"End,SaleLineManager,VeriftGiftCertPage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return giftCertPage;
            }
            // "You do NOT sell Gift Certificates"
            error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8125, null, MessageType.OkOnly);
            error.StatusCode = HttpStatusCode.Unauthorized;
            Performancelog.Debug($"End,SaleLineManager,VeriftGiftCertPage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null; //   
        }
        private PSInetPage VerifyPSInetPage(bool isReturnMode, Sale_Line saleLine, out ErrorMessage error)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var psinetPage = new PSInetPage
            {
                OpenPSInetPage = false,
                StockCode = string.Empty,
                RegularPrice = "0.00"
            };
            error = new ErrorMessage();
            if (isReturnMode)
            {
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 55, null, MessageType.OkOnly);
                return null;
            }
            if (_policyManager.PSInet)
            {
                psinetPage.OpenPSInetPage = true;
                psinetPage.StockCode = saleLine.Stock_Code;
                psinetPage.RegularPrice = saleLine.Regular_Price.ToString("0.00");
                return psinetPage;
            }
            // "You do NOT sell PSInet products"
            error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8125, null, MessageType.OkOnly);
            error.StatusCode = HttpStatusCode.Unauthorized;
            return null;
        }
        /// <summary>
        /// Method to verify if the stock is for a gift card
        /// </summary>
        /// <param name="isReturnMode">Return mode</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="error">Error</param>
        /// <returns>Giftcard page</returns>
        private GiftCardPage VerifyGiftCardPage(bool isReturnMode, Sale_Line saleLine, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,VerifyGiftCardPage,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var giftCardPage = new GiftCardPage
            {
                OpenGivexPage = false,
                StockCode = string.Empty,
                RegularPrice = "0.00"

            };
            error = new ErrorMessage();
            if (isReturnMode)
            {

                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 55, null, MessageType.OkOnly);
                Performancelog.Debug($"End,SaleLineManager,VerifyGiftCardPage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            if (_policyManager.GIFTCERT)
            {
                giftCardPage.OpenGivexPage = true;
                giftCardPage.StockCode = saleLine.Stock_Code;
                giftCardPage.RegularPrice = saleLine.Regular_Price.ToString("0.00");
                Performancelog.Debug($"End,SaleLineManager,VerifyGiftCardPage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return giftCardPage;
            }
            // "You do NOT sell Gift Certificates"
            error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8125, null, MessageType.OkOnly);
            error.StatusCode = HttpStatusCode.Unauthorized;
            Performancelog.Debug($"End,SaleLineManager,VerifyGiftCardPage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return null; //   
        }

        /// <summary>
        /// Method to confirm quantity if negative quantity is allowed
        /// </summary>
        /// <param name="saleLine">SaleLine</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Message style</returns>
        private MessageStyle VerifyQuantity(Sale_Line saleLine, float quantity)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,VerifyQuantity,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            MessageStyle quantityPrompt = new MessageStyle();
            object[] msgvalue = new object[3];
            var user = _loginManager.GetExistingUser(UserCode);
            if (user.User_Group.Code == Entities.Constants.Trainer) //Behrooz Jan-12-06
            {
                return quantityPrompt;
            }

            if ((saleLine.Stock_Type == 'V' || saleLine.Stock_Type == 'O') && !saleLine.ProductIsFuel) // Only for Tracking type stocks
            {
                if (!_policyManager.GetPol("ALLOC_NEG", saleLine) && quantity > 0)
                {
                    // var stockBr = _stockService.GetStockBr(saleLine.Stock_Code);
                    // if (stockBr != null)
                    {
                        if (Convert.ToInt32(saleLine.AvailItems - quantity) < 0)
                        {
                            msgvalue[1] = saleLine.AvailItems;
                            msgvalue[2] = saleLine.Description;
                            quantityPrompt = _resourceManager.CreateMessage(offSet, 0, 8111, msgvalue, MessageType.YesNo);
                        }
                    }
                    // 
                } //
            }
            Performancelog.Debug($"End,SaleLineManager,VerifyQuantity,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return quantityPrompt;
        }


        /// <summary>
        /// Method to make special prices
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <returns>Special prices</returns>
        private SP_Prices Make_Prices(Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,Make_Prices,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            // No Special pricing for Gift Certificates


            if (saleLine.Stock_Type == 'G')
            {
                return null;
            }
            var pr = MakeItemPrice(saleLine);

            var returnValue = pr;
            Performancelog.Debug($"End,SaleLineManager,Make_Prices,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to make item price
        /// </summary>
        /// <param name="saleLine">SaleLine</param>
        /// <returns>Special price</returns>
        private SP_Prices MakeItemPrice(Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,MakeItemPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            SP_Prices returnValue;

            //###PERFORMAPRIL09### ''Stock.Find "STOCK_CODE='" & Me.Stock_Code & "'", , adSearchForward, 1
            var cStock = saleLine.Stock_Code;
            // Pr_From, Pr_To could be set from PriceL if nothing is set for active vendor, so don't set these variables here
            //    Pr_From = Stock![Pr_From]
            //    Pr_To = Stock![Pr_To]

            var sp = new SP_Prices();

            //  
            if (saleLine.Price_Type == 'R' && !string.IsNullOrEmpty(saleLine.PromoID))
            {
                //        SP.Add 1, Me.QtyInPromoLine * SA.NoOfPromo, Me.Promo_Price, Date, DateAdd("yyyy", 1, Date)
                sp.Add(1, saleLine.QtyForPromo, (float)saleLine.Promo_Price, DateAndTime.Today,
                    DateAndTime.DateAdd(DateInterval.Year, 1, DateAndTime.Today), "");
                returnValue = sp;
                //  

            }
            else if (saleLine.Price_Type != 'R')
            {
                var vendor = saleLine.Vendor;
                var allPrices = _stockService.GetPriceLForRange(cStock);
                var selectedPrices = allPrices.Where(p => p.Vendor == vendor).ToList();
                // if no specific special price was set for the active vendor, look for the price applicable to all vendors
                if (selectedPrices == null || selectedPrices.Count == 0)
                {
                    selectedPrices = allPrices.Where(p => p.Vendor == "ALL").ToList();

                }

                foreach (var price in selectedPrices)
                {
                    var prFrom = price.StartDate ?? DateAndTime.Today;
                    var prTo = price.EndDate ?? DateAndTime.Today;
                    sp.Add(Convert.ToSingle(price.FQuantity),
                     Convert.ToSingle(price.TQuantity ?? 0),
                     Convert.ToSingle(price.Price ?? 0), prFrom, prTo, "");
                }
                returnValue = sp;

            }
            else
            {
                returnValue = null;
            }

            Performancelog.Debug($"End,SaleLineManager,MakeItemPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Set policies at each sale line level
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        public void SetLevelPolicies(ref Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetLevelPolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            saleLine.QUANT_DEC = Convert.ToInt16(_policyManager.GetPol("QUANT_DEC", saleLine));
            saleLine.PRICE_DEC = Convert.ToInt16(_policyManager.GetPol("PRICE_DEC", saleLine));
            saleLine.LOY_EXCLUDE = Convert.ToBoolean(_policyManager.GetPol("LOY-EXCLUDE", saleLine));
            saleLine.I_RIGOR = Convert.ToBoolean(_policyManager.GetPol("I_RIGOR", saleLine));
            saleLine.VOL_POINTS = Convert.ToBoolean(_policyManager.GetPol("VOL_POINTS", saleLine));
            saleLine.LOYAL_PPU = Convert.ToInt16(_policyManager.GetPol("LOYAL_PPU", saleLine));
            saleLine.TE_COLLECTTAX = Convert.ToString(_policyManager.GetPol("TE_COLLECTTAX", saleLine));
            saleLine.LINE_TYPE = Convert.ToString(_policyManager.GetPol("LINE_$_TYPE", saleLine));
            saleLine.ALLOW_QC = _policyManager.GetPol("ALLOW_QC", saleLine);
            saleLine.ALLOW_PC = _policyManager.GetPol("ALLOW_PC", saleLine);
            saleLine.DISC_REASON = _policyManager.GetPol("DISC_REASON", saleLine);
            saleLine.PR_REASON = _policyManager.GetPol("PR_REASON", saleLine);
            saleLine.RET_REASON = _policyManager.GetPol("RET_REASON", saleLine);

            if (saleLine.Stock_Type == 'G')
            {
                saleLine.GiftType = Convert.ToString(_policyManager.GetPol("GiftType", saleLine)); //NONE,LocalGift,GiveX,Milliplein
            }
            else
            {
                saleLine.GiftType = "NONE";
            }


            if (_policyManager.ThirdParty)
            {
                saleLine.ThirdPartyExtractCode = Convert.ToString(_policyManager.GetPol("TrdPtyExt", saleLine)); //3,5,6,7,10
            }



            if (_policyManager.TAX_EXEMPT)
            {
                saleLine.TE_AgeRstr = Convert.ToString(_policyManager.GetPol("TE_AgeRstr", saleLine));
            }
            else
            {
                saleLine.TE_AgeRstr = "false";
            }
            saleLine.Quantity_Decimals = saleLine.QUANT_DEC; // Quantity Decimals
            saleLine.Price_Decimals = saleLine.PRICE_DEC; // Price Decimals
            saleLine.IncludeInLoyalty = !saleLine.LOY_EXCLUDE;
            saleLine.PointsOnVolume = saleLine.VOL_POINTS;
            saleLine.PointsPerDollar = _policyManager.LOYAL_PPD;
            saleLine.PointsPerUnit = saleLine.LOYAL_PPU;
            saleLine.IRigor = saleLine.I_RIGOR; // Recursive application of incremental prices.

            Performancelog.Debug($"End,SaleLineManager,SetLevelPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set kits, charges, taxes
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="errorMessage"></param>
        private void SetPromotionalInformation(ref Sale sale, ref Sale_Line saleLine,
            out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetPromotionalInformation,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            errorMessage = new ErrorMessage();

            if (saleLine.Stock_Type == 'G')
            {
                saleLine.Line_Taxes = new Line_Taxes();
                saleLine.SP_Prices = new SP_Prices();
                saleLine.Line_Kits = new Line_Kits();
                saleLine.Charges = new Charges();
            }
            else
            {
                var security = _policyManager.LoadSecurityInfo();
                if ((Strings.UCase(Convert.ToString(security.BackOfficeVersion)) == "FULL" ||
                     _policyManager.PROMO_SALE) && !saleLine.HotButton)
                {
                    Make_Promo(ref sale, ref saleLine);
                }
                saleLine.SP_Prices = Make_Prices(saleLine);
            }
            Performancelog.Debug($"End,SaleLineManager,SetPromotionalInformation,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set available items
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        private void SetAvailableItems(ref Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetAvailableItems,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockCode = saleLine.Stock_Code;
            var stockBr = _stockService.GetStockBr(stockCode);
            saleLine.Active_StockCode = stockBr == null || Convert.ToBoolean(stockBr.ActiveStock ?? true);
            if (saleLine.Stock_Type == 'V' || saleLine.Stock_Type == 'O') //  
            {
                if (stockBr != null)
                {
                    saleLine.AvailItems = Convert.ToDouble(stockBr.AvalItems ?? 0);
                }
            }
            Performancelog.Debug($"End,SaleLineManager,SetAvailableItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set special price
        /// </summary>
        /// <param name="saleLine"></param>
        private bool IsPriceSet(ref Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,IsPriceSet,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var vendor = saleLine.Vendor;
            var stockCode = saleLine.Stock_Code;
            var priceByCode = _stockService.GetStockPricesByCode(stockCode);
            var price = priceByCode?.FirstOrDefault(p => p.Vendor == vendor);
            var priceLs = _stockService.GetPriceLByCode(stockCode);
            var priceL = priceLs.FirstOrDefault(p => p.Vendor == vendor);
            if (priceL == null)
            {
                // design
                priceL = priceLs.FirstOrDefault(p => p.Vendor == "ALL");

                if (priceL != null)
                {
                    saleLine.Price_Type = priceL.PriceType;
                    saleLine.Price_Units = priceL.PriceUnit;
                }
            }
            else
            {
                saleLine.Price_Type = priceL.PriceType;
                saleLine.Price_Units = priceL.PriceUnit;
            }
            CheckOpenItem(ref saleLine, stockCode); 

            if (!_policyManager.USE_FUEL && saleLine.OpenItem)
            {
            }
            else
            {
                if (price != null)
                {
                    saleLine.Regular_Price = Convert.ToDouble(price.Price);
                }
                else
                {
                    price = priceByCode == null ? null : priceByCode.FirstOrDefault(p => p.Vendor == "ALL");

                    if (price == null)
                    {
                        saleLine.Regular_Price = 0.01; // Default to $0.01
                        return false;
                    }
                    saleLine.Regular_Price = Convert.ToDouble(price.Price);
                }
            }

            Performancelog.Debug($"End,SaleLineManager,IsPriceSet,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }
                

        /// <summary>
        /// Method to set active stock code
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="errorMessage">Error message</param>
        private void SetActiveStockCode(ref Sale_Line saleLine, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,SetActiveStockCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            errorMessage = new ErrorMessage();
            var days = _stockService.IsStockByDayAvailable(saleLine.Stock_Code);
            if (days.Count != 0)
            {
                // processing
                var dayOfWeek =
                    Convert.ToString(DateAndTime.Weekday(DateAndTime.Today));
                if (!days.Contains(dayOfWeek))
                {
                    saleLine.Active_StockCode = true;
                    // to avoid inactive stock item message in the main screen, saleLine item is not added to the sale anyway
                    saleLine.Active_DayOfWeek = false;
                    //PM to fix the issue related to Hot Buttons on August 27, 2012
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    errorMessage = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8890, saleLine.Stock_Code, MessageType.OkOnly),
                        StatusCode = HttpStatusCode.Conflict
                    };
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    // no Else required, if it is found the product can be sold today, continue processing
                }
            }
            else
            {
                saleLine.Active_DayOfWeek = true;
            }
            Performancelog.Debug($"End,SaleLineManager,SetActiveStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to check open items
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="stockCode">Stock code</param>
        private void CheckOpenItem(ref Sale_Line saleLine, string stockCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleLineManager,CheckOpenItem,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            saleLine.OpenItem = _stockService.CheckGroupButton(stockCode);
            Performancelog.Debug($"End,SaleLineManager,CheckOpenItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to map sale line
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="stockCache">Stock cache</param>
        private void MapSaleLine(ref Sale_Line saleLine, Stock stockCache)
        {
            saleLine.Stock_Code = stockCache.StockCode;
            saleLine.Description = stockCache.Description ?? "";
            saleLine.Dept = stockCache.Department;
            saleLine.Sub_Dept = stockCache.SubDepartment;
            saleLine.Sub_Detail = stockCache.SubDetail;
            saleLine.Stock_Type = stockCache.StockType;
            saleLine.Vendor = stockCache.Vendor;
            saleLine.Price_Type = stockCache.PRType;
            saleLine.Price_Units = stockCache.PRUnit;
            saleLine.Vendor = stockCache.Vendor ?? "";
            saleLine.Loyalty_Save = 0;
            saleLine.Prod_Discount_Code = stockCache.ProductDescription;
            saleLine.Stock_BY_Weight = stockCache.SByWeight;
            saleLine.UM = stockCache.UM ?? "";
            saleLine.Cost = stockCache.StandardCost;
            saleLine.Cost = stockCache.AverageCost;
            saleLine.LoyaltyEligible = stockCache.EligibleLoyalty;
            saleLine.FuelRebateEligible = stockCache.EligibleFuelRebate;
            saleLine.FuelRebate = stockCache.FuelRebate;
            saleLine.EligibleTaxRebate = stockCache.EligibletaxRebate;
            saleLine.QualifiedTaxRebate = stockCache.QualtaxRebate;
            saleLine.EligibleTaxEx = stockCache.EligibleTaxExemption;
            saleLine.Rebate = stockCache.Rebate;
            saleLine.Charges = stockCache.Charges;
            saleLine.Line_Kits = stockCache.LineKits;
            saleLine.Line_Taxes = stockCache.LineTaxes;
            saleLine.PluType = stockCache.PLUType;
            saleLine.AvailItems = stockCache.AvailableItems;
            saleLine.Active_StockCode = stockCache.ActiveStock;
            saleLine.Gift_Certificate = stockCache.StockType == 'G' || stockCache.StockType == 'g';
        }


        #endregion
    }
}
