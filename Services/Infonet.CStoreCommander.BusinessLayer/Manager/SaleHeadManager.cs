using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using System;
using System.Linq;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Sale head manager
    /// </summary>
    public class SaleHeadManager : ManagerBase, ISaleHeadManager
    {
        private readonly ISaleService _saleService;
        private readonly ICustomerManager _customerManager;
        private readonly IPolicyManager _policyManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="saleService"></param>
        /// <param name="customerManager"></param>
        /// <param name="policyManager"></param>
        public SaleHeadManager(ISaleService saleService,
            ICustomerManager customerManager,
            IPolicyManager policyManager)
        {
            _saleService = saleService;
            _customerManager = customerManager;
            _policyManager = policyManager;
        }

        /// <summary>
        /// Method to set sale policies
        /// </summary>
        /// <param name="sale">Sale</param>
        public void SetSalePolicies(ref Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleHeadManager,SetSalePolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //update the policies
            sale.USE_LOYALTY = _policyManager.USE_LOYALTY;
            sale.LOYAL_TYPE = _policyManager.LOYAL_TYPE;
            sale.Loyal_pricecode = _policyManager.LOYAL_PRICE;
            sale.CUST_DISC = _policyManager.CUST_DISC;
            sale.Loydiscode = _policyManager.LOYAL_DISC;
            sale.PROD_DISC = _policyManager.PROD_DISC;
            sale.Combine_Policy = _policyManager.COMBINE_LINE;
            sale.XRigor = _policyManager.X_RIGOR;
            Load_Taxes(ref sale);
            Performancelog.Debug($"End,SaleHeadManager,SetSalePolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to create a new sale
        /// </summary>
        /// <returns>Sale</returns>
        public Sale CreateNewSale()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleHeadManager,CreateNewSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var newSale = new Sale();
            Load_Taxes(ref newSale);
            RemoveTempData(newSale.Sale_Num, newSale.TillNumber);
            SetSalePolicies(ref newSale);
            newSale.Customer = _customerManager.LoadCustomer(string.Empty);
            Performancelog.Debug($"End,SaleHeadManager,CreateNewSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return newSale;
        }

        /// <summary>
        /// Load the tax definitions
        /// <param name="sale">Sale</param>
        /// </summary>
        public void Load_Taxes(ref Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleHeadManager,Load_Taxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            if (sale.Sale_Totals.Sale_Taxes != null && sale.Sale_Totals.Sale_Taxes.Count > 0)
            {
                return;
            }
            var taxMast = _saleService.GetTaxMast();
            var taxRates = _saleService.GetTaxRates();

            foreach (var tax in taxMast)
            {
                var txRates = taxRates.Where(t => t.TaxName == tax.TaxName);
                foreach (var taxRate in txRates)
                {
                    sale.Sale_Totals.Sale_Taxes?.Add(Convert.ToString(tax.TaxName), Convert.ToString(taxRate.TaxCode), taxRate.Rate ?? 0, 0, 0, 0, 0, taxRate.Rebate ?? 0, 0, tax.TaxName + Convert.ToString(taxRate.TaxCode));
                }
            }
            Performancelog.Debug($"End,SaleHeadManager,Load_Taxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        #region Private methods

        /// <summary>
        /// Method to remove temp data
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        private void RemoveTempData(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleHeadManager,RemoveTempData,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            _saleService.RemoveTempDataInDbTill(tillNumber, saleNumber);
            Performancelog.Debug($"End,SaleHeadManager,RemoveTempData,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        #endregion
    }
}
