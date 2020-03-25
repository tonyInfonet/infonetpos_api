using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Till
    {

        private short mvarNumber;
        private bool mvarActive;

        private bool mvarProcessing;
        private decimal mvarFloat;
        private decimal mvarCash;
        // Nicolette added next 4 lines for TillAudit
        private decimal mvarChange;
        private decimal mvarDraws;
        private decimal mvarPayouts;
        private decimal mvarOverPayment;
        private DateTime mvarDate_Open;
        private DateTime mvarTime_Open;
        //Shiny added the following for store shifts
        private short mvarShift;
        private DateTime mvarShiftDate;
        private string mvarUserLoggedOn;
        private int Handle;
        private decimal mvarBonusFloat; //  Cash Bonus
        private decimal mvarBonusDraw; //  - Cash bonus Draw
        private decimal mvarBonusGiveaway;

        //public void Update_Cash(decimal vData)
        //{
        //    //Smriti move this code to manager
        //    //dynamic Policy_Renamed = default(dynamic);

        //    //ADODB.Recordset rsTill = default(ADODB.Recordset);
        //    //short Limit = 0; //binal

        //    //rsTill = Chaps_Main.Get_Records("Select * From Tills Where Tills.Till_Num = " + System.Convert.ToString(this.Number), Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly);

        //    //if (!rsTill.EOF)
        //    //{
        //    //    rsTill.Fields["Cash"].Value = rsTill.Fields["Cash"].Value + vData;
        //    //    rsTill.Update();
        //    //}

        //    ////BINAL SEPT 25,2002-TO CLEAR TILL HAVING BIGGER AMOUNT
        //    //
        //    //
        //    //
        //    //if (Policy_Renamed.USE_CL_TILL)
        //    //{
        //    //    Limit = System.Convert.ToInt16(Policy_Renamed.CLEAR_TILL);
        //    //    
        //    //    if (System.Convert.ToInt32(rsTill.Fields["Cash"].Value) > Limit)
        //    //    {
        //    //        
        //    //        
        //    //        
        //    //        ///                Call DisplayMsgForm("PLEASE CLEAR THE TILL ! " & _
        //    //        ///                             "AMOUNT EXCEEDED THE LIMIT OF $" & limit, 99)
        //    //        
        //    //        ///                Handle = frmMsgBoxR.hWnd
        //    //        ///                Do While IsWindow(Handle)
        //    //        ///                    DoEvents
        //    //        ///                Loop
        //    //        
        //    //        
        //    //        ///                Call DisplayMsgForm("WARNING: PLEASE CLEAR THE TILL !  AMOUNT EXCEEDED THE LIMIT OF $" & limit, 99)
        //    //        
        //    //        ///                Handle = frmMsgBox.hWnd
        //    //        ///                Do While IsWindow(Handle)
        //    //        ///                    DoEvents
        //    //        ///                Loop
        //    //        
        //    //        MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Information + MsgBoxStyle.OkOnly;
        //    //        Chaps_Main.DisplayMessage(0, (short)8309, temp_VbStyle, Limit, (byte)22);
        //    //        
        //    //        
        //    //    }
        //    //}

        //    //rsTill = null;

        //}

        //public void Update_CashBonus(decimal vData)
        //{
        //    //Smriti move this code to manager
        //    //ADODB.Recordset rsTill = default(ADODB.Recordset);

        //    //rsTill = Chaps_Main.Get_Records("Select * From Tills Where Tills.Till_Num = " + System.Convert.ToString(this.Number), Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly);

        //    //if (!rsTill.EOF)
        //    //{
        //    //    rsTill.Fields["CashBonus"].Value = rsTill.Fields["CashBonus"].Value + vData;
        //    //    rsTill.Update();
        //    //}


        //    //rsTill = null;

        //}
        public void Save()
        {
        }


        public DateTime Time_Open
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarTime_Open;
                return returnValue;
            }
            set
            {
                mvarTime_Open = value;
            }
        }


        public DateTime Date_Open
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarDate_Open;
                return returnValue;
            }
            set
            {
                mvarDate_Open = value;
            }
        }


        public decimal Cash
        {
            get
            {
                return mvarCash;
            }
            set
            {
                mvarCash = value;
            }
        }


        public decimal Float
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarFloat;
                return returnValue;
            }
            set
            {
                mvarFloat = value;
            }
        }

        //

        public string UserLoggedOn
        {
            get
            {
                string returnValue = "";
                returnValue = mvarUserLoggedOn;
                return returnValue;
            }
            set
            {
                mvarUserLoggedOn = value;
            }
        }
        //End - SV


        // Nicolette added next proprieties to use in AuditTill frm

        public decimal Change
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarChange;
                return returnValue;
            }
            set
            {
                mvarChange = value;
            }
        }


        public decimal Draws
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarDraws;
                return returnValue;
            }
            set
            {
                mvarDraws = value;
            }
        }


        public decimal Payouts
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPayouts;
                return returnValue;
            }
            set
            {
                mvarPayouts = value;
            }
        }


        public decimal OverPayment
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarOverPayment;
                return returnValue;
            }
            set
            {
                mvarOverPayment = value;
            }
        }

        // Nicolette end


        public bool Processing
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarProcessing;
                return returnValue;
            }
            set
            {
                mvarProcessing = value;
            }
        }


        public bool Active
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarActive;
                return returnValue;
            }
            set
            {
                mvarActive = value;
            }
        }


        public short Number
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarNumber;
                return returnValue;
            }
            set
            {
                mvarNumber = value;
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
        //  Cash bonus Program


        public decimal BonusFloat
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBonusFloat;
                return returnValue;
            }
            set
            {
                mvarBonusFloat = value;
            }
        }

        public decimal BonusDraw
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBonusDraw;
                return returnValue;
            }
            set
            {
                mvarBonusDraw = value;
            }
        }

        public decimal BonusGiveAway
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBonusGiveaway;
                return returnValue;
            }
            set
            {
                mvarBonusGiveaway = value;
            }
        }


        private void Class_Initialize_Renamed()
        {
            mvarNumber = (short)0;
        }
        public Till()
        {
            Class_Initialize_Renamed();
        }
        public int POSId { get; set; }
        public decimal CashBonus{ get; set; }
        // 
    }
}
