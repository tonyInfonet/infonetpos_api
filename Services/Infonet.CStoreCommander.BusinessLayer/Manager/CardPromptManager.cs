using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class CardPromptManager : ManagerBase, ICardPromptManager
    {
        private readonly ICardService _cardService;
        private readonly IPolicyManager _policyManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cardService"></param>
        /// <param name="policyManager"></param>
        public CardPromptManager(ICardService cardService, IPolicyManager policyManager)
        {
            _cardService = cardService;
            _policyManager = policyManager;
        }

        //  
        // This method is designed to load prompts for type "O" - optional data for fleet cards
        // type "F" - fuel prompts are loaded along with the profile in Loadprofile method (card profile class)
        /// <summary>
        /// Method to load prompts
        /// </summary>
        /// <param name="cardPrompts">Card prompts</param>
        /// <param name="cc">Credit card</param>
        /// <param name="profileId">Profile Id</param>
        public void Load_Prompts(ref CardPrompts cardPrompts, Credit_Card cc, string profileId)
        {
            //2013 11 08 - Reji - Wex Fleet Card Integration
            var profPromptLinkClause = " And 1=2 ";

            if (cc != null)
            {
                if (!string.IsNullOrEmpty(cc.Crd_Type))
                {
                    profPromptLinkClause = "";
                }


                if (cc.Crd_Type == "F" && cc.GiftType.ToUpper() == "W")
                {
                   // profPromptLinkClause = ProfPromptLinkList(cc, profileId);
                }
            }

            var prompts = _cardService.LoadCardPrompts(profileId);
            //2013 11 08 - Reji - Wex Fleet Card Integration - End

            foreach (var prompt in prompts)
            {
                cardPrompts.Add(prompt.MaxLength, prompt.MinLength, prompt.PromptMessage, prompt.PromptSeq, prompt.PromptID, "", prompt.PromptID.ToString());
            }

        }

        #region Private methods

        //   end



        //2013 11 08 - Reji - Wex Fleet Card Integration
        /// <summary>
        /// Method to get profile prompt ist
        /// </summary>
        /// <param name="cc">Credit card</param>
        /// <param name="profileId">Profile id</param>
        /// <returns>Profile prompt</returns>
        private string ProfPromptLinkList(Credit_Card cc, string profileId)
        {
            string returnValue;

            var promptCodeStr = cc.Track2.Replace(";" + cc.Cardnumber + "=", "").Replace("?", "");
            promptCodeStr = promptCodeStr.Substring(4, 1) + promptCodeStr.Substring(16, 1);

            if (!_policyManager.WEXEnabled)
            {
                returnValue = " And 1=2 ";
            }
            else if (promptCodeStr == "00")
            {
                returnValue = " And 1=2 ";
            }
            else if (string.IsNullOrEmpty(promptCodeStr))
            {
                returnValue = " AND A.PromptID=5009 ";
            }
            else
            {
                var cardPromptList = "0";
                //select [PromptID] from CardProfilePromptLink where CardPromptID='" & PromptCodeStr & "'

                cardPromptList = cardPromptList + _cardService.GetPromptIds(promptCodeStr, profileId);
                returnValue = " AND A.PromptID in (" + cardPromptList + ") ";
            }
            return returnValue;
        }

        #endregion
    }
}
