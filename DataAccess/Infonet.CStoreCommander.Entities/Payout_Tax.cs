namespace Infonet.CStoreCommander.Entities
{
    public class Payout_Tax
    {

        private string mvarTax_Name;
        private string mvarTax_Description;
        private decimal mvarTax_Amount;
        private bool mvarTax_Active;



        public decimal Tax_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTax_Amount;
                return returnValue;
            }
            set
            {
                mvarTax_Amount = value;
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




        public string Tax_Description
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTax_Description;
                return returnValue;
            }
            set
            {
                mvarTax_Description = value;
            }
        }


        public bool Tax_Active
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTax_Active;
                return returnValue;
            }
            set //  'HSt change
            {
                mvarTax_Active = value;
            }
        }
    }
}
