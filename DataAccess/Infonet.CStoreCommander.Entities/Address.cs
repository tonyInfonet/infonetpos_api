namespace Infonet.CStoreCommander.Entities
{
    public class Address
    {

        private string mvarStreet1;
        private string mvarStreet2;
        private string mvarCity;
        private string mvarProvState;
        private string mvarPostalCode;
        private string mvarCountry;
        private string mvarEMail;
        private Phones mvarPhones;



        public Phones Phones
        {
            get
            {
                Phones returnValue = default(Phones);
                if (mvarPhones == null)
                {
                    mvarPhones = new Phones();
                }
                returnValue = mvarPhones;
                return returnValue;
            }
            set
            {
                mvarPhones = value;
            }
        }





        public string EMail
        {
            get
            {
                string returnValue = "";
                returnValue = mvarEMail;
                return returnValue;
            }
            set
            {
                mvarEMail = value;
            }
        }





        public string Country
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCountry;
                return returnValue;
            }
            set
            {
                mvarCountry = value;
            }
        }





        public string PostalCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPostalCode;
                return returnValue;
            }
            set
            {
                mvarPostalCode = value;
            }
        }





        public string ProvState
        {
            get
            {
                string returnValue = "";
                returnValue = mvarProvState;
                return returnValue;
            }
            set
            {
                mvarProvState = value;
            }
        }





        public string City
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCity;
                return returnValue;
            }
            set
            {
                mvarCity = value;
            }
        }





        public string Street2
        {
            get
            {
                string returnValue = "";
                returnValue = mvarStreet2;
                return returnValue;
            }
            set
            {
                mvarStreet2 = value;
            }
        }





        public string Street1
        {
            get
            {
                string returnValue = "";
                returnValue = mvarStreet1;
                return returnValue;
            }
            set
            {
                mvarStreet1 = value;
            }
        }
    }
}
