namespace Infonet.CStoreCommander.ADOData
{
    public interface IAiteCardHolderService
    {
        /// <summary>
        /// Method to affix bar code
        /// </summary>
        /// <param name="strBarcode">Bar code</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="validateMode">Validation mode</param>
        /// <returns>True or false</returns>
        bool AffixBarcode(string strBarcode, string cardNumber, byte validateMode);
    }
}