using ADODB;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace Infonet.CStoreCommander.ADOData
{
    public class SignatureService : SqlDbService, ISignatureService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Save Signature
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="imageFilePath">Image file path</param>
        /// <returns>True or false</returns>
        public bool SaveSignature(int tillNumber, int saleNumber, string imageFilePath)
        {
            bool returnValue = false;
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("Select * from Signature  where Sale_No = " + Convert.ToString(saleNumber) + "  and Till_num = " + Convert.ToString(tillNumber), _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var rsFields = _dataTable.NewRow();

                if (SavePictureToDb(rsFields, imageFilePath))
                {
                    rsFields["Sale_No"] = saleNumber;
                    rsFields["Till_Num"] = tillNumber;
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _dataTable.Rows.Add(rsFields);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(_dataTable);
                    _connection.Close();
                    _adapter?.Dispose();
                    returnValue = true;
                }
            }
            return returnValue;
        }


        /// <summary>
        /// Save Picture to DB
        /// </summary>
        /// <param name="rsFields">Data row</param>
        /// <param name="sFileName">File name</param>
        /// <returns>True or false</returns>
        private dynamic SavePictureToDb(DataRow rsFields, string sFileName)
        {
            bool returnValue = false;
            ADODB.Stream strStream = new ADODB.Stream();

            try
            {
                if (!File.Exists(sFileName))
                {
                    return false;
                }

                strStream = new ADODB.Stream { Type = StreamTypeEnum.adTypeBinary };
                strStream.Open();
                strStream.LoadFromFile(sFileName);

                rsFields["Signature"] = strStream.Read();

                returnValue = true;
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                strStream.Close();
            }
            return returnValue;
        }

    }
}
