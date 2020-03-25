using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class BR_Payment
    {

        private int mvarSale_Num;
        private decimal mvarAmount;
        private BottleReturns mvarBr_Lines;
        private decimal mvarPenny_Adj; //  

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        ///    mvarTopLeft.Caption = "Bottle Return : " & vbCrLf
        
        
        
        
        


        public BottleReturns Br_Lines
        {
            get
            {
                BottleReturns returnValue = default(BottleReturns);
                if (mvarBr_Lines == null)
                {
                    mvarBr_Lines = new BottleReturns();
                }
                returnValue = mvarBr_Lines;
                return returnValue;
            }
            set
            {
                mvarBr_Lines = value;
            }
        }


        public decimal Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
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


        public decimal Penny_Adj
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPenny_Adj;
                return returnValue;
            }
            set
            {
                mvarPenny_Adj = value;
            }
        }

        public void Add_a_Line(BottleReturn oLine)
        {
            mvarBr_Lines.AddLine((short)(this.Br_Lines.Count+1), oLine, "");
            mvarAmount = mvarAmount + oLine.Amount;
        }

        private void Class_Initialize()
        {
            mvarBr_Lines = new BottleReturns();
            mvarPenny_Adj = 0;
        }
        public BR_Payment()
        {
            Class_Initialize();
        }

        private void Class_Terminate()
        {
            mvarBr_Lines = null;
        }

        public void Dispose()
        {
           
        }

        ~BR_Payment()
        {
            Class_Terminate();
        }

        // Smriti:1 Addded a new property
        public int TillNumber { get; set; }

        public byte RegisterNumber { get; set; }
    }
}
