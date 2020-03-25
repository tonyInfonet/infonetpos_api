using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class TaxExemptSale : IDisposable
    {


        private TaxExemptSaleLines mvarTe_Sale_Lines;
        private int mvarSale_Num;
        private byte mvarTill;
        private string mvarUserCode;
        private bool mvarGasOverLimit;
        private bool mvarPropaneOverLimit;
        private bool mvarTobaccoOverLimit;
        private string mvarGasReason;
        private string mvarPropaneReason;
        private string mvarTobaccoReason;
        private string mvarGasReasonDesp;
        private string mvarPropaneReasonDesp;
        private string mvarTobaccoReasonDesp;
        private string mvarGasReasonDetail;
        private string mvarPropaneReasonDetail;
        private string mvarTobaccoReasonDetail;
        private teCardholder mvarTeCardholder;
        private float mvarAmount;
        private float mvarTotalExemptedTax;
        private DateTime mvarShiftDate;
        private DateTime mvarSale_Time;
        private float mvarTotalExemptTobacco;
        private float mvarTotalExemptGas;
        private float mvarTotalExemptPropane;
        private bool mvarHasTobacco;
        private short mvarShift;
        private bool mvarHasUpdatedLine;
        private Sale_Taxes mvarTaxCredit; 
        private TaxCreditLines mvarTaxCreditLines; 

        // Delete a line from the sale
        public void Remove_TE_Line(short LineNum)
        {
            mvarTe_Sale_Lines.Remove(LineNum);
            CalculateTotal(true, true);
            ReduceItemID(LineNum); 
        }

        
        
        private void ReduceItemID(short ItemFrom)
        {
            TaxExemptSaleLine TESL = default(TaxExemptSaleLine);

            foreach (TaxExemptSaleLine tempLoopVar_TESL in this.Te_Sale_Lines)
            {
                TESL = tempLoopVar_TESL;
                if (TESL.ItemID >= ItemFrom)
                {
                    TESL.ItemID--;
                }
            }
        }
        

        public bool Add_a_Line(TaxExemptSaleLine oLine, ref bool CheckOverLimit, ref bool CheckQuota)
        {
            bool returnValue = false;
          
            returnValue = false;

            if (oLine.Quantity == 0)
            {
                return returnValue;
            }

            var teLineNumber = mvarTe_Sale_Lines.Count + 1;

            mvarTe_Sale_Lines.AddLine((short)(mvarTe_Sale_Lines.Count + 1), oLine, teLineNumber.ToString());

            CalculateTotal(CheckOverLimit, CheckQuota);

            returnValue = true;

            return returnValue;
        }

        
        
        public void CalculateTotal(bool CheckOverLimit, bool CheckQuota)
        {
            float GasRunningQuota = 0;
            float PropaneRunningQuota = 0;
            float TobaccoRunningQuota = 0;
            TaxExemptSaleLine TESL = default(TaxExemptSaleLine);
            float mAmount = 0;
            float mExemptedTax = 0;
            float mTotalExemptTobacco = 0;
            float mTotalExemptGas = 0;
            float mTotalExemptPropane = 0;
            bool mHasTobacco = false;
            
            float mTotalTobacco = 0;
            float mTotalGas = 0;
            float mTotalPropane = 0;

            mTotalTobacco = 0;
            mTotalGas = 0;
            mTotalPropane = 0;
            

            mAmount = 0;
            mExemptedTax = 0;
            mTotalExemptTobacco = 0;
            mTotalExemptGas = 0;
            mTotalExemptPropane = 0;
            mHasTobacco = false;

            GasRunningQuota = this.teCardholder.GasQuota;
            PropaneRunningQuota = this.teCardholder.PropaneQuota;
            TobaccoRunningQuota = this.teCardholder.TobaccoQuota;
            if (CheckOverLimit)
            {
                mvarGasOverLimit = false;
                mvarPropaneOverLimit = false;
                mvarTobaccoOverLimit = false;
            }

            foreach (TaxExemptSaleLine tempLoopVarTesl in mvarTe_Sale_Lines)
            {
                TESL = tempLoopVarTesl;
                mAmount = mAmount + TESL.Amount;
                mExemptedTax = mExemptedTax + TESL.ExemptedTax;
                
                
                if (CheckOverLimit)
                {
                    TESL.OverLimit = false; 
                }
                if (((TESL.ProductType == mPrivateGlobals.teProductEnum.eCigarette) || (TESL.ProductType == mPrivateGlobals.teProductEnum.eCigar)) || (TESL.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
                {
                    mTotalTobacco = mTotalTobacco + TESL.Amount; 
                    mTotalExemptTobacco = mTotalExemptTobacco + TESL.ExemptedTax;
                    mHasTobacco = true;
                    TobaccoRunningQuota = TobaccoRunningQuota + TESL.ExemptedTax;
                    
                    if (CheckQuota)
                    {
                        
                        TESL.RunningQuota = TobaccoRunningQuota;
                    }
                    
                    
                    if (CheckOverLimit && TobaccoRunningQuota > mPrivateGlobals.theSystem.TobaccoLimit)
                    {
                        mvarTobaccoOverLimit = true;
                        
                        if (TESL.Amount > 0)
                        {
                            
                            TESL.OverLimit = true;
                        } 
                    }
                } // hen
                else if ((((TESL.ProductType == mPrivateGlobals.teProductEnum.eGasoline) || (TESL.ProductType == mPrivateGlobals.teProductEnum.eDiesel)) || (TESL.ProductType == mPrivateGlobals.teProductEnum.emarkedGas)) || (TESL.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel))
                {

                    mTotalGas = mTotalGas + TESL.Amount; 
                    mTotalExemptGas = mTotalExemptGas + TESL.ExemptedTax;
                    GasRunningQuota = GasRunningQuota + TESL.ExemptedTax;
                    
                    if (CheckQuota)
                    {
                        
                        TESL.RunningQuota = GasRunningQuota;
                    }
                    
                    
                    if (CheckOverLimit && GasRunningQuota > mPrivateGlobals.theSystem.GasLimit)
                    {
                        mvarGasOverLimit = true;
                        
                        if (TESL.Amount > 0)
                        {
                            
                            TESL.OverLimit = true;
                        } 
                    }
                }
                else if (TESL.ProductType == mPrivateGlobals.teProductEnum.ePropane)
                {
                    mTotalPropane = mTotalPropane + TESL.Amount; 
                    mTotalExemptPropane = mTotalExemptPropane + TESL.ExemptedTax;
                    PropaneRunningQuota = PropaneRunningQuota + TESL.ExemptedTax;
                    
                    if (CheckQuota)
                    {
                        
                        TESL.RunningQuota = PropaneRunningQuota;
                    }
                    
                    
                    if (CheckOverLimit && PropaneRunningQuota > mPrivateGlobals.theSystem.PropaneLimit)
                    {
                        mvarPropaneOverLimit = true;
                        
                        if (TESL.Amount > 0)
                        {
                            
                            TESL.OverLimit = true;
                        } 
                    }
                }
            }

            
            
            if (CheckOverLimit)
            {
                if (mvarTobaccoOverLimit && mTotalTobacco < 0)
                {
                    mvarTobaccoOverLimit = false;
                }
                if (mvarGasOverLimit && mTotalGas < 0)
                {
                    mvarGasOverLimit = false;
                }
                if (mvarPropaneOverLimit && mTotalPropane < 0)
                {
                    mvarPropaneOverLimit = false;
                }
            }
            

            mvarHasTobacco = mHasTobacco;
            mvarAmount = mAmount;
            mvarTotalExemptedTax = mExemptedTax;
            mvarTotalExemptTobacco = mTotalExemptTobacco;
            mvarTotalExemptGas = mTotalExemptGas;
            mvarTotalExemptPropane = mTotalExemptPropane;
            TESL = null;
        }


        public TaxExemptSaleLines Te_Sale_Lines
        {
            get
            {
                TaxExemptSaleLines returnValue = default(TaxExemptSaleLines);
                if (mvarTe_Sale_Lines == null)
                {
                    mvarTe_Sale_Lines = new TaxExemptSaleLines();
                }
                returnValue = mvarTe_Sale_Lines;
                return returnValue;
            }
            set
            {
                mvarTe_Sale_Lines = value;
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


        public string UserCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUserCode;
                return returnValue;
            }
            set
            {
                mvarUserCode = value;
            }
        }


        public bool HasUpdatedLine
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarHasUpdatedLine;
                return returnValue;
            }
            set
            {
                mvarHasUpdatedLine = value;
            }
        }

        public bool HasTobacco
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarHasTobacco;
                return returnValue;
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


        public DateTime ShiftDate
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarShiftDate;
                return returnValue;
            }
            set
            {
                mvarShiftDate = value;
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


        public teCardholder teCardholder
        {
            get
            {
                teCardholder returnValue = default(teCardholder);
                returnValue = mvarTeCardholder;
                return returnValue;
            }
            set
            {
                mvarTeCardholder = value;
            }
        }


        public short Shift
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarShift;
                return returnValue;
            }
            set
            {
                mvarShift = value;
            }
        }


        public float Amount
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
            }
        }


        public float TotalExemptedTax
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTotalExemptedTax;
                return returnValue;
            }
            set
            {
                mvarTotalExemptedTax = value;
            }
        }


        public bool GasOverLimit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarGasOverLimit;
                return returnValue;
            }
            set
            {
                mvarGasOverLimit = value;
            }
        }


        public bool PropaneOverLimit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPropaneOverLimit;
                return returnValue;
            }
            set
            {
                mvarPropaneOverLimit = value;
            }
        }


        public bool TobaccoOverLimit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTobaccoOverLimit;
                return returnValue;
            }
            set
            {
                mvarTobaccoOverLimit = value;
            }
        }


        public string GasReason
        {
            get
            {
                string returnValue = "";
                returnValue = mvarGasReason;
                return returnValue;
            }
            set
            {
                mvarGasReason = value;
            }
        }


        public string PropaneReason
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPropaneReason;
                return returnValue;
            }
            set
            {
                mvarPropaneReason = value;
            }
        }


        public string TobaccoReason
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTobaccoReason;
                return returnValue;
            }
            set
            {
                mvarTobaccoReason = value;
            }
        }


        public string GasReasonDesp
        {
            get
            {
                string returnValue = "";
                returnValue = mvarGasReasonDesp;
                return returnValue;
            }
            set
            {
                mvarGasReasonDesp = value;
            }
        }


        public string PropaneReasonDesp
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPropaneReasonDesp;
                return returnValue;
            }
            set
            {
                mvarPropaneReasonDesp = value;
            }
        }


        public string TobaccoReasonDesp
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTobaccoReasonDesp;
                return returnValue;
            }
            set
            {
                mvarTobaccoReasonDesp = value;
            }
        }


        public string GasReasonDetail
        {
            get
            {
                string returnValue = "";
                returnValue = mvarGasReasonDetail;
                return returnValue;
            }
            set
            {
                mvarGasReasonDetail = value;
            }
        }


        public string PropaneReasonDetail
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPropaneReasonDetail;
                return returnValue;
            }
            set
            {
                mvarPropaneReasonDetail = value;
            }
        }


        public string TobaccoReasonDetail
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTobaccoReasonDetail;
                return returnValue;
            }
            set
            {
                mvarTobaccoReasonDetail = value;
            }
        }

        public float TotalExemptTobacco
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTotalExemptTobacco;
                return returnValue;
            }
        }

        public float TotalExemptGas
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTotalExemptGas;
                return returnValue;
            }
        }

        public float TotalExemptPropane
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTotalExemptPropane;
                return returnValue;
            }
        }

        

        public Sale_Taxes TaxCredit
        {
            get
            {
                Sale_Taxes returnValue = default(Sale_Taxes);
                if (mvarTaxCredit == null)
                {
                    mvarTaxCredit = new Sale_Taxes();
                }
                returnValue = mvarTaxCredit;
                return returnValue;
            }
            set
            {
                mvarTaxCredit = value;
            }
        }


        public TaxCreditLines TaxCreditLines
        {
            get
            {
                TaxCreditLines returnValue = default(TaxCreditLines);
                if (mvarTaxCreditLines == null)
                {
                    mvarTaxCreditLines = new TaxCreditLines();
                }
                returnValue = mvarTaxCreditLines;
                return returnValue;
            }
            set
            {
                mvarTaxCreditLines = value;
            }
        }

        private void Class_Initialize_Renamed()
        {
            InitProperties();
        }
        public TaxExemptSale()
        {
            Class_Initialize_Renamed();
        }

        private void InitProperties()
        {
            mvarSale_Num = 0;
            mvarTill = (byte)0;
            mvarShift = (short)0;
            mvarGasOverLimit = false;
            mvarPropaneOverLimit = false;
            mvarHasUpdatedLine = false;
            mvarHasTobacco = false;
            mvarTobaccoOverLimit = false;
            mvarTotalExemptTobacco = 0;
            mvarTotalExemptGas = 0;
            mvarTotalExemptPropane = 0;
            mvarAmount = 0;
            mvarTotalExemptedTax = 0;

            Te_Sale_Lines = new TaxExemptSaleLines();
            mvarTeCardholder = new teCardholder();

            mvarTaxCredit = new Sale_Taxes(); 
            mvarTaxCreditLines = new TaxCreditLines(); 
        }

        private void Class_Terminate_Renamed()
        {
            mvarTe_Sale_Lines = null;
            mvarTeCardholder = null;
            mvarTaxCredit = null; 
            mvarTaxCreditLines = null; 
        }
        public void Dispose()
        {
           
        }

        ~TaxExemptSale()
        {
            Class_Terminate_Renamed();
        }
        
    }
}
