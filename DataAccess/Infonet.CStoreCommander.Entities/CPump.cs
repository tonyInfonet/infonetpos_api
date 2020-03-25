namespace Infonet.CStoreCommander.Entities
{
    public class CPump
    {
        private object myModel;
        private object myTitle;
        private object myPlaceOnScreen;
        private object myPump_Space;
        private object myTierID;
        private object myLevelID;
        private object myCutOff;
        private object myAutoPay;
        private object myAutoAuthorize;
        private object myCashierAuthorize;
        private object myPrepayOnly;
        private object myAllowRegularSale;
        private bool myManualPump;
        private bool myAllowPostPay; 
        private short myMaxPositionID; 
        private bool myAllowPreauth; 
        private bool myAuthorizeFromTill; //


        public dynamic Model
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myModel;
                return returnValue;
            }
            set
            {
                myModel = value;
            }
        }


        public dynamic Title
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myTitle;
                return returnValue;
            }
            set
            {
                myTitle = value;
            }
        }


        public dynamic PlaceOnScreen
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPlaceOnScreen;
                return returnValue;
            }
            set
            {
                myPlaceOnScreen = value;
            }
        }


        public dynamic Pump_Space
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPump_Space;
                return returnValue;
            }
            set
            {
                myPump_Space = value;
            }
        }


        public dynamic TierID
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myTierID;
                return returnValue;
            }
            set
            {
                myTierID = value;
            }
        }


        public dynamic LevelID
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myLevelID;
                return returnValue;
            }
            set
            {
                myLevelID = value;
            }
        }


        public dynamic CutOff
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myCutOff;
                return returnValue;
            }
            set
            {
                myCutOff = value;
            }
        }


        public dynamic AutoPay
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myAutoPay;
                return returnValue;
            }
            set
            {
                myAutoPay = value;
            }
        }


        public dynamic AutoAuthorize
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myAutoAuthorize;
                return returnValue;
            }
            set
            {
                myAutoAuthorize = value;
            }
        }


        public dynamic CashierAuthorize
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myCashierAuthorize;
                return returnValue;
            }
            set
            {
                myCashierAuthorize = value;
            }
        }


        public dynamic PrepayOnly
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPrepayOnly;
                return returnValue;
            }
            set
            {
                myPrepayOnly = value;
            }
        }


        public dynamic AllowRegularSale
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myAllowRegularSale;
                return returnValue;
            }
            set
            {
                myAllowRegularSale = value;
            }
        }


        public bool ManualPump
        {
            get
            {
                bool returnValue = false;
                returnValue = myManualPump;
                return returnValue;
            }
            set
            {
                myManualPump = value;
            }
        }


        

        public bool AllowPostPay
        {
            get
            {
                bool returnValue = false;
                returnValue = myAllowPostPay;
                return returnValue;
            }
            set
            {
                myAllowPostPay = value;
            }
        }
        

        

        public bool AllowPreauth
        {
            get
            {
                bool returnValue = false;
                returnValue = myAllowPreauth;
                return returnValue;
            }
            set
            {
                myAllowPreauth = value;
            }
        }
        

        

        public short MaxPositionID
        {
            get
            {
                short returnValue = 0;
                returnValue = myMaxPositionID;
                return returnValue;
            }
            set
            {
                myMaxPositionID = value;
            }
        }
        

        // added for to see if this pump can be authorized from the till

        public bool AuthorizeFromTill
        {
            get
            {
                bool returnValue = false;
                returnValue = myAuthorizeFromTill;
                return returnValue;
            }
            set
            {
                myAuthorizeFromTill = value;
            }
        }
        
    }
}
