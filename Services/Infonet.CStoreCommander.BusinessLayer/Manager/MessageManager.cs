using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using log4net;
using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class MessageManager : ManagerBase, IMessageManager
    {
        private readonly IUtilityService _utilityService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ITillService _tillService;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="utilityService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="tillService"></param>
        public MessageManager(IUtilityService utilityService, IApiResourceManager resourceManager,
            ITillService tillService)
        {
            _utilityService = utilityService;
            _resourceManager = resourceManager;
            _tillService = tillService;
        }

        /// <summary>
        /// Method to get list of all messages
        /// </summary>
        /// <returns></returns>
        public List<MessageButton> GetMessages()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MessageManager,GetMessages,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            var messages = new List<MessageButton>
            {
                new MessageButton
                {
                    Index = 0,
                    Caption = _resourceManager.GetResString(offSet,1803),
                    Message = string.Empty
                }
            };

            messages.AddRange(_utilityService.GetAllMessageButtons());
            _performancelog.Debug($"End,MessageManager,GetMessages,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return messages;
        }

        /// <summary>
        /// Method to save a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        public void SaveMessage(MessageButton message, int tillNumber, string userCode,
            out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MessageManager,SaveMessage,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            error = new ErrorMessage();
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Till does not exists"
                    },
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return;
            }
            if (string.IsNullOrEmpty(message.Message))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Message cannot be empty",
                        MessageType = MessageType.OkOnly
                    },
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return;
            }
            if (message.Index != 0)
            {
                var originalMessage = _utilityService.GetMessageByButtonId(message.Index);
                if (originalMessage == null || originalMessage.Message != message.Message)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = "Request is invalid",
                            MessageType = MessageType.OkOnly
                        },
                        StatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return;
                }
                message.Caption = originalMessage.Caption;
            }
            else
            {
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                message.Caption = _resourceManager.GetResString(offSet,1803);
            }
            _utilityService.SaveMessageButton(message, till, userCode);
            _performancelog.Debug($"End,MessageManager,SaveMessage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }
    }
}
