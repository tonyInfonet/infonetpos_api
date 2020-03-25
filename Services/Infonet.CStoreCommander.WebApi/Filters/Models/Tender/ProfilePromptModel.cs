using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Profile prompt model
    /// </summary>
    public class ProfilePromptModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ProfilePromptModel()
        {
            Prompts = new List<Prompt>();
        }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Profile Id
        /// </summary>
        public string ProfileId { get; set; }

        /// <summary>
        /// Prompts
        /// </summary>
        public List<Prompt> Prompts { get; set; }
    }

    /// <summary>
    /// Prompt
    /// </summary>
    public class Prompt
    {
        /// <summary>
        /// Prompt message
        /// </summary>
        public string PromptMessage { get; set; }

        /// <summary>
        /// Prompt answer
        /// </summary>
        public string PromptAnswer { get; set; }
    }
}