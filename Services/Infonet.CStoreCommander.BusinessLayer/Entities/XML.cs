using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class XML
    {

        //   changed the name of this class from KickBackXML to XML
        // POS needs to create XMLs files for other systems than KickBack and we cannot add a
        // new class with another name everytime , so made this class generic
        // use the class to create XML-s, but be carefull about Initialize because is related
        // to KickBack and has not be changed

        
        public string PosLoyaltyInterfaceVersion { get; set; }

        

        public string VendorName { get; set; }
        public string VendorModelVersion { get; set; }
        public string PosSequenceID { get; set; }
        public string LoyaltySequenceID { get; set; }
        public string StoreLocationID { get; set; }
        public string LoyaltyOfflineFlag { get; set; }

        public string rewardStatusType { get; set; }
        private string pointStatusType; 
        public string entryMethod { get; set; } 
        public string LoyaltyID { get; set; }

        public MSXML2.DOMDocument60 xmldocRequest { get; set; }
        public MSXML2.IXMLDOMNode xmldocRequestRoot { get; set; }
        public MSXML2.IXMLDOMNode xmldocHeaderNode { get; set; }
        public MSXML2.IXMLDOMNode xmldocParent { get; set; }
        public MSXML2.IXMLDOMNode xmldocNode { get; set; }
        public MSXML2.IXMLDOMAttribute xmldocAttribute { get; set; }

        public MSXML2.DOMDocument60 xmldocResponse { get; set; }
        public MSXML2.IXMLDOMNodeList xmldocNodeList { get; set; }

        public bool ResponseValid { get; set; }
        public string ResponseCommand { get; set; }
        public short ResponseError { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponsePointsBalance { get; set; }
        public string ResponseCollectionRatio { get; set; }
        public bool boolLoyaltyIDValid { get; set; }

        public struct typeBusinessPeriod
        {
            public string BusinessData;
            public string PrimaryReportPeriod_Attribute;
            public string PrimaryReportPeriod_Text;
            public string SecondaryReportPeriod_Attribute;
            public string SecondaryReportPeriod_Text;
            public string BeginDate;
            public string BeginTime;
            public string EndDate;
            public string EndTime;
        }

        public typeBusinessPeriod typeBusinessPeriod_Transaction;

        private short i;
        private short k;
        public Sale mvarSale { get; set; }
        public Tenders mvarTenders { get; set; }
        public Sale_Line SL { get; set; }
        public Tender td { get; set; }
        public bool mvarLoyaltyIDRegistered { get; set; }

        private string mvarXMLString; //  ; read-only property

        // For carwash server

        private string Result;
        private string errMsg;
        private string validity;
        private IPolicyManager _policyManager;

        /* added by sonali */
        //public string EntryMethod()
        //{
        //    get;set;
        //}


            /* ended by sonali */
        public string getErrMsg()
        {
            string returnValue = "";
            returnValue = errMsg;
            return returnValue;
        }
        public string getResult()
        {
            string returnValue = "";
            returnValue = Result;
            return returnValue;
        }
        public string getValidity()
        {
            string returnValue = "";
            returnValue = validity;
            return returnValue;
        }
        public Sale Sale
        {
            get
            {
                Sale returnValue = default(Sale);
                if (mvarSale == null)
                {
                    mvarSale = new Sale();
                }
                returnValue = mvarSale;
                return returnValue;
            }
            set
            {
                mvarSale = value;
            }
        }

        //   to add tenders to FinalizeRewards for KickBack

        public Tenders Tenders
        {
            get
            {
                Tenders returnValue = default(Tenders);
                if (mvarTenders == null)
                {
                    mvarTenders = new Tenders();
                }
                returnValue = mvarTenders;
                return returnValue;
            }
            set
            {
                mvarTenders = value;
            }
        }
        //   end

        public dynamic xmlString
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = mvarXMLString;
                return returnValue;
            }
        }
        
        

        public string GetResponseCommandID
        {
            get
            {
                string returnValue = "";
                
                
                if (ResponseValid == true)
                {
                    returnValue = Convert.ToString(xmldocResponse.documentElement.baseName);
                }
                else
                {
                    returnValue = "";
                }
                return returnValue;
            }
        }
        
        
        public string GetLoyaltySequenceID
        {
            get
            {
                string returnValue = "";
                
                
                if (ResponseValid == true)
                {
                    returnValue = LoyaltySequenceID;
                }
                else
                {
                    returnValue = "";
                }
                return returnValue;
            }
        }
        
        
        public bool GetLoyaltyIDValid
        {
            get
            {
                bool returnValue = false;
                
                
                if (ResponseValid == true)
                {
                    returnValue = boolLoyaltyIDValid;
                }
                else
                {
                    returnValue = false;
                }
                return returnValue;
            }
        }
        
        
        public static string GetCustomerMessageData
        {
             get; set; 
            //get
            //{
            //    string returnValue = "";
                
                
            //    if (ResponseValid == true)
            //    {
            //        returnValue = ResponseMessage;
            //    }
            //    else
            //    {
            //        returnValue = "";
            //    }
            //    return returnValue;
            //}
        }
        
        
        public string GetPointsBalance
        {
            get
            {
                string returnValue = "";
                
                
                if (ResponseValid == true)
                {
                    returnValue = ResponsePointsBalance;
                }
                else
                {
                    returnValue = "";
                }
                return returnValue;
            }
        }
        
        
        public string GetCollectionRatio
        {
            get
            {
                string returnValue = "";

                if (ResponseValid == true)
                {
                    returnValue = ResponseCollectionRatio;
                }
                else
                {
                    returnValue = "";
                }
                return returnValue;
            }
        }

        public string GetRequestXMLstring
        {
            get
            {
                string returnValue = "";

                if ((xmldocRequest == null) == false)
                {
                    returnValue = Convert.ToString(xmldocRequest.documentElement.xml);
                }
                else
                {
                    returnValue = "";
                }
                return returnValue;
            }
        }

        public string GetRequestCommandID
        {
            get
            {
                string returnValue = "";

                if ((xmldocRequest == null) == false)
                {
                    
                    
                    returnValue = Convert.ToString(xmldocRequest.documentElement.baseName);
                }
                else
                {
                    returnValue = "";
                }
                return returnValue;
            }
        }

        // Apr 02, 2009 Nicolette added
        public bool LoyaltyIDRegistered
        {
            get
            {
                bool returnValue = false;

                if (ResponseValid == true)
                {
                    returnValue = mvarLoyaltyIDRegistered;
                }
                else
                {
                    returnValue = false;
                }
                return returnValue;
            }
        }

        public object get { get; private set; }

        //public bool Initialize(string strPosLoyaltyInterfaceVersion, string strVendorName, string strVendorModelVersion, string strPOSSequenceID, string strStoreLocationID, string strLoyaltyOfflineFlag)
        //{
        //    bool returnValue = false;


        //    try
        //    {
        //        PosLoyaltyInterfaceVersion = "1.0.0";
        //        VendorName = "InfoNet-Tech";
        //        VendorModelVersion = "Pos 3.00.01";
        //        PosSequenceID = "";
        //        LoyaltySequenceID = "";
        //        StoreLocationID = "gasking";
        //        LoyaltyOfflineFlag = "no"; 
                                           
        //        rewardStatusType = "summary";
        //        entryMethod = "swipe";
        //        LoyaltyID = "";

                
        //        typeBusinessPeriod_Transaction.BusinessData = string.Format("YYYY-MM-DD", DateAndTime.Today);
        //        typeBusinessPeriod_Transaction.PrimaryReportPeriod_Attribute = "day";
        //        typeBusinessPeriod_Transaction.PrimaryReportPeriod_Text = "1001";
        //        typeBusinessPeriod_Transaction.SecondaryReportPeriod_Attribute = "cashier";
        //        typeBusinessPeriod_Transaction.SecondaryReportPeriod_Text = "0";
        //        typeBusinessPeriod_Transaction.BeginDate = string.Format("YYYY-MM-DD", DateAndTime.Today);
        //        typeBusinessPeriod_Transaction.BeginTime = string.Format("HH:MM:SS", DateAndTime.TimeOfDay);
        //        typeBusinessPeriod_Transaction.EndDate = "2100-01-01";
        //        typeBusinessPeriod_Transaction.EndTime = "00:00:00";

        //        if (strPosLoyaltyInterfaceVersion.Trim() != "")
        //        {
        //            PosLoyaltyInterfaceVersion = strPosLoyaltyInterfaceVersion;
        //        }

        //        if (strVendorName.Trim() != "")
        //        {
        //            VendorName = strVendorName;
        //        }

        //        if (strVendorModelVersion.Trim() != "")
        //        {
        //            VendorModelVersion = strVendorModelVersion;
        //        }

        //        if (strPOSSequenceID.Trim() != "")
        //        {
        //            PosSequenceID = strPOSSequenceID;
        //        }

        //        if (strStoreLocationID.Trim() != "")
        //        {
        //            StoreLocationID = strStoreLocationID;
        //        }

        //        if (strLoyaltyOfflineFlag.Trim() != "")
        //        {
        //            LoyaltyOfflineFlag = strLoyaltyOfflineFlag;
        //        }

        //        returnValue = true;
        //    }
        //    catch
        //    {
        //        WriteToLog("Cannot initialize KickBackXML object");
        //        returnValue = false;

        //    }

        //    return returnValue;
        //}
        
        
        public bool SetBusinessPeriod_Transaction(string strBusinessDate, string strAttribute_PrimaryReportPeriod, string strAttribute_SecondaryReportPeriod, string strPrimaryReportPeriod, string strSecondaryReportPeriod, string strBeginDate, string strBeginTime, string strEndDate, string strEndTime)
        {
            bool returnValue = false;
            
            
            try
            {
                
                if (strBusinessDate.Trim() != "")
                {
                    typeBusinessPeriod_Transaction.BusinessData = strBusinessDate;
                }
                
                switch (strAttribute_PrimaryReportPeriod.Trim().ToUpper())
                {
                    case "CASHIER":
                    case "WEEK":
                    case "MONTH":
                    case "YEAR":
                    case "OTHER":
                        typeBusinessPeriod_Transaction.PrimaryReportPeriod_Attribute = strAttribute_PrimaryReportPeriod;
                        break;
                    default:
                        typeBusinessPeriod_Transaction.PrimaryReportPeriod_Attribute = "day";
                        break;
                }
                
                if (strPrimaryReportPeriod.Trim() != "")
                {
                    typeBusinessPeriod_Transaction.PrimaryReportPeriod_Text = strPrimaryReportPeriod;
                }
                
                switch (strAttribute_SecondaryReportPeriod.Trim().ToUpper())
                {
                    case "DAY":
                    case "WEEK":
                    case "MONTH":
                    case "YEAR":
                    case "OTHER":
                        typeBusinessPeriod_Transaction.SecondaryReportPeriod_Attribute = strAttribute_SecondaryReportPeriod;
                        break;
                    default:
                        typeBusinessPeriod_Transaction.SecondaryReportPeriod_Attribute = "cashier";
                        break;
                }
                
                if (strSecondaryReportPeriod.Trim() != "")
                {
                    typeBusinessPeriod_Transaction.SecondaryReportPeriod_Text = strSecondaryReportPeriod;
                }
                
                if (strBeginDate.Trim() != "")
                {
                    typeBusinessPeriod_Transaction.BeginDate = strBeginDate;
                }
                
                if (strBeginTime.Trim() != "")
                {
                    typeBusinessPeriod_Transaction.BeginTime = strBeginTime;
                }
                
                if (strEndDate.Trim() != "")
                {
                    typeBusinessPeriod_Transaction.EndDate = strEndDate;
                }
                
                if (strEndTime.Trim() != "")
                {
                    typeBusinessPeriod_Transaction.EndTime = strEndTime;
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            
            return returnValue;
        }
        
        
        public bool FormatBeginCustomerRequestXML(string strOutsideSalesFlag, string strChoiceName, string strChoiceValue)
        {
            bool returnValue = false;
            
            try
            {
                
                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BeginCustomerRequest", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);
                
                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "OutsideSalesFlag", "");
                xmldocAttribute = xmldocRequest.createAttribute("value");
                
                if (strOutsideSalesFlag.Trim().ToUpper() == "YES")
                {
                    xmldocAttribute.value = "yes";
                }
                else
                {
                    xmldocAttribute.value = "no"; 
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocRequest.documentElement.appendChild(xmldocNode);
                
                
                switch (strChoiceName.Trim().ToUpper())
                {
                    case "KIOSKID":
                        
                        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "KioskID", "");
                        xmldocNode.text = strChoiceValue;
                        xmldocRequest.documentElement.appendChild(xmldocNode);
                        break;
                    case "FUELPOSITIONID":
                        
                        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelPositionID", "");
                        xmldocNode.text = strChoiceValue;
                        xmldocRequest.documentElement.appendChild(xmldocNode);
                        break;
                    default: 
                             
                        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RegisterID", "");
                        xmldocNode.text = strChoiceValue;
                        xmldocRequest.documentElement.appendChild(xmldocNode);
                        break;
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        public bool FormatEndCustomerRequestXML()
        {
            bool returnValue = false;
            
            
            try
            {
                
                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndCustomerRequest", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);
                
                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        public bool FormatCancelTransactionRequestXML()
        {
            bool returnValue = false;
            
            
            try
            {
                
                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "CancelTransactionRequest", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);
                
                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        public bool FormatGetRewardStatusRequestXML(string strLoyaltyID, string strEntryMethod, string strRewardStatusType)
        {
            bool returnValue = false;
            
            try
            {
                
                if (strRewardStatusType.Trim().ToUpper() == "DETAIL")
                {
                    rewardStatusType = "detail";
                }
                else
                {
                    rewardStatusType = "summary";
                }
                switch (strEntryMethod.Trim().ToUpper())
                {
                    case "SCAN":
                        entryMethod = "scan";
                        break;
                    case "RFID":
                        entryMethod = "RFID";
                        break;
                    case "MANUAL":
                        entryMethod = "manual";
                        break;
                    case "OTHER":
                        entryMethod = "other";
                        break;
                    default:
                        entryMethod = "swipe";
                        break;
                }
                
                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "GetRewardStatusRequest", "");
                xmldocAttribute = xmldocRequest.createAttribute("rewardStatusType");
                xmldocAttribute.value = rewardStatusType;
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);
                
                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyID", "");
                xmldocAttribute = xmldocRequest.createAttribute("entryMethod");
                xmldocAttribute.value = entryMethod;
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocNode.text = strLoyaltyID;
                xmldocRequest.documentElement.appendChild(xmldocNode);
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        public bool FormatEndPeriodRequestXML(string Attribute_PrimaryReportPeriod, string Attribute_SecondaryReportPeriod, string From_BusinessDate, string From_PrimaryReportPeriod, string From_SecondaryReportPeriod, string From_BeginDate, string From_BeginTime, string From_EndDate, string From_EndTime, string To_BusinessDate, string To_PrimaryReportPeriod, string To_SecondaryReportPeriod, string To_BeginDate, string To_BeginTime, string To_EndDate, string To_EndTime)
        {
            bool returnValue = false;
            
            
            try
            {
                
                
                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndPeriodRequest", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);
                
                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FromPeriod", "");
                xmldocParent = xmldocRequestRoot.appendChild(xmldocNode);
                
                if (InsertBusinessPeriod(xmldocParent, Attribute_PrimaryReportPeriod, Attribute_SecondaryReportPeriod, From_BusinessDate, From_PrimaryReportPeriod, From_SecondaryReportPeriod, From_BeginDate, From_BeginTime, From_EndDate, From_EndTime) == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ToPeriod", "");
                xmldocParent = xmldocRequestRoot.appendChild(xmldocNode);
                
                if (InsertBusinessPeriod(xmldocParent, Attribute_PrimaryReportPeriod, Attribute_SecondaryReportPeriod, To_BusinessDate, To_PrimaryReportPeriod, To_SecondaryReportPeriod, To_BeginDate, To_BeginTime, To_EndDate, To_EndTime) == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        public bool FormatGetLoyaltyOnlineStatusRequestXML()
        {
            bool returnValue = false;
            
            
            try
            {
                
                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "GetLoyaltyOnlineStatusRequest", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);
                
                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        public bool FormatCancelRedemptionRequestXML(string strLoyaltyRewardIDlist)
        {
            bool returnValue = false;
            
            
            
            string[] strArrayFields = null;
            
            
            try
            {
                
                if (strLoyaltyRewardIDlist.Trim() == "")
                {
                    returnValue = false;
                    return returnValue;
                }
                
                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "CancelRedemptionRequest", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);
                
                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                strArrayFields = (strLoyaltyRewardIDlist + ",").Split(',');
                
                for (i = 0; i <= (strArrayFields.Length - 1); i++)
                {
                    if (strArrayFields[i].Trim() != "")
                    {
                        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                        xmldocNode.text = strArrayFields[i].Trim();
                        xmldocRequestRoot.appendChild(xmldocNode);
                    }
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        public bool FormatGetCustomerMessagingRequestXML()
        {
            bool returnValue = false;
            
            
            try
            {
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }

        //public bool FormatGetPointStatusRequestXML(string strLoyaltyID, string strEntryMethod)
        //{
        //    bool returnValue = false;
        //    switch (strEntryMethod.Trim().ToUpper())
        //    {
        //        case "SCAN":
        //            entryMethod = "scan";
        //            break;
        //        case "RFID":
        //            entryMethod = "RFID";
        //            break;
        //        case "MANUAL":
        //            entryMethod = "manual";
        //            break;
        //        case "OTHER":
        //            entryMethod = "other";
        //            break;
        //        default:
        //            entryMethod = "swipe";
        //            break;
        //    }

        //    xmldocRequest = new MSXML2.DOMDocument60();
        //    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "GetRewardsRequest", "");
        //    xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);

        //    if (InsertRequestHeader() == false)
        //    {
        //        returnValue = false;
        //        return returnValue;
        //    }

        //    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyID", "");
        //    xmldocAttribute = xmldocRequest.createAttribute("entryMethod");
        //    xmldocAttribute.value = entryMethod;
        //    xmldocNode.attributes.setNamedItem(xmldocAttribute);
        //    xmldocNode.text = strLoyaltyID;
        //    xmldocRequest.documentElement.appendChild(xmldocNode);



        //    returnValue = true;
        //    return returnValue;

        //    Err_Handle:
        //    WriteToLog("Cannot create XML GetRewardsRequest Request.");
        //    returnValue = false;

        //    return returnValue;
        //}


        public bool FormatFinalizeRewardsRequestXML(string strLoyaltyID, string strEntryMethod, string strPointsRedeemed, string strFinalTransTotal, string strTrainingModeFlag, string strOutsideSalesFlag, string strChoiceName, string strChoiceValue, string strCashierID, string strTillID, string strEventStartDate, string strEventStartTime, string strEventEndDate, string strEventEndTime)
        {
            bool returnValue = false;
            dynamic Policy_Renamed = default(dynamic);
            
            string FuelServiceLevel = "";
            string IsChange = "";
            string IsPrepay = "";

            try
            {

                xmldocRequest = new MSXML2.DOMDocument60();
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FinalizeRewardsRequest", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);

                if (InsertRequestHeader() == false)
                {
                    returnValue = false;
                    return returnValue;
                }

                if (strLoyaltyID.Trim() != "")
                {

                    switch (strEntryMethod.Trim().ToUpper())
                    {
                        case "SCAN":
                            entryMethod = "scan";
                            break;
                        case "RFID":
                            entryMethod = "RFID";
                            break;
                        case "MANUAL":
                            entryMethod = "manual";
                            break;
                        case "OTHER":
                            entryMethod = "other";
                            break;
                        default:
                            entryMethod = "swipe";
                            break;
                    }

                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyID", "");
                    xmldocAttribute = xmldocRequest.createAttribute("entryMethod");
                    xmldocAttribute.value = entryMethod;
                    xmldocNode.attributes.setNamedItem(xmldocAttribute);
                    xmldocNode.text = strLoyaltyID;
                    xmldocRequest.documentElement.appendChild(xmldocNode);
                }


                if (Conversion.Val(strPointsRedeemed) > 0)
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PointsRedeemed", "");
                    xmldocNode.text = strPointsRedeemed;
                    xmldocRequest.documentElement.appendChild(xmldocNode);
                }


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FinalTransTotal", "");
                xmldocNode.text = strFinalTransTotal;
                xmldocRequest.documentElement.appendChild(xmldocNode);


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionHeader", "");
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);


                if (strTrainingModeFlag.Trim().ToUpper() == "YES")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TrainingModeFlag", "");
                    xmldocAttribute = xmldocRequest.createAttribute("value");
                    xmldocAttribute.value = "yes";
                    xmldocNode.attributes.setNamedItem(xmldocAttribute);
                    xmldocHeaderNode.appendChild(xmldocNode);
                }


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "OutsideSalesFlag", "");
                xmldocAttribute = xmldocRequest.createAttribute("value");
                if (strOutsideSalesFlag.Trim().ToUpper() == "YES")
                {
                    xmldocAttribute.value = "yes";
                }
                else
                {
                    xmldocAttribute.value = "no";
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode.appendChild(xmldocNode);


                switch (strChoiceName.Trim().ToUpper())
                {
                    case "KIOSKID":

                        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "KioskID", "");
                        xmldocNode.text = strChoiceValue;
                        xmldocHeaderNode.appendChild(xmldocNode);
                        break;
                    case "FUELPOSITIONID":

                        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelPositionID", "");
                        xmldocNode.text = strChoiceValue;
                        xmldocHeaderNode.appendChild(xmldocNode);
                        break;
                    default:

                        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RegisterID", "");
                        xmldocNode.text = strChoiceValue;
                        xmldocHeaderNode.appendChild(xmldocNode);
                        break;
                }


                if (strCashierID.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "CashierID", "");
                    xmldocNode.text = strCashierID;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TillID", "");
                xmldocNode.text = strTillID;
                xmldocHeaderNode.appendChild(xmldocNode);


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSTransactionID", "");
                xmldocNode.text = PosSequenceID;
                xmldocHeaderNode.appendChild(xmldocNode);


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BusinessPeriod", "");
                xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BusinessDate", "");
                xmldocNode.text = typeBusinessPeriod_Transaction.BusinessData;
                xmldocParent.appendChild(xmldocNode);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PrimaryReportPeriod", "");
                xmldocAttribute = xmldocRequest.createAttribute("interval");
                xmldocAttribute.value = typeBusinessPeriod_Transaction.PrimaryReportPeriod_Attribute;
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocNode.text = typeBusinessPeriod_Transaction.PrimaryReportPeriod_Text;
                xmldocParent.appendChild(xmldocNode);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SecondaryReportPeriod", "");
                xmldocAttribute = xmldocRequest.createAttribute("interval");
                xmldocAttribute.value = typeBusinessPeriod_Transaction.SecondaryReportPeriod_Attribute;
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocNode.text = typeBusinessPeriod_Transaction.SecondaryReportPeriod_Text;
                xmldocParent.appendChild(xmldocNode);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BeginDate", "");
                xmldocNode.text = typeBusinessPeriod_Transaction.BeginDate;
                xmldocParent.appendChild(xmldocNode);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BeginTime", "");
                xmldocNode.text = typeBusinessPeriod_Transaction.BeginTime;
                xmldocParent.appendChild(xmldocNode);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndDate", "");
                xmldocNode.text = typeBusinessPeriod_Transaction.EndDate;
                xmldocParent.appendChild(xmldocNode);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndTime", "");
                xmldocNode.text = typeBusinessPeriod_Transaction.EndTime;
                xmldocParent.appendChild(xmldocNode);


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventStartDate", "");
                xmldocNode.text = strEventStartDate;
                xmldocHeaderNode.appendChild(xmldocNode);


                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventStartTime", "");
                xmldocNode.text = strEventStartTime;
                xmldocHeaderNode.appendChild(xmldocNode);


                if (strEventEndDate.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventEndDate", "");
                    xmldocNode.text = strEventEndDate;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }


                if (strEventEndTime.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventEndTime", "");
                    xmldocNode.text = strEventEndTime;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }

                foreach (Sale_Line tempLoopVar_SL in mvarSale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    //  : messages have been changed to use ItemLine for non fuel items and FuelItem for fuel items
                    if (SL.ProductIsFuel)
                    {
                        IsPrepay = Convert.ToString(SL.Prepay ? "yes" : "no");
                        if (SL.pumpID == 0)
                        {
                            FuelServiceLevel = "full";
                        }
                        else
                        {
                            FuelServiceLevel = Convert.ToString(((int)(Variables.gPumps.get_Pump(SL.pumpID).LevelID) == 1) ? "self" : "full");
                        }
                        AppendFuelLineInfo("", IsPrepay, (SL.GradeID).ToString(), (SL.PositionID).ToString(), "", "", FuelServiceLevel, "", (SL.price).ToString(), (SL.Regular_Price).ToString(), (SL.Quantity).ToString(), (SL.Amount).ToString(), "liter", "", "", "", "1");
                    }
                    else
                    {
                        AppendItemLineInfo("", "yes", "upcA", SL.Stock_Code, "0", SL.Dept, SL.Sub_Dept, SL.Sub_Detail, SL.Description, "", (SL.price).ToString(), (SL.Regular_Price).ToString(), "1", (SL.Quantity).ToString(), (SL.Amount).ToString(), "", "", "", "", "");

                    }
                    //   end
                }
            

                //   add TenderInfo element to FinalizeRewardsRequest
                if (!(mvarTenders == null))
                {
                    foreach (Tender tempLoopVar_td in mvarTenders)
                    {
                        td = tempLoopVar_td;
                        if (td.Amount_Used != 0)
                        {
                            IsChange = Convert.ToString((Tenders.Tend_Totals.Change != 0 && td.Tender_Name.ToUpper() == Strings.UCase(Convert.ToString(Policy_Renamed.BASECURR))) ? "yes" : "no");
                            AppendTenderInfo("", td.PCATSGroup, "generic", "", "", (td.Amount_Used).ToString(), IsChange);
                        }
                    }
                }
                //   end

                returnValue = true;
            }
            catch
            {

                WriteToLog("Cannot create XML FinalizeRewards Request .");
                WriteToLog("Customer " + Chaps_Main.SA.Customer.Name + " will not earn points with card " + Chaps_Main.SA.Customer.PointCardNum);

                returnValue = false;
            }
            
            return returnValue;
        }
        
        
        public bool AppendTenderInfo(string strTransactionLineAttribute, string strTenderCode, string strTenderSubCode, string strISOPrefix, string strLoyaltyRewardID, string strTenderAmount, string strChangeFlagAttribute)
        {
            bool returnValue = false;
            
            string strLineNumber = "";
            
            try
            {
                
                if ((xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                if ((xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
                {
                    
                    returnValue = false;
                    return returnValue;
                }
                
                ///    Set xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData")
                ///    If xmldocNodeList.Length = 0 Then
                ///        ''Tag <TransactionData> not exist, creating the node
                ///        Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "TransactionData", "")
                ///        Set xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode)
                ///    Else
                ///        Set xmldocHeaderNode = xmldocNodeList.nextNode
                ///    End If
                ///    ''
                ///    Set xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup")
                ///    If xmldocNodeList.Length = 0 Then
                ///        ''Tag <TransactionDetailGroup> not exist, creating the node
                ///        Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "TransactionDetailGroup", "")
                ///        Set xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode)
                ///    Else
                ///        ''Tag <TransactionDetailGroup> exists
                ///        Set xmldocHeaderNode = xmldocNodeList.nextNode
                ///    End If
                ///    ''
                ///    Set xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine")
                ///    If xmldocNodeList.Length = 0 Then
                ///        strLineNumber = "1"
                ///    Else
                ///        strLineNumber = Trim$(CStr(xmldocNodeList.Length + 1))
                ///    End If
                ///    ''
                ///    ''Create Tag <TransactionLine>
                ///    Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "TransactionLine", "")
                ///    Set xmldocAttribute = xmldocRequest.createAttribute("status")
                ///    Select Case UCase(Trim$(strTransactionLineAttribute))
                ///        Case "VOID", "RETURN"
                ///            xmldocAttribute.Value = strTransactionLineAttribute
                ///        Case Else
                ///            xmldocAttribute.Value = "normal"
                ///    End Select
                ///    xmldocNode.Attributes.setNamedItem xmldocAttribute
                ///    Call xmldocHeaderNode.appendChild(xmldocNode)
                ///    ''
                ///    ''Create Tag <LineNumber>
                ///    Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "LineNumber", "")
                ///    xmldocNode.Text = strLineNumber
                ///    Call xmldocHeaderNode.appendChild(xmldocNode)
                ///    ''
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
                }
                else
                {
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                }
                else
                {
                    
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xmldocNodeList.length + 1).ToString().Trim();
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xmldocAttribute.value = "normal";
                        break;
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xmldocNode.text = strLineNumber;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderInfo", "");
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderCode", "");
                xmldocNode.text = strTenderCode;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderSubCode", "");
                xmldocNode.text = strTenderSubCode;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                if (strISOPrefix.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ISOPrefix", "");
                    xmldocNode.text = strISOPrefix;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                
                if (strLoyaltyRewardID.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                    xmldocNode.text = strLoyaltyRewardID;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderAmount", "");
                xmldocNode.text = strTenderAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ChangeFlag", "");
                xmldocAttribute = xmldocRequest.createAttribute("value");
                if (strChangeFlagAttribute.Trim().ToUpper() == "YES")
                {
                    xmldocAttribute.value = strChangeFlagAttribute;
                }
                else
                {
                    xmldocAttribute.value = "no";
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode.appendChild(xmldocNode);
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            
            return returnValue;
        }
        
        
        public bool AppendFuelLineInfo(string strTransactionLineAttribute, string strFuelLineAttribute, string strFuelGradeID, string strFuelPositionID, string strPriceTierCode, string strTimeTierCode, string strServiceLevelCode, string strDescription, string strActualSalesPrice, string strRegularSellPrice, string strSalesQuantity, string strSalesAmount, string strSalesUOM, string strPromotion, string strDiscount, string strItemTax, string strPaymentSystemsProductCode)
        {
            bool returnValue = false;
            
            
            
            
            
            
            
            string strLineNumber = "";
            string[] strArrayItems = null; 
            string[] strArrayFields = null; 
                                            
            try
            {
                
                if ((xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                if ((xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
                {
                    
                    returnValue = false;
                    return returnValue;
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
                }
                else
                {
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                }
                else
                {
                    
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xmldocNodeList.length + 1).ToString().Trim();
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xmldocAttribute.value = "normal";
                        break;
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xmldocNode.text = strLineNumber;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("fuelPrepayFlag");
                if (strFuelLineAttribute.Trim().ToUpper() == "YES")
                {
                    xmldocAttribute.value = "yes";
                }
                else
                {
                    xmldocAttribute.value = "no";
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelGradeID", "");
                xmldocNode.text = strFuelGradeID;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelPositionID", "");
                xmldocNode.text = strFuelPositionID;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                if (strPriceTierCode.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceTierCode", "");
                    xmldocNode.text = strPriceTierCode;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                
                if (strPriceTierCode.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TimeTierCode", "");
                    xmldocNode.text = strTimeTierCode;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ServiceLevelCode", "");
                xmldocNode.text = strServiceLevelCode;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                if (strDescription.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Description", "");
                    xmldocNode.text = strDescription;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ActualSalesPrice", "");
                xmldocNode.text = strActualSalesPrice;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RegularSellPrice", "");
                xmldocNode.text = strRegularSellPrice;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesQuantity", "");
                xmldocNode.text = strSalesQuantity;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesAmount", "");
                xmldocNode.text = strSalesAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                //   added SalesUOM tag
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesUOM", "");
                xmldocNode.text = strSalesUOM;
                xmldocHeaderNode.appendChild(xmldocNode);
                //   end

                
                if (strPromotion.Trim() != "")
                {
                    strArrayItems = (strPromotion + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Promotion", "");
                            xmldocAttribute = xmldocRequest.createAttribute("status");
                            xmldocAttribute.value = strArrayFields[0].Trim();
                            xmldocNode.attributes.setNamedItem(xmldocAttribute);
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionID", "");
                            xmldocNode.text = strArrayFields[1].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            if (strArrayFields[2].Trim() != "")
                            {
                                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                                xmldocNode.text = strArrayFields[2].Trim();
                                xmldocParent.appendChild(xmldocNode);
                            }
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionAmount", "");
                            xmldocNode.text = strArrayFields[3].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionReason", "");
                            xmldocNode.text = strArrayFields[4].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                
                if (strDiscount.Trim() != "")
                {
                    strArrayItems = (strDiscount + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Discount", "");
                            xmldocAttribute = xmldocRequest.createAttribute("status");
                            xmldocAttribute.value = strArrayFields[0].Trim();
                            xmldocNode.attributes.setNamedItem(xmldocAttribute);
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountID", "");
                            xmldocNode.text = strArrayFields[1].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountAmount", "");
                            xmldocNode.text = strArrayFields[2].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountReason", "");
                            xmldocNode.text = strArrayFields[3].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                
                if (strItemTax.Trim() != "")
                {
                    strArrayItems = (strItemTax + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemTax", "");
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxLevelID", "");
                            xmldocNode.text = strArrayFields[0].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            switch (strArrayFields[1].Trim().ToUpper())
                            {
                                case "TAXREFUNDEDAMOUNT":
                                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxRefundedAmount", "");
                                    break;
                                default:
                                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxCollectedAmount", "");
                                    break;
                            }
                            xmldocNode.text = strArrayFields[2].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                //   added PaymentSystemsProductCode tag
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PaymentSystemsProductCode", "");
                xmldocNode.text = strPaymentSystemsProductCode;
                xmldocHeaderNode.appendChild(xmldocNode);
                // Nicolette, M ay 02, 2011 end

                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            
            return returnValue;
        }
        
        

        
        public bool AppendTransactionTaxInfo(string strTransactionLineAttribute, string strTaxLevelID, string strTaxableSalesAmount, string strTaxCollectedAmount, string strTaxableSalesRefundedAmount, string strTaxRefundedAmount)
        {
            bool returnValue = false;
            
            string strLineNumber = "";
            
            try
            {
                
                if ((xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                if ((xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
                {
                    
                    returnValue = false;
                    return returnValue;
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
                }
                else
                {
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                }
                else
                {
                    
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xmldocNodeList.length + 1).ToString().Trim();
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xmldocAttribute.value = "normal";
                        break;
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xmldocNode.text = strLineNumber;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionTax", "");
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxLevelID", "");
                xmldocNode.text = strTaxLevelID;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxableSalesAmount", "");
                xmldocNode.text = strTaxableSalesAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxCollectedAmount", "");
                xmldocNode.text = strTaxCollectedAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxableSalesRefundedAmount", "");
                xmldocNode.text = strTaxableSalesRefundedAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxRefundedAmount", "");
                xmldocNode.text = strTaxRefundedAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            
            return returnValue;
        }
        
        
        public bool AppendMerchandiseCodeLineInfo(string strTransactionLineAttribute, string strMerchandiseCodeLineAttribute, string strDepartment, string strSubDepartment, string strSubDetail, string strDescription, string strActualSalesPrice, string strSalesQuantity, string strSalesAmount, string strPromotion, string strDiscount, string strItemTax)
        {
            bool returnValue = false;
            
            
            
            
            
            
            
            string strLineNumber = "";
            string[] strArrayItems = null; 
            string[] strArrayFields = null; 
                                            
            try
            {
                
                if ((xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                if ((xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
                {
                    
                    returnValue = false;
                    return returnValue;
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
                }
                else
                {
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                }
                else
                {
                    
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xmldocNodeList.length + 1).ToString().Trim();
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xmldocAttribute.value = "normal";
                        break;
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xmldocNode.text = strLineNumber;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "MerchandiseCodeLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("discountable");
                if (strMerchandiseCodeLineAttribute.Trim().ToUpper() == "YES")
                {
                    xmldocAttribute.value = "yes";
                }
                else
                {
                    xmldocAttribute.value = "no";
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                ///    Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "MerchandiseCode", "")
                ///    xmldocNode.Text = strMerchandiseCode
                ///    Call xmldocHeaderNode.appendChild(xmldocNode)
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Department", "");
                xmldocNode.text = strDepartment;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SubDepartment", "");
                xmldocNode.text = strSubDepartment;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SubDetail", "");
                xmldocNode.text = strSubDetail;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                if (strDescription.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Description", "");
                    xmldocNode.text = strDescription;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ActualSalesPrice", "");
                xmldocNode.text = strActualSalesPrice;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesQuantity", "");
                xmldocNode.text = strSalesQuantity;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesAmount", "");
                xmldocNode.text = strSalesAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                if (strPromotion.Trim() != "")
                {
                    strArrayItems = (strPromotion + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Promotion", "");
                            xmldocAttribute = xmldocRequest.createAttribute("status");
                            xmldocAttribute.value = strArrayFields[0].Trim();
                            xmldocNode.attributes.setNamedItem(xmldocAttribute);
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionID", "");
                            xmldocNode.text = strArrayFields[1].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            if (strArrayFields[2].Trim() != "")
                            {
                                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                                xmldocNode.text = strArrayFields[2].Trim();
                                xmldocParent.appendChild(xmldocNode);
                            }
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionAmount", "");
                            xmldocNode.text = strArrayFields[3].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionReason", "");
                            xmldocNode.text = strArrayFields[4].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                
                if (strDiscount.Trim() != "")
                {
                    strArrayItems = (strDiscount + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Discount", "");
                            xmldocAttribute = xmldocRequest.createAttribute("status");
                            xmldocAttribute.value = strArrayFields[0].Trim();
                            xmldocNode.attributes.setNamedItem(xmldocAttribute);
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountID", "");
                            xmldocNode.text = strArrayFields[1].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountAmount", "");
                            xmldocNode.text = strArrayFields[2].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountReason", "");
                            xmldocNode.text = strArrayFields[3].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                
                if (strItemTax.Trim() != "")
                {
                    strArrayItems = (strItemTax + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemTax", "");
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxLevelID", "");
                            xmldocNode.text = strArrayFields[0].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            switch (strArrayFields[1].Trim().ToUpper())
                            {
                                case "TAXREFUNDEDAMOUNT":
                                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxRefundedAmount", "");
                                    break;
                                default:
                                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxCollectedAmount", "");
                                    break;
                            }
                            xmldocNode.text = strArrayFields[2].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            
            return returnValue;
        }
       //TODO: Kickback_Removed
        //public void AnalyseKickBackResponse(string responseXml)
        //{

        //    string strValidResult;

        //    try
        //    {
        //        
        //        ResponseValid = false;
        //        ResponseCommand = "";
        //        ResponseError = (short)(-1);
        //        ResponseMessage = "";
        //        ResponsePointsBalance = "";
        //        ResponseCollectionRatio = "";
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
        //        xmldocResponse = new MSXML2.DOMDocument60();
        //        xmldocResponse.loadXML(responseXml);

        //        ResponseValid = true;
        //        ResponseCommand = System.Convert.ToString(xmldocResponse.documentElement.nodeName);
        //        ResponseError = (short)0;

        //        
        //        if ((ResponseCommand == "GetRewardsResponse") || (ResponseCommand == "GetRewardStatusResponse") || (ResponseCommand == "GetPointStatusResponse"))
        //        {

        //            xmldocNodeList = xmldocResponse.documentElement.selectNodes("//LoyaltyIDValidFlag");
        //            if (xmldocNodeList.length > 0)
        //            {
        //                if (Strings.UCase(xmldocNodeList[0].attributes[0].text).Trim() == "YES")
        //                {
        //                    boolLoyaltyIDValid = true;
        //                }
        //                else
        //                {
        //                    boolLoyaltyIDValid = false;
        //                }
        //            }
        //            else
        //            {
        //                boolLoyaltyIDValid = false;
        //            }

        //            // Apr 02, 2009 Nicolette added LoyaltyIDRegistered checking
        //            xmldocNodeList = xmldocResponse.documentElement.selectNodes("//LoyaltyIDRegistered");
        //            if (xmldocNodeList.length > 0)
        //            {
        //                if (Strings.UCase(xmldocNodeList[0].attributes[0].text).Trim() == "YES")
        //                {
        //                    mvarLoyaltyIDRegistered = true;
        //                }
        //                else
        //                {
        //                    mvarLoyaltyIDRegistered = false;
        //                }
        //            }
        //            else
        //            {
        //                mvarLoyaltyIDRegistered = false;
        //            }
        //            // Apr 02, 2009 Nicolette end
        //        }

        //        
        //        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//LoyaltySequenceID");
        //        if (xmldocNodeList.length > 0)
        //        {
        //            LoyaltySequenceID = xmldocNodeList[0].text;
        //        }
        //        
        //        
        //        
        //        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//PointBalance");
        //        if (xmldocNodeList.length > 0)
        //        {
        //            ResponsePointsBalance = xmldocNodeList[0].text;
        //            // Feb 18, 2009: Nicolette added to set the balance inside this class
        //            // so the system doesn't have to check the balance outside
        //            if (!(Sale.Customer == null))
        //            {
        //                Sale.Customer.Balance_Points = Conversion.Val(ResponsePointsBalance);
        //            }
        //            // Feb 18, 2009: Nicolette end
        //        }
        //        
        //        
        //        
        //        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//CollectionRatio");
        //        if (xmldocNodeList.length > 0)
        //        {
        //            ResponseCollectionRatio = xmldocNodeList[0].text;
        //            // Feb 18, 2009: Nicolette added to set the exchange rate for the points inside this class
        //            if (!(Sale.Customer == null))
        //            {
        //                Sale.Customer.Points_ExchangeRate = Conversion.Val(ResponseCollectionRatio);
        //            }
        //            else
        //            {
        //                Sale.Customer.Points_ExchangeRate = 1;
        //            }
        //            // Feb 18, 2009: Nicolette end

        //        }
        //        
        //        
        //        
        //        
        //        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//ReceiptLine");
        //        if (xmldocNodeList.length > 0)
        //        {
        //            for (i = 1; i <= xmldocNodeList.length; i++)
        //            {
        //                ResponseMessage = ResponseMessage + xmldocNodeList[i - 1].text + "\r\n";
        //            }
        //        }
        //        
        //        
        //        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//DisplayLine");
        //        if (xmldocNodeList.length > 0)
        //        {
        //            Sale.Customer.DisplayLine = xmldocNodeList[0].text;
        //        }

        //        
        //    }
        //    catch
        //    {
        //        ResponseValid = false;
        //        
        //        WriteToLog("Cannot analyse XML GetPointStatus Response");
        //    }
        //    
        //}
        //END: Kickback_Removed

        public bool InsertRequestHeader()
        {
            bool returnValue = false;

            try
            {
                
                
                if ((xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                xmldocRequestRoot = xmldocRequest.documentElement;
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RequestHeader", "");
                xmldocHeaderNode = xmldocRequestRoot.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PosLoyaltyInterfaceVersion", "");
                xmldocNode.text = PosLoyaltyInterfaceVersion;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "VendorName", "");
                xmldocNode.text = VendorName;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "VendorModelVersion", "");
                xmldocNode.text = VendorModelVersion;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSSequenceID", "");
                if (PosSequenceID.Trim() != "")
                {
                    xmldocNode.text = PosSequenceID;
                }
                xmldocHeaderNode.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltySequenceID", "");
                if (LoyaltySequenceID.Trim() != "")
                {
                    xmldocNode.text = LoyaltySequenceID;
                }
                xmldocHeaderNode.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "StoreLocationID", "");
                xmldocNode.text = StoreLocationID;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyOfflineFlag", "");
                xmldocAttribute = xmldocRequest.createAttribute("value");
                xmldocAttribute.value = LoyaltyOfflineFlag;
                
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode.appendChild(xmldocNode);
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        private bool InsertBusinessPeriod(MSXML2.IXMLDOMNode xmldocBusinessPeriodParent, string Attribute_PrimaryReportPeriod, string Attribute_SecondaryReportPeriod, string strBusinessDate, string strPrimaryReportPeriod, string strSecondaryReportPeriod, string strBeginDate, string strBeginTime, string strEndDate, string strEndTime)
        {
            bool returnValue = false;
            
            
            try
            {
                
                
                if ((xmldocRequest == null) || (xmldocBusinessPeriodParent == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BusinessPeriod", "");
                xmldocParent = xmldocBusinessPeriodParent.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BusinessDate", "");
                xmldocNode.text = strBusinessDate;
                xmldocParent.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PrimaryReportPeriod", "");
                xmldocAttribute = xmldocRequest.createAttribute("interval");
                switch (Attribute_PrimaryReportPeriod.Trim().ToUpper())
                {
                    case "CASHIER":
                    case "WEEK":
                    case "MONTH":
                    case "YEAR":
                    case "OTHER":
                        xmldocAttribute.value = Attribute_PrimaryReportPeriod;
                        break;
                    default:
                        xmldocAttribute.value = "day";
                        break;
                }
                
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocNode.text = strPrimaryReportPeriod;
                xmldocParent.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SecondaryReportPeriod", "");
                xmldocAttribute = xmldocRequest.createAttribute("interval");
                switch (Attribute_SecondaryReportPeriod.Trim().ToUpper())
                {
                    case "DAY":
                    case "WEEK":
                    case "MONTH":
                    case "YEAR":
                    case "OTHER":
                        xmldocAttribute.value = Attribute_SecondaryReportPeriod;
                        break;
                    default:
                        xmldocAttribute.value = "cashier";
                        break;
                }
                
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocNode.text = strSecondaryReportPeriod;
                xmldocParent.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BeginDate", "");
                xmldocNode.text = strBeginDate;
                xmldocParent.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BeginTime", "");
                xmldocNode.text = strBeginTime;
                xmldocParent.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndDate", "");
                xmldocNode.text = strEndDate;
                xmldocParent.appendChild(xmldocNode);
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndTime", "");
                xmldocNode.text = strEndTime;
                xmldocParent.appendChild(xmldocNode);
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        
        
        private string ValidateXML(string sXML, string sXSDPath)
        {
            string returnValue = "";
            string sNamespace = "";
            MSXML2.XMLSchemaCache60 objSchemas = default(MSXML2.XMLSchemaCache60);
            MSXML2.DOMDocument60 oXML = default(MSXML2.DOMDocument60);
            MSXML2.DOMDocument60 oXSD = default(MSXML2.DOMDocument60);
            MSXML2.IXMLDOMParseError oErr = default(MSXML2.IXMLDOMParseError);
            
            
            //On Error Resume Next VBConversions Warning: On Error Resume Next not supported in C#
            
            returnValue = "";
            
            
            oXSD = new MSXML2.DOMDocument60();
            if (Information.Err().Number != 0)
            {
                returnValue = "Fail - ActiveX Component Load failure";
                return returnValue;
            }
            
            oXSD.async = false;
            if (!oXSD.load(sXSDPath))
            {
                returnValue = "Fail - XSD Load failure";
                return returnValue;
            }
            else
            {
                
                sNamespace = Convert.ToString(oXSD.documentElement.getAttribute("targetNamespace"));
            }
            
            
            objSchemas = new MSXML2.XMLSchemaCache60();
            if (Information.Err().Number != 0)
            {
                returnValue = "Fail - ActiveX Component Load failure";
                return returnValue;
            }
            objSchemas.add(sNamespace, oXSD);
            
            
            oXML = new MSXML2.DOMDocument60();
            if (Information.Err().Number != 0)
            {
                returnValue = "Fail - ActiveX Component Load failure";
                return returnValue;
            }
            
            oXML.async = false;
            oXML.validateOnParse = false;
            oXML.resolveExternals = false;
            
            
            if (!oXML.loadXML(sXML))
            {
                returnValue = "Fail - XML Load failure";
                return returnValue;
            }
            
            
            oXML.schemas = objSchemas;
            
            
            oErr = oXML.validate();
            
            if (oErr.errorCode != 0)
            {
                returnValue = "Fail - " + oErr.reason;
            }
            else
            {
                returnValue = "";
            }
            
            return returnValue;
        }
        
        

        public void Class_Initialize_Renamed()
        {
            
            

            xmldocRequest = null;

            xmldocResponse = null;
            mvarXMLString = "";
            if (_policyManager.Use_KickBack)
            {
                mvarLoyaltyIDRegistered = false;
            }

        }
        public XML(IPolicyManager policyManager)
        {
            _policyManager = policyManager;
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {


            xmldocRequest = null;

            xmldocRequestRoot = null;

            xmldocParent = null;

            xmldocNode = null;

            xmldocAttribute = null;

            xmldocHeaderNode = null;
            

            xmldocResponse = null;

            xmldocNodeList = null;

            mvarSale = null;

            mvarTenders = null;

        }
        ~XML()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }

        //
        private void WriteToLog(string Msgstr)
        {

            string FileName = "";
            short fnum = 0;
            short i;
            string NewFName = "";

            try
            {
                if (Chaps_Main.Register_Renamed.WritePosLog)
                {
                    //   made this log file based on policy, because we have to reuse this class
                    // for other XMLs than KickBack
                    if (_policyManager.Use_KickBack)
                    {
                        FileName = Chaps_Main.Logs_Path + "KickBack.log";
                    }
                    else
                    {
                        FileName = Chaps_Main.Logs_Path + "XML.log";
                    }
                    if (FileSystem.Dir(FileName) != "")
                    {
                        if (FileSystem.FileLen(FileName) > 1000000)
                        {
                            if (_policyManager.Use_KickBack)
                            {
                                //NewFName = Chaps_Main.Logs_Path + "KickBack" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(VB.DateAndTime.Day(DateAndTime.Today), "00") + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(DateAndTime.Hour(DateAndTime.TimeOfDay), "00") +".log";
                            }
                            else
                            {
                                NewFName = Chaps_Main.Logs_Path + "XML" + DateTime.Today.Day.ToString("00") + DateTime.Today.Hour.ToString("00") + ".log";
                            }
                            Variables.CopyFile(FileName, NewFName, 0);
                            Variables.DeleteFile(FileName);
                        }
                    }

                    fnum = (short)(FileSystem.FreeFile());
                    FileSystem.FileOpen(fnum, FileName, OpenMode.Append);
                    FileSystem.PrintLine(fnum, DateTime.Now + " " + Msgstr);
                    FileSystem.FileClose(fnum);
                }
            }
            catch
            {
                goto Err_End;
            }
            Err_End:
            1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.

        }
        //End - SV

        public bool AppendItemLineInfo(string strTransactionLineAttribute, string strItemLineAttribute, string strItemCode_POSCodeFormat, string strItemCode_POSCode, string strItemCode_POSCodeModifier, string strDepartment, string strSubDepartment, string strSubDetail, string strDescription, string strEntryMethod, string strActualSalesPrice, string strRegularSellPrice, string strSellingUnits, string strSalesQuantity, string strSalesAmount, string strPriceOverride, string strPromotion, string strDiscount, string strItemTax, string strLinkedFromLineNumber)
        {
            bool returnValue = false;
            
            string strLineNumber = "";
            string[] strArrayItems = null; 
            string[] strArrayFields = null; 
                                            
            try
            {
                
                if ((xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }
                
                if ((xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
                {
                    
                    returnValue = false;
                    return returnValue;
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
                }
                else
                {
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xmldocNodeList.length == 0)
                {
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                }
                else
                {
                    
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }
                
                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xmldocNodeList.length + 1).ToString().Trim();
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xmldocAttribute.value = "normal";
                        break;
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xmldocNode.text = strLineNumber;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemLine", "");
                xmldocAttribute = xmldocRequest.createAttribute("discountable");
                if (strItemLineAttribute.Trim().ToUpper() == "YES")
                {
                    xmldocAttribute.value = "yes";
                }
                else
                {
                    xmldocAttribute.value = "no";
                }
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemCode", "");
                xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSCodeFormat", "");
                
                switch (strItemCode_POSCodeFormat.Trim().ToUpper())
                {
                    case "UPCA":
                        xmldocNode.text = "upcA";
                        break;
                    case "UPCE":
                        xmldocNode.text = "upcE";
                        break;
                    case "EAN8":
                        xmldocNode.text = "ean8";
                        break;
                    case "EAN13":
                        xmldocNode.text = "ean13";
                        break;
                    case "PLU":
                        xmldocNode.text = "plu";
                        break;
                    case "GTIN":
                        xmldocNode.text = "gtin";
                        break;
                    case "RSS14":
                        xmldocNode.text = "rss14";
                        break;
                    default:
                        xmldocNode.text = "none";
                        break;
                }
                xmldocParent.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSCode", "");
                xmldocNode.text = strItemCode_POSCode;
                xmldocParent.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSCodeModifier", "");
                xmldocNode.text = strItemCode_POSCodeModifier;
                xmldocParent.appendChild(xmldocNode);
                
                
                ///    Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "MerchandiseCode", "")
                ///    xmldocNode.Text = strMerchandiseCode
                ///    Call xmldocHeaderNode.appendChild(xmldocNode)
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Department", "");
                xmldocNode.text = strDepartment;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SubDepartment", "");
                xmldocNode.text = strSubDepartment;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SubDetail", "");
                xmldocNode.text = strSubDetail;
                xmldocHeaderNode.appendChild(xmldocNode);

                
                if (strDescription.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Description", "");
                    xmldocNode.text = strDescription;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EntryMethod", "");
                switch (strEntryMethod.Trim().ToUpper())
                {
                    case "MANUAL":
                        xmldocNode.text = "manual";
                        break;
                    case "OTHER":
                        xmldocNode.text = "other";
                        break;
                    default:
                        xmldocNode.text = "scan";
                        break;
                }
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ActualSalesPrice", "");
                xmldocNode.text = strActualSalesPrice;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RegularSellPrice", "");
                xmldocNode.text = strRegularSellPrice;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SellingUnits", "");
                xmldocNode.text = strSellingUnits;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesQuantity", "");
                xmldocNode.text = strSalesQuantity;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesAmount", "");
                xmldocNode.text = strSalesAmount;
                xmldocHeaderNode.appendChild(xmldocNode);
                
                
                if (strPriceOverride.Trim() != "")
                {
                    strArrayFields = (strPriceOverride + ",").Split(',');
                    
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceOverride", "");
                    xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                    
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceOverridePrice", "");
                    xmldocNode.text = strArrayFields[0].Trim();
                    xmldocParent.appendChild(xmldocNode);
                    
                    
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceOverrideReason", "");
                    xmldocNode.text = strArrayFields[1].Trim();
                    xmldocParent.appendChild(xmldocNode);
                    
                }
                
                
                if (strPromotion.Trim() != "")
                {
                    strArrayItems = (strPromotion + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Promotion", "");
                            xmldocAttribute = xmldocRequest.createAttribute("status");
                            xmldocAttribute.value = strArrayFields[0].Trim();
                            xmldocNode.attributes.setNamedItem(xmldocAttribute);
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionID", "");
                            xmldocNode.text = strArrayFields[1].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            if (strArrayFields[2].Trim() != "")
                            {
                                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                                xmldocNode.text = strArrayFields[2].Trim();
                                xmldocParent.appendChild(xmldocNode);
                            }
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionAmount", "");
                            xmldocNode.text = strArrayFields[3].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionReason", "");
                            xmldocNode.text = strArrayFields[4].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                
                if (strDiscount.Trim() != "")
                {
                    strArrayItems = (strDiscount + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Discount", "");
                            xmldocAttribute = xmldocRequest.createAttribute("status");
                            xmldocAttribute.value = strArrayFields[0].Trim();
                            xmldocNode.attributes.setNamedItem(xmldocAttribute);
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountID", "");
                            xmldocNode.text = strArrayFields[1].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountAmount", "");
                            xmldocNode.text = strArrayFields[2].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountReason", "");
                            xmldocNode.text = strArrayFields[3].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                
                if (strItemTax.Trim() != "")
                {
                    strArrayItems = (strItemTax + ";").Split(';');
                    for (i = 0; i <= (strArrayItems.Length - 1); i++)
                    {
                        
                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemTax", "");
                            xmldocParent = xmldocHeaderNode.appendChild(xmldocNode);
                            
                            
                            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxLevelID", "");
                            xmldocNode.text = strArrayFields[0].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                            
                            switch (strArrayFields[1].Trim().ToUpper())
                            {
                                case "TAXREFUNDEDAMOUNT":
                                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxRefundedAmount", "");
                                    break;
                                default:
                                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxCollectedAmount", "");
                                    break;
                            }
                            xmldocNode.text = strArrayFields[2].Trim();
                            xmldocParent.appendChild(xmldocNode);
                            
                        }
                    }
                }
                
                
                if (strLinkedFromLineNumber.Trim() != "")
                {
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LinkedFromLineNumber", "");
                    xmldocNode.text = strLinkedFromLineNumber;
                    xmldocHeaderNode.appendChild(xmldocNode);
                }
                
                returnValue = true;
            }
            catch
            {
                returnValue = false;
                
            }
            
            return returnValue;
        }
        // Apr 02, 2009 Nicolette end

        //  ; create Basket XML for BreakPoint integration based on basket
        public bool Create_Basket_XML(short Index, bool boolStackSale)
        {
            bool returnValue = false;

            MSXML2.IXMLDOMProcessingInstruction pi = default(MSXML2.IXMLDOMProcessingInstruction);
            xmldocRequest = new MSXML2.DOMDocument60();

            try
            {
                pi = xmldocRequest.createProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
                xmldocRequest.appendChild(pi);

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FCSCommand", "");
                xmldocRequestRoot = xmldocRequest.appendChild(xmldocNode);

                // add "Basket" tag
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Basket", "");
                xmldocAttribute = xmldocRequest.createAttribute("type");
                xmldocAttribute.value = "Create";
                xmldocNode.attributes.setNamedItem(xmldocAttribute);
                xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);

                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//Basket");
                if (xmldocNodeList.length == 0)
                {
                    // If tag <Basket> doesn't exist, create the node
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Basket", "");
                    xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
                }
                else
                {
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }

                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BasketDetail", "");
                xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);

                xmldocNodeList = xmldocRequest.documentElement.selectNodes("//BasketDetail");
                if (xmldocNodeList.length == 0)
                {
                    // If tag <BasketDetail> doesn't exist, create the node
                    xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BasketDetail", "");
                    xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
                }
                else
                {
                    xmldocHeaderNode = xmldocNodeList.nextNode();
                }

                // Add Tag <BasketID>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BasketID", "");
                if (boolStackSale)
                {
                    xmldocNode.text = Convert.ToString(Variables.gBasket[Index].stackBaskID);
                }
                else
                {
                    xmldocNode.text = Convert.ToString(Variables.gBasket[Index].currBaskID);
                }
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <PumpID>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PumpID", "");
                xmldocNode.text = (Index).ToString();
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <InvoiceID>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "InvoiceID", "");
                xmldocNode.text = (Chaps_Main.SA.Sale_Num).ToString();
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <InvoiceType>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "InvoiceType", "");
                xmldocNode.text = "SALE"; 
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <PayType>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PayType", "");
                xmldocNode.text = "Cash";
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <Grade>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Grade", "");
                if (boolStackSale)
                {
                    xmldocNode.text = Strings.Trim(Convert.ToString(Variables.Pump[Index].Stock_Code[Variables.gBasket[Index].posIDStack]));
                }
                else
                {
                    xmldocNode.text = Strings.Trim(Convert.ToString(Variables.Pump[Index].Stock_Code[Variables.gBasket[Index].PosIDCurr]));
                }
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <Amount>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Amount", "");
                if (boolStackSale)
                {
                    xmldocNode.text = string.Format("#0.00", Variables.gBasket[Index].AmountStack);
                }
                else
                {
                    xmldocNode.text = string.Format("#0.00", Variables.gBasket[Index].AmountCurrent);
                }
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <Volume>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Volume", "");
                if (boolStackSale)
                {
                    xmldocNode.text = (Variables.gBasket[Index].VolumeStack).ToString();
                }
                else
                {
                    xmldocNode.text = (Variables.gBasket[Index].VolumeCurrent).ToString();
                }
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                // Add Tag <UnitPrice>
                xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "UnitPrice", "");
                if (boolStackSale)
                {
                    xmldocNode.text = string.Format("#,##0.000", Variables.gBasket[Index].UPStack);
                }
                else
                {
                    xmldocNode.text = string.Format("#,##0.000", Variables.gBasket[Index].UPCurrent);
                }
                xmldocNode = xmldocHeaderNode.appendChild(xmldocNode);

                returnValue = true;
                mvarXMLString = Convert.ToString(xmldocRequest.xml);
                WriteToLog("created XML is " + xmldocRequest.xml);

                xmldocRequest = null;
            }
            catch
            {
                returnValue = false;

                xmldocRequest = null;
                mvarXMLString = "";
                WriteToLog("XML cannot be created. Error is " + Information.Err().Description + " in Create_Basket_XML");

            }

            return returnValue;
        }

        // dll
        public int Read_RTVP_TimeOut()
        {
            int returnValue = 0;

            int TimeOut = 0;

            try
            {
                xmldocResponse = new MSXML2.DOMDocument60();
                xmldocResponse.load("C:\\Program Files\\RTVP\\RTVP POS Service\\bin\\RTVP.POSService.config");

                xmldocNodeList = xmldocResponse.documentElement.selectNodes("//WebServiceTimeout");
                if (xmldocNodeList.length > 0)
                {
                    if (Information.IsNumeric(Strings.UCase(xmldocNodeList[0].text).Trim()))
                    {
                        TimeOut = int.Parse(xmldocNodeList[0].text);
                    }
                    else
                    {
                        TimeOut = 0;
                    }
                }
                else
                {
                    TimeOut = 0;
                }

                returnValue = TimeOut;
            }
            catch
            {
                returnValue = 0;

            }

            return returnValue;
        }


        /// <summary>
        /// method to analyse the responce of the carwash server in case of 
        /// cancel carwash code or check the validity of the code 
        /// </summary>
        /// <param name="responseXml"></param>
        /// <returns></returns>
        public bool AnalyzeCarwashResponse(string responseXml)
        {
            Result = "";
            errMsg = "";
            validity = "";

            try
            {
                responseXml = responseXml.Substring(6, responseXml.Length - 7); //get rid of header information
                xmldocResponse = new MSXML2.DOMDocument60();
                xmldocResponse.loadXML(responseXml);

                if (!xmldocResponse.documentElement.hasChildNodes())
                {
                    return false;
                }

                if (xmldocResponse.documentElement.firstChild.nodeName == "CheckCarwashCodeResp")
                {
                    xmldocNodeList = xmldocResponse.documentElement.selectNodes("//Result");
                    if (xmldocNodeList.length > 0)
                    {
                        Result = System.Convert.ToString(xmldocNodeList[0].nodeTypedValue);
                        if (Result == "ERR")
                        {
                            xmldocNodeList = xmldocResponse.documentElement.selectNodes("//ErrMsg");

                            errMsg = System.Convert.ToString(xmldocNodeList[0].nodeTypedValue);
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    xmldocNodeList = xmldocResponse.documentElement.selectNodes("//Validity");
                    if (xmldocNodeList.length > 0)
                    {

                        validity = System.Convert.ToString(xmldocNodeList[0].nodeTypedValue);
                    }
                    else
                    {
                        return false;
                    }
                }
                
                // Add code here to set the price of the carwash stock from server

                else if (xmldocResponse.documentElement.firstChild.nodeName == "CancelCarwashCodeResp") //xmldocResponse.documentElement.childNodes(0).baseName == "CancelCarwashCodeResp"
                {
                    xmldocNodeList = xmldocResponse.documentElement.selectNodes("//Result");
                    if (xmldocNodeList.length > 0)
                    {
                        Result = Convert.ToString(xmldocNodeList[0].nodeTypedValue);
                        if (Result == "ERR")
                        {
                            xmldocNodeList = xmldocResponse.documentElement.selectNodes("//ErrMsg");
                            errMsg = Convert.ToString(xmldocNodeList[0].nodeTypedValue);
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                
            }
            catch(Exception ex)
            {
                WriteToLog("AnalyzeCarwashResponse in XML.cs  is throwing this exception" + ex.ToString());
                return false;
            }
            return true;
            
            
        }


        /// <summary>
        /// method to analyse the responce of carwash server in case of the 
        /// carwash product sale 
        /// </summary>
        /// <param name="responseXml"></param>
        /// <param name="stockCodeArr"></param>
        /// <returns></returns>
        public bool AnalyzeCarwashCodeResponse(string responseXml, short[] stockCodeArr)
        {
            Result = "";
            errMsg = "";

            try
            {
                responseXml = responseXml.Substring(6, responseXml.Length - 7); //get rid of header information
                xmldocResponse = new MSXML2.DOMDocument60();
                xmldocResponse.loadXML(responseXml);

                if (!xmldocResponse.documentElement.hasChildNodes())
                {
                    return false;
                }
               // short Index = 0;
                if (xmldocResponse.documentElement.firstChild.nodeName == "GetCarwashCodeResp")
                {
                    
                    xmldocNodeList = xmldocResponse.documentElement.selectNodes("//Result");
                    if (xmldocNodeList.length > 0)
                    {
                        Result = System.Convert.ToString(xmldocNodeList[0].nodeTypedValue);
                        if (Result == "ERR")
                        {
                            xmldocNodeList = xmldocResponse.documentElement.selectNodes("//ErrMsg");
                            errMsg = System.Convert.ToString(xmldocNodeList[0].nodeTypedValue);
                           // returnValue = true;
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    xmldocNodeList = xmldocResponse.documentElement.selectNodes("//InvoiceID");

                    xmldocNodeList = xmldocResponse.documentElement.selectNodes("//Data");
                    short index = 0;
                    short length = 0;
                    var expiryDate = default(DateTime);
                    var codeTemp = "";
                    short intValidFor = 0;
                    var i = (short)0; 
                    var j = (short)xmldocNodeList.length;
                    Chaps_Main.SA.CarwashReceipt = "************************************" + "\r\n";
                    while (i < j)
                    {
                        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//Product");
                        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//Code");
                        codeTemp = Convert.ToString(xmldocNodeList[i].nodeTypedValue);
                        xmldocNodeList = xmldocResponse.documentElement.selectNodes("//ValidFor");
                        intValidFor = Convert.ToInt16(xmldocNodeList[i].nodeTypedValue);
                        expiryDate = DateAndTime.DateAdd(Microsoft.VisualBasic.DateInterval.Day, intValidFor, DateAndTime.Today);
                        var expiry = expiryDate.Date.ToString("MM-dd-yyyy");
                        expiry = expiry.Replace("-", "/");
                        index = stockCodeArr[i];
                        Chaps_Main.SA.Sale_Lines[index].CarwashCode = codeTemp;
                        length = (short)(14 + Strings.Len(Chaps_Main.SA.Sale_Lines[index].Description));
                        length = (short)((double)(40 - length) / 4); 
                        Chaps_Main.SA.CarwashReceipt = Chaps_Main.SA.CarwashReceipt + "CARWASH TYPE: ";
                        Chaps_Main.SA.CarwashReceipt = Chaps_Main.SA.CarwashReceipt + Strings.UCase(Chaps_Main.SA.Sale_Lines[index].Description) + "\r\n";
                        Chaps_Main.SA.CarwashReceipt = Chaps_Main.SA.CarwashReceipt + "CARWASH CODE: " + codeTemp + "\r\n"; 
                        Chaps_Main.SA.CarwashReceipt = Chaps_Main.SA.CarwashReceipt + "EXPIRY DATE : " + expiry + "\r\n";
                        i++;
                    }
                    Chaps_Main.SA.CarwashReceipt = Chaps_Main.SA.CarwashReceipt + "************************************" + "\r\n";
                }
                
            }
            catch(Exception ex)
            {
                WriteToLog("AnalyzeCarwashCodeResponse method in XML.cs is throwing this exception" + ex.ToString());
                return false;
            }
            return true;
            
        }


        //public bool AppendTenderInfo(string strTransactionLineAttribute, string strTenderCode, string strTenderSubCode, string strISOPrefix, string strLoyaltyRewardID, string strTenderAmount, string strChangeFlagAttribute)
        //{
        //    bool returnValue = false;
        //    //'
        //    string strLineNumber = "";
        //    //'
        //    try
        //    {
        //        //'
        //        if ((xmldocRequest == null) == true)
        //        {
        //            returnValue = false;
        //            return returnValue;
        //        }
        //        //'
        //        if ((xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
        //        {
        //            //'
        //            returnValue = false;
        //            return returnValue;
        //        }
        //        //'
        //        ///    Set xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData")
        //        ///    If xmldocNodeList.Length = 0 Then
        //        ///        ''Tag <TransactionData> not exist, creating the node
        //        ///        Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "TransactionData", "")
        //        ///        Set xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode)
        //        ///    Else
        //        ///        Set xmldocHeaderNode = xmldocNodeList.nextNode
        //        ///    End If
        //        ///    ''
        //        ///    Set xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup")
        //        ///    If xmldocNodeList.Length = 0 Then
        //        ///        ''Tag <TransactionDetailGroup> not exist, creating the node
        //        ///        Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "TransactionDetailGroup", "")
        //        ///        Set xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode)
        //        ///    Else
        //        ///        ''Tag <TransactionDetailGroup> exists
        //        ///        Set xmldocHeaderNode = xmldocNodeList.nextNode
        //        ///    End If
        //        ///    ''
        //        ///    Set xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine")
        //        ///    If xmldocNodeList.Length = 0 Then
        //        ///        strLineNumber = "1"
        //        ///    Else
        //        ///        strLineNumber = Trim$(CStr(xmldocNodeList.Length + 1))
        //        ///    End If
        //        ///    ''
        //        ///    ''Create Tag <TransactionLine>
        //        ///    Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "TransactionLine", "")
        //        ///    Set xmldocAttribute = xmldocRequest.createAttribute("status")
        //        ///    Select Case UCase(Trim$(strTransactionLineAttribute))
        //        ///        Case "VOID", "RETURN"
        //        ///            xmldocAttribute.Value = strTransactionLineAttribute
        //        ///        Case Else
        //        ///            xmldocAttribute.Value = "normal"
        //        ///    End Select
        //        ///    xmldocNode.Attributes.setNamedItem xmldocAttribute
        //        ///    Call xmldocHeaderNode.appendChild(xmldocNode)
        //        ///    ''
        //        ///    ''Create Tag <LineNumber>
        //        ///    Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "LineNumber", "")
        //        ///    xmldocNode.Text = strLineNumber
        //        ///    Call xmldocHeaderNode.appendChild(xmldocNode)
        //        ///    ''
        //        xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionData");
        //        if (xmldocNodeList.length == 0)
        //        {
        //            //'Tag <TransactionData> not exist, creating the node
        //            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
        //            xmldocHeaderNode = xmldocRequest.documentElement.appendChild(xmldocNode);
        //        }
        //        else
        //        {
        //            xmldocHeaderNode = xmldocNodeList.nextNode();
        //        }
        //        //'
        //        xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
        //        if (xmldocNodeList.length == 0)
        //        {
        //            //'Tag <TransactionDetailGroup> not exist, creating the node
        //            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
        //            xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
        //        }
        //        else
        //        {
        //            //'Tag <TransactionDetailGroup> exists
        //            xmldocHeaderNode = xmldocNodeList.nextNode();
        //        }
        //        //'
        //        xmldocNodeList = xmldocRequest.documentElement.selectNodes("//TransactionLine");
        //        if (xmldocNodeList.length == 0)
        //        {
        //            strLineNumber = "1";
        //        }
        //        else
        //        {
        //            strLineNumber = (xmldocNodeList.length + 1).ToString().Trim();
        //        }
        //        //'
        //        //'Create Tag <TransactionLine>
        //        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
        //        xmldocAttribute = xmldocRequest.createAttribute("status");
        //        switch (strTransactionLineAttribute.Trim().ToUpper())
        //        {
        //            case "VOID":
        //            case "RETURN":
        //                xmldocAttribute.value = strTransactionLineAttribute;
        //                break;
        //            default:
        //                xmldocAttribute.value = "normal";
        //                break;
        //        }
        //        xmldocNode.attributes.setNamedItem(xmldocAttribute);
        //        ///'              Call xmldocHeaderNode.appendChild(xmldocNode)
        //        xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
        //        //'
        //        //'Create Tag <LineNumber>
        //        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
        //        xmldocNode.text = strLineNumber;
        //        xmldocHeaderNode.appendChild(xmldocNode);

        //        //'Create Tag <TenderInfo>
        //        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderInfo", "");
        //        xmldocHeaderNode = xmldocHeaderNode.appendChild(xmldocNode);
        //        //'
        //        //'Create Tag <TenderCode>
        //        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderCode", "");
        //        xmldocNode.text = strTenderCode;
        //        xmldocHeaderNode.appendChild(xmldocNode);
        //        //'
        //        //'Create Tag <TenderSubCode>
        //        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderSubCode", "");
        //        xmldocNode.text = strTenderSubCode;
        //        xmldocHeaderNode.appendChild(xmldocNode);
        //        //'
        //        //'Create Tag <ISOPrefix>
        //        if (strISOPrefix.Trim() != "")
        //        {
        //            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ISOPrefix", "");
        //            xmldocNode.text = strISOPrefix;
        //            xmldocHeaderNode.appendChild(xmldocNode);
        //        }
        //        //'
        //        //'Create Tag <LoyaltyRewardID>
        //        if (strLoyaltyRewardID.Trim() != "")
        //        {
        //            xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
        //            xmldocNode.text = strLoyaltyRewardID;
        //            xmldocHeaderNode.appendChild(xmldocNode);
        //        }
        //        //'
        //        //'Create Tag <TenderAmount>
        //        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderAmount", "");
        //        xmldocNode.text = strTenderAmount;
        //        xmldocHeaderNode.appendChild(xmldocNode);
        //        //'
        //        //'Create Tag <ChangeFlag>
        //        xmldocNode = xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ChangeFlag", "");
        //        xmldocAttribute = xmldocRequest.createAttribute("value");
        //        if (strChangeFlagAttribute.Trim().ToUpper() == "YES")
        //        {
        //            xmldocAttribute.value = strChangeFlagAttribute;
        //        }
        //        else
        //        {
        //            xmldocAttribute.value = "no";
        //        }
        //        xmldocNode.attributes.setNamedItem(xmldocAttribute);
        //        xmldocHeaderNode.appendChild(xmldocNode);
        //        //'
        //        returnValue = true;
        //    }
        //    catch
        //    {
        //        returnValue = false;
        //    }
        //    //'
        //    return returnValue;
        //}
        //'
        //'
        //   end
    }
}