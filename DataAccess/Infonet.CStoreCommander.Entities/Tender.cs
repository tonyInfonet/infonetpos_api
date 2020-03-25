
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Tender: IDisposable
    {
        private string mvarTender_Display;
        private string mvarTender_Name;
        private string mvarTender_Class;
        private string mvarBankCardID;
        private double mvarExchange_Rate;
        private bool mvarCallTheBank;
        private bool mvarGive_Change;
        private bool mvarGive_As_Refund;
        private bool mvarSystem_Can_Adjust;
        private bool mvarSignatureLine;
        private bool mvarRefundAllowed;
        private bool mvarLimitToSale;
        private short mvarSequence_Number;
        private short mvarPrintCopies;
        private string mvarTender_Code;
        private string mvarReceiptTotalText;
        private bool mvarExact_Change;
        private double mvarMaxAmount;
        private double mvarMinAmount;
        private double mvarSmallest_Unit;
        private decimal mvarAmountEntered;
        private decimal mvarAmount_Used;
        private decimal mvarPurchaseLimit;
        private decimal mvarFloorLimit;
        private decimal mvarMaxCashBack;
        private bool mvarOpen_Drawer;
        private bool mvarAcceptAsPayment;
        private object mvarTendersArray;
        private Credit_Card mvarCreditCard;
        private string mvarPCATSGroup;

        
        //Private mvarShort_Name           As String

        
        private string mvarAuthUser;
        private bool mvarInactive; // Nov 02, 2009 Nicolette



        public string TendDescription { get; set; }
        public string TendClassDescription { get; set; }
        public string Image { get; set; }

        public string AuthUser
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAuthUser;
                return returnValue;
            }
            set
            {
                mvarAuthUser = value;
            }
        }
        



        public dynamic Tenders_Array
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = mvarTendersArray;
                return returnValue;
            }
            set
            {
                mvarTendersArray = value;
            }
        }



        public bool LimitToSale
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLimitToSale;
                return returnValue;
            }
            set
            {
                mvarLimitToSale = value;
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




        public bool RefundAllowed
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarRefundAllowed;
                return returnValue;
            }
            set
            {
                mvarRefundAllowed = value;
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


        public bool Open_Drawer
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarOpen_Drawer;
                return returnValue;
            }
            set
            {
                mvarOpen_Drawer = value;
            }
        }


        public double Smallest_Unit
        {
            get
            {
                double returnValue = 0;
                if (mvarSmallest_Unit == 0)
                {
                    returnValue = 0.01;
                }
                else
                {
                    returnValue = mvarSmallest_Unit;
                }
                return returnValue;
            }
            set
            {
                mvarSmallest_Unit = value;
            }
        }


        public double MinAmount
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarMinAmount;
                return returnValue;
            }
            set
            {
                mvarMinAmount = value;
            }
        }


        public double MaxAmount
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarMaxAmount;
                return returnValue;
            }
            set
            {
                mvarMaxAmount = value;
            }
        }


        public bool Exact_Change
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarExact_Change;
                return returnValue;
            }
            set
            {
                mvarExact_Change = value;
            }
        }



        public string ReceiptTotalText
        {
            get
            {
                string returnValue = "";
                returnValue = mvarReceiptTotalText;
                return returnValue;
            }
            set
            {
                mvarReceiptTotalText = value;
            }
        }



        public string BankCardID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarBankCardID;
                return returnValue;
            }
            set
            {
                mvarBankCardID = value;
            }
        }



        public string Tender_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTender_Code;
                return returnValue;
            }
            set
            {
                mvarTender_Code = value;
            }
        }


        public short Sequence_Number
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarSequence_Number;
                return returnValue;
            }
            set
            {
                mvarSequence_Number = value;
            }
        }


        public short PrintCopies
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarPrintCopies;
                return returnValue;
            }
            set
            {
                mvarPrintCopies = value;
            }
        }


        public bool System_Can_Adjust
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarSystem_Can_Adjust;
                return returnValue;
            }
            set
            {
                mvarSystem_Can_Adjust = value;
            }
        }


        public bool Give_As_Refund
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarGive_As_Refund;
                return returnValue;
            }
            set
            {
                mvarGive_As_Refund = value;
            }
        }


        public bool Give_Change
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarGive_Change;
                return returnValue;
            }
            set
            {
                mvarGive_Change = value;
            }
        }


        public double Exchange_Rate
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarExchange_Rate;
                return returnValue;
            }
            set
            {
                mvarExchange_Rate = value;
            }
        }


        public string Tender_Class
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTender_Class;
                return returnValue;
            }
            set
            {
                mvarTender_Class = value;
            }
        }


        public string Tender_Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTender_Name;
                return returnValue;
            }
            set
            {
                mvarTender_Name = value;
            }
        }


        public decimal Amount_Entered
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAmountEntered;
                return returnValue;
            }
            set
            {
                mvarAmountEntered = value;
                // and add the new one.
                if (!this.System_Can_Adjust)
                {
                    this.Amount_Used = mvarAmountEntered * (decimal)mvarExchange_Rate;
                }
            }
        }


        public decimal MaxCashBack
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarMaxCashBack;
                return returnValue;
            }
            set
            {
                mvarMaxCashBack = value;
            }
        }


        public decimal PurchaseLimit
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPurchaseLimit;
                return returnValue;
            }
            set
            {
                mvarPurchaseLimit = value;
            }
        }


        public decimal FloorLimit
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarFloorLimit;
                return returnValue;
            }
            set
            {
                mvarFloorLimit = value;
            }
        }


        public decimal Amount_Used
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAmount_Used;
                return returnValue;
            }
            set
            {
                mvarAmount_Used = value;
            }
        }

        //   end

        public Credit_Card Credit_Card
        {
            get
            {
                Credit_Card returnValue = default(Credit_Card);
                if (mvarCreditCard == null)
                {
                    mvarCreditCard = new Credit_Card();
                }
                returnValue = mvarCreditCard;
                return returnValue;
            }
            set
            {
                mvarCreditCard = value;
            }
        }
        // 

        public bool AcceptAspayment
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAcceptAsPayment;
                return returnValue;
            }
            set
            {
                mvarAcceptAsPayment = value;
            }
        }
        //Shiny end

        // Nov 02, 2009 Nicolette

        public bool Inactive
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarInactive;
                return returnValue;
            }
            set
            {
                mvarInactive = value;
            }
        }
        // Nov 02, 2009 Nicolette end

        //  

        public string PCATSGroup
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPCATSGroup;
                return returnValue;
            }
            set
            {
                mvarPCATSGroup = value;
            }
        }

        private void Class_Initialize_Renamed()
        {
            mvarCreditCard = new Credit_Card();
            mvarAuthUser = ""; 
            mvarInactive = false;
        }
        public Tender()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mvarCreditCard = null;
        }

        public void Dispose()
        {
          
        }
        ~Tender()
        {
            Class_Terminate_Renamed();
        }
    }
}
