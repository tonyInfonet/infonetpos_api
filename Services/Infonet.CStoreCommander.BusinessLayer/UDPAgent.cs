using System.Net;
using System.Net.Sockets;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using log4net;
using Infonet.CStoreCommander.Logging;

namespace Infonet.CStoreCommander.BusinessLayer
{
    public class UDPAgent : ManagerBase
    {
        private static UDPAgent _instance;
        private static UdpClient _client;
        private readonly ILog _performlog = LoggerManager.PerformanceLogger;
        private UDPAgent() { }

        public static UDPAgent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UDPAgent();
                }
                return _instance;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (_client == null)
                {
                    return false;
                }
                return true;
            }
        }


        public void OpenPort()
        {
            if (_client == null)
            {
                try
                {
                    _client = new UdpClient(Variables.gPumps.IP.POS_UDP_Port);
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(Variables.gPumps.IP.FC_IP), Variables.gPumps.IP.POS_UDP_Port);
                }
                catch (SocketException)
                {
                    throw new SocketException();
                }
            }
            //if (!_client.Client.Connected)
            //{
            //    _client = new UdpClient(Variables.gPumps.IP.POS_UDP_Port);
            //    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(Variables.gPumps.IP.FC_IP), Variables.gPumps.IP.POS_UDP_Port);
            //    _client.Connect(remoteIpEndPoint);
            //}

            
            WriteToLogFile("UDP Connected to: " + System.Convert.ToString(Variables.gPumps.IP.FC_IP) + " " + System.Convert.ToString(Variables.gPumps.IP.POS_UDP_Port));
        }

        public void ClosePort()
        {
            _client?.Close();
            _client = null;
        }



        public string ReceiveData()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, Variables.gPumps.IP.POS_UDP_Port);
            byte[] data = _client.Receive(ref remoteIpEndPoint);
            string strPacket = System.Text.Encoding.UTF8.GetString(data);
            
         //   WriteUDPData("Receive data "+ strPacket);
            return strPacket;
        }


    }
}
