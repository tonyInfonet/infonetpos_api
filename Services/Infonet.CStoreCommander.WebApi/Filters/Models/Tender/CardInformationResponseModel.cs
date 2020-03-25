using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Card swipe information model
    /// </summary>
    public class CardInformationResponseModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CardInformationResponseModel()
        {
            CardType = CardForm.None;
            PromptMessages = new List<string>();
        }

        /// <summary>
        /// Card type
        /// </summary>
        public CardForm CardType { get; set; }

        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }


        public string TenderClass { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Caption
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Ask pin
        /// </summary>
        public bool AskPin { get; set; }

        /// <summary>
        /// Pin
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// PO number message
        /// </summary>
        public string POMessage { get; set; }

        /// <summary>
        /// Prompt messages
        /// </summary>
        public List<string> PromptMessages { get; set; }

        /// <summary>
        /// Profile Id
        /// </summary>
        public string ProfileId { get; set; }
        
        /// <summary>
        /// IsAR Customer
        /// </summary>
        public bool IsArCustomer { get; set; }

        public List<MessageStyle> ProfileValidations { get; set; }

        public bool IsGasKing { get; set; }

        public double KickbackPoints { get; set; }
        public string KickBackValue { get; set; }
        public bool IsFleet { get; set; }
        public bool IsKickBackLinked { get; set; }

        public bool IsInvalidLoyaltyCard { get; set; }

    }
}