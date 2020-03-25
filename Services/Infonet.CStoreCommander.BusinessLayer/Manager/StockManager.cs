using System;
using System.Collections.Generic;
using System.Net;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System.Linq;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class StockManager : ManagerBase, IStockManager
    {
        private readonly IStockService _stockService;
        private readonly IPolicyManager _policyManager;
        private readonly ITaxService _taxService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ILoginManager _loginManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="stockService"></param>
        /// <param name="policyManager"></param>
        /// <param name="taxService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="loginManager"></param>
        public StockManager(IStockService stockService, IPolicyManager policyManager, ITaxService taxService,
            IApiResourceManager resourceManager, ILoginManager loginManager)
        {
            _stockService = stockService;
            _policyManager = policyManager;
            _taxService = taxService;
            _resourceManager = resourceManager;
            _loginManager = loginManager;
        }

        /// <summary>
        /// Method to add a stock Item
        /// </summary>
        /// <param name="userName">UserCode</param>
        /// <param name="stockItem">Stock Item</param>
        /// <param name="taxCodes">List of tax codes</param>
        /// <param name="error">Error message</param>
        public void AddStockItem(string userName, StockItem stockItem, List<string> taxCodes, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,AddStockItem,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage();
            if (!string.IsNullOrEmpty(userName) && _loginManager.GetUser(userName) != null)
            {
                //var user = _userService.GetUser(userName);
                var user = _loginManager.GetUser(userName);

                var stockCode = stockItem.StockCode;
                var canAddStock = _policyManager.GetPol("U_AddStock", user);
                if (!Convert.ToBoolean(canAddStock))
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 39, 91, stockCode, (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly));
                    error.StatusCode = HttpStatusCode.NotFound;
                    return;
                }
                if (string.IsNullOrEmpty(stockCode))
                {
                    error.MessageStyle = new MessageStyle
                    {
                        Message = "Stock code is required",
                        MessageType = MessageType.OkOnly
                    };
                    error.StatusCode = HttpStatusCode.NotAcceptable;
                    return;
                }
                if (string.IsNullOrEmpty(stockItem.Description))
                {
                    error.MessageStyle = new MessageStyle
                    {
                        Message = "Description is required",
                        MessageType = MessageType.OkOnly
                    };
                    error.StatusCode = HttpStatusCode.NotAcceptable;
                    return;
                }
                if (Conversion.Val(stockItem.Price) <= 0)
                {
                    error.MessageStyle = new MessageStyle
                    {
                        Message = "Price should be more than 0",
                        MessageType = MessageType.OkOnly
                    };
                    error.StatusCode = HttpStatusCode.NotAcceptable;
                    return;
                }
                if (SetupTaxes(taxCodes, stockCode) == false)
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 39, 96, stockCode, (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly));
                    error.StatusCode = HttpStatusCode.Conflict;
                    return;
                }
                if (_stockService.GetStockItemByCode(stockCode, true) != null)
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 39, 95, stockCode, (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly));
                    error.StatusCode = HttpStatusCode.Conflict;
                    return;
                }
                if (_stockService.GetPluMast(stockCode) != null)
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 39, 95, stockCode, (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly));
                    error.StatusCode = HttpStatusCode.Conflict;
                    return;
                }
                _stockService.AddStockItem(stockItem, _policyManager.ELG_LOY);
                _stockService.AddPluMast(stockCode);
                _stockService.AddStockBranch(stockCode);
                _stockService.AddStockPrice(stockCode, stockItem.Price);
            }
            else
            {
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 10, 95, userName, (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly));
                error.StatusCode = HttpStatusCode.Unauthorized;
            }

            Performancelog.Debug($"End,StockManager,AddStockItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Gets the List of HotButton Pages
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetHotButonPages()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,GetHotButonPages,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var hotButtonPages = _stockService.GetHotButonPages();
            Performancelog.Debug($"End,StockManager,GetHotButonPages,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return hotButtonPages;
        }

        /// <summary>
        /// Get Hot Buttons
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public List<HotButton> GetHotButons(int pageId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,GetHotButons,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (pageId == 0)
            {
                pageId = 1;
            }
            var firstIndex = (pageId - 1) * 36 + 1;
            var lastIndex = pageId * 36;
            var hotButtons = _stockService.GetHotButtons(firstIndex, lastIndex);
            Performancelog.Debug($"End,StockManager,GetHotButons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return hotButtons;
        }

        /// <summary>
        /// Method to get stock item using stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="error">Error message</param>
        /// <returns>Stock item</returns>
        public StockItem GetStockByCode(string stockCode, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,GetStockByCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            error = new ErrorMessage();
            if (string.IsNullOrEmpty(stockCode))
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Stock code is required",
                    MessageType = MessageType.OkOnly
                };
                error.StatusCode = HttpStatusCode.NotAcceptable;
                Performancelog.Debug($"End,StockManager,GetStockByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }
            var stock = _stockService.GetStockItemByCode(stockCode, _policyManager.Sell_Inactive);
            if (stock == null)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 39, 91, stockCode);
                error.StatusCode = HttpStatusCode.NotFound;
                Performancelog.Debug($"End,StockManager,GetStockByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }
            Performancelog.Debug($"End,StockManager,GetStockByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return stock;
        }

        /// <summary>
        /// Get list of stock items using pagination
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock items</returns>
        public List<StockItem> GetStockItems(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,GetStockItems,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockItems = _policyManager.Sell_Inactive
                ? _stockService.GetStockItems(pageIndex, pageSize)
                : _stockService.GetActiveStockItems(pageIndex, pageSize);

            Performancelog.Debug($"End,StockManager,GetStockItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return stockItems;
        }

        /// <summary>
        /// Method to get stock item using a stock term
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock items</returns>
        public List<StockItem> SearchStockItems(string searchTerm, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,SearchStockItems,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockItems = _stockService.SearchStock(searchTerm, _policyManager.Sell_Inactive, pageIndex, pageSize);

            Performancelog.Debug($"End,StockManager,SearchStockItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return stockItems;
        }

        public void SetVendorCoupon(ref Vendor vendor, string code)
        {
            var vendorDetails = _stockService.GetVendorByCode(code);
            if (vendorDetails != null)
            {
                vendor.Code = code;
                vendor.Name = vendorDetails.Name;
                vendor.Address = vendorDetails.Address;
            }
            else
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                vendor.Code = code;
                vendor.Name = _resourceManager.GetResString(offSet, 288);
            }
        }

        public Stock GetStockDetails(string code)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var none = _resourceManager.GetResString(offSet, 347);
            var plu = _stockService.GetPluMast(code);
            if (plu == null)
                return new Stock();
            var stock = CacheManager.GetStockInformation(code);

            stock = new Stock
            {
                PLUPrim = plu.PLUPrim,
                PLUType = plu.PLUType
            };
            var stockItem = _stockService.GetStockByStockCode(code);
            if (stockItem != null)
            {
                stock.StockCode = stockItem.StockCode;
                stock.Description = stockItem.Description ?? "";
                stock.Department = stockItem.Department;
                stock.SubDepartment = stockItem.SubDepartment;
                stock.SubDetail = stockItem.SubDetail;
                stock.StockType = stockItem.StockType == '\0' ? 'V' : stockItem.StockType;
                stock.Vendor = stockItem.Vendor;
                stock.PRType = stockItem.PRType == '\0' ? 'R' : stockItem.PRType;
                stock.PRUnit = stockItem.PRUnit == '\0' ? '$' : stockItem.PRUnit;
                stock.Vendor = stockItem.Vendor ?? "";
                stock.LoyaltySave = 0;
                stock.ProductDescription = stockItem.ProductDescription;
                stock.SByWeight = stockItem.SByWeight;
                stock.UM = stockItem.UM ?? "";
                stock.StandardCost = stockItem.StandardCost;
                stock.AverageCost = stockItem.AverageCost;
                stock.EligibleLoyalty = stockItem.EligibleLoyalty;
                stock.EligibleFuelRebate = _policyManager.FuelRebate && stockItem.EligibleFuelRebate;
                stock.FuelRebate = stockItem.FuelRebate;
                stock.EligibletaxRebate = stockItem.EligibletaxRebate;
                stock.QualtaxRebate = stockItem.QualtaxRebate;
                stock.EligibleTaxExemption = stockItem.EligibleTaxExemption;
            }
            else
            {
                stock.StockType = 'V';
            }
            // Set the rebate for the product, if any,  
            if (stockItem != null && !string.IsNullOrEmpty(stockItem.Vendor))
            {
                stock.Rebate = _stockService.GetStockRebate(stockItem.Vendor, stockItem.StockCode);
            }


            if (_policyManager.TAX_EXEMPT)
            //  -made the same for all taxexempt- after talking to Nicolette 'And Policy.TE_Type = "SITE" Then
            {
                if (stockItem != null)
                {
                    var productTe = _stockService.GetProductTaxExempt(stockItem.StockCode);
                    if (productTe != null)
                    {
                        stock.TECategory =
                            Convert.ToInt16(productTe.CategoryFK ?? 0);
                        stock.TEVendor = Convert.ToString(productTe.TEVendor ?? "");
                        //shiny added the TE vendor
                    }
                }
            }

            stock.Charges = GetCharges(stock.StockCode, stock.StockType);
            stock.LineKits = GetLineKits(stock.StockCode, stock.StockType, stock.Description, stock.Vendor);
            stock.LineTaxes = GetLineTaxes(stock.StockCode, stock.StockType);

            CacheManager.AddStockInformation(stock, stock.StockCode);
            return stock;
        }
                

        #region Private methods

        /// <summary>
        /// Method to make charges
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="stockType">Stock type</param>
        /// <returns>Charges</returns>
        private Charges GetCharges(string stockCode, char stockType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,GetCharges,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var cg = new Charges();
            
            if (stockType == 'G')
            {
                return null;
            }
            var allTaxes = _taxService.GetAllTaxes();
            // Load Charges
            var charges = _stockService.GetAssociateCharges(stockCode);
            foreach (var charge in charges)
            {
                var taxes = _stockService.GetTax(charge.AsCode);
                var cs = new Charge_Taxes();
                foreach (var tax in taxes)
                {
                    var taxMast = allTaxes.FirstOrDefault(t => t.TaxName == tax.Tax_Name && t.Active.HasValue && t.Active.Value);
                    if (taxMast != null)
                    {
                        cs.Add(Convert.ToString(tax.Tax_Name), Convert.ToString(tax.Tax_Code),
                            Convert.ToSingle(tax.Tax_Rate), Convert.ToBoolean(tax.Tax_Included), "");
                    }
                }

                cg.Add(Convert.ToString(charge.AsCode), Convert.ToString(charge.Description),
                    Convert.ToSingle(charge.Price), cs, "");
            }

            var returnValue = cg;
            Performancelog.Debug($"End,StockManager,GetCharges,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to make taxes
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="stockType">Stock type</param>
        /// <returns>Line taxes</returns>
        private Line_Taxes GetLineTaxes(string stockCode, char stockType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,GetLineTaxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var lt = new Line_Taxes();
            
            if (stockType == 'G')
            {
                return null;
            }

            var taxes = _taxService.GetTaxInfoByStockCode(stockCode);
            if (taxes != null && taxes.Count != 0)
            {
                foreach (var tax in taxes)
                {
                    lt.Add(tax.TaxName,
                        tax.TaxCode,
                        tax.Rate,
                        tax.Included,
                        Convert.ToSingle(tax.Rebate), 0,
                        tax.TaxName);

                }
            }
            var returnValue = lt;
            Performancelog.Debug($"End,StockManager,GetLineTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to make kits
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="stockType">Stock type</param>
        /// <param name="description">Description</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Line kits</returns>
        private Line_Kits GetLineKits(string stockCode, char stockType, string description
            , string vendor)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,GetLineKits,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Charge_Taxes cts = default(Charge_Taxes);
            var kitCharges = new K_Charges();
            var lineKitsRenamed = new Line_Kits();

            if (stockType == 'G')
            {
                return null;
            }

            if (_stockService.IsKitPresent(stockCode))
            {
                var kits = _stockService.GetKitIems(stockCode);
                if (kits != null && kits.Count != 0)
                    foreach (var kit in kits)
                    {
                        var allPrices = _stockService.GetStockPricesByCode(kit.StockCode);
                        var price = allPrices.FirstOrDefault(p => p.Vendor == vendor) ??
                                    allPrices.FirstOrDefault(p => p.Vendor == "ALL");
                        var stockDescription = _stockService.GetKitDescription(kit.StockCode);
                        if (price != null)
                        {
                            var allTaxes = _taxService.GetAllTaxes();
                            // Get Charges on Kit Items
                            // Build the collection of charges
                            var charges = _stockService.GetAssociateCharges(kit.StockCode);

                            if (charges != null && charges.Count != 0)
                            {
                                var firstOrDefault = charges.FirstOrDefault();
                                if (firstOrDefault != null)
                                {
                                    var taxes = _stockService.GetTax(firstOrDefault.AsCode);

                                    if (taxes != null && taxes.Count != 0)
                                    {
                                        var saleTax = taxes.FirstOrDefault();
                                        if (allTaxes.FirstOrDefault(t => saleTax != null && t.TaxName == saleTax.Tax_Name) != null)
                                        {
                                            cts = new Charge_Taxes();
                                            foreach (var tax in taxes)
                                            {
                                                cts.Add(Convert.ToString(tax.Tax_Name), Convert.ToString(tax.Tax_Code),
                                                    Convert.ToSingle(tax.Tax_Rate),
                                                    Convert.ToBoolean(tax.Tax_Included), "");
                                            }
                                        }
                                    }
                                }
                                kitCharges = new K_Charges();
                                foreach (var charge in charges)
                                {
                                    kitCharges.Add(Convert.ToDouble(charge.Price), charge.Description,
                                        charge.AsCode, cts, "");
                                }
                            }

                            // Add the kit item & charges to the kits collection
                            lineKitsRenamed.Add(kit.StockCode, Convert.ToString(stockDescription),
                                Convert.ToSingle(kit.Quantity), Convert.ToSingle(price.Price),
                                0, 0, "", kitCharges, "");

                            kitCharges = null;
                        }
                        else
                        {
                            //TIMsgbox "Kit Item " & Kits![Stock_Code] & " not found in stock master.", vbInformation + vbOKOnly, "Missing Kit Item"
                            var offSet = _policyManager.LoadStoreInfo().OffSet;
                            MessageType type = (int)MessageType.Critical + MessageType.OkOnly;
                            var messageStyle = _resourceManager.CreateMessage(offSet, 0, 8113, kit.StockCode, type);
                        }
                    }
            }
            var returnValue = lineKitsRenamed;
            Performancelog.Debug($"End,StockManager,GetLineKits,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to set up taxes
        /// </summary>
        /// <param name="selectedTaxes">SelEcted taxes</param>
        /// <param name="stockCode">sTOCK CODE</param>
        /// <returns>True or false</returns>
        private bool SetupTaxes(List<string> selectedTaxes, string stockCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,StockManager,SetupTaxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (selectedTaxes != null && selectedTaxes.Count != 0)
            {
                var taxes = _taxService.GetAllActiveTaxes();
                var taxNames = new List<string>();
                foreach (var tax in taxes)
                {
                    taxNames.Add($"{tax.Name} - {tax.Code}");
                }
                var status = _taxService.SetupTaxes(stockCode, taxNames, selectedTaxes);
                Performancelog.Debug($"End,StockManager,SetupTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return status;
            }
            Performancelog.Debug($"End,StockManager,SetupTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return true;
        }


        #endregion
    }
}
