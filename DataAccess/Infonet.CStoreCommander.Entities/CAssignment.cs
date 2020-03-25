namespace Infonet.CStoreCommander.Entities
{
    public class CAssignment
    {
        private object myGradeID;
        private object myTankID;
        private object myRegTank;
        private object myPremTank;


        public dynamic GradeID
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myGradeID;
                return returnValue;
            }
            set
            {
                myGradeID = value;
            }
        }


        public dynamic TankID
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myTankID;
                return returnValue;
            }
            set
            {
                myTankID = value;
            }
        }


        public dynamic RegTank
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myRegTank;
                return returnValue;
            }
            set
            {
                myRegTank = value;
            }
        }

        public dynamic PremTank
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPremTank;
                return returnValue;
            }
            set
            {
                myPremTank = value;
            }
        }
    }
}
