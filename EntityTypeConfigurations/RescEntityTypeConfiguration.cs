using System.Data.Entity.ModelConfiguration;

namespace Mentoz.AspNet.Api
{
    public class RescEntityTypeConfiguration : EntityTypeConfiguration<Resc>
    {
        public RescEntityTypeConfiguration()
        {
            ToTable(nameof(Resc));
            HasKey(x => x.RescId);
            Property(x => x.Identity).HasMaxLength(50).IsRequired();
            HasIndex(x => x.Identity).IsUnique();
        }
    }
}