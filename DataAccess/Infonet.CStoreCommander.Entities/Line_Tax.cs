using System;
using System.Security.AccessControl;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Line_Tax
    {
        private string mvarTax_Name;
        private string mvarTax_Code;
        private float mvarTax_Rate;
        private float mvarTaxable_Amount;
        private float mvarTax_Added_Amount;
        private float mvarTax_Incl_Amount;
        private float mvarTax_Incl_Total;
        private bool mvarTax_Included;
        
        private float mvarTax_Hidden_Total;
        private bool mvarTax_Hidden;
        
        private decimal mvarTax_Rebate;
        private float mvarTax_Rebate_Rate;

        public int SaleNumber { get; set; }
        public int TillNumber { get; set; }
        public int LineNumber { get; set; }


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



        public float Taxable_Amount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTaxable_Amount;
                return returnValue;
            }
            set
            {
                mvarTaxable_Amount = value;
            }
        }


        public float Tax_Added_Amount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTax_Added_Amount;
                return returnValue;
            }
            set
            {
                mvarTax_Added_Amount = value;
            }
        }


        public float Tax_Incl_Amount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTax_Incl_Amount;
                return returnValue;
            }
            set
            {
                mvarTax_Incl_Amount = value;
            }
        }



        public float Tax_Incl_Total
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTax_Incl_Total;
                return returnValue;
            }
            set
            {
                mvarTax_Incl_Total = value;
            }
        }



        public bool Tax_Included
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTax_Included;
                return returnValue;
            }
            set
            {
                mvarTax_Included = value;
            }
        }

        

        public float Tax_Hidden_Total
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTax_Hidden_Total;
                return returnValue;
            }
            set
            {
                mvarTax_Hidden_Total = value;
            }
        }


        public bool Tax_Hidden
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTax_Hidden;
                return returnValue;
            }
            set
            {
                mvarTax_Hidden = value;
                
                if (value)
                {
                    mvarTax_Included = true;
                }
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
        //   end
    }
}
