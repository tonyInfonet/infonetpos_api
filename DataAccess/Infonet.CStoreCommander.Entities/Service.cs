
namespace Infonet.CStoreCommander.Entities
{
    public class Service
    {
        private bool mvarPostPayEnabled;
        private bool mvarPayPumpEnabled;
        private bool mvarPrepayEnabled;


        public bool PostPayEnabled
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPostPayEnabled;
                return returnValue;
            }
            set
            {
                mvarPostPayEnabled = value;
            }
        }


        public bool PayPumpEnabled
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPayPumpEnabled;
                return returnValue;
            }
            set
            {
                mvarPayPumpEnabled = value;
            }
        }


        public bool PrepayEnabled
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPrepayEnabled;
                return returnValue;
            }
            set
            {
                mvarPrepayEnabled = value;
            }
        }

       
        public Service()
        {
           // Class_Initialize_Renamed();
        }

        //   to keep if the PostPay was set from POS manually or from FC based on activation/deactivation time
        // PostPayManually is used combined with AUTDACT_PI policy. If the policy is set to "No" the value will be 1 always.
    }
}
