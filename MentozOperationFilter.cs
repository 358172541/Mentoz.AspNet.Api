using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace Mentoz.AspNet.Api
{
    public class MentozOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var uploadAttribute = apiDescription.ActionDescriptor.GetCustomAttributes<UploadAttribute>().Any();
            if (uploadAttribute)
            {
                if (operation.parameters == null)
                    operation.parameters = new List<Parameter>(1);
                else
                    operation.parameters.Clear();
                operation.parameters.Add(new Parameter
                {
                    name = "File",
                    @in = "formData",
                    description = "Upload software package",
                    required = true,
                    type = "file"
                });
                operation.consumes.Add("application/form-data");
            }

            // operation.security.Add(new OpenApiSecurityRequirement { });
            //var authorize = apiDescription.ActionDescriptor.GetFilterPipeline().Select(x => x.Instance).Any(x => x is IAuthorizationFilter);
            //var allowAnonymous = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
            //if (authorize && allowAnonymous == false)
            //{
            //    /*
            //    operation.parameters = operation.parameters ?? new List<Parameter>();
            //    operation.parameters.Add(new Parameter
            //    {
            //        @in = "header",
            //        name = "Authorization",
            //        description = "",
            //        type = "string",
            //        required = true
            //    });
            //    */
            //    /*
            //    if (operation.security == null)
            //        operation.security = new List<IDictionary<string, IEnumerable<string>>>();
            //    var auth = new Dictionary<string, IEnumerable<string>>
            //    {
            //        {"basic", Enumerable.Empty<string>()}
            //    };
            //    operation.security.Add(auth);
            //    */
            //}
        }
    }
}