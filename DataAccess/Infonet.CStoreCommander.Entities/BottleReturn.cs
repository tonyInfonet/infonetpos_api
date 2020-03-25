namespace Infonet.CStoreCommander.Entities
{
    public class BottleReturn
    {
        private float mvarPrice;
        private float mvarQuantity;
        private short mvarLine_Num;
        private string mvarProduct;
        private decimal mvarAmount;

        public string Image_Url { get; set; }
        public string Description { get; set; }

        public string Product
        {
            get
            {
                string returnValue = "";
                returnValue = mvarProduct;
                return returnValue;
            }
            set
            {
                mvarProduct = value;
            }
        }


        public short LineNumber
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


        public decimal Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
            }
        }
    }
}
