using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ICardPromptManager
    {
        /// <summary>
        /// Method to load prompts
        /// </summary>
        /// <param name="cardPrompts">Card prompts</param>
        /// <param name="cc">Credit card</param>
        /// <param name="profileId">Profile Id</param>
        void Load_Prompts(ref CardPrompts cardPrompts, Credit_Card cc, string profileId);
    }
}
