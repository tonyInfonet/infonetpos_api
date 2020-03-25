using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class AR_Payment
    {
        private Customer mvarCustomer;
        private Credit_Card mvarCard;
        private string mvarAccount;
        private decimal mvarAmount;
        private string mvarTotalLabel;
        private string mvarSummaryLabel;
        private string mvarTopLeft;
        private string mvarTopRight;
        private int mvarSale_Num;
        private decimal mvarPenny_Adj; //  


        public AR_Payment()
        {
        }

        public string TotalLabel
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTotalLabel;
                return returnValue;
            }
            set
            {
                mvarTotalLabel = "";
            }
        }


        public string TopLeft
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTopLeft;
                return returnValue;
            }
            set
            {
                mvarTopLeft = value;
                if (!(this.TopRight == null))
                {
                    //Top_Box();
                }
            }
        }


        public string TopRight
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTopRight;
                return returnValue;
            }
            set
            {
                mvarTopRight = value;
                if (!(this.TopLeft == null))
                {
                   // Top_Box();
                }

            }
        }


        public string SummaryLabel
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSummaryLabel;
                return returnValue;
            }
            set
            {
                mvarSummaryLabel = value;
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
                if (mvarCustomer == null)
                {
                    mvarCustomer = new Customer();
                }
                returnValue = mvarCustomer;
                return returnValue;
            }
            set
            {
                mvarCustomer = value;
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

       

     
    }
}

