namespace Infonet.CStoreCommander.Entities
{
    public class User_Group
    {

        private string mvarCode;
        private string mvarName;
        private byte mvarSecurityLevel; 



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

        

        public byte SecurityLevel
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarSecurityLevel;
                return returnValue;
            }
            set
            {
                mvarSecurityLevel = value;
            }
        }
        
    }
}
