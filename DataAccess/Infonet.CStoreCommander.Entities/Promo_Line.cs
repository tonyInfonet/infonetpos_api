
namespace Infonet.CStoreCommander.Entities
{
    public class Promo_Line
    {
        private byte mvarLink;
        private string mvarStock_Code;
        private float mvarQuantity;
        private string mvarDept;
        private string mvarSub_Dept;
        private string mvarSub_Detail;
        private float mvarItem_Fraction;
        private float mvarAmount;
        private byte mvarLevel;


        public byte Link
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarLink;
                return returnValue;
            }
            set
            {
                mvarLink = value;
            }
        }


        public string Stock_Code
        {
            get
            {
                string returnValue = "";
                returnValue = mvarStock_Code;
                return returnValue;
            }
            set
            {
                mvarStock_Code = value;
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


        public string Sub_Detail
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSub_Detail;
                return returnValue;
            }
            set
            {
                mvarSub_Detail = value;
            }
        }


        public float Item_Fraction
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarItem_Fraction;
                return returnValue;
            }
            set
            {
                mvarItem_Fraction = value;
            }
        }


        public string Sub_Dept
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSub_Dept;
                return returnValue;
            }
            set
            {
                mvarSub_Dept = value;
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


        public float Amount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
            }
        }


        public byte Level
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarLevel;
                return returnValue;
            }
            set
            {
                mvarLevel = value;
            }
        }
    }
}
