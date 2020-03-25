using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public static class CacheManager
    {
        private static MemoryCacher _memCacher;
        
        private static MemoryCacher  MemCacher
        {
            get { return _memCacher ?? new MemoryCacher(); }
            set { _memCacher = value; }
        }


        public static Sale GetCurrentSaleForTill(int tillNumber, int saleNumber)
        {
            var currentSale = MemCacher.GetValue($"CURRENTSALE_{tillNumber}_{saleNumber}");
            return currentSale as Sale;
        }

        public static void AddCurrentSaleForTill(int tillNumber, int saleNumber, Sale sale)
        {
            MemCacher.Delete($"CURRENTSALE_{tillNumber}_{saleNumber}");

            MemCacher.Add($"CURRENTSALE_{tillNumber}_{saleNumber}", sale, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static Credit_Card GetCreditCard(int tillNumber, int saleNumber)
        {
            var currentSale = MemCacher.GetValue($"CREDITCARD_{tillNumber}_{saleNumber}");
            return currentSale as Credit_Card;
        }

        public static void AddCreditCard(int tillNumber, int saleNumber, Credit_Card card)
        {
            MemCacher.Delete($"CREDITCARD_{tillNumber}_{saleNumber}");

            MemCacher.Add($"CREDITCARD_{tillNumber}_{saleNumber}", card, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void DeleteCurrentSaleForTill(int tillNumber, int saleNumber)
        {
            MemCacher.Delete($"CURRENTSALE_{tillNumber}_{saleNumber}");
        }

        public static User GetUser(string userCode)
        {
            var currentUser = MemCacher.GetValue($"USER_{userCode}");
            return currentUser as User;
        }

        public static void AddUser(string userCode, User user)
        {
            MemCacher.Delete($"USER_{userCode}");

            MemCacher.Add($"USER_{userCode}", user, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void AddStoreInfo(Store store)
        {
            MemCacher.Delete($"STORE");

            MemCacher.Add($"STORE", store, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static Store GetStoreInfo()
        {
            var store = MemCacher.GetValue($"STORE");
            return store as Store;
        }

        public static void DeleteStoreInfo()
        {
            MemCacher.Delete($"STORE");
        }

        public static void AddSecurityInfo(Security security)
        {
            MemCacher.Delete($"SECURITY");

            MemCacher.Add($"SECURITY", security, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static Security GetSecurityInfo()
        {
            var store = MemCacher.GetValue($"SECURITY");
            return store as Security;
        }


        public static void DeleteSecurityInfo()
        {
            MemCacher.Delete($"SECURITY");
        }

        public static void AddPromos(string promoId, Promos promos)
        {
            MemCacher.Delete($"PROMOS_{promoId}");

            MemCacher.Add($"PROMOS_{promoId}", promos, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static Promos GetPromosForPromoId(string promoId)
        {
            var promos = MemCacher.GetValue($"PROMOS_{promoId}");
            return promos as Promos;
        }

        public static void AddTenders(string transactionType, string reason, Tenders tenders)
        {
            MemCacher.Delete($"TENDERS_{transactionType}_{reason}");

            MemCacher.Add($"TENDERS_{transactionType}_{reason}", tenders, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static Tenders GetTenderForTransactionTypeAndReason(string transactionType, string reason)
        {
            var tenders = MemCacher.GetValue($"TENDERS_{transactionType}_{reason}");
            return tenders as Tenders;
        }

        public static void DeleteTendersForCashDrop(string transactionType, string reason)
        {
            MemCacher.Delete($"TENDERS_{transactionType}_{reason}");
        }

        public static void AddTendersForSale(int saleNumber, int tillNumber, Tenders tenders)
        {
            MemCacher.Delete($"TENDERS_{saleNumber}_{tillNumber}");

            MemCacher.Add($"TENDERS_{saleNumber}_{tillNumber}", tenders, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void AddTendersForArPay(int saleNumber, Tenders tenders)
        {
            MemCacher.Delete($"TENDERS_{saleNumber}");

            MemCacher.Add($"TENDERS_{saleNumber}", tenders, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void DeleteTendersForArPay(int saleNumber)
        {
            MemCacher.Delete($"TENDERS_{saleNumber}");
        }

        public static void AddTendersForPayment(int saleNumber, Tenders tenders)
        {
            MemCacher.Delete($"TENDERS_{saleNumber}");

            MemCacher.Add($"TENDERS_{saleNumber}", tenders, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void DeleteTendersForPayment(int saleNumber)
        {
            MemCacher.Delete($"TENDERS_{saleNumber}");
        }

        public static void DeleteTendersForSale(int saleNumber, int tillNumber)
        {
            MemCacher.Delete($"TENDERS_{saleNumber}_{tillNumber}");
        }

        public static Tenders GetTenderForSale(int saleNumber, int tillNumber)
        {
            var tenders = MemCacher.GetValue($"TENDERS_{saleNumber}_{tillNumber}");
            return tenders as Tenders;
        }

        public static Tenders GetTenderForArpAy(int saleNumber)
        {
            var tenders = MemCacher.GetValue($"TENDERS_{saleNumber}");
            return tenders as Tenders;
        }

        public static Tenders GetTenderForPayment(int saleNumber)
        {
            var tenders = MemCacher.GetValue($"TENDERS_{saleNumber}");
            return tenders as Tenders;
        }

        public static TaxExemptSale GetTaxExemptSaleForTill(int tillNumber, int saleNumber)
        {
            var taxExemptSale = MemCacher.GetValue($"TaxExemptSale_{tillNumber}_{saleNumber}");
            return taxExemptSale as TaxExemptSale;
        }

        public static void AddTaxExemptSaleForTill(int tillNumber, int saleNumber, TaxExemptSale sale)
        {
            MemCacher.Delete($"TaxExemptSale_{tillNumber}_{saleNumber}");

            MemCacher.Add($"TaxExemptSale_{tillNumber}_{saleNumber}", sale, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void RemoveTaxExemptSaleForTill(int tillNumber, int saleNumber)
        {
            MemCacher.Delete($"TaxExemptSale_{tillNumber}_{saleNumber}");
        }

        public static tePurchaseList GetPurchaseListSaleForTill(int tillNumber, int saleNumber)
        {
            var purchaseList = MemCacher.GetValue($"PURCHASELIST_{tillNumber}_{saleNumber}");
            if (purchaseList != null)
                return purchaseList as tePurchaseList;
            else
            {
                return Chaps_Main.oPurchaseList;
            }
            
        }
        
        public static void RemovePurchaseListSaleForTill(int tillNumber, int saleNumber)
        {
            MemCacher.Delete($"PURCHASELIST_{tillNumber}_{saleNumber}");
            Chaps_Main.oPurchaseList = null;
        }

        public static void AddPurchaseListSaleForTill(int tillNumber, int saleNumber, tePurchaseList purchaseList)
        {
            MemCacher.Delete($"PURCHASELIST_{tillNumber}_{saleNumber}");

            MemCacher.Add($"PURCHASELIST_{tillNumber}_{saleNumber}", purchaseList, DateTimeOffset.UtcNow.AddHours(1));

            Chaps_Main.oPurchaseList = purchaseList;
        }

        public static teSystem GetTeSystemForTill(int tillNumber, int saleNumber)
        {
            var teSystem = MemCacher.GetValue($"TESYSTEM_{tillNumber}_{saleNumber}");
            return teSystem as teSystem;
        }

        public static void AddTeSystemForTill(int tillNumber, int saleNumber, teSystem teSystem)
        {
            MemCacher.Delete($"TESYSTEM_{tillNumber}_{saleNumber}");

            MemCacher.Add($"TESYSTEM_{tillNumber}_{saleNumber}", teSystem, DateTimeOffset.UtcNow.AddHours(1));
        }


        public static void RemovePromotionalItems(string promoId)
        {
            MemCacher.Delete($"PROMOS_{promoId}");
        }

        public static void AddArPayment(int saleNumber, AR_Payment arPay)
        {
            MemCacher.Delete($"ARPayment_{saleNumber}");

            MemCacher.Add($"ARPayment_{saleNumber}", arPay, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void DeleteArPayment(int saleNumber)
        {
            MemCacher.Delete($"ARPayment_{saleNumber}");
        }

        public static AR_Payment GetArPayment(int saleNumber)
        {
            var arPay = MemCacher.GetValue($"ARPayment_{saleNumber}");
            return arPay as AR_Payment;
        }

        public static void AddFleetPayment(int saleNumber, Payment arPay)
        {
            MemCacher.Delete($"Payment_{saleNumber}");

            MemCacher.Add($"Payment_{saleNumber}", arPay, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void DeleteFleetPayment(int saleNumber)
        {
            MemCacher.Delete($"Payment_{saleNumber}");
        }

        public static Payment GetFleetPayment(int saleNumber)
        {
            var arPay = MemCacher.GetValue($"Payment_{saleNumber}");
            return arPay as Payment;
        }

        public static List<Policy> GetPoliciesForPos()
        {
            var policies = MemCacher.GetValue($"POLICIES");
            return policies as List<Policy>;
        }


        public static void AddPoliciesForPos(List<Policy> policies)
        {
            MemCacher.Delete($"POLICIES");

            MemCacher.Add($"POLICIES", policies, DateTimeOffset.UtcNow.AddHours(1));
        }


        public static List<BackOfficePolicy> GetAllPoliciesForPos()
        {
            var policies = MemCacher.GetValue($"ALL_POLICIES");
            return policies as List<BackOfficePolicy>;
        }


        public static void AddAllPoliciesForPos(List<BackOfficePolicy> policies)
        {
            MemCacher.Delete($"ALL_POLICIES");

            MemCacher.Add($"ALL_POLICIES", policies, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static List<PolicySet> GetPoliciesSetForPos()
        {
            var policies = MemCacher.GetValue($"POLICIESSET");
            return policies as List<PolicySet>;
        }


        public static void AddPoliciesSetForPos(List<PolicySet> policies)
        {
            MemCacher.Delete($"POLICIESSET");

            MemCacher.Add($"POLICIESSET", policies, DateTimeOffset.UtcNow.AddHours(1));
        }


        public static List<PolicyCanbe> GetPoliciesCanbeForPos()
        {
            var policies = MemCacher.GetValue($"POLICIESCANBE");
            return policies as List<PolicyCanbe>;
        }


        public static void AddPoliciesCanbeForPos(List<PolicyCanbe> policies)
        {
            MemCacher.Delete($"POLICIESCANBE");

            MemCacher.Add($"POLICIESCANBE", policies, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void AddDevice(Device device)
        {
            MemCacher.Delete($"DEVICE");

            MemCacher.Add($"DEVICE", device, DateTimeOffset.UtcNow.AddHours(1));

        }

        public static Device GetDevice()
        {
            var device = MemCacher.GetValue($"DEVICE");
            return device as Device;
        }

        public static void AddRegister(Register register, int posId)
        {
            MemCacher.Delete($"REGISTER_{posId}");

            MemCacher.Add($"REGISTER_{posId}", register, DateTimeOffset.UtcNow.AddHours(1));

        }

        public static Register GetRegister(int posId)
        {
            var device = MemCacher.GetValue($"REGISTER_{posId}");
            return device as Register;
        }

        public static void DeleteRegister(int posId)
        {
            MemCacher.Delete($"REGISTER_{posId}");

        }

        public static void AddVendorCoupons(VendorCoupons coupons)
        {
            MemCacher.Delete($"VENDORCOUPON");

            MemCacher.Add($"VENDORCOUPON", coupons, DateTimeOffset.UtcNow.AddHours(1));

        }

        public static VendorCoupons GetVendorCoupon()
        {
            var vendorCoupon = MemCacher.GetValue($"VENDORCOUPON");
            return vendorCoupon as VendorCoupons;
        }

        public static void AddStockBr(List<StockBr> stockBr)
        {
            MemCacher.Delete($"STOCKBR");
            MemCacher.Add($"STOCKBR", stockBr, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static List<StockBr> GetAllStockBr()
        {
            var stockBr = MemCacher.GetValue($"STOCKBR");
            return stockBr as List<StockBr>;
        }

        public static void AddStockInformation(Stock stock, string stockCode)
        {
            MemCacher.Delete($"STOCK_{stockCode}");

            MemCacher.Add($"STOCK_{stockCode}", stock, DateTimeOffset.UtcNow.AddHours(1));

        }

        public static Stock GetStockInformation(string code)
        {
            var saleLine = MemCacher.GetValue($"STOCK_{code}");
            return saleLine as Stock;
        }

        public static List<Promo> GetPromosForToday()
        {
            var promosForTodayt = MemCacher.GetValue($"PROMOSTODAY");
            return promosForTodayt as List<Promo>;
        }

        public static void AddPromosForToday(List<Promo> promos)
        {
            MemCacher.Delete($"PROMOSTODAY");

            MemCacher.Add($"PROMOSTODAY", promos, DateTimeOffset.UtcNow.AddHours(1));

        }

        public static void DeletePromosForToday()
        {
            MemCacher.Delete($"PROMOSTODAY");
        }


        public static SaleVendorCoupon GetSaleVendorCoupon(int saleNumber, string tenderCode)
        {
            var vendorCoupon = MemCacher.GetValue($"VENDORCOUPON_{saleNumber}_{tenderCode}");
            return vendorCoupon as SaleVendorCoupon;
        }

        public static void AddSaleVendorCoupon(int saleNumber, string tenderCode, SaleVendorCoupon vendorCoupon)
        {
            MemCacher.Delete($"VENDORCOUPON_{saleNumber}_{tenderCode}");

            MemCacher.Add($"VENDORCOUPON_{saleNumber}_{tenderCode}", vendorCoupon, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void DeleteSaleVendorCoupon(int saleNumber, string tenderCode)
        {
            MemCacher.Delete($"VENDORCOUPON_{saleNumber}_{tenderCode}");
        }

        public static List<PumpControl> GetAllPumps()
        {
            var pumps = MemCacher.GetValue($"PUMPSLIST");
            return pumps as List<PumpControl>;
        }

        public static void AddAllPumps(List<PumpControl> pumps)
        {
            MemCacher.Delete($"PUMPSLIST");

            MemCacher.Add($"PUMPSLIST", pumps, DateTimeOffset.UtcNow.AddHours(2));
        }

        public static recPump[] GetAllVariablePumps()
        {
            var pumps = MemCacher.GetValue($"VARIABLESPUMPSLIST");
            return pumps as recPump[];
        }


        public static void AddAllVariablePumps(recPump[] pumps)
        {
            MemCacher.Delete($"VARIABLESPUMPSLIST");

            MemCacher.Add($"VARIABLESPUMPSLIST", pumps, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static List<PropaneGrade> GetPropaneGrades()
        {
            var propaneGrades = MemCacher.GetValue($"PROPANEGRADES");
            return propaneGrades as List<PropaneGrade>;
        }

        public static void AddPropaneGrades(List<PropaneGrade> propaneGrades)
        {
            MemCacher.Delete($"PROPANEGRADES");

            MemCacher.Add($"PROPANEGRADES", propaneGrades, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static void DeletePropaneGrades()
        {
            MemCacher.Delete($"PROPANEGRADES");
        }

        public static void AddTillCloseModel(int tillNumber, CloseCurrentTillResponseModel model)
        {
            MemCacher.Delete($"TILLCLOSE_{tillNumber}");

            MemCacher.Add($"TILLCLOSE_{tillNumber}", model, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static CloseCurrentTillResponseModel GetTillCloseModel(int tillNumber)
        {
            var tillClose = MemCacher.GetValue($"TILLCLOSE_{tillNumber}");
            return tillClose as CloseCurrentTillResponseModel;
        }

        public static void DeleteTillCloseModel(int tillNumber)
        {
            MemCacher.Delete($"TILLCLOSE_{tillNumber}");

        }

        public static void AddTillCloseDataForTill(int tillNumber, Till_Close tillClose)
        {
            MemCacher.Delete($"CLOSETILL_{tillNumber}");

            MemCacher.Add($"CLOSETILL_{tillNumber}", tillClose, DateTimeOffset.UtcNow.AddHours(1));
        }

        public static Till_Close GetTillCloseData(int tillNumber)
        {
            var tillClose = MemCacher.GetValue($"CLOSETILL_{tillNumber}");
            return tillClose as Till_Close;
        }

        internal static void AddPoNumber(string pONumber, int saleNumber, int tillNumber)
        {
            MemCacher.Delete($"PONUMBER_{saleNumber}_{tillNumber}");

            MemCacher.Add($"PONUMBER_{saleNumber}_{tillNumber}", pONumber, DateTimeOffset.UtcNow.AddHours(1));
        }

        internal static string GetPoNumber(int saleNumber, int tillNumber)
        {
            var result = MemCacher.GetValue($"PONUMBER_{saleNumber}_{tillNumber}");
            return result as string;
        }

        public static string AmountEnterdForFleet { get; set; }
    }
}
