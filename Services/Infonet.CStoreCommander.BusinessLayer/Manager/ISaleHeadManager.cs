using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public  interface ISaleHeadManager
    { 

        /// <summary>
        /// Load the tax definitions
        /// </summary>
        void Load_Taxes(ref Sale sale);

        /// <summary>
        /// Method to create a new sale
        /// </summary>
        /// <returns></returns>
        Sale CreateNewSale();

        /// <summary>
        /// Method to set sale policies
        /// </summary>
        /// <param name="sale">Sale</param>
        void SetSalePolicies(ref Sale sale);
    }
}
