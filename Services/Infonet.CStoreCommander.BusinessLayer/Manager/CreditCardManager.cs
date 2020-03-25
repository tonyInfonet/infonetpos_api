using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Linq;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class CreditCardManager : ManagerBase, ICreditCardManager
    {
        private readonly ICardService _cardService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ITenderService _tenderService;
        private readonly IPolicyManager _policyManager;
        private readonly ICustomerService _customerService;
        private readonly ICardPromptManager _cardPromptManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cardService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="tenderservice"></param>
        /// <param name="customerService"></param>
        /// <param name="cardPromptManager"></param>
        public CreditCardManager(ICardService cardService, IApiResourceManager resourceManager,
            IPolicyManager policyManager, ITenderService tenderservice, ICustomerService customerService,
            ICardPromptManager cardPromptManager)
        {
            _cardService = cardService;
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _tenderService = tenderservice;
            _customerService = customerService;
            _cardPromptManager = cardPromptManager;
        }


        /// <summary>
        /// Method to chek if card is negative card
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        public bool CardInNcf(ref Credit_Card creditCard)
        {
            bool returnValue = true;
            // If we are not supposed to check for this card type in the NCF then
            // just return False (i.e. not in the NCF)

            if (!creditCard.Check_NCF)
            {
                returnValue = false;
            }
            else
            {
                var messageCode = _cardService.GetMessageCode(creditCard.Cardnumber);
                if (string.IsNullOrEmpty(messageCode))
                {
                    returnValue = false;
                }
                else
                {
                    creditCard.Decline_Message = _cardService.GetMessage(messageCode);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Method to check if card is expired or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        public bool Card_Is_Expired(ref Credit_Card creditCard)
        {
            var monthNow = DateAndTime.Month(DateTime.Now);
            var yearNow = DateAndTime.Year(DateTime.Now);

            var returnValue = true;

            // If we are not supposed to check the expiry date then
            // just set it to 'Not Expired'.
            if (CardIsValid(ref creditCard))
            {
                if (!creditCard.CheckExpiryDate)
                {
                    returnValue = false;

                }
                else if (!string.IsNullOrEmpty(creditCard.Expiry_Year) && !string.IsNullOrEmpty(creditCard.Expiry_Month))
                {
                    // Convert the Expiry Year and month to an internal VB date. The
                    // day of the month doesn't matter so I used '20' just for the
                    // hell of it.
                    var cardDate = DateTime.Parse(creditCard.Expiry_Month + "/20" + creditCard.Expiry_Year);
                    // Nicolette added to fix expiry date problem for cards which will expire
                    // between 2030 and 2070
                    int expiryYear;
                    if (double.Parse(creditCard.Expiry_Year) >= 30 && double.Parse(creditCard.Expiry_Year) <= 70)
                    {
                        expiryYear = DateAndTime.Year(cardDate) + 100;
                    }
                    else
                    {
                        expiryYear = DateAndTime.Year(cardDate);
                    }
                    // Nicolette end
                    //            Expiry_Year = Year(Card_Date)
                    var expiryMonth = DateAndTime.Month(cardDate);
                    if (expiryYear > yearNow)
                    {
                        returnValue = false;
                    }
                    else if (expiryYear == yearNow)
                    {
                        if (expiryMonth >= monthNow)
                        {
                            returnValue = false;
                        }
                    }
                }
            }
            else
            {
                returnValue = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Method to check if card is valid or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        public bool CardIsValid(ref Credit_Card creditCard)
        {
            short n = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            creditCard.CardType = Get_CardType(ref creditCard);
            if (creditCard.Card_Swiped)
            {
                n = (short)(creditCard.Swipe_String.IndexOf("?") + 1);
                var m = (short)(creditCard.Swipe_String.IndexOf(";") + 1);

                // No Track 2 if you don't have a ';' character
                if (m == 0)
                {
                    creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, (short)8141); // "Track 2 Missing"
                    return false;
                }
                //Shiny Oct8, 2009 commentd this and change the track1 checking not to use  n should be m-1  because we found cards with more than 1 qn mark before ;, so system will consider % .... ? (first ) as track1 and ignore rest of the part before ;

                // Invalid card if '?;' doesn't occur between track 1 and track 2





                if (!string.IsNullOrEmpty(creditCard.Track1) && n > 1 & n > m) // assumption if there is track1 , n should be before m (;)
                {
                    creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, (short)8142); //"Invalid Card Scan"
                    return false;
                }
            }
            //Shiny Oct8, 2009 commentd this and change the track1 checking not to use %b- because new cards are with %E and %B( they have track1 but nt starting with %b
            //    ' Track 1 must be of the form "%b .... ?"
            //    If creditCard.Track1 <> "" And _

            //        CardIsValid = False
            //        creditCard.Invalid_Reason = GetResString(8143)     ' "Invalid Track 1 Data"
            //        Exit Property
            //    End If



            // Track 1 must be of the form "% .... ?"
            if (!string.IsNullOrEmpty(creditCard.Track1)
                && (creditCard.Track1.Substring(0, 1) != "%"
                || creditCard.Track1.Substring(creditCard.Track1.Length - 1, 1) != "?"))
            {
                creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8143); // "Invalid Track 1 Data"
                return false;
            }
            //Shiny end

            // Track 2 must be of the form "; ..... ?"



            //       (Left$(creditCard.Track2, 1) <> ";" Or Right$(creditCard.Track2, 1) <> "?" Or InStr(1, creditCard.Track2, "=") = 0) Then
            if (!string.IsNullOrEmpty(creditCard.Track2)
                && (creditCard.Track2.Substring(0, 1) != ";"
                || creditCard.Track2.Substring(creditCard.Track2.Length - 1, 1) != "?"))
            {

                creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8144); //"Invalid Track 2 Data"
                return false;
            }




            // We still need to check the = sign and 5 characters
            //Only if CheckExpiryDate is checked
            if (creditCard.CheckExpiryDate)
            {
                if (!string.IsNullOrEmpty(creditCard.Track2))
                {
                    var eq = (short)(creditCard.Track2.IndexOf("=") + 1);
                    if (eq == 0 | eq + 5 > creditCard.Track2.Length)
                    {
                        creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8144); //"Invalid Track 2 Data"
                        return false;
                    }
                }
            }

            // If it didn't match any of the card codes then it is not an accepted card.

            if (creditCard.CardType == null)
            {
                creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8145); //"Not an Accepted Card Type."
                return false;
            }
            creditCard.Crd_Type = creditCard.CardType.CardType; //Sajan added to assign cardtype-02/13/02
            creditCard.Name = creditCard.CardType.Name; // 
            // Card must be all numeric
            for (n = 1; n <= creditCard.Cardnumber.Length; n++)
            {
                if (double.Parse(creditCard.Cardnumber.Substring(n - 1, 1)) < double.Parse("0") || double.Parse(creditCard.Cardnumber.Substring(n - 1, 1)) > double.Parse("9"))
                {
                    creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8146); //"Non-Numeric characters in Card Number"
                    return false;
                }
            }

            // It cannot have less then the minimum or more than the maximum digits.
            if (creditCard.Cardnumber.Length < creditCard.CardType.MinLength || creditCard.Cardnumber.Length > creditCard.CardType.MaxLength)
            {
                creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8147); //"Invalid card number length"
                return false;
            }

            // It must pass the Mod 10 Check
            string cardMask;
            cardMask = creditCard.CardType.CheckDigitMask.Length == 0 ? "" : creditCard.CardType.CheckDigitMask.ToUpper();

            if (Valid_Mod10(ref creditCard, cardMask)) return true;
            creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8148); //"Invalid Check Digit"
            return false;
        }

        /// <summary>
        /// Method to set invalid credit card reason
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Reason</returns>
        public string Invalid_Reason(ref Credit_Card creditCard)
        {
            string returnValue;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (string.IsNullOrEmpty(creditCard.Invalid_Reason))
            {
                returnValue = CardIsValid(ref creditCard) ? _resourceManager.GetResString(offSet, (short)8149) : creditCard.Invalid_Reason;
            }
            else
            {
                returnValue = creditCard.Invalid_Reason;
            }
            return returnValue;

        }

        // This returns the language as a string. The default is 'English' if no language code was supplied.
        /// <summary>
        /// Method to set credit card language
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Language</returns>
        public string Language(ref Credit_Card creditCard)
        {
            string returnValue;
            if (string.IsNullOrEmpty(creditCard.Language)) //Sajan
            {
                if (creditCard.CardType == null)
                {
                    returnValue = "English"; //GetResString(100)

                }
                else if (creditCard.CardType.LanguageDigitPosition <= 0 || !creditCard.Card_Swiped)
                {
                    returnValue = "English"; //GetResString(100)     '

                }
                else
                {
                    if (!string.IsNullOrEmpty(creditCard.Track2))
                    {





                        var iQuestion = (short)(creditCard.Track2.IndexOf("?") + 1); //if it's manually entry,Track2="",then set default language as English ''04/13/05 Nancy
                        var ld = "";
                        ld = iQuestion > 1 ? creditCard.Track2.Substring(iQuestion - 1 - 1, 1) : "1";

                        switch (ld)
                        {
                            case "1":
                                returnValue = "English"; //GetResString(100)
                                break;
                            case "2":
                                returnValue = "French"; //GetResString(300)
                                break;
                            default:
                                returnValue = "English"; //GetResString(100)
                                break;
                        }
                    }
                    else
                    {
                        returnValue = "English"; //GetResString(100)
                    }
                }


                creditCard.Language = returnValue;

            }
            else
            {
                returnValue = creditCard.Language;
            }
            return returnValue;

        }

        /// <summary>
        /// Method to set card number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="cardNumber">Card number</param>
        public void SetCardnumber(ref Credit_Card creditCard, string cardNumber)
        {
            creditCard.Cardnumber = cardNumber;
            if (_policyManager.EMVVersion && (creditCard.Crd_Type == "C" || creditCard.Crd_Type == "D")) //EMVVERSION
            {
                creditCard.CardType = null;
            }
            else
            {
                creditCard.CardType = Get_CardType(ref creditCard);
            }

            creditCard.entryMethod = Convert.ToString(creditCard.Card_Swiped ? "S" : "M");
            if (creditCard.CardType != null)
            {
                if (creditCard.Card_Swiped)
                {
                    if (creditCard.CardType.UsageIDPosition > 0)
                    {
                        SetUsageCode(ref creditCard, creditCard.Track2.Substring(creditCard.CardType.UsageIDPosition - 1, creditCard.CardType.UsageIDLength));
                    }
                }
                else
                {
                    //If Not CardType.UserEnteredRestriction Then
                    //
                    //WEX and VI have default values and UserEnteredRest=false
                    //VF and MC UserEntereRest=true; For VI user has to enter pr.restriction which inclueds usage id
                    //For MC user needs to enter pr.restriction which IS usage id; prompt id is 1 by default
                    SetUsageCode(ref creditCard, creditCard.CardType.DefaultUsageCode);
                }
            }

        }

        /// <summary>
        /// Method to validate whether to call the bank or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        public bool Call_The_Bank(ref Credit_Card creditCard)
        {

            bool returnValue;
            if (_policyManager.EMVVersion && (creditCard.Crd_Type == "C" || creditCard.Crd_Type == "D")) //EMVVERSION
            {
                returnValue = true;
            }
            else
            {
                returnValue = creditCard.TendCard != null && creditCard.TendCard.CallTheBank;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get receipt total text
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Receipt total</returns>
        public string ReceiptTotalText(ref Credit_Card creditCard)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var returnValue = creditCard.TendCard == null ? _resourceManager.GetResString(offSet, 210) : creditCard.TendCard.ReceiptTotalText;
            return returnValue;
        }

        /// <summary>
        /// Method to set track 1
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="track1">Track 1</param>
        public void SetTrack1(ref Credit_Card creditCard, string track1)
        {
            short n1;
            short n2 = 0;
            string[] namePart = new string[11];

            for (n1 = 1; n1 <= (namePart.Length - 1); n1++)
            {
                namePart[n1] = "";
            }
            creditCard.Track1 = track1;
            var name = "";

            if (creditCard.Track1.Length > 0)
            {

                n1 = (short)(creditCard.Track1.IndexOf("^") + 1);
                if (n1 > 0)
                {
                    n2 = (short)(creditCard.Track1.IndexOf("^", n1 + 1 - 1) + 1);
                }

                if (n1 > 0 & n2 > n1)
                {
                    name = creditCard.Track1.Substring(n1 + 1 - 1, n2 - n1 - 1).Trim();
                }

                n2 = 0;
                if (name.Length > 0 && name.IndexOf("/") + 1 > 0)
                {
                    while (name.Length > 0)
                    {
                        n2++;
                        n1 = (short)(name.IndexOf("/") + 1);
                        if (n1 > 0)
                        {
                            namePart[n2] = name.Substring(0, n1 - 1);
                            name = name.Substring(n1 + 1 - 1).Trim();
                        }
                        else
                        {
                            namePart[n2] = name.Trim();
                            name = "";
                        }
                    }
                }

                if (n2 > 0)
                {
                    for (n1 = (short)(namePart.Length - 1); n1 >= 1; n1--)
                    {
                        if (namePart[n1] != "")
                        {
                            name = name + " " + namePart[n1];
                        }
                    }
                }

                name = modStringPad.Proper_Case(name.Trim());
            }

            //mvarCustomerName = Name
            creditCard.Customer_Name = name;
        }

        /// <summary>
        /// Method to set track 2
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="track2">Track 2</param>
        public void SetTrack2(ref Credit_Card creditCard, string track2)
        {
            short n;
            string c;
            string cardNum = "";
            creditCard.Track2 = track2;

            var eq = (short)(creditCard.Track2.IndexOf("=") + 1);



            if (eq == 0)
            {
                var pos = (short)(creditCard.Track2.IndexOf("?") + 1);
                if (pos <= 2)
                {
                    SetCardnumber(ref creditCard, "");
                }
                else
                {
                    for (n = (short)(pos - 1); n > 1; n--)
                    {
                        c = creditCard.Track2.Substring(n - 1, 1);
                        if (double.Parse(c) >= double.Parse("0") && double.Parse(c) <= double.Parse("9"))
                        {
                            cardNum = c + cardNum;
                        }
                        else
                        {
                            break;
                        }
                    }
                    SetCardnumber(ref creditCard, cardNum);
                }
                creditCard.Service_Code = "";
                creditCard.Expiry_Date = "";
                return;
            }

            if (eq == 0 | eq + 4 > creditCard.Track2.Length)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                creditCard.Invalid_Reason = _resourceManager.GetResString(offSet, 8144); //"Invalid Track 2 Data"
                return;
            }

            for (n = (short)(eq - 1); n > 1; n--)
            {
                c = creditCard.Track2.Substring(n - 1, 1);
                if (double.Parse(c) >= double.Parse("0") && double.Parse(c) <= double.Parse("9"))
                {
                    cardNum = c + cardNum;
                }
                else
                {
                    break;
                }
            }

            creditCard.Service_Code = eq + 8 <= creditCard.Track2.Length ? creditCard.Track2.Substring(eq + 5 - 1, 3) : "";

            creditCard.Expiry_Date = creditCard.Track2.Substring(eq + 1 - 1, 4);

            SetCardnumber(ref creditCard, cardNum);
        }

        /// <summary>
        /// Method to check if card is positive card
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        public bool CardInPcf(ref Credit_Card creditCard)
        {
            var returnValue = false;

            // If we are not supposed to check for this card type in the PCF then
            // just return true (i.e. not in the PCF)
            creditCard.AskPCFPin = false; //###PTC


            if (!creditCard.CheckPCF)
            {
                returnValue = true;
            }
            else
            {
                var pin = _cardService.GetPostalCardPin(creditCard.Cardnumber);
                if (pin != null)
                {
                    returnValue = true;
                }
                if (!string.IsNullOrEmpty(pin))
                {
                    //###PTC - Mar19,2008 - if pin number ask for the pin number from user
                    creditCard.AskPCFPin = true;
                    creditCard.PCFPIN = pin;
                }
                //##PTC End
            }

            return returnValue;
        }

        /// <summary>
        /// Method to set usage code
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="usageCode">Usage code</param>
        public void SetUsageCode(ref Credit_Card creditCard, string usageCode)
        {
            creditCard.UsageCode = usageCode;

            // Nicolette added, set PromptCode property
            if (creditCard.Card_Swiped)
            {
                if (!string.IsNullOrEmpty(creditCard.Track2) & creditCard.CardType.PromptIDPosition > 0)
                {
                    SetPromptCode(ref creditCard, creditCard.Track2.Substring(creditCard.CardType.PromptIDPosition - 1, creditCard.CardType.PromptIDlength));
                    return;
                }
            }

            // PromptCode is on CardNumber
            if (creditCard.CardType.PromptIDPosition <= creditCard.CardType.MaxLength & creditCard.CardType.PromptIDPosition != 0)
            {
                // - cardnumber does not have a prefix of ";" so promptidpos starts 1 ch sooner
                SetPromptCode(ref creditCard, creditCard.Cardnumber.Substring(creditCard.CardType.PromptIDPosition - 1 - 1, creditCard.CardType.PromptIDlength));
                return;
            }

            // PromptCode is the same with UsageCode
            if (creditCard.CardType.UsageIDPosition == creditCard.CardType.PromptIDPosition & creditCard.CardType.UsageIDLength == creditCard.CardType.PromptIDlength & creditCard.CardType.UsageIDPosition != 0 & creditCard.CardType.PromptIDPosition != 0)
            {
                SetPromptCode(ref creditCard, creditCard.UsageCode);
                return;
            }

            // Use Default value
            SetPromptCode(ref creditCard, creditCard.CardType.DefaultPromptCode);
            creditCard.usageType = _cardService.GetUsageType(creditCard.CardCode, creditCard.UsageCode);
            // Nicolette end

        }

        /// <summary>
        /// Method to set vehicle number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="vehicleNumber">Vehicle number</param>
        public void SetVehicleNumber(ref Credit_Card creditCard, string vehicleNumber)
        {
            var field = "Vehicle";
            var formatAnswer = _cardService.GetFormatAnswers(creditCard.CardCode.ToString(), field);
            if (formatAnswer != null)
            {
                creditCard.Vechicle_Number = Convert.ToString(Strings.UCase(formatAnswer.Justified) == "L" ?
                    Strings.Left(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                                     formatAnswer.Length) + vehicleNumber, formatAnswer.Length) :
                    (Strings.Right(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                    formatAnswer.Length) + vehicleNumber,
                    formatAnswer.Length)));
            }
            else // if there is not setting into database, use a default format
            {
                creditCard.Vechicle_Number = ("000000" + vehicleNumber).Substring(("000000" + vehicleNumber).Length - 6, 6);
            }
        }

        /// <summary>
        /// Method to set driver number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="driverNumber">Driver number</param>
        public void SetDriverNumber(ref Credit_Card creditCard, string driverNumber)
        {
            var field = "Driver Number";
            var formatAnswer = _cardService.GetFormatAnswers(creditCard.CardCode.ToString(), field);
            if (formatAnswer != null)
            {
                creditCard.Driver_Number = Convert.ToString((Strings.UCase(formatAnswer.Justified) == "L") ?
                    (Strings.Left(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                    formatAnswer.Length) + driverNumber,
                   formatAnswer.Length)) :
                    (Strings.Right(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                    formatAnswer.Length) + driverNumber,
                    formatAnswer.Length)));
            }
            else // if there is not setting into database, use a default format
            {
                creditCard.Driver_Number = ("000000" + driverNumber).Substring(("000000" + driverNumber).Length - 6, 6);
            }
        }

        /// <summary>
        /// Method to set Id number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="idNumber">Id number</param>
        public void SetIdNumber(ref Credit_Card creditCard, string idNumber)
        {
            var field = "ID Number";
            var formatAnswer = _cardService.GetFormatAnswers(creditCard.CardCode.ToString(), field);
            if (formatAnswer != null)
            {
                creditCard.ID_Number = Convert.ToString((Strings.UCase(formatAnswer.Justified) == "L") ?
                    (Strings.Left(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                    formatAnswer.Length) + idNumber,
                    formatAnswer.Length)) :
                    (Strings.Right(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                    formatAnswer.Length) + idNumber,
                    formatAnswer.Length)));
            }
            else // if there is not setting into database, use a default format
            {
                creditCard.ID_Number = ("000000" + idNumber).Substring(("000000" + idNumber).Length - 6, 6);
            }
        }

        /// <summary>
        /// Method to set odometer number 
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="odoMeter">Odometer number</param>
        public void SetOdoMeter(ref Credit_Card creditCard, string odoMeter)
        {
            var field = "Odometer";
            var formatAnswer = _cardService.GetFormatAnswers(creditCard.CardCode.ToString(), field);
            if (formatAnswer != null)
            {
                creditCard.Odometer_Number = Convert.ToString((Strings.UCase(formatAnswer.Justified) == "L") ?
                    (Strings.Left(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                    formatAnswer.Length) + odoMeter,
                    formatAnswer.Length)) :
                    (Strings.Right(new string(Strings.Chr(Convert.ToInt32(formatAnswer.CharFilled)),
                    formatAnswer.Length) + odoMeter,
                    formatAnswer.Length)));
            }
            else // if there is not setting into database, use a default format
            {
                creditCard.Odometer_Number = ("000000" + odoMeter).Substring(("000000" + odoMeter).Length - 6, 6);
            }

        }

        /// <summary>
        /// Method to return tender description
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Tender description</returns>
        public string Return_TendDesc(Credit_Card creditCard)
        {
            var returnValue = "";
            if (creditCard.TendCard == null)
            {
                return returnValue;
            }
            returnValue = _tenderService.GetTenderName(creditCard.TendCode);

            return returnValue;
        }

        // This property returns the tender class from TendMast table based on tendcode.
        /// <summary>
        /// Method to return tender class
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Tender class</returns>
        public string Return_TendClass(Credit_Card creditCard)
        {
            var returnValue = "";
            //   to get the correct tender class for missing properties of tenders in EMV version
            if (_policyManager.EMVVersion)
            {
                if (creditCard.TendCard == null && creditCard.Crd_Type == "D")
                {
                    returnValue = _cardService.GetTendClass(creditCard.CardCode);
                    return returnValue;
                }
                if (creditCard.TendCard == null)
                {
                    return returnValue;
                }
            }
            else
            {
                //   end
                if (creditCard.TendCard == null)
                {
                    return returnValue;
                }
            }
            returnValue = _cardService.GetTendClassByTendCode(creditCard.TendCode);
            return returnValue;
        }

        // type
        // it works only for tenders with a signle instance. It is mainly written to fix the mess in debit cards processing
        /// <summary>
        /// Method to return card codes
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Card code</returns>
        public string Return_CardCode(ref Credit_Card creditCard)
        {
            string returnValue;
            var card = _cardService.GetCardCode(creditCard.Crd_Type);
            if (card == null)
            {
                returnValue = "";
                creditCard.Name = "";
            }
            else
            {
                returnValue = card.CardCode.ToString();
                creditCard.Name = card.Name;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to authorize a card
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        public void Authorize_Card(ref Credit_Card creditCard)
        {
            if (Conversion.Val(DateTime.Now.ToString("ss")) < 3)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                creditCard.Authorization_Number = "";
                creditCard.Decline_Message = _resourceManager.GetResString(offSet, 8150); //"Bank Decline Message"
            }
            else
            {
                creditCard.Authorization_Number = DateTime.Now.ToString("hhmmss");
            }

        }

        //==============================================
        // This function create the product codes string
        //==============================================
        /// <summary>
        /// Method to set product codes for a till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Product codes</returns>
        public string ProductCodes(int tillNumber)
        {



            string strTemp = "";
            short i = 0;












            _cardService.CreateTableIndex();
            var rsProcess = _cardService.CardProcessFields(tillNumber);
            var rsFormat = _cardService.GetStringFormat();
            int index = 0;
            do
            {
                int count = 0;
                foreach (var fld in rsProcess)
                {
                    count++;
                    if (fld.Name == rsFormat[index].MapField)
                    {
                        if (rsFormat[index].Sum & fld.Value == typeof(double))
                        {
                            var sum = _cardService.GetCardProcessSum(fld.Name, tillNumber);
                            strTemp = strTemp + NumberFormat(sum.ToString(), rsFormat[index].Format);
                        }
                        else
                        {
                            if (count >= rsProcess.Count)
                            {
                                if (Information.IsNumeric(fld.Value))
                                {
                                    if (rsFormat[index].Sum)
                                    {
                                    }
                                    else
                                    {
                                        strTemp = strTemp + NumberFormat(Convert.ToString(fld.Value),
                                           rsFormat[index].Format);
                                    }
                                }
                                else
                                {
                                    strTemp = strTemp + Strings.Left(Convert.ToString(fld.Value),
                                        Strings.Len(rsFormat[index].Format));
                                }
                            }
                            else
                            {
                                strTemp = strTemp + NumberFormat("0", rsFormat[index].Format);
                            }
                        }
                        i = rsFormat[index].NoField;
                        break;
                    }
                }
                index++;
                if (index >= rsFormat.Count)
                {
                    break;
                }
                if (count >= rsProcess.Count)
                {
                    if (Convert.ToInt32(rsFormat[index].NoField) > i)
                    {
                        index++;
                    }
                    if (rsFormat[index].NoField == 1)
                    {
                        index = 0;
                    }
                }
            } while (index < rsFormat.Count);
            var returnValue = strTemp;
            return returnValue;
        }

        //=====================================================
        // Insert data into CardProcess table
        //======================================================
        /// <summary>
        /// Method to insert data
        /// </summary>
        /// <param name="cdSale">Sale</param>
        /// <param name="cc">Credit card</param>
        /// <param name="fuelType">Fuel type</param>
        /// <returns>Amount</returns>
        public decimal InsertData(Sale cdSale, Credit_Card cc, string fuelType)
        {
            decimal returnValue;





            Sale_Line sl;
            //string strCardProduct;
            //string strFuelMeasure;
            //string strFuelServiceType;
            //float sngQty;
            //decimal curAmount;
            //decimal dblDiscount;
            //string strSql;
            //string blnFuel;
            decimal retAmt = new decimal();
            string usageType;

            _cardService.DeleteCardProcessFromDbTill(cdSale.TillNumber);
            var cardProcesses = _cardService.GetCardProcessFromDbTIll(cdSale.TillNumber);
            var cardProducts = _cardService.GetCardProducts();
            var blnIndex = _cardService.GetCardFormat();


            foreach (Sale_Line tempLoopVarSl in cdSale.Sale_Lines)
            {
                sl = tempLoopVarSl;
                // Add only the lines whom products codes are related to
                // proper usage of selected fleet card

                //
                //First see what usagecode translated to ALL or FUEL
                var cardUsage = _cardService.GetUsageType(cc.Bank_CardID, cc.UsageCode);
                if (!string.IsNullOrEmpty(cardUsage))
                {
                    usageType = cardUsage;
                }
                else
                {
                    returnValue = 0;
                    return returnValue;
                }
                var selectedProduct = cardProducts.FirstOrDefault(c => c.BankCardID == cc.Bank_CardID
                && c.CardProductCode == sl.CardProductCode
                && c.UsageType == usageType);
                //end - Svetlana

                if (selectedProduct != null)
                {




                    if (bool.Parse(blnIndex))
                    {
                        var selectedCardProcess = cardProcesses.FirstOrDefault(c => c.CardProductCode == sl.CardProductCode);
                        if (selectedCardProcess == null)
                        {
                            selectedCardProcess = new CardProcess();
                            selectedCardProcess.TILL_NUM = cdSale.TillNumber;
                            selectedCardProcess.FuelMeasure = _policyManager.FUEL_UM;
                            selectedCardProcess.FuelServiceType = fuelType;
                            selectedCardProcess.CardProductCode = sl.CardProductCode;
                            selectedCardProcess.Qty = sl.Quantity;
                            selectedCardProcess.Amount = sl.Amount;
                            selectedCardProcess.Discount = sl.Line_Discount + sl.Discount_Adjust;
                            selectedCardProcess.SaleTax = sl.AddedTax; // curSaleTax
                            selectedCardProcess.Fuel = sl.ProductIsFuel;
                            _cardService.AddCardProcess(selectedCardProcess);
                        }
                        else
                        {
                            selectedCardProcess.Qty = selectedCardProcess.Qty + sl.Quantity;
                            selectedCardProcess.Amount = selectedCardProcess.Amount + sl.Amount;
                            selectedCardProcess.Discount = selectedCardProcess.Discount + sl.Line_Discount + sl.Discount_Adjust;
                            selectedCardProcess.SaleTax = selectedCardProcess.SaleTax + sl.AddedTax; // curSaleTax
                            _cardService.UpdateCardProcess(selectedCardProcess);
                        }
                    }
                    else
                    {
                        var selectedCardProcess = new CardProcess();
                        selectedCardProcess.TILL_NUM = cdSale.TillNumber;
                        selectedCardProcess.FuelMeasure = _policyManager.FUEL_UM;
                        selectedCardProcess.FuelServiceType = fuelType;
                        selectedCardProcess.CardProductCode = sl.CardProductCode;
                        selectedCardProcess.Qty = sl.Quantity;
                        selectedCardProcess.Amount = sl.Amount;
                        selectedCardProcess.Discount = sl.Line_Discount + sl.Discount_Adjust;
                        selectedCardProcess.SaleTax = sl.AddedTax; // curSaleTax
                        selectedCardProcess.Fuel = sl.ProductIsFuel;
                        _cardService.AddCardProcess(selectedCardProcess);
                    }

                    //        strFuelMeasure = GetPol("FUEL_UM")
                    //        strFuelServiceType = Me.FuelServiceType
                    //        strCardProduct = SL.CardProductCode
                    //        sngQty = SL.Quantity
                    //        curAmount = SL.Amount
                    //        blnFuel = SL.ProductIsFuel
                    //        dblDiscount = SL.Line_Discount + SL.Discount_Adjust
                    //        strSQL = "INSERT INTO CardProcess " & _



                    //        dbTill_Renamed.Execute strSQL




                    retAmt = retAmt + sl.Amount - (decimal)sl.Line_Discount - (decimal)sl.Discount_Adjust + sl.AddedTax + sl.TotalCharge; //curSaleTax

                }
            }
            returnValue = retAmt;
            return returnValue;
        }

        /// <summary>
        /// Method to get card product codes
        /// </summary>
        /// <param name="cdSale">Sale</param>
        /// <param name="creditCard">Credit card</param>
        public void GetCardProductCodes(ref Sale cdSale, Credit_Card creditCard)
        {
            foreach (Sale_Line tempLoopVarSl in cdSale.Sale_Lines)
            {
                var sl = tempLoopVarSl;
                var cardProductCode = _cardService.GetCardProductLinkCode(creditCard.Bank_CardID, sl.Dept, "", "", "");
                if (!string.IsNullOrEmpty(cardProductCode))
                {
                    sl.CardProductCode = cardProductCode;
                }
                else
                {
                    cardProductCode = _cardService.GetCardProductLinkCode(creditCard.Bank_CardID, "", sl.Sub_Dept, "", "");
                    if (!string.IsNullOrEmpty(cardProductCode))
                    {
                        sl.CardProductCode = cardProductCode;
                    }
                    else
                    {
                        cardProductCode = _cardService.GetCardProductLinkCode(creditCard.Bank_CardID, "", "", sl.Sub_Detail, "");
                        if (!string.IsNullOrEmpty(cardProductCode))
                        {
                            sl.CardProductCode = cardProductCode;
                        }
                        else
                        {
                            cardProductCode = _cardService.GetCardProductLinkCode(creditCard.Bank_CardID, "", "", "", sl.Stock_Code);
                            if (!string.IsNullOrEmpty(cardProductCode))
                            {
                                sl.CardProductCode = cardProductCode;
                            }
                            else
                            {
                                sl.CardProductCode = "";
                            }
                        }
                    }
                }
            }
        }

        //
        /// <summary>
        /// Method to get valid products for card
        /// </summary>
        /// <param name="cdSale">Sale</param>
        /// <param name="creditCard">Credit card</param>
        /// <param name="strLineNum">Line number</param>
        /// <returns>Valid amount</returns>
        public float GetValidProductForCard(Sale cdSale, Credit_Card creditCard, ref string strLineNum)
        {
            float returnValue = 0;
            Sale_Line sl;
            float validAmt = 0;

            validAmt = 0;
            strLineNum = "";



            if (string.IsNullOrEmpty(creditCard.Bank_CardID))
            {
                returnValue = 0;
                return returnValue;
            }

            if (!creditCard.CardProductRestrict)
            {

                returnValue = (float)cdSale.Sale_Totals.Gross;


                strLineNum = "";
                foreach (Sale_Line tempLoopVar_sl in cdSale.Sale_Lines)
                {
                    sl = tempLoopVar_sl;


                    if (!string.IsNullOrEmpty(sl.ThirdPartyExtractCode))
                    {
                        sl.CardProductCode = sl.ThirdPartyExtractCode;
                    }
                    else
                    {
                        sl.CardProductCode = "10";
                    }
                    strLineNum = strLineNum + Convert.ToString(sl.Line_Num) + ",";
                }
                if (!string.IsNullOrEmpty(strLineNum) && strLineNum.Substring(strLineNum.Length - 1, 1) == ",")
                {
                    strLineNum = strLineNum.Substring(0, strLineNum.Length - 1);
                }


                return returnValue;
            }


            var cardProductRestriction = _cardService.GetCardProductRestriction(creditCard.Bank_CardID, "", "", "", "");
            if (cardProductRestriction == null)
            {
                returnValue = 0;
                return returnValue;
            }

            foreach (Sale_Line tempLoopVarSl in cdSale.Sale_Lines)
            {
                sl = tempLoopVarSl;
                cardProductRestriction = !string.IsNullOrEmpty(sl.Dept) ? _cardService.GetCardProductRestriction(creditCard.Bank_CardID, sl.Dept, "", "", "") : null;
                if (cardProductRestriction != null)
                {
                    sl.CardProductCode = cardProductRestriction.CardProductCode;
                    sl.RestrictedAmount = cardProductRestriction.Amount;
                }
                else
                {
                    cardProductRestriction = !string.IsNullOrEmpty(sl.Sub_Dept) ? _cardService.GetCardProductRestriction(creditCard.Bank_CardID, "", sl.Sub_Dept, "", "") : null;
                    if (cardProductRestriction != null)
                    {
                        sl.CardProductCode = cardProductRestriction.CardProductCode;
                        sl.RestrictedAmount = cardProductRestriction.Amount;
                    }
                    else
                    {
                        cardProductRestriction = !string.IsNullOrEmpty(sl.Sub_Detail) ? _cardService.GetCardProductRestriction(creditCard.Bank_CardID, "", "", sl.Sub_Detail, "") : null;
                        if (cardProductRestriction != null)
                        {
                            sl.CardProductCode = cardProductRestriction.CardProductCode;
                            sl.RestrictedAmount = cardProductRestriction.Amount;
                        }
                        else
                        {
                            cardProductRestriction = !string.IsNullOrEmpty(sl.Stock_Code) ? _cardService.GetCardProductRestriction(creditCard.Bank_CardID, "", "", "", sl.Stock_Code) : null;
                            if (cardProductRestriction != null)
                            {
                                sl.CardProductCode = cardProductRestriction.CardProductCode;
                                sl.RestrictedAmount = cardProductRestriction.Amount;
                            }
                            else
                            {
                                sl.CardProductCode = "";
                                sl.RestrictedAmount = 0;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sl.CardProductCode))
                {
                    var lineAmount = sl.Amount - (decimal)sl.Line_Discount - (decimal)sl.Discount_Adjust + sl.AddedTax + sl.TotalCharge;
                    if ((sl.RestrictedAmount == 0) || (sl.RestrictedAmount != 0 & lineAmount < sl.RestrictedAmount)) // no amount restriction set for the product, consider the total
                    {
                        validAmt = validAmt + (float)lineAmount;
                    }
                    else
                    {
                        validAmt = validAmt + (float)sl.RestrictedAmount;
                    }
                    strLineNum = strLineNum + Convert.ToString(sl.Line_Num) + ",";
                }
            }
            if (!string.IsNullOrEmpty(strLineNum))
            {
                if (strLineNum.Substring(strLineNum.Length - 1, 1) == ",")
                {
                    strLineNum = strLineNum.Substring(0, strLineNum.Length - 1);
                }
            }

            returnValue = validAmt;
            return returnValue;
        }

        //   this function returns the tender code based on the swipestring
        /// <summary>
        /// Method to find tender code
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Tender code</returns>
        public string Find_TenderCode(ref Sale sale, ref Credit_Card creditCard)
        {
            string returnValue = "";
            string tenderUsed = "";

            // For debit card this function returns tenderclass
            var startPos = (short)(creditCard.Swipe_String.IndexOf("=") + 1);
            if (startPos > 0 && creditCard.Swipe_String.Length >= startPos + 7)
            {
                if (creditCard.Swipe_String.Substring(startPos + 5 - 1, 3) == "798" ||
                    creditCard.Swipe_String.Substring(startPos + 5 - 1, 3) == "799" ||
                    creditCard.Swipe_String.Substring(startPos + 5 - 1, 3) == "220" ||
                    creditCard.Swipe_String.Substring(startPos + 5 - 1, 3) == "120")
                {
                    tenderUsed = "DBCARD";
                }
            }

            if (!string.IsNullOrEmpty(tenderUsed))
            {
                returnValue = tenderUsed;
                creditCard.AutoRecognition = true;
                return returnValue;
            }

            var cardTypes = Load_CardTypes();
            // Wasn't a Debit Card. Check other card types.
            foreach (Card tempLoopVarCd in cardTypes)
            {
                var cd = tempLoopVarCd;
                foreach (CardCode tempLoopVarCCode in cd.CardCodes)
                {
                    var cCode = tempLoopVarCCode;
                    var ll = (short)cCode.LowerLimit.Length;
                    if (cd.CardType == "D") continue;
                    if (creditCard.Cardnumber.Length < ll || (!(double.Parse(creditCard.Cardnumber.Substring(0, ll)) >= double.Parse(cCode.LowerLimit)) || !(double.Parse(creditCard.Cardnumber.Substring(0, ll)) <= double.Parse(cCode.UpperLimit))))
                        continue;
                    creditCard.CardCode = cd.CardID;
                    creditCard.Name = cd.Name;
                    creditCard.Crd_Type = cd.CardType;
                    //creditCard.CardType = cd.CardType;
                    creditCard.TendCard = new TenderCard();
                    creditCard.TendCard = _cardService.LoadTenderCard(cd.CardID);
                    if (creditCard.Crd_Type == "C")
                    {
                        returnValue = !_policyManager.COMBINECR ? creditCard.TendCard.TenderCode : Return_TendClass(creditCard);
                    }

                    if (creditCard.Crd_Type == "F")
                    {

                        CacheManager.AddCreditCard(sale.TillNumber, sale.Sale_Num, creditCard);
                        //  - Gasking Charges
                        if (cd.VerifyCardNumber)
                        {
                            // 
                            if (ValidCustomerCard(ref creditCard))
                            {
                                // 
                                //  
                                if (_policyManager.CUST_EXPDATE &&
                                    DateAndTime.DateSerial(Convert.ToInt32(Convert.ToInt32(Strings.Left(Convert.ToString
                                            ((string.IsNullOrEmpty(creditCard.Expiry_Date)) ? "000" : creditCard.Expiry_Date), 2))),
                                        Convert.ToInt32(Convert.ToInt32(Strings.Right(Convert.ToString
                                            ((string.IsNullOrEmpty(creditCard.Expiry_Date)) ? "000" : creditCard.Expiry_Date), 2))),
                                        int.Parse("20")) < DateAndTime.Today)
                                {
                                    //WriteToLogFile "Expiry date set to " & Me.Expiry_Date
                                    //WriteToLogFile "Find_TenderCode date used is " & DateSerial(Right$(Me.Expiry_Date, 2), Left$(Me.Expiry_Date, 2), "20")
                                    //TODO: expiry date message
                                    // Chaps_Main.DisplayMessage(0, (short)1492, MessageType.OkOnly, creditCard.Expiry_Date, (byte)0);
                                }
                                else
                                {
                                    //   end
                                    if (creditCard.ARcustomerCard) //  - since we are verifying the card and identifying the customer we can know whether it is a an AR customer - If ValidARCustomerCard Then
                                    {
                                        returnValue = Convert.ToString(_policyManager.ARTender); //"ACCOUNT"
                                        creditCard.Trans_Date = DateAndTime.Now;
                                        creditCard.Trans_Time = DateAndTime.Now;
                                        sale.IsArTenderUsed = true;
                                        CacheManager.AddCreditCard(sale.TillNumber, sale.Sale_Num, creditCard);
                                    }
                                   else
                                    {
                                       // returnValue = creditCard.TendCard.TenderCode;
                                    }
                                }
                            }
                            // 
                        }
                        else
                        {
                            // 
                            returnValue = !_policyManager.COMBINEFLEET ? creditCard.TendCard.TenderCode : Return_TendClass(creditCard);
                        }
                    }

                    // Apr 06, 2009 Nicolette added to handle combined third party cards
                    if (creditCard.Crd_Type == "T")
                    {
                        if (_policyManager.ThirdParty)
                        {
                            //Milliplein removed
                            //if (!(Variables.Milliplein == null))
                            //{
                            //    if (Variables.Milliplein.CombineThirdParty)
                            //    {
                            //        returnValue = Return_TendClass(creditCard);
                            //    }
                            //    else
                            //    {
                            //        returnValue = creditCard.TendCard.TenderCode;
                            //    }
                            //}
                            //else
                            {
                                returnValue = creditCard.TendCard.TenderCode;
                            }
                        }
                        else
                        {
                            returnValue = creditCard.TendCard.TenderCode;
                        }
                    }
                    // Apr 06, 2009 Nicolette end

                    if (creditCard.Crd_Type != "C" && creditCard.Crd_Type != "F" && creditCard.Crd_Type != "T")
                    {
                        returnValue = creditCard.TendCard.TenderCode;
                    }
                    creditCard.AutoRecognition = true;
                    return returnValue;
                }
            }

            return returnValue;
        }
        //###PTC -End - Mar19,2008
        //  - Customercard to identify customer

        /// <summary>
        /// Method to set the swiped string
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="swipeData">Swiped text</param>
        public void SetSwipeString(ref Credit_Card creditCard, string swipeData)
        {
            creditCard.Swipe_String = swipeData;
            creditCard.Card_Swiped = true;
            var n = (short)(creditCard.Swipe_String.IndexOf("?") + 1);
            var m = (short)(creditCard.Swipe_String.IndexOf(";") + 1);

            if (m == 1)
            {
                SetTrack1(ref creditCard, "");
                SetTrack2(ref creditCard, creditCard.Swipe_String);
                //  EKO's special cards got more ? before the ;, so it is not matching the criteria n = m-1
                //    ElseIf n = m - 1 Then
            }
            else if (n <= m - 1)
            {
                // 
                SetTrack1(ref creditCard, creditCard.Swipe_String.Substring(0, n));
                SetTrack2(ref creditCard, creditCard.Swipe_String.Substring(m - 1));
            }
        }

        /// <summary>
        /// Method to check if valid customer card or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        public bool ValidCustomerCard(ref Credit_Card creditCard) // changed this to be the validcustomercard ' ValidARCustomerCard() As Boolean
        {
            var returnValue = false;


            //             "SELECT * " & _
            //             "FROM ClientCard inner join client on clientcard.cl_code = client.cl_code " & _
            //             "WHERE CardNum = '" & Me.CardNumber & "' and client.CL_arcust =1", dbMaster, _
            //             adOpenForwardOnly, adLockReadOnly)
            var clientCard = _customerService.GetClientCardForCustomer(creditCard.Cardnumber);
            if (clientCard == null) return false;
            //shiny added in april13,
            if (clientCard.CardStatus != 'V')
            {
                creditCard.ARcustomerCard = false;
                creditCard.CardProfileID = "";
            }
            else
            {
                returnValue = true; //ValidARCustomerCard = True
                creditCard.CustomerCode = clientCard.ClientCode;
                creditCard.ARcustomerCard = clientCard.ClientArCustomer;
                //            mvarExpiry_Date = IIf(IsNull(rs![ExpDate]), Date, rs![ExpDate])  '   don't overwrite Expiry date from the DB
                creditCard.CardProfileID = _policyManager.RSTR_PROFILE ? clientCard.ProfileID : "";
            }
            return returnValue;
        }

        //   end

        // ********************************************************************************
        // Load_CardTypes - Load the definitions of the cards accepted by this system
        //                   into the 'CardTypes' collection.
        // ********************************************************************************
        /// <summary>
        /// Method to load card types
        /// </summary>
        /// <returns>Card types</returns>
        public CardTypes Load_CardTypes()
        {
            var cardType = new CardTypes();
            var cards = _cardService.GetCards();
            foreach (var card in cards)
            {
                var myCard = card;
                myCard.CardCodes = new CardCodes();
                var cardCodes = _cardService.GetCardCodes(card.CardID);
                foreach (var cardCode in cardCodes)
                {
                    myCard.CardCodes.Add(cardCode.LowerLimit, cardCode.UpperLimit, "");
                }
                cardType.Add(myCard, card.CardID.ToString());
            }
            return cardType;
        }

        /// <summary>
        /// Method to get tender card using tender code
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Tender card</returns>
        public TenderCard GetTendCard(string tenderCode)
        {
            return _cardService.GetTenderCardByTenderCode(tenderCode);
        }

        #region Private methods















        //=============================================================================
        // This function format the string containing a number according with strFormat
        // eg.: NumberFormat("22.85","0000v000")->0022850 - Nicolette
        //=============================================================================
        /// <summary>
        /// Method to get number format
        /// </summary>
        /// <param name="strSend">Send message</param>
        /// <param name="strFormat">Format</param>
        /// <returns>Format text</returns>
        private string NumberFormat(string strSend, string strFormat)
        {
            string returnValue;

            if (Information.IsNumeric(strSend) && strFormat.IndexOf("v") + 1 != 0)
            {
                var dblValue = Conversion.Val(strSend) * Math.Pow(10, (strFormat.Substring(strFormat.IndexOf("v") + 2 - 1).Length));
                returnValue = Strings.Right(new string('0', strFormat.Length - 1) + System.Convert.ToString(dblValue), strFormat.Length - 1);
            }
            else
            {
                returnValue = Strings.Right(new string('0', strFormat.Length) + strSend, strFormat.Length);
            }

            return returnValue;
        }

        /// <summary>
        /// Method to set prompt code
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="promptCode">Prompt code</param>
        private void SetPromptCode(ref Credit_Card creditCard, string promptCode)
        {
            // Nicolette added to reevaluate ask questions properties when PromptCode is set
            creditCard.PromptCode = promptCode;
            if (creditCard.CardType != null)
            {
                var cardType = _cardService.GetCardPromptByEntryMode(creditCard.CardType.CardID, creditCard.entryMethod, creditCard.PromptCode);
                if (cardType != null)
                {
                    creditCard.CardType.AskDriverNo = cardType.AskDriverNo;
                    creditCard.CardType.AskIdentificationNo = cardType.AskIdentificationNo;
                    creditCard.CardType.AskOdometer = cardType.AskOdometer;
                    creditCard.CardType.AskVehicle = cardType.AskVehicle;
                }
                else
                {
                    creditCard.CardType.AskDriverNo = false;
                    creditCard.CardType.AskIdentificationNo = false;
                    creditCard.CardType.AskOdometer = false;
                    creditCard.CardType.AskVehicle = false;
                }
                creditCard.AskProdRestrictCode = creditCard.entryMethod == "M" && creditCard.CardType.UserEnteredRestriction;
            }
            // Nicolette end
        }

        /// <summary>
        /// Method to get card type
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Card</returns>
        private Card Get_CardType(ref Credit_Card creditCard)
        {
            Card returnValue;
            Card cd;
            var cardTypes = Load_CardTypes();

            if (_policyManager.Version != "US")
            {
                // If the card was swiped then check for debit card.
                if (creditCard.Card_Swiped)
                {
                    foreach (Card tempLoopVarCd in cardTypes)
                    {
                        cd = tempLoopVarCd;

                        if (cd.CardType != "D") continue;
                        if (creditCard.Service_Code == "798" ||
                            creditCard.Service_Code == "799" ||
                            creditCard.Service_Code == "220" ||
                            creditCard.Service_Code == "120")
                        {
                            returnValue = cd;
                            creditCard.TendCard = new TenderCard();
                            creditCard.TendCard = _cardService.LoadTenderCard(cd.CardID);
                            CardPrompts cardPrompt = new CardPrompts();
                            _cardPromptManager.Load_Prompts(ref cardPrompt, creditCard, creditCard.TendCard.OptDataProfileID);
                            creditCard.TendCard.CardPrompts = cardPrompt;
                            return returnValue;
                        }
                        break;
                    }
                }

            }

            // Wasn't a Debit Card. Check other card types.
            foreach (Card tempLoopVarCd in cardTypes)
            {
                cd = tempLoopVarCd;
                foreach (CardCode tempLoopVarCc in cd.CardCodes)
                {
                    var cc = tempLoopVarCc;
                    var ll = (short)cc.LowerLimit.Length;

                    if (cd.CardType != "D" || _policyManager.Version == "US")
                    {
                        //  added the extra checking of card length. if the cards belongs to same range but length is different it is picking up wrong card
                        if (creditCard.Cardnumber.Length < ll ||
                            !(double.Parse(creditCard.Cardnumber.Substring(0, ll)) >= double.Parse(cc.LowerLimit)) ||
                            !(double.Parse(creditCard.Cardnumber.Substring(0, ll)) <= double.Parse(cc.UpperLimit)))
                            continue;
                        if (creditCard.Cardnumber.Length < cd.MinLength || creditCard.Cardnumber.Length > cd.MaxLength)
                            continue;
                        returnValue = cd;
                        creditCard.CardCode = cd.CardID;
                        creditCard.TendCard = new TenderCard();
                        creditCard.TendCard = _cardService.LoadTenderCard(cd.CardID);
                        creditCard.CardPrompts = creditCard.TendCard.CardPrompts; //   to load prompts for fleet cards optional data
                        return returnValue;
                        // 
                    }
                }
            }

            // Didn't find the card. Set the object to nothing which indicates that
            // this is not an accepted card type

            return null;
        }


        /// <summary>
        /// Method to validate card number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="cMask">Card mask</param>
        /// <returns>True or false</returns>
        private bool Valid_Mod10(ref Credit_Card creditCard, string cMask = "")
        {
            bool returnValue;
            short n; //Byte
            char checkDigit = '\0';

            var cardNum = creditCard.Cardnumber;

            // If they didn't supply a mask then build the standard check mask.
            if (cMask.Length < cardNum.Length)
            {
                cMask = "";
                //        For n = 1 To Len(Card_Num) - 1
                for (n = (short)(cardNum.Length - 1); n >= 1; n--)
                {
                    cMask = Convert.ToString(cMask + Convert.ToString(n % 2 == 0 ? "1" : "2"));
                }
                cMask = cMask + "C"; // Last one's the check digit.
            }

            short s = 0;
            for (n = 1; n <= cardNum.Length; n++)
            {
                var c = Convert.ToChar(cardNum.Substring(n - 1, 1));
                var d = Convert.ToChar(cMask.Substring(n - 1, 1));
                if (d == 'C')
                {
                    checkDigit = c;
                }
                else
                {
                    var f = (short)(Conversion.Val(c) * Conversion.Val(d));
                    if (f > 9)
                    {
                        f = (short)(f - 9);
                    }
                    s = (short)(s + f);
                }
            }

            // Determine what the check digit should be.
            creditCard.Check_Digit = s % 10 == 0 ? "0" : Conversion.Str(10 - (s % 10)).Trim();

            // If we are not verifying the check digit then just return
            // that the check digit is valid.
            if (creditCard.VerifyCheckDigit)
            {
                returnValue = (Convert.ToChar(creditCard.Check_Digit) == checkDigit);
            }
            else
            {
                returnValue = true;
            }

            return returnValue;
        }

        #endregion
    }
}

