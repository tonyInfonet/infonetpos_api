using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class ClientCard
    {
        public string CardNumber { get; set; }

        public string ClientCode { get; set; }

        public string CardName { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Pin { get; set; }

        public char CardStatus { get; set; }

        public decimal CreditLimiit { get; set; }

        public decimal Balance { get; set; }

        public bool AllowRedemption { get; set; }

        public string TaxExemptedCardNumber { get; set; }

        public string ProfileID { get; set; }

        public bool ClientArCustomer { get; set; }

    }
}
