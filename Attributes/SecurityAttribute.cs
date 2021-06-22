using Autofac;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Mentoz.AspNet.Api
{
    public class SecurityAttribute : AuthorizationFilterAttribute
    {
        public string[] Identities { get; }
        public SecurityAttribute(params string[] identities)
        {
            Identities = identities;
        }
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            await base.OnAuthorizationAsync(actionContext, cancellationToken);
            var currentUser = await CurrentUser(actionContext);
            if (currentUser == null) PermissionDenied(actionContext);
            if (currentUser.Type == UserType.MANAGER == false)
            {
                var currentUserRoleIds = await CurrentUserRoleIds(actionContext, currentUser.UserId);
                var currentUserRescIds = await CurrentUserRescIds(actionContext, currentUserRoleIds);
                var convertAttrRescIds = await ConvertAttrRescIds(actionContext);
                if (currentUserRescIds.Intersect(convertAttrRescIds).Any() == false) PermissionDenied(actionContext);
            }
        }
        private async Task<User> CurrentUser(HttpActionContext actionContext)
        {
            var identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
            var subject = identity.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub);
            var currentUserId = subject == null ? Guid.Empty : Guid.Parse(subject.Value);
            var scope = actionContext.Request.GetDependencyScope();
            var repository = scope.GetService(typeof(IUserRepository)) as IUserRepository;
            return await repository.FindAsync(currentUserId);
        }
        private async Task<List<Guid>> CurrentUserRoleIds(HttpActionContext actionContext, Guid currentUserId)
        {
            var scope = actionContext.Request.GetDependencyScope();
            var repository = scope.GetService(typeof(IUserRoleRepository)) as IUserRoleRepository;
            return await repository.Entities.AsNoTracking()
                .Where(x => x.UserId == currentUserId)
                .Select(x => x.RoleId)
                .ToListAsync();
        }
        private async Task<List<Guid>> CurrentUserRescIds(HttpActionContext actionContext, List<Guid> currentUserRoleIds)
        {
            var scope = actionContext.Request.GetDependencyScope();
            var repository = scope.GetService(typeof(IRoleRescRepository)) as IRoleRescRepository;
            return await repository.Entities.AsNoTracking()
                .Where(x => currentUserRoleIds.Contains(x.RoleId))
                .Select(x => x.RescId)
                .ToListAsync();
        }
        private async Task<List<Guid>> ConvertAttrRescIds(HttpActionContext actionContext)
        {
            var scope = actionContext.Request.GetDependencyScope();
            var repository = scope.GetService(typeof(IRescRepository)) as IRescRepository;
            return await repository.Entities.AsNoTracking()
                .Where(x => Identities.Contains(x.Identity))
                .Select(x => x.RescId)
                .ToListAsync();
        }
        private void PermissionDenied(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent("Permission denied.", Encoding.UTF8, "application/json")
            };
            actionContext.Response.Content.Headers.Add("Access-Control-Expose-Headers", "X-Permission");
            actionContext.Response.Headers.Add("X-Permission", "Denied");
        }
    }
}