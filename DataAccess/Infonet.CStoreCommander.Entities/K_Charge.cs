using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class K_Charge
    {

        private double mvarCharge_Price;
        private string mvarCharge_Desc;
        private string mvarCharge_Code;
        private Charge_Taxes mvarCharge_Taxes;



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





        public double Charge_Price
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarCharge_Price;
                return returnValue;
            }
            set
            {
                mvarCharge_Price = value;
            }
        }

        private void Class_Terminate_Renamed()
        {
            mvarCharge_Taxes = null;
        }
        ~K_Charge()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }
    }
}
