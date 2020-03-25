namespace Infonet.CStoreCommander.Entities
{
    public class VendorCoupon
    {

        private float mvarValue;
        private string mvarCode;
        private string mvarName;
        private string mvarVendorCode;
        private string mvarStockCode;
        private string mvarDept;
        private string mvarSubDept;
        private string mvarSubDetail;
        private short mvarIdNum;
        private string mvarTendDesc; 
                                     
        private bool mvarDefaultCoupon;
        private short mvarSerNumLen;


        public bool DefaultCoupon
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarDefaultCoupon;
                return returnValue;
            }
            set
            {
                mvarDefaultCoupon = value;
            }
        }


        public short SerNumLen
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarSerNumLen;
                return returnValue;
            }
            set
            {
                mvarSerNumLen = value;
            }
        }
        


        public short IdNum
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarIdNum;
                return returnValue;
            }
            set
            {
                mvarIdNum = value;
            }
        }


        public string Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCode;
                return returnValue;
            }
            set
            {
                mvarCode = value;
            }
        }


        public string Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarName;
                return returnValue;
            }
            set
            {
                mvarName = value;
            }
        }


        public string VendorCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarVendorCode;
                return returnValue;
            }
            set
            {
                mvarVendorCode = value;
            }
        }


        public string StockCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarStockCode;
                return returnValue;
            }
            set
            {
                mvarStockCode = value;
            }
        }


        public float Value
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarValue;
                return returnValue;
            }
            set
            {
                mvarValue = value;
            }
        }


        public string Dept
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDept;
                return returnValue;
            }
            set
            {
                mvarDept = value;
            }
        }


        public string SubDept
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSubDept;
                return returnValue;
            }
            set
            {
                mvarSubDept = value;
            }
        }


        public string SubDetail
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSubDetail;
                return returnValue;
            }
            set
            {
                mvarSubDetail = value;
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
        
    }
}
