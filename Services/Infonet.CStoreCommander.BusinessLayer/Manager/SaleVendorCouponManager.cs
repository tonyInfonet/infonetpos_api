using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class SaleVendorCouponManager : ManagerBase, ISaleVendorCouponManager
    {
        private readonly ISaleVendorCouponService _svcService;

        /// <summary>
        /// Ctors
        /// </summary>
        /// <param name="svcService"></param>
        public SaleVendorCouponManager(ISaleVendorCouponService svcService)
        {
            _svcService = svcService;
        }

        /// <summary>
        /// Method to add a line to sale vendor coupon
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="saveToTmp">Save to Db</param>
        public void Add_a_Line(ref SaleVendorCoupon svc, int tillNumber, SaleVendorCouponLine oLine,
            bool saveToTmp)
        {
            svc.SVC_Lines.AddLine((short)(svc.SVC_Lines.Count + 1), oLine, "");

            svc.Amount = svc.Amount + oLine.TotalValue;

            if (saveToTmp)
            {
                
                _svcService.AddSaleVendorCouponLine(oLine, svc.Sale_Num, tillNumber);
            }
        }

        
        /// <summary>
        /// Method to update serial number
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="seqNum">Sequence number</param>
        /// <param name="serialNumber">Serial number</param>
        /// <param name="saveToTmp">Save to Db</param>
        public void UpdateSerialNumber(ref SaleVendorCoupon svc, int tillNumber, short lineNum,
            short seqNum, string serialNumber, bool saveToTmp)
        {
            foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
            {
                var svcLine = tempLoopVarSvcLine;
                if (!(svcLine.Line_Num == lineNum & svcLine.SeqNum == seqNum)) continue;
                svcLine.SerialNumber = serialNumber;
                break;
            }

            if (saveToTmp)
            {
                _svcService.UpdateSaleVendorCoupon(svc.Sale_Num, tillNumber, lineNum, seqNum, serialNumber);
            }
        }


        
        /// <summary>
        /// Method to recompute total
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saveToTmp">Save to Db</param>
        public void RecomputeTotal(ref SaleVendorCoupon svc, int tillNumber, bool
            saveToTmp)
        {
            decimal tmpAmount = 0;

            foreach (SaleVendorCouponLine tempLoopVarOLine in svc.SVC_Lines)
            {
                var oLine = tempLoopVarOLine;
                tmpAmount = tmpAmount + oLine.TotalValue;
            }

            svc.Amount = tmpAmount;

            if (saveToTmp)
            {
                _svcService.SaveSaleVendorCoupon(svc, tillNumber, DataSource.CSCCurSale);

            }
        }

        /// <summary>
        /// Method to save sale vendor coupon
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        public void Save_Sale(SaleVendorCoupon svc, int tillNumber)
        {
            _svcService.SaveSaleVendorCoupon(svc, tillNumber, DataSource.CSCTills);

        }

        /// <summary>
        /// Method to remove a sale vendor coupon line
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="itemNum">Item number</param>
        public void Remove_a_Line(ref SaleVendorCoupon svc, int tillNumber, short itemNum)
        {

            svc.SVC_Lines.Remove(itemNum);

            ReduceLineNum(ref svc, itemNum);

            RecomputeTotal(ref svc, tillNumber, false);

        }

        #region Private methods

        /// <summary>
        /// Method to reduce line number
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="itemFrom">Item number</param>
        private void ReduceLineNum(ref SaleVendorCoupon svc, short itemFrom)
        {
            foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
            {
                var svcLine = tempLoopVarSvcLine;
                if (svcLine.ItemNum >= itemFrom)
                {
                    svcLine.ItemNum--;
                }
            }
        }

        #endregion
    }
}
