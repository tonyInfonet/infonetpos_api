using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class PayAtPump : IDisposable
    {

        private DateTime mvarSale_Date;
        private DateTime mvarSale_Time;
        private int mvarSale_Num;
        private string mvarSale_Client;
        private string mvarSale_Type;
        private decimal mvarSale_Line_Disc;
        private decimal mvarSale_Invc_Disc;
        private decimal mvarSale_Amount;
        private decimal mvarSale_Tender;
        private decimal mvarSale_Change;
        private byte mvarRegister;
        private byte mvarTill;
        private string mvarLanguage;
        private string mvarHeader;
        private string mvarFooter;
        private Sale_Lines mvarSale_Lines;
        private Sale_Totals mvarSale_Totals;
        private SP_Prices mvarSP_Prices;
        private Customer mvarCustomer;
        private bool mvarApplyTaxes;

        
        private decimal mvarCouponTotal;
        private string mvarCouponID;
        

        public Customer Customer
        {
            get
            {
                Customer returnValue = default(Customer);
                returnValue = mvarCustomer;
                return returnValue;
            }
            set
            {
                mvarCustomer = value;
            }
        }


        public string Language
        {
            get
            {
                string returnValue = "";
                returnValue = mvarLanguage;
                return returnValue;
            }
            set
            {

                mvarLanguage = value;

            }
        }


        public SP_Prices Prices
        {
            get
            {
                SP_Prices returnValue = default(SP_Prices);
                returnValue = mvarSP_Prices;
                return returnValue;
            }
            set
            {
                mvarSP_Prices = value;
            }
        }


        public string Header
        {
            get
            {
                string returnValue = "";
                returnValue = mvarHeader;
                return returnValue;
            }
            set
            {
                mvarHeader = value;
            }
        }


        public string Footer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarFooter;
                return returnValue;
            }
            set
            {
                mvarFooter = value;
            }
        }


        public Sale_Totals Sale_Totals
        {
            get
            {
                Sale_Totals returnValue = default(Sale_Totals);
                if (mvarSale_Totals == null)
                {
                    mvarSale_Totals = new Sale_Totals();
                }
                returnValue = mvarSale_Totals;
                return returnValue;
            }
            set
            {
                mvarSale_Totals = value;
            }
        }


        public Sale_Lines Sale_Lines
        {
            get
            {
                Sale_Lines returnValue = default(Sale_Lines);
                if (mvarSale_Lines == null)
                {
                    mvarSale_Lines = new Sale_Lines();
                }
                returnValue = mvarSale_Lines;
                return returnValue;
            }
            set
            {
                mvarSale_Lines = value;
            }
        }


        public byte Register
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarRegister;
                return returnValue;
            }
            set
            {
                mvarRegister = value;
            }
        }


        public byte TillNumber
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarTill;
                return returnValue;
            }
            set
            {
                mvarTill = value;
            }
        }

        

        public string Sale_Type
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSale_Type;
                return returnValue;
            }
            set
            {
                mvarSale_Type = value;
            }
        }

        

        public decimal CouponTotal
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarCouponTotal;
                return returnValue;
            }
            set
            {
                mvarCouponTotal = value;
            }
        }


        public string CouponID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCouponID;
                return returnValue;
            }
            set
            {
                mvarCouponID = value;
            }
        }


        public decimal Sale_Change
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Change;
                return returnValue;
            }
            set
            {
                mvarSale_Change = value;
            }
        }


        public decimal Sale_Tender
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Tender;
                return returnValue;
            }
            set
            {
                mvarSale_Tender = value;
            }
        }


        public decimal Sale_Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Amount;
                return returnValue;
            }
            set
            {
                mvarSale_Amount = value;
            }
        }


        public decimal Sale_Invc_Disc
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Invc_Disc;
                return returnValue;
            }
            set
            {
                mvarSale_Invc_Disc = value;
            }
        }


        public decimal Sale_Line_Disc
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSale_Line_Disc;
                return returnValue;
            }
            set
            {
                mvarSale_Line_Disc = value;
            }
        }


        public string Sale_Client
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSale_Client;
                return returnValue;
            }
            set
            {
                mvarSale_Client = value;
            }
        }


        public int Sale_Num
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarSale_Num;
                return returnValue;
            }
            set
            {
                mvarSale_Num = value;
            }
        }


        public DateTime Sale_Date
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarSale_Date;
                return returnValue;
            }
            set
            {
                mvarSale_Date = value;
            }
        }


        public DateTime Sale_Time
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarSale_Time;
                return returnValue;
            }
            set
            {
                mvarSale_Time = value;
            }
        }
        


        public bool ApplyTaxes
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarApplyTaxes;
                return returnValue;
            }
            set
            {
                mvarApplyTaxes = value;
            }
        }

        public string UserCode;

        // Add a new line
        
        
        ///                           Optional ByVal Adjust As Boolean, _
        ///                           Optional ByVal Table_Adjust As Boolean = True) As Boolean


        public void Class_Initialize_Renamed()
        {
            mvarSale_Totals = new Sale_Totals();
            mvarSale_Lines = new Sale_Lines();
            mvarCustomer = new Customer();
            mvarSP_Prices = new SP_Prices();

            mvarSale_Date = DateTime.Now;
            mvarSale_Num = 0;
            mvarSale_Client = "";
            mvarSale_Line_Disc = 0;
            mvarSale_Invc_Disc = 0;
            mvarSale_Amount = 0;
            mvarSale_Tender = 0;
            mvarSale_Change = 0;
            mvarApplyTaxes = true;

        }

        public PayAtPump()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {

            mvarSale_Totals = null;

            mvarSale_Lines = null;

            mvarSP_Prices = null;

            mvarCustomer = null;
        }

        public void Dispose()
        {

            //base.Finalize();
        }
        ~PayAtPump()
        {
            Class_Terminate_Renamed();
        }


        
        // Add or Subtract taxes from a line in the Sale Tax totals. - new function from POS
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        


    }
}
