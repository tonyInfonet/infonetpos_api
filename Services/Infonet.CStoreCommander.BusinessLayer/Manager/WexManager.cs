using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.Compatibility.VB6;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// manager to perform the actions related to the WEX FLEET Transaction
    /// </summary>
    public class WexManager: ManagerBase, IWexManager
    {
        private readonly IWexService _wexService;
        private readonly IApiResourceManager _resourceManager;
        private readonly IPolicyManager _policyManager;
        private readonly IEncryptDecryptUtilityManager _encryptDecryptManager;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="wexService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="encryptDecryptManager"></param>
        public WexManager(IWexService wexService,IApiResourceManager resourceManager,IPolicyManager policyManager, IEncryptDecryptUtilityManager encryptDecryptManager)
        {
            _wexService = wexService;
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _encryptDecryptManager = encryptDecryptManager;
        }

        /// <summary>
        /// Method to get the XML request string to be sent to the server
        /// </summary>
        /// <param name="saleLineCount"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string GetWexString(int saleLineCount, float amount)
        {
            var index = 0;
            var tempRequestString = "";
            var xmlRequestString = "";
            var stockCodeArr = new string[saleLineCount];
            double discountTotal = 0;
            float taxHST = 0;
            float taxGST = 0;
            float taxPST = 0;
            float taxOther = 0;
           
            foreach(Sale_Line saleLine in Chaps_Main.SA.Sale_Lines)
            {
               stockCodeArr[index] =  "\'" + saleLine.Stock_Code + "\'";
               index++;
            }
            
            var profileId = _wexService.GetWexProfileId();
            if (string.IsNullOrEmpty(profileId))
            {
                profileId = "0";
            }
            var saleString = "SALE";
            tempRequestString = "\r\n" + "<Products> " + "\r\n";
            index = 0;
            foreach (Sale_Line saleLine in Chaps_Main.SA.Sale_Lines)
            {
                if (saleLine.Net_Amount < 0)
                {
                    saleString = "RETURN";
                }
                
                discountTotal = discountTotal + saleLine.Line_Discount;

                tempRequestString = tempRequestString + "<Product Quantity=\"" + string.Format("{0:0.00000}", saleLine.Quantity) + "\"";
                tempRequestString = tempRequestString + " UnitPrice=\"" + string.Format("{0:0.000}", saleLine.price) + "\"";
                tempRequestString = tempRequestString + " Amount=\"" + string.Format("{0:0.000}", (saleLine.Net_Amount + (decimal)saleLine.Line_Discount)) + "\"";

                var tempString = "00";
                var extractionCodeArray = _wexService.GetExtractionCode(stockCodeArr[index], profileId);
                index++;
                if (saleLine.Stock_Code == extractionCodeArray[1])
                {
                    tempString = extractionCodeArray[0];
                }
                tempRequestString = tempRequestString + " ProductCode=\"" + tempString + "\" />" + "\r\n";
            }

            if (discountTotal != 0)
            {
                tempRequestString = tempRequestString + "<Product Quantity=\"1.00000\" ";
                tempRequestString = tempRequestString + " UnitPrice=\"" + String.Format("{0:0.000}", discountTotal) + "\" ";
                tempRequestString = tempRequestString + " Amount=\"" + String.Format("{0:0.00}", discountTotal) + "\" ";
                tempRequestString = tempRequestString + " ProductCode=\"900\" />" + "\r\n";
            }

            float i = 0;
            string tax;
            for (i = 1; i <= Chaps_Main.SA.Sale_Totals.Sale_Taxes.Count; i++)
            {
                if (Chaps_Main.SA.Sale_Totals.Sale_Taxes[i].Tax_Added_Amount != 0)
                {
                    tax = Strings.UCase(Chaps_Main.SA.Sale_Totals.Sale_Taxes[i].Tax_Name).Trim();
                    if (tax == "HST")
                    {
                        taxHST = taxHST + (float)(Chaps_Main.SA.Sale_Totals.Sale_Taxes[i].Tax_Added_Amount);
                    }
                    else if (tax == "GST" || tax == "TPS")
                    {
                       taxGST = taxGST + (float)(Chaps_Main.SA.Sale_Totals.Sale_Taxes[i].Tax_Added_Amount);
                    }
                    else if (tax == "PST" || tax == "TVQ" || tax == "QST" || tax == "RST")
                    {
                      taxPST = taxPST + (float)(Chaps_Main.SA.Sale_Totals.Sale_Taxes[i].Tax_Added_Amount);
                    }
                    else
                    {
                        taxOther = taxOther + (float)(Chaps_Main.SA.Sale_Totals.Sale_Taxes[i].Tax_Added_Amount);
                    }
                }
            }

            if (taxHST != 0)
            {
                tempRequestString = tempRequestString + "<Product Quantity=\"1.00000\" ";
                tempRequestString = tempRequestString + " UnitPrice=\"" + String.Format("{0:0.00}", taxHST) + "\" ";
                tempRequestString = tempRequestString + " Amount=\"" + String.Format("{0:0.00}", taxHST) + "\" ";
                tempRequestString = tempRequestString + " ProductCode=\"965\" />" + "\r\n";
            }

            if (taxGST != 0)
            {
                tempRequestString = tempRequestString + "<Product Quantity=\"1.00000\" ";
                tempRequestString = tempRequestString + " UnitPrice=\"" + String.Format("{0:0.00}", taxGST) + "\" ";
                tempRequestString = tempRequestString + " Amount=\"" + String.Format("{0:0.00}", taxGST) + "\" ";
                tempRequestString = tempRequestString + " ProductCode=\"963\" />" + "\r\n";
            }

            if (taxPST != 0)
            {
                tempRequestString = tempRequestString + "<Product Quantity=\"1.00000\" ";
                tempRequestString = tempRequestString + " UnitPrice=\"" + String.Format("{0:0.00}", taxPST) + "\" ";
                tempRequestString = tempRequestString + " Amount=\"" + String.Format("{0:0.00}", taxPST) + "\" ";
                tempRequestString = tempRequestString + " ProductCode=\"964\" />" + "\r\n";
            }

            if (taxOther != 0)
            {
                tempRequestString = tempRequestString + "<Product Quantity=\"1.00000\" ";
                tempRequestString = tempRequestString + " UnitPrice=\"" + String.Format("{0:0.00}", taxOther) + "\" ";
                tempRequestString = tempRequestString + " Amount=\"" + String.Format("{0:0.00}", taxOther) + "\" ";
                tempRequestString = tempRequestString + " ProductCode=\"950\" />" + "\r\n";
            }

            // Nicolette, Oct 17, 2014 added total charges
            if (Chaps_Main.SA.Sale_Totals.Charge != 0)
            {
                tempRequestString = tempRequestString + "<Product Quantity=\"1.00000\" ";
                tempRequestString = tempRequestString + " UnitPrice=\"" + String.Format("{0:0.00}", Chaps_Main.SA.Sale_Totals.Charge) + "\" ";
                tempRequestString = tempRequestString + " Amount=\"" + String.Format("{0:0.00}", Chaps_Main.SA.Sale_Totals.Charge) + "\" ";
                tempRequestString = tempRequestString + " ProductCode=\"957\" />" + "\r\n";
            }

            tempRequestString = tempRequestString + "</Products>";
            var cardDetail = "";

            if (!modTPS.cc.Card_Swiped)
            {
                if (!string.IsNullOrEmpty(modTPS.cc.Expiry_Date))
                {
                    cardDetail = modTPS.cc.Cardnumber + "=" + modTPS.cc.Expiry_Date;
                }
                else
                {
                    cardDetail = modTPS.cc.Cardnumber;
                }
            }
            else
            {
                cardDetail = modTPS.cc.Track2.Replace(";", "").Replace("?", "");
            }
            
            xmlRequestString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + "\r\n" + "<WexTPSRequest>" 
                        + "\r\n" + "<Transaction Type=\"" + saleString + "\" />" 
                        + "<Payment Type=\"FLEET\" />" + "\r\n" + "<Source Type=\"POS\" />" 
                        + "<CardType>" + modTPS.cc.Bank_CardID + "</CardType>" + "<Lane>" 
                        + System.Convert.ToString(PosId) + "</Lane>" + "\r\n" 
                        + "<NetAmount>" + String.Format("{0:0.000}", amount)
                        + "</NetAmount>" + "\r\n" + "<InvoiceNo>" + System.Convert.ToString(Chaps_Main.SA.Sale_Num) 
                        + "</InvoiceNo>" + "\r\n" + "<Card>" + cardDetail + "</Card>";
            
            if (!modTPS.cc.Card_Swiped)
            {
                xmlRequestString = xmlRequestString + "\r\n" + "<PDSN>" + modTPS.cc.CardPrompts[1].PromptAnswer + "</PDSN>";
            }
            else
            {
                xmlRequestString = xmlRequestString + "\r\n" + "<PromptData>";
                foreach (CardPrompt cardPrompt in modTPS.cc.CardPrompts)
                {
                    xmlRequestString = xmlRequestString + "\r\n" + "<" + System.Convert.ToString(GetPromptTag((cardPrompt.PromptID).ToString())) 
                        + ">" + cardPrompt.PromptAnswer + "</" 
                        + System.Convert.ToString(GetPromptTag((cardPrompt.PromptID).ToString())) 
                        + ">";
                }
                xmlRequestString = xmlRequestString + "\r\n" + "</PromptData>";
            }

            xmlRequestString = xmlRequestString + tempRequestString; 
            xmlRequestString = xmlRequestString + "\r\n" + "</WexTPSRequest>";
            modTPS.cc.CardPrompts = null;
            return xmlRequestString;
        }
        
        /// <summary>
        /// method to validate the Wex transaction
        /// </summary>
        /// <param name="strLineNum"></param>
        /// <param name="amount"></param>
        /// <param name="msgNum"></param>
        /// <returns></returns>
        public bool ValidWexTransaction(int strLineNum, float amount,ref int msgNum)
        {
            float positiveCount = 0;
            float negativeCount = 0;

            for (var index = 1; index <= strLineNum; index++)
            {
                var saleLine = Chaps_Main.SA.Sale_Lines[index];
                if (saleLine.Amount < 0)
                {
                    negativeCount++;
                }
                else
                {
                    positiveCount++;
                }

                if (negativeCount > 0 && positiveCount > 0)
                {
                    msgNum = 8964;
                    return false;
                }
            }

            // uncomment if validation needed for the number of digis of the promptanswer
            //if (!modTPS.cc.Card_Swiped)
            //{
            //    if (modTPS.cc.CardPrompts != null)
            //    {
            //        if (Strings.Len(modTPS.cc.CardPrompts[1].PromptAnswer) != 5)
            //        {
            //            msgNum = 8966;
            //            return false;
            //        }
            //    }
            //}

            if (Math.Abs(amount) != Math.Abs((float)Chaps_Main.SA.Sale_Totals.Gross))
            {
                msgNum = 8965;
                return false;
            }
            return true;
        }

        /// <summary>
        /// method to analyse the response from the WEX server 
        /// </summary>
        /// <param name="receivedStr"></param>
        /// <param name="cc"></param>
        public void AnalyseWexResponse(string receivedStr, ref Credit_Card cc)
        {
            if (GetTagValue(receivedStr, "Transaction", "Type").ToUpper() == "CLOSEBATCH")
            {
                cc.Response = GetTagValue(receivedStr, "Response", "Type").ToUpper();
                cc.Report = GetTagValue(receivedStr, "Report");
                if (cc.Report != "")
                {
                    cc.Report = cc.Report.Replace("<![CDATA[", "");
                }
                if (cc.Report != "")
                {
                    cc.Report = cc.Report.Replace("]]>", "");
                }
                cc.Sequence_Number = GetTagValue(receivedStr, "BATCHNUMBER");
                if (cc.Response.ToUpper() == "BATCH SETTLED")
                {
                    cc.Response = "APPROVED";
                }
            }
            else
            {
                cc.Response = GetTagValue(receivedStr, "Response", "Type").ToUpper();
                if (GetTagValue(receivedStr, "Response", "Type").ToUpper() == "APPROVED")
                {
                    if (string.Format("{0:0.000}", cc.Trans_Amount) == GetTagValue(receivedStr, "Amount"))
                    {
                        if (Chaps_Main.SA.Sale_Num == double.Parse(GetTagValue(receivedStr, "invoiceno"))) 
                        {
                            cc.Sequence_Number = GetTagValue(receivedStr, "PDSN"); 
                            cc.Response = "APPROVED";
                        }
                        else
                        {
                            cc.Response = "INVOICE NOT MATCHING";
                        }
                    }
                    else
                    {
                       cc.Response = "AMOUNT NOT MATCHING";
                    }
                }
            }

            if (cc.Response == "APPROVED")
            {
                var dateTime = System.DateTime.FromOADate(DateAndTime.Today.ToOADate() + DateAndTime.TimeOfDay.ToOADate());

                if (GetTagValue(receivedStr, "TransDate") != "")
                {
                    dateTime = DateTime.Parse(GetTagValue(receivedStr, "TransDate").Trim().Substring(0, 19).ToUpper().Replace("T", " "));
                }
                if (GetTagValue(receivedStr, "Date") != "" && GetTagValue(receivedStr, "Time") != "")
                {
                    dateTime = DateTime.Parse(GetTagValue(receivedStr, "Date") + " " + GetTagValue(receivedStr, "Time"));
                }
                cc.Trans_Date = dateTime.Date;
                cc.Trans_Time = dateTime;
                cc.ApprovalCode = GetTagValue(receivedStr, "ApprovalCode");
                cc.TerminalID = GetTagValue(receivedStr, "TerminalID");
                cc.Receipt_Display = cc.Response;
                cc.Result = "W";
            }
            modTPS.cc = cc;
        }
        
        /// <summary>
        /// method to return the values of the profile prompts
        /// </summary>
        /// <returns></returns>
        public List<string> GetWexProfilePrompts()
        {
            return _wexService.GetWexProfilePrompts();
        }
        
        /// <summary>
        /// method to return the WEX reciept
        /// </summary>
        /// <param name="width"></param>
        /// <param name="sale"></param>
        /// <param name="tenders"></param>
        /// <returns></returns>
        public string GetWexRecieptString(ref short width, Sale sale, ref Tenders tenders)
        {
            
            if(modTPS.cc != null)
            {
               return  CreateWexReciept(width, modTPS.cc.Name, (sale.Sale_Num).ToString(), modTPS.cc.TerminalID, modTPS.cc.Cardnumber, modTPS.cc.Sequence_Number, modTPS.cc.ApprovalCode, tenders);
            }
           
            return  "";
        }

        /// <summary>
        /// method to get the card profile prompts
        /// </summary>
        /// <param name="prompts"></param>
        /// <param name="cc"></param>
        /// <param name="profileId"></param>
        public void GetProfilePrompts(ref CardPrompts prompts, Credit_Card cc,string profileId)
        {
            var promptCodeString = cc.Track2?.Replace(";" + cc.Cardnumber + "=", "").Replace("?", "");
            promptCodeString = promptCodeString?.Substring(4, 1) + promptCodeString?.Substring(promptCodeString.Length-1, 1);
            if (promptCodeString == "00")
            {
                return;
            }
            if (string.IsNullOrEmpty(promptCodeString))
            {
                prompts.Add(20,3,"Enter Card Data ",1,5009,"","");
                return;
            }
            _wexService.GetCardProfilePrompts(ref prompts, promptCodeString, profileId);
        }

        /// <summary>
        /// method to get the WEX close batch request string
        /// </summary>
        /// <returns></returns>
        public string GetWexCloseBatchString()
        {
            var xmlRequest = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + "\r\n";
            xmlRequest = xmlRequest + "<WexTPSRequest>" + "\r\n";
            xmlRequest = xmlRequest + "<Transaction Type=\"CloseBatch\" />" + "\r\n";
            xmlRequest = xmlRequest + "<Payment Type=\"FLEET\" />" + "\r\n";
            xmlRequest = xmlRequest + "<Source Type=\"POS\" />" + "\r\n";
            xmlRequest = xmlRequest + "</WexTPSRequest>";
            return xmlRequest;
        }

        /// <summary>
        /// Method to construct the reciept of wex tranaction
        /// </summary>
        /// <param name="width"></param>
        /// <param name="name"></param>
        /// <param name="saleNumber"></param>
        /// <param name="terminalId"></param>
        /// <param name="cardNumber"></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="approvalCode"></param>
        /// <param name="tenders"></param>
        /// <returns></returns>
        private string CreateWexReciept(short width, string name, string saleNumber, string terminalId, string cardNumber, string sequenceNumber, string approvalCode, Tenders tenders)
        {
            var wexReciept = "";
            var tempStringArray = new string[5];
            wexReciept = "";
            tempStringArray[0] = terminalId;
            tempStringArray[1] = PosId.ToString();
            tempStringArray[2] = System.Convert.ToString(_encryptDecryptManager.Encrypt(cardNumber, "C"));
            tempStringArray[3] = sequenceNumber;
            tempStringArray[4] = approvalCode;
            wexReciept = wexReciept + "\r\n" + modStringPad.PadR(name, width);
            if (Strings.Left(System.Convert.ToString(_policyManager.LoadStoreInfo().Language), 1).ToUpper() == "F")
            {
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Terminal Id: " + tempStringArray[0], width) + "\r\n";
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Acc#: " + tempStringArray[2], width);
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("PD Seq#: " + tempStringArray[3], width);
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Récupération Numéro de référence: " + saleNumber, width);
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Code d\'autorisation: " + tempStringArray[4], width);

                wexReciept = wexReciept + "\r\n" + "\r\n" + "\r\n" + modStringPad.PadR(_resourceManager.GetResString(_policyManager.LoadStoreInfo().OffSet, (short)247) + ":  ", width, "_"); //Signature
                wexReciept = wexReciept + "\r\n" + "    Le Titulaire versera ce montant a  ";
                wexReciept = wexReciept + "\r\n" + "        L\'emetteur conformement au    ";
                wexReciept = wexReciept + "\r\n" + "             contrat adherent  ";
            }
            else
            {
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Terminal Id: " + tempStringArray[0], width) + "\r\n";
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Acc#: " + tempStringArray[2], width);
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("PD Seq#: " + tempStringArray[3], width);
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Retrieval Reference Number: " + saleNumber, width);
                wexReciept = wexReciept + "\r\n" + modStringPad.PadR("Approval Code: " + tempStringArray[4], width);

                wexReciept = wexReciept + "\r\n" + "\r\n" + "\r\n" + modStringPad.PadR(_resourceManager.GetResString(_policyManager.LoadStoreInfo().OffSet, (short)247) + ":  ", 40, "_"); //Signature
                wexReciept = wexReciept + "\r\n" + "CUSTOMER AGREES TO PAY THE ABOVE AMOUNT";
                wexReciept = wexReciept + "\r\n" + " ACCORDING TO THE CARD ISSUER AGREEMENT";
            }
            wexReciept = wexReciept + "\r\n";
            return wexReciept;
        }

        /// <summary>
        /// method to get the value of tags in the XML string 
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="tagName"></param>
        /// <param name="attrib"></param>
        /// <returns></returns>
        private string GetTagValue(string xmlString, string tagName, string attrib = "")
        {
            var returnString = "";
            if (xmlString != "" && tagName != "")
            {
                var tags = (xmlString.ToUpper()).IndexOf(Strings.UCase("<" + tagName) + " ") + 1; 
                if (tags <= 0)
                {
                    tags = (xmlString.ToUpper()).IndexOf(Strings.UCase("<" + tagName) + ">") + 1; 
                }

                if (tags > 0)
                {
                    returnString = xmlString.Substring(xmlString.Length - (xmlString.Length - tags), xmlString.Length - tags);

                    if (attrib == "")
                    {
                        tags = tags + returnString.IndexOf(">") + 1; 
                                                                
                        if (returnString.IndexOf("/") + 1 < returnString.IndexOf(">") + 1)
                        {
                            return "";
                        }
                        returnString = xmlString.Substring(xmlString.Length - (xmlString.Length - tags), xmlString.Length - tags);
                        tags = (returnString.ToUpper()).IndexOf("</") + 1;
                        returnString = returnString.Substring(0, tags - 1);
                    }
                    else
                    {
                        tags = (returnString.ToUpper()).IndexOf(attrib.ToUpper()) + 1;
                        if (tags > 0)
                        {
                            returnString = returnString.Substring(returnString.Length - (returnString.Length - tags - attrib.Length - 1), returnString.Length - tags - attrib.Length - 1);
                            returnString = returnString.Trim();
                            tags = returnString.IndexOf("\"") + 1;
                            returnString = returnString.Substring(0, tags - 1);
                        }
                        else
                        {
                            returnString = "";
                        }
                    }
                }
            }
            returnString = returnString.Replace("\"", "");
            return returnString;
        }

        /// <summary>
        /// Method to return the PromptTag respective to the prompt code
        /// </summary>
        /// <param name="promptId"></param>
        /// <returns></returns>
        private string GetPromptTag(string promptId)
        {
            var wexPromptTag = "";
            switch (promptId.Trim())
            {
                case "5001":
                    wexPromptTag = "DriverID";
                    break;
                case "5002":
                    wexPromptTag = "Odometer";
                    break;
                case "5003":
                    wexPromptTag = "VehicleID";
                    break;
                case "5004":
                    wexPromptTag = "JobNumber";
                    break;
                case "5005":
                    wexPromptTag = "Data";
                    break;
                case "5006":
                    wexPromptTag = "DeptNumber";
                    break;
                case "5007":
                    wexPromptTag = "LicenseNumber";
                    break;
                case "5008":
                    wexPromptTag = "UserID";
                    break;
                case "5009":
                    wexPromptTag = "CardData";
                    break;
                default:
                    wexPromptTag = "OTHER";
                    break;
            }
            return wexPromptTag;
        }
        
    }
}
