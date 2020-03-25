namespace Infonet.CStoreCommander.Entities
{
    public class BillCoin
    {
        public BillCoin()
        {
            Amount = string.Empty;
        }
        public string Description { get; set; }

        public string Value { get; set; }

        public string Amount { get; set; }

    }
}
