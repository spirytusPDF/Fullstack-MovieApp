using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public interface IJwtTokenService
    {
        Task<AuthResult> GenerateTokenAsync(User user);
    }

    public class JwtTokenGenerator : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public JwtTokenGenerator(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<AuthResult> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new("login", user.Login),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim("role", role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)); 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var time = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpirationTime"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: time,
                signingCredentials: creds
            );

            return new AuthResult()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpirationTime = time
            };
                
        }
    }
}
