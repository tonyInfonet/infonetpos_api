using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
   public interface IMessageManager
    {
        /// <summary>
        /// Method to get list of all messages
        /// </summary>
        /// <returns></returns>
        List<MessageButton> GetMessages();


        /// <summary>
        /// Method to save a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        void SaveMessage(MessageButton message, int tillNumber, string userCode,
            out ErrorMessage error);
    }
}
