namespace Mentoz.AspNet.Api
{
    public class UserRoleRepository : MentozRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ITransaction transaction) : base(transaction) { }
    }
}