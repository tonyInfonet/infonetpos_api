using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class TillCloseResponse
    {
        public MessageStyle PrepayMessage { get; set; }

        public MessageStyle SuspendSaleMessage { get; set; }

        public MessageStyle CloseTillMessage { get; set; }

        public MessageStyle ReadTotalizerMessage { get; set; }

        public MessageStyle TankDipMessage { get; set; }

        public bool ProcessTankDip { get; set; }

        public MessageStyle EndSaleSessionMessage { get; set; }
    }
}
