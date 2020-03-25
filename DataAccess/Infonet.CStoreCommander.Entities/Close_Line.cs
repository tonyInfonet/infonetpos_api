
namespace Infonet.CStoreCommander.Entities
{
    public class Close_Line
    {

        private int mvarSequence;
        private string mvarTender_Name;
        private string mvarShort_Name;
        private string mvarTender_Class;
        private short mvarTender_Count;
        private decimal mvarEntered;
        private decimal mvarSystem;
        private decimal mvarBalance;
        private double mvarExchange_Rate;
        private decimal mvarConverted_Entered;
        private decimal mvarConverted_System;
        private decimal mvarConverted_Balance;



        public double Exchange_Rate
        {
            get
            {
                double returnValue = 0;
                returnValue = mvarExchange_Rate;
                return returnValue;
            }
            set
            {
                mvarExchange_Rate = value;
            }
        }



        public decimal Converted_Balance
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarConverted_Balance;
                return returnValue;
            }
            set
            {
                mvarConverted_Balance = value;
            }
        }





        public decimal Converted_System
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarConverted_System;
                return returnValue;
            }
            set
            {
                mvarConverted_System = value;
            }
        }





        public decimal Converted_Entered
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarConverted_Entered;
                return returnValue;
            }
            set
            {
                mvarConverted_Entered = value;
            }
        }





        public decimal Balance
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBalance;
                return returnValue;
            }
            set
            {
                mvarBalance = value;
            }
        }





        public decimal System
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSystem;
                return returnValue;
            }
            set
            {
                mvarSystem = value;
            }
        }





        public decimal Entered
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarEntered;
                return returnValue;
            }
            set
            {
                mvarEntered = value;
            }
        }





        public short Tender_Count
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarTender_Count;
                return returnValue;
            }
            set
            {
                mvarTender_Count = value;
            }
        }





        public string Tender_Class
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTender_Class;
                return returnValue;
            }
            set
            {
                mvarTender_Class = value;
            }
        }





        public string Short_Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarShort_Name;
                return returnValue;
            }
            set
            {
                mvarShort_Name = value;
            }
        }





        public string Tender_Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTender_Name;
                return returnValue;
            }
            set
            {
                mvarTender_Name = value;
            }
        }





        public int Sequence
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarSequence;
                return returnValue;
            }
            set
            {
                mvarSequence = value;
            }
        }
    }
}
