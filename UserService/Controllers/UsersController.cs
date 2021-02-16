using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Forms.Create;
using Models.Forms.Update;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Interfaces.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClientResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ClientResponse<string>), 400)]
        [ProducesResponseType(typeof(void), 204)]
        public async Task<IActionResult> AddUser(CreateUserForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var serviceResponse = await _userService.AddUser(form);
            if (serviceResponse.Error != null)
            {
                return Ok(new ClientResponse<string>(true,
                    serviceResponse.Error.Message));
            }
            return Ok(new ClientResponse<UserDto>(serviceResponse.Value));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClientResponse<bool>), 200)]
        [ProducesResponseType(typeof(ClientResponse<string>), 400)]
        [ProducesResponseType(typeof(void), 204)]
        public async Task<IActionResult> UpdateUser(UpdateUserForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var serviceResponse = await _userService.UpdateUser(form);
            if (serviceResponse.Error != null)
            {
                return Ok(new ClientResponse<string>(true,
                    serviceResponse.Error.Message));
            }
            return Ok(new ClientResponse<bool>(serviceResponse.Value));
        }
        [HttpGet]
        [ProducesResponseType(typeof(ClientResponse<List<UserDto>>), 200)]
        [ProducesResponseType(typeof(ClientResponse<string>), 400)]
        [ProducesResponseType(typeof(void), 204)]
        public async Task<IActionResult> GetUsers(int start, int take)
        {

            var serviceResponse = await _userService.GetUsers(start, take);
            if (serviceResponse.Error != null)
            {
                return Ok(new ClientResponse<string>(true,
                    serviceResponse.Error.Message));
            }
            return Ok(new ClientResponse<List<UserDto>>(serviceResponse.Value, serviceResponse.TotalCount));
        }

    }
}
