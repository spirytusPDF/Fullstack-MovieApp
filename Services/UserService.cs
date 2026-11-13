using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Repositories.UserRepo;
using Services.DTOs.User;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class UserService(IUserRepository userRepository, IMapper _mapper)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result> RegisterUserAsync(UserRegisterDTO userRegisterDTO)
        {
            if (await _userRepository.FindUserAsync(userRegisterDTO.Email) != null)
                return new Result() { IsSucces = false, Message = "User already exists. Please, sign in" };

            var user = _mapper.Map<User>(userRegisterDTO);
            user.Created = DateTime.Now;
            await _userRepository.RegisterAsync(user);
            var result = await _userRepository.SaveChangesAsync();
            if (result) return new Result() { IsSucces = true, Message = "Registered! Please sign in" };
            else return new Result() { IsSucces = false, Message = "Could't register user" };
        }

        public async Task<ClaimsPrincipal?> LoginUser(UserLoginDTO userLoginDTO)
        {
            var user = await _userRepository.FindUserAsync(userLoginDTO.Email);
            if (user == null) return null;

            if (user.Password.Equals(userLoginDTO.Password))
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                return new ClaimsPrincipal(identity);
            }
            else return null;
        }

        public async  Task<Result> DeleteUser(string email)
        {
            var user = await _userRepository.FindUserAsync(email);
            if (user == null) return new Result() { IsSucces = false, Message = "Couldn't find user" };
            await _userRepository.Delete(user);
            var res = await _userRepository.SaveChangesAsync();
            if (res) return new Result() { IsSucces = true };
            else return new Result() {IsSucces = false, Message="Could't delete user" };
        }

        public async Task<UserResponseDTO?> GetUser(string email)
        {
            var user = await _userRepository.FindUserAsync(email);
            if (user == null) return null;

            return _mapper.Map<UserResponseDTO>(user);

        }
    }
}
