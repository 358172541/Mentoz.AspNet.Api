using System;

namespace Mentoz.AspNet.Api
{
    public class Role : Entity
    {
        public Guid RoleId { get; set; }
        public string Display { get; set; }
        public bool Available { get; set; }
    }
}