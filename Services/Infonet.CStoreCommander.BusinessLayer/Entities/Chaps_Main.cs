using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    sealed class Chaps_Main
    {
        public static ADODB.Connection dbTrans;
        public static ADODB.Connection dbTill;

        public static ADODB.Recordset Stock_Br;
        public static ADODB.Recordset Stock;
        public static ADODB.Recordset PLU;
        public static ADODB.Recordset rsComp;

        private static DbService _dbService;

        public static teTreatyNo oTreatyNo;
        public static tePurchaseList oPurchaseList;

        public static teCardholder oTeCardHolder;
        public static TaxExemptSale oTeSale;
        public static tePurchaseList overPurchaseList; 

        public static Promos Promos_Renamed; // July 09, 2008
        public static Store Store_Renamed;
        public static Sale SA;
        public static Tenders Tenders_Renamed;
        public static VendorCoupons VendorCoupons; 
        public static CardTypes CardTypes;
        public static Service Service; 
        public static IEncryptDecryptUtilityManager EncryptDecrypt_Renamed; 
        public static Store_Credit STORE_CREDIT_Renamed;
        public static Device Device_Renamed; 
        public static Register Register_Renamed;
        public static User User_Renamed;
        public static Till Till_Renamed;
        public static Payment Payment_Renamed;
        public static AR_Payment ARPay;
        public static CashDrop CashDrop;
        public static Return_Reason RR;
        public static BR_Payment BR_Payment;
        public static Reprint_Cards Reprint_Cards;
        public static Prepay Prepay_Renamed;
        public static Security Security;
        public static short H_Width;

        public static string TimeFormatHM;
        public static string TimeFormatHMS;

        public static string SC;
        public static string CL;

        public static string LoyCard;
        public static string LoyExpDate;
        public static bool LoyCrdSwiped;
        public static string Reprint_Type;
        public static string Last_Printed;
        public static string Transaction_Type;
        public static short MsgRespClick;
        public static bool MSGHandler;

        public static string Logs_Path;
        public static object Norm_Col;
        public static object Hi_Color;
        public static object Sel_Col;
        public static string KeyPadEntry; //
        public static string Sound_Path;
        public static string FuelDept;
        public static string POS_IP;
        public static int MAXGNO;
        public static bool boolFP_HO;

        public const string PayAtPumpTill = "100";
        public const string CASH_SALE_CLIENT = "Cash Sale";
        [DllImport("kernel32", EntryPoint = "DeleteFileA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int DeleteFile(string lpFileName);

        [DllImport("winmm", EntryPoint = "sndPlaySoundA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int sndPlaySound(string lpszSoundName, int uflags);

        [DllImport("User32", EntryPoint = "FindWindowA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int FindWindow(object lpClassName, object lpWindowName);
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetWindow(int hWnd, int wCmd);
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int OpenIcon(int hWnd);
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int SetForegroundWindow(int hWnd);
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int BringWindowToTop(int hWnd);
        public const short PreviousHandle = 3;

        //nancy add for change system date info
        private const int LOCALE_SSHORTDATE = 0x1F;
        private const int WM_SETTINGCHANGE = 0x1A;
        private const int HWND_BROADCAST = 0xFFFF;
        [DllImport("kernel32", EntryPoint = "SetLocaleInfoA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool SetLocaleInfo(int Locale, int LCType, string lpLCData);
        [DllImport("User32", EntryPoint = "PostMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int PostMessage(int hWnd, int wMsg, int wParam, int lParam);
        [DllImport("kernel32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int GetSystemDefaultLCID();


        private static IPolicyManager modPolicy;

        

        public const int MAX_ADAPTER_NAME_LENGTH = 256;
        public const int MAX_ADAPTER_DESCRIPTION_LENGTH = 128;
        public const int MAX_ADAPTER_ADDRESS_LENGTH = 8;
        public const int ERROR_SUCCESS = 0;

        public struct IP_ADDRESS_STRING
        {
            [VBFixedArray(15)]
            public byte[] IpAddr;

            public void Initialize()
            {
                IpAddr = new byte[16];
            }
        }

        public struct IP_MASK_STRING
        {
            [VBFixedArray(15)]
            public byte[] IpMask;

            public void Initialize()
            {
                IpMask = new byte[16];
            }
        }

        public struct IP_ADDR_STRING
        {
            public int dwNext;
            public IP_ADDRESS_STRING IpAddress;
            public IP_MASK_STRING IpMask;
            public int dwContext;

            public void Initialize()
            {
                IpAddress.Initialize();
                IpMask.Initialize();
            }
        }

        //Reji 10/07/2013 Adding SQL authentication
        private static bool SQL_AUTH;

        public static string FSGDCouponStr; // 2014 05 01 - Reji for Fuel Slale Grocery Coupon
        public static string FSGDCouponReceipt; // 2014 05 06 - Reji for Fuel Slale Grocery Coupon

        public struct IP_ADAPTER_INFO
        {
            public int dwNext;
            public int ComboIndex;
            [VBFixedArray(MAX_ADAPTER_NAME_LENGTH + 3)]
            public byte[] sAdapterName;
            [VBFixedArray(MAX_ADAPTER_DESCRIPTION_LENGTH + 3)]
            public byte[] sDescription;
            public int dwAddressLength;
            [VBFixedArray(MAX_ADAPTER_ADDRESS_LENGTH - 1)]
            public byte[] sIPAddress;
            public int dwIndex;
            public int uType;
            public int uDhcpEnabled;
            public int CurrentIpAddress;
            public IP_ADDR_STRING IpAddressList;
            public IP_ADDR_STRING GatewayList;
            public IP_ADDR_STRING DhcpServer;
            public int bHaveWins;
            public IP_ADDR_STRING PrimaryWinsServer;
            public IP_ADDR_STRING SecondaryWinsServer;
            public int LeaseObtained;
            public int LeaseExpires;

            public void Initialize()
            {
                sAdapterName = new byte[(MAX_ADAPTER_NAME_LENGTH + 3) + 1];
                sDescription = new byte[(MAX_ADAPTER_DESCRIPTION_LENGTH + 3) + 1];
                sIPAddress = new byte[(MAX_ADAPTER_ADDRESS_LENGTH - 1) + 1];
                IpAddressList.Initialize();
                GatewayList.Initialize();
                DhcpServer.Initialize();
                PrimaryWinsServer.Initialize();
                SecondaryWinsServer.Initialize();
            }
        }



        public static short IDLEMINUTES; 
        public static bool blChangeUser; 

        private static string Source;
        private static string Provider;
        private static short RegisterNum;
        public static string strFunctionTend; //###PTC - 
        public static short cashIndex; //###PTC - 
        public static string EziPrefix; //
        public static bool sellEzi; //
        public static bool EMVProcess; // May12, 2010
        
    }
}
