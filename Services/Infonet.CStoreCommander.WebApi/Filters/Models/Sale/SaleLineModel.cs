using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Models.Sale
{

    /// <summary>
    /// Sale line model
    /// </summary>
    public class AddSaleLineModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public AddSaleLineModel()
        {
            GiftCard = new GiftCard();
        }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Register number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Stock code
        /// </summary>
        public string StockCode { get; set; }
        
        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gift card
        /// </summary>
        public GiftCard GiftCard { get; set; }

        /// <summary>
        /// Return mode
        /// </summary>
        public bool IsReturnMode { get; set; }

        /// <summary>
        /// Is Manually Added
        /// </summary>
        public bool IsManuallyAdded { get; set; }

    }

    /// <summary>
    /// Sale line model
    /// </summary>
    public class UpdateSaleLineModel
    {

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Register number
        /// </summary>
        public byte RegisterNumber { get; set; }


        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// Discount
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Discount type
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Reason code
        /// </summary>
        public string ReasonCode { get; set; }

        /// <summary>
        /// Reason type
        /// </summary>
        public string ReasonType { get; set; }

        /// <summary>
        /// Line number
        /// </summary>
        public int LineNumber { get; set; }


    }
}