using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class tePurchaseItem
    {
        // Programmer: Ashish Mittal                      Date: July 26,2004
        // Environment: Visual Basic 5.0                  File: tePurchaseItem.cls
        // OS: Windows 2k                                 Ver: 1

        // tePurchaseItem:
        // The tePurchaseItem class represents a single tax exempt item in a purchase.

        private const short kNoCodeSet = -1;
        private bool bIsInit;
        private string sLastError;
        private mPrivateGlobals.teProductEnum peProdType;
        private string psProductKey; //
        public float pdQuantity
        {
            get; set;
        }
        private short piRowInSalesMain;
        private float pdOriginalPrice;
        public float pdTaxFreePrice { get; set; }
        private float pdUnitsPerPkg;
        private string psUpcCode;
        private teTreatyNo psTreatyNo;
        
        private string psGradeStockCode;
        private short psGradeID;
        private short psTierID;
        private short psLevelID;
        private bool psIsFuelItem;
        
        private short piOverrideCode;
        private string psOverrideFormNumber;
        private string psOverrideDetails;
        private bool psRequireOverride;
        private int peInvoiceID;
        private short peTillID;
        public float petaxInclPrice { get; set; }
        private bool peIsLimitRequired;
        private float pecurrentQuota;
        private bool mvarWasTaxExemptReturn; 
        private short mvarLineItem; //   Real Time Validation
        private string mvarItemUPC; //   look up for Real Time Validation SITE
        private string mvarStockCode; //   look up for Real Time Validation SITE
        private float mvarQuantity; //  
        private float mvarItemEquivalence; //  
        private double mvarUnitsPerPkg; //  
        private const short kFormNumberNotSet = 0;
        private const string kDetailsNotSet = "Not Set";

        private void Class_Initialize_Renamed()
        {
            bIsInit = false;
            sLastError = "No Error";
            mvarWasTaxExemptReturn = false; 
        }
        public tePurchaseItem()
        {
            Class_Initialize_Renamed();
        }

        public bool IsOverrideDone()
        {
            bool returnValue = false;

            if (piOverrideCode == -1)
            {
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }

            return returnValue;
        }


        public bool OverrideRequired
        {
            get
            {
                bool returnValue = false;

                returnValue = psRequireOverride;

                return returnValue;
            }
            set
            {

                psRequireOverride = value;

            }
        }


        public int InvoiceID
        {
            get { return peInvoiceID; }
            set { peInvoiceID = value; }
        }

        public short TillID
        {
            get { return peTillID; }
            set { peTillID = value; }
        }


        public string LastError
        {
            get { return sLastError; }
            set { sLastError = value; }
        }

        public float TaxFreePrice
        {
            get { return pdTaxFreePrice; }
            set { pdTaxFreePrice = value; }
        }

        public mPrivateGlobals.teProductEnum ProdType
        {
            get { return peProdType; }
            set { peProdType = value; }
        }

        public string UpcCode
        {
            get { return psUpcCode; }
            set { psUpcCode = value; }
        }



        
        
        //
        //Public Function GetTaxFreeUnitPrice() As Single
        //   CheckInit
        //   GetTaxFreeUnitPrice = 1.5
        //
        //End Function

        public float CurrentQuota
        {
            get
            {
                float returnValue = 0;

                returnValue = pecurrentQuota;

                return returnValue;
            }
            set
            {

                pecurrentQuota = value;

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
        

        //  

        public short LineItem
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLineItem;
                return returnValue;
            }
            set
            {
                mvarLineItem = value;
            }
        }


        public bool PsIsFuelItem
        {
            get { return psIsFuelItem; }
            set { psIsFuelItem = value; }
        }
        public string PsGradeStockCode
        {
            get { return psGradeStockCode; }
            set { psGradeStockCode = value; }
        }

        //   end

        //  

        public string ItemUPC
        {
            get
            {
                return mvarItemUPC;
            }
            set
            {
                mvarItemUPC = value;
            }
        }
        //   end

        //  

        public string stockcode
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
        //   end

        //   quantity from the sale line

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
        //   end

        //   equivalence for the item

        public float ItemEquivalence
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarItemEquivalence;
                return returnValue;
            }
            set
            {
                mvarItemEquivalence = value;
            }
        }
        //   end

        //   units per package for the item

        public float UnitsPerPkg
        {
            get
            {
                float returnValue = 0;
                returnValue = pdUnitsPerPkg;
                return returnValue;
            }
            set
            {
                pdUnitsPerPkg = value;
            }
        }


        //Description: The Init method is used to initialize this class.  Calling any
        // other methods of this class without calling the Init method first will
        // raise an error.
        //Inputs:
        //  eProdType:  The type of product, eg eDiesel.
        //  sProductKey: The primary key used to identify this product in the existing
        //    C-Store commander database.  (The Primary Key in C-Store database).
        //  dQuantity: The amount of the item being purchased.
        //Remarks: If the product is a gasoline or diesel product, set the sProductKey
        //  parameter to "Fuel: G:g,T:t,L:l" Where g, t and l are the GradeId, TierId
        //  and the LevelId.     Eg: "Fuel: G:1,T:2,L:4"

        //public bool Init(teTreatyNo oTreatyNo, ref string sProductKey, double dOriginalPrice, double dQuantity, short iRowNumberInSalesMainForm, int invoiceID, short TillID, ref string stockcode, double TaxInclPrice, bool IsFuelItem, bool NoRTVP)
        // {
        //  bool returnValue = false;

        //peInvoiceID = invoiceID;
        //peTillID = TillID;

        //returnValue = false;

        //bool bFound = false;

        //if (!mPrivateGlobals.theSystem.teGetTaxFreePrice(sProductKey, (float)dOriginalPrice, pdTaxFreePrice, pdUnitsPerPkg, peProdType, psUpcCode, bFound, stockcode, "", 0))
        //{
        //    sLastError = mPrivateGlobals.theSystem.teGetLastError();
        //    return returnValue;
        //}

        //if (!bFound)
        //{
        //    //      sLastError = "Error: Not a tax-free item: " & sProductKey
        //    return returnValue;
        //    //   Real Time Validation; line item for the next UPC line item in the transaction
        //}
        //else
        //{
        //    if (policyManager.SITE_RTVAL && !NoRTVP) //   And Not NoRTVP condition
        //    {
        //        mvarLineItem = System.Convert.ToInt16(Variables.RTVPService.AddLineItem());
        //        modStringPad.WriteToLogFile("Response is " + System.Convert.ToString(mvarLineItem) + " from AddLineItem sent with no parameters");
        //    }
        //}
        ////   end
        //mvarQuantity = (float)dQuantity; //  

        //
        //if (IsFuelItem)
        //{
        //    psIsFuelItem = true;
        //    psGradeStockCode = stockcode;
        //    mPrivateGlobals.theSystem.teExtractFuelKey(ref sProductKey, ref psGradeID, ref psTierID, ref psLevelID);
        //}
        //else
        //{
        //    psIsFuelItem = false;
        //}
        //

        //psProductKey = sProductKey;
        //pdQuantity = (float)dQuantity;
        //piRowInSalesMain = iRowNumberInSalesMainForm;
        //petaxInclPrice = (float)dOriginalPrice;

        //
        //pdOriginalPrice = (float)dOriginalPrice;
        //

        //piOverrideCode = kNoCodeSet;
        //psOverrideFormNumber = (kFormNumberNotSet).ToString();

        //psOverrideDetails = kDetailsNotSet;

        //petaxInclPrice = (float)TaxInclPrice;

        ////    pdQuantity = pdQuantity * pdUnitsPerPkg

        //psTreatyNo = oTreatyNo;

        //bIsInit = true;
        //returnValue = true;
        ////   added next if, for real time validation this in not required
        //if (!policyManager.SITE_RTVAL)
        //{
        //    peIsLimitRequired = mPrivateGlobals.theSystem.IsLimitRequired(peProdType);
        //}

        //   return returnValue;
        // }


        public bool IsLimitRequired()
        {
            bool returnValue = false;

            returnValue = peIsLimitRequired;

            return returnValue;
        }

        //Description: Returns the tax free price of an item by multiplying its
        // quantity by its tax-free unit price.

        public float GetTaxFreePrice()
        {
            float returnValue = 0;
            CheckInit();
            returnValue = pdTaxFreePrice;
            return returnValue;
        }

        //  set original price
        public dynamic SetOriginalPrice(float Value)
        {
            CheckInit();
            pdOriginalPrice = Value;

            // TODO: No Return value - Ipsit_9
            return pdOriginalPrice;

        }
        //shinyend
        public dynamic SetTaxFreePrice(float Value)
        {
            CheckInit();
            pdTaxFreePrice = Value;

            // TODO: No Return value - Ipsit_10
            return pdTaxFreePrice;
        }

        
        public void SetQuantity(float Value)
        {
            pdQuantity = Value;
            petaxInclPrice = (float)(Math.Round(pdQuantity * pdOriginalPrice, 2));
        }



        //Description: Returns the tax free price of an item by multiplying its
        // quantity by its tax-free unit price.

        public float GetQuantity()
        {
            float returnValue = 0;
            CheckInit();
            returnValue = pdQuantity;
            return returnValue;
        }

        //Description: Sets iOutputCode to the Override code.  Returns False is no override
        // code was set.
        public float GetUnitsPerPkg()
        {
            float returnValue = 0;
            CheckInit();
            returnValue = pdUnitsPerPkg;
            return returnValue;
        }


        public bool GetOverrideCode(ref short iOutputCode)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            if (piOverrideCode != kNoCodeSet)
            {
                iOutputCode = piOverrideCode;
                returnValue = true;
            }

            return returnValue;
        }

        public bool GetProductKey(ref string iKey)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            iKey = psProductKey;

            returnValue = true;

            return returnValue;
        }

        
        public bool GetGradeStockCode(ref string iKey)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            iKey = psGradeStockCode;

            returnValue = true;

            return returnValue;
        }

        public bool GetGradeID(ref short iKey)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            iKey = psGradeID;

            returnValue = true;

            return returnValue;
        }

        public bool GetTierID(ref short iKey)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            iKey = psTierID;

            returnValue = true;

            return returnValue;
        }

        public bool GetIsFuelItem(ref bool iKey)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            iKey = psIsFuelItem;

            returnValue = true;

            return returnValue;
        }

        public bool GetLevelID(ref short iKey)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            iKey = psLevelID;

            returnValue = true;

            return returnValue;
        }
        

        public bool GetProductType(ref mPrivateGlobals.teProductEnum iType)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            iType = peProdType;

            returnValue = true;

            return returnValue;
        }

        //Description: Sets sOutputFormNumber to the number of the override form used to
        // override a tax exempt limit.  Returns False is no form number or override code
        // was provided.

        public bool GetOverrideFormNumber(ref string sOutputFormNumber)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            if (double.Parse(psOverrideFormNumber) != kFormNumberNotSet)
            {
                sOutputFormNumber = psOverrideFormNumber;
                returnValue = true;
            }

            return returnValue;
        }

        public bool GetOverrideDetails(ref string sOutputDetails)
        {
            bool returnValue = false;
            CheckInit();

            returnValue = false;

            if (psOverrideDetails != kDetailsNotSet)
            {
                sOutputDetails = psOverrideDetails;
                returnValue = true;
            }

            return returnValue;
        }

        public void SetOverride(short overrideCode, string overrideFormNumber, string overrideDetails)
        {
            piOverrideCode = overrideCode;
            psOverrideFormNumber = overrideFormNumber;
            psOverrideDetails = overrideDetails;
        }

        public float GetOriginalPrice()
        {
            float returnValue = 0;

            returnValue = pdOriginalPrice;

            return returnValue;
        }

        
        public float GetTaxIncludeAmount()
        {
            float returnValue = 0;
            returnValue = petaxInclPrice;
            return returnValue;
        }
        

        public short GetRowInSalesMain()
        {
            short returnValue = 0;
            CheckInit();
            returnValue = piRowInSalesMain;
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

        

        //Description: Raises an error if the class has not been initialized.
        public void CheckInit()
        {
            if (!bIsInit)
            {
                Information.Err().Raise(Constants.vbObjectError + 15001, null, "TE Purchase Item not set", null, null);
            }
        }

        //added properties
        public string ProductType { get; set; }
        public string TreatyNo { get; set; }

        public short PsGradeIDpsTreatyNo
        {
            get { return psGradeID; }
            set { psGradeID = value; }
        }

        public short PsTierID
        {
            get { return psTierID; }
            set { psTierID = value; }
        }

        public short PsLevelID
        {
            get { return psLevelID; }
            set { psLevelID = value; }
        }

        public string PsProductKey
        {
            get { return psProductKey; }
            set { psProductKey = value; }
        }

        public float PdQuantity
        {
            get { return pdQuantity; }
            set { pdQuantity = value; }
        }

        public short PiOverrideCode
        {
            get { return piOverrideCode; }
            set { piOverrideCode = value; }
        }


        public float PdOriginalPrice
        {
            get { return pdOriginalPrice; }
            set { pdOriginalPrice = value; }
        }


        public short PiRowInSalesMain
        {
            get { return piRowInSalesMain; }
            set { piRowInSalesMain = value; }
        }

        public string PsOverrideDetails
        {
            get { return psOverrideDetails; }
            set { psOverrideDetails = value; }
        }

        public float PetaxInclPrice
        {
            get { return petaxInclPrice; }
            set { petaxInclPrice = value; }
        }

        public bool BIsInit
        {
            get { return bIsInit; }
            set { bIsInit = value; }
        }

        public teTreatyNo PsTreatyNo
        {
            get { return psTreatyNo; }
            set { psTreatyNo = value; }
        }

        public string PsOverrideFormNumber
        {
            get { return psOverrideFormNumber; }
            set { psOverrideFormNumber = value; }
        }

        public bool PeIsLimitRequired
        {
            get { return peIsLimitRequired; }
            set { peIsLimitRequired = value; }
        }


    }
}
