using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    sealed class modTPS
    {
        public static Credit_Card cc;
        
        public static string GetStrPosition(string str_Renamed, short Lo)
        {
            string returnValue = "";
            short i = 0;
            byte C;
            short j;
            string strT = "";
            short intTemp = 0;
            string strTemp = "";

            strT = str_Renamed;
            returnValue = "";
            i = (short)0;
            if (str_Renamed.Length > 0)
            {
                while (i < Lo)
                {
                    intTemp = (short)(strT.IndexOf(",") + 1);
                    if ((intTemp == 0) && (!string.IsNullOrEmpty(strT)))
                    {
                        strTemp = strT;
                        strT = "";
                    }
                    else
                    {
                        if (intTemp > 0) //  added to prevent from occurring runtime error
                        {
                            strTemp = strT.Substring(0, intTemp - 1);
                            strT = strT.Substring(intTemp + 1 - 1);
                        }
                    }
                    i++;
                }
                returnValue = strTemp;

            }
            return returnValue;
        }
        
    }
}
