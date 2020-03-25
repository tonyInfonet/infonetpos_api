namespace Infonet.CStoreCommander.Entities
{
    public class PropaneGrade
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Shortname { get; set; }

        public string StockCode { get; set; }
    }

    public class PropanePump
    {
        public int Id { get; set; }

        public string Name { get; set; }
       

        public int PositionId { get; set; }
    }
}
