using System;
using System.Collections.Generic;

namespace Mentoz.AspNet.Api
{
    public class RoleCreateModel
    {
        public string Display { get; set; }
        public bool Available { get; set; }
        public List<Guid> RescIds { get; set; } = new List<Guid>();
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
}