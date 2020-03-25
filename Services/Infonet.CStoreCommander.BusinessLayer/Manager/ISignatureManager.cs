using Infonet.CStoreCommander.BusinessLayer.Utilities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ISignatureManager
    {
        /// <summary>
        /// Save Signature
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="imageFile">Image</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        bool SaveSignature(int tillNumber, int saleNumber, string imageFile, out ErrorMessage error);
    }
}