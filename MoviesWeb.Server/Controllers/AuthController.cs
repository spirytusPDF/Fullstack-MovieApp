using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Server.Contracts;
using Services;
using Services.DTOs.User;
using System.Security.Claims;

namespace MoviesWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserService _userService,
        IValidator<UserRegisterContract> _registerValidator,
        IValidator<UserLoginContract> _loginValidator,
        IMapper _mapper) : ControllerBase
    {
        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] UserRegisterContract registerContract)
        {
            Console.WriteLine("Reached");
            var valid = _registerValidator.Validate(registerContract);
            if (!valid.IsValid) return BadRequest(valid.Errors);

            var userDto = _mapper.Map<UserRegisterDTO>(registerContract);
            var res = _userService.RegisterUser(userDto);
            if (res.IsSucces) return Ok();
            else return StatusCode(500, res.Message);

        }
        [HttpPost("login")]
        public async Task<IActionResult> SignIn(UserLoginContract loginContract)
        {
            var valid = _loginValidator.Validate(loginContract);
            if (!valid.IsValid) return BadRequest(valid.Errors);

            var userDto = _mapper.Map<UserLoginDTO>(loginContract);
            var principal = _userService.LoginUser(userDto);
            if (principal is null)
            {
                return Unauthorized(new { message = "Incorrect pass or login" });
            }
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Ok(new { message = "Signed in" });

        }
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout" });

        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.Identity?.Name;
            var email = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new { id = userId, username, email });
        }



        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] string email)
        {
            var result = _userService.DeleteUser(email);
            if (result.IsSucces) return Ok();
            else return StatusCode(500, result.Message);
        }
    }
}
