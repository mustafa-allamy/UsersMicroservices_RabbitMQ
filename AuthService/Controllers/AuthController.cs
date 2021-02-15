using AuthService.Interfaces.Services;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _service;
        public AuthController(IUserService service)
        {
            _service = service;
        }

        [Route(template: "api/[controller]/[action]"), ActionName("Token"),
         AllowAnonymous, HttpPost, ProducesResponseType(type: typeof(ClientResponse<string>), statusCode: 200),
         ProducesResponseType(type: typeof(ClientResponse<string>), statusCode: 400),
         ProducesResponseType(type: typeof(ClientResponse<string>), statusCode: 401)]
        public async Task<IActionResult> Token([FromBody]
            LoginDto form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(error: new ClientResponse<string>(data: "please provide username and password"));
            }
            var serviceResponse = await _service.DoLogin(form: form);
            if (serviceResponse.Error != null)
            {
                return Unauthorized();
            }
            var claims = new List<Claim>
            {
                new Claim(type: JwtRegisteredClaimNames.Sub, value: serviceResponse.Value.Id.ToString()),
                new Claim(type: JwtRegisteredClaimNames.Email, value: serviceResponse.Value.Email),
                new Claim(type: JwtRegisteredClaimNames.GivenName, value: serviceResponse.Value.FullName),
                new Claim(type: "FeRole", value: serviceResponse.Value.Role),
                new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                new Claim(type: "id", value: serviceResponse.Value.Id.ToString()),
            };
            AddRolesToClaims(claims, serviceResponse.Value.Role.Split(','));
            var token = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.UtcNow.AddDays(value: 3),
                notBefore: DateTime.UtcNow,
                audience: "Audience",
                issuer: "Issuer",
                signingCredentials: new SigningCredentials(
                    key: new
                        SymmetricSecurityKey(key: Encoding
                                                .UTF8
                                                .GetBytes(s: "dmiWqigAEvWmCq5TgJLhuHvByNY5PCnb")),
                    algorithm: SecurityAlgorithms.HmacSha256)
            );
            return Ok(value: new { token = new JwtSecurityTokenHandler().WriteToken(token: token) });
        }
        private void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }
    }
}
