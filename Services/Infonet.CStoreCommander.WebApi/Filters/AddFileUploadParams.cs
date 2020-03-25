using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace Infonet.CStoreCommander.WebApi.Filters
{
    /// <summary>
    /// Add File Upload Params
    /// </summary>
    public class AddFileUploadParams : IOperationFilter
    {
        /// <summary>
        /// Apply File Upload Param
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.operationId == "Signature_SaveSignature")  // controller and action name
            {
                operation.consumes.Add("multipart/form-data");
                operation.parameters?.Add(
                    new Parameter
                    {
                        name = "file",
                        @in = "formData",
                        required = true,
                        type = "file"
                    });
            }
        }
    }
}