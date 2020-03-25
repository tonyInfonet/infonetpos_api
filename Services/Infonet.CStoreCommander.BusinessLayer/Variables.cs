using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;

namespace Infonet.CStoreCommander.BusinessLayer
{
    public sealed class Variables
    {

        public static short RecsAffected;
        public static bool[] pumpPumping = new bool[33];
        public static bool[] pumpFinished = new bool[33];
        public static string statusString;
        public static string oldStatusString;
        public static string[] gGradeDescription = new string[10];
        public static short[,] gPumpPositionGrade = new short[33, 8];
        public static string UnitMeasurement = string.Empty; // Nicolette changed from 1 to 3 to use FUEL_UM policy
        public static byte iPumpCount;
        public static bool closeMDI;
        public static bool closeMsgBox;
        public static byte[] PumpStatus = new byte[33];
        public const short MAXID = 255;
        public static bool DisplayFueling;
        public static TCPAgent TCPAgent;
        public static bool LockUdp;
        public static DateTime LockUdpDate;


        public static string ReadUDP_strOldPrepayStatus = "";

        public static System.Net.Sockets.Socket GiveX_TCP;


        public static byte activePump;
        public static object PrepAmount;
        public const float ButDel_MAX = 0.5F;
        public const short FrmChildWidth = 11820;
        public const short FrmChildHeight = 4320;
        public static string UDPonLock;
        public static bool ReadTotalizerSuccess;
        public static bool ReadTotalizerInTillClose;
        public static bool ReadTankDipInTillClose;
        public static bool ReadTankDipSuccess;
        public static float basketClick;
        public static byte basket_click_delay;

        public static bool IsChildForm;
        public static bool IsModalForm;
        [DllImport("kernel32", EntryPoint = "CopyFileA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int CopyFile(string lpExistingFileName, string lpNewFileName, int bFailIfExists);
        [DllImport("kernel32", EntryPoint = "DeleteFileA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int DeleteFile(string lpFileName);

        public const string ErrorFile = "ErrorLog.txt";

        public static bool IsWin98;

        public static PriceType[] MyGradeIncre;
        public static PriceType[,] MyTLDiff = new PriceType[3, 3];
        public static bool[] DisplayValue; // display sale value for this pump or not

        public static string STFDNumber; // Store Forward Number
        public static string STFDNumber2; // ACKROO
        public static bool blChargeAcct;

        public static string strVoidInvoiceNumber; //invoice number for voiding

        public static byte DisplayPumpID;
        public static bool POSOnly; //Binal for POS Only
        public static bool[] Cashier_Auth = new bool[33];

        public const short sCalling = 1;


        //Pump

        public static recPump[] Pump = new recPump[33];
        public static SoundType PumpErrorSound;
        public struct pSound
        {

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string calling; //ID #1

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string pumpAuthorized; //ID#2

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string stopped; //ID #3

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string driveoff; //ID #4

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string finished; //ID #5

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string payatPumpStarted; //ID #6

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string payatPumpDone; //ID #7

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string pumpError; //ID #8

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string Help; //ID #9
        }
        public static pSound pumpSound;
        public struct dSound
        {
            ///   stockFound As String * 10 'ID 1
            ///   stocknotFound As String * 10 'ID 2

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string drawerOpened; //ID 1 was 3

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string PrintDone; //ID 2  was 4
        }
        public static dSound DevSound;
        public struct sSound
        {

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string ItemDeleted; //ID 1

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string Eror; //ID 2
                                // Nicolette moved here from pSound

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string stockFound; //ID 3

            [VBFixedString(10), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public string stocknotFound; //ID 3
                                         // Nicolette end
        }
        public static sSound SysSound;
        public struct recBasket
        {
            public float VolumeCurrent;
            public float AmountCurrent;
            public float VolumeStack;
            public float AmountStack;
            public bool CurrentFilled;
            public bool StackFilled;
            public float UPCurrent;
            public float UPStack;

            [VBFixedString(3), MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public string DescCurrent;

            [VBFixedString(3), MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public string DescStack;
            public short gradeIDCurr;
            public short gradeIDStack;
            public short PosIDCurr;
            public short posIDStack;
            public short currMOP;
            public short stackMOP;

            [VBFixedString(16), MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public string Stock_Code_Cur;

            [VBFixedString(3), MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public string currBaskID;

            [VBFixedString(3), MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public string stackBaskID;
        }
        public static recBasket[] gBasket = new recBasket[33];

        public static string[] StringData = new string[2101];
        public struct OperatorInfo
        {
            public int PumpStatus;
            public int Hose;
            public int Dollar;
            public int Volume;
        }

        public static OperatorInfo[] OperatorBlock = new OperatorInfo[33];
        public static byte gBasketID;
        public struct basket
        {

            [VBFixedString(3), MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public string BasketID;

            [VBFixedString(1), MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public string comma1;

            [VBFixedString(23), MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
            public string SaleString;

            [VBFixedString(1), MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public string comma2;
        }
        public static basket basket_Renamed;
        public static float[] PrepAmt = new float[33]; //Added to display amount for prepays
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int ShowWindow(int hWnd, int nCmdShow);
        //Public Declare Function FindWindow Lib "User32" Alias "FindWindowA" (ByVal lpClassName As Any, ByVal lpWindowName As Any) As Long

        [DllImport("kernel32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Sleep(int dwMilliseconds);
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int SetTimer(int hWnd, int nIDEvent, int uElapse, int lpTimerFunc);
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int KillTimer(int hWnd, int nIDEvent);

        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int LockWindowUpdate(int hwndLock);


        public static string[,] GC_Remove = new string[51, 3];
        public static string[,] SC_Remove = new string[51, 3];


        public static bool IsWaiting;


        /// Ashish 09/01/04 for override form

        //Public overPurchaseList As tePurchaseList ''Required for Override Form



        public static bool NeedToShowManualButton;
        public static bool ManualIsPressed;
        public static bool PrepayIsPressed;
        public static GetProperty gPumps;
        public static string MyPrepayBaskets;
        public static string SwitchPrepayBaskets; //   "CHANGE PREPAY GRADE"
        public static bool AlreadyInputDip;

        public struct GiveXResponseType
        {
            public string ServiceType;
            public string TransactionCode;
            public string Result;
            public string OperatorID;
            public string TerminalID;
            public string TransactionReference;
            public string TransactionReferenceFrom;
            public string PointsTransactionReference;
            public string PointsTransactionReferenceFrom;
            public string PreAuthReference;
            public string SecurityCode;
            public string Amount;
            public string AmountRedeemed;
            public string AuthorizedAmount;
            public string Units;
            public string Points;
            public string PointsAdded;
            public string PointsCancelled;
            public string CertificateBalance;
            public string PointsBalance;
            public string GivexNumber;
            public string ExpiryDate;
            public string TransmissionDate;
            public string TransmissionTime;
            public string CurrencyCode;
            public string MemberName;
            public string Services;
            public string OperatorLoginFlag;
            public string OperatorPasswordFlag;
            public string Continuation;
            public string CashoutId;
            public string ReportLines;
        }

        public struct GiveXRequestType
        {
            public string ServiceType;
            public string UserID;
            public string UserPassword;
            public string OperatorID;
            public string OperatorPassword;
            public string GivexNumberFrom; //GivexNumber of the Source(Used in BalanceTransfer Service only)
            public string ExpiryDateFrom; //ExpiryDate of the SourceGivexNumber (Used in BalanceTransfer Service only)
            public string TrackIIDataFrom; //TrackII of the SourceGivexNumber (Used in BalanceTransfer Service only)
            public string GivexNumber;
            public string ExpiryDate;
            public string TrackIIData;
            public string Language;
            public string Amount;
            public string TransactionCode;
            public string SecurityCode;
            public string TransmissionDate;
            public string TransmissionTime;
            public string TransactionReference;
            public string Units;
            public string PromoCode;
            public string Points;
            public string SerialNumber;
            public string ReportType;
            public string CashoutId;
            public string TerminalID;
            public string PreAuthReference;
        }
        public static GiveXRequestType GX_Request;


        public static GiveX GiveX_Renamed;
        //TODO: Milliplein_Removed
        //public static Milliplein Milliplein_Renamed; 
        public static string AccumulateCard;
        public static float AccumulateAmount;
        public static float MillExchangeAmount;
        public static string MillExchangeCard;
        public static float MillBalanceAfterExchange;


        public struct GiveXReceiptType
        {
            public string Date;
            public string Time;
            public string UserID;
            public short TranType;
            public int SaleNum;
            public string SeqNum;
            public string CardNum;
            public string ExpDate;
            public float Balance;
            public float PointBalance;
            public float SaleAmount;
            public string ResponseCode;
        }
        public static GiveXReceiptType GX_Receipt;

        //Receipt
        public struct AckReceiptType
        {
            public string Date_Renamed;
            public string Time;
            public string UserID;
            public short TranType;
            public int SaleNum;
            public string SeqNum;
            public string CardNum;
            public string ExpDate;
            public float ElBalance;
            public float GiftBalance;
            public float SaleAmount;
            public string ResponseCode;
        }
        public static AckReceiptType Ack_Receipt;


        public static int Return_SaleNo;
        public static bool Return_Mode;


        public struct typProduct
        {
            public string mProduct;
            public float mAmount;
            public float mPrice;
            public float mQty;
        }

        public static string strStockCode;

        public static bool NeedToRefreshPumpPrice;
        public static bool blCloseTill; // 'Need to set when closing the till, so we can only authorize the pump
        public const string CSTOOLS4_LICENSE_KEY = "FMKLPMFRIKHBMURI";
        public static RTVP.POSService.Transaction RTVPService;

    }
}
