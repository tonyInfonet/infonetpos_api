using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Resources;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Signature Controller
    /// </summary>
    [RoutePrefix("api/v1/signature")]
    [ApiAuthorization]
    public class SignatureController : ApiController
    {
        private readonly ISignatureManager _signatureManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="signatureManager"></param>
        public SignatureController(ISignatureManager signatureManager)
        {
            _signatureManager = signatureManager;
        }


        /// <summary>
        /// Post Signature Image
        /// </summary>
        /// <returns></returns>
        [Route("save")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SuccessReponse>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SaveSignature(int saleNumber, int tillNumber)
        {
            var httpRequest = HttpContext.Current.Request;
            foreach (string file in httpRequest.Files)
            {
                var postedFile = httpRequest.Files[file];
                if (postedFile != null && postedFile.ContentLength > 0)
                {

                    const int maxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                    IList<string> allowedFileExtensions = new List<string> { ".jpg", ".png", ".bmp" };
                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!allowedFileExtensions.Contains(extension))
                    {

                        var message = Resource.UploadImageMessage;

                        return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                        {
                            Error = new MessageStyle { Message = message, MessageType = 0 }
                        });
                    }
                    if (postedFile.ContentLength > maxContentLength)
                    {

                        var message = Resource.ImageSizeMessage;

                        return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                        {
                            Error = new MessageStyle { Message = message, MessageType = 0 }
                        });
                    }
                    var directory = HttpContext.Current.Server.MapPath("~/Userimages");
                    Directory.CreateDirectory(directory);

                    var filePath = HttpContext.Current.Server.MapPath("~/Userimages/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    ErrorMessage error;
                    var success = _signatureManager.SaveSignature(tillNumber, saleNumber, filePath, out error);

                    if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                    {
                        var statusCode = error.StatusCode == 0 ? HttpStatusCode.BadRequest : error.StatusCode;
                        return Request.CreateResponse(
                            statusCode,
                            new ErrorResponse
                            {
                                Error = error.MessageStyle,
                            });
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse { Success = success });

                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
            {
                Error = new MessageStyle { Message = Resource.UploadImageError, MessageType = 0 }
            });

        }

    }
}
