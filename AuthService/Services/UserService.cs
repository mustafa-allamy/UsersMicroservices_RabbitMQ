using AuthService.Interfaces.Repositories;
using AuthService.Interfaces.Services;
using Infrastructure.Helpers;
using Models.Dtos;
using Models.Models;
using System;
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
        public async Task<ServiceResponse<UserDto>> AddUser(User form)
        {
            var user = await _userRepository.FindItemByCondition(x =>
                x.Email == form.Email || x.Password == form.Password);
            if (user != null)
                return new ServiceResponse<UserDto>(null)
                {
                    Error = new ResponseError("User Already Exist!")
                };
            user = new User()
            {
                Id = Guid.NewGuid(),
                Email = form.Email,
                Password = form.Password,
                FullName = form.FullName,
                PhoneNumber = form.PhoneNumber,
                Role = form.Role
            };
            await _userRepository.Insert(user);
            await _userRepository.Commit();

            return new ServiceResponse<UserDto>(new UserDto()
            {
                Email = user.Email,
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            });
        }

        public async Task<ServiceResponse<bool>> UpdateUser(User form)
        {
            var user = await _userRepository.FindItemByCondition(x => x.Id == form.Id);
            if (user == null)
                return new ServiceResponse<bool>(false)
                {
                    Error = new ResponseError("User Not Found!")
                };
            if (!string.IsNullOrEmpty(form.PhoneNumber))
            {
                var similerUser = await _userRepository.FindItemByCondition(x => x.Id != form.Id && x.PhoneNumber == form.PhoneNumber);
                if (similerUser != null)
                    return new ServiceResponse<bool>(false)
                    {
                        Error = new ResponseError("Phone Number Is Taken")
                    };
                user.PhoneNumber = form.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(form.FullName))
                user.FullName = form.FullName;
            if (!string.IsNullOrEmpty(form.Password))
                user.Password = form.Password;

            await _userRepository.Update(user);
            await _userRepository.Commit();

            return new ServiceResponse<bool>(true);
        }




    }
}