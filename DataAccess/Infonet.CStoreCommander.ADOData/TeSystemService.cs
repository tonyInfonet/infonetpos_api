using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class TeSystemService : SqlDbService, ITeSystemService
    {
        /// <summary>
        /// Method to get admin value
        /// </summary>
        /// <param name="adminName">Admin value</param>
        /// <returns>Admin value</returns>
        public object GetAdminValue(string adminName)
        {
            object result = null;

            var sSql = "SELECT Value FROM Admin WHERE Name=\'" + adminName + "\'";
            var oRecs = GetRecords(sSql, DataSource.CSCAdmin);
            if (oRecs.Rows.Count > 0)
            {
                result = oRecs.Rows[0]["Value"];
            }
            return result;
        }

        /// <summary>
        /// Method to check if limit is required
        /// </summary>
        /// <param name="productType">Product type</param>
        /// <param name="isError">Is error not not</param>
        /// <returns>True or false</returns>
        public bool IsLimitRequired(int productType, out bool isError)
        {
            var sSql = "SELECT LimitProductSale FROM Category WHERE ID= " + Convert.ToString(productType) + " ";

            var oRecs = GetRecords(sSql, DataSource.CSCMaster);

            if (oRecs.Rows.Count == 0)
            {
                isError = true;
                return false;
            }
            isError = false;
            bool returnValue = CommonUtility.GetBooleanValue(oRecs.Rows[0]["LimitProductSale"]);
            return returnValue;
        }

        /// <summary>
        /// Method to get report values
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Report value</returns>
        public object GetReportValues(string key)
        {
            object result = null;
            var sSql = "SELECT* FROM Receipt where ID=\'" + key + "\'";
            var oRecs = GetRecords(sSql, DataSource.CSCMaster);
            if (oRecs.Rows.Count > 0)
            {
                result = oRecs.Rows[0]["Sale_Foot"];
            }
            return result;
        }

        /// <summary>
        /// Method to get list of override codes
        /// </summary>
        /// <param name="isError">Is error not not</param>
        /// <returns>List of override codes</returns>
        public List<OverrideCode> GetOverrideCodes(out bool isError)
        {
            isError = false;
            var sSql = "SELECT * FROM OverrideCode";
            var oRecs = GetRecords(sSql, DataSource.CSCMaster);

            if (oRecs.Rows.Count == 0)
            {
                isError = true;
                return null;
            }
            return (from DataRow row in oRecs.Rows
                select new OverrideCode
                {
                    Code = CommonUtility.GetStringValue(row["Code"]),
                    Name = CommonUtility.GetStringValue(row["Name"]),
                    Id = CommonUtility.GetStringValue(row["ID"]),
                    IsTobacco = CommonUtility.GetStringValue(CommonUtility.GetBooleanValue(row["bTobacco"]) ? "True" : "False"),
                    CanBeUsedForTobaccoMaxThreshold = CommonUtility.GetStringValue(CommonUtility.GetBooleanValue(row["bCanBeUsedForTobaccoMaxThreshold"]) ? "True" : "False")
                }).ToList();
        }
    }
}
