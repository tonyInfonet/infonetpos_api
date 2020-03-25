using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    public class Vendor
    {
        private string mvarCode;
        private string mvarName;
        private Address mvarAddress;


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


        public string Name
        {
            get
            {
                return mvarName;
            }
            set
            {
                mvarName = value;
            }
        }


        public Address Address
        {
            get
            {
                if (mvarAddress == null)
                {
                    mvarAddress = new Address();
                }
                return mvarAddress;
            }
            set
            {
                mvarAddress = value;
            }
        }


        private void Class_Initialize_Renamed()
        {
            mvarAddress = new Address();
        }
        public Vendor()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {

            mvarAddress = null;
        }
        ~Vendor()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }
    }
}
