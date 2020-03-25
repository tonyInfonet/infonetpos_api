using Infonet.CStoreCommander.BusinessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SocketWrenchCtl;
using SocketTools;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ICommManager
    {
        bool SendData(string strSend);

        void Class_Initialize_Renamed();
        string FormatKickBackRequest(string strRequest);

        uint CRC32(string DataStr);

        bool Initialize(string strHostIP, short intHostPort, short System_Renamed);

        void swTCP_OnRead(Object eventSender, EventArgs eventArgs);
        void swTCP_OnRead12();
        void swTCP_OnError(Object eventSender, SocketWrench.ErrorEventArgs eventArgs);
        void swTCP_OnDisconnect(Object eventSender, EventArgs eventArgs);

        void swTCP_OnConnect(Object eventSender, EventArgs eventArgs);

         void EndClass();
    }
}
