using System.Data.Entity.ModelConfiguration;

namespace Mentoz.AspNet.Api
{
    public class RoleRescEntityTypeConfiguration : EntityTypeConfiguration<RoleResc>
    {
        public RoleRescEntityTypeConfiguration()
        {
            ToTable(nameof(RoleResc));
            HasKey(x => new { x.RoleId, x.RescId });
        }
    }
}