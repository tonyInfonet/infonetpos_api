using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class ReasonService : SqlDbService, IReasonService
    {

        #region CSCMaster services 

        /// <summary>
        /// Method to get list of reasons
        /// </summary>
        /// <param name="type">Reason type</param>
        /// <returns>List of reasons</returns>
        public List<Return_Reason> GetReasons(char type)
        {
            var reasons = new List<Return_Reason>();
            if (type == '\0') return reasons;
            var rs = GetRecords(" SELECT Code, Reason FROM Reasons WHERE Type=\'" + type + "\' ORDER BY Code", DataSource.CSCMaster);
            reasons.AddRange(from DataRow field in rs.Rows
                select new Return_Reason
                {
                    Reason = CommonUtility.GetStringValue(field["Code"]),
                    Description = CommonUtility.GetStringValue(field["Reason"])
                });
            return reasons;
        }

        /// <summary>
        /// Method to get reason type name
        /// </summary>
        /// <param name="type">Reason type</param>
        /// <returns>List of reasons</returns>
        public string GetReasonType(char type)
        {
            var rsType = GetRecords("SELECT * FROM ReasonType  WHERE R_Type=\'" + type + "\'", DataSource.CSCMaster);
            if (rsType.Rows.Count == 0) return string.Empty;
            var fields = rsType.Rows[0];
            return CommonUtility.GetStringValue(fields["Description"]);
        }

        /// <summary>
        /// Method to get description according to code and reason type
        /// </summary>
        /// <param name="code">Reason code</param>
        /// <param name="type">Reason type</param>
        /// <returns>Description</returns>
        public string GetDescription(string code, char type)
        {
            var rs = GetRecords("select * from reasons  WHERE code=\'" + code + "\' and type=\'" + type + "\'", DataSource.CSCMaster);
            if (rs.Rows.Count == 0) return string.Empty;
            var fields = rs.Rows[0];
            return CommonUtility.GetStringValue(fields["reason"]);
        }

        public Return_Reason GetReturnReason(string code, char type)
        {
            var query = "select * from reasons  WHERE code=\'" + code + "\' and type=\'" + type + "\'";
            var rs = GetRecords(query, DataSource.CSCMaster);

            if (rs.Rows.Count == 0) return null;
            var fields = rs.Rows[0];
            return new Return_Reason
            {
                Description = CommonUtility.GetStringValue(fields["reason"]),
                Reason = code,
                RType = type.ToString()
            };
        }

        /// <summary>
        /// Method to get tax exemption reasons
        /// </summary>
        /// <param name="reasonType">Reason type</param>
        /// <returns>Tax exemption reasons</returns>
        public List<TaxExemptReason> GetTaxExemptReasons(string reasonType)
        {
            var reasons = new List<TaxExemptReason>();
            var rsReason = GetRecords("select * from TaxExemptReasons Where ReasonType=\'" + reasonType + "\'", DataSource.CSCMaster);
            foreach (DataRow fields in rsReason.Rows)
            {
                var ter = new TaxExemptReason
                {
                    Code = CommonUtility.GetStringValue(fields["ReasonCode"]),
                    Description = CommonUtility.GetStringValue(fields["ReasonDescription"]),
                    ExplanationCode = CommonUtility.GetShortValue(fields["ExplanationCode"]),
                    Explanation =
                        string.IsNullOrEmpty(CommonUtility.GetStringValue(fields["Explanation"]))
                            ? "0"
                            : CommonUtility.GetStringValue(fields["Explanation"])
                };
                reasons.Add(ter);
            }
            return reasons;
        }

        #endregion
    }
}
