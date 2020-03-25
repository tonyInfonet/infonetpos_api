
namespace Infonet.CStoreCommander.Entities
{
    public class Prepay
    {

        private bool mPrePayment;
        private short mPumpID;
        private float mPrepAmount;

        private float mCompAmount;
        private float mPosition;
        private short mGradeID;
        private int mvarSaleNo;
        private decimal mvarOverPayment;
        private double mvarUnitPrice;
        private float mvarVolume;
        private bool[] misLocked = new bool[33];
        private Tenders mvarTenders;
        private Customer mvarCustomer;
        private string mvarTopLeft;
        private string mvarTopRight;

        private void Class_Initialize_Renamed()
        {
            mvarTenders = new Tenders();
            mvarCustomer = new Customer();
        }
        public Prepay()
        {
            Class_Initialize_Renamed();
        }


        public string TopLeft
        {
            get
            {
                string returnValue = default(string);
                returnValue = mvarTopLeft;
                return returnValue;
            }
            set
            {
                mvarTopLeft = value;
                mvarTopLeft = "";
                //if (!(this.TopRight == null))
                //{
                //    Top_Box();
                //}
            }
        }


        public string TopRight
        {
            get
            {
                string returnValue = default(string);
                returnValue = mvarTopRight;
                return returnValue;
            }
            set
            {
                mvarTopRight = value;
                mvarTopRight = "";
                //if (!(this.TopLeft == null))
                //{
                //    Top_Box();
                //}
            }
        }


        public Customer Customer
        {
            get
            {
                Customer returnValue = default(Customer);
                returnValue = mvarCustomer;
                return returnValue;
            }
            set
            {
                mvarCustomer = value;
            }
        }


        public Tenders Tenders
        {
            get
            {
                Tenders returnValue = default(Tenders);
                if (mvarTenders == null)
                {
                    mvarTenders = new Tenders();
                }

                returnValue = mvarTenders;
                return returnValue;
            }
            set
            {
                mvarTenders = value;
            }
        }



        public float PrepAmount
        {
            get
            {
                float returnValue = 0;
                returnValue = mPrepAmount;
                return returnValue;
            }
            set
            {
                mPrepAmount = value;
            }
        }


        public double UnitPrice
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarUnitPrice;
                return returnValue;
            }
            set
            {
                mvarUnitPrice = value;
            }
        }


        public float Volume
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarVolume;
                return returnValue;
            }
            set
            {
                mvarVolume = value;
            }
        }


        public decimal OverPayment
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarOverPayment;
                return returnValue;
            }
            set
            {
                mvarOverPayment = value;
            }
        }


        public float compAmount
        {
            get
            {
                float returnValue = 0;
                returnValue = mCompAmount;
                return returnValue;
            }
            set
            {
                mCompAmount = value;
            }
        }


        public short Position
        {
            get
            {
                short returnValue = 0;
                returnValue = (short)mPosition;
                return returnValue;
            }
            set
            {
                mPosition = value;
            }
        }


        public int SaleNo
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarSaleNo;
                return returnValue;
            }
            set
            {
                mvarSaleNo = value;
            }
        }


        public short Grade
        {
            get
            {
                short returnValue = 0;
                returnValue = mGradeID;
                return returnValue;
            }
            set
            {
                mGradeID = value;
            }
        }


        public short Pump
        {
            get
            {
                short returnValue = 0;
                returnValue = mPumpID;
                return returnValue;
            }
            set
            {
                mPumpID = value;
            }
        }


        public bool get_isLocked(short pumpID)
        {
            bool returnValue = false;
            returnValue = misLocked[pumpID];
            return returnValue;
        }
        public void set_isLocked(short pumpID, bool Value)
        {
            misLocked[pumpID] = Value;
        }

        //private void Top_Box()
        //{
        //    //    mvarTopLeft.Caption = GetResString(278) & " : " & vbCrLf
        //    mvarTopLeft.Text = Chaps_Main.Resource.DisplayCaption((short)278, (short)0, null, (short)0) + " : " + "\r\n";
        //    mvarTopRight.Text = this.PrepAmount.ToString("###,##0.00") + "\r\n";
        //}


   

        private void Class_Terminate_Renamed()
        {
            mvarTenders = null;
            mvarCustomer = null;
        }
        ~Prepay()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }
        
    }
}
