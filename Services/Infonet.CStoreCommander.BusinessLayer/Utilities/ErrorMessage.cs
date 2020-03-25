using Infonet.CStoreCommander.Resources;
using System.Net;

namespace Infonet.CStoreCommander.BusinessLayer.Utilities
{
    public class ErrorMessage
    {
        public ErrorMessage()
        {
            MessageStyle = new MessageStyle();
        }

        public HttpStatusCode StatusCode { get; set; }

        public MessageStyle MessageStyle { get; set; }

        public bool ShutDownPos { get; set; }

        public int TillNumber { get; set; }

        public int ShiftNumber { get; set; }

        public decimal FloatAmount { get; set; }

        public string ShiftDate { get; set; }
       
    }
}
