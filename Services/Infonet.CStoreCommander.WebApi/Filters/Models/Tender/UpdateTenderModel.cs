namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Update tender model
    /// </summary>
    public class UpdateTenderModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public UpdateTenderModel()
        {
            Tender = new SelectedTender();
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
        /// Till close
        /// </summary>
        public bool TillClose { get; set; }

        /// <summary>
        /// Selected tender
        /// </summary>
        public SelectedTender Tender { get; set; }
    }
}