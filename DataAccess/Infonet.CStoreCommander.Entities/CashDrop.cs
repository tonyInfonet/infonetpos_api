using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class CashDrop
    {
        short cntDrop;
        //Smriti:3 removed UI labels
        //private System.Windows.Forms.Label mvarTopLeft;
        //private System.Windows.Forms.Label mvarTopRight;
        private string mvarEnvelope_No;
        private string mvarreasonCode;
        private int mvarDropID;

        //public System.Windows.Forms.Label TopRight
        //{
        //    set
        //    {
        //        mvarTopRight = value;
        //        mvarTopRight.Text = "";
        //    }
        //}

        public CashDrop()
        {
        }

        //public System.Windows.Forms.Label TopLeft
        //{
        //    set
        //    {
        //        mvarTopLeft = value;
        //        mvarTopLeft.Text = Chaps_Main.GetResString((short)257); //"Tender Drop"
        //    }
        //}

        // - Envelope Number

        public string Envelope_No
        {
            get
            {
                string returnValue = "";
                returnValue = mvarEnvelope_No;
                return returnValue;
            }
            set
            {
                mvarEnvelope_No = value;
            }
        }
        //End - SV
        //  - ATMSafe- reasoncode


        public string ReasonCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarreasonCode;
                return returnValue;
            }
            set
            {
                mvarreasonCode = value;
            }
        }


        public int DropID
        {
            get // 
            {
                int returnValue = 0;
                returnValue = mvarDropID;
                return returnValue;
            }
            set
            {
                mvarDropID = value;
            }
        }

        public void PrintDrop(Tenders Tenders, bool PrintIt)
        {
            // Smriti move this code to manager
            //dynamic Store_Renamed = default(dynamic);
            //dynamic Policy_Renamed = default(dynamic);
            //dynamic Till_Renamed = default(dynamic);
            //dynamic CashDrop_Renamed = default(dynamic);
            //short nH = 0;
            //string FileName = "";
            //short H_Width = 0;
            //decimal S_Amount = new decimal();
            //decimal S_Value = new decimal();
            //string Just = "";
            //ADODB.Recordset rsdrop = default(ADODB.Recordset);
            //ADODB.Recordset rs = new ADODB.Recordset();
            ////  -   adding the unique dropid (max drop number)
            //rs = _dbService.GetRecords("Select max(DropID) as DropID from DropHeader", DataSource.CSCTills);
            //if (rs.EOF)
            //{
            //    CashDrop_Renamed.DropID = 0;
            //}
            //else
            //{
            //    CashDrop_Renamed.DropID = (Information.IsDBNull(rs.Fields["DropID"].Value)) ? 0 : (System.Convert.ToInt32(rs.Fields["DropID"].Value) + 1);
            //}
            //if (CashDrop_Renamed.DropID == 0)
            //{
            //    rs = _dbService.GetRecords("Select max(DropID) as DropID from DropHeader", DataSource.CSCTrans);
            //    if (rs.EOF)
            //    {
            //        CashDrop_Renamed.DropID = 1;
            //    }
            //    else
            //    {
            //        CashDrop_Renamed.DropID = (Information.IsDBNull(rs.Fields["DropID"].Value)) ? 1 : (System.Convert.ToInt32(rs.Fields["DropID"].Value) + 1);
            //    }
            //}
            //rs = null;
            //// 

            //cntDrop = (short)0;

            //rsdrop = _dbService.GetRecords("select max(dropcount) as [maxCnt] from DropHeader where " + " TILL_NUM = " + Till_Renamed.Number + " and Shiftdate = \'" + Till_Renamed.ShiftDate.ToString("yyyyMMdd") + "\'" + " and shiftid = " + Till_Renamed.Shift, DataSource.CSCTills, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly);
            //if (rsdrop.EOF)
            //{
            //    cntDrop++;
            //}
            //else
            //{
            //    cntDrop = System.Convert.ToInt16((Information.IsDBNull(rsdrop.Fields["maxcnt"].Value)) ? 1 : (System.Convert.ToInt32(rsdrop.Fields["maxcnt"].Value) + 1));
            //}

            //FileName = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\CashDrop_Renamed.txt";
            //nH = (short)(FileSystem.FreeFile());
            //H_Width = (short)40;
            //S_Amount = 0;
            //S_Value = 0;
            //Just = Strings.Left(System.Convert.ToString(Policy_Renamed.REC_JUSTIFY), 1).ToUpper(); // Header Justification

            //FileSystem.FileOpen(nH, FileName, OpenMode.Output);
            //// shiny added on Apr11.2005- Header info

            ////  - store code printing should be based on policy - Gasking will enter store code as part of store name - so they don't want to see store code in the beginning
            ////   If Policy_Renamed.PRN_CO_NAME Then Print #nH, PadIt(Just, Store_Renamed.Code & "  " & Store_Renamed.Name, H_Width)
            //if (Policy_Renamed.PRN_CO_NAME)
            //{
            //    FileSystem.PrintLine(nH, modStringPad.PadIt(Just, (Policy_Renamed.PRN_CO_CODE ? Store_Renamed.Code + "  " : "") + Store_Renamed.Name, H_Width));
            //}
            //// 

            //if (Policy_Renamed.PRN_CO_ADDR)
            //{
            //    FileSystem.PrintLine(nH, modStringPad.PadIt(Just, System.Convert.ToString(Store_Renamed.Address.Street1), H_Width));
            //    if (Store_Renamed.Address.Street2 != "")
            //    {
            //        FileSystem.PrintLine(nH, modStringPad.PadIt(Just, System.Convert.ToString(Store_Renamed.Address.Street2), H_Width));
            //    }
            //    FileSystem.PrintLine(nH, modStringPad.PadIt(Just, Strings.Trim(System.Convert.ToString(Store_Renamed.Address.City)) + ", " + Store_Renamed.Address.ProvState, H_Width) + "\r\n" + modStringPad.PadIt(Just, System.Convert.ToString(Store_Renamed.Address.PostalCode), H_Width));
            //}
            //FileSystem.PrintLine(nH);

            //// 

            //if (Policy_Renamed.SAFEATMDROP) // 
            //{
            //    FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)258), H_Width)); //"CASH DROP RECEIPT"
            //    if (Strings.Trim(System.Convert.ToString(CashDrop_Renamed.ReasonCode)) == "ATM")
            //    {
            //        FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)1272) + "(" + Chaps_Main.GetResString((short)1204) + ":" + CashDrop_Renamed.DropID + ")", H_Width));
            //    }
            //    else
            //    {
            //        FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)1273) + "(" + Chaps_Main.GetResString((short)1204) + ":" + CashDrop_Renamed.DropID + ")", H_Width));
            //    }
            //}
            //else
            //{
            //    FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)258), H_Width)); //"CASH DROP RECEIPT"
            //    FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)1204) + ":" + CashDrop_Renamed.DropID, H_Width));
            //}
            /////    Print #nH, PadC(GetResString(259) & Format(Date, "dd-mmm-yyyy") & GetResString(208) & Format(Now, "hh:nn AMPM"), H_Width)         '"Drop on "," at "
            //FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)259) + DateAndTime.Today.ToString("dd-mmm-yyyy") + Chaps_Main.GetResString((short)208) + DateTime.Now.ToString(Chaps_Main.TimeFormatHM), H_Width)); //"Drop on "," at "  '  
            //                                                                                                                                                                                                                              // 
            //                                                                                                                                                                                                                              //    Print #nH, PadC(GetResString(260) & ": " & User.Code & " - " & User.Name, H_Width)     '"Dropped By
            //if (Policy_Renamed.PRN_UName)
            //{
            //    FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)260) + ": " + Chaps_Main.User_Renamed.Code + " - " + Chaps_Main.User_Renamed.Name, H_Width)); //"Dropped By
            //}
            //else
            //{
            //    FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)260) + ": " + Chaps_Main.User_Renamed.Code, H_Width)); //"Dropped By
            //}
            //// 
            //FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)132) + ": " + System.Convert.ToString(Chaps_Main.Register_Renamed.Register_Num), H_Width)); //Register
            //FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)131) + ": " + Till_Renamed.Number, H_Width)); //Till
            //FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)389) + ": " + Till_Renamed.ShiftDate, H_Width)); //Shiftdate
            //FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)346) + ": " + Till_Renamed.Shift, H_Width)); //Shift id
            //                                                                                                                       //
            //if (Policy_Renamed.DropEnv == true)
            //{
            //    FileSystem.PrintLine(nH, modStringPad.PadC(Chaps_Main.GetResString((short)4703) + " " + CashDrop_Renamed.Envelope_No, H_Width)); //Envelope Number
            //}
            ////End - SV

            //FileSystem.PrintLine(nH);
            //FileSystem.PrintLine(nH, Chaps_Main.GetResString((short)390) + ": " + System.Convert.ToString(cntDrop)); //Cntdrop
            //FileSystem.PrintLine(nH);

            //Tender T = default(Tender);
            //FileSystem.PrintLine(nH, modStringPad.PadR(Chaps_Main.GetResString((short)115), (short)20) + modStringPad.PadL(Chaps_Main.GetResString((short)106), (short)10) + modStringPad.PadL(Chaps_Main.GetResString((short)114), (short)10)); //"Tender","Amount","Value"
            //FileSystem.PrintLine(nH, modStringPad.PadR("_", (short)20, "_") + modStringPad.PadL("_", (short)10, "_") + modStringPad.PadL("_", (short)10, "_"));
            //foreach (Tender tempLoopVar_T in Tenders)
            //{
            //    T = tempLoopVar_T;
            //    if (T.Amount_Entered != 0)
            //    {
            //        FileSystem.PrintLine(nH, modStringPad.PadR(T.Tender_Name, (short)20) + modStringPad.PadL(T.Amount_Entered.ToString("#,##0.00"), (short)10) + modStringPad.PadL(T.Amount_Used.ToString("#,##0.00"), (short)10));
            //        S_Amount = S_Amount + T.Amount_Entered;
            //        S_Value = S_Value + T.Amount_Used;
            //    }
            //}
            //FileSystem.PrintLine(nH, modStringPad.PadR("_", (short)20, "_") + modStringPad.PadL("_", (short)10, "_") + modStringPad.PadL("_", (short)10, "_"));
            //FileSystem.PrintLine(nH, modStringPad.PadR(Chaps_Main.GetResString((short)136).ToUpper(), (short)20) + modStringPad.PadL(S_Amount.ToString("#,##0.00"), (short)10) + modStringPad.PadL(S_Value.ToString("#,##0.00"), (short)10)); //"TOTALS"
            //FileSystem.PrintLine(nH);
            //FileSystem.PrintLine(nH);
            //FileSystem.FileClose();
            //Chaps_Main.Last_Printed = "CashDrop_Renamed.txt";

            //
            /////    If PrintIt Then Dump_To_Printer FileName, 1, True 'False  nancy
            //
            //if (PrintIt)
            //{
            //    modPrint.Dump_To_Printer(FileName, System.Convert.ToInt16(Policy_Renamed.CashDropReceiptCopies), true, true, false);
            //}
            //

        }

        public void SaveDrop(Tenders Tenders)
        {
            //Smriti move this code to manager
            //dynamic CashDrop_Renamed = default(dynamic);
            //dynamic Till_Renamed = default(dynamic);
            //dynamic User = default(dynamic);
            //dynamic Policy_Renamed = default(dynamic);
            //Tender T = default(Tender);
            //DateTime Drop_Date = default(DateTime);
            //ADODB.Recordset rh = default(ADODB.Recordset);
            //ADODB.Recordset rl = default(ADODB.Recordset);
            //ADODB.Recordset rs = default(ADODB.Recordset);
            //string BT = "";

            //
            //
            //BT = System.Convert.ToString(Policy_Renamed.BASECURR);
            //
            //rh = _dbService.GetRecords("select * from DropHeader", DataSource.CSCTills, ADODB.CursorTypeEnum.adOpenForwardOnly);
            //rl = _dbService.GetRecords("select * from DropLines", DataSource.CSCTills, ADODB.CursorTypeEnum.adOpenForwardOnly);

            //Drop_Date = DateTime.Now;
            //rh.AddNew();
            //rh.Fields["DropDate"].Value = Drop_Date;
            //rh.Fields["User"].Value = User.Code;
            //rh.Fields["Till_Num"].Value = Till_Renamed.Number;
            //rh.Fields["DropCount"].Value = cntDrop;
            //rh.Fields["shiftid"].Value = Till_Renamed.Shift;
            //rh.Fields["ShiftDate"].Value = Till_Renamed.ShiftDate;
            //rh.Fields["EnvelopeNo"].Value = CashDrop_Renamed.Envelope_No == "" ? System.DBNull.Value : CashDrop_Renamed.Envelope_No;
            //rh.Fields["ReasonCode"].Value = (Information.IsDBNull(CashDrop_Renamed.ReasonCode)) ? "SAFE" : (Strings.Trim(System.Convert.ToString(CashDrop_Renamed.ReasonCode))); // 
            //rh.Fields["DropID"].Value = CashDrop_Renamed.DropID; // 
            //rh.Update();

            //foreach (Tender tempLoopVar_T in Tenders)
            //{
            //    T = tempLoopVar_T;
            //    if (T.Amount_Entered > 0)
            //    {
            //        rl.AddNew();
            //        rl.Fields["Till_Num"].Value = Till_Renamed.Number;
            //        rl.Fields["DropDate"].Value = Drop_Date;
            //        rl.Fields["Tender_Name"].Value = T.Tender_Name;
            //        rl.Fields["Exchange_Rate"].Value = T.Exchange_Rate;
            //        rl.Fields["Amount"].Value = T.Amount_Entered;
            //        rl.Fields["Conv_Amount"].Value = T.Amount_Used;
            //        rl.Fields["DropID"].Value = CashDrop_Renamed.DropID; // 
            //        rl.Update();
            //        if (T.Tender_Name.ToUpper() == BT.ToUpper())
            //        {
            //            rs = _dbService.GetRecords("Select *  FROM   Tills  WHERE  Tills.Till_Num = " + Till_Renamed.Number + " ", DataSource.CSCMaster, ADODB.CursorTypeEnum.adOpenForwardOnly);
            //            rs.Fields["Cash"].Value = rs.Fields["Cash"].Value - T.Amount_Entered;
            //            rs.Update();
            //            rs = null;
            //        }
            //        //  track the cash bonus
            //        if (Policy_Renamed.Use_CashBonus)
            //        {
            //            if (T.Tender_Name.ToUpper() == modGlobalFunctions.Get_TenderName(System.Convert.ToString(Policy_Renamed.CBonusTend)).ToUpper())
            //            {
            //                rs = _dbService.GetRecords("Select *  FROM   Tills  WHERE  Tills.Till_Num = " + Till_Renamed.Number + " ", DataSource.CSCMaster, ADODB.CursorTypeEnum.adOpenForwardOnly);
            //                rs.Fields["CashBonus"].Value = rs.Fields["CashBonus"].Value - T.Amount_Entered;
            //                rs.Update();
            //                rs = null;
            //            }
            //        }
            //        //Shiny end
            //    }
            //}
            //rh = null;
            //rl = null;
        }
    }
}
