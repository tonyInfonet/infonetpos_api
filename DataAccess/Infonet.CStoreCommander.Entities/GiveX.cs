namespace Infonet.CStoreCommander.Entities
{
    public class GiveX
    {

        private string mvarIP;
        private short mvarTcpPort;
        private string mvarUserID;
        private string mvarUserPassword;
        private short mvarCommunicationTimeOut;
        private string mvarGiveXMerchID;
        private bool mvarAlwAdjGiveX;

        

        public string IP
        {
            get
            {
                string returnValue = "";
                returnValue = mvarIP;
                return returnValue;
            }
            set
            {
                mvarIP = value;
            }
        }

        

        public short TcpPort
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarTcpPort;
                return returnValue;
            }
            set
            {
                mvarTcpPort = value;
            }
        }

        

        public short CommunicationTimeOut
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarCommunicationTimeOut;
                return returnValue;
            }
            set
            {
                mvarCommunicationTimeOut = value;
            }
        }

        

        public string UserID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUserID;
                return returnValue;
            }
            set
            {
                mvarUserID = value;
            }
        }

        

        public string UserPassword
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUserPassword;
                return returnValue;
            }
            set
            {
                mvarUserPassword = value;
            }
        }

        

        public string MerchantID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarGiveXMerchID;
                return returnValue;
            }
            set
            {
                mvarGiveXMerchID = value;
            }
        }

        

        public string AllowAdjustGiveX
        {
            get
            {
                string returnValue = "";
                returnValue = (mvarAlwAdjGiveX).ToString();
                return returnValue;
            }
            set
            {
                mvarAlwAdjGiveX = bool.Parse(value);
            }
        }
        

        private void Class_Initialize_Renamed()
        {
            LoadGivexPolicy();
        }
        public GiveX()
        {
            Class_Initialize_Renamed();
        }

        public void LoadGivexPolicy()
        {
            //Smriti move this code to manager
            //string temp_Policy_Name = "GiveX_IP";
            //mvarIP = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name, null));
            //string temp_Policy_Name2 = "GiveX_Port";
            //mvarTcpPort = System.Convert.ToInt16(modPolicy.GetPol(temp_Policy_Name2, null));
            //string temp_Policy_Name3 = "GiveTimeOut";
            //mvarCommunicationTimeOut = System.Convert.ToInt16(modPolicy.GetPol(temp_Policy_Name3, null));
            //string temp_Policy_Name4 = "GiveX_User";
            //mvarUserID = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name4, null));
            //string temp_Policy_Name5 = "GiveX_Pass";
            //mvarUserPassword = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name5, null));
            //string temp_Policy_Name6 = "GiveXMerchID";
            //mvarGiveXMerchID = System.Convert.ToString(modPolicy.GetPol(temp_Policy_Name6, null));
            //string temp_Policy_Name7 = "AlwAdjGiveX";
            //mvarAlwAdjGiveX = System.Convert.ToBoolean(modPolicy.GetPol(temp_Policy_Name7, null)); 
        }
    }
}
