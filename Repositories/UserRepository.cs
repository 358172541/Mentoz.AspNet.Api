namespace Mentoz.AspNet.Api
{
    public class UserRepository : MentozRepository<User>, IUserRepository
    {
        public UserRepository(ITransaction transaction) : base(transaction) { }
    }
}