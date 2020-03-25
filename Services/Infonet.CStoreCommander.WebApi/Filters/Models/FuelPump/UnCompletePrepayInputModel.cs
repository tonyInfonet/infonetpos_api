namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Uncomplete prepay
    /// </summary>
    public class UnCompletePrepayInputModel
    {
        public int PumpId { get; set; }
        public int SaleNum { get; set; }
        public int TillNumber { get; set; }
        public float FinishAmount { get; set; }
        public float FinishQty { get; set; }
        public float FinishPrice { get; set; }
        public float PrepayAmount { get; set; }
        public short PositionId { get; set; }
        public short GradeId { get; set; }
    }

    /// <summary>
    /// Delete Uncomplete prepay model
    /// </summary>
    public class DeleteUnCompletePrepayModel
    {
        public int PumpId { get; set; }
        public int SaleNum { get; set; }
        public int TillNumber { get; set; }        
    }
}