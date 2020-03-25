namespace Infonet.CStoreCommander.WebApi.Models.Till
{
    /// <summary>
    /// User model
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Shift number
        /// </summary>
        public int? ShiftNumber { get; set; }

        /// <summary>
        /// Shift date
        /// </summary>
        public string ShiftDate { get; set; }

        /// <summary>
        /// POS id
        /// </summary>
        public int PosId { get; set; }

        /// <summary>
        /// Float amount
        /// </summary>
        public decimal FloatAmount { get; set; }

        /// <summary>
        /// Unauthorized access
        /// </summary>
        public bool UnauthorizedAccess { get; set; }
    }
}