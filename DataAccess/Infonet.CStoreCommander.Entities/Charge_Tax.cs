using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Charge_Tax
    {
        private string mvarTax_Name;
        private string mvarTax_Code;
        private float mvarTax_Rate;
        private float mvarTaxable_Amount;
        private float mvarTax_Added_Amount;
        private float mvarTax_Incl_Amount;
        private float mvarTax_Incl_Total;
        private bool mvarTax_Included;


        public int SaleNumber { get; set; }
        public int TillNumber { get; set; }
        public int LineNumber { get; set; }
        public string KitItem { get; set; }
        public string ChargeCode { get; set; }
        public float Quanitity { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }

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
    }
}
