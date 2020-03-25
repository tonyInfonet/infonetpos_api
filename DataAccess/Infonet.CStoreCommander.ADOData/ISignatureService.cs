namespace Infonet.CStoreCommander.ADOData
{
    public interface ISignatureService
    {
        /// <summary>
        /// Save Signature
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="imageFilePath"></param>
        /// <returns></returns>
        bool SaveSignature(int tillNumber, int saleNumber, string imageFilePath);
    }
}