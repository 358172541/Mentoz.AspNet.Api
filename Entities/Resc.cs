using System;

namespace Mentoz.AspNet.Api
{
    public class Resc : Entity
    {
        public Guid RescId { get; set; }
        public RescType Type { get; set; }
        public string Identity { get; set; }
        public string Icon { get; set; }
        public string Display { get; set; }
        public bool Available { get; set; }
        public Guid? ParentId { get; set; }
    }
}