using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TaxExemptSaleManager : ManagerBase, ITaxExemptSaleManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ITillService _tillService;
        private readonly ITaxExemptService _taxExemptSaleService;
        private readonly ITreatyManager _treatyManager;
        private readonly IPurchaseListManager _purchaseListManager;
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="tillService"></param>
        /// <param name="taxExemptSaleService"></param>
        /// <param name="treatyManager"></param>
        /// <param name="purchaseListManager"></param>
        public TaxExemptSaleManager(IPolicyManager policyManager,
            ITillService tillService,
            ITaxExemptService taxExemptSaleService,
            ITreatyManager treatyManager,
            IPurchaseListManager purchaseListManager)
        {
            _policyManager = policyManager;
            _tillService = tillService;
            _taxExemptSaleService = taxExemptSaleService;
            _treatyManager = treatyManager;
            _purchaseListManager = purchaseListManager;
        }

        /// <summary>
        /// Method to save tax exempt sale
        /// </summary>
        /// <param name="oteSale">Tax exempt sale</param>
        public void SaveSale(TaxExemptSale oteSale)
        {
            
            

            
            
            //   - QITE , we need to save in the Taxexempt salehead( but taxexemptsaleline) '
            if ((_policyManager.TE_Type == "AITE" && !oteSale.teCardholder.GstExempt && oteSale.Te_Sale_Lines.Count > 0) || (_policyManager.TE_Type == "QITE" && !oteSale.teCardholder.GstExempt && oteSale.TaxCreditLines.Count > 0))
            {
                var shiftdate = _tillService.GetTill(oteSale.TillNumber).ShiftDate;
                _taxExemptSaleService.SaveTaxExemptSaleHead(oteSale, shiftdate);

            } 

            _taxExemptSaleService.SaveTaxCredit(oteSale);
        }

        /// <summary>
        /// Method to load tax exempt prepay
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="sl">Sale line</param>
        /// <param name="lineNo">Line number</param>
        /// <param name="sale">Sale</param>
        /// <param name="oPurchaseList">Purchase list</param>
        /// <param name="oTreatyNo">Treaty number</param>
        /// <returns>True or false</returns>
        public bool LoadTaxExemptPrepay(int saleNo, Sale_Line sl, short lineNo, ref 
            Sale sale, ref tePurchaseList oPurchaseList, ref teTreatyNo oTreatyNo)
        {
            var returnValue = false;
            if (!_policyManager.TAX_EXEMPT)
            {
                return false;
            }
            var user = CacheManager.GetUser(UserCode);
            if (_policyManager.TE_Type == "SITE")
            {
                var teItem = _taxExemptSaleService.GetPurchaseItem(saleNo, lineNo);
                if (teItem != null)
                {
                    _treatyManager.Init(ref oTreatyNo, teItem.TreatyNo, false);
                    oPurchaseList.Init(oTreatyNo, sale.Sale_Num, sale.TillNumber);
                    
                    var tempSProductKey = mPrivateGlobals.theSystem.teMakeFuelKey(sl.GradeID, teItem.PsTierID, teItem.PsLevelID);
                    var tempIsFuelItem = true;
                    var stockCode = string.Empty;
                    short iRowNumberinSale = 0;
                    double tempQuantity = 0f;
                    double org = teItem.PdOriginalPrice;
                    double taxIncldPrice = teItem.petaxInclPrice;
                    _purchaseListManager.AddItem(ref oPurchaseList,
                        ref sale, ref oTreatyNo, ref tempSProductKey, ref tempQuantity, ref org, ref iRowNumberinSale, ref stockCode, ref taxIncldPrice, ref tempIsFuelItem);
                    sl.Stock_Code = stockCode;
                }
            }
            else
            {
                var oTeSale = new TaxExemptSale();

                if (_taxExemptSaleService.LoadTaxExemptForDeletePrepay(ref oTeSale, 
                    _policyManager.TE_Type, _tillService.GetTill(sale.TillNumber), 
                    user, saleNo, sale.TillNumber, lineNo, sale.Sale_Num))
                {
                    returnValue = true;
                }
                else if (_taxExemptSaleService.LoadGstExemptForDeletePrepay(ref oTeSale,
                    saleNo, sale.TillNumber, DataSource.CSCTills))
                {
                    oTeSale.teCardholder.GstExempt = true;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Method to update tax exempt sale
        /// </summary>
        /// <param name="taxExemptSale">Tax exempt sale</param>
        public void UpdateSale(ref TaxExemptSale taxExemptSale)
        {
            if (taxExemptSale.HasUpdatedLine)
            {

                
                float gasLessQuota = 0;
                float propaneLessQuota = 0;
                float tobaccoLessQuota = 0;
                TaxExemptSaleLine tesl;
                foreach (TaxExemptSaleLine tempLoopVarTesl in taxExemptSale.Te_Sale_Lines)
                {
                    tesl = tempLoopVarTesl;
                    if (tesl.ToBeUpdated)
                    {
                        float quotaReduce;
                        if (tesl.ProductType == mPrivateGlobals.teProductEnum.eCigarette || (tesl.ProductType == mPrivateGlobals.teProductEnum.eCigar) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
                        {
                            quotaReduce = float.Parse(((tesl.Quantity - tesl.NewQuantity) * (tesl.OriginalPrice - tesl.TaxFreePrice)).ToString("#0.00"));
                            tobaccoLessQuota = tobaccoLessQuota + quotaReduce;
                            tesl.Quantity = tesl.NewQuantity;
                            tesl.EquvQuantity = float.Parse((tesl.UnitsPerPkg * tesl.Quantity).ToString("#0.00"));
                        } // hen
                        else if (tesl.ProductType == mPrivateGlobals.teProductEnum.eGasoline || (tesl.ProductType == mPrivateGlobals.teProductEnum.eDiesel) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedGas) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel))
                        {
                            quotaReduce = float.Parse(((tesl.Quantity - tesl.NewQuantity) * (tesl.OriginalPrice - tesl.TaxFreePrice)).ToString("#0.00"));
                            gasLessQuota = gasLessQuota + quotaReduce;
                            tesl.Quantity = tesl.NewQuantity;
                            tesl.EquvQuantity = float.Parse((tesl.UnitsPerPkg * tesl.Quantity).ToString("#0.000"));
                        }
                        else if (tesl.ProductType == mPrivateGlobals.teProductEnum.ePropane)
                        {
                            quotaReduce = float.Parse(((tesl.Quantity - tesl.NewQuantity) * (tesl.OriginalPrice - tesl.TaxFreePrice)).ToString("#0.00"));
                            propaneLessQuota = propaneLessQuota + quotaReduce;
                            tesl.Quantity = tesl.NewQuantity;
                            tesl.EquvQuantity = float.Parse((tesl.UnitsPerPkg * tesl.Quantity).ToString("#0.000"));
                        }

                        
                        
                        
                        
                        tesl.ExemptedTax = mPrivateGlobals.theSystem.RoundToHighCent((tesl.OriginalPrice - tesl.TaxFreePrice) * tesl.Quantity);
                        tesl.Amount = tesl.TaxInclPrice - tesl.ExemptedTax - tesl.TaxCreditAmount;
                        
                    }
                }

                
                
                taxExemptSale.Amount = 0;
                taxExemptSale.TotalExemptedTax = 0;
                taxExemptSale.TobaccoOverLimit = false;
                taxExemptSale.GasOverLimit = false;
                taxExemptSale.PropaneOverLimit = false;
                foreach (TaxExemptSaleLine tempLoopVarTesl in taxExemptSale.Te_Sale_Lines)
                {
                    tesl = tempLoopVarTesl;
                    if (((tesl.ProductType == mPrivateGlobals.teProductEnum.eCigarette) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eCigar)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
                    {
                        tesl.RunningQuota = tesl.RunningQuota - tobaccoLessQuota;
                        if (tesl.RunningQuota > mPrivateGlobals.theSystem.TobaccoLimit)
                        {
                            taxExemptSale.TobaccoOverLimit = true;
                            tesl.OverLimit = true;
                        }
                    } // hen
                    else if ((((tesl.ProductType == mPrivateGlobals.teProductEnum.eGasoline) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eDiesel)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedGas)) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel))
                    {
                        tesl.RunningQuota = tesl.RunningQuota - gasLessQuota;
                        if (tesl.RunningQuota > mPrivateGlobals.theSystem.GasLimit)
                        {
                            taxExemptSale.GasOverLimit = true;
                            tesl.OverLimit = true;
                        }
                    }
                    else if (tesl.ProductType == mPrivateGlobals.teProductEnum.ePropane)
                    {
                        tesl.RunningQuota = tesl.RunningQuota - propaneLessQuota;
                        if (tesl.RunningQuota > mPrivateGlobals.theSystem.PropaneLimit)
                        {
                            taxExemptSale.PropaneOverLimit = true;
                            tesl.OverLimit = true;
                        }
                    }
                    taxExemptSale.Amount = taxExemptSale.Amount + tesl.Amount;
                    taxExemptSale.TotalExemptedTax = taxExemptSale.TotalExemptedTax + tesl.ExemptedTax;
                }
                _taxExemptSaleService.UpdateSale(taxExemptSale, gasLessQuota, propaneLessQuota,
                    tobaccoLessQuota);
            }
        }

    }
}
