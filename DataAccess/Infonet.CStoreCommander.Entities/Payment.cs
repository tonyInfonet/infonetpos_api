using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Payment
    {

        private Customer mvarCustomer;
        private Credit_Card mvarCard;
        private string mvarAccount;
        private decimal mvarAmount;
        private string mvarTotalLabel;
        private string mvarSummaryLabel;
        private string mvarTopLeft;
        private string mvarTopRight;
        private decimal mvarPenny_Adj; //  


        public string TotalLabel
        {
            get
            {
                string returnValue = default(string);
                returnValue = mvarTotalLabel;
                return returnValue;
            }
            set
            {
                mvarTotalLabel = value;
                mvarTotalLabel = "";
            }
        }


        public string TopLeft
        {
            get
            {
                string returnValue = default(string);
                returnValue = mvarTopLeft;
                return returnValue;
            }
            set
            {
                mvarTopLeft = value;
                //if (!(this.TopRight == null))
                //{
                //    Top_Box();
                //}
            }
        }


        public string TopRight
        {
            get
            {
                string returnValue = default(string);
                returnValue = mvarTopRight;
                return returnValue;
            }
            set
            {
                mvarTopRight = value;
                //if (!(this.TopLeft == null))
                //{
                //    Top_Box();
                //}

            }
        }


        public string SummaryLabel
        {
            get
            {
                string returnValue = default(string);
                returnValue = mvarSummaryLabel;
                return returnValue;
            }
            set
            {
                mvarSummaryLabel = value;
                mvarSummaryLabel = "";
            }
        }


        public decimal Amount
        {
            get
            {
                return mvarAmount;
            }
            set
            {
                mvarAmount = value;
            }
        }


        public string Account
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAccount;
                return returnValue;
            }
            set
            {
                mvarAccount = value;
            }
        }



        public Credit_Card Card
        {
            get
            {
                Credit_Card returnValue = default(Credit_Card);
                returnValue = mvarCard;
                return returnValue;
            }
            set
            {
                mvarCard = value;
            }
        }


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





    }
}
