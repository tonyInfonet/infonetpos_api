namespace Infonet.CStoreCommander.Entities
{
    public class Phone
    {

        private string mvarPhoneName;
        private string mvarPhoneType;
        private string mvarAreaCode;
        private string mvarNumber;



        public string Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarNumber;
                return returnValue;
            }
            set
            {
                mvarNumber = value;
            }
        }



        public string AreaCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAreaCode;
                return returnValue;
            }
            set
            {
                mvarAreaCode = value;
            }
        }





        public string PhoneType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPhoneType;
                return returnValue;
            }
            set
            {
                mvarPhoneType = value;
            }
        }





        public string PhoneName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPhoneName;
                return returnValue;
            }
            set
            {
                mvarPhoneName = value;
            }
        }
    }
}
