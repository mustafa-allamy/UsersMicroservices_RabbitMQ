using Infrastructure.Helpers;
using Models.Dtos;
using Models.Forms.Create;
using Models.Forms.Update;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserService.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> AddUser(CreateUserForm form);
        Task<ServiceResponse<bool>> UpdateUser(UpdateUserForm form);
        Task<ServiceResponse<List<UserDto>>> GetUsers(int start, int take);
    }
}