namespace Infonet.CStoreCommander.Entities
{
    public class CPricesToDisplay
    {
        private byte myGradeID;
        private byte myTierID;
        private byte myLevelID;


        public byte GradeID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myGradeID;
                return returnValue;
            }
            set
            {
                myGradeID = value;
            }
        }


        public byte TierID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myTierID;
                return returnValue;
            }
            set
            {
                myTierID = value;
            }
        }


        public byte LevelID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myLevelID;
                return returnValue;
            }
            set
            {
                myLevelID = value;
            }
        }
    }
}
