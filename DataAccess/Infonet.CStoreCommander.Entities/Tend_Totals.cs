using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Tend_Totals
    {
        private decimal mvarTend_Amount;
        private decimal mvarTend_Used;
        private decimal mvarChange;
        private decimal mvarNoChangeTotal;
        private decimal mvarGross;
        private decimal mvarChangePaid; // Nicolette added
        private decimal mvarPenny_Adj; //  
        private bool mvarPennyAdj_Required; //  

        public decimal Gross
        {
            set
            {
                //TODO : remove dependency of another function
                mvarGross = (decimal)(Math.Round((double)value, 2));
            }
        }


        public decimal Tend_Used
        {
            get
            {
                decimal returnValue = 0;
                returnValue = (decimal)(Math.Round((double)mvarTend_Used, 2));
                return returnValue;
            }
            set
            {
                mvarTend_Used = (decimal)(Math.Round((double)value, 2));
                //   for penny rounding
                if (mvarPennyAdj_Required)
                {
                    mvarPenny_Adj = (decimal)(Math.Round((double)(mvarGross - Conversion.Int(mvarGross * 10) / 10), 2));
                    if ((mvarPenny_Adj == 0.01M) || (mvarPenny_Adj == 0.06M))
                    {
                        mvarPenny_Adj = -0.01M;
                    }
                    else if ((mvarPenny_Adj == 0.02M) || (mvarPenny_Adj == 0.07M))
                    {
                        mvarPenny_Adj = -0.02M;
                    }
                    else if ((mvarPenny_Adj == 0.03M) || (mvarPenny_Adj == 0.08M))
                    {
                        mvarPenny_Adj = 0.02M;
                    }
                    else if ((mvarPenny_Adj == 0.04M) || (mvarPenny_Adj == 0.09M))
                    {
                        mvarPenny_Adj = 0.01M;
                    }
                    else if ((mvarPenny_Adj == 0.05M) || (mvarPenny_Adj == 0.1M))
                    {
                        mvarPenny_Adj = 0;
                    }
                    if (mvarTend_Used < 0 & mvarGross < 0)
                    {
                        mvarPenny_Adj = (-1) * mvarPenny_Adj; //   reverse the sign for refunds
                    }
                    //        mvarPenny_Adj = -1 * (MinThreeValues(mvarPenny_Adj, (0.05 - mvarPenny_Adj), (0.1 - mvarPenny_Adj)))
                }
                else
                {
                    mvarPenny_Adj = 0;
                }
                //   end
                this.Change = (decimal)(Math.Round((double)(System.Math.Abs(mvarGross)), 2) + Math.Round((double)mvarPenny_Adj, 2) - Math.Round((double)(System.Math.Abs(mvarTend_Used)), 2));
            }
        }


        public decimal No_Change_Total
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarNoChangeTotal;
                return returnValue;
            }
            set
            {
                mvarNoChangeTotal = value;
            }
        }


        public decimal Tend_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTend_Amount;
                return returnValue;
            }
            set
            {
                //TODO : remove dependency of another function
                mvarTend_Amount = (decimal)(Math.Round((double)value, 2));
            }
        }


        public decimal Change
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarChange;
                return returnValue;
            }
            set
            {
                //TODO : remove dependency of another function
                mvarChange = (decimal)(Math.Round((double)value, 2));
            }
        }
        // Nicolette added

        public decimal ChangePaid
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarChangePaid;
                return returnValue;
            }
            set
            {
                mvarChangePaid = value;
            }
        }
        // Nicolette end


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


        public bool PennyAdj_Required
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPennyAdj_Required;
                return returnValue;
            }
            set
            {
                mvarPennyAdj_Required = value;
            }
        }

        private void Class_Initialize_Renamed()
        {
            mvarPennyAdj_Required = false;
            mvarPenny_Adj = 0;
        }
        public Tend_Totals()
        {
            Class_Initialize_Renamed();
        }
    }
}
