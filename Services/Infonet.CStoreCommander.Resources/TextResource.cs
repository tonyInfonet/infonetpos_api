using System;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Resources
{
    public class TextResource
    {
        //local variable(s) to hold property value(s)
        private string _mvarLanguage; //local copy
        private short _mvarOffSet; //local copy


        public short OffSet
        {
            get
            {
                //used when retrieving value of a property, on the right side of an assignment.
                var returnValue = _mvarOffSet;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                _mvarOffSet = value;
            }
        }



        public string Language
        {
            get
            {
                //used when retrieving value of a property, on the right side of an assignment.
                var returnValue = _mvarLanguage;
                return returnValue;
            }
            set
            {
                //used when assigning a value to the property, on the left side of an assignment.
                _mvarLanguage = value;
            }
        }
        
        public string Multiple_Lines(ref string msgstr)
        {
            var msgLines = Strings.Split(msgstr, "_", -1, CompareMethod.Text);
            msgstr = Convert.ToString(msgLines[0]);
            var i = (short)1;
            while (i <= (msgLines.Length - 1))
            {
                msgstr = msgstr + "\r\n" + Convert.ToString(msgLines[i]);
                i++;
            }
            var returnValue = msgstr;
            return returnValue;
        }

        public string Insert_Variables(ref string msgstr, object value)
        {
            string[] msgLines;
            short i;
            if ((Information.VarType(value) == (int)VariantType.Array + VariantType.Object) || (Information.VarType(value) == (int)VariantType.Array + VariantType.String)) // vbArray Then
            {
                var values = value as object[];
                msgLines = Strings.Split(msgstr, "{}", -1, CompareMethod.Text);
                msgstr = Convert.ToString(msgLines[0]);
                i = 1;
                while (i <= (msgLines.Length - 1))
                {
                    msgstr = msgstr + Convert.ToString(values[i]) + Convert.ToString(msgLines[i]);
                    i++;
                }
            }
            else
            {
                msgLines = Strings.Split(msgstr, "{}", -1, CompareMethod.Text);
                
                
                i = 1;
                msgstr = Convert.ToString(msgLines[0]);
                while (i <= (msgLines.Length - 1))
                {
                    msgstr = msgstr + Convert.ToString(value) + Convert.ToString(msgLines[i]);
                    i++;
                }
                
            }
            var returnValue = msgstr;
            return returnValue;
        }
               
    }
}
