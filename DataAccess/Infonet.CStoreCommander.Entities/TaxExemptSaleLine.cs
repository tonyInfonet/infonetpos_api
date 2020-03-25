// VBConversions Note: VB project level imports

namespace Infonet.CStoreCommander.Entities
{
    public class TaxExemptSaleLine
    {

        private float mvarTaxInclPrice;
        private float mvarOriginalPrice;
        private float mvarTaxFreePrice;
        private string mvarStockCode;
        private string mvarDescription;
        private string mvarProductKey;
        private string mvarProductCode;
        private float mvarQuantity;
        private float mvarUnitsPerPkg;
        
        private short mvarQuantityPerPkg;
        private short mvarBaseUnitQty;
        
        private float mvarEquvQuantity;
        private mPrivateGlobals.teProductEnum mvarProductType;
        private string mvarUpcCode;
        private bool mvarIsFuelItem;
        private short mvarGradeID;
        private short mvarTierID;
        private short mvarLevelID;
        private string sLastError;
        private float mvarExemptedTax;
        private float mvarAmount;
        private short mvarLine_Num;
        private bool mvarOverLimit;
        private float mvarRunningQuota;
        private short mvarItemID;
        
        private bool mvarToBeUpdated;
        private float mvarNewQuantity;
        
        private bool mvarWasTaxExemptReturn; 
        private float mvarTaxExemptRate; 
        private float mvarTaxCreditAmount; 
        private bool mvarStockIsChanged; 
        private decimal mvarIncludedTax; // Dec3, 2008

        private void Class_Initialize_Renamed()
        {
            mvarOverLimit = false;
            mvarToBeUpdated = false;
            mvarStockIsChanged = false; 
            sLastError = "No Error";
            mvarWasTaxExemptReturn = false; 
        }

        public TaxExemptSaleLine()
        {
            Class_Initialize_Renamed();
        }

        //Description: this function has to be called before adding a tax exempt line
        
        
        
        //public bool MakeTaxExemptLine()
        //{
        //    bool returnValue = false;
        //    bool bFound = false;

        //    returnValue = false;
        //    
        //    
        //    if (!mPrivateGlobals.theSystem.teGetTaxFreePrice(mvarProductKey, mvarOriginalPrice, mvarTaxFreePrice,mvarUnitsPerPkg,  mvarProductType,mvarUpcCode, bFound, mvarStockCode,mvarProductCode, mvarTaxExemptRate))
        //    {
        //        
        //        sLastError = mPrivateGlobals.theSystem.teGetLastError();
        //        return returnValue;
        //    }

        //    if (!bFound)
        //    {
        //        return returnValue;
        //    }

        //    
        //    if (((mvarProductType == mPrivateGlobals.teProductEnum.eCigarette) || (mvarProductType == mPrivateGlobals.teProductEnum.eCigar)) || (mvarProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
        //    {
        //        mvarEquvQuantity = float.Parse((mvarUnitsPerPkg * mvarQuantity).ToString("#0.00"));
        //    } // hen
        //    else if (((((mvarProductType == mPrivateGlobals.teProductEnum.eGasoline) || (mvarProductType == mPrivateGlobals.teProductEnum.eDiesel)) || (mvarProductType == mPrivateGlobals.teProductEnum.ePropane)) || (mvarProductType == mPrivateGlobals.teProductEnum.emarkedGas)) || (mvarProductType == mPrivateGlobals.teProductEnum.emarkedDiesel))
        //    {
        //        mvarEquvQuantity = float.Parse((mvarUnitsPerPkg * mvarQuantity).ToString("#0.000"));
        //    }

        //    
        //    
        //    
        //    
        //    
        //    mvarExemptedTax = mPrivateGlobals.theSystem.RoundToHighCent((mvarOriginalPrice - mvarTaxFreePrice) * mvarQuantity);

        //    
        //    
        //    
        //    mvarAmount = mvarTaxInclPrice - mvarExemptedTax - mvarTaxCreditAmount;
        //    

        //    returnValue = true;
        //    return returnValue;
        //}


        public float Quantity
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarQuantity;
                return returnValue;
            }
            set
            {
                mvarQuantity = value;
            }
        }


        public float UnitsPerPkg
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarUnitsPerPkg;
                return returnValue;
            }
            set
            {
                mvarUnitsPerPkg = value;
            }
        }

        public string UpcCode
        {
            get { return mvarUpcCode; }
            set { mvarUpcCode = value; }
        }


        public float EquvQuantity
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarEquvQuantity;
                return returnValue;
            }
            set
            {
                mvarEquvQuantity = value;
            }
        }


        public float TaxInclPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTaxInclPrice;
                return returnValue;
            }
            set
            {
                mvarTaxInclPrice = value;
            }
        }


        public float OriginalPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarOriginalPrice;
                return returnValue;
            }
            set
            {
                mvarOriginalPrice = value;
            }
        }


        public float TaxFreePrice
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTaxFreePrice;
                return returnValue;
            }
            set
            {
                mvarTaxFreePrice = value;
            }
        }

        

        public float TaxCreditAmount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTaxCreditAmount;
                return returnValue;
            }
            set
            {
                mvarTaxCreditAmount = value;
            }
        }
        


        public string StockCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarStockCode;
                return returnValue;
            }
            set
            {
                mvarStockCode = value;
            }
        }


        public string Description
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDescription;
                return returnValue;
            }
            set
            {
                mvarDescription = value;
            }
        }


        public string ProductKey
        {
            get
            {
                string returnValue = "";
                returnValue = mvarProductKey;
                return returnValue;
            }
            set
            {
                mvarProductKey = value;
            }
        }


        public string ProductCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarProductCode;
                return returnValue;
            }
            set
            {
                mvarProductCode = value;
            }
        }


        public mPrivateGlobals.teProductEnum ProductType
        {
            get
            {
                mPrivateGlobals.teProductEnum returnValue = default(mPrivateGlobals.teProductEnum);
                returnValue = mvarProductType;
                return returnValue;
            }
            set
            {
                mvarProductType = value;
            }
        }


        public short GradeID
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarGradeID;
                return returnValue;
            }
            set
            {
                mvarGradeID = value;
            }
        }


        public short TierID
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarTierID;
                return returnValue;
            }
            set
            {
                mvarTierID = value;
            }
        }


        public short LevelID
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLevelID;
                return returnValue;
            }
            set
            {
                mvarLevelID = value;
            }
        }


        public bool IsFuelItem
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsFuelItem;
                return returnValue;
            }
            set
            {
                mvarIsFuelItem = value;
            }
        }


        public float ExemptedTax
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarExemptedTax;
                return returnValue;
            }
            set
            {
                mvarExemptedTax = value;
            }
        }


        public float Amount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
            }
        }


        public short ItemID
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarItemID;
                return returnValue;
            }
            set
            {
                mvarItemID = value;
            }
        }


        public short Line_Num
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLine_Num;
                return returnValue;
            }
            set
            {
                mvarLine_Num = value;
            }
        }


        public bool OverLimit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarOverLimit;
                return returnValue;
            }
            set
            {
                mvarOverLimit = value;
            }
        }

        

        public bool ToBeUpdated
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarToBeUpdated;
                return returnValue;
            }
            set
            {
                mvarToBeUpdated = value;
            }
        }


        public float NewQuantity
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarNewQuantity;
                return returnValue;
            }
            set
            {
                mvarNewQuantity = value;
            }
        }
        

        

        public bool StockIsChanged
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarStockIsChanged;
                return returnValue;
            }
            set
            {
                mvarStockIsChanged = value;
            }
        }
        

        

        public float TaxExemptRate
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTaxExemptRate;
                return returnValue;
            }
            set
            {
                mvarTaxExemptRate = value;
            }
        }
        


        public float RunningQuota
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarRunningQuota;
                return returnValue;
            }
            set
            {
                mvarRunningQuota = value;
            }
        }

        

        public bool WasTaxExemptReturn
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarWasTaxExemptReturn;
                return returnValue;
            }
            set
            {
                mvarWasTaxExemptReturn = value;
            }
        }
        
        //  - To keep Included Tax information

        public decimal IncludedTax
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarIncludedTax;
                return returnValue;
            }
            set
            {
                mvarIncludedTax = value;
            }
        }

        public string GetLastError()
        {
            string returnValue = "";
            returnValue = sLastError;
            return returnValue;
        }

        public string LastError { get; set; }
        //shiny end

        //Added new property
        public string CardHolderID { get; set; }
    }
}
