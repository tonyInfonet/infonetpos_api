using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ISaleVendorCouponManager
    {
        /// <summary>
        /// Method to add a line to sale vendor coupon
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="saveToTmp">Save to Db</param>
        void Add_a_Line(ref SaleVendorCoupon svc, int tillNumber, SaleVendorCouponLine oLine,
            bool saveToTmp);

        /// <summary>
        /// Method to update serial number
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="seqNum">Sequence number</param>
        /// <param name="serialNumber">Serial number</param>
        /// <param name="saveToTmp">Save to Db</param>
        void UpdateSerialNumber(ref SaleVendorCoupon svc, int tillNumber, short lineNum,
            short seqNum, string serialNumber, bool saveToTmp);

        /// <summary>
        /// Method to recompute total
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saveToTmp">Save to Db</param>
        void RecomputeTotal(ref SaleVendorCoupon svc, int tillNumber, bool saveToTmp);

        /// <summary>
        /// Method to save sale vendor coupon
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        void Save_Sale(SaleVendorCoupon svc, int tillNumber);

        /// <summary>
        /// Method to remove a sale vendor coupon line
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="itemNum">Item number</param>
        void Remove_a_Line(ref SaleVendorCoupon svc, int tillNumber, short itemNum);

    }
}
