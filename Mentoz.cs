using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;

[assembly: OwinStartup(typeof(Mentoz.AspNet.Api.Mentoz))]

namespace Mentoz.AspNet.Api
{
    public class Mentoz
    {
        public static string Audience = "https://api.mentoz.com";
        public static string Issuer = "https://api.mentoz.com";
        public static string Secret = "Wjagjbmui5ZBCC0nV6HMdTsEYjznXJGqhOVIRbH50P8=";
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var config = new HttpConfiguration();

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Clear();
            config.Formatters.Add(formatter); // is there a better way to set the JsonMediaTypeFormatter as default ?

            config.MapHttpAttributeRoutes();

            config.EnableSwagger(x =>
            {
                x.ApiKey("Bearer").Name("Authorization").In("header");
                x.SingleApiVersion("v1", "Mentoz.AspNet.Api Title");
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                x.OperationFilter<MentozOperationFilter>();
            }).EnableSwaggerUi(x =>
            {
                x.EnableApiKeySupport("Authorization", "header");
                x.DocExpansion(DocExpansion.List);
            }); // https://codingsight.com/swashbuckle-swagger-configuration-for-webapi/

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<MentozDbContext>().As<ITransaction>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                   .AsClosedTypesOf(typeof(IRepository<>))
                   .InstancePerLifetimeScope(); // https://stackoverflow.com/questions/18263852/how-to-register-services-and-types-that-are-in-separate-assemblies-in-autofac

            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterType<ActionFilter>().AsWebApiActionFilterForAllControllers().InstancePerLifetimeScope();
            builder.RegisterType<ExceptionFilter>().AsWebApiExceptionFilterForAllControllers().InstancePerLifetimeScope();

            builder.AddAutoMapper(Assembly.GetExecutingAssembly());

            config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());

            var physicalFileSystem = new PhysicalFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot"));
            var fileServerOptions = new FileServerOptions { RequestPath = PathString.Empty, EnableDefaultFiles = true, FileSystem = physicalFileSystem };
            fileServerOptions.StaticFileOptions.FileSystem = physicalFileSystem;
            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = false;
            app.UseFileServer(fileServerOptions); // web.config -> system.webServer
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Secret)),
                    RequireExpirationTime = true,
                    ValidAudience = Audience,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                }
            });
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}