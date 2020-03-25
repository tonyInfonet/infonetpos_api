using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ISaleVendorCouponService
    {
        /// <summary>
        /// Method to add sale vendor coupon line
        /// </summary>
        /// <param name="oLine">Sale vendor coupon line</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        void AddSaleVendorCouponLine(SaleVendorCouponLine oLine, int saleNumber, int tillNumber);

        /// <summary>
        /// Method to update sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="seqNumber">Sequence number</param>
        /// <param name="serialNumber"></param>
        void UpdateSaleVendorCoupon(int saleNumber, int tillNumber, int lineNumber, short seqNumber, string serialNumber);

        /// <summary>
        /// Method to save sale vendor coupon service
        /// </summary>
        /// <param name="sv">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        void SaveSaleVendorCoupon(SaleVendorCoupon sv, int tillNumber, DataSource dataSource);
    }
}
