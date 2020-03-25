using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using System;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PurchaseItemManager : ManagerBase, IPurchaseItemManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ITeSystemManager _teSystemManager;
        private readonly ITaxExemptService _taxExemptService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="teSystemManager"></param>
        /// <param name="taxExemptService"></param>
        public PurchaseItemManager(IPolicyManager policyManager,
            ITeSystemManager teSystemManager, ITaxExemptService taxExemptService)
        {
            _policyManager = policyManager;
            _teSystemManager = teSystemManager;
            _taxExemptService = taxExemptService;
        }

        /// <summary>
        /// Method to initiliase purchase item
        /// </summary>
        /// <param name="purchaseItem">Purchase item</param>
        /// <param name="oTreatyNo">Treaty number</param>
        /// <param name="sProductKey">Product key</param>
        /// <param name="dOriginalPrice">Original price</param>
        /// <param name="dQuantity">Orginal quantity</param>
        /// <param name="iRowNumberInSalesMainForm">Row number is sale line</param>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="tillId">Till Id</param>
        /// <param name="stockcode">Stock code</param>
        /// <param name="taxInclPrice">Tax included price</param>
        /// <param name="isFuelItem">Is fuel item or not</param>
        /// <param name="noRtvp">RTVP or not</param>
        /// <returns>True or false</returns>
        public bool Init(ref tePurchaseItem purchaseItem, ref teTreatyNo oTreatyNo,
            ref string sProductKey, double dOriginalPrice, double dQuantity,
            short iRowNumberInSalesMainForm, int invoiceId, short tillId, ref string stockcode,
            double taxInclPrice, bool isFuelItem, bool noRtvp)
        {
            purchaseItem.InvoiceID = invoiceId;
            purchaseItem.TillID = tillId;

            if (!_teSystemManager.TeGetTaxFreePrice(ref mPrivateGlobals.theSystem, ref purchaseItem, sProductKey, (float)dOriginalPrice, false, stockcode, "", 0))
            {
                return false;
            }
            
            if (_policyManager.SITE_RTVAL && !noRtvp) //   And Not NoRTVP condition
            {
                if (Variables.RTVPService == null)
                {
                    Variables.RTVPService = new RTVP.POSService.Transaction(); // Writing this to solve the issue of products not getting added in DB in tax exempt sale due to cache loss
                }
                purchaseItem.LineItem = Convert.ToInt16(Variables.RTVPService.AddLineItem());
                WriteToLogFile("Response is " + Convert.ToString(purchaseItem.LineItem) + " from AddLineItem sent with no parameters");
            }
            purchaseItem.Quantity = (float)dQuantity; //  

            if (isFuelItem)
            {
                purchaseItem.PsIsFuelItem = true;
                purchaseItem.PsGradeStockCode = stockcode;
                var psGradeId = purchaseItem.PsGradeIDpsTreatyNo;
                var psTierId = purchaseItem.PsTierID;
                var psLevelId = purchaseItem.PsLevelID;
                _teSystemManager.TeExtractFuelKey(ref mPrivateGlobals.theSystem, ref sProductKey, ref psGradeId, ref psTierId, ref psLevelId);
                purchaseItem.PsGradeIDpsTreatyNo = psGradeId;
                purchaseItem.PsTierID = psTierId;
                purchaseItem.PsLevelID = psLevelId;
            }
            else
            {
                purchaseItem.PsIsFuelItem = false;
            }

            purchaseItem.PsProductKey = sProductKey;
            purchaseItem.PdQuantity = (float)dQuantity;
            purchaseItem.PiRowInSalesMain = iRowNumberInSalesMainForm;
            purchaseItem.PetaxInclPrice = (float)dOriginalPrice;

            purchaseItem.PdOriginalPrice = (float)dOriginalPrice;

            purchaseItem.PiOverrideCode = -1;
            purchaseItem.PsOverrideFormNumber = (0).ToString();

            purchaseItem.PsOverrideDetails = "Not Set";

            purchaseItem.PetaxInclPrice = (float)taxInclPrice;

            purchaseItem.PsTreatyNo = oTreatyNo;

            purchaseItem.BIsInit = true;
            if (!_policyManager.SITE_RTVAL)
            {
                purchaseItem.PeIsLimitRequired = _teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, purchaseItem.ProdType);
            }
            return true;
        }


        
        
        
        /// <summary>
        /// Method to update quantity in db
        /// </summary>
        /// <param name="purchaseItem">Purchase item</param>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="reducedQuantity">Reduced quantity</param>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="stock">Stock code</param>
        /// <param name="tePrice">Tax exemot price</param>
        /// <param name="orgPrice">Original price</param>
        public void UpdateQuantityInDb(tePurchaseItem purchaseItem, int invoiceId,
            short lineNum, float reducedQuantity, string treatyNo, string stock = "",
            float tePrice = 0, float orgPrice = 0)
        {
            
            string sSql = "";

            if (!string.IsNullOrEmpty(stock))
            {
                sSql = "UPDATE PurchaseItem SET Quantity=" +
                   Convert.ToString(purchaseItem.pdQuantity) + ", Amount="
                    + Convert.ToString(Math.Round(purchaseItem.pdTaxFreePrice * purchaseItem.pdQuantity, 2))
                    + ", TotalTaxSaved=" + Convert.ToString(Math.Round(purchaseItem.petaxInclPrice - purchaseItem.pdTaxFreePrice * purchaseItem.pdQuantity, 2)) + ", CscPurchaseItemKey=\'" + stock + "\' " + ", TaxFreePrice=" + Convert.ToString(tePrice) + ", OriginalPrice="
                    + Convert.ToString(orgPrice) + " WHERE Sale_No=" + Convert.ToString(invoiceId) + " and Line_No=" + Convert.ToString(lineNum);
            }
            else
            {
                sSql = "UPDATE PurchaseItem SET Quantity=" + Convert.ToString(purchaseItem.pdQuantity) + ", Amount=" + Convert.ToString(Math.Round(purchaseItem.pdTaxFreePrice * purchaseItem.pdQuantity, 2)) + ", TotalTaxSaved=" + Convert.ToString(Math.Round(purchaseItem.petaxInclPrice - purchaseItem.pdTaxFreePrice * purchaseItem.pdQuantity, 2)) + " WHERE Sale_No=" + Convert.ToString(invoiceId) + " and Line_No=" + Convert.ToString(lineNum);

            }
            _taxExemptService.UpdatePurchaseItem(sSql);
            
            sSql = "UPDATE TreatyNo " + "SET GasQuota=GasQuota - " + Convert.ToString(reducedQuantity) + "  WHERE TreatyNo=\'" + treatyNo + "\'";
            _taxExemptService.UpdateTreatyNo(sSql);

        }
        

        /// <summary>
        /// Method to save purchase item
        /// </summary>
        /// <param name="purchaseItem">Purchase item</param>
        /// <param name="user">User</param>
        /// <param name="till">Till number</param>
        /// <returns>True or false</returns>
        public bool Save(tePurchaseItem purchaseItem, User user, Till till)
        {
            string sSql = "";

            //   don't save data in PurchaseItem for training mode
            if (user.User_Group.Code == Utilities.Constants.Trainer)
            {
                return true;
            }
            //   end

            purchaseItem.CheckInit();

            //Prepare for the worst

            //Record does not exist, so insert a new one

            string swipeStatus = "";
            swipeStatus = purchaseItem.PsTreatyNo.isSwiped ? "M" : "K";
            

            if (purchaseItem.PsIsFuelItem)
            {
                //   added SD, ShiftDate, Phone for fuel only for FNGTR
                sSql = "INSERT INTO PurchaseItem (TreatyNo, Sale_No, Line_No,"
                    + "IDCaptureMethod, CscPurchaseItemKey, TierID, LevelID, TaxFreePrice, "
                    + "OverrideCodeFK, OverrideDocNumber, OverrideExplanation, CategoryCodeFK, Till_No, "
                    + "OriginalPrice, Quantity, Amount, TotalTaxSaved, SD, ShiftDate, Phone ) VALUES (\'"
                    + purchaseItem.PsTreatyNo.GetTreatyNo() + "\'," + Convert.ToString(purchaseItem.InvoiceID) + ", "
                    + Convert.ToString(purchaseItem.PiRowInSalesMain) + ", \'" + swipeStatus + "\', \'"
                    + purchaseItem.PsGradeStockCode + "\'," + Convert.ToString(purchaseItem.PsTierID)
                    + ", " + Convert.ToString(purchaseItem.PsLevelID) + ", " + Convert.ToString(purchaseItem.pdTaxFreePrice)
                    + " , \'" + Convert.ToString(purchaseItem.PiOverrideCode) + "\', \'" + purchaseItem.PsOverrideFormNumber + "\', "
                    + " \'" + purchaseItem.PsOverrideDetails + "\', " + Convert.ToInt32(purchaseItem.ProdType) + ", "
                    + Convert.ToString(purchaseItem.TillID) + ", " + Convert.ToString(purchaseItem.PdOriginalPrice) + ", "
                    + Convert.ToString(purchaseItem.pdQuantity) + ", " + Convert.ToString(Math.Round(purchaseItem.pdQuantity * purchaseItem.pdTaxFreePrice, 2))
                    + ", " + Convert.ToString(Math.Round(purchaseItem.petaxInclPrice - purchaseItem.pdQuantity * purchaseItem.pdTaxFreePrice, 2))
                    + ", \'" + DateTime.Now.ToString("yyyy-MM-dd") + "\', \'" + till.ShiftDate.ToString("yyyy-MM-dd")
                    + "\', \'" + purchaseItem.PsTreatyNo.PhoneNumber + "\')";
            }
            else
            {
                sSql = "INSERT INTO PurchaseItem (TreatyNo, Sale_No, Line_No,"
                    + "IDCaptureMethod, CscPurchaseItemKey, TierID, LevelID, TaxFreePrice, "
                    + "OverrideCodeFK, OverrideDocNumber, OverrideExplanation, CategoryCodeFK, Till_No, "
                    + "OriginalPrice, Quantity, Amount, TotalTaxSaved ) VALUES (\'"
                    + purchaseItem.PsTreatyNo.GetTreatyNo() + "\',"
                    + Convert.ToString(purchaseItem.InvoiceID) + ", "
                    + Convert.ToString(purchaseItem.PiRowInSalesMain) + ", \'"
                    + swipeStatus + "\', \'" + purchaseItem.PsProductKey + "\', 0, 0, "
                    + " " + Convert.ToString(purchaseItem.pdTaxFreePrice) + " , \'"
                    + Convert.ToString(purchaseItem.PiOverrideCode) + "\', \'" + purchaseItem.PsOverrideFormNumber
                    + "\', " + " \'" + purchaseItem.PsOverrideDetails + "\', " + Convert.ToInt32(purchaseItem.ProdType)
                    + ", " + Convert.ToString(purchaseItem.TillID) + ", " + Convert.ToString(purchaseItem.PdOriginalPrice)
                    + ", " + Convert.ToString(purchaseItem.pdQuantity) + ", " + Convert.ToString(Math.Round(purchaseItem.pdQuantity * purchaseItem.pdTaxFreePrice, 2))
                    + ", " + Convert.ToString(Math.Round(purchaseItem.petaxInclPrice - purchaseItem.pdQuantity * purchaseItem.pdTaxFreePrice, 2)) + ")";
            }
            

            _taxExemptService.UpdatePurchaseItem(sSql);
            return true;
        }

    }
}
