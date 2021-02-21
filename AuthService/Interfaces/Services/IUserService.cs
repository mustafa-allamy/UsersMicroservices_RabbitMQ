using Infrastructure.Helpers;
using Models.Dtos;
using Models.Models;
using System.Threading.Tasks;

namespace AuthService.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> DoLogin(LoginDto form);
        Task<ServiceResponse<UserDto>> AddUser(User form);
        Task<ServiceResponse<bool>> UpdateUser(User form);
    }
}