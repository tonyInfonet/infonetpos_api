using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Models.Till
{
    /// <summary>
    /// Active shift response model
    /// </summary>
    public class ActiveShiftResponseModel
    {
        /// <summary>
        /// Shifts
        /// </summary>
        public List<Shift> Shifts { get; set; }

        /// <summary>
        /// Shifts used for day
        /// </summary>
        public  bool ShiftsUsedForDay { get; set; }

        /// <summary>
        /// Cash float
        /// </summary>
        public decimal CashFloat { get; set; }

        /// <summary>
        /// Force shift
        /// </summary>
        public bool ForceShift { get; set; }
    }
}