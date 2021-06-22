using System;

namespace Mentoz.AspNet.Api
{
    public class RescModel
    {
        public Guid Id { get; set; }
        public RescType Type { get; set; }
        public string TypeDisplay { get; set; }
        public string Identity { get; set; }
        public string Icon { get; set; }
        public string Display { get; set; }
        public bool Available { get; set; }
        public string AvailableDisplay { get; set; }
        public Guid? ParentId { get; set; }
    }
}