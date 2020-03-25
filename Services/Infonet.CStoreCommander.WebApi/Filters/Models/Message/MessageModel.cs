
namespace Infonet.CStoreCommander.WebApi.Models.Message
{
    /// <summary>
    /// Message model
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }
    }
}