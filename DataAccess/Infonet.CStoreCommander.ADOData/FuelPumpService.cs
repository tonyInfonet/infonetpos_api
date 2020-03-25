using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class FuelPumpService : SqlDbService, IFuelPumpService
    {
        private SqlConnection _connection;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to initilaize get property
        /// </summary>
        /// <param name="posId">Pos Id</param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="isAuthPumpPos">Is authenticated POS</param>
        /// <returns>Property</returns>
        public GetProperty InitializeGetProperty(int posId, bool isTaxExempt, bool isTeByRate,
            string teType, bool isAuthPumpPos)
        {
            GetProperty result = new GetProperty();
            short i;

            var cmdtxt = "SELECT MAX(ID) FROM Pump where manual=0";
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.PumpsCount = CommonUtility.GetShortValue(objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT MAX(ID) FROM Grade ";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.GradesCount = CommonUtility.GetShortValue(objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT MAX(PlaceOnScreen) FROM pump ";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.MaxPlaceOnScreen = CommonUtility.GetShortValue(objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT COUNT(*) FROM TankInfo ";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.TanksCount = CommonUtility.GetShortValue(objRec.Rows[0][0]);
            }

            for (i = 1; i <= result.PumpsCount; i++)
            {
                cmdtxt = "SELECT MAX(PositionID) FROM Assignment WHERE PumpID = " + Conversion.Str(i);
                objRec = GetRecords(cmdtxt, DataSource.CSCPump);
                if (objRec.Rows.Count > 0)
                {
                    result.PositionsCount[i] =
                        CommonUtility.GetByteValue(Information.IsDBNull(objRec.Rows[0][0]) ? 0 : objRec.Rows[0][0]);
                }
            }

            for (i = 1; i <= 9; i++)
            {
                cmdtxt = "SELECT * FROM Grade WHERE ID = " + Conversion.Str(i);
                objRec = GetRecords(cmdtxt, DataSource.CSCPump);
                if (objRec.Rows.Count > 0)
                {
                    result.GradeIsExist[i] = true;
                }
                else
                {
                    result.GradeIsExist[i] = false;
                }
            }


            cmdtxt = "SELECT count(*) FROM GradePriceIncrement ";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.UnBaseGradesCount =
                    CommonUtility.GetShortValue(Information.IsDBNull(objRec.Rows[0][0]) ? 0 : objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT count(*) FROM GradePriceIncrement " +
                     " INNER JOIN Grade ON GradePriceIncrement.GradeID=Grade.ID " + " where Grade.FuelType=\'G\'";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.UnBaseGasGradesCount =
                    CommonUtility.GetShortValue(Information.IsDBNull(objRec.Rows[0][0]) ? 0 : objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT count(*) FROM TierLevelPriceDiff";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.UnBaseTierLevelCount =
                    CommonUtility.GetShortValue(Information.IsDBNull(objRec.Rows[0][0]) ? 0 : objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT MAX(ID) FROM ServiceLevel";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.LevelsCount =
                    CommonUtility.GetByteValue(Information.IsDBNull(objRec.Rows[0][0]) ? 0 : objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT * FROM ServiceLevel";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            foreach (DataRow dataRow in objRec.Rows)
            {
                result.Level[CommonUtility.GetIntergerValue(dataRow["ID"])] =
                    CommonUtility.GetStringValue(dataRow["Description"]);
            }

            cmdtxt = "SELECT MAX(ID) FROM Tier";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                result.TiersCount =
                    CommonUtility.GetByteValue(Information.IsDBNull(objRec.Rows[0][0]) ? 0 : objRec.Rows[0][0]);
            }

            cmdtxt = "SELECT * FROM Tier ";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            foreach (DataRow dataRow in objRec.Rows)
            {
                result.Tier[CommonUtility.GetIntergerValue(dataRow["ID"])] =
                    CommonUtility.GetStringValue(dataRow["Description"]);
            }

            cmdtxt = "SELECT * FROM Setup";
            DataTable dataTable = new DataTable();
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                result.Pump_Space = CommonUtility.GetByteValue(dataTable.Rows[0]["Pump_Space"]);
                result.BasketInterval = CommonUtility.GetFloatValue(dataTable.Rows[0]["Basket_Interval"]);
                result.BrdCst_Value = CommonUtility.GetBooleanValue(dataTable.Rows[0]["BrdCst_Value"]);
                result.WriteLogFile = CommonUtility.GetBooleanValue(dataTable.Rows[0]["WriteLogFile"]);
                result.ComPort = CommonUtility.GetByteValue(dataTable.Rows[0]["ComPort"]);
                result.PumpLogFile = CommonUtility.GetBooleanValue(dataTable.Rows[0]["PumpLogFile"]);
                result.PriceDisplayComPort =
                    CommonUtility.GetByteValue(Information.IsDBNull(dataTable.Rows[0]["PriceDisplayComPort"])
                        ? 3
                        : dataTable.Rows[0]["PriceDisplayComPort"]);
                result.PriceDisplayRows =
                    CommonUtility.GetByteValue(Information.IsDBNull(dataTable.Rows[0]["PriceDisplayRows"])
                        ? 1
                        : dataTable.Rows[0]["PriceDisplayRows"]);
                result.PoolingTimer = CommonUtility.GetShortValue(dataTable.Rows[0]["PoolingTimer"]);
                result.SoundInterval =
                    CommonUtility.GetFloatValue(Information.IsDBNull(dataTable.Rows[0]["SoundInterval"])
                        ? 1000
                        : dataTable.Rows[0]["SoundInterval"]);
                result.ClickDelay =
                    CommonUtility.GetByteValue(Information.IsDBNull(dataTable.Rows[0]["ClickDelay"])
                        ? 0
                        : dataTable.Rows[0]["ClickDelay"]);
                result.SpacesCount = CommonUtility.GetStringValue(dataTable.Rows[0][0]);
                result.CommunicationTimeOut = CommonUtility.GetByteValue(dataTable.Rows[0]["CommunicationTimeOut"]);
                if (Information.IsDBNull(dataTable.Rows[0]["RepeatSoundRatio"]))
                {
                    result.RepeatSoundRatio = 3;
                    dataTable.Rows[0]["RepeatSoundRatio"] = 3;
                }
                else
                {
                    result.RepeatSoundRatio = CommonUtility.GetByteValue(dataTable.Rows[0]["RepeatSoundRatio"]);
                }

                if (Information.IsDBNull(dataTable.Rows[0]["ReadTotDelay"]))
                {
                    result.ReadTotDelay = 5;
                    dataTable.Rows[0]["ReadTotDelay"] = 5;
                }
                else
                {
                    result.ReadTotDelay = CommonUtility.GetByteValue(dataTable.Rows[0]["ReadTotDelay"]);
                }
                try
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(dataTable);
                }
                catch
                {
                    result.Pump_Space = 0;
                    result.BasketInterval = 1000;
                    result.BrdCst_Value = true;
                    result.WriteLogFile = false;
                    result.ComPort = 1;
                    result.PriceDisplayComPort = 3;
                    result.PriceDisplayRows = 1;
                    result.PumpLogFile = false;
                    result.PoolingTimer = 300;
                    result.SoundInterval = 1000;
                    result.SpacesCount = 0.ToString();
                    result.ClickDelay = 0;
                    result.CommunicationTimeOut = 0;
                    result.RepeatSoundRatio = 3;
                    result.ReadTotDelay = 5;
                }
                _connection.Close();
                _adapter?.Dispose();
            }
            else
            {
                result.Pump_Space = 0;
                result.BasketInterval = 1000;
                result.BrdCst_Value = true;
                result.WriteLogFile = false;
                result.ComPort = 1;
                result.PriceDisplayComPort = 3;
                result.PriceDisplayRows = 1;
                result.PumpLogFile = false;
                result.PoolingTimer = 300;
                result.SoundInterval = 1000;
                result.SpacesCount = 0.ToString();
                result.ClickDelay = 0;
                result.CommunicationTimeOut = 0;
                result.RepeatSoundRatio = 3;
                result.ReadTotDelay = 5;
            }


            if (result.CommunicationTimeOut == 0)
            {
                result.CommunicationTimeOut = 20;
            }
            if (result.RepeatSoundRatio < 1)
            {
                result.RepeatSoundRatio = 1;
            }


            cmdtxt = "SELECT * FROM TankType ";
            objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            result.UnitMeasurement = objRec.Rows.Count > 0
                ? CommonUtility.GetStringValue(Information.IsDBNull(objRec.Rows[0][10]) ? "" : objRec.Rows[0][1])
                : "N";
            Read_AllData(ref result, posId, isTaxExempt, isTeByRate, teType, isAuthPumpPos);
            return result;
        }

        /// <summary>
        /// Method to read all data
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        /// <param name="posId">POS id</param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="isAuthPumpPos">Is authenticated POS</param>
        public void Read_AllData(ref GetProperty fuelProperties, int posId, bool isTaxExempt,
            bool isTeByRate, string teType, bool isAuthPumpPos)
        {
            Read_Assignment(ref fuelProperties);
            Read_Grade(ref fuelProperties);
            Read_FuelPrice(ref fuelProperties, isTaxExempt, isTeByRate, teType);
            Read_FCIP(ref fuelProperties);
            Read_pump(ref fuelProperties, posId, isAuthPumpPos);
        }

        /// <summary>
        /// Method to read assignment
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        public void Read_Assignment(ref GetProperty fuelProperties)
        {
            var cmdtxt = "SELECT * FROM Assignment ";
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            foreach (DataRow dataRow in objRec.Rows)
            {
                var p = CommonUtility.GetByteValue(dataRow["PumpID"]);
                var ps = CommonUtility.GetByteValue(dataRow["PositionID"]);
                fuelProperties.Assignment[p, ps] = new CAssignment
                {
                    GradeID = dataRow["GradeID"],
                    TankID = dataRow["TankID"],
                    RegTank = dataRow["RegTank"],
                    PremTank = dataRow["PremTank"]
                };
            }
        }

        /// <summary>
        /// Method to read grade
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        public void Read_Grade(ref GetProperty fuelProperties)
        {
            var cmdtxt = "SELECT * FROM Grade ";
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            foreach (DataRow dataRow in objRec.Rows)
            {
                var id = CommonUtility.GetByteValue(dataRow["ID"]);
                fuelProperties.Grade[id] = new CGrade
                {
                    ShortName = dataRow["ShortName"],
                    FullName = dataRow["FullName"],
                    Stock_Code = dataRow["Stock_Code"],
                    SubDept = dataRow["SubDept"],
                    Ratio = dataRow["Ratio"],
                    FeedStock = dataRow["FeedStock"],
                    Price_diff = dataRow["Price_diff"],
                    StockGradeID = dataRow["StockGradeID"],
                    FuelType = dataRow["FuelType"]
                };
            }
        }

        /// <summary>
        /// Method to read fuel price
        /// </summary>
        /// <param name="fuelProperties"></param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        public void Read_FuelPrice(ref GetProperty fuelProperties, bool isTaxExempt,
            bool isTeByRate, string teType)
        {
            var cmdtxt = "SELECT * FROM FuelPrice  ";
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);

            foreach (DataRow dataRow in objRec.Rows)
            {
                var g = CommonUtility.GetByteValue(dataRow["GradeID"]);
                var T = CommonUtility.GetByteValue(dataRow["TierID"]);
                var l = CommonUtility.GetByteValue(dataRow["LevelID"]);
                fuelProperties.FuelPrice[g, T, l] = new CFuelPrice();
                fuelProperties.FuelPrice[g, T, l].CashPrice = CommonUtility.GetFloatValue(dataRow["CashPrice"]);
                fuelProperties.FuelPrice[g, T, l].CreditPrice = CommonUtility.GetFloatValue(dataRow["CreditPrice"]);
                fuelProperties.FuelPrice[g, T, l].EmplID = dataRow["EmplID"];
                fuelProperties.FuelPrice[g, T, l].ReportID = CommonUtility.GetIntergerValue(dataRow["ReportID"]);
                fuelProperties.FuelPrice[g, T, l].Date_Time =
                    (int)CommonUtility.GetDateTimeValue(dataRow["Date_Time"]).ToOADate();

                fuelProperties.FuelPrice[g, T, l].teCashPrice = 0;
                fuelProperties.FuelPrice[g, T, l].teCreditPrice = 0;
                if (!isTaxExempt) continue;
                if (isTeByRate && teType == "SITE")
                {
                    fuelProperties.FuelPrice[g, T, l].teCashPrice = CommonUtility.GetFloatValue(dataRow["TaxFreePrice"]);
                    fuelProperties.FuelPrice[g, T, l].teCreditPrice =
                        CommonUtility.GetFloatValue(dataRow["TaxFreeCreditPrice"]);
                }
                else
                {
                    fuelProperties.FuelPrice[g, T, l].teCashPrice = TE_Fuel_Price_ByRate(g,
                        fuelProperties.FuelPrice[g, T, l].CashPrice, fuelProperties);
                    fuelProperties.FuelPrice[g, T, l].teCreditPrice = TE_Fuel_Price_ByRate(g,
                        fuelProperties.FuelPrice[g, T, l].CreditPrice, fuelProperties);
                }
            }
        }

        /// <summary>
        /// Method to read FCIP
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        public void Read_FCIP(ref GetProperty fuelProperties)
        {

            var objRec = GetRecords("SELECT * FROM IP", DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                DataRow dataRow = objRec.Rows[0];
                fuelProperties.IP = new CIP
                {
                    FC_IP = dataRow["FC_IP"],
                    FC_TCP_Port = dataRow["FC_TCP_Port"],
                    FC_UDP_Port = dataRow["FC_UDP_Port"],
                    POS_TCP_Port = dataRow["POS_TCP_Port"],
                    POS_UDP_Port = dataRow["POS_UDP_Port"]
                };
            }
        }

        /// <summary>
        /// Method to read pump
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        /// <param name="posId">POS id</param>
        /// <param name="isAuthPumpPos">Is pump authenticated</param>
        public void Read_pump(ref GetProperty fuelProperties, int posId, bool isAuthPumpPos)
        {
            var cmdtxt = "SELECT * FROM Pump ";
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            foreach (DataRow dataRow in objRec.Rows)
            {
                var id = CommonUtility.GetByteValue(dataRow["ID"]);
                fuelProperties.Pump[id] = new CPump
                {
                    Model = dataRow["Model"],
                    Title = dataRow["Title"],
                    PlaceOnScreen = dataRow["PlaceOnScreen"],
                    TierID = dataRow["TierID"],
                    LevelID = dataRow["LevelID"],
                    CutOff = dataRow["CutOff"],
                    AutoPay = dataRow["AutoPay"],
                    AutoAuthorize = dataRow["AutoAuthorize"],
                    CashierAuthorize = dataRow["CashierAuthorize"],
                    PrepayOnly = dataRow["PrepayOnly"],
                    AllowRegularSale = dataRow["AllowRegularSale"],
                    ManualPump = CommonUtility.GetBooleanValue(dataRow["Manual"]),
                    AllowPostPay = CommonUtility.GetBooleanValue(dataRow["AllowPostPay"]),
                    AllowPreauth = CommonUtility.GetBooleanValue(dataRow["AllowPreauth"])
                };

                
                var rsTemp =
                    GetRecords(
                        "select Max(PositionID) as MaxPos from Assignment" + " where PumpID=" +
                        CommonUtility.GetStringValue(id), DataSource.CSCPump);
                if (Information.IsDBNull(rsTemp.Rows[0]["MaxPos"]))
                {
                    fuelProperties.Pump[id].MaxPositionID = 0;
                }
                else
                {
                    fuelProperties.Pump[id].MaxPositionID = CommonUtility.GetShortValue(rsTemp.Rows[0]["MaxPos"]);
                }

                // AuthorizeFromTill being reset after price change
                if (isAuthPumpPos)
                {
                    var rsPumpTill =
                        GetRecords("select * from PumpByPOSID where POSID=" + CommonUtility.GetStringValue(posId),
                            DataSource.CSCMaster);
                    if (rsPumpTill.Rows.Count == 0)
                    {
                        fuelProperties.Pump[id].AuthorizeFromTill = false;
                    }
                    else
                    {
                        fuelProperties.Pump[id].AuthorizeFromTill = CommonUtility.GetBooleanValue(rsPumpTill.Rows[0][id]);
                    }
                }
            }
        }

        /// <summary>
        /// Method to get tax exempt fuel price by rate
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="regPrice">Regular price</param>
        /// <param name="fuelProperties">Fuel properties</param>
        /// <returns>Tax exempt fuel price</returns>
        private float TE_Fuel_Price_ByRate(short gradeId, float regPrice, GetProperty fuelProperties)
        {
            var returnValue = float.Parse(regPrice.ToString("#0.000"));
            var rs =
                GetRecords(
                    "select TaxExemptRate.TAX_RATE as TAX_RATE,  TaxExemptRate.RateType as RateType from TaxExemptRate INNER JOIN ProductTaxExempt ON TaxExemptRate.TAX_CODE=ProductTaxExempt.TAX_CODE where ProductTaxExempt.ProductKey=\'" +
                    CommonUtility.GetStringValue(fuelProperties.get_Grade((byte)gradeId).Stock_Code) + "\' ",
                    DataSource.CSCMaster);
            if (rs.Rows.Count == 0)
            {
                return returnValue;
            }
            float taxRate = CommonUtility.GetFloatValue(rs.Rows[0]["Tax_Rate"]);
            string rateType =
                CommonUtility.GetStringValue(Information.IsDBNull(rs.Rows[0]["RateType"])
                    ? "$"
                    : Strings.Trim(CommonUtility.GetStringValue(rs.Rows[0]["RateType"])));
            switch (rateType)
            {
                case "%":
                    returnValue = float.Parse((regPrice * (1 - taxRate / 100)).ToString("#0.000"));
                    break;
                case "$":
                    returnValue = float.Parse((regPrice - taxRate).ToString("#0.000"));
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get prices to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Prices to display</returns>
        public CPricesToDisplay get_PricesToDisplay(byte row)
        {
            CPricesToDisplay returnValue = new CPricesToDisplay();
            var cmdtxt = "SELECT * FROM PricesToDisplay WHERE Row = " + CommonUtility.GetStringValue(row);
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                returnValue.GradeID = CommonUtility.GetByteValue(objRec.Rows[0]["GradeID"]);
                returnValue.LevelID = CommonUtility.GetByteValue(objRec.Rows[0]["LevelID"]);
                returnValue.TierID = CommonUtility.GetByteValue(objRec.Rows[0]["TierID"]);
            }
            return returnValue;
        }

        /// <summary>
        /// Method to set prices to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="value">Value</param>
        public void set_PricesToDisplay(byte row, CPricesToDisplay value)
        {
            var cmdtxt = "SELECT * FROM PricesToDisplay WHERE Row = " + CommonUtility.GetStringValue(row);

            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var objRec = new DataTable();
            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(objRec);
            var builder = new SqlCommandBuilder(_adapter);
            if (objRec.Rows.Count == 0)
            {
                var dataRow = objRec.NewRow();
                dataRow["GradeID"] = value.GradeID;
                dataRow["LevelID"] = value.LevelID;
                dataRow["TierID"] = value.TierID;
                objRec.Rows.Add(dataRow);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                objRec.Rows[0]["GradeID"] = value.GradeID;
                objRec.Rows[0]["LevelID"] = value.LevelID;
                objRec.Rows[0]["TierID"] = value.TierID;
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            _adapter.Update(objRec);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to get tier level price difference
        /// </summary>
        /// <param name="tier">Tier</param>
        /// <param name="level">Level</param>
        /// <returns>Tier level price difference</returns>
        public CTierLevelPriceDiff get_TierLevelPriceDiff(byte tier, byte level)
        {
            CTierLevelPriceDiff returnValue = new CTierLevelPriceDiff();
            var cmdtxt = "SELECT * FROM TierLevelPriceDiff WHERE Tier = " + CommonUtility.GetStringValue(tier) +
                         " and Level =" + CommonUtility.GetStringValue(level);
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count <= 0) return returnValue;
            returnValue.CashDiff = objRec.Rows[0]["CashDiff"];
            returnValue.CreditDiff = objRec.Rows[0]["CreditDiff"];
            returnValue.TaxExemptCashDiff = objRec.Rows[0]["TaxExemptCashDiff"];
            returnValue.TaxExemptCreditDiff = objRec.Rows[0]["TaxExemptCreditDiff"];
            return returnValue;
        }

        /// <summary>
        /// Method to set tier level price difference
        /// </summary>
        /// <param name="tier">Tier</param>
        /// <param name="level">Level</param>
        /// <param name="value">Tier level price difference</param>
        public void set_TierLevelPriceDiff(byte tier, byte level, CTierLevelPriceDiff value)
        {
            var cmdtxt = "SELECT * FROM TierLevelPriceDiff WHERE Tier = " + CommonUtility.GetStringValue(tier) +
                         " and Level =" + CommonUtility.GetStringValue(level);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var objRec = new DataTable();
            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(objRec);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            if (objRec.Rows.Count == 0)
            {
                var dataRow = objRec.NewRow();
                dataRow["CashDiff"] = value.CashDiff ?? DBNull.Value;
                dataRow["CreditDiff"] = value.CreditDiff ?? DBNull.Value;
                dataRow["TaxExemptCashDiff"] = value.TaxExemptCashDiff ?? DBNull.Value;
                dataRow["TaxExemptCreditDiff"] = value.TaxExemptCreditDiff ?? DBNull.Value;
                objRec.Rows.Add(dataRow);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                objRec.Rows[0]["CashDiff"] = value.CashDiff ?? DBNull.Value;
                objRec.Rows[0]["CreditDiff"] = value.CreditDiff ?? DBNull.Value;
                objRec.Rows[0]["TaxExemptCashDiff"] = value.TaxExemptCashDiff ?? DBNull.Value;
                objRec.Rows[0]["TaxExemptCreditDiff"] = value.TaxExemptCreditDiff ?? DBNull.Value;
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            _adapter.Update(objRec);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to get grade price increment
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <returns>Grade price increment</returns>
        public CGradePriceIncrement get_GradePriceIncrement(byte gradeId)
        {
            CGradePriceIncrement returnValue = new CGradePriceIncrement();
            var cmdtxt = "SELECT * FROM GradePriceIncrement WHERE GradeID = " + CommonUtility.GetStringValue(gradeId);
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                returnValue.CashPriceIncre = objRec.Rows[0]["CashPriceIncre"];
                returnValue.CreditPriceIncre = objRec.Rows[0]["CreditPriceIncre"];
                returnValue.TaxExemptCashPriceIncre = objRec.Rows[0]["TaxExemptCashPriceIncre"];
                returnValue.TaxExemptCreditPriceIncre = objRec.Rows[0]["TaxExemptCreditPriceIncre"];
            }
            return returnValue;
        }

        /// <summary>
        /// Method to set grade price increment
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="value">Grade price increment</param>
        public void set_GradePriceIncrement(byte gradeId, CGradePriceIncrement value)
        {
            var cmdtxt = "SELECT * FROM GradePriceIncrement WHERE GradeID = " + CommonUtility.GetStringValue(gradeId);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var objRec = new DataTable();
            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(objRec);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            if (objRec.Rows.Count == 0)
            {
                DataRow dataRow = objRec.NewRow();
                dataRow["GradeID"] = gradeId;
                objRec.Rows.Add(dataRow);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                objRec.Rows[0]["CashPriceIncre"] = value.CashPriceIncre ?? DBNull.Value;
                objRec.Rows[0]["CreditPriceIncre"] = value.CreditPriceIncre ?? DBNull.Value;
                objRec.Rows[0]["TaxExemptCashPriceIncre"] = value.TaxExemptCashPriceIncre ?? DBNull.Value;
                objRec.Rows[0]["TaxExemptCreditPriceIncre"] = value.TaxExemptCreditPriceIncre ?? DBNull.Value;
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            _adapter.Update(objRec);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to set fuel price
        /// </summary>
        /// <param name="fuelProperty">Get property</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="value">Fuel price</param>
        public void set_FuelPrice(ref GetProperty fuelProperty, byte gradeId, byte tierId,
            byte levelId, CFuelPrice value)
        {

            var cmdtxt = "SELECT * FROM FuelPrice WHERE GradeID = " + Conversion.Str(gradeId) + " and TierID = " +
                         Conversion.Str(tierId) + " and LevelID = " + Conversion.Str(levelId);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var objRec = new DataTable();
            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(objRec);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            if (objRec.Rows.Count == 0)
            {
                DataRow dataRow = objRec.NewRow();
                dataRow["GradeID"] = gradeId;
                dataRow["TierID"] = tierId;
                dataRow["LevelID"] = levelId;
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                objRec.Rows[0]["CashPrice"] = value.CashPrice;
                objRec.Rows[0]["CreditPrice"] = value.CreditPrice;
                objRec.Rows[0]["TaxFreePrice"] = value.teCashPrice;
                objRec.Rows[0]["TaxFreeCreditPrice"] = value.teCreditPrice;
                objRec.Rows[0]["EmplID"] = value.EmplID;
                objRec.Rows[0]["ReportID"] = value.ReportID;
                objRec.Rows[0]["Date_Time"] = DateTime.Now;
                objRec.Rows[0]["Source"] = "B";
                _adapter.UpdateCommand = builder.GetUpdateCommand();

            }
            _adapter.Update(objRec);

            if (fuelProperty.FuelPrice[gradeId, tierId, levelId] == null)
            {
                fuelProperty.FuelPrice[gradeId, tierId, levelId] = fuelProperty.EFuelPrice;
            }
            fuelProperty.FuelPrice[gradeId, tierId, levelId].CashPrice = value.CashPrice;
            fuelProperty.FuelPrice[gradeId, tierId, levelId].CreditPrice = value.CreditPrice;
            fuelProperty.FuelPrice[gradeId, tierId, levelId].EmplID = value.EmplID;
            fuelProperty.FuelPrice[gradeId, tierId, levelId].ReportID = value.ReportID;
            fuelProperty.FuelPrice[gradeId, tierId, levelId].Date_Time = (int)DateTime.Now.ToOADate();
        }

        /// <summary>
        /// Method to get tank info
        /// </summary>
        /// <param name="id">Grade id</param>
        /// <returns>Tank info</returns>
        public CTankInfo get_TankInfo(byte id)
        {
            CTankInfo returnValue = new CTankInfo();
            var cmdtxt = "SELECT * FROM TankInfo WHERE ID = " + Conversion.Str(id);
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                returnValue.TankCode =
                    CommonUtility.GetStringValue(Information.IsDBNull(objRec.Rows[0]["TankCode"])
                        ? ""
                        : objRec.Rows[0]["TankCode"]);
                returnValue.GradeID =
                    CommonUtility.GetShortValue(Information.IsDBNull(objRec.Rows[0]["GradeID"])
                        ? 0
                        : objRec.Rows[0]["GradeID"]);
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get tank type
        /// </summary>
        /// <param name="tankCode">Tank code</param>
        /// <returns>Tank type</returns>
        public CTankType get_TankType(string tankCode)
        {
            CTankType returnValue = new CTankType();
            var cmdtxt = "SELECT * FROM TankType WHERE TankCode = \'" + tankCode + "\'";
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                returnValue.Description = objRec.Rows[0]["Description"];
                returnValue.Length = objRec.Rows[0]["Length"];
                returnValue.Diameter = objRec.Rows[0]["Diameter"];
                returnValue.Capacity = objRec.Rows[0]["Capacity"];
                returnValue.TankEnds = objRec.Rows[0]["TankEnds"];
                returnValue.Orientation = objRec.Rows[0]["Orientation"];
                returnValue.Readingtype = objRec.Rows[0]["Readingtype"];
                returnValue.UseChart = objRec.Rows[0]["UseChart"];
                returnValue.LengthUnits = objRec.Rows[0]["LengthUnits"];
                returnValue.VolumeUnits = objRec.Rows[0]["VolumeUnits"];
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get tank chart
        /// </summary>
        /// <param name="dipChart">Dip chart</param>
        /// <param name="depth">Depth</param>
        /// <returns>Tank chart</returns>
        public dynamic get_TankChart(string dipChart, short depth)
        {
            dynamic returnValue = default(dynamic);
            var cmdtxt = "SELECT * FROM TankChart WHERE DipChart = \'" + dipChart + "\' and Depth = " +
                         Conversion.Str(depth);
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                returnValue = objRec.Rows[0]["Volume"];
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get sale option
        /// </summary>
        /// <param name="optionId">Option id</param>
        /// <returns>Sale option</returns>
        public dynamic get_SaleOption(byte optionId)
        {
            dynamic returnValue = default(dynamic);
            var cmdtxt = "SELECT * FROM SaleOption WHERE OptionID = " + Conversion.Str(optionId);
            var objRec = GetRecords(cmdtxt, DataSource.CSCPump);
            if (objRec.Rows.Count > 0)
            {
                returnValue = objRec.Rows[0]["OptionData"];
            }
            return returnValue;
        }

        /// <summary>
        /// Method to set fuel price in history
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="value">Fuel price</param>
        public void set_PutPriceinHist(byte gradeId, byte tierId, byte levelId, CFuelPrice value)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var objRec = new DataTable();
            _adapter = new SqlDataAdapter("select * from FuelPriceHist", _connection);
            _adapter.Fill(objRec);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            if (objRec.Rows.Count == 0)
            {
                DataRow dataRow = objRec.NewRow();

                dataRow["CashPrice"] = value.CashPrice;
                dataRow["CreditPrice"] = value.CreditPrice;
                dataRow["EmplID"] = value.EmplID;
                dataRow["ReportID"] = value.ReportID;
                dataRow["Date_Time"] = DateTime.Now;
                dataRow["tierID"] = tierId;
                dataRow["gradeID"] = gradeId;
                dataRow["levelID"] = levelId;
                dataRow["TaxFreePrice"] = value.teCashPrice;
                dataRow["TaxFreeCreditPrice"] = value.teCreditPrice;
                dataRow["Source"] = "B";
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                objRec.Rows[0]["CashPrice"] = value.CashPrice;
                objRec.Rows[0]["CreditPrice"] = value.CreditPrice;
                objRec.Rows[0]["EmplID"] = value.EmplID;
                objRec.Rows[0]["ReportID"] = value.ReportID;
                objRec.Rows[0]["Date_Time"] = DateTime.Now;
                objRec.Rows[0]["TaxFreePrice"] = value.teCashPrice;
                objRec.Rows[0]["TaxFreeCreditPrice"] = value.teCreditPrice;
                objRec.Rows[0]["Source"] = "B";
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }

            _adapter.Update(objRec);
        }

        /// <summary>
        /// Method to write totalizer
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="posId">Pos Id</param>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="volume">Volume</param>
        /// <param name="amount">Amount</param>
        /// <param name="mg">Group number</param>
        /// <returns>True or false</returns>
        public bool WriteTotalizer(Till till, short pumpId, short posId, byte gradeId,
            string volume, string amount, short mg)
        {
            //keep totalizer into TotalizerHist instead of Totalizer, for FuelManagement
            var cmdtxt = "select * from TotalizerHist";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var objRec = new DataTable();

            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(objRec);
            var builder = new SqlCommandBuilder(_adapter);
            var datatRow = objRec.NewRow();
            datatRow["pumpID"] = pumpId;
            datatRow["PositionID"] = posId;
            datatRow["Grade"] = gradeId;
            datatRow["Volume"] = Conversion.Val(volume) / 1000;
            datatRow["dollars"] = Conversion.Val(amount) / 100;
            datatRow["Date_Time"] = DateTime.Now;
            datatRow["GroupNumber"] = mg;

            if (till?.Number != 0)
            {
                datatRow["ShiftDate"] = till.ShiftDate;
            }

            objRec.Rows.Add(datatRow);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(objRec);

            Calculate_Sales_InTotalizer(mg, (byte)pumpId, (byte)posId, gradeId);
            return true;
        }

        /// <summary>
        /// Method to write total high low
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="highVolume">High volume</param>
        /// <param name="lowVolume">Low volume</param>
        /// <param name="mg">Group number</param>
        /// <returns>True or false</returns>
        public bool WriteTotalHighLow(short pumpId, string highVolume, string lowVolume, short mg)
        {
            var cmdtxt = "select * from TotalHighLow";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var objRec = new DataTable();
            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(objRec);
            var builder = new SqlCommandBuilder(_adapter);
            var datatRow = objRec.NewRow();

            datatRow["pumpID"] = pumpId;
            datatRow["HighVolume"] = Conversion.Val(highVolume) / 1000;
            datatRow["LowVolume"] = Conversion.Val(lowVolume) / 1000;
            datatRow["Date_Time"] = DateTime.Now;
            datatRow["GroupNumber"] = mg;
            objRec.Rows.Add(datatRow);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(objRec);
            return true;
        }


        /// <summary>
        /// Method to calculate sales in totalizer
        /// </summary>
        /// <param name="readingNumber">Reading number</param>
        /// <param name="pumpId">Pump id</param>
        /// <param name="position">Position</param>
        /// <param name="grade">Grade</param>
        private void Calculate_Sales_InTotalizer(int readingNumber, byte pumpId, byte position, int grade)
        {
            var cmdtxt = "SELECT * FROM TotalizerHist  WHERE GroupNumber=" + CommonUtility.GetStringValue(readingNumber) +
                         " AND PumpID=" + CommonUtility.GetStringValue(pumpId) + " AND Grade=" +
                         CommonUtility.GetStringValue(grade) + " AND PositionID=" +
                         CommonUtility.GetStringValue(position);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var rsTot = new DataTable();
            _adapter = new SqlDataAdapter(cmdtxt, _connection);
            _adapter.Fill(rsTot);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            foreach (DataRow dataRow in rsTot.Rows)
            {
                var readNumber = Get_Last_Reading_Number(readingNumber, pumpId, grade, position);
                var rsTotRef =
                    GetRecords(
                        "SELECT * FROM TotalizerHist  WHERE GroupNumber=" + CommonUtility.GetStringValue(readNumber) +
                        " AND PumpID=" + CommonUtility.GetStringValue(pumpId) + " AND Grade=" +
                        CommonUtility.GetStringValue(grade) + " AND PositionID=" +
                        CommonUtility.GetStringValue(position), DataSource.CSCPump);
                if (rsTotRef.Rows.Count > 0)
                {
                    if (
                        CommonUtility.GetDoubleValue(CommonUtility.GetIntergerValue(dataRow["dollars"]) -
                                                     CommonUtility.GetIntergerValue(rsTotRef.Rows[0]["dollars"])) >= 0)
                    {
                        dataRow["SalesDollars"] =
                        (CommonUtility.GetDoubleValue(dataRow["dollars"]) -
                         CommonUtility.GetDoubleValue(rsTotRef.Rows[0]["dollars"])).ToString("#,##0.00");
                        dataRow["Open_Dollars"] = rsTotRef.Rows[0]["dollars"];
                    }
                    else // rollover for dollars
                    {
                        dataRow["SalesDollars"] =
                        (CommonUtility.GetDoubleValue(CommonUtility.GetIntergerValue(dataRow["dollars"]) -
                                                      CommonUtility.GetDoubleValue(rsTotRef.Rows[0]["dollars"])) +
                         1000000).ToString("#,##0.00");
                        dataRow["Open_Dollars"] = rsTotRef.Rows[0]["dollars"];
                    }
                    if (
                        CommonUtility.GetDoubleValue(CommonUtility.GetIntergerValue(dataRow["Volume"]) -
                                                     CommonUtility.GetDoubleValue(rsTotRef.Rows[0]["Volume"])) >= 0)
                    {
                        dataRow["SalesLiters"] =
                        (CommonUtility.GetDoubleValue(dataRow["Volume"]) -
                         CommonUtility.GetDoubleValue(rsTotRef.Rows[0]["Volume"])).ToString("#,##0.000");
                        dataRow["Open_Liters"] = rsTotRef.Rows[0]["Volume"];
                    }
                    else // rollover for volume
                    {
                        dataRow["SalesLiters"] =
                        (CommonUtility.GetDoubleValue(CommonUtility.GetIntergerValue(dataRow["Volume"]) -
                                                      CommonUtility.GetDoubleValue(rsTotRef.Rows[0]["Volume"])) +
                         1000000).ToString("#,##0.000");
                        dataRow["Open_Liters"] = rsTotRef.Rows[0]["Volume"];
                    }
                    dataRow["ReadingTypeFlag"] = "C";
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(rsTot);
                }
                else
                {
                    dataRow["SalesDollars"] = 0.ToString("#,##0.00");
                    dataRow["SalesLiters"] = 0.ToString("#,##0.000");
                    dataRow["Open_Liters"] = CommonUtility.GetDoubleValue(dataRow["Volume"]).ToString("#,##0.000");
                    dataRow["Open_Dollars"] = CommonUtility.GetDoubleValue(dataRow["dollars"]).ToString("#,##0.00");
                    dataRow["ReadingTypeFlag"] = "O";
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(rsTot);
                }
            }
        }

        /// <summary>
        /// Method to get the last reading number
        /// </summary>
        /// <param name="readingNumber">Reading number</param>
        /// <param name="pumpId">Pump id</param>
        /// <param name="grade">Grade</param>
        /// <param name="position">Position</param>
        /// <returns>Last reading number</returns>
        private int Get_Last_Reading_Number(int readingNumber, byte pumpId, int grade,
            byte position)
        {
            int returnValue;

            var rsNumber =
                GetRecords(
                    "SELECT MAX(GroupNumber) AS " + "ReadNumber FROM TotalizerHist " + " WHERE GroupNumber<>" +
                    CommonUtility.GetStringValue(readingNumber) + " AND PumpID=" + CommonUtility.GetStringValue(pumpId) +
                    " AND Grade=" + CommonUtility.GetStringValue(grade) + " AND PositionID=" +
                    CommonUtility.GetStringValue(position), DataSource.CSCPump);

            if (rsNumber.Rows.Count > 0)
            {
                returnValue =
                    CommonUtility.GetIntergerValue(Information.IsDBNull(rsNumber.Rows[0]["ReadNumber"])
                        ? -1
                        : rsNumber.Rows[0]["ReadNumber"]);
            }
            else
            {
                returnValue = -1;
            }
            return returnValue;
        }


        /// <summary>
        /// Method to get the maximum report id
        /// </summary>
        /// <returns>Report id</returns>
        public int Get_ReportID()
        {
            int returnValue;
            int maxValue;

            var rsFuelPrice = GetRecords("SELECT MAX(ReportID) AS ReportID FROM FuelPrice", DataSource.CSCPump);
            if (rsFuelPrice.Rows.Count == 0)
            {
                maxValue = 1;
            }
            else
            {
                if (Information.IsDBNull(rsFuelPrice.Rows[0]["ReportID"]))
                {
                    maxValue = 1;
                }
                else
                {
                    maxValue = CommonUtility.GetIntergerValue(rsFuelPrice.Rows[0]["ReportID"]);
                }
            }

            var rsFuelPriceHist = GetRecords("SELECT MAX(ReportID) AS ReportID FROM FuelPriceHist", DataSource.CSCPump);
            if (rsFuelPriceHist.Rows.Count == 0)
            {
                returnValue = maxValue + 1;
            }
            else
            {
                if (Information.IsDBNull(rsFuelPriceHist.Rows[0]["ReportID"]))
                {
                    returnValue = maxValue + 1;
                }
                else
                {
                    if (maxValue > CommonUtility.GetIntergerValue(rsFuelPriceHist.Rows[0]["ReportID"]))
                    {
                        returnValue = maxValue + 1;
                    }
                    else
                    {
                        returnValue = CommonUtility.GetIntergerValue(rsFuelPriceHist.Rows[0]["ReportID"]) + 1;
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Method to find if price is chaged from head office
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsPriceChangeFromHo()
        {
            var recs = GetRecords("SELECT * FROM FuelPrice_HO", DataSource.CSCPump);
            return recs.Rows.Count > 0;
        }

        /// <summary>
        /// Method to delete fuel price
        /// </summary>
        public void DeleteFuelPrice()
        {
            Execute("DELETE FROM FuelPrice_HO", DataSource.CSCPump);
        }

        /// <summary>
        /// Method to get the maximum group number of totalizer history
        /// </summary>
        /// <returns>Group number</returns>
        public short GetMaxGroupNumberofTotalizerHistory()
        {
            short returnValue = 0;
            var rs = GetRecords("Select Max(GroupNumber) As [MG]  FROM   TotalizerHist ", DataSource.CSCPump);
            if (!Information.IsDBNull(rs.Rows[0]["MG"]))
            {
                returnValue = (short)(CommonUtility.GetShortValue(rs.Rows[0]["MG"]));
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get maximum group number of total high low
        /// </summary>
        /// <returns>Group number</returns>
        public short GetMaxGroupNumberofTotalHighLow()
        {
            short returnValue = 0;
            var rs = GetRecords("Select Max(GroupNumber) As [MG]  FROM   TotalHighLow ", DataSource.CSCPump);
            if (!Information.IsDBNull(rs.Rows[0]["MG"]))
            {
                returnValue = (short)(CommonUtility.GetShortValue(rs.Rows[0]["MG"]) + 1);
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get list of head office fuel prices
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <returns>List of fuel prices</returns>
        public List<FuelPrice> GetHeadOfficeFuelPrices(short gradeId, short tierId, short levelId)
        {
            var cmdtxt =
                "SELECT CashPrice, CreditPrice, TaxFreePrice, TaxFreeCreditPrice FROM FuelPrice_HO WHERE GradeID=" +
                Convert.ToString(gradeId) + " AND TierID=" + tierId + " AND LevelID=" + levelId;

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var rsTot = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(rsTot);

                var prices = new List<FuelPrice>();

                foreach (DataRow dataRow in rsTot.Rows)
                {
                    prices.Add(new FuelPrice
                    {
                        GradeId = gradeId,
                        TierId = tierId,
                        LevelId = levelId,
                        CashPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["CashPrice"])
                                ? 0
                                : dataRow["CashPrice"]),
                        CreditPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["CreditPrice"])
                                ? 0
                                : dataRow["CreditPrice"]),
                        TaxExemptedCashPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["TaxFreePrice"])
                                ? 0
                                : dataRow["TaxFreePrice"]),
                        TaxExemptedCreditPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["TaxFreeCreditPrice"])
                                ? 0
                                : dataRow["TaxFreeCreditPrice"])
                    });
                }
                return prices;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to get fuel price by grade id, tier id and level id
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier Id</param>
        /// <param name="levelId">Level Id</param>
        /// <returns>Fuel price</returns>
        public FuelPrice GetFuelPrice(short gradeId, short tierId, short levelId)
        {
            var cmdtxt = "SELECT * FROM FuelPrice WHERE GradeID=" + Convert.ToString(gradeId) + " AND TierID=" +
                         tierId + " AND LevelID=" + levelId;

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var rsTot = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(rsTot);

                if (rsTot.Rows.Count > 0)
                {
                    var dataRow = rsTot.Rows[0];

                    _connection.Close();
                    _adapter?.Dispose();
                    return new FuelPrice
                    {
                        GradeId = gradeId,
                        TierId = tierId,
                        LevelId = levelId,
                        CashPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["CashPrice"])
                                ? 0
                                : dataRow["CashPrice"]),
                        CreditPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["CreditPrice"])
                                ? 0
                                : dataRow["CreditPrice"]),
                        TaxExemptedCashPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["TaxFreePrice"])
                                ? 0
                                : dataRow["TaxFreePrice"]),
                        TaxExemptedCreditPrice =
                            CommonUtility.GetStringValue(Information.IsDBNull(dataRow["TaxFreeCreditPrice"])
                                ? 0
                                : dataRow["TaxFreeCreditPrice"])
                    };
                }
                return null;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to fuel price by rate
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="regularCashPrice">Regular cash price</param>
        /// <param name="regularCreditPrice">Regular credit price</param>
        /// <param name="taxExemptTaxCode">Tax exempt tax code</param>
        /// <returns>True or false</returns>
        public bool GetFuelPriceByRate(string stockCode, int gradeId, int tierId, int levelId,
            ref float regularCashPrice, ref float regularCreditPrice, ref short taxExemptTaxCode)
        {
            var sql = "select Grade.ID, Grade.Stock_Code, " +
                      " FuelPrice.CashPrice as CashPrice, FuelPrice.CreditPrice as CreditPrice," +
                      " (SELECT CategoryFK From CSCMaster.dbo.ProductTaxExempt Where" + " ProductKey =\'" + stockCode +
                      "\') As CategoryFK," + " (SELECT TAX_CODE From CSCMaster.dbo.ProductTaxExempt Where" +
                      " ProductKey =\'" + stockCode + "\') As TAX_CODE," + " 1 As UnitsPerPkg, " + " \'\' As UpcCode " +
                      " from Grade INNER JOIN FuelPrice ON " + " Grade.ID=FuelPrice.GradeID " + " where GradeID=" +
                      Convert.ToString(gradeId) + " " + " AND FuelPrice.TierID=" + tierId +
                      " AND FuelPrice.LevelID=" + levelId;

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var rsTot = new DataTable();
                _adapter = new SqlDataAdapter(sql, _connection);
                _adapter.Fill(rsTot);

                if (rsTot.Rows.Count > 0)
                {
                    var dataRow = rsTot.Rows[0];

                    regularCashPrice = CommonUtility.GetFloatValue(dataRow["CashPrice"]);
                    regularCreditPrice = CommonUtility.GetFloatValue(dataRow["CreditPrice"]);
                    taxExemptTaxCode = CommonUtility.GetShortValue(dataRow["Tax_Code"]);
                    return true;
                }
                return false;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to find if prepay is set
        /// </summary>
        /// <param name="tillNumber">Till Number</param>
        /// <returns>True or false</returns>
        public bool IsPrepaySet(int tillNumber)
        {
            var count = GetRecordCount("select Count(*) from PrepayGlobal where TillID=" + tillNumber + " and Amount<>\'\' and Locked <> 1", DataSource.CSCTrans);

            return count > 0;
        }

        /// <summary>
        /// Method to get all totalizer history
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="gradeId">Grade id</param>
        /// <returns>List of totaliser history</returns>
        public List<TotalizerHist> GetTotalizerHist(int groupNumber, short pumpId, short gradeId)
        {
            var totalizerHists = new List<TotalizerHist>();
            var sql = "Select *  FROM   Totalizerhist";

            var whereClause = "";

            if (groupNumber != 0)
            {
                whereClause += " GroupNumber = " + groupNumber;
            }
            if (pumpId != 0)
            {
                if (whereClause.Length > 0)
                {
                    whereClause += " AND";
                }
                whereClause += " PumpId = " + pumpId;
            }
            if (gradeId != 0)
            {
                if (whereClause.Length > 0)
                {
                    whereClause += " AND";
                }
                whereClause += " Grade = " + gradeId;
            }

            if (whereClause.Length > 0)
            {
                sql += " WHERE " + whereClause;
            }

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var rs = new DataTable();
                _adapter = new SqlDataAdapter(sql, _connection);
                _adapter.Fill(rs);

                if (rs.Rows.Count > 0)
                {
                    totalizerHists = (from DataRow dataRow in rs.Rows
                                      select new TotalizerHist
                                      {
                                          Dollars = CommonUtility.GetDecimalValue(dataRow["dollars"]),
                                          Grade = CommonUtility.GetIntergerValue(dataRow["Grade"]),
                                          GroupNumber = CommonUtility.GetIntergerValue(dataRow["GroupNumber"]),
                                          PumpId = CommonUtility.GetIntergerValue(dataRow["PumpId"]),
                                          Volume = CommonUtility.GetDecimalValue(dataRow["Volume"])
                                      }).ToList();
                }
                return totalizerHists;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to get tax exempt by rate
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="regPrice">Regular price</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Tax exempt price</returns>
        public string GetTePriceByRate(short gradeId, float regPrice, string stockCode)
        {
            float taxRate;

            var returnValue = regPrice.ToString("#0.000");

            var sql = "select TaxExemptRate.TAX_RATE as TAX_RATE,  " + " TaxExemptRate.RateType as RateType from " +
                      " TaxExemptRate INNER JOIN ProductTaxExempt " +
                      " ON TaxExemptRate.TAX_CODE=ProductTaxExempt.TAX_CODE " + " where ProductTaxExempt.ProductKey=\'" +
                      stockCode + "\' ";

            var rs = GetRecords(sql, DataSource.CSCMaster);
            if (rs == null || rs.Rows.Count == 0)
            {
                return returnValue;
            }
            taxRate = Convert.ToSingle(Information.IsDBNull(rs.Rows[0]["Tax_Rate"]) ? 0 : rs.Rows[0]["Tax_Rate"]);

            var rateType =
                Convert.ToString(Information.IsDBNull(rs.Rows[0]["RateType"])
                    ? "$"
                    : Strings.Trim(Convert.ToString(rs.Rows[0]["RateType"])));
            switch (rateType)
            {
                case "%":
                    returnValue = (regPrice * (1 - taxRate / 100)).ToString("#0.000");
                    break;
                case "$":
                    returnValue = (regPrice - taxRate).ToString("#0.000");
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// Get Propane Grades
        /// </summary>
        /// <returns></returns>
        public List<PropaneGrade> GetPropaneGrades()
        {
            var result = new List<PropaneGrade>();
            var rstGrade = GetRecords("Select * from Grade where FuelType=\'O\' or FuelType=\'o\'", DataSource.CSCPump);

            foreach (DataRow datarow in rstGrade.Rows)
            {
                var grade = new PropaneGrade();
                grade.FullName = CommonUtility.GetStringValue(datarow["FullName"]);
                grade.Shortname = CommonUtility.GetStringValue(datarow["ShortName"]);
                grade.Id = CommonUtility.GetByteValue(datarow["ID"]);
                grade.StockCode = CommonUtility.GetStringValue(datarow["Stock_Code"]);
                result.Add(grade);
            }
            return result;
        }

        /// <summary>
        /// Gets Pumps by Propane Grade Id
        /// </summary>
        /// <param name="gradeId"></param>
        /// <returns></returns>
        public List<PropanePump> GetPumpsByPropaneGradeId(int gradeId)
        {
            var result = new List<PropanePump>();
            var rs = GetRecords("Select distinct PumpID from Assignment where GradeID=" + Convert.ToString(gradeId),
                DataSource.CSCPump);

            foreach (DataRow datarow in rs.Rows)
            {
                PropanePump pmp = new PropanePump();
                pmp.Id = CommonUtility.GetIntergerValue(datarow["pumpID"]);
                result.Add(pmp);
            }
            return result;
        }


        /// <summary>
        /// Get Position ID by pump and Grade
        /// </summary>
        /// <param name="pumpId"></param>
        /// <param name="gradeId"></param>
        /// <returns></returns>
        public int GetPositionId(int pumpId, int gradeId)
        {
            var result = 0;
            var rs =
                GetRecords(
                    "Select PositionID from Assignment where PumpID=" + Convert.ToString(pumpId) + " and GradeID=" +
                    Convert.ToString(gradeId), DataSource.CSCPump);


            if (rs.Rows.Count > 0)
            {
                result = CommonUtility.GetIntergerValue(rs.Rows[0]["PositionID"]);
            }
            return result;
        }


        /// <summary>
        /// Method to unlock prepay
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        public List<UnCompleteSale> LoadMyUnlockedPrepay(int tillNumber)
        {
            List<UnCompleteSale> result = new List<UnCompleteSale>();

            var rsPrepay =
                GetRecords("select * from PrepayGlobal where TillID=" + tillNumber + " and Amount<>\'\' and Locked<>1",
                    DataSource.CSCTrans);

            if (rsPrepay.Rows.Count == 0)
            {
                return result;
            }

            
            foreach (DataRow dataRow in rsPrepay.Rows)
            {
                UnCompleteSale unCompleteSale = new UnCompleteSale();

                unCompleteSale.PumpId = CommonUtility.GetShortValue(dataRow["pumpID"]);
                unCompleteSale.PositionId = CommonUtility.GetShortValue(dataRow["PositionID"]);
                unCompleteSale.SaleNumber = CommonUtility.GetIntergerValue(dataRow["invoiceID"]);
                unCompleteSale.PrepayAmount = CommonUtility.GetDoubleValue(dataRow["Amount"]);
                unCompleteSale.Mop = CommonUtility.GetByteValue(dataRow["MOP"]);

                result.Add(unCompleteSale);
            }
            return result;
        }

        /// <summary>
        /// Update Tier and Level for pump
        /// </summary>
        /// <param name="pumpId"></param>
        /// <param name="tierId"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public bool UpdateTierLevelForPump(int pumpId, int tierId, int levelId)
        {
            
            var cmdtxt = "Select * from pump where ID =" + Convert.ToString(pumpId);

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var objRec = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(objRec);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                if (objRec.Rows.Count == 0)
                {
                    DataRow dataRow = objRec.NewRow();
                    dataRow["TierID"] = tierId;
                    dataRow["LevelID"] = levelId;
                    _adapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    objRec.Rows[0]["TierID"] = Conversion.Val(tierId);
                    objRec.Rows[0]["LevelID"] = Conversion.Val(levelId);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();

                }
                _adapter.Update(objRec);
                return true;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to get grades for price to display
        /// </summary>
        /// <returns></returns>
        public List<string> GetGradesForPriceToDisplay()
        {
            var data = new List<string>();
            var cmdtxt = "select (CAST(ID AS nvarchar(5)) + \' - \' + ShortName) as name from Grade";

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var objRec = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(objRec);
                if (objRec?.Rows?.Count != 0)
                {
                    foreach (DataRow row in objRec.Rows)
                    {
                        data.Add(row["Name"].ToString());
                    }
                }

                return data;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to get tiers for prices to display
        /// </summary>
        /// <returns></returns>
        public List<string> GetTiersForPriceToDisplay()
        {
            var data = new List<string>();
            var cmdtxt = "select (CAST(ID AS nvarchar(5)) + \' - \' + Description) as name from Tier";

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var objRec = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(objRec);
                if (objRec?.Rows?.Count != 0)
                {
                    foreach (DataRow row in objRec.Rows)
                    {
                        data.Add(row["Name"].ToString());
                    }
                }

                return data;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to get levels for prices to display
        /// </summary>
        /// <returns></returns>
        public List<string> GetLevelsForPriceToDisplay()
        {
            var data = new List<string>();
            var cmdtxt = "select (CAST(ID AS nvarchar(5)) + \' - \' + Description) as name  from ServiceLevel";

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var objRec = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(objRec);
                if (objRec?.Rows?.Count != 0)
                {
                    foreach (DataRow row in objRec.Rows)
                    {
                        data.Add(row["Name"].ToString());
                    }
                }

                return data;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to save price to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="grade">Grade</param>
        /// <param name="level">Level</param>
        /// <param name="tier">Tier</param>
        /// <returns>True or false</returns>
        public bool SavePriceToDisplay(byte row, string grade, string level, string tier)
        {
            var cmdtxt = "select * from PricesToDisplay where Row=" + row;

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var objRec = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(objRec);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                if (objRec.Rows.Count == 0)
                {
                    DataRow dataRow = objRec.NewRow();
                    dataRow["Row"] = row;
                    _adapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    objRec.Rows[0]["GradeID"] = Conversion.Val(grade);
                    objRec.Rows[0]["LevelID"] = Conversion.Val(level);
                    objRec.Rows[0]["TierID"] = Conversion.Val(tier);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();

                }
                _adapter.Update(objRec);
                return true;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to get grade for price increment ids
        /// </summary>
        /// <returns>Grade price increments</returns>
        public List<short> GetGradePriceIncrementIds()
        {
            var data = new List<short>();
            var cmdtxt = "select * from GradePriceIncrement " + " INNER JOIN Grade ON GradePriceIncrement.GradeID=Grade.ID where Grade.FuelType=\'G\'";

            try
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var objRec = new DataTable();
                _adapter = new SqlDataAdapter(cmdtxt, _connection);
                _adapter.Fill(objRec);
                if (objRec?.Rows?.Count != 0)
                {
                    foreach (DataRow row in objRec.Rows)
                    {
                        data.Add(CommonUtility.GetShortValue(row["GradeID"]));
                    }
                }

                return data;
            }
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
        }
    }
}

