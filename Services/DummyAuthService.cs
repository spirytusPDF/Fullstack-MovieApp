using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Identity;
using Services.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class DummyAuthService(UserManager<User> _userManager,
        SignInManager<User> _signInManager,
        IMapper _mapper)
    {
        public async Task<AuthResult> RegisterUserAsync(UserRegisterDTO userDto, string password)
        {
            var user = _mapper.Map<User>(userDto);
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    return new AuthResult
                    {
                        Succes = false,
                        UserId = user.Id.ToString(),
                        Errors = roleResult.Errors.Select(error => error.Description).ToArray()
                    };
                }


                return new AuthResult
                {
                    Succes = true,
                    UserId = user.Id.ToString(),
                    Errors = Array.Empty<string>()
                };

            }
            else
            {
                return new AuthResult
                {
                    Succes = false,
                    UserId = null,
                    Errors = result.Errors.Select(e => e.Description).ToArray()
                };
            }

        }

        public async Task<bool> ValidateUserCredsAsync(UserLoginDTO user)
        {
            var userFound = await _userManager.FindByEmailAsync(user.Email);
            if(userFound == null)
            {
                return false;
            }
            var res = await _signInManager.CheckPasswordSignInAsync(userFound, user.Password, false);
            return res.Succeeded;
        }
    }
}
