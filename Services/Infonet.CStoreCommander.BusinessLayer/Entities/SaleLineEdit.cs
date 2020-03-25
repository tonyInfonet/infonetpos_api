namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class SaleLineEdit
    {
        public int LineNumber { get; set; }
        public bool AllowPriceChange { get; set; }
        public bool AllowQuantityChange { get; set; }
        public bool AllowDiscountChange { get; set; }
        public bool AllowDiscountReason { get; set; }
        public bool AllowPriceReason { get; set; }
        public bool AllowReturnReason { get; set; }
        public bool ConfirmDelete { get; set; }
    }
}
