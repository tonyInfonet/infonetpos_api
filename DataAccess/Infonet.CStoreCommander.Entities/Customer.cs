using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Customer
    {

        private string mvarCode;
        private string mvarName;
        private string mvarAddress_1;
        private string mvarAddress_2;
        private string mvarCity;
        private string mvarProvince;
        private string mvarCountry;
        private string mvarPostal_Code;
        private string mvarPhone;
        private string mvarFax;
        private string mvarWork_Phone;
        private string mvarCell_Phone;
        private string mvarArea_Code;
        private string mvarE_Mail;
        private string mvarToll_Free;
        private string mvarCategory;
        private string mvarContact_1;
        private string mvarContact_2;
        private string mvarLoyalty_Code;
        private double mvarLoyalty_Points;
        private bool mvarAR_Customer;
        private string mvarCustomer_Type;
        private short mvarTerms;
        private float mvarDiscount;
        private float mvarPercent;
        private byte mvarDiscount_Code;
        private byte mvarPrice_Code;
        private double mvarOpening_Balance;
        private double mvarAmount_Paid;
        private double mvarCredit_Limit;
        private double mvarCurrent_Balance;
        private string mvarstatus; //  
        private decimal mvarPointsAwarded;
        private bool mvarUsePO; //05/05/05 Nancy added field UsePO for Purchase Order feature
        private string mvarGroupID; //05/17/06 Nancy added for Fuel Loyalty
        private string mvarGroupName;
        private string mvarDiscountType;
        private float mvarDiscountRate;
        private string mvarFooter;
        private string mvarLoyaltyCard;
        private string mvarLoyaltyExpDate;
        private bool mvarLoyaltyCardSwiped;
        private bool mvarUseFuelRebate; 
        private bool mvarUseFuelRebateDiscount; 
        private string mvarCL_Note;

        private string mvarCustomerCardNum; //Added by sonali
        private string mvarPointCardNum; //
        private string mvarPointCardPhone; //
        private string mvarPointCardSwipe; //
        private double mvarPoints_Redeemed; //
        private bool mvarPointsCard_AllowRedemption; //  
        private double mvarAvailableDollars;
        private double mvarBalance_Points;
        private double mvarPoints_ExchangeRate;
        private bool mvarPointCard_Registered;
        private bool mvarTaxExempt; //  - QITE
        private string mvarPlatenumber; //  - QITE
        private bool mvarMultiUse_PO; // 
        private string mvarTECardnumber; // 
        private string mvarDiscountName; // 
        private string mvarCardProfileID; // 

        public string DisplayLine { get; set; }
        //TODO: Kickback_Removed
        //private string mvarDisplayLine; //   to pass data from KickBack response

        //public void Load()
        //{
        //    //Smriti move this code to manager
        //    //dynamic Policy_Renamed = default(dynamic);

        //    ////    If Me.Code <> ""  Then ' used
        //    //// and default customer code is set, then load it and use it
        //    //if (Policy_Renamed.DefaultCust && Policy_Renamed.DEF_CUST_CODE != "")
        //    //{
        //    //    if (this.Code == Policy_Renamed.DEF_CUST_CODE || this.Code == Chaps_Main.CASH_SALE_CLIENT || this.Code == "")
        //    //    {
        //    //        this.Code = System.Convert.ToString(Policy_Renamed.DEF_CUST_CODE);
        //    //    }
        //    //}

        //    //ADODB.Recordset rs = default(ADODB.Recordset);
        //    //ADODB.Recordset rsTmp = default(ADODB.Recordset);
        //    //if (this.Code != "")
        //    //{

        //    //    rs = Chaps_Main.Get_Records("SELECT * FROM CLIENT WHERE CLIENT.CL_CODE = \'" + this.Code + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //    //    if (!rs.EOF)
        //    //    {
        //    //        if ((Policy_Renamed.DefaultCust && Policy_Renamed.DEF_CUST_CODE != "") && this.Code == Chaps_Main.CASH_SALE_CLIENT)
        //    //        {
        //    //            mvarCode = System.Convert.ToString(Policy_Renamed.DEF_CUST_CODE);
        //    //        }
        //    //        mvarName = System.Convert.ToString((Information.IsDBNull(rs.Fields["Cl_Name"].Value)) ? " " : (rs.Fields["Cl_Name"].Value));
        //    //        mvarAddress_1 = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Add1"].Value)) ? "" : (rs.Fields["CL_Add1"].Value));
        //    //        mvarAddress_2 = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Add2"].Value)) ? "" : (rs.Fields["CL_Add2"].Value));
        //    //        mvarCity = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_City"].Value)) ? "" : (rs.Fields["CL_City"].Value));
        //    //        mvarProvince = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Prov"].Value)) ? "" : (rs.Fields["CL_Prov"].Value));
        //    //        mvarCountry = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Country"].Value)) ? "" : (rs.Fields["CL_Country"].Value));
        //    //        mvarPostal_Code = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Postal"].Value)) ? "" : (rs.Fields["CL_Postal"].Value));
        //    //        mvarPhone = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Phone"].Value)) ? "" : (rs.Fields["CL_Phone"].Value));
        //    //        mvarFax = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Fax"].Value)) ? "" : (rs.Fields["CL_Fax"].Value));
        //    //        mvarWork_Phone = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Off_Ph"].Value)) ? "" : (rs.Fields["CL_Off_Ph"].Value));
        //    //        mvarCell_Phone = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Cell"].Value)) ? "" : (rs.Fields["CL_Cell"].Value));
        //    //        mvarArea_Code = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Acode"].Value)) ? "" : (rs.Fields["CL_Acode"].Value));
        //    //        mvarE_Mail = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_EMail"].Value)) ? "" : (rs.Fields["CL_EMail"].Value));
        //    //        mvarToll_Free = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Toll"].Value)) ? "" : (rs.Fields["CL_Toll"].Value));
        //    //        mvarCategory = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Categ"].Value)) ? "" : (rs.Fields["CL_Categ"].Value));
        //    //        mvarContact_1 = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Cont1"].Value)) ? "" : (rs.Fields["CL_Cont1"].Value));
        //    //        mvarContact_2 = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Cont2"].Value)) ? "" : (rs.Fields["CL_Cont2"].Value));
        //    //        mvarLoyalty_Code = System.Convert.ToString((Information.IsDBNull(rs.Fields["Lo_Num"].Value)) ? "" : (rs.Fields["Lo_Num"].Value));
        //    //        mvarLoyalty_Points = System.Convert.ToDouble((Information.IsDBNull(rs.Fields["Lo_Points"].Value)) ? 0 : (rs.Fields["Lo_Points"].Value));
        //    //        mvarAR_Customer = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["CL_ARCust"].Value)) ? false : (rs.Fields["CL_ARCust"].Value));
        //    //        mvarCustomer_Type = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Type"].Value)) ? "" : (rs.Fields["CL_Type"].Value));
        //    //        mvarTerms = System.Convert.ToInt16((Information.IsDBNull(rs.Fields["Terms"].Value)) ? 0 : (rs.Fields["Terms"].Value));
        //    //        mvarOpening_Balance = System.Convert.ToDouble((Information.IsDBNull(rs.Fields["CL_HistBal"].Value)) ? 0 : (rs.Fields["CL_HistBal"].Value));
        //    //        mvarAmount_Paid = System.Convert.ToDouble((Information.IsDBNull(rs.Fields["CL_HPaid"].Value)) ? 0 : (rs.Fields["CL_HPaid"].Value));
        //    //        mvarCredit_Limit = System.Convert.ToDouble((Information.IsDBNull(rs.Fields["CL_Limit"].Value)) ? 0 : (rs.Fields["CL_Limit"].Value));
        //    //        mvarCurrent_Balance = System.Convert.ToDouble((Information.IsDBNull(rs.Fields["CL_CurBal"].Value)) ? 0 : (rs.Fields["CL_CurBal"].Value));
        //    //        mvarPrice_Code = System.Convert.ToByte((Information.IsDBNull(rs.Fields["Price_Code"].Value)) ? 0 : (rs.Fields["Price_Code"].Value));
        //    //        mvarDiscount_Code = System.Convert.ToByte((Information.IsDBNull(rs.Fields["CUST_DISC"].Value)) ? 0 : (rs.Fields["CUST_DISC"].Value));
        //    //        mvarstatus = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Status"].Value)) ? "A" : (rs.Fields["CL_Status"].Value)); // 
        //    //        mvarUsePO = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UsePO"].Value)) ? false : (rs.Fields["UsePO"].Value)); 
        //    //                                                                                                                                     //  - Multiuse PO
        //    //        if (mvarUsePO == true) // need to do only if use po for this customer
        //    //        {
        //    //            mvarMultiUse_PO = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["MultiUse_PO"].Value)) ? false : (rs.Fields["MultiUse_PO"].Value));
        //    //        }
        //    //        else
        //    //        {
        //    //            mvarMultiUse_PO = false;
        //    //        }
        //    //        //shiny end
        //    //        mvarUseFuelRebate = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseFuelRebate"].Value)) ? false : (rs.Fields["UseFuelRebate"].Value)); 
        //    //        mvarUseFuelRebateDiscount = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseFuelRebateDiscount"].Value)) ? false : (rs.Fields["UseFuelRebateDiscount"].Value)); 
        //    //        mvarDisplayLine = "";

        //    //        
        //    //        mvarGroupID = Strings.Trim(System.Convert.ToString((Information.IsDBNull(rs.Fields["GroupID"].Value)) ? "" : (rs.Fields["GroupID"].Value)));
        //    //        if (!string.IsNullOrEmpty(mvarGroupID))
        //    //        {
        //    //            rsTmp = Chaps_Main.Get_Records("select * from ClientGroup " + " where GroupID=\'" + mvarGroupID + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //    //            if (rsTmp.EOF)
        //    //            {
        //    //                
        //    //                
        //    //                mvarGroupID = "";
        //    //            }
        //    //            else
        //    //            {
        //    //                mvarGroupName = System.Convert.ToString((Information.IsDBNull(rsTmp.Fields["GroupName"].Value)) ? "" : (rsTmp.Fields["GroupName"].Value));
        //    //                mvarDiscountType = System.Convert.ToString((Information.IsDBNull(rsTmp.Fields["DiscountType"].Value)) ? "" : (rsTmp.Fields["DiscountType"].Value));
        //    //                mvarDiscountRate = System.Convert.ToSingle(rsTmp.Fields["DiscountRate"].Value);
        //    //                mvarFooter = System.Convert.ToString((Information.IsDBNull(rsTmp.Fields["LoyaltyFooter"].Value)) ? "" : (rsTmp.Fields["LoyaltyFooter"].Value));
        //    //                mvarDiscountName = System.Convert.ToString((Information.IsDBNull(rsTmp.Fields["DiscountName"].Value)) ? "" : (rsTmp.Fields["DiscountName"].Value));
        //    //            }
        //    //            rsTmp = null;
        //    //        }
        //    //        
        //    //        mvarCL_Note = System.Convert.ToString((Information.IsDBNull(rs.Fields["CL_Note"].Value)) ? "" : (rs.Fields["CL_Note"].Value)); // 
        //    //        mvarPlatenumber = System.Convert.ToString((Information.IsDBNull(rs.Fields["PlateNumber"].Value)) ? "" : (rs.Fields["PlateNumber"].Value)); // 
        //    //        mvarTaxExempt = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["TaxExempt"].Value)) ? false : (rs.Fields["TaxExempt"].Value)); // 
        //    //        mvarTECardnumber = System.Convert.ToString((Information.IsDBNull(rs.Fields["TECardNumber"].Value)) ? "" : (rs.Fields["TECardNumber"].Value)); // 
        //    //TODO: Kickback_Removed
        //    //        //   to reset the points card number and phone that are not set throught loyalty swipe or phone number entry
        //    //        // For KickBack loyalty the customer load and change can happen after the card was swiped and set because the customer is
        //    //        // identified from the card (or phone number). In this case we don't have to reset the customer card number already set
        //    //        //  - When using Gasking card at the tender screen, we are reloading customer. In this case we shouldn't clear trhe kick back card info we used before
        //    //        //                If Policy.Use_KickBack And STFDNumber = "" Then
        //    //        if (Policy_Renamed.Use_KickBack && string.IsNullOrEmpty(Variables.STFDNumber) && Variables.blChargeAcct == false)
        //    //        {
        //    //            // 
        //    //            mvarPointCardNum = "";
        //    //            mvarPointCardPhone = "";
        //    //            mvarPointCardSwipe = "";
        //    //        }
        //    //        //   end
        //    //    }
        //    //    else // if default customer has been deleted from Client table and is still set in the policy, then use cash sale
        //    //    {
        //    //        if (!(Policy_Renamed.DefaultCust && Policy_Renamed.DEF_CUST_CODE != ""))
        //    //        {
        //    //            mvarCode = Chaps_Main.CASH_SALE_CLIENT; // set in English Only because this is saved in sale head table
        //    //        }
        //    //        else
        //    //        {
        //    //            mvarCode = System.Convert.ToString(Policy_Renamed.DEF_CUST_CODE);
        //    //        }
        //    //        mvarName = Chaps_Main.GetResString((short)400);
        //    //        mvarPrice_Code = (byte)1;
        //    //        //  - if we selected a customer and switching backto cash sale , so many properties are not resetting (I added only few properties to fix the issues what I found- Need to review if find any issue)
        //    //        mvarDiscount_Code = (byte)0;
        //    //        mvarstatus = "A";
        //    //        mvarUsePO = false;
        //    //        mvarUseFuelRebate = false;
        //    //        mvarUseFuelRebateDiscount = false;
        //    //        mvarGroupID = "";
        //    //        mvarGroupName = "";
        //    //        mvarDiscountType = "";
        //    //        mvarDiscountRate = 0;
        //    //        mvarDiscountName = ""; // 
        //    //        mvarFooter = "";
        //    //        mvarCL_Note = "";
        //    //        mvarPlatenumber = ""; // 
        //    //        mvarTaxExempt = false; // 
        //    //        mvarTECardnumber = "";
        //    //        mvarDisplayLine = "";
        //    //        // 
        //    //        //   to reset the points card number and phone that are not set throught loyalty swipe or phone number entry
        //    //        // For KickBack loyalty the customer load and change can happen after the card was swiped and set because the customer is
        //    //        // identified from the card (or phone number). In this case we don't have to reset the customer card number already set
        //    //        //  - When using Gasking card at the tender screen, we are reloading customer. In this case we shouldn't clear trhe kick back card info we used before
        //    //        //                If Policy.Use_KickBack And STFDNumber = "" Then
        //    //        if (Policy_Renamed.Use_KickBack && string.IsNullOrEmpty(Variables.STFDNumber) && Variables.blChargeAcct == false)
        //    //        {
        //    //            // 
        //    //            mvarPointCardNum = "";
        //    //            mvarPointCardPhone = "";
        //    //            mvarPointCardSwipe = "";
        //    //        }
        //    //        //   end
        //    //        mvarAR_Customer = false;
        //    //    }
        //    //    rs = null;
        //    //}
        //    //else // if the code is empty string, the policy is set to no, or the default customer is not set
        //    //{
        //    //    mvarCode = Chaps_Main.CASH_SALE_CLIENT;
        //    //    mvarName = Chaps_Main.GetResString((short)400);
        //    //    mvarPrice_Code = (byte)1;
        //    //    mvarAR_Customer = false;
        //    //    //   to reset the points card number and phone that are not set throught loyalty swipe or phone number entry
        //    //    // For KickBack loyalty the customer load and change can happen after the card was swiped and set because the customer is
        //    //    // identified from the card (or phone number). In this case we don't have to reset the customer card number already set
        //    //    //  - When using Gasking card at the tender screen, we are reloading customer. In this case we shouldn't clear trhe kick back card info we used before
        //    //    //                If Policy.Use_KickBack And STFDNumber = "" Then
        //    //    if (Policy_Renamed.Use_KickBack && string.IsNullOrEmpty(Variables.STFDNumber) && Variables.blChargeAcct == false)
        //    //    {
        //    //        // 
        //    //        mvarPointCardNum = "";
        //    //        mvarPointCardPhone = "";
        //    //        mvarPointCardSwipe = "";
        //    //    }
        //    //    //   end
        //    //    mvarDisplayLine = "";
        //    //}
        //    //END: Kickback_Removed
        //}

        //public void Save()
        //{
        //    //Smriti move this code to manager
        //    //ADODB.Recordset rs = default(ADODB.Recordset);
        //    //if (this.Code != "")
        //    //{
        //    //    rs = Chaps_Main.Get_Records("select * from Client", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly);
        //    //    rs.Find(Criteria: "CL_CODE=\'" + this.Code + "\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
        //    //    if (rs.EOF)
        //    //    {
        //    //        rs.AddNew();
        //    //        rs.Fields["cl_code"].Value = this.Code; // Nicolette added for new customers
        //    //    }
        //    //    rs.Fields["Cl_Name"].Value = mvarName;
        //    //    rs.Fields["CL_Add1"].Value = mvarAddress_1;
        //    //    rs.Fields["CL_Add2"].Value = mvarAddress_2;
        //    //    rs.Fields["CL_City"].Value = mvarCity;
        //    //    rs.Fields["CL_Prov"].Value = mvarProvince;
        //    //    rs.Fields["CL_Country"].Value = mvarCountry;
        //    //    rs.Fields["CL_Postal"].Value = mvarPostal_Code;
        //    //    rs.Fields["CL_Phone"].Value = mvarPhone;
        //    //    rs.Fields["CL_Fax"].Value = mvarFax;
        //    //    rs.Fields["CL_Off_Ph"].Value = mvarWork_Phone;
        //    //    rs.Fields["CL_Cell"].Value = mvarCell_Phone;
        //    //    rs.Fields["CL_Acode"].Value = mvarArea_Code;
        //    //    rs.Fields["CL_EMail"].Value = mvarE_Mail;
        //    //    rs.Fields["CL_Toll"].Value = mvarToll_Free;
        //    //    rs.Fields["CL_Categ"].Value = mvarCategory;
        //    //    rs.Fields["CL_Cont1"].Value = mvarContact_1;
        //    //    rs.Fields["CL_Cont2"].Value = mvarContact_2;
        //    //    rs.Fields["Lo_Num"].Value = mvarLoyalty_Code;
        //    //    rs.Fields["Lo_Points"].Value = mvarLoyalty_Points;
        //    //    rs.Fields["CL_ARCust"].Value = mvarAR_Customer;
        //    //    rs.Fields["CL_Type"].Value = mvarCustomer_Type;
        //    //    rs.Fields["Terms"].Value = mvarTerms;
        //    //    rs.Fields["Discount_Days"].Value = mvarDiscount;
        //    //    rs.Fields["Early_Pay_Percent"].Value = mvarPercent;
        //    //    rs.Fields["CUST_DISC"].Value = mvarDiscount_Code;
        //    //    rs.Fields["Price_Code"].Value = mvarPrice_Code;
        //    //    if (mvarstatus.Trim().Length == 0)
        //    //    {
        //    //        rs.Fields["CL_Status"].Value = "A";
        //    //    }
        //    //    else
        //    //    {
        //    //        rs.Fields["CL_Status"].Value = mvarstatus; //  For Customer Status ( A- Active, F- Frozen for Lyalty , I-inactive)
        //    //    }
        //    //    rs.Fields["CL_HistBal"].Value = mvarOpening_Balance;
        //    //    rs.Fields["CL_HPaid"].Value = mvarAmount_Paid;
        //    //    rs.Fields["CL_Limit"].Value = mvarCredit_Limit;
        //    //    rs.Fields["CL_CurBal"].Value = mvarCurrent_Balance;
        //    //    rs.Fields["UsePO"].Value = mvarUsePO; 
        //    //    rs.Fields["MultiUse_PO"].Value = mvarMultiUse_PO; // 
        //    //    rs.Fields["UseFuelRebate"].Value = mvarUseFuelRebate; 
        //    //    rs.Fields["UseFuelRebateDiscount"].Value = mvarUseFuelRebateDiscount; 
        //    //    rs.Fields["CL_Note"].Value = mvarCL_Note; // 
        //    //    rs.Fields["PlateNumber"].Value = mvarPlatenumber; // 
        //    //    rs.Fields["TaxExempt"].Value = mvarTaxExempt; // 
        //    //    rs.Fields["TECardNumber"].Value = mvarTECardnumber; // 
        //    //                                                        //             

        //    //    rs.Update();
        //    //    rs = null;
        //    //}
        //    //else
        //    //{
        //    //    //TIMsgbox "Customer Code Must not be blank." & vbCrLf & _
        //    //    //"Customer record was not added.", vbCritical + vbOKOnly, "Customer Code Required"
        //    //    MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
        //    //    Chaps_Main.DisplayMessage(0, (short)8108,temp_VbStyle, null, (byte)0);
        //    //}

        //}


        public decimal PointsAwarded
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPointsAwarded;
                return returnValue;
            }
            set
            {
                mvarPointsAwarded = value;
            }
        }


        public double Current_Balance
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarCurrent_Balance;
                return returnValue;
            }
            set
            {
                mvarCurrent_Balance = value;
            }
        }


        public double Credit_Limit
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarCredit_Limit;
                return returnValue;
            }
            set
            {
                mvarCredit_Limit = value;
            }
        }


        public double Amount_Paid
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarAmount_Paid;
                return returnValue;
            }
            set
            {
                mvarAmount_Paid = value;
            }
        }


        public double Opening_Balance
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarOpening_Balance;
                return returnValue;
            }
            set
            {
                mvarOpening_Balance = value;
            }
        }


        public byte Price_Code
        {
            get
            {
                byte returnValue;
                if (mvarPrice_Code == 0)
                {
                    returnValue = (byte)1;
                }
                else
                {
                    returnValue = mvarPrice_Code;
                }
                return returnValue;
            }
            set
            {
                mvarPrice_Code = value;
            }
        }


        public byte Discount_Code
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarDiscount_Code;
                return returnValue;
            }
            set
            {
                mvarDiscount_Code = value;
            }
        }


        public float Percent
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarPercent;
                return returnValue;
            }
            set
            {
                mvarPercent = value;
            }
        }


        public float Discount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarDiscount;
                return returnValue;
            }
            set
            {
                mvarDiscount = value;
            }
        }


        public short Terms
        {
            get
            {
                return Terms = mvarTerms;
            }
            set
            {
                mvarTerms = value;
            }
        }


        public string Customer_Type
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCustomer_Type;
                return returnValue;
            }
            set
            {
                mvarCustomer_Type = value;
            }
        }


        public bool AR_Customer
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAR_Customer;
                return returnValue;
            }
            set
            {
                mvarAR_Customer = value;
            }
        }

        

        public bool UsePO
        {
            get
            {
               return UsePO = mvarUsePO;
            }
            set
            {
                mvarUsePO = value;
            }
        }
        

        

        public bool UseFuelRebate
        {
            get
            {
                 return UseFuelRebate = mvarUseFuelRebate;
            }
            set
            {
                mvarUseFuelRebate = value;
            }
        }
        

        

        public bool UseFuelRebateDiscount
        {
            get
            {
                 return UseFuelRebateDiscount = mvarUseFuelRebateDiscount;
            }
            set
            {
                mvarUseFuelRebateDiscount = value;
            }
        }
        

        

        // discount
        public string GroupID
        {
            get
            {
               return GroupID = mvarGroupID;
            }
            set
            {             
                mvarGroupID = value;
                

            }
        }
        //   end

        public string GroupName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarGroupName;
                return returnValue;
            }
            set
            {
                mvarGroupName = value;
            }
        }

        public string DiscountType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDiscountType;
                return returnValue;
            }

            set
            {
                mvarDiscountType = value;
            }
        }

        public float DiscountRate
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarDiscountRate;
                return returnValue;
            }

            set
            {
                mvarDiscountRate = value;
            }
        }

        public string Footer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarFooter;
                return returnValue;
            }
            set
            {
                mvarFooter = value;
            }
        }


        public string LoyaltyCard
        {
            get
            {
                string returnValue = "";
                returnValue = mvarLoyaltyCard;
                return returnValue;
            }
            set
            {
                mvarLoyaltyCard = value;
            }
        }


        public string LoyaltyExpDate
        {
            get
            {
                string returnValue = "";
                returnValue = mvarLoyaltyExpDate;
                return returnValue;
            }
            set
            {
                mvarLoyaltyExpDate = value;
            }
        }

        

        public bool LoyaltyCardSwiped
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLoyaltyCardSwiped;
                return returnValue;
            }
            set
            {
                mvarLoyaltyCardSwiped = value;
            }
        }
        


        public double Loyalty_Points
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarLoyalty_Points;
                return returnValue;
            }
            set
            {
                mvarLoyalty_Points = value;
            }
        }


        public string Loyalty_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarLoyalty_Code;
                return returnValue;
            }
            set
            {
                mvarLoyalty_Code = value;
            }
        }


        public string Contact_2
        {
            get
            {
                string returnValue = "";
                returnValue = mvarContact_2;
                return returnValue;
            }
            set
            {
                mvarContact_2 = value;
            }
        }


        public string Contact_1
        {
            get
            {
                string returnValue = "";
                returnValue = mvarContact_1;
                return returnValue;
            }
            set
            {
                mvarContact_1 = value;
            }
        }


        public string Category
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCategory;
                return returnValue;
            }
            set
            {
                mvarCategory = value;
            }
        }


        public string Toll_Free
        {
            get
            {
                string returnValue = "";
                returnValue = mvarToll_Free;
                return returnValue;
            }
            set
            {
                mvarToll_Free = value;
            }
        }


        public string E_Mail
        {
            get
            {
                string returnValue = "";
                returnValue = mvarE_Mail;
                return returnValue;
            }
            set
            {
                mvarE_Mail = value;
            }
        }


        public string Area_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarArea_Code;
                return returnValue;
            }
            set
            {
                mvarArea_Code = value;
            }
        }


        public string Cell_Phone
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCell_Phone;
                return returnValue;
            }
            set
            {
                mvarCell_Phone = value;
            }
        }


        public string Work_Phone
        {
            get
            {
                string returnValue = "";
                returnValue = mvarWork_Phone;
                return returnValue;
            }
            set
            {
                mvarWork_Phone = value;
            }
        }


        public string Fax
        {
            get
            {
                string returnValue = "";
                returnValue = mvarFax;
                return returnValue;
            }
            set
            {
                mvarFax = value;
            }
        }


        public string Phone
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPhone;
                return returnValue;
            }
            set
            {
                mvarPhone = value;
            }
        }


        public string Postal_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPostal_Code;
                return returnValue;
            }
            set
            {
                mvarPostal_Code = value;
            }
        }


        public string Province
        {
            get
            {
                string returnValue = "";
                returnValue = mvarProvince;
                return returnValue;
            }
            set
            {
                mvarProvince = value;
            }
        }


        public string Country
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCountry;
                return returnValue;
            }
            set
            {
                mvarCountry = value;
            }
        }


        public string City
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCity;
                return returnValue;
            }
            set
            {
                mvarCity = value;
            }
        }


        public string Address_2
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAddress_2;
                return returnValue;
            }
            set
            {
                mvarAddress_2 = value;
            }
        }


        public string Address_1
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAddress_1;
                return returnValue;
            }
            set
            {
                mvarAddress_1 = value;
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


        public string Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCode;
                return returnValue;
            }
            set
            {
                mvarCode = value;
            }
        }


        public string CL_Status
        {
            get
            {
                return CL_Status = mvarstatus;
            }
            set
            {
                mvarstatus = value;
            }
        }

        //   - Customer note

        public string CL_Note
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCL_Note;
                return returnValue;
            }
            set
            {
                mvarCL_Note = value;
            }
        }
        //shiny end

        //

        public string PointCardNum
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPointCardNum;
                return returnValue;
            }
            set
            {
                mvarPointCardNum = value;
            }
        }
        /* Added by sonali */
        public string CustomerCardNum
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCustomerCardNum;
                return returnValue;
            }
            set
            {
                mvarCustomerCardNum = value;
            }
        }
        /* ended by sonali */

        public string PointCardPhone
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPointCardPhone;
                return returnValue;
            }
            set
            {
                mvarPointCardPhone = value;
            }
        }


        public string PointCardSwipe
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPointCardSwipe;
                return returnValue;
            }
            set
            {
                mvarPointCardSwipe = value;
            }
        }

        //

        public double Points_Redeemed
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarPoints_Redeemed;
                return returnValue;
            }
            set
            {
                mvarPoints_Redeemed = value;
            }
        }
        //End - SV

        public double AvailableDollars
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarAvailableDollars;
                return returnValue;
            }
            set
            {
                mvarAvailableDollars = value;
            }
        }

        // Feb 18, 2009: Nicolette added to keep the points available from KickBack
        // Added a new property because KickBack has to separated from local loyalty program
        // otherwise we could use Loaylty_Points property
        //TODO: Kickback_Removed
        public double Balance_Points
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarBalance_Points;
                return returnValue;
            }
            set
            {
                mvarBalance_Points = value;
            }
        }
        //END: Kickback_Removed

        public double Points_ExchangeRate
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarPoints_ExchangeRate;
                return returnValue;
            }
            set
            {
                mvarPoints_ExchangeRate = value;
            }
        }
        // Feb 18, 2009: Nicolette end

        // Apr 03, 2009 Nicolette added

        public bool PointCard_Registered
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPointCard_Registered;
                return returnValue;
            }
            set
            {
                mvarPointCard_Registered = value;
            }
        }
        // Apr 03, 2009 Nicolette end

        // QITE

        public string PlateNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPlatenumber;
                return returnValue;
            }
            set
            {
                mvarPlatenumber = value;
            }
        }


        public bool TaxExempt
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTaxExempt;
                return returnValue;
            }
            set
            {
                mvarTaxExempt = value;
            }
        }

        //Shiny end-QITE

        //  

        public bool PointsCard_AllowRedemption
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPointsCard_AllowRedemption;
                return returnValue;
            }
            set
            {
                mvarPointsCard_AllowRedemption = value;
            }
        }
        // 

        public bool MultiUse_PO
        {
            get
            {
                return MultiUse_PO = mvarMultiUse_PO;
            }
            set
            {
                mvarMultiUse_PO = value;
            }
        }
        // Band card member

        public string TECardNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTECardnumber;
                return returnValue;
            }
            set
            {
                mvarTECardnumber = value;
            }
        }
        // Discount name

        public string DiscountName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDiscountName;
                return returnValue;
            }
            set
            {
                mvarDiscountName = value;
            }
        }
        // 

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

        //  
        //TODO: Kickback_Removed
        //public string DisplayLine
        //{
        //    get
        //    {
        //        string returnValue = "";
        //        returnValue = mvarDisplayLine;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarDisplayLine = value;
        //    }
        //}

        //   modified the function to handle entry from card swipe
        // and to set the customer code and load the customer
        // Modified Dec 22, 2009 if the customer was not identified by card number
        // return the entered value and let Loyalty Program decide is valid or not;
        // this is a quick fix for the wrong design, has to be properly redone
        // March 03, 2009: KickBack cards are not used to identify the customer
        // This is GasKing requirement, only GK cards should be used to change the customer
        // This function only sets the PointsCard or Phone number to be sent to Kick Back
        // public void Set_Customer_KickBack_Data(string InputValue)
        //{
        //Smriti move this code to manager
        //ADODB.Recordset rsTemp = default(ADODB.Recordset);
        //short pos = 0;
        //short posqm = 0;
        //bool boolIsPhoneNumber = false;
        //string PointCardNumber = "";
        //string Cardnumber;
        //string Code;

        //if (InputValue.Trim() == "")
        //{
        //    return;
        //}
        //boolIsPhoneNumber = true;

        //// May 26, 2009: Nicolette changed to fix the crash for track1 being enabled
        //// look for "?" after the ";" to consider only track2. If any of ";" or "?"
        //// are not found, system assumes that is a phone number
        //pos = (short)(InputValue.IndexOf(";") + 1);
        ////    posqm = InStr(1, InputValue, "?") ' May 26, 2009: Nicolette see comment
        //if (pos > 0)
        //{
        //    posqm = (short)(pos.ToString().IndexOf(InputValue) + 1);
        //}
        //else
        //{
        //    posqm = (short)0;
        //}
        //if (posqm > 0 & pos > 0)
        //{
        //    boolIsPhoneNumber = false;
        //    pos = (short)(InputValue.IndexOf(";") + 1);
        //    //        If pos < 0 Then Exit Sub   ' May 26, 2009: Nicolette see comment
        //    PointCardNumber = InputValue.Substring(pos + 1 - 1, posqm - pos - 1);
        //}

        //if (boolIsPhoneNumber)
        //{
        //    rsTemp = Chaps_Main.Get_Records("SELECT CustomerCardNum, PointCardNum FROM KickBack  WHERE PhoneNum=\'" + InputValue.Trim() + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //    if (!rsTemp.EOF)
        //    {
        //        Cardnumber = System.Convert.ToString(rsTemp.Fields["CustomerCardNum"].Value);
        //        mvarPointCardNum = System.Convert.ToString(rsTemp.Fields["PointCardNum"].Value);
        //        mvarPointCardPhone = InputValue.Trim();
        //        mvarPointCardSwipe = "1"; //0-from database based on a swiped GK card, 1-from phone number, 2-swiped
        //    }
        //    else
        //    {
        //        // if the phone number is not in the database, send to Kick Back to validate
        //        Cardnumber = InputValue.Trim();
        //        mvarPointCardNum = InputValue.Trim();
        //        mvarPointCardPhone = InputValue.Trim();
        //        mvarPointCardSwipe = "1"; //0-from database based on a swiped GK card, 1-from phone number, 2-swiped
        //    }
        //    mvarPointsCard_AllowRedemption = false; // no redemption for phone number
        //}
        //else
        //{
        //    rsTemp = Chaps_Main.Get_Records("SELECT CustomerCardNum, PhoneNum FROM KickBack  WHERE PointCardNum=\'" + PointCardNumber.Trim() + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //    if (!rsTemp.EOF)
        //    {
        //        Cardnumber = System.Convert.ToString(rsTemp.Fields["CustomerCardNum"].Value);
        //        mvarPointCardNum = PointCardNumber.Trim();
        //        mvarPointCardPhone = System.Convert.ToString(rsTemp.Fields["phonenum"].Value);
        //        mvarPointCardSwipe = "2"; //0-from database, 1-from phone number, 2-swiped
        //    }
        //    else
        //    {
        //        // if the phone number is not in the database, send to Kick Back to validate
        //        Cardnumber = PointCardNumber.Trim();
        //        mvarPointCardNum = PointCardNumber.Trim();
        //        mvarPointCardPhone = PointCardNumber.Trim();
        //        mvarPointCardSwipe = "2"; //0-from database, 1-from phone number, 2-swiped
        //    }

        //    //   if Kick Back card is swiped then the system should allow redemption
        //    // redemption is also based on the KickBack response (card should be registered, otherwise no redemption allowed)
        //    mvarPointsCard_AllowRedemption = true;
        //}

        //// not
        //// Only Gas King cards sets a customer, otherwise is the generic customer or cash sale
        /////    Set rsTemp = Get_Records("SELECT CL_Code FROM ClientCard WHERE CardNum='" & _
        ///
        /////    If Not rsTemp.EOF Then
        /////        Code = rsTemp![cl_code]  ' this cause the Customer_Change function to set correct prices, discounts and labels on the form for selected customer
        /////    Else
        /////        Code = ""
        /////    End If
        /////
        /////    Get_CustomerCode = Code
        //rsTemp = null;

        // }
        //End - SV
        //END: Kickback_Removed
        private void Class_Initialize_Renamed()
        {
            mvarUseFuelRebate = false; 
            mvarUseFuelRebateDiscount = false; 
            mvarPointCardNum = "";
            mvarPointCardPhone = "";
            mvarPointCardSwipe = "";
            mvarPointCard_Registered = false;
            mvarPointsCard_AllowRedemption = false;
            mvarMultiUse_PO = false;
            mvarCardProfileID = "";
        }

        public Customer()
        {
            Class_Initialize_Renamed();
        }
      

    
      
    }
}
