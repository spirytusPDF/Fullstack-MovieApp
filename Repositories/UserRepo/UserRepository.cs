using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.UserRepo
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;
        public async Task Delete(User user)
        {
            try
            {
                _context.Users.Remove(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Task<User?> FindUserAsync(string login)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == login);
        }

        public async Task RegisterAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0 ? true : false;
        }
    }
}
