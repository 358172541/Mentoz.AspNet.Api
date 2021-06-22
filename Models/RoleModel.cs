using System;

namespace Mentoz.AspNet.Api
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Display { get; set; }
        public bool Available { get; set; }
        public string AvailableDisplay { get; set; }
    }
}