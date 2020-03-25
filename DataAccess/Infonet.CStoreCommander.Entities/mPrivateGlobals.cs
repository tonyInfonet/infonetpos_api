// VBConversions Note: VB project level imports
//using AxccrpMonthcal6;
//using AxCCRPDTP6;
using Microsoft.VisualBasic;
using System;
// End of VB project level imports


namespace Infonet.CStoreCommander.Entities
{
    public sealed class mPrivateGlobals
    {
        // Programmer: Troy Chard                         Date: July 26,2004
        // Environment: Visual Basic 6.0                  File: mPrivateGlobals.bas
        // OS: Windows 2k                                 Ver: 1

        //All private variables global to this dll are stored here.


        //Initialized and terminated from within teSystem
        public static teSystem theSystem = new teSystem();

        //This is used to prevent infinite recursion while trying to init
        //theSystem var declared above.
        public static bool bInitializingInternalInstance;


        public const int kNoDaoInitNum = Constants.vbObjectError + 15000;
        public const string kNoDaoInitDesc = "Calling code forgot to call clsDaoDb.Init method.";


        public const int kDaoAlreadyInitNum = Constants.vbObjectError + 15001;
        public const string kDaoAlreadyInitDesc = "clsDaoDb.Init has already been called.";


        public const int kNoAdoInitNum = Constants.vbObjectError + 15002;
        public const string kNoAdoInitDesc = "Calling code forgot to call clsAdoDb.Init method.";

        public const int kAdoAlreadyInitNum = Constants.vbObjectError + 15003;
        public const string kAdoAlreadyInitDesc = "clsAdoDb.Init has already been called.";


        public const int kNoTaxExemptInitNum = Constants.vbObjectError + 15004;
        public const string kNoTaxExemptInitDesc = "Calling code forgot to call teInit function.";


        public const int kPurchaseListAlreadyInitNum = Constants.vbObjectError + 15005;
        public const string kPurchaseListAlreadyInitDesc = "Calling code forgot to call tePurchaseList.Init method.";


        public const int kNoQuotaForNoneCategoryNum = Constants.vbObjectError + 15006;
        public const string kNoQuotaForNoneCategoryDesc = "Tax exempt category of \'None\' does not have a quota (try cigarettes or gas etc).";


        public const int kNoQuotaForUnknownProductNum = Constants.vbObjectError + 15006;
        public const string kNoQuotaForUnknownProductDesc = "Tax exempt category of \'None\' does not have a quota (try cigarettes or gas etc).";

        //teProductEnum: all products that have tax free prices (and eNone for none).
        public enum teProductEnum
        {
            eNone = 0,
            eCigarette = 1,
            eLooseTobacco = 2,
            eCigar = 3,
            eGasoline = 4,
            eDiesel = 5,
            ePropane = 6,
            emarkedGas = 7, // 
            emarkedDiesel = 8 // 
        }


        //teLimitEnum: All of the different kinds of purchase limits for tax free items.
        public enum teLimitEnum
        {
            eCigLimit = 0,
            eTobaccoLimit = 1,
            eCigarLimit = 2,
            eCigMaxThreshhold = 3,
            eTobaccoMaxThreshhold = 4,
            eCigarMaxThreshhold = 5,
            eGasLimit = 6,
            eDieselLimit = 7,
            ePropaneLimit = 8,
            eGasTransactionLimit = 9,
            eDieselTransactionLimit = 10,
            ePropaneTransactionLimit = 11
        }


        //teTobaccoEnum: All of the different kinds of tobacco products
        public enum teTobaccoEnum
        {
            eTobCigarette = 1,
            eTobLooseTobacco = 2,
            eTobCigar = 3
        }

        public static short gintTRA_CLOSENO;
        public static DateTime gdateTRA_TRANSTIME;
        public static int glngTRA_AHRNo;
        public static int glngTRA_RegistryNo;
        public static int gintLastCloseNo;
        public static DateTime gdateCurrentTime;

        public static string gstrTRA_FILEINPATH;
        public static string gstrTRA_FILEOUTPATH;
    }
}
