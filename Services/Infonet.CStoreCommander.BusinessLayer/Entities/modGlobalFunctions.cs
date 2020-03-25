using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    sealed class modGlobalFunctions
    {
        public struct OSVERSIONINFO
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;

            [VBFixedString(128), MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szCSDVersion;
        }
        
        //Type for Ezipin request to be send to Ezipin
        public struct Products
        {
            public string StockCode;
            public int cardno;
        }
        public static bool BoolFuelPriceApplied; 

        
        // 
        //To solve the issue without changing the function in everywhere we are using the same function and first formating and then rounding using VBA.Math.round. it will work
        //Whoever is using Round function they can use the same way as VB round function
        public static double Round(double number, int digits = 0)
        {
            double returnValue = 0;
            string strFormat = "";
            short i = 0;
            strFormat = "0";
            if (digits > 0)
            {
                strFormat = strFormat + ".";
            }
            for (i = 1; i <= digits; i++)
            {
                strFormat = strFormat + "0";
            }
            returnValue = Math.Round(Conversion.Val(number.ToString(strFormat)), digits);

            return returnValue;
        }

        public static decimal Calculate_Penny_Adj(decimal grossAmount)
        {
            decimal returnValue = 0;


            decimal Penny_Adj = new decimal();

            Penny_Adj = (decimal)(Round((double)(grossAmount - (Conversion.Int(grossAmount * 10) / 10)), 2));
            if ((Penny_Adj == 0.01M) || (Penny_Adj == 0.06M))
            {
                Penny_Adj = -0.01M;
            }
            else if ((Penny_Adj == 0.02M) || (Penny_Adj == 0.07M))
            {
                Penny_Adj = -0.02M;
            }
            else if ((Penny_Adj == 0.03M) || (Penny_Adj == 0.08M))
            {
                Penny_Adj = 0.02M;
            }
            else if ((Penny_Adj == 0.04M) || (Penny_Adj == 0.09M))
            {
                Penny_Adj = 0.01M;
            }
            else if ((Penny_Adj == 0.05M) || (Penny_Adj == 0.1M))
            {
                Penny_Adj = 0;
            }
            else
            {
                Penny_Adj = 0;
            }

            returnValue = Penny_Adj;

            return returnValue;
        }

        public static bool HasGivexSale(Sale mySale, ref string lineNumbers)
        {
            bool returnValue = false;
            string strTmp = "";
            strTmp = "";

            foreach (Sale_Line tempLoopVar_SL_Count in mySale.Sale_Lines)
            {
                var slCount = tempLoopVar_SL_Count;
                if (slCount.GiftType == "GiveX")
                {
                    returnValue = true;
                    if (slCount.Line_Num != 0)
                    {
                        strTmp = strTmp + Convert.ToString(slCount.Line_Num) + ",";
                    }
                }
            }

            if (strTmp.Length > 0 && strTmp.Substring(strTmp.Length - 1, 1) == ",")
            {
                strTmp = strTmp.Substring(0, strTmp.Length - 1);
            }
            lineNumbers = strTmp;
            return returnValue;
        }


    }
}
