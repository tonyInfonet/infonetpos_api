using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Charge
    {
        private string mvarCharge_Code;
        private string mvarCharge_Desc;
        private float mvarCharge_Price;
        private Charge_Taxes mvarCharge_Taxes;

        public int SaleNumber { get; set; }
        public int TillNumber { get; set; }
        public int LineNumber { get; set; }
        public string AsCode { get; set; }
        public string KitItem { get; set; }
        public float Quantity { get; set; }
        public float Amount { get; set; }
        public string Tax_Code { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public Charge_Taxes Charge_Taxes
        {
            get
            {
                Charge_Taxes returnValue = default(Charge_Taxes);
                if (mvarCharge_Taxes == null)
                {
                    mvarCharge_Taxes = new Charge_Taxes();
                }
                returnValue = mvarCharge_Taxes;
                return returnValue;
            }
            set
            {
                mvarCharge_Taxes = value;
            }
        }

        public float Charge_Price
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarCharge_Price;
                return returnValue;
            }
            set
            {
                mvarCharge_Price = value;
            }
        }

        public string Charge_Desc
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCharge_Desc;
                return returnValue;
            }
            set
            {
                mvarCharge_Desc = value;
            }
        }

        public string Charge_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCharge_Code;
                return returnValue;
            }
            set
            {
                mvarCharge_Code = value;
            }
        }

        private void Class_Terminate_Renamed()
        {

            mvarCharge_Taxes = null;
        }
        ~Charge()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }
    }
}
