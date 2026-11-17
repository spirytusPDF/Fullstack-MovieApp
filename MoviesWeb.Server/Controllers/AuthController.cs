using AutoMapper;
using Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Server.Contracts;
using Services;
using Services.DTOs.User;
using System.Security.Claims;

namespace MoviesWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IValidator<UserRegisterContract> _registerValidator,
        IValidator<UserLoginContract> _loginValidator,
        IMapper _mapper,IdentityAuthService _authService) : ControllerBase
        
    {

        [HttpPost("signup")]

        public async Task<IActionResult> Register([FromBody] UserRegisterContract registerContract)
        {
            var validationresult = _registerValidator.Validate(registerContract);
            if (!validationresult.IsValid) return BadRequest(new { errors = validationresult.Errors });

            var user = _mapper.Map<UserRegisterDTO>(registerContract);
            var result = await _authService.RegisterUserAsync(user, registerContract.Password);
            if (!result.Succes)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { token = result.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignIn(UserLoginContract loginContract)
        {
            var validationresult = _loginValidator.Validate(loginContract);
            if (!validationresult.IsValid) return BadRequest(new { errors = validationresult.Errors });
            var user = _mapper.Map<UserLoginDTO>(loginContract);

            var result = await _authService.LoginUserAsync(user);
            if (!result.Succes)
                return Unauthorized(new { errors = result.Errors });

            return Ok(new { token = result.Token });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut([FromBody] string id)
        {
            await _authService.LogoutAsync(id);
            return Ok();
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] UserLoginContract dto)
        {
            var validationresult = _loginValidator.Validate(dto);
            if (!validationresult.IsValid) return BadRequest(new { errors = validationresult.Errors });
            var user = _mapper.Map<UserLoginDTO>(dto);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var result = await _authService.DeleteAccountAsync(dto.Email, dto.Password);
            if (!result.Succes)
                return BadRequest(new { errors = result.Errors });

            return NoContent();
        }
    }
}
