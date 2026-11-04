using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.UserRepo
{
    public interface IUserRepository
    {
        public bool Register(User user);
        public User FindUser(string login);
        public bool Delete(User user);

    }
}
