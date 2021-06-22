using System.Data.Entity.ModelConfiguration;

namespace Mentoz.AspNet.Api
{
    public class UserRoleEntityTypeConfiguration : EntityTypeConfiguration<UserRole>
    {
        public UserRoleEntityTypeConfiguration()
        {
            ToTable(nameof(UserRole));
            HasKey(x => new { x.UserId, x.RoleId });
        }
    }
}