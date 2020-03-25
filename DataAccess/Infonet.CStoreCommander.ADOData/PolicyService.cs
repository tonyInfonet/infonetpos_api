using ADODB;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    ///  Policy service
    /// </summary>
    public class PolicyService : DbService, IPolicyService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Method to load all policies
        /// </summary>
        /// <returns>List of policies</returns>
        public List<Policy> LoadAllPolicies()
        {
            List<Policy> result = new List<Policy>();
            var rsComp = GetRecords("select P_CLASS,Implemented,P_NAME,P_SET,P_VARTYPE from P_Comp", DataSource.CSCAdmin);

            var rsCompFields = ((dynamic)rsComp).Fields;
            while (!rsComp.EOF)
            {
                var policy = new Policy
                {
                    ClassName = CommonUtility.GetStringValue(rsCompFields["P_CLASS"].Value),
                    Implemented = CommonUtility.GetBooleanValue(rsCompFields["Implemented"].Value),
                    PolicyName = CommonUtility.GetStringValue(rsCompFields["P_NAME"].Value),
                    Value = CommonUtility.GetStringValue(rsCompFields["P_SET"].Value),
                    VarType = CommonUtility.GetStringValue(rsCompFields["P_VARTYPE"].Value)
                };
                result.Add(policy);
                rsComp.MoveNext();
            }
            return result;
        }

        /// <summary>
        /// Method to load all policy can be
        /// </summary>
        /// <returns>List of Policy can be</returns>
        public List<PolicyCanbe> LoadAllPolicyCanbe()
        {
            List<PolicyCanbe> result = new List<PolicyCanbe>();
            var rsCanbe = GetRecords("select * from P_Canbe", DataSource.CSCAdmin);

            var rsCanbeFields = ((dynamic)rsCanbe).Fields;
            while (!rsCanbe.EOF)
            {
                var policy = new PolicyCanbe
                {
                    CanBe = CommonUtility.GetStringValue(rsCanbeFields["P_Canbe"].Value),
                    PolicyName = CommonUtility.GetStringValue(rsCanbeFields["P_NAME"].Value),
                    Sequence = CommonUtility.GetIntergerValue(rsCanbeFields["P_Seq"].Value)
                };
                result.Add(policy);
                rsCanbe.MoveNext();
            }
            return result;
        }

        /// <summary>
        /// Method to load all set policies
        /// </summary>
        /// <returns>Lsit of set polices</returns>
        public List<PolicySet> LoadAllPolicySet()
        {
            List<PolicySet> result = new List<PolicySet>();
            var rsSet = GetRecords("select * from P_Set", DataSource.CSCAdmin);

            var rsSetFields = ((dynamic)rsSet).Fields;
            while (!rsSet.EOF)
            {
                var policy = new PolicySet
                {
                    Value = CommonUtility.GetStringValue(rsSetFields["P_VALUE"].Value),
                    PolicyName = CommonUtility.GetStringValue(rsSetFields["P_NAME"].Value),
                    Level = CommonUtility.GetStringValue(rsSetFields["P_LEVEL"].Value),
                    Set = CommonUtility.GetStringValue(rsSetFields["P_SET"].Value)
                };
                result.Add(policy);
                rsSet.MoveNext();
            }
            return result;
        }


        /// <summary>
        /// Set up Policy
        /// </summary>
        /// <param name="security"></param>
        public void SetUpPolicy(Security security)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PolicyService,SetUpPolicy,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            //  Pay_InsideCreditDebit
            var rsComp = GetRecords("select * from P_Comp", DataSource.CSCAdmin);

            rsComp.Find("P_NAME=\'PAY_INSIDE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);

            var fields = ((dynamic)rsComp).Fields;
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pay_InsideCreditDebit;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'MASK_CARDNO\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pay_InsideCreditDebit;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'SWIPE_CARD\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pay_InsideCreditDebit;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'CC_MODE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = true; // Me.Pay_InsideCreditDebit
                if (!security.Pay_InsideCreditDebit)
                {
                    fields["P_Set"].Value = "Cross-Ring";
                }
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'USE_PINPAD\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pay_InsideCreditDebit;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // Fleet Card
            rsComp.Find("P_NAME=\'FLEET_CARD\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Fleet_Card;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // ThirdParty Card
            rsComp.Find("P_NAME=\'THIRD_PARTY\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Fleet_Card;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // A/R
            rsComp.Find("P_NAME=\'AR\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_OR_LIMIT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_ARSALE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'CREDTERM\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'USE_ARCUST\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'HO_ARPROCESS\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R && security.Head_Office_Support;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_ARSALES\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_ARPRINT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_ARCLOSE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.A_and_R;
                rsComp.Update();
            }

            // LOYALTY
            rsComp.Find("P_NAME=\'LOYALTY\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'USE_LOYALTY\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOYAL_TYPE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOYAL_PRICE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOYAL_DISC\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOYAL_NAME\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOYAL_PPD\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOY-EXCLUDE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOYAL_LIMIT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOY_NOREDPO\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'LOYAL_PPU\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'SHOW_POINT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'ALLOW_CUR_PT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'GIVE_POINTS\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'VOL_POINTS\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'USE_CUST\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'CUST_DISC\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_CLSTATUS\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'PROD_DISC\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'OR_USER_DISC\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Loyalty;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // ENHANCED REPORTS
            rsComp.Find("P_NAME=\'E_REPORTS\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Enhanced_reports;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // MULTI POS
            rsComp.Find("P_NAME=\'MULTI_POS\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Multi_POS_Support;
                rsComp.Update();
            }


            //----------------------------------------------------------
            // HEAD OFFICE SUPPORT
            rsComp.Find("P_NAME=\'HO\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Head_Office_Support;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_SEND\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Head_Office_Support;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_GET\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Head_Office_Support;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // PUMP CONTROL
            rsComp.Find("P_NAME=\'PUMP_CONTROL\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pump_Control;
                rsComp.Update();
            }
            rsComp.Find("P_NAME=\'USE_FUEL\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pump_Control;
                rsComp.Update();
            }

            //  Making these pricechange policies  true irrespective of fuel control for posonly (pump control disabled from security)- Now we need to allow price change for posonly
            rsComp.Find("P_NAME=\'U_CHGFPRICE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = true; // Me.Pump_Control
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_FUELSETUP\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = true; //Me.Pump_Control
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_FUELGP\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = true; //Me.Pump_Control
                rsComp.Update();
            }


            rsComp.Find("P_NAME=\'FUEL_UM\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = true; //Me.Pump_Control
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'FUEL_GP\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = true; //Me.Pump_Control
                rsComp.Update();
            }
            //
            //----------------------------------------------------------
            // PAY AT THE PUMP CREDIT
            rsComp.Find("P_NAME=\'PUMP_CREDIT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pay_Pump_Credit;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // PAY AT THE PUMP DEBIT
            rsComp.Find("P_NAME=\'PUMP_DEBIT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Pay_Pump_Debit;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // One Reader To more Pumps
            rsComp.Find("P_NAME=\'MULTI_PUMP\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Multi_Pumps;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // Fuel Price Display
            rsComp.Find("P_NAME=\'PRICEDISPLAY\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.PRICEDISPLAY;
                rsComp.Update();
            }

            //----------------------------------------------------------
            // FUEL MANAGEMENT
            rsComp.Find("P_NAME=\'FUEL_MGMT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Fuel_Management;
                rsComp.Update();
            }

            rsComp.Find("P_NAME=\'U_FUELMGMT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.Fuel_Management;
                rsComp.Update();
            }

            //-------------------------------
            // Policies without feature link
            rsComp.Find("P_NAME=\'GROUP_PRTY\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.BackOfficeVersion == "Full";
                rsComp.Update();
            }

            // Policy for Tax Exempt
            rsComp.Find("P_NAME=\'TAX_EXEMPT\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.TAX_EXEMPT;
                rsComp.Update();
            }

            // Policy for Promotion
            rsComp.Find("P_NAME=\'PROMO_SALE\'", SearchDirection: SearchDirectionEnum.adSearchForward, Start: 1);
            if (!rsComp.EOF)
            {
                fields["Implemented"].Value = security.PROMO_SALE;
                rsComp.Update();
            }
            _performancelog.Debug($"End,PolicyService,SetUpPolicy,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Add Policy
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public dynamic AddPolicy(string pName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PolicyService,AddPolicy,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            dynamic returnValue = default(dynamic);

            double maxSeq;

            var rsC = GetRecords("SELECT Max(P_Seq) AS MaxSeq FROM P_Comp", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly);
            var rsCFields = ((dynamic)rsC).Fields;

            if (!rsC.EOF)
            {
                maxSeq = Convert.ToDouble(rsCFields["maxSeq"].Value);
            }
            else
            {
                maxSeq = 10;
            }

            rsC = GetRecords("select * from P_Comp", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly);

            rsC.MoveLast();
            var seq = maxSeq;
            rsC.AddNew();
            rsCFields = ((dynamic)rsC).Fields;
            rsCFields["P_SEQ"].Value = seq + 10;
            rsCFields["Implemented"].Value = true;
            switch (pName)
            {
                case "PRICE_TRACK":
                    rsCFields["P_CLASS"].Value = "PRICE";
                    rsCFields["P_Name"].Value = "PRICE_TRACK";
                    rsCFields["P_Desc"].Value = "Track Price Change?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "EOD_GROUP":
                    rsCFields["P_CLASS"].Value = "TILL_EOD";
                    rsCFields["P_Name"].Value = "EOD_GROUP";
                    rsCFields["P_Desc"].Value = "Allow grouping by \'EOD Groups\' in the Till Close Report?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;


                case "USE_OVERRIDE":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT"; //"TILL_SAL"  ''since added TAXEXEMPT class
                    rsCFields["P_Name"].Value = "USE_OVERRIDE";
                    rsCFields["P_Desc"].Value = "Use OverRide code in Tax Exempt?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                case "TE_BREAKDOWN":
                    rsCFields["P_CLASS"].Value = "TILL_EOD";
                    rsCFields["P_Name"].Value = "TE_BREAKDOWN";
                    rsCFields["P_Desc"].Value = "Break down Tax Exempt in the Till Close Report?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;



                case "BANKSYSTEM":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "BANKSYSTEM";
                    rsCFields["P_Desc"].Value = "Bank system";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Global,Moneris,None}";
                    rsCFields["P_Set"].Value = "Global";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Global";
                    break;


                //2013 10 23 Reji - Allow Manual credit card processing & pinpad type
                case "MANUALCCARD":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "MANUALCCARD";
                    rsCFields["P_Desc"].Value = "Allow Manual credit card processing ? ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = "Global";
                    break;

                case "PINPADTYPE":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "PINPADTYPE";
                    rsCFields["P_Desc"].Value = "Pinpad Type";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Vx810, iPP320}";
                    rsCFields["P_Set"].Value = "iPP320";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Vx810";
                    break;
                //2013 10 23 Reji - End


                case "COUPONMSG":
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; //Till Sales Option
                    rsCFields["P_Name"].Value = "COUPONMSG";
                    rsCFields["P_Desc"].Value = "Display Fuel Coupon Message?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "CUPNTHREHLD":
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; //Till Sales Option
                    rsCFields["P_Name"].Value = "CUPNTHREHLD";
                    rsCFields["P_Desc"].Value = "Fuel Coupon message threshold value";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-1000}";
                    rsCFields["P_Set"].Value = "20";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 20;
                    break;

                case "COUPONTYPE":
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; //Till Sales Option
                    rsCFields["P_Name"].Value = "COUPONTYPE";
                    rsCFields["P_Desc"].Value = "Fuel Coupon Type";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Fuel Coupon,Fuel Stamp,Litre Log}";
                    rsCFields["P_Set"].Value = "Fuel Stamp";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Fuel Stamp";
                    break;

                //  - missing the following 2 policies from clients database
                case "TAX_EXEMPT":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "TAX_EXEMPT";
                    rsCFields["P_Desc"].Value = "Supports Tax Exempt?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "PROMO_SALE":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "PROMO_SALE";
                    rsCFields["P_Desc"].Value = "Supports Promotional Sale?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //


                case "ALLOWPOSTPAY": //enable or disable payinside
                    rsCFields["P_CLASS"].Value = "FUEL";
                    rsCFields["P_Name"].Value = "ALLOWPOSTPAY";
                    rsCFields["P_Desc"].Value = "Allow Post Pay?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;



                case "ALLOWPREPAY": //enable or disable payinside
                    rsCFields["P_CLASS"].Value = "FUEL";
                    rsCFields["P_Name"].Value = "ALLOWPREPAY";
                    rsCFields["P_Desc"].Value = "Allow Prepay?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;


                //Behrooz Jul-25-05
                case "VERSION":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "VERSION";
                    rsCFields["P_Desc"].Value = "Version";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{US,Canadian}";
                    rsCFields["P_Set"].Value = "Canadian";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Canadian";
                    break;


                case "DEBITSWIPE":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "DEBITSWIPE";
                    rsCFields["P_Desc"].Value = "Use Debit Swipe?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "MAXDBSWIPE":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "MAXDBSWIPE";
                    rsCFields["P_Desc"].Value = "Maximum Number of Debit Swipes.";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-10}";
                    rsCFields["P_Set"].Value = "2";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 2;
                    break;



                case "ALLOWMANUAL":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "ALLOWMANUAL";
                    rsCFields["P_Desc"].Value = "Allow Manual Fuel Sale?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;



                case "U_MANUALF":
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "U_MANUALF";
                    rsCFields["P_Desc"].Value = "User can sell Manual Fuel Sale?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    AddPolicyCanBe("U_ManualF", "USER", "UGROUP", "COMPANY");
                    break;



                case "EOD_CLOSE":
                    rsCFields["P_CLASS"].Value = "TILL_EOD"; //Till End of Day Options
                    rsCFields["P_Name"].Value = "EOD_CLOSE";
                    rsCFields["P_Desc"].Value = "Close batch when Till Close?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "DIP_INPUT":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "DIP_INPUT";
                    rsCFields["P_Desc"].Value = "Support Dip Input?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "DIPINPUTTIME":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "DIPINPUTTIME";
                    rsCFields["P_Desc"].Value = "Dip Input Time";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "TM(12:00:00 AM,12:00:00 PM)";
                    rsCFields["P_Set"].Value = "10:00:00 PM";
                    rsCFields["P_VARTYPE"].Value = "T";

                    returnValue = DateTime.Parse("10:00:00 PM");
                    break;



                case "LASTSHIFT":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "LASTSHIFT";
                    rsCFields["P_Desc"].Value = "Shift Number for Automatic Totalizer Reading";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{0-5}";
                    rsCFields["P_Set"].Value = "0";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 0;
                    break;


                case "TAX_COMP":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "TAX_COMP";
                    rsCFields["P_Desc"].Value = "Compute tax on taxes?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                //  
                case "COST_TYPE":
                    rsCFields["P_CLASS"].Value = "STOCK";
                    rsCFields["P_Name"].Value = "COST_TYPE";
                    rsCFields["P_Desc"].Value = "Type of cost used_____";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Standard,Average}";
                    rsCFields["P_Set"].Value = "Standard";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Standard";
                    break;


                case "FUELLOYALTY":
                    rsCFields["P_CLASS"].Value = "LOYALTY"; //LOYALTY
                    rsCFields["P_Name"].Value = "FUELLOYALTY";
                    rsCFields["P_Desc"].Value = "Support Fuel Loyalty?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;


                case "SCANLOYCARD":
                    rsCFields["P_CLASS"].Value = "LOYALTY"; //LOYALTY
                    rsCFields["P_Name"].Value = "SCANLOYCARD";
                    rsCFields["P_Desc"].Value = "Take Loyalty Card from Scanner?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "COUPONTEND":
                    rsCFields["P_CLASS"].Value = "LOYALTY"; //LOYALTY
                    rsCFields["P_Name"].Value = "COUPONTEND";
                    rsCFields["P_Desc"].Value = "Coupon Tender Name for Fuel Loyalty.";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Coupon,None}";
                    rsCFields["P_Set"].Value = "Coupon";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Coupon";
                    break;



                case "GIFTTYPE":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GIFTTYPE";
                    rsCFields["P_Desc"].Value = "What kind of Gift Certificate do you support?";
                    rsCFields["P_LEVELS"].Value = "{STOCK, SUBDETAIL, SUBDEPT, DEPT, COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{NONE,LocalGift,GiveX,Milliplein}";
                    rsCFields["P_Set"].Value = "NONE";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "NONE";
                    AddPolicyCanBe("GiftType", "STOCK", "SUBDETAIL", "SUBDEPT", "DEPT", "COMPANY");
                    break;

                case "PSI_DEPT":
                    rsCFields["P_CLASS"].Value = "STOCK";
                    rsCFields["P_Name"].Value = "PSI_DEPT";
                    rsCFields["P_Desc"].Value = "Select which department PSINet Products belong to";
                    rsCFields["P_LEVELS"].Value = "{STOCK, SUBDETAIL, SUBDEPT, DEPT, COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = "No";
                    AddPolicyCanBe("GiftPSI_Dept", "STOCK", "SUBDETAIL", "SUBDEPT", "DEPT", "COMPANY");
                    break;

                case "SCANGIFTCARD":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "SCANGIFTCARD";
                    rsCFields["P_Desc"].Value = "Take Gift Card from Scanner?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "GIVEX_IP":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GIVEX_IP";
                    rsCFields["P_Desc"].Value = "IP address of GiveX TPS";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "127.0.0.1";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "127.0.0.1";
                    break;

                case "GIVEX_PORT":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GIVEX_PORT";
                    rsCFields["P_Desc"].Value = "TCP listening port of GiveX TPS";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,999999)";
                    rsCFields["P_Set"].Value = "1111";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "1111";
                    break;

                case "GIVETIMEOUT":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GIVETIMEOUT";
                    rsCFields["P_Desc"].Value = "GiveX TPS communication Time Out";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,99)";
                    rsCFields["P_Set"].Value = "10";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 10;
                    break;

                case "GIVEX_USER":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GIVEX_USER";
                    rsCFields["P_Desc"].Value = "GiveX User ID";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "9888";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "9888";
                    break;

                case "GIVEX_PASS":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GIVEX_PASS";
                    rsCFields["P_Desc"].Value = "GiveX User Password";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "2687";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "2687";
                    break;

                case "GiveXMerchID":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GiveXMerchID";
                    rsCFields["P_Desc"].Value = "GiveX Merchant ID";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "001";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "001";
                    break;



                case "AlwAdjGiveX":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "AlwAdjGiveX";
                    rsCFields["P_Desc"].Value = "Allow Adjustment for GiveX?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;


                case "Log_Unlimit": //
                    rsCFields["P_CLASS"].Value = "USER_SYS"; //User can log on another till
                    rsCFields["P_Name"].Value = "LOG_UNLIMIT";
                    rsCFields["P_Desc"].Value = "User can log on more than one till";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "Lock_Till": //
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; //User can log on another till
                    rsCFields["P_Name"].Value = "LOCK_TILL";
                    rsCFields["P_Desc"].Value = "Till is locked automatically after inactivity?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                case "Lock_Time": //
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; //User can log on another till
                    rsCFields["P_Name"].Value = "LOCK_TIME";
                    rsCFields["P_Desc"].Value = "Till/BackOffice is locked automatically after _____ minutes";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG{1,1440}";
                    rsCFields["P_Set"].Value = "10";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 10;
                    break;


                case "ShowAccBal":
                    rsCFields["P_CLASS"].Value = "CUST"; //Customer
                    rsCFields["P_Name"].Value = "ShowAccBal";
                    rsCFields["P_Desc"].Value = "Show customer account balance in receipt?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "PRINT_LOGO":
                    rsCFields["P_CLASS"].Value = "TILL_PRT"; //Printing Option
                    rsCFields["P_Name"].Value = "PRINT_LOGO";
                    rsCFields["P_Desc"].Value = "Print Logo?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "PRINT_COPIES":
                    rsCFields["P_CLASS"].Value = "TILL_PRT"; //User Class
                    rsCFields["P_Name"].Value = "PRINT_COPIES";
                    rsCFields["P_Desc"].Value = "How many copies do you want to print receipt?";
                    rsCFields["P_LEVELS"].Value = "{SALETYPE,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,9)";
                    rsCFields["P_Set"].Value = "1";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 1;
                    AddPolicyCanBe("PRINT_COPIES", "SALETYPE", "COMPANY");
                    break;



                case "U_BR_LIMIT":
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "U_BR_LIMIT";
                    rsCFields["P_Desc"].Value = "Bottle return limit for user.";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(0,1000)";
                    rsCFields["P_Set"].Value = "2.00";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 2;
                    AddPolicyCanBe("U_BR_LIMIT", "USER", "UGROUP", "COMPANY");
                    break;

                case "U_BOTTLERTN":
                    rsCFields["P_CLASS"].Value = "USER"; //Printing Option
                    rsCFields["P_Name"].Value = "U_BOTTLERTN";
                    rsCFields["P_Desc"].Value = "User Can do bottle return";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    AddPolicyCanBe("U_BOTTLERTN", "USER", "UGROUP", "COMPANY");
                    break;


                case "DROP_ENV": //
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; //Need to enter envelope number for cash drop
                    rsCFields["P_Name"].Value = "DROP_ENV";
                    rsCFields["P_Desc"].Value = "Use Envelope Number for Cash Drops?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;


                case "ThirdParty":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "ThirdParty";
                    rsCFields["P_Desc"].Value = "Supports Third Party Card?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "TrdPty_IP":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "TrdPty_IP";
                    rsCFields["P_Desc"].Value = "IP address of Third Party Card TPS";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "127.0.0.1";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "127.0.0.1";
                    break;

                case "TrdPty_Port":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "TrdPty_Port";
                    rsCFields["P_Desc"].Value = "TCP listening port of Third Party Card TPS";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,999999)";
                    rsCFields["P_Set"].Value = "45451";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "45451";
                    break;

                case "TrdPtyTmt":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "TrdPtyTmt";
                    rsCFields["P_Desc"].Value = "Third Party Card TPS communication Time Out";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,99)";
                    rsCFields["P_Set"].Value = "10";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 10;
                    break;

                case "Cmbn_TrdPty":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "Cmbn_TrdPty";
                    rsCFields["P_Desc"].Value = "Tender Screen Display Only One Third Party Card?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "GiftTender":
                    rsCFields["P_CLASS"].Value = "GCERT"; //GiftCertificate
                    rsCFields["P_Name"].Value = "GiftTender";
                    rsCFields["P_Desc"].Value = "Gift Certificate type for tender";
                    rsCFields["P_LEVELS"].Value = "{TENDER,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{LocalGift,EKO}";
                    rsCFields["P_Set"].Value = "LocalGift";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "LocalGift";
                    AddPolicyCanBe("GiftTender", "TENDER", "COMPANY");
                    break;



                case "Accumlation":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "Accumlation";
                    rsCFields["P_Desc"].Value = "Accumlate loyalty points?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "LoyExpDate":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "LoyExpDate";
                    rsCFields["P_Desc"].Value = "Tender screen display expiry date for loyalty card?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "TrdPtyLoyal":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "TrdPtyLoyal";
                    rsCFields["P_Desc"].Value = "Third Party Loyalty program name";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Milliplein}";
                    rsCFields["P_Set"].Value = "Milliplein";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Milliplein";
                    break;



                case "TrdPtyExt":
                    rsCFields["P_CLASS"].Value = "THIRDPARTY"; //Third Party Card
                    rsCFields["P_Name"].Value = "TrdPtyExt";
                    rsCFields["P_Desc"].Value = "Third Party product extraction code";
                    rsCFields["P_LEVELS"].Value = "{STOCK, SUBDETAIL, SUBDEPT, DEPT, COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{3,5,6,7,10}";
                    rsCFields["P_Set"].Value = "10";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "10";
                    AddPolicyCanBe("TrdPtyExt", "STOCK", "SUBDETAIL", "SUBDEPT", "DEPT", "COMPANY");
                    break;



                case "U_AddStock":
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "U_AddStock";
                    rsCFields["P_Desc"].Value = "User can add stock in POS?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    AddPolicyCanBe("U_AddStock", "USER", "UGROUP", "COMPANY");
                    break;



                case "ELG_LOY": //  
                    rsCFields["P_CLASS"].Value = "STOCK";
                    rsCFields["P_Name"].Value = "ELG_LOY";
                    rsCFields["P_Desc"].Value = "Eligible for loyalty set to True by default";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;



                case "PRINT_VOID":
                    rsCFields["P_CLASS"].Value = "TILL_PRT";
                    rsCFields["P_Name"].Value = "PRINT_VOID";
                    rsCFields["P_Desc"].Value = "Print receipt for void and return?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "PRN_SgnRtn":
                    rsCFields["P_CLASS"].Value = "TILL_PRT";
                    rsCFields["P_Name"].Value = "PRN_SgnRtn";
                    rsCFields["P_Desc"].Value = "Print signature for refund sales?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                case "U_RUNAWAY":
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "U_RUNAWAY";
                    rsCFields["P_Desc"].Value = "User can process run away?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("U_RUNAWAY", "USER", "UGROUP", "COMPANY");
                    break;




                case "TE_ByRate":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_ByRate";
                    rsCFields["P_Desc"].Value = "Calculate price based on tax rate(AITE-Yes,SITE-No)?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                case "TE_Type":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_Type";
                    rsCFields["P_Desc"].Value = "Tax Exempt Type";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{SITE,AITE}"; //Saskatchewan Indian Tax Exemption; Alberta Indian Tax Exemption
                    rsCFields["P_Set"].Value = "AITE";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "AITE";
                    break;



                case "TE_AgeRstr":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_AgeRstr";
                    rsCFields["P_Desc"].Value = "Reject the sale if the Cardholder is under restriction age?";
                    rsCFields["P_LEVELS"].Value = "{STOCK, SUBDETAIL, SUBDEPT, DEPT, COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "";

                    returnValue = false;
                    AddPolicyCanBe("TE_AgeRstr", "STOCK", "SUBDETAIL", "SUBDEPT", "DEPT", "COMPANY");
                    break;



                case "Vouch_Copy":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT"; //TAX EXEMPT
                    rsCFields["P_Name"].Value = "Vouch_Copy";
                    rsCFields["P_Desc"].Value = "How many copies do you want to print Tax Exempt Voucher?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,9)";
                    rsCFields["P_Set"].Value = "1";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 1;
                    break;

                case "AgeRestrict":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT"; //TAX EXEMPT
                    rsCFields["P_Name"].Value = "AgeRestrict";
                    rsCFields["P_Desc"].Value = "What\'s the age restriction for tobacco sale?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(15,25)";
                    rsCFields["P_Set"].Value = "18";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 18;
                    break;



                case "CheckUpsell":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "CheckUpsell";
                    rsCFields["P_Desc"].Value = "Check up-sell sales?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "Msg_Input":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "Msg_Input";
                    rsCFields["P_Desc"].Value = "Support input messages?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "CashAuthPP":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "CashAuthPP";
                    rsCFields["P_Desc"].Value = "Support cashier authorize for pay@pump?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                //   - AUTOMATIC PROCESS OF aite file extraction and loadin of message file and customer registry
                case "TE_AUTOMATE":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_AUTOMATE";
                    rsCFields["P_Desc"].Value = "Use automation for TRA Files processing?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //

                // format
                case "TIMEFORMAT":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "TIMEFORMAT";
                    rsCFields["P_Desc"].Value = "Time is printed in _____ format";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{12 HOURS,24 HOURS}";
                    rsCFields["P_Set"].Value = "12 HOURS";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "12 HOURS";
                    break;
                // Nicolette end


                case "StopMsg":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "StopMsg";
                    rsCFields["P_Desc"].Value = "Display confirm message to stop pump?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "AllowStack":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "AllowStack";
                    rsCFields["P_Desc"].Value = "Allow stack fuel sale?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;



                case "AllowMin":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practice
                    rsCFields["P_Name"].Value = "AllowMin";
                    rsCFields["P_Desc"].Value = "Allow Minimize POS window?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;



                case "U_ManuFPrice":
                    rsCFields["P_CLASS"].Value = "USER"; //User level
                    rsCFields["P_Name"].Value = "U_ManuFPrice";
                    rsCFields["P_Desc"].Value = "User can manually change fuel price?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    AddPolicyCanBe("U_ManuFPrice", "USER", "UGROUP", "COMPANY");
                    break;



                case "PRINT_CrdHld":
                    rsCFields["P_CLASS"].Value = "TILL_PRT"; //Printing Option
                    rsCFields["P_Name"].Value = "PRINT_CrdHld";
                    rsCFields["P_Desc"].Value = "Print Card Holder Agreement for Fleet Card?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "Scale_Item":
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; //Till sales options
                    rsCFields["P_Name"].Value = "Scale_Item";
                    rsCFields["P_Desc"].Value = "Support Scale Item and Loto-Quebec?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "FuelRebate":
                    rsCFields["P_CLASS"].Value = "DISCO"; //Discount options
                    rsCFields["P_Name"].Value = "FuelRebate";
                    rsCFields["P_Desc"].Value = "Support Customer Fuel Rebate?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "CpnSerialID":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "CpnSerialID";
                    rsCFields["P_Desc"].Value = "Input serial number for vendor coupon?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "UPC_Format":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "UPC_Format";
                    rsCFields["P_Desc"].Value = "What\'s the UPC format system?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{UPC-A,EAN-13}";
                    rsCFields["P_Set"].Value = "UPC-A";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "UPC-A";
                    break;

                case "ItemCodeDgt":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "ItemCodeDgt";
                    rsCFields["P_Desc"].Value = "How many digits of the item code for scalable items?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,6)";
                    rsCFields["P_Set"].Value = "5";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 5;
                    break;

                case "AmountDigit":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "AmountDigit";
                    rsCFields["P_Desc"].Value = "How many digits of Amount/Weight for scalable items?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,7)";
                    rsCFields["P_Set"].Value = "4";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 4;
                    break;

                case "UseWeight":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "UseWeight";
                    rsCFields["P_Desc"].Value = "Do you print weight in scalable item UPC?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;


                case "TreatyNumDgt":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TreatyNumDgt";
                    rsCFields["P_Desc"].Value = "How many digits of SITE treaty number card?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,15)";
                    rsCFields["P_Set"].Value = "10";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 10;
                    break;

                case "PrtExmptTax":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "PrtExmptTax";
                    rsCFields["P_Desc"].Value = "Print exempted tax in receipt?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;




                case "DftRdTotal":
                    rsCFields["P_CLASS"].Value = "FUEL";
                    rsCFields["P_Name"].Value = "DftRdTotal";
                    rsCFields["P_Desc"].Value = "Default to read totalizer while setting fuel price.";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "TankGauge":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "TankGauge";
                    rsCFields["P_Desc"].Value = "Support Tank Gauge System integration?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "U_DipRead":
                    rsCFields["P_CLASS"].Value = "USER"; //User level
                    rsCFields["P_Name"].Value = "U_DipRead";
                    rsCFields["P_Desc"].Value = "User can process tank dip reading?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    AddPolicyCanBe("U_DipRead", "USER", "UGROUP", "COMPANY");
                    break;

                case "DftRdTankDip":
                    rsCFields["P_CLASS"].Value = "FUEL";
                    rsCFields["P_Name"].Value = "DftRdTankDip";
                    rsCFields["P_Desc"].Value = "Default to process tank dip reading when setting fuel price.";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;



                case "SOUND_ALARM":
                    rsCFields["P_CLASS"].Value = "SOUNDS";
                    rsCFields["P_Name"].Value = "SOUND_ALARM";
                    rsCFields["P_Desc"].Value = "Alarm sound repeat interval (min)";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,60)";
                    rsCFields["P_Set"].Value = "30";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 30;
                    break;


                case "FuelPriceChg":
                    rsCFields["P_CLASS"].Value = "FUEL";
                    rsCFields["P_Name"].Value = "FuelPriceChg";
                    rsCFields["P_Desc"].Value = "Remain in Fuel Price Screen until prices are set?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                //  -  if the policy is tru print user code and name , otherwise print only code( mr Gas put full name on user name and printing that is aganist privacy policy)
                case "PRN_UName":
                    rsCFields["P_CLASS"].Value = "TILL_PRT";
                    rsCFields["P_Name"].Value = "PRN_UName";
                    rsCFields["P_Desc"].Value = "Print User Name on Receipts?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //

                case "AutoShftPick":
                    rsCFields["P_CLASS"].Value = "TILL_EOD";
                    rsCFields["P_Name"].Value = "AutoShftPick";
                    rsCFields["P_Desc"].Value = "Automatically pick the next shift (it\'s not based on time)?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "Hour24Store":
                    rsCFields["P_CLASS"].Value = "TILL_EOD";
                    rsCFields["P_Name"].Value = "Hour24Store";
                    rsCFields["P_Desc"].Value = "Is it 24 Hour Store?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //
                case "CreditMsg":
                    rsCFields["P_CLASS"].Value = "CUST";
                    rsCFields["P_Name"].Value = "CreditMsg";
                    rsCFields["P_Desc"].Value = "Show customer note if the customer is over limit(Y\\N)?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //shiny end
                //
                case "ShowCardCust":
                    rsCFields["P_CLASS"].Value = "CUST";
                    rsCFields["P_Name"].Value = "ShowCardCust";
                    rsCFields["P_Desc"].Value = "Display Parent Account if customer card is linked(Y\\N)?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //shiny end

                //
                case "AuthPumpPOS":
                    rsCFields["P_CLASS"].Value = "FUEL";
                    rsCFields["P_Name"].Value = "AuthPumpPOS";
                    rsCFields["P_Desc"].Value = "Authorize Pump by POS ID?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                //
                case "USE_KICKBACK":
                    rsCFields["P_CLASS"].Value = "I_LOYALTY";
                    rsCFields["P_Name"].Value = "USE_KICKBACK";
                    rsCFields["P_Desc"].Value = "Support KickBack Reward Points?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "KICKBACK_IP":
                    rsCFields["P_CLASS"].Value = "I_LOYALTY";
                    rsCFields["P_Name"].Value = "KICKBACK_IP";
                    rsCFields["P_Desc"].Value = "IP Address of KickBack TPS";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "127.0.0.1";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "127.0.0.1";
                    break;

                case "KICKBACK_PRT":
                    rsCFields["P_CLASS"].Value = "I_LOYALTY";
                    rsCFields["P_Name"].Value = "KICKBACK_PRT";
                    rsCFields["P_Desc"].Value = "TCP listening port of KickBack TPS";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-99999}";
                    rsCFields["P_Set"].Value = "10001";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "10001";
                    break;

                case "KICKBACK_TMT":
                    rsCFields["P_CLASS"].Value = "I_LOYALTY";
                    rsCFields["P_Name"].Value = "KICKBACK_TMT";
                    rsCFields["P_Desc"].Value = "KickBack TPS communication Timeout";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-99}";
                    rsCFields["P_Set"].Value = "5";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "5";
                    break;

                case "LOYALTYMESG":
                    rsCFields["P_CLASS"].Value = "I_LOYALTY";
                    rsCFields["P_Name"].Value = "LOYALTYMESG";
                    rsCFields["P_Desc"].Value = "Customized message";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "Ask for KickBack Card or Enter Phone Number.";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Ask for KickBack Card or Enter Phone Number.";
                    break;
                //
                case "AUTHPUMP_EOD":
                    rsCFields["P_CLASS"].Value = "TILL_EOD";
                    rsCFields["P_Name"].Value = "AUTHPUMP_EOD";
                    rsCFields["P_Desc"].Value = "Be able to authorize pumps when closing the till?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;


                //
                case "U_ALLOWFLPAY":
                    rsCFields["P_CLASS"].Value = "USER";
                    rsCFields["P_Name"].Value = "U_ALLOWFLPAY";
                    rsCFields["P_Desc"].Value = "User can accept fleet payments?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("U_ALLOWFLPAY", "USER", "UGROUP", "COMPANY");
                    break;

                case "U_OPENDRW":
                    rsCFields["P_CLASS"].Value = "USER";
                    rsCFields["P_Name"].Value = "U_OPENDRW";
                    rsCFields["P_Desc"].Value = "User can open cash drawer?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("U_OPENDRW", "USER", "UGROUP", "COMPANY");
                    break;

                // boolean
                case "DEFAULTCUST":
                    rsCFields["P_CLASS"].Value = "CUST";
                    rsCFields["P_Name"].Value = "DEFAULTCUST";
                    rsCFields["P_Desc"].Value = "Use default customer";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                //  define default customer for Fuel Discount
                case "DEFCUST_CODE":
                    rsCFields["P_CLASS"].Value = "CUST";
                    rsCFields["P_Name"].Value = "DEFCUST_CODE";
                    rsCFields["P_Desc"].Value = "Default customer is";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(Client, CL_Code)";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                //   end

                // 11/17/2008 Nicolette added to control user's access to "Suspend" and "Unsuspend" buttons
                case "U_SUSP":
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "U_SUSP";
                    rsCFields["P_Desc"].Value = "User can suspend and unsuspend sales?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("U_SUSP", "USER", "UGROUP", "COMPANY");
                    break;

                //  
                case "EXACTCHG":
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "EXACTCHG";
                    rsCFields["P_Desc"].Value = "Activate the \'Exact Change\' Button on the POS?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("EXACTCHG", "USER", "UGROUP", "COMPANY");
                    break;
                //Shiny end

                //   to print coupon on a separate receipt
                case "PRT_CPN":
                    rsCFields["P_CLASS"].Value = "TILL_PRT";
                    rsCFields["P_Name"].Value = "PRT_CPN";
                    rsCFields["P_Desc"].Value = "Print coupon on a separate receipt?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //  

                case "Auto_EOD":
                    rsCFields["P_CLASS"].Value = "TILL_EOD"; //Till End of Day Options
                    rsCFields["P_Name"].Value = "Auto_EOD";
                    rsCFields["P_Desc"].Value = "Support Automatic EOD?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "AutoEodTime":
                    rsCFields["P_CLASS"].Value = "TILL_EOD"; //Till End of Day Options
                    rsCFields["P_Name"].Value = "AutoEodTime";
                    rsCFields["P_Desc"].Value = "Automatic EOD time";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "TM(12:00:00 AM,12:00:00 PM)";
                    rsCFields["P_Set"].Value = "11:00:00 PM";
                    rsCFields["P_VARTYPE"].Value = "T";

                    returnValue = DateTime.Parse("11:00:00 PM");
                    break;

                case "ChangeShift":
                    rsCFields["P_CLASS"].Value = "TILL_EOD"; //Till End of Day Options
                    rsCFields["P_Name"].Value = "ChangeShift";
                    rsCFields["P_Desc"].Value = "Change shift after Auto EOD?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;


                case "EodRetryNum":
                    rsCFields["P_CLASS"].Value = "TILL_EOD"; //Till End of Day Options
                    rsCFields["P_Name"].Value = "EodRetryNum";
                    rsCFields["P_Desc"].Value = "Automatic EOD Retry Limit";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,10)";
                    rsCFields["P_Set"].Value = "5";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 5;
                    break;

                // added security policy for Cash Bonus Program
                case "CashBonus":
                    rsCFields["P_CLASS"].Value = "LOYALTY";
                    rsCFields["P_Name"].Value = "CASHBONUS";
                    rsCFields["P_Desc"].Value = "Support Cash Bonus Program?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "CBONUSNAME":
                    rsCFields["P_CLASS"].Value = "LOYALTY";
                    rsCFields["P_Name"].Value = "CBONUSNAME";
                    rsCFields["P_Desc"].Value = "Cash Bonus Name";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                case "CBONUSDRAW":
                    rsCFields["P_CLASS"].Value = "LOYALTY"; //Allow Cash Draw\Drop for Cash Bonus
                    rsCFields["P_Name"].Value = "CBONUSDRAW";
                    rsCFields["P_Desc"].Value = "Do you support till Cash Bonus Draw\\Drop?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "CBONUSFLOAT":
                    rsCFields["P_CLASS"].Value = "LOYALTY"; //Allow Till Float for Cash Bonus
                    rsCFields["P_Name"].Value = "CBONUSFLOAT";
                    rsCFields["P_Desc"].Value = "Do you provide till Cash Bonus float??";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "CBONUSTEND": // Tender name for Cash bonus Program
                    rsCFields["P_CLASS"].Value = "LOYALTY"; //LOYALTY
                    rsCFields["P_Name"].Value = "CBONUSTEND";
                    rsCFields["P_Desc"].Value = "Tender Name for Cash Bonus Program:";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(TendMast, TendDesc)";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                //
                //  - ####Tidel
                case "TidelSafe":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "TidelSafe";
                    rsCFields["P_Desc"].Value = "Using Tidel Safe(Y/N)?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "Tidel_IP":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "Tidel_IP";
                    rsCFields["P_Desc"].Value = "IP address of Tidel Server";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "127.0.0.1";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "127.0.0.1";
                    break;

                case "Tidel_Port":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "Tidel_Port";
                    rsCFields["P_Desc"].Value = "TCP listening port of Tidel Server";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,999999)";
                    rsCFields["P_Set"].Value = "1111";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "1111";
                    break;

                case "TidelTimeOut":
                    rsCFields["P_CLASS"].Value = "PRAC"; //GiftCertificate
                    rsCFields["P_Name"].Value = "TidelTimeOut";
                    rsCFields["P_Desc"].Value = "Tidel communication Time Out";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,99)";
                    rsCFields["P_Set"].Value = "10";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 10;
                    break;

                case "Tidel_User":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "Tidel_User";
                    rsCFields["P_Desc"].Value = "Tidel User ID";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "9888";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "9888";
                    break;
                case "TidelMarkUp":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "TidelMarkup";
                    rsCFields["P_Desc"].Value = "Tidel Markup File Location?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "S:\\";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "S:\\";
                    break;
                //   -### Tidel
                case "FMSHIFT": // security is to set which shift is doing fuel management
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "FMSHIFT";
                    rsCFields["P_Desc"].Value = "Shift Number for Automatic Fuel Management Reading";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{0-5}";
                    rsCFields["P_Set"].Value = "0";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 0;
                    break;

                //  - for pump test
                case "U_PUMPTEST":
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "U_PUMPTEST";
                    rsCFields["P_Desc"].Value = "User can process Pump Test?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("U_PUMPTEST", "USER", "UGROUP", "COMPANY");
                    break;
                //
                case "SELL_INACT": //  
                    rsCFields["P_CLASS"].Value = "TILL_SAL"; // Till sales
                    rsCFields["P_Name"].Value = "SELL_INACT";
                    rsCFields["P_Desc"].Value = "Sell Inactive Item?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //  - Gasking Charges - Automatically Charge the account when using Customer card ( we need security policy to load Account tender, even if we didn't select the customer
                case "CHARGE_ACCT":
                    rsCFields["P_CLASS"].Value = "CUST"; //Customer
                    rsCFields["P_Name"].Value = "CHARGE_ACCT";
                    rsCFields["P_Desc"].Value = "Charge the Customer Account when using Customer Card?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //
                //  - Support SAF transactions
                case "SUPPORT_SAF":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business practice
                    rsCFields["P_Name"].Value = "SUPPORT_SAF";
                    rsCFields["P_Desc"].Value = "Support Store & Forward(SAF) Transactions?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                case "CALLBANKONLY": // yes means , allow only callbank ; no means ; look for floor limit
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business practice
                    rsCFields["P_Name"].Value = "CALLBANKONLY";
                    rsCFields["P_Desc"].Value = "Allow only \'Call Bank Authorization\' for SAF Transactions?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //2013 10 17 Reji  - Added new polity for Bank transaction timeout
                case "BANKTIMEOUTSEC": // Bank transaction timeout in seconds
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business practice
                    rsCFields["P_Name"].Value = "BANKTIMEOUTSEC";
                    rsCFields["P_Desc"].Value = "Bank transaction timeout in seconds";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-10000}";
                    rsCFields["P_Set"].Value = "85";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = true;
                    break;
                case "U_INITDEBIT": //  to control initdebit and stps button in pos
                    rsCFields["P_CLASS"].Value = "USER"; //User Class
                    rsCFields["P_Name"].Value = "U_INITDEBIT";
                    rsCFields["P_Desc"].Value = "User can process Init Debit?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("U_INITDEBIT", "USER", "UGROUP", "COMPANY");
                    break;
                case "ARTENDER": // Tender name for Customer AR charge Account
                    rsCFields["P_CLASS"].Value = "CUST"; //Customer
                    rsCFields["P_Name"].Value = "ARTENDER";
                    rsCFields["P_Desc"].Value = "Tender Name for Customer AR Account:";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(TendMast, TendDesc)";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                //  - To get TE customer name for SITE OR MITE OR BITE( ALl are using policy name 'SITE"
                case "TE_GETNAME":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_GETNAME";
                    rsCFields["P_Desc"].Value = "Required to Collect Tax Exempt Customer Name?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    //Saskatchewan Indian Tax Exemption; Alberta Indian Tax Exemption
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //  - To get TE customer signature for SITE OR MITE OR BITE( ALl are using policy name 'SITE") - later for squamish need to add additional policy to get the signature manually or automatically
                case "TE_SIGNATURE":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_SIGNATURE";
                    rsCFields["P_Desc"].Value = "Required to Collect Tax Exempt Customer Signature?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //  - To get TE customer signature for SITE OR MITE OR BITE( ALl are using policy name 'SITE") - later for squamish need to add additional policy to get the signature manually or using signature reader
                case "TE_SIGNMODE":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_SIGNMODE";
                    rsCFields["P_Desc"].Value = "Mode for collecting Tax Exempt Customer Signature:";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Manual, Reader}";
                    rsCFields["P_Set"].Value = "Manual";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = true;
                    break;

                //shiny end
                //  - to track TE inventory
                case "TRACK_TEINV":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TRACK_TEINV";
                    rsCFields["P_Desc"].Value = "Track Tax Exempt Inventory Separately?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                //shiny end
                //Shiny - Nov6, 2009 -EMVVERSION
                case "EMVVERSION": // security is to identify EMVVERSION
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business Practices
                    rsCFields["P_Name"].Value = "EMVVERSION";
                    rsCFields["P_Desc"].Value = "Supports EMV Compliance?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //shiny end EMVVERSION

                //  , run POS with FuelOnly (BreakPoint integration)
                case "FUELONLY":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "FUELONLY";
                    rsCFields["P_Desc"].Value = "Fuel Only System?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                //  , default user for POS in FuelOnly mode (BreakPoint integration)
                case "DEF_USER":
                    rsCFields["P_CLASS"].Value = "USER_SYS";
                    rsCFields["P_Name"].Value = "DEF_USER";
                    rsCFields["P_Desc"].Value = "Default user for Fuel Only System";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(User,U_Code)";
                    rsCFields["P_Set"].Value = "X";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "X";
                    break;
                //   end

                //  , run POS with FuelOnly
                case "FUELONLY_IP":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "FUELONLY_IP";
                    rsCFields["P_Desc"].Value = "IP address for Integrated System?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "127.0.0.1";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "127.0.0.1";
                    break;

                case "FUELONLY_PRT":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "FUELONLY_PRT";
                    rsCFields["P_Desc"].Value = "TCP listening port for Integrated System";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-99999}";
                    rsCFields["P_Set"].Value = "10001";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "10001";
                    break;

                case "FUELONLY_TMT":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "FUELONLY_TMT";
                    rsCFields["P_Desc"].Value = "Integrated System communication timeout";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-99}";
                    rsCFields["P_Set"].Value = "5";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "5";
                    break;
                //   end

                //Svetlana
                case "SUPPORTEZI":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "SUPPORTEZI";
                    rsCFields["P_Desc"].Value = "Support Ezipin?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "Ezi_Dept":
                    rsCFields["P_CLASS"].Value = "STOCK";
                    rsCFields["P_Name"].Value = "Ezi_Dept";
                    rsCFields["P_Desc"].Value = "Select which department EziPin Products belong to";
                    rsCFields["P_LEVELS"].Value = "{COMPANY, DEPT, SUBDEPT, SUBDETAIL, STOCK }";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    AddPolicyCanBe("Ezi_Dept", "STOCK", "SUBDETAIL", "SUBDEPT", "DEPT");
                    break;

                case "Ezi_Reprint":
                    rsCFields["P_CLASS"].Value = "USER";
                    rsCFields["P_Name"].Value = "Ezi_Reprint";
                    rsCFields["P_Desc"].Value = "Users that can print the last receipt with voucher";
                    rsCFields["P_LEVELS"].Value = "{COMPANY, USER, UGROUP, COMPANY }";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    AddPolicyCanBe("Ezi_Reprint", "USER", "UGROUP", "COMPANY");
                    break;


                // March 31, 2010 Nicolette added for autodeactivate PostPay feature
                case "U_AD_PI": // User can activate/deactivate post pay
                    rsCFields["P_CLASS"].Value = "USER"; // User Authorization class
                    rsCFields["P_Name"].Value = "U_AD_PI";
                    rsCFields["P_Desc"].Value = "User can activate/deactivate Post Pay?";
                    rsCFields["P_LEVELS"].Value = "{USER,UGROUP,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("U_AD_PI", "USER", "UGROUP", "COMPANY");
                    break;
                // March 31, 2010 Nicolette end
                //  Added security policy to allow Cardprofiles
                case "RSTR_PROFILE":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "RSTR_PROFILE";
                    rsCFields["P_Desc"].Value = "Supports Private Card Restriction Profiles? ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //

                //
                case "CarWash":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "CarWash";
                    rsCFields["P_Desc"].Value = "Supports carwash? ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "CW_IP":
                    rsCFields["P_CLASS"].Value = "CARWASH";
                    rsCFields["P_Name"].Value = "CW_IP";
                    rsCFields["P_Desc"].Value = "Carwash server IP Address: ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "127.0.0.1";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "127.0.0.1";
                    break;

                case "CW_Port":
                    rsCFields["P_CLASS"].Value = "CARWASH";
                    rsCFields["P_Name"].Value = "CW_Port";
                    rsCFields["P_Desc"].Value = "TCP listening port of Car Wash Server: ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,999999)";
                    rsCFields["P_Set"].Value = "20988";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = "20988";
                    break;

                case "CW_TOut":
                    rsCFields["P_CLASS"].Value = "CARWASH";
                    rsCFields["P_Name"].Value = "CW_TOut";
                    rsCFields["P_Desc"].Value = "Car Wash Communication Timeout (sec): ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,2000)";
                    rsCFields["P_Set"].Value = "15";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 15;
                    break;

                case "CarWash_Int":
                    rsCFields["P_CLASS"].Value = "CARWASH";
                    rsCFields["P_Name"].Value = "CarWash_Int";
                    rsCFields["P_Desc"].Value = "Is Car Wash Integrated? ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                case "CarWash_Dep":
                    rsCFields["P_CLASS"].Value = "CARWASH";
                    rsCFields["P_Name"].Value = "CarWash_Dep";
                    rsCFields["P_Desc"].Value = "Car Wash Category Name: ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    //!P_CHOICES = "{?}" '
                    rsCFields["P_CHOICES"].Value = "STAB(Dept,dept_name)";
                    rsCFields["P_Set"].Value = "CarWash";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "CarWash";
                    break;


                // exempt
                case "SITE_RTVAL":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "SITE_RTVAL";
                    rsCFields["P_Desc"].Value = "Supports Real-Time Validation for SITE?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                //   for HST rebate (Ontario)
                case "TAX_REBATE":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "TAX_REBATE";
                    rsCFields["P_Desc"].Value = "Support tax rebate?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "TX_RB_AMT":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "TX_RB_AMT";
                    rsCFields["P_Desc"].Value = "Maximum amount to qualify for tax rebate?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-100}";
                    rsCFields["P_Set"].Value = "4";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 4;
                    break;
                //   end
                //shiny added security for squamish member\Nonmember- instead of  they select customer manually- we will automatically identify member\nonmember from the treaty number- then will use the setting form member\non-member accordingly.
                case "BANDMEMBER": //  - change(c)
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "BANDMEMBER";
                    rsCFields["P_Desc"].Value = "Customer settings linked to Band Member:";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(Client, CL_code)";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                case "NONBANDMEMBER": //   - change(c)
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "NONBANDMEMBER";
                    rsCFields["P_Desc"].Value = "Customer settings linked to Non Band Member:";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(Client, CL_code)";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;

                case "ENABLE_BANDACCT": // security is for handling Jd edward band account - they can link a band account treaty number with customer(not customer card) and when selecting security customer pickup security treaty number automatically
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "ENABLE_BANDACCT";
                    rsCFields["P_Desc"].Value = "Do you support Band Account for Tax Exempt Sales?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "MEMBER_IDENTITY": //  - change(c)
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "MEMBER_IDENTITY";
                    rsCFields["P_Desc"].Value = "Band Account Number to identify the band member:";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "555";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                case "IDENTIFY_MEMBER": //   -security is for automatically identify band member
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "IDENTIFY_MEMBER";
                    rsCFields["P_Desc"].Value = "Automatically identify the Band Member using Band Account?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "PRN_CO_CODE": //  -
                    rsCFields["P_CLASS"].Value = "TILL_PRT";
                    rsCFields["P_Name"].Value = "PRN_CO_CODE";
                    rsCFields["P_Desc"].Value = "Print Store Code on Receipts?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                case "SAFE&ATMDROP": //
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business practice
                    rsCFields["P_Name"].Value = "SAFE&ATMDROP";
                    rsCFields["P_Desc"].Value = "Allow both SAFE and ATM Cash Drop?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                //  , key pad format in login screen
                case "KP_LOGIN":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "KP_LOGIN";
                    rsCFields["P_Desc"].Value = "Keypad format in login screen";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{D,2}";
                    rsCFields["P_Set"].Value = "D";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "D";
                    break;

                //   to process dip reading in POS without asking question
                case "ASK_DIPREAD":
                    rsCFields["P_CLASS"].Value = "FUEL"; //Fuel Management
                    rsCFields["P_Name"].Value = "ASK_DIPREAD";
                    rsCFields["P_Desc"].Value = "Prompt user for dip readings in POS till close";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;

                //   fuel price change from HeadOffice
                case "FUELPR_HO":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "FUELPR_HO";
                    rsCFields["P_Desc"].Value = "Support fuel price change from HeadOffice?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;

                case "FPR_NOTE_CNT":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "FPR_NOTE_CNT";
                    rsCFields["P_Desc"].Value = "Notify about fuel price changes from HeadOffice __times";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,10)";
                    rsCFields["P_Set"].Value = "3";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 3;
                    break;

                case "FPR_TIME":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "FPR_TIME";
                    rsCFields["P_Desc"].Value = "Notify about fuel price changes from HeadOffice every __ min";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "RG(1,10)";
                    rsCFields["P_Set"].Value = "2";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 2;
                    break;

                case "FPR_USER":
                    rsCFields["P_CLASS"].Value = "USER";
                    rsCFields["P_Name"].Value = "FPR_USER";
                    rsCFields["P_Desc"].Value = "User can reject fuel price changes from HeadOffice";
                    rsCFields["P_LEVELS"].Value = "{USER,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    AddPolicyCanBe("FPR_USER", "USER", "UGROUP", "COMPANY");
                    break;
                //   end

                //   predefined till numbers
                case "TILL_NUM":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "TILL_NUM";
                    rsCFields["P_Desc"].Value = "Use predefined till number";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                //  
                case "EMVVERSION_PATP":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    rsCFields["P_Name"].Value = "EMVVERSION_PATP";
                    rsCFields["P_Desc"].Value = "Supports EMV Compliance at the pump?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                //  
                case "CUST_SCAN":
                    rsCFields["P_CLASS"].Value = "CUST";
                    rsCFields["P_Name"].Value = "CUST_SCAN";
                    rsCFields["P_Desc"].Value = "Scan customer cards in POS to identify the customer?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                //  
                case "CUST_SWP":
                    rsCFields["P_CLASS"].Value = "CUST";
                    rsCFields["P_Name"].Value = "CUST_SWP";
                    rsCFields["P_Desc"].Value = "Swipe customer cards in POS to identify the customer?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                //  
                case "EXC_CASHBONUS":
                    rsCFields["P_CLASS"].Value = "LOYALTY";
                    rsCFields["P_Name"].Value = "EXC_CASHBONUS";
                    rsCFields["P_Desc"].Value = "Exclude from Cash Bonus Program";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(StockMst, Stock_Code)";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                //   end

                //  
                case "CUST_EXPDATE":
                    rsCFields["P_CLASS"].Value = "CUST";
                    rsCFields["P_Name"].Value = "CUST_EXPDATE";
                    rsCFields["P_Desc"].Value = "Verify expiry date on customer cards?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                // PM, Feb 4, 2013 Penny Adjustment
                case "PENNY_ADJ":
                    rsCFields["P_CLASS"].Value = "TILL_SAL";
                    rsCFields["P_Name"].Value = "PENNY_ADJ";
                    rsCFields["P_Desc"].Value = "Use Penny Adjustment";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                // PM, Feb 4, 2013 End

                // Reji, Apr 24, 2013 Windows Login
                case "WINDOWS_LOGIN":
                    rsCFields["P_CLASS"].Value = "PRAC";
                    //!P_SEQ = "2510"  ' code
                    rsCFields["P_Name"].Value = "WINDOWS_LOGIN";
                    rsCFields["P_Desc"].Value = "Login using Windows User";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L"; //   "C" char

                    returnValue = false;
                    break;
                // Reji, Apr 24, 2013 End

                // overpayment
                case "PRT_OVERPAY":
                    rsCFields["P_CLASS"].Value = "TILL_PRT";
                    rsCFields["P_Name"].Value = "PRT_OVERPAY";
                    rsCFields["P_Desc"].Value = "Print receipt for prepay overpayment?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                // quota
                case "U_OR_TEQUOTA":
                    rsCFields["P_CLASS"].Value = "USER";
                    rsCFields["P_Name"].Value = "U_OR_TEQUOTA";
                    rsCFields["P_Desc"].Value = "User can over-ride cigarette sale quota";
                    rsCFields["P_LEVELS"].Value = "{USER,COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //   end

                // 2013 12 12 - Reji for WEX Fleet card
                case "WEXEnabled": //
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business practice
                    rsCFields["P_Name"].Value = "WEXEnabled";
                    rsCFields["P_Desc"].Value = "Enable Wex Fleet Card";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "WEXTPSIP": //
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business practice
                    rsCFields["P_Name"].Value = "WEXTPSIP";
                    rsCFields["P_Desc"].Value = "Wex Fleet TPS Server IP";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "192.168.85.100";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                case "WEXTPSPORT":
                    rsCFields["P_CLASS"].Value = "PRAC"; //Business practice
                    rsCFields["P_Name"].Value = "WEXTPSPORT";
                    rsCFields["P_Desc"].Value = "Wex Fleet TPS Server Port";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-10000}";
                    rsCFields["P_Set"].Value = "8889";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 8889;
                    break;
                //End - 2013 12 12 - Reji for WEX Fleet card

                //Start - 2014 04 30 - Reji for Fuel Slale Grocery Coupon
                case "FSGC_ENABLE": //Fuel Sale Grocery Coupon
                    rsCFields["P_CLASS"].Value = "DISCO"; //Discount
                    rsCFields["P_Name"].Value = "FSGC_ENABLE";
                    rsCFields["P_Desc"].Value = "Enable Fuel Sale Grocery Coupon";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "FSGC_CREDIT":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "FSGC_CREDIT";
                    rsCFields["P_Desc"].Value = "Fuel Sale Grocery Coupon - Value for Credit ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "3.5";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 3.5;
                    break;
                case "FSGC_OTHER":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "FSGC_OTHER";
                    rsCFields["P_Desc"].Value = "Fuel Sale Grocery Coupon - Value for Debit and Others ";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "5";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 5;
                    break;
                case "FSGC_PLU":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "FSGC_PLU";
                    rsCFields["P_Desc"].Value = "Fuel Sale Grocery Coupon - PLU Number";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;
                case "FSGC_EXP":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "FSGC_EXP";
                    rsCFields["P_Desc"].Value = "Fuel Sale Grocery Coupon - Expiration Period in days";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "60";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 60;
                    break;
                case "FSGC_CalcType":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "FSGC_CalcType";
                    rsCFields["P_Desc"].Value = "Fuel Sale Grocery Coupon - Rate Calculated from";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{AMOUNT,QUANTITY}";
                    rsCFields["P_Set"].Value = "QUANTITY";
                    rsCFields["P_VARTYPE"].Value = "C";
                    break;
                //End - 2014 04 30 - Reji for Fuel Slale Grocery Coupon

                // 10-Nov-14 Jim for Ackroo Loyalty and Gift Card
                case "REWARDS_Enabled": //
                    rsCFields["P_CLASS"].Value = "REWARDS"; //Gift and Loyalty Program
                    rsCFields["P_Name"].Value = "REWARDS_Enabled";
                    rsCFields["P_Desc"].Value = "Enable ACKROO Loyalty and Gift Card";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                case "REWARDS_Message":
                    rsCFields["P_CLASS"].Value = "REWARDS";
                    rsCFields["P_Name"].Value = "REWARDS_Message";
                    rsCFields["P_Desc"].Value = "Customized message";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "Does Customer have a Loyalty Card?";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = true;
                    break;
                case "REWARDS_Caption":
                    rsCFields["P_CLASS"].Value = "REWARDS";
                    rsCFields["P_Name"].Value = "REWARDS_Caption";
                    rsCFields["P_Desc"].Value = "Rewards Caption on the Receipt";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "Cash Plus $$$ Earned";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = true;
                    break;
                case "REWARDS_NoRedmpt": //
                    rsCFields["P_CLASS"].Value = "REWARDS"; //Gift and Loyalty Program
                    rsCFields["P_Name"].Value = "REWARDS_NoRedmpt";
                    rsCFields["P_Desc"].Value = "Exclude ACKROO Loyalty Redemption";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                case "REWARDS_TpsIp": //
                    rsCFields["P_CLASS"].Value = "REWARDS"; //Gift and Loyalty Program
                    rsCFields["P_Name"].Value = "REWARDS_TpsIp";
                    rsCFields["P_Desc"].Value = "ACKROO TPS Server IP";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "";
                    rsCFields["P_Set"].Value = "192.168.85.100";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = true;
                    break;

                case "REWARDS_TpsPort":
                    rsCFields["P_CLASS"].Value = "REWARDS"; //Gift and Loyalty Program
                    rsCFields["P_Name"].Value = "REWARDS_TpsPort";
                    rsCFields["P_Desc"].Value = "ACKROO TPS Server Port";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-10000}";
                    rsCFields["P_Set"].Value = "8892";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = true;
                    break;

                case "REWARDS_Gift": //
                    rsCFields["P_CLASS"].Value = "REWARDS"; //Gift and Loyalty Program
                    rsCFields["P_Name"].Value = "REWARDS_Gift";
                    rsCFields["P_Desc"].Value = "ACKROO GiftCard";
                    rsCFields["P_LEVELS"].Value = "{STOCK,SUBDETAIL,SUBDEPT,DEPT}";
                    rsCFields["P_CHOICES"].Value = "{NONE,AckrooGift}";
                    rsCFields["P_Set"].Value = "AckrooGift";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "AckrooGift";
                    break;

                case "REWARDS_Timeout":
                    rsCFields["P_CLASS"].Value = "REWARDS";
                    rsCFields["P_Name"].Value = "REWARDS_Timeout";
                    rsCFields["P_Desc"].Value = "Ackroo TPS communication Timeout";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{1-99}";
                    rsCFields["P_Set"].Value = "10";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 10;
                    break;

                //End 10-Nov-14 Jim for Ackroo Loyalty and Gift Card

                // agencies
                case "TAX_EXEMPT_GA":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TAX_EXEMPT_GA";
                    rsCFields["P_Desc"].Value = "Support tax exemption for Government Agencies";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                // change
                case "FUEL_MAXTH":
                    rsCFields["P_CLASS"].Value = "FUEL";
                    rsCFields["P_Name"].Value = "FUEL_MAXTH";
                    rsCFields["P_Desc"].Value = "Maximum threshold for fuel price change is_____.";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{0.001-1.000}";
                    rsCFields["P_Set"].Value = "0.15";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 0.15;
                    break;
                //   end

                // e-Service
                case "TAX_EXEMPT_FNGTR":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TAX_EXEMPT_FNGTR";
                    rsCFields["P_Desc"].Value = "Use Ontario Gas Tax Refunds E-Services";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                // policy
                case "FSGC_ARTENDER":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "FSGC_ARTENDER";
                    rsCFields["P_Desc"].Value = "Print Fuel Sale Grocery Coupon for AR tender";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                // discount
                case "CADB_FUELDISC":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "CADB_FUELDISC";
                    rsCFields["P_Desc"].Value = "Support cash/debit fuel discount?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                //   end

                // customers
                case "TE_COLLECTTAX":
                    rsCFields["P_CLASS"].Value = "TAXEXEMPT";
                    rsCFields["P_Name"].Value = "TE_COLLECTTAX";
                    rsCFields["P_Desc"].Value = "Collected tax for tax exempt customers";
                    rsCFields["P_LEVELS"].Value = "{STOCK, SUBDETAIL, SUBDEPT, DEPT, COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(TAXMAST, TAX_NAME)";
                    rsCFields["P_Set"].Value = "";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    AddPolicyCanBe("TE_COLLECTTAX", "STOCK", "SUBDETAIL", "SUBDEPT", "DEPT", "COMPANY");
                    break;
                //   end

                //   reprint till close for the day for all shifts
                case "PRT_ALLSHIFTS":
                    rsCFields["P_CLASS"].Value = "TILL_PRT";
                    rsCFields["P_Name"].Value = "PRT_ALLSHIFTS";
                    rsCFields["P_Desc"].Value = "Shift number to reprint till close report for the day";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{0-5}";
                    rsCFields["P_Set"].Value = "0";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 0;
                    break;
                //   end
                // 06/03/2019 Tony
                case "TDRS_EXEPTGID":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "TDRS_EXEPTGID";
                    rsCFields["P_Desc"].Value = "Display fleet and credit for customer group ID";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "STAB(ClientGroup, GroupID)";
                    rsCFields["P_Set"].Value = "1";
                    rsCFields["P_VARTYPE"].Value = "N";

                    returnValue = 1;
                    break;
                case "TDRS_FUELDISC":
                    rsCFields["P_CLASS"].Value = "DISCO";
                    rsCFields["P_Name"].Value = "TDRS_FUELDISC";
                    rsCFields["P_Desc"].Value = "Display fleet and credit for cash/debit fuel discount?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "No";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = false;
                    break;
                case "PSINet_Type":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "PSINet_Type";
                    rsCFields["P_Desc"].Value = "PSINet Program Type";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Now Prepay, Ezipin, Payment Source}";
                    rsCFields["P_Set"].Value = "Payment Source";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "Payment Source";
                    break;
                case "SUPPORT_PSINet":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "SUPPORT_PSINet";
                    rsCFields["P_Desc"].Value = "Support PSINet?";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                case "REWARDS_Carwash":
                    rsCFields["P_CLASS"].Value = "REWARDS";
                    rsCFields["P_Name"].Value = "REWARDS_Carwash";
                    rsCFields["P_Desc"].Value = "ACKROO Carwash Category";
                    rsCFields["P_LEVELS"].Value = "{STOCK}";
                    rsCFields["P_CHOICES"].Value = "{ CAR WASH - PREMIUM,CAR WASH - BASIC }";
                    rsCFields["P_Set"].Value = "NONE";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "NONE";
                    break;
                case "REWARDS_CWGIFT":
                    rsCFields["P_CLASS"].Value = "REWARDS";
                    rsCFields["P_Name"].Value = "REWARDS_CWGIFT";
                    rsCFields["P_Desc"].Value = "ACKROO Carwash Giftcard";
                    rsCFields["P_LEVELS"].Value = "{STOCK}";
                    rsCFields["P_CHOICES"].Value = "{  }";
                    rsCFields["P_Set"].Value = "NONE";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "NONE";
                    break;
                case "REWARDS_CWPKG":
                    rsCFields["P_CLASS"].Value = "REWARDS";
                    rsCFields["P_Name"].Value = "REWARDS_CWPKG";
                    rsCFields["P_Desc"].Value = "ACKROO Carwash Package";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{?}";
                    rsCFields["P_Set"].Value = "5,10,25,50";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "5,10,25,50";
                    break;
                case "REWARDS_DefaultLoyal":
                    rsCFields["P_CLASS"].Value = "REWARDS";
                    rsCFields["P_Name"].Value = "REWARDS_DefaultLoyal";
                    rsCFields["P_Desc"].Value = "Use Ackroo as default Loyalty";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{Yes,No}";
                    rsCFields["P_Set"].Value = "Yes";
                    rsCFields["P_VARTYPE"].Value = "L";

                    returnValue = true;
                    break;
                //06/03/2019 end 
                //policies added by Tony  07/29/2019
                case "RECEIPT_TYPE":
                    rsCFields["P_CLASS"].Value = "FEATURES";
                    rsCFields["P_Name"].Value = "RECEIPT_TYPE";
                    rsCFields["P_Desc"].Value = "Receipt printing type";
                    rsCFields["P_LEVELS"].Value = "{COMPANY}";
                    rsCFields["P_CHOICES"].Value = "{DEFAULT,EN-AR}";
                    rsCFields["P_Set"].Value = "DEFAULT";
                    rsCFields["P_VARTYPE"].Value = "C";

                    returnValue = "";
                    break;



                    //policies added by Tony end 07/29/2019
                    // END TAG - please add any policy before security tag, so the tag can be used to find the end of procedure without scrooling down
            }

            rsCFields["P_ACTIVE"].Value = true;
            rsCFields["P_USED"].Value = true;
            rsC.Update();

            _performancelog.Debug($"End,PolicyService,AddPolicy,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }


        /// <summary>
        /// Add policy can be
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="pSeq1"></param>
        /// <param name="pSeq2"></param>
        /// <param name="pSeq3"></param>
        /// <param name="pSeq4"></param>
        /// <param name="pSeq5"></param>
        private void AddPolicyCanBe(string pName, string pSeq1, string pSeq2 = "", string pSeq3 = "", string pSeq4 = "", string pSeq5 = "")
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PolicyService,AddPolicyCanBe,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rsC = GetRecords("SELECT * FROM P_CANBE where P_NAME=\'" + pName + "\' and P_Seq=1", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly);
            var rsCFields = ((dynamic)rsC).Fields;
            if (rsC.EOF)
            {
                rsC.AddNew();
                rsCFields["P_Name"].Value = pName;
                rsCFields["P_SEQ"].Value = 1;
            }
            rsCFields["P_Canbe"].Value = pSeq1;
            rsC.Update();

            if (pSeq2.Length > 0)
            {
                rsC = GetRecords("SELECT * FROM P_CANBE where P_NAME=\'" + pName + "\' and P_Seq=2", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly);
                rsCFields = ((dynamic)rsC).Fields;
                if (rsC.EOF)
                {
                    rsC.AddNew();
                    rsCFields["P_Name"].Value = pName;
                    rsCFields["P_SEQ"].Value = 2;
                }
                rsCFields["P_Canbe"].Value = pSeq2;
                rsC.Update();
            }

            if (pSeq3.Length > 0)
            {
                rsC = GetRecords("SELECT * FROM P_CANBE where P_NAME=\'" + pName + "\' and P_Seq=3", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly);
                rsCFields = ((dynamic)rsC).Fields;
                if (rsC.EOF)
                {
                    rsC.AddNew();
                    rsCFields["P_Name"].Value = pName;
                    rsCFields["P_SEQ"].Value = 3;
                }
                rsCFields["P_Canbe"].Value = pSeq3;
                rsC.Update();
            }

            if (pSeq4.Length > 0)
            {
                rsC = GetRecords("SELECT * FROM P_CANBE where P_NAME=\'" + pName + "\' and P_Seq=4", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly);
                rsCFields = ((dynamic)rsC).Fields;
                if (rsC.EOF)
                {
                    rsC.AddNew();
                    rsCFields["P_Name"].Value = pName;
                    rsCFields["P_SEQ"].Value = 4;
                }
                rsCFields["P_Canbe"].Value = pSeq4;
                rsC.Update();
            }

            if (pSeq5.Length > 0)
            {
                rsC = GetRecords("SELECT * FROM P_CANBE where P_NAME=\'" + pName + "\' and P_Seq=5", DataSource.CSCAdmin, (int)CursorTypeEnum.adOpenForwardOnly);
                rsCFields = ((dynamic)rsC).Fields;
                if (rsC.EOF)
                {
                    rsC.AddNew();
                    rsCFields["P_Name"].Value = pName;
                    rsCFields["P_SEQ"].Value = 5;
                }
                rsCFields["P_Canbe"].Value = pSeq5;
                rsC.Update();
            }
            _performancelog.Debug($"End,PolicyService,AddPolicyCanBe,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }



        public List<BackOfficePolicy> GetAllPolicies()
        {
            var query = @"SELECT plc.Implemented
                        ,plc.P_CLASS
                        ,plc.P_NAME
                        ,plc.P_Seq
                        ,plc.CompanyLevelSet
                        ,plc.P_VARTYPE
                        ,plc.PolicyLevel
                        ,ps.P_VALUE AS PolicyValue
                        ,ps.P_SET AS OtherLevelSet FROM
                        (SELECT pcp.Implemented
                        ,pcp.P_CLASS
                        ,pcp.P_NAME
                        ,pcb.P_Seq
                        ,pcp.P_SET AS CompanyLevelSet
                        ,pcp.P_VARTYPE
                        ,pcb.P_Canbe as PolicyLevel
                        FROM [CSCAdmin].[dbo].[P_COMP] AS pcp
                        LEFT JOIN [dbo].[P_CANBE] AS pcb
                        ON pcp.P_NAME=pcb.P_NAME  ) AS plc
                        LEFT JOIN [dbo].[P_SET] AS ps
                        ON plc.P_NAME = ps.P_NAME";

            List<BackOfficePolicy> result = new List<BackOfficePolicy>();
            var rsComp = GetRecords(query, DataSource.CSCAdmin);

            var rsCompFields = ((dynamic)rsComp).Fields;
            while (!rsComp.EOF)
            {
                var policy = new BackOfficePolicy
                {
                    ClassName = CommonUtility.GetStringValue(rsCompFields["P_CLASS"].Value),
                    Implemented = CommonUtility.GetBooleanValue(rsCompFields["Implemented"].Value),
                    PolicyName = CommonUtility.GetStringValue(rsCompFields["P_NAME"].Value),
                    CompanyLevelSet = CommonUtility.GetStringValue(rsCompFields["CompanyLevelSet"].Value),
                    VarType = CommonUtility.GetStringValue(rsCompFields["P_VARTYPE"].Value),
                    OtherLevelSet = CommonUtility.GetStringValue(rsCompFields["OtherLevelSet"].Value),
                    PolicyLevel = CommonUtility.GetStringValue(rsCompFields["PolicyLevel"].Value),
                    PolicyValue = CommonUtility.GetStringValue(rsCompFields["PolicyValue"].Value),
                    Sequence = CommonUtility.GetIntergerValue(rsCompFields["P_Seq"].Value)
                };
                result.Add(policy);
                rsComp.MoveNext();
            }
            return result;

        }


    }
}