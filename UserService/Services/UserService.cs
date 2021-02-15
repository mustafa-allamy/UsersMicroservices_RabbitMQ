using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using Models.Forms.Create;
using Models.Forms.Update;
using Models.Models;
using Newtonsoft.Json;
using RabbitMQService.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.Interfaces.Services;
using Cipher = BCrypt.Net.BCrypt;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDataSender _dataSender;

        public UserService(IUserRepository userRepository, IDataSender dataSender)
        {
            _userRepository = userRepository;
            _dataSender = dataSender;
        }
        public async Task<ServiceResponse<UserDto>> AddUser(CreateUserForm form)
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
                Password = Cipher.HashPassword(inputKey: form.Password),
                FullName = form.FullName,
                PhoneNumber = form.PhoneNumber,
                Role = form.Role
            };
            var json = JsonConvert.SerializeObject(user);
            _dataSender.SendData(json, "AuthQueue");

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

        public async Task<ServiceResponse<bool>> UpdateUser(UpdateUserForm form)
        {
            var user = await _userRepository.FindItemByCondition(x => x.Id == form.Id);
            if (user == null)
                return new ServiceResponse<bool>(false)
                {
                    Error = new ResponseError("User Not Found!")
                };
            if (!string.IsNullOrEmpty(form.PhoneNumber))
            {
                var similerUser = await _userRepository.FindItemByCondition(x => x.Id != form.Id && x.Email == form.PhoneNumber);
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
                user.Password = Cipher.HashPassword(form.Password);

            await _userRepository.Update(user);
            await _userRepository.Commit();
            var json = JsonConvert.SerializeObject(user);
            _dataSender.SendData(json, "AuthQueue");
            _dataSender.SendData(json, "NotificationQueue");
            return new ServiceResponse<bool>(true);
        }

        public async Task<ServiceResponse<List<UserDto>>> GetUsers(int start, int take)
        {
            var users = await _userRepository.FindAll().Select(x => new UserDto()
            {
                Email = x.Email,
                FullName = x.FullName,
                Id = x.Id,
                PhoneNumber = x.PhoneNumber
            }).Skip(start).Take(take).ToListAsync();
            if (!users.Any())
                return new ServiceResponse<List<UserDto>>(null)
                {
                    Error = new ResponseError("No Data")
                };
            return new ServiceResponse<List<UserDto>>(users, await _userRepository.FindAll().CountAsync());
        }

    }
}