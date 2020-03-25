
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Payout : IDisposable
    {

        private decimal mvarGross;
        private Payout_Taxes mvarPayout_Taxes;
        private Return_Reason mvarReturnReason; //Nicolette
        private decimal mvarNet;
        private Vendor mvarVendor; // Nicolette
        private int mvarSale_Num;
        private decimal mvarPenny_Adj; //  



        public decimal Net
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarNet;
                return returnValue;
            }
            set
            {
                mvarNet = value;
            }
        }



        public Payout_Taxes Payout_Taxes
        {
            get
            {
                Payout_Taxes returnValue = default(Payout_Taxes);
                if (mvarPayout_Taxes == null)
                {
                    mvarPayout_Taxes = new Payout_Taxes();
                }
                returnValue = mvarPayout_Taxes;
                return returnValue;
            }
            set
            {
                mvarPayout_Taxes = value;
            }
        }


        public decimal Gross
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarGross;
                return returnValue;
            }
            set
            {
                mvarGross = value;
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


        public Vendor Vendor
        {
            get
            {
                Vendor returnValue = default(Vendor);
                if (mvarVendor == null)
                {
                    mvarVendor = new Vendor();
                }
                returnValue = mvarVendor;
                return returnValue;
            }
            set
            {
                mvarVendor = value;
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


        public decimal Penny_Adj
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPenny_Adj;
                return returnValue;
            }
            set
            {
                mvarPenny_Adj = value;
            }
        }


        private void Class_Initialize_Renamed()
        {
            mvarReturnReason = new Return_Reason();
            mvarPenny_Adj = 0;
        }
        public Payout()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {

            mvarPayout_Taxes = null;

            mvarReturnReason = null;
        }

        public void Dispose()
        {
            
            //base.Finalize();
        }

        ~Payout()
        {
            Class_Terminate_Renamed();
        }
    }
}
