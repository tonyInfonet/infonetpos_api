using ADODB;
using System.Configuration;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Db Service
    /// </summary>
    public class DbService
    {
        private Connection _connection;
        private Recordset _recordSet;

        /// <summary>
        /// Get Records 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <param name="cursorType"></param>
        /// <param name="lockType"></param>
        /// <param name="options"></param>
        /// <param name="cursorLocation"></param>
        /// <returns>RecordSet</returns>
        public Recordset GetRecords(string query,
            DataSource source,
            CursorTypeEnum cursorType = CursorTypeEnum.adOpenDynamic,
            LockTypeEnum lockType = LockTypeEnum.adLockOptimistic,
            CommandTypeEnum options = CommandTypeEnum.adCmdText,
            CursorLocationEnum cursorLocation = CursorLocationEnum.adUseClient)
        {
            _connection = new Connection();

            var cnStr = GetConnectionString(source);

            _connection.Open(cnStr, null, null, 0);

            _recordSet = new Recordset { CursorLocation = cursorLocation };

            _recordSet.Open(query, _connection, cursorType, lockType, (int)options);
            return _recordSet;
        }


        /// <summary>
        /// Get Paged Records
        /// </summary>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursorType"></param>
        /// <param name="lockType"></param>
        /// <param name="options"></param>
        /// <param name="cursorLocation"></param>
        /// <returns></returns>
        public Recordset GetPagedRecords(string query,
            DataSource source,
            int pageIndex,
            int pageSize = 100,
            CursorTypeEnum cursorType = CursorTypeEnum.adOpenDynamic,
            LockTypeEnum lockType = LockTypeEnum.adLockOptimistic,
            CommandTypeEnum options = CommandTypeEnum.adCmdText,
            CursorLocationEnum cursorLocation = CursorLocationEnum.adUseClient)
        {
            _connection = new Connection();

            var cnStr = GetConnectionString(source);

            _connection.Open(cnStr, null, null, 0);

            _recordSet = new Recordset
            {
                CursorLocation = cursorLocation,
                CacheSize = pageSize,
                PageSize = pageSize
            };

            //_recordSet.Open(query,_connection, cursorType, lockType, (int)options);
            _recordSet.Open(query, _connection);

            if (_recordSet.RecordCount != 0)
            {
                _recordSet.AbsolutePage = (PositionEnum)pageIndex;
            }
            return _recordSet;
        }


        /// <summary>
        /// Execute the query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public object Execute(string query, DataSource source)
        {
            _connection = new Connection();

            var cnStr = GetConnectionString(source);

            _connection.Open(cnStr, null, null, 0);

            object recordsAffected;
            _connection.Execute(query, out recordsAffected);
            return recordsAffected;
        }

        /// <summary>
        /// Reset the connection 
        /// </summary>
        public void ResetConnection()
        {
            _recordSet.Close();
            _connection.Close();
        }

        /// <summary>
        /// Get connection string 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetConnectionString(DataSource source)
        {
            switch (source)
            {
                case DataSource.CSCAdmin:
                    return
                        ConfigurationManager.ConnectionStrings["CSCAdmin"].ConnectionString;
                case DataSource.CSCMaster:
                    return
                        ConfigurationManager.ConnectionStrings["CSCMaster"].ConnectionString;
                case DataSource.CSCCurSale:
                    return
                        ConfigurationManager.ConnectionStrings["CSCCurSale"].ConnectionString;
                case DataSource.CSCTills:
                    return
                        ConfigurationManager.ConnectionStrings["CSCTills"].ConnectionString;
                case DataSource.CSCTrans:
                    return
                        ConfigurationManager.ConnectionStrings["CSCTrans"].ConnectionString;
                case DataSource.CSCPump:
                    return
                        ConfigurationManager.ConnectionStrings["CSCPump"].ConnectionString;
                case DataSource.CSCReader:
                    return
                        ConfigurationManager.ConnectionStrings["CSCReader"].ConnectionString;
                default:
                    return string.Empty;
            }
        }

        
        /// <summary>
        /// Get the record count 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <param name="cursorType"></param>
        /// <param name="lockType"></param>
        /// <param name="options"></param>
        /// <param name="cursorLocation"></param>
        /// <returns></returns>
        public short GetRecordCount(string query,
           DataSource source,
           CursorTypeEnum cursorType = CursorTypeEnum.adOpenDynamic,
           LockTypeEnum lockType = LockTypeEnum.adLockOptimistic,
           CommandTypeEnum options = CommandTypeEnum.adCmdText,
           CursorLocationEnum cursorLocation = CursorLocationEnum.adUseClient)
        {
            _connection = new Connection();

            var cnStr = GetConnectionString(source);

            _connection.Open(cnStr, null, null, 0);

            _recordSet = new Recordset { CursorLocation = cursorLocation };

            _recordSet.Open(query, _connection);

            var returnValue = (short)_recordSet.RecordCount;
            _recordSet = null;
            return returnValue;
        }

    }
}
