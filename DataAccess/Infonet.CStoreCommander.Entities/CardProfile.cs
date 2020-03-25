// VBConversions Note: VB project level imports
//using AxccrpMonthcal6;
//using AxCCRPDTP6;

using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class CardProfile : IDisposable
    {
        private string mvarProfileID;
        private string mvarProfileName;
        private double mvarSngleTransaction;
        private double mvarDailyTransaction;
        private double mvarMonthlyTransaction;
        private int mvarTransactionsPerDay;
        private bool mvarAskForPO;
        private bool mvarPromptForFuel;
        private bool mvarLimitTimeofPurchase;
        private double mvarPurchaseAmount;
        private bool mvarRestrictProducts;
        private string mvarReason;
        private string mvarPONumber;
        private bool mvarPartialUse;
        private bool mvarRestrictedUse; //  - to show restricted message & option to cancel
        private CardPrompts mvarCardPrompts;


        public string ProfileID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarProfileID;
                return returnValue;
            }
            set
            {
                mvarProfileID = value;
                //Loadprofile(mvarProfileID);
            }
        }


        public string ProfileName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarProfileName;
                return returnValue;
            }
            set
            {
                mvarProfileName = value;
            }
        }


        public double SngleTransaction
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarSngleTransaction;
                return returnValue;
            }
            set
            {
                mvarSngleTransaction = value;
            }
        }


        public double DailyTransaction
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarDailyTransaction;
                return returnValue;
            }
            set
            {
                mvarDailyTransaction = value;
            }
        }

        public double MonthlyTransaction
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarMonthlyTransaction;
                return returnValue;
            }
            set
            {
                mvarMonthlyTransaction = value;
            }
        }

        public int TransactionsPerDay
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarTransactionsPerDay;
                return returnValue;
            }
            set
            {
                mvarTransactionsPerDay = value;
            }
        }

        public bool AskForPO
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAskForPO;
                return returnValue;
            }
            set
            {
                mvarAskForPO = value;
            }
        }
        public bool PromptForFuel
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPromptForFuel;
                return returnValue;
            }
            set
            {
                mvarPromptForFuel = value;
            }
        }
        public bool LimitTimeofPurchase
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLimitTimeofPurchase;
                return returnValue;
            }
            set
            {
                mvarLimitTimeofPurchase = value;
            }
        }

        public double PurchaseAmount
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarPurchaseAmount;
                return returnValue;
            }
            set
            {
                mvarPurchaseAmount = value;
            }
        }
        public bool RestrictProducts
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarRestrictProducts;
                return returnValue;
            }
            set
            {
                mvarRestrictProducts = value;
            }
        }

        public string Reason
        {
            get
            {
                string returnValue = "";

                returnValue = mvarReason;
                return returnValue;
            }
            set
            {
                mvarReason = value;
            }
        }

        public string PONumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPONumber;
                return returnValue;
            }
            set
            {
                mvarPONumber = value;
            }
        }

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
        public bool PartialUse
        {
            get //   allow partial use if with this purchase it will be overlimit
            {
                bool returnValue = false;
                returnValue = mvarPartialUse;
                return returnValue;
            }
            set
            {
                mvarPartialUse = value;
            }
        }
        public bool RestrictedUse
        {
            get //   give option to cancel the transaction after restricted message
            {
                bool returnValue = false;
                returnValue = mvarRestrictedUse;
                return returnValue;
            }
            set
            {
                mvarRestrictedUse = value;
            }
        }

        private void Class_Initialize()
        {
            mvarSngleTransaction = 0;
            mvarDailyTransaction = 0;
            mvarMonthlyTransaction = 0;
            mvarTransactionsPerDay = (byte)0;
            mvarPurchaseAmount = 0; // how much we can charge on the card
            mvarAskForPO = false;
            mvarPromptForFuel = false;
            mvarLimitTimeofPurchase = false;
            mvarRestrictProducts = false;
            mvarReason = "";
        }
        public CardProfile()
        {
            Class_Initialize();
         
        }

        private void Class_Terminate_Renamed()
        {
            mvarCardPrompts = null;
        }

        public void Dispose()
        {
           
        }
        ~CardProfile()
        {
            Class_Terminate_Renamed();
        }

    }
}
