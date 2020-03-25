namespace Infonet.CStoreCommander.Entities
{
    public class HotButton
    {

        private short mvarButton_Number;
        private short mvarPage_Number;
        private short mvarButton_Count;
        private string mvarButton_Product;
        private short mvarDefaultValue;
        private float mvarBottlePrice;
        private float mvarQuantity; // 
        private bool mvarby_weight; // 

        public string StockCode { get; set; }

        public int DefaultQuantity { get; set; }

        public string ImageUrl { get; set; }

        public string Button_Product
        {
            get
            {
                string returnValue = "";
                returnValue = mvarButton_Product;
                return returnValue;
            }
            set
            {
                mvarButton_Product = value;
            }
        }


        public short Button_Count
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarButton_Count;
                return returnValue;
            }
            set
            {
                mvarButton_Count = value;
            }
        }


        public short Page_Number
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarPage_Number;
                return returnValue;
            }
            set
            {
                mvarPage_Number = value;
            }
        }


        public short Button_Number
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarButton_Number;
                return returnValue;
            }
            set
            {
                mvarButton_Number = value;
            }
        }


        public short DefaultValue
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarDefaultValue;
                return returnValue;
            }
            set
            {
                mvarDefaultValue = value;
            }
        }

        

        public float BottlePrice
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarBottlePrice;
                return returnValue;
            }
            set
            {
                mvarBottlePrice = value;
            }
        }
        

        //  Scale intgration from Hot button. get the quantity from scale and keep it

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
        //Shiny end - dec8
        //  To identify it as a  item by weight

        public bool By_Weight
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarby_weight;
                return returnValue;
            }
            set
            {
                mvarby_weight = value;
            }
        }
        // 
    }
}
