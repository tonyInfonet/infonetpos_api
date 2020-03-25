namespace Infonet.CStoreCommander.Entities
{
    public class CardCode
    {

        // =================================================================================
        // These are the ranges of code values that identify a card type. They are
        // included in the CardCodes collection in each Card object.
        // =================================================================================

        private string mvarLowerLimit;
        private string mvarUpperLimit;



        public string UpperLimit
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUpperLimit;
                return returnValue;
            }
            set
            {
                mvarUpperLimit = value;
            }
        }



        public string LowerLimit
        {
            get
            {
                string returnValue = "";
                returnValue = mvarLowerLimit;
                return returnValue;
            }
            set
            {
                mvarLowerLimit = value;
            }
        }
    }
}
