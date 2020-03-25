using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace Infonet.CStoreCommander.ADOData
{
    public class SqlDbService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;
        private SqlCommand _command;
        private SqlDataReader _reader;

        /// <summary>
        /// Get Records 
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="source">Source</param>
        /// <returns>DataTable</returns>
        public DataTable GetRecords(string query, DataSource source)
        {
            _connection = new SqlConnection(GetConnectionString(source));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            return _dataTable;
        }


        /// <summary>
        /// Get Paged Records
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="source">Data source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Data table</returns>
        public DataTable GetPagedRecords(string query,
            DataSource source,
            int pageIndex,
            int pageSize = 100)
        {
            _connection = new SqlConnection(GetConnectionString(source));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill((pageIndex - 1) * pageSize, pageSize, _dataTable);
            _connection.Close();
            _adapter?.Dispose();
            return _dataTable;
        }


        /// <summary>
        /// Execute the query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public object Execute(string query, DataSource source)
        {
            _connection = new SqlConnection(GetConnectionString(source));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            SqlCommand cmd = new SqlCommand(query, _connection);
            int recordsAffected = cmd.ExecuteNonQuery();
            _connection.Close();
            cmd.Dispose();
            return recordsAffected;
        }

        /// <summary>
        /// Reset the connection 
        /// </summary>
        public void ResetConnection()
        {
            _dataTable.Dispose();
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Get connection string 
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns></returns>
        public string GetConnectionString(DataSource source)
        {
            switch (source)
            {
                case DataSource.CSCAdmin:
                    return
                        ConfigurationManager.ConnectionStrings["CSCAdmin1"].ConnectionString;
                case DataSource.CSCMaster:
                    return
                        ConfigurationManager.ConnectionStrings["CSCMaster1"].ConnectionString;
                case DataSource.CSCCurSale:
                    return
                        ConfigurationManager.ConnectionStrings["CSCCurSale1"].ConnectionString;
                case DataSource.CSCTills:
                    return
                        ConfigurationManager.ConnectionStrings["CSCTills1"].ConnectionString;
                case DataSource.CSCTrans:
                    return
                        ConfigurationManager.ConnectionStrings["CSCTrans1"].ConnectionString;
                case DataSource.CSCPump:
                    return
                        ConfigurationManager.ConnectionStrings["CSCPump1"].ConnectionString;
                case DataSource.CSCReader:
                    return
                        ConfigurationManager.ConnectionStrings["CSCReader1"].ConnectionString;
                case DataSource.CSCPayPump:
                    return
                        ConfigurationManager.ConnectionStrings["CSCPayPump1"].ConnectionString;

                case DataSource.CSCPayPumpHist:
                    return
                        ConfigurationManager.ConnectionStrings["CSCPayPumpHist1"].ConnectionString;
                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// Get the record count 
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="source">Data source</param>
        /// <returns>Record count</returns>
        public int GetRecordCount(string query,
           DataSource source)
        {
            _connection = new SqlConnection(GetConnectionString(source));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            _command = new SqlCommand(query, _connection);
            int returnValue = (int)_command.ExecuteScalar();
            _connection.Close();
            _command?.Dispose();
            return returnValue;
        }


        public SqlDataReader GetDataReaderRecords(string query, DataSource source)
        {
            _connection = new SqlConnection(GetConnectionString(source));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _command = new SqlCommand(query, _connection);
            _reader = _command.ExecuteReader();
            return _reader;
        }

    
        /// <summary>
        /// method to convert datatable to ADODB.Recordset
        /// </summary>
        /// <param name="inTable"></param>
        /// <returns></returns>
        static public ADODB.Recordset ConvertToRecordset(DataTable inTable)
        {
            try
            {
                ADODB.Recordset result = new ADODB.Recordset();
                result.CursorLocation = ADODB.CursorLocationEnum.adUseClient;

                ADODB.Fields resultFields = result.Fields;
                System.Data.DataColumnCollection inColumns = inTable.Columns;

                foreach (DataColumn inColumn in inColumns)
                {
                    resultFields.Append(inColumn.ColumnName
                        , TranslateType(inColumn.DataType)
                        , inColumn.MaxLength
                        , inColumn.AllowDBNull ? ADODB.FieldAttributeEnum.adFldIsNullable :
                                                 ADODB.FieldAttributeEnum.adFldUnspecified
                        , null);
                }

                result.Open(System.Reflection.Missing.Value
                        , System.Reflection.Missing.Value
                        , ADODB.CursorTypeEnum.adOpenStatic
                        , ADODB.LockTypeEnum.adLockOptimistic, 0);

                foreach (DataRow dr in inTable.Rows)
                {
                    result.AddNew(System.Reflection.Missing.Value,
                                  System.Reflection.Missing.Value);

                    for (int columnIndex = 0; columnIndex < inColumns.Count; columnIndex++)
                    {
                        resultFields[columnIndex].Value = dr[columnIndex];
                    }
                }


                return result;
            }
            catch (Exception ex)
            {
                return new ADODB.Recordset();
            }
        }

        /// <summary>
        /// helper method of ConvertToRecordset
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        static ADODB.DataTypeEnum TranslateType(Type columnType)
        {
            switch (columnType.UnderlyingSystemType.ToString())
            {
                case "System.Boolean":
                    return ADODB.DataTypeEnum.adBoolean;

                case "System.Byte":
                    return ADODB.DataTypeEnum.adUnsignedTinyInt;

                case "System.Char":
                    return ADODB.DataTypeEnum.adChar;

                case "System.DateTime":
                    return ADODB.DataTypeEnum.adDate;

                case "System.Decimal":
                    return ADODB.DataTypeEnum.adCurrency;

                case "System.Double":
                    return ADODB.DataTypeEnum.adDouble;

                case "System.Int16":
                    return ADODB.DataTypeEnum.adSmallInt;

                case "System.Int32":
                    return ADODB.DataTypeEnum.adInteger;

                case "System.Int64":
                    return ADODB.DataTypeEnum.adBigInt;

                case "System.SByte":
                    return ADODB.DataTypeEnum.adTinyInt;

                case "System.Single":
                    return ADODB.DataTypeEnum.adSingle;

                case "System.UInt16":
                    return ADODB.DataTypeEnum.adUnsignedSmallInt;

                case "System.UInt32":
                    return ADODB.DataTypeEnum.adUnsignedInt;

                case "System.UInt64":
                    return ADODB.DataTypeEnum.adUnsignedBigInt;

                case "System.String":
                default:
                    return ADODB.DataTypeEnum.adVarChar;
            }
            
        }
        //public static ADODB.Recordset Get_Records(string Source, DataSource DB, int CursorType = 2, int LockType = 3, int Options = 1, int CursorLocation = 3) //VBConversions Note: 2 =  ADODB.CursorTypeEnum.adOpenDynamic. Integer value had to be used as default to prevent CS1750 error. //VBConversions Note: 3 =  ADODB.LockTypeEnum.adLockOptimistic. Integer value had to be used as default to prevent CS1750 error. //VBConversions Note: 1 =  ADODB.CommandTypeEnum.adCmdText. Integer value had to be used as default to prevent CS1750 error. //VBConversions Note: 3 =  ADODB.CursorLocationEnum.adUseClient. Integer value had to be used as default to prevent CS1750 error.
        //{
        //    ADODB.Recordset returnValue = default(ADODB.Recordset);

        //    ADODB.Recordset rs = new ADODB.Recordset();
        //    rs.CursorLocation = (ADODB.CursorLocationEnum)CursorLocation; // shiny October 2, 2007 -added a parameter cursor location as part of Performance audit - Now we can open recordset in Serverside or client side, default is clientside. If you want server side pass cursor loccation as aduseserver(2)
        //    rs.Open(Source, DB, (ADODB.CursorTypeEnum)CursorType, (ADODB.LockTypeEnum)LockType, Options);
        //    returnValue = rs;
        //    rs = null;

        //    return returnValue;
        //}
        public static DataTable GetTableSchema(string sCon, string tblName)
        {
            string[] val = tblName.Split('.');

            string sSQL = "select * from " + val[0] + ".INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + val[2] + "'";
            SqlDataAdapter oDA = null;
            SqlConnection oCon = new SqlConnection(sCon);
            DataTable dt = null;
            try
            {
                dt = new DataTable();
                oDA = new SqlDataAdapter(sSQL, oCon);
                oDA.Fill(dt);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }
        public static DataTable GetTableTemplate(string sTableName, string sConnection)
        {
            DataTable dt = null;
            try
            {
                string sSQL = "select * from " + sTableName + " where 0=1";
                SqlConnection oCon = new SqlConnection(sConnection);
                SqlCommand oCom = new SqlCommand(sSQL, oCon);

                SqlDataAdapter oDA = new SqlDataAdapter(oCom);
                dt = new DataTable();
                oDA.Fill(dt);

                oDA.Dispose();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }
        public static SqlDataAdapter CreateInsertSqlDataAdapter(string sTableName, SqlConnection oCon, DataTable dtSchema)
        {
            string strSQL = "select * from " + sTableName + " where 0=1";
            string strSQL1;
            string sName;
            //DataTable dtSchema = GetTableSchema(oCon.ConnectionString, sTableName);


            SqlDataAdapter oDA = new SqlDataAdapter(strSQL, oCon);

            try
            {
                oDA.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                strSQL = "";
                strSQL1 = "";
                foreach (DataRow drow in dtSchema.Rows)
                {
                    sName = drow["COLUMN_NAME"].ToString();
                    strSQL = strSQL + "[" + sName + "],";
                    strSQL1 = strSQL1 + "@" + sName + ",";


                }
                strSQL = strSQL.Substring(0, strSQL.Length - 1);
                strSQL1 = strSQL1.Substring(0, strSQL1.Length - 1);
                strSQL = "insert into " + sTableName + " (" + strSQL + ") values (" + strSQL1 + ")";

                oDA.InsertCommand = new SqlCommand(strSQL, oCon);
                oDA.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
                foreach (DataRow drow in dtSchema.Rows)
                {
                    AddParameter(drow["COLUMN_NAME"].ToString(), oDA.InsertCommand, dtSchema);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return oDA;
        }
        public static void AddParameter(string sParamName, SqlCommand oCom, DataTable dtSchema)
        {

            try
            {
                DataRow[] dr = dtSchema.Select("COLUMN_NAME='" + sParamName + "'");
                string sDataType = dr[0]["DATA_TYPE"].ToString();
                int iSize;
                switch (sDataType)
                {
                    case "bigint":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.BigInt, 8, sParamName);
                        break;
                    case "bit":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Bit, 1, sParamName);
                        break;
                    case "tinyint":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.TinyInt, 1, sParamName);
                        break;
                    case "int":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Int, 4, sParamName);
                        break;
                    case "smallint":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.SmallInt, 2, sParamName);
                        break;
                    case "money":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Money, 8, sParamName);
                        break;
                    case "smallmoney":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.SmallMoney, 4, sParamName);
                        break;
                    case "decimal":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Decimal, 8, sParamName);
                        break;
                    case "real":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Real, 4, sParamName);
                        break;
                    case "float":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Float, 8, sParamName);
                        break;
                    case "numeric":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Decimal, 8, sParamName);
                        break;
                    case "smalldatetime":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.SmallDateTime, 4, sParamName);
                        break;
                    case "datetime":
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.DateTime, 8, sParamName);
                        break;
                    case "char":
                        iSize = (int)dr[0]["CHARACTER_MAXIMUM_LENGTH"];
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Char, iSize, sParamName);
                        break;
                    case "nchar":
                        iSize = (int)dr[0]["CHARACTER_MAXIMUM_LENGTH"];
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.NChar, iSize, sParamName);
                        break;
                    case "varchar":
                        iSize = (int)dr[0]["CHARACTER_MAXIMUM_LENGTH"];
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.VarChar, iSize, sParamName);
                        break;
                    case "nvarchar":
                        iSize = (int)dr[0]["CHARACTER_MAXIMUM_LENGTH"];
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.NVarChar, iSize, sParamName);
                        break;
                    case "ntext":
                        iSize = (int)dr[0]["CHARACTER_MAXIMUM_LENGTH"];
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.NText, iSize, sParamName);
                        break;
                    case "image":
                        iSize = (int)dr[0]["CHARACTER_MAXIMUM_LENGTH"];
                        oCom.Parameters.Add("@" + sParamName, SqlDbType.Image, iSize, sParamName);
                        break;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public static void InitializeRow(DataRow dr, DataTable dtSchema)
        {
            try
            {
                //Assign null to each nullable column
                DataRow[] drs = dtSchema.Select("IS_NULLABLE='YES'");
                foreach (DataRow r in drs)
                {
                    dr[r["COLUMN_NAME"].ToString()] = DBNull.Value;
                }
                //Assign defaul value
                foreach (DataRow r in dtSchema.Rows)
                {
                    if (r["COLUMN_DEFAULT"] != DBNull.Value)
                    {
                        switch (r["COLUMN_DEFAULT"].ToString())
                        {
                            case "(0)":
                            case "((0))":
                            case "(0.00)":
                            case "((0.00))":
                                dr[r["COLUMN_NAME"].ToString()] = 0;
                                break;
                            case "(1)":
                            case "((1))":
                                dr[r["COLUMN_NAME"].ToString()] = 1;
                                break;
                            case "(getdate())":
                                dr[r["COLUMN_NAME"].ToString()] = DateTime.Now;
                                break;
                            case "(N'NEWTANK')":
                                dr[r["COLUMN_NAME"].ToString()] = "NEWTANK";
                                break;
                            case "('')":
                                dr[r["COLUMN_NAME"].ToString()] = "";
                                break;
                            case "('E')":
                                dr[r["COLUMN_NAME"].ToString()] = "E";
                                break;
                            case "('F')":
                                dr[r["COLUMN_NAME"].ToString()] = "F";
                                break;
                            case "(N' ')":
                                dr[r["COLUMN_NAME"].ToString()] = " ";
                                break;
                            case "(N'P')":
                                dr[r["COLUMN_NAME"].ToString()] = "P";
                                break;
                            case "(N'R')":
                                dr[r["COLUMN_NAME"].ToString()] = "R";
                                break;
                            case "(N'V')":
                                dr[r["COLUMN_NAME"].ToString()] = "V";
                                break;
                            case "(N'$')":
                                dr[r["COLUMN_NAME"].ToString()] = "$";
                                break;
                            case "(N'E')":
                                dr[r["COLUMN_NAME"].ToString()] = "E";
                                break;
                            case "(N'O')":
                                dr[r["COLUMN_NAME"].ToString()] = "O";
                                break;
                            case "(N'+')":
                                dr[r["COLUMN_NAME"].ToString()] = "+";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static DataTable GetDBRecords(string query, DataSource source)
        {
            using (SqlConnection oCon = new SqlConnection(GetConnection(source)))
            {
                DataTable dt = new DataTable();
                SqlDataAdapter oda = new SqlDataAdapter(query, oCon);
                oda.Fill(dt);
                oda.Dispose();
                oCon.Dispose();
                return dt;
            }
        }
        public void BatchInsert(string schema, DataTable data, DataSource source)
        {
            using (SqlConnection oCon = new SqlConnection(GetConnectionString(source)))
            {
                SqlDataAdapter oda = new SqlDataAdapter(schema, oCon);
                new SqlCommandBuilder(oda);
                oda.Update(data);
            }
        }
        public static object ExecuteCmd(string cmd, DataSource source)
        {
            using (SqlConnection oCon = new SqlConnection(GetConnection(source)))
            {
                oCon.Open();
                SqlCommand ocmd = new SqlCommand(cmd, oCon);
                int recordsAffected = ocmd.ExecuteNonQuery();
                oCon.Close();
                ocmd.Dispose();
                return recordsAffected;
            }
        }
        private static string GetConnection(DataSource source)
        {
            switch (source)
            {
                case DataSource.CSCAdmin:
                    return
                        ConfigurationManager.ConnectionStrings["CSCAdmin1"].ConnectionString;
                case DataSource.CSCMaster:
                    return
                        ConfigurationManager.ConnectionStrings["CSCMaster1"].ConnectionString;
                case DataSource.CSCCurSale:
                    return
                        ConfigurationManager.ConnectionStrings["CSCCurSale1"].ConnectionString;
                case DataSource.CSCTills:
                    return
                        ConfigurationManager.ConnectionStrings["CSCTills1"].ConnectionString;
                case DataSource.CSCTrans:
                    return
                        ConfigurationManager.ConnectionStrings["CSCTrans1"].ConnectionString;
                case DataSource.CSCPump:
                    return
                        ConfigurationManager.ConnectionStrings["CSCPump1"].ConnectionString;
                case DataSource.CSCReader:
                    return
                        ConfigurationManager.ConnectionStrings["CSCReader1"].ConnectionString;
                case DataSource.CSCPayPump:
                    return
                        ConfigurationManager.ConnectionStrings["CSCPayPump1"].ConnectionString;

                case DataSource.CSCPayPumpHist:
                    return
                        ConfigurationManager.ConnectionStrings["CSCPayPumpHist1"].ConnectionString;
                default:
                    return string.Empty;
            }
        }
    }
}

