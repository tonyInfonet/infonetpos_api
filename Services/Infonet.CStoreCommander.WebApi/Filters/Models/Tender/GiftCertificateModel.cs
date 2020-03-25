using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Gift certificate model
    /// </summary>
    public class GiftCertificateModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public GiftCertificateModel()
        {
            GiftCerts = new List<GiftCertInput>();
        }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }

        /// <summary>
        /// Gift certs
        /// </summary>
        public List<GiftCertInput> GiftCerts { get; set; }
    }
}