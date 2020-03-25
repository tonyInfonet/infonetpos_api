using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Sale
    {
        public string STFDNumber { get; set; }
        public bool USE_LOYALTY;
        public string LOYAL_TYPE;
        public short Loyal_pricecode;
        public bool CUST_DISC;
        public short Loydiscode;
        public bool PROD_DISC;
        public bool Combine_Policy;
        private bool mvarLoadingTemp;
        private DateTime mvarSale_Date;
        private DateTime mvarSale_Time;
        private int mvarSale_Num;
        private string mvarSale_Client;
        private string mvarSale_Type;
        private decimal mvarSale_Line_Disc;
        private decimal mvarSale_Invc_Disc;
        private decimal mvarSale_Assoc;
        private decimal mvarSale_Amount;
        private decimal mvarSale_Tender;
        private decimal mvarSale_Change;
        private decimal mvarSale_Payment;
        private decimal mvarSale_Deposit;
        private decimal mvarSale_Credits;
        private byte mvarRegister;
        private byte mvarTill;
        private string mvarReason_Code;
        private string mvarReason_Type;
        private decimal mvarPointsAmount;
        private decimal mvarLoyaltyPoints;
        private bool mvarApplyTaxes;
        private bool mvarApplyCharges;
        private bool mvarXRigor;
        private decimal Sum_AMT;
        private decimal Sum_VOL;
        private double mvarCurPoints;
        private decimal mvarOverPayment;
        private string mvarVendor; // Only need code to save into database  
        private Sale_Lines mvarSale_Lines;
        private Sale_Totals mvarSale_Totals;
        private SP_Prices mvarSP_Prices;
        private Customer mvarCustomer;
        private Return_Reason mvarReturnReason;
        private Restrictions mvarRestrictions;
        private int mvarVoid_Num;
        private bool mvarPayment; //  'For Crash Recovery
        private string mvarTreatyNumber;
        private float mvarTotalTaxSaved;
        private string mvarAR_PO;
        private bool mvarForCorrection;

        private decimal mvarCouponTotal;
        private string mvarCouponID;

        private bool mvarDeletePrepay;

        private bool mvarUpsell;
        private bool mvarHasRebateLine;
        private float mvarNoOfPromo; //  
        private bool mvarMakingPromo; //  
        private bool mvarGoLastLine; //  
        private bool mvarReverseRunaway; //  
        private decimal mvarCBonusTotal; // 
        private bool mvarReversePumpTest; // 
        private string mvarTreatyName; // 
        private byte mvarShift;
        //TODO: Ezipin_Removed
        //private bool mvarHasEziProducts; //
        private bool mvarHasCarwashProducts; //
        private string mvarCarwashReceipt; //
        private bool mvarTECustomerChange; // 
        private bool mvarCarwashProcessed; //
        private string mvarReferenceNumber; //    changed to string
        private bool mvarEligibleTaxEx; //  
        private bool mvarApply_CustomerChange; //   run Customer_Change procedure without a customer change (to set propreties/discounts)
        private bool mvarTaxForTaxExempt; //  
        private bool isWexTenderUsed;

        public bool IsWexTenderUsed
        {
            get
            {
                return isWexTenderUsed;
            }
            set
            {
                isWexTenderUsed = value;
            }
        }

        public bool Upsell
        {
            get
            {
                return mvarUpsell;
            }
            set
            {
                mvarUpsell = value;
            }
        }

        public bool DeletePrepay
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarDeletePrepay;
                return returnValue;
            }
            set
            {
                mvarDeletePrepay = value;
            }
        }



        public bool ForCorrection
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarForCorrection;
                return returnValue;
            }
            set
            {
                mvarForCorrection = value;
            }
        }



        public bool HasRebateLine
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarHasRebateLine;
                return returnValue;
            }
            set
            {
                mvarHasRebateLine = value;
            }
        }


        // 
        public bool Payment
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPayment;
                return returnValue;
            }
            set
            {
                mvarPayment = value;
            }
        }
        //Shiny end

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


        public bool LoadingTemp
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLoadingTemp;
                return returnValue;
            }
            set
            {
                mvarLoadingTemp = value;
            }
        }

        public bool XRigor
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarXRigor;
                return returnValue;
            }
            set
            {
                mvarXRigor = value;
            }
        }


        public SaleHead SaleHead { get; set; }


        public Customer Customer
        {
            get
            {
                Customer returnValue = default(Customer);
                returnValue = mvarCustomer;
                return returnValue;
            }
            set
            {
                mvarCustomer = value;
            }
        }


        public SP_Prices Prices
        {
            get
            {
                SP_Prices returnValue = default(SP_Prices);
                returnValue = mvarSP_Prices;
                return returnValue;
            }
            set
            {
                mvarSP_Prices = value;
            }
        }


        public Return_Reason Return_Reason
        {
            get
            {
                Return_Reason returnValue = default(Return_Reason);
                returnValue = mvarReturnReason;
                return returnValue;
            }
            set
            {
                mvarReturnReason = value;
            }
        }


        public Restrictions Restrictions
        {
            get
            {
                Restrictions returnValue = default(Restrictions);
                if (mvarRestrictions == null)
                {
                    mvarRestrictions = new Restrictions();
                }
                returnValue = mvarRestrictions;
                return returnValue;
            }
            set
            {
                mvarRestrictions = value;
            }
        }


        public decimal PointsAmount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPointsAmount;
                return returnValue;
            }
            set
            {
                mvarPointsAmount = value;
            }
        }

        public decimal OverPayment
        {
            get
            {
                return mvarOverPayment;
            }
            set
            {
                mvarOverPayment = value;
            }
        }


        public string TreatyNumber
        {
            get
            {
                return mvarTreatyNumber;
            }
            set
            {
                mvarTreatyNumber = value;
            }
        }




        public decimal CouponTotal
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarCouponTotal;
                return returnValue;
            }
            set
            {
                mvarCouponTotal = value;
            }
        }


        public string CouponID
        {
            get
            {
                return mvarCouponID;
            }
            set
            {
                mvarCouponID = value;
            }
        }


        public Sale_Totals Sale_Totals
        {
            get
            {
                Sale_Totals returnValue = default(Sale_Totals);
                if (mvarSale_Totals == null)
                {
                    mvarSale_Totals = new Sale_Totals();
                }
                returnValue = mvarSale_Totals;
                return returnValue;
            }
            set
            {
                mvarSale_Totals = value;
            }
        }


        public Sale_Lines Sale_Lines
        {
            get
            {
                if (mvarSale_Lines == null)
                {
                    mvarSale_Lines = new Sale_Lines();
                }
                return mvarSale_Lines;
            }
            set
            {
                mvarSale_Lines = value;
            }
        }


        public byte Register
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarRegister;
                return returnValue;
            }
            set
            {
                mvarRegister = value;
            }
        }


        public byte TillNumber
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarTill;
                return returnValue;
            }
            set
            {
                mvarTill = value;
            }
        }


        public decimal Sale_Credits
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Credits;
                return returnValue;
            }
            set
            {
                mvarSale_Credits = value;
            }
        }


        public string Sale_Type
        {
            get
            {
                return mvarSale_Type;
            }
            set
            {
                mvarSale_Type = value;
            }
        }


        public bool ApplyTaxes
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarApplyTaxes;
                return returnValue;
            }
            set { mvarApplyTaxes = value; }
        }


        public bool ApplyCharges
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarApplyCharges;
                return returnValue;
            }
            set
            {
                mvarApplyCharges = value;
            }
        }


        public decimal Sale_Deposit
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Deposit;
                return returnValue;
            }
            set
            {
                mvarSale_Deposit = value;
            }
        }


        public decimal Sale_Payment
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Payment;
                return returnValue;
            }
            set
            {
                mvarSale_Payment = value;
            }
        }


        public decimal Sale_Change
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Change;
                return returnValue;
            }
            set
            {
                mvarSale_Change = value;
            }
        }


        public decimal Sale_Tender
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Tender;
                return returnValue;
            }
            set
            {
                mvarSale_Tender = value;
            }
        }


        public decimal Sale_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Amount;
                return returnValue;
            }
            set
            {
                mvarSale_Amount = value;
            }
        }


        public decimal Sale_Assoc
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Assoc;
                return returnValue;
            }
            set
            {
                mvarSale_Assoc = value;
            }
        }


        public decimal Sale_Invc_Disc
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Invc_Disc;
                return returnValue;
            }
            set
            {
                mvarSale_Invc_Disc = value;
            }
        }


        public decimal Sale_Line_Disc
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Line_Disc;
                return returnValue;
            }
            set
            {
                mvarSale_Line_Disc = value;
            }
        }


        public string Sale_Client
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSale_Client;
                return returnValue;
            }
            set
            {
                mvarSale_Client = value;
            }
        }


        public int Sale_Num
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarSale_Num;
                return returnValue;
            }
            set
            {
                mvarSale_Num = value;
            }
        }


        public DateTime Sale_Date
        {
            get
            {
                return mvarSale_Date;
            }
            set
            {
                mvarSale_Date = value;
            }
        }


        public DateTime Sale_Time
        {
            get
            {
                return mvarSale_Time;
            }
            set
            {
                mvarSale_Time = value;
            }
        }


        public decimal LoyaltyPoints
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarLoyaltyPoints;
                return returnValue;
            }
            set
            {
                mvarLoyaltyPoints = value;
            }
        }


        public string Vendor
        {
            get
            {
                return mvarVendor;
            }
            set
            {
                mvarVendor = value;
            }
        }





        public float TotalTaxSaved
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTotalTaxSaved;
                return returnValue;
            }
            set
            {
                mvarTotalTaxSaved = value;
            }
        }




        public string AR_PO
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAR_PO;
                return returnValue;
            }
            set
            {
                mvarAR_PO = value;
            }
        }
        //End - SV

        //  

        public float NoOfPromo
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarNoOfPromo;
                return returnValue;
            }
            set
            {
                mvarNoOfPromo = value;
            }
        }


        public bool MakingPromo
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarMakingPromo;
                return returnValue;
            }
            set
            {
                mvarMakingPromo = value;
            }
        }
        // End  

        public bool GoLastLine
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarGoLastLine;
                return returnValue;
            }
            set
            {
                mvarGoLastLine = value;
            }
        }

        public bool ReverseRunaway
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarReverseRunaway;
                return returnValue;
            }
            set
            {
                mvarReverseRunaway = value;
            }
        }
        //Shiny Aug6, 2009- pumptest
        public bool ReversePumpTest
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarReversePumpTest;
                return returnValue;
            }
            set
            {
                mvarReversePumpTest = value;
            }
        }


        public decimal CBonusTotal
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarCBonusTotal;
                return returnValue;
            }
            set
            {
                mvarCBonusTotal = value;
            }
        }

        // 
        public string TreatyName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTreatyName;
                return returnValue;
            }
            set
            {
                mvarTreatyName = value;
            }
        }
        //  - for reprint receipt to show correct receipt


        public byte Shift
        {
            get
            {
                return mvarShift;
            }
            set
            {
                mvarShift = value;
            }
        }

        //shiny end

        //
        //TODO: Ezipin_Removed
        //public bool HasEziProducts
        //{
        //    get
        //    {
        //        bool returnValue = false;
        //        returnValue = mvarHasEziProducts;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarHasEziProducts = value;
        //    }
        //}
        //End - SV


        #region Code added by manish for carwash
        public bool HasCarwashProducts
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarHasCarwashProducts;
                return returnValue;
            }
            set
            {
                mvarHasCarwashProducts = value;
            }
        }
      
        
        public string CarwashReceipt
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCarwashReceipt;
                return returnValue;
            }
            set
            {
                mvarCarwashReceipt = value;
            }
        }

        public bool IsCarwashProcessed
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCarwashProcessed;
                return returnValue;
            }
            set
            {
                mvarCarwashProcessed = value;
            }
        }

        #endregion
        
        public bool TECustomerChange
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTECustomerChange;
                return returnValue;
            }
            set
            {
                mvarTECustomerChange = value;
            }
        }


        public string ReferenceNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarReferenceNumber;
                return returnValue;
            }
            set
            {
                mvarReferenceNumber = value;
            }
        }

        //  

        public bool EligibleTaxEx
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarEligibleTaxEx;
                return returnValue;
            }
            set
            {
                mvarEligibleTaxEx = value;
            }
        }
        //   end

        //  

        public bool Apply_CustomerChange
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarApply_CustomerChange;
                return returnValue;
            }
            set
            {
                mvarApply_CustomerChange = value;
            }
        }
        //   end

        //  

        public bool TaxForTaxExempt
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTaxForTaxExempt;
                return returnValue;
            }
            set
            {
                mvarTaxForTaxExempt = value;
            }
        }


        private void Class_Initialize_Renamed()
        {
            mvarSale_Totals = new Sale_Totals();
            mvarSale_Lines = new Sale_Lines();
            mvarCustomer = new Customer();
            mvarSP_Prices = new SP_Prices();
            mvarReturnReason = new Return_Reason();
            mvarRestrictions = new Restrictions();
            CustomerDisplay = new CustomerDisplay();
            mvarHasRebateLine = false;

            mvarSale_Date = DateTime.Now;
            mvarSale_Num = 0;
            mvarVoid_Num = 0;
            mvarSale_Client = "";
            mvarSale_Line_Disc = 0;
            mvarSale_Invc_Disc = 0;
            mvarSale_Assoc = 0;
            mvarSale_Amount = 0;
            mvarSale_Tender = 0;
            mvarSale_Change = 0;
            mvarSale_Payment = 0;
            mvarSale_Deposit = 0;
            mvarSale_Credits = 0;
            mvarPointsAmount = 0;
            mvarVoid_Num = 0;
            mvarApplyTaxes = true;
            mvarApplyCharges = true;
            //TODO: Ezipin_Removed
            // mvarHasEziProducts = false; //
            //TODO : Carwash_Removed
            //mvarHasCarwashProducts = false; //
            // mvarCarwashProcessed = false; //
            mvarEligibleTaxEx = false; //  
            mvarApply_CustomerChange = false; //  
            mvarTaxForTaxExempt = false; //  

            // Load_Taxes();
            //Smriti move this code to manager
            //// Remove the sale from the temporary files.
            //object recordsAffected = null;
            //Chaps_Main.dbTemp.Execute("Delete  from SaleHead  Where SaleHead.Sale_No = " + System.Convert.ToString(this.Sale_Num) + " " + " and TILL=" + Till.Number, out recordsAffected, (int)ADODB.CommandTypeEnum.adCmdText | (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
            //Variables.iRecsAffected = (short)recordsAffected;
            //Chaps_Main.dbTemp.Execute("Delete  from SaleLine  Where SaleLine.Sale_No = " + System.Convert.ToString(this.Sale_Num) + " " + " and TILL_NUM=" + Till.Number, out recordsAffected, (int)ADODB.CommandTypeEnum.adCmdText | (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
            //Variables.iRecsAffected = (short)recordsAffected;
            //Chaps_Main.dbTemp.Execute("Delete  from SLineReason Where SLineReason.Sale_No = " + System.Convert.ToString(this.Sale_Num) + " " + " and TILL_NUM=" + Till.Number, out recordsAffected, (int)ADODB.CommandTypeEnum.adCmdText | (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
            //Variables.iRecsAffected = (short)recordsAffected;
            //
            //Chaps_Main.dbTemp.Execute("Delete  from CardSales Where CardSales.SALE_NO = " + System.Convert.ToString(this.Sale_Num) + " " + " and TILL_NUM=" + Till.Number, out recordsAffected, (int)ADODB.CommandTypeEnum.adCmdText | (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
            //Variables.iRecsAffected = (short)recordsAffected;
            //
            //Chaps_Main.dbTemp.Execute("Delete FROM SaleTotals Where Sale_No = " + System.Convert.ToString(this.Sale_Num), out recordsAffected, (int)ADODB.CommandTypeEnum.adCmdText | (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords); // change
            //Variables.iRecsAffected = (short)recordsAffected;

            //USE_LOYALTY = System.Convert.ToBoolean(Policy_Renamed.USE_LOYALTY);
            //LOYAL_TYPE = System.Convert.ToString(Policy_Renamed.LOYAL_TYPE);
            //Loyal_pricecode = System.Convert.ToInt16(Policy_Renamed.LOYAL_PRICE);
            //CUST_DISC = System.Convert.ToBoolean(Policy_Renamed.CUST_DISC);
            //Loydiscode = System.Convert.ToInt16(Policy_Renamed.LOYAL_DISC);
            //PROD_DISC = System.Convert.ToBoolean(Policy_Renamed.PROD_DISC);
            //Combine_Policy = System.Convert.ToBoolean(Policy_Renamed.COMBINE_LINE);
            //mvarXRigor = System.Convert.ToBoolean(Policy_Renamed.X_RIGOR);
            mvarUpsell = false;
            mvarDeletePrepay = false;
            mvarReverseRunaway = false; // 
            mvarReversePumpTest = false; // 
            mvarTECustomerChange = false; // 
        }
        public Sale()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mvarCustomer = null;
            mvarSale_Totals = null;
            mvarSale_Lines = null;
            mvarSP_Prices = null;
            mvarReturnReason = null;
            mvarRestrictions = null;

        }
        ~Sale()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }

        public CustomerDisplay CustomerDisplay { get; set; }

        public bool EMVVersion { get; set; }

        // set this whenever payment is done using AR Tender for the sale 
        public bool IsArTenderUsed { get; set; }
    }
}
