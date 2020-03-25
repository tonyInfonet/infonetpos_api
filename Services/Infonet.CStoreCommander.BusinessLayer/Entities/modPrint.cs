using System.Runtime.InteropServices;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    sealed class modPrint
    {
        [DllImport("LPTtest.dll", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern byte TestPrinterStatus();

        [DllImport("user32", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int SendMessage(int hWnd, int wMsg, int wParam, ref object lParam);
        [DllImport("kernel32", EntryPoint = "GetWindowsDirectoryA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetWindowsDirectory(string lpBuffer, int nSize);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetPrivateProfileString(string lpApplicationName, object lpKeyName, string lpDefault, string lpReturnedString, int nSize, string lpFileName);


        [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int WritePrivateProfileString(string lpApplicationName, object lpKeyName, object lpString, string lpFileName);
        
        public struct DOCINFO
        {
            public string pDocName;
            public string pOutputFile;
            public string pDatatype;
        }

        public const byte PRINT_WIDTH = 40;
        
    }
}
