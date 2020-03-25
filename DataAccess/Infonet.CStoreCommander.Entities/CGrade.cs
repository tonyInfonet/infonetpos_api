namespace Infonet.CStoreCommander.Entities
{
    public class CGrade
    {
        private object myShortName;
        private object myFuelType;
        private object myFullName;
        private object myStock_Code;
        //Private myCommodity As Variant
        private object mySubDetail;
        private object mySubDept;
        private object myRatio;
        private object myPrice_diff;
        private object myFeedStock;
        private object myStockGradeID;


        public dynamic FuelType
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myFuelType;
                return returnValue;
            }
            set
            {
                myFuelType = value;
            }
        }


        public dynamic ShortName
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myShortName;
                return returnValue;
            }
            set
            {
                myShortName = value;
            }
        }


        public dynamic FullName
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myFullName;
                return returnValue;
            }
            set
            {
                myFullName = value;
            }
        }


        public dynamic Stock_Code
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myStock_Code;
                return returnValue;
            }
            set
            {
                myStock_Code = value;
            }
        }

        //Public Property Get Commodity() As Variant
        //Commodity = myCommodity
        //End Property
        //
        //Public Property Let Commodity(ByVal vNewValue As Variant)
        //myCommodity = vNewValue
        //End Property


        public dynamic SubDept
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = mySubDept;
                return returnValue;
            }
            set
            {
                mySubDept = value;
            }
        }


        public dynamic SubDetail
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = mySubDetail;
                return returnValue;
            }
            set
            {
                mySubDetail = value;
            }
        }


        public dynamic Ratio
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myRatio;
                return returnValue;
            }
            set
            {
                myRatio = value;
            }
        }


        public dynamic FeedStock
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myFeedStock;
                return returnValue;
            }
            set
            {
                myFeedStock = value;
            }
        }

        public dynamic Price_diff
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPrice_diff;
                return returnValue;
            }
            set
            {
                myPrice_diff = value;
            }
        }


        public dynamic StockGradeID
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myStockGradeID;
                return returnValue;
            }
            set
            {
                myStockGradeID = value;
            }
        }
    }
}
