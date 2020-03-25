using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IWexService
    {
        /// <summary>
        /// method to get the profile id 
        /// </summary>
        /// <returns></returns>
        string GetWexProfileId();

        /// <summary>
        /// method to get the extraction code by profileId
        /// </summary>
        /// <param name="x"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        string[] GetExtractionCode(string x, string profileId);

        /// <summary>
        /// method to load the profile prompts
        /// </summary>
        /// <returns></returns>
        List<string> GetWexProfilePrompts();

        /// <summary>
        ///  method to load the profilr prompts associated with a card
        /// </summary>
        /// <param name="prompts"></param>
        /// <param name="promptString"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        void GetCardProfilePrompts(ref CardPrompts prompts, string promptString, string profileId);

    }
}
