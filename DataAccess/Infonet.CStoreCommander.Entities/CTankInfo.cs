namespace Infonet.CStoreCommander.Entities
{
    public class CTankInfo
    {

        private string myTankCode;
        private short myGradeID;


        public string TankCode
        {
            get
            {
                string returnValue = "";
                returnValue = myTankCode;
                return returnValue;
            }
            set
            {
                myTankCode = value;
            }
        }


        public short GradeID
        {
            get
            {
                short returnValue = 0;
                returnValue = myGradeID;
                return returnValue;
            }
            set
            {
                myGradeID = value;
            }
        }
    }
}
