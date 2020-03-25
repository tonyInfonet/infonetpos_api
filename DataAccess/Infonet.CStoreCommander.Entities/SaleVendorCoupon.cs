using System;

namespace Infonet.CStoreCommander.Entities
{
    public class SaleVendorCoupon: IDisposable
    {

        private int mvarSale_Num;
        private decimal mvarAmount;
        private SaleVendorCouponLines mvarSVC_Lines;
        private byte mvarTill_Num;



        public SaleVendorCouponLines SVC_Lines
        {
            get
            {
                SaleVendorCouponLines returnValue = default(SaleVendorCouponLines);
                if (mvarSVC_Lines == null)
                {
                    mvarSVC_Lines = new SaleVendorCouponLines();
                }
                returnValue = mvarSVC_Lines;
                return returnValue;
            }
            set
            {
                mvarSVC_Lines = value;
            }
        }


        public decimal Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
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


        public byte Till_Num
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarTill_Num;
                return returnValue;
            }
            set
            {
                mvarTill_Num = value;
            }
        }

   
        private void Class_Initialize_Renamed()
        {
            mvarSVC_Lines = new SaleVendorCouponLines();
        }
        public SaleVendorCoupon()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mvarSVC_Lines = null;
        }

        public void Dispose()
        {
            //base.Finalize();
        }
        ~SaleVendorCoupon()
        {
            Class_Terminate_Renamed();
        }

       
    }
}
