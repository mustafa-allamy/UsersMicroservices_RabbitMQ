using AuthService.Interfaces.Repositories;
using Infrastructure;
using Models.Models;

namespace AuthService.Repositories
{
    public class UserRepository : RepositoryBase<User, AuthContext>, IUserRepository
    {
        public UserRepository(AuthContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}