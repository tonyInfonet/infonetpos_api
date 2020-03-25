namespace Infonet.CStoreCommander.Entities
{
    public class User
    {

        private string mvarCode;
        private string mvarName;
        private string mvarPassword;
        private string mvarepw;
        private User_Group mvarUserGroup;
        

        public User_Group User_Group
        {
            get
            {
                User_Group returnValue = default(User_Group);
                if (mvarUserGroup == null)
                {
                    mvarUserGroup = new User_Group();
                }
                returnValue = mvarUserGroup;
                return returnValue;
            }
            set
            {
                mvarUserGroup = value;
            }
        }



        public string Password
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPassword;
                return returnValue;
            }
            set
            {
                mvarPassword = value;
            }
        }
        //  for encrypted password


        public string epw
        {
            get
            {
                return mvarepw;
            }
            set
            {
                mvarepw = value;
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

        public bool Valid_User
        {
            get
            {
                bool returnValue = false;
                if (mvarCode.Length > 0)
                {
                    returnValue = true;
                }
                else
                {
                    returnValue = false;
                }
                return returnValue;
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
                
    }
}
