using System.Data.Entity.ModelConfiguration;

namespace Mentoz.AspNet.Api
{
    public class UserEntityTypeConfiguration : EntityTypeConfiguration<User>
    {
        public UserEntityTypeConfiguration()
        {
            ToTable(nameof(User));
            HasKey(x => x.UserId);
            Property(x => x.Account).HasMaxLength(50).IsRequired();
            HasIndex(x => x.Account).IsUnique();
        }
    }
}