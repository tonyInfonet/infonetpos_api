using System.Collections.Generic;
using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.WebApi.Models.Till
{
    /// <summary>
    /// Active tills
    /// </summary>
    public class ActiveTillResponseModel
    {
        /// <summary>
        /// Till numbers
        /// </summary>
        public List<Till> Tills { get; set; }

        /// <summary>
        /// Shift numbers
        /// </summary>
        public int ShiftNumber { get; set; }

        /// <summary>
        /// Shift date
        /// </summary>
        public string ShiftDate { get; set; }

        /// <summary>
        /// float amount
        /// </summary>
        public decimal CashFloat { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public MessageStyle Message { get; set; }

        /// <summary>
        /// Shut down POS
        /// </summary>
        public bool ShutDownPOS { get; set; }

        /// <summary>
        /// Force till
        /// </summary>
        public bool ForceTill { get; set; }

        /// <summary>
        /// Is Trainer
        /// </summary>
        public bool IsTrainer { get; set; }
    }

    public class Till
    {
        public int TillNumber { get; set; }
    }
}