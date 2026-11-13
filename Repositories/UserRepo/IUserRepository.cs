using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.UserRepo
{
    public interface IUserRepository
    {
        public Task RegisterAsync(User user);
        public Task<User?> FindUserAsync(string login);
        public Task Delete(User user);

        public Task<bool> SaveChangesAsync();

    }
}
