using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Credit_Card
    {

        // =================================================================================
        // The Credit_Card class defines each credit or debit card that is read by the
        // system and attaches the correct CardType object that defines that card's
        // properties by scanning the CardCodes collection in each Card object to find
        // the one that matches the entered card. If none of the defined card objects
        // matches the card being processed then the associated card object is set to
        // 'Nothing' and default object property values are returned to the
        // referencing code.
        // =================================================================================

        private string mvarTrack1;
        private string mvarTrack2;
        private string mvarCardNumber;
        private string mvarCardName;
        private string mvarExpiry_Date;
        private bool mvarExpiry_Date_Valid;
        private string mvarInvalid_Reason;
        private bool mvarCardSwiped;
        private string mvarServiceCode;
        private string mvarCustomerName;
        private string mvarSwipeString;
        private string mvarDeclineMsg;
        private string mvarCheckDigit;
        private string mvarAuthorization_Number;
        private string mvarCardType;
        private int mvarCardCode;
        private string mvarResponse;
        private string mvarResult;
        private string mvarTerminalID;
        private string mvarDebitAccount;
        private string mvarResponseCode;
        private string mvarApprovalCode;
        private string mvarSequence_Number;
        private float mvarTrans_Amount;
        private DateTime mvarTrans_Date;
        private DateTime mvarTrans_Time;
        private string mvarTrans_Type;
        private string mvarTrans_Number;
        private string mvarReceipt_Display;
        private string mvarReport;
        private bool mvarStoreAndForward;
        private string mvarLanguage;
        private string mvarPromptCode;
        private string mvarUsageCode;
        private string mvarUsageType;
        private string mvarvechicle_number;
        private string mvarDriver_Number;
        private string mvarId_Number;
        private string mvarOdometer_Number;
        private bool mvarAskProdRestrictCode;
        private bool mvarPrintVechicleNo;
        private bool mvarPrintOdometer;
        private bool mvarPrintIdentification;
        private bool mvarPrintUsage;
        private bool mvarPrintDriver;
        public Card CardType
        {
            get; set;
        }
        public TenderCard TendCard
        {
            get; set;
        }
        private char mvarEntryMethod; // Nicolette added
        private char mvarAuthorizationSource; // Nicolette added
        private char mvarTerminalType; // Nicolette added
        private int mvarVoid_Num;
        private decimal mvarBalance;
        private float mvarQuantity; 
        private bool mvarAutoRecognition; // Dec 17, 2008 Nicolette
        private string mvarPCFPIN; //###PTC
        private bool mvarAskPCFPin; //####PTC
        private string mvarCustomerCode;
        private bool mvarManualCardProcess; //EMVVERSION 'Shiny Nov9, 2009
        private string mvarBankMessage; //EMVVERSION
        private bool mvarARcustomerCard; // 
        private string mvarCardProfileID; // 
        private bool mvarVerifyCardNumber; // 
        private string mvarPONumber; // 
        private string mvarOptDataString; //  
        private string mvarOptDataProfileID; //  
        private CardPrompts mvarCardPrompts; //  
        private string mvarOptDataProfileIDEMV; //  

        //Behrooz Nov-14

        //Behrooz Nov-14
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

        // Nicolette added to check entry method for the card (Swiped or Manual)

        public string entryMethod
        {
            get
            {
                string returnValue = "";
                returnValue = mvarEntryMethod.ToString();
                return returnValue;
            }
            set
            {
                mvarEntryMethod = Convert.ToChar(value);
            }
        }


        public string TerminalType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTerminalType.ToString();
                return returnValue;
            }
            set
            {
                mvarTerminalType = Convert.ToChar(value);
            }
        }
        // Nicolette end

        public bool Card_In_NCF
        {
            get
            {
                bool returnValue = false;
                return returnValue;
            }
        }

        public string Decline_Message
        {
            get
            {
                string returnValue = "";
                bool x;
                x = this.Card_In_NCF;
                returnValue = mvarDeclineMsg;
                return returnValue;
            }
            set
            {
                mvarDeclineMsg = value;
            }
        }

        public bool Card_Is_Expired
        {
            get
            {
                bool returnValue = false;

                return returnValue;
            }
        }

        public bool CardIsValid
        {
            get
            {
                bool returnValue = false;
                return returnValue;
            }
        }

        public string Invalid_Reason
        {
            get
            {
                string returnValue = "";
                returnValue = mvarInvalid_Reason;
                return returnValue;
            }

            set
            {
                mvarInvalid_Reason = value;
            }
        }


        // This returns the language as a string. The default is 'English' if no language code was supplied.
        public string Language
        {
             get
            {
                string returnValue = "";
                return returnValue;
            }
            set //Sajan
            {
                mvarLanguage = value;
            }
        }

        // This returns the language code from the card. The default is '0' if there
        // was no language code on the card or the card wasn't swiped.
        public string Language_Code
        {
            get
            {
                string returnValue = "";

                if (CardType == null)
                {
                    returnValue = "0";

                }
                else if (CardType.LanguageDigitPosition <= 0 || !this.Card_Swiped)
                {
                    returnValue = "0";

                }
                else
                {
                    returnValue = this.Track2.Substring(this.Track2.Length - CardType.LanguageDigitPosition - 1, 1);
                }

                return returnValue;
            }
        }

        // This sets the expiry date but only if it is 4 numeric digits
        // and the month is between 1 and 12 inclusive.

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
                if (value != "****")
                {
                    short n = 0;

                    if (value.Length != 4)
                    {
                        mvarExpiry_Date = "";
                        mvarExpiry_Date_Valid = false;
                    }
                    else
                    {
                        for (n = 1; n <= 4; n++)
                        {
                            if (double.Parse(value.Substring(n - 1, 1)) < double.Parse("0") || double.Parse(value.Substring(n - 1, 1)) > double.Parse("9"))
                            {
                                mvarExpiry_Date = "";
                                mvarExpiry_Date_Valid = false;
                                return;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(value) && (Conversion.Val(value.Substring(value.Length - 2, 2)) < 1 || Conversion.Val(value.Substring(value.Length - 2, 2)) > 12))
                    {
                        mvarExpiry_Date = "";
                        mvarExpiry_Date_Valid = false;
                        return;
                    }
                }
                mvarExpiry_Date = value;
                mvarExpiry_Date_Valid = true;

            }
        }

        public bool Expiry_Date_Valid
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarExpiry_Date_Valid;
                return returnValue;
            }
        }

        public string Check_Digit
        {
            get
            {
                string returnValue = "";
                bool x;
                if (string.IsNullOrEmpty(mvarCheckDigit))
                {
                    x = CardIsValid;
                }
                returnValue = mvarCheckDigit;
                return returnValue;
            }
            set
            {
                mvarCheckDigit = value;
            }
        }

        public string Expiry_Year
        {
            get
            {
                string returnValue = "";
                if (!string.IsNullOrEmpty(this.Expiry_Date) && this.Expiry_Date.Length >= 2)
                {
                    returnValue = this.Expiry_Date.Substring(0, 2);
                }
                else
                {
                    returnValue = "  ";
                }

                return returnValue;
            }
        }

        public string Expiry_Month
        {
            get
            {
                string returnValue = "";
                if (!string.IsNullOrEmpty(this.Expiry_Date) && this.Expiry_Date.Length == 4)
                {
                    returnValue = this.Expiry_Date.Substring(this.Expiry_Date.Length - 2, 2);
                }
                else
                {
                    returnValue = "  ";
                }
                return returnValue;
            }
        }


        public string Cardnumber
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
        // 
        public string Name
        {
            get
            {
                string returnValue = "";
                //    If CardType Is Nothing Then
                //        Name = ""
                //    Else
                //        Name = CardType.Name
                //    End If
                returnValue = mvarCardName;
                return returnValue;
            }
            set // 
            {

                if (CardType == null)
                {
                    mvarCardName = value;
                }
                else
                {
                    mvarCardName = CardType.Name;
                }

            }
        }


        public string Crd_Type
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardType;
                return returnValue;
            }
            set
            {
                if (CardType == null)
                {
                    mvarCardType = value;
                }
                else
                {
                    mvarCardType = CardType.CardType;
                }
            }
        }


        public string AuthorizationSource
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAuthorizationSource.ToString();
                return returnValue;
            }
            set
            {
                mvarAuthorizationSource = Convert.ToChar(value);
            }
        }

        public bool Call_The_Bank
        {
            get
            {
                bool returnValue = false;

                return returnValue;
            }
        }

        public bool LimitToSale
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = true;
                }
                else
                {
                    returnValue = TendCard.LimitToSale;
                }
                return returnValue;
            }
        }

        public bool VerifyCheckDigit
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.VerifyCheckDigit;
                }
                return returnValue;
            }
        }

        public bool CheckEncoding
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.CheckEncoding;
                }
                return returnValue;
            }
        }

        public bool Check_NCF
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.SearchNegativeCardFile;
                }
                return returnValue;
            }
        }

        public bool CheckExpiryDate
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.CheckExpiryDate;
                }
                return returnValue;
            }
        }

        public string ReceiptTotalText
        {
            get
            {
                string returnValue = "";
                return returnValue;
            }
        }

        public short PrintCopies
        {
            get
            {
                short returnValue = 0;
                if (TendCard == null)
                {
                    returnValue = (short)1;
                }
                else
                {
                    returnValue = TendCard.PrintCopies;
                }
                return returnValue;
            }
        }

        public string Bank_CardID
        {
            get
            {
                string returnValue = "";
                if (TendCard == null)
                {
                    returnValue = "";
                }
                else
                {
                    returnValue = TendCard.BankCardID;
                }
                return returnValue;
            }
        }

        public bool Print_Signature
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.SignatureLine;
                }
                return returnValue;
            }
        }

        public bool RefundAllowed
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.RefundAllowed;
                }
                return returnValue;
            }
        }

        public float PurchaseLimit
        {
            get
            {
                float returnValue = 0;
                if (TendCard == null)
                {
                    returnValue = 0;
                }
                else
                {
                    returnValue = TendCard.PurchaseLimit;
                }
                return returnValue;
            }
        }

        public float FloorLimit
        {
            get
            {
                float returnValue = 0;
                if (TendCard == null)
                {
                    returnValue = 0;
                }
                else
                {
                    returnValue = TendCard.FloorLimit;
                }
                return returnValue;
            }
        }

        public bool AllowPayment
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.AllowPayment;
                }
                return returnValue;
            }
        }

        public decimal MaxCashBack
        {
            get
            {
                decimal returnValue = 0;
                if (TendCard == null)
                {
                    returnValue = 0;
                }
                else
                {
                    returnValue = (decimal)TendCard.MaxCashBack;
                }
                return returnValue;
            }
        }

        public string Report_Group
        {
            get
            {
                string returnValue = "";
                if (TendCard == null)
                {
                    returnValue = "";
                }
                else
                {
                    returnValue = (TendCard.ReportGroup).ToString();
                }
                return returnValue;
            }
        }


        public string Authorization_Number
        {
            get
            {
                string returnValue = "";
                
                if (this.Crd_Type == "G")
                {
                    returnValue = "";
                    return returnValue;
                }
                

                // If it's a bank card then the authorization number should have been set
                // by the bank module (i.e. Tender Retail). If it's a locally validated
                // card then set the authorization as 'hhnnss' of the current time.
                if (!string.IsNullOrEmpty(this.Decline_Message) || string.IsNullOrEmpty(this.Cardnumber))
                {
                    returnValue = "";
                }
                else
                {
                    if (this.Call_The_Bank)
                    {
                        returnValue = mvarAuthorization_Number.Trim(); // Me.Authorization_Type &
                    }
                    else
                    {
                        returnValue = this.Authorization_Type + DateTime.Now.ToString("hhmmss");
                    }
                }
                return returnValue;
            }
            set
            {
                mvarAuthorization_Number = value;
            }
        }

        public string Authorization_Type
        {
            get
            {
                string returnValue = "";
                returnValue = System.Convert.ToString(this.Call_The_Bank ? "B" : "T");
                return returnValue;
            }
        }

        public string TendCode
        {
            get //  
            {
                string returnValue = "";
                if (TendCard == null)
                {
                    returnValue = "";
                }
                else
                {
                    returnValue = TendCard.TenderCode;
                }
                return returnValue;
            }
        }


        public bool Card_Swiped
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCardSwiped;
                return returnValue;
            }
            set
            {
                mvarCardSwiped = value;
            }
        }


        public string Track1
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTrack1;
                return returnValue;
            }
             set
            {

                mvarTrack1 = value;
            }
        }


        public string Customer_Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCustomerName;
                return returnValue;
            }
            set
            {
                mvarCustomerName = value;
            }
        }


        public string Swipe_String
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSwipeString;
                return returnValue;
            }
            set
            {            
                mvarSwipeString = value;
            }
        }


        public string Track2
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTrack2;
                return returnValue;
            }
             set
            {
                mvarTrack2 = value;
            }
        }


        public string Service_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarServiceCode;
                return returnValue;
            }
            set
            {
                mvarServiceCode = value;
            }
        }


        public string Response
        {
            get
            {
                string returnValue = "";
                returnValue = mvarResponse;
                return returnValue;
            }
            set
            {
                mvarResponse = value;
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
                mvarDebitAccount = value;
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


        public string Report
        {
            get
            {
                string returnValue = "";
                returnValue = mvarReport;
                return returnValue;
            }
            set
            {
                mvarReport = value;
            }
        }


        public bool StoreAndForward
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarStoreAndForward;
                return returnValue;
            }
            set
            {
                if (value)
                {
                    mvarStoreAndForward = value;
                }
                else
                {
                    mvarStoreAndForward = false;
                }
            }
        }

         bool CardInPCF
        {
             get
            {
                bool returnValue = false;


                return returnValue;
            }
        }

        public bool CheckPCF
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.SearchPositiveCardFile;
                }
                return returnValue;
            }
        }

        public bool AllowPayAtPump
        {
            get //  Need To allow Paypump or Pay Inside
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.AllowPayPump;
                }
                return returnValue;
            }
        }


        
        public bool AskDriverNo
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.AskDriverNo;
                }
                return returnValue;
            }
        }

        public bool AskIdentificationNo
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.AskIdentificationNo;
                }
                return returnValue;
            }
        }

        public bool AskOdometer
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.AskOdometer;
                }
                return returnValue;
            }
        }

        public bool AskVechicle
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.AskVehicle;
                }
                return returnValue;
            }
        }


        public bool AskProdRestrictCode
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAskProdRestrictCode;
                return returnValue;
            }
            set
            {
                if (value)
                {
                    mvarAskProdRestrictCode = value;
                }
                else
                {
                    mvarAskProdRestrictCode = false;
                }
            }
        }

        public decimal CashBackAmount
        {
            get
            {
                decimal returnValue = 0;
                // This property should return a value <> 0 only for debit cards.
                // When we will use it, we need to check if used card is a debit card.
                if (TendCard == null)
                {
                    returnValue = 0;
                }
                else
                {
                    returnValue = TendCard.CashBackAmount;
                }
                return returnValue;
            }
        }


        public string usageType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUsageType;
                return returnValue;
            }
            set
            {
                mvarUsageType = value;
            }
        }


        // This returns the usage code from the card.
        public string UsageCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUsageCode;
                return returnValue;
            }
            set
            {
                mvarUsageCode = value;
               
            }
        }


        // This returns the prompt code from the card.
        public string PromptCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPromptCode;
                return returnValue;
            }
             set
            {
                mvarPromptCode = value;
                
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


        public int CardCode
        {
            get
            {
                return mvarCardCode;
            }
            set
            {
                mvarCardCode = value;
            }
        }

        public bool CardProductRestrict
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.CardProductRestrict;
                }
                return returnValue;
            }
        }

        //  - To print info on the receipt/not

        public bool Print_VechicleNo
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.PrintVechicle;
                }
                return returnValue;
            }
            set
            {
                if (CardType == null)
                {
                    //    Print_VechicleNo = False'  it was going in loop for debit card crash
                    mvarPrintVechicleNo = false;
                }
                else
                {
                    mvarPrintVechicleNo = value;
                }
            }
        }


        public bool Print_DriverNo
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.PrintDriver;
                }
                return returnValue;
            }
            set
            {
                if (CardType == null)
                {
                    //    Print_DriverNo = False'  it was going in loop for debit card crash
                    mvarPrintDriver = false;
                }
                else
                {
                    mvarPrintDriver = value;
                }
            }
        }


        public bool Print_IdentificationNo
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = mvarPrintDriver;
                }
                else
                {
                    returnValue = TendCard.PrintIdentification;
                }
                return returnValue;
            }
            set
            {
                if (CardType == null)
                {
                    //    Print_IdentificationNo = False'  it was going in loop for debit card crash
                    mvarPrintIdentification = false;
                }
                else
                {
                    mvarPrintIdentification = value;
                }
            }
        }


        public bool Print_Odometer
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.PrintOdometer;
                }
                return returnValue;
            }
            set
            {
                if (CardType == null)
                {
                    //    Print_Odometer = False'  it was going in loop for debit card crash
                    mvarPrintOdometer = false;
                }
                else
                {
                    mvarPrintOdometer = value;
                }
            }
        }


        //shiny end
        public bool Print_Usage
        {
            get
            {
                bool returnValue = false;
                if (TendCard == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = TendCard.PrintUsage;
                }
                return returnValue;
            }
            set
            {
                if (CardType == null)
                {
                    //    Print_Usage = False'  it was going in loop for debit card crash
                    mvarPrintUsage = false;
                }
                else
                {
                    mvarPrintUsage = value;
                }
            }
        }

        public string FuelServiceType
        {
            get
            {
                string returnValue = "";
                if (CardType == null)
                {
                    returnValue = "";
                }
                else
                {
                    returnValue = CardType.FuelServiceType;
                }
                return returnValue;
            }
        }

        public short ProductsPerTrans
        {
            get
            {
                short returnValue = 0;
                if (CardType == null)
                {
                    returnValue = (short)0;
                }
                else
                {
                    returnValue = CardType.ProductPerTransaction;
                }
                return returnValue;
            }
        }

        public bool VerifyThirdPartySoftware
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.VerifyThirdPartySoftware;
                }
                return returnValue;
            }
        }

        
        public string GiftType
        {
            get
            {
                string returnValue = "";
                if (CardType == null)
                {
                    returnValue = "";
                }
                else
                {
                    returnValue = CardType.GiftType;
                }
                return returnValue;
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
        

        // This property returns the tender description from TendMast table based on tendcode.
        // TendDesc has to be recorded in SaleTend and CardTenders table.
        // When the policy "combine" fleets or credit cards is set to "Yes",
        // tender name in both tables was recorded as "Credit Card" or "Fleet", which was wrong.
        string Return_TendDesc
        {
             get
            {
                string returnValue = "";
               

                return returnValue;
            }
        }
        
        // This property returns the tender class from TendMast table based on tendcode.
        string Return_TendClass
        {
            get
            {
                string returnValue = "";
                
                return returnValue;
            }
        }
        public bool EncryptCard
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.EncryptCard;
                }
                return returnValue;
            }
        }
        // recognition

        public bool AutoRecognition
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAutoRecognition;
                return returnValue;
            }
            set
            {
                mvarAutoRecognition = value;
            }
        }
        //   end
        //###PTC - positivecard PCFPIN- March19,2008
        public string PCFPIN
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPCFPIN;
                return returnValue;
            }
            set
            {
                mvarPCFPIN = value;
            }
        }
        public bool AskPCFPin
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAskPCFPin;
                return returnValue;
            }
            set
            {
                mvarAskPCFPin = value;
            }
        }

        public string CustomerCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCustomerCode;
                return returnValue;
            }
            set
            {
                mvarCustomerCode = value;
            }
        }
        // 
        //EMVVERSION - NOV9, 2009- To suport Manual Card process from STPS

        public bool ManualCardProcess
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarManualCardProcess;
                return returnValue;
            }
            set
            {
                mvarManualCardProcess = value;
            }
        }



        public string BankMessage
        {
            get
            {
                string returnValue = "";
                returnValue = mvarBankMessage;
                return returnValue;
            }
            set
            {
                mvarBankMessage = value;
            }
        }
        public bool ARcustomerCard
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarARcustomerCard;
                return returnValue;
            }
            set
            {
                mvarARcustomerCard = value;
            }
        }

        public string CardProfileID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardProfileID;
                return returnValue;
            }
            set
            {
                mvarCardProfileID = value;
            }
        }
        public bool VerifyCardNumber
        {
            get
            {
                bool returnValue = false;
                if (CardType == null)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = CardType.VerifyCardNumber;
                }
                return returnValue;
            }
        }


        public string PONumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPONumber;
                return returnValue;
            }
            set
            {
                mvarPONumber = value;
            }
        }

        //   to read optional data string
        public string OptDataString
        {
            get
            {
                string returnValue = "";
                returnValue = mvarOptDataString;
                return returnValue;
            }
            set
            {
                mvarOptDataString = value;
            }
        }

        // type
        // it works only for tenders with a signle instance. It is mainly written to fix the mess in debit cards processing
        string Return_CardCode
        {
            get
            {
                string returnValue = "";
                
                return returnValue;
            }
        }

        //  
        public string OptDataProfileID
        {
            get
            {
                string returnValue = "";
                if (TendCard == null)
                {
                    returnValue = "";
                }
                else
                {
                    returnValue = TendCard.OptDataProfileID;
                }
                return returnValue;
            }
        }
        //   end

        //  

        public CardPrompts CardPrompts
        {
            get
            {
                CardPrompts returnValue = default(CardPrompts);

                if (mvarCardPrompts == null)
                {
                    mvarCardPrompts = new CardPrompts();
                }
                returnValue = mvarCardPrompts;
                return returnValue;
            }
            set
            {
                mvarCardPrompts = value;
            }
        }
        //   end

        //  
        //   end

        //  
        public string OptDataProfileIDEMV
        {
            get
            {
                string returnValue = "";
                returnValue = mvarOptDataProfileIDEMV;
                return returnValue;
            }
            set
            {
                mvarOptDataProfileIDEMV = value;
            }
        }


        //        private Card Get_CardType()
        //        {
        //            Card returnValue = default(Card);
        //            //Smriti move this code to manager
        //            // Card cd = default(Card);
        //            // CardCode cc = default(CardCode);
        //            //short LL = 0;

        //            
        //            
        //            
        //            
        //            
        //            
        //            //if (policyManager.Version != "US")
        //            //{
        //            //    

        //            //    // If the card was swiped then check for debit card.
        //            //    if (this.Card_Swiped)
        //            //    {
        //            //        foreach (Card tempLoopVar_cd in Chaps_Main.CardTypes_Renamed)
        //            //        {
        //            //            cd = tempLoopVar_cd;

        //            //            if (cd.CardType == "D")
        //            //            {
        //            //                if (this.Service_Code == "798" || this.Service_Code == "799" || this.Service_Code == "220" || this.Service_Code == "120")
        //            //                {
        //            //                    returnValue = cd;
        //            //                    TendCard = new TenderCard();
        //            //                    TendCard.LoadTenderCard(cd.CardID);
        //            //                    return returnValue;
        //            //                }
        //            //                else
        //            //                {
        //            //                    break;
        //            //                }
        //            //            }

        //            //        }
        //            //    }

        //            //} 

        //            //// Wasn't a Debit Card. Check other card types.
        //            //foreach (Card tempLoopVar_cd in Chaps_Main.CardTypes_Renamed)
        //            //{
        //            //    cd = tempLoopVar_cd;
        //            //    foreach (CardCode tempLoopVar_cc in cd.CardCodes)
        //            //    {
        //            //        cc = tempLoopVar_cc;
        //            //        LL = (short)cc.LowerLimit.Length;

        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        if (cd.CardType != "D" || policyManager.Version == "US")
        //            //        {
        //            //            

        //            //            ///                If Left$(Me.CardNumber, LL) >= cc.LowerLimit And _
        //            //            /
        //            //            ///                    Set Get_CardType = Cd
        //            //            ///                    mvarCardCode = Cd.CardID
        //            //            ///                    Set TendCard = New TenderCard
        //            //            ///                    TendCard.LoadTenderCard Cd.CardID
        //            //            ///                    Exit Function
        //            //            ///                End If
        //            //            //  added the extra checking of card length. if the cards belongs to same range but length is different it is picking up wrong card
        //            //            if (double.Parse(this.Cardnumber.Substring(0, LL)) >= double.Parse(cc.LowerLimit) && double.Parse(this.Cardnumber.Substring(0, LL)) <= double.Parse(cc.UpperLimit))
        //            //            {
        //            //                if (this.Cardnumber.Length >= cd.MinLength && this.Cardnumber.Length <= cd.MaxLength)
        //            //                {
        //            //                    returnValue = cd;
        //            //                    mvarCardCode = cd.CardID;
        //            //                    TendCard = new TenderCard();
        //            //                    TendCard.LoadTenderCard(cd.CardID);
        //            //                    this.CardPrompts = TendCard.CardPrompts; //   to load prompts for fleet cards optional data
        //            //                    return returnValue;
        //            //                }
        //            //            }
        //            //            // 
        //            //        }
        //            //    }
        //            //}

        //            //// Didn't find the card. Set the object to nothing which indicates that
        //            //// this is not an accepted card type.

        //            //returnValue = null;

        //            return returnValue;
        //        }

        //        public bool Valid_Mod10(string cMask = "")
        //        {
        //            bool returnValue = false;
        //            //Smriti move this code to manager
        //            //short n = 0; //Byte
        //            //char C;
        //            //char D;
        //            //short s = 0;
        //            //short f = 0;
        //            ////TODO:Assign null value for compilation  somvir_1
        //            //char Check_Digit = '\0';
        //            //string Card_Num = "";

        //            //Card_Num = this.Cardnumber;

        //            //// If they didn't supply a mask then build the standard check mask.
        //            //if (cMask.Length < Card_Num.Length)
        //            //{
        //            //    cMask = "";
        //            //    //        For n = 1 To Len(Card_Num) - 1
        //            //    for (n = (short)(Card_Num.Length - 1); n >= 1; n--)
        //            //    {
        //            //        cMask = System.Convert.ToString(cMask + System.Convert.ToString(n % 2 == 0 ? "1" : "2"));
        //            //    }
        //            //    cMask = cMask + "C"; // Last one's the check digit.
        //            //}

        //            //s = (short)0;
        //            //for (n = 1; n <= Card_Num.Length; n++)
        //            //{
        //            //    C = Convert.ToChar(Card_Num.Substring(n - 1, 1));
        //            //    D = Convert.ToChar(cMask.Substring(n - 1, 1));
        //            //    if (D == 'C')
        //            //    {
        //            //        Check_Digit = C;
        //            //    }
        //            //    else
        //            //    {
        //            //        f = (short)(Conversion.Val(C) * Conversion.Val(D));
        //            //        if (f > 9)
        //            //        {
        //            //            f = (short)(f - 9);
        //            //        }
        //            //        s = (short)(s + f);
        //            //    }
        //            //}

        //            //// Determine what the check digit should be.
        //            //if (s % 10 == 0)
        //            //{
        //            //    mvarCheckDigit = "0";
        //            //}
        //            //else
        //            //{
        //            //    mvarCheckDigit = Conversion.Str(10 - (s % 10)).Trim();
        //            //}

        //            //// If we are not verifying the check digit then just return
        //            //// that the check digit is valid.
        //            //if (this.VerifyCheckDigit)
        //            //{
        //            //    returnValue = (Convert.ToChar(mvarCheckDigit) == Check_Digit);
        //            //}
        //            //else
        //            //{
        //            //    returnValue = true;
        //            //}

        //            return returnValue;
        //        }

        private void Class_Initialize_Renamed()
        {
            mvarDeclineMsg = "";
            mvarAutoRecognition = false;
            mvarCardProfileID = "";
            CardType = null;
            mvarSwipeString = "";
        }
        public Credit_Card()
        {
            Class_Initialize_Renamed();
        }
        //        //==============================================
        //        // This function create the product codes string
        //        //==============================================
        //        public string ProductCodes()
        //        {
        //            string returnValue = "";
        //            //Smriti move this code to manager
        //            //dynamic Till_Renamed = default(dynamic);

        //            //

        //            //ADODB.Recordset rsFormat = default(ADODB.Recordset);
        //            //ADODB.Recordset rsProcess = default(ADODB.Recordset);
        //            //ADODB.Recordset rsTF = default(ADODB.Recordset);
        //            //ADODB.Recordset rsTemp = default(ADODB.Recordset);
        //            //ADODB.Field fld = default(ADODB.Field);
        //            //
        //            //string strTemp = "";
        //            //short i = 0;
        //            //string strFldName = "";
        //            //ADOX.Table tdf = default(ADOX.Table); // TableDef
        //            //ADOX.Index tblIdx = default(ADOX.Index);
        //            //ADOX.Catalog Cat = default(ADOX.Catalog);
        //            //
        //            //

        //            //Cat = new ADOX.Catalog();
        //            //Cat.let_ActiveConnection(Chaps_Main.dbTill);
        //            //
        //            //
        //            //
        //            //
        //            //
        //            //
        //            //
        //            //
        //            //tdf = Cat.Tables["CardProcess"];

        //            //// Create the indexes based on sort fields in CardFormat table
        //            //rsTF = Chaps_Main.Get_Records("SELECT * from CardFormat where Sort=1 order by SortNumber", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //            //foreach (ADOX.Index tempLoopVar_tblIdx in tdf.Indexes)
        //            //{
        //            //    tblIdx = tempLoopVar_tblIdx;
        //            //    if (tblIdx.Name == "Sort")
        //            //    {
        //            //        tdf.Indexes.Delete("Sort");
        //            //    }
        //            //}
        //            //if (!rsTF.EOF)
        //            //{
        //            //    tblIdx = new ADOX.Index();
        //            //    tblIdx.Name = "Sort";
        //            //    while (!rsTF.EOF)
        //            //    {
        //            //        strFldName = System.Convert.ToString(rsTF.Fields["MapField"].Value);
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        

        //            //        tblIdx.Columns.Append(strFldName);
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        rsTF.MoveNext();
        //            //    }
        //            //    tdf.Indexes.Append(tblIdx);
        //            //    tdf.Indexes.Refresh();
        //            //}

        //            //rsProcess = Chaps_Main.Get_Records(Source: "select * from CardProcess where TILL_NUM=" + Till_Renamed.Number, DB: Chaps_Main.dbTill, LockType: (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //rsFormat = Chaps_Main.Get_Records("select * from StringFormat", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //            //while (!rsFormat.EOF)
        //            //{
        //            //    foreach (ADODB.Field tempLoopVar_fld in rsProcess.Fields)
        //            //    {
        //            //        fld = tempLoopVar_fld;
        //            //        if (fld.Name == (string)(rsFormat.Fields["MapField"].Value))
        //            //        {
        //            //            if (rsFormat.Fields["Sum"].Value == true & fld.Type == ADODB.DataTypeEnum.adDouble)
        //            //            {
        //            //                rsTemp = Chaps_Main.Get_Records("SELECT SUM( CardProcess." + fld.Name + ") as [Suma] FROM CardProcess where TILL_NUM=" + Till_Renamed.Number, Chaps_Main.dbTill, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //                strTemp = strTemp + NumberFormat(System.Convert.ToString((Information.IsDBNull(rsTemp.Fields["Suma"].Value)) ? "0" : (rsTemp.Fields["Suma"].Value)), System.Convert.ToString(rsFormat.Fields["Format"].Value));
        //            //            }
        //            //            else
        //            //            {
        //            //                if (!rsProcess.EOF)
        //            //                {
        //            //                    if (Information.IsNumeric(fld.Value))
        //            //                    {
        //            //                        if (rsFormat.Fields["Sum"].Value == true)
        //            //                        {
        //            //                        }
        //            //                        else
        //            //                        {
        //            //                            strTemp = strTemp + NumberFormat(System.Convert.ToString(fld.Value), System.Convert.ToString(rsFormat.Fields["Format"].Value));
        //            //                        }
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        strTemp = strTemp + Strings.Left(System.Convert.ToString(fld.Value), Strings.Len(rsFormat.Fields["Format"].Value));
        //            //                    }
        //            //                }
        //            //                else
        //            //                {
        //            //                    strTemp = strTemp + NumberFormat("0", System.Convert.ToString(rsFormat.Fields["Format"].Value));
        //            //                }
        //            //            }
        //            //            i = System.Convert.ToInt16(rsFormat.Fields["nofield"].Value);
        //            //            break;
        //            //        }
        //            //    }
        //            //    rsFormat.MoveNext();
        //            //    if (rsFormat.EOF)
        //            //    {
        //            //        break;
        //            //    }
        //            //    if (!rsProcess.EOF)
        //            //    {
        //            //        if (System.Convert.ToInt32(rsFormat.Fields["nofield"].Value) > i)
        //            //        {
        //            //            rsProcess.MoveNext();
        //            //        }
        //            //        if ((int)(rsFormat.Fields["nofield"].Value) == 1)
        //            //        {
        //            //            rsProcess.MoveFirst();
        //            //        }
        //            //    }
        //            //}

        //            //returnValue = strTemp;

        //            //rsProcess = null;
        //            //rsFormat = null;
        //            //rsTemp = null;
        //            //rsTF = null;
        //            //tdf = null;

        //            return returnValue;
        //        }
        //        //=====================================================
        //        // Insert data into CardProcess table
        //        //======================================================
        //        public decimal InsertData(Sale CdSale)
        //        {
        //            decimal returnValue = 0;
        //            //Smriti move this code to manager
        //            //dynamic Policy_Renamed = default(dynamic);
        //            //dynamic Till_Renamed = default(dynamic);

        //            //
        //            //
        //            //
        //            //
        //            //
        //            //Sale_Line sl = default(Sale_Line);
        //            //string strCardProduct;
        //            //string strFuelMeasure;
        //            //string strFuelServiceType;
        //            //float sngQty;
        //            //decimal curAmount;
        //            //decimal dblDiscount;
        //            //string strSql;
        //            //string blnFuel;
        //            //string blnIndex = "";
        //            //object recordAffected = null;
        //            //ADODB.Recordset rsProcess = default(ADODB.Recordset);
        //            //ADODB.Recordset rsFormat = default(ADODB.Recordset);
        //            //ADODB.Recordset rsProducts = default(ADODB.Recordset);
        //            //decimal RetAmt = new decimal();
        //            //string usageType = "";
        //            //ADODB.Recordset rsCardUsage = default(ADODB.Recordset);

        //            //Chaps_Main.dbTill.Execute("Delete  FROM CardProcess where TILL_NUM=" + Till_Renamed.Number, out recordAffected, (int)ADODB.CommandTypeEnum.adCmdText | (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
        //            //Variables.iRecsAffected = (short)recordAffected;
        //            //rsProcess = Chaps_Main.Get_Records("select * from CardProcess where TILL_NUM=" + Till_Renamed.Number, Chaps_Main.dbTill, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly);
        //            //rsFormat = Chaps_Main.Get_Records("SELECT * FROM CardFormat WHERE CardFormat.[Group]=1", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //rsProducts = Chaps_Main.Get_Records("select * from CardProducts", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //            //if (!rsFormat.EOF)
        //            //{
        //            //    blnIndex = (true).ToString();
        //            //}

        //            //foreach (Sale_Line tempLoopVar_sl in CdSale.Sale_Lines)
        //            //{
        //            //    sl = tempLoopVar_sl;
        //            //    // Add only the lines whom products codes are related to
        //            //    // proper usage of selected fleet card

        //            //    //
        //            //    //First see what usagecode translated to ALL or FUEL
        //            //    rsCardUsage = Chaps_Main.Get_Records("Select * from Cardusage where BankCardId=\'" + modTPS.cc.Bank_CardID + "\' and UsageID=\'" + modTPS.cc.UsageCode + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //    if (rsCardUsage.RecordCount > 0)
        //            //    {
        //            //        usageType = System.Convert.ToString(rsCardUsage.Fields["UsageType"].Value);
        //            //    }
        //            //    else
        //            //    {
        //            //        returnValue = 0;
        //            //        return returnValue;
        //            //    }

        //            //    rsProducts.Find(Criteria: "BankCardID=\'" + modTPS.cc.Bank_CardID + "\' and CardProductCode=\'" + sl.CardProductCode + "\' and UsageType=\'" + usageType + "\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
        //            //    //end - Svetlana

        //            //    if (!rsProducts.EOF)
        //            //    {
        //            //        
        //            //        
        //            //        
        //            //        
        //            //        if (bool.Parse(blnIndex))
        //            //        {
        //            //            rsProcess.Find(Criteria: "CardProductCode=\'" + sl.CardProductCode + "\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
        //            //            if (rsProcess.EOF)
        //            //            {
        //            //                rsProcess.AddNew();
        //            //                rsProcess.Fields["FuelMeasure"].Value = Policy_Renamed.FUEL_UM;
        //            //                rsProcess.Fields["FuelServiceType"].Value = this.FuelServiceType;
        //            //                rsProcess.Fields["CardProductCode"].Value = sl.CardProductCode;
        //            //                rsProcess.Fields["Qty"].Value = sl.Quantity;
        //            //                rsProcess.Fields["Amount"].Value = sl.Amount;
        //            //                rsProcess.Fields["Discount"].Value = sl.Line_Discount + sl.Discount_Adjust;
        //            //                rsProcess.Fields["SaleTax"].Value = sl.AddedTax; // curSaleTax
        //            //                rsProcess.Fields["FUEL"].Value = sl.ProductIsFuel;
        //            //            }
        //            //            else
        //            //            {
        //            //                rsProcess.Fields["Qty"].Value = rsProcess.Fields["Qty"].Value + sl.Quantity;
        //            //                rsProcess.Fields["Amount"].Value = rsProcess.Fields["Amount"].Value + sl.Amount;
        //            //                rsProcess.Fields["Discount"].Value = System.Convert.ToDouble(rsProcess.Fields["Discount"].Value) + sl.Line_Discount + sl.Discount_Adjust;
        //            //                rsProcess.Fields["SaleTax"].Value = rsProcess.Fields["SaleTax"].Value + sl.AddedTax; // curSaleTax
        //            //            }
        //            //            rsProcess.Update();
        //            //        }
        //            //        else
        //            //        {
        //            //            rsProcess.AddNew();
        //            //            rsProcess.Fields["FuelMeasure"].Value = Policy_Renamed.FUEL_UM;
        //            //            rsProcess.Fields["FuelServiceType"].Value = this.FuelServiceType;
        //            //            rsProcess.Fields["CardProductCode"].Value = sl.CardProductCode;
        //            //            rsProcess.Fields["Qty"].Value = sl.Quantity;
        //            //            rsProcess.Fields["Amount"].Value = sl.Amount;
        //            //            rsProcess.Fields["Discount"].Value = sl.Line_Discount + sl.Discount_Adjust;
        //            //            rsProcess.Fields["SaleTax"].Value = sl.AddedTax; // curSaleTax
        //            //            rsProcess.Fields["FUEL"].Value = sl.ProductIsFuel;
        //            //            rsProcess.Update();
        //            //        }

        //            //        //        strFuelMeasure = GetPol("FUEL_UM")
        //            //        //        strFuelServiceType = Me.FuelServiceType
        //            //        //        strCardProduct = SL.CardProductCode
        //            //        //        sngQty = SL.Quantity
        //            //        //        curAmount = SL.Amount
        //            //        //        blnFuel = SL.ProductIsFuel
        //            //        //        dblDiscount = SL.Line_Discount + SL.Discount_Adjust
        //            //        //        strSQL = "INSERT INTO CardProcess " & _
        //            //        
        //            //        
        //            //        
        //            //        //        dbTill_Renamed.Execute strSQL

        //            //        
        //            //        
        //            //        
        //            //        RetAmt = (decimal)(RetAmt + sl.Amount - (decimal)sl.Line_Discount - (decimal)sl.Discount_Adjust + sl.AddedTax + sl.TotalCharge); //curSaleTax
        //            //                                                                                                                                         
        //            //    }
        //            //}
        //            //returnValue = RetAmt;

        //            //rsFormat = null;
        //            //rsProcess = null;
        //            //rsProducts = null;

        //            return returnValue;
        //        }

        //        public void GetCardProductCodes(Sale CdSale)
        //        {
        //            //Smriti move this code to manager
        //            //    Sale_Line sl = default(Sale_Line);
        //            //    ADODB.Recordset rs = default(ADODB.Recordset);
        //            //    string strSql = "";

        //            //    foreach (Sale_Line tempLoopVar_sl in CdSale.Sale_Lines)
        //            //    {
        //            //        sl = tempLoopVar_sl;
        //            //        strSql = "SELECT * FROM CardProductLink  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "Dept=\'" + sl.Dept + "\'";
        //            //        rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //        if (!rs.EOF)
        //            //        {
        //            //            sl.CardProductCode = System.Convert.ToString(rs.Fields["CardProductCode"].Value);
        //            //        }
        //            //        else
        //            //        {
        //            //            strSql = "SELECT * FROM CardProductLink  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "SubDept=\'" + sl.Sub_Dept + "\'";
        //            //            rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //            if (!rs.EOF)
        //            //            {
        //            //                sl.CardProductCode = System.Convert.ToString(rs.Fields["CardProductCode"].Value);
        //            //            }
        //            //            else
        //            //            {
        //            //                strSql = "SELECT * FROM CardProductLink  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "SubDetail=\'" + sl.Sub_Detail + "\'";
        //            //                rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //                if (!rs.EOF)
        //            //                {
        //            //                    sl.CardProductCode = System.Convert.ToString(rs.Fields["CardProductCode"].Value);
        //            //                }
        //            //                else
        //            //                {
        //            //                    strSql = "SELECT * FROM CardProductLink  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "StockCode=\'" + sl.Stock_Code + "\'";
        //            //                    rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //                    if (!rs.EOF)
        //            //                    {
        //            //                        sl.CardProductCode = System.Convert.ToString(rs.Fields["CardProductCode"].Value);
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        sl.CardProductCode = "";
        //            //                    }
        //            //                }
        //            //            }
        //            //        }
        //            //    }

        //            //    rs = null;

        //        }

        //        //
        //        public float GetValidProductForCard(Sale CdSale, ref string strLineNum)
        //        {
        //            float returnValue = 0;
        //            // Smriti move this code to manager
        //            //    Sale_Line sl = default(Sale_Line);
        //            //    ADODB.Recordset rs = default(ADODB.Recordset);
        //            //    string strSql = "";
        //            //    float ValidAmt = 0;
        //            //    decimal LineAmount = new decimal();

        //            //    ValidAmt = 0;
        //            //    strLineNum = "";
        //            //    
        //            //    
        //            //    
        //            //    if (this.Bank_CardID == "")
        //            //    {
        //            //        returnValue = 0;
        //            //        return returnValue;
        //            //    }

        //            //    if (!this.CardProductRestrict)
        //            //    {
        //            //        
        //            //        returnValue = (float)CdSale.Sale_Totals.Gross;

        //            //        
        //            //        strLineNum = "";
        //            //        foreach (Sale_Line tempLoopVar_sl in CdSale.Sale_Lines)
        //            //        {
        //            //            sl = tempLoopVar_sl;
        //            //            
        //            //            
        //            //            if (sl.ThirdPartyExtractCode != "")
        //            //            {
        //            //                sl.CardProductCode = sl.ThirdPartyExtractCode;
        //            //            }
        //            //            else
        //            //            {
        //            //                sl.CardProductCode = "10";
        //            //            }
        //            //            strLineNum = strLineNum + System.Convert.ToString(sl.Line_Num) + ",";
        //            //        }
        //            //        if (strLineNum.Substring(strLineNum.Length - 1, 1) == ",")
        //            //        {
        //            //            strLineNum = strLineNum.Substring(0, strLineNum.Length - 1);
        //            //        }
        //            //        

        //            //        return returnValue;
        //            //    }
        //            //    else
        //            //    {
        //            //        
        //            //        
        //            //        strSql = "SELECT * FROM CardProductRestriction  WHERE BankCardID=\'" + this.Bank_CardID + "\'";
        //            //        rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //        if (rs.EOF)
        //            //        {
        //            //            returnValue = 0;
        //            //            rs = null;
        //            //            return returnValue;
        //            //        }

        //            //        foreach (Sale_Line tempLoopVar_sl in CdSale.Sale_Lines)
        //            //        {
        //            //            sl = tempLoopVar_sl;
        //            //            strSql = "SELECT * FROM CardProductRestriction  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "Dept=\'" + sl.Dept + "\'";
        //            //            rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //            if (!rs.EOF)
        //            //            {
        //            //                sl.CardProductCode = System.Convert.ToString(rs.Fields["CardProductCode"].Value);
        //            //                sl.RestrictedAmount = System.Convert.ToDecimal((Information.IsDBNull(rs.Fields["Amount"].Value)) ? 0 : (rs.Fields["Amount"].Value));
        //            //            }
        //            //            else
        //            //            {
        //            //                strSql = "SELECT * FROM CardProductRestriction  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "SubDept=\'" + sl.Sub_Dept + "\'";
        //            //                rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //                if (!rs.EOF)
        //            //                {
        //            //                    sl.CardProductCode = System.Convert.ToString(rs.Fields["CardProductCode"].Value);
        //            //                    sl.RestrictedAmount = System.Convert.ToDecimal((Information.IsDBNull(rs.Fields["Amount"].Value)) ? 0 : (rs.Fields["Amount"].Value));
        //            //                }
        //            //                else
        //            //                {
        //            //                    strSql = "SELECT * FROM CardProductRestriction  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "SubDetail=\'" + sl.Sub_Detail + "\'";
        //            //                    rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //                    if (!rs.EOF)
        //            //                    {
        //            //                        sl.CardProductCode = System.Convert.ToString(rs.Fields["CardProductCode"].Value);
        //            //                        sl.RestrictedAmount = System.Convert.ToDecimal((Information.IsDBNull(rs.Fields["Amount"].Value)) ? 0 : (rs.Fields["Amount"].Value));
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        strSql = "SELECT * FROM CardProductRestriction  WHERE BankCardID=\'" + this.Bank_CardID + "\' AND " + "StockCode=\'" + sl.Stock_Code + "\'";
        //            //                        rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //                        if (!rs.EOF)
        //            //                        {
        //            //                            sl.CardProductCode = Strings.Trim(System.Convert.ToString(rs.Fields["CardProductCode"].Value));
        //            //                            sl.RestrictedAmount = System.Convert.ToDecimal((Information.IsDBNull(rs.Fields["Amount"].Value)) ? 0 : (rs.Fields["Amount"].Value));
        //            //                        }
        //            //                        else
        //            //                        {
        //            //                            sl.CardProductCode = "";
        //            //                            sl.RestrictedAmount = 0;
        //            //                        }
        //            //                    }
        //            //                }
        //            //            }
        //            //            if (sl.CardProductCode != "")
        //            //            {
        //            //                LineAmount = (decimal)(sl.Amount - (decimal)sl.Line_Discount - (decimal)sl.Discount_Adjust + sl.AddedTax + sl.TotalCharge);
        //            //                if ((sl.RestrictedAmount == 0) || (sl.RestrictedAmount != 0 & LineAmount < sl.RestrictedAmount)) // no amount restriction set for the product, consider the total
        //            //                {
        //            //                    ValidAmt = ValidAmt + (float)LineAmount;
        //            //                }
        //            //                else
        //            //                {
        //            //                    ValidAmt = ValidAmt + (float)sl.RestrictedAmount;
        //            //                }
        //            //                strLineNum = strLineNum + System.Convert.ToString(sl.Line_Num) + ",";
        //            //            }
        //            //        }
        //            //    }

        //            //    rs = null;

        //            //    if (strLineNum.Substring(strLineNum.Length - 1, 1) == ",")
        //            //    {
        //            //        strLineNum = strLineNum.Substring(0, strLineNum.Length - 1);
        //            //    }

        //            //    returnValue = ValidAmt;
        //            return returnValue;
        //        }

        //        
        //        
        //        
        //        
        //        
        //        
        //        
        //        
        //        
        //        
        //        
        //        


        //        //=============================================================================
        //        // This function format the string containing a number according with strFormat
        //        // eg.: NumberFormat("22.85","0000v000")->0022850 - Nicolette
        //        //=============================================================================
        //        private string NumberFormat(string strSend, string strFormat)
        //        {
        //            string returnValue = "";
        //            //Smriti move this code to manager
        //            //double dblValue = 0;

        //            //if (Information.IsNumeric(strSend) && strFormat.IndexOf("v") + 1 != 0)
        //            //{
        //            //    dblValue = Conversion.Val(strSend) * Math.Pow(10, (strFormat.Substring(strFormat.IndexOf("v") + 2 - 1).Length));
        //            //    returnValue = Strings.Right(new string('0', strFormat.Length - 1) + System.Convert.ToString(dblValue), strFormat.Length - 1);
        //            //}
        //            //else
        //            //{
        //            //    returnValue = Strings.Right(new string('0', strFormat.Length) + strSend, strFormat.Length);
        //            //}

        //            return returnValue;
        //        }

        //        //   this function returns the tender code based on the swipestring
        //        public string Find_TenderCode()
        //        {
        //            string returnValue = "";
        //            //Smriti move this code to manager
        //            //dynamic Policy_Renamed = default(dynamic);

        //            //short StartPos = 0;
        //            //string TenderUsed = "";
        //            //Card cd = default(Card);
        //            //CardCode CCode = default(CardCode);
        //            //short LL = 0;
        //            //MsgBoxStyle msgBoxStyle = MsgBoxStyle.OkOnly;

        //            //// For debit card this function returns tenderclass
        //            //StartPos = (short)(mvarSwipeString.IndexOf("=") + 1);
        //            //if (StartPos > 0)
        //            //{
        //            //    if (mvarSwipeString.Substring(StartPos + 5 - 1, 3) == "798" || mvarSwipeString.Substring(StartPos + 5 - 1, 3) == "799" || mvarSwipeString.Substring(StartPos + 5 - 1, 3) == "220" || mvarSwipeString.Substring(StartPos + 5 - 1, 3) == "120")
        //            //    {
        //            //        TenderUsed = "DBCARD";
        //            //    }
        //            //}

        //            //if (!string.IsNullOrEmpty(TenderUsed))
        //            //{
        //            //    returnValue = TenderUsed;
        //            //    mvarAutoRecognition = true;
        //            //    return returnValue;
        //            //}

        //            //// Wasn't a Debit Card. Check other card types.
        //            //foreach (Card tempLoopVar_cd in Chaps_Main.CardTypes_Renamed)
        //            //{
        //            //    cd = tempLoopVar_cd;
        //            //    foreach (CardCode tempLoopVar_CCode in cd.CardCodes)
        //            //    {
        //            //        CCode = tempLoopVar_CCode;
        //            //        LL = (short)CCode.LowerLimit.Length;
        //            //        if (cd.CardType != "D") // Debit Cards already checked.
        //            //        {
        //            //            if (double.Parse(this.Cardnumber.Substring(0, LL)) >= double.Parse(CCode.LowerLimit) && double.Parse(this.Cardnumber.Substring(0, LL)) <= double.Parse(CCode.UpperLimit))
        //            //            {
        //            //                mvarCardCode = cd.CardID;
        //            //                mvarCardName = cd.Name;
        //            //                Crd_Type = cd.CardType;
        //            //                TendCard = new TenderCard();
        //            //                TendCard.LoadTenderCard(cd.CardID);
        //            //                if (Crd_Type == "C")
        //            //                {
        //            //                    if (!Policy_Renamed.COMBINECR)
        //            //                    {
        //            //                        returnValue = TendCard.TenderCode;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        returnValue = this.Return_TendClass;
        //            //                    }
        //            //                }

        //            //                if (Crd_Type == "F")
        //            //                {
        //            //                    //  - Gasking Charges
        //            //                    if (cd.VerifyCardNumber)
        //            //                    {
        //            //                        // 
        //            //                        if (ValidCustomerCard())
        //            //                        {
        //            //                            // 
        //            //                            //  
        //            //                            if (Policy_Renamed.CUST_EXPDATE && DateAndTime.DateSerial(System.Convert.ToInt32(System.Convert.ToInt32(Strings.Left(System.Convert.ToString((Information.IsDBNull(this.Expiry_Date) || this.Expiry_Date == "") ? "000" : this.Expiry_Date), 2))), System.Convert.ToInt32(System.Convert.ToInt32(Strings.Right(System.Convert.ToString((Information.IsDBNull(this.Expiry_Date) || this.Expiry_Date == "") ? "000" : this.Expiry_Date), 2))), int.Parse("20")) < DateAndTime.Today)
        //            //                            {
        //            //                                //WriteToLogFile "Expiry date set to " & Me.Expiry_Date
        //            //                                //WriteToLogFile "Find_TenderCode date used is " & DateSerial(Right$(Me.Expiry_Date, 2), Left$(Me.Expiry_Date, 2), "20")
        //            //                                Chaps_Main.DisplayMessage(0, (short)1492, msgBoxStyle, mvarExpiry_Date, (byte)0);
        //            //                            }
        //            //                            else
        //            //                            {
        //            //                                //   end
        //            //                                if (this.ARcustomerCard) //  - since we are verifying the card and identifying the customer we can know whether it is a an AR customer - If ValidARCustomerCard Then
        //            //                                {
        //            //                                    returnValue = System.Convert.ToString(Policy_Renamed.ARTender); //"ACCOUNT"
        //            //                                }
        //            //                            }
        //            //                        }
        //            //                        // 
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        // 
        //            //                        if (!Policy_Renamed.COMBINEFLEET)
        //            //                        {
        //            //                            returnValue = TendCard.TenderCode;
        //            //                        }
        //            //                        else
        //            //                        {
        //            //                            returnValue = this.Return_TendClass;
        //            //                        }
        //            //                    }
        //            //                }

        //            //                // Apr 06, 2009 Nicolette added to handle combined third party cards
        //            //                if (Crd_Type == "T")
        //            //                {
        //            //                    if (Policy_Renamed.ThirdParty)
        //            //                    {
        //            //                        if (!(Variables.Milliplein_Renamed == null))
        //            //                        {
        //            //                            if (Variables.Milliplein_Renamed.CombineThirdParty)
        //            //                            {
        //            //                                returnValue = this.Return_TendClass;
        //            //                            }
        //            //                            else
        //            //                            {
        //            //                                returnValue = TendCard.TenderCode;
        //            //                            }
        //            //                        }
        //            //                        else
        //            //                        {
        //            //                            returnValue = TendCard.TenderCode;
        //            //                        }
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        returnValue = TendCard.TenderCode;
        //            //                    }
        //            //                }
        //            //                // Apr 06, 2009 Nicolette end

        //            //                if (Crd_Type != "C" && Crd_Type != "F" && Crd_Type != "T")
        //            //                {
        //            //                    returnValue = TendCard.TenderCode;
        //            //                }
        //            //                mvarAutoRecognition = true;
        //            //                return returnValue;
        //            //            }
        //            //        }
        //            //    }
        //            //}

        //            return returnValue;
        //        }
        //        //###PTC -End - Mar19,2008
        //        //  - Customercard to identify customer

        //        public bool ValidCustomerCard() // changed this to be the validcustomercard ' ValidARCustomerCard() As Boolean
        //        {
        //            bool returnValue = false;
        //            //Smriti move this code to manager
        //            //ADODB.Recordset rs = new ADODB.Recordset();

        //            //
        //            /////             "SELECT * " & _
        //            /////             "FROM ClientCard inner join client on clientcard.cl_code = client.cl_code " & _
        //            /////             "WHERE CardNum = '" & Me.CardNumber & "' and client.CL_arcust =1", dbMaster, _
        //            /////             adOpenForwardOnly, adLockReadOnly)
        //            //rs = Chaps_Main.Get_Records("SELECT * FROM ClientCard inner join client on clientcard.cl_code = client.cl_code  WHERE CardNum = \'" + this.Cardnumber + "\' ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //            //if (!rs.EOF)
        //            //{
        //            //    //shiny added in april13,
        //            //    if (rs.Fields["CardStatus"].Value != "V")
        //            //    {
        //            //        returnValue = false;
        //            //        mvarARcustomerCard = false;
        //            //        mvarCardProfileID = "";
        //            //    }
        //            //    else
        //            //    {
        //            //        returnValue = true; //ValidARCustomerCard = True
        //            //        mvarCustomerCode = System.Convert.ToString((Information.IsDBNull(rs.Fields["cl_code"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rs.Fields["cl_code"].Value))));
        //            //        mvarARcustomerCard = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["CL_arcust"].Value)) ? false : (rs.Fields["CL_arcust"].Value));
        //            //        ///            mvarExpiry_Date = IIf(IsNull(rs![ExpDate]), Date, rs![ExpDate])  '   don't overwrite Expiry date from the DB
        //            //        if (policyManager.RSTR_PROFILE)
        //            //        {
        //            //            mvarCardProfileID = System.Convert.ToString((Information.IsDBNull(rs.Fields["ProfileID"].Value)) ? "" : (rs.Fields["ProfileID"].Value));
        //            //        }
        //            //        else
        //            //        {
        //            //            mvarCardProfileID = "";
        //            //        }
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    returnValue = false; //ValidARCustomerCard = False
        //            //}

        //            //rs = null;

        //            return returnValue;
        //        }


        //        //   end

    }
}
