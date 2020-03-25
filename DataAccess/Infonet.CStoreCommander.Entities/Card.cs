// VBConversions Note: VB project level imports
//using AxccrpMonthcal6;
//using AxCCRPDTP6;
// End of VB project level imports


namespace Infonet.CStoreCommander.Entities
{
    public class Card
    {

        // *******************************************************************************
        // This class defines basic credit and/or debit card properties. The system
        // loads all instances of the class from the database at startup. When a card
        // is read the system scans the class collection to find the card type and then
        // uses the properties of the object found to describe the card.
        // *******************************************************************************

        private int mvarCardID;
        //Private mvarCardCode                 As Long
        private string mvarCardName;
        private string mvarCardType;
        private bool mvarVerifyCheckDigit;
        private bool mvarCheckEncoding;
        private bool mvarSearchNegativeCardFile;
        private bool mvarSearchPositiveCardFile;
        private bool mvarCheckExpiryDate;
        //Private mvarBankIDforCard            As String
        private short mvarLanguageDigitPosition;
        private bool mvarAllowPayment;
        private short mvarMinLength;
        private short mvarMaxLength;
        private string mvarCheckDigitMask;
        private CardCodes mvarCardCodes;
        //The following added For ADS Certification-  
        private short mvarUsageIDPosition; //local copy
        private short mvarUsageIDLength; //local copy
        private short mvarPromptIDPosition; //local copy
        private short mvarPromptIDlength; //local copy
        private string mvarDefaultPromptCode; //local copy
        private string mvarDefaultUsageCode; //local copy
        private bool mvarUserEnteredRestriction;
        private short mvarProductPerTransaction;
        private string mvarServiceTypeCode;
        private bool mvarAskDriverNo; //local copy
        private bool mvarAskIdentificationNo; //local copy
        private bool mvarAskOdometer; //local copy
        private bool mvarAskVehicle; //local copy
        private string mvarFuelServiceType;
        private short mvarProducttPerTransaction;
        private bool mvarVerifyThirdPartySoftware;
        private string mvarGiftType; 
        private bool mvarEncryptCard; //  - For encrypt the card
        private bool mvarVerifyCardNumber; // 



        public string CardType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardType;
                return returnValue;
            }
            set
            {
                mvarCardType = value;
            }
        }


        public CardCodes CardCodes
        {
            get
            {
                CardCodes returnValue = default(CardCodes);
                if (mvarCardCodes == null)
                {
                    mvarCardCodes = new CardCodes();
                }
                returnValue = mvarCardCodes;
                return returnValue;
            }
            set
            {
                mvarCardCodes = value;
            }
        }


        public string CheckDigitMask
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCheckDigitMask;
                return returnValue;
            }
            set
            {
                mvarCheckDigitMask = value;
            }
        }


        public short MaxLength
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarMaxLength;
                return returnValue;
            }
            set
            {
                mvarMaxLength = value;
            }
        }


        public short MinLength
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarMinLength;
                return returnValue;
            }
            set
            {
                mvarMinLength = value;
            }
        }


        public bool AllowPayment
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAllowPayment;
                return returnValue;
            }
            set
            {
                mvarAllowPayment = value;
            }
        }


        public short LanguageDigitPosition
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLanguageDigitPosition;
                return returnValue;
            }
            set
            {
                mvarLanguageDigitPosition = value;
            }
        }

        //Public Property Let BankIDforCard(ByVal vData As String)
        //    mvarBankIDforCard = vData
        //End Property
        //
        //Public Property Get BankIDforCard() As String
        //    BankIDforCard = mvarBankIDforCard
        //End Property


        public bool CheckExpiryDate
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCheckExpiryDate;
                return returnValue;
            }
            set
            {
                mvarCheckExpiryDate = value;
            }
        }


        public bool SearchNegativeCardFile
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarSearchNegativeCardFile;
                return returnValue;
            }
            set
            {
                mvarSearchNegativeCardFile = value;
            }
        }


        public bool SearchPositiveCardFile
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarSearchPositiveCardFile;
                return returnValue;
            }
            set
            {
                mvarSearchPositiveCardFile = value;
            }
        }


        public bool CheckEncoding
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCheckEncoding;
                return returnValue;
            }
            set
            {
                mvarCheckEncoding = value;
            }
        }


        public bool VerifyCheckDigit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarVerifyCheckDigit;
                return returnValue;
            }
            set
            {
                mvarVerifyCheckDigit = value;
            }
        }


        public string Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardName;
                return returnValue;
            }
            set
            {
                mvarCardName = value;
            }
        }

        //Public Property Let CardCode(ByVal vData As Long)
        //    mvarCardCode = vData
        //End Property
        //
        //
        //Public Property Get CardCode() As Long
        //    CardCode = mvarCardCode
        //End Property


        public int CardID
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarCardID;
                return returnValue;
            }
            set
            {
                mvarCardID = value;
            }
        }


        

        public string DefaultUsageCode
        {
            get
            {
                string returnValue = "";
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.defaultusagecode
                returnValue = mvarDefaultUsageCode;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.defaultusagecode = 5
                mvarDefaultUsageCode = value;
            }
        }


        public string DefaultPromptCode
        {
            get
            {
                string returnValue = "";
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.defaultpromptcode
                returnValue = mvarDefaultPromptCode;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.defaultpromptcode = 5
                mvarDefaultPromptCode = value;
            }
        }


        public short PromptIDlength
        {
            get
            {
                short returnValue = 0;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.PromptIDlength
                returnValue = mvarPromptIDlength;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.PromptIDlength = 5
                mvarPromptIDlength = value;
            }
        }


        public short PromptIDPosition
        {
            get
            {
                short returnValue = 0;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.PromptIDPosition
                returnValue = mvarPromptIDPosition;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.PromptIDPosition = 5
                mvarPromptIDPosition = value;
            }
        }


        public short UsageIDLength
        {
            get
            {
                short returnValue = 0;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.UsageIDLength
                returnValue = mvarUsageIDLength;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.UsageIDLength = 5
                mvarUsageIDLength = value;
            }
        }


        public short UsageIDPosition
        {
            get
            {
                short returnValue = 0;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.UsageIDPosition
                returnValue = mvarUsageIDPosition;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.UsageIDPosition = 5
                mvarUsageIDPosition = value;
            }
        }


        public bool UserEnteredRestriction
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarUserEnteredRestriction;
                return returnValue;
            }
            set
            {
                mvarUserEnteredRestriction = value;
            }
        }


        public short ProductPerTransaction
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarProductPerTransaction;
                return returnValue;
            }
            set
            {
                mvarProductPerTransaction = value;
            }
        }


        public string ServiceTypeCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarServiceTypeCode;
                return returnValue;
            }
            set
            {
                mvarServiceTypeCode = value;
            }
        }
        ///ADS Changes Ends-Shiny


        public bool AskVehicle
        {
            get
            {
                bool returnValue = false;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.Vechicle
                returnValue = mvarAskVehicle;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.Vechicle = 5
                mvarAskVehicle = value;
            }
        }


        public bool AskOdometer
        {
            get
            {
                bool returnValue = false;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.Odometer
                returnValue = mvarAskOdometer;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.Odometer = 5
                mvarAskOdometer = value;
            }
        }


        public bool AskIdentificationNo
        {
            get
            {
                bool returnValue = false;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.IdentificationNo
                returnValue = mvarAskIdentificationNo;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.IdentificationNo = 5
                mvarAskIdentificationNo = value;
            }
        }


        public bool AskDriverNo
        {
            get
            {
                bool returnValue = false;
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.DriverNo
                returnValue = mvarAskDriverNo;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.DriverNo = 5
                mvarAskDriverNo = value;
            }
        }


        public string FuelServiceType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarFuelServiceType;
                return returnValue;
            }
            set
            {
                mvarFuelServiceType = value;
            }
        }


        public short ProducttPerTransaction
        {
            get
            {
                 return mvarProductPerTransaction;
            }
            set
            {
                mvarProducttPerTransaction = value;
            }
        }


        public bool VerifyThirdPartySoftware
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarVerifyThirdPartySoftware;
                return returnValue;
            }
            set
            {
                mvarVerifyThirdPartySoftware = value;
            }
        }

        

        public string GiftType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarGiftType;
                return returnValue;
            }
            set
            {
                mvarGiftType = value;
            }
        }
        
        // 
        public bool EncryptCard
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarEncryptCard;
                return returnValue;
            }
            set
            {
                mvarEncryptCard = value;
            }
        }
        //shiny end
        // 
        public bool VerifyCardNumber
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarVerifyCardNumber;
                return returnValue;
            }
            set
            {
                mvarVerifyCardNumber = value;
            }
        }

        private void ClassTerminate()
        {
             mvarCardCodes = null;
        }
        //shiny end
    }
}
