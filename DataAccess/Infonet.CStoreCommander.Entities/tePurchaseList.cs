using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class tePurchaseList
    {
        // Programmer: Ashish Mittal                      Date: August 16, 2004
        // Environment: Visual Basic 6.0                  File: tePurchaseList.cls
        // OS: Windows 2k                                 Ver: 1

        // tePurchaseList:
        // The tePurchaseList class represents a list of tePurchaseItems

        private bool bIsInit;
        private string sLastError;

        private bool peisOverLimit;
        private Collection pecolItems = new Collection();
        public Collection purchaseItems
        {
            get { return pecolItems; }
            set
            {
                pecolItems = value;
            }
        }
        private double peQuantity;

        private short peitemCount;
        private teTreatyNo petreatyNo;
        private string psProductKey;
        private bool mvarIsSuccess; 

        
        private short runningQuotaT; //runningQuota Tobacco
        private float runningQuotaF; //runningQuota Fuel

        private bool isOverlimitF;
        private bool isOverlimitT;

        private bool isFirstIterationF;
        private bool isFirstIterationT;

        private int peInvoiceID;
        private short peTillID;

        private float mTotalExemptedTax; 
        private string mvarRTVPCommand; //   to keep last command send for real time validation
        private bool mvarRTVPError; //   to kepp the error from RTVP dll if any; because an response <>0 is not an error for all the commands, system decides if is an error or not based on command and response code; this property tells POS to display an error
        private short mvarRTVPResponse; //   to keep the response of RTVP dll in case we have to show it in the main form as an error
        private bool mvarNoRTVP; //   to handle incomplete and switch prepay

        private void Class_Initialize_Renamed()
        {
            bIsInit = false;
            sLastError = "No Error";
            mTotalExemptedTax = 0; 
        }
        public tePurchaseList()
        {
            Class_Initialize_Renamed();
        }

        public void AddItemInCollection(tePurchaseItem oItem)
        {
            pecolItems.Add(oItem, null, null, null);
            peitemCount++;
        }

        public bool Init(teTreatyNo oTreatyNo, int invoiceID, short TillID)
        {
            bool returnValue = false;
            //  Set pecolItems = New Collection

            petreatyNo = oTreatyNo;
            peInvoiceID = invoiceID;
            peTillID = TillID;

            isFirstIterationT = true;
            isFirstIterationF = true;

            peisOverLimit = false;
            bIsInit = true;
            returnValue = true;

            return returnValue;
        }

        public teTreatyNo GetTreatyNo()
        {
            teTreatyNo returnValue = default(teTreatyNo);

            returnValue = petreatyNo;

            return returnValue;
        }

        public bool RemoveItem(short RowNumberInSalesMain)
        {

            short iCount = 0;

            try
            {
                for (iCount = 1; iCount <= pecolItems.Count; iCount++)
                {

                    if ((pecolItems[iCount] as tePurchaseItem).GetRowInSalesMain() == RowNumberInSalesMain)
                    {
                        pecolItems.Remove(iCount);
                        peitemCount--;
                        return false;
                    }

                }
            }
            catch
            {
                sLastError = "Failed to Remove Item: "; //& oDb.GetLastError()

            }

            // TODO: Returning true because of no use of the returned value - Ipsit_15
            return true;
        }

        public dynamic Clear()
        {

            short iCount = 0;

            try
            {
                for (iCount = 1; iCount <= pecolItems.Count; iCount++)
                {
                    pecolItems.Remove(iCount);
                    peitemCount--;
                }
            }
            catch
            {
                sLastError = "Failed to Remove Item: "; //& oDb.GetLastError()

            }

            // TODO: no return value returning null - Ipsit_12
            return null;

        }

        public double PeQuantity
        {
            get { return peQuantity; }
            set { peQuantity = value; }
        }

        public string PsProductKey
        {
            get { return psProductKey; }
            set { psProductKey = value; }
        }

        public short Count()
        {
            short returnValue = 0;
            returnValue = peitemCount;
            return returnValue;
        }

        //Name: AddItem
        //Description: This method will check in the TaxExempt data to see if the item
        //  has a tax free price.  If it does, it will be added to the list of items in
        //  the purchase list.  If it does not, it will do nothing.
        //Inputs:
        //   sProductKey: The primary key used to identify this product in the existing
        //      C-Store commander database.  (The Primary Key in C-Store database).
        //   dQuantity: The amount of the item being purchased.
        //   dOriginalPrice: Note: It may be better to use Currency datatype here, but
        //      this should be checked with other developers. This is the original price
        //      with taxes included for the item.
        //   iRowNumberInSalesMainForm: The Row number of this item in the sales main form.
        //Outputs:  True if function worked ok, False if there was an error.
        //   Check GetLastError() for details.
        //Remarks: If the product is a gasoline or diesel product, set the sProductKey
        //   parameter to "Fuel: G:g,T:t,L:l" Where g, t and l are the GradeId, TierId
        //   and the LevelId.   Eg: "Fuel: G:1,T:2,L:4"

        //public bool AddItem(ref string sProductKey, ref double dQuantity, ref double dOriginalPrice, ref short iRowNumberInSalesMainForm, ref string stockcode, ref double TaxInclPrice, ref bool IsFuelItem)
        //{
          //  bool returnValue = false;

          //  returnValue = false;
           //tePurchaseItem oItem = default(tePurchaseItem);
            //double GasLimit;
            //
            //double Quantity = 0; 
            //double quota = 0; 
            //mPrivateGlobals.teProductEnum ProductType = default(mPrivateGlobals.teProductEnum); 
            //mPrivateGlobals.teLimitEnum limitMaxType = default(mPrivateGlobals.teLimitEnum); 
            //mPrivateGlobals.teLimitEnum limitThresholdType = default(mPrivateGlobals.teLimitEnum); 
            //double UnitsPerPkg = 0; 
            //double limitMax = 0; 
            //double limitThreshold = 0; 
            //short ret = 0;
            //double dblEquivalent = 0;
            //string FormatString = "";

            //oItem = new tePurchaseItem();

            ////initialize item then add to purchase list
            //if (!oItem.Init(petreatyNo, ref sProductKey, dOriginalPrice, dQuantity, iRowNumberInSalesMainForm, peInvoiceID, peTillID, ref stockcode, TaxInclPrice, IsFuelItem, this.NoRTVP))
            //{
            //    sLastError = mPrivateGlobals.theSystem.teGetLastError(); //"Failed to Add Item: " & oDb.GetLastError()
            //    return returnValue;
            //}

            //oItem.GetProductType(ref ProductType);

            //// Validation
            //if (policyManager.SITE_RTVAL && !this.NoRTVP) // And Not Me.NoRTVP  
            //{
            //    if (ProductType == mPrivateGlobals.teProductEnum.eCigarette | ProductType == mPrivateGlobals.teProductEnum.eCigar | ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco)
            //    {
            //        ret = System.Convert.ToInt16(Variables.RTVPService.SetItemUPC(oItem.LineItem, sProductKey, (int)ProductType));
            //        modStringPad.WriteToLogFile("Response is " + System.Convert.ToString(ret) + " from SetItemUPC sent with parameters " + System.Convert.ToString(oItem.LineItem) + "," + sProductKey + "," + System.Convert.ToString(ProductType));
            //        FormatString = "0.00";
            //    }
            //    else
            //    {
            //        oItem.stockcode = stockcode;
            //        ///            ret = RTVPService.SetItemUPC(oItem.LineItem, StockCode, ProductType)
            //        ret = System.Convert.ToInt16(Variables.RTVPService.SetItemUPC(oItem.LineItem, oItem.ItemUPC, (int)ProductType));
            //        modStringPad.WriteToLogFile("Response is " + System.Convert.ToString(ret) + " from SetItemUPC sent with parameters " + System.Convert.ToString(oItem.LineItem) + "," + oItem.ItemUPC + "," + System.Convert.ToString(ProductType));
            //        FormatString = "0.000";
            //    }
            //    if (ret != 0)
            //    {
            //        mvarRTVPCommand = "SetItemUPC";
            //        mvarRTVPError = true;
            //        mvarRTVPResponse = ret;
            //        return returnValue;
            //    }
            //    else
            //    {
            //        mvarRTVPError = false;
            //    }

            //    ret = System.Convert.ToInt16(Variables.RTVPService.SetItemTotal(oItem.LineItem,
            //        double.Parse((dQuantity * oItem.GetTaxFreePrice()).ToString(FormatString))));
            //    modStringPad.WriteToLogFile("Response is " + System.Convert.ToString(ret) + " from SetItemTotal sent with parameters " + System.Convert.ToString(oItem.LineItem) + "," +
            //        (dQuantity * oItem.GetTaxFreePrice()).ToString(FormatString));
            //    if (ret != 0)
            //    {
            //        mvarRTVPCommand = "SetItemTotal";
            //        mvarRTVPError = true;
            //        mvarRTVPResponse = ret;
            //        return returnValue;
            //    }
            //    else
            //    {
            //        mvarRTVPError = false;
            //    }
            //}
            ////   end

            //UnitsPerPkg = oItem.GetUnitsPerPkg();
            //Quantity = dQuantity * UnitsPerPkg;

            //// validation
            //if (policyManager.SITE_RTVAL && !this.NoRTVP) // And Not Me.NoRTVP  
            //{
            //    // based on email from SITE all products need SetItemEquivalence including fuel
            //    // for fuel we decided to set Equivalence to 1, meaning send the quantity
            //    if (ProductType == mPrivateGlobals.teProductEnum.eCigarette | ProductType == mPrivateGlobals.teProductEnum.eCigar | ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco)
            //    {
            //        dblEquivalent = mPrivateGlobals.theSystem.teGetCigaretteEquivalentUnits((mPrivateGlobals.teTobaccoEnum)System.Convert.ToInt16(ProductType));
            //    }
            //    else
            //    {
            //        dblEquivalent = 1;
            //    }
            //    oItem.Quantity = (float)dQuantity; //  
            //    oItem.ItemEquivalence = (float)dblEquivalent; //  
            //    oItem.UnitsPerPkg = UnitsPerPkg;

            //    ret = System.Convert.ToInt16(Variables.RTVPService.SetItemEquivalence(oItem.LineItem, double.Parse((Quantity * dblEquivalent).ToString(FormatString))));
            //    modStringPad.WriteToLogFile("Response is " + System.Convert.ToString(ret) + " from SetItemEquivalence sent with parameters " + System.Convert.ToString(oItem.LineItem) + "," +
            //        (Quantity * dblEquivalent).ToString(FormatString));
            //    if (ret != 0)
            //    {
            //        mvarRTVPCommand = "SetItemEquivalence";
            //        mvarRTVPError = true;
            //        mvarRTVPResponse = ret;
            //        return returnValue;
            //    }
            //    else
            //    {
            //        mvarRTVPError = false;
            //    }
            //}
            //else
            //{
            //    // Nicolette, April 26 end
            //    petreatyNo.GetQuota(ref ProductType, ref quota);
            //}

            ////   added the If condition; real time validation for SITE doesn't require next code
            //if (!policyManager.SITE_RTVAL)
            //{
            //    GetLimitType(ProductType, ref limitMaxType, ref limitThresholdType);
            //    mPrivateGlobals.theSystem.teGetLimit(limitMaxType, ref limitMax);
            //    mPrivateGlobals.theSystem.teGetLimit(limitThresholdType, ref limitThreshold);

            //    if (ProductType == mPrivateGlobals.teProductEnum.eGasoline | ProductType == mPrivateGlobals.teProductEnum.eDiesel | ProductType == mPrivateGlobals.teProductEnum.ePropane | ProductType == mPrivateGlobals.teProductEnum.emarkedGas | ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel) // hen
            //    {

            //        if (isFirstIterationF == true)
            //        {
            //            runningQuotaF = (float)(runningQuotaF + Quantity + quota);
            //            isFirstIterationF = false;
            //        }
            //        else
            //        {
            //            runningQuotaF = (float)(runningQuotaF + Quantity);
            //        }

            //        //   added next two lines, for FNGTR Gasoline Single Transaction Limit is the only validation required
            //        if (policyManager.TAX_EXEMPT_FNGTR)
            //        {
            //            if (runningQuotaF > limitMax)
            //            {
            //                isOverlimitF = true;
            //            }
            //            else
            //            {
            //                isOverlimitT = false;
            //            }
            //            isOverlimitT = false;
            //        }
            //        else
            //        {
            //            if (runningQuotaF > limitThreshold)
            //            {
            //                isOverlimitF = true;

            //                
            //                
            //            }
            //            else if (Quantity > limitMax)
            //            {
            //                

            //                isOverlimitF = true;
            //            }
            //            else
            //            {
            //                isOverlimitF = false;
            //            }
            //        }
            //        if (!mPrivateGlobals.theSystem.IsLimitRequired(ProductType))
            //        {
            //            isOverlimitF = false;
            //        }

            //    }
            //    else if (ProductType == mPrivateGlobals.teProductEnum.eCigarette | ProductType == mPrivateGlobals.teProductEnum.eCigar | ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco)
            //    {
            //        if (isFirstIterationT == true)
            //        {
            //            runningQuotaT = (short)(runningQuotaT + Quantity + quota);
            //            isFirstIterationT = false;
            //        }
            //        else
            //        {
            //            runningQuotaT = (short)(runningQuotaT + Quantity);
            //        }

            //        if (runningQuotaT > limitThreshold)
            //        {
            //            isOverlimitT = true;
            //        }
            //        else if (runningQuotaT > limitMax)
            //        {
            //            isOverlimitT = true;
            //        }
            //        else
            //        {
            //            isOverlimitT = false;
            //        }

            //        
            //        
            //        if (!mPrivateGlobals.theSystem.IsLimitRequired(ProductType))
            //        {
            //            isOverlimitT = false;
            //        }

            //    }
            //}
            //// real time validation end if '  

            //pecolItems.Add(oItem, null, null, null); //save the item in purchase list

            //peitemCount++;

            //
            //
            //
            //peQuantity = Quantity;
            //

            //psProductKey = sProductKey;

            //returnValue = true;

          //  return returnValue;
        //}
        //returns true if either tobacco or gas is over limit anywhere
        public bool IsOverLimit()
        {
            bool returnValue = false;

            returnValue = peisOverLimit;

            return returnValue;
        }

        

        public bool IsSuccess
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsSuccess;
                return returnValue;
            }
            set
            {
                mvarIsSuccess = value;
            }
        }

        //   changed these two function to properties and added Let

        public bool IsFuelOverLimit
        {
            get
            {
                bool returnValue = false;
                //Public Function IsFuelOverLimit() As Boolean
                returnValue = isOverlimitF;
                return returnValue;
            }
            set
            {
                isOverlimitF = value;
            }
        }


        public bool IsTobaccoOverLimit
        {
            get
            {
                bool returnValue = false;
                //Public Function IsTobaccoOverLimit() As Boolean
                returnValue = isOverlimitT;
                return returnValue;
            }
            set
            {
                isOverlimitT = value;
            }
        }

        
        public float TotalExemptedTax
        {
            get
            {
                float returnValue = 0;
                returnValue = mTotalExemptedTax;
                return returnValue;
            }
            set
            {
                mTotalExemptedTax = value;
            }
        }
        

        //   read only properties error handling for RTVP (real time validation dll)
        public string RTVPCommand
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRTVPCommand;
                return returnValue;
            }
            set { mvarRTVPCommand = value; }
        }

        public bool RTVPError
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarRTVPError;
                return returnValue;
            }
            set { mvarRTVPError = value; }
        }

        public short RTVPResponse
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarRTVPResponse;
                return returnValue;
            }
            set { mvarRTVPResponse = value; }
        }
        //   end

        //  

        public bool NoRTVP
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarNoRTVP;
                return returnValue;
            }
            set
            {
                mvarNoRTVP = value;
            }
        }
        

        public bool Show()
        {
            //TODO 
            // Chaps_Main.overPurchaseList = this;

            // TODO: Returning true because of no use of the return value
            return true;
        }

        
        public tePurchaseItem Item(short I)
        {
            tePurchaseItem returnValue = default(tePurchaseItem);
            returnValue = pecolItems[I] as tePurchaseItem;
            return returnValue;
        }

        //Description: Returns information about the last error encountered by this
        // class.  Returns "no error" if no error has occurred.
        public string GetLastError()
        {
            string returnValue = "";
            returnValue = sLastError;
            return returnValue;
        }

        public void SetLastError(string error)
        {
            sLastError = error;
        }

        //Description: Raises an error if the class has not been initialized.
        private void CheckInit()
        {
            if (!bIsInit)
            {
                Information.Err().Raise(mPrivateGlobals.kPurchaseListAlreadyInitNum, null, mPrivateGlobals.kPurchaseListAlreadyInitDesc, null, null);
            }
        }
        //   end

        //private dynamic GetLimitType(mPrivateGlobals.teProductEnum ProductType, ref mPrivateGlobals.teLimitEnum limitMax, ref mPrivateGlobals.teLimitEnum limitThreshold)
        //{

        //    if ((ProductType) == mPrivateGlobals.teProductEnum.eCigarette)
        //    {
        //        limitMax = mPrivateGlobals.teLimitEnum.eCigLimit;
        //        limitThreshold = mPrivateGlobals.teLimitEnum.eCigMaxThreshhold;
        //        return default(dynamic);
        //    }
        //    else if ((ProductType) == mPrivateGlobals.teProductEnum.eCigar)
        //    {
        //        limitMax = mPrivateGlobals.teLimitEnum.eCigarLimit;
        //        limitThreshold = mPrivateGlobals.teLimitEnum.eCigarMaxThreshhold;
        //        return default(dynamic);
        //    }
        //    else if ((ProductType) == mPrivateGlobals.teProductEnum.eLooseTobacco)
        //    {
        //        limitMax = mPrivateGlobals.teLimitEnum.eTobaccoLimit;
        //        limitThreshold = mPrivateGlobals.teLimitEnum.eTobaccoMaxThreshhold;
        //        return default(dynamic);
        //    }
        //    else if (((ProductType) == mPrivateGlobals.teProductEnum.eGasoline) || ((ProductType) == mPrivateGlobals.teProductEnum.emarkedGas))
        //    {
        //        limitMax = mPrivateGlobals.teLimitEnum.eGasTransactionLimit; // 
        //        limitThreshold = mPrivateGlobals.teLimitEnum.eGasLimit;
        //        return default(dynamic);
        //    }
        //    else if (((ProductType) == mPrivateGlobals.teProductEnum.eDiesel) || ((ProductType) == mPrivateGlobals.teProductEnum.emarkedDiesel))
        //    {
        //        limitMax = mPrivateGlobals.teLimitEnum.eDieselTransactionLimit; // 
        //        limitThreshold = mPrivateGlobals.teLimitEnum.eDieselLimit;
        //        return default(dynamic);
        //    }
        //    else if ((ProductType) == mPrivateGlobals.teProductEnum.ePropane)
        //    {
        //        limitMax = mPrivateGlobals.teLimitEnum.ePropaneTransactionLimit;
        //        limitThreshold = mPrivateGlobals.teLimitEnum.ePropaneLimit;
        //        return default(dynamic);
        //    }

        //    // TODO: Returning null because of no use of the returned value - Ipsit_14
        //    return null;
        //}
        //   end
    }
}
