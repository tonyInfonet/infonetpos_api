namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Switch prepay model
    /// </summary>
    public class SwitchPrepayModel
    {
        /// <summary>
        /// Active pump
        /// </summary>
        public short ActivePump { get; set; }

        /// <summary>
        /// New pump Id
        /// </summary>
        public short NewPumpId { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }
    }

    /// <summary>
    /// Delete prepay model
    /// </summary>
    public class DeletePrepayModel
    {
        /// <summary>
        /// Active pump
        /// </summary>
        public short ActivePump { get; set; }

        /// <summary>
        /// Shift number
        /// </summary>
        public short ShiftNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Register number
        /// </summary>
        public byte RegisterNumber { get; set; }
    }



    /// <summary>
    /// Add prepay model
    /// </summary>
    public class AddPrepayModel
    {
        /// <summary>
        /// Active pump
        /// </summary>
        public short ActivePump { get; set; }

        /// <summary>
        /// Shift number
        /// </summary>
        public short ShiftNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Register number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Fuel grade
        /// </summary>
        public string FuelGrade { get; set; }

        /// <summary>
        /// Is cash amount
        /// </summary>
        public bool IsAmountCash { get; set; }
    }

    /// <summary>
    /// Fuel basket model
    /// </summary>
    public class FuelBasketModel
    {
        /// <summary>
        /// Active pump
        /// </summary>
        public short ActivePump { get; set; }

        /// <summary>
        /// Basket value
        /// </summary>
        public float BasketValue { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }

    }



    /// <summary>
    /// Propane Sale model
    /// </summary>
    public class PropaneSaleItemModel
    {

        /// <summary>
        /// Grade Id
        /// </summary>
        public int GradeId { get; set; }

        /// <summary>
        /// Pump Id
        /// </summary>
        public int PumpId { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Is AMount
        /// </summary>
        public bool IsAmount { get; set; }

        /// <summary>
        /// Propane Value
        /// </summary>
        public decimal PropaneValue { get; set; }
    }


    /// <summary>
    /// Propane Sale model
    /// </summary>
    public class PropaneVolumeModel
    {

        /// <summary>
        /// Grade Id
        /// </summary>
        public int GradeId { get; set; }

        /// <summary>
        /// Pump Id
        /// </summary>
        public int PumpId { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }


        /// <summary>
        /// Propane Value
        /// </summary>
        public decimal PropaneValue { get; set; }
    }
}