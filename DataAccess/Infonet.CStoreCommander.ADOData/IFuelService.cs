using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IFuelService
    {
        #region CSCPump services

        /// <summary>
        /// Method to get fuelType
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Fuel type</returns>
        string GetFuelTypeFromDbPump(string stockCode);

        #endregion

        #region CSCMaster services

        /// <summary>
        /// Method to get fuel discount rate
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <param name="gradeId">Grade id</param>
        /// <returns>Discount rate</returns>
        float? GetDiscountRate(string groupId, string gradeId);

        /// <summary>
        /// Method to save post pay enabled
        /// </summary>
        /// <param name="vData">Data</param>
        void SavePostPayEnabled(bool vData);

        /// <summary>
        /// Method to save pay pump enabled
        /// </summary>
        /// <param name="vData">Data</param>
        void SavePayPumpEnabled(bool vData);

        /// <summary>
        /// Method to save prepay enabled
        /// </summary>
        /// <param name="vData">Data</param>
        void SavePrepayEnabled(bool vData);


        /// <summary>
        /// Method to load service
        /// </summary>
        /// <returns></returns>
        Service LoadService();

        /// <summary>
        /// Method to save post pay set manually
        /// </summary>
        /// <param name="vData">Data</param>
        void Save_PostPaySetManually(bool vData);

        #endregion

        #region CSCAdmin

        /// <summary>
        /// Method to load fuel department
        /// </summary>
        /// <returns></returns>
        string LoadFuelDept();

        /// <summary>
        /// Method to load fuel sale grocery coupon
        /// </summary>
        /// <returns>FSGD coupon</returns>
        string LoadFsgdCoupon();

        /// <summary>
        /// Method to get header line
        /// </summary>
        /// <param name="num">Line number</param>
        /// <returns>Header line</returns>
        string GetHeaderLine(short num);

        /// <summary>
        /// Method to get footer line
        /// </summary>
        /// <param name="num">Line num</param>
        /// <returns>Footer line</returns>
        string GetFooterLine(short num);

        /// <summary>
        /// Method to check if this is existing coupon
        /// </summary>
        /// <param name="couponId">Coupon</param>
        /// <returns>True or false</returns>
        bool IsExistingCoupon(string couponId);

        /// <summary>
        /// Method to get list of prices
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of prices</returns>
        List<PriceL> GetPriceL(string stockCode);


        #endregion

        /// <summary>
        /// Get PumpSpace Value
        /// </summary>
        /// <returns></returns>
        byte GetPumpSpace();

        /// <summary>
        /// Gets the delay in seconds needed between 2 pumps operations
        /// </summary>
        /// <returns>Delay in seconds</returns>
        int GetClickDelayForPumps();
    }
}
