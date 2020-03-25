using System;

namespace Infonet.CStoreCommander.Entities
{
    public class TaxCreditLine: IDisposable
    {

        private Line_Taxes mvarLine_Taxes;
        private short mvarLine_Num;

        private void Class_Initialize_Renamed()
        {
            mvarLine_Taxes = new Line_Taxes();
        }
        public TaxCreditLine()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mvarLine_Taxes = null;
        }

        public void Dispose()
        {
            
        }

        ~TaxCreditLine()
        {
            Class_Terminate_Renamed();
        }

        public Line_Taxes Line_Taxes
        {
            get
            {
                Line_Taxes returnValue = default(Line_Taxes);
                if (mvarLine_Taxes == null)
                {
                    mvarLine_Taxes = new Line_Taxes();
                }

                returnValue = mvarLine_Taxes;
                return returnValue;
            }
            set
            {
                mvarLine_Taxes = value;
            }
        }


        public short Line_Num
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLine_Num;
                return returnValue;
            }
            set
            {
                mvarLine_Num = value;
            }
        }
    }
}
