using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.BusinessLayer.Utilities
{
    public class Helper
    {
        
        /// <summary>
        /// Method to calculate penny adjustment
        /// </summary>
        /// <param name="grossAmount">Gross amount</param>
        /// <returns></returns>
        public static decimal Calculate_Penny_Adj(decimal grossAmount)
        {
            var pennyAdj = (decimal)(Round((double)(grossAmount - (Conversion.Int(grossAmount * 10) / 10)), 2));
            if ((pennyAdj == 0.01M) || (pennyAdj == 0.06M))
            {
                pennyAdj = -0.01M;
            }
            else if ((pennyAdj == 0.02M) || (pennyAdj == 0.07M))
            {
                pennyAdj = -0.02M;
            }
            else if ((pennyAdj == 0.03M) || (pennyAdj == 0.08M))
            {
                pennyAdj = 0.02M;
            }
            else if ((pennyAdj == 0.04M) || (pennyAdj == 0.09M))
            {
                pennyAdj = 0.01M;
            }
            else if ((pennyAdj == 0.05M) || (pennyAdj == 0.1M))
            {
                pennyAdj = 0;
            }
            else
            {
                pennyAdj = 0;
            }

            var returnValue = pennyAdj;

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double Round(double number, int digits = 0)
        {
            short i;
            var strFormat = "0";
            if (digits > 0)
            {
                strFormat = strFormat + ".";
            }
            for (i = 1; i <= digits; i++)
            {
                strFormat = strFormat + "0";
            }
            var returnValue = Math.Round(Conversion.Val(number.ToString(strFormat)), digits);

            return returnValue;
        }

        public static string GetKey(string keyStr)
        {
            short i;
            decimal secNum = 0;

            for (i = 1; i <= keyStr.Length; i++)
            {
                secNum = secNum + i * Strings.Asc(keyStr.Substring(i - 1, 1));
            }

            while (secNum >= 10000000 | secNum < 1000000)
            {
                if (secNum >= 10000000)
                {
                    secNum = (secNum / (decimal)Math.Log(10));
                }
                if (secNum < 1000000)
                {
                    secNum = (secNum * 3.14159265m);
                }
            }
            var secStr = secNum.ToString("0");
            var returnValue = secStr.Substring(5, 1) + secStr.Substring(2, 1) + secStr.Substring(4, 1) + secStr.Substring(1, 1) + secStr.Substring(6, 1) + secStr.Substring(0, 1) + secStr.Substring(3, 1);
            return returnValue;
        }

        /// <summary>
        /// Returns the minimum between three values, used for mix and match promotions 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float MinThreeValues(float a, float b, float c)
        {
            float minValue;

            if (a < b)
            {
                if (b < c)
                {
                    minValue = a;
                }
                else
                {
                    minValue = a < c ? a : c;
                }
            }
            else
            {
                if (b < c)
                {
                    minValue = b;
                }
                else
                {
                    minValue = a < c ? a : c;
                }
            }

            var returnValue = minValue;
            return returnValue;
        }

        /// <summary>
        /// Returns the minimum value in an array
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static short Min_Array(short[] a)
        {
            short returnValue;

            short i;

            if (a.Length - 1 == 0)
            {
                returnValue = 0;
                return returnValue;
            }

            var minVal = a[1];
            for (i = 1; i <= a.Length - 1; i++)
            {
                if (minVal > a[i])
                {
                    minVal = a[i];
                }
            }

            returnValue = minVal;
            return returnValue;
        }

        /// <summary>
        /// Method to create bytes from stream
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static string CreateBytes(System.IO.FileStream fs)
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            var content = Convert.ToBase64String(bytes);
            return content;
        }

        /// <summary>
        /// Method to create base string from bytes 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string CreateBytes(byte[] bytes)
        {
            if (bytes!=null)
            {
                var content = Convert.ToBase64String(bytes);
                return content;
            }
            return "";            
        }

        //04/11/13 Reji Added for validating user inputs from Sql Injections
        public static string SqlQueryCheck(ref string UserInputString, ref string AvoidedValuesString)
        {
            AvoidedValuesString = AvoidedValuesString.ToUpper();
            AvoidedValuesString = AvoidedValuesString + "##$$@@";

            if (AvoidedValuesString.IndexOf("\'") + 1 == 0)
            {
                UserInputString = UserInputString.Replace("\'", "\"");
            }
            if (AvoidedValuesString.IndexOf("--") + 1 == 0)
            {
                UserInputString = UserInputString.Replace("--", "");
            }
            if (AvoidedValuesString.IndexOf("/*") + 1 == 0)
            {
                UserInputString = UserInputString.Replace("/*", "");
            }
            if (AvoidedValuesString.IndexOf("*/") + 1 == 0)
            {
                UserInputString = UserInputString.Replace("*/", "");
            }
            if (AvoidedValuesString.IndexOf(";") + 1 == 0)
            {
                UserInputString = UserInputString.Replace(";", "");
            }

            return UserInputString;
        }

       



            }

            //public static string GetLocalIPAddress()
            //{
            //    var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //    var ipAddress = Convert.ToString(ipHostInfo.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork));

            //    return ipAddress;
            //   //return "OPTIMUS-156";
            //}

        }//end class
//end namespace

