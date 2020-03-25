using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Card_Reprint
    {

        // =================================================================================
        // We use this object rather than the Credit_Card class when we are reprinting
        // a credit card receipt. The Credit_Card class was designed to validate credit
        // cards and, when we are reprinting, we just want static data without any
        // validation or processing.
        // =================================================================================

        private string mvarCustomer_Name;
        private string mvarName;
        private string mvarCardNumber;
        private bool mvarCard_Swiped;
        private bool mvarPrint_Signature;
        private string mvarExpiry_Month;
        private string mvarExpiry_Year;
        private string mvarAuthorization_Number;
        //The following properties are added by Sajan
        private string mvarTerminalID;
        private string mvarDebitAccount;
        private string mvarResponseCode;
        private string mvarApprovalCode;
        private string mvarSequence_Number;
        private string mvarCard_Type;
        private float mvarTrans_Amount;
        private DateTime mvarTrans_Date;
        private DateTime mvarTrans_Time;
        private string mvarTrans_Type;
        private string mvarReceipt_Display;
        //Sajan
        // 
        private string mvarusagetype;
        private string mvarvechicle_number;
        private string mvarDriver_Number;
        private string mvarId_Number;
        private string mvarOdometer_Number;
        private bool mvarPrintVechicle;
        private bool mvarPrintDriver;
        private bool mvarPrintIdentification;
        private bool mvarPrintOdometer;
        private bool mvarPrintUsage;
        
        private int mvarVoid_Num;
        private string mvarLanguage;
        private string mvarTrans_Number;
        private string mvarExpiry_Date;
        private bool mvarStore_Forward; 
        private decimal mvarBalance; 
        private string mvarResult; 

        private string mvarMessage; 

        
        private float mvarQuantity;

        private string mvarCardprofileID; // 
        private byte mvarTillNumber; // 
        private string mvarPOnumber; // 


        public decimal Quantity
        {
            get
            {
                decimal returnValue = 0;
                returnValue = (decimal)mvarQuantity;
                return returnValue;
            }
            set
            {
                mvarQuantity = (float)value;
            }
        }
        

        

        public string Message
        {
            get
            {
                string returnValue = "";
                returnValue = mvarMessage;
                return returnValue;
            }
            set
            {
                mvarMessage = value;
            }
        }
        


        public int Void_Num
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarVoid_Num;
                return returnValue;
            }
            set
            {
                mvarVoid_Num = value;
            }
        }


        public string Language
        {
            get
            {
                string returnValue = "";
                returnValue = mvarLanguage;
                return returnValue;
            }
            set
            {
                mvarLanguage = value;
            }
        }

        public string Trans_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTrans_Number;
                return returnValue;
            }
            set
            {
                mvarTrans_Number = value;
            }
        }


        public string Expiry_Date
        {
            get
            {
                string returnValue = "";
                returnValue = mvarExpiry_Date;
                return returnValue;
            }
            set
            {
                mvarExpiry_Date = value;
            }
        }

        

        public decimal Balance
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBalance;
                return returnValue;
            }
            set
            {
                mvarBalance = value;
            }
        }

        

        public string Result
        {
            get
            {
                string returnValue = "";
                returnValue = mvarResult;
                return returnValue;
            }
            set
            {
                mvarResult = value;
            }
        }
        

        //===============


        public string Authorization_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAuthorization_Number;
                return returnValue;
            }
            set
            {
                mvarAuthorization_Number = value;
            }
        }





        public string Expiry_Year
        {
            get
            {
                string returnValue = "";
                returnValue = mvarExpiry_Year;
                return returnValue;
            }
            set
            {
                mvarExpiry_Year = value;
            }
        }





        public string Expiry_Month
        {
            get
            {
                string returnValue = "";
                returnValue = mvarExpiry_Month;
                return returnValue;
            }
            set
            {
                mvarExpiry_Month = value;
            }
        }





        public bool Print_Signature
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrint_Signature;
                return returnValue;
            }
            set
            {
                mvarPrint_Signature = value;
            }
        }





        public bool Card_Swiped
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCard_Swiped;
                return returnValue;
            }
            set
            {
                mvarCard_Swiped = value;
            }
        }





        public string CardNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardNumber;
                return returnValue;
            }
            set
            {
                mvarCardNumber = value;
            }
        }





        public string Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarName;
                return returnValue;
            }
            set
            {
                mvarName = value;
            }
        }


        public string Customer_Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCustomer_Name;
                return returnValue;
            }
            set
            {
                mvarCustomer_Name = value;
            }
        }


        public string TerminalID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTerminalID;
                return returnValue;
            }
            set
            {
                mvarTerminalID = value;
            }
        }

        public string DebitAccount
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDebitAccount;
                return returnValue;
            }
            set
            {
                if (this.Card_Type == "D")
                {
                    if (this.Language.Substring(0, 1) == "F")
                    {
                        mvarDebitAccount = System.Convert.ToString(value == "S" ? "Epargne" : "Cheque");
                    }
                    else
                    {
                        mvarDebitAccount = System.Convert.ToString(value == "S" ? "Savings" : "Chequing");
                    }
                }
            }
        }

        public string ResponseCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarResponseCode;
                return returnValue;
            }
            set
            {
                mvarResponseCode = value;
            }
        }

        public string ApprovalCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarApprovalCode;
                return returnValue;
            }
            set
            {
                mvarApprovalCode = value;
            }
        }

        public string Sequence_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSequence_Number;
                return returnValue;
            }
            set
            {
                mvarSequence_Number = value;
            }
        }

        //Nancy add Card_Type property for checking Fleet card, 09/15/02

        public string Card_Type
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCard_Type;
                return returnValue;
            }
            set
            {
                mvarCard_Type = value;
            }
        }


        public float Trans_Amount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTrans_Amount;
                return returnValue;
            }
            set
            {
                mvarTrans_Amount = value;
            }
        }

        public DateTime Trans_Date
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarTrans_Date;
                return returnValue;
            }
            set
            {
                mvarTrans_Date = value;
            }
        }

        public DateTime Trans_Time
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarTrans_Time;
                return returnValue;
            }
            set
            {
                mvarTrans_Time = value;
            }
        }

        public string Trans_Type
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTrans_Type;
                return returnValue;
            }
            set
            {
                mvarTrans_Type = value;
            }
        }

        public string Receipt_Display
        {
            get
            {
                string returnValue = "";
                returnValue = mvarReceipt_Display;
                return returnValue;
            }
            set
            {
                mvarReceipt_Display = value;
            }
        }
        // start

        public bool PrintVechicle
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintVechicle;
                return returnValue;
            }
            set
            {
                mvarPrintVechicle = value;
            }
        }

        public bool PrintDriver
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintDriver;
                return returnValue;
            }
            set
            {
                mvarPrintDriver = value;
            }
        }

        public bool PrintOdometer
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintOdometer;
                return returnValue;
            }
            set
            {
                mvarPrintOdometer = value;
            }
        }

        public bool PrintUsage
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintUsage;
                return returnValue;
            }
            set
            {
                mvarPrintUsage = value;
            }
        }

        //04/14/05 Nancy added Store_Forward property to keep for reprint SAF sales

        public bool Store_Forward
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarStore_Forward;
                return returnValue;
            }
            set
            {
                mvarStore_Forward = value;
            }
        }
        


        public bool PrintIdentification
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintIdentification;
                return returnValue;
            }
            set
            {
                mvarPrintIdentification = value;
            }
        }

        public string Vechicle_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarvechicle_number;
                return returnValue;
            }
            set
            {
                mvarvechicle_number = value;
            }
        }

        public string Driver_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDriver_Number;
                return returnValue;
            }
            set
            {
                mvarDriver_Number = value;
            }
        }

        public string ID_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarId_Number;
                return returnValue;
            }
            set
            {
                mvarId_Number = value;
            }
        }

        public string Odometer_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarOdometer_Number;
                return returnValue;
            }
            set
            {
                mvarOdometer_Number = value;
            }
        }


        public string UsageType
        {
            get
            {
                string returnValue = "";
                //used when retrieving value of a property, on the right side of an assignment.
                //Syntax: Debug.Print X.UsageType
                returnValue = mvarusagetype;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                //Syntax: X.UsageType = 5
                mvarusagetype = value;
            }
        }

        public string CardprofileID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardprofileID;
                return returnValue;
            }
            set
            {
                mvarCardprofileID = value;
            }
        }

        public byte TillNumber
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarTillNumber;
                return returnValue;
            }
            set
            {
                mvarTillNumber = value;
            }
        }

        public string PONumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPOnumber;
                return returnValue;
            }
            set
            {
                mvarPOnumber = value;
            }
        }
        //shiny end
        
        private void Class_Initialize_Renamed()
        {
            mvarVoid_Num = 0;
        }
        public Card_Reprint()
        {
            Class_Initialize_Renamed();
        }
    }
}
