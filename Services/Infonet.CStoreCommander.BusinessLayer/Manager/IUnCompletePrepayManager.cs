using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IUnCompletePrepayManager
    {

        /// <summary>
        /// Method to change uncomplete prepay
        /// </summary>
        /// <param name="ipumpId">Pump Id</param>
        /// <param name="lngSaleNum">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="finishAmount">Finish amount</param>
        /// <param name="finishQty">Finish quantity</param>
        /// <param name="finishPrice">Finish price</param>
        /// <param name="prepayAmount">Prepay amount</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="openDrawer">Open drawer</param>
        /// <param name="error">Error message</param>
        /// <returns>Tax exmept voucher</returns>
        Report ChangeUncompletePrepay(int ipumpId, int lngSaleNum, int tillNumber, float finishAmount, float finishQty,
            float finishPrice, float prepayAmount, short iPositionId, short iGradeId, out string changeDue,
            out bool openDrawer, out ErrorMessage error);


        /// <summary>
        /// Method to delete uncomplete prepay
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <returns>Sale</returns>
        Sale DeleteUnCompletePrepay(int pumpId, int saleNumber, int tillNumber, out ErrorMessage error);

        /// <summary>
        /// Method to load uncomplete grid
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <returns>Uncomplete prepay response</returns>
        UnCompletePrepayResponse LoadUncompleteGrid(int tillNumber, out ErrorMessage error);

        /// <summary>
        /// Method to finish overpayment uncomplete prepay
        /// </summary>
        /// <param name="ipumpId">Pump Id</param>
        /// <param name="lngSaleNum">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="finishAmount">Finish amount</param>
        /// <param name="finishQty">Finish quantity</param>
        /// <param name="finishPrice">Finish price</param>
        /// <param name="prepayAmount">Prepay amount</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="openDrawer">Open drawer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax exmept report</returns>
        Report OverpaymentUncompletePrepay(int ipumpId, int lngSaleNum, int tillNumber, float finishAmount,
            float finishQty, float finishPrice, float prepayAmount, short iPositionId, short iGradeId,
            out bool openDrawer, out ErrorMessage error);
    }
}