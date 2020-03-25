using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PurchaseListManager : ManagerBase, IPurchaseListManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ITreatyManager _treatyManager;
        private readonly IPurchaseItemManager _purchaseItemManager;
        private readonly ITeSystemManager _teSystemManager;
        private readonly ITaxExemptService _taxExemptService;
        private readonly IFuelPumpService _fuelPumpService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleManager _saleManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="treatyManager"></param>
        /// <param name="purchaseItemManager"></param>
        /// <param name="teSystemManager"></param>
        /// <param name="taxExemptService"></param>
        public PurchaseListManager(IPolicyManager policyManager,
            ITreatyManager treatyManager,
            IPurchaseItemManager purchaseItemManager,
            ITeSystemManager teSystemManager,
            ITaxExemptService taxExemptService,
            IFuelPumpService fuelPumpService,
            IApiResourceManager resourceManager,
            ISaleManager saleManager)
        {
            _policyManager = policyManager;
            _treatyManager = treatyManager;
            _purchaseItemManager = purchaseItemManager;
            _teSystemManager = teSystemManager;
            _taxExemptService = taxExemptService;
            _fuelPumpService = fuelPumpService;
            _resourceManager = resourceManager;
            _saleManager = saleManager;
        }

        /// <summary>
        /// Add Item to Purchase List
        /// </summary>
        /// <param name="purchaseList">Purchase list</param>
        /// <param name="sale">Sale</param>
        /// <param name="peTreatyNo">Treaty number</param>
        /// <param name="sProductKey">Product key</param>
        /// <param name="dQuantity">Quantity</param>
        /// <param name="dOriginalPrice">Original price</param>
        /// <param name="iRowNumberInSalesMainForm">Row number</param>
        /// <param name="stockcode">Stock code</param>
        /// <param name="taxInclPrice">Tax included price</param>
        /// <param name="isFuelItem">Is fuel item or not</param>
        /// <returns>True or false</returns>
        public bool AddItem(ref tePurchaseList purchaseList, ref Sale sale,
            ref teTreatyNo peTreatyNo, ref string sProductKey, ref double dQuantity,
            ref double dOriginalPrice, ref short iRowNumberInSalesMainForm,
            ref string stockcode, ref double taxInclPrice, ref bool isFuelItem)
        {
            
            double quota = 0;
            var productType = default(mPrivateGlobals.teProductEnum); 
            var limitMaxType = default(mPrivateGlobals.teLimitEnum); 
            var limitThresholdType = default(mPrivateGlobals.teLimitEnum); 
            double limitMax = 0; 
            double limitThreshold = 0; 
            short ret;
            string formatString = "";
            double runningQuotaF = 0;
            short runningQuotaT = 0;

            var oItem = new tePurchaseItem { BIsInit = true };

            //initialize item then add to purchase list
            if (!_purchaseItemManager.Init(ref oItem, ref peTreatyNo, ref sProductKey, dOriginalPrice, dQuantity, iRowNumberInSalesMainForm, sale.Sale_Num, sale.TillNumber, ref stockcode, taxInclPrice, isFuelItem, purchaseList.NoRTVP))
            {
                purchaseList.SetLastError(mPrivateGlobals.theSystem.teGetLastError());//"Failed to Add Item: " & oDb.GetLastError()
                return false;
            }

            oItem.GetProductType(ref productType);

            // Validation
            if (_policyManager.SITE_RTVAL && !purchaseList.NoRTVP) // And Not Me.NoRTVP  
            {
                if (productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                {
                    ret = Convert.ToInt16(Variables.RTVPService.SetItemUPC(oItem.LineItem, sProductKey, (int)productType));
                    WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetItemUPC sent with parameters " + Convert.ToString(oItem.LineItem) + "," + sProductKey + "," + Convert.ToString(productType));
                    formatString = "0.00";
                }
                else
                {
                    SetStockCode(ref oItem, stockcode);
                    ret = Convert.ToInt16(Variables.RTVPService.SetItemUPC(oItem.LineItem, oItem.ItemUPC, (int)productType));
                    WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetItemUPC sent with parameters " + Convert.ToString(oItem.LineItem) + "," + oItem.ItemUPC + "," + Convert.ToString(productType));
                    formatString = "0.000";
                }
                if (ret != 0)
                {
                    purchaseList.RTVPCommand = "SetItemUPC";
                    purchaseList.RTVPError = true;
                    purchaseList.RTVPResponse = ret;
                    return false;
                }
                purchaseList.RTVPError = false;

                ret = Convert.ToInt16(Variables.RTVPService.SetItemTotal(oItem.LineItem,
                    double.Parse((dQuantity * oItem.GetTaxFreePrice()).ToString(formatString))));
                WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetItemTotal sent with parameters " + Convert.ToString(oItem.LineItem) + "," +
                     (dQuantity * oItem.GetTaxFreePrice()).ToString(formatString));
                if (ret != 0)
                {
                    purchaseList.RTVPCommand = "SetItemTotal";
                    purchaseList.RTVPError = true;
                    purchaseList.RTVPResponse = ret;
                    return false;
                }
                purchaseList.RTVPError = false;
            }
            //   end

            var unitsPerPkg = oItem.UnitsPerPkg;
            var quantity = dQuantity * unitsPerPkg;

            // validation
            if (_policyManager.SITE_RTVAL && !purchaseList.NoRTVP) // And Not Me.NoRTVP  
            {
                // based on email from SITE all products need SetItemEquivalence including fuel
                // for fuel we decided to set Equivalence to 1, meaning send the quantity
                double dblEquivalent;
                if (productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                {
                    dblEquivalent = _teSystemManager.GetCigaretteEquivalentUnits((mPrivateGlobals.teTobaccoEnum)Convert.ToInt16(productType));
                }
                else
                {
                    dblEquivalent = 1;
                }
                oItem.Quantity = (float)dQuantity; //  
                oItem.ItemEquivalence = (float)dblEquivalent; //  
                oItem.UnitsPerPkg = unitsPerPkg;

                ret = Convert.ToInt16(Variables.RTVPService.SetItemEquivalence(oItem.LineItem, double.Parse((quantity * dblEquivalent).ToString(formatString))));
                WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetItemEquivalence sent with parameters " + Convert.ToString(oItem.LineItem) + "," +
                     (quantity * dblEquivalent).ToString(formatString));
                if (ret != 0)
                {
                    purchaseList.RTVPCommand = "SetItemEquivalence";
                    purchaseList.RTVPError = true;
                    purchaseList.RTVPResponse = ret;
                    return false;
                }
                purchaseList.RTVPError = false;
            }
            else
            {
                _treatyManager.GetQuota(ref peTreatyNo, ref productType, ref quota);
            }

            //   added the If condition; real time validation for SITE doesn't require next code
            if (!_policyManager.SITE_RTVAL)
            {
                GetLimitType(productType, ref limitMaxType, ref limitThresholdType);
                _teSystemManager.TeGetLimit(limitMaxType, ref limitMax);
                _teSystemManager.TeGetLimit(limitThresholdType, ref limitThreshold);

                if (productType == mPrivateGlobals.teProductEnum.eGasoline | productType == mPrivateGlobals.teProductEnum.eDiesel | productType == mPrivateGlobals.teProductEnum.ePropane | productType == mPrivateGlobals.teProductEnum.emarkedGas | productType == mPrivateGlobals.teProductEnum.emarkedDiesel) // hen
                {
                    runningQuotaF = (float)(runningQuotaF + quantity + quota);

                    //   added next two lines, for FNGTR Gasoline Single Transaction Limit is the only validation required
                    if (_policyManager.TAX_EXEMPT_FNGTR)
                    {
                        if (runningQuotaF > limitMax)
                        {
                            purchaseList.IsFuelOverLimit = true;
                        }
                        else
                        {
                            purchaseList.IsTobaccoOverLimit = false;
                        }
                        purchaseList.IsTobaccoOverLimit = false;
                    }
                    else
                    {
                        if (runningQuotaF > limitThreshold)
                        {
                            purchaseList.IsFuelOverLimit = true;
                            
                            
                        }
                        else if (quantity > limitMax)
                        {
                            

                            purchaseList.IsFuelOverLimit = true;
                        }
                        else
                        {
                            purchaseList.IsFuelOverLimit = false;
                        }
                    }
                    if (!_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                    {
                        purchaseList.IsFuelOverLimit = false;
                    }
                }
                else if (productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                {
                    runningQuotaT = (short)(runningQuotaT + quantity + quota);

                    if (runningQuotaT > limitThreshold)
                    {
                        purchaseList.IsTobaccoOverLimit = true;
                    }
                    else if (runningQuotaT > limitMax)
                    {
                        purchaseList.IsTobaccoOverLimit = true;
                    }
                    else
                    {
                        purchaseList.IsTobaccoOverLimit = false;
                    }

                    
                    
                    if (!_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                    {
                        purchaseList.IsTobaccoOverLimit = false;
                    }
                }
            }
            // real time validation end if '  

            purchaseList.AddItemInCollection(oItem); //save the item in purchase list

            
            
            purchaseList.PeQuantity = quantity;
            purchaseList.PsProductKey = sProductKey;
            return true;
        }

        //Description: All tax exempt items in the list have their quantities
        // added to the weekly running total for the given TreatyNo. As well,
        // all purchases are saved to TaxExempt.mdb.
        /// <summary>
        /// Method to save and assign to quotas
        /// </summary>
        /// <param name="purchaseList">Purchase list</param>
        /// <param name="user">User</param>
        /// <param name="till">Till</param>
        /// <param name="saveToDb">Save to db</param>
        /// <returns>True or false</returns>
        public bool SaveAndAssignToQuotas(ref tePurchaseList purchaseList,
            User user, Till till, bool saveToDb = true)
        {
            short index;
            var pType = default(mPrivateGlobals.teProductEnum);

            float vTaxSaved = 0;

            for (index = 1; index <= purchaseList.purchaseItems.Count; index++)
            {

                purchaseList.Item(index).GetProductType(ref pType);

                
                if (saveToDb)
                {
                    

                    
                    
                    
                    
                    
                    if (!_treatyManager.AddToQuota(purchaseList.GetTreatyNo(), ref pType, purchaseList.Item(index).GetQuantity() * purchaseList.Item(index).GetUnitsPerPkg()))
                    {
                        
                        purchaseList.SetLastError(purchaseList.GetTreatyNo().GetLastError());
                        return false;
                    }
                    if (!_purchaseItemManager.Save(purchaseList.Item(index), user, till))
                    {
                        purchaseList.SetLastError(purchaseList.Item(index).GetLastError());
                        return false;
                    }
                    

                } 
                  //  - We need to show totalexemptedtax + gST\PST exemption
                  
                  

                //If Me.Item(Index).WasTaxExemptReturn Then
                //        vTaxSaved = vTaxSaved + (Me.Item(Index).GetOriginalPrice - Me.Item(Index).GetTaxFreePrice) * Me.Item(Index).GetQuantity
                //Else
                vTaxSaved = vTaxSaved + (purchaseList.Item(index).GetTaxIncludeAmount() - (purchaseList.Item(index).GetTaxFreePrice() * purchaseList.Item(index).GetQuantity()));
                //End If
                // 
                
            }

            
            purchaseList.TotalExemptedTax = (float)(Math.Round(vTaxSaved, 2));
            
            return true;
        }


        public tePurchaseList GetPurchaseList(int saleNumber, int tillNumber,string userCode, string treatyNumber, string treatyName, out ErrorMessage error)
        {
            var sale  = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out error);
            var oTreatyNo = new teTreatyNo() { TreatyNumber = treatyNumber, Name = treatyName };

            var offSet = _policyManager.LoadStoreInfo().OffSet;
            double originalPrice = 0;
            double taxIncldAmount = 0;

            sale.TreatyNumber = oTreatyNo.TreatyNumber;
            sale.TreatyName = oTreatyNo.Name;

            error = null;

            var oPurchaseList = new tePurchaseList();

            oPurchaseList.Init(oTreatyNo, sale.Sale_Num, sale.TillNumber);

            var y = oPurchaseList.GetTreatyNo();

            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                var sl = tempLoopVarSl;
                string strError;

                if (sl.Amount < 0 && sl.IsTaxExemptItem)
                {
                    taxIncldAmount = -1 * sl.TaxInclPrice;
                    //shiny - Dec9, 2009 - Squamish nation
                    //                OriginalPrice = sl.Regular_Price

                    if (_policyManager.TE_ByRate == false) // Regualr Site customers SITE , TE_By rate = no
                    {
                        originalPrice = sl.Regular_Price; //  - editing the price for TE is keeping different price in purchaseitem and saleline'SL.price

                    }
                    else if (_policyManager.TE_ByRate) //squamish SITE , TE_By rate = yes
                    {
                        originalPrice = sl.price;
                    }
                }
                else
                {
                    //shiny - Dec9, 2009 - Squamish nation
                    //                OriginalPrice = sl.Regular_Price

                    if (_policyManager.TE_ByRate == false) // Regualr Site customers SITE , TE_By rate = no
                    {
                        originalPrice = sl.Regular_Price; //  - editing the price for TE is keeping different price in purchaseitem and saleline'SL.price

                    }
                    else if (_policyManager.TE_ByRate) //squamish SITE , TE_By rate = yes
                    {
                        originalPrice = sl.price;
                    }
                }



                if (sl.ProductIsFuel && !sl.IsPropane)
                {
                    if (_policyManager.USE_FUEL)
                    {

                        if (Variables.gPumps == null)
                        {
                            Variables.gPumps = _fuelPumpService.InitializeGetProperty(PosId, _policyManager.TAX_EXEMPT, _policyManager.TE_ByRate, _policyManager.TE_Type, _policyManager.AuthPumpPOS);
                        }
                        string tempSProductKey = _teSystemManager.TeMakeFuelKey(sl.GradeID, Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).TierID), Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).LevelID));
                        double tempDQuantity = sl.Quantity;
                        var tempIRowNumberInSalesMainForm = sl.Line_Num;
                        bool tempIsFuelItem = true;
                        string tempStockCode = sl.Stock_Code;
                        if (!AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey, ref tempDQuantity, ref originalPrice, ref tempIRowNumberInSalesMainForm, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem))
                        {
                            strError = oPurchaseList.GetLastError();
                            if (strError == "2")
                            {
                                //MsgBox ("Cannot load Tax Exempt price, Please set Tax Exempt Category for Grade-" & SL.GradeID & " first in the BackOffice system! ")
                                //_resourceManager.CreateMessage(offSet,this, 17, temp_VbStyle, sl.GradeID, 0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 17, sl.GradeID, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                                return null;
                            }
                            if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                            {
                                //MsgBox ("Error(" & strError & ") for getting Tax Exempt price, will use original price for this sale!")
                                //_resourceManager.CreateMessage(offSet,this, 18, temp_VbStyle2, strError, 0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                                return null;
                            }
                            break;
                        }
                    }
                    else
                    {
                        var tempSProductKey2 = sl.Stock_Code;
                        double tempDQuantity2 = sl.Quantity;
                        var tempIRowNumberInSalesMainForm2 = sl.Line_Num;
                        bool tempIsFuelItem2 = false;
                        var tempStockCode = sl.Stock_Code;
                        if (!AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey2, ref tempDQuantity2, ref originalPrice, ref tempIRowNumberInSalesMainForm2, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem2))
                        {
                            strError = oPurchaseList.GetLastError();
                            if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                            {
                                //_resourceManager.CreateMessage(offSet,this, 18, temp_VbStyle3, strError, 0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                                return null;
                            }
                        }
                    }
                }
                else
                {
                    var tempSProductKey3 = sl.Stock_Code;
                    double tempDQuantity3 = sl.Quantity;
                    short tempIRowNumberInSalesMainForm3 = sl.Line_Num;
                    bool tempIsFuelItem3 = false;
                    var tempStockCode = sl.Stock_Code;
                    if (!AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey3, ref tempDQuantity3, ref originalPrice, ref tempIRowNumberInSalesMainForm3, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem3))
                    {
                        strError = oPurchaseList.GetLastError();
                        if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                        {
                            //_resourceManager.CreateMessage(offSet,this, 18, temp_VbStyle4, strError, 0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                            return null;
                        }
                    }
                }
            }

            var x = oPurchaseList.GetTreatyNo();

            CacheManager.AddPurchaseListSaleForTill(tillNumber, saleNumber, oPurchaseList);
            return oPurchaseList;
        }


        #region Private methods

        /// <summary>
        /// Method to get limit type
        /// </summary>
        /// <param name="productType">Product type</param>
        /// <param name="limitMax">Maximum limit</param>
        /// <param name="limitThreshold">Threshold limit</param>
        /// <returns>Limit type</returns>
        private void GetLimitType(mPrivateGlobals.teProductEnum productType, ref mPrivateGlobals.teLimitEnum limitMax, ref mPrivateGlobals.teLimitEnum limitThreshold)
        {

            switch (productType)
            {
                case mPrivateGlobals.teProductEnum.eCigarette:
                    limitMax = mPrivateGlobals.teLimitEnum.eCigLimit;
                    limitThreshold = mPrivateGlobals.teLimitEnum.eCigMaxThreshhold;
                    return;
                case mPrivateGlobals.teProductEnum.eCigar:
                    limitMax = mPrivateGlobals.teLimitEnum.eCigarLimit;
                    limitThreshold = mPrivateGlobals.teLimitEnum.eCigarMaxThreshhold;
                    return;
                case mPrivateGlobals.teProductEnum.eLooseTobacco:
                    limitMax = mPrivateGlobals.teLimitEnum.eTobaccoLimit;
                    limitThreshold = mPrivateGlobals.teLimitEnum.eTobaccoMaxThreshhold;
                    return;
                case mPrivateGlobals.teProductEnum.eGasoline:
                case mPrivateGlobals.teProductEnum.emarkedGas:
                    limitMax = mPrivateGlobals.teLimitEnum.eGasTransactionLimit; // 
                    limitThreshold = mPrivateGlobals.teLimitEnum.eGasLimit;
                    return;
                case mPrivateGlobals.teProductEnum.eDiesel:
                case mPrivateGlobals.teProductEnum.emarkedDiesel:
                    limitMax = mPrivateGlobals.teLimitEnum.eDieselTransactionLimit; // 
                    limitThreshold = mPrivateGlobals.teLimitEnum.eDieselLimit;
                    return;
                case mPrivateGlobals.teProductEnum.ePropane:
                    limitMax = mPrivateGlobals.teLimitEnum.ePropaneTransactionLimit;
                    limitThreshold = mPrivateGlobals.teLimitEnum.ePropaneLimit;
                    return;
                case mPrivateGlobals.teProductEnum.eNone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(productType), productType, null);
            }
        }

        /// <summary>
        /// Method to set stock code
        /// </summary>
        /// <param name="purchaseItem">Purchse item</param>
        /// <param name="stockCode">Stock code</param>
        private void SetStockCode(ref tePurchaseItem purchaseItem, string stockCode)
        {
            purchaseItem.stockcode = stockCode;
            purchaseItem.ItemUPC = _taxExemptService.GetItemUpc(stockCode);
        }

       
        #endregion
    }
}
