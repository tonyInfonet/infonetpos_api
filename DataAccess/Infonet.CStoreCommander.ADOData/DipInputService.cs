using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class DipInputService : SqlDbService, IDipInputService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Get Dip Input values
        /// </summary>
        /// <returns>List of dip input</returns>
        public List<DipInput> GetDipInputValues()
        {
            List<DipInput> dipInputs = new List<DipInput>();
            var strDate = DateAndTime.Year(DateAndTime.Today) + Strings.Right("00" + CommonUtility.GetStringValue(DateAndTime.Month(DateAndTime.Today)), 2) + Strings.Right("00" + CommonUtility.GetStringValue(DateAndTime.Day(DateAndTime.Today)), 2);
            var strSql = "select A.ID, A.GradeID, B.FullName from TankInfo as A INNER JOIN Grade as B ON A.GradeID=B.ID where B.FuelType=\'G\'";
            var dt = GetRecords(strSql, DataSource.CSCPump);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DipInput input = new DipInput
                    {
                        TankId = CommonUtility.GetStringValue(dr["ID"]),
                        Grade = CommonUtility.GetStringValue(dr["FullName"]),
                        GradeId = CommonUtility.GetStringValue(dr["GradeID"])
                    };
                    string query = "select * from DipInput where TankID=" + CommonUtility.GetStringValue(dr["ID"]) + " and CONVERT(char(8), DipDate, 112)=\'" + strDate + "\'";
                    var rsDip = GetRecords(query, DataSource.CSCTrans);
                    if (rsDip == null || rsDip.Rows.Count == 0)
                    {
                        input.DipValue = "";
                    }
                    else
                    {
                        input.DipValue = CommonUtility.GetStringValue(rsDip.Rows[0]["DipValue"]);
                    }
                    dipInputs.Add(input);
                }
            }
            return dipInputs;
        }


        /// <summary>
        /// Get Maximum Dip value For Tank No
        /// </summary>
        /// <param name="tankno">Tank number</param>
        /// <returns>Maximum dip</returns>
        public float MaximumDip(byte tankno)
        {
            float returnValue;

            var dt = GetRecords("Select TankCode from TankInfo where ID = " + CommonUtility.GetStringValue(tankno), DataSource.CSCPump);
            if (dt != null && dt.Rows.Count > 0)
            {
                var rs = GetRecords("SELECT Length, Diameter, Capacity, TankEnds, Orientation,ReadingType, UseChart FROM   TankType WHERE  TankCode = \'" + CommonUtility.GetStringValue(dt.Rows[0]["TankCode"]) + "\' ", DataSource.CSCPump);
                if (rs != null && rs.Rows.Count > 0)
                {
                    if (Strings.UCase(CommonUtility.GetStringValue(rs.Rows[0]["Readingtype"])) == "G")
                    {
                        returnValue = 100;
                    }
                    else
                    {
                        if (CommonUtility.GetBooleanValue(rs.Rows[0]["UseChart"]) == false)
                        {
                            if (Strings.UCase(CommonUtility.GetStringValue(rs.Rows[0]["Orientation"])) == "H")
                            {
                                // For horizontal tanks it's the diameter
                                returnValue = CommonUtility.GetFloatValue(rs.Rows[0]["Diameter"]);
                            }
                            else
                            {
                                // For vertical tanks its the length
                                returnValue = CommonUtility.GetFloatValue(rs.Rows[0]["length"]);
                            }
                        }
                        else
                        {
                            var rsMax = GetRecords("select max(Depth) as [MaxDip] from TankChart Where   DipChart = \'" + CommonUtility.GetStringValue(dt.Rows[0]["TankCode"]) + "\' ", DataSource.CSCPump);
                            if (rsMax != null && rsMax.Rows.Count > 0)
                            {
                                returnValue = CommonUtility.GetFloatValue(rsMax.Rows[0]["maxdip"]);
                            }
                            else
                            {
                                returnValue = 0;
                            }
                        }
                    }
                }
                else
                {
                    // If the Tank Type wasn't found in the database then
                    // load dummy values.
                    returnValue = 0;
                }
            }
            else
            {
                returnValue = 0;
            }
            return returnValue;
        }


        /// <summary>
        /// Is Tank Exists
        /// </summary>
        /// <param name="tankno">Tank number</param>
        /// <param name="gradeId">Grade id</param>
        /// <returns>True or false</returns>
        public bool IsTankExists(byte tankno, int gradeId)
        {
            var rstank = GetRecords("Select TankCode from TankInfo where ID = " + CommonUtility.GetStringValue(tankno) + "AND GradeID=" + CommonUtility.GetStringValue(gradeId), DataSource.CSCPump);
            if (rstank != null && rstank.Rows.Count != 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Save DipInputs
        /// </summary>
        /// <param name="dipInputs">List of dip inputs</param>
        public void SaveDipInputs(List<DipInput> dipInputs)
        {
            foreach (DipInput dipInput in dipInputs)
            {
                var addNew = false;
                var strSql = "select * from DipInput where TankID=" + Convert.ToString(dipInput.TankId);
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter(strSql, _connection);
                _adapter.Fill(_dataTable);
                DataRow fields;
                if (_dataTable.Rows.Count == 0)
                {
                    fields = _dataTable.NewRow();
                    fields["TankID"] = CommonUtility.GetDoubleValue(dipInput.TankId);
                    addNew = true;
                }
                else
                {
                    fields = _dataTable.Rows[0];
                }
                fields["grade"] = CommonUtility.GetDoubleValue(dipInput.GradeId);
                fields["DipValue"] = CommonUtility.GetDoubleValue(dipInput.DipValue);
                fields["dipdate"] = DateTime.Now;
                if (addNew)
                {
                    _dataTable.Rows.Add(fields);
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                    _adapter.Update(_dataTable);
                }
                else
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(_dataTable);
                }
                _connection.Close();
                _adapter?.Dispose();
            }
        }


        /// <summary>
        /// Get Dip Inputs for Report
        /// </summary>
        /// <returns>List of dip inputs</returns>
        public List<DipInput> GetDipInputsForReport(string strDate)
        {
            List<DipInput> dipInputs = new List<DipInput>();

            var dt = GetRecords("select * from DipInput where CONVERT(char(8), DipDate, 112)=\'" + strDate + "\'", DataSource.CSCTrans);

            foreach (DataRow dr in dt.Rows)
            {
                DipInput input = new DipInput
                {
                    TankId = CommonUtility.GetStringValue(dr["TankID"]),
                    Grade = CommonUtility.GetStringValue(dr["Grade"]),
                    DipValue = CommonUtility.GetStringValue(dr["DipValue"])
                };
                dipInputs.Add(input);
            }
            return dipInputs;
        }

    }
}
