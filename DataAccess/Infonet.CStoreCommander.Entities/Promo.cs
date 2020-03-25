using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Promo
    {
        //Added new property
        public string StockCode { get; set; }
        public string Dept { get; set; }
        public string SubDept { get; set; }
        public string SubDetail { get; set; }

        private string mvarPromoID;
        private string mvarDescription;
        private DateTime mvarStartDate;
        private DateTime mvarEndDate;
        private string mvarDiscType;
        private double mvarAmount;
        private string mvarPrType;
        private double mvarSumRegPrice;
        private short mvarTotalQty;
        private short mvarMaxLink;
        private byte mvarDay;
        private bool mvarHasFuel;
        private bool mvarMultiLink;
        private Promo_Lines mvarPromo_Lines;


        public Promo_Lines Promo_Lines
        {
            get
            {
                Promo_Lines returnValue = default(Promo_Lines);
                if (mvarPromo_Lines == null)
                {
                    mvarPromo_Lines = new Promo_Lines();
                }
                returnValue = mvarPromo_Lines;
                return returnValue;
            }
            set
            {
                mvarPromo_Lines = value;
            }
        }


        public string PromoID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPromoID;
                return returnValue;
            }
            set
            {
                mvarPromoID = value;
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


        public DateTime StartDate
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarStartDate;
                return returnValue;
            }
            set
            {
                mvarStartDate = value;
            }
        }


        public DateTime EndDate
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarEndDate;
                return returnValue;
            }
            set
            {
                mvarEndDate = value;
            }
        }


        public string DiscType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDiscType;
                return returnValue;
            }
            set
            {
                mvarDiscType = value;
            }
        }


        public double Amount
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
            }
        }


        public double SumRegPrice
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarSumRegPrice;
                return returnValue;
            }
            set
            {
                mvarSumRegPrice = value;
            }
        }

        public short TotalQty
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarTotalQty;
                return returnValue;
            }
            set
            {
                mvarTotalQty = value;
            }
        }

        public short MaxLink
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarMaxLink;
                return returnValue;
            }
            set
            {
                mvarMaxLink = value;
            }
        }



        public byte Day
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarDay;
                return returnValue;
            }
            set
            {
                mvarDay = value;
            }
        }


        public bool HasFuel
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarHasFuel;
                return returnValue;
            }
            set
            {
                mvarHasFuel = value;
            }
        }


        public bool MultiLink
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarMultiLink;
                return returnValue;
            }
            set
            {
                mvarMultiLink = value;
            }
        }


        public string PrType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPrType;
                return returnValue;
            }
            set
            {
                mvarPrType = value;
            }
        }
    }
}
