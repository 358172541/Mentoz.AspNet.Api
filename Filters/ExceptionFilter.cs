using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Mentoz.AspNet.Api
{
    public class ExceptionFilter : IAutofacExceptionFilter
    {
        public Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            HandleValidationException(actionExecutedContext);
            HandleTokenException(actionExecutedContext);
            HandleDbUpdateConcurrencyException(actionExecutedContext);
            return Task.CompletedTask;
        }
        private void HandleValidationException(HttpActionExecutedContext context)
        {
            if (context.Exception is ValidationException exception) // https://blog.csdn.net/maaici/article/details/104558351
            {
                context.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonConvert.SerializeObject(new string[] { exception.Message }), Encoding.UTF8, "application/json")
                };
                context.Response.Content.Headers.Add("Access-Control-Expose-Headers", "X-Validation");
                context.Response.Headers.Add("X-Validation", "Failed");
            }
        }
        private void HandleTokenException(HttpActionExecutedContext context)
        {
            if (context.Exception is TokenException exception)
            {
                context.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent(JsonConvert.SerializeObject(new string[] { exception.Message }), Encoding.UTF8, "application/json")
                };
                context.Response.Content.Headers.Add("Access-Control-Expose-Headers", "X-Token");
                context.Response.Headers.Add("X-Token", "Expired");
            }
        }
        private void HandleDbUpdateConcurrencyException(HttpActionExecutedContext context)
        {
            if (context.Exception is DbUpdateConcurrencyException exception)
            {
                context.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonConvert.SerializeObject(new string[] { exception.Message }), Encoding.UTF8, "application/json")
                };
                context.Response.Content.Headers.Add("Access-Control-Expose-Headers", "X-Version");
                context.Response.Headers.Add("X-Version", "Conflict");
            }
        }
    }
}