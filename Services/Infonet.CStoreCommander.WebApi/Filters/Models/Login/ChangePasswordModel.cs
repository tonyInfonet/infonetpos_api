namespace Infonet.CStoreCommander.WebApi.Models.Login
{
    /// <summary>
    ///Change password model
    /// </summary>
    public class ChangePasswordModel
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
        /// Confirm password
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}