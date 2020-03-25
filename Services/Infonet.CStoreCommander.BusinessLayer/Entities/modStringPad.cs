using Microsoft.VisualBasic;
using System;


namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public sealed class modStringPad
    {
        private static System.Windows.Forms.Control PComLCD;
        //Replaced Output with text'
        // Apply Justification 'Just' to a string 'S' to make a string of length 'L'
        // using 'P' as the pad character.
        public static string PadIt(string Just, string s, short L, string P = "")
        {
            string returnValue = "";

            switch (Just.ToUpper())
            {
                case "C":
                    returnValue = PadC(s, L, P);
                    break;
                case "L":
                    returnValue = PadR(s, L, P);
                    break;
                case "R":
                    returnValue = PadL(s, L, P);
                    break;
                default:
                    returnValue = s;
                    break;
            }

            return returnValue;
        }

        // Right Justify string S in a Field L characters long.
        public static string Right_Justify(string s, short L)
        {
            string returnValue = "";
            returnValue = PadL(s, L);
            return returnValue;
        }

        // Left Justify string S in a Field L characters long.
        public static string Left_Justify(string s, short L)
        {
            string returnValue = "";
            returnValue = PadR(s, L);
            return returnValue;
        }

        // Left Pad a string 'S' with zeros to make a string of length 'L'
        public static string PadLZ(string s, short L)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = "";
            }
            string returnValue = "";
            returnValue = PadL(s.Trim(), L, "0");
            return returnValue;
        }

        // Right pad a string 'S' with character 'P' to make a string of length 'L'.
        public static string PadR(string s, short L, string P = " ")
        {
            if (string.IsNullOrEmpty(s))
            {
                s = "";
            } 

            string returnValue = "";

            string C = "";

            returnValue = "";
            if (L <= 0)
            {
                return returnValue;
            }

            C = Convert.ToString((P.Length == 0) ? " " : P);

            s = s.TrimEnd();

            if (s.Length >= L)
            {
                returnValue = s.Substring(0, L);
            }
            else
            {
                returnValue = s + Repl((short)(L - s.Length), C);
            }

            return returnValue;
        }

        // Left Pad a string 'S' to make a string of length 'L' using char 'P'
        public static string PadL(string s, short L, string P = " ")
        {
            if (string.IsNullOrEmpty(s))
            {
                s = "";
            }
            string returnValue = "";

            string C = "";

            returnValue = "";
            if (L <= 0)
            {
                return returnValue;
            }

            C = Convert.ToString((P.Length == 0) ? " " : P);

            s = s.TrimStart();
            if (s.Length >= L)
            {
                returnValue = s.Substring(s.Length - L, L);
            }
            else
            {
                returnValue = Repl((short)(L - s.Length), C) + s;
            }

            return returnValue;
        }

        // Centre a character string 'S' in a string 'L' long padded with character 'P'.
        public static string PadC(string s, short L, string P = " ")
        {
            if (string.IsNullOrEmpty(s))
            {
                s = "";
            }
            string returnValue = "";

            short n = 0;
            short nL = 0;
            short nR = 0;
            string C = "";

            returnValue = "";
            if (L <= 0)
            {
                return returnValue;
            }

            C = Convert.ToString((P.Length == 0) ? " " : P);

            s = s.Trim();
            n = (short)s.Length;
            if (n >= L)
            {
                returnValue = s.Substring(0, L);
            }
            else
            {
                nL = (short)(Conversion.Int((double)(L - n) / 2));
                nR = (short)(L - n - nL);
                returnValue = Repl(nL, C) + s + Repl(nR, C);
            }

            return returnValue;
        }

        // ===============================================================================
        // Replicate a string 'S' to make a string of length 'L'.
        // ===============================================================================
        private static string Repl(short L, string s)
        {
            string returnValue = "";
            string C = "";
            if (s.Length == 0 | L <= 0)
            {
                returnValue = "";
            }
            else if (s.Length == 1)
            {
                returnValue = new string(Convert.ToChar(s), L);
            }
            else
            {
                C = "";
                while (C.Length < L)
                {
                    C = C + s;
                }
                returnValue = C.Substring(0, L);
            }
            return returnValue;
        }

        // ===============================================================================
        // Return the minimum value of two supplied values
        // ===============================================================================
        public static double MinVal(double a, double b)
        {
            double returnValue = 0;
            returnValue = Convert.ToDouble(a <= b ? a : b);
            return returnValue;
        }

        // ===============================================================================
        // Return the maximum value of two supplied values
        // ===============================================================================
        public static double MaxVal(double a, double b)
        {
            double returnValue = 0;
            returnValue = Convert.ToDouble(a >= b ? a : b);
            return returnValue;
        }

        // ===============================================================================
        //   Return the minimum value of three supplied values
        // ===============================================================================
        public static double MinThreeValues(double a, double b, double C)
        {
            double returnValue = 0;

            double MinValue = 0;

            if (a < b)
            {
                if (b < C)
                {
                    MinValue = a;
                }
                else
                {
                    if (a < C)
                    {
                        MinValue = a;
                    }
                    else
                    {
                        MinValue = C;
                    }
                }
            }
            else
            {
                if (b < C)
                {
                    MinValue = b;
                }
                else
                {
                    if (a < C)
                    {
                        MinValue = a;
                    }
                    else
                    {
                        MinValue = C;
                    }
                }
            }

            returnValue = MinValue;

            return returnValue;
        }
        // ===============================================================================
        // Capitalize the first letter of each word in a string.
        // ===============================================================================
        public static string Proper_Case(string Name)
        {
            string returnValue = "";

            string C = "";
            string R = "";
            short n = 0;
            bool fs = false;

            C = Name.ToLower();
            returnValue = "";

            if (C.Length > 0)
            {
                fs = true;
                for (n = 1; n <= C.Length; n++)
                {
                    R = C.Substring(n - 1, 1);
                    if (R == " ")
                    {
                        fs = true;
                    }
                    if (fs && R != " ")
                    {
                        returnValue = returnValue + R.ToUpper();
                        fs = false;
                    }
                    else
                    {
                        returnValue = returnValue + R;
                    }
                }
            }

            return returnValue;
        }



        internal static object[] PadC(object p, short v)
        {
            throw new NotImplementedException();
        }

        

        //TODO: change the MSComm type to dynamic  Somvir_1

        //public static void SetComm(MSComm ComL) //  as Control
        //{
        //    //PComLCD = ComL;
        //}
        public static void SetComm(dynamic ComL) //  as Control
        {
            PComLCD = ComL;
        }
        //END TODO: Somvir_1
        
      
        // VBConversions Note: Former VB static variables moved to class level because they aren't supported in C#.
        static short WriteToLogFile_FL = 0;

        public static void WriteToLogFile(string MsgStr)
        {
            string FileName = "";
            // static short FL = 0; VBConversions Note: Static variable moved to class level and renamed WriteToLogFile_FL. Local static variables are not supported in C#.
            string NewFName = "";
            short fnum = 0;
            Chaps_Main.Register_Renamed = new CStoreCommander.Entities.Register { WritePosLog = true };
            try
            {
                if (Chaps_Main.Register_Renamed.WritePosLog)
                {

                    
                    
                    FileName = @"C:\APILog\PosLog.txt";
                    

                    WriteToLogFile_FL++;
                    if (WriteToLogFile_FL > 100)
                    {
                        WriteToLogFile_FL = (short)0;

                        if (FileSystem.Dir(FileName) != "")
                        {
                            
                            
                            if (FileSystem.FileLen(FileName) > 1000000)
                            {
                                
                                
                                
                                NewFName = Chaps_Main.Logs_Path + "PosLog" + DateAndTime.Day(DateAndTime.Today).ToString("00") + DateAndTime.Hour(DateAndTime.TimeOfDay).ToString("00") + ".txt";
                                

                                Variables.CopyFile(FileName, NewFName, 0);
                                Variables.DeleteFile(FileName);
                            }
                        }
                    }
                    fnum = (short)(FileSystem.FreeFile());
                    FileSystem.FileOpen(fnum, FileName, OpenMode.Append);
                    FileSystem.PrintLine(fnum, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + Strings.Space(3) + MsgStr);
                    FileSystem.FileClose(fnum);
                }
            }
            catch
            {
                goto err1;
            }
            err1:
            1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.
        }

        public static bool SplitResponse(string strBuffer, string strWaiting, ref string strResponse, ref string strRemain)
        {
            bool returnValue = false;
            object arryRes;
            short i = 0;
            short j = 0;

            strResponse = "";
            strRemain = "";
            if (strBuffer == "")
            {
                returnValue = false;
            }
            else //if strBuffer<>""
            {
                i = (short)(strBuffer.IndexOf(strWaiting) + 1);
                if (i < 1)
                {
                    returnValue = false;
                }
                else
                {
                    j = (short)(strBuffer.IndexOf(";", i + 1 - 1) + 1);
                    strResponse = strBuffer.Substring(i - 1, j - i); // + 1) 'strResponse should include the ";"
                    strRemain = strBuffer.Substring(0, i - 1) + strBuffer.Substring(j + 1 - 1);
                    returnValue = true;
                }                
            }
            return returnValue;
        }

        
        public static bool SplitGiveXResponse(string strBuffer, string strWaiting, ref Variables.GiveXResponseType GX_Response, ref string strRemain)
        {
            bool returnValue = false;

            short i = 0;
            short j = 0;
            string strResponse = "";
            string[] Arr1 = null;

            returnValue = false;
            strResponse = "";
            strRemain = "";
            if (strBuffer == "")
            {
                returnValue = false;
            }
            else //if strBuffer<>""
            {
                i = (short)(strBuffer.IndexOf(strWaiting) + 1);
                if (i < 1)
                {
                    returnValue = false;
                }
                else
                {
                    j = (short)(strBuffer.IndexOf(",END-DATA", i + 1 - 1) + 1);
                    if (j < 1)
                    {
                        return returnValue;
                    }
                    strResponse = strBuffer.Substring(i - 1, j - i);
                    strRemain = strBuffer.Substring(0, i - 1) + strBuffer.Substring(j + 9 - 1);

                    Arr1 = Strings.Split(Expression: strResponse, Delimiter: ",", Compare: CompareMethod.Text);
                    for (i = 0; i <= (Arr1.Length - 1); i++)
                    {
                        switch (i)
                        {
                            case (short)0:
                                GX_Response.ServiceType = Arr1[i].Trim();
                                break;
                            case (short)1:
                                GX_Response.TransactionCode = Arr1[i].Trim();
                                break;
                            case (short)2:
                                GX_Response.Result = Arr1[i].Trim();
                                break;
                            case (short)3:
                                GX_Response.OperatorID = Arr1[i].Trim();
                                break;
                            case (short)4:
                                GX_Response.TerminalID = Arr1[i].Trim();
                                break;
                            case (short)5:
                                GX_Response.TransactionReference = Arr1[i].Trim();
                                break;
                            case (short)6:
                                GX_Response.TransactionReferenceFrom = Arr1[i].Trim();
                                break;
                            case (short)7:
                                GX_Response.PointsTransactionReference = Arr1[i].Trim();
                                break;
                            case (short)8:
                                GX_Response.PointsTransactionReferenceFrom = Arr1[i].Trim();
                                break;
                            case (short)9:
                                GX_Response.PreAuthReference = Arr1[i].Trim();
                                break;
                            case (short)10:
                                GX_Response.SecurityCode = Arr1[i].Trim();
                                break;
                            case (short)11:
                                GX_Response.Amount = Arr1[i].Trim();
                                break;
                            case (short)12:
                                GX_Response.AmountRedeemed = Arr1[i].Trim();
                                break;
                            case (short)13:
                                GX_Response.AuthorizedAmount = Arr1[i].Trim();
                                break;
                            case (short)14:
                                GX_Response.Units = Arr1[i].Trim();
                                break;
                            case (short)15:
                                GX_Response.Points = Arr1[i].Trim();
                                break;
                            case (short)16:
                                GX_Response.PointsAdded = Arr1[i].Trim();
                                break;
                            case (short)17:
                                GX_Response.PointsCancelled = Arr1[i].Trim();
                                break;
                            case (short)18:
                                GX_Response.CertificateBalance = Arr1[i].Trim();
                                break;
                            case (short)19:
                                GX_Response.PointsBalance = Arr1[i].Trim();
                                break;
                            case (short)20:
                                GX_Response.GivexNumber = Arr1[i].Trim();
                                break;
                            case (short)21:
                                GX_Response.ExpiryDate = Arr1[i].Trim();
                                break;
                            case (short)22:
                                GX_Response.TransmissionDate = Arr1[i].Trim();
                                break;
                            case (short)23:
                                GX_Response.TransmissionTime = Arr1[i].Trim();
                                break;
                            case (short)24:
                                GX_Response.CurrencyCode = Arr1[i].Trim();
                                break;
                            case (short)25:
                                GX_Response.MemberName = Arr1[i].Trim();
                                break;
                            case (short)26:
                                GX_Response.Services = Arr1[i].Trim();
                                break;
                            case (short)27:
                                GX_Response.OperatorLoginFlag = Arr1[i].Trim();
                                break;
                            case (short)28:
                                GX_Response.OperatorPasswordFlag = Arr1[i].Trim();
                                break;
                            case (short)29:
                                GX_Response.Continuation = Arr1[i].Trim();
                                break;
                            case (short)30:
                                GX_Response.CashoutId = Arr1[i].Trim();
                                break;
                            case (short)31:
                                GX_Response.ReportLines = Arr1[i].Trim();
                                break;
                        }
                    }

                    returnValue = true;
                }
            }
            return returnValue;
        }
        

        
        public static string GetGiveXRequest()
        {
            string returnValue = "";
            string strTmp = "";

            returnValue = "";

            strTmp = Variables.GX_Request.ServiceType + ",";
            strTmp = strTmp + Variables.GX_Request.UserID + ",";
            strTmp = strTmp + Variables.GX_Request.UserPassword + ",";
            strTmp = strTmp + Variables.GX_Request.OperatorID + ",";
            strTmp = strTmp + Variables.GX_Request.OperatorPassword + ",";
            strTmp = strTmp + Variables.GX_Request.GivexNumberFrom + ",";
            strTmp = strTmp + Variables.GX_Request.ExpiryDateFrom + ",";
            strTmp = strTmp + Variables.GX_Request.TrackIIDataFrom + ",";
            strTmp = strTmp + Variables.GX_Request.GivexNumber + ",";
            strTmp = strTmp + Variables.GX_Request.ExpiryDate + ",";
            strTmp = strTmp + Variables.GX_Request.TrackIIData + ",";
            strTmp = strTmp + Variables.GX_Request.Language + ",";
            strTmp = strTmp + Variables.GX_Request.Amount + ",";
            strTmp = strTmp + Variables.GX_Request.TransactionCode + ",";
            strTmp = strTmp + Variables.GX_Request.SecurityCode + ",";
            strTmp = strTmp + Variables.GX_Request.TransmissionDate + ",";
            strTmp = strTmp + Variables.GX_Request.TransmissionTime + ",";
            strTmp = strTmp + Variables.GX_Request.TransactionReference + ",";
            strTmp = strTmp + Variables.GX_Request.Units + ",";
            strTmp = strTmp + Variables.GX_Request.PromoCode + ",";
            strTmp = strTmp + Variables.GX_Request.Points + ",";
            strTmp = strTmp + Variables.GX_Request.SerialNumber + ",";
            strTmp = strTmp + Variables.GX_Request.ReportType + ",";
            strTmp = strTmp + Variables.GX_Request.CashoutId + ",";
            strTmp = strTmp + Variables.GX_Request.TerminalID + ",";
            strTmp = strTmp + Variables.GX_Request.PreAuthReference + ",END-DATA";

            returnValue = strTmp;
            return returnValue;
        }

        public static void InitGiveXRequest()
        {
            Variables.GX_Request.ServiceType = "";
            Variables.GX_Request.UserID = "";
            Variables.GX_Request.UserPassword = "";
            Variables.GX_Request.OperatorID = "";
            Variables.GX_Request.OperatorPassword = "";
            Variables.GX_Request.GivexNumberFrom = "";
            Variables.GX_Request.ExpiryDateFrom = "";
            Variables.GX_Request.TrackIIDataFrom = "";
            Variables.GX_Request.GivexNumber = "";
            Variables.GX_Request.ExpiryDate = "";
            Variables.GX_Request.TrackIIData = "";
            Variables.GX_Request.Language = "";
            Variables.GX_Request.Amount = "";
            Variables.GX_Request.TransactionCode = "";
            Variables.GX_Request.SecurityCode = "";
            Variables.GX_Request.TransmissionDate = "";
            Variables.GX_Request.TransmissionTime = "";
            Variables.GX_Request.TransactionReference = "";
            Variables.GX_Request.Units = "";
            Variables.GX_Request.PromoCode = "";
            Variables.GX_Request.Points = "";
            Variables.GX_Request.SerialNumber = "";
            Variables.GX_Request.ReportType = "";
            Variables.GX_Request.CashoutId = "";
            Variables.GX_Request.TerminalID = "";
            Variables.GX_Request.PreAuthReference = "";
        }
        

        
        public static void InitGiveXReceipt()
        {
            Variables.GX_Receipt.Date = "";
            Variables.GX_Receipt.Time = "";
            Variables.GX_Receipt.UserID = "";
            Variables.GX_Receipt.TranType = (short)0;
            Variables.GX_Receipt.SaleNum = 0;
            Variables.GX_Receipt.SeqNum = "";
            Variables.GX_Receipt.CardNum = "";
            Variables.GX_Receipt.ExpDate = "";
            Variables.GX_Receipt.Balance = 0;
            Variables.GX_Receipt.PointBalance = 0;
            Variables.GX_Receipt.SaleAmount = 0;
            Variables.GX_Receipt.ResponseCode = "";
        }
        
        public static string GETIDONLY(string ID_DESC)
        {
            string returnValue = "";
            short Pos = 0;

            returnValue = ID_DESC;
            Pos = (short)(ID_DESC.IndexOf("-") + 1);
            if (Pos != 0)
            {
                returnValue = ID_DESC.Substring(0, Pos - 1).Trim();
            }
            return returnValue;
        }
        public static string GETDESCONLY(string ID_DESC)
        {
            string returnValue = "";
            short Pos = 0;

            returnValue = "";
            Pos = (short)(ID_DESC.IndexOf("-") + 1);
            if (Pos != 0)
            {
                returnValue = ID_DESC.Substring(Pos + 1 - 1).Trim();
            }
            return returnValue; // Nicolette replaced this procedure to add Stock Cost and Stock Prices to
        }
    }
}
