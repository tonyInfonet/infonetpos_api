using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System.Net;
using RTVP.POSService;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TreatyManager : ManagerBase, ITreatyManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ITreatyService _treatyService;
        private readonly ITeSystemManager _teSystemManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="treatyService"></param>
        /// <param name="teSystemManager"></param>
        public TreatyManager(IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ITreatyService treatyService,
            ITeSystemManager teSystemManager)
        {
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _treatyService = treatyService;
            _teSystemManager = teSystemManager;
        }

        /// <summary>
        /// Is Valid Treaty Number
        /// </summary>
        /// <param name="sTreatyNo">Treaty number</param>
        /// <param name="captureMethod">capture method</param>
        /// <param name="user">User</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        public bool IsValidTreatyNo(ref string sTreatyNo, ref short captureMethod, User user, out ErrorMessage error)
        {
            bool returnValue;
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //   card validation for Ontario gas tax refunds e-service
            if (_policyManager.TAX_EXEMPT_FNGTR)
            {
                var rsInvalidCert = _treatyService.IsInvalidCertificate(sTreatyNo);
                if (rsInvalidCert)
                {
                    //Chaps_Main.DisplayMessage(0, (short)8800, temp_VbStyle, null, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8800, null, ExclamationOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return false;
                }
                if (sTreatyNo.Length < 9 || sTreatyNo.Length > 14)
                {
                    //Chaps_Main.DisplayMessage(0, (short)8801, temp_VbStyle2, null, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8801, null, ExclamationOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return false;
                }
                if (sTreatyNo.Length <= 14)
                {
                    returnValue = true;
                }
                else
                {
                    //Chaps_Main.DisplayMessage(0, (short)8801, temp_VbStyle3, null, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8801, null, ExclamationOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return false;
                }
            }
            else
            {
                //   end
                returnValue = sTreatyNo.Length == _policyManager.TreatyNumDgt;
            }

            // SITE
            if (_policyManager.SITE_RTVAL)
            {
                short tillMode = user.User_Group.Code == "Trainer" ? (short)2 : (short)1;
                // SetCustomer doesn't validate the customer; RTVP dll always returns 0 (success)

                if (Variables.RTVPService == null)
                {
                    Variables.RTVPService = new Transaction();
                }
                var ret = System.Convert.ToInt16(Variables.RTVPService.SetCustomer(sTreatyNo, captureMethod));
                WriteToLogFile("Response is " + System.Convert.ToString(ret) + " from SetCustomer sent with parameters " + sTreatyNo + "," + System.Convert.ToString(captureMethod));
                ret = System.Convert.ToInt16(Variables.RTVPService.SetTillMode(tillMode));
                WriteToLogFile("Response is " + System.Convert.ToString(ret) + " from SetTillMode sent with parameter " + System.Convert.ToString(tillMode));
            }

            return returnValue;
        }

        /// <summary>
        /// Get Quota
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="eProdType">Product type</param>
        /// <param name="dOutQuota">Quota</param>
        /// <returns>True or false</returns>
        public bool GetQuota(ref teTreatyNo treatyNo, ref mPrivateGlobals.teProductEnum eProdType,
            ref double dOutQuota)
        {
            treatyNo.CheckInit();

            //   for real time validation SITE and Jan 12, 2015 for FNGTR validation this validation not required
            //   moved in processing sale to display the remaining qty
            if (_policyManager.SITE_RTVAL || _policyManager.TAX_EXEMPT_FNGTR)
            {
                return true;
            }
            //   end

            var sFieldName = GetQuotaField(eProdType);

            dOutQuota = _treatyService.GetQuota(sFieldName, treatyNo.TreatyNumber);

            
            if (_policyManager.TE_Type != "SITE") return true;
            if (eProdType == mPrivateGlobals.teProductEnum.eCigar | eProdType == mPrivateGlobals.teProductEnum.eLooseTobacco)
            {
                dOutQuota = dOutQuota * _teSystemManager.GetCigaretteEquivalentUnits((mPrivateGlobals.teTobaccoEnum)System.Convert.ToInt16(eProdType));
            }
            return true;
        }

        
        //Name: AddToQuota
        //Description: Adds the given quantity to the purchaser's current quota.  This
        // function will take care of cigarette equivalency math, the caller only
        // needs to enter the quantity sold.  For example, if a 30g pouch of tobacco
        // is sold, this function will look up the cigarette equivalency and do the
        // appropriate conversion internally.
        //Inputs: eProdType, the product type that was sold.
        //   dQuantity: The amount sold, grams for tobacco, number of cigarettes, number
        //      of cigars or number of liters of fuel.
        //Outputs: Returns True if function worked ok.  If False, check GetLastError()
        //Postconditions: On success, the appropriate amount will be added to the person's
        //   quota.  Note, all cigartte equivalency conversions are done internally.
        /// <summary>
        /// Method to add to quota
        /// </summary>
        /// <param name="treatyNo">Treaty umber</param>
        /// <param name="eProdType">Product type</param>
        /// <param name="dQuantity">Quantity</param>
        /// <returns>True or false</returns>
        public bool AddToQuota(teTreatyNo treatyNo, ref mPrivateGlobals.teProductEnum eProdType, double dQuantity)
        {
            treatyNo.CheckInit();
            if (string.IsNullOrEmpty(treatyNo.GetTreatyNo()))
            {
                return false;
            }

            var dConvertedQuantity = dQuantity;

            if (eProdType == mPrivateGlobals.teProductEnum.eCigar | eProdType == mPrivateGlobals.teProductEnum.eLooseTobacco)
            {
                dConvertedQuantity = dConvertedQuantity * mPrivateGlobals.theSystem.teGetCigaretteEquivalentUnits((mPrivateGlobals.teTobaccoEnum)System.Convert.ToInt16(eProdType));
            }

            //Check if an entry in TreatyNo exists for this person.
            var sSql = "SELECT * FROM TreatyNo WHERE TreatyNo=\'" + treatyNo.GetTreatyNo() + "\'";

            var oRecs = _treatyService.GetTreatyNumbers(sSql);

            var sField = GetQuotaField(eProdType);

            if (oRecs.Count == 0)
            {
                //Record does not exist, so insert a new one
                //  New info - Treaty name
                
                //      "VALUES ('" & psTreatyNo & "', " & dConvertedQuantity & ")"
                sSql = "INSERT INTO TreatyNo ( TreatyNo, " + sField + ",TreatyName ) " + "VALUES (\'" + treatyNo.GetTreatyNo() + "\', " + System.Convert.ToString(dConvertedQuantity) + ", \'" + treatyNo.Name + "\')";
                //shiny end
            }
            else
            {
                foreach (var treaty in oRecs)
                {
                    if (treaty.TreatyNumber != treatyNo.GetTreatyNo()) continue;
                    
                    
                    double quantity = sField == "TobaccoQuota" ? treaty.TobaccoQuota : treaty.GasQuota;
                    dConvertedQuantity = System.Convert.ToDouble(dConvertedQuantity + quantity);
                    
                    goto resumeOutofLoop;
                }

                resumeOutofLoop:
                //Record exists, so update the current quota with the new value
                //  Update Treaty name

                sSql = "UPDATE TreatyNo " + "SET " + sField + "=" + (dConvertedQuantity) + " " + " , TreatyName =\'" + treatyNo.Name + "\'  WHERE TreatyNo=\'" + treatyNo.GetTreatyNo() + "\'";
            }
            _treatyService.UpdateTreatyNumber(sSql);

            return true;
        }

        /// <summary>
        /// Method to set remaining tobacco quantity
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="oPurchaseList">Purchase list</param>
        /// <param name="sale">Sale</param>
        /// <param name="remainingTobaccoQuota">Remauning tobacco quota</param>
        public void SetRemainingTobaccoQuantity(ref teTreatyNo treatyNo, ref tePurchaseList oPurchaseList, ref Sale sale, double remainingTobaccoQuota)
        {
            treatyNo.RemainingTobaccoQuantity = remainingTobaccoQuota;
            //   set which items in the purchase list require override
            short index;
            var productType = default(mPrivateGlobals.teProductEnum);

            var runningQuota = remainingTobaccoQuota;
            for (index = oPurchaseList.Count(); index >= 1; index--)
            {
                oPurchaseList.Item(index).GetProductType(ref productType);
                if (productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                {
                    double dblTempQty = oPurchaseList.Item(index).Quantity * oPurchaseList.Item(index).UnitsPerPkg * oPurchaseList.Item(index).ItemEquivalence;
                    if (dblTempQty <= runningQuota)
                    {
                        oPurchaseList.Item(index).OverrideRequired = false;
                        treatyNo.OverrideRequired = false;
                        treatyNo.ValidTreatyNo = true;
                    }
                    else
                    {
                        oPurchaseList.Item(index).OverrideRequired = true;
                        treatyNo.OverrideRequired = true;
                        if (!treatyNo.ValidTreatyNo)
                        {
                            treatyNo.ValidTreatyNo = false;
                        }
                        var rowNumber = oPurchaseList.Item(index).GetRowInSalesMain();
                        sale.Sale_Lines[rowNumber].IsTaxExemptItem = false;
                        sale.Sale_Lines[rowNumber].overrideCode = 0;
                        oPurchaseList.RemoveItem(index);
                    }
                    runningQuota = runningQuota - dblTempQty;
                }
            }
            //   end
        }

        /// <summary>
        /// Method to set remaining fuel quantity
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="oPurchaseList">Purchase list</param>
        /// <param name="sale">Sale</param>
        /// <param name="remainingFuelQuantity">Remaining fuel quantity</param>
        public void SetRemainingFuelQuantity(ref teTreatyNo treatyNo, ref tePurchaseList oPurchaseList, ref Sale sale, double remainingFuelQuantity)
        {
            //  set which items in the purchase list require override
            treatyNo.RemainingFuelQuantity = remainingFuelQuantity;
            short index;
            var productType = default(mPrivateGlobals.teProductEnum);

            var runningQuota = remainingFuelQuantity;
            for (index = oPurchaseList.Count(); index >= 1; index--)
            {
                oPurchaseList.Item(index).GetProductType(ref productType);
                if (
                    !(productType == mPrivateGlobals.teProductEnum.eGasoline |
                      productType == mPrivateGlobals.teProductEnum.eDiesel |
                      productType == mPrivateGlobals.teProductEnum.ePropane |
                      productType == mPrivateGlobals.teProductEnum.emarkedGas |
                      productType == mPrivateGlobals.teProductEnum.emarkedDiesel)) continue;
                double dblTempQty = oPurchaseList.Item(index).Quantity * oPurchaseList.Item(index).UnitsPerPkg * oPurchaseList.Item(index).ItemEquivalence;
                if (dblTempQty <= runningQuota)
                {
                    oPurchaseList.Item(index).OverrideRequired = false;
                    treatyNo.ValidTreatyNo = true;
                    treatyNo.OverrideRequired = false;
                }
                else
                {
                    oPurchaseList.Item(index).OverrideRequired = true;
                    treatyNo.OverrideRequired = true;
                    if (!treatyNo.ValidTreatyNo)
                    {
                        treatyNo.ValidTreatyNo = false;
                    }
                    var rowNumber = oPurchaseList.Item(index).GetRowInSalesMain();
                    sale.Sale_Lines[rowNumber].IsTaxExemptItem = false;
                    sale.Sale_Lines[rowNumber].overrideCode = 0;
                    oPurchaseList.RemoveItem(index);
                }
                runningQuota = runningQuota - dblTempQty;
            }
            //   end
        }

        
        //Name: Init
        //Description: Used to initialize the class with a Treaty Number.  The treaty
        // number can be valid or invalid.  Invalid treaty numbers are permitted because
        // the system is supposed to allow cashiers to provide tax exempt purchases
        // for invalid treaty numbers at their own risk.
        //Inputs:
        // sTreatyNo: the Treaty Number used with this class.  May be a valid treaty
        //    number or it may be garbage.  This conforms to the Sask Fin spec.
        // bSwiped: True if the treaty number was swiped, false if it was keyed in.
        //Note: In the original design, if swiped is True, the sTreatyNo parameter
        // will be treated as "alternate purchase identifier" and the actual TreatyNo
        // will be looked up in a table.
        //Note: In the current version this method will always return True.  Should there
        // be a requirements change to deny tax exempt purchases to invalid treaty numbers
        //(as originally specified), this method will return false if the treaty number is
        // invalid.  Until that time, it will always return True.
        /// <summary>
        /// Method to initialise treaty number
        /// </summary>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="sTreatyNo">Treaty value</param>
        /// <param name="bSwiped">Swiped or not</param>
        /// <returns>True or false</returns>
        public bool Init(ref teTreatyNo treatyNo, string sTreatyNo, bool bSwiped)
        {

            treatyNo.TreatyNumber = sTreatyNo;
            treatyNo.isSwiped = bSwiped;
            treatyNo.IsInit = true;
            treatyNo.Name = _treatyService.GetTreatyName(sTreatyNo);
            return treatyNo.IsInit;
        }

        #region Private methods

        /// <summary>
        /// Get Quota fields
        /// </summary>
        /// <param name="eProdType">Product type</param>
        /// <returns>Quota field</returns>
        private string GetQuotaField(mPrivateGlobals.teProductEnum eProdType)
        {
            string sResult = "";

            //REVISIT: Raise an error if input is eNone
            if ((eProdType == mPrivateGlobals.teProductEnum.eCigarette) || (eProdType == mPrivateGlobals.teProductEnum.eLooseTobacco) || (eProdType == mPrivateGlobals.teProductEnum.eCigar))
            {
                sResult = "TobaccoQuota";
            } // hen
            else if ((eProdType == mPrivateGlobals.teProductEnum.eGasoline) || (eProdType == mPrivateGlobals.teProductEnum.eDiesel) || (eProdType == mPrivateGlobals.teProductEnum.emarkedGas) || (eProdType == mPrivateGlobals.teProductEnum.emarkedDiesel))
            {
                sResult = "GasQuota";
            }
            else if (eProdType == mPrivateGlobals.teProductEnum.ePropane)
            {
                if (_policyManager.TE_Type == "AITE")
                {
                    sResult = "PropaneQuota";
                }
                else
                {
                    sResult = "GasQuota";
                }
            }
            else if (eProdType == mPrivateGlobals.teProductEnum.eNone)
            {
                Information.Err().Raise(mPrivateGlobals.kNoQuotaForNoneCategoryNum, mPrivateGlobals.kNoQuotaForNoneCategoryDesc);
            }
            else
            {
                //REVISIT: Raise an error here too maybe
                Information.Err().Raise(mPrivateGlobals.kNoQuotaForUnknownProductNum, mPrivateGlobals.kNoQuotaForUnknownProductDesc);
            }

            return sResult;
        }

        #endregion

    }
}
