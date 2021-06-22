using System;

namespace Mentoz.AspNet.Api
{
    public class User : Entity
    {
        public Guid UserId { get; set; }
        public UserType Type { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Display { get; set; }
        public bool Available { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireTime { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }
    }
}