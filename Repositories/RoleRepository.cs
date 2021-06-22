namespace Mentoz.AspNet.Api
{
    public class RoleRepository : MentozRepository<Role>, IRoleRepository
    {
        public RoleRepository(ITransaction transaction) : base(transaction) { }
    }
}