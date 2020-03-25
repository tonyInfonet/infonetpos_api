using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Sale_Line : IDisposable
    {

        public string Sale_Num { get; set; }
        public short Till_Num { get; set; }
        public char PluType { get; set; }
        public bool IsPriceSet { get; set; }

        public string BasketId { get; set; }

        private short mvarLine_Num;
        private string mvarStock_Code;
        private char mvarStock_Type;
        private string mvarDescription;
        private float mvarQuantity;
        private double mvarPrice;
        private bool mvarGroup_Price;
        private double mvarLine_Discount;
        private double mvarDiscount_Adjust;
        private string mvarDiscount_Type;
        private string mvarDiscount_Code;
        private short mvarProd_Discount_Code;
        private bool mvarUser_Discount;
        private decimal mvarAssociate_Amount;
        private decimal mvarAmount;
        private decimal mvarTotal_Amount;
        private float mvarDiscount_Rate;
        private char mvarPrice_Type;
        private char mvarPrice_Units;
        private double mvarRegular_Price;
        private double mvarCost;
        private decimal mvarRebate;
        private string mvarUnits;
        private string mvarSerial_No;
        private float mvarLoyalty_Save;
        private bool mvarGift_Certificate;
        private bool mvarNoLoading;
        private string mvarGift_Num;
        private string mvarDept;
        private string mvarSub_Dept;
        private string mvarSub_Detail;
        private string mvarUser;
        private string mvarPLU_Code;
        private decimal mvarNetAmount;
        private short mvarPrice_Number;
        private bool mvarConfirmed;
        private byte mvarPumpID;
        private byte mvarPositionID;
        private bool mvarIsFuel;
        private bool mvarIsPropane;
        private byte mvarGradeID;
        private bool mvarPrePay;
        private byte mvarMOP;
        private double mvarAvailItems;
        private decimal mvarPointsPerDollar;
        private decimal mvarPointsPerUnit;
        private bool mvarLoyaltyDiscount;
        private short mvarQuantity_Decimals;
        private short mvarPrice_Decimals;
        private bool mvarIncludeInLoyalty;
        private bool mvarPointsOnVolume;
        private bool mvarIRigor;
        private string mvarCardProductCode;
        private Line_Taxes mvarLine_Taxes;
        private Return_Reasons mvarReturn_Reasons;
        private Line_Kits mvarLine_Kits;
        private Charges mvarCharges;
        private SP_Prices mvarSP_Prices;
        private string mvarPromoID; //  
                                    ///Private mvarTotalQty            As Single   '  
        private float mvarQtyInPromoLine; //  
        private float mvarAmountInPromoLine; //  
        private string mvarLink; //  
        private double mvarPromo_Price; //  
        private bool mvarNoPriceFormat; //  
        private bool mvarHotButton; //  
        private bool mvarNoPromo; //  
        private float mvarQtyForPromo; // only
        private bool mvarForPromo; //  
        private double mvarTotalQty_ByStock; //  
        private bool mvarRefreshPrice; //  
        private string mvarVendor;
        private bool mvarProcessed; // 'Used to validate coupons
        private short mvarAvailableQty; // 'Used to validate coupons
        public string GC_REPT;
        public decimal LOYAL_PPD;

        public short NUM_PRICE;
        public bool GROUP_PRTY;
        public bool GC_DISCOUNT;
        public short LOYAL_DISC;
        private bool mvarIsTaxExemptItem;
        private short mvarOverRideCode;
        //TODO: Ezipin_Removed
        //private bool mvarIsEziProduct; //
        //private int mvarEziCard; //
        private string mvarTE_COLLECTTAX; //  

        public delegate void StockExceededEventHandler(string Stock_Code, string Description, float InStock);
        private StockExceededEventHandler StockExceededEvent;

        public event StockExceededEventHandler StockExceeded
        {
            add
            {
                StockExceededEvent = (StockExceededEventHandler)System.Delegate.Combine(StockExceededEvent, value);
            }
            remove
            {
                StockExceededEvent = (StockExceededEventHandler)System.Delegate.Remove(StockExceededEvent, value);
            }
        }



        private bool mvarACCEPT_RET;
        private string mvarADD_RET_TO;
        private bool mvarALLOW_ENT;
        private bool mvarALLOW_PC;
        private bool mvarALLOW_QC;
        private bool mvarCL_DISCOUNTS;
        private bool mvarCOMBINE_LINE;
        private bool mvarDISC_REASON;
        private string mvarFUEL_UM;
        private bool mvarI_RIGOR;
        private bool mvarLOY_EXCLUDE;
        private bool mvarLOY_NOREDPO;
        private int mvarMAX_DISC_D;
        private short mvarMAX_DISC_P;
        private bool mvarPR_REASON;
        private short mvarPRICE_DEC;
        private short mvarQUANT_DEC;
        private short mvarRESTR_SALE;
        private short mvarRET_REASON;
        private short mvarVOL_POINTS;
        private short mvarLOYAL_PPU;


        public bool ALLOC_NEG;
        public string LINE_TYPE;

        private bool mvarManualFuel;
        private string mvarGiftType;
        private decimal mvarTotalCharge;
        private decimal mvarAddedTax;
        private bool mvarLoyaltyEligible;
        private string mvarThirdPartyExtractCode;
        private byte mvarPaidByCard;
        private bool mvarTE_AgeRstr;
        private string mvarGiftCard;


        private bool mvarUpsell;


        private float mvarTaxInclPrice;
        private float mvarOriginalPrice;

        private bool mvarLoadFuelAmount;


        private bool mvarFuelRebateEligible;
        private decimal mvarFuelRebate;


        private bool mvarScalableItem;
        private bool mvarStock_BY_Weight;
        private string mvarUM;
        private bool mvarPriceCheck;
        private bool mvarOpenItem;
        private bool mvarSendToLCD; // LCD
        private bool mvarIsFromHotButtons; // July 24, 2009 Nicolette added to control sending to lcd all items again when comming back from hot buttons screen. Once Hot Button property is used in Make_Promo and CANNOT be reset without screwing up the promotions I added another property
        private bool mvarActive_StockCode; //  
        //TODO: Ezipin_Removed 
        //private bool mvarEziProcessed; //
        private string mvarCardProfileID; // 
        //Code added by manish for cawash                               
        private bool mvarIsCarwashProduct; 
        private string mvarCarwashCode = ""; 
        //manish/
        private bool mvarEligibleTaxRebate; //  
        private bool mvarQualifiedTaxRebate; //  
        private double mvarTEPrice; // 
        private string mvarDiscountName; // may25,2010
        private double mvarOrigQty; //may26,2010
        private short mvarCategoryFK; // database
        private string mvarTEvendor; // 
        private double mvarTECost; // 
        private string mvarOrigVendor; // 
        private double mvarOrigCost; // 
        private decimal mvarOrigTaxIncldAmount;
        private string mvarProductExtract; //  
        private decimal mvarRestrictedAmount; //  
        private bool mvarActive_DayOfWeek; // PM added on August 27, 2012
        private bool mvarEligibleTaxEx; //  
        private decimal mvarTE_Amount_Incl; //  
        private bool mvarTaxForTaxExempt; //  

        #region CarwashCode
        public string CarwashCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCarwashCode;
                return returnValue;
            }
            set
            {
                mvarCarwashCode = value;
            }
        }
        public bool IsCarwashProduct
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsCarwashProduct;
                return returnValue;
            }
            set
            {
                mvarIsCarwashProduct = value;
            }
        }

        #endregion


        //
        //TODO: Ezipin_Removed 
        //public bool EziProcessed
        //{
        //    get
        //    {
        //        bool returnValue = false;
        //        returnValue = mvarEziProcessed;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarEziProcessed = value;
        //    }
        //}
        //End - SV

        //  - for skipping quantity setting and rest of the unwanted section for pricechexck
        public bool PriceCheck
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPriceCheck;
                return returnValue;
            }
            set
            {
                mvarPriceCheck = value;
            }
        }
        // 
        //  for items measured by weight -need unit of measure
        public string UM
        {
            get
            {
                return mvarUM;
            }
            set
            {
                mvarUM = value;
            }
        }
        // 
        //  for items measured by weight ( weight coming from the scale as kg\lb)- it is a setting in stock screen by product ( this is different than saclable - which is using for scanning and getting the information from the sticker for lotto tickets0

        public bool Stock_BY_Weight
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarStock_BY_Weight;
                return returnValue;
            }
            set
            {
                mvarStock_BY_Weight = value;
            }
        }
        // 
        public bool ScalableItem
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarScalableItem;
                return returnValue;
            }
            set
            {
                mvarScalableItem = value;
            }
        }


        public bool Upsell
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarUpsell;
                return returnValue;
            }
            set
            {
                mvarUpsell = value;
            }
        }




        public bool IsTaxExemptItem
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsTaxExemptItem;
                return returnValue;
            }
            set
            {
                mvarIsTaxExemptItem = value;
            }
        }


        public short overrideCode
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarOverRideCode;
                return returnValue;
            }
            set
            {
                mvarOverRideCode = value;
            }
        }



        public bool IRigor
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIRigor;
                return returnValue;
            }
            set
            {
                mvarIRigor = value;
            }
        }


        public short Quantity_Decimals
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarQuantity_Decimals;
                return returnValue;
            }
            set
            {
                mvarQuantity_Decimals = value;
            }
        }

        public bool No_Loading
        {
            get
            {
                return mvarNoLoading;
            }
            set
            {
                mvarNoLoading = value;
            }
        }


        public bool LoadFuelAmount
        {
            get { return mvarLoadFuelAmount; }
            set
            {
                mvarLoadFuelAmount = value;
            }
        }


        //
        //TODO: Ezipin_Removed 
        //public bool isEziProduct
        //{
        //    get
        //    {
        //        bool returnValue = false;
        //        returnValue = mvarIsEziProduct;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarIsEziProduct = value;
        //    }
        //}


        //public int EziCard
        //{
        //    get
        //    {
        //        int returnValue = 0;
        //        returnValue = mvarEziCard;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarEziCard = value;
        //    }
        //}
        //End - Sv
        //TODO: Ezipin_Removed
        
  

        public Charges Charges
        {
            get
            {
                Charges returnValue = default(Charges);
                if (mvarCharges == null)
                {
                    mvarCharges = new Charges();
                }

                returnValue = mvarCharges;
                return returnValue;
            }
            set
            {
                mvarCharges = value;
            }
        }


        public bool IncludeInLoyalty
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIncludeInLoyalty;
                return returnValue;
            }
            set
            {
                mvarIncludeInLoyalty = value;
            }
        }


        public bool PointsOnVolume
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPointsOnVolume;
                return returnValue;
            }
            set
            {
                mvarPointsOnVolume = value;
            }
        }


        public Line_Kits Line_Kits
        {
            get
            {
                Line_Kits returnValue = default(Line_Kits);
                if (mvarLine_Kits == null)
                {
                    mvarLine_Kits = new Line_Kits();
                }

                returnValue = mvarLine_Kits;
                return returnValue;
            }
            set
            {
                mvarLine_Kits = value;
            }
        }


        public Return_Reasons Return_Reasons
        {
            get
            {
                Return_Reasons returnValue = default(Return_Reasons);
                if (mvarReturn_Reasons == null)
                {
                    mvarReturn_Reasons = new Return_Reasons();
                }

                returnValue = mvarReturn_Reasons;
                return returnValue;
            }
            set
            {
                mvarReturn_Reasons = value;
            }
        }


        public Line_Taxes Line_Taxes
        {
            get
            {
                Line_Taxes returnValue = default(Line_Taxes);
                if (mvarLine_Taxes == null)
                {
                    mvarLine_Taxes = new Line_Taxes();
                }

                returnValue = mvarLine_Taxes;
                return returnValue;
            }
            set
            {
                mvarLine_Taxes = value;
            }
        }


        public string PLU_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPLU_Code;
                return returnValue;
            }
            set
            {
                mvarPLU_Code = value;
            }
        }


        public char Stock_Type
        {
            get
            {
                return mvarStock_Type;
            }
            set
            {
                mvarStock_Type = value;
            }
        }


        public byte pumpID
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarPumpID;
                return returnValue;
            }
            set
            {
                mvarPumpID = value;
            }
        }


        public byte PositionID
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarPositionID;
                return returnValue;
            }
            set
            {
                mvarPositionID = value;
            }
        }


        public byte GradeID
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarGradeID;
                return returnValue;
            }
            set
            {
                mvarGradeID = value;
            }
        }


        public bool Prepay
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrePay;
                return returnValue;
            }
            set
            {
                mvarPrePay = value;
            }
        }



        public bool ManualFuel
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarManualFuel;
                return returnValue;
            }
            set
            {
                mvarManualFuel = value;
            }
        }




        public byte PaidByCard
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarPaidByCard;
                return returnValue;
            }
            set
            {
                mvarPaidByCard = value;
            }
        }



        public double AvailItems
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarAvailItems;
                return returnValue;
            }
            set
            {
                mvarAvailItems = value;
            }
        }


        public bool Confirmed
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarConfirmed;
                return returnValue;
            }
            set
            {
                mvarConfirmed = value;
            }
        }


        public string User
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUser;
                return returnValue;
            }
            set
            {
                mvarUser = value;
            }
        }


        public string CardProductCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardProductCode;
                return returnValue;
            }
            set
            {
                mvarCardProductCode = value;
            }
        }


        public string Sub_Dept
        {
            get
            {
                return mvarSub_Dept;
            }
            set
            {
                mvarSub_Dept = value;
            }
        }


        public string Sub_Detail
        {
            get
            {
                return mvarSub_Detail;
            }
            set
            {

                mvarSub_Detail = value;
            }
        }


        public string Dept
        {
            get
            {
                return mvarDept;
            }
            set
            {
                mvarDept = value;
            }
        }


        public bool Gift_Certificate
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarGift_Certificate;
                return returnValue;
            }
            set
            {
                mvarGift_Certificate = value;
            }
        }






        public string Gift_Num
        {
            get
            {
                string returnValue = "";

                returnValue = mvarGift_Num;
                return returnValue;
            }
            set
            {

                mvarGift_Num = value;
            }
        }


        public float Loyalty_Save
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarLoyalty_Save;
                return returnValue;
            }
            set
            {
                mvarLoyalty_Save = value;
            }
        }


        public bool LoyaltyDiscount
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLoyaltyDiscount;
                return returnValue;
            }
            set
            {
                mvarLoyaltyDiscount = value;
            }
        }


        public string Serial_No
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSerial_No;
                return returnValue;
            }
            set
            {
                mvarSerial_No = value;
            }
        }


        public string Units
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUnits;
                return returnValue;
            }
            set
            {
                mvarUnits = value;
            }
        }


        public double Cost
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarCost;
                return returnValue;
            }
            set
            {
                mvarCost = value;
            }
        }


        public double Regular_Price
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarRegular_Price;
                return returnValue;
            }
            set
            {
                string fs;
                //  - it was creating problem for special price product like Xfor price

                //    'mvarRegular_Price = vData
                //    fs = IIf(Me.PRICE_DEC = 0, "0", "0." & String$(Me.PRICE_DEC, "0"))
                //    If mvarNoPriceFormat Then
                //       mvarRegular_Price = vData
                //    Else
                //        mvarRegular_Price = Format(vData, fs)
                //    End If
                mvarRegular_Price = value;
            }
        }


        public char Price_Type
        {
            get
            {
                char returnValue = '\0';
                returnValue = mvarPrice_Type;
                return returnValue;
            }
            set
            {
                mvarPrice_Type = value;
            }
        }


        public char Price_Units
        {
            get
            {
                char returnValue = '\0';
                returnValue = mvarPrice_Units;
                return returnValue;
            }
            set
            {
                mvarPrice_Units = value;
            }
        }


        public float Discount_Rate
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarDiscount_Rate;
                return returnValue;
            }
            set
            {

                mvarDiscount_Rate = value;
            }
        }


        public decimal Total_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTotal_Amount;
                return returnValue;
            }
            set
            {
                mvarTotal_Amount = value;
            }
        }

        public decimal Amount
        {
            get
            {
                return mvarAmount;
            }
            set { mvarAmount = value; }
        }


        public short Price_Decimals
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarPrice_Decimals;
                return returnValue;
            }
            set
            {
                mvarPrice_Decimals = value;
            }
        }

        public decimal Net_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarNetAmount;
                return returnValue;
            }
            set { mvarNetAmount = value; }
        }

        public byte MOP
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarMOP;
                return returnValue;
            }
            set
            {
                mvarMOP = value;
            }
        }


        public decimal Associate_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAssociate_Amount;
                return returnValue;
            }
            set
            {
                mvarAssociate_Amount = value;
            }
        }


        public string Discount_Type
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDiscount_Type;
                return returnValue;
            }
            set
            {
                mvarDiscount_Type = value;
            }
        }


        public string Discount_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDiscount_Code;
                return returnValue;
            }
            set
            {
                mvarDiscount_Code = value;
            }
        }


        public short Prod_Discount_Code
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarProd_Discount_Code;
                return returnValue;
            }
            set
            {
                mvarProd_Discount_Code = value;
            }
        }


        public bool User_Discount
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarUser_Discount;
                return returnValue;
            }
            set
            {
                mvarUser_Discount = value;
            }
        }


        public double Discount_Adjust
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarDiscount_Adjust;
                return returnValue;
            }
            set
            {
                mvarDiscount_Adjust = value;
                mvarNetAmount = (mvarAmount - (decimal)Math.Round(mvarLine_Discount + mvarDiscount_Adjust, 2));
            }
        }


        public double Line_Discount
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarLine_Discount;
                return returnValue;
            }
            set
            {
                mvarLine_Discount = Math.Round(Convert.ToDouble(value.ToString("0.000")), 2);
                //  mvarLine_Discount = value;

            }
        }


        public double price
        {
            get
            {
                return mvarPrice;
            }
            set
            {
                mvarPrice = value;

            }
        }


        public float Quantity
        {
            get
            {
                return mvarQuantity;
            }
            set
            {
                mvarQuantity = value;

            }
        }


        public string Description
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDescription;
                return returnValue;
            }
            set
            {
                mvarDescription = value;
            }
        }


        public SP_Prices SP_Prices
        {
            get
            {
                SP_Prices returnValue = default(SP_Prices);
                if (mvarSP_Prices == null)
                {
                    mvarSP_Prices = new SP_Prices();
                }

                returnValue = mvarSP_Prices;
                return returnValue;
            }
            set
            {
                mvarSP_Prices = value;
            }
        }


        public string Stock_Code
        {
            get
            {
                return mvarStock_Code;
            }
            set
            {
                //Smriti move this code to manager
                //dynamic Security_Renamed = default(dynamic);
                //dynamic Policy_Renamed = default(dynamic);

                //ADODB.Recordset Stock_Prices = default(ADODB.Recordset);
                //ADODB.Recordset rsRebates = default(ADODB.Recordset);
                //ADODB.Recordset rsPrType = default(ADODB.Recordset);
                //ADODB.Recordset rsTemp = default(ADODB.Recordset);
                //string Where_Vendor = "";
                //string fs = "";

                mvarStock_Code = value;

                //Sale_Line obj;
                //ADODB.Recordset rsDept = default(ADODB.Recordset);
                //if (!mvarNoLoading)
                //{

                //    if (!this.Valid_Stock_Code)
                //    {
                //        return;
                //    }

                //    rsTemp = Chaps_Main.Get_Records("SELECT * FROM StockByDay WHERE Stock_Code=\'" + value + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                //    // processing
                //    if (!rsTemp.EOF)
                //    {
                //        rsTemp = Chaps_Main.Get_Records("SELECT * FROM StockByDay WHERE Stock_Code=\'" + value + "\' AND DayOfWeek= " + System.Convert.ToString(DateAndTime.Weekday(DateAndTime.Today, FirstDayOfWeek.Sunday)), Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                //        if (rsTemp.EOF)
                //        {
                //            this.Active_StockCode = true; // to avoid inactive stock item message in the main screen, this item is not added to the sale anyway
                //            this.Active_DayOfWeek = false; //PM to fix the issue related to Hot Buttons on August 27, 2012
                //            Chaps_Main.DisplayMessage(0, (short)8890, MsgBoxStyle.OkOnly, Chaps_Main.Stock.Fields["Stock_Code"], (byte)0);
                //            return;
                //            // no Else requuired, if it is found the product can be sold today, continue processing
                //        }
                //    }
                //    else
                //    {
                //        this.Active_DayOfWeek = true; //PM on August 27, 2012
                //    }

                //    //###PERFORMAPRIL09### ''Stock.Find "STOCK_CODE='" & PLU.Fields![PLU_Prim] & "'", , adSearchForward, 1

                //    
                //    this.Dept = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Dept"].Value)) ? "" : (Chaps_Main.Stock.Fields["Dept"].Value));
                //    this.Sub_Dept = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Sub_Dept"].Value)) ? "" : (Chaps_Main.Stock.Fields["Sub_Dept"].Value));
                //    this.Sub_Detail = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Sub_Detail"].Value)) ? "" : (Chaps_Main.Stock.Fields["Sub_Detail"].Value));

                //    string temp_Policy_Name = "ALLOC_NEG";
                //    ALLOC_NEG = System.Convert.ToBoolean(modPolicy.GetPol(temp_Policy_Name, this));
                //    string temp_Policy_Name2 = "LINE_$_TYPE";
                //    LINE_TYPE = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name2, this));
                //    

                //    this.Description = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Descript"].Value)) ? " " : (Chaps_Main.Stock.Fields["Descript"].Value));
                //    this.Discount_Adjust = 0;
                //    this.Discount_Type = " ";
                //    this.Associate_Amount = 0;
                //    this.Total_Amount = 0;
                //    this.Discount_Rate = 0;
                //    this.Stock_Type = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Stock_Type"].Value)) ? "V" : (Chaps_Main.Stock.Fields["Stock_Type"].Value));
                //   this.Price_Type = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["pr_type"].Value)) ? "R" : (Chaps_Main.Stock.Fields["pr_type"].Value));
                //    this.Price_Units = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Pr_Units"].Value)) ? "$" : (Chaps_Main.Stock.Fields["Pr_Units"].Value));
                //    this.Vendor = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Vendor"].Value)) ? "" : (Chaps_Main.Stock.Fields["Vendor"].Value));
                //    this.Loyalty_Save = 0;
                //    this.Prod_Discount_Code = System.Convert.ToInt16((Information.IsDBNull(Chaps_Main.Stock.Fields["PROD_DISC"].Value)) ? 0 : (Chaps_Main.Stock.Fields["PROD_DISC"].Value));

                //    
                //    
                //    this.Gift_Certificate = (this.Stock_Type.ToUpper() == "G") ? true : false;
                //    
                //    this.User = User;
                //    this.Stock_BY_Weight = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["s_by_WGHT"].Value)) ? 0 : (Chaps_Main.Stock.Fields["s_by_WGHT"].Value)); // 
                //    this.UM = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["UM"].Value)) ? "" : (Chaps_Main.Stock.Fields["UM"].Value)); //  To display Unit of measure for scalable product in receipt
                //                                                                                                                                                 
                //    if (Variables.Return_Mode)
                //    {
                //        this.Quantity = -1;
                //    }
                //    else
                //    {
                //        
                //        this.Quantity = 1;
                //    } 

                //    //
                //    //We need to know if the product is ezipin or not so we can send a purchase voucher to ezipin
                //    //Set obj = New Sale_Line
                //    //obj.Dept = Me.Dept
                //    this.isEziProduct = false;
                //TODO: Ezipin_Removed 
                //    string temp_Policy_Name3 = "Ezi_Dept";
                //    if (Policy_Renamed.SUPPORTEZI && modPolicy.GetPol(temp_Policy_Name3, this))
                //    {
                //        //this product is an ezipin product
                //        this.isEziProduct = true;
                //        Chaps_Main.SA.HasEziProducts = true;
                //    }
                //    //Set obj = Nothing

                //    //
                //    if (Policy_Renamed.SUPPORTWASH && Policy_Renamed.CarwashIntegrated)
                //    {
                //        rsDept = Chaps_Main.Get_Records("SELECT Dept, Dept_Name FROM Dept WHERE Dept=\'" + this.Dept + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                //        if (rsDept.RecordCount > 0)
                //        {
                //            // Switching to Dept defined by Dept_number not Dept_name
                //            //If UCase(rsDept!Dept_Name) = UCase(Policy.CarWash_Dep) Then
                //            if (Strings.UCase(System.Convert.ToString(rsDept.Fields["Dept"].Value)) == Strings.UCase(System.Convert.ToString(Policy_Renamed.CarWash_Dep)))
                //            {
                //                //this product is a carwash product
                //                this.isCarwashProduct = true;
                //                Chaps_Main.SA.HasCarwashProducts = true;
                //            }
                //        }
                //        rsDept = null;
                //    }
                //    //End - SV
                //END: Ezipin_Removed

                //    // Set the rebate for the product, if any,  
                //    if (mvarVendor.Length != 0)
                //    {
                //        rsRebates = Chaps_Main.Get_Records("SELECT SUM(Rebate) AS TRebate FROM StockRebates  WHERE VendorID=\'" + mvarVendor + "\' AND Stock_Code=\'" + mvarStock_Code + "\' AND OrderNo=0", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                //        this.Rebate = System.Convert.ToDecimal((Information.IsDBNull(rsRebates.Fields["TRebate"].Value)) ? 0 : (rsRebates.Fields["TRebate"].Value));
                //    }
                //    //  

                //    // Set the cost for the product,   for cost tracking
                //    if (Policy_Renamed.COST_TYPE == "Standard")
                //    {
                //        this.Cost = System.Convert.ToDouble((Information.IsDBNull(Chaps_Main.Stock.Fields["Std_Cost"].Value)) ? 0 : (Chaps_Main.Stock.Fields["Std_Cost"].Value));
                //    }
                //    else if (Policy_Renamed.COST_TYPE == "Average")
                //    {
                //        this.Cost = System.Convert.ToDouble((Information.IsDBNull(Chaps_Main.Stock.Fields["Avg_Cost"].Value)) ? 0 : (Chaps_Main.Stock.Fields["Avg_Cost"].Value));
                //    }
                //    // Nicolette end

                //    
                //    this.LoyaltyEligible = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["ElgLoyalty"].Value)) ? false : (Chaps_Main.Stock.Fields["ElgLoyalty"].Value));
                //    
                //    
                //    this.FuelRebateEligible = System.Convert.ToBoolean(Policy_Renamed.FuelRebate && ((Information.IsDBNull(Chaps_Main.Stock.Fields["ElgFuelRebate"].Value)) ? false : (Chaps_Main.Stock.Fields["ElgFuelRebate"].Value)));
                //    this.FuelRebate = System.Convert.ToDecimal((Information.IsDBNull(Chaps_Main.Stock.Fields["FuelRebate"].Value)) ? 0 : (Chaps_Main.Stock.Fields["FuelRebate"].Value));
                //    
                //    //  
                //    this.EligibleTaxRebate = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["ElgTaxRebate"].Value)) ? false : (Chaps_Main.Stock.Fields["ElgTaxRebate"].Value));
                //    this.QualifiedTaxRebate = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["QualTaxRebate"].Value)) ? false : (Chaps_Main.Stock.Fields["QualTaxRebate"].Value));
                //    //   end
                //    this.EligibleTaxEx = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["ElgTaxExemption"].Value)) ? false : (Chaps_Main.Stock.Fields["ElgTaxExemption"].Value)); //  

                //    // and now look up the regular price for the product, based on vendor and orderno
                //    if (string.IsNullOrEmpty(mvarVendor))
                //    {
                //        //            Where_Vendor = " AND (VendorID='' OR VendorID IS NULL) AND OrderNo=0 "
                //        // design
                //        Where_Vendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
                //    }
                //    else
                //    {
                //        Where_Vendor = " AND VendorID=\'" + mvarVendor + "\' AND OrderNo=0 ";
                //    }

                //    Stock_Prices = Chaps_Main.Get_Records("SELECT   *  FROM     Stock_Prices  WHERE    Stock_Prices.Stock_Code = \'" + value + "\' AND " + "         Stock_Prices.Price_Number = 1 " + Where_Vendor + "ORDER BY Stock_Prices.Price_Number ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

                //    rsPrType = Chaps_Main.Get_Records("SELECT Pr_Type, Pr_Units FROM PriceL WHERE Stock_Code=\'" + value + "\'  " + Where_Vendor, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

                //    if (rsPrType.EOF)
                //    {
                //        //                Set rsPrType = Get_Records("SELECT Pr_Type, Pr_Units FROM PriceL WHERE Stock_Code='" & _
                //        
                //        // design
                //        rsPrType = Chaps_Main.Get_Records("SELECT Pr_Type, Pr_Units FROM PriceL WHERE Stock_Code=\'" + value + "\' " + " AND (VendorID=\'ALL\' AND OrderNo=0) ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

                //        if (!rsPrType.EOF)
                //        {
                //            this.Price_Type = System.Convert.ToString((Information.IsDBNull(rsPrType.Fields["pr_type"].Value)) ? "R" : (rsPrType.Fields["pr_type"].Value));
                //            this.Price_Units = System.Convert.ToString((Information.IsDBNull(rsPrType.Fields["Pr_Units"].Value)) ? "$" : (rsPrType.Fields["Pr_Units"].Value));
                //        }
                //    }
                //    else
                //    {
                //        this.Price_Type = System.Convert.ToString((Information.IsDBNull(rsPrType.Fields["pr_type"].Value)) ? "R" : (rsPrType.Fields["pr_type"].Value));
                //        this.Price_Units = System.Convert.ToString((Information.IsDBNull(rsPrType.Fields["Pr_Units"].Value)) ? "$" : (rsPrType.Fields["Pr_Units"].Value));
                //    }
                //    CheckOpenItem(); //###PTC

                //    if (Variables.POSOnly && this.OpenItem == true) //###PTC-  Show this message only for items not open items
                //    {
                //    }
                //    else
                //    {
                //        if (!(Stock_Prices.EOF && Stock_Prices.BOF))
                //        {
                //            this.Regular_Price = System.Convert.ToDouble(Stock_Prices.Fields["price"].Value);
                //        }
                //        else
                //        {
                //            // if no specific price was set for the active vendor, look for the price applicable to all vendors
                //            //                Set Stock_Prices = Get_Records( _
                //            
                //            
                //            
                //            
                //            
                //            
                //            
                //            // design
                //            Stock_Prices = Chaps_Main.Get_Records("SELECT   *  FROM     Stock_Prices  WHERE    Stock_Prices.Stock_Code = \'" + value + "\' AND " + "         Stock_Prices.Price_Number = 1 " + "AND (VendorID=\'ALL\' AND OrderNo=0) " + "ORDER BY Stock_Prices.Price_Number ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

                //            if (Stock_Prices.EOF)
                //            {
                //                //                TimedMessageBox "No Regular Price Set for Product " & Stock![Stock_Code] & vbCrLf & _
                //                //"Defaulting to $0.01", vbOKOnly, "No Regular Price", 3, GetActiveWindow
                //                Chaps_Main.DisplayMessage(0, (short)8112, MsgBoxStyle.OkOnly, Chaps_Main.Stock.Fields["Stock_Code"], (byte)0);
                //                this.Regular_Price = 0.01; // Default to $0.01
                //            }
                //            else
                //            {
                //                this.Regular_Price = System.Convert.ToDouble(Stock_Prices.Fields["price"].Value);
                //            }
                //        }

                //        Stock_Prices = null;
                //    } //##PTC
                //      
                //      //  
                //    if (Policy_Renamed.TAX_EXEMPT) //  -made the same for all taxexempt- after talking to Nicolette 'And Policy.TE_Type = "SITE" Then
                //    {
                //        rsTemp = Chaps_Main.Get_Records("SELECT CategoryFK,TEVendor FROM ProductTaxExempt WHERE UpcCode=\'" + value + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                //        if (!rsTemp.EOF)
                //        {
                //            mvarCategoryFK = System.Convert.ToInt16((Information.IsDBNull(rsTemp.Fields["CategoryFK"].Value)) ? "" : (rsTemp.Fields["CategoryFK"].Value));
                //            mvarTEvendor = System.Convert.ToString((Information.IsDBNull(rsTemp.Fields["TEVendor"].Value)) ? "" : (rsTemp.Fields["TEVendor"].Value)); //shiny added the TE vendor
                //        }
                //    }
                //    //   ends
                //}
                //else
                //{

                //    //###PERFORMAPRIL09### '' Stock.Find "STOCK_CODE='" & mvarStock_Code & "'", , adSearchForward, 1
                //    Chaps_Main.Stock = Chaps_Main.Get_Records("Select * from StockMst where Stock_code =\'" + mvarStock_Code + "\'", Chaps_Main.dbMaster); //###PERFORMAPRIL09###

                //    ///        '  , reload from StockMst to avoid saving in SaleLine table
                //    ///        ' (should be discussed with PM)
                //    ///        Me.EligibleTaxRebate = IIf(IsNull(Stock![ElgTaxRebate]), False, Stock![ElgTaxRebate])
                //    ///        Me.QualifiedTaxRebate = IIf(IsNull(Stock![QualTaxRebate]), False, Stock![QualTaxRebate])
                //    ///        '   end

                //    
                //    ///        binal feb27 returning a GC --- No Current Record, So if GC then set (Stock_Type)= V
                //    
                //    
                //    
                //    
                //    if (!Chaps_Main.Stock.EOF)
                //    {
                //        // 
                //        //  , reload from StockMst to avoid saving in SaleLine table
                //        // (should be discussed with PM)
                //        this.EligibleTaxRebate = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["ElgTaxRebate"].Value)) ? false : (Chaps_Main.Stock.Fields["ElgTaxRebate"].Value));
                //        this.QualifiedTaxRebate = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["QualTaxRebate"].Value)) ? false : (Chaps_Main.Stock.Fields["QualTaxRebate"].Value));
                //        //   end
                //        // 
                //        this.Stock_Type = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Stock_Type"].Value)) ? "V" : (Chaps_Main.Stock.Fields["Stock_Type"].Value));
                //        this.Stock_BY_Weight = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock.Fields["s_by_WGHT"].Value)) ? 0 : (Chaps_Main.Stock.Fields["s_by_WGHT"].Value)); //  for reprint we need to set this
                //        this.UM = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["UM"].Value)) ? "" : (Chaps_Main.Stock.Fields["UM"].Value)); //  To display Unit of measure for scalable product in receipt'for reprint we need to set this
                //                                                                                                                                                     //  - Dept\subdept and sub detail information for prepay switch
                //        this.Dept = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Dept"].Value)) ? "" : (Chaps_Main.Stock.Fields["Dept"].Value));
                //        this.Sub_Dept = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Sub_Dept"].Value)) ? "" : (Chaps_Main.Stock.Fields["Sub_Dept"].Value));
                //        this.Sub_Detail = System.Convert.ToString((Information.IsDBNull(Chaps_Main.Stock.Fields["Sub_Detail"].Value)) ? "" : (Chaps_Main.Stock.Fields["Sub_Detail"].Value));
                //        // 

                //    }
                //    else
                //    {
                //        this.Stock_Type = "V";
                //    }
                //    

                //    
                //    string temp_Policy_Name4 = "ALLOC_NEG";
                //    ALLOC_NEG = System.Convert.ToBoolean(modPolicy.GetPol(temp_Policy_Name4, this));
                //    string temp_Policy_Name5 = "LINE_$_TYPE";
                //    LINE_TYPE = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name5, this));
                //    
                //}

                //
                //Chaps_Main.Stock_Br.Find(Criteria: "STOCK_CODE=\'" + this.Stock_Code + "\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
                //// values
                /////        If Not Stock_Br.EOF Then Me.AvailItems = Stock_Br.Fields![Avail]
                ////  - Null and 1 is active stock ;  0- Inactive stock
                //if (!Chaps_Main.Stock_Br.EOF)
                //{
                //    this.Active_StockCode = System.Convert.ToBoolean((Information.IsDBNull(Chaps_Main.Stock_Br.Fields["Activestock"].Value)) ? true : (Chaps_Main.Stock_Br.Fields["Activestock"].Value));
                //}
                //else
                //{
                //    this.Active_StockCode = true; // if missing from Stock_br need to consider it as active stock
                //}
                //// 
                //if (this.Stock_Type == "V" || this.Stock_Type == "O") //  
                //{
                //    if (!Chaps_Main.Stock_Br.EOF)
                //    {
                //        this.AvailItems = System.Convert.ToDouble((Information.IsDBNull(Chaps_Main.Stock_Br.Fields["AVAIL"].Value)) ? 0 : (Chaps_Main.Stock_Br.Fields["AVAIL"].Value));
                //    }
                //}

                //
                //
                //

                //if (this.Stock_Type == "G")
                //{
                //    
                //    this.Line_Taxes = new Line_Taxes();
                //    this.SP_Prices = new SP_Prices();
                //    this.Line_Kits = new Line_Kits();
                //    this.Charges = new Charges();
                //}
                //else
                //{
                //    this.Line_Taxes = Make_Taxes();
                //    this.Line_Kits = Make_Kits();
                //    this.Charges = Make_Charges();
                //    if ((Strings.UCase(System.Convert.ToString(Security_Renamed.BackOfficeVersion)) == "FULL" || Policy_Renamed.PROMO_SALE) && !this.HotButton)
                //    {
                //        this.Make_Promo(); //  
                //    }
                //    this.SP_Prices = Make_Prices();
                //}

                //
                //
                //
                //

                //
                //LoadSaleLinePolicies();
                //

                //
                //this.Quantity_Decimals = this.QUANT_DEC; // Quantity Decimals
                //this.Price_Decimals = this.PRICE_DEC; // Price Decimals
                //this.IncludeInLoyalty = !this.LOY_EXCLUDE;
                //this.PointsOnVolume = this.VOL_POINTS;
                //this.PointsPerDollar = LOYAL_PPD;
                //this.PointsPerUnit = this.LOYAL_PPU; 
                //this.IRigor = this.I_RIGOR; // Recursive application of incremental prices.

                //// Dec 18, 2006 Nicolette added to format field according to the policy before the control goes back to sale main, otherwise the user will be asked for a reason if the policy to use reasons is set to Yes
                //fs = System.Convert.ToString(this.PRICE_DEC == 0 ? "0" : ("0." + new string('0', this.PRICE_DEC)));
                //mvarRegular_Price = double.Parse(mvarRegular_Price.ToString(fs));
                //// End Dec 18, 2006

                ////   added the Me.Price_Type <> "R" condition to avoid the Price_Type property let to be processed again
                //if (this.Price_Type != "R" && this.SP_Prices == null)
                //{
                //    this.Price_Type = "R";
                //}
                //else if (this.Price_Type != "R" && this.SP_Prices.Count == 0) //  
                //{
                //    this.Price_Type = "R";
                //}
                //rsRebates = null;
                //Stock_Prices = null;
                //rsPrType = null;
                //rsTemp = null;

            }
        }


        public decimal PointsPerDollar
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPointsPerDollar;
                return returnValue;
            }
            set
            {
                mvarPointsPerDollar = value;
            }
        }


        public decimal PointsPerUnit
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPointsPerUnit;
                return returnValue;
            }
            set
            {
                mvarPointsPerUnit = value;
            }
        }

        public bool Valid_Stock_Code
        {
            get
            {
                bool returnValue = false;

                returnValue = this.Stock_Code != "";

                return returnValue;
            }
        }


        public short Line_Num
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLine_Num;
                return returnValue;
            }
            set
            {
                mvarLine_Num = value;
            }
        }


        public bool Group_Price
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarGroup_Price;
                return returnValue;
            }
            set
            {
                mvarGroup_Price = value;
            }
        }


        public bool ProductIsFuel
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsFuel;
                return returnValue;
            }
            set
            {
                mvarIsFuel = value;
            }
        }

        //Nancy add IsPropane property,01/29/03

        public bool IsPropane
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsPropane;
                return returnValue;
            }
            set
            {
                mvarIsPropane = value;
            }
        }


        public short Price_Number
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarPrice_Number;
                return returnValue;
            }
            set
            {
                //SMriti move this code to manager
                //ADODB.Recordset Stock_Prices = default(ADODB.Recordset);
                //string Where_Vendor = "";

                mvarPrice_Number = value;
                //if (this.ManualFuel)
                //{
                //    return; //  - for manual fuel sales with customers it is saving wrong price and discount(from stock tables) in the csccursale. But screen is correct. So in crash recovery wrong info
                //}
                //if (mvarNoLoading)
                //{
                //    return;
                //}

                //if (value >= 1 & value <= NUM_PRICE)
                //{

                //    if (string.IsNullOrEmpty(mvarVendor))
                //    {
                //        //            Where_Vendor = " AND (VendorID='' OR VendorID IS NULL) AND OrderNo=0 "
                //        // design
                //        Where_Vendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
                //    }
                //    else
                //    {
                //        Where_Vendor = " AND VendorID=\'" + mvarVendor + "\' AND OrderNo=0 ";
                //    }
                //    //###PERFORMAPRIL09###-Start
                //    
                //    ///            "SELECT * FROM Stock_Prices " & _
                //    ///            "WHERE Stock_Prices.Stock_Code = '" & Me.Stock_Code & "' AND " & _
                //    ///            "      Stock_Prices.Price_Number = " & vData & " " & Where_Vendor & _
                //    ///            "ORDER BY Stock_Prices.Price_Number ", _
                //    ///            dbMaster, adOpenForwardOnly, adLockReadOnly)
                //    Stock_Prices = Chaps_Main.Get_Records("SELECT * FROM Stock_Prices  WHERE Stock_Prices.Stock_Code = \'" + this.Stock_Code + "\' AND " + "      Stock_Prices.Price_Number = " + System.Convert.ToString(value) + " " + Where_Vendor, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                //    //###PERFORMAPRIL09###
                //    if (!Stock_Prices.EOF)
                //    {
                //        if ((int)(Stock_Prices.Fields["price"].Value) == 0)
                //        {
                //            this.price = this.Regular_Price;
                //        }
                //        else
                //        {
                //            this.price = System.Convert.ToDouble(Stock_Prices.Fields["price"].Value);
                //        }
                //        if (value > 1)
                //        {
                //            this.Price_Type = "R"; //  
                //        }
                //    }
                //    else
                //    {
                //        //            Set Stock_Prices = Get_Records( _
                //        
                //        
                //        
                //        
                //        
                //        

                //        // design
                //        //###PERFORMAPRIL09###-Start

                //        
                //        ///                "SELECT * FROM Stock_Prices " & _
                //        ///                "WHERE Stock_Prices.Stock_Code = '" & Me.Stock_Code & "' AND " & _
                //        ///                "      Stock_Prices.Price_Number = " & vData & _
                //        ///                " AND (VendorID='ALL' AND OrderNo=0) " & _
                //        ///                "ORDER BY Stock_Prices.Price_Number ", _
                //        ///                dbMaster, adOpenForwardOnly, adLockReadOnly)

                //        Stock_Prices = Chaps_Main.Get_Records("SELECT * FROM Stock_Prices  WHERE Stock_Prices.Stock_Code = \'" + this.Stock_Code + "\' AND " + "      Stock_Prices.Price_Number = " + System.Convert.ToString(value) + " AND (VendorID=\'ALL\' AND OrderNo=0)", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                //        //###PERFORMAPRIL09###
                //        if (!Stock_Prices.EOF)
                //        {
                //            if ((int)(Stock_Prices.Fields["price"].Value) == 0)
                //            {
                //                this.price = this.Regular_Price;
                //            }
                //            else
                //            {
                //                this.price = System.Convert.ToDouble(Stock_Prices.Fields["price"].Value);
                //            }
                //        }
                //        else
                //        {
                //            this.price = this.Regular_Price;
                //        }
                //        if (value > 1)
                //        {
                //            this.Price_Type = "R"; //  
                //        }
                //    }
                //    Stock_Prices = null;
                //}
                //else
                //{
                //    mvarPrice_Number = (short)1;
                //    this.price = this.Regular_Price;
                //}
                //Stock_Prices = null;

            }
        }



        // to keep StockCode level's "Do you accept product returns?" {Yes,No} - Boolean
        // for it's Company level's setting, we save it in Policy class

        public bool ACCEPT_RET
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarACCEPT_RET;
                return returnValue;
            }
            set
            {
                mvarACCEPT_RET = value;
            }
        }

        // to keep "Add returned stock to" {Hold, In Stock, Waste} - string

        public string ADD_RET_TO
        {
            get
            {
                string returnValue = "";
                returnValue = mvarADD_RET_TO;
                return returnValue;
            }
            set
            {
                mvarADD_RET_TO = value;
            }
        }

        // to keep "Allow Manual Entry" {Yes,No} - Boolean

        public bool ALLOW_ENT
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarALLOW_ENT;
                return returnValue;
            }
            set
            {
                mvarALLOW_ENT = value;
            }
        }

        // to keep "Allow Item Price Change" {Yes,No} - Boolean

        public bool ALLOW_PC
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarALLOW_PC;
                return returnValue;
            }
            set
            {
                mvarALLOW_PC = value;
            }
        }

        // to keep "Allow Item Quantity Change" {Yes,No} - Boolean

        public bool ALLOW_QC
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarALLOW_QC;
                return returnValue;
            }
            set
            {
                mvarALLOW_QC = value;
            }
        }

        // to keep "Do you offer discounts?" {Yes,No} - Boolean

        public bool CL_DISCOUNTS
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCL_DISCOUNTS;
                return returnValue;
            }
            set
            {
                mvarCL_DISCOUNTS = value;
            }
        }

        // to keep StockCode level "Do you accept product returns?" {Yes,No} - Boolean
        // for it's Company level's setting, we save it in Policy class

        public bool COMBINE_LINE
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCOMBINE_LINE;
                return returnValue;
            }
            set
            {
                mvarCOMBINE_LINE = value;
            }
        }

        // to keep StockCode level "Use reasons for discounts ?" {Yes,No} - Boolean
        // for it's Company level's setting, we save it in Policy class

        public bool DISC_REASON
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarDISC_REASON;
                return returnValue;
            }
            set
            {
                mvarDISC_REASON = value;
            }
        }

        // to keep StockCode level "Units of Measure for Fuel" {L,G,Kg,Lb} - string
        // for it's Company level's setting, we save it in Policy class

        public string FUEL_UM
        {
            get
            {
                string returnValue = "";
                returnValue = mvarFUEL_UM;
                return returnValue;
            }
            set
            {
                mvarFUEL_UM = value;
            }
        }

        // to keep "Strictly Enforce Incremental Prices" {Yes,No} - Boolean

        public bool I_RIGOR
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarI_RIGOR;
                return returnValue;
            }
            set
            {
                mvarI_RIGOR = value;
            }
        }

        // to keep "Exclude from Loyalty Program" {Yes,No} - Boolean

        public bool LOY_EXCLUDE
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLOY_EXCLUDE;
                return returnValue;
            }
            set
            {
                mvarLOY_EXCLUDE = value;
            }
        }

        // to keep "Exclude from Redeemed Points." {Yes,No} - Boolean

        public bool LOY_NOREDPO
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLOY_NOREDPO;
                return returnValue;
            }
            set
            {
                mvarLOY_NOREDPO = value;
            }
        }

        // to keep "Maximum discount in dollars" RG(0,999999) - Long

        public int MAX_DISC_D
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarMAX_DISC_D;
                return returnValue;
            }
            set
            {
                mvarMAX_DISC_D = value;
            }
        }

        // to keep "Maximum discount percent" RG(0,100) - Integer

        public short MAX_DISC_P
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarMAX_DISC_P;
                return returnValue;
            }
            set
            {
                mvarMAX_DISC_P = value;
            }
        }

        // to keep "Use reasons for price changes ?" {Yes,No} - Boolean

        public bool PR_REASON
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPR_REASON;
                return returnValue;
            }
            set
            {
                mvarPR_REASON = value;
            }
        }

        // to keep "Decimal places in Sell price" RG(0,4) - Integer

        public short PRICE_DEC
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarPRICE_DEC;
                return returnValue;
            }
            set
            {
                mvarPRICE_DEC = value;
            }
        }

        // to keep "Decimal places in Quantities" RG(0,3) - Integer

        public short QUANT_DEC
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarQUANT_DEC;
                return returnValue;
            }
            set
            {
                mvarQUANT_DEC = value;
            }
        }

        // to keep "Restrict Sales To Authorization Level" RG(0,10) - Integer

        public short RESTR_SALE
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarRESTR_SALE;
                return returnValue;
            }
            set
            {
                mvarRESTR_SALE = value;
            }
        }

        // to keep "Use reasons for returns ?" {Yes,No} - Boolean

        // TODO: Assigning non zero value as true - Ipsit_27
        public bool RET_REASON
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarRET_REASON != 0;
                return returnValue;
            }
            set
            {
                mvarRET_REASON = value ? (short)1 : (short)0;
            }
        }

        // to keep "Award points on Quantity" {Yes,No} - Boolean
        // TODO: Assigning non zero value as true - Ipsit_28
        public bool VOL_POINTS
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarVOL_POINTS != 0;
                return returnValue;
            }
            set
            {
                mvarVOL_POINTS = value ? (short)1 : (short)0;
            }
        }

        //   - Integer

        public short LOYAL_PPU
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLOYAL_PPU;
                return returnValue;
            }
            set
            {
                mvarLOYAL_PPU = value;
            }
        }


        // to keep "What kind of Gift Certificate system do you support?" {NONE,LocalGift,GiveX,Milliplein} - string

        public string GiftType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarGiftType;
                return returnValue;
            }
            set
            {
                mvarGiftType = value;
            }
        }





        public string ThirdPartyExtractCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarThirdPartyExtractCode;
                return returnValue;
            }
            set
            {
                mvarThirdPartyExtractCode = value;
            }
        }



        public string TE_AgeRstr
        {
            get
            {
                string returnValue = "";
                returnValue = (mvarTE_AgeRstr).ToString();
                return returnValue;
            }
            set
            {
                mvarTE_AgeRstr = Convert.ToBoolean(value);
            }
        }
        //add


        public decimal Rebate
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarRebate;
                return returnValue;
            }
            set
            {
                mvarRebate = value;
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

        //

        public bool Processed
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarProcessed;
                return returnValue;
            }
            set
            {
                mvarProcessed = value;
            }
        }


        public short AvailableQty
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarAvailableQty;
                return returnValue;
            }
            set
            {
                mvarAvailableQty = value;
            }
        }
        //End - Svetlana



        public decimal TotalCharge
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTotalCharge;
                return returnValue;
            }
            set
            {
                mvarTotalCharge = value;
            }
        }


        public decimal AddedTax
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAddedTax;
                return returnValue;
            }
            set
            {
                mvarAddedTax = value;
            }
        }



        public bool LoyaltyEligible
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLoyaltyEligible;
                return returnValue;
            }
            set
            {
                mvarLoyaltyEligible = value;
            }
        }




        public bool FuelRebateEligible
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarFuelRebateEligible;
                return returnValue;
            }
            set
            {
                mvarFuelRebateEligible = value;
            }
        }


        public decimal FuelRebate
        {
            get
            {
                return mvarFuelRebate;
            }
            set
            {
                mvarFuelRebate = value;
            }
        }





        public float TaxInclPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTaxInclPrice;
                return returnValue;
            }
            set
            {
                mvarTaxInclPrice = value;
            }
        }


        public float OriginalPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarOriginalPrice;
                return returnValue;
            }
            set
            {
                mvarOriginalPrice = value;
            }
        }



        public string PromoID
        {
            get
            {
                return mvarPromoID;
            }
            set
            {
                mvarPromoID = value;
            }
        }


        public double Promo_Price
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarPromo_Price;
                return returnValue;
            }
            set
            {
                mvarPromo_Price = value;
            }
        }


        public float QtyInPromoLine
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarQtyInPromoLine;
                return returnValue;
            }
            set
            {
                mvarQtyInPromoLine = value;
            }
        }


        public float AmountInPromoLine
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarAmountInPromoLine;
                return returnValue;
            }
            set
            {
                mvarAmountInPromoLine = value;
            }
        }
        // end July 09, 2008

        //  

        public string Link
        {
            get
            {
                return mvarLink;
            }
            set
            {
                mvarLink = value;
            }
        }
        // End  

        //  

        public bool NoPriceFormat
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarNoPriceFormat;
                return returnValue;
            }
            set
            {
                mvarNoPriceFormat = value;
            }
        }
        // End  

        //  

        public bool HotButton
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarHotButton;
                return returnValue;
            }
            set
            {
                mvarHotButton = value;
            }
        }
        // End  

        // defined

        public bool IsFromHotButtons
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsFromHotButtons;
                return returnValue;
            }
            set
            {
                mvarIsFromHotButtons = value;
            }
        }
        // End  

        //  

        public bool NoPromo
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarNoPromo;
                return returnValue;
            }
            set
            {
                mvarNoPromo = value;
            }
        }
        // End  

        //  

        public float QtyForPromo
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarQtyForPromo;
                return returnValue;
            }
            set
            {
                mvarQtyForPromo = value;
            }
        }
        // End  

        //  

        public bool ForPromo
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarForPromo;
                return returnValue;
            }
            set
            {
                mvarForPromo = value;
            }
        }
        //####PTC - Feb6, 2008 -Start

        public bool OpenItem
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarOpenItem;
                return returnValue;
            }
            set
            {
                mvarOpenItem = value;
            }
        }


        public float TotalQty_ByStock
        {
            get
            {
                float returnValue = 0;
                returnValue = (float)mvarTotalQty_ByStock;
                return returnValue;
            }
            set
            {
                mvarTotalQty_ByStock = value;
            }
        }

        public bool SendToLCD
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarSendToLCD;
                return returnValue;
            }
            set
            {
                mvarSendToLCD = value;
            }
        }

        public bool RefreshPrice
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarRefreshPrice;
                return returnValue;
            }
            set
            {
                mvarRefreshPrice = value;
            }
        }
        // 
        public bool Active_StockCode
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarActive_StockCode;
                return returnValue;
            }
            set
            {
                mvarActive_StockCode = value;
            }
        }
        //Shiny end


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

        //   for Tax Rebate (Ontario)

        public bool EligibleTaxRebate
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarEligibleTaxRebate;
                return returnValue;
            }
            set
            {
                mvarEligibleTaxRebate = value;
            }
        }


        public bool QualifiedTaxRebate
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarQualifiedTaxRebate;
                return returnValue;
            }
            set
            {
                mvarQualifiedTaxRebate = value;
            }
        }
        //   end
        //  - To keep taxxempt price for prepay ( based on cash or credit )

        public double TEPrice
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarTEPrice;
                return returnValue;
            }
            set
            {
                mvarTEPrice = value;
            }
        }
        //  - fuel loyalty discount name

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

        public double OrigQty
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarOrigQty;
                return returnValue;
            }
            set
            {
                mvarOrigQty = value;
            }
        }

        // SITE

        public short CategoryFK
        {
            get
            {
                return mvarCategoryFK;
            }
            set
            {
                mvarCategoryFK = value;
            }
        }
        //   end

        // 

        public double TECost
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarTECost;
                return returnValue;
            }
            set
            {
                mvarTECost = value;
            }
        }

        public string TEVendor
        {
            get
            {
                return mvarTEvendor;
            }
            set
            {
                mvarTEvendor = value;
            }
        }
        //  - to keep orig\vendor cost info-- after tax exempt, if we want to clear taxexempt- need to reverse the info


        public string OrigVendor
        {
            get
            {
                string returnValue = "";
                returnValue = mvarOrigVendor;
                return returnValue;
            }
            set
            {
                mvarOrigVendor = value;
            }
        }


        public double OrigCost
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarOrigCost;
                return returnValue;
            }
            set
            {
                mvarOrigCost = value;
            }
        }


        public decimal OrigTaxIncldAmount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarOrigTaxIncldAmount;
                return returnValue;
            }
            set
            {
                mvarOrigTaxIncldAmount = value;
            }
        }

        //  

        public decimal RestrictedAmount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarRestrictedAmount;
                return returnValue;
            }
            set
            {
                mvarRestrictedAmount = value;
            }
        }
        //   end

        //PM added on August 27, 2012
        public bool Active_DayOfWeek
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarActive_DayOfWeek;
                return returnValue;
            }
            set
            {
                mvarActive_DayOfWeek = value;
            }
        }
        //PM End August 27, 2012

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

        // TE_Amount property added for Tax Exemption for govt. agencies; can be used for any tax exempt type to keep the total included exempted amount by sale line
        //  

        public decimal TE_Amount_Incl
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTE_Amount_Incl;
                return returnValue;
            }
            set
            {
                mvarTE_Amount_Incl = value;
            }
        }
        //   end

        //  

        public string TE_COLLECTTAX
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTE_COLLECTTAX;
                return returnValue;
            }
            set
            {
                mvarTE_COLLECTTAX = value;
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
        //end - sv


        private void Class_Initialize_Renamed()
        {
            mvarCharges = new Charges();
            mvarLine_Kits = new Line_Kits();
            mvarReturn_Reasons = new Return_Reasons();
            mvarLine_Taxes = new Line_Taxes();
            mvarSP_Prices = new SP_Prices();

            mvarNoLoading = false;
            mvarLoadFuelAmount = false;
            mvarConfirmed = true;
            mvarIsFuel = false;
            mvarHotButton = false; //  
            mvarNoPriceFormat = false; //  
            mvarNoPromo = false; //  
            mvarQtyForPromo = 0; //  
            mvarForPromo = false; //  
            mvarPriceCheck = false; // 
            mvarPaidByCard = (byte)0;
            mvarUpsell = false;
            mvarRefreshPrice = false;
            mvarSendToLCD = true; // line


            mvarDept = string.Empty;
            mvarSub_Dept = string.Empty;
            mvarSub_Detail = string.Empty;


        }
        public Sale_Line()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mvarCharges = null;
            mvarLine_Kits = null;
            mvarReturn_Reasons = null;
            mvarLine_Taxes = null;
            mvarSP_Prices = null;
        }
        public void Dispose()
        {

            //base.Finalize();
        }

        ~Sale_Line()
        {
            Class_Terminate_Renamed();
        }

        ////private Line_Taxes Make_Taxes()
        ////{
        ////    Line_Taxes returnValue = default(Line_Taxes);
        ////    //Smriti move this code to manager
        ////    //    ADODB.Recordset Taxes = default(ADODB.Recordset);
        ////    //    ADODB.Recordset T_Main = default(ADODB.Recordset);
        ////    //    ADODB.Recordset T_Code = default(ADODB.Recordset);
        ////    //    Sale_Tax STX;
        ////    //    Line_Taxes LT = default(Line_Taxes);
        ////    //    bool Tax_Exists;
        ////    //    string cStock = "";

        ////    //    cStock = this.Stock_Code;

        ////    //    STX = new Sale_Tax();
        ////    //    LT = new Line_Taxes();

        ////    //    
        ////    //    
        ////    //    if (this.Stock_Type == "G")
        ////    //    {
        ////    //        
        ////    //        STX = null;
        ////    //        LT = null;
        ////    //        returnValue = LT;
        ////    //        return returnValue;
        ////    //    }

        ////    //    Taxes = Chaps_Main.Get_Records("SELECT StockTax.Tax_Name, StockTax.Tax_Code  FROM   StockTax  WHERE  StockTax.Stock_Code = \'" + cStock + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //    while (!Taxes.EOF)
        ////    //    {

        ////    //        T_Main = Chaps_Main.Get_Records("SELECT TaxMast.Tax_Active, TaxMast.Tax_Ord FROM TaxMast  WHERE Tax_Name = \'" + System.Convert.ToString(Taxes.Fields["Tax_Name"].Value) + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //        if (!(T_Main.EOF && T_Main.BOF))
        ////    //        {
        ////    //            if (T_Main.Fields["Tax_Active"].Value)
        ////    //            {

        ////    //                T_Code = Chaps_Main.Get_Records("SELECT Tax_Rate, Tax_Incl, Tax_Rebate FROM TaxRate  WHERE Tax_Name = \'" + System.Convert.ToString(Taxes.Fields["Tax_Name"].Value) + "\'" + " AND " + "      Tax_Code = \'" + System.Convert.ToString(Taxes.Fields["Tax_Code"].Value) + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //                if (!T_Code.EOF)
        ////    //                {
        ////    //                    LT.Add(System.Convert.ToString(Taxes.Fields["Tax_Name"].Value), System.Convert.ToString(Taxes.Fields["Tax_Code"].Value), System.Convert.ToSingle(T_Code.Fields["Tax_Rate"].Value), System.Convert.ToBoolean(T_Code.Fields["Tax_Incl"].Value), System.Convert.ToSingle((Information.IsDBNull(T_Code.Fields["Tax_Rebate"].Value)) ? 0 : (T_Code.Fields["Tax_Rebate"].Value)), 0, System.Convert.ToString(Taxes.Fields["Tax_Name"].Value));
        ////    //                }

        ////    //            }
        ////    //        }
        ////    //        Taxes.MoveNext();
        ////    //    }

        ////    //    Taxes = null;
        ////    //    T_Main = null;
        ////    //    T_Code = null;
        ////    //    returnValue = LT;
        ////    //    LT = null;
        ////    //    STX = null;
        ////    return returnValue;
        ////}

        ////private SP_Prices Make_Prices()
        ////{
        ////    SP_Prices returnValue = default(SP_Prices);

        ////    //    SP_Prices Pr = default(SP_Prices);
        ////    //    ADODB.Recordset Prices;

        ////    //    // No Special pricing for Gift Certificates
        ////    //    
        ////    //    
        ////    //    if (this.Stock_Type == "G")
        ////    //    {
        ////    //        
        ////    //        returnValue = null;
        ////    //        return returnValue;
        ////    //    }
        ////    //    ///###PERFORMAPRIL09###- Started - Takeout group pricing check

        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    // Get Special Item Pricing
        ////    //    Pr = MakeItemPrice();
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    ///                                  IIf(Me.Sub_Dept = "", GetResString(347), Me.Sub_Dept), _
        ////    //    ///                                  IIf(Me.Sub_Detail = "", GetResString(347), Me.Sub_Detail))
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    ///                                  IIf(Me.Sub_Dept = "", GetResString(347), Me.Sub_Dept), _
        ////    //    ///                                  GetResString(347))
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    ///                                  GetResString(347), _
        ////    //    ///                                  GetResString(347))
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    ///                                IIf(Me.Sub_Dept = "", GetResString(347), Me.Sub_Dept), _
        ////    //    ///                                IIf(Me.Sub_Detail = "", GetResString(347), Me.Sub_Detail))
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    ///                              IIf(Me.Sub_Dept = "", GetResString(347), Me.Sub_Dept), _
        ////    //    ///                              GetResString(347))
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    ///                              GetResString(347), _
        ////    //    ///                              GetResString(347))
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    
        ////    //    //###PERFORMAPRIL09###- END
        ////    //    Prices = null;
        ////    //    returnValue = Pr;
        ////    //    Pr = null;

        ////    return returnValue;
        ////}

        ////private Line_Kits Make_Kits()
        ////{
        ////    Line_Kits returnValue = default(Line_Kits);
        ////    //Smriti move this code to manager
        ////    //short n = 0;
        ////    //Line_Kits Line_Kits_Renamed = default(Line_Kits);
        ////    //ADODB.Recordset rs = default(ADODB.Recordset);
        ////    //ADODB.Recordset rp = default(ADODB.Recordset);
        ////    //ADODB.Recordset rt = default(ADODB.Recordset);
        ////    //ADODB.Recordset Kits = default(ADODB.Recordset);
        ////    //ADODB.Recordset Charges_Renamed = default(ADODB.Recordset);
        ////    //ADODB.Recordset T_Main = default(ADODB.Recordset);
        ////    //K_Charges Kit_Charges = default(K_Charges);
        ////    //Charge_Taxes cts = default(Charge_Taxes);
        ////    //string cStock = "";
        ////    //string VendorID = "";
        ////    //string Where_Vendor = "";

        ////    //cStock = this.Stock_Code;

        ////    //Kit_Charges = new K_Charges();
        ////    //Line_Kits_Renamed = new Line_Kits();

        ////    //
        ////    //
        ////    //if (this.Stock_Type == "G")
        ////    //{
        ////    //    Kit_Charges = null;
        ////    //    Line_Kits_Renamed = null;
        ////    //    returnValue = Line_Kits_Renamed;
        ////    //    return returnValue;
        ////    //}

        ////    //// Load Kit Components
        ////    //Kits = Chaps_Main.Get_Records("select * from Kit_Mast", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        ////    //
        ////    //Kits.Find(Criteria: "KIT_CODE=\'" + cStock + "\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);

        ////    //if (!Kits.EOF)
        ////    //{

        ////    //    Kits = Chaps_Main.Get_Records("SELECT Kit_Item.Stock_Code, Kit_Item.Quantity  FROM   Kit_Item  WHERE  Kit_Item.Kit_Code = \'" + cStock + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //    n = (short)0;
        ////    //    while (!Kits.EOF)
        ////    //    {
        ////    //        n++;
        ////    //        rs = Chaps_Main.Get_Records("SELECT *  FROM StockMst  WHERE StockMst.Stock_Code = \'" + System.Convert.ToString(Kits.Fields["Stock_Code"].Value) + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //        if (!rs.EOF)
        ////    //        {
        ////    //            VendorID = System.Convert.ToString((Information.IsDBNull(rs.Fields["Vendor"].Value)) ? "" : (rs.Fields["Vendor"].Value));
        ////    //        }

        ////    //        if (string.IsNullOrEmpty(VendorID))
        ////    //        {
        ////    //            //                Where_Vendor = " AND (VendorID='' OR VendorID IS NULL) AND OrderNo=0 "
        ////    //            // design
        ////    //            Where_Vendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
        ////    //        }
        ////    //        else
        ////    //        {
        ////    //            Where_Vendor = " AND VendorID=\'" + VendorID + "\' AND OrderNo=0 ";
        ////    //        }

        ////    //        rp = Chaps_Main.Get_Records("SELECT *  FROM Stock_Prices  WHERE Stock_Prices.Stock_Code = \'" + System.Convert.ToString(Kits.Fields["Stock_Code"].Value) + "\' AND " + "      Stock_Prices.Price_Number = 1 " + Where_Vendor, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //        if (rp.EOF)
        ////    //        {
        ////    //            //                Set rp = Get_Records( _
        ////    //            
        ////    //            
        ////    //            
        ////    //            
        ////    //            
        ////    //            // design
        ////    //            rp = Chaps_Main.Get_Records("SELECT *  FROM Stock_Prices  WHERE Stock_Prices.Stock_Code = \'" + System.Convert.ToString(Kits.Fields["Stock_Code"].Value) + "\' AND " + "Stock_Prices.Price_Number = 1 AND (VendorID=\'ALL\' AND OrderNo=0) ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        ////    //        }

        ////    //        if (!rs.EOF)
        ////    //        {

        ////    //            // Get Charges on Kit Items
        ////    //            // Build the collection of charges
        ////    //            Charges_Renamed = Chaps_Main.Get_Records("SELECT STOCK_AC.STOCK_CODE, ASSOC.AS_CODE, " + "       ASSOC.AS_DESC, ASSOC.AS_PRICE  FROM   ASSOC INNER JOIN STOCK_AC ON " + "       ASSOC.AS_CODE = STOCK_AC.AS_CODE  WHERE  STOCK_AC.STOCK_CODE = \'" + System.Convert.ToString(rs.Fields["Stock_Code"].Value) + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //            if (!Charges_Renamed.EOF)
        ////    //            {

        ////    //                rt = Chaps_Main.Get_Records("SELECT Assoc_Tax.Tax_Name, " + "       Assoc_Tax.Tax_Code, " + "       TaxRate.Tax_Incl, " + "       TaxRate.Tax_Rate  FROM   Assoc_Tax INNER JOIN TaxRate ON " + "       (Assoc_Tax.Tax_Name = TaxRate.Tax_Name AND " + "        Assoc_Tax.Tax_Code = TaxRate.Tax_Code)  WHERE  Assoc_Tax.As_Code = \'" + System.Convert.ToString(Charges_Renamed.Fields["As_Code"].Value) + "\' ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //                if (!(rt.EOF && rt.BOF))
        ////    //                {
        ////    //                    T_Main = Chaps_Main.Get_Records("SELECT TaxMast.Tax_Active, TaxMast.Tax_Ord FROM TaxMast  WHERE Tax_Name = \'" + System.Convert.ToString(rt.Fields["Tax_Name"].Value) + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //                    if (!T_Main.EOF)
        ////    //                    {
        ////    //                        cts = new Charge_Taxes();
        ////    //                        if (T_Main.Fields["Tax_Active"].Value)
        ////    //                        {
        ////    //                            while (!rt.EOF)
        ////    //                            {
        ////    //                                cts.Add(System.Convert.ToString(rt.Fields["Tax_Name"].Value), System.Convert.ToString(rt.Fields["Tax_Code"].Value), System.Convert.ToSingle(rt.Fields["Tax_Rate"].Value), System.Convert.ToBoolean(rt.Fields["Tax_Incl"].Value), "");
        ////    //                                rt.MoveNext();
        ////    //                            }
        ////    //                        }
        ////    //                    }
        ////    //                }

        ////    //                n = (short)0;
        ////    //                Kit_Charges = new K_Charges();
        ////    //                while (!Charges_Renamed.EOF)
        ////    //                {
        ////    //                    n++;
        ////    //                    Kit_Charges.Add(System.Convert.ToDouble(Charges_Renamed.Fields["As_Price"].Value), System.Convert.ToString(Charges_Renamed.Fields["As_Desc"].Value), System.Convert.ToString(Charges_Renamed.Fields["As_Code"].Value), cts, "");
        ////    //                    Charges_Renamed.MoveNext();
        ////    //                }


        ////    //            }


        ////    //            Charges_Renamed = null;

        ////    //            // Add the kit item & charges to the kits collection
        ////    //            Line_Kits_Renamed.Add(System.Convert.ToString(Kits.Fields["Stock_Code"].Value), System.Convert.ToString(rs.Fields["Descript"].Value), System.Convert.ToSingle(Kits.Fields["Quantity"].Value), System.Convert.ToSingle(rp.Fields["price"].Value), 0, 0, "", Kit_Charges, "");

        ////    //            Kit_Charges = null;
        ////    //        }
        ////    //        else
        ////    //        {
        ////    //            //TIMsgbox "Kit Item " & Kits![Stock_Code] & " not found in stock master.", vbInformation + vbOKOnly, "Missing Kit Item"
        ////    //            MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
        ////    //            Chaps_Main.DisplayMessage(0, (short)8113, temp_VbStyle, Kits.Fields["Stock_Code"], (byte)0);
        ////    //        }
        ////    //        Kits.MoveNext();
        ////    //    }

        ////    //}
        ////    //returnValue = Line_Kits_Renamed;

        ////    //Kits = null;
        ////    //rs = null;
        ////    //rp = null;
        ////    //rt = null;
        ////    //Line_Kits_Renamed = null;
        ////    //T_Main = null;
        ////    //cts = null;

        ////    return returnValue;
        ////}

        ////private Charges Make_Charges()
        ////{
        ////    Charges returnValue = default(Charges);
        ////    //Smriti move this code to manager
        ////    //    short n;
        ////    //    short m;
        ////    //    ADODB.Recordset Charges_Renamed = default(ADODB.Recordset);
        ////    //    ADODB.Recordset rs = default(ADODB.Recordset);
        ////    //    ADODB.Recordset T_Main = default(ADODB.Recordset);
        ////    //    Charge_Taxes CS = default(Charge_Taxes);
        ////    //    Charges CG = default(Charges);
        ////    //    string cStock = "";

        ////    //    cStock = this.Stock_Code;

        ////    //    CG = new Charges();
        ////    //    CS = new Charge_Taxes();

        ////    //    
        ////    //    
        ////    //    if (this.Stock_Type == "G")
        ////    //    {
        ////    //        
        ////    //        CS = null;
        ////    //        CG = null;
        ////    //        returnValue = CG;
        ////    //        return returnValue;
        ////    //    }

        ////    //    // Load Charges
        ////    //    Charges_Renamed = Chaps_Main.Get_Records("SELECT STOCK_AC.STOCK_CODE, ASSOC.AS_CODE, " + "       ASSOC.AS_DESC, ASSOC.AS_PRICE   FROM   ASSOC INNER JOIN STOCK_AC ON " + "       ASSOC.AS_CODE = STOCK_AC.AS_CODE  WHERE  STOCK_AC.STOCK_CODE = \'" + cStock + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //    n = (short)0;
        ////    //    while (!Charges_Renamed.EOF)
        ////    //    {

        ////    //        rs = Chaps_Main.Get_Records("SELECT Assoc_Tax.Tax_Name, " + "       Assoc_Tax.Tax_Code, " + "       TaxRate.Tax_Incl, " + "       TaxRate.Tax_Rate  FROM   Assoc_Tax INNER JOIN TaxRate ON " + "       (Assoc_Tax.Tax_Name = TaxRate.Tax_Name AND " + "        Assoc_Tax.Tax_Code = TaxRate.Tax_Code)  WHERE  Assoc_Tax.As_Code = \'" + System.Convert.ToString(Charges_Renamed.Fields["As_Code"].Value) + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //        CS = new Charge_Taxes();
        ////    //        while (!rs.EOF)
        ////    //        {
        ////    //            T_Main = Chaps_Main.Get_Records("SELECT TaxMast.Tax_Active, TaxMast.Tax_Ord FROM TaxMast  WHERE Tax_Name = \'" + System.Convert.ToString(rs.Fields["Tax_Name"].Value) + "\' ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //            if (!T_Main.EOF)
        ////    //            {
        ////    //                if (T_Main.Fields["Tax_Active"].Value)
        ////    //                {
        ////    //                    CS.Add(System.Convert.ToString(rs.Fields["Tax_Name"].Value), System.Convert.ToString(rs.Fields["Tax_Code"].Value), System.Convert.ToSingle(rs.Fields["Tax_Rate"].Value), System.Convert.ToBoolean(rs.Fields["Tax_Incl"].Value), "");
        ////    //                }
        ////    //            }
        ////    //            rs.MoveNext();
        ////    //        }

        ////    //        CG.Add(System.Convert.ToString(Charges_Renamed.Fields["As_Code"].Value), System.Convert.ToString(Charges_Renamed.Fields["As_Desc"].Value), System.Convert.ToSingle(Charges_Renamed.Fields["As_Price"].Value), CS, "");

        ////    //        Charges_Renamed.MoveNext();
        ////    //    }

        ////    //    CS = null;
        ////    //    Charges_Renamed = null;
        ////    //    rs = null;
        ////    //    returnValue = CG;
        ////    //    CG = null;
        ////    //    T_Main = null;

        ////    return returnValue;
        ////}

        ////public void Apply_Table_Discount(short PC, short cc)
        ////{

        ////    //    
        ////    //    
        ////    //    if (!policyManager.OR_USER_DISC && this.User_Discount)
        ////    //    {
        ////    //        return;
        ////    //    }
        ////    //    

        ////    //    
        ////    //    
        ////    //    if (!policyManager.AUTO_SALE && this.Price_Type != "R")
        ////    //    {
        ////    //        return;
        ////    //    }
        ////    //    

        ////    //    
        ////    //    int maxDisc = 0;
        ////    //    ADODB.Recordset rs = default(ADODB.Recordset);
        ////    //    string temp_Policy_Name = "CL_DISCOUNTS";
        ////    //    if (modPolicy.GetPol(temp_Policy_Name, this) && (!this.Gift_Certificate || (this.Gift_Certificate && GC_DISCOUNT)))
        ////    //    {
        ////    //        
        ////    //        ///       (Not Me.Gift_Certificate Or (Me.Gift_Certificate And GC_DISCOUNT)) Then
        ////    //        


        ////    //        
        ////    //        string temp_Policy_Name2 = "MAX_DISC%";
        ////    //        maxDisc = System.Convert.ToInt32(modPolicy.GetPol(temp_Policy_Name2, this));
        ////    //        
        ////    //        

        ////    //        rs = Chaps_Main.Get_Records("Select * From   DiscTab  WHERE  DiscTab.Prod_Disc = " + System.Convert.ToString(PC) + " AND " + "       DiscTab.Cust_Disc = " + System.Convert.ToString(cc) + " ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //        if (!rs.EOF)
        ////    //        {
        ////    //            if (System.Convert.ToInt32(rs.Fields["Disc_Perc"].Value) <= maxDisc)
        ////    //            {
        ////    //                this.Discount_Type = "%";
        ////    //                this.Discount_Rate = System.Convert.ToSingle(rs.Fields["Disc_Perc"].Value);
        ////    //            }
        ////    //            else
        ////    //            {
        ////    //                MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
        ////    //                Chaps_Main.DisplayMessage(0, (short)8196, temp_VbStyle, maxDisc, (byte)0);
        ////    //            }
        ////    //        }
        ////    //        else
        ////    //        {
        ////    //            this.Discount_Type = " ";
        ////    //            this.Discount_Rate = 0;
        ////    //        }
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        this.Discount_Type = "";
        ////    //        this.Discount_Rate = 0;
        ////    //    }
        ////    //    //  For discount loyalty, system was not showing the savings
        ////    //    short Loyalty_Discount_Code;
        ////    //    Loyalty_Discount_Code = LOYAL_DISC;
        ////    //    this.LoyaltyDiscount = Loyalty_Discount_Code == cc;
        ////    //    //shiny end
        ////    //    rs = null;
        ////}

        ////
        ////public void ApplyFuelLoyalty(string DiscountType, float DiscountRate, string DiscountName = "")
        ////{
        ////    //Smriti move this code to manager
        ////    //    string fs;
        ////    //    if (DiscountType == "")
        ////    //    {
        ////    //        return;
        ////    //    }

        ////    //    switch (DiscountType)
        ////    //    {
        ////    //        case "%": 
        ////    //            this.Discount_Type = "%";
        ////    //            this.Discount_Rate = DiscountRate;
        ////    //            this.DiscountName = DiscountName; // 
        ////    //            break;
        ////    //        case "$": 
        ////    //        case "D":
        ////    //            //  - discount chart is entered in cents, we need to make it as $
        ////    //            if (DiscountType == "D")
        ////    //            {
        ////    //                if (DiscountRate != 0)
        ////    //                {
        ////    //                    DiscountRate = DiscountRate / 100;
        ////    //                }
        ////    //            }
        ////    //            //shiny end
        ////    //            this.Discount_Type = "$";
        ////    //            this.DiscountName = DiscountName; // 
        ////    //                                              
        ////    //                                              
        ////    //                                              
        ////    //                                              
        ////    //                                              
        ////    //                                              /
        ////    //            
        ////    //            LINE_TYPE = "Each Item"; 
        ////    //                                     //  (2.5C) to 3 c
        ////    //                                     //            Me.Discount_Rate = Format(DiscountRate, "#0.00")
        ////    //                                     //            fs = IIf(GetPol("PRICE_DEC", Me) = 0, "0", "0." & String$(GetPol("PRICE_DEC", Me), "0"))
        ////    //            this.Discount_Rate = DiscountRate; //Format(DiscountRate, fs)
        ////    //            break;
        ////    //        // 09
        ////    //        
        ////    //        default:
        ////    //            this.Discount_Type = " ";
        ////    //            this.Discount_Rate = 0;
        ////    //            this.DiscountName = ""; // 
        ////    //            break;
        ////    //    }

        ////}
        //////
        ////public float GetFuelDiscountChartRate(string GroupID, byte GradeID)
        ////{
        ////    float returnValue = 0;
        ////    //Smriti move this code to manager
        ////    //    ADODB.Recordset rschart = new ADODB.Recordset();
        ////    //    returnValue = 0;
        ////    //    rschart = Chaps_Main.Get_Records("SELECT DiscountRate FROM FuelDiscountChart  WHERE GroupID = \'" + GroupID + "\' and Grade= " + System.Convert.ToString(GradeID), Chaps_Main.dbMaster);
        ////    //    if (!rschart.EOF)
        ////    //    {
        ////    //        returnValue = System.Convert.ToSingle((Information.IsDBNull(rschart.Fields["DiscountRate"].Value)) ? 0 : (rschart.Fields["DiscountRate"].Value));
        ////    //    }

        ////    //    rschart = null;
        ////    return returnValue;
        ////}

        ////
        ////public void ApplyFuelRebate()
        ////{
        ////    //Smriti move this code to manager
        ////    //if (this.FuelRebate <= 0)
        ////    //{
        ////    //    return;
        ////    //}

        ////    //this.Discount_Type = "$";
        ////    //
        ////    //
        ////    //

        ////    //
        ////    //
        ////    //LINE_TYPE = "Each Item"; 
        ////    //this.Discount_Rate = float.Parse(this.FuelRebate.ToString("#0.00"));
        ////    //

        ////}
        //

        //private SP_Prices MakeItemPrice()
        //{
        //    //Smriti move this code to manager
        //    SP_Prices returnValue = default(SP_Prices);

        //    //SP_Prices SP = default(SP_Prices);
        //    //ADODB.Recordset Prices = default(ADODB.Recordset);
        //    //string cStock = "";
        //    //object Pr_From = null;
        //    //object Pr_To = null;
        //    //string Where_Vendor = "";

        //    ////###PERFORMAPRIL09### ''Stock.Find "STOCK_CODE='" & Me.Stock_Code & "'", , adSearchForward, 1
        //    //cStock = this.Stock_Code;
        //    //// Pr_From, Pr_To could be set from PriceL if nothing is set for active vendor, so don't set these variables here
        //    /////    Pr_From = Stock![Pr_From]
        //    /////    Pr_To = Stock![Pr_To]

        //    //SP = new SP_Prices();

        //    ////  
        //    //if (this.Price_Type == "R" && this.PromoID != "")
        //    //{
        //    //    ///        SP.Add 1, Me.QtyInPromoLine * SA.NoOfPromo, Me.Promo_Price, Date, DateAdd("yyyy", 1, Date)
        //    //    SP.Add(1, this.QtyForPromo, (float)(this.Promo_Price), DateAndTime.Today, DateAndTime.DateAdd(Microsoft.VisualBasic.DateInterval.Year, 1, DateAndTime.Today), "");
        //    //    returnValue = SP;
        //    //    //  

        //    //}
        //    //else if (this.Price_Type != "R")
        //    //{

        //    //    if (string.IsNullOrEmpty(mvarVendor))
        //    //    {
        //    //        //            Where_Vendor = " AND (VendorID='' OR VendorID IS NULL) AND OrderNo=0 "
        //    //        // design
        //    //        Where_Vendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
        //    //    }
        //    //    else
        //    //    {
        //    //        Where_Vendor = " AND VendorID=\'" + mvarVendor + "\' AND OrderNo=0 ";
        //    //    }

        //    //    Prices = Chaps_Main.Get_Records("SELECT   Pr_F_Qty, Pr_T_Qty, Price, StartDate, EndDate   FROM     PriceL  WHERE    PriceL.Stock_Code = \'" + cStock + "\' AND StartDate<=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' AND EndDate>=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' " + Where_Vendor + "ORDER BY PriceL.Pr_F_Qty", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //    //    // if no specific special price was set for the active vendor, look for the price applicable to all vendors
        //    //    if (Prices.EOF)
        //    //    {
        //    //        //            Set Prices = Get_Records( _
        //    //        
        //    //        
        //    //        
        //    //        
        //    //        
        //    //        
        //    //        
        //    //        // design
        //    //        Prices = Chaps_Main.Get_Records("SELECT   Pr_F_Qty, Pr_T_Qty, Price, StartDate, EndDate  FROM     PriceL  WHERE    PriceL.Stock_Code = \'" + cStock + "\' " + "AND StartDate<=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' AND EndDate>=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' " + "AND (VendorID=\'ALL\' AND OrderNo=0) " + "ORDER BY PriceL.Pr_F_Qty", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        //    //        if (!Prices.EOF)
        //    //        {
        //    //            Pr_From = (Information.IsDBNull(Prices.Fields["StartDate"].Value)) ? DateAndTime.Today : (Prices.Fields["StartDate"].Value);
        //    //            Pr_To = (Information.IsDBNull(Prices.Fields["EndDate"].Value)) ? DateAndTime.Today : (Prices.Fields["EndDate"].Value);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        Pr_From = (Information.IsDBNull(Prices.Fields["StartDate"].Value)) ? DateAndTime.Today : (Prices.Fields["StartDate"].Value);
        //    //        Pr_To = (Information.IsDBNull(Prices.Fields["EndDate"].Value)) ? DateAndTime.Today : (Prices.Fields["EndDate"].Value);
        //    //    }

        //    //    while (!Prices.EOF)
        //    //    {
        //    //        SP.Add(System.Convert.ToSingle(Prices.Fields["Pr_F_Qty"].Value), System.Convert.ToSingle(Prices.Fields["pr_T_Qty"].Value), System.Convert.ToSingle(Prices.Fields["price"].Value), Pr_From, Pr_To, "");
        //    //        Prices.MoveNext();
        //    //    }
        //    //    returnValue = SP;

        //    //}
        //    //else
        //    //{
        //    //    returnValue = null;
        //    //}

        //    //SP = null;
        //    //Prices = null;

        //    return returnValue;
        //}

        ////private SP_Prices MakeGroupPrice(string Dept, string SubDept, string SubDetail)
        ////{
        ////    //Smriti move this code to manager
        ////    SP_Prices returnValue = default(SP_Prices);
        ////    //ADODB.Recordset Prices = default(ADODB.Recordset);
        ////    //ADODB.Recordset rp = default(ADODB.Recordset);
        ////    //SP_Prices SP = default(SP_Prices);

        ////    //rp = Chaps_Main.Get_Records("Select * From GP_Head  WHERE Dept = \'" + Dept + "\' AND " + "      SubDept = \'" + SubDept + "\' AND " + "      SubDetail = \'" + SubDetail + "\' AND " + "      Pr_Type <> \'R\' ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //SP = new SP_Prices();

        ////    //if (!(rp.EOF && rp.BOF))
        ////    //{

        ////    //    if (rp.Fields["pr_type"].Value != "R" && (DateTime)(rp.Fields["Pr_From"].Value) <= DateAndTime.Today && (Information.IsDBNull(rp.Fields["Pr_To"].Value) || (int)(rp.Fields["Pr_To"].Value) == 0 || (DateTime)(rp.Fields["Pr_To"].Value) >= DateAndTime.Today))
        ////    //    {

        ////    //        this.Price_Type = System.Convert.ToString((Information.IsDBNull(rp.Fields["pr_type"].Value)) ? "" : (rp.Fields["pr_type"].Value));
        ////    //        this.Price_Units = System.Convert.ToString(rp.Fields["Pr_Units"].Value);

        ////    //        Prices = Chaps_Main.Get_Records("Select *  FROM   GP_Lines  WHERE Dept = \'" + Dept + "\' AND " + "      SubDept = \'" + SubDept + "\' AND " + "      SubDetail = \'" + SubDetail + "\' " + "Order By GP_Lines.Pr_F_Qty ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //        while (!Prices.EOF)
        ////    //        {
        ////    //            SP.Add(System.Convert.ToSingle(Prices.Fields["Pr_F_Qty"].Value), System.Convert.ToSingle(Prices.Fields["pr_T_Qty"].Value), System.Convert.ToSingle(Prices.Fields["price"].Value), rp.Fields["Pr_From"], rp.Fields["Pr_To"], "");
        ////    //            Prices.MoveNext();
        ////    //        }
        ////    //        returnValue = SP;
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        returnValue = null;
        ////    //    }

        ////    //}

        ////    //Prices = null;
        ////    //rp = null;
        ////    //SP = null;

        ////    return returnValue;
        ////}
        ////

        ////public void LoadSaleLinePolicies()
        ////{
        ////    //Smriti move this code to manager
        ////    //string temp_Policy_Name = "QUANT_DEC";
        ////    //mvarQUANT_DEC = System.Convert.ToInt16(modPolicy.GetPol(temp_Policy_Name, this));
        ////    //string temp_Policy_Name2 = "PRICE_DEC";
        ////    //mvarPRICE_DEC = System.Convert.ToInt16(modPolicy.GetPol(temp_Policy_Name2, this));
        ////    //string temp_Policy_Name3 = "LOY-EXCLUDE";
        ////    //mvarLOY_EXCLUDE = System.Convert.ToBoolean(modPolicy.GetPol(temp_Policy_Name3, this));
        ////    //string temp_Policy_Name4 = "I_RIGOR";
        ////    //mvarI_RIGOR = System.Convert.ToBoolean(modPolicy.GetPol(temp_Policy_Name4, this));
        ////    //string temp_Policy_Name5 = "VOL_POINTS";
        ////    //mvarVOL_POINTS = System.Convert.ToInt16(modPolicy.GetPol(temp_Policy_Name5, this));
        ////    //string temp_Policy_Name6 = "LOYAL_PPU";
        ////    //mvarLOYAL_PPU = System.Convert.ToInt16(modPolicy.GetPol(temp_Policy_Name6, this));
        ////    //string temp_Policy_Name7 = "TE_COLLECTTAX";
        ////    //mvarTE_COLLECTTAX = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name7, this));

        ////    //
        ////    //if (this.Stock_Type == "G")
        ////    //{
        ////    //    string temp_Policy_Name8 = "GiftType";
        ////    //    mvarGiftType = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name8, this)); //NONE,LocalGift,GiveX,Milliplein
        ////    //}
        ////    //else
        ////    //{
        ////    //    mvarGiftType = "NONE";
        ////    //}
        ////    //

        ////    //TODO: Ackroo_Removed
        ////    //////Card
        ////    //////If Me.Stock_Code = "ACKG" Then
        ////    ////if (IsAckrooGift(this.Stock_Code))
        ////    ////{
        ////    ////    string temp_Policy_Name9 = "REWARDS_Gift";
        ////    ////    mvarGiftCard = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name9, this)); //NONE,AckrooGift
        ////    ////}
        ////    ////else
        ////    ////{
        ////    ////    mvarGiftCard = "NONE";
        ////    ////}
        ////    //////end
        ////    //END: Ackroo_Removed

        ////    //
        ////    //if (policyManager.ThirdParty)
        ////    //{
        ////    //    string temp_Policy_Name10 = "TrdPtyExt";
        ////    //    mvarThirdPartyExtractCode = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name10, this)); //3,5,6,7,10
        ////    //}
        ////    //

        ////    //
        ////    //if (policyManager.TAX_EXEMPT)
        ////    //{
        ////    //    string temp_Policy_Name11 = "TE_AgeRstr";
        ////    //    mvarTE_AgeRstr = System.Convert.ToBoolean(modPolicy.GetPol(temp_Policy_Name11, this));
        ////    //}
        ////    //else
        ////    //{
        ////    //    mvarTE_AgeRstr = false;
        ////    //}
        ////    //
        ////}
        //

        ////TODO: Ackroo_Removed
        ////Card
        ////private bool IsAckrooGift(string sCode)
        ////{
        ////    bool returnValue = false;
        ////    //SMriti move this code to manager
        //////    string strSql = "";
        //////    ADODB.Recordset rs = default(ADODB.Recordset);

        //////    strSql = "Select [P_SET] From [P_SET] Where P_VALUE = \'" + sCode + "\' ";
        //////    rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbAdmin, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //////    if (rs.EOF)
        //////    {
        //////        rs = null;
        //////        returnValue = false;
        //////        return returnValue;
        //////    }
        //////    else
        //////    {
        //////        if ((string)(rs.Fields["P_Set"].Value) == "AckrooGift" || (string)(rs.Fields["P_Set"].Value) == "GivexGift")
        //////        {
        //////            returnValue = true;
        //////        }
        //////    }

        //////    return returnValue;
        //////}
        ////END: Ackroo_Removed

        ////// This function is used by Make_Promo. It returns the total qty of the items in sale
        ////// by levels (stock code, dept,...) except the item in the current line
        ////// July 03, 2009 modified to be by link and not by level; need to know number of items
        ////// in the sale belonging to the same link not level
        ////private double Get_TotalQuantity(byte Link, string PromoID = "")
        ////{
        ////    double returnValue = 0;
        ////    //    dynamic Promos_Renamed = default(dynamic);

        ////    //    Sale_Line SL = default(Sale_Line);
        ////    //    Promo_Line Pr = default(Promo_Line);
        ////    //    double TotQty = 0;
        ////    //    Promo Promo_Renamed = default(Promo);
        ////    //    bool boolExistingPromo = false;
        ////    //    short i;
        ////    //    Promo pro = default(Promo);

        ////    //    if (PromoID.Trim().Length != 0)
        ////    //    {
        ////    //        boolExistingPromo = false;
        ////    //        foreach (Promo tempLoopVar_pro in (IEnumerable)Promos_Renamed)
        ////    //        {
        ////    //            pro = tempLoopVar_pro;
        ////    //            if (pro.PromoID == PromoID)
        ////    //            {
        ////    //                boolExistingPromo = true;
        ////    //                break;
        ////    //            }
        ////    //        }
        ////    //        if (!boolExistingPromo)
        ////    //        {
        ////    //        }
        ////    //        Promo_Renamed = Promos_Renamed.Fields("Qty").Value.Item(PromoID);
        ////    //    }

        ////    //    // PromoID can be empty string only for stock code
        ////    //    if (PromoID.Trim().Length == 0)
        ////    //    {
        ////    //        foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //        {
        ////    //            SL = tempLoopVar_SL;
        ////    //            if (SL.Stock_Code == mvarStock_Code)
        ////    //            {
        ////    //                TotQty = TotQty + SL.Quantity;
        ////    //            }
        ////    //        }
        ////    //        returnValue = TotQty;
        ////    //        return returnValue;
        ////    //    }

        ////    //    foreach (Promo_Line tempLoopVar_Pr in Promo_Renamed.Promo_Lines)
        ////    //    {
        ////    //        Pr = tempLoopVar_Pr;
        ////    //        if (Pr.Link == Link)
        ////    //        {

        ////    //            if (Pr.Level == ((byte)1))
        ////    //            {
        ////    //                foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //                {
        ////    //                    SL = tempLoopVar_SL;
        ////    //                    if (SL.Stock_Code == Pr.Stock_Code && (SL.PromoID == PromoID || SL.PromoID == ""))
        ////    //                    {
        ////    //                        TotQty = TotQty + SL.Quantity;
        ////    //                    }
        ////    //                }
        ////    //            }
        ////    //            else if (Pr.Level == ((byte)2))
        ////    //            {
        ////    //                foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //                {
        ////    //                    SL = tempLoopVar_SL;
        ////    //                    if (SL.Dept == Pr.Dept && (SL.PromoID == PromoID || SL.PromoID == ""))
        ////    //                    {
        ////    //                        TotQty = TotQty + SL.Quantity;
        ////    //                    }
        ////    //                }
        ////    //            }
        ////    //            else if (Pr.Level == ((byte)3))
        ////    //            {
        ////    //                foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //                {
        ////    //                    SL = tempLoopVar_SL;
        ////    //                    if (SL.Dept == Pr.Dept && SL.Sub_Dept == Pr.Sub_Dept && (SL.PromoID == PromoID || SL.PromoID == ""))
        ////    //                    {
        ////    //                        TotQty = TotQty + SL.Quantity;
        ////    //                    }
        ////    //                }
        ////    //            }
        ////    //            else if (Pr.Level == ((byte)4))
        ////    //            {
        ////    //                foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //                {
        ////    //                    SL = tempLoopVar_SL;
        ////    //                    if (SL.Dept == Pr.Dept && SL.Sub_Dept == Pr.Sub_Dept && SL.Sub_Detail == Pr.Sub_Detail && (SL.PromoID == PromoID || SL.PromoID == ""))
        ////    //                    {
        ////    //                        TotQty = TotQty + SL.Quantity;
        ////    //                    }
        ////    //                }
        ////    //            }

        ////    //        }
        ////    //    }

        ////    //    //    Select Case Level
        ////    //    //
        ////    //    //    Case 1
        ////    //    //        For Each sl In SA.Sale_Lines
        ////    //    //            If Len(Trim$(PromoID)) = 0 Then
        ////    //    //                If sl.Stock_Code = mvarStock_Code Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            Else
        ////    //    //                If sl.Stock_Code = mvarStock_Code And (sl.PromoID = PromoID Or sl.PromoID = "") Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            End If
        ////    //    //        Next sl
        ////    //    //    Case 2
        ////    //    //        If Promo.MaxLink = 1 And Promo.MultiLink Then
        ////    //    //            For Each sl In SA.Sale_Lines
        ////    //    //                If (sl.PromoID = PromoID Or sl.PromoID = "") Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            Next sl
        ////    //    //        Else
        ////    //    //            For Each sl In SA.Sale_Lines
        ////    //    //                If sl.Dept = Me.Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            Next sl
        ////    //    //        End If
        ////    //    //
        ////    //    //    Case 3
        ////    //    //        If Promo.MaxLink = 1 And Promo.MultiLink Then
        ////    //    //            For Each sl In SA.Sale_Lines
        ////    //    //                If sl.Dept = Me.Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            Next sl
        ////    //    //        Else
        ////    //    //            For Each sl In SA.Sale_Lines
        ////    //    //                If sl.Dept = Me.Dept And sl.Sub_Dept = Me.Sub_Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            Next sl
        ////    //    //        End If
        ////    //    //
        ////    //    //    Case 4
        ////    //    //        If Promo.MaxLink = 1 And Promo.MultiLink Then
        ////    //    //            For Each sl In SA.Sale_Lines
        ////    //    //                If sl.Dept = Me.Dept And sl.Sub_Dept = Me.Sub_Dept And (sl.PromoID = PromoID Or sl.PromoID = "") Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            Next sl
        ////    //    //        Else
        ////    //    //            For Each sl In SA.Sale_Lines
        ////    //    //                If sl.Dept = Me.Dept And sl.Sub_Dept = Me.Sub_Dept And sl.Sub_Detail = Me.Sub_Detail And (sl.PromoID = PromoID Or sl.PromoID = "") Then
        ////    //    //                    TotQty = TotQty + sl.Quantity
        ////    //    //                End If
        ////    //    //            Next sl
        ////    //    //        End If
        ////    //    //
        ////    //    //    End Select

        ////    //    returnValue = TotQty;
        ////    //    Promo_Renamed = null;

        ////    return returnValue;
        ////}
        ////// End  




        //////  
        ////// Temp... arrays keep values for the current line being processed
        ////// Arr... arrays keep values for the other lines in the sale that might be in the same promo as current item
        ////public void Make_Promo(bool ChangeQuantity = false)
        ////{
        ////    //Smriti move this code to manager
        ////    //dynamic Promos_Renamed = default(dynamic);

        ////    //bool boolItemInPromo = false;
        ////    //float tempNoOfPromo = 0;
        ////    //ADODB.Recordset rsPromoDetail;
        ////    //ADODB.Recordset rsCurSale;
        ////    //ADODB.Recordset rsPromo = default(ADODB.Recordset);
        ////    //ADODB.Recordset rsPrice;
        ////    //bool boolValidPromo = false;
        ////    //bool boolExistingPromo = false;
        ////    //string fs = "";
        ////    //string NONE = "";
        ////    //string strSql = "";
        ////    //string ProcPromoID = "";
        ////    //string[] ArrPromos = null;
        ////    //string[] ArrStockCodeToUpdate = null;
        ////    //float[] ArrQtyInPromoLine = null;
        ////    //float[] ArrTotalQtyLine = null; // keeps total qty by link, for level 1 is equal with ArrTotalQuantity value
        ////    //float[] ArrAmountPromoLine = null;
        ////    //float[] ArrReg_Price = null;
        ////    //float[] ArrLinkNo = null;
        ////    //float[] ArrTotalQuantity = null; // keeps total quantity by stock item
        ////    //float[,] tempArrPromo = new float[1, 1];
        ////    //bool[] Links = null;
        ////    //float[] RemainingQtyToProcess = null;
        ////    //short i = 0;
        ////    //short k = 0;
        ////    //short j = 0;
        ////    //short NewIndex = 0;
        ////    //double SumRegPrice = 0;
        ////    //float TQty_ByStock = 0;
        ////    //float LinkNo;
        ////    //float TotalQty = 0;
        ////    //Promo Pr = default(Promo);
        ////    //Sale_Line SL = default(Sale_Line);
        ////    //SP_Prices SP = default(SP_Prices);
        ////    //Promo objPromo = default(Promo);
        ////    //short IdxFound = 0;
        ////    //Promo_Line PL = default(Promo_Line);
        ////    //bool boolAddItem = false;
        ////    //bool boolItemExists = false;
        ////    //short kk = 0;
        ////    //string[] Arr_SC_Set = null;
        ////    //short[] Arr_Qty_Set = null;

        ////    //if (mvarNoLoading)
        ////    //{
        ////    //    return;
        ////    //}
        ////    //if (mvarNoPromo)
        ////    //{
        ////    //    return;
        ////    //}

        ////    //NONE = Chaps_Main.GetResString((short)347);
        ////    //ArrPromos = new string[1];
        ////    //ArrStockCodeToUpdate = new string[1];
        ////    //ArrQtyInPromoLine = new float[1];
        ////    //ArrTotalQtyLine = new float[1];
        ////    //ArrAmountPromoLine = new float[1];
        ////    //ArrLinkNo = new float[1];
        ////    //ArrTotalQuantity = new float[1];
        ////    //tempArrPromo = new float[6, 1];
        ////    //ArrReg_Price = new float[1];
        ////    //if (mvarHotButton || mvarIsFuel)
        ////    //{
        ////    //    ChangeQuantity = true; // added on September 02, 2008
        ////    //}

        ////    //// added on August 20, 2009 to fix the issue for quantity with decimals
        ////    //// quantity has to be already formated when the promo is made
        ////    //fs = System.Convert.ToString(mvarQUANT_DEC == 0 ? "0" : ("0." + new string('0', mvarQUANT_DEC)));
        ////    //mvarQuantity = float.Parse(mvarQuantity.ToString(fs));
        ////    //// endif

        ////    //// 1. verify item is in a promo based on stock item code
        ////    //// set the mvarTotalQty_ByStock variable, is the same for all levels because is based on stock code
        ////    //if (ChangeQuantity)
        ////    //{
        ////    //    mvarTotalQty_ByStock = Get_TotalQuantity((byte)1);
        ////    //}
        ////    //else
        ////    //{
        ////    //    mvarTotalQty_ByStock = mvarQuantity + Get_TotalQuantity((byte)1);
        ////    //}

        ////    //// August 21, 2008 took out the quantity validation to implement mix and match promotions
        ////    //strSql = "SELECT DISTINCT [PromoHeader].[PromoID] AS PromoID, [PromoHeader].[Day] AS Day, " + "[PromoDetail].[Qty] AS Qty, [PromoDetail].[Link] AS Link, " + "[PromoDetail].[Amount] AS Amount FROM PromoHeader LEFT JOIN PromoDetail " + "ON [PromoHeader].[PromoID] =[PromoDetail].[PromoID]  WHERE (Day IS NULL OR Day=0 OR Day=" + System.Convert.ToString(DateAndTime.Weekday(DateAndTime.Today, FirstDayOfWeek.Sunday)) + ") AND [PromoHeader].[EndDate]>= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoHeader].[StartDate]<= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoDetail].[Stock_Code]=\'" + mvarStock_Code + "\'";

        ////    //// mvarTotalQty calculation is based on stock_code for level 1
        ////    //// mvarTotalQty calculation is based on dept for level 2
        ////    //// mvarTotalQty calculation is based on dept and sub_dept for level 3
        ////    //// mvarTotalQty calculation is based on dept, sub_dept and subdetail for level 4

        ////    //rsPromo = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //if (rsPromo.EOF)
        ////    //{
        ////    //    boolItemInPromo = false;
        ////    //}

        ////    //while (!rsPromo.EOF)
        ////    //{
        ////    //    boolItemInPromo = true;

        ////    //    k = (short)(Information.UBound(tempArrPromo, 2) + 1);
        ////    //    tempArrPromo = (float[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)tempArrPromo, new float[6, k + 1]);
        ////    //    tempArrPromo[1, k] = System.Convert.ToSingle(rsPromo.Fields["Link"].Value);
        ////    //    tempArrPromo[2, k] = 1;
        ////    //    tempArrPromo[3, k] = System.Convert.ToSingle(rsPromo.Fields["Qty"].Value);

        ////    //    // total quantity is based on promoID because tempArrPromo(4, k) is based on promo
        ////    //    // so for different promo could be different totalqty based on link
        ////    //    // TotalQty is based on link number July 03, 2009
        ////    //    TotalQty = 0;
        ////    //    if (ChangeQuantity)
        ////    //    {
        ////    //        TotalQty = (float)(Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        TotalQty = (float)(mvarQuantity + Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }

        ////    //    tempArrPromo[4, k] = TotalQty;
        ////    //    tempArrPromo[5, k] = System.Convert.ToSingle((Information.IsDBNull(rsPromo.Fields["Amount"].Value)) ? 0 : (rsPromo.Fields["Amount"].Value));

        ////    //    Array.Resize(ref ArrPromos, (ArrPromos.Length - 1) + 1 + 1);
        ////    //    ArrPromos[ArrPromos.Length - 1] = System.Convert.ToString(rsPromo.Fields["PromoID"].Value);

        ////    //    rsPromo.MoveNext();
        ////    //}

        ////    //// 2. verify item is in promo based on dept
        ////    //strSql = "SELECT DISTINCT [PromoHeader].[PromoID] AS PromoID, [PromoHeader].[Day] AS Day, " + "[PromoDetail].[Qty] AS Qty, [PromoDetail].[Link] AS Link, " + "[PromoDetail].[Amount] AS Amount FROM PromoHeader LEFT JOIN PromoDetail " + "ON [PromoHeader].[PromoID] =[PromoDetail].[PromoID]  WHERE (Day IS NULL OR Day=0 OR Day=" + System.Convert.ToString(DateAndTime.Weekday(DateAndTime.Today, FirstDayOfWeek.Sunday)) + ") AND [PromoHeader].[EndDate]>= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoHeader].[StartDate]<= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoDetail].[Dept]=\'" + mvarDept + "\' AND [PromoDetail].[Sub_Dept]=\'" + NONE + "\' AND [PromoDetail].[SubDetail]=\'" + NONE + "\'";

        ////    //rsPromo = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //if (rsPromo.EOF && !boolItemInPromo)
        ////    //{
        ////    //    boolItemInPromo = false;
        ////    //}

        ////    //while (!rsPromo.EOF)
        ////    //{
        ////    //    boolItemInPromo = true;

        ////    //    k = (short)(Information.UBound(tempArrPromo, 2) + 1);
        ////    //    tempArrPromo = (float[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)tempArrPromo, new float[6, k + 1]);
        ////    //    tempArrPromo[1, k] = System.Convert.ToSingle(rsPromo.Fields["Link"].Value);
        ////    //    tempArrPromo[2, k] = 2;
        ////    //    tempArrPromo[3, k] = System.Convert.ToSingle(rsPromo.Fields["Qty"].Value);

        ////    //    TotalQty = 0;
        ////    //    if (ChangeQuantity)
        ////    //    {
        ////    //        TotalQty = (float)(Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        TotalQty = (float)(mvarQuantity + Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }

        ////    //    tempArrPromo[4, k] = TotalQty;
        ////    //    tempArrPromo[5, k] = System.Convert.ToSingle((Information.IsDBNull(rsPromo.Fields["Amount"].Value)) ? 0 : (rsPromo.Fields["Amount"].Value));

        ////    //    Array.Resize(ref ArrPromos, (ArrPromos.Length - 1) + 1 + 1);
        ////    //    ArrPromos[ArrPromos.Length - 1] = System.Convert.ToString(rsPromo.Fields["PromoID"].Value);

        ////    //    rsPromo.MoveNext();
        ////    //}

        ////    //// 3 verify that item is in a promo based on dept and subdept
        ////    //strSql = "SELECT DISTINCT [PromoHeader].[PromoID] AS PromoID, [PromoHeader].[Day] AS Day, " + "[PromoDetail].[Qty] AS Qty, [PromoDetail].[Link] AS Link, " + "[PromoDetail].[Amount] AS Amount FROM PromoHeader LEFT JOIN PromoDetail " + "ON [PromoHeader].[PromoID] =[PromoDetail].[PromoID]  WHERE (Day IS NULL OR Day=0 OR Day=" + System.Convert.ToString(DateAndTime.Weekday(DateAndTime.Today, FirstDayOfWeek.Sunday)) + ") AND [PromoHeader].[EndDate]>= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoHeader].[StartDate]<= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoDetail].[Dept]=\'" + mvarDept + "\' AND [PromoDetail].[Sub_Dept]=\'" + mvarSub_Dept + "\' AND [PromoDetail].[SubDetail]=\'" + NONE + "\'";

        ////    //rsPromo = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //if (rsPromo.EOF && !boolItemInPromo)
        ////    //{
        ////    //    boolItemInPromo = false;
        ////    //}

        ////    //while (!rsPromo.EOF)
        ////    //{
        ////    //    boolItemInPromo = true;

        ////    //    k = (short)(Information.UBound(tempArrPromo, 2) + 1);
        ////    //    tempArrPromo = (float[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)tempArrPromo, new float[6, k + 1]);
        ////    //    tempArrPromo[1, k] = System.Convert.ToSingle(rsPromo.Fields["Link"].Value);
        ////    //    tempArrPromo[2, k] = 3;
        ////    //    tempArrPromo[3, k] = System.Convert.ToSingle(rsPromo.Fields["Qty"].Value);

        ////    //    TotalQty = 0;
        ////    //    if (ChangeQuantity)
        ////    //    {
        ////    //        TotalQty = (float)(Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        TotalQty = (float)(mvarQuantity + Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }

        ////    //    tempArrPromo[4, k] = TotalQty;
        ////    //    tempArrPromo[5, k] = System.Convert.ToSingle((Information.IsDBNull(rsPromo.Fields["Amount"].Value)) ? 0 : (rsPromo.Fields["Amount"].Value));

        ////    //    Array.Resize(ref ArrPromos, (ArrPromos.Length - 1) + 1 + 1);
        ////    //    ArrPromos[ArrPromos.Length - 1] = System.Convert.ToString(rsPromo.Fields["PromoID"].Value);

        ////    //    rsPromo.MoveNext();
        ////    //}

        ////    //// 4 verify that item is in a promo based on dept, subdept and subdetail
        ////    //strSql = "SELECT DISTINCT [PromoHeader].[PromoID] AS PromoID,[PromoHeader].[Day] AS Day, " + "[PromoDetail].[Qty] AS Qty, [PromoDetail].[Link] AS Link, " + "[PromoDetail].[Amount] AS Amount FROM PromoHeader LEFT JOIN PromoDetail " + "ON [PromoHeader].[PromoID] =[PromoDetail].[PromoID]  WHERE (Day IS NULL OR Day=0 OR Day=" + System.Convert.ToString(DateAndTime.Weekday(DateAndTime.Today, FirstDayOfWeek.Sunday)) + ") AND [PromoHeader].[EndDate]>= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoHeader].[StartDate]<= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND " + "[PromoDetail].[Dept]=\'" + mvarDept + "\' AND [PromoDetail].[Sub_Dept]=\'" + mvarSub_Dept + "\' AND [PromoDetail].[SubDetail]=\'" + mvarSub_Detail + "\'";

        ////    //rsPromo = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

        ////    //if (rsPromo.EOF && !boolItemInPromo)
        ////    //{
        ////    //    boolItemInPromo = false;
        ////    //}

        ////    //while (!rsPromo.EOF)
        ////    //{
        ////    //    boolItemInPromo = true;

        ////    //    k = (short)(Information.UBound(tempArrPromo, 2) + 1);
        ////    //    tempArrPromo = (float[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)tempArrPromo, new float[6, k + 1]);
        ////    //    tempArrPromo[1, k] = System.Convert.ToSingle(rsPromo.Fields["Link"].Value);
        ////    //    tempArrPromo[2, k] = 4;

        ////    //    TotalQty = 0;
        ////    //    if (ChangeQuantity)
        ////    //    {
        ////    //        TotalQty = (float)(Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        TotalQty = (float)(mvarQuantity + Get_TotalQuantity((byte)tempArrPromo[1, k], System.Convert.ToString(rsPromo.Fields["PromoID"].Value)));
        ////    //    }

        ////    //    tempArrPromo[3, k] = System.Convert.ToSingle(rsPromo.Fields["Qty"].Value);
        ////    //    tempArrPromo[4, k] = TotalQty;
        ////    //    tempArrPromo[5, k] = System.Convert.ToSingle((Information.IsDBNull(rsPromo.Fields["Amount"].Value)) ? 0 : (rsPromo.Fields["Amount"].Value));

        ////    //    Array.Resize(ref ArrPromos, (ArrPromos.Length - 1) + 1 + 1);
        ////    //    ArrPromos[ArrPromos.Length - 1] = System.Convert.ToString(rsPromo.Fields["PromoID"].Value);

        ////    //    rsPromo.MoveNext();
        ////    //}

        ////    //// at this point we know the item is in a promotion and we know the level, but we have
        ////    //// to make sure all others items in the promotion are already in the sale lines
        ////    //// all others posible items in the promotion should be in the CSCCurSale DB
        ////    //// in the SaleLine table for this particular sale number
        ////    //// go throught all the posible PromoID, sorted by link. Once a link is valid,
        ////    //// (there are enought items for that link in the sale) move forward to the next link
        ////    //// in the SQL statement exclude the item and the link we alredy know is in the promo
        ////    //// A special case are promos with one link that are already valid at this point, so
        ////    //// we can set boolValidPromo= True and go further to assign the prices

        ////    //if (!boolItemInPromo)
        ////    //{
        ////    //    ProcPromoID = this.PromoID;
        ////    //    // reset the promoID and price for all other items belonging to same promo
        ////    //    foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //    {
        ////    //        SL = tempLoopVar_SL;
        ////    //        if (SL.PromoID == ProcPromoID && SL.PromoID != "") // added And SL.PromoID <> "" condition on August 06, 2008
        ////    //        {
        ////    //            SL.PromoID = "";
        ////    //            SL.price = SL.Regular_Price;
        ////    //            SL.NoPriceFormat = false;
        ////    //        }
        ////    //    }

        ////    //    //  
        ////    //    if (this.PromoID != "")
        ////    //    {
        ////    //        this.PromoID = "";
        ////    //        this.price = this.Regular_Price;
        ////    //        this.NoPriceFormat = false;
        ////    //    }
        ////    //    rsPromo = null;
        ////    //    rsCurSale = null;
        ////    //    rsPrice = null;
        ////    //    rsPromoDetail = null;
        ////    //    ArrAmountPromoLine = null;
        ////    //    ArrTotalQtyLine = null;
        ////    //    ArrReg_Price = null;
        ////    //    ArrPromos = null;
        ////    //    ArrStockCodeToUpdate = null;
        ////    //    ArrQtyInPromoLine = null;
        ////    //    ArrLinkNo = null;
        ////    //    tempArrPromo = null;
        ////    //    ArrTotalQuantity = null;
        ////    //    return;
        ////    //}

        ////    //for (i = 1; i <= (ArrPromos.Length - 1); i++)
        ////    //{
        ////    //    ArrStockCodeToUpdate = new string[1];
        ////    //    ArrQtyInPromoLine = new float[1];
        ////    //    ArrTotalQtyLine = new float[1];
        ////    //    ArrAmountPromoLine = new float[1];
        ////    //    ArrTotalQuantity = new float[1];
        ////    //    ArrLinkNo = new float[1];
        ////    //    ArrReg_Price = new float[1]; // reset the array that keeps regular prices, otherwise it contains extra prices  ' July 25, 2008
        ////    //    Chaps_Main.SA.NoOfPromo = 0; // reset the number of promos in the sale when we start to process a new promo
        ////    //    SumRegPrice = 0; // reset the summary for regular prices of the items in this promo

        ////    //    // Handle a promo being loaded after the POS was started
        ////    //    boolExistingPromo = false;
        ////    //    foreach (Promo tempLoopVar_Pr in (IEnumerable)Promos_Renamed)
        ////    //    {
        ////    //        Pr = tempLoopVar_Pr;
        ////    //        if (Pr.PromoID == ArrPromos[i])
        ////    //        {
        ////    //            boolExistingPromo = true;
        ////    //            break;
        ////    //        }
        ////    //    }
        ////    //    if (!boolExistingPromo)
        ////    //    {
        ////    //    }
        ////    //    objPromo = Promos_Renamed.Item(ArrPromos[i]);

        ////    //    Links = new bool[objPromo.MaxLink + 1];
        ////    //    RemainingQtyToProcess = new float[objPromo.MaxLink + 1]; // it has to be by link
        ////    //    ArrQtyInPromoLine = new float[objPromo.MaxLink + 1]; // Apr 28, 2009: changed by link
        ////    //    ArrTotalQtyLine = new float[objPromo.MaxLink + 1]; // Apr 28, 2009: changed by link

        ////    //    // July 06, 2009 start
        ////    //    ArrQtyInPromoLine[(int)tempArrPromo[1, i]] = tempArrPromo[3, i];
        ////    //    ArrTotalQtyLine[(int)tempArrPromo[1, i]] = tempArrPromo[4, i];
        ////    //    if (ArrTotalQtyLine[(int)tempArrPromo[1, i]] >= ArrQtyInPromoLine[(int)tempArrPromo[1, i]])
        ////    //    {
        ////    //        Links[(int)tempArrPromo[1, i]] = true;
        ////    //    }

        ////    //    foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //    {
        ////    //        SL = tempLoopVar_SL;
        ////    //        boolAddItem = false;
        ////    //        foreach (Promo_Line tempLoopVar_PL in objPromo.Promo_Lines)
        ////    //        {
        ////    //            PL = tempLoopVar_PL;
        ////    //            if (PL.Level == ((byte)1))
        ////    //            {
        ////    //                if (SL.Stock_Code == PL.Stock_Code && (SL.PromoID == "" || SL.PromoID == objPromo.PromoID))
        ////    //                {
        ////    //                    boolAddItem = true;
        ////    //                }
        ////    //            }
        ////    //            else if (PL.Level == ((byte)2))
        ////    //            {
        ////    //                if (SL.Dept == PL.Dept && (SL.PromoID == "" || SL.PromoID == objPromo.PromoID))
        ////    //                {
        ////    //                    boolAddItem = true;
        ////    //                }
        ////    //            }
        ////    //            else if (PL.Level == ((byte)3))
        ////    //            {
        ////    //                if (SL.Dept == PL.Dept && SL.Sub_Dept == PL.Sub_Dept && (SL.PromoID == "" || SL.PromoID == objPromo.PromoID))
        ////    //                {
        ////    //                    boolAddItem = true;
        ////    //                }
        ////    //            }
        ////    //            else if (PL.Level == ((byte)4))
        ////    //            {
        ////    //                if (SL.Dept == PL.Dept && SL.Sub_Dept == PL.Sub_Dept && SL.Sub_Detail == PL.Sub_Detail && (SL.PromoID == "" || SL.PromoID == objPromo.PromoID))
        ////    //                {
        ////    //                    boolAddItem = true;
        ////    //                }
        ////    //            }
        ////    //            if (boolAddItem)
        ////    //            {
        ////    //                boolItemExists = false;
        ////    //                for (kk = 1; kk <= (ArrStockCodeToUpdate.Length - 1); kk++)
        ////    //                {
        ////    //                    if (ArrStockCodeToUpdate[kk] == SL.Stock_Code)
        ////    //                    {
        ////    //                        boolItemExists = true;
        ////    //                        break;
        ////    //                    }
        ////    //                }

        ////    //                if (!boolItemExists)
        ////    //                {
        ////    //                    NewIndex = (short)((ArrStockCodeToUpdate.Length - 1) + 1);
        ////    //                    Array.Resize(ref ArrStockCodeToUpdate, NewIndex + 1);
        ////    //                    ArrStockCodeToUpdate[NewIndex] = SL.Stock_Code; //  rsPrice!Stock_Code

        ////    //                    // Regular price to calculate type "P" price promo
        ////    //                    Array.Resize(ref ArrReg_Price, NewIndex + 1);
        ////    //                    ArrReg_Price[NewIndex] = (float)SL.price; // rsPrice!price

        ////    //                    // Link number for the sale line
        ////    //                    Array.Resize(ref ArrLinkNo, NewIndex + 1);
        ////    //                    ArrLinkNo[NewIndex] = PL.Link; // LinkNo ''' rsPromoDetail!Link

        ////    //                    // Quantity required by this line in promo
        ////    //                    ArrQtyInPromoLine[PL.Link] = PL.Quantity; // rsPromoDetail!Qty

        ////    //                    // Amount for this promo line for type "B" promos
        ////    //                    Array.Resize(ref ArrAmountPromoLine, NewIndex + 1);
        ////    //                    ArrAmountPromoLine[NewIndex] = PL.Amount; // IIf(IsNull(rsPromoDetail!Amount), 0, rsPromoDetail!Amount)

        ////    //                    if (PL.Link == tempArrPromo[1, i] && !ChangeQuantity)
        ////    //                    {
        ////    //                        ArrTotalQtyLine[PL.Link] = (float)(mvarQuantity + Get_TotalQuantity(PL.Link, objPromo.PromoID));
        ////    //                    }
        ////    //                    else
        ////    //                    {
        ////    //                        ArrTotalQtyLine[PL.Link] = (float)(Get_TotalQuantity(PL.Link, objPromo.PromoID));
        ////    //                    }
        ////    //                }

        ////    //                if (ArrTotalQtyLine[PL.Link] < ArrQtyInPromoLine[PL.Link])
        ////    //                {
        ////    //                    // the link is not valid because there are not enough items in sale
        ////    //                    // but need to check other links with same number
        ////    //                    if (!Links[PL.Link])
        ////    //                    {
        ////    //                        Links[PL.Link] = false;
        ////    //                    }
        ////    //                }
        ////    //                else
        ////    //                {
        ////    //                    if (ArrTotalQtyLine[PL.Link] == 0 || ArrQtyInPromoLine[PL.Link] == 0)
        ////    //                    {
        ////    //                        if (!Links[PL.Link])
        ////    //                        {
        ////    //                            Links[PL.Link] = false;
        ////    //                        }
        ////    //                    }
        ////    //                    else
        ////    //                    {
        ////    //                        Links[PL.Link] = true;
        ////    //                    }
        ////    //                }

        ////    //            }
        ////    //        }
        ////    //    }
        ////    //    // July 06, 2009 end

        ////    //    // July 10, 2009 add the current item being processed
        ////    //    boolItemExists = false;
        ////    //    for (kk = 1; kk <= (ArrStockCodeToUpdate.Length - 1); kk++)
        ////    //    {
        ////    //        if (ArrStockCodeToUpdate[kk] == mvarStock_Code)
        ////    //        {
        ////    //            boolItemExists = true;
        ////    //            break;
        ////    //        }
        ////    //    }

        ////    //    if (!boolItemExists)
        ////    //    {
        ////    //        NewIndex = (short)((ArrStockCodeToUpdate.Length - 1) + 1);
        ////    //        Array.Resize(ref ArrStockCodeToUpdate, NewIndex + 1);
        ////    //        ArrStockCodeToUpdate[NewIndex] = mvarStock_Code;

        ////    //        // Regular price to calculate type "P" price promo
        ////    //        Array.Resize(ref ArrReg_Price, NewIndex + 1);
        ////    //        ArrReg_Price[NewIndex] = (float)mvarRegular_Price; // price

        ////    //        // Link number for the sale line
        ////    //        Array.Resize(ref ArrLinkNo, NewIndex + 1);
        ////    //        ArrLinkNo[NewIndex] = tempArrPromo[1, i]; // LinkNo

        ////    //        // Amount for this promo line for type "B" promos
        ////    //        Array.Resize(ref ArrAmountPromoLine, NewIndex + 1);
        ////    //        ArrAmountPromoLine[NewIndex] = tempArrPromo[5, i];
        ////    //    }
        ////    //    // July 10, 2009 end add the current item being processed

        ////    //    // Validate all the links
        ////    //    boolValidPromo = true;
        ////    //    for (k = 1; k <= (Links.Length - 1); k++)
        ////    //    {
        ////    //        if (Links[k] == false)
        ////    //        {
        ////    //            boolValidPromo = false;
        ////    //        }
        ////    //    }

        ////    //    if (boolValidPromo)
        ////    //    {
        ////    //        // Calculate TotalQty_ByStock property for each sale line; has to be done here because is not reliable from Adjust_Lines
        ////    //        for (k = 1; k <= (ArrStockCodeToUpdate.Length - 1); k++)
        ////    //        {
        ////    //            TQty_ByStock = 0;
        ////    //            Array.Resize(ref ArrTotalQuantity, (ArrTotalQuantity.Length - 1) + 1 + 1);
        ////    //            // July 07, 2009; for processed stock code total is already calculated in mvarTotalQty_ByStock variable
        ////    //            if (ArrStockCodeToUpdate[k] == mvarStock_Code)
        ////    //            {
        ////    //                ArrTotalQuantity[k] = (float)mvarTotalQty_ByStock;
        ////    //            }
        ////    //            else
        ////    //            {
        ////    //                // July 07, 2009 end
        ////    //                foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //                {
        ////    //                    SL = tempLoopVar_SL;
        ////    //                    if (SL.Stock_Code == ArrStockCodeToUpdate[k] && (SL.PromoID == "" || SL.PromoID == objPromo.PromoID) && SL.Stock_Code != mvarStock_Code)
        ////    //                    {
        ////    //                        TQty_ByStock = TQty_ByStock + SL.Quantity;
        ////    //                    }
        ////    //                }
        ////    //                ArrTotalQuantity[k] = TQty_ByStock;
        ////    //            } // July 07, 2009
        ////    //        }
        ////    //        tempNoOfPromo = 0;

        ////    //        // Apr 21, 2009 calculate the number of promos in the sale going through all the links
        ////    //        // Has to be done at the beginning of processing because for item one the number of
        ////    //        // promo can be 2 but for the second can be 1, so the system tries to assign 2 promos
        ////    //        // to the second item and this is wrong
        ////    //        // start with the item being processed by sale line (mvarStock_Code)

        ////    //        // check all links and keep the lowest number
        ////    //        // calculate number of promos in the sale for this PromoID
        ////    //        for (k = 1; k <= (Links.Length - 1); k++)
        ////    //        {
        ////    //            if (tempNoOfPromo == 0 | tempNoOfPromo > ArrTotalQtyLine[k] / ArrQtyInPromoLine[k])
        ////    //            {
        ////    //                tempNoOfPromo = System.Convert.ToSingle(ArrTotalQtyLine[k] / ArrQtyInPromoLine[k]);
        ////    //            }
        ////    //        }
        ////    //        Chaps_Main.SA.NoOfPromo = tempNoOfPromo;

        ////    //        // mvarStock_Code has been included in ArrStockCodeToUpdate array and is processed there

        ////    //        // Apr 27, 2009: calculate the RemainingQtyToProcess by link number
        ////    //        for (j = 1; j <= (ArrStockCodeToUpdate.Length - 1); j++)
        ////    //        {
        ////    //            if (RemainingQtyToProcess[(int)ArrLinkNo[j]] == 0)
        ////    //            {
        ////    //                RemainingQtyToProcess[(int)ArrLinkNo[j]] = Chaps_Main.SA.NoOfPromo * ArrQtyInPromoLine[(int)ArrLinkNo[j]];
        ////    //            }
        ////    //        }
        ////    //        // Apr 27, 2009 end

        ////    //        Arr_SC_Set = new string[1];
        ////    //        Arr_Qty_Set = new short[1];

        ////    //        // assign the promoID to all stock items that are in this promo
        ////    //        for (j = 1; j <= (ArrStockCodeToUpdate.Length - 1); j++)
        ////    //        {
        ////    //            if (RemainingQtyToProcess[(int)ArrLinkNo[j]] <= 0)
        ////    //            {
        ////    //                foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //                {
        ////    //                    SL = tempLoopVar_SL;
        ////    //                    if (SL.Stock_Code == ArrStockCodeToUpdate[j] && SL.PromoID == ArrPromos[i])
        ////    //                    {
        ////    //                        SL.PromoID = "";
        ////    //                        SL.Price_Type = "R";
        ////    //                        SL.price = ArrReg_Price[j];
        ////    //                        SL.NoPriceFormat = false;
        ////    //                        SL.QtyForPromo = 0;
        ////    //                    }
        ////    //                }
        ////    //                ///                    Exit For  ' ??
        ////    //            }
        ////    //            foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //            {
        ////    //                SL = tempLoopVar_SL;
        ////    //                if (SL.Stock_Code == ArrStockCodeToUpdate[j] && (SL.PromoID == "" || SL.PromoID == ArrPromos[i]))
        ////    //                {
        ////    //                    SL.PromoID = ArrPromos[i];
        ////    //                    SL.QtyInPromoLine = ArrQtyInPromoLine[(int)ArrLinkNo[j]];
        ////    //                    SL.AmountInPromoLine = ArrAmountPromoLine[j];
        ////    //                    SL.Price_Type = "R";

        ////    //                    // July 16, 2009 to keep quantity already set and to set it for all stock codes that are the same in the sale
        ////    //                    boolItemExists = false;
        ////    //                    for (kk = 1; kk <= (Arr_SC_Set.Length - 1); kk++)
        ////    //                    {
        ////    //                        if (Arr_SC_Set[kk] == ArrStockCodeToUpdate[j])
        ////    //                        {
        ////    //                            boolItemExists = true;
        ////    //                            IdxFound = j;
        ////    //                            break;
        ////    //                        }
        ////    //                    }
        ////    //                    if (boolItemExists)
        ////    //                    {
        ////    //                        SL.QtyForPromo = Arr_Qty_Set[IdxFound];
        ////    //                        SL.PromoID = ArrPromos[i]; // Aug 18, 2009
        ////    //                        if (ArrReg_Price[j] != SL.Regular_Price)
        ////    //                        {
        ////    //                            ArrReg_Price[j] = (float)SL.Regular_Price;
        ////    //                        }
        ////    //                        // don't decrease RemainingQtyToProcess or add regular price to SumRegPrice, only mark items in the promo
        ////    //                        //                            SumRegPrice = SumRegPrice + ArrReg_Price(j) * sl.QtyForPromo
        ////    //                        //                            RemainingQtyToProcess(ArrLinkNo(j)) = RemainingQtyToProcess(ArrLinkNo(j)) - sl.QtyForPromo
        ////    //                    }
        ////    //                    else
        ////    //                    {
        ////    //                        SL.QtyForPromo = MinThreeValues(RemainingQtyToProcess[(int)ArrLinkNo[j]], ArrTotalQuantity[j], 999);
        ////    //                        if (ArrReg_Price[j] != SL.Regular_Price)
        ////    //                        {
        ////    //                            ArrReg_Price[j] = (float)SL.Regular_Price;
        ////    //                        }
        ////    //                        SumRegPrice = SumRegPrice + ArrReg_Price[j] * SL.QtyForPromo;
        ////    //                        SL.PromoID = ArrPromos[i]; // Aug 18, 2009
        ////    //                        RemainingQtyToProcess[(int)ArrLinkNo[j]] = RemainingQtyToProcess[(int)ArrLinkNo[j]] - SL.QtyForPromo;

        ////    //                        NewIndex = (short)((Arr_SC_Set.Length - 1) + 1);
        ////    //                        Array.Resize(ref Arr_SC_Set, NewIndex + 1);
        ////    //                        Array.Resize(ref Arr_Qty_Set, NewIndex + 1);

        ////    //                        Arr_SC_Set[NewIndex] = SL.Stock_Code;
        ////    //                        Arr_Qty_Set[NewIndex] = (short)SL.QtyForPromo;
        ////    //                        boolItemExists = true;
        ////    //                    }
        ////    //                    // July 16, 2009 end
        ////    //                    // August 12, 2009 added "If Not ChangeQuantity Then" condition to next line to fix EKO' issue with same item in the second line in the sale and quantity change was not getting the promotion
        ////    //                    // commented on August 18, 2009 have to go through all the sale lines
        ////    //                    
        ////    //                    // go through all items because the same stock item can be in
        ////    //                    // another line if the policy to combine items is set to "No"
        ////    //                }
        ////    //            }
        ////    //            if (mvarStock_Code == ArrStockCodeToUpdate[j] && (string.IsNullOrEmpty(mvarPromoID) || mvarPromoID == ArrPromos[i]) && !ChangeQuantity)
        ////    //            {

        ////    //                if (RemainingQtyToProcess[(int)ArrLinkNo[j]] > 0)
        ////    //                {
        ////    //                    boolItemExists = false;
        ////    //                    for (kk = 1; kk <= (Arr_SC_Set.Length - 1); kk++)
        ////    //                    {
        ////    //                        if (Arr_SC_Set[kk] == ArrStockCodeToUpdate[j])
        ////    //                        {
        ////    //                            boolItemExists = true;
        ////    //                            IdxFound = j;
        ////    //                            break;
        ////    //                        }
        ////    //                    }
        ////    //                    if (boolItemExists)
        ////    //                    {
        ////    //                        mvarQtyForPromo = Arr_Qty_Set[IdxFound];
        ////    //                        mvarPromoID = ArrPromos[i]; // Aug 18, 2009
        ////    //                        mvarQtyInPromoLine = ArrQtyInPromoLine[(int)ArrLinkNo[j]]; // Aug 18, 2009
        ////    //                        mvarAmountInPromoLine = ArrAmountPromoLine[j]; // Oct 14, 2009
        ////    //                        if (ArrReg_Price[j] != mvarRegular_Price)
        ////    //                        {
        ////    //                            ArrReg_Price[j] = (float)mvarRegular_Price;
        ////    //                        }
        ////    //                        mvarRefreshPrice = true;
        ////    //                    }
        ////    //                    else
        ////    //                    {
        ////    //                        mvarPromoID = ArrPromos[i];
        ////    //                        mvarPrice_Type = "R";
        ////    //                        mvarQtyInPromoLine = ArrQtyInPromoLine[(int)ArrLinkNo[j]];
        ////    //                        mvarQtyForPromo = MinThreeValues(RemainingQtyToProcess[(int)ArrLinkNo[j]], ArrTotalQuantity[j], 999);
        ////    //                        mvarAmountInPromoLine = ArrAmountPromoLine[j];
        ////    //                        if (ArrReg_Price[j] != mvarRegular_Price)
        ////    //                        {
        ////    //                            ArrReg_Price[j] = (float)mvarRegular_Price;
        ////    //                        }
        ////    //                        SumRegPrice = SumRegPrice + ArrReg_Price[j] * mvarQtyForPromo;
        ////    //                        RemainingQtyToProcess[(int)ArrLinkNo[j]] = RemainingQtyToProcess[(int)ArrLinkNo[j]] - mvarQtyForPromo;
        ////    //                        // go through all items because the same stock item can be in
        ////    //                        // another line if the policy to combine items is set to "No"
        ////    //                    }
        ////    //                }
        ////    //                else
        ////    //                {
        ////    //                    // Aug 18, 2009 to search in the Arr_SC_Set for item being processed
        ////    //                    boolItemExists = false;
        ////    //                    for (kk = 1; kk <= (Arr_SC_Set.Length - 1); kk++)
        ////    //                    {
        ////    //                        if (Arr_SC_Set[kk] == ArrStockCodeToUpdate[j])
        ////    //                        {
        ////    //                            boolItemExists = true;
        ////    //                            IdxFound = j;
        ////    //                            break;
        ////    //                        }
        ////    //                    }
        ////    //                    if (boolItemExists)
        ////    //                    {
        ////    //                        mvarQtyForPromo = Arr_Qty_Set[IdxFound];
        ////    //                        mvarPromoID = ArrPromos[i];
        ////    //                        mvarQtyInPromoLine = ArrQtyInPromoLine[(int)ArrLinkNo[j]];
        ////    //                        mvarAmountInPromoLine = ArrAmountPromoLine[j];
        ////    //                        if (ArrReg_Price[j] != mvarRegular_Price)
        ////    //                        {
        ////    //                            ArrReg_Price[j] = (float)mvarRegular_Price;
        ////    //                        }
        ////    //                    }
        ////    //                    else
        ////    //                    {
        ////    //                        // Aug 18, 2009 end
        ////    //                        if (!string.IsNullOrEmpty(mvarPromoID))
        ////    //                        {
        ////    //                            mvarPromoID = "";
        ////    //                            mvarPrice_Type = "R";
        ////    //                            mvarPrice = ArrReg_Price[j];
        ////    //                            mvarNoPriceFormat = false;
        ////    //                        }
        ////    //                    }
        ////    //                    ///                        Exit For  ' ??
        ////    //                    mvarRefreshPrice = true;
        ////    //                }
        ////    //            }
        ////    //        }

        ////    //        // July 22, 2009 this should not happen, but it will prevent crash in Adjust_Lines procedure
        ////    //        foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //        {
        ////    //            SL = tempLoopVar_SL;
        ////    //            if (SL.QtyForPromo == 0 && SL.PromoID == objPromo.PromoID)
        ////    //            {
        ////    //                SL.PromoID = "";
        ////    //                SL.Price_Type = "R";
        ////    //                SL.NoPriceFormat = false;
        ////    //                SL.price = SL.Regular_Price;
        ////    //            }
        ////    //        }
        ////    //        if (mvarQtyForPromo == 0 && mvarPromoID == objPromo.PromoID)
        ////    //        {
        ////    //            mvarPromoID = "";
        ////    //            mvarPrice_Type = "R";
        ////    //            mvarNoPriceFormat = false;
        ////    //            mvarPrice = mvarRegular_Price;
        ////    //        }
        ////    //        // July 22, 2009 end

        ////    //        // calculate the price for other stock items that are in this promo
        ////    //        foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //        {
        ////    //            SL = tempLoopVar_SL;
        ////    //            // Added the second condition to handle one link promos when sale lines are not combined
        ////    //            if (SL.PromoID == objPromo.PromoID)
        ////    //            {
        ////    //                switch (objPromo.DiscType)
        ////    //                {
        ////    //                    case "$":
        ////    //                        SL.Promo_Price = double.Parse((SL.Regular_Price - objPromo.Amount).ToString("#,##0.00"));
        ////    //                        break;
        ////    //                    case "%":
        ////    //                        SL.Promo_Price = double.Parse((SL.Regular_Price * (1 - (objPromo.Amount / 100))).ToString("#,##0.00"));
        ////    //                        break;
        ////    //                    case "P":
        ////    //                        if (!(objPromo.MaxLink == 1 && objPromo.MultiLink))
        ////    //                        {
        ////    //                            SL.Promo_Price = (SL.Regular_Price * SL.QtyInPromoLine / SumRegPrice) * objPromo.Amount / SL.QtyInPromoLine;
        ////    //                            SL.Promo_Price = SL.Promo_Price * Chaps_Main.SA.NoOfPromo;
        ////    //                        }
        ////    //                        else
        ////    //                        {
        ////    //                            SL.Promo_Price = objPromo.Amount / SL.QtyInPromoLine; // mvarQtyInPromoLine  ''' (SL.Regular_Price * SL.QtyForPromo / SumRegPrice) * objPromo.Amount / SL.QtyForPromo * SA.NoOfPromo
        ////    //                        }
        ////    //                        break;
        ////    //                    case "B":
        ////    //                        if (objPromo.PrType == "%") // this should be only for new promo set as % type
        ////    //                        {
        ////    //                            SL.Promo_Price = double.Parse((SL.Regular_Price * (100 - SL.AmountInPromoLine) / 100).ToString("#,##0.00"));
        ////    //                        }
        ////    //                        else // else handles dollar and NULL PrType records
        ////    //                        {
        ////    //                            SL.Promo_Price = SL.Regular_Price - SL.AmountInPromoLine; // took out the format on September 24, 2008
        ////    //                        }
        ////    //                        break;
        ////    //                }

        ////    //                if (!SL.ProductIsFuel)
        ////    //                {
        ////    //                    SP = new SP_Prices();
        ////    //                    //                        If objPromo.DiscType = "P" And objPromo.MaxLink = 1 Then

        ////    //                    string temp_Policy_Name = "PRICE_DEC";
        ////    //                    string temp_Policy_Name2 = "PRICE_DEC";
        ////    //                    fs = System.Convert.ToString(((int)modPolicy.GetPol(temp_Policy_Name, SL) == 0) ? "0" : ("0." + new string('0', System.Convert.ToInt32(modPolicy.GetPol(temp_Policy_Name2, SL)))));
        ////    //                    if ((!(SL.QtyForPromo * SL.QtyInPromoLine * double.Parse(SL.Promo_Price.ToString(fs)) == objPromo.Amount)) && (Conversion.Val((SL.Promo_Price).ToString()) != Conversion.Val(SL.Promo_Price.ToString(fs))))
        ////    //                    {
        ////    //                        SL.NoPriceFormat = true;
        ////    //                    }
        ////    //                    //                        End If
        ////    //                    SP.Add(1, SL.QtyForPromo, (float)(SL.Promo_Price), DateAndTime.Today, DateAndTime.DateAdd(Microsoft.VisualBasic.DateInterval.Year, 1, DateAndTime.Today), "");
        ////    //                    SL.SP_Prices = SP;
        ////    //                }
        ////    //            }
        ////    //            if (SL.Stock_Code == mvarStock_Code)
        ////    //            {
        ////    //                SL.RefreshPrice = true; // Oct 14, 2009
        ////    //            }
        ////    //            if (SL.HotButton && objPromo.MaxLink == 1 && objPromo.MultiLink)
        ////    //            {
        ////    //                SL.NoPromo = true; // added on September 12, 2008 to avoid promo being remade from HotButtons screen
        ////    //            }
        ////    //        }
        ////    //        // moved here July 10, 2009
        ////    //        // calculate the price for mvarStock_Code that is in promo
        ////    //        if (mvarPromoID == objPromo.PromoID && mvarQtyForPromo > 0)
        ////    //        {
        ////    //            switch (objPromo.DiscType)
        ////    //            {
        ////    //                case "$":
        ////    //                    mvarPromo_Price = double.Parse((mvarRegular_Price - objPromo.Amount).ToString("#,##0.00"));
        ////    //                    break;
        ////    //                case "%":
        ////    //                    mvarPromo_Price = double.Parse((mvarRegular_Price * (1 - (objPromo.Amount / 100))).ToString("#,##0.00"));
        ////    //                    break;
        ////    //                case "P":
        ////    //                    if (!(objPromo.MaxLink == 1 && objPromo.MultiLink))
        ////    //                    {
        ////    //                        mvarPromo_Price = (mvarRegular_Price * mvarQtyInPromoLine / SumRegPrice) * objPromo.Amount / mvarQtyInPromoLine;
        ////    //                        mvarPromo_Price = mvarPromo_Price * Chaps_Main.SA.NoOfPromo;
        ////    //                    }
        ////    //                    else
        ////    //                    {
        ////    //                        mvarPromo_Price = objPromo.Amount / mvarQtyInPromoLine; 
        ////    //                    }
        ////    //                    break;
        ////    //                case "B":
        ////    //                    if (objPromo.PrType == "%") // this should be only for new promo set as % type
        ////    //                    {
        ////    //                        mvarPromo_Price = double.Parse((mvarRegular_Price * (100 - mvarAmountInPromoLine) / 100).ToString("#,##0.00"));
        ////    //                    }
        ////    //                    else // else handles dollar and NULL PrType records
        ////    //                    {
        ////    //                        mvarPromo_Price = double.Parse((mvarRegular_Price - mvarAmountInPromoLine).ToString("#,##0.00"));
        ////    //                    }
        ////    //                    break;
        ////    //            }

        ////    //            string temp_Policy_Name3 = "PRICE_DEC";
        ////    //            string temp_Policy_Name4 = "PRICE_DEC";
        ////    //            fs = System.Convert.ToString(((int)modPolicy.GetPol(temp_Policy_Name3, this) == 0) ? "0" : ("0." + new string('0', System.Convert.ToInt32(modPolicy.GetPol(temp_Policy_Name4, this)))));
        ////    //            if ((!(mvarQtyForPromo * mvarQtyInPromoLine * double.Parse(mvarPromo_Price.ToString(fs)) == objPromo.Amount)) && Conversion.Val((mvarPromo_Price).ToString()) != Conversion.Val(mvarPromo_Price.ToString(fs)))
        ////    //            {
        ////    //                mvarNoPriceFormat = true;
        ////    //            }
        ////    //            mvarRefreshPrice = true; // Oct 14, 2009
        ////    //        }
        ////    //        // moved here July 10, 2009
        ////    //        break;
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        // it is an invalid promo
        ////    //        //  
        ////    //        if (this.PromoID != "")
        ////    //        {
        ////    //            // reset the promoID and price for all other items belonging to same promo
        ////    //            foreach (Sale_Line tempLoopVar_SL in Chaps_Main.SA.Sale_Lines)
        ////    //            {
        ////    //                SL = tempLoopVar_SL;
        ////    //                if (SL.PromoID != "" && SL.PromoID == objPromo.PromoID)
        ////    //                {
        ////    //                    SL.PromoID = "";
        ////    //                    SL.price = SL.Regular_Price;
        ////    //                    SL.NoPriceFormat = false;
        ////    //                }
        ////    //            }
        ////    //            this.PromoID = "";
        ////    //            this.price = this.Regular_Price;
        ////    //            this.NoPriceFormat = false;
        ////    //        }
        ////    //        objPromo = null;
        ////    //    }
        ////    //}

        ////    //rsPrice = null;
        ////    //rsPromo = null;
        ////    //rsPromoDetail = null;
        ////    //rsCurSale = null;
        ////    //SL = null;
        ////    //SP = null;
        ////    //objPromo = null;
        ////    //Pr = null;
        ////    //ArrAmountPromoLine = null;
        ////    //ArrTotalQtyLine = null;
        ////    //ArrReg_Price = null;
        ////    //ArrPromos = null;
        ////    //ArrStockCodeToUpdate = null;
        ////    //ArrQtyInPromoLine = null;
        ////    //ArrLinkNo = null;
        ////    //tempArrPromo = null;
        ////    //ArrTotalQuantity = null;
        ////    //Arr_Qty_Set = null;
        ////    //Arr_SC_Set = null;

        ////}
        ////   end
    }
}
