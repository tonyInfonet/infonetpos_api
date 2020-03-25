using Infonet.CStoreCommander.BusinessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SocketWrenchCtl;
using SocketTools;
using Microsoft.VisualBasic;
using Infonet.CStoreCommander.BusinessLayer.Manager;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class CommManager : ManagerBase, ICommManager
    {
        // public  Comm Comm.KBComm { get; set; }
        private IPolicyManager _policyManager { get; set; }
        public static int kickBackfalg = 0;
        public Action onRead;
        //   public event EventHandler OnConnect;
        public CommManager(IPolicyManager policyManager)
        {

            _policyManager = policyManager;
            Class_Initialize_Renamed();
        }
        public void Class_Initialize_Renamed()
        {
            //'  This is the official polynomial used by CRC32 in PKZip.
            //'  Often the polynomial is shown reversed (04C11DB7).
           // var comm = new Comm();
            // const int dwPolynomial = unchecked((int)0xEDB88320);
            const int dwPolynomial = unchecked((int)0xEDB88320);
            short i = 0;
            short j = 0;
            int dwCrc = 0;
            if (kickBackfalg == 1)
            {
                if (_policyManager.Use_KickBack)
                {

                    Comm.KBComm.crc32Table = new uint[257];

                    for (i = 0; i <= 255; i++)
                    {
                        dwCrc = i;
                        for (j = 8; j >= 1; j--)
                        {
                            if ((dwCrc & 1) == 1)
                            {
                                // dwCrc = System.Convert.ToInt32(((dwCrc & 0xFFFFFFFE) / 2) & 0x7FFFFFFF);
                                dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                                // dwCrc = dwCrc ^ dwPolynomial;
                            }
                            else
                            {
                                dwCrc >>= 1;
                                // dwCrc = System.Convert.ToInt32(((dwCrc & 0xFFFFFFFE) / 2) & 0x7FFFFFFF);
                            }
                        }
                        Comm.KBComm.crc32Table[i] = (uint)dwCrc;
                    }

                    Comm.KBComm.HostIP = "216.83.78.100";
                    Comm.KBComm.HostPort = (short)701;



                }
            }

        }


        public bool SendData(string strSend)
        {
           
           // var comm = new Comm();
            bool returnValue = false;
            string RespStr = "";

            Comm.KBComm.swTCP.Blocking = false;
           // var a1 = Comm.KBComm.IsConnected;
           // var a2 = Comm.KBComm.swTCP.IsWritable;
            //if (a1 && a2)
            if (Comm.KBComm.IsConnected && Comm.KBComm.swTCP.IsWritable)
            {
             
                //If Policy.Use_KickBack Then
                 if (Comm.KBComm.commSystem == 0)
             //  if (_policyManager.Use_KickBack)
                {
                   // var s = FormatKickBackRequest(strSend);
                    int i=Comm.KBComm.swTCP.Write(FormatKickBackRequest(strSend));
                    int j = Comm.KBComm.swTCP.Read(ref RespStr, 4096);
                    //onRead = swTCP_OnRead12;
                    //onRead?.Invoke();
                    
                    //WriteToLogFile("Before 83: " + Comm.KBComm.swTCP.IsWritable);
                    returnValue = true;
                    //WriteToLogFile("Before 85: " + a);
                    // WriteToLogFile("Send to Kickback Server: " + FormatKickBackRequest(strSend));
                    //ElseIf Policy.FUELONLY Then
                }


                returnValue = true;
            }
            else
            {
                WriteToLogFile("Cannot Send to Server: " + strSend);
                returnValue = false;
            }

            return returnValue;
        }



        public string FormatKickBackRequest(string strRequest)
        {
            
            var comm = new Comm();
           
            string returnValue = "";


            string strTemp = "";
            uint longTemp = 0;

            strTemp = "";
            strTemp = strTemp + Comm.KB_Signature + "\0" + "\0";
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Comm.KB_Action)) + "\0" + "\0" + "\0";

            longTemp = Convert.ToUInt32(strRequest.Length);
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            var a5 = Comm.KBComm.swTCP.IsConnected;
            longTemp = CRC32(strRequest);
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));

            longTemp = CRC32(strTemp);
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));
            longTemp = longTemp / 256;
            strTemp = strTemp + System.Convert.ToString(Convert.ToChar(Convert.ToInt32(longTemp % 256)));

            returnValue = strTemp + strRequest;
            var a7 = Comm.KBComm.swTCP.IsConnected;
            return returnValue;
        }



        public uint CRC32(string DataStr)
        {
            var a = Comm.KBComm;
            kickBackfalg = 1;
            var commanager = new CommManager(_policyManager);
            var comm = new Comm();
            uint returnValue = 0;
            var a5 = Comm.KBComm.swTCP.IsConnected;

            uint crc32Result = 0;
           // ulong crc32Result = 0;
            short i = 0;
            short strLen = 0;
            // short iLookup = 0;
            ulong iLookup = 0;
            byte[] array = Encoding.ASCII.GetBytes(DataStr);
            crc32Result = 0xFFFFFFFF;
            strLen = (short)DataStr.Length;

            for (i = 0; i < strLen; i++)
            {
                //var a = DataStr.Substring(i - 1, 1).ToCharArray();
                //char c = a[0];
                //int f = Convert.ToInt32(c); 

                //iLookup = Convert.ToInt16((crc32Result & 0xFF) ^ Convert.ToInt32(DataStr.Substring(i - 1, 1)));
                //crc32Result = Convert.ToUInt32(((crc32Result & 0xFFFFFF00) / 0x100) & 16777215); // nasty shr 8 with vb :/
                // crc32Result = crc32Result ^ Convert.ToUInt32(crc32Table[iLookup]);
                iLookup = (crc32Result & 0xFF) ^ array[i];
                crc32Result >>= 8;
               // var a = Comm.KBComm.crc32Table[iLookup];
                crc32Result ^= Comm.KBComm.crc32Table[iLookup];

                //crc32Result = crc32Result ^ a2;
            }

            if (crc32Result < 0)
            {
                returnValue = ~(crc32Result);
            }
            else
            {
                returnValue = crc32Result;
            }
          
            kickBackfalg = 0;
            return returnValue;
        }


        public bool Initialize(string strHostIP, short intHostPort, short System_Renamed)
        {
             var comm = new Comm();
            bool returnValue = false;
            //  dynamic Policy_Renamed = default(dynamic);
          
            int nError = 0;
            float timeIN = 0;
            byte TimeOut = 0;

            Comm.KBComm.HostIP = strHostIP;
            Comm.KBComm.HostPort = intHostPort;

            Comm.KBComm.swTCP = new SocketWrench();
            Comm.KBComm.swTCP.OnConnect += new EventHandler(swTCP_OnConnect);
            //Comm.Comm.KBComm.swTCP.OnConnect += (sender2,e2)=> swTCP_OnConnect(sender2,e2,ref Comm Comm.KBComm);
            Comm.KBComm.swTCP.OnDisconnect += new EventHandler(swTCP_OnDisconnect);
            Comm.KBComm.swTCP.OnError += new SocketWrench.OnErrorEventHandler(swTCP_OnError);
            Comm.KBComm.swTCP.OnRead += new EventHandler(swTCP_OnRead);

            Comm.KBComm.swTCP.AutoResolve = false;
            Comm.KBComm.swTCP.Blocking = false;
            Comm.KBComm.swTCP.Secure = false;
            // TODO: Updated call of the socket wrench for the new dll. - Ipsit_33
            // Refer: https://sockettools.com/webhelp/dotnet/dotnet/htmlhelp/SocketTools.SocketWrench.Initialize_overload_1.html
            // var result = Comm.KBComm.swTCP.Initialize("FMKLPMFRIKHBMURI");
            var result = Comm.KBComm.swTCP.Initialize("BMKJQDIQHXKJUJGRIKTLOSHHJQFOYO");
            nError = Convert.ToInt32(result);
            if (nError != 1)
            {
                returnValue = false;
                WriteToLogFile("Failed to initialize SocketWrench control");
                return returnValue;
            }

            // TODO: Updated call of the socket wrench for the new dll. - Ipsit_34
            if (Comm.KBComm.swTCP.Status == SocketWrench.SocketStatus.statusDisconnect ||
                            Comm.KBComm.swTCP.Status == SocketWrench.SocketStatus.statusUnused)
            {
               // bool i=Comm.KBComm.swTCP.Connect(Comm.KBComm.HostIP, Comm.KBComm.HostPort, SocketWrench.SocketProtocol.socketStream, TimeOut);
                nError = Convert.ToInt32(Comm.KBComm.swTCP.Connect(Comm.KBComm.HostIP, Comm.KBComm.HostPort, SocketWrench.SocketProtocol.socketStream, TimeOut));
                var s = Comm.KBComm.swTCP.LastErrorString;
                if (nError != 1)
                {
                    returnValue = false;

                    WriteToLogFile("Connect method returned an error in Comm class. Cannot connect to  " + Comm.KBComm.HostIP + ":" + Convert.ToString(Comm.KBComm.HostPort));
                    return returnValue;
                }

                //
                Comm.KBComm.commSystem = System_Renamed;

                if (Comm.KBComm.commSystem == 1)
                {
                    TimeOut = Convert.ToByte(_policyManager.FUELONLY_TMT);
                }
                else
                {
                    TimeOut = (byte)5; //default timeout
                }

                timeIN = (float)DateAndTime.Timer;

                while (true)
                {
                    System.Windows.Forms.Application.DoEvents(); // Because of this line , OnConnect event is fired.
                    // since this function is used not only by Kickback
                    //If (Timer - timeIN > Policy.KICKBACK_TMT) Or boolIsConnected Then
                    var a1 = DateAndTime.Timer - timeIN;
                    if (((DateAndTime.Timer - timeIN) > TimeOut) || Comm.KBComm.boolIsConnected)
                    {
                        break;
                    }
                }
            }
            if (Comm.KBComm.boolIsConnected)
            {
                if (Comm.KBComm.commSystem == 1)
                {
                    WriteToLogFile("Connected to FuelOnly Server " + Comm.KBComm.HostIP + ":" + Convert.ToString(Comm.KBComm.HostPort));
                }

            }
            else
            {
                WriteToLogFile("Cannot connect to " + Comm.KBComm.HostIP + ":" + Convert.ToString(Comm.KBComm.HostPort));
            }
            var a = Comm.KBComm.swTCP;
            returnValue = Comm.KBComm.boolIsConnected;

            return returnValue;
        }

        public void EndClass()
        {
            // var comm = new Comm();
            Comm.KBComm.swTCP.Cancel();
            Comm.KBComm.swTCP.Disconnect();
            Comm.KBComm.swTCP = null;
           // Comm.KBComm.swTCP.OnConnect += new EventHandler(swTCP_OnConnect);
           // Comm.KBComm.swTCP.OnDisconnect += new EventHandler(swTCP_OnDisconnect);
           // Comm.KBComm.swTCP.OnError += new SocketWrench.OnErrorEventHandler(swTCP_OnError);
           //Comm.KBComm.swTCP.OnRead += new EventHandler(swTCP_OnRead);
            Comm.KBComm.boolIsConnected = false;

        }


        public void swTCP_OnConnect(Object eventSender, EventArgs eventArgs)
        {
            //var comm = new Comm();
            Comm.KBComm.boolIsConnected = true;
            var a = Comm.KBComm.swTCP.LastErrorString;
        }

        public void swTCP_OnDisconnect(Object eventSender, EventArgs eventArgs)
        {
            // var comm = new Comm();
            Comm.KBComm.boolIsConnected = false;
            Comm.KBComm.swTCP.Disconnect();
        }
         
        public void swTCP_OnError(Object eventSender, SocketWrench.ErrorEventArgs eventArgs)
        {
            //var comm = new Comm();
            //  if (!string.IsNullOrEmpty(comm.CommErrorEvent(eventArgs.Error.ToString(), eventArgs.Description)))
            Comm.KBComm.CommErrorEvent?.Invoke(eventArgs.Error.ToString(), eventArgs.Description);

            WriteToLogFile("Error in Comm class. Error is " + eventArgs.Error + " " + eventArgs.Description);

        }

        public  void swTCP_OnRead(Object eventSender,EventArgs eventArgs)
     
        {
            var comm = new Comm();
            string RespStr = "";

            if (Comm.KBComm.swTCP.IsReadable == true)
            {
                Comm.KBComm.swTCP.Read(ref RespStr, 4096);
                //
                if (Comm.KBComm.commSystem == 0 | Comm.KBComm.commSystem == 1)
                {
                    Comm.KBComm.CommDataEvent?.Invoke(RespStr.Substring(28));
                }
                else
                {
                    Comm.KBComm.CommDataEvent?.Invoke(RespStr);
                }
            }

            if (Comm.KBComm.commSystem == 1)
            {
               //WriteToLogFile("Received from FuelOnly Server: " + RespStr);
            }


        }

        public void swTCP_OnRead12()
        {
            var comm = new Comm();
            string RespStr = "";

            if (Comm.KBComm.swTCP.IsReadable == true)
            {
                var a=Comm.KBComm.swTCP.Read(ref RespStr, 4096);
                //
                if (Comm.KBComm.commSystem == 0 | Comm.KBComm.commSystem == 1)
                {
                    Comm.KBComm.CommDataEvent?.Invoke(RespStr.Substring(28));
                }
                else
                {
                    Comm.KBComm.CommDataEvent?.Invoke(RespStr);
                }
            }

            if (Comm.KBComm.commSystem == 1)
            {
                //WriteToLogFile("Received from FuelOnly Server: " + RespStr);
            }


        }
    }
}
