using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Sale_Tax
    {
        private string mvarTax_Name;
        private string mvarTax_Code;
        private decimal mvarTaxable_Amount;
        private decimal mvarTax_Added_Amount;
        private decimal mvarTax_Included_Amount;
        private decimal mvarTax_Included_Total;
        private decimal mvarTaxable_Amt_ForIncluded;
        private float mvarTax_Rate;
        private decimal mvarTax_Rebate;
        private float mvarTax_Rebate_Rate;
        private decimal mvarRebatable_Amount;
        private decimal mvarTax_Exemption_GA_Incl;
        private decimal mvarTax_Exemption_GA_Added;

        public int TillNumber { get; set; }

        public int  SaleNumber { get; set; }

        public string Tax_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTax_Code;
                return returnValue;
            }
            set
            {
                mvarTax_Code = value;
            }
        }


        public string Tax_Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTax_Name;
                return returnValue;
            }
            set
            {
                mvarTax_Name = value;
            }
        }


        public decimal Taxable_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTaxable_Amount;
                return returnValue;
            }
            set
            {
                mvarTaxable_Amount = value;
            }
        }



        public decimal Tax_Added_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTax_Added_Amount;
                return returnValue;
            }
            set
            {
                mvarTax_Added_Amount = value;
            }
        }




        public decimal Tax_Included_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTax_Included_Amount;
                return returnValue;
            }
            set
            {
                mvarTax_Included_Amount = value;
            }
        }


        public decimal Tax_Included_Total
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTax_Included_Total;
                return returnValue;
            }
            set
            {
                mvarTax_Included_Total = value;
            }
        }


        public float Tax_Rate
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTax_Rate;
                return returnValue;
            }
            set
            {
                mvarTax_Rate = value;
            }
        }


        public decimal Taxable_Amt_ForIncluded
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTaxable_Amt_ForIncluded;
                return returnValue;
            }
            set
            {
                mvarTaxable_Amt_ForIncluded = value;
            }
        }

        //   for Ontario tax rebate

        public float Tax_Rebate_Rate
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTax_Rebate_Rate;
                return returnValue;
            }
            set
            {
                mvarTax_Rebate_Rate = value;
            }
        }


        public decimal Tax_Rebate
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTax_Rebate;
                return returnValue;
            }
            set
            {
                mvarTax_Rebate = value;
            }
        }

        public bool Tax_Included { get; set; }

        public decimal Rebatable_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarRebatable_Amount;
                return returnValue;
            }
            set
            {
                mvarRebatable_Amount = value;
            }
        }
        //   end

        //  

        public decimal Tax_Exemption_GA_Incl
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTax_Exemption_GA_Incl;
                return returnValue;
            }
            set
            {
                mvarTax_Exemption_GA_Incl = value;
            }
        }


        public decimal Tax_Exemption_GA_Added
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTax_Exemption_GA_Added;
                return returnValue;
            }
            set
            {
                mvarTax_Exemption_GA_Added = value;
            }
        }
        //   end
    }
}
