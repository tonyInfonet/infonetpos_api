namespace Infonet.CStoreCommander.WebApi.Models.Till
{
    /// <summary>
    /// Login user model
    /// </summary>
    public class LoginUserModel
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
        /// POS id
        /// </summary>
        public int PosId { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }
    }
}