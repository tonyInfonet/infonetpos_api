using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ICardManager
    {
        /// <summary>
        /// Method ro build optiona data string
        /// </summary>
        /// <param name="objSa">Sale</param>
        /// <param name="card">Credit card</param>
        /// <param name="tendCard">Tend card</param>
        void Build_OptDataString(Sale objSa, ref Credit_Card card, ref TenderCard tendCard);


        /// <summary>
        /// Method to get request string
        /// </summary>
        /// <param name="cc">Credit card</param>
        /// <param name="sale">Sale</param>
        /// <param name="trnType">Transaction type</param>
        /// <param name="cardType">Card type</param>
        /// <param name="amount">Amount</param>
        /// <param name="authCode">Auth code</param>
        /// <returns>Request string</returns>
        string GetRequestString(ref Credit_Card cc, Sale sale, string trnType, string cardType, float amount,
            string authCode);

        /// <summary>
        /// Method to load profile by profile Id
        /// </summary>
        /// <param name="profileId">Profile Id</param>
        /// <returns>Card profile</returns>
        CardProfile Loadprofile(string profileId);

        /// <summary>
        /// Method to check whether valid profile time limit exists or not
        /// </summary>
        /// <param name="cardProfile">Card profile</param>
        /// <returns>True or false</returns>
        bool ValidProfileTimeLimit(ref CardProfile cardProfile);

        /// <summary>
        /// Method to check whether valid products are present for profile or not
        /// </summary>
        /// <param name="cardProfile">Card profile</param>
        /// <param name="cSale"></param>
        /// <returns>True or false</returns>
        bool ValidProductsForProfile(ref CardProfile cardProfile, Sale cSale);

        /// <summary>
        /// Method to find if valid transaction limits exists for profile or not
        /// </summary>
        /// <param name="cardProfile">Card profile</param>
        /// <param name="cardNumber">Card number</param>
        /// <returns>True or false</returns>
        bool ValidTransactionLimits(ref CardProfile cardProfile, string cardNumber);

        /// <summary>
        /// Method to get transaction amount limit
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="mode">Card number</param>
        /// <returns>Transaction amount</returns>
        double TransAmountLimit(string cardNumber, byte mode);
    }
}
