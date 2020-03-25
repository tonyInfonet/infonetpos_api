using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Shift Service
    /// </summary>
    public class ShiftService : SqlDbService, IShiftService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Get Shifts
        /// </summary>
        /// <param name="active"></param>
        /// <returns>Shifts</returns>
        public List<ShiftStore> GetShifts(byte? active)
        {
            var sShifts = active.HasValue ?
                GetRecords("select * from shiftstore where active =" + active, DataSource.CSCMaster)
                : GetRecords("select * from shiftstore order by ShiftNumber", DataSource.CSCMaster);
            var shifts = new List<ShiftStore>();

            foreach (DataRow fields in sShifts.Rows)
            {
                var shift = new ShiftStore
                {
                    ShiftNumber = CommonUtility.GetIntergerValue(fields["ShiftNumber"]),
                    CurrentDay = CommonUtility.GetByteValue(fields["CurrentDay"]),
                    Active = CommonUtility.GetByteValue(fields["Active"]),
                    StartTime = CommonUtility.GetDateTimeValue(fields["StartTime"])
                };
                shifts.Add(shift);
            }
            return shifts;
        }

        /// <summary>
        /// Get the Shift by Shift Number
        /// </summary>
        /// <param name="shiftNumber"></param>
        /// <param name="active"></param>
        /// <returns>Shift</returns>
        public ShiftStore GetShift(int shiftNumber, int active)
        {
            var sShift = GetRecords("SELECT * from ShiftStore A where A.ShiftNumber > " + shiftNumber + " and A.Active =" + active, DataSource.CSCMaster);
            if (sShift == null || sShift.Rows.Count == 0) return null;
            var fields = sShift.Rows[0];
            var shift = new ShiftStore
            {
                ShiftNumber = CommonUtility.GetIntergerValue(fields["ShiftNumber"]),
                CurrentDay = CommonUtility.GetByteValue(fields["CurrentDay"]),
                Active = CommonUtility.GetByteValue(fields["Active"]),
                StartTime = CommonUtility.GetDateTimeValue(fields["StartTime"])
            };

            return shift;
        }

        /// <summary>
        /// Get the next Shift 
        /// </summary>
        /// <param name="shiftNumber"></param>
        /// <param name="active"></param>
        /// <param name="tillNumber"></param>
        /// <returns>Shifts</returns>
        public List<ShiftStore> GetNextShift(int shiftNumber, int active, int tillNumber)
        {
            var innerQuery = "Select ShiftNumber from Tills where Active = " + active + " and Till_Num = " + tillNumber;
            var sqlQuery = "SELECT * from ShiftStore A where A.ShiftNumber > " + shiftNumber + " and A.Active =" + active + " and ShiftNumber Not in ( " + innerQuery + ") ORDER BY A.ShiftNumber";
            var sShifts = GetRecords(sqlQuery, DataSource.CSCMaster);
            var shifts = new List<ShiftStore>();

            foreach (DataRow fields in sShifts.Rows)
            {
                var shift = new ShiftStore
                {
                    ShiftNumber = CommonUtility.GetIntergerValue(fields["ShiftNumber"]),
                    CurrentDay = CommonUtility.GetByteValue(fields["CurrentDay"]),
                    Active = CommonUtility.GetByteValue(fields["Active"]),
                    StartTime = CommonUtility.GetDateTimeValue(fields["StartTime"])
                };
                shifts.Add(shift);
            }
            return shifts;

        }

        /// <summary>
        /// Get the next active Shift
        /// </summary>
        /// <param name="active"></param>
        /// <param name="tillNumber"></param>
        /// <returns>Shift</returns>
        public List<ShiftStore> GetNextActiveShift(int active, int tillNumber)
        {
            var innerQuery = "SELECT ShiftNumber from tills where active = " + active + "and Till_Num =" + tillNumber;
            var sqlQuery = "SELECT * from ShiftStore Where Active = " + active + " and (ShiftNumber Not in (" + innerQuery + ")) ORDER BY ShiftNumber";
            var sShifts = GetRecords(sqlQuery, DataSource.CSCMaster);
            var shifts = new List<ShiftStore>();
            foreach (DataRow fields in sShifts.Rows)
            {
                var shift = new ShiftStore
                {
                    ShiftNumber = CommonUtility.GetIntergerValue(fields["ShiftNumber"]),
                    CurrentDay = CommonUtility.GetByteValue(fields["CurrentDay"]),
                    Active = CommonUtility.GetByteValue(fields["Active"]),
                    StartTime = CommonUtility.GetDateTimeValue(fields["StartTime"])
                };
                shifts.Add(shift);
            }
            return shifts;

        }

        /// <summary>
        /// Get the maximum Shift Number
        /// </summary>
        /// <returns></returns>
        public int GetMaximumShiftNumber()
        {
            var sShift = GetRecords("Select Max(ShiftStore.ShiftNumber) as [MaxShift]  FROM   ShiftStore ", DataSource.CSCMaster);

            var fields = sShift.Rows[0];

            return CommonUtility.GetIntergerValue(fields["MaxShift"]);
        }

        /// <summary>
        /// Update Shift
        /// </summary>
        /// <param name="shift"></param>
        /// <returns>Shift</returns>
        public ShiftStore UpdateShift(ShiftStore shift)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from ShiftStore where ShiftNumber=" + shift.ShiftNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var fields = _dataTable.Rows[0];
                fields["Active"] = shift.Active;
                fields["CurrentDay"] = shift.CurrentDay;
                fields["StartTime"] = shift.StartTime;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            return shift;
        }

        /// <summary>
        /// Get shift by Shift number
        /// </summary>
        /// <param name="shiftNumber"></param>
        /// <returns>Shift</returns>
        public ShiftStore GetShiftByShiftNumber(int shiftNumber)
        {
            var sShift = GetRecords("SELECT * from ShiftStore A where A.ShiftNumber =" + shiftNumber, DataSource.CSCMaster);
            if (sShift != null && sShift.Rows.Count != 0)
            {
                var fields = sShift.Rows[0];

                var shift = new ShiftStore
                {
                    ShiftNumber = CommonUtility.GetIntergerValue(fields["ShiftNumber"]),
                    CurrentDay = CommonUtility.GetByteValue(fields["CurrentDay"]),
                    Active = CommonUtility.GetByteValue(fields["Active"]),
                    StartTime = CommonUtility.GetDateTimeValue(fields["StartTime"])
                };
                return shift;
            }
            return null;
        }
    }
}
