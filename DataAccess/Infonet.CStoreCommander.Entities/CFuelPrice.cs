using System;

namespace Infonet.CStoreCommander.Entities
{
    public class CFuelPrice
    {

        private float myCashPrice;
        private float myCreditPrice;
        private float myTECashPrice;
        private float myTECreditPrice;
        private object myEmplID;
        private int myReportID;
        private DateTime myDateTime;


        public float CashPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = myCashPrice;
                return returnValue;
            }
            set
            {
                myCashPrice = value;
            }
        }


        public float CreditPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = myCreditPrice;
                return returnValue;
            }
            set
            {
                myCreditPrice = value;
            }
        }


        public dynamic EmplID
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myEmplID;
                return returnValue;
            }
            set
            {
                myEmplID = value;
            }
        }


        public int ReportID
        {
            get
            {
                int returnValue = 0;
                returnValue = myReportID;
                return returnValue;
            }
            set
            {
                myReportID = value;
            }
        }

        public int Date_Time
        {
            get
            {
                int returnValue = 0;
                returnValue = (int)(myDateTime.ToOADate());
                return returnValue;
            }
            set
            {
                myDateTime = Convert.ToDateTime(DateTime.FromOADate(value));
            }
        }


        public float teCashPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = myTECashPrice;
                return returnValue;
            }
            set
            {
                myTECashPrice = value;
            }
        }


        public float teCreditPrice
        {
            get
            {
                float returnValue = 0;
                returnValue = myTECreditPrice;
                return returnValue;
            }
            set
            {
                myTECreditPrice = value;
            }
        }
    }
}
