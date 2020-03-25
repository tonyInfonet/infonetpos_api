using System;

namespace Infonet.CStoreCommander.Entities
{
    public class CloseBatch
    {
        public string BatchNumber { get; set; }

        public string TerminalId { get; set; }

        public DateTime Date { get; set; }

        public DateTime Time { get; set; }

        public string Report { get; set; }
    }
}
