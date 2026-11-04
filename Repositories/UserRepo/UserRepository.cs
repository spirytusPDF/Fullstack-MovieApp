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
        public bool Delete(User user)
        {
            try
            {
                _context.Users.Remove(user);
                var res = _context.SaveChanges();
                return res > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public User FindUser(string login)
        {
            return _context.Users.FirstOrDefault(u => u.Email == login);
        }

        public bool Register(User user)
        {
            try
            {
                _context.Users.Add(user);
                var res = _context.SaveChanges();
                return res > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
