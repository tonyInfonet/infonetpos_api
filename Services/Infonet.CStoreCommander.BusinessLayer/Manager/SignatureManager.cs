using System.Net;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class SignatureManager : ManagerBase, ISignatureManager
    {
        private readonly ISignatureService _signatureService;
        private readonly IPolicyManager _policyManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="signatureService"></param>
        /// <param name="policyManager"></param>
        public SignatureManager(
            ISignatureService signatureService,
            IPolicyManager policyManager)
        {
            _signatureService = signatureService;
            _policyManager = policyManager;
        }


        /// <summary>
        /// Save Signature
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="imageFile">Image</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        public bool SaveSignature(int tillNumber, int saleNumber, string imageFile, out ErrorMessage error)
        {
            error = new ErrorMessage();
            if (!_policyManager.TE_SIGNATURE)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select Required to collect customer signature policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            return _signatureService.SaveSignature(tillNumber, saleNumber, imageFile);
        }
    }
}
