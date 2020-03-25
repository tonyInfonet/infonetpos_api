namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IEncryptDecryptUtilityManager
    {
        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="strToDecrypt"></param>
        /// <returns></returns>
        string Decrypt(string strToDecrypt);

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <param name="strCardType"></param>
        /// <returns></returns>
        string Encrypt(string strToEncrypt, string strCardType);
    }
}
