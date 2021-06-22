using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Mentoz.AspNet.Api
{
    public class ActionFilter : IAutofacActionFilter
    {
        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                var validationResults = new List<string>();
                var modelErrorCollection = actionContext.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors });
                foreach (var item in modelErrorCollection)
                    validationResults.AddRange(item.Errors.Select(x => string.IsNullOrEmpty(x.ErrorMessage) ? x.Exception.Message : x.ErrorMessage)); // item.Key
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonConvert.SerializeObject(validationResults), Encoding.UTF8, "application/json")
                };
                actionContext.Response.Content.Headers.Add("Access-Control-Expose-Headers", "X-Validation");
                actionContext.Response.Headers.Add("X-Validation", "Failure");
            }
            return Task.CompletedTask;
        }
    }
}