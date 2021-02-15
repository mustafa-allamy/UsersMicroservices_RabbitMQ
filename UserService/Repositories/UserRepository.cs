using Infrastructure;
using Models.Models;
using UserService.Interfaces;

namespace UserService.Repositories
{
    public class UserRepository : RepositoryBase<User, UserContext>, IUserRepository
    {
        public UserRepository(UserContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}