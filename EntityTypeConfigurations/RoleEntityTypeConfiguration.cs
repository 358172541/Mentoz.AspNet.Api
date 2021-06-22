using System.Data.Entity.ModelConfiguration;

namespace Mentoz.AspNet.Api
{
    public class RoleEntityTypeConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleEntityTypeConfiguration()
        {
            ToTable(nameof(Role));
            HasKey(x => x.RoleId);
            Property(x => x.Display).HasMaxLength(50).IsRequired();
            HasIndex(x => x.Display).IsUnique();
        }
    }
}