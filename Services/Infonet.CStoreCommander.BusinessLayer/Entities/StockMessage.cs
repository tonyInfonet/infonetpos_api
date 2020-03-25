using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class StockMessage
    {
        public StockMessage()
        {
            AddStockPage = new AddStockPage();
            RestrictionPage = new RestrictionPage();
            GiftCertPage = new GiftCertPage();
            GivexPage = new GiftCardPage();
            RegularPriceMessage = new MessageStyle();
            QuantityMessage = new MessageStyle();
            PSInetPage = new PSInetPage();
        }

        public MessageStyle QuantityMessage { get; set; }

        public MessageStyle RegularPriceMessage { get; set; }

        public bool CanManuallyEnterProduct { get; set; }

        public string ManuallyEnterMessage { get; set; }

        public AddStockPage AddStockPage { get; set; }

        public RestrictionPage RestrictionPage { get; set; }

        public GiftCertPage GiftCertPage { get; set; }

        public GiftCardPage GivexPage { get; set; }
        public PSInetPage PSInetPage { get; set; }

    }

    public class AddStockPage
    {
        public bool OpenAddStockPage { get; set; }

        public string StockCode { get; set; }
    }

    public class RestrictionPage
    {
        public bool OpenRestrictionPage { get; set; }

        public string Description { get; set; }

    }

    public class GiftCertPage
    {
        public bool OpenGiftCertPage { get; set; }

        public string StockCode { get; set; }

        public string RegularPrice { get; set; }

        public int GiftNumber { get; set; }
    }

    public class GiftCardPage
    {
        public bool OpenGivexPage { get; set; }

        public string StockCode { get; set; }

        public string RegularPrice { get; set; }
    }
    public class PSInetPage
    {
        public bool OpenPSInetPage { get; set; }
        public string StockCode { get; set; }

        public string RegularPrice { get; set; }
    }
}
