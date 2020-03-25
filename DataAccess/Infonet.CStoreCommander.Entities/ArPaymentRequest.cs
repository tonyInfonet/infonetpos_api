namespace Infonet.CStoreCommander.Entities
{
    public class ArPaymentRequest
    {
        public string CustomerCode { get; set; }

        public decimal Amount { get; set; }

        public int SaleNumber { get; set; }

        public int TillNumber { get; set; }

        public byte RegisterNumber { get; set; }

        public bool IsReturnMode { get; set; }
    }
}
