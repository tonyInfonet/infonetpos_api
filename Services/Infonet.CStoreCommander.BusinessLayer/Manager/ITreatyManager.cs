using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITreatyManager
    {
        

        /// <summary>
        /// Get Quota
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="eProdType">Product type</param>
        /// <param name="dOutQuota">Quota</param>
        /// <returns>True or false</returns>
        bool GetQuota(ref teTreatyNo treatyNo, ref mPrivateGlobals.teProductEnum eProdType, ref double dOutQuota);

        /// <summary>
        /// Is Valid Treaty Number
        /// </summary>
        /// <param name="sTreatyNo">Treaty number</param>
        /// <param name="captureMethod">capture method</param>
        /// <param name="user">User</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        bool IsValidTreatyNo(ref string sTreatyNo, ref short captureMethod, User user, out ErrorMessage error);

        /// <summary>
        /// Method to set remaining tobacco quantity
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="oPurchaseList">Purchase list</param>
        /// <param name="sale">Sale</param>
        /// <param name="remainingTobaccoQuota">Remauning tobacco quota</param>
        void SetRemainingTobaccoQuantity(ref teTreatyNo treatyNo, ref tePurchaseList oPurchaseList,
            ref Sale sale, double remainingTobaccoQuota);

        /// <summary>
        /// Method to set remaining fuel quantity
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="oPurchaseList">Purchase list</param>
        /// <param name="sale">Sale</param>
        /// <param name="remainingFuelQuantity">Remaining fuel quantity</param>
        void SetRemainingFuelQuantity(ref teTreatyNo treatyNo, ref tePurchaseList oPurchaseList,
            ref Sale sale, double remainingFuelQuantity);

        /// <summary>
        /// Method to initialise treaty number
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="sTreatyNo">Treaty value</param>
        /// <param name="bSwiped">Swiped or not</param>
        /// <returns>True or false</returns>
        bool Init(ref teTreatyNo treatyNo, string sTreatyNo, bool bSwiped);

        /// <summary>
        /// Method to add to quota
        /// </summary>
        /// <param name="treatyNo">Treaty umber</param>
        /// <param name="eProdType">Product type</param>
        /// <param name="dQuantity">Quantity</param>
        /// <returns>True or false</returns>
        bool AddToQuota(teTreatyNo treatyNo, ref mPrivateGlobals.teProductEnum eProdType, double dQuantity);
    }
}