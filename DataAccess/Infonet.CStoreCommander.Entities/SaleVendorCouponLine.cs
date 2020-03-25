namespace Infonet.CStoreCommander.Entities
{
    public class SaleVendorCouponLine
    {

        private float mvarUnitValue;
        private float mvarQuantity;
        private short mvarLine_Num;
        private string mvarCouponCode;
        private string mvarCouponName;
        private decimal mvarTotalValue;
        private string mvarSerialNumber; 
        private short mvarSeqNum; 
        private string mvarTendDesc; 
        private short mvarItemNum; 

        public string CouponCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCouponCode;
                return returnValue;
            }
            set
            {
                mvarCouponCode = value;
            }
        }


        public string CouponName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCouponName;
                return returnValue;
            }
            set
            {
                mvarCouponName = value;
            }
        }


        public short Line_Num
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLine_Num;
                return returnValue;
            }
            set
            {
                mvarLine_Num = value;
            }
        }


        public float Quantity
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarQuantity;
                return returnValue;
            }
            set
            {
                mvarQuantity = value;
            }
        }


        public float UnitValue
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarUnitValue;
                return returnValue;
            }
            set
            {
                mvarUnitValue = value;
            }
        }


        public decimal TotalValue
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTotalValue;
                return returnValue;
            }
            set
            {
                mvarTotalValue = value;
            }
        }

        

        public string SerialNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSerialNumber;
                return returnValue;
            }
            set
            {
                mvarSerialNumber = value;
            }
        }


        public short SeqNum
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarSeqNum;
                return returnValue;
            }
            set
            {
                mvarSeqNum = value;
            }
        }
        

        

        public string TendDesc
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTendDesc;
                return returnValue;
            }
            set
            {
                mvarTendDesc = value;
            }
        }

        

        public short ItemNum
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarItemNum;
                return returnValue;
            }
            set
            {
                mvarItemNum = value;
            }
        }

        private void Class_Initialize_Renamed()
        {
            mvarSerialNumber = "";
            mvarSeqNum = (short)1;
        }
        public SaleVendorCouponLine()
        {
            Class_Initialize_Renamed();
        }
        
    }
}
