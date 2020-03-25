using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using System.Net.Http;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
   public  class XMLManager:ManagerBase,IXMLManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ITenderService _tenderService;
       private readonly ITenderManager _tenderManager;
        public XMLManager(IPolicyManager policyManager,ITenderService tenderService, ITenderManager tenderManager)
        {

            _policyManager = policyManager;
            _tenderService = tenderService;
            _tenderManager = tenderManager;
        }


        public bool FormatGetPointStatusRequestXML(string strLoyaltyID, string strEntryMethod,ref XML xml)
        {
           // var xml = new XML(_policyManager);
            bool returnValue = false;
            switch (strEntryMethod.Trim().ToUpper())
            {
                case "SCAN":
                   xml.entryMethod = "scan";
                    break;
                case "RFID":
                   xml.entryMethod = "RFID";
                    break;
                case "MANUAL":
                   xml.entryMethod = "manual";
                    break;
                case "OTHER":
                   xml.entryMethod = "other";
                    break;
                default:
                   xml.entryMethod = "swipe";
                    break;
            }

            try { 
           xml.xmldocRequest = new MSXML2.DOMDocument60();
           xml.xmldocNode =xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "GetRewardsRequest", "");
           xml.xmldocRequestRoot =xml.xmldocRequest.appendChild(xml.xmldocNode);

            if (InsertRequestHeader(ref xml) == false)
            {
                returnValue = false;
                return returnValue;
            }

           xml.xmldocNode =xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyID", "");
           xml.xmldocAttribute =xml.xmldocRequest.createAttribute("entryMethod");
           xml.xmldocAttribute.value =xml.entryMethod;
           xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
           xml.xmldocNode.text = strLoyaltyID;
           xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);



                returnValue = true;
        }      
        catch
			{


                WriteTokickBackLogFile("Cannot create XML GetRewardsRequest Request .");

                WriteTokickBackLogFile("Customer " + xml.Sale.Customer.Name + " will not earn points with card " + xml.Sale.Customer.PointCardNum);

                returnValue = false;
			}
    //'

   

            return returnValue;
        }

      

        public bool FormatFinalizeRewardsRequestXML(string strLoyaltyID, string strEntryMethod, string strPointsRedeemed, string strFinalTransTotal, string strTrainingModeFlag, string strOutsideSalesFlag, string strChoiceName, string strChoiceValue, string strCashierID, string strTillID, string strEventStartDate, string strEventStartTime, string strEventEndDate, string strEventEndTime,ref XML xml,ref Sale sale)
        {
         //   var xml = new XML(_policyManager);
            bool returnValue = false;
            ErrorMessage error = null;
         //   if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            string FuelServiceLevel = "";
            string IsChange = "";
            string IsPrepay = "";

            try
            {

                xml.xmldocRequest = new MSXML2.DOMDocument60();
                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FinalizeRewardsRequest", "");
                xml.xmldocRequestRoot = xml.xmldocRequest.appendChild(xml.xmldocNode);

                if (InsertRequestHeader(ref xml) == false)
                {
                    returnValue = false;
                    return returnValue;
                }

                if (strLoyaltyID.Trim() != "")
                {

                    switch (strEntryMethod.Trim().ToUpper())
                    {
                        case "SCAN":
                            xml.entryMethod = "scan";
                            break;
                        case "RFID":
                            xml.entryMethod = "RFID";
                            break;
                        case "MANUAL":
                            xml.entryMethod = "manual";
                            break;
                        case "OTHER":
                            xml.entryMethod = "other";
                            break;
                        default:
                            xml.entryMethod = "swipe";
                            break;
                    }

                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyID", "");
                    xml.xmldocAttribute = xml.xmldocRequest.createAttribute("entryMethod");
                    xml.xmldocAttribute.value = xml.entryMethod;
                    xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                    xml.xmldocNode.text = strLoyaltyID;
                    xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);
                }


                if (Conversion.Val(strPointsRedeemed) > 0)
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PointsRedeemed", "");
                    xml.xmldocNode.text = strPointsRedeemed;
                    xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FinalTransTotal", "");
                xml.xmldocNode.text = strFinalTransTotal;
                xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                xml.xmldocHeaderNode = xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionHeader", "");
                xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                if (strTrainingModeFlag.Trim().ToUpper() == "YES")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TrainingModeFlag", "");
                    xml.xmldocAttribute = xml.xmldocRequest.createAttribute("value");
                    xml.xmldocAttribute.value = "yes";
                    xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "OutsideSalesFlag", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("value");
                if (strOutsideSalesFlag.Trim().ToUpper() == "YES")
                {
                    xml.xmldocAttribute.value = "yes";
                }
                else
                {
                    xml.xmldocAttribute.value = "no";
                }
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                switch (strChoiceName.Trim().ToUpper())
                {
                    case "KIOSKID":

                        xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "KioskID", "");
                        xml.xmldocNode.text = strChoiceValue;
                        xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                        break;
                    case "FUELPOSITIONID":

                        xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelPositionID", "");
                        xml.xmldocNode.text = strChoiceValue;
                        xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                        break;
                    default:

                        xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RegisterID", "");
                        xml.xmldocNode.text = strChoiceValue;
                        xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                        break;
                }


                if (strCashierID.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "CashierID", "");
                    xml.xmldocNode.text = strCashierID;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TillID", "");
                xml.xmldocNode.text = strTillID;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSTransactionID", "");
                xml.xmldocNode.text = xml.PosSequenceID;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BusinessPeriod", "");
                xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BusinessDate", "");
                xml.xmldocNode.text = xml.typeBusinessPeriod_Transaction.BusinessData;
                xml.xmldocParent.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PrimaryReportPeriod", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("interval");
                xml.xmldocAttribute.value = xml.typeBusinessPeriod_Transaction.PrimaryReportPeriod_Attribute;
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                xml.xmldocNode.text = xml.typeBusinessPeriod_Transaction.PrimaryReportPeriod_Text;
                xml.xmldocParent.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SecondaryReportPeriod", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("interval");
                xml.xmldocAttribute.value = xml.typeBusinessPeriod_Transaction.SecondaryReportPeriod_Attribute;
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                xml.xmldocNode.text = xml.typeBusinessPeriod_Transaction.SecondaryReportPeriod_Text;
                xml.xmldocParent.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BeginDate", "");
                xml.xmldocNode.text = xml.typeBusinessPeriod_Transaction.BeginDate;
                xml.xmldocParent.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "BeginTime", "");
                xml.xmldocNode.text = xml.typeBusinessPeriod_Transaction.BeginTime;
                xml.xmldocParent.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndDate", "");
                xml.xmldocNode.text = xml.typeBusinessPeriod_Transaction.EndDate;
                xml.xmldocParent.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EndTime", "");
                xml.xmldocNode.text = xml.typeBusinessPeriod_Transaction.EndTime;
                xml.xmldocParent.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventStartDate", "");
                xml.xmldocNode.text = strEventStartDate;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventStartTime", "");
                xml.xmldocNode.text = strEventStartTime;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                if (strEventEndDate.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventEndDate", "");
                    xml.xmldocNode.text = strEventEndDate;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                if (strEventEndTime.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EventEndTime", "");
                    xml.xmldocNode.text = strEventEndTime;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }

                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    xml.SL = tempLoopVar_SL;
                    //  : messages have been changed to use ItemLine for non fuel items and FuelItem for fuel items
                    if (xml.SL.ProductIsFuel)
                    {
                        IsPrepay = Convert.ToString(xml.SL.Prepay? "yes" : "no");
                        if (xml.SL.pumpID == 0)
                        {
                            FuelServiceLevel = "full";
                        }
                        else
                        {
                            FuelServiceLevel = Convert.ToString(((int)(Variables.gPumps.get_Pump(xml.SL.pumpID).LevelID) == 1) ? "self" : "full");
                        }
                        AppendFuelLineInfo("", IsPrepay, (xml.SL.GradeID).ToString(), (xml.SL.PositionID).ToString(), "", "", FuelServiceLevel, "", (xml.SL.price).ToString(), (xml.SL.Regular_Price).ToString(), (xml.SL.Quantity).ToString(), (xml.SL.Amount).ToString(), "liter", "", "", "", "1",ref xml);
                    }
                    else
                    {
                        AppendItemLineInfo("", "yes", "upcA", xml.SL.Stock_Code, "0", xml.SL.Dept, xml.SL.Sub_Dept, xml.SL.Sub_Detail, xml.SL.Description, "", (xml.SL.price).ToString(), (xml.SL.Regular_Price).ToString(), "1", (xml.SL.Quantity).ToString(), (xml.SL.Amount).ToString(), "", "", "", "", "",ref xml);

                    }
                    //   end
                }
                  //var tenders = _tenderService.GetAlltenders();
                //  var tender = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false,
                //  "", out errorMessage);

                //  var tenders = tender;
                //   add TenderInfo element to FinalizeRewardsRequest
                ErrorMessage errorMessage;
                var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", strCashierID, false,
                "", out errorMessage);

                if (!(tenders == null))
                {
                    foreach (Tender tempLoopVar_td in tenders)
                    {
                        xml.td = tempLoopVar_td;
                        if (xml.td.Amount_Used != 0)
                        {
                            IsChange = Convert.ToString((xml.Tenders.Tend_Totals.Change != 0 && xml.td.Tender_Name.ToUpper() == Strings.UCase(Convert.ToString(_policyManager.BASECURR))) ? "yes" : "no");
                            AppendTenderInfo("", xml.td.PCATSGroup, "generic", "", "", (xml.td.Amount_Used).ToString(), IsChange,ref xml);
                        }
                    }
                }
                //   end

                returnValue = true;
            }
            catch
            {

                WriteToLogFile("Cannot create XML FinalizeRewards Request .");
                WriteToLogFile("Customer " + Chaps_Main.SA.Customer.Name + " will not earn points with card " + Chaps_Main.SA.Customer.PointCardNum);

                returnValue = false;
            }
            var ref12= xml;
            return returnValue;
        }


        public bool Initialize(string strPosLoyaltyInterfaceVersion, string strVendorName, string strVendorModelVersion, string strPOSSequenceID, string strStoreLocationID, string strLoyaltyOfflineFlag,ref XML xml)
        {
            bool returnValue = false;
           // var xml = new XML(_policyManager);

            try
            {
                xml.PosLoyaltyInterfaceVersion = "1.0.0";
                xml.VendorName = "InfoNet-Tech";
                xml.VendorModelVersion = "Pos 3.00.01";
                xml.PosSequenceID = "";
                xml.LoyaltySequenceID = "";
               xml.StoreLocationID = "gasking";
                xml.LoyaltyOfflineFlag = "no"; //'InfoNet uses "no" for default
                                           //'OutsideSalesFlag = "no"         ''KickBack uses "yes" as default, while InfoNet uses "no" as default
                xml.rewardStatusType = "summary";
                xml.entryMethod = "swipe";
                xml.LoyaltyID = "";

                //'Set Default Value for BusinessPeriod for Transaction in Command "FinalizeReward"
                xml.typeBusinessPeriod_Transaction.BusinessData = DateTime.Today.ToString("yyyy-MM-dd");
                xml.typeBusinessPeriod_Transaction.PrimaryReportPeriod_Attribute = "day";
                xml.typeBusinessPeriod_Transaction.PrimaryReportPeriod_Text = "1001";
                xml.typeBusinessPeriod_Transaction.SecondaryReportPeriod_Attribute = "cashier";
                xml.typeBusinessPeriod_Transaction.SecondaryReportPeriod_Text = "0";
                xml.typeBusinessPeriod_Transaction.BeginDate = DateTime.Today.ToString("yyyy-MM-dd");
                xml.typeBusinessPeriod_Transaction.BeginTime = DateTime.Now.ToString("HH:mm:ss tt");
               xml. typeBusinessPeriod_Transaction.EndDate = "2100-01-01";
               xml. typeBusinessPeriod_Transaction.EndTime = "00:00:00";

                if (strPosLoyaltyInterfaceVersion.Trim() != "")
                {
                    xml.PosLoyaltyInterfaceVersion = strPosLoyaltyInterfaceVersion;
                }

                if (strVendorName.Trim() != "")
                {
                    xml.VendorName = strVendorName;
                }

                if (strVendorModelVersion.Trim() != "")
                {
                    xml.VendorModelVersion = strVendorModelVersion;
                }

                if (strPOSSequenceID.Trim() != "")
                {
                    xml.PosSequenceID = strPOSSequenceID;
                }

                if (strStoreLocationID.Trim() != "")
                {
                    xml.StoreLocationID = strStoreLocationID;
                }

                if (strLoyaltyOfflineFlag.Trim() != "")
                {
                    xml.LoyaltyOfflineFlag = strLoyaltyOfflineFlag;
                }

                returnValue = true;
            }
            catch
            {
                WriteTokickBackLogFile("Cannot initialize KickBackXML object");
                returnValue = false;

            }

            return returnValue;
        }

        public bool InsertRequestHeader(ref XML xml)
        {
            bool returnValue = false;

            try
            {


                if ((xml.xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }

                xml.xmldocRequestRoot = xml.xmldocRequest.documentElement;


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RequestHeader", "");
                xml.xmldocHeaderNode = xml.xmldocRequestRoot.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PosLoyaltyInterfaceVersion", "");
                xml.xmldocNode.text = xml.PosLoyaltyInterfaceVersion;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "VendorName", "");
                xml.xmldocNode.text = xml.VendorName;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "VendorModelVersion", "");
                xml.xmldocNode.text = xml.VendorModelVersion;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSSequenceID", "");
                if (xml.PosSequenceID.Trim() != "")
                {
                    xml.xmldocNode.text = xml.PosSequenceID;
                }
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltySequenceID", "");
                if (xml.LoyaltySequenceID.Trim() != "")
                {
                    xml.xmldocNode.text = xml.LoyaltySequenceID;
                }
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "StoreLocationID", "");
                xml.xmldocNode.text = xml.StoreLocationID;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyOfflineFlag", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("value");
                xml.xmldocAttribute.value = xml.LoyaltyOfflineFlag;

                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                returnValue = true;
            }
            catch
            {
                returnValue = false;

            }

            return returnValue;
        }



        public void AnalyseKickBackResponse(string ResponseXML,ref XML xml)
        {
            //var xml = new XML(_policyManager);
            string strValidResult;

            try
            {
                //'Init Properties for KickBack Response
                xml.ResponseValid = false;
               xml. ResponseCommand = "";
                xml.ResponseError = (short)(-1);
                xml.ResponseMessage = "";
                xml.ResponsePointsBalance = "";
                xml.ResponseCollectionRatio = "";
                //'
                //'Parser Processing
                //'    strValidResult = ValidateXML(ResponseXML, App.Path & "\PLXML-POSLoyaltyInterface.xsd")
                //'    If Trim$(strValidResult) <> "" Then
                //'        ''XML is invalid
                //'        AnalyseKickBackResponse = False
                //'        Exit Function
                //'    End If
                //'
                //'Analyse Detail Information Based on Each Command Type
               xml. xmldocResponse = new MSXML2.DOMDocument60();
                 xml.xmldocResponse.loadXML(ResponseXML);
              //  xml.xmldocResponse.loadXML(@"<GetRewardsResponse><ResponseHeader><POSLoyaltyInterfaceVersion>1.0.0</POSLoyaltyInterfaceVersion><VendorName>KickbackPoints</VendorName><VendorModelVersion>1.1</VendorModelVersion><POSSequenceID>817</POSSequenceID><LoyaltySequenceID>0</LoyaltySequenceID></ResponseHeader><LoyaltyIDValidFlag value='yes'/><LoyaltyIDRegistered value='yes'/><PointInfo><PointBalance>4</PointBalance><CollectionRatio>000000.010</CollectionRatio></PointInfo></GetRewardsResponse>");
                xml.ResponseValid = true;
                xml.ResponseCommand = System.Convert.ToString(xml.xmldocResponse.documentElement.nodeName);
                xml.ResponseError = (short)0;

                //'Check LoyaltyIDValidFlag and LoyaltyIDRegistered from KickBack
                if ((xml.ResponseCommand == "GetRewardsResponse") || (xml.ResponseCommand == "GetRewardStatusResponse") || (xml.ResponseCommand == "GetPointStatusResponse"))
                {

                    xml.xmldocNodeList = xml.xmldocResponse.documentElement.selectNodes("//LoyaltyIDValidFlag");
                    if (xml.xmldocNodeList.length > 0)
                    {
                        if ((xml.xmldocNodeList[0].attributes[0].text).ToUpper().Trim() == "YES")
                        {
                          
                            xml.boolLoyaltyIDValid = true;
                        }
                        else
                        {
                            xml.boolLoyaltyIDValid = false;
                        }
                    }
                    else
                    {
                        xml.boolLoyaltyIDValid = false;
                    }

                    // Apr 02, 2009 Nicolette added LoyaltyIDRegistered checking
                    xml.xmldocNodeList = xml.xmldocResponse.documentElement.selectNodes("//LoyaltyIDRegistered");
                    if (xml.xmldocNodeList.length > 0)
                    {
                        if ((xml.xmldocNodeList[0].attributes[0].text).ToUpper().Trim() == "YES")
                        {
                            xml.mvarLoyaltyIDRegistered = true;
                        }
                        else
                        {
                            xml.mvarLoyaltyIDRegistered = false;
                        }
                    }
                    else
                    {
                        xml.mvarLoyaltyIDRegistered = false;
                    }
                    // Apr 02, 2009 Nicolette end
                }

                //'check LoyaltySequenceID from KickBack
               xml. xmldocNodeList = xml.xmldocResponse.documentElement.selectNodes("//LoyaltySequenceID");
                if (xml.xmldocNodeList.length > 0)
                {
                    xml.LoyaltySequenceID = xml.xmldocNodeList[0].text;
                }
                //'
                //'Check whether PointBalance item available? If True, get PointBalance value
                //'Perform checking for all children for DocumentElement, not necessary and should be improved later
                xml.xmldocNodeList = xml.xmldocResponse.documentElement.selectNodes("//PointBalance");
                if (xml.xmldocNodeList.length > 0)
                {
                   xml. ResponsePointsBalance = xml.xmldocNodeList[0].text;
                    // Feb 18, 2009: Nicolette added to set the balance inside this class
                    // so the system doesn't have to check the balance outside
                    if (!(xml.Sale.Customer == null))
                    {
                      xml.Sale.Customer.Balance_Points = Conversion.Val(xml.ResponsePointsBalance);
                    }
                    // Feb 18, 2009: Nicolette end
                }
                //'
                //'Check whether CollectionRatio item available? If True, get CollectionRatio value
                //'Perform checking for all children for DocumentElement, not necessary and should be improved later
               xml. xmldocNodeList = xml.xmldocResponse.documentElement.selectNodes("//CollectionRatio");
                if (xml.xmldocNodeList.length > 0)
                {
                    xml.ResponseCollectionRatio = xml.xmldocNodeList[0].text;
                    // Feb 18, 2009: Nicolette added to set the exchange rate for the points inside this class
                    if (!(xml.Sale.Customer == null))
                    {
                        xml.Sale.Customer.Points_ExchangeRate = Conversion.Val(xml.ResponseCollectionRatio);
                    }
                    else
                    {
                        xml.Sale.Customer.Points_ExchangeRate = 1;
                    }
                    // Feb 18, 2009: Nicolette end

                }
                //'
                //'Check whether CustomerMessageData item available? If True, get CustomerMessageData info
                //'only perform checking within DocumentElement context
                //'Set xmldocNodeList = xmldocResponse.documentElement.selectNodes("CustomerMessageData")
                xml.xmldocNodeList = xml.xmldocResponse.documentElement.selectNodes("//ReceiptLine");
                if (xml.xmldocNodeList.length > 0)
                {
                    for (int i = 1; i <= xml.xmldocNodeList.length; i++)
                    {
                        xml.ResponseMessage = xml.ResponseMessage + xml.xmldocNodeList[i - 1].text + "\r\n";
                    }
                }
                //'
                //'check DisplayLine from KickBack
               xml. xmldocNodeList = xml.xmldocResponse.documentElement.selectNodes("//DisplayLine");
                if (xml.xmldocNodeList.length > 0)
                {
                   xml. Sale.Customer.DisplayLine = xml.xmldocNodeList[0].text;
                }

                XML.GetCustomerMessageData = xml.ResponseMessage;
            
                //'    AnalyseKickBackResponse = True
            }
            catch
            {

                xml.ResponseValid = false;
                //'    AnalyseKickBackResponse = False

                WriteTokickBackLogFile("Cannot analyse XML GetPointStatus Response");
            }
            //
        }

        public bool AppendFuelLineInfo(string strTransactionLineAttribute, string strFuelLineAttribute, string strFuelGradeID, string strFuelPositionID, string strPriceTierCode, string strTimeTierCode, string strServiceLevelCode, string strDescription, string strActualSalesPrice, string strRegularSellPrice, string strSalesQuantity, string strSalesAmount, string strSalesUOM, string strPromotion, string strDiscount, string strItemTax, string strPaymentSystemsProductCode,ref XML xml)
        {
           // var xml = new XML(_policyManager);
            bool returnValue = false;
            string strLineNumber = "";
            string[] strArrayItems = null;
            string[] strArrayFields = null;

            try
            {

                if ((xml. xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }

                if ((xml.xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xml.xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xml.xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
                {

                    returnValue = false;
                    return returnValue;
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xml.xmldocNodeList.length == 0)
                {

                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xml.xmldocHeaderNode = xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);
                }
                else
                {
                    xml.xmldocHeaderNode = xml.xmldocNodeList.nextNode();
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xml.xmldocNodeList.length == 0)
                {

                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }
                else
                {

                    xml.xmldocHeaderNode = xml.xmldocNodeList.nextNode();
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xml.xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xml.xmldocNodeList.length + 1).ToString().Trim();
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xml.xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xml.xmldocAttribute.value = "normal";
                        break;
                }
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);

                xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xml.xmldocNode.text = strLineNumber;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelLine", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("fuelPrepayFlag");
                if (strFuelLineAttribute.Trim().ToUpper() == "YES")
                {
                    xml.xmldocAttribute.value = "yes";
                }
                else
                {
                    xml.xmldocAttribute.value = "no";
                }
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelGradeID", "");
                xml.xmldocNode.text = strFuelGradeID;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "FuelPositionID", "");
                xml.xmldocNode.text = strFuelPositionID;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                if (strPriceTierCode.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceTierCode", "");
                    xml.xmldocNode.text = strPriceTierCode;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                if (strPriceTierCode.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TimeTierCode", "");
                    xml.xmldocNode.text = strTimeTierCode;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ServiceLevelCode", "");
                xml.xmldocNode.text = strServiceLevelCode;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                if (strDescription.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Description", "");
                    xml.xmldocNode.text = strDescription;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ActualSalesPrice", "");
                xml.xmldocNode.text = strActualSalesPrice;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RegularSellPrice", "");
                xml.xmldocNode.text = strRegularSellPrice;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesQuantity", "");
                xml.xmldocNode.text = strSalesQuantity;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesAmount", "");
                xml.xmldocNode.text = strSalesAmount;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                //   added SalesUOM tag

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesUOM", "");
                xml.xmldocNode.text = strSalesUOM;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                //   end


                if (strPromotion.Trim() != "")
                {
                    strArrayItems = (strPromotion + ";").Split(';');
                    for (int i = 0; i <= (strArrayItems.Length - 1); i++)
                    {

                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,,").Split(',');


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Promotion", "");
                            xml.xmldocAttribute = xml.xmldocRequest.createAttribute("status");
                            xml.xmldocAttribute.value = strArrayFields[0].Trim();
                            xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                            xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionID", "");
                            xml.xmldocNode.text = strArrayFields[1].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            if (strArrayFields[2].Trim() != "")
                            {
                                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                                xml.xmldocNode.text = strArrayFields[2].Trim();
                                xml.xmldocParent.appendChild(xml.xmldocNode);
                            }


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionAmount", "");
                            xml.xmldocNode.text = strArrayFields[3].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionReason", "");
                            xml.xmldocNode.text = strArrayFields[4].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);

                        }
                    }
                }


                if (strDiscount.Trim() != "")
                {
                    strArrayItems = (strDiscount + ";").Split(';');
                    for (int i = 0; i <= (strArrayItems.Length - 1); i++)
                    {

                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Discount", "");
                            xml.xmldocAttribute = xml.xmldocRequest.createAttribute("status");
                            xml.xmldocAttribute.value = strArrayFields[0].Trim();
                            xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                            xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountID", "");
                            xml.xmldocNode.text = strArrayFields[1].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountAmount", "");
                            xml.xmldocNode.text = strArrayFields[2].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountReason", "");
                            xml.xmldocNode.text = strArrayFields[3].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);

                        }
                    }
                }


                if (strItemTax.Trim() != "")
                {
                    strArrayItems = (strItemTax + ";").Split(';');
                    for (int i = 0; i <= (strArrayItems.Length - 1); i++)
                    {

                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemTax", "");
                            xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxLevelID", "");
                            xml.xmldocNode.text = strArrayFields[0].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            switch (strArrayFields[1].Trim().ToUpper())
                            {
                                case "TAXREFUNDEDAMOUNT":
                                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxRefundedAmount", "");
                                    break;
                                default:
                                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxCollectedAmount", "");
                                    break;
                            }
                            xml.xmldocNode.text = strArrayFields[2].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);

                        }
                    }
                }

                //   added PaymentSystemsProductCode tag

                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PaymentSystemsProductCode", "");
                xml.xmldocNode.text = strPaymentSystemsProductCode;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                // Nicolette, M ay 02, 2011 end

                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }

            return returnValue;
        }


        public bool AppendItemLineInfo(string strTransactionLineAttribute, string strItemLineAttribute, string strItemCode_POSCodeFormat, string strItemCode_POSCode, string strItemCode_POSCodeModifier, string strDepartment, string strSubDepartment, string strSubDetail, string strDescription, string strEntryMethod, string strActualSalesPrice, string strRegularSellPrice, string strSellingUnits, string strSalesQuantity, string strSalesAmount, string strPriceOverride, string strPromotion, string strDiscount, string strItemTax, string strLinkedFromLineNumber,ref XML xml)
        {
            //var xml = new XML(_policyManager);
            bool returnValue = false;
            
            string strLineNumber = "";
            string[] strArrayItems = null;
            string[] strArrayFields = null;

            try
            {

                if ((xml.xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }

                if ((xml.xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xml.xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xml.xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
                {

                    returnValue = false;
                    return returnValue;
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xml.xmldocNodeList.length == 0)
                {

                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xml.xmldocHeaderNode = xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);
                }
                else
                {
                    xml.xmldocHeaderNode = xml.xmldocNodeList.nextNode();
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xml.xmldocNodeList.length == 0)
                {

                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }
                else
                {

                    xml.xmldocHeaderNode = xml.xmldocNodeList.nextNode();
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xml.xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xml.xmldocNodeList.length + 1).ToString().Trim();
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xml.xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xml.xmldocAttribute.value = "normal";
                        break;
                }
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);

                xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xml.xmldocNode.text = strLineNumber;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemLine", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("discountable");
                if (strItemLineAttribute.Trim().ToUpper() == "YES")
                {
                    xml.xmldocAttribute.value = "yes";
                }
                else
                {
                    xml.xmldocAttribute.value = "no";
                }
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemCode", "");
                xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSCodeFormat", "");

                switch (strItemCode_POSCodeFormat.Trim().ToUpper())
                {
                    case "UPCA":
                        xml.xmldocNode.text = "upcA";
                        break;
                    case "UPCE":
                        xml.xmldocNode.text = "upcE";
                        break;
                    case "EAN8":
                        xml.xmldocNode.text = "ean8";
                        break;
                    case "EAN13":
                        xml.xmldocNode.text = "ean13";
                        break;
                    case "PLU":
                        xml.xmldocNode.text = "plu";
                        break;
                    case "GTIN":
                        xml.xmldocNode.text = "gtin";
                        break;
                    case "RSS14":
                        xml.xmldocNode.text = "rss14";
                        break;
                    default:
                        xml.xmldocNode.text = "none";
                        break;
                }
                xml.xmldocParent.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSCode", "");
                xml.xmldocNode.text = strItemCode_POSCode;
                xml.xmldocParent.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "POSCodeModifier", "");
                xml.xmldocNode.text = strItemCode_POSCodeModifier;
                xml.xmldocParent.appendChild(xml.xmldocNode);


                ///    Set xmldocNode = xmldocRequest.createNode(NODE_ELEMENT, "MerchandiseCode", "")
                ///    xmldocNode.Text = strMerchandiseCode
                ///    Call xmldocHeaderNode.appendChild(xmldocNode)


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Department", "");
                xml.xmldocNode.text = strDepartment;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SubDepartment", "");
                xml.xmldocNode.text = strSubDepartment;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SubDetail", "");
                xml.xmldocNode.text = strSubDetail;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                if (strDescription.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Description", "");
                    xml.xmldocNode.text = strDescription;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "EntryMethod", "");
                switch (strEntryMethod.Trim().ToUpper())
                {
                    case "MANUAL":
                        xml.xmldocNode.text = "manual";
                        break;
                    case "OTHER":
                        xml.xmldocNode.text = "other";
                        break;
                    default:
                        xml.xmldocNode.text = "scan";
                        break;
                }
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ActualSalesPrice", "");
                xml.xmldocNode.text = strActualSalesPrice;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "RegularSellPrice", "");
                xml.xmldocNode.text = strRegularSellPrice;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SellingUnits", "");
                xml.xmldocNode.text = strSellingUnits;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesQuantity", "");
                xml.xmldocNode.text = strSalesQuantity;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "SalesAmount", "");
                xml.xmldocNode.text = strSalesAmount;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                if (strPriceOverride.Trim() != "")
                {
                    strArrayFields = (strPriceOverride + ",").Split(',');


                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceOverride", "");
                    xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceOverridePrice", "");
                    xml.xmldocNode.text = strArrayFields[0].Trim();
                    xml.xmldocParent.appendChild(xml.xmldocNode);


                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PriceOverrideReason", "");
                    xml.xmldocNode.text = strArrayFields[1].Trim();
                    xml.xmldocParent.appendChild(xml.xmldocNode);

                }


                if (strPromotion.Trim() != "")
                {
                    strArrayItems = (strPromotion + ";").Split(';');
                    for (int i = 0; i <= (strArrayItems.Length - 1); i++)
                    {

                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,,").Split(',');


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Promotion", "");
                            xml.xmldocAttribute = xml.xmldocRequest.createAttribute("status");
                            xml.xmldocAttribute.value = strArrayFields[0].Trim();
                            xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                            xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionID", "");
                            xml.xmldocNode.text = strArrayFields[1].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            if (strArrayFields[2].Trim() != "")
                            {
                                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                                xml.xmldocNode.text = strArrayFields[2].Trim();
                                xml.xmldocParent.appendChild(xml.xmldocNode);
                            }


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionAmount", "");
                            xml.xmldocNode.text = strArrayFields[3].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "PromotionReason", "");
                            xml.xmldocNode.text = strArrayFields[4].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);

                        }
                    }
                }


                if (strDiscount.Trim() != "")
                {
                    strArrayItems = (strDiscount + ";").Split(';');
                    for (int i = 0; i <= (strArrayItems.Length - 1); i++)
                    {

                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "Discount", "");
                            xml.xmldocAttribute = xml.xmldocRequest.createAttribute("status");
                            xml.xmldocAttribute.value = strArrayFields[0].Trim();
                            xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                            xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountID", "");
                            xml.xmldocNode.text = strArrayFields[1].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountAmount", "");
                            xml.xmldocNode.text = strArrayFields[2].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "DiscountReason", "");
                            xml.xmldocNode.text = strArrayFields[3].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);

                        }
                    }
                }


                if (strItemTax.Trim() != "")
                {
                    strArrayItems = (strItemTax + ";").Split(';');
                    for (int i = 0; i <= (strArrayItems.Length - 1); i++)
                    {

                        if (strArrayItems[i].Trim() != "")
                        {
                            strArrayFields = (strArrayItems[i] + ",,,").Split(',');


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ItemTax", "");
                            xml.xmldocParent = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                            xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxLevelID", "");
                            xml.xmldocNode.text = strArrayFields[0].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);


                            switch (strArrayFields[1].Trim().ToUpper())
                            {
                                case "TAXREFUNDEDAMOUNT":
                                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxRefundedAmount", "");
                                    break;
                                default:
                                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TaxCollectedAmount", "");
                                    break;
                            }
                            xml.xmldocNode.text = strArrayFields[2].Trim();
                            xml.xmldocParent.appendChild(xml.xmldocNode);

                        }
                    }
                }


                if (strLinkedFromLineNumber.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LinkedFromLineNumber", "");
                    xml.xmldocNode.text = strLinkedFromLineNumber;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }

                returnValue = true;
            }
            catch
            {
                returnValue = false;

            }

            return returnValue;
        }


        public bool AppendTenderInfo(string strTransactionLineAttribute, string strTenderCode, string strTenderSubCode, string strISOPrefix, string strLoyaltyRewardID, string strTenderAmount, string strChangeFlagAttribute,ref XML xml)
        {
           // var xml = new XML(_policyManager);
            bool returnValue = false;

            string strLineNumber = "";

            try
            {

                if ((xml.xmldocRequest == null) == true)
                {
                    returnValue = false;
                    return returnValue;
                }

                if ((xml.xmldocRequest.documentElement.nodeName != "FinalizeRewardsRequest") && (xml.xmldocRequest.documentElement.nodeName != "GetRewardsRequest") && (xml.xmldocRequest.documentElement.nodeName != "GetCustomerMessagingRequest"))
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
                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionData");
                if (xml.xmldocNodeList.length == 0)
                {

                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionData", "");
                    xml.xmldocHeaderNode = xml.xmldocRequest.documentElement.appendChild(xml.xmldocNode);
                }
                else
                {
                    xml.xmldocHeaderNode = xml.xmldocNodeList.nextNode();
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionDetailGroup");
                if (xml.xmldocNodeList.length == 0)
                {

                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionDetailGroup", "");
                    xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }
                else
                {

                    xml.xmldocHeaderNode = xml.xmldocNodeList.nextNode();
                }

                xml.xmldocNodeList = xml.xmldocRequest.documentElement.selectNodes("//TransactionLine");
                if (xml.xmldocNodeList.length == 0)
                {
                    strLineNumber = "1";
                }
                else
                {
                    strLineNumber = (xml.xmldocNodeList.length + 1).ToString().Trim();
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TransactionLine", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("status");
                switch (strTransactionLineAttribute.Trim().ToUpper())
                {
                    case "VOID":
                    case "RETURN":
                        xml.xmldocAttribute.value = strTransactionLineAttribute;
                        break;
                    default:
                        xml.xmldocAttribute.value = "normal";
                        break;
                }
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);

                xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LineNumber", "");
                xml.xmldocNode.text = strLineNumber;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderInfo", "");
                xml.xmldocHeaderNode = xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderCode", "");
                xml.xmldocNode.text = strTenderCode;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderSubCode", "");
                xml.xmldocNode.text = strTenderSubCode;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                if (strISOPrefix.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ISOPrefix", "");
                    xml.xmldocNode.text = strISOPrefix;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                if (strLoyaltyRewardID.Trim() != "")
                {
                    xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "LoyaltyRewardID", "");
                    xml.xmldocNode.text = strLoyaltyRewardID;
                    xml.xmldocHeaderNode.appendChild(xml.xmldocNode);
                }


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "TenderAmount", "");
                xml.xmldocNode.text = strTenderAmount;
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);


                xml.xmldocNode = xml.xmldocRequest.createNode(MSXML2.tagDOMNodeType.NODE_ELEMENT, "ChangeFlag", "");
                xml.xmldocAttribute = xml.xmldocRequest.createAttribute("value");
                if (strChangeFlagAttribute.Trim().ToUpper() == "YES")
                {
                    xml.xmldocAttribute.value = strChangeFlagAttribute;
                }
                else
                {
                    xml.xmldocAttribute.value = "no";
                }
                xml.xmldocNode.attributes.setNamedItem(xml.xmldocAttribute);
                xml.xmldocHeaderNode.appendChild(xml.xmldocNode);

                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }

            return returnValue;
        }
    }
}
