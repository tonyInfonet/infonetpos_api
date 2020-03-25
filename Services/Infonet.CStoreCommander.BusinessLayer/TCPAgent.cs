using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Microsoft.VisualBasic;
using System.Threading;
using Infonet.CStoreCommander.BusinessLayer.Manager;

namespace Infonet.CStoreCommander.BusinessLayer
{

    public class TCPAgent : ManagerBase
    {
        private string _mPortReading;
        private int _mServConRequest;
        private bool _mPortOpened;
        private string _myRemoteHost;
        private short _myRemotePort;
        private Socket _client;

        private static TCPAgent _instance;

        private TCPAgent() { }

        public static TCPAgent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TCPAgent { GiveXFormat = false };
                }
                return _instance;
            }
        }

        public bool GiveXFormat { get; set; }

        public string PortReading
        {
            get
            {
                return _mPortReading;
            }
            set
            {
                _mPortReading = System.Convert.ToString(value);
                WriteToLogFile("PortReading property is set to:" + _mPortReading);
            }
        }

        public string NewPortReading
        {
            get
            {
                Read_Port(true);
                return _mPortReading;
            }
            set
            {
                _mPortReading = System.Convert.ToString(value);
                WriteToLogFile("PortReading property is set to:" + _mPortReading);
            }
        }

        public int Serv_ConRequest
        {
            get
            {
                return _mServConRequest;
            }
            set
            {
                _mServConRequest = System.Convert.ToInt32(value);
            }
        }

        public bool PortOpened
        {
            get
            {
                return _mPortOpened;
            }
            set
            {
                _mPortOpened = System.Convert.ToBoolean(value);
            }
        }

        private Socket Socket
        {
            get
            {
                if (_client == null)
                {
                    _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                    var ipAddress = IPAddress.Parse(Variables.gPumps?.IP?.FC_IP);
                    var remoteEndPoint = new IPEndPoint(ipAddress, Variables.gPumps.IP.FC_TCP_Port);
                    try
                    {
                        _client.Connect(remoteEndPoint);
                        this.PortOpened = true;
                    }
                    catch (Exception exception)
                    {

                    }
                }
                return _client;
            }
        }

        public string RecievePacket()
        {
            byte[] data = new byte[1024];
            int size = Socket.Receive(data);
            string strPacket = Encoding.UTF8.GetString(data);
            return strPacket;
        }

        public bool IsConnected
        {
            get
            {
                if (Socket == null)
                {
                    return false;
                }
                var retries = 2;
                try
                {
                    while (retries > 0)
                    {
                        if (Socket.Connected) { return true; }
                        retries--;
                        Variables.Sleep(100);
                        _client = null;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        public bool SocketConnected
        {
            get
            {
                if (Socket == null)
                {
                    return false;
                }
                bool part1 = Socket.Poll(1000, SelectMode.SelectRead);
                bool part2 = (Socket.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }

        }


        public bool Send_TCP(ref string Command_Renamed, bool Dummy_Msg)
        {
            bool returnValue = false;
            short i;
            try
            {
                if (this.PortOpened)
                {
                  //  WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside ReadPort 11111 start");

                    Socket.Send(Encoding.ASCII.GetBytes(Command_Renamed));
                    if (GiveXFormat)
                    {
                        WriteToLogFile("Send TCP command to GiveX server: " + Command_Renamed);
                    }
                    else
                    {
                        WriteToLogFile("Send TCP command: " + Command_Renamed);
                    }
                   // WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside ReadPort 2222 mid");
                    //if (Dummy_Msg)
                    //{
                    Variables.Sleep(50);

                    
                    Read_Port(Dummy_Msg);

                 //   WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")+"Inside Readport 3333 end");
                    //}
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile("Error in TCP send data: " + Command_Renamed + " (" + ex.Message + ")");
                returnValue = false;
            }
            return returnValue;
        }

        public bool OpenPort(string RemIP, short RemPort)
        {
            try
            {
                try
                {
                    if (_client != null)
                    {
                        _client.Shutdown(SocketShutdown.Both);
                        _client.Disconnect(true);
                        _client.Close();
                    }
                }
                catch (Exception ex)
                {
                }
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EndPoint ipAddress = new IPEndPoint(IPAddress.Parse(RemIP), RemPort);
                _client.Connect(ipAddress);
                Thread.Sleep(100);
                this.PortOpened = true;
            }
            catch (Exception ex)
            {
                this.PortOpened = false;
                WriteToLogFile("Error in TCP open port " + RemIP + " " + System.Convert.ToString(RemPort) + " (" + ex.Message + ")");
            }
            return false;
        }

        public void ClosePort()
        {
            try
            {
                _client.Shutdown(SocketShutdown.Both);
                _client.Disconnect(true);
                _client.Close();
            }
            catch (Exception)
            {

            }

            _client = null;
        }

        public bool Read_Port(bool dummyMsg)
        {
            string Response = "";
            short L;
            string strRemain = "";
            string strSplitResponse = "";
            short i = 0;
            short j = 0;


            byte[] data = new byte[1024];

            if (dummyMsg)
            {
                if (Socket.Available > 0)
                {
                    int bytesRec = Socket.Receive(data);
                }
            }
            else
            {
                int size = Socket.Receive(data, SocketFlags.None);
            }

            Response = Encoding.UTF8.GetString(data);

            // wTCP.GetData(Response, VariantType.String); 
            //object tempResponse = null;


            //wTCP.GetData(ref tempResponse);
            //if (tempResponse != null)
            //{
            //    Response = tempResponse.ToString();
            //}

            WriteToLogFile("TCP Receive from " + _myRemoteHost + " " + System.Convert.ToString(_myRemotePort) + ": " + Response);

            if (GiveXFormat)
            {
                _mPortReading = _mPortReading + Response;
                WriteToLogFile("Get response from GiveX server:" + Response);
                return false;
            }

            //strRemain = Response;
            strRemain = Response.Replace("\0", string.Empty);
            //response = Mid(response, 1, Len(response) - 1)
            while (Response.Length != 0)
            {
                L = (short)Response.Length;
                switch (Response.Substring(0, 3))
                {
                    case "Prp":
                    case "Rpr":
                    case "DAP":
                    case "Ist":
                    case "Stp":
                    case "Rsm":
                    case "Set":
                    case "STL":
                    case "HPP":
                    case "HRP":
                    case "CHR":
                        if (Response.Substring(5, 1) == "O")
                        {
                            strSplitResponse = Response.Substring(0, 7);
                            strRemain = Response.Substring(7);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 8);
                            strRemain = Response.Substring(8);
                        }
                        break;

                    case "Rmv":
                        if (Response.Substring(6, 1) == "O")
                        {
                            strSplitResponse = Response.Substring(0, 8);
                            strRemain = Response.Substring(8);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 9);
                            strRemain = Response.Substring(9);
                        }
                        break;


                    case "SPR":
                        if (Response.Substring(7, 1) == "O")
                        {
                            strSplitResponse = Response.Substring(0, 9);
                            strRemain = Response.Substring(9);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 10);
                            strRemain = Response.Substring(10);
                        }
                        break;


                    case "Tot":
                        //OR: "Tot" & pumpid(2) & Position(1) & "ERR"
                        if (Response.Substring(6, 1) == "E")
                        {
                            strSplitResponse = Response.Substring(0, 9);
                            strRemain = Response.Substring(9);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 33);
                            strRemain = Response.Substring(33);
                        }
                        break;

                    case "THL":
                        //OR: "THL" & pumpid(2) & "ERR"
                        if (Response.Substring(5, 1) == "E")
                        {
                            strSplitResponse = Response.Substring(0, 8);
                            strRemain = Response.Substring(8);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 27);
                            strRemain = Response.Substring(27);
                        }
                        break;

                    case "Sal":
                        //Or: "Sal" & Pumpid(2) & "ERR"
                        if (Response.Substring(5, 1) == "E")
                        {
                            strSplitResponse = Response.Substring(0, 8);
                            strRemain = Response.Substring(8);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 28);
                            strRemain = Response.Substring(28);
                        }
                        break;

                    case "Ath":
                    case "Dau":
                        if (Response.Substring(5, 1) == "O")
                        {
                            strRemain = Response.Substring(7);
                        }
                        else
                        {
                            strRemain = Response.Substring(8);
                        }
                        strSplitResponse = "";
                        break;

                    case "Brd":
                        if (Response.Substring(6, 1) == "O")
                        {
                            strRemain = Response.Substring(8);
                        }
                        else
                        {
                            strRemain = Response.Substring(9);
                        }
                        strSplitResponse = "";
                        break;


                    case "Isd":
                    case "Ppy":
                        if (Response.Substring(4, 1) == "O")
                        {
                            strSplitResponse = Response.Substring(0, 6);
                            strRemain = Response.Substring(6);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 7);
                            strRemain = Response.Substring(7);
                        }
                        break;



                    case "DPS":
                        if (Response.Substring(3, 1) == "O")
                        {
                            strSplitResponse = Response.Substring(0, 5);
                            strRemain = Response.Substring(5);
                        }
                        else
                        {
                            strSplitResponse = Response.Substring(0, 6);
                            strRemain = Response.Substring(6);
                        }
                        break;



                    case "DIP":
                        i = (short)(Response.IndexOf("OK", StringComparison.Ordinal) + 1);
                        j = (short)(Response.IndexOf("ERR", StringComparison.Ordinal) + 1);
                        if (i > 0)
                        {
                            strSplitResponse = Response.Substring(0, i + 1);
                            strRemain = Response.Substring(i + 2 - 1);
                        }
                        else if (j > 0)
                        {
                            strSplitResponse = Response.Substring(0, j + 2);
                            strRemain = Response.Substring(j + 3 - 1);
                        }
                        else
                        {
                            strSplitResponse = "";
                            strRemain = "";
                        }
                        break;


                    default:
                        i = (short)(Response.IndexOf("OK", StringComparison.Ordinal) + 1);
                        j = (short)(Response.IndexOf("ERR", StringComparison.Ordinal) + 1);
                        if (i > 0)
                        {
                            if (j > 0)
                            {
                                if (i < j)
                                {
                                    strRemain = Response.Substring(i + 2 - 1);
                                }
                                else //i>0,j>0,i>j
                                {
                                    strRemain = Response.Substring(j + 3 - 1);
                                }
                            }
                            else
                            {
                                strRemain = Response.Substring(i + 2 - 1);
                            }
                        }
                        else if (j > 0)
                        {
                            strRemain = Response.Substring(j + 3 - 1);
                        }
                        strSplitResponse = "";
                        break;

                }
                if (!string.IsNullOrEmpty(strSplitResponse))
                {
                    _mPortReading = _mPortReading + strSplitResponse + ";";
                }
                if (Response == strRemain)
                {
                    break;
                }
                Response = strRemain;
                WriteToLogFile("get new response:" + strSplitResponse + " and change PortReading to: " + _mPortReading);
            }

            return true;
        }

        public bool Act_Listener(short locPort)
        {
            bool returnValue = false;
            if ((int)this.Serv_ConRequest == 0)
            {
                Socket.Close();
                EndPoint ipAddress = new IPEndPoint(IPAddress.Any, locPort);
                Socket.Connect(ipAddress);
                returnValue = true;
            }
            return returnValue;
        }

    }
}
