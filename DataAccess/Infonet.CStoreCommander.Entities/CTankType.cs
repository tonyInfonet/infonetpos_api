namespace Infonet.CStoreCommander.Entities
{
    public class CTankType
    {
        private object myDescription;
        private object myLength;
        private object myDiameter;
        private object myCapacity;
        private object myTankEnds;
        private object myOrientation;
        private object myReadingtype;
        private object myUseChart;
        private object myLengthUnits;
        private object myVolumeUnits;



        public dynamic Description
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myDescription;
                return returnValue;
            }
            set
            {
                myDescription = value;
            }
        }


        public dynamic Length
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myLength;
                return returnValue;
            }
            set
            {
                myLength = value;
            }
        }

        public dynamic Diameter
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myDiameter;
                return returnValue;
            }
            set
            {
                myDiameter = value;
            }
        }

        public dynamic Capacity
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myCapacity;
                return returnValue;
            }
            set
            {
                myCapacity = value;
            }
        }

        public dynamic TankEnds
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myTankEnds;
                return returnValue;
            }
            set
            {
                myTankEnds = value;
            }
        }



        public dynamic Orientation
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myOrientation;
                return returnValue;
            }
            set
            {
                myOrientation = value;
            }
        }


        public dynamic Readingtype
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myReadingtype;
                return returnValue;
            }
            set
            {
                myReadingtype = value;
            }
        }

        public dynamic UseChart
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myUseChart;
                return returnValue;
            }
            set
            {
                myUseChart = value;
            }
        }

        public dynamic LengthUnits
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myLengthUnits;
                return returnValue;
            }
            set
            {
                myLengthUnits = value;
            }
        }

        public dynamic VolumeUnits
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myVolumeUnits;
                return returnValue;
            }
            set
            {
                myVolumeUnits = value;
            }
        }
    }
}
