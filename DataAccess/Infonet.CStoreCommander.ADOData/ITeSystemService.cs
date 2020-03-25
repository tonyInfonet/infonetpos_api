using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ITeSystemService
    {
        /// <summary>
        /// Method to get admin value
        /// </summary>
        /// <param name="adminName">Admin value</param>
        /// <returns>Admin value</returns>
        object GetAdminValue(string adminName);

        /// <summary>
        /// Method to check if limit is required
        /// </summary>
        /// <param name="productType">Product type</param>
        /// <param name="isError">Is error not not</param>
        /// <returns>True or false</returns>
        bool IsLimitRequired(int productType, out bool isError);

        /// <summary>
        /// Method to get report values
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Report value</returns>
        object GetReportValues(string key);

        /// <summary>
        /// Method to get list of override codes
        /// </summary>
        /// <param name="isError">Is error not not</param>
        /// <returns>List of override codes</returns>
        List<OverrideCode> GetOverrideCodes(out bool isError);
    }
}