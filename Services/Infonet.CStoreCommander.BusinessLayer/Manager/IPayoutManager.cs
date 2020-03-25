using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.IO;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPayoutManager
    {
        /// <summary>
        /// Method to get payout vendor
        /// </summary>
        /// <param name="error">Error</param>
        /// <returns>Vendor payout</returns>
        VendorPayout GetPayoutVendor(out ErrorMessage error);

        /// <summary>
        /// Method to save vendor payout
        /// </summary>
        /// <param name="po">Payout</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="taxes">Taxes</param>
        /// <param name="openDrawer">Open cash drawer</param>
        /// <param name="error">Error message</param>
        /// <returns>Report</returns>
        Report SaveVendorPayout(Payout po, int tillNumber, string userCode, byte registerNumber,
         List<Tax> taxes, out bool openDrawer, out ErrorMessage error);

        /// <summary>
        /// Method to validate payout by fleet card
        /// </summary>
        /// <param name="allowSwipe">Allow Swipe</param>
        /// <param name="error">Error message</param>
        /// <returns>Caption</returns>
        string ValidateFleetPayout(out bool allowSwipe, out ErrorMessage error);
    }
}
