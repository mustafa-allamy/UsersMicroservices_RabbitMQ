using AuthService.Interfaces.Repositories;
using AuthService.Interfaces.Services;
using Infrastructure.Helpers;
using Models.Dtos;
using Models.Models;
using System.Threading.Tasks;
using Cipher = BCrypt.Net.BCrypt;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<User>> DoLogin(LoginDto form)
        {
            var user = await _userRepository.FindItemByCondition(x => x.Email == form.Email);
            if (user == null)
            {
                return new ServiceResponse<User>(value: null)
                {
                    Error = new ResponseError(message: "Bad Username")
                };
            }
            if (!Cipher.Verify(text: form.Password, hash: user.Password))
            {
                return new ServiceResponse<User>(value: null)
                {

                    Error = new ResponseError(message: "Bad password")
                };
            }
            return new ServiceResponse<User>(value: user);
        }




    }
}