using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Givex
{
    /// <summary>
    /// Givex model
    /// </summary>
    public class GivexModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public string GivexCardNumber { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Stock code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal GivexPrice { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }
       
    }


    /// <summary>
    /// Givex Model for Close Batch
    /// </summary>
    public class GivexCloseBatchModel
    {       
        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }
    }


    /// <summary>
    /// Activate Givex model
    /// </summary>
    public class ActivateGivexModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public string GivexCardNumber { get; set; }
        
        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Stock code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal GivexPrice { get; set; }

    }


    /// <summary>
    /// Activate Givex model
    /// </summary>
    public class AdjustGivexModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public string GivexCardNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Stock code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Amount { get; set; }

    }


    /// <summary>
    /// Givex Report model
    /// </summary>
    public class GivexReportModel
    {
        /// <summary>
        /// Givex Details
        /// </summary>
        public List<GivexDetails> Details { get; set; }
        
        /// <summary>
        /// Stock code
        /// </summary>
        public Entities.Report GivexReport { get; set; }
                
    }

    /// <summary>
    /// Givex Details
    /// </summary>
    public class GivexDetails
    {
        public int Id { get; set; }

        public decimal CashOut { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string Report { get; set; }
    }
}