using Microsoft.VisualBasic;
using SocketTools;
using System;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class Comm
    {
        //public Comm()
        //{
           
        //}

        
        public static Comm KBComm { get; set; }
       // public const string CSTOOLS4_LICENSE_KEY = "FMKLPMFRIKHBMURI";
        public const string CSTOOLS4_LICENSE_KEY = "BMKJQDIQHXKJUJGRIKTLOSHHJQFOYO";
        public const string KB_Signature = "POSLOYALTY";
        public const int KB_Action = 0x1; 

        public SocketWrench swTCP { get; set; }

        public  string HostIP { get; set; }
        public  short HostPort { get; set; }

        public bool boolIsConnected { get; set; }
        public uint[] crc32Table { get; set; }

        private byte carwashSeqNum;
        public short commSystem { get; set; } //
                                  //It is possible that the site uses all three systems and therefore we need to differentiate
                                  //btw each request
                                  //commSystem=0 -> Kickback
                                  //commSystem=1 -> FuelOnly
                                  //commSystem=2 -> Carwash

        public delegate void CommErrorEventHandler(string ErrorNum, string ErrData);
        public CommErrorEventHandler CommErrorEvent;

        public event CommErrorEventHandler CommError
        {
            add
            {
                CommErrorEvent = (CommErrorEventHandler)Delegate.Combine(CommErrorEvent, value);
            }
            remove
            {
                CommErrorEvent = (CommErrorEventHandler)Delegate.Remove(CommErrorEvent, value);
            }
        }

        public delegate void CommDataEventHandler(string Data);
        public CommDataEventHandler CommDataEvent;

        public event CommDataEventHandler CommData
        {
            add
            {
                CommDataEvent = (CommDataEventHandler)Delegate.Combine(CommDataEvent, value);
            }
            remove
            {
                CommDataEvent = (CommDataEventHandler)Delegate.Remove(CommDataEvent, value);
            }
        }


        public bool IsConnected
        {
            get
            {
                bool returnValue = false;
                returnValue = boolIsConnected;                
                return returnValue;
            }
        }


        public bool SendData(string strSend)
        {
            bool returnValue = false;

            swTCP.Blocking = false;
            if (boolIsConnected && swTCP.IsWritable)
            {
                if (commSystem == 1)
                {
                    swTCP.Write(strSend);
                    WriteToLog("Send to FuelOnly Server: " + strSend);
                }
                returnValue = true;
            }
            else
            {
                WriteToLog("Cannot Send to Server: " + strSend);
                returnValue = false;
            }

            return returnValue;
        }

        // Nov 27, 2008: Nicolette modified this function to initialize the connection to
        // KickBack in one function and to check the time out inside this class instead of the POS
        // April 15, 2010: Svetlana added system to make this class completely generic
        //Public Function Initialize(ByVal strHostIP As String, _
        //ByVal intHostPort As Integer) As Boolean
        public bool Initialize(string strHostIP, short intHostPort, short System_Renamed)
        {
            bool returnValue = false;
            dynamic Policy_Renamed = default(dynamic);

            int nError = 0;
            float timeIN = 0;
            byte TimeOut = 0;

            HostIP = strHostIP;
            HostPort = intHostPort;

            swTCP = new SocketWrench();
            swTCP.OnConnect += new EventHandler(swTCP_OnConnect);
            swTCP.OnDisconnect += new EventHandler(swTCP_OnDisconnect);
            swTCP.OnError += new SocketWrench.OnErrorEventHandler(swTCP_OnError);
            swTCP.OnRead += new EventHandler(swTCP_OnRead);

            swTCP.AutoResolve = false;
            swTCP.Blocking = false;
            swTCP.Secure = false;
            // TODO: Updated call of the socket wrench for the new dll. - Ipsit_33
            // Refer: https://sockettools.com/webhelp/dotnet/dotnet/htmlhelp/SocketTools.SocketWrench.Initialize_overload_1.html
            nError = Convert.ToInt32(swTCP.Initialize(CSTOOLS4_LICENSE_KEY));
            if (nError != 0)
            {
                returnValue = false;
                WriteToLog("Failed to initialize SocketWrench control");
                return returnValue;
            }

            // TODO: Updated call of the socket wrench for the new dll. - Ipsit_34
            if (swTCP.Status == SocketWrench.SocketStatus.statusDisconnect ||
                swTCP.Status == SocketWrench.SocketStatus.statusUnused)
            {
                nError = Convert.ToInt32(swTCP.Connect(HostIP, HostPort, SocketWrench.SocketProtocol.socketStream, TimeOut));

                if (nError != 0)
                {
                    returnValue = false;

                    WriteToLog("Connect method returned an error in Comm class. Cannot connect to " + HostIP + ":" + Convert.ToString(HostPort));
                    return returnValue;
                }

                //
                commSystem = System_Renamed;

                if (commSystem == 1)
                {
                    TimeOut = Convert.ToByte(Policy_Renamed.FUELONLY_TMT);
                }
                else
                {
                    TimeOut = (byte)5; //default timeout
                }

                timeIN = (float)DateAndTime.Timer;
                while (true)
                {
                    System.Windows.Forms.Application.DoEvents();
                    // since this function is used not only by Kickback
                    //If (Timer - timeIN > Policy.KICKBACK_TMT) Or boolIsConnected Then
                    if ((DateAndTime.Timer - timeIN > TimeOut) || boolIsConnected)
                    {
                        break;
                    }
                }
            }
            if (boolIsConnected)
            {
                if (commSystem == 1)
                {
                    WriteToLog("Connected to FuelOnly Server " + HostIP + ":" + Convert.ToString(HostPort));
                }
                if (commSystem == 2)
                {
                    carwashSeqNum = (byte)1;
                    WriteToLog("Connected to the Carwash Server" + HostIP + ":" + Convert.ToString(HostPort));
                }

            }
            else
            {
                WriteToLog("Cannot connect to " + HostIP + ":" + Convert.ToString(HostPort));
            }
            returnValue = boolIsConnected;

            return returnValue;
        }

        public void EndClass()
        {

            swTCP.Cancel();
            swTCP.Disconnect();
            swTCP = null;
            swTCP.OnConnect += new EventHandler(swTCP_OnConnect);
            swTCP.OnDisconnect += new EventHandler(swTCP_OnDisconnect);
            swTCP.OnError += new SocketWrench.OnErrorEventHandler(swTCP_OnError);
            swTCP.OnRead += new EventHandler(swTCP_OnRead);
            boolIsConnected = false;

        }

        //private void Class_Initialize_Renamed()
        //{
        //    const int dwPolynomial = unchecked((int)0xEDB88320);
        //    short i = 0;
        //    short j = 0;
        //    int dwCrc = 0;

        //}
        //public Comm()
        //{
        //    Class_Initialize_Renamed();
        //}

        public void swTCP_OnConnect(Object eventSender, EventArgs eventArgs)
        {
            boolIsConnected = true;
        }

        public void swTCP_OnDisconnect(Object eventSender, EventArgs eventArgs)
        {
            boolIsConnected = false;
            swTCP.Disconnect();
        }

        public void swTCP_OnError(Object eventSender, SocketWrench.ErrorEventArgs eventArgs)
        {
            if (CommErrorEvent != null)
                CommErrorEvent(eventArgs.Error.ToString(), eventArgs.Description);
            WriteToLog("Error in Comm class. Error is " + eventArgs.Error + " " + eventArgs.Description);
        }

        private void swTCP_OnRead(Object eventSender, EventArgs eventArgs)
        {
            string RespStr = "";

            if (swTCP.IsReadable == true)
            {
                swTCP.Read(ref RespStr, 4096);
                //
                if (commSystem == 0 | commSystem == 1)
                {
                    if (CommDataEvent != null)
                        CommDataEvent(RespStr.Substring(28));
                }
                else
                {
                    if (CommDataEvent != null)
                        CommDataEvent(RespStr);
                }
            }
            if (commSystem == 2)
            {
                WriteToLog("Recieved from Carwash Server" + RespStr);
            }

            if (commSystem == 1)
            {
                WriteToLog("Received from FuelOnly Server: " + RespStr);
            }
            
        }



            //private uint CRC32(string DataStr)
            //{
            //    uint returnValue = 0;

            //    uint crc32Result = 0;
            //    short i = 0;
            //    short strLen = 0;
            //    short iLookup = 0;

            //    crc32Result = 0xFFFFFFFF;
            //    strLen = (short)DataStr.Length;

            //    for (i = 1; i <= strLen; i++)
            //    {
            //        iLookup = Convert.ToInt16((crc32Result & 0xFF) ^ Strings.Asc(DataStr.Substring(i - 1, 1)));
            //        crc32Result = Convert.ToUInt32(((crc32Result & 0xFFFFFF00) / 0x100) & 16777215); // nasty shr 8 with vb :/
            //        crc32Result = crc32Result ^ Convert.ToUInt32(crc32Table[iLookup]);
            //    }

            //    if (crc32Result < 0)
            //    {
            //        returnValue = ~(crc32Result);
            //    }
            //    else
            //    {
            //        returnValue = crc32Result;
            //    }

            //    return returnValue;
            //}

        public void WriteToLog(string MsgStr)
        {

            string FileName = "";
            short fnum = 0;
            short i;
            string NewFName = "";

            try
            {
                if (Chaps_Main.Register_Renamed.WritePosLog)
                {
                    {
                        FileName = Chaps_Main.Logs_Path + "XML.log";
                    }
                    if (FileSystem.Dir(FileName) != "")
                    {
                        if (FileSystem.FileLen(FileName) > 1000000)
                        {

                            NewFName = Chaps_Main.Logs_Path + "XML" + DateAndTime.Day(DateAndTime.Today).ToString("00") +
                                DateAndTime.Hour(DateAndTime.TimeOfDay).ToString("00") + ".log";

                            Variables.CopyFile(FileName, NewFName, 0);
                            Variables.DeleteFile(FileName);
                        }
                    }

                    fnum = (short)(FileSystem.FreeFile());
                    FileSystem.FileOpen(fnum, FileName, OpenMode.Append);
                    FileSystem.PrintLine(fnum, DateTime.Now + " " + MsgStr);
                    FileSystem.FileClose(fnum);
                }
            }
            catch
            {
                goto Err_End;
            }
            Err_End:
            1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.
        }

        //private string FormatCarwashRequest(string strRequest)
        //{
        //    string returnValue = "";
        //    string strTemp = "";
        //    int longTemp = 0;
        //    string strLength = "";

        //    strTemp = "";
        //    strTemp = strTemp + "\u0001"; //0x01
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(carwashSeqNum)); //seq number

        //    //Calculate length
        //    strLength = "";
        //    longTemp = strRequest.Length;
        //    strLength = Strings.Chr(longTemp % 256) + strLength;
        //    longTemp = longTemp / 256;
        //    strLength = Strings.Chr(longTemp % 256) + strLength;
        //    longTemp = longTemp / 256;
        //    strLength = Strings.Chr(longTemp % 256) + strLength;
        //    //Svetlana - April lenght is 3 for testing, in product it is 4
        //    longTemp = longTemp / 256;
        //    strLength = Strings.Chr(longTemp % 256) + strLength;
        //    strTemp = strTemp + strLength;

        //    strTemp = strTemp + strRequest; //actual request
        //    strTemp = strTemp + "\u0004"; //0x04
        //    returnValue = strTemp;
        //    return returnValue;
        //}

        /* Added by sonali 22-Dec-2017 */
        //private string FormatKickBackRequest(string strRequest)
        //{
        //    string returnValue = "";

        //    string strTemp = "";
        //    uint longTemp = 0;

        //    strTemp = "";
        //    strTemp = strTemp + KB_Signature + "\0" + "\0";
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(KB_Action)) + "\0" + "\0" + "\0";

        //    longTemp = Convert.ToUInt32(strRequest.Length);
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));

        //    longTemp = CRC32(strRequest);
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));

        //    longTemp = CRC32(strTemp);
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));
        //    longTemp = longTemp / 256;
        //    strTemp = strTemp + System.Convert.ToString(Strings.Chr(Convert.ToInt32(longTemp % 256)));

        //    returnValue = strTemp + strRequest;

        //    return returnValue;
        //}

        /*End by sonali 22-Dec-2017 */
    }
}
