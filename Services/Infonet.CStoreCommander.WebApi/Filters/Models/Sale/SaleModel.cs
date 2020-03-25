using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// Sale model
    /// </summary>
    public class SaleModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SaleModel()
        {
            SaleLines = new List<SaleLine>();
        }

        /// <summary>
        /// Till number
        /// </summary>
        public byte TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Customer
        /// </summary>
        public string Customer { get; set; }
        
        /// <summary>
        /// Enable exact change
        /// </summary>
        public bool EnableExactChange { get; set; }

        /// <summary>
        /// Enable write off
        /// </summary>
        public bool EnableWriteOffButton { get; set; }

        /// <summary>
        /// Sale lines
        /// </summary>
        public List<SaleLine> SaleLines { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public string TotalAmount { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Sale line errors
        /// </summary>
        public List<object> SaleLineErrors { get; set; }

        /// <summary>
        /// Text for customer display
        /// </summary>
        public CustomerDisplay CustomerDisplayText { get; set; }

        /// <summary>
        /// Sale has any cawash product or not
        /// </summary>
        public bool HasCarwashProducts { get; set; }

    }
}