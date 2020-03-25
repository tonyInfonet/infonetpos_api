using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TaxExemptSaleLineManager : ManagerBase, ITaxExemptSaleLineManager
    {

        private readonly ITeSystemManager _teSystemManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="teSystemManager"></param>
        public TaxExemptSaleLineManager(ITeSystemManager teSystemManager)
        {
            _teSystemManager = teSystemManager;
        }

        /// <summary>
        /// Method to make default tax exempt sale line
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="theSystem">Tax exempt system</param>
        /// <returns>True or false</returns>
        public bool MakeTaxExemptLine(ref TaxExemptSaleLine saleLine, ref teSystem theSystem)
        {
            bool bFound = false;
            
            
            if (!_teSystemManager.TeGetTaxFreePrice(ref theSystem, ref saleLine, ref bFound))// mPrivateGlobals.theSystem.teGetTaxFreePrice(mvarProductKey, mvarOriginalPrice, mvarTaxFreePrice, mvarUnitsPerPkg, mvarProductType, mvarUpcCode, bFound, mvarStockCode, mvarProductCode, mvarTaxExemptRate))
            {
                saleLine.LastError = theSystem.teGetLastError();
                return false;
            }

            if (!bFound)
            {
                return false;
            }
            
            if ((saleLine.ProductType == mPrivateGlobals.teProductEnum.eCigarette) || (saleLine.ProductType== mPrivateGlobals.teProductEnum.eCigar) || (saleLine.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
            {
                saleLine.EquvQuantity = float.Parse((saleLine.UnitsPerPkg * saleLine.Quantity).ToString("#0.00"));
            } // hen
            else if ((saleLine.ProductType == mPrivateGlobals.teProductEnum.eGasoline) || (saleLine.ProductType == mPrivateGlobals.teProductEnum.eDiesel) || (saleLine.ProductType == mPrivateGlobals.teProductEnum.ePropane) || (saleLine.ProductType == mPrivateGlobals.teProductEnum.emarkedGas) || (saleLine.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel))
            {
                saleLine.EquvQuantity = float.Parse((saleLine.UnitsPerPkg * saleLine.Quantity).ToString("#0.000"));
            }
                        
            saleLine.ExemptedTax = theSystem.RoundToHighCent((saleLine.OriginalPrice - saleLine.TaxFreePrice) * saleLine.Quantity);
                       
            saleLine.Amount = saleLine.TaxInclPrice - saleLine.ExemptedTax - saleLine.TaxCreditAmount;

            return true;
        }
    }
}
