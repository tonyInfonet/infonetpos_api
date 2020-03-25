namespace Infonet.CStoreCommander.Entities
{
    public class teTreatyNo
    {        
        private bool bIsInit;
        private bool pbSwiped;
        public bool IsInit
        {
            get
            {
                return bIsInit;
            }
            set
            {
                bIsInit = value;
            }
        }
        private string sLastError;

        private string psTreatyNo; //p is for private.  sTreatyNo is used lots.
        private string mvarName; //  
        private bool mvarValidTreatyNo; //   to determine if the customer is a valid tax exempt customer validated by real time validation system
        private string mvarOverrideNumber; //   to pass OverrideNumber from treaty form to process_site, changed on Oct 27, 2011 from Integer to String based on new requirement
        private short mvarOverrideReason; //   to pass OverrideReason from treaty form to process_site
        private bool mvarSendOverride; //   to determine if cashier clicked override in treaty form
        private double mvarRemainingTobaccoQuantity; //   to keep the value returned by SITE
        private double mvarRemainingFuelQuantity; //   to keep the value returned by SITE
        private double mvarReaminingCigaretteEquivalentUnits; //   to keep the converted value returned by SITE
        private bool mvarOverrideRequired; //   for SITE validation.Logic: if any of tobacco or fuel items are over limit, force the cashier to go back to the SaleMain and remove the items overlimit. To process a sale with items over limit POS needs another transaction (this is Sask. Finance requirement, POS just use it).
        private bool mvarRemoveTax; //   for SITE real time validation for new button Remove Tax in Treaty form
        private string mvarPhoneNumber; //   for FNGTR to keep phone number if over limit


        private void Class_Initialize_Renamed()
        {
            bIsInit = false;
            sLastError = "No Error";
        }
        public teTreatyNo()
        {
            Class_Initialize_Renamed();
        }

        


        public dynamic IsValid()
        {
            dynamic returnValue = default(dynamic);
            CheckInit();


            var captureMethod = (short)1;
            returnValue = mPrivateGlobals.theSystem.teIsValidTreatyNo(ref psTreatyNo, ref captureMethod);


            return returnValue;
        }


        
        public string GetInvalidWarning()
        {
            string returnValue = "";
            CheckInit();

            returnValue = "This treaty number is invalid.";
            return returnValue;
        }


       
        public string GetLastError()
        {
            string returnValue = "";
            returnValue = sLastError;
            return returnValue;
        }


      
        public void CheckInit()
        {
            if (bIsInit)
            {
            }
        }
       

        public string GetTreatyNo()
        {
            string returnValue = "";

            returnValue = psTreatyNo;

            return returnValue;
        }


        public string TreatyNumber
        {
            get { return psTreatyNo; }
            set
            {
                psTreatyNo = value;
            }
        }


        public bool isSwiped
        {
            get
            {
                bool returnValue = false;

                returnValue = pbSwiped;

                return returnValue;
            }
            set
            {
                pbSwiped = value;
            }
        }

        public string Name { get; set; }
        

        public bool ValidTreatyNo
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarValidTreatyNo;
                return returnValue;
            }
            set
            {
                mvarValidTreatyNo = value;
            }
        }


        public bool SendOverride
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarSendOverride;
                return returnValue;
            }
            set
            {
                mvarSendOverride = value;
            }
        }


        public string OverrideNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarOverrideNumber;
                return returnValue;
            }
            set
            {
                mvarOverrideNumber = value;
            }
        }


        public short OverrideReason
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarOverrideReason;
                return returnValue;
            }
            set
            {
                mvarOverrideReason = value;
            }
        }

        public double RemainingFuelQuantity
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarRemainingFuelQuantity;
                return returnValue;
            }
            set
            {
                mvarRemainingFuelQuantity = value;


            }
        }


        public double RemainingTobaccoQuantity
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarRemainingTobaccoQuantity;
                return returnValue;
            }
            set
            {
                mvarRemainingTobaccoQuantity = value;

            }
        }

        public bool OverrideRequired
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarOverrideRequired;
                return returnValue;
            }
            set
            {
                mvarOverrideRequired = value;
            }
        }

        public bool RemoveTax
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarRemoveTax;
                return returnValue;
            }
            set
            {
                mvarRemoveTax = value;
            }
        }


        public string PhoneNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPhoneNumber;
                return returnValue;
            }
            set
            {
                mvarPhoneNumber = value;
            }
        }

    }
}
