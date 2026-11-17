using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Identity;
using Services.DTOs.User;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class IdentityAuthService(UserManager<User> _userManager, 
        IMapper _mapper, SignInManager<User> _signInManager, IJwtTokenGenerator _jwtGenerator)
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

                var token = await _jwtGenerator.GenerateTokenAsync(user);

                return new AuthResult
                {
                    Succes = true,
                    Token = token,
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
                    Errors = result.Errors.Select(e=>e.Description).ToArray()
                };
            }

        }

        public async Task<AuthResult> LoginUserAsync(UserLoginDTO userLoginDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);
            if (user is null)
                return new AuthResult
                {
                    Succes = false,
                    UserId = null,
                    Errors = ["Invalid email"]
                };

            var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            userLoginDTO.Password,
            lockoutOnFailure: true);

            if (result.IsLockedOut)
                return  new AuthResult
                {
                    Succes = false,
                    UserId = null,
                    Errors = ["Account is locked due to multiple failed attempts."]
                };

            if (!result.Succeeded)
                return new AuthResult
                {
                    Succes = false,
                    UserId = null,
                    Errors = ["Invalid email or password"]
                };

            var token = await _jwtGenerator.GenerateTokenAsync(user);
            return new AuthResult
            {
                Succes = true,
                Token = token,
                UserId = user.Id.ToString(),
                Errors = Array.Empty<string>()
            };
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return;

            await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task<AuthResult> DeleteAccountAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return new AuthResult
                {
                    Succes = false,
                    UserId = null,
                    Errors = ["User not found"]
                };

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                return new AuthResult
                {
                    Succes = false,
                    UserId = null,
                    Errors = ["Invalid password"]
                };

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return new AuthResult
                {
                    Succes = false,
                    UserId = null,
                    Errors = result.Errors.Select(e => e.Description).ToArray()
                };

            return new AuthResult
            {
                Succes = true,
                UserId = null,
                Errors = Array.Empty<string>()
            };
        }
    }
}
