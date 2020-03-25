using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITeCardholderManager
    {
        /// <summary>
        /// Method to get valid card holder
        /// </summary>
        /// <param name="cardHolder">Card holder</param>
        /// <param name="isBarCode">Bar code exists or not</param>
        /// <param name="strNumber">Number</param>
        /// <param name="matchCount">Match count</param>
        /// <returns>True or false</returns>
        bool ValidCardHolder(ref teCardholder cardHolder, bool isBarCode, string strNumber,
            ref short matchCount);

        /// <summary>
        /// Method to check if valid tax exempt customer
        /// </summary>
        /// <param name="cardHolder">Card holder</param>
        /// <param name="customerId">Customer Id</param>
        /// <returns>True or false</returns>
        bool ValidTaxExemptCustomer(ref teCardholder cardHolder, string customerId);
    }
}