using Microsoft.Owin;
using System.Threading.Tasks;

namespace Mentoz.AspNet.Api
{
    public class MentozMiddleware : OwinMiddleware // https://odetocode.com/blogs/scott/archive/2013/11/11/writing-owin-middleware.aspx
    {
        public MentozMiddleware(OwinMiddleware next) : base(next) { }
        public override Task Invoke(IOwinContext context)
        {
            if (context.Request.Uri.AbsolutePath == "/")
                context.Response.Redirect("swagger/ui/index");
            return Next.Invoke(context);
        }
    }
}