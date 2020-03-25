
namespace Infonet.CStoreCommander.Entities
{
    public class CardProcess
    {
        public byte TILL_NUM { get; set; }
        public string FuelMeasure { get; set; }
        public string FuelServiceType { get; set; }
        public string CardProductCode { get; set; }
        public float Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal SaleTax { get; set; }
        public double Discount { get; set; }
        public bool Fuel { get; set; }
    }
}
