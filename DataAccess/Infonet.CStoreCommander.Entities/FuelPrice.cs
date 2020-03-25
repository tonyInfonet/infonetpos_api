namespace Infonet.CStoreCommander.Entities
{
    public class FuelPrice
    {
        public short GradeId { get; set; }
        public short TierId { get; set; }
        public short LevelId { get; set; }

        public short Row { get; set; }
        public string Grade { get; set; }
        public string Tier { get; set; }
        public string Level { get; set; }
        public bool IsGradeEnabled { get; set; } = true;
        public string CashPrice { get; set; }
        public string CreditPrice { get; set; }
        public string TaxExemptedCashPrice { get; set; }
        public string TaxExemptedCreditPrice { get; set; }
    }
}
