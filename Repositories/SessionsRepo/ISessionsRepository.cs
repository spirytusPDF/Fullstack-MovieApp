using Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repositories.SessionsRepo
{
    public interface ISessionsRepository
    {
        public Task<MovieSession?> GetOneAsync(int id);
        public Task<Tuple<List<MovieSession>, int>> GetPagedSessionsAsync(Expression<Func<MovieSession, bool>> predicate, int page, int size, string sort);


        public Task AddSessionAsync(MovieSession session);
        public Task UpdateSessionAsync(MovieSession session);
        public Task DeleteSession(MovieSession session);

        public Task<bool> SaveChangesAsync();
    }
}
