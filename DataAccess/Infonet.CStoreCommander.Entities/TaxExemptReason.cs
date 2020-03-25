
namespace Infonet.CStoreCommander.Entities
{
    public class TaxExemptReason
    {

        private string mvarCode;
        private string mvarDescription;
        private short mvarExplanationCode;
        private string mvarExplanation;


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


        public string Description
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDescription;
                return returnValue;
            }
            set
            {
                mvarDescription = value;
            }
        }


        public short ExplanationCode
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarExplanationCode;
                return returnValue;
            }
            set
            {
                mvarExplanationCode = value;
            }
        }


        public string Explanation
        {
            get
            {
                string returnValue = "";
                returnValue = mvarExplanation;
                return returnValue;
            }
            set
            {
                mvarExplanation = value;
            }
        }
    }
}
