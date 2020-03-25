using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Line_Kit
    {
        private string mvarKit_Item;
        private string mvarKit_Item_Desc;
        private float mvarKit_Item_Qty;
        private float mvarTTL_Kit_Items;
        private float mvarKit_Item_Base;
        private float mvarKit_Item_Fraction;
        private float mvarKit_Item_Allocate;
        private string mvarKit_Item_Serial;

        private K_Charges mvarK_Charges;

        public int SaleNumber { get; set; }
        public int TillNumber { get; set; }
        public int LineNumber { get; set; }


        public K_Charges K_Charges
        {
            get
            {
                K_Charges returnValue = default(K_Charges);
                if (mvarK_Charges == null)
                {
                    mvarK_Charges = new K_Charges();
                }
                returnValue = mvarK_Charges;
                return returnValue;
            }
            set
            {
                mvarK_Charges = value;
            }
        }



        public string Kit_Item_Serial
        {
            get
            {
                string returnValue = "";
                returnValue = mvarKit_Item_Serial;
                return returnValue;
            }
            set
            {
                mvarKit_Item_Serial = value;
            }
        }





        public float Kit_Item_Allocate
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarKit_Item_Allocate;
                return returnValue;
            }
            set
            {
                mvarKit_Item_Allocate = value;
            }
        }





        public float Kit_Item_Fraction
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarKit_Item_Fraction;
                return returnValue;
            }
            set
            {
                mvarKit_Item_Fraction = value;
            }
        }





        public float Kit_Item_Base
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarKit_Item_Base;
                return returnValue;
            }
            set
            {
                mvarKit_Item_Base = value;
            }
        }




        public float Kit_Item_Qty
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarKit_Item_Qty;
                return returnValue;
            }
            set
            {
                mvarKit_Item_Qty = value;
            }
        }





        public string Kit_Item_Desc
        {
            get
            {
                string returnValue = "";
                returnValue = mvarKit_Item_Desc;
                return returnValue;
            }
            set
            {
                mvarKit_Item_Desc = value;
            }
        }





        public string Kit_Item
        {
            get
            {
                string returnValue = "";
                returnValue = mvarKit_Item;
                return returnValue;
            }
            set
            {
                mvarKit_Item = value;
            }
        }


        private void Class_Terminate_Renamed()
        {

            mvarK_Charges = null;
        }
        ~Line_Kit()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }
    }
}
