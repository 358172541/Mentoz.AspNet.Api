using System;
using System.Collections.Generic;

namespace Mentoz.AspNet.Api
{
    public class RescCreateModel
    {
        public RescType Type { get; set; }
        public string Identity { get; set; }
        public string Icon { get; set; }
        public string Display { get; set; }
        public bool Available { get; set; }
        public Guid? ParentId { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }

}