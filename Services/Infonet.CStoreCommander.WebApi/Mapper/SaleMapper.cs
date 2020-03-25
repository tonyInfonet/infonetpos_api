using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Models.Sale;

namespace Infonet.CStoreCommander.WebApi.Mapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class SaleMapper
    {
        private static readonly IApiResourceManager ResourceManager = new ApiResourceManager();
        /// <summary>
        /// Method to create sale model
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="editLines"></param>
        /// <param name="enableExactExchange">Enable exact exchange button or not</param>
        /// <param name="writeOff">Write off</param>
        /// <param name="message">Message</param>
        /// <returns>Sale model</returns>
        public static SaleModel CreateSaleModel(Sale sale, List<SaleLineEdit> editLines,
            bool enableExactExchange, bool writeOff, List<object> message = null)
        {
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;

            sale.HasCarwashProducts = false;
            foreach (Sale_Line sl in sale.Sale_Lines)
            {
                if (sl.IsCarwashProduct)
                {
                    sale.HasCarwashProducts = true;
                }
            }

            var saleModel = new SaleModel
            {
                TillNumber = sale.TillNumber,
                SaleNumber = sale.Sale_Num,
                Customer = sale.Customer.Name,
                EnableExactChange = enableExactExchange,
                TotalAmount = sale.Sale_Totals.TotalLabel,
                Summary = sale.Sale_Totals.SummaryLabel,
                SaleLineErrors = message,
                HasCarwashProducts = sale.HasCarwashProducts
            };

            var writeOffSaleLine = true;
            foreach (Sale_Line saleLine in sale.Sale_Lines)
            {
                var lineEdit = editLines.FirstOrDefault(e => e.LineNumber == saleLine.Line_Num);
                var newSaleLine = new SaleLine
                {
                    LineNumber = saleLine.Line_Num,
                    StockCode = saleLine.Stock_Code
                };
                //Tony 03/19/2019
                newSaleLine.Dept = saleLine.Dept;
                if (saleLine.ProductIsFuel)
                {
                    //shiny end
                    if (saleLine.IsPropane)
                    {
                        //shiny end
                        if (saleLine.IsPropane)
                        {
                            newSaleLine.Description = saleLine.Description + " " + ResourceManager.GetResString(371, offSet);
                        }
                        else
                        {
                            newSaleLine.Description = saleLine.Description + " " + ResourceManager.GetResString(281, offSet) + " " + Convert.ToString(saleLine.pumpID);
                        }
                    }
                    else
                    {
                        newSaleLine.Description = saleLine.Description + " " + ResourceManager.GetResString( 281, offSet) +
                                                  " " + Convert.ToString(saleLine.pumpID);
                    }

                }
                else
                {
                    newSaleLine.Description = saleLine.Description;
                }

                // Derive the format strings for quantity and price.

                var fs = Convert.ToString(saleLine.QUANT_DEC == 0 ? "0" : "0." + new string('0', saleLine.QUANT_DEC));
                newSaleLine.Quantity = saleLine.Quantity.ToString(fs);
                //WriteToLogFile "Qty changed on main screen for index " & Index & " to " & SL.Quantity
                fs = Convert.ToString(saleLine.PRICE_DEC == 0 ? "0" : "0." + new string('0', saleLine.PRICE_DEC));
                newSaleLine.Price = saleLine.NoPriceFormat ? saleLine.price.ToString(CultureInfo.InvariantCulture) : saleLine.price.ToString(fs);

                // Show % or $ discounts
                if (saleLine.Discount_Rate > 0)
                {
                    newSaleLine.DiscountRate = saleLine.Discount_Type == "%" ? saleLine.Discount_Rate.ToString("#0.00") : saleLine.Discount_Rate.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    newSaleLine.DiscountRate = "";
                }

                newSaleLine.Amount = saleLine.Amount == 0 ? "0.00" : (saleLine.Amount - Convert.ToDecimal(saleLine.Line_Discount)).ToString("###,##0.00");
                if (saleLine.ProductIsFuel || saleLine.Gift_Certificate)
                {
                    writeOffSaleLine = false;
                }
                newSaleLine.DiscountType = saleLine.Discount_Type;
                newSaleLine.AllowPriceChange = lineEdit != null && lineEdit.AllowPriceChange;
                newSaleLine.AllowQuantityChange = lineEdit != null && lineEdit.AllowQuantityChange;
                newSaleLine.AllowDiscountChange = lineEdit != null && lineEdit.AllowDiscountChange && !(saleLine.FuelRebateEligible && sale.Customer.UseFuelRebate && sale.Customer.UseFuelRebateDiscount);
                newSaleLine.AllowDiscountReason = lineEdit != null && lineEdit.AllowDiscountReason;
                newSaleLine.AllowPriceReason = lineEdit != null && lineEdit.AllowPriceReason;
                newSaleLine.AllowReturnReason = lineEdit != null && lineEdit.AllowReturnReason;
                newSaleLine.ConfirmDelete = lineEdit != null && lineEdit.ConfirmDelete;
                saleModel.SaleLines.Add(newSaleLine);
            }
            saleModel.EnableWriteOffButton = writeOff && sale.Sale_Lines.Count != 0 && writeOffSaleLine;
            saleModel.CustomerDisplayText = sale.CustomerDisplay;
            return saleModel;
        }
    }
}