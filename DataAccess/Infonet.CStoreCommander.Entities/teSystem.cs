using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    [System.Runtime.InteropServices.ProgId("teSystem_NET.teSystem")]
    public class teSystem : IDisposable
    {
                private string sLastError;
        private bool bInit;

        private float mvarGasLimit;
        private float mvarPropaneLimit;
        private float mvarTobaccoLimit;

        private TaxExemptReasons mvarGasReasons;
        private TaxExemptReasons mvarPropaneReasons;
        private TaxExemptReasons mvarTobaccoReasons;

        private string mvarRetailer;
        private string mvarVoucherFooter;
        private string mvarTaxCertifyCode;
        private string mvarTaxProgram;
        private string mvarRetailerID;
        private int mvarRTVP_TimeOut;




        public teSystem()
        {
            
        }

        private void Class_Terminate_Renamed()
        {
        }

        public void Dispose()
        {
        }

        ~teSystem()
        {
            Class_Terminate_Renamed();
        }

        public string SLastError
        {
            get { return sLastError; }
            set { sLastError = value; }
        }

        

       

        public double teGetCigaretteEquivalentUnits(mPrivateGlobals.teTobaccoEnum eTobaccoProduct)
        {
            double returnValue = 0;
            return returnValue;
        }


        public bool teGetLimit(mPrivateGlobals.teLimitEnum eLimit, ref double dOutLimit)
        {
            bool returnValue = false;
            returnValue = false;
            return returnValue;
        }

        
        //Name: teGetTaxFreePrice
        //Description: Returns the tax free unit price of a given product.
        //To get the TaxFree price of a gas or diesel item, set sProductKey to the
        // result of teSystem.teMakeFuelKey()
        //Inputs: sProductKey, a string containing a product key.  Either a StockCode,
        //   or a compound fuel key. (eg: "Fuel:G:1,T:2,L:3").
        //Outputs: dOutPrice: The taxexempt price, or 0 if no price was found.
        //         eOutCategory, the product category (or eNone if none was found).
        //         bFound: True if a tax exempt price was found, false otherwise.
        //Returns: False if there was an error, True if function ran ok.

        public bool teGetTaxFreeFuelPrice(ref string sProductKey, ref double CashPrice, ref double CreditPrice, ref bool bFound)
        {
            bool returnValue = false;
            
            return returnValue;
        }

        private bool teGetTaxFreeFuelPriceByPrice(ref string sProductKey, ref double CashPrice, ref double CreditPrice, ref bool bFound)
        {
            bool returnValue = false;
            
            return returnValue;
        }

        
        private bool teGetTaxFreeFuelPriceByRate(ref string sProductKey, ref double CashPrice, ref double CreditPrice, ref bool bFound)
        {
            bool returnValue = false;            

            return returnValue;
        }

                //Name: teGetTaxFreePrice
        //Description: Returns the tax free unit price of a given product.
        //To get the TaxFree price of a gas or diesel item, set sProductKey to the
        // result of teSystem.teMakeFuelKey()
        //Inputs: sProductKey, a string containing a product key.  Either a StockCode,
        //   or a compound fuel key. (eg: "Fuel:G:1,T:2,L:3").
        //Outputs: dOutPrice: The taxexempt price, or 0 if no price was found.
        //         eOutCategory, the product category (or eNone if none was found).
        //         bFound: True if a tax exempt price was found, false otherwise.
        //Returns: False if there was an error, True if function ran ok.

        public bool teGetTaxFreePrice(string sProductKey, float OriginalPrice, float dOutPrice, float dOutUnitsPerPkg, mPrivateGlobals.teProductEnum eOutCategory, string sOutUpcCode, bool bFound, string stockcode, string ProductCode, float OutTaxExemptRate)
        {
            bool returnValue = false;
           
            return returnValue;
        }

        private bool teGetTaxFreePriceByPrice(ref string sProductKey, double OriginalPrice, ref float dOutPrice, ref float dOutUnitsPerPkg, ref mPrivateGlobals.teProductEnum eOutCategory, ref string sOutUpcCode, ref bool bFound, string stockcode)
        {
            bool returnValue = false;


            return returnValue;
        }

        
        private bool teGetTaxFreePriceByRate(ref string sProductKey, float OriginalPrice, ref float dOutPrice, ref float dOutUnitsPerPkg, ref mPrivateGlobals.teProductEnum eOutCategory, ref string sOutUpcCode, ref bool bFound, ref float OutTaxExemptRate, string stockcode, ref string ProductCode) 
        {
            bool returnValue = false;
            

            return returnValue;
        }

        
        public float RoundToHighCent(float Value)
        {
            float returnValue = 0;
            
            
            returnValue = (float)((double.Parse((Conversion.Int(Value * 100) / 100).ToString("#0.00"))) + (((Conversion.Int(Value * 1000) % 10) != 0) ? 0.01 : 0));
            
            return returnValue;
        }
        

        
        //Author: Nancy, Nov.2nd,2004
        //Name: teGetTaxFreeGradePriceIncrement
        //Description: Returns the tax free unit price increment of a given grade.
        //Inputs: GradeID
        //Outputs: cashPriceIncre: The taxexempt cash price increment, or 0 if no price was found.
        //         creditPriceIncre: The taxexempt credit price increment, or 0 if no price was found.
        //         bFound: True if the TE grade was found, false otherwise.
        //Returns: False if there was an error, True if function ran ok.

        public bool teGetTaxFreeGradePriceIncrement(short GradeID, ref double cashPriceIncre, ref double creditPriceIncre, ref bool bFound)
        {
            bool returnValue = false;
                       return returnValue;
        }

        
        //Author: Nancy, Nov.2nd,2004
        //Name: teSetTaxFreeGradePriceIncrement
        //Description: Returns the tax free unit price increment of a given grade.
        //Inputs: GradeID, cashPriceIncre, creditPriceIncre
        //Returns: False if there was an error, True if function ran ok.

        public bool teSetTaxFreeGradePriceIncrement(short GradeID, double cashPriceIncre, double creditPriceIncre)
        {
            bool returnValue = false;
           
            return returnValue;
        }

        
        //Author: Nancy, Nov.2nd,2004
        //Name: teGetTaxFreeTierLevelPriceDiff
        //Description: Returns the tax free unit price increment of a given Tier/Level combination.
        //Inputs: Tier and Level
        //Outputs: cashPriceIncre: The taxexempt cash price increment, or 0 if no price was found.
        //         creditPriceIncre: The taxexempt credit price increment, or 0 if no price was found.
        //         bFound: True if the TE grade was found, false otherwise.
        //Returns: False if there was an error, True if function ran ok.

        public bool teGetTaxFreeTierLevelPriceDiff(short Tier, short Level, ref double cashPriceIncre, ref double creditPriceIncre, ref bool bFound)
        {
            bool returnValue = false;
            
            return returnValue;
        }

        
        //Author: Nancy, Nov.2nd,2004
        //Name: teSetTaxFreeTierLevelPriceDiff
        //Description: Returns the tax free unit price increment of a given grade.
        //Inputs: Tier,Level, cashPriceIncre, creditPriceIncre
        //Returns: False if there was an error, True if function ran ok.
        public bool teSetTaxFreeTierLevelPriceDiff(short Tier, short Level, double cashPriceIncre, double creditPriceIncre)
        {
            bool returnValue = false;

            returnValue = false;            
            return returnValue;
        }

        
        //Name: teIsValidTreatyNo
        //Description: This is a convenience function in case a treaty number needs to
        // be checked without the bother of instantiating a class to do it.
        //Pre/Inputs: sTreatyNo, the treaty number to be checked.
        //Post/Outputs: Returns true if the treaty number is valid, false otherwise.

        public bool teIsValidTreatyNo(ref string sTreatyNo, ref short captureMethod) //   - RTVP return was failing - maybe becuase of zero capture method
        {
            bool returnValue = false;            
            return returnValue;
        }

        
        //Name: teMakeFuelKey
        //Description: This is a convenience function used to generate a string variable
        //for Fuel products.  This string variable can be passed to tePurchaseItem.Init
        // for the sake of initializing the purchase item with a gas price.
        //Pre/Inputs: iGradeID, iTierId & iLevelID
        //Post/Outputs: The return value will be a string formatted as
        //   "Fuel:G:1,T:2,L:3", where 1,2 and 3 are the GradeID, TierId and LevelID.
        //Remarks:

        public string teMakeFuelKey(short iGradeID, short iTierId, short iLevelID)
        {
            string returnValue = "";

            returnValue = "Fuel:G:" + (iGradeID).ToString() + ",T:" + (iTierId).ToString() + ",L:" + (iLevelID).ToString();

            return returnValue;
        }


        
        //Name: teExtractFuelKey
        //Description: Extracts the Grade,Level and Tier from a previously created
        //   fuel key.
        //Pre/Inputs: sCompoundKey: a key made by calling teMakeFuelKey,
        //    eg "Fuel:G:1,T:2,L:3"
        //Post/Outputs: Returns True if key was extracted ok, false if there was a
        //   problem (check teGetLastError() )

        public bool teExtractFuelKey(ref string sCompoundKey, ref short iGradeID, ref short iTierId, ref short iLevelID)
        {
            bool returnValue = false;

            returnValue = false;
            try
            {
                short iPos = 0;
                short iOldPos = 0;
                string sTempError = "";
                sTempError = sLastError; //Anticipate something going wrong
                sLastError = "Failed to extract fuel key from \"" + sCompoundKey + "\"";

                if (!teIsFuelKey(sCompoundKey))
                {
                    return returnValue;
                }

                iOldPos = (short)(sCompoundKey.IndexOf(":G:") + 1);
                if (iOldPos == 0)
                {
                    return returnValue;
                }
                iOldPos = (short)(iOldPos + ":G:".Length); //iOldPos now points at 1 past ":G:"

                iPos = (short)(sCompoundKey.IndexOf(",T:") + 1);
                if (iPos == 0)
                {
                    return returnValue;
                }

                iGradeID = short.Parse(sCompoundKey.Substring(iOldPos - 1, iPos - iOldPos));

                iOldPos = (short)(iPos + ",T:".Length);
                iPos = (short)(sCompoundKey.IndexOf(",L:", iPos + 1 - 1) + 1);
                if (iPos == 0)
                {
                    return returnValue;
                }

                iTierId = short.Parse(sCompoundKey.Substring(iOldPos - 1, iPos - iOldPos));

                iPos = (short)(iPos + ",T:".Length);
                iLevelID = short.Parse(sCompoundKey.Substring(iPos - 1, (sCompoundKey.Length + 1) - iPos));

                sLastError = sTempError;
                returnValue = true;
            }
            catch (Exception ex)
            {
                //ExcludeSE
                sLastError = sLastError + " " + ex.HResult.ToString() + ":" + ex.Message;
            }
            return returnValue;
        }


       

        public bool teSetTaxFreePrice(ref string sProductKey, ref mPrivateGlobals.teProductEnum eProductCategory, ref double dNewPrice, ref string sEmpId, ref double dUnitsPerPackage, ref string sUpcCode)
        {
            bool returnValue = false;

            returnValue = false;

            string sSql;
            if (teIsFuelKey(sProductKey))
            {
                returnValue = SetTaxFreeFuelPrice(ref sProductKey, eProductCategory, dNewPrice, 0, sEmpId);
            }
            else
            {
                returnValue = SetTaxFreeInventoryPrice(sProductKey, eProductCategory, dNewPrice, sEmpId, dUnitsPerPackage, sUpcCode);
            }

            return returnValue;
        }


        
        //Name: teGetLastError
        //Description: Returns a detailed error message should an error be encountered.
        //  Default value is: "No Error"
        //Preconditions: Meant to be called when an error occurs.

        public string teGetLastError()
        {
            string returnValue = "";
            returnValue = sLastError;
            return returnValue;
        }


        public bool teIsFuelKey(string sProductKey)
        {
            bool returnValue = false;

            returnValue = false;
            short iPos = 0;
            short I = 0;


            iPos = (short)(sProductKey.IndexOf("Fuel:G:") + 1); //Must start with "Fuel:G:"
            if (iPos != 1)
            {
                return returnValue;
            }

            for (I = 1; I <= 4; I++) //Look for at least 4 colons
            {
                iPos = (short)(sProductKey.IndexOf(":", iPos + 1 - 1) + 1);
                if (iPos == 0)
                {
                    return returnValue; //if colon not found then return false
                }
            }

            returnValue = true;
            return returnValue;
        }


        public bool SetTaxFreeFuelPrice(ref string sProductKey, mPrivateGlobals.teProductEnum eProductCategory, double dNewCashPrice, double dNewCreditPrice, string sEmpId)
        {
            bool returnValue = false;

            returnValue = false;

            return returnValue;
        }


        private bool SetTaxFreeInventoryPrice(string sProductKey, mPrivateGlobals.teProductEnum eProductCategory, double dNewPrice, string sEmpId, double dUnitsPerPackage, string sUpcCode)
        {
            bool returnValue = false;

            returnValue = false;
            return returnValue;
        }


        public float GasLimit
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarGasLimit;
                return returnValue;
            }
            set
            {
                mvarGasLimit = value;
            }
        }


        public float PropaneLimit
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarPropaneLimit;
                return returnValue;
            }
            set
            {
                mvarPropaneLimit = value;
            }
        }


        public float TobaccoLimit
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTobaccoLimit;
                return returnValue;
            }
            set
            {
                mvarTobaccoLimit = value;
            }
        }

        public TaxExemptReasons GasReasons
        {
            get
            {
                TaxExemptReasons returnValue = default(TaxExemptReasons);
                returnValue = mvarGasReasons;
                return returnValue;
            }
            set { mvarGasReasons = value; }

        }

        public TaxExemptReasons TobaccoReasons
        {
            get
            {
                TaxExemptReasons returnValue = default(TaxExemptReasons);
                returnValue = mvarTobaccoReasons;
                return returnValue;
            }
            set { mvarTobaccoReasons = value; }
        }

        public TaxExemptReasons PropaneReasons
        {
            get
            {
                TaxExemptReasons returnValue = default(TaxExemptReasons);
                returnValue = mvarPropaneReasons;
                return returnValue;
            }
            set { mvarPropaneReasons = value; }
        }

        public string Retailer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRetailer;
                return returnValue;
            }
            set { mvarRetailer = value; }
        }

        public string VoucherFooter
        {
            get
            {
                string returnValue = "";
                returnValue = mvarVoucherFooter;
                return returnValue;
            }
            set { mvarVoucherFooter = value; }
        }
        public string TaxCertifyCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTaxCertifyCode;
                return returnValue;
            }
            set { mvarTaxCertifyCode = value; }
        }

        public string TaxProgram
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTaxProgram;
                return returnValue;
            }
            set { mvarTaxProgram = value; }
        }

        public int RTVP_TimeOut
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarRTVP_TimeOut;
                return returnValue;
            }
            set
            {
                mvarRTVP_TimeOut = value;
            }
        }
    }
}
