using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TeCardholderManager : ManagerBase, ITeCardholderManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="customerService"></param>
        public TeCardholderManager(IPolicyManager policyManager,
            ICustomerService customerService)
        {
            _policyManager = policyManager;
            _customerService = customerService;
        }

        /// <summary>
        /// Method to get valid card holder
        /// </summary>
        /// <param name="cardHolder">Card holder</param>
        /// <param name="isBarCode">Bar code exists or not</param>
        /// <param name="strNumber">Number</param>
        /// <param name="matchCount">Match count</param>
        /// <returns>True or false</returns>
        public bool ValidCardHolder(ref teCardholder cardHolder, bool isBarCode, string strNumber, ref short matchCount)
        {
            cardHolder = _customerService.GetCardHolder(isBarCode, strNumber, out matchCount, _policyManager.AgeRestrict);
            
            return cardHolder.IsValidCardHolder;
        }


        /// <summary>
        /// Method to check if valid tax exempt customer
        /// </summary>
        /// <param name="cardHolder">Card holder</param>
        /// <param name="customerId">Customer Id</param>
        /// <returns>True or false</returns>
        public bool ValidTaxExemptCustomer(ref teCardholder cardHolder, string customerId)
        {
            cardHolder = _customerService.GetTaxExemptCustomer(customerId);
            var returnValue = cardHolder.IsValidCardHolder;
            return returnValue;
        }
        
    }
}
