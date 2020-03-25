using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class CardSummary
    {
        public CardSummary()
        {
            SelectedCard = CardForm.None;
            GiftCerts = new List<GiftCert>();
            StoreCredits = new List<Store_Credit>();
            PromptMessages = new List<string>();
            ValidationMessages = new List<MessageStyle>();
        }
        public CardForm SelectedCard { get; set; }

        public List<GiftCert> GiftCerts { get; set; }

        public List<Store_Credit> StoreCredits { get; set; }

        public string TenderCode { get; set; }

        public string TenderClass { get; set; }

        public string Amount { get; set; }

        public string CardNumber { get; set; }

        public bool AskPin { get; set; }

        public string Pin { get; set; }

        public bool AskVehicle { get; set; }

        public bool AskDriverNo { get; set; }

        public bool AskProductRestrictionCode { get; set; }

        public bool AskOdometer { get; set; }

        public bool AskIdentiifcationNumber { get; set; }

        public List<string> PromptMessages { get; set; }

        public string PONumber { get; set; }

        public string Caption { get; set; }

        public string ProfileId { get; set; }

        public bool IsArCustomer { get; set; }

        public List<MessageStyle> ValidationMessages { get; set; }

        public bool IsGasKing { get; set; }

        public double KickbackPoints { get; set; }
        public string KickBackValue { get; set; }

        public bool IsFleet { get; set; }

        public bool IsKickBackLinked { get; set; }

    }

    public enum CardForm
    {
        None,
        Givex,
        Fleet,        
        Credit,
        Debit,
        Account
    }

}
