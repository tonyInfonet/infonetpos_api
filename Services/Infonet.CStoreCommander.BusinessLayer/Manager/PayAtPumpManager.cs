using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Linq;


namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PayAtPumpManager : ManagerBase, IPayAtPumpManager
    {
        private readonly IFuelService _fuelService;
        private readonly IPolicyManager _policyManager;
        private readonly ILoginManager _loginManager;
        private readonly ISaleLineManager _saleLineManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fuelService"></param>
        /// <param name="policyManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="saleLineManager"></param>
        public PayAtPumpManager(IFuelService fuelService, IPolicyManager policyManager,
            ILoginManager loginManager, ISaleLineManager saleLineManager)
        {
            _fuelService = fuelService;
            _policyManager = policyManager;
            _loginManager = loginManager;
            _saleLineManager = saleLineManager;
        }

        /// <summary>
        /// Method to set language
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="language">Language</param>
        public void SetLanguage(ref PayAtPump payPump, string language)
        {
            payPump.Language = language;

            if (string.IsNullOrEmpty(language))
            {
                return;
            }
            // Behrooz Oct-08-2002
            var num = (short)((language.Substring(0, 1).ToUpper() == "E") ? 0 : 1);

            payPump.Header = payPump.Header + _fuelService.GetHeaderLine(num);
            payPump.Footer = payPump.Footer + _fuelService.GetFooterLine(num);

        }

        /// <summary>
        /// Method to add a line
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="user">User</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="adjust">Adjust or not</param>
        /// <param name="tableAdjust">Table adjust or not</param>
        /// <param name="forReprint">For reprint or not</param>
        /// <returns>True or false</returns>
        public bool Add_a_Line(ref PayAtPump payPump, User user, Sale_Line oLine,
            bool adjust = false, bool tableAdjust = true, bool forReprint = false)
        {
            bool returnValue = false;
            //

            if (oLine.Quantity == 0)
            {
                return false;
            }

            returnValue = true;

            oLine = Compute_Taxes(ref payPump, oLine, (short)1);

            oLine.User = Convert.ToString(user.Code);
            if (payPump.Sale_Totals.Invoice_Discount != 0)
            {
                Sale_Discount(ref payPump, payPump.Sale_Totals.Invoice_Discount, payPump.Sale_Totals.Invoice_Discount_Type);
            }

            if (adjust)
            {
                if (Adjust_Lines(ref payPump, ref oLine, true))
                {
                    if (!forReprint)
                    {
                        if (oLine.FuelRebateEligible && oLine.FuelRebate > 0 && payPump.Customer.UseFuelRebate && payPump.Customer.UseFuelRebateDiscount) 
                        {

                            _saleLineManager.ApplyFuelRebate(ref oLine);
                        }
                        else
                        {
                            if (oLine.ProductIsFuel && payPump.Customer.GroupID != "" && _policyManager.FuelLoyalty)
                            {
                                if (payPump.Customer.DiscountType == "%" || payPump.Customer.DiscountType == "$")
                                {
                                    _saleLineManager.ApplyFuelLoyalty(ref oLine, payPump.Customer.DiscountType, payPump.Customer.DiscountRate, payPump.Customer.DiscountName); // 
                                }
                            }
                        }
                    }
                    payPump.Sale_Lines.AddLine((short)(payPump.Sale_Lines.Count + 1), oLine, "");
                }
                else
                {
                    returnValue = false;
                }
            }
            else
            {
                if (tableAdjust)
                {
                    if (!forReprint)
                    {
                        if (oLine.FuelRebateEligible && oLine.FuelRebate > 0 && payPump.Customer.UseFuelRebate && payPump.Customer.UseFuelRebateDiscount) 
                        {

                            _saleLineManager.ApplyFuelRebate(ref oLine);
                        }
                        else
                        {
                            if (oLine.ProductIsFuel && payPump.Customer.GroupID != "" && _policyManager.FuelLoyalty)
                            {
                                if (payPump.Customer.DiscountType == "%" || payPump.Customer.DiscountType == "$")
                                {
                                    _saleLineManager.ApplyFuelLoyalty(ref oLine, payPump.Customer.DiscountType, payPump.Customer.DiscountRate, payPump.Customer.DiscountName);
                                }
                            }
                        }
                    }
                    payPump.Sale_Lines.AddLine((short)(payPump.Sale_Lines.Count + 1), oLine, "");
                }
            }

            if (!forReprint)
            {
                ReCompute_Coupon(ref payPump);
            }

            ReCompute_Totals(ref payPump);
            return returnValue;
        }

        #region Private methods

        // Recompute the sale totals
        /// <summary>
        /// Method to recompute totals
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        private void ReCompute_Totals(ref PayAtPump payPump)
        {
            foreach (Sale_Tax tempLoopVarStx in payPump.Sale_Totals.Sale_Taxes)
            {
                var stx = tempLoopVarStx;
                stx.Tax_Included_Total = 0;
                stx.Tax_Included_Amount = 0;
                stx.Taxable_Amount = 0;
                stx.Tax_Added_Amount = 0;
            }

            payPump.Sale_Line_Disc = 0;
            payPump.Sale_Totals.Net = 0;
            payPump.Sale_Totals.Charge = 0;
            foreach (Sale_Line tempLoopVarSl in payPump.Sale_Lines)
            {
                var sl = tempLoopVarSl;
                Compute_Taxes(ref payPump, sl, 1);
            }

        }

        /// <summary>
        /// Method to compute taxes
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="nSign">Signature</param>
        /// <returns>Sale lineS</returns>
        private Sale_Line Compute_Taxes(ref PayAtPump payPump, Sale_Line oLine, short nSign)
        {
            Sale_Tax stx;
            Line_Tax ltx;
            double inclTaxes = 0;
            double netTaxable = 0;
            var key = "";
            var boolComputeTaxes = false;
            double totalInclRates = 0;
            var strKeyLast = "";
            double prevTax = 0;


            boolComputeTaxes = Convert.ToBoolean(_policyManager.TAX_COMP);

            netTaxable = (double)oLine.Net_Amount;

            foreach (Line_Tax tempLoopVarLtx in oLine.Line_Taxes)
            {
                ltx = tempLoopVarLtx;
                if (ltx.Tax_Included)
                {
                    key = ltx.Tax_Name + ltx.Tax_Code;
                    stx = payPump.Sale_Totals.Sale_Taxes[key];
                    if (boolComputeTaxes)
                    {
                        if (totalInclRates == 0)
                        {
                            totalInclRates = totalInclRates + stx.Tax_Rate;
                        }
                        else
                        {
                            totalInclRates = totalInclRates + stx.Tax_Rate + totalInclRates * stx.Tax_Rate / 100;
                        }
                    }
                    else
                    {
                        totalInclRates = totalInclRates + stx.Tax_Rate;
                    }
                    strKeyLast = key;
                }
            }

            if (totalInclRates != 0)
            {
                inclTaxes = Math.Round(netTaxable * (totalInclRates / 100) / (1 + totalInclRates / 100), 2);
            }

            // Apply Taxes to the Sale Items
            foreach (Line_Tax tempLoopVarLtx in oLine.Line_Taxes)
            {
                ltx = tempLoopVarLtx;
                key = ltx.Tax_Name + ltx.Tax_Code;
                stx = payPump.Sale_Totals.Sale_Taxes[key];
                if (ltx.Tax_Included)
                {
                    ltx.Tax_Incl_Total = (float)(oLine.Net_Amount * nSign);
                    if (key == strKeyLast)
                    {
                        
                        if (boolComputeTaxes)
                        {
                            ltx.Tax_Incl_Amount = (float)(Math.Round(((netTaxable - inclTaxes + prevTax) * stx.Tax_Rate / 100) * nSign, 2));
                        }
                        else
                        {
                            ltx.Tax_Incl_Amount = (float)(inclTaxes - prevTax);
                        }
                    }
                    else
                    {
                        if (boolComputeTaxes)
                        {
                            ltx.Tax_Incl_Amount = (float)(Math.Round(((netTaxable - inclTaxes) * stx.Tax_Rate / 100) * nSign, 2));
                        }
                        else
                        {
                            ltx.Tax_Incl_Amount = (float)(Math.Round((inclTaxes * stx.Tax_Rate / totalInclRates) * nSign, 2));
                        }
                        prevTax = prevTax + ltx.Tax_Incl_Amount;
                    }

                    stx.Tax_Included_Total = stx.Tax_Included_Total + (decimal)ltx.Tax_Incl_Total;
                    stx.Tax_Included_Amount = stx.Tax_Included_Amount + (decimal)ltx.Tax_Incl_Amount;

                    if (boolComputeTaxes)
                    {
                        if (key == strKeyLast)
                        {
                            stx.Taxable_Amt_ForIncluded = (decimal)(netTaxable - inclTaxes + prevTax);
                        }
                        else
                        {
                            stx.Taxable_Amt_ForIncluded = (decimal)(netTaxable - inclTaxes);
                        }
                    }
                }
                else
                {
                    ltx.Taxable_Amount = (float)(netTaxable * nSign);
                    ltx.Tax_Added_Amount = (float)(Math.Round(ltx.Taxable_Amount * ltx.Tax_Rate / 100, 2)); // Nicolette added to record taxes for each line in a sale
                    stx.Taxable_Amount = (decimal)(Math.Round((double)stx.Taxable_Amount + (double)netTaxable * nSign, 2)); // 
                    stx.Tax_Added_Amount = (decimal)(Math.Round((double)((float)stx.Taxable_Amount * stx.Tax_Rate / 100), 2));
                    if (boolComputeTaxes)
                    {
                        netTaxable = netTaxable + ltx.Tax_Added_Amount;
                    }

                }
            }

            
            payPump.Sale_Totals.Net = payPump.Sale_Totals.Net + oLine.Net_Amount * nSign;
            var returnValue = oLine;
            return returnValue;
        }

        /// <summary>
        /// Method to apply taxes
        /// </summary>
        /// <param name="payAtPump">Pay at pump</param>
        /// <param name="applyTax">Apply tax or not</param>
        private void ApplyTaxes(ref PayAtPump payAtPump, bool applyTax)
        {
            var sl = default(Sale_Line);
            if (applyTax)
            {
                if (payAtPump.ApplyTaxes)
                {
                    return;
                }
                payAtPump.ApplyTaxes = applyTax;
                // Changing from FALSE to TRUE ... Add the taxes back in.
                foreach (Sale_Line tempLoopVarSl in payAtPump.Sale_Lines)
                {
                    sl = tempLoopVarSl;
                    Compute_Taxes(ref payAtPump, sl, 1);
                }
            }
            else
            {
                if (!payAtPump.ApplyTaxes)
                {
                    return;
                }
                payAtPump.ApplyTaxes = applyTax;
                // Changing from TRUE to FALSE - Remove the taxes
                foreach (Sale_Line tempLoopVarSl in payAtPump.Sale_Lines)
                {
                    sl = tempLoopVarSl;
                    Compute_Taxes(ref payAtPump, sl, 1);
                }
            }

            ReCompute_Totals(ref payAtPump);

        }

        // Set the price on a line.
        /// <summary>
        /// Method to set price on line
        /// </summary>
        /// <param name="payPump">pay at pump</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="price">Price</param>
        private void Line_Price(ref PayAtPump payPump, ref Sale_Line oLine, double price)
        {
            Compute_Taxes(ref payPump, oLine, -1);
            oLine.price = price;
            Compute_Taxes(ref payPump, oLine, 1);

            if (payPump.Sale_Totals.Invoice_Discount != 0)
            {
                Sale_Discount(ref payPump, payPump.Sale_Totals.Invoice_Discount, payPump.Sale_Totals.Invoice_Discount_Type);
            }
        }


        /// <summary>
        /// Method to set quantity
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="adjust">Adjust or not</param>
        /// <returns>True or false</returns>
        private void Line_Quantity(ref PayAtPump payPump, ref Sale_Line oLine, float quantity, bool adjust = true)
        {
            Compute_Taxes(ref payPump, oLine, -1);
            oLine.Quantity = quantity;
            if (adjust)
            {
                if (!Adjust_Lines(ref payPump, ref oLine, false))
                {
                    ReCompute_Totals(ref payPump);
                }
                else
                {
                    Compute_Taxes(ref payPump, oLine, 1);
                }
            }

            if (payPump.Sale_Totals.Invoice_Discount != 0)
            {
                Sale_Discount(ref payPump, payPump.Sale_Totals.Invoice_Discount,
                    payPump.Sale_Totals.Invoice_Discount_Type);
            }
        }

        /// <summary>
        /// Method to set the discount
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="discount">Discount</param>
        /// <param name="discType">DIscount type</param>
        private void Sale_Discount(ref PayAtPump payPump, decimal discount, string
            discType)
        {
            double discPer = 0;

            if (discType == "$")
            {
                if (payPump.Sale_Totals.Net != 0) //  - Division by zero
                {
                    discPer = (double)(discount / payPump.Sale_Totals.Net);
                } //Change ends
                payPump.Sale_Totals.Invoice_Discount = discount;
            }
            else
            {
                discPer = (double)(discount / 100);
                payPump.Sale_Totals.Invoice_Discount = (decimal)discPer * payPump.Sale_Totals.Net;
            }
            payPump.Sale_Totals.Invoice_Discount_Type = discType;

            foreach (Sale_Line tempLoopVarSl in payPump.Sale_Lines)
            {
                var sl = tempLoopVarSl;
                Compute_Taxes(ref payPump, sl, -1);
                sl.Discount_Adjust = (double)((decimal)discPer * sl.Amount);
                Compute_Taxes(ref payPump, sl, 1);
            }
            ReCompute_Totals(ref payPump);
        }

        /// <summary>
        /// Method to recompute coupon
        /// </summary>
        /// <param name="payAtPump">Pay at pump</param>
        private void ReCompute_Coupon(ref PayAtPump payAtPump)
        {
            if (payAtPump.Customer.GroupID == "" || payAtPump.Customer.DiscountType != "C" || (!_policyManager.FuelLoyalty))
            {
                return;
            }

            payAtPump.CouponTotal = 0;
            foreach (Sale_Line tempLoopVarSl in payAtPump.Sale_Lines)
            {
                var sl = tempLoopVarSl;
                if (sl.ProductIsFuel)
                {
                    payAtPump.CouponTotal = (decimal)((float)payAtPump.CouponTotal + sl.Quantity * payAtPump.Customer.DiscountRate);
                }
            }

            payAtPump.CouponTotal = decimal.Parse(payAtPump.CouponTotal.ToString("##0.00"));
            
            if (payAtPump.CouponTotal > 0 && string.IsNullOrEmpty(payAtPump.CouponID))
            {
                payAtPump.CouponID = GetCouponId();
            }
        }

        /// <summary>
        /// Method to get coupon id
        /// </summary>
        /// <returns></returns>
        private string GetCouponId()
        {
            string strPrefix = "";
            string strCoupon = "";

            bool found = false;

            
            strPrefix = DateAndTime.Today.ToString("MMddyy") + "100"; //Date + TillID
            found = false;
            while (!found)
            {
                var strNum = Conversion.Int(29999 * VBMath.Rnd()).ToString("00000");
                strCoupon = strPrefix + strNum.Trim();
                if (_fuelService.IsExistingCoupon(strCoupon))
                {
                    found = true;
                    break;
                }
                strNum = "";
            }
            var returnValue = strCoupon;
            return returnValue;
        }

        /// <summary>
        /// Method to get quantity
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        internal float[,] PQuantity(Sale_Line sl, float quantity)
        {
            //float[,] returnValue = default(float[,]);

            float[,] prices = new float[51, 4];

            float[,] pHold = new float[51, 4];

            const short qty = 1;
            const short pri = 2;
            const short amt = 3;

            short n;

            var priceType = '\0';
            short m = 0;

            var ns = (short)(quantity < 0 ? -1 : 1);
            quantity = Math.Abs((short)quantity);

            var regPrice = (decimal)sl.Regular_Price;
            priceType = sl.Price_Type;
            var priceUnits = sl.Price_Units;

            for (n = 1; n <= Information.UBound(prices, qty); n++)
            {
                prices[n, qty] = 0;
                prices[n, pri] = 0;
                prices[n, amt] = 0;
                pHold[n, qty] = 0;
                pHold[n, pri] = 0;
                pHold[n, amt] = 0;
            }

            if (sl.Price_Number != 1)
            {
                // If the price number isn't 1 then none of the special pricing applies.
                prices[1, qty] = quantity * ns;
                prices[1, pri] = (float)sl.price;
                prices[1, amt] = (float)(quantity * sl.price * ns);
                m = (short)1;
            }
            else
            {
                var spPrices = _fuelService.GetPriceL(sl.Stock_Code);
                if (spPrices.Count == 0)
                {
                    // No Special Pricing defined for this item.
                    prices[1, qty] = quantity * ns;
                    prices[1, pri] = (float)regPrice;
                    prices[1, amt] = quantity * (float)regPrice * ns;
                    m = (short)1;
                }
                else
                {
                    switch (priceType)
                    {

                        case 'R':
                            // Regular Price
                            prices[1, qty] = quantity * ns;
                            prices[1, pri] = (float)regPrice;
                            prices[1, amt] = quantity * (float)regPrice * ns;
                            m = (short)1;
                            break;

                        
                        
                        case 'F':
                            // First Unit Pricing
                            spPrices.Reverse();
                            foreach (var spPrice in spPrices)
                            {
                                if (spPrice.FQuantity <= quantity)
                                {
                                    prices[1, qty] = quantity * ns;
                                    prices[1, pri] = priceUnits == '%' ? (float)(regPrice * (1 - Convert.ToInt32(Convert.ToInt32(spPrice.Price.Value) / 100))) : (spPrice.Price.Value);
                                    prices[1, amt] = quantity * prices[1, pri] * ns;
                                    break;
                                }
                            }
                            if (prices[1, qty] == 0)
                            {
                                prices[1, qty] = quantity * ns;
                                prices[1, pri] = (float)regPrice;
                                prices[1, amt] = quantity * (float)regPrice * ns;
                            }
                            m = (short)1;
                            break;

                        case 'X':

                            float RP_Qty = 0;
                            if (_policyManager.X_RIGOR)
                            {
                                // "X" for Pricing - RIGOROUS
                                var firstXPrice = spPrices.FirstOrDefault();
                                RP_Qty = Convert.ToSingle(firstXPrice.FQuantity - 1);
                                m = (short)0;
                                // spPrices.Reverse();
                                int xIndex = spPrices.Count - 1;
                                if (quantity > RP_Qty)
                                {
                                    while (!(xIndex != 0 || quantity <= RP_Qty))
                                    {
                                        if (spPrices[xIndex].FQuantity <= quantity)
                                        {
                                            m++;
                                            prices[m, qty] = Convert.ToSingle(Convert.ToInt32(spPrices[xIndex].FQuantity) * ns);
                                            prices[m, pri] = priceUnits == '%' ? (float)(regPrice * (1 - Convert.ToInt32(Convert.ToInt32(spPrices[xIndex].Price.Value) / 100))) : (spPrices[xIndex].Price.Value);
                                            prices[m, amt] = prices[m, pri] * ns;
                                            quantity = Convert.ToSingle(quantity - spPrices[xIndex].FQuantity);
                                            if (spPrices[xIndex].FQuantity > quantity)
                                            {
                                                xIndex--;
                                            }
                                        }
                                        else
                                        {
                                            xIndex--;
                                        }
                                    }
                                }

                                if (RP_Qty > 0 & quantity > 0)
                                {
                                    m++;
                                    prices[m, qty] = (quantity >= RP_Qty ? RP_Qty : quantity) * ns;
                                    prices[m, pri] = (float)regPrice;
                                    prices[m, amt] = prices[m, qty] * (float)regPrice;
                                }
                            }
                            else
                            {
                                // X-for Pricing - NON-RIGOROUS
                                m = (short)0;
                                var lastXPrice = spPrices.LastOrDefault();
                                RP_Qty = Convert.ToSingle(lastXPrice.FQuantity - 1);
                                prices[1, qty] = 1;
                                prices[1, pri] = (float)regPrice;
                                prices[1, amt] = (float)regPrice;
                                int index = spPrices.Count - 1;
                                // Find the best pricing available
                                while (index != 0)
                                {
                                    if (spPrices[index].FQuantity <= quantity)
                                    {
                                        m++;
                                        prices[m, qty] = Convert.ToSingle(spPrices[index].FQuantity * ns);
                                        prices[m, pri] = priceUnits == '%' ? (float)(regPrice * (1 - Convert.ToInt32(Convert.ToInt32(spPrices[index].Price.Value) / 100))) : (spPrices[index].Price.Value);
                                        prices[m, amt] = prices[m, pri] * ns;
                                        break;
                                    }
                                    index--;
                                }

                                var remainingQty = quantity;
                                if (m > 0)
                                {
                                    remainingQty = quantity - prices[1, qty];
                                    while (remainingQty >= prices[1, qty])
                                    {
                                        m++;
                                        prices[m, qty] = prices[1, qty];
                                        prices[m, pri] = prices[1, pri];
                                        prices[m, amt] = prices[1, amt];
                                        remainingQty = remainingQty - prices[1, qty];
                                    }
                                }

                                if (remainingQty > 0)
                                {
                                    m++;
                                    // Compute the best price for the remaining units.
                                    var q = (prices[1, pri] / prices[1, qty]).ToString("0.000");
                                    float bestPrice;
                                    if (q.Substring(q.Length - 1, 1) == "0")
                                    {
                                        bestPrice = (float)(Math.Round(prices[1, pri] / prices[1, qty], 2));
                                    }
                                    else
                                    {
                                        q = q.Substring(0, q.Length - 1);
                                        bestPrice = (float)(Conversion.Val(q) + 0.01);
                                    }

                                    prices[m, qty] = remainingQty * ns;
                                    prices[m, pri] = bestPrice;
                                    prices[m, amt] = prices[m, qty] * prices[m, pri];
                                }
                            }
                            break;
                        

                        case 'S':
                            // Sale Pricing
                            prices[1, qty] = quantity * ns;
                            prices[1, pri] = priceUnits == '%' ? (float)(regPrice * (1 - Convert.ToInt32(Convert.ToInt32(spPrices.FirstOrDefault().Price.Value) / 100))) : (spPrices.FirstOrDefault().Price.Value);
                            prices[1, amt] = prices[1, qty] * prices[1, pri] * ns;
                            m = 1;
                            break;

                        
                        
                        case 'I':
                            // Incremental Pricing

                            m = 0;
                            var firstPrice = spPrices.FirstOrDefault();
                            // Sell anything below the first quantity at regular price
                            short qtyInRange = 0;
                            if (quantity < firstPrice.FQuantity)
                            {
                                qtyInRange = (short)quantity;
                                m++;
                                prices[m, qty] = qtyInRange * ns;
                                prices[m, pri] = (float)regPrice;
                                prices[m, amt] = qtyInRange * prices[m, pri] * ns;
                                quantity = quantity - qtyInRange;
                            }

                            var lastPrice = spPrices.LastOrDefault();
                            // Sell anything above the upper limit at regular price.
                            if (lastPrice != null && quantity > lastPrice.TQuantity)
                            {
                                qtyInRange = Convert.ToInt16(quantity - lastPrice.TQuantity);
                                m++;
                                prices[m, qty] = qtyInRange * ns;
                                prices[m, pri] = (float)regPrice;
                                prices[m, amt] = qtyInRange * prices[m, pri] * ns;
                                quantity = quantity - qtyInRange;
                            }
                            int iIndex = spPrices.Count - 1;
                            // Apply the ranges
                            while (iIndex != 0)
                            {
                                qtyInRange = (short)(Convert.ToInt32(quantity - spPrices[iIndex].FQuantity) + 1);
                                if (qtyInRange > Convert.ToInt32(Convert.ToDouble(spPrices[iIndex].TQuantity) - Convert.ToDouble(spPrices[iIndex].FQuantity) + 1))
                                {
                                    qtyInRange = Convert.ToInt16(Convert.ToDouble(Convert.ToInt32(spPrices[iIndex].TQuantity) - Convert.ToDouble(spPrices[iIndex].FQuantity)) + 1);
                                }
                                if (qtyInRange > 0)
                                {
                                    m++;
                                    prices[m, qty] = qtyInRange * ns;
                                    prices[m, pri] = priceUnits == '%' ? (float)(regPrice * (1 - Convert.ToInt32(Convert.ToInt32(spPrices[iIndex].Price.Value) / 100))) : (spPrices[iIndex].Price.Value);
                                    prices[m, amt] = qtyInRange * prices[m, pri] * ns;
                                    quantity = quantity - qtyInRange;
                                    if (spPrices[iIndex].FQuantity > quantity)
                                    {
                                        iIndex--;
                                    }
                                }
                                else
                                {
                                    iIndex--;
                                }
                            }

                            if (quantity > 0)
                            {
                                m++;
                                prices[m, qty] = quantity * ns;
                                prices[m, pri] = (float)regPrice;
                                prices[m, amt] = prices[m, qty] * (float)regPrice;
                            }
                            break;
                            
                    }
                }
            }

            // Reverse the order for cosmetic reasons
            for (n = 1; n <= m; n++)
            {
                var I = (short)(m - n + 1);
                pHold[I, qty] = prices[n, qty];
                pHold[I, pri] = prices[n, pri];
                pHold[I, amt] = prices[n, amt];
            }

            Array.Copy(pHold, prices, pHold.Length);
            return prices;
        }

        /// <summary>
        /// Method to adjust lines
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="thisLine">Sale line</param>
        /// <param name="newLine">New line</param>
        /// <param name="remove">Remove or not</param>
        /// <returns>True or false</returns>
        internal bool Adjust_Lines(ref PayAtPump payPump, ref Sale_Line thisLine, bool
            newLine, bool remove = false)
        {
            bool returnValue = false;

            // Get the pricing for that many products
            float[,] spr = null;
            Sale_Line ss = default(Sale_Line);
            float saleQty = 0;
            short saleLin = 0;
            Sale_Line New_Line = default(Sale_Line);
            short n = 0;
            short m;
            short nDel = 0;
            bool Fd;
            bool Can_Combine = false;
            bool Combined = false;
            short Lines_Needed = 0;
            short ns;
            short nL;
            bool Combine_Policy = false;

            
            

            Combine_Policy = Convert.ToBoolean(_policyManager.COMBINE_LINE);
            

            returnValue = true;

            // If the incoming line is New then include it's quantity in the count. If it isn't
            // then the quantity will be included in the following loop.
            saleQty = Convert.ToSingle(newLine ? thisLine.Quantity : 0);
            if (remove)
            {
                saleQty = Convert.ToSingle(-thisLine.Quantity);
            }
            saleLin = Convert.ToInt16(newLine ? 1 : 0);

            // Compute the total quantity of the item in the sale and the number of lines
            // on which it appears.
            foreach (Sale_Line tempLoopVarSLine in payPump.Sale_Lines)
            {
                var sLine = tempLoopVarSLine;
                if (thisLine.Stock_Code != sLine.Stock_Code) continue;
                saleQty = saleQty + sLine.Quantity;
                saleLin++;
            }
            ns = (short)(saleQty < 0 ? -1 : 1);

            // Call 'PQuantity' to build the array of prices and quantities to be used for this
            // item.


            spr = PQuantity(thisLine, saleQty);
            saleQty = Math.Abs((short)saleQty);

            if (thisLine.Price_Number > 1)
            {

                // If Price_Number > 1 then it is NOT regular price.
                if (newLine)
                {
                    foreach (Sale_Line tempLoopVarSs in payPump.Sale_Lines)
                    {
                        ss = tempLoopVarSs;
                        if (ss.Stock_Code == thisLine.Stock_Code)
                        {
                            string temp_Policy_Name = "COMBINE_LINE";
                            Can_Combine = Combine_Policy && _policyManager.GetPol(temp_Policy_Name, thisLine) && (!thisLine.Gift_Certificate) && ss.price == thisLine.price & ss.Quantity > 0 & thisLine.Quantity > 0 & ss.Discount_Rate == thisLine.Discount_Rate && ss.Serial_No == "" && thisLine.Serial_No == "" && ss.Discount_Type == thisLine.Discount_Type;
                            

                            if (!Can_Combine) continue;
                            ss.Quantity = ss.Quantity + thisLine.Quantity;
                            ss.Amount = (decimal)(ss.Quantity * ss.price);
                            returnValue = false;
                            break;
                        }
                    }
                }
                else
                {
                    thisLine.Amount = (decimal)(thisLine.Quantity * thisLine.price);
                }

            }
            else
            {
                // Price_Code = 1 means that this is based on regular price.
                if (thisLine.Price_Type == 'R')
                {
                    if (newLine)
                    {
                        foreach (Sale_Line tempLoopVarSs in payPump.Sale_Lines)
                        {
                            ss = tempLoopVarSs;
                            if (ss.Stock_Code == thisLine.Stock_Code)
                            {
                                var temp_Policy_Name2 = "COMBINE_LINE";
                                Can_Combine = Combine_Policy && _policyManager.GetPol(temp_Policy_Name2, thisLine) && (!thisLine.Gift_Certificate) && ss.price == thisLine.price & ss.Quantity > 0 & thisLine.Quantity > 0 & ss.Discount_Rate == thisLine.Discount_Rate && ss.Serial_No == "" && thisLine.Serial_No == "" && ss.Discount_Type == thisLine.Discount_Type;
                                

                                if (!Can_Combine) continue;
                                ss.Quantity = ss.Quantity + thisLine.Quantity;
                                ss.Amount = (decimal)(ss.Quantity * ss.price);
                                returnValue = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        thisLine.Amount = (decimal)(thisLine.Quantity * thisLine.price);
                    }


                    
                    
                }
                else if (thisLine.Price_Type == 'F')
                {
                    // "F" - First Unit Pricing
                    if (newLine)
                    {
                        foreach (Sale_Line tempLoopVarSs in payPump.Sale_Lines)
                        {
                            ss = tempLoopVarSs;
                            // Assign the computed prices to each item in the sale.
                            if (ss.Stock_Code == thisLine.Stock_Code)
                            {

                                Line_Price(ref payPump, ref ss, (double)(Convert.ToDecimal(spr[1, 2])));
                                ss.Amount = (decimal)(ss.Quantity * ss.price);
                            }
                        }

                        foreach (Sale_Line tempLoopVarSs in payPump.Sale_Lines)
                        {
                            ss = tempLoopVarSs;

                            if (ss.Stock_Code == thisLine.Stock_Code)
                            {
                                var temp_Policy_Name3 = "COMBINE_LINE";
                                Can_Combine = Combine_Policy && _policyManager.GetPol(temp_Policy_Name3, thisLine) && (!thisLine.Gift_Certificate) && (decimal)ss.price == Convert.ToDecimal(spr[1, 2]) && ss.Quantity > 0 & thisLine.Quantity > 0 & ss.Discount_Rate == thisLine.Discount_Rate && ss.Serial_No == "" && thisLine.Serial_No == "" && ss.Discount_Type == thisLine.Discount_Type;

                                Line_Price(ref payPump, ref ss, (double)(Convert.ToDecimal(spr[1, 2])));
                                if (Can_Combine && !Combined)
                                {
                                    Line_Quantity(ref payPump, ref ss, ss.Quantity + thisLine.Quantity, false);
                                    Combined = true;
                                }
                                ss.Amount = (decimal)(ss.Quantity * ss.price);
                            }
                        }

                        if (Combined)
                        {
                            returnValue = false;
                        }
                        else
                        {

                            thisLine.price = (double)(Convert.ToDecimal(spr[1, 2]));
                            thisLine.Amount = (decimal)(thisLine.Quantity * thisLine.price);
                        }
                    }
                    else
                    {

                        if (thisLine.Price_Number == 1)
                        {
                            thisLine.price = (double)(Convert.ToDecimal(spr[1, 2]));
                        }
                        thisLine.Amount = (decimal)(thisLine.Quantity * thisLine.price);
                    }
                    

                }
                else if (thisLine.Price_Type == 'S')
                {
                    // "S" - Sale Pricing
                    if (newLine)
                    {
                        Combined = false;
                        foreach (Sale_Line tempLoopVarSs in payPump.Sale_Lines)
                        {
                            ss = tempLoopVarSs;

                            if (ss.Stock_Code == thisLine.Stock_Code)
                            {
                                var temp_Policy_Name4 = "COMBINE_LINE";
                                Can_Combine = Combine_Policy && _policyManager.GetPol(temp_Policy_Name4, thisLine) && (!thisLine.Gift_Certificate) && (decimal)ss.price == Convert.ToDecimal(spr[1, 2]) && ss.Quantity > 0 & thisLine.Quantity > 0 & ss.Discount_Rate == thisLine.Discount_Rate && ss.Serial_No == "" && thisLine.Serial_No == "" && ss.Discount_Type == thisLine.Discount_Type;
                                


                                Line_Price(ref payPump, ref ss, (double)(Convert.ToDecimal(spr[1, 2])));
                                if (Can_Combine && !Combined)
                                {
                                    Line_Quantity(ref payPump, ref ss, ss.Quantity + thisLine.Quantity, false);
                                    Combined = true;
                                }
                                ss.Amount = (decimal)(ss.Quantity * ss.price);
                            }
                        }

                        if (Combined)
                        {
                            returnValue = false;
                        }
                        else
                        {

                            thisLine.price = (double)(Convert.ToDecimal(spr[1, 2]));
                            thisLine.Amount = (decimal)(thisLine.Quantity * thisLine.price);
                        }
                    }
                    else
                    {

                        if (thisLine.Price_Number == 1)
                        {
                            thisLine.price = (double)(Convert.ToDecimal(spr[1, 2]));
                        }
                        thisLine.Amount = (decimal)(thisLine.Quantity * thisLine.price);
                    }

                    
                    
                }
                else if (thisLine.Price_Type == 'X' || thisLine.Price_Type == 'I')
                {
                    // "X" - X for Pricing;      "I" - Incremental Pricing

                    // Compute how many lines are needed by counting the number of prices
                    // that the 'PQuantity' routine set.
                    Lines_Needed = (short)0;
                    for (n = 1; n <= (spr.Length - 1); n++)
                    {

                        if (spr[n, 1] != 0)
                        {
                            Lines_Needed++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    nDel = (short)0;

                    // Remove Lines if there are more than we need ...
                    if (Lines_Needed < saleLin)
                    {
                        if (newLine)
                        {
                            // Don't add the incoming line. That reduces the number we need to
                            // delete by 1.
                            nDel = (short)1;
                        }
                        if (nDel < saleLin - Lines_Needed)
                        {
                            foreach (Sale_Line tempLoopVarSs in payPump.Sale_Lines)
                            {
                                ss = tempLoopVarSs;
                                if (ss.Stock_Code == thisLine.Stock_Code)
                                {
                                    //Me.Remove_a_Line SS.Line_Num, False
                                    nDel++;
                                    if (nDel == saleLin - Lines_Needed)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        // Add lines if there are not enough.
                    }
                    else if (Lines_Needed > saleLin)
                    {
                        for (n = saleLin; n <= Lines_Needed - 1; n++)
                        {
                            nDel++;
                            New_Line = new Sale_Line
                            {
                                PLU_Code = thisLine.PLU_Code,
                                Discount_Type = thisLine.Discount_Type,
                                Discount_Rate = thisLine.Discount_Rate,
                                Line_Num = (short)(payPump.Sale_Lines.Count + 1)
                            };
                            var user = _loginManager.GetExistingUser(payPump.UserCode);
                            Add_a_Line(ref payPump, user, New_Line, false);
                        }
                    }

                    // Set the pricing on each line
                    n = (short)0;
                    foreach (Sale_Line tempLoopVarSs in payPump.Sale_Lines)
                    {
                        ss = tempLoopVarSs;
                        if (ss.Stock_Code == thisLine.Stock_Code)
                        {
                            n++;

                            Line_Price(ref payPump, ref ss, (double)(Convert.ToDecimal(spr[n, 2])));
                            Line_Quantity(ref payPump, ref ss, Convert.ToInt16(spr[n, 1]), false);
                            ss.Amount = Convert.ToDecimal(spr[n, 3]);
                            saleQty = saleQty - System.Math.Abs(Convert.ToInt16(spr[n, 1]));
                        }
                    }

                    // Set the quantity on the new line
                    if (saleQty != 0)
                    {
                        n++;
                        thisLine.Quantity = saleQty;

                        thisLine.price = (double)(Convert.ToDecimal(spr[n, 2]));

                        thisLine.Amount = Convert.ToDecimal(spr[n, 3]);
                        returnValue = true;
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
            }

            return returnValue;
        }

        #endregion

    }
}
