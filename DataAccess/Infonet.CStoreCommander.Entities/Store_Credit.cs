using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Store_Credit
    {
        private int mvarNumber;
        private decimal mvarAmount;
        private DateTime mvarSC_Date;
        private int mvarSale_Number;
        private string mvarCustomer;
        private DateTime _expiresOn;

        public DateTime Expires_On
        {
            get
            {
                //DateTime returnValue = default(DateTime);
                //if (this.EXPIRE_DAYS == 0)
                //{
                //    returnValue = DateAndTime.DateAdd(Microsoft.VisualBasic.DateInterval.Day, 99999, DateAndTime.Today);
                //}
                //else
                //{
                //    returnValue = DateAndTime.DateAdd(Microsoft.VisualBasic.DateInterval.Day, this.EXPIRE_DAYS, DateAndTime.Today);
                //}

                //return returnValue;

                return _expiresOn;
            }
            set
            {
                _expiresOn = value;
            }
        }

        public int EXPIRE_DAYS
        {
            get; set;
        }

        public string Customer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCustomer;
                return returnValue;
            }
            set
            {
                mvarCustomer = value;
            }
        }

        public int Sale_Number
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarSale_Number;
                return returnValue;
            }
            set
            {
                mvarSale_Number = value;
            }
        }

        public DateTime SC_Date
        {
            get
            {
                return mvarSC_Date;
            }
            set
            {
                mvarSC_Date = value;
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

        public int Number
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarNumber;
                return returnValue;
            }
            set
            {
                mvarNumber = value;
            }
        }
    }
}
