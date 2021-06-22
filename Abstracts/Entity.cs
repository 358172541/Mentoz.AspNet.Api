using System;

namespace Mentoz.AspNet.Api
{
    public abstract class Entity
    {
        public DateTime CreateTime { get; set; }
        public Guid Creator { get; set; }
        public DateTime UpdateTime { get; set; }
        public Guid Updator { get; set; }
        public DateTime DeleteTime { get; set; }
        public Guid Deletor { get; set; }
        public byte[] Version { get; set; }
    }
}