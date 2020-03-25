using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Globalization;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{

    public class CardManager : ManagerBase, ICardManager
    {
        private readonly ICardService _cardService;
        private readonly IUtilityService _utilityService;
        private readonly IApiResourceManager _resourceManager;
        private readonly IPolicyManager _policyManager;
        private readonly ICreditCardManager _creditCardManager;
        private readonly IEncryptDecryptUtilityManager _encryptDecrpDecryptUtilityManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cardService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="utilityService"></param>
        /// <param name="creditCardManager"></param>
        /// <param name="encryptDecryptUtilityManager"></param>
        public CardManager(ICardService cardService,
            IApiResourceManager resourceManager,
            IPolicyManager policyManager,
            IUtilityService utilityService,
            ICreditCardManager creditCardManager,
            IEncryptDecryptUtilityManager encryptDecryptUtilityManager)
        {
            _cardService = cardService;
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _utilityService = utilityService;
            _creditCardManager = creditCardManager;
            _encryptDecrpDecryptUtilityManager = encryptDecryptUtilityManager;
        }

        //   end

        //   to build optional data string
        /// <summary>
        /// Method ro build optiona data string
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="card">Credit card</param>
        /// <param name="tendCard">Tend card</param>
        public void Build_OptDataString(Sale sale, ref Credit_Card card, ref TenderCard tendCard)
        {
            if (sale.EMVVersion)
            {
                if (string.IsNullOrEmpty(card.OptDataProfileIDEMV))
                {
                    card.OptDataString = "";
                    return;
                }
                tendCard = new TenderCard { OptDataProfileID = card.OptDataProfileIDEMV };
            }
            else
            {
                if (tendCard == null)
                {
                    card.OptDataString = "";
                    return;
                }
                if (string.IsNullOrEmpty(tendCard.OptDataProfileID))
                {
                    card.OptDataString = "";
                    return;
                }
            }

            string strData = "";
            Sale_Line sl = default(Sale_Line);
            var maxIdFuel = _cardService.GetMaxFuelId(tendCard.OptDataProfileID);

            var maxIdNonFuel = _cardService.GetMaxNonFuelId(tendCard.OptDataProfileID);

            var noFuelLines = _cardService.GetTotalOptionalData(tendCard.OptDataProfileID);

            var exemptDataCode = _cardService.GetOptionalDataCode(tendCard.OptDataProfileID);

            var strExemptDept = string.IsNullOrEmpty(exemptDataCode) ? "NOT DEFINED EXEMPTION" : exemptDataCode;

            var noNonFuelLines = _cardService.GetTotalNonFuelOptionalData(tendCard.OptDataProfileID);

            var arrFuelLines = new short[1];
            var arrNonFuelLines = new short[1];
            var arrExemptDeptLines = new short[1];

            foreach (Sale_Line saleLine in sale.Sale_Lines)
            {
                sl = saleLine;
                if (sl.ProductIsFuel)
                {
                    Array.Resize(ref arrFuelLines, arrFuelLines.Length - 1 + 1 + 1);
                    arrFuelLines[arrFuelLines.Length - 1] = sl.Line_Num;
                }
                else if (sl.Dept != strExemptDept)
                {
                    Array.Resize(ref arrNonFuelLines, arrNonFuelLines.Length - 1 + 1 + 1);
                    arrNonFuelLines[arrNonFuelLines.Length - 1] = sl.Line_Num;
                }
                else if (sl.Dept == strExemptDept)
                {
                    Array.Resize(ref arrExemptDeptLines, arrExemptDeptLines.Length - 1 + 1 + 1);
                    arrExemptDeptLines[arrExemptDeptLines.Length - 1] = sl.Line_Num;
                }
            }

            var optionalDatas = _cardService.GetOptionalDatas(tendCard.OptDataProfileID);
            short fuelId = 0;
            short nonFuelId = 0;
            var ubFuel = (short)(arrFuelLines.Length - 1);
            var ubNonFuel = (short)(arrNonFuelLines.Length - 1);

            var maxFuel = (short)modStringPad.MinVal(ubFuel, noFuelLines);
            var maxNonFuel = (short)modStringPad.MinVal(ubNonFuel, noNonFuelLines);

            foreach (var optionalData in optionalDatas)
            {
                var taxCode = optionalData.Code;
                var strValue = "";
                decimal total = 0;
                var id = Convert.ToInt32(optionalData.ID);
                Line_Tax lt;
                switch (id)
                {
                    case 20: // Exemption Dept quantity
                        foreach (Sale_Line saleLine in sale.Sale_Lines)
                        {
                            sl = saleLine;
                            if (sl.Dept == strExemptDept)
                            {
                                total = total + (decimal)sl.Quantity;
                            }
                        }
                        strValue = total.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 22: // Exemption Dept amount
                        foreach (Sale_Line saleLine in sale.Sale_Lines)
                        {
                            sl = saleLine;
                            if (sl.Dept == strExemptDept)
                            {
                                total = total + sl.Amount;
                            }
                        }
                        strValue = total.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 24: //  Total Transaction Amount
                        strValue = sale.Sale_Totals.Gross.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 101: //  Fuel Product Code
                        if (fuelId == 0)
                        {
                            fuelId++;
                        }
                        if (fuelId <= maxFuel)
                        {
                            strValue = _cardService.Get_ProductCode(tendCard.OptDataProfileID, sale.Sale_Lines[arrFuelLines[fuelId]].Stock_Code);
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 108: //  Fuel Price
                        if (fuelId == 0)
                        {
                            fuelId++;
                        }
                        strValue = fuelId <= maxFuel ? sale.Sale_Lines[arrFuelLines[fuelId]].price.ToString(CultureInfo.InvariantCulture) : 0.ToString();
                        break;
                    case 105: //  Fuel Extended Amount
                        if (fuelId == 0)
                        {
                            fuelId++;
                        }
                        strValue = fuelId <= maxFuel ? sale.Sale_Lines[arrFuelLines[fuelId]].Amount.ToString(CultureInfo.InvariantCulture) : 0.ToString();
                        break;
                    case 107: //  Fuel Quantity
                        if (fuelId == 0)
                        {
                            fuelId++;
                        }
                        strValue = fuelId <= maxFuel ? sale.Sale_Lines[arrFuelLines[fuelId]].Quantity.ToString(CultureInfo.InvariantCulture) : 0.ToString();
                        break;
                    case 109: // Fuel GST/HST Amount and Fuel PST Amount (Tax Code is based on Code field in OptData table)
                    case 110:
                        strValue = 0.ToString();
                        if (fuelId == 0)
                        {
                            fuelId++;
                        }
                        if (fuelId <= maxFuel)
                        {
                            foreach (Line_Tax lineTax in sale.Sale_Lines[arrFuelLines[fuelId]].Line_Taxes)
                            {
                                lt = lineTax;
                                if (lt.Tax_Name == taxCode)
                                {
                                    strValue = lt.Tax_Incl_Amount.ToString(CultureInfo.InvariantCulture);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 209: // Accumulated Fuel GST/HST Amount and Accumulated Fuel PST Amount (Tax Code is based on Code field in OptData table)
                    case 210:
                        foreach (Sale_Line saleLine in sale.Sale_Lines)
                        {
                            sl = saleLine;
                            if (sl.ProductIsFuel)
                            {
                                foreach (Line_Tax lineTax in sl.Line_Taxes)
                                {
                                    lt = lineTax;
                                    if (lt.Tax_Code == taxCode)
                                    {
                                        total = total + (decimal)lt.Tax_Incl_Amount;
                                    }
                                }
                            }
                        }
                        strValue = total.ToString(CultureInfo.InvariantCulture);
                        break;
                    case 301: //  Non-Fuel Product Code
                        if (nonFuelId == 0)
                        {
                            nonFuelId++;
                        }
                        if (nonFuelId <= maxNonFuel)
                        {
                            strValue = _cardService.Get_ProductCode(tendCard.OptDataProfileID, sale.Sale_Lines[arrNonFuelLines[nonFuelId]].Stock_Code);
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 307: //  Non-Fuel Quantity
                        if (nonFuelId == 0)
                        {
                            nonFuelId++;
                        }
                        if (nonFuelId <= maxNonFuel)
                        {
                            strValue = sale.Sale_Lines[arrNonFuelLines[nonFuelId]].Quantity.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 305: //  Non-Fuel Extended Amount
                        if (nonFuelId == 0)
                        {
                            nonFuelId++;
                        }
                        if (nonFuelId <= maxNonFuel)
                        {
                            strValue = sale.Sale_Lines[arrNonFuelLines[nonFuelId]].Amount.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 314: //  Non-Fuel Tax Description
                        if (nonFuelId == 0)
                        {
                            nonFuelId++;
                        }
                        if (nonFuelId <= maxNonFuel)
                        {
                            strValue = sale.Sale_Lines[arrNonFuelLines[nonFuelId]].Line_Taxes.Count > 0 ? sale.Sale_Lines[arrNonFuelLines[nonFuelId]].Line_Taxes[1].Tax_Name : 0.ToString();
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 405: // Accumulated non-fuel extended amount
                        total = 0;
                        foreach (Sale_Line saleLine in sale.Sale_Lines)
                        {
                            sl = saleLine;
                            if (!sl.ProductIsFuel)
                            {
                                total = total + sl.Amount;
                            }
                        }
                        strValue = total.ToString(CultureInfo.InvariantCulture);
                        break;

                    case 415: // Accumulated Non-Fuel Tax Amount - includes exempted dept items
                        total = 0;
                        foreach (Sale_Line saleLine in sale.Sale_Lines)
                        {
                            sl = saleLine;
                            if (!sl.ProductIsFuel)
                            {
                                foreach (Line_Tax lineTax in sl.Line_Taxes)
                                {
                                    lt = lineTax;
                                    total = total + (decimal)lt.Tax_Incl_Amount + (decimal)lt.Tax_Added_Amount;
                                }
                            }
                        }
                        strValue = total.ToString(CultureInfo.InvariantCulture);
                        break;

                    case 121: //  Non Fuel Product Code
                        if (nonFuelId == 0)
                        {
                            nonFuelId++;
                        }
                        if (nonFuelId <= maxNonFuel)
                        {
                            strValue = _cardService.Get_ProductCode(tendCard.OptDataProfileID, sale.Sale_Lines[arrNonFuelLines[nonFuelId]].Stock_Code);
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;

                    case 102: //  Fuel Description
                        if (fuelId == 0)
                        {
                            fuelId++;
                        }
                        if (fuelId <= maxFuel)
                        {
                            strValue = sl.Description;
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 103: //  Fuel Service type
                        break;
                    case 2: // Merchant Terminal Prompt Data
                        var promptSeq = _cardService.GetPromptSeq(tendCard.OptDataProfileID);
                        if (promptSeq.HasValue)
                        {
                            strValue = card.CardPrompts[promptSeq.Value].PromptAnswer;
                        }
                        else
                        {
                            strValue = 0.ToString();
                        }
                        break;
                    case 3: // Card Prompt Code
                        promptSeq = _cardService.GetPromptSeq(tendCard.OptDataProfileID);
                        strValue = promptSeq.HasValue ? card.CardPrompts[promptSeq.Value].PromptAnswer : 0.ToString();
                        break;
                    default:
                        strValue = "";
                        break;
                }
                if (Convert.ToInt32(optionalData.ID) == maxIdFuel)
                {
                    fuelId++;
                }
                if (Convert.ToInt32(optionalData.ID) == maxIdNonFuel)
                {
                    nonFuelId++;
                }
                if (Convert.ToInt32(optionalData.ID) == 29)
                {
                    strData = Convert.ToString(strData + new string(Strings.Chr(Convert.ToInt32(string.IsNullOrEmpty(optionalData.Format) ? "0" : optionalData.Format)), optionalData.Length));
                }
                else
                {
                    strData = strData +
                       Convert.ToString(Format_Value(strValue,
                                                      (short)optionalData.Length, (short)optionalData.Decimals,
                                                     string.IsNullOrEmpty(optionalData.Alignment) ? "RIGHT" : optionalData.Alignment));
                }
            }
            card.OptDataString = strData;
            //todo:
            // modStringPad.WriteToLogFile("Optional data string is " + strData)

        }

        /// <summary>
        /// Method to get request string
        /// </summary>
        /// <param name="cc">Credit card</param>
        /// <param name="sale">Sale</param>
        /// <param name="trnType">Transaction type</param>
        /// <param name="cardType">Card type</param>
        /// <param name="amount">Amount</param>
        /// <param name="authCode">Auth code</param>
        /// <returns>Request string</returns>
        public string GetRequestString(ref Credit_Card cc, Sale sale, string trnType, string cardType, float amount,
            string authCode)
        {
            string str = "";
            short pos = 0;
            //string strOptionalData;

            switch (trnType.Trim())
            {
                case "SwipeInside":
                    str = str + trnType.Trim() + "," + cardType.Trim() + ",";
                    str = str + amount.ToString().Trim() + ",,,,,,,,,,,,,,,";
                    break;
                case "EODTerminal": //authcode as terminal ID - changed on 06/19/02 'Sajan
                    str = str + trnType.Trim() + "," + cardType.Trim() + ",1,,,,," + authCode.Trim() + ",,,,,,,,,,,,Y,,,,,,,,,,,,,,";
                    break;
                case "CloseBatchInside":
                    if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "GLOBAL")
                    {
                        if (cardType == "NoDebit") //Added by Mina - For no trans debit
                        {
                            str = str + trnType.Trim() + "," + cardType.Trim() + ",1,,,,,,,,,,,,,,,,,Y,,,,,,,,,,,,,,";
                        }
                        else
                        {
                            str = str + trnType.Trim() + ",,1,,,,,,,,,,,,,,,,,Y,,,,,,,,,,,,,,";
                        }
                    }
                    else
                    {
                        str = str + trnType.Trim() + ",,1,,,,,,,,,,,,,,,,,Y,,,,,,,,,,,,,,";
                    }
                    break;
                case "InitDebitInside":
                    str = str + trnType.Trim() + "," + cardType.Trim() + ",,,,,,,,,,,,,,,,,";
                    break;
                default:
                    if (trnType == "VoidInside")
                    {
                        str = trnType.Trim() + "," + cardType.Trim() + ",1,";
                    }
                    else
                    {
                        str = trnType.Trim() + "," + cardType.Trim() + ",1,";
                    }
                    str = str + Convert.ToString(sale.Sale_Num) + ",,,";
                    if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "GLOBAL")
                    {
                        if (trnType == "VoidInside" || trnType.ToUpper() == "SAFVOIDINSIDE") //Added by Mina : UCase(TrnType) = "SAFVOIDINSIDE"
                        {
                            str = str + Convert.ToString(sale.Void_Num) + ",,"; //void refnum
                        }
                        else
                        {
                            str = str + ",,"; //void refnum
                        }
                    }
                    else
                    {
                        // 
                        if (trnType == "VoidInside")
                        {
                            str = str + Convert.ToString(sale.Void_Num) + ",,"; //void refnum
                        }
                        else
                        {
                            str = str + ",,"; //void refnum
                        }
                    }
                    if (_policyManager.Version == "US") //Behriooz Oct25-05
                    {
                        str = str + amount.ToString("0.00").Trim() + ","; //    CStr(Int(Abs(Amount * 100)))) & ","
                    }
                    else
                    {
                        str = str + (double.Parse(amount.ToString("0.00")) * 100).ToString().Trim() + ","; //    CStr(Int(Abs(Amount * 100)))) & ","
                    }
                    str = str + _creditCardManager.Language(ref cc).Substring(0, 1) + ",,";
                    //the following lines were added by Sajan
                    if (!string.IsNullOrEmpty(cc.Track2))
                    {
                        pos = (short)(cc.Track2.IndexOf("?") + 1);
                    }
                    var track2 = "";
                    if (pos != 0)
                    {
                        track2 = cc.Track2.Substring(0, pos - 1);
                    }
                    else
                    {
                        track2 = cc.Track2 ?? "";
                    }
                    pos = (short)(track2.IndexOf(";") + 1);
                    if (pos != 0)
                    {
                        track2 = track2.Substring(pos + 1 - 1);
                    }
                    if (string.IsNullOrEmpty(cc.Track2))
                    {
                        track2 = (string.IsNullOrEmpty(cc.Cardnumber) ? string.Empty : cc.Cardnumber.Trim()) + "=" +
                            (string.IsNullOrEmpty(cc.Expiry_Date) ? string.Empty : cc.Expiry_Date.Trim());
                    }
                    str = str + track2 + ",,,";
                    if (!string.IsNullOrEmpty(authCode))
                    {
                        if (_policyManager.EMVVersion)
                        {
                            str = str + authCode + ",,,,,";
                        }
                        else
                        {
                            //shiny end - EMVVERSION
                            str = str + authCode + ",,,,";
                        }
                    }
                    else
                    {
                        str = str + ",,,,,Y";
                    }

                    if (cc.Crd_Type == "F")
                    {
                        str = str + ",";
                        str = str + _creditCardManager.ProductCodes(sale.TillNumber) + ",,";
                        str = str + cc.Vechicle_Number + ",";
                        str = Convert.ToString((string.IsNullOrEmpty(cc.ID_Number) ? str + cc.Driver_Number : str + cc.ID_Number) + ",");
                        str = str + cc.Odometer_Number + ",,,";
                        str = str + cc.TerminalType + ",,"; // Terminal Type

                        string temp_Policy_Name = "USE_PINPAD";
                        str = str + Convert.ToString(_policyManager.GetPol(temp_Policy_Name, null)) + ",";

                        if (_policyManager.USE_PINPAD)
                        {

                            str = str + Convert.ToString(cc.AskDriverNo) + ",";
                            str = str + Convert.ToString(cc.AskIdentificationNo) + ",";
                            str = str + Convert.ToString(cc.AskOdometer) + ",";
                            str = str + Convert.ToString(cc.AskVechicle) + ",";
                            str = str + Convert.ToString(cc.AskProdRestrictCode) + ",";
                        }
                        else
                        {
                            str = str + "False,False,False,False,False,";
                        }
                        str = str + cc.UsageCode + ","; // Nicolette, Jan 27 added a extra comma to match the number of commas send to STPS
                    }
                    else
                    {
                        str = str + new string(',', 16);
                    }
                    // Nicolette end
                    //Shiny Nov9, 2009'EMVVERSION
                    if (_policyManager.EMVVersion) //- only needed for credit
                    {
                        //                    str = str & IIf(cc.ManualCardProcess, "M", "") & ","
                        str = str + Convert.ToString(cc.ManualCardProcess ? "M85" : "") + ","; //  - added the POs trans timeout 85  to passing to TPS with Farhad's instruction
                    }
                    //shiny end
                    //   to assign optional data profile for any debit or credit card selected
                    // for EMV version, TPS will drop th unnecessary optional data string sent by POS
                    // debit cards are included because the cashier might select debit but the customer inserts a credit card
                    if (cc.Crd_Type == "F" || cc.Crd_Type == "C" || cc.Crd_Type == "D") //   added Or cc.Crd_Type = "C" for Crevier Visa
                    {
                        TenderCard tenderCard = _cardService.LoadTenderCard(cc.CardCode);
                        Build_OptDataString(sale, ref cc, ref tenderCard); //   optional data for fleet cards
                        str = str + "," + cc.OptDataString;
                    }
                    else
                    {
                        str = str + ",";
                    }
                    break;
            }
            var returnValue = str;

            return returnValue;
        }

        /// <summary>
        /// Method to load card profil by profile Id
        /// </summary>
        /// <param name="profileId">Profile Id</param>
        /// <returns>Card profile</returns>
        public CardProfile Loadprofile(string profileId)
        {
            var cardProfile = _cardService.GetCardRestrProfiles(profileId);
            if (cardProfile == null) return null;
            //adding the prompts for this profile
            cardProfile.CardPrompts = new CardPrompts();
            var cardPrompts = _cardService.GetCardPrompts(profileId);
            foreach (var cardPrompt in cardPrompts)
            {
                cardProfile.CardPrompts.Add(cardPrompt.MaxLength, cardPrompt.MinLength, cardPrompt.PromptMessage,
                    cardPrompt.PromptSeq, cardPrompt.PromptID, "", "");
            }
            return cardProfile;
        }

        /// <summary>
        /// Method to check whether valid profile time limit exists or not
        /// </summary>
        /// <param name="cardProfile">Card profile</param>
        /// <returns>True or false</returns>
        public bool ValidProfileTimeLimit(ref CardProfile cardProfile) // checking whether purchase is in a valid time period for the card
        {
            var sTime = DateTime.Now;
            var sdate = DateTime.Now;
            var dayOfWeek = (byte)DateAndTime.Weekday(sdate);
            //sTime = DateTime.Parse(Sdate.ToString("hh:mm:ss"));
            var timeLimit = _cardService.GetCardProfileTimeLimit(cardProfile.ProfileID, dayOfWeek);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (timeLimit == null) return false;
            if (timeLimit.AllowPurchase == false)
            {
                cardProfile.Reason = _resourceManager.GetResString(offSet, 466) + " " + DateAndTime.WeekdayName(dayOfWeek, FirstDayOfWeekValue: FirstDayOfWeek.Sunday); //" Purchase is not allowed  with this card on "
            }
            else
            {
                if (!timeLimit.TimeRestriction) return true;

                if (sTime.TimeOfDay >= timeLimit.StartTime.TimeOfDay &&
                    sTime.TimeOfDay <= timeLimit.EndTime.TimeOfDay) return true;
                //                    mvarreason = " Purchase is allowed  with this card on " & WeekdayName(DayOfWeek, , vbSunday) & " only between " & !starttime & " and " & !EndTime
                cardProfile.Reason = _resourceManager.GetResString(offSet, 467) + " "
                                     + DateAndTime.WeekdayName(dayOfWeek, FirstDayOfWeekValue: FirstDayOfWeek.Sunday)
                                     + " " + _resourceManager.GetResString(offSet, 468) + " "
                                     + timeLimit.StartTime.ToString("hh:mm tt") + " "
                                     + _resourceManager.GetResString(offSet, 469) + " "
                                     + timeLimit.EndTime.ToString("hh:mm tt");
            }

            return false;
        }

        /// <summary>
        /// Method to check whether valid products are present for profile or not
        /// </summary>
        /// <param name="cardProfile">Card profile</param>
        /// <param name="cSale"></param>
        /// <returns>True or false</returns>
        public bool ValidProductsForProfile(ref CardProfile cardProfile, Sale cSale) // checking whether this sales product is valid based on profile product restriction setting
        {
            bool returnValue;

            Sale_Line sl;
            //    ValidProductsForProfile = False
            cardProfile.PromptForFuel = false; // '  -Make this true only if there is fuel products and using this card for purchase '
                                               //    mvarReason = GetResString(484) 'GetResString(1441) '" This card can't be used for buying selected products."
            cardProfile.RestrictedUse = false;
            double validAmount = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var all = _resourceManager.GetResString(offSet, 347);
            var fuelDept = _utilityService.GetFuelDepartmentId();
            if (cardProfile.RestrictProducts) // Do not allow product specified in the restriction (exclude them)
            {
                foreach (Sale_Line tempLoopVarSl in cSale.Sale_Lines)
                {
                    sl = tempLoopVarSl;
                    if (!string.IsNullOrEmpty(sl.CardProfileID) && sl.CardProfileID != cardProfile.ProfileID)
                        continue;
                    if (!_cardService.IsCardProductRestriction(cardProfile.ProfileID, all, sl)) // no restriction products , since it is not specified
                    {
                        //  - ask fuel prompt question only if there is fuel products in the purchase
                        if (sl.Dept == fuelDept)
                        {
                            if (cardProfile.PromptForFuel == false)
                            {
                                cardProfile.PromptForFuel = true;
                            }
                        }
                        //shiny end
                        validAmount = validAmount + (double)sl.Amount - sl.Line_Discount - sl.Discount_Adjust + (double)sl.AddedTax + (double)sl.TotalCharge;
                        sl.CardProfileID = cardProfile.ProfileID;
                    }
                    else
                    {
                        //these are the restricted product in the sale
                        cardProfile.RestrictedUse = true; //  
                    }
                }
            }
            else if (cardProfile.RestrictProducts == false) // Allow\Include only products specified in the restricyion
            {
                foreach (Sale_Line tempLoopVarSl in cSale.Sale_Lines)
                {
                    sl = tempLoopVarSl;

                    if (string.IsNullOrEmpty(sl.CardProfileID) || (sl.CardProfileID == cardProfile.ProfileID)) //  Sometimes sl.profileid is set, but not paid(e.g Crash recovery)0
                    {
                        // rs = _dbService.GetRecords("SELECT * FROM CardProductRestriction " + " Where ProfileID =\'" +cardProfile.ProfileID + "\' " + " and ((dept = \'" + _resourceManager.GetResString(offSet,(short)347) + "\' and subdept =\'" + _resourceManager.GetResString(offSet,(short)347) + "\' and subdetail = \'" + _resourceManager.GetResString(offSet,(short)347) + "\' and stockcode = \'" + SL.Stock_Code + "\') or " + " ( dept = \'" + SL.Dept + "\'  and subdept = \'" + SL.Sub_Dept + "\' and subdetail = \'" + SL.Sub_Detail + "\') or " + " ( dept = \'" + SL.Dept + "\'  and subdept = \'" + SL.Sub_Dept + "\' and subdetail = \'" + _resourceManager.GetResString(offSet,(short)347) + "\') or " + " ( dept = \'" + SL.Dept + "\'  and subdept = \'" + _resourceManager.GetResString(offSet,(short)347) + "\' and subdetail = \'" + _resourceManager.GetResString(offSet,(short)347) + "\'))", DataSource.CSCMaster);
                        if (_cardService.IsCardProductRestriction(cardProfile.ProfileID, all, sl)) // allow products , since it is specified
                        {
                            //  - ask fuel prompt question only if there is fuel products in the purchase
                            if (sl.Dept == _utilityService.GetFuelDepartmentId())
                            {
                                if (cardProfile.PromptForFuel == false)
                                {
                                    cardProfile.PromptForFuel = true;
                                }
                            }
                            //shiny end

                            validAmount = validAmount + (double)sl.Amount - sl.Line_Discount - sl.Discount_Adjust + (double)sl.AddedTax + (double)sl.TotalCharge;
                            sl.CardProfileID = cardProfile.ProfileID;
                        }
                        else
                        {
                            cardProfile.RestrictedUse = true; //   'these are the restricted product in the sale
                        }
                    }
                }
            }


            if (validAmount != 0)
            {
                returnValue = true;
                cardProfile.Reason = "";
            }
            else
            {
                cardProfile.Reason = _resourceManager.GetResString(offSet, 484);
                returnValue = false;
            }
            cardProfile.PurchaseAmount = cardProfile.PurchaseAmount + validAmount;
            return returnValue;
        }

        /// <summary>
        /// Method to find if valid transaction limits exists for profile or not
        /// </summary>
        /// <param name="cardProfile">Card profile</param>
        /// <param name="cardNumber">Card number</param>
        /// <returns>True or false</returns>
        public bool ValidTransactionLimits(ref CardProfile cardProfile, string cardNumber)
        {
            //1- check single transaction limit . If above than limit adjust the amount only for single transaction and ask the question

            // 2- check daily transaction number limit ( 0 means unlimited)
            //3- check daily transaction amount limit( 0 means unlimited)If above than limit adjust the amount only for single transaction and ask the question
            //4 - check monthly transaction amount limit( 0 means unlimited)If above than limit adjust the amount only for single transaction and ask the question
            //   allow partial purchase, if one of the single, daily or monthly criteria fails not allowing purchase with the card - ask quetion and if continue use the balane, no not use the card .

            decimal balance;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var returnValue = true;
            cardProfile.PartialUse = false; // 
            if (cardProfile.SngleTransaction != 0)
            {
                //shiny jun2,2010
                if (Math.Abs(cardProfile.PurchaseAmount) > cardProfile.SngleTransaction)
                {
                    balance = (decimal)cardProfile.SngleTransaction;
                    cardProfile.PartialUse = true;
                    cardProfile.Reason = _resourceManager.GetResString(offSet, 480) + Convert.ToString(cardProfile.SngleTransaction); //& GetResString(481) & Balance & vbCrLf
                }
                else
                {
                    balance = (decimal)cardProfile.PurchaseAmount;
                    cardProfile.Reason = "";
                }

                //        If Abs(Me.PurchaseAmount) > Me.SngleTransaction Then 'added the abs value for refund we shouldn't allow more than single transaction limit
                if (cardProfile.PurchaseAmount < 0)
                {
                    cardProfile.PurchaseAmount = (double)(-1 * balance); //Me.SngleTransaction
                }
                else
                {
                    cardProfile.PurchaseAmount = (double)balance; //Me.SngleTransaction
                }
                //        End If
                // 
            }
            if (!(cardProfile.PurchaseAmount > 0)) return true;
            if (cardProfile.TransactionsPerDay != 0)
            {
                if (cardProfile.TransactionsPerDay <= DailyTransCnt(cardNumber))
                {
                    returnValue = false; // failed '
                    cardProfile.Reason = _resourceManager.GetResString(offSet, 463) + " " + Convert.ToString(cardProfile.TransactionsPerDay); // Maximum card transactions per day is
                }
            }
            decimal used;
            if (returnValue) // not yet failed' passed daily cnt limit
            {
                if (cardProfile.DailyTransaction != 0 & cardProfile.PurchaseAmount > 0) //checking daily amount limit only for sales
                {
                    //SHINY JUNE2,210
                    used = System.Convert.ToDecimal(TransAmountLimit(cardNumber, 1));
                    balance = (decimal)(cardProfile.DailyTransaction - (double)used);
                    if (balance <= 0) // no balance
                    {
                        //                If Me.DailyTransaction < TransAmountLimit(cardnumber, 1) + Me.PurchaseAmount Then
                        returnValue = false; // failed
                        cardProfile.Reason = _resourceManager.GetResString(offSet, 464) + " " + Convert.ToString(cardProfile.DailyTransaction);
                    }
                    else
                    {
                        if (cardProfile.PurchaseAmount > (double)balance)
                        {
                            cardProfile.PurchaseAmount = (double)balance;
                            cardProfile.PartialUse = true;
                            cardProfile.Reason = _resourceManager.GetResString(offSet, 478) + Convert.ToString(cardProfile.DailyTransaction) + _resourceManager.GetResString(offSet, (short)481) + System.Convert.ToString(balance);

                            //                    Else
                            //                        Me.PurchaseAmount = Me.PurchaseAmount
                        }
                        // 
                    }
                }
            }
            if (returnValue) // not yet failed' passed daily cnt limit and daily trans limit
            {
                if (cardProfile.MonthlyTransaction != 0 & cardProfile.PurchaseAmount > 0) //checking monthly amount limit
                {
                    //SHINY JUNE3,210
                    used = System.Convert.ToDecimal(TransAmountLimit(cardNumber, 2));
                    balance = (decimal)(cardProfile.MonthlyTransaction - (double)used);
                    if (balance <= 0) // no balance to purchase
                    {
                        //                If Me.MonthlyTransaction < TransAmountLimit(cardnumber, 2) + Me.PurchaseAmount Then
                        returnValue = false; // failed
                        cardProfile.Reason = _resourceManager.GetResString(offSet, 465) + " " + Convert.ToString(cardProfile.MonthlyTransaction); //"Maximum card transaction amount per month is "
                    }
                    else
                    {
                        if (!(cardProfile.PurchaseAmount > (double)balance)) return true;
                        cardProfile.PurchaseAmount = (double)balance;
                        cardProfile.PartialUse = true;
                        cardProfile.Reason = _resourceManager.GetResString(offSet, 479) + Convert.ToString(cardProfile.MonthlyTransaction) + _resourceManager.GetResString(offSet, (short)481) + System.Convert.ToString(balance);

                        //                    Else
                        //                        Me.PurchaseAmount = Me.PurchaseAmount
                        // 
                    }
                }
            }
            return returnValue;
        }

        //includes sales and returns
        /// <summary>
        /// Method to get transaction amount limit
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="mode">Card number</param>
        /// <returns>Transaction amount</returns>
        public double TransAmountLimit(string cardNumber, byte mode)
        {
            string criteria = "";
            string encryptcardnumber = Convert.ToString(_encryptDecrpDecryptUtilityManager.Encrypt(cardNumber, ""));

            switch (mode)
            {
                case 1:
                    criteria = " and sale_date = \'" + DateAndTime.Today.ToString("yyyyMMdd") + "\'";
                    break;
                case 2:
                    criteria = " and  year(sale_date) = \'" + Convert.ToString(DateAndTime.Year(DateAndTime.Today)) + "\' and  month(sale_date) = \'" + Convert.ToString(DateAndTime.Month(DateAndTime.Today)) + "\'";
                    break;
            }

            double returnValue = 0;
            returnValue = returnValue + _cardService.GetAmountTendered(cardNumber, encryptcardnumber, criteria, DataSource.CSCTills);
            returnValue = returnValue + _cardService.GetAmountTendered(cardNumber, encryptcardnumber, criteria, DataSource.CSCTrans);

            return returnValue;
        }

        #region Private methods

        //   to build optional data string by value
        /// <summary>
        /// Method to format value
        /// </summary>
        /// <param name="parValue">Parse vale</param>
        /// <param name="length">Length</param>
        /// <param name="decimals">Decimals</param>
        /// <param name="aligned">Alignment</param>
        /// <returns>Format</returns>
        private string Format_Value(string parValue, short length, short decimals,
            string aligned)
        {
            string retString = "";
            string sign = "";
            double value;
            if (double.TryParse(parValue, out value))
            {
                parValue = Math.Abs(value).ToString(CultureInfo.InvariantCulture);
            }
            if (decimals != 0 && double.TryParse(parValue, out value))
            {
                parValue = (value * Convert.ToDouble("1" + new string('0', decimals))).ToString(CultureInfo.InvariantCulture);
            }
            var replChar = double.TryParse(parValue, out value) ? "0" : " ";
            var formatString = new string('0', length);
            if (aligned.Substring(0, 1) == "R")
            {
                retString = sign.Trim() + value.ToString(formatString).Substring(0, length);
            }
            else if (aligned == "L")
            {
                retString = sign.Trim() + string.Format(parValue, "!" + new string('@', length)).Replace(" ", replChar).Substring(string.Format(parValue, "!" + new string('@', length)).Replace(" ", replChar).Length - length, length);
            }

            return retString;
        }

        // Considering both sales & return transactions- eg 3 sales and 1 return is co nsidering as 2 counts ( 3-1 = 2)
        /// <summary>
        /// Method to get daily transaction count
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Daily count</returns>
        private short DailyTransCnt(string cardNumber)
        {
            short returnValue;
            // Transaction number per day 'assumption customer card won't be encrypted

            string encryptcardnumber = Convert.ToString(_encryptDecrpDecryptUtilityManager.Encrypt(cardNumber, "")); //  - to support both encypted card and non encrypted card

            var saleCnt = (short)0;
            var returnCnt = (short)0;

            saleCnt = Convert.ToInt16(saleCnt + _cardService.GetSalesCount(cardNumber, encryptcardnumber, DataSource.CSCTills));

            saleCnt = Convert.ToInt16(saleCnt + _cardService.GetSalesCount(cardNumber, encryptcardnumber, DataSource.CSCTrans));

            //Returncnt
            returnCnt = Convert.ToInt16(returnCnt + _cardService.GetReturnsCount(cardNumber, encryptcardnumber, DataSource.CSCTills));
            returnCnt = Convert.ToInt16(returnCnt + _cardService.GetReturnsCount(cardNumber, encryptcardnumber, DataSource.CSCTrans));
            if (returnCnt <= saleCnt)
            {
                returnValue = (short)(saleCnt - returnCnt);
            }
            else
            {
                returnValue = 0; // if more return we need to consider it as zero cnt
            }
            return returnValue;
        }

        #endregion

    }
}

