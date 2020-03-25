
using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Return_Reason
    {
        private string mvarReason;
        private string mvarRType; // Nicolette added to keep reason type
        private string mvarDescription; // Nicolette added to keep reason description


        public string Reason
        {
            get
            {
                return mvarReason;
            }
            set
            {
                mvarReason = value;
            }
        }


        public string RType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRType;
                return returnValue;
            }
            set
            {
                mvarRType = value;
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

        private void Class_Initialize_Renamed()
        {
            mvarDescription = "";
            mvarReason = "";
        }
        public Return_Reason()
        {
            Class_Initialize_Renamed();
        }
    }
}
