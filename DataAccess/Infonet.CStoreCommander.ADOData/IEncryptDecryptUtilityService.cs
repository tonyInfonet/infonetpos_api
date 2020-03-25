namespace Infonet.CStoreCommander.ADOData
{
    public interface IEncryptDecryptUtilityService
    {
        /// <summary>
        /// Get Password
        /// </summary>
        /// <returns></returns>
        string GetPassword();

        /// <summary>
        /// Save Password
        /// </summary>
        /// <param name="strPassword"></param>
        void SavePassword(string strPassword);

        /// <summary>
        /// Method to initialise data
        /// </summary>
        /// <returns></returns>
        bool ClassInitialize();
    }
}
