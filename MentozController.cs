using Autofac;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace Mentoz.AspNet.Api
{
    public class UploadAttribute : Attribute { }

    [Authorize]
    public class MentozController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly ITransaction _transaction;
        private readonly IRescRepository _rescRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleRescRepository _roleRescRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        public MentozController(
            IMapper mapper,
            ITransaction transaction,
            IRescRepository rescRepository,
            IRoleRepository roleRepository,
            IRoleRescRepository roleRescRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository)
        {
            _mapper = mapper;
            _transaction = transaction;
            _rescRepository = rescRepository;
            _roleRepository = roleRepository;
            _roleRescRepository = roleRescRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, Route("api/auths/token")]
        public async Task<IHttpActionResult> Auths_Token([FromBody]TokenRequest request)
        {
            var user = await _userRepository.Entities.SingleOrDefaultAsync(x => x.Account == request.Account && x.Password == request.Password);
            if (user == null)
                return NotFound();
            if (string.IsNullOrEmpty(user.AccessToken) || user.AccessTokenExpireTime == null ||
                string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpireTime == null ||
                user.AccessTokenExpireTime <= DateTime.Now || user.RefreshTokenExpireTime <= DateTime.Now)
            {
                var token = TokenGenerate(user.UserId.ToString());
                user.AccessToken = token.AccessToken;
                user.AccessTokenExpireTime = token.AccessTokenExpireTime;
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpireTime = token.RefreshTokenExpireTime;
                await _userRepository.UpdateAsync(user);
                await _transaction.SaveChangesAsync();
            }
            return Ok(new { user.AccessToken, user.RefreshToken });
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut, Route("api/auths/token/refresh")]
        public async Task<IHttpActionResult> Auths_Refresh([FromBody]TokenRefreshRequest request)
        {
            var userId = Guid.Empty;
            var subject = (TokenValidate(request.AccessToken).Identity as ClaimsIdentity).FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (subject != null) Guid.TryParse(subject, out userId);
            var user = await _userRepository.FindAsync(userId);
            if (user?.AccessToken != request.AccessToken || user?.RefreshToken != request.RefreshToken)
                throw new TokenException("operation invalid."); // the exception filter will handle it
            if (user.RefreshTokenExpireTime <= DateTime.Now)
                throw new TokenException("refresh_token invalid."); // the exception filter will handle it
            if (user.AccessTokenExpireTime <= DateTime.Now)
            {
                var token = TokenGenerate(user.UserId.ToString());
                user.AccessToken = token.AccessToken;
                user.AccessTokenExpireTime = token.AccessTokenExpireTime;
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpireTime = token.RefreshTokenExpireTime;
                await _userRepository.UpdateAsync(user);
                await _transaction.SaveChangesAsync();
            }
            return Ok(new { user.AccessToken, user.RefreshToken });
        }

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/auths/profile")]
        public async Task<IHttpActionResult> Auths_Profile()
        {
            var user = await _userRepository.FindAsync(Identity);
            return Ok(user);
        }

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/auths/resource")]
        public async Task<IHttpActionResult> Auths_Resource()
        {
            var user = await _userRepository.FindAsync(Identity);
            if (user.Type == UserType.MANAGER)
            {
                var rescs = await _rescRepository.Entities.AsNoTracking().ToListAsync();
                return Ok(_mapper.Map<List<RescModel>>(rescs));
            }
            else
            {
                var roleIds = await _userRoleRepository.Entities.AsNoTracking().Where(x => x.UserId == Identity).Select(x => x.RoleId).ToListAsync();
                var rescIds = await _roleRescRepository.Entities.AsNoTracking().Where(x => roleIds.Contains(x.RoleId)).Select(x => x.RescId).ToListAsync();
                var rescs = await _rescRepository.Entities.AsNoTracking().Where(x => rescIds.Contains(x.RescId)).ToListAsync();
                return Ok(_mapper.Map<List<RescModel>>(rescs));
            }
        }

        /*
        // https://tahirhassan.blogspot.com/2017/12/web-apiswagger-file-upload.html
        // https://www.talkingdotnet.com/how-to-upload-file-via-swagger-in-asp-net-core-web-api/
        // https://www.jianshu.com/p/8652e1ff30c2
        // https://www.cnblogs.com/CreateMyself/p/13436060.html
        // https://www.cnblogs.com/CreateMyself/p/13458917.html
        // https://www.cnblogs.com/CreateMyself/p/13466457.html
        [Upload]
        [HttpPost, Route("api/files/upload")]
        public async Task<IHttpActionResult> Files_Upload() // 'Content-Type': 'application/x-www-form-urlencoded'
        {
            if (Request.Content.IsMimeMultipartContent() == false)
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            var path = HttpContext.Current.Server.MapPath("~/wwwroot/assets");
            var provider = new MultipartFormDataStreamProvider(path);
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var fileData in provider.FileData) // provider.FormData
            {
                var fileName = fileData.Headers.ContentDisposition.FileName.Trim('"');
                var filePath = Path.Combine(path, fileName);
                File.Move(fileData.LocalFileName, filePath);
            }
            return Ok();
        }

        // https://www.codeguru.com/csharp/.net/returning-images-from-asp.net-web-api.htm
        // https://www.c-sharpcorner.com/UploadFile/2b481f/upload-files-in-and-download-files-from-database-in-web-api/
        // https://techbrij.com/download-parallel-requests-range-asp-net-webapi
        // https://morioh.com/p/f4d331b62cda/
        [HttpGet, Route("api/files/download")]
        public IHttpActionResult Files_Download()
        {
            var path = HttpContext.Current.Server.MapPath("~/wwwroot/assets/foo.png");
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            result.Content = new StreamContent(stream);
            // result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            // result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png"); // application/octet-stream
            result.Content.Headers.ContentLength = stream.Length;
            return ResponseMessage(result);
        }
        */

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [Security("Resc")]
        [HttpGet, Route("api/rescs")]
        public async Task<IHttpActionResult> Rescs_Search()
        {
            var rescs = await _rescRepository.Entities.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map<List<RescModel>>(rescs));
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Security("Resc")]
        [HttpGet, Route("api/rescs/{id}", Name = "rescs:single")]
        public async Task<IHttpActionResult> Rescs_Single(Guid id)
        {
            var resc = await _rescRepository.FindAsync(id);
            if (resc == null)
                return NotFound();
            var model = _mapper.Map<RescUpdateModel>(resc);
            model.RoleIds = await _roleRescRepository.Entities.Where(x => x.RescId == resc.RescId).Select(x => x.RoleId).ToListAsync();
            return Ok(model);
        }

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [Security("Resc.Create", "Resc.Update")]
        [HttpGet, Route("api/rescs/before")]
        public async Task<IHttpActionResult> Rescs_Before()
        {
            /*
            var types = new List<KeyValuePair<RescType, string>>();
            var rescTypesCache = await _database.StringGetAsync("Resc`Types");
            if (string.IsNullOrWhiteSpace(rescTypesCache))
            {
                types = BaseProfile.RESCTYPE.Select(x => new KeyValuePair<RescType, string>(x.Key, x.Value)).ToList();
                await _database.StringSetAsync("Resc`Types", JsonConvert.SerializeObject(types));
            }
            else
            {
                types = JsonConvert.DeserializeObject<List<KeyValuePair<RescType, string>>>(rescTypesCache);
            }

            var icons = new List<KeyValuePair<string, string>>();
            var iconsCache = await _database.StringGetAsync("Resc`Icons");
            if (string.IsNullOrWhiteSpace(iconsCache))
            {
                icons = BaseProfile.ICON.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList();
                await _database.StringSetAsync("Resc`Icons", JsonConvert.SerializeObject(icons));
            }
            else
            {
                icons = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(iconsCache);
            }

            var roles = new List<RoleModel>();
            var rolesCache = await _database.StringGetAsync("Role");
            if (string.IsNullOrWhiteSpace(rolesCache))
            {
                roles = _mapper.Map<List<RoleModel>>(await _roleRepository.Entities.AsNoTracking().ToListAsync());
                await _database.StringSetAsync("Role", JsonConvert.SerializeObject(roles));
            }
            else
            {
                roles = JsonConvert.DeserializeObject<List<RoleModel>>(rolesCache);
            }
            */
            var types = MentozProfile.RESCTYPE.Select(x => new KeyValuePair<RescType, string>(x.Key, x.Value)).ToList();
            var icons = MentozProfile.ICON.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList();
            var roles = _mapper.Map<List<RoleModel>>(await _roleRepository.Entities.AsNoTracking().ToListAsync());
            return Ok(new { types, icons, roles });
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Security("Resc.Create")]
        [HttpPost, Route("api/rescs")]
        public async Task<IHttpActionResult> Rescs_Create([FromBody]RescCreateModel model)
        {
            var single = await _rescRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Identity == model.Identity);
            if (single != null)
                throw new ValidationException("resc's identity exists. please try another one.");
            if (model.ParentId.HasValue)
            {
                var resc2 = await _rescRepository.FindAsync(model.ParentId);
                if (resc2 == null)
                    return BadRequest(nameof(model.ParentId));
            }
            var resc = _mapper.Map<Resc>(model);
            await _rescRepository.InsertAsync(resc);
            if (model.RoleIds.Count > 0)
                await _roleRescRepository.InsertAsync(
                    model.RoleIds.Select(x => new RoleResc
                    {
                        RoleId = x,
                        RescId = resc.RescId
                    }).ToList());
            await _transaction.SaveChangesAsync();
            return CreatedAtRoute("rescs:single", new { id = resc.RescId }, "");
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Security("Resc.Update")]
        [HttpPut, Route("api/rescs/{id}")]
        public async Task<IHttpActionResult> Rescs_Update(Guid id, [FromBody]RescUpdateModel model)
        {
            if (model.Id == id)
            {
                var resc = await _rescRepository.FindAsync(id);
                if (resc == null)
                    return NotFound();
                var single = await _rescRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Identity == model.Identity && x.RescId != model.Id);
                if (single != null)
                    throw new ValidationException("resc's identity exists. please try another one.");
                if (model.Version != resc.Version.ToHexString())
                    throw new DbUpdateConcurrencyException("data changed.");
                if (model.ParentId.HasValue)
                {
                    var resc2 = await _rescRepository.FindAsync(model.ParentId);
                    if (resc2 == null)
                        return BadRequest(nameof(model.ParentId));
                }
                await _rescRepository.UpdateAsync(_mapper.Map(model, resc));

                var roleRescs = model.RoleIds.Select(x => new RoleResc
                {
                    RoleId = x,
                    RescId = resc.RescId
                }).ToList();
                var roleRescs2 = await _roleRescRepository.Entities.Where(x => x.RescId == resc.RescId).ToListAsync();
                var insertRoleRescs = roleRescs.Except(roleRescs2).ToList();
                if (insertRoleRescs.Count > 0)
                    await _roleRescRepository.InsertAsync(insertRoleRescs);
                var deleteRoleRescs = roleRescs2.Except(roleRescs).ToList();
                if (deleteRoleRescs.Count > 0)
                    await _roleRescRepository.DeleteAsync(deleteRoleRescs);

                await _transaction.SaveChangesAsync();
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Security("Resc.Delete")]
        [HttpDelete, Route("api/rescs/{id}")]
        public async Task<IHttpActionResult> Rescs_Delete(Guid id)
        {
            var resc = await _rescRepository.FindAsync(id);
            if (resc == null)
                return NotFound();
            await _rescRepository.DeleteAsync(resc);
            await _transaction.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [Security("Role")]
        [HttpGet, Route("api/roles")]
        public async Task<IHttpActionResult> Roles_Search()
        {
            var roles = await _roleRepository.Entities.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map<List<RoleModel>>(roles));
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Security("Role")]
        [HttpGet, Route("api/roles/{id}", Name = "roles:single")]
        public async Task<IHttpActionResult> Roles_Single(Guid id)
        {
            var role = await _roleRepository.FindAsync(id);
            if (role == null)
                return NotFound();
            var model = _mapper.Map<RoleUpdateModel>(role);
            model.RescIds = await _roleRescRepository.Entities.Where(x => x.RoleId == role.RoleId).Select(x => x.RescId).ToListAsync();
            model.UserIds = await _userRoleRepository.Entities.Where(x => x.RoleId == role.RoleId).Select(x => x.UserId).ToListAsync();
            return Ok(model);
        }

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [Security("Role.Create", "Role.Update")]
        [HttpGet, Route("api/roles/before")]
        public async Task<IHttpActionResult> Roles_Before()
        {
            var rescs = _mapper.Map<List<RescModel>>(await _rescRepository.Entities.AsNoTracking().ToListAsync());
            var users = _mapper.Map<List<UserModel>>(await _userRepository.Entities.AsNoTracking().ToListAsync());
            return Ok(new { rescs, users });
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Security("Role.Create")]
        [HttpPost, Route("api/roles")]
        public async Task<IHttpActionResult> Roles_Create([FromBody]RoleCreateModel model)
        {
            var single = await _roleRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Display == model.Display);
            if (single != null)
                throw new ValidationException("role's display exists. please try another one.");
            var role = _mapper.Map<Role>(model);
            await _roleRepository.InsertAsync(role);
            if (model.RescIds.Count > 0)
                await _roleRescRepository.InsertAsync(
                    model.RescIds.Select(x => new RoleResc
                    {
                        RescId = x,
                        RoleId = role.RoleId
                    }).ToList());
            if (model.UserIds.Count > 0)
                await _userRoleRepository.InsertAsync(
                    model.UserIds.Select(x => new UserRole
                    {
                        UserId = x,
                        RoleId = role.RoleId
                    }).ToList());
            await _transaction.SaveChangesAsync();
            return CreatedAtRoute("roles:single", new { id = role.RoleId }, "");
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Security("Role.Update")]
        [HttpPut, Route("api/roles/{id}")]
        public async Task<IHttpActionResult> Roles_Update(Guid id, [FromBody]RoleUpdateModel model)
        {
            if (model.Id == id)
            {
                var role = await _roleRepository.FindAsync(id);
                if (role == null)
                    return NotFound();
                var single = await _roleRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Display == model.Display && x.RoleId != model.Id);
                if (single != null)
                    throw new ValidationException("role's display exists. please try another one.");
                if (model.Version != role.Version.ToHexString())
                    throw new DbUpdateConcurrencyException("data changed.");
                await _roleRepository.UpdateAsync(_mapper.Map(model, role));
                var roleRescs = model.RescIds.Select(x => new RoleResc
                {
                    RescId = x,
                    RoleId = role.RoleId
                }).ToList();
                var roleRescs2 = await _roleRescRepository.Entities.Where(x => x.RoleId == role.RoleId).ToListAsync();
                var insertRoleRescs = roleRescs.Except(roleRescs2).ToList();
                if (insertRoleRescs.Count > 0)
                    await _roleRescRepository.InsertAsync(insertRoleRescs);
                var deleteRoleRescs = roleRescs2.Except(roleRescs).ToList();
                if (deleteRoleRescs.Count > 0)
                    await _roleRescRepository.DeleteAsync(deleteRoleRescs);
                var userRoles = model.UserIds.Select(x => new UserRole
                {
                    UserId = x,
                    RoleId = role.RoleId
                }).ToList();
                var userRoles2 = await _userRoleRepository.Entities.Where(x => x.RoleId == role.RoleId).ToListAsync();
                var insertUserRoles = userRoles.Except(userRoles2).ToList();
                if (insertUserRoles.Count > 0)
                    await _userRoleRepository.InsertAsync(insertUserRoles);
                var deleteUserRoles = userRoles2.Except(userRoles).ToList();
                if (deleteUserRoles.Count > 0)
                    await _userRoleRepository.DeleteAsync(deleteUserRoles);
                await _transaction.SaveChangesAsync();
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Security("Role.Delete")]
        [HttpDelete, Route("api/roles/{id}")]
        public async Task<IHttpActionResult> Roles_Delete(Guid id)
        {
            var role = await _roleRepository.FindAsync(id);
            if (role == null)
                return NotFound();
            await _roleRepository.DeleteAsync(role);
            await _transaction.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [Security("User")]
        [HttpGet, Route("api/users")]
        public async Task<IHttpActionResult> Users_Search()
        {
            var users = await _userRepository.Entities.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map<List<UserModel>>(users));
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Security("User")]
        [HttpGet, Route("api/users/{id}", Name = "users:single")]
        public async Task<IHttpActionResult> Users_Single(Guid id)
        {
            var user = await _userRepository.FindAsync(id);
            if (user == null)
                return NotFound();
            var model = _mapper.Map<UserUpdateModel>(user);
            model.RoleIds = await _userRoleRepository.Entities.Where(x => x.UserId == user.UserId).Select(x => x.RoleId).ToListAsync();
            return Ok(model);
        }

        /// <summary>
        /// √
        /// </summary>
        /// <returns></returns>
        [Security("User.Create", "User.Update")]
        [HttpGet, Route("api/users/before")]
        public async Task<IHttpActionResult> Users_Before()
        {
            var roles = _mapper.Map<List<RoleModel>>(await _roleRepository.Entities.AsNoTracking().ToListAsync());
            return Ok(new { roles });
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Security("User.Create")]
        [HttpPost, Route("api/users")]
        public async Task<IHttpActionResult> Users_Create([FromBody]UserCreateModel model)
        {
            var single = await _userRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Account == model.Account);
            if (single != null)
                throw new ValidationException("user's account exists. please try another one.");
            var user = _mapper.Map<User>(model);
            await _userRepository.InsertAsync(user);
            if (model.RoleIds.Count > 0)
                await _userRoleRepository.InsertAsync(
                    model.RoleIds.Select(x => new UserRole
                    {
                        RoleId = x,
                        UserId = user.UserId
                    }).ToList());
            await _transaction.SaveChangesAsync();
            return CreatedAtRoute("users:single", new { id = user.UserId }, "");
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Security("User.Update")]
        [HttpPut, Route("api/users/{id}")]
        public async Task<IHttpActionResult> Users_Update(Guid id, [FromBody]UserUpdateModel model)
        {
            if (model.Id == id)
            {
                var user = await _userRepository.FindAsync(id);
                if (user == null)
                    return NotFound();
                var single = await _userRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Account == model.Account && x.UserId != model.Id);
                if (single != null)
                    throw new ValidationException("user's account exists. please try another one.");
                if (model.Version != user.Version.ToHexString())
                    throw new DbUpdateConcurrencyException("data changed.");
                await _userRepository.UpdateAsync(_mapper.Map(model, user));
                var userRoles = model.RoleIds.Select(x => new UserRole
                {
                    RoleId = x,
                    UserId = user.UserId
                }).ToList();
                var userRoles2 = await _userRoleRepository.Entities.Where(x => x.UserId == user.UserId).ToListAsync();
                var insertUserRoles = userRoles.Except(userRoles2).ToList();
                if (insertUserRoles.Count > 0)
                    await _userRoleRepository.InsertAsync(insertUserRoles);
                var deleteUserRoles = userRoles2.Except(userRoles).ToList();
                if (deleteUserRoles.Count > 0)
                    await _userRoleRepository.DeleteAsync(deleteUserRoles);
                await _transaction.SaveChangesAsync();
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// √
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Security("User.Delete")]
        [HttpDelete, Route("api/users/{id}")]
        public async Task<IHttpActionResult> Users_Delete(Guid id)
        {
            var user = await _userRepository.FindAsync(id);
            if (user == null)
                return NotFound();
            await _userRepository.DeleteAsync(user);
            await _transaction.SaveChangesAsync();
            return NoContent();
        }

        #region ActionResults
        private StatusCodeResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
        #endregion

        #region Identity
        private Guid Identity
        {
            get
            {
                var identity = Guid.Empty;
                var subject = (User.Identity as ClaimsIdentity).FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub);
                if (subject != null) Guid.TryParse(subject.Value, out identity);
                return identity;
            }
        }
        #endregion

        #region Token
        public class TokenRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
        public class TokenRefreshRequest
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
        private dynamic TokenGenerate(string subject)
        {
            var notBefore = DateTime.Now;
            var accessTokenExpireTime = notBefore.AddSeconds(3600);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    Mentoz.Issuer,
                    Mentoz.Audience,
                    new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, subject)
                    },
                    notBefore,
                    accessTokenExpireTime,
                    new SigningCredentials(new SymmetricSecurityKey(
                        Convert.FromBase64String(Mentoz.Secret)),
                        SecurityAlgorithms.HmacSha256)
                )
            );
            var refreshTokenExpireTime = notBefore.AddSeconds(3600 * 24);
            var bytes = new byte[32];
            using (var generator = RandomNumberGenerator.Create()) generator.GetBytes(bytes);
            var refreshToken = Convert.ToBase64String(bytes);
            return new
            {
                AccessToken = accessToken,
                AccessTokenExpireTime = accessTokenExpireTime,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = refreshTokenExpireTime
            };
        }
        private ClaimsPrincipal TokenValidate(string accessToken)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Mentoz.Secret)),
                    RequireExpirationTime = true,
                    ValidAudience = Mentoz.Audience,
                    ValidIssuer = Mentoz.Issuer,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false
                };
                return new JwtSecurityTokenHandler().ValidateToken(accessToken, validationParameters, out var validatedToken);
            }
            catch (Exception)
            {
                throw new TokenException("access_token invalid."); // exception filter will handle it
            }
        }
        #endregion
    }
}