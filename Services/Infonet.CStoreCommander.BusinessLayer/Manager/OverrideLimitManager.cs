using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class OverrideLimitManager : ManagerBase, IOverrideLimitManager
    {

        private readonly IApiResourceManager _resourceManager;
        private readonly IPolicyManager _policyManager;
        private readonly ISaleManager _saleManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ITeSystemManager _teSystemManager;
        private readonly ITreatyManager _treatyManager;
        private readonly ISiteMessageService _siteMessageService;
        private readonly ITenderManager _tenderManager;
        private readonly IPurchaseListManager _purchaseListManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="teSystemManager"></param>
        /// <param name="treatyManager"></param>
        /// <param name="siteMessageService"></param>
        /// <param name="tenderManager"></param>
        public OverrideLimitManager(
            IApiResourceManager resourceManager,
            IPolicyManager policyManager,
            ISaleManager saleManager,
            ISaleLineManager saleLineManager,
            ITeSystemManager teSystemManager,
            ITreatyManager treatyManager,
            ISiteMessageService siteMessageService,
            ITenderManager tenderManager,
            IPurchaseListManager purchaseListManager)
        {
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _saleManager = saleManager;
            _saleLineManager = saleLineManager;
            _teSystemManager = teSystemManager;
            _treatyManager = treatyManager;
            _siteMessageService = siteMessageService;
            _tenderManager = tenderManager;
            _purchaseListManager = purchaseListManager;
        }


        private string _peProductKey;

        private bool _okOverrideF; //Override was successful for Fuel
        private bool _okOverrideT; //Override was successful for Tobacco
        private bool _okOverrideTm; //Override was successful for Tobacco Threshold
        private bool _isOverMax;
        private bool _isOverThreshold;

        private bool _isOverrideRequiredF;
        private bool _isOverrideRequiredT;

        private bool _isOverrideRequiredTm;

        private short _runningQuotaT; //runningQuota Tobacco
        private float _runningQuotaF; //runningQuota Fuel
        private float _runningQuotaFt; //runningQuota Fuel Transaction
        private mPrivateGlobals.teLimitEnum _limitMaxType;
        private mPrivateGlobals.teLimitEnum _limitThresholdType;
        private bool _isSuccessful; //Field to indicate if Ok was successful
        private double _peQuantity;
        private double _peUnitsPerPkg;
        private short _itemIndex;

        /// <summary>
        /// Method to complete override limit
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Sale summary response</returns>
        public SaleSummaryResponse CompleteOverrideLimit(int tillNumber, int saleNumber,
            byte registerNumber, string userCode, out ErrorMessage error)
        {
            var result = new SaleSummaryResponse();
            var oPurchaseList = CacheManager.GetPurchaseListSaleForTill(tillNumber, saleNumber);
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);
            var currentUser = CacheManager.GetUser(userCode);
            short index;
            short i;
            short[] arraylist = null;
            i = 0;
            var blremove = false;
            short nOverRide = 0;

            if (!_policyManager.USE_OVERRIDE || _policyManager.TE_Type != "SITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select SITE Tax Exempt and enable Use override policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            if (oPurchaseList == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "The request is invalid.",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }


            //For the items for which override required but not done, restore original values
            for (index = 1; index <= oPurchaseList.Count(); index++)
            {
                if (oPurchaseList.Item(index).OverrideRequired)
                {
                    if (!oPurchaseList.Item(index).IsOverrideDone())
                    {
                        i++;

                        object rowNumber = oPurchaseList.Item(index).GetRowInSalesMain();
                        sale.Sale_Lines[rowNumber].IsTaxExemptItem = false;
                        sale.Sale_Lines[rowNumber].overrideCode = 0;
                        if (_policyManager.TE_ByRate == false)
                        {
                            var saleLine = sale.Sale_Lines[rowNumber];
                            _saleLineManager.SetPrice(ref saleLine, oPurchaseList.Item(index).GetOriginalPrice());
                        }
                        else
                        {
                            var saleLine = sale.Sale_Lines[rowNumber];
                            _saleLineManager.SetPrice(ref saleLine, sale.Sale_Lines[rowNumber].Regular_Price);
                        }
                        Array.Resize(ref arraylist, i + 1);
                        arraylist[i] = index;
                        blremove = true;
                    }
                }
            }

            if (blremove)
            {
                for (i = 1; i <= arraylist.Length - 1; i++) // sinnce for each won't work for this collection
                {
                    oPurchaseList.RemoveItem(arraylist[i]);
                }
            }
            

            //  
            if (_policyManager.TE_Type == "SITE" && _policyManager.TE_ByRate && !_policyManager.SITE_RTVAL)
            {
                if (currentUser != null)
                {
                    WriteToLogFile("User reversed to user " + currentUser.Code + " after override.");
                }
            }
            //   end

            if (ValidateOverride())
            {
                //    isSuccessful = True
                //this.Close();
            }
            else
            {
                
                //Chaps_Main.DisplayMessage(0, (short)5292, temp_VbStyle, "Valid override req\'d", (byte)0);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5292, "Valid override req\'d", CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                // send
                // This is because SITE doesn't care about GST or other tax, only about tax exempt
                // so if the customer is over limit and no valid override code is sent don't send purchasetransaction
                if (_policyManager.SITE_RTVAL)
                {
                    //oTreatyNo.ValidTreatyNo = false;
                }
            }
            if (oPurchaseList.Count() > 0)
            {
                for (i = 1; i <= oPurchaseList.Count(); i++)
                {
                    var rowNumber = oPurchaseList.Item(i).GetRowInSalesMain();

                    var saleLine = sale.Sale_Lines[rowNumber];

                    
                    
                    
                    
                    if (saleLine.Amount < 0 && saleLine.IsTaxExemptItem)
                    {
                        oPurchaseList.Item(i).SetTaxFreePrice((float)saleLine.price);
                        oPurchaseList.Item(i).WasTaxExemptReturn = true;
                    }
                    else
                    {
                        oPurchaseList.Item(i).WasTaxExemptReturn = false;
                        

                        saleLine.IsTaxExemptItem = true;

                        //  added this to take the cost and price for Tax exempt vandor ( if there is a special setup for TE Vendor- otherwise use active vendor values
                        if (string.IsNullOrEmpty(saleLine.TEVendor)) // If TE vendor is not defined consider TE vendor  same as activevendor
                        {
                            saleLine.TEVendor = saleLine.Vendor;
                            saleLine.TECost = saleLine.Cost;
                        }
                        else
                        {
                            if (saleLine.Vendor == saleLine.TEVendor) // IF TAX EXEMPT VENDOR
                            {
                                saleLine.TEVendor = saleLine.Vendor;
                                saleLine.TECost = saleLine.Cost;
                            }
                            else
                            {
                                // Set the cost for the product based on the TE vendor,
                                var cost = _siteMessageService.GetStockCost(saleLine.Stock_Code, saleLine.TEVendor);
                                saleLine.TECost = cost ?? saleLine.Cost;
                            }
                            if (saleLine.TEVendor != saleLine.Vendor)
                            {
                                var getVendorPrice = true;

                                saleLine.Vendor = saleLine.TEVendor;
                                saleLine.Cost = saleLine.TECost;

                                if (_policyManager.TE_ByRate && getVendorPrice && saleLine.ProductIsFuel == false) //shiny - need to do the price change only for nonfuel products
                                {
                                    saleLine.Price_Number = sale.Customer.Price_Code != 0 ? sale.Customer.Price_Code : (short)1;
                                    oPurchaseList.Item(i).SetTaxFreePrice((float)saleLine.price);
                                    oPurchaseList.Item(i).SetOriginalPrice((float)saleLine.Regular_Price);
                                }
                            }
                        }
                        // settings
                        saleLine.OrigVendor = saleLine.Vendor;
                        saleLine.OrigCost = saleLine.Cost;
                        //   end

                        _saleLineManager.SetPrice(ref saleLine, oPurchaseList.Item(i).GetTaxFreePrice());
                        //saleLine.price = oPurchaseList.Item(i).GetTaxFreePrice();
                        // 

                        if (oPurchaseList.Item(i).GetOverrideCode(ref nOverRide))
                        {
                            saleLine.overrideCode = nOverRide;
                        }

                        
                        
                        //saleLine.Amount = decimal.Parse((saleLine.Quantity * saleLine.price).ToString("#0.00"));
                        _saleLineManager.SetAmount(ref saleLine, decimal.Parse((saleLine.Quantity * saleLine.price).ToString("#0.00")));
                        if (saleLine.Prepay)
                        {
                            saleLine.No_Loading = false; // 
                        }
                    }
                }
            }
            _saleManager.ReCompute_Totals(ref sale);
            var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "", out error);

            result.SaleSummary = SaleSummary(sale);
            result.Tenders = tenders;
            return result;
        }

        /// <summary>
        /// Method to check if SUccess
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                var returnValue = _isSuccessful;

                return returnValue;
            }
        }

       
        /// <summary>
        /// Method to validate override limit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="purchaseItemNo">Purchase item info</param>
        /// <param name="documentNo">Document number</param>
        /// <param name="overRideCode">Override code</param>
        /// <param name="documentDetail">Document detail</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public bool DoneOverRideLimit(int saleNumber, int tillNumber, string userCode, short purchaseItemNo, string documentNo, string overRideCode, string documentDetail, out ErrorMessage error)
        {
            error = new ErrorMessage();
            User userRenamed = CacheManager.GetUser(userCode);
            var currentUser = CacheManager.GetUser(userCode);
            var pepurchaseList = CacheManager.GetPurchaseListSaleForTill(tillNumber, saleNumber);


            if (!_policyManager.USE_OVERRIDE || _policyManager.TE_Type != "SITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select SITE Tax Exempt and enable Use override policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            if (pepurchaseList == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "The request is invalid.",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            if (string.IsNullOrEmpty(overRideCode))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select override code",
                        MessageType = CriticalOkMessageType
                    },
                    StatusCode = HttpStatusCode.Conflict
                };
                return false;
            }

            if (purchaseItemNo > pepurchaseList.Count() || purchaseItemNo <= 0)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "The request is invalid.",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }


            short tobaccoIndex = 1; //tobacco index for override
            const short fuelIndex = 1; //fuel index for override
            short tempI = 0;
            mPrivateGlobals.teProductEnum tempType = default(mPrivateGlobals.teProductEnum);
            bool boolRequiresOverride = false;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            // SITE
            // Moved here on Nov 30, 2010, override code no longer required for SITE
            if (_policyManager.SITE_RTVAL)
            {
                if (documentNo == "")
                {
                    //Chaps_Main.DisplayMessage(0, (short)5291, temp_VbStyle, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5291, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideTm = false;
                    return false;
                }
                if (!Information.IsNumeric(documentNo))
                {
                    //Chaps_Main.DisplayMessage(0, (short)5293, temp_VbStyle2, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5293, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideTm = false;
                    return false;
                }

                _okOverrideTm = true;
                _okOverrideF = true;
                _okOverrideT = true;

                var ret = Convert.ToInt16(Variables.RTVPService.SetPermitNo(documentNo));
                WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetPermitNo sent with parameter " + documentNo);
                // back
                for (tempI = 1; tempI <= pepurchaseList.Count(); tempI++)
                {
                    pepurchaseList.Item(tempI).GetProductType(ref tempType);
                    if (pepurchaseList.IsFuelOverLimit)
                    {
                        if (tempType == mPrivateGlobals.teProductEnum.eDiesel | tempType == mPrivateGlobals.teProductEnum.eGasoline | tempType == mPrivateGlobals.teProductEnum.ePropane | tempType == mPrivateGlobals.teProductEnum.emarkedGas | tempType == mPrivateGlobals.teProductEnum.emarkedDiesel)
                        {
                            if (pepurchaseList.Item(tempI).OverrideRequired) //   need to set only for required items
                            {
                                pepurchaseList.Item(tempI).SetOverride(0, documentNo, "");
                            }
                        }
                    }
                    else if (pepurchaseList.IsTobaccoOverLimit)
                    {
                        if (tempType == mPrivateGlobals.teProductEnum.eCigar | tempType == mPrivateGlobals.teProductEnum.eCigarette | tempType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                        {
                            if (pepurchaseList.Item(tempI).OverrideRequired) //   need to set only for required items
                            {
                                pepurchaseList.Item(tempI).SetOverride(0, documentNo, "");
                            }
                        }
                    }
                }
                return true;
            }
            if (_policyManager.TE_Type == "SITE" && _policyManager.TE_ByRate && !_policyManager.SITE_RTVAL && !_policyManager.GetPol("U_OR_TEQUOTA", userRenamed))
            {
                // cigatettes only, verify that requires override
                for (tempI = 1; tempI <= pepurchaseList.Count(); tempI++)
                {
                    pepurchaseList.Item(tempI).GetProductType(ref tempType);
                    if (pepurchaseList.IsTobaccoOverLimit)
                    {
                        if (tempType == mPrivateGlobals.teProductEnum.eCigar | tempType == mPrivateGlobals.teProductEnum.eCigarette | tempType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                        {
                            if (pepurchaseList.Item(tempI).OverrideRequired)
                            {
                                boolRequiresOverride = true;
                                break;
                            }
                        }
                    }
                }
                if (boolRequiresOverride)
                {
                    //Chaps_Main.DisplayMessage(this, (short)99, temp_VbStyle3, null, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 54, 99, null, CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    //if frmUseChg exit without changing user
                    if (currentUser != null)
                    {
                        if (userRenamed.Code == currentUser.Code)
                        {
                            _okOverrideTm = false;
                            return false;
                        }
                        WriteToLogFile("User changed for tobacco override. New user is " + userRenamed.Code + ". After override user will be reversed to previous user " + currentUser.Code);
                    }
                }
                //   end
            }

            var codeLen = (short)(overRideCode.IndexOf(",", StringComparison.Ordinal) + 1);
            if (codeLen <= 1)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "The request is invalid.",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }
            var code = Strings.Left(overRideCode, codeLen - 1).Trim();


            
            short tempCode = 0;
            if (code == "98" || code == "99")
            {
                if (documentNo == "")
                {
                    //Chaps_Main.DisplayMessage(0, (short)5291, temp_VbStyle, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5291, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideTm = false;
                    return false;
                }
                if (!Information.IsNumeric(documentNo))
                {
                    //Chaps_Main.DisplayMessage(0, (short)5293, temp_VbStyle2, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5293, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideTm = false;
                    return false;
                }
                _okOverrideTm = true; 

                if (!_policyManager.SITE_RTVAL) // only
                {
                    _itemIndex = purchaseItemNo;

                    pepurchaseList.Item(_itemIndex).GetProductType(ref tempType);

                    
                    for (tempI = tobaccoIndex; tempI <= pepurchaseList.Count(); tempI++)
                    {
                        if (tempType == mPrivateGlobals.teProductEnum.eCigarette | tempType == mPrivateGlobals.teProductEnum.eCigar | tempType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                        {

                            //        productType = eCigarette Or productType = ecigar Or productType = eLoosetobacco Then
                            if (pepurchaseList.Item(tempI).OverrideRequired) //   need to set only for required items
                            {
                                pepurchaseList.Item(_itemIndex).SetOverride(short.Parse(code), documentNo, documentDetail);
                            }
                            if (!(tempI + 1 > pepurchaseList.Count()))
                            {
                                pepurchaseList.Item((short)(tempI + 1)).GetProductType(ref tempType);
                            }
                        }
                        else
                        {
                            if (!(tempI + 1 > pepurchaseList.Count()))
                            {
                                pepurchaseList.Item((short)(tempI + 1)).GetProductType(ref tempType);
                            }

                        }

                    }
                }
                
            }
            else if (short.Parse(code) >= 10 && short.Parse(code) <= 15)
            {
                if (documentNo == "")
                {
                    //Chaps_Main.DisplayMessage(0, (short)5291, temp_VbStyle, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5291, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideT = false;
                    return false;
                }
                if (!Information.IsNumeric(documentNo))
                {
                    //Chaps_Main.DisplayMessage(0, (short)5293, temp_VbStyle2, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5293, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideT = false;
                    return false;
                }
                _okOverrideT = true; 
                if (!_policyManager.SITE_RTVAL) // only
                {
                    _itemIndex = purchaseItemNo;
                    pepurchaseList.Item(_itemIndex).GetProductType(ref tempType);

                    
                    for (tempI = tobaccoIndex; tempI <= pepurchaseList.Count(); tempI++)
                    {
                        if (tempType == mPrivateGlobals.teProductEnum.eCigarette | tempType == mPrivateGlobals.teProductEnum.eCigar | tempType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                        {


                            pepurchaseList.Item(tempI).GetOverrideCode(ref tempCode);
                            if (pepurchaseList.Item(tempI).OverrideRequired) //   need to set only for required items
                            {
                                pepurchaseList.Item(tempI).SetOverride(short.Parse(code), documentNo, documentDetail);
                            }
                        }
                        if (!(tempI + 1 > pepurchaseList.Count()))
                        {
                            pepurchaseList.Item((short)(tempI + 1)).GetProductType(ref tempType);
                        }
                    }
                }
            }
            else if (short.Parse(code) >= 20 && short.Parse(code) <= 25)
            {
                _itemIndex = purchaseItemNo;
                if (documentNo == "")
                {
                    //Chaps_Main.DisplayMessage(0, (short)5291, temp_VbStyle, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5291, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideF = false;
                    return false;
                }
                if (!Information.IsNumeric(documentNo))
                {
                    //Chaps_Main.DisplayMessage(0, (short)5293, temp_VbStyle2, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5293, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    _okOverrideF = false;
                    return false;
                }

                _okOverrideF = true; 
                if (!_policyManager.SITE_RTVAL) // only
                {
                    pepurchaseList.Item(_itemIndex).GetProductType(ref tempType);

                    
                    for (tempI = fuelIndex; tempI <= pepurchaseList.Count(); tempI++)
                    {
                        if (tempType == mPrivateGlobals.teProductEnum.eDiesel | tempType == mPrivateGlobals.teProductEnum.eGasoline | tempType == mPrivateGlobals.teProductEnum.ePropane | tempType == mPrivateGlobals.teProductEnum.emarkedGas | tempType == mPrivateGlobals.teProductEnum.emarkedDiesel)
                        {
                            if (pepurchaseList.Item(tempI).OverrideRequired) //   need to set only for required items
                            {
                                pepurchaseList.Item(tempI).SetOverride(short.Parse(code), documentNo, documentDetail);
                            }
                            if (!(tempI + 1 > pepurchaseList.Count()))
                            {
                                pepurchaseList.Item((short)(tempI + 1)).GetProductType(ref tempType);
                            }
                        }
                        else
                        {
                            if (!(tempI + 1 > pepurchaseList.Count()))
                            {
                                pepurchaseList.Item((short)(tempI + 1)).GetProductType(ref tempType);
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method to load override limit details
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Override limit repsonse</returns>
        public OverrideLimitResponse LoadOverrideLimitDetails(int saleNumber, int tillNumber, string userCode,string treatyNumber, string treatyName, out ErrorMessage error)
        {
            error = new ErrorMessage();
            double limitMax;
            OverrideLimitResponse result = new OverrideLimitResponse();
            if (!_policyManager.USE_OVERRIDE || _policyManager.TE_Type != "SITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select SITE Tax Exempt and enable Use override policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return result;
            }
            if (_policyManager.USE_OVERRIDE && _policyManager.TE_Type == "SITE")
            {
                var overPurchaseList = CacheManager.GetPurchaseListSaleForTill(tillNumber, saleNumber);

                if (overPurchaseList == null)
                {
                    overPurchaseList = _purchaseListManager.GetPurchaseList(saleNumber,tillNumber,userCode, treatyNumber,treatyName , out error);
                }
               
                if (overPurchaseList == null)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = "The request is invalid.",
                            MessageType = 0
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }

                _isOverrideRequiredTm = false;
                _isOverrideRequiredT = false;
                _isOverrideRequiredF = false;
                

                //   for new DLL design only document number is required for override
                if (_policyManager.SITE_RTVAL)
                {
                    result.IsRtvpValidationEnabled = true;
                }
                var petreatyNo = overPurchaseList.GetTreatyNo();

                //Nancy added TreatyNo display into PurchaseList frame
                result.Caption = "Purchase List" + " for Treaty Number " + petreatyNo.TreatyNumber;

                _isOverMax = false;
                _isOverThreshold = false;
                Overridable();

                _okOverrideF = false;
                _okOverrideT = false;
                _okOverrideTm = false;

                double limitThreshold;
                result.PurchaseItems = LoadPurchaseItems(tillNumber, saleNumber, ref overPurchaseList, out limitThreshold, out limitMax);

                result.OverrideCodes = LoadOverrideCodes(ref overPurchaseList, result.PurchaseItems, limitThreshold, limitMax, out error);
                
            }
            return result;
        }

        /// <summary>
        /// Method to load purchase items
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="pepurchaseList">Purchase list</param>
        /// <param name="limitThreshold">Threshold limit</param>
        /// <param name="limitMax">Maximum limit</param>
        /// <returns>List of purchase item response</returns>
        public List<PurchaseItemResponse> LoadPurchaseItems(int tillNumber, int saleNumber, ref tePurchaseList pepurchaseList, out double limitThreshold, out double limitMax)
        {
            
            
            var purchaseItems = new List<PurchaseItemResponse>();
            mPrivateGlobals.teProductEnum productType = default(mPrivateGlobals.teProductEnum);
            var petreatyNo = pepurchaseList.GetTreatyNo();
            double quota = 0;
            short index = 0;
            short runningQuota = 0; 

            bool isFirstGasIteration = false;
            object isFirstTobaccoIteration = null;

            limitMax = 0;
            limitThreshold = 0;

            isFirstGasIteration = true;

            isFirstTobaccoIteration = true;
            runningQuota = 0;
            _runningQuotaT = 0;
            _runningQuotaF = 0;

            //msfgQuota.Rows = 1;

            short indexRows = 0;

            indexRows = 1;
            short indexT = 0; 
            short indexF = 0; 
            double displayQuota = 0; //  
            if (_policyManager.SITE_RTVAL)
            {
                _runningQuotaF = (float)petreatyNo.RemainingFuelQuantity;
                _runningQuotaT = (short)petreatyNo.RemainingTobaccoQuantity;
                // end if  
            }
            //for all items in purchase list populate flex grid with products over the limit
            for (index = 1; index <= pepurchaseList.Count(); index++)
            {

                pepurchaseList.Item(index).GetProductType(ref productType);
                _peQuantity = pepurchaseList.Item(index).GetQuantity();

                _peUnitsPerPkg = pepurchaseList.Item(index).UnitsPerPkg;

                var quantity = _peQuantity * _peUnitsPerPkg;

                //   for real time validation we already know the remaiming qty
                if (!_policyManager.SITE_RTVAL)
                {
                    _treatyManager.GetQuota(ref petreatyNo, ref productType, ref quota);
                }

                
                // May 19, 2010, Nicolette added next if this is not needed for real time validation
                if (!_policyManager.SITE_RTVAL)
                {
                    if (productType == mPrivateGlobals.teProductEnum.eGasoline | productType == mPrivateGlobals.teProductEnum.eDiesel | productType == mPrivateGlobals.teProductEnum.ePropane | productType == mPrivateGlobals.teProductEnum.emarkedGas | productType == mPrivateGlobals.teProductEnum.emarkedDiesel)
                    {
                        if (isFirstGasIteration)
                        {
                            _runningQuotaF = (float)(_runningQuotaF + quota);
                            isFirstGasIteration = false;
                            //fuelIndex = index;
                        }
                    }
                    if (productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                    {

                        if ((bool)isFirstTobaccoIteration)
                        {
                            _runningQuotaT = (short)(_runningQuotaT + quota);

                            isFirstTobaccoIteration = false;
                            //tobaccoIndex = index;
                        }
                    }
                }

                double price = pepurchaseList.Item(index).GetTaxFreePrice();
                pepurchaseList.Item(index).GetProductKey(ref _peProductKey);
                var amount = price * _peQuantity;

                //   for real time validation to display the grid in a different format
                if (!_policyManager.SITE_RTVAL)
                {
                    GetLimitType(productType, ref _limitMaxType, ref _limitThresholdType);
                    _teSystemManager.TeGetLimit(_limitMaxType, ref limitMax);
                    _teSystemManager.TeGetLimit(_limitThresholdType, ref limitThreshold);

                    
                    if (productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                    {
                        indexT++;
                        _runningQuotaT = (short)(_runningQuotaT + quantity);
                        runningQuota = _runningQuotaT;
                        pepurchaseList.Item(index).CurrentQuota = runningQuota;
                        
                        if (_runningQuotaT > limitThreshold)
                        {
                            _isOverThreshold = true;
                            ShowOverride();
                            _isOverrideRequiredTm = true;
                            Overridable();
                            pepurchaseList.Item(index).OverrideRequired = true;
                            
                            
                            if (_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                            {
                                pepurchaseList.Item(index).OverrideRequired = true;
                                purchaseItems.Add(new PurchaseItemResponse
                                {
                                    ProductTypeId = productType,
                                    ProductId = _peProductKey,
                                    Quantity = _peQuantity.ToString("#0.00"),
                                    Price = price.ToString("#0.00"),
                                    Amount = amount.ToString("#0.00"),
                                    EquivalentQuantity = quantity.ToString("#0.00"),
                                    QuotaUsed = runningQuota.ToString(CultureInfo.InvariantCulture),
                                    QuotaLimit = limitThreshold.ToString(CultureInfo.InvariantCulture),
                                    DisplayQuota = (indexRows - 1).ToString()
                                });
                            }
                            else
                            {
                                pepurchaseList.Item(index).OverrideRequired = false;
                            }

                            indexRows++;
                            
                        }
                        else if (_runningQuotaT > limitMax)
                        {
                            _isOverMax = true;
                            _isOverThreshold = false;
                            ShowOverride();
                            _isOverrideRequiredT = true;
                            _isOverrideRequiredTm = false; 
                            Overridable();
                            
                            
                            if (_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                            {
                                pepurchaseList.Item(index).OverrideRequired = true;
                                purchaseItems.Add(new PurchaseItemResponse
                                {
                                    ProductTypeId = productType,
                                    ProductId = _peProductKey,
                                    Quantity = _peQuantity.ToString("#0.00"),
                                    Price = price.ToString("#0.00"),
                                    Amount = amount.ToString("#0.00"),
                                    EquivalentQuantity = quantity.ToString("#0.00"),
                                    QuotaUsed = runningQuota.ToString(CultureInfo.InvariantCulture),
                                    QuotaLimit = limitMax.ToString(CultureInfo.InvariantCulture),
                                    DisplayQuota = (indexRows - 1).ToString()
                                });
                            }
                            else
                            {
                                pepurchaseList.Item(index).OverrideRequired = false;

                            }
                            indexRows++;
                        }
                        else
                        {
                            pepurchaseList.Item(index).OverrideRequired = false;
                            Overridable();
                            _isOverrideRequiredT = false;
                            _isOverrideRequiredTm = false;
                            _isOverrideRequiredF = false;
                        }
                        // end if here and else clause to display grid for real time validation
                        if (_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                        {
                            _isOverrideRequiredT = false;
                            _isOverrideRequiredTm = false;
                        }

                        
                    }
                    else if (productType == mPrivateGlobals.teProductEnum.eGasoline | productType == mPrivateGlobals.teProductEnum.eDiesel | productType == mPrivateGlobals.teProductEnum.ePropane | productType == mPrivateGlobals.teProductEnum.emarkedGas | productType == mPrivateGlobals.teProductEnum.emarkedDiesel)
                    {
                        indexF++;
                        _runningQuotaFt = (float)(_runningQuotaFt + quantity);
                        _runningQuotaF = (float)(_runningQuotaF + quantity);
                        runningQuota = (short)_runningQuotaF;
                        pepurchaseList.Item(index).CurrentQuota = runningQuota;
                        
                        if (_runningQuotaF > limitThreshold)
                        {
                            _isOverThreshold = true;
                            ShowOverride();
                            _isOverrideRequiredF = true;
                            Overridable();
                            
                            
                            if (_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                            {
                                pepurchaseList.Item(index).OverrideRequired = true;
                                purchaseItems.Add(new PurchaseItemResponse
                                {
                                    ProductTypeId = productType,
                                    ProductId = _peProductKey,
                                    Quantity = _peQuantity.ToString("#0.00"),
                                    Price = price.ToString("#0.00"),
                                    Amount = amount.ToString("#0.00"),
                                    EquivalentQuantity = quantity.ToString("#0.00"),
                                    QuotaUsed = runningQuota.ToString(CultureInfo.InvariantCulture),
                                    QuotaLimit = limitThreshold.ToString(CultureInfo.InvariantCulture),
                                    DisplayQuota = (indexRows - 1).ToString(),
                                    FuelOverLimitText = "Fuel over Max limit is not allowed to be tax free"
                                });
                            }
                            else
                            {
                                pepurchaseList.Item(index).OverrideRequired = false;
                            }
                            indexRows++;
                            
                        }
                        else if (_runningQuotaFt > limitMax)
                        {
                            _isOverMax = true;
                            _isOverThreshold = false;
                            ShowOverride();
                            _isOverrideRequiredF = true;
                            //this.Show();
                            Overridable();
                            //Shiny end
                            
                            
                            if (_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                            {
                                pepurchaseList.Item(index).OverrideRequired = true;
                                //msfgQuota.AddItem(productType + "\t" + peProductKey + "\t" + peQuantity.ToString("#0.000") + "\t" + price.ToString("#0.000") + "\t" + Amount.ToString("#0.00") + "\t" + quantity.ToString("#0.000") + "\t" + runningQuota.ToString("#0.000") + "\t" + System.Convert.ToString(limitMax), indexRows - 1);
                                purchaseItems.Add(new PurchaseItemResponse
                                {
                                    ProductTypeId = productType,
                                    ProductId = _peProductKey,
                                    Quantity = _peQuantity.ToString("#0.00"),
                                    Price = price.ToString("#0.00"),
                                    Amount = amount.ToString("#0.00"),
                                    EquivalentQuantity = quantity.ToString("#0.00"),
                                    QuotaUsed = runningQuota.ToString(CultureInfo.InvariantCulture),
                                    QuotaLimit = limitMax.ToString(CultureInfo.InvariantCulture),
                                    DisplayQuota = (indexRows - 1).ToString()
                                });
                            }
                            else
                            {
                                pepurchaseList.Item(index).OverrideRequired = false;

                            }
                            indexRows++;
                        }
                        else
                        {
                            pepurchaseList.Item(index).OverrideRequired = false;
                            Overridable();
                            //Shiny end
                            _isOverrideRequiredT = false;
                            _isOverrideRequiredTm = false;
                            _isOverrideRequiredF = false;
                        }
                        if (!_teSystemManager.IsLimitRequired(ref mPrivateGlobals.theSystem, productType))
                        {
                            _isOverrideRequiredF = false;
                        }
                    }
                    //EnterCellHelper(index);
                }
                else // from real time validation
                {
                    // available information from real time validation are: RemainingTobaccoQuantity and RemainingFuelQuantity
                    // both are values AFTER this sale is done (quantities in this sale already have been deducted)
                    displayQuota = 0;
                    pepurchaseList.Item(index).OverrideRequired = false;
                    if (productType == mPrivateGlobals.teProductEnum.eGasoline | productType == mPrivateGlobals.teProductEnum.eDiesel | productType == mPrivateGlobals.teProductEnum.ePropane | productType == mPrivateGlobals.teProductEnum.emarkedGas | productType == mPrivateGlobals.teProductEnum.emarkedDiesel)
                    {
                        quota = petreatyNo.RemainingFuelQuantity;
                        _runningQuotaF = (float)(_runningQuotaF + quantity);
                        if (quota < 0 & _runningQuotaF < 0)
                        {
                            displayQuota = 0;
                            pepurchaseList.Item(index).OverrideRequired = true;
                        }
                        else if (quota < 0 && Math.Abs(quota) > Math.Abs(quantity))
                        {
                            pepurchaseList.Item(index).OverrideRequired = false;
                        }
                        else if (quota < 0)
                        {
                            displayQuota = _runningQuotaF;
                            pepurchaseList.Item(index).OverrideRequired = true;
                        }
                        if (pepurchaseList.Item(index).OverrideRequired)
                        {
                            //msfgQuota.AddItem(productType + "\t" + peProductKey + "\t" + peQuantity.ToString("#0.000") + "\t" + price.ToString("#0.000") + "\t" + Amount.ToString("#0.00") + "\t" + quantity.ToString("#0.000") + "\t" + quota.ToString("#0.000") + "\t" + (quota * pepurchaseList.Item(index).ItemEquivalence).ToString("#0.000") + "\t" + displayQuota.ToString("#0.000"));
                            purchaseItems.Add(new PurchaseItemResponse
                            {
                                ProductTypeId = productType,
                                ProductId = _peProductKey,
                                Quantity = _peQuantity.ToString("#0.00"),
                                Price = price.ToString("#0.00"),
                                Amount = amount.ToString("#0.00"),
                                EquivalentQuantity = quantity.ToString("#0.00"),
                                QuotaUsed = quota.ToString("#0"),
                                QuotaLimit = (quota * pepurchaseList.Item(index).ItemEquivalence).ToString("#0"),
                                DisplayQuota = displayQuota.ToString("#0")
                            });
                        }
                    }
                    else
                    {
                        quota = petreatyNo.RemainingTobaccoQuantity;
                        _runningQuotaT = (short)(_runningQuotaT + quantity);
                        if (quota < 0 & _runningQuotaT < 0)
                        {
                            displayQuota = 0;
                            pepurchaseList.Item(index).OverrideRequired = true;
                        }
                        else if (quota < 0 && Math.Abs(quota) > Math.Abs(quantity))
                        {
                            pepurchaseList.Item(index).OverrideRequired = false;
                        }
                        else if (quota < 0)
                        {
                            displayQuota = _runningQuotaT;
                            pepurchaseList.Item(index).OverrideRequired = true;
                        }
                        if (pepurchaseList.Item(index).OverrideRequired)
                        {
                            //msfgQuota.AddItem(productType + "\t" + peProductKey + "\t" + peQuantity.ToString("#0") + "\t" + price.ToString("#0.00") + "\t" + Amount.ToString("#0.00") + "\t" + quantity.ToString("#0") + "\t" + quota.ToString("#0") + "\t" + (quota * pepurchaseList.Item(index).ItemEquivalence).ToString("#0") + "\t" + displayQuota.ToString("#0"));
                            purchaseItems.Add(new PurchaseItemResponse
                            {
                                ProductTypeId = productType,
                                ProductId = _peProductKey,
                                Quantity = _peQuantity.ToString("#0.00"),
                                Price = price.ToString("#0.00"),
                                Amount = amount.ToString("#0.00"),
                                EquivalentQuantity = quantity.ToString("#0.00"),
                                QuotaUsed = quota.ToString("#0"),
                                QuotaLimit = (quota * pepurchaseList.Item(index).ItemEquivalence).ToString("#0"),
                                DisplayQuota = displayQuota.ToString("#0")
                            });
                        }
                    }
                    _isOverrideRequiredF = true;
                    _isOverrideRequiredT = true;
                    //cmdOverride.Visible = true;
                }
            }

            if (_policyManager.SITE_RTVAL)
            {
                Overridable();
            }
            else
            {
                if (!IsOverrideRequired())
                {
                    _isSuccessful = false;
                    //this.Close();
                }
                else
                {
                    _isSuccessful = true;
                }
            }
            return purchaseItems;
        }



        /// <summary>
        /// Method to check override
        /// </summary>
        /// <returns>True or false</returns>
        public bool CheckOverride()
        {
            return _isOverrideRequiredF || _isOverrideRequiredT ||
                _isOverrideRequiredTm;
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

            String returnValue = string.Empty;
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
                    limitMax = mPrivateGlobals.teLimitEnum.eGasTransactionLimit;
                    limitThreshold = mPrivateGlobals.teLimitEnum.eGasLimit;
                    return;
                case mPrivateGlobals.teProductEnum.eDiesel:
                case mPrivateGlobals.teProductEnum.emarkedDiesel:
                    limitMax = mPrivateGlobals.teLimitEnum.eDieselTransactionLimit;
                    limitThreshold = mPrivateGlobals.teLimitEnum.eDieselLimit;
                    return;
                case mPrivateGlobals.teProductEnum.ePropane:
                    limitMax = mPrivateGlobals.teLimitEnum.ePropaneTransactionLimit;
                    limitThreshold = mPrivateGlobals.teLimitEnum.ePropaneLimit;
                    return;
            }
            return;
        }

        /// <summary>
        /// Method to show override
        /// </summary>
        /// <returns>Message</returns>
        private string ShowOverride()
        {
            string returnValue = "";
            //MsgBox "Quota exceeded, Override required, please enter override information below", vbExclamation

            //Call LoadOverrideCombo
            //###Sept22,2008- Fixed Display Issue(shiny)
            
            
            Overridable();
            //Shiny End
            return returnValue;
        }

        /// <summary>
        /// Method to load override codes
        /// </summary>
        /// <param name="pepurchaseList">Purchase list</param>
        /// <param name="items">List of purchase items</param>
        /// <param name="limitThreshold">Threshold limit</param>
        /// <param name="limitMax">Maximum limit</param>
        /// <param name="error">Error</param>
        /// <returns>List of combo override limit</returns>
        private List<ComboOverrideCodes> LoadOverrideCodes(ref tePurchaseList pepurchaseList,
            List<PurchaseItemResponse> items, double limitThreshold, double limitMax, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var arrCodes = new List<OverrideCode>();
            var overrideCodes = new List<ComboOverrideCodes>();
            var prodType = default(mPrivateGlobals.teProductEnum);

            if (!_teSystemManager.TeGetAllOverrideCodes(ref mPrivateGlobals.theSystem, ref arrCodes))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Failed to get override codes",
                        MessageType = 0
                    }
                };
                return null;
            }

            short i = 0;

            foreach (PurchaseItemResponse purchaseItemResponse in items)
            {
                i++;
                var codes = new List<string>();
                if (_policyManager.SITE_RTVAL)
                {
                    // 2010
                    // display tobacco reason for tobacco and tobacco and fuel overlimit
                    // and fuel reason if only fuel items are overlimit
                    if (pepurchaseList.IsTobaccoOverLimit || pepurchaseList.IsOverLimit())
                    {
                        codes.AddRange(from overrideCode in arrCodes
                                       where overrideCode.Code[0].ToString() == "9"
                                       || overrideCode.Code[0].ToString() == "1"
                                       select overrideCode.Code + "," + overrideCode.Name);
                    }
                    else
                    {
                        codes.AddRange(from overrideCode in arrCodes
                                       where overrideCode.Code[0].ToString() == "2"
                                       select overrideCode.Code + "," + overrideCode.Name);
                    }
                }
                else
                {
                    if (purchaseItemResponse.ProductTypeId != 0)
                    {
                        prodType = purchaseItemResponse.ProductTypeId;
                    }

                    if (prodType == mPrivateGlobals.teProductEnum.eCigar | prodType == mPrivateGlobals.teProductEnum.eCigarette | prodType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                    {

                        if (pepurchaseList.Item(i).CurrentQuota > limitThreshold)
                        {
                            foreach (OverrideCode overrideCode in arrCodes)
                            {
                                if (bool.Parse(overrideCode.IsTobacco))
                                {
                                    if (overrideCode.Code[0].ToString() == "9")
                                    {
                                        codes.Add(overrideCode.Code + "," + overrideCode.Name);
                                    }
                                }
                            }
                        }
                        else if (pepurchaseList.Item(i).CurrentQuota > limitMax)
                        {
                            foreach (OverrideCode overrideCode in arrCodes)
                            {
                                if (bool.Parse(overrideCode.IsTobacco))
                                {
                                    if (overrideCode.Code[0].ToString() == "1")
                                    {
                                        codes.Add(overrideCode.Code + "," + overrideCode.Name);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (OverrideCode overrideCode in arrCodes)
                        {
                            if (!bool.Parse(overrideCode.IsTobacco))
                            {
                                if (overrideCode.Code[0].ToString() == "2")
                                {
                                    codes.Add(overrideCode.Code + "," + overrideCode.Name);
                                }
                            }
                        }
                    }
                }
                overrideCodes.Add(new ComboOverrideCodes
                {
                    RowId = (int)purchaseItemResponse.ProductTypeId,
                    Codes = codes
                });
            }

            return overrideCodes;
        }

        /// <summary>
        /// Set overridable or not
        /// </summary>
        private void Overridable()
        {

            if (_policyManager.SITE_RTVAL)
            {
                return; //  
            }

            //cboOverrideCodes.Visible = Enabled_Renamed;
            //lblOverrideCode.Visible = Enabled_Renamed;
            //cmdOverride.Visible = Enabled_Renamed;
            //lblDocumentNo.Visible = Enabled_Renamed;
            //txtDocumentNo.Visible = Enabled_Renamed;
            //lblDetails.Visible = Enabled_Renamed;
            //txtDetails.Visible = Enabled_Renamed;
            //fmeOverride.Visible = Enabled_Renamed;

        }

        /// <summary>
        /// Method to validate override
        /// </summary>
        /// <returns>True or false</returns>
        private bool ValidateOverride()
        {
            bool returnValue;
            
            

            if (_isOverrideRequiredT == false && _isOverrideRequiredTm == false && _isOverrideRequiredF == false)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _isOverrideRequiredTm && _isOverrideRequiredF && _okOverrideT && _okOverrideF && _okOverrideTm)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredF && _okOverrideF && !_isOverrideRequiredT && !_isOverrideRequiredTm)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _okOverrideT && !_isOverrideRequiredF && !_isOverrideRequiredTm)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredTm && _okOverrideTm && !_isOverrideRequiredF && !_isOverrideRequiredT)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _okOverrideT && _isOverrideRequiredF && _okOverrideF && !_isOverrideRequiredTm)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredTm && _okOverrideTm && _isOverrideRequiredF && _okOverrideF && !_isOverrideRequiredT)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _okOverrideT && _isOverrideRequiredTm && _okOverrideTm && !_isOverrideRequiredF)
            {

                returnValue = true;
                _isSuccessful = true;

            }
            else
            {
                returnValue = false;
                _isSuccessful = false;

            }

            return returnValue;
        }

        /// <summary>
        /// Method to verify if override is required or not
        /// </summary>
        /// <returns>True or false</returns>
        private bool IsOverrideRequired()
        {
            bool returnValue;
            

            if (_isOverrideRequiredT == false && _isOverrideRequiredTm == false && _isOverrideRequiredF == false)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _isOverrideRequiredTm && _isOverrideRequiredF && _okOverrideT && _okOverrideF && _okOverrideTm)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredF && _okOverrideF && !_isOverrideRequiredT && !_isOverrideRequiredTm)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _okOverrideT && !_isOverrideRequiredF && !_isOverrideRequiredTm)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredTm && _okOverrideTm && !_isOverrideRequiredF && !_isOverrideRequiredT)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _okOverrideT && _isOverrideRequiredF && _okOverrideF && !_isOverrideRequiredTm)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredTm && _okOverrideTm && _isOverrideRequiredF && _okOverrideF && !_isOverrideRequiredT)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else if (_isOverrideRequiredT && _okOverrideT && _isOverrideRequiredTm && _okOverrideTm && !_isOverrideRequiredF)
            {

                returnValue = false;
                _isSuccessful = true;

            }
            else
            {
                returnValue = true;
                _isSuccessful = false;

            }

            return returnValue;
        }

        /// <summary>
        /// Sale Summary
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        private Dictionary<string, string> SaleSummary(Sale sale)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Sale_Tax saleTaxRenamed;
            decimal curTotalTaxes = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (sale.Sale_Totals.Gross < 0)
            {
                result.Add(_resourceManager.GetResString(offSet, 266), sale.Sale_Totals.Net.ToString("###,##0.00"));
                //topLeftText = _resourceManager.GetResString(offSet,(short)266) + "\r\n"; //"Net Refund "
                //topRightText = sale.Sale_Totals.Net.ToString("###,##0.00") + "\r\n";

                foreach (Sale_Tax tempLoopVarSaleTaxRenamed in sale.Sale_Totals.Sale_Taxes)
                {
                    saleTaxRenamed = tempLoopVarSaleTaxRenamed;
                    if ((double)Math.Abs(saleTaxRenamed.Tax_Added_Amount) > 0.005)
                    {
                        //                mvarTopLeft.Caption = mvarTopLeft.Caption & Sale_Tax.Tax_Name & vbCrLf
                        //                mvarTopRight.Caption = mvarTopRight.Caption & Format(Sale_Tax.Tax_Added_Amount, "###,##0.00") & vbCrLf
                        curTotalTaxes = curTotalTaxes + saleTaxRenamed.Tax_Added_Amount;
                    }
                }

                result.Add(_resourceManager.GetResString(offSet, 137), curTotalTaxes.ToString("###,##0.00"));

                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)137) + "\r\n";
                //topRightText = topRightText + curTotalTaxes.ToString("###,##0.00") + "\r\n";

                if (sale.Sale_Totals.Charge != 0)
                {
                    result.Add(_resourceManager.GetResString(offSet, 138), sale.Sale_Totals.Charge.ToString("###,##0.00"));
                    //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)138) + "\r\n"; //"Charges "
                    //topRightText = topRightText + sale.Sale_Totals.Charge.ToString("###,##0.00") + "\r\n";
                }
                //topLeftText = topLeftText + "\r\n";
                //topRightText = topRightText + "________" + "\r\n";

                result.Add(_resourceManager.GetResString(offSet, 210), sale.Sale_Totals.Gross.ToString("###,##0.00"));
                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)210); //"Total"
                //topRightText = topRightText + sale.Sale_Totals.Gross.ToString("###,##0.00");
            }
            else
            {
                result.Add(_resourceManager.GetResString(offSet, 267), sale.Sale_Totals.Net.ToString("###,##0.00"));
                //topLeftText = _resourceManager.GetResString(offSet,(short)267) + " : " + "\r\n"; //Net Sale
                //topRightText = sale.Sale_Totals.Net.ToString("###,##0.00") + "\r\n";

                foreach (Sale_Tax tempLoopVarSaleTaxRenamed in sale.Sale_Totals.Sale_Taxes)
                {
                    saleTaxRenamed = tempLoopVarSaleTaxRenamed;
                    if ((double)Math.Abs(saleTaxRenamed.Tax_Added_Amount) > 0.005)
                    {
                        //                mvarTopLeft.Caption = mvarTopLeft.Caption & Sale_Tax.Tax_Name & vbCrLf
                        //                mvarTopRight.Caption = mvarTopRight.Caption & Format(Sale_Tax.Tax_Added_Amount, "###,##0.00") & vbCrLf
                        curTotalTaxes = curTotalTaxes + saleTaxRenamed.Tax_Added_Amount;
                    }
                }
                result.Add(_resourceManager.GetResString(offSet, 137), curTotalTaxes.ToString("###,##0.00"));
                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)137) + "\r\n";
                //topRightText = topRightText + curTotalTaxes.ToString("###,##0.00") + "\r\n";

                if (sale.Sale_Totals.Charge != 0)
                {
                    result.Add(_resourceManager.GetResString(offSet, 138), sale.Sale_Totals.Charge.ToString("###,##0.00"));

                    //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)138) + "\r\n"; //"Charges "
                    //topRightText = topRightText + sale.Sale_Totals.Charge.ToString("###,##0.00") + "\r\n";
                }
                //topLeftText = topLeftText + "\r\n";
                //topRightText = topRightText + "________" + "\r\n";
                result.Add(_resourceManager.GetResString(offSet, 210), sale.Sale_Totals.Gross.ToString("###,##0.00"));

                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)210); // "Total"
                //topRightText = topRightText + sale.Sale_Totals.Gross.ToString("###,##0.00");
            }

            return result;
        }

        #endregion
    }
}
