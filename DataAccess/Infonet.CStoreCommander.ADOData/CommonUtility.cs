using System;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Common Utility 
    /// </summary>
    public class CommonUtility
    {
        /// <summary>
        /// Get String Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(object value)
        {
            return DBNull.Value.Equals(value) ? string.Empty : Convert.ToString(value);
        }

        /// <summary>
        /// Get Date time Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeValue(object value)
        {
            DateTime dt;
            DateTime.TryParse(value.ToString(), out dt);
            return dt;
        }

        /// <summary>
        /// Get Boolean Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool GetBooleanValue(object value)
        {
            return !DBNull.Value.Equals(value) && Convert.ToBoolean(value);
        }

        /// <summary>
        /// Get Double Value 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDoubleValue(object value)
        {
            return DBNull.Value.Equals(value) ? 0 : Convert.ToDouble(value);
        }

        /// <summary>
        /// Get Interger Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetIntergerValue(object value)
        {
            return DBNull.Value.Equals(value) ? 0 : Convert.ToInt32(value);
        }

        /// <summary>
        /// Get Short Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short GetShortValue(object value)
        {
            return DBNull.Value.Equals(value) ? (short)0 : Convert.ToInt16(value);
        }

        /// <summary>
        /// Gets the Byte value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetByteValue(object value)
        {
            return DBNull.Value.Equals(value) ? (byte)0 : Convert.ToByte(value);
        }

        /// <summary>
        /// Gets the Float value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetFloatValue(object value)
        {
            return DBNull.Value.Equals(value) ? 0 : Convert.ToSingle(value);
        }

        /// <summary>
        /// Gets the Decimal value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetDecimalValue(object value)
        {
            return DBNull.Value.Equals(value) ? 0 : Convert.ToDecimal(value);
        }

        /// <summary>
        /// Get Char Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char GetCharValue(object value)
        {
            return DBNull.Value.Equals(value) ? '\0' : Convert.ToChar(value);
        }
        private static object syncObject = new object();
        public static string GetPSTransactionID()
        {
            lock (syncObject)
            {
                var dt = SqlDbService.GetDBRecords("select isnull(TransactionNo,0)   from [dbo].[PSProfile]", DataSource.CSCMaster);
                int TransID = (int)dt.Rows[0][0];
                if (TransID == 0 || TransID == int.MaxValue)
                {
                    TransID = 1;
                }
                else
                {
                    TransID += 1;
                }
                SqlDbService.ExecuteCmd("update [dbo].[PSProfile] set TransactionNo=" + TransID, DataSource.CSCMaster);
                return string.Format("{0:0000000000}", TransID);
            }
        }
    }
}
