namespace Infonet.CStoreCommander.WebApi.Models.Login
{
    /// <summary>
    /// Login response model
    /// </summary>
    public class LoginResponseModel
    {
        /// <summary>
        /// Auth token
        /// </summary>
        public  string AuthToken { get; set; }

        /// <summary>
        /// Trainer caption
        /// </summary>
        public string TrainerCaption { get; set; }
    }
}