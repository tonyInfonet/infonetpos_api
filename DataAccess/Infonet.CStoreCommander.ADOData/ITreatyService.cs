using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ITreatyService
    {
        /// <summary>
        /// Add or Update the Treaty Name
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="treatyName">Treaty name</param>
        /// <returns>Treaty name</returns>
        string AddorUpdateTreatyName(string treatyNumber, string treatyName);

        /// <summary>
        /// Get Treaty Name
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <returns>Treaty name</returns>
        string GetTreatyName(string treatyNumber);

        /// <summary>
        /// Get Cigarette equivalent units
        /// </summary>
        /// <param name="productType">Product type</param>
        /// <returns>Units</returns>
        double GetCigaretteEquivalentUnits(string productType);


        /// <summary>
        /// Get Quota
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <returns>Quota</returns>
        double GetQuota(string fieldName, string treatyNumber);

        /// <summary>
        /// Is Invalid certificate
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <returns>True or false</returns>
        bool IsInvalidCertificate(string treatyNumber);

        /// <summary>
        /// Method to update treaty number
        /// </summary>
        /// <param name="query">Query</param>
        void UpdateTreatyNumber(string query);

        /// <summary>
        /// Method to get list of treaty numbers
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>List of treaty numbers</returns>
        List<TreatyNo> GetTreatyNumbers(string query);

    }
}