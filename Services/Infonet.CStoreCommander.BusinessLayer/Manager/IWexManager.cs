using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IWexManager
    {
        /// <summary>
        /// Method to get the wex request string to be sent to the server at the time of sale
        /// </summary>
        /// <param name="strLineNum"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        string GetWexString(int strLineNum, float amount);

        /// <summary>
        /// method to validate the wex transaction
        /// </summary>
        /// <param name="strLineNum"></param>
        /// <param name="amount"></param>
        /// <param name="msgNum"></param>
        /// <returns></returns>
        bool ValidWexTransaction(int strLineNum, float amount, ref int msgNum);
        
        /// <summary>
        /// method to analyse the response from the WEX server 
        /// </summary>
        /// <param name="recievedString"></param>
        /// <param name="cc"></param>
        void AnalyseWexResponse(string recievedString, ref Credit_Card cc);

        /// <summary>
        /// method to get the profile prompts 
        /// </summary>
        /// <returns></returns>
        List<string> GetWexProfilePrompts();

        /// <summary>
        /// method to get wex reciept string 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="sale"></param>
        /// <param name="tenders"></param>
        /// <returns></returns>
        string GetWexRecieptString(ref short width, Sale sale, ref Tenders tenders);

        /// <summary>
        /// method to get the wex close batch string 
        /// </summary>
        /// <returns></returns>
        string GetWexCloseBatchString();

        /// <summary>
        /// method to get the cardprompts associated with the cards
        /// </summary>
        /// <param name="prompts"></param>
        /// <param name="cc"></param>
        /// <param name="ProfileID"></param>
        void GetProfilePrompts(ref CardPrompts prompts, Credit_Card cc, string ProfileID);
    }
}
