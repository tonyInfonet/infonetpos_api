using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class TenderCard: IDisposable
    {

        private string mvarTenderCode;
        private short mvarReportGroup;
        private string mvarBankCardID;
        private bool mvarCallTheBank;
        private bool mvarSignatureLine;
        private float mvarDiscountRate;
        private string mvarDiscountType;
        private bool mvarRefundAllowed;
        private float mvarPurchaseLimit;
        private float mvarFloorLimit;
        private short mvarPrintCopies;
        private string mvarReceiptTotalText;
        private bool mvarLimitToSale;
        private float mvarMaxCashBack;
        private string mvarCardCode;
        private bool mvarAllowPayPump; // 
        private bool mvarCardProductRestrict; //  - To determine whether need to use CardProduct Link to fill p5/P6 format
                                              //shiny added the following on april4.2002
        private bool mvarPrintVechicle;
        private bool mvarPrintDriver;
        private bool mvarPrintIdentification;
        private bool mvarPrintOdometer;
        private bool mvarPrintUsage;
        //shiny end
        private decimal mvarCashBackAmount;
        private string mvarOptDataProfileID; //  
        private CardPrompts mvarCardPrompts; //  

        public string CardCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardCode;
                return returnValue;
            }
            set
            {
                mvarCardCode = value;
            }
        }


        public float MaxCashBack
        {
            get
            {
                return mvarMaxCashBack;
            }
            set
            {
                mvarMaxCashBack = value;
            }
        }


        public bool LimitToSale
        {
            get
            {
                return mvarLimitToSale;
            }
            set
            {
                mvarLimitToSale = value;
            }
        }


        public string ReceiptTotalText
        {
            get
            {
                return mvarReceiptTotalText;
            }
            set
            {
                mvarReceiptTotalText = value;
            }
        }


        public short PrintCopies
        {
            get
            {
                return mvarPrintCopies;
            }
            set
            {
                mvarPrintCopies = value;
            }
        }


        public float FloorLimit
        {
            get
            {
                return mvarFloorLimit;
            }
            set
            {
                mvarFloorLimit = value;
            }
        }


        public float PurchaseLimit
        {
            get
            {
                return mvarPurchaseLimit;
            }
            set
            {
                mvarPurchaseLimit = value;
            }
        }


        public bool RefundAllowed
        {
            get
            {
                return mvarRefundAllowed;
            }
            set
            {
                mvarRefundAllowed = value;
            }
        }


        public string DiscountType
        {
            get
            {
                return mvarDiscountType;
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
                return mvarDiscountRate;
            }
            set
            {
                mvarDiscountRate = value;
            }
        }


        public bool SignatureLine
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarSignatureLine;
                return returnValue;
            }
            set
            {
                mvarSignatureLine = value;
            }
        }


        public bool CallTheBank
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCallTheBank;
                return returnValue;
            }
            set
            {
                mvarCallTheBank = value;
            }
        }


        public string BankCardID
        {
            get
            {
                return mvarBankCardID;
            }
            set
            {
                mvarBankCardID = value;
            }
        }


        public short ReportGroup
        {
            get
            {
                return mvarReportGroup;
            }
            set
            {
                mvarReportGroup = value;
            }
        }


        public string TenderCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTenderCode;
                return returnValue;
            }
            set
            {
                mvarTenderCode = value;
            }
        }

        // 

        public bool AllowPayPump
        {
            get
            {
                return mvarAllowPayPump;
            }
            set
            {
                mvarAllowPayPump = value;
            }
        }

        //  for ADS

        public bool CardProductRestrict
        {
            get
            {
                return mvarCardProductRestrict;
            }
            set
            {
                mvarCardProductRestrict = value;
            }
        }
        //shiny end
        //  To decide whether we need to print vechileno/driver no/identification no/ odometer/ usage in the receipt


        public bool PrintVechicle
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintVechicle;
                return returnValue;
            }
            set
            {
                mvarPrintVechicle = value;
            }
        }


        public bool PrintDriver
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintDriver;
                return returnValue;
            }
            set
            {
                mvarPrintDriver = value;
            }
        }


        public bool PrintIdentification
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrintIdentification;
                return returnValue;
            }
            set
            {
                mvarPrintIdentification = value;
            }
        }


        public bool PrintOdometer
        {
            get
            {
                return mvarPrintOdometer;
            }
            set
            {
                mvarPrintOdometer = value;
            }
        }


        public bool PrintUsage
        {
            get
            {
                return mvarPrintUsage;
            }
            set
            {
                mvarPrintUsage = value;
            }
        }
        //shiny end


        public decimal CashBackAmount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarCashBackAmount;
                return returnValue;
            }
            set
            {
                mvarCashBackAmount = value;
            }
        }

        //  

        public string OptDataProfileID
        {
            get
            {
                return mvarOptDataProfileID;
            }
            set
            {
                mvarOptDataProfileID = value;
                if (value.Length == 0)
                {
                    return;
                }
                mvarCardPrompts = new CardPrompts();

                //CardPrompts.Load_Prompts(mvarOptDataProfileID);
            }
        }
        //   end


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

        private void Class_Terminate_Renamed()
        {
            mvarCardPrompts = null;
        }

        public void Dispose()
        {
            
            //base.Finalize();
        }

        ~TenderCard()
        {
            Class_Terminate_Renamed();
        }
    }
}
