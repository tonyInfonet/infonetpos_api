namespace Infonet.CStoreCommander.Resources
{
    /// <summary>
    /// Resource manager interface
    /// </summary>
    public interface IApiResourceManager
    {
        /// <summary>
        /// Get string by resource id 
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        string GetResString(short offSet,short resourceId);

        /// <summary>
        /// Create error message by msgnumber
        /// </summary>
        /// <param name="offSet"></param>
        /// <param name="tag"></param>
        /// <param name="msgNumber"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        MessageStyle CreateMessage(short offSet,int tag, int msgNumber, object value, MessageType messageBox = MessageType.Information);

        /// <summary>
        /// Create Caption
        /// </summary>
        /// <param name="offSet"></param>
        /// <param name="cTag"></param>
        /// <param name="fTag"></param>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        string CreateCaption(short offSet,short cTag, short fTag, object value, short position);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offSet"></param>
        /// <param name="strMessage"></param>
        /// <param name="MsgType"></param>
        /// <param name="Value"></param>
        /// <param name="messageBox"></param>
        /// <returns></returns>
        MessageStyle DisplayMsgForm(short offSet,object strMessage, short MsgType, object Value, MessageType messageBox = MessageType.Information);
    }
}