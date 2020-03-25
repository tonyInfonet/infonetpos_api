using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.WebApi.Hubs
{
    /// <summary>
    /// Pump Status Hub
    /// </summary>
    [HubName("PumpStatusHub")]
    public class PumpStatusHub : Hub
    {
        private readonly ILog _pumplog = LoggerManager.PumpLogger;

        /// <summary>
        /// On Connected
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            _pumplog.Info("Client connected to Hub with id" + Context.ConnectionId);
            return Clients.All.joined();
        }

        /// <summary>
        /// Called when a client gets disconnected
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            _pumplog.Info("Client disconnected from Hub with id" + Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenPortReading()
        {
            PumpStatusCache.Instance.ReadUdpPort();
        }
    }
}