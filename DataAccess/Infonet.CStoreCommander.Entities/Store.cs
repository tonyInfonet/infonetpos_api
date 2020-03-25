using Infonet.CStoreCommander.Entities;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Store
    {

        private string mvarCode;
        private string mvarName;
        private string mvarRegName;
        private string mvarRegNum;
        private string mvarSaleFoot;
        private string mvarRefFoot;
        private Address mvarAddress;
        private string mvarLanguage;
        private DateTime mvarInstall_Date; //local copy
        private string mvarSecurity_Key; //local copy
        private string mvarStatuse; //local copy
        private string mvarSecRegNum; 
        private string mvarSecRegName; 
        private string mvarTaxExempt_Footer;
        private string mvarRetailerID; // SITE
        public short OffSet { get; set; } //added for resource string

        public string Statuse
        {
            get
            {
                string returnValue = "";
                returnValue = mvarStatuse;
                return returnValue;
            }
            set
            {
                mvarStatuse = value;
            }
        }


        public string Security_Key
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSecurity_Key;
                return returnValue;
            }
            set
            {
                mvarSecurity_Key = value;
            }
        }


        public DateTime Install_Date
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarInstall_Date;
                return returnValue;
            }
            set
            {
                mvarInstall_Date = value;
            }
        }


        public Address Address
        {
            get
            {
                Address returnValue = default(Address);
                if (mvarAddress == null)
                {
                    mvarAddress = new Address();
                }
                returnValue = mvarAddress;
                return returnValue;
            }
            set
            {
                mvarAddress = value;
            }
        }




        public string Sale_Footer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSaleFoot;
                return returnValue;
            }
            set
            {
                mvarSaleFoot = value;
            }
        }



        public string Refund_Footer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRefFoot;
                return returnValue;
            }
            set
            {
                mvarRefFoot = value;
            }
        }



        public string RegNum
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRegNum;
                return returnValue;
            }
            set
            {
                mvarRegNum = value;
            }
        }




        public string RegName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRegName;
                return returnValue;
            }
            set
            {
                mvarRegName = value;
            }
        }




        public string Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarName;
                return returnValue;
            }
            set
            {
                mvarName = value;
            }
        }





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
        public string Language
        {
            get
            {
                string returnValue = "";
                returnValue = mvarLanguage;
                return returnValue;
            }
            set
            {
                mvarLanguage = value;
            }
        }

        public string SecRegNum
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSecRegNum;
                return returnValue;
            }
            set
            {
                mvarSecRegNum = value;
            }
        }


        public string SecRegName
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSecRegName;
                return returnValue;
            }
            set
            {
                mvarSecRegName = value;
            }
        }
        

        // 


        public string TaxExempt_Footer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTaxExempt_Footer;
                return returnValue;
            }
            set
            {
                mvarTaxExempt_Footer = value;
            }
        }
        //shiny end

        // SITE

        public string RetailerID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRetailerID;
                return returnValue;
            }
            set
            {
                mvarRetailerID = value;
            }
        }
        //   end
    }
}
