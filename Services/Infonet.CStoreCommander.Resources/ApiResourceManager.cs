using Microsoft.VisualBasic;
using System;
using System.Resources;

namespace Infonet.CStoreCommander.Resources
{
    /// <summary>
    /// Resource manger 
    /// </summary>
    public class ApiResourceManager : IApiResourceManager
    {
        private readonly ResourceManager _resourceManager;
        private readonly TextResource _resource;
        public ApiResourceManager()
        {
            _resourceManager = new ResourceManager(typeof(Resources.Resources));
            _resource = new TextResource();
        }


        /// <summary>
        /// Get string by resource Id
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public string GetResString(short offSet,short resourceId)
        {
            string returnValue = string.Empty;
            try
            {
                returnValue = _resourceManager.GetString(Convert.ToString("str" + (resourceId + offSet)));
            }
            catch
            {
                // ignored
            }
            return returnValue;
        }

        /// <summary>
        /// Create error message by msg number
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="msgNumber"></param>
        /// <param name="value"></param>
        /// <param name="messageBox"></param>
        /// <returns></returns>
        public MessageStyle CreateMessage(short offSet, int tag, int msgNumber, object value,
            MessageType messageBox = MessageType.Information)
        {
            if (tag > 0)
            {
                msgNumber = msgNumber + (tag * 100);
            }
            string sMsgString = GetResString((short)msgNumber, offSet);
            //string sTitle = string.Empty;
            //if (Convert.ToBoolean(sMsgString.IndexOf("|", StringComparison.Ordinal) + 1))
            //{
            //    object msgLines = Strings.Split(sMsgString, "|", -1, CompareMethod.Text);
            //    sMsgString = Convert.ToString(msgLines);
            //}

            //var iMsgTitle = (short)(sMsgString.IndexOf("~", StringComparison.Ordinal) + 1);
            //if (iMsgTitle > 0)
            //{
            //    sTitle = sMsgString.Substring(sMsgString.Length - iMsgTitle);
            //    sMsgString = sMsgString.Substring(0, iMsgTitle - 1);
            //}
            //else
            //{
            //    sTitle = "Alert";
            //}
            //if (Convert.ToBoolean(sMsgString.IndexOf("_", StringComparison.Ordinal) + 1))
            //{
            //    sMsgString = _resource.Multiple_Lines(ref sMsgString);
            //}
            if (Convert.ToBoolean(sMsgString.IndexOf("{}", StringComparison.Ordinal) + 1))
            {
                sMsgString = _resource.Insert_Variables(ref sMsgString, value);
            }
            return new MessageStyle
            {
                Message = sMsgString,
                MessageType = messageBox

            };
        }


        /// <summary>
        /// Create Caption
        /// </summary>
        /// <param name="cTag"></param>
        /// <param name="fTag"></param>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public string CreateCaption(short offSet, short cTag, short fTag, object value, short position)
        {
            string strCaption = "";
            if (cTag < 0)
            {
                strCaption = GetResString(Math.Abs(cTag), offSet);
            }
            else if (cTag > 100)
            {
                strCaption = GetResString(cTag, offSet);
            }
            else if (fTag > 0)
            {
                strCaption = GetResString(Convert.ToInt16(fTag * 100 + cTag), offSet);
            }
            if (strCaption.Length > 0)
            {
                strCaption = GetStringFromList(strCaption, position);
                if (value != null)
                {
                    strCaption = _resource.Insert_Variables(ref strCaption, value);
                }
                var tooltip = (short)(strCaption.IndexOf("^", StringComparison.Ordinal) + 1);
                if (tooltip > 0)
                {
                    strCaption = strCaption.Substring(0, tooltip - 1);
                }
                if (Convert.ToBoolean(strCaption.IndexOf("_", StringComparison.Ordinal) + 1))
                {
                    strCaption = _resource.Multiple_Lines(ref strCaption);
                }
            }
            var returnValue = strCaption;
            return returnValue;
        }

        public MessageStyle DisplayMsgForm(short offSet,object strMessage, short msgType, object value, MessageType messageBox = MessageType.Information)
        {
            string sMsgString = "";
            string sTitle = "";
            object MsgLines = null;
            short iMsgTitle = 0;
            if (Information.TypeName(strMessage) == "String")
            {
                // 
                sMsgString = System.Convert.ToString(strMessage);
            }
            else
            {
                sMsgString = GetResString(Convert.ToInt16(strMessage),offSet);
            }

            if (System.Convert.ToBoolean(sMsgString.IndexOf("|") + 1))
            {
                MsgLines = Strings.Split(sMsgString, "|", -1, CompareMethod.Text);
                sMsgString = System.Convert.ToString(MsgLines);
            }

            iMsgTitle = (short)(sMsgString.IndexOf("~") + 1);
            if (iMsgTitle > 0)
            {
                sTitle = sMsgString.Substring(sMsgString.Length - (sMsgString.Length - iMsgTitle), sMsgString.Length - iMsgTitle);
                sMsgString = sMsgString.Substring(0, iMsgTitle - 1);
            }
            else
            {
                sTitle = "";
            }
            if (Convert.ToBoolean(sMsgString.IndexOf("_") + 1))
            {
                sMsgString = _resource.Multiple_Lines(ref sMsgString);
            }
            if (Convert.ToBoolean(sMsgString.IndexOf("{}") + 1))
            {
                sMsgString = _resource.Insert_Variables(ref sMsgString, value);
            }
            return new MessageStyle
            {
                Message = sMsgString,
                MessageType = messageBox

            };

            /* System.Windows.Forms.Form f = default(System.Windows.Forms.Form);

             foreach (System.Windows.Forms.Form tempLoopVar_f in Application.OpenForms)
             {
                 f = tempLoopVar_f;
                 if (f.Name == "frmMsgBox")
                 {
                     f.Close();
                     return returnValue;
                 }
             }

             if (Value == null)
             {
                 Value = 0;
             }
             //TODO : UI Intervention ,update the property of messsge box somvir_11

             //frmMsgBox.Default.PromptText = "";
             //frmMsgBox.Default.CaptionText = TextCaption; //  - instead of adding different message type we can add the text label
             //frmMsgBox.Default.CaptionText2 = TextCaption2; //Ackroo
             //frmMsgBox.Default.CaptionText3 = TextCaption3; //Ackroo
             //frmMsgBox.Default.CaptionText4 = TextCaption4; //  added for Ackroo
             //frmMsgBox.Default.MessageType = MsgType;

             //END :TODO UI Intervention,update the property of message box somvir_11
             */
            /*if (Information.TypeName(strMessage) == "String")
            {
                // 
                sMsgString = System.Convert.ToString(strMessage);
                if (System.Convert.ToBoolean(sMsgString.IndexOf("|") + 1))
                {
                    MsgLines = Strings.Split(sMsgString, "|", -1, CompareMethod.Text);
                    sMsgString = System.Convert.ToString(MsgLines);
                }

                iMsgTitle = (short)(sMsgString.IndexOf("~") + 1);
                if (iMsgTitle > 0)
                {
                    sTitle = sMsgString.Substring(sMsgString.Length - (sMsgString.Length - iMsgTitle), sMsgString.Length - iMsgTitle);
                    sMsgString = sMsgString.Substring(0, iMsgTitle - 1);
                }
                else
                {
                    sTitle = "";
                }
                if (Convert.ToBoolean(sMsgString.IndexOf("_") + 1))
                {
                    sMsgString = Resource.Multiple_Lines(ref sMsgString);
                }
                if (Convert.ToBoolean(sMsgString.IndexOf("{}") + 1))
                {
                    sMsgString = Resource.Insert_Variables(ref sMsgString, Value);
                }

                //TODO: UI intervention, update the prompt text somvir_12

                //frmMsgBox.Default.PromptText = sMsgString;
                //END :TODO UI intervention, update the prompt text somvir_12

                //frmMsgBox.PromptText =strMessage
                // 
            }
            else
            {*/
            //  sMsgString = GetResString(Convert.ToInt16(strMessage));
            /*  if (Convert.ToBoolean(sMsgString.IndexOf("|") + 1))
              {
                  MsgLines = Strings.Split(sMsgString, "|", -1, CompareMethod.Text);
                  sMsgString = Convert.ToString(MsgLines);
              }

              iMsgTitle = (short)(sMsgString.IndexOf("~") + 1);
              if (iMsgTitle > 0)
              {
                  sTitle = sMsgString.Substring(sMsgString.Length - (sMsgString.Length - iMsgTitle), sMsgString.Length - iMsgTitle);
                  sMsgString = sMsgString.Substring(0, iMsgTitle - 1);
              }
              else
              {
                  sTitle = "";
              }
              if (Convert.ToBoolean(sMsgString.IndexOf("_") + 1))
              {
                  sMsgString = Resource.Multiple_Lines(ref sMsgString);
              }
              if (Convert.ToBoolean(sMsgString.IndexOf("{}") + 1))
              {
                  sMsgString = Resource.Insert_Variables(ref sMsgString, Value);
              }
              frmMsgBox.Default.PromptText = sMsgString;
          }
          MsgRespClick = (short)0;

          frmMsgBox.Default.IconID = Icon;
          frmMsgBox.Default.Title = sTitle;
          frmMsgBox.Default.ButtonStyle = ButtonStyle; 

          
          
          
          FormLoadChild(frmMsgBox.Default);
          

          Handle = frmMsgBox.Default.Handle.ToInt32();

          while (Convert.ToBoolean(TIMSmsgbox.IsWindow(Handle)))
          {
              Variables.Sleep(100); 
              Application.DoEvents();
              TIMSmsgbox.SetWindowPos(Handle, TIMSmsgbox.HWND_TOP, 0, 0, 0, 0, System.Convert.ToInt32(TIMSmsgbox.FLAGS)); //03/26/03
          }

          returnValue = MsgRespClick;

          //return returnValue;
          */

        }


        /// <summary>
        /// Gets String from list
        /// </summary>
        /// <param name="msgstr"></param>
        /// <param name="capPos"></param>
        /// <returns></returns>
        private string GetStringFromList(string msgstr, short capPos)
        {
            string returnValue;
            if (Convert.ToBoolean(msgstr.IndexOf("|", StringComparison.Ordinal) + 1))
            {
                var msgLines = Strings.Split(msgstr, "|", -1, CompareMethod.Text);
                returnValue = Convert.ToString(msgLines[capPos - 1]);
            }
            else
            {
                returnValue = msgstr;
            }
            return returnValue;
        }
    }
}
