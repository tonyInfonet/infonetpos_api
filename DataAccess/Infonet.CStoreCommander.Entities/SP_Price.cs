
using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class SP_Price
    {
        private float mvarFrom_Quantity;
        private float mvarTo_Quantity;
        private float mvarPrice;
        private object mvarFromDate;
        private object mvarToDate;



        public float Price
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarPrice;
                return returnValue;
            }
            set
            {
                mvarPrice = value;
            }
        }


        public dynamic FromDate
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = mvarFromDate;
                return returnValue;
            }
            set
            {
                mvarFromDate = value;
            }
        }


        public dynamic ToDate
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = mvarToDate;
                return returnValue;
            }
            set
            {
                mvarToDate = value;
            }
        }




        public float To_Quantity
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTo_Quantity;
                return returnValue;
            }
            set
            {
                mvarTo_Quantity = value;
            }
        }





        public float From_Quantity
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarFrom_Quantity;
                return returnValue;
            }
            set
            {
                mvarFrom_Quantity = value;
            }
        }
    }
}
