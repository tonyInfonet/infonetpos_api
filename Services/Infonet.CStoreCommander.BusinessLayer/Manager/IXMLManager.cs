using Infonet.CStoreCommander.BusinessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IXMLManager
    {
        bool FormatGetPointStatusRequestXML(string strLoyaltyID, string strEntryMethod,ref XML xml);

        // bool FormatFinalizeRewardsRequestXML(string strLoyaltyID, string strEntryMethod, string strPointsRedeemed, string strFinalTransTotal, string strTrainingModeFlag, string strOutsideSalesFlag, string strChoiceName, string strChoiceValue, string strCashierID, string strTillID, string strEventStartDate, string strEventStartTime, string strEventEndDate, string strEventEndTime);

         bool Initialize(string strPosLoyaltyInterfaceVersion, string strVendorName, string strVendorModelVersion, string strPOSSequenceID, string strStoreLocationID, string strLoyaltyOfflineFlag,ref XML xml);

        void AnalyseKickBackResponse(string ResponseXML,ref XML xml);
        bool InsertRequestHeader(ref XML xml);

        bool FormatFinalizeRewardsRequestXML(string strLoyaltyID, string strEntryMethod, string strPointsRedeemed, string strFinalTransTotal, string strTrainingModeFlag, string strOutsideSalesFlag, string strChoiceName, string strChoiceValue, string strCashierID, string strTillID, string strEventStartDate, string strEventStartTime, string strEventEndDate, string strEventEndTime,ref XML xml,ref Sale sale);

        bool AppendItemLineInfo(string strTransactionLineAttribute, string strItemLineAttribute, string strItemCode_POSCodeFormat, string strItemCode_POSCode, string strItemCode_POSCodeModifier, string strDepartment, string strSubDepartment, string strSubDetail, string strDescription, string strEntryMethod, string strActualSalesPrice, string strRegularSellPrice, string strSellingUnits, string strSalesQuantity, string strSalesAmount, string strPriceOverride, string strPromotion, string strDiscount, string strItemTax, string strLinkedFromLineNumber, ref XML xml);

         bool AppendFuelLineInfo(string strTransactionLineAttribute, string strFuelLineAttribute, string strFuelGradeID, string strFuelPositionID, string strPriceTierCode, string strTimeTierCode, string strServiceLevelCode, string strDescription, string strActualSalesPrice, string strRegularSellPrice, string strSalesQuantity, string strSalesAmount, string strSalesUOM, string strPromotion, string strDiscount, string strItemTax, string strPaymentSystemsProductCode, ref XML xml);
        bool AppendTenderInfo(string strTransactionLineAttribute, string strTenderCode, string strTenderSubCode, string strISOPrefix, string strLoyaltyRewardID, string strTenderAmount, string strChangeFlagAttribute,ref XML xml);



    }
}
