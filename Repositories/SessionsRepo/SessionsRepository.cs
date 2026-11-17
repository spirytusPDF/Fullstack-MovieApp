using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Contexts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repositories.SessionsRepo
{
    public class SessionsRepository(AppDbContext _context) : ISessionsRepository
    {
        public async Task AddSessionAsync(MovieSession session)
        {
            try
            {
                await _context.Sessions.AddAsync(session);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteSession(MovieSession session)
        {
            try
            {
                _context.Sessions.Remove(session);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<MovieSession?> GetOneAsync(int id)
        {
            return await _context.Sessions.Include(s=> s.Movie)
               .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Tuple<List<MovieSession>, int>> GetPagedSessionsAsync(Expression<Func<MovieSession, bool>> predicate, int page, int size, string sort)
        {
            var query = _context.Sessions.Include(session => session.Movie).Where(predicate);
            var totalSize = query.Count();
            query = sort?.ToLowerInvariant() switch
            {
                "soon" => query.OrderBy(s => s.StartsAt).ThenBy(s => s.Id),
                "rating" => query.OrderByDescending(s => s.Movie.Rating)
                                  .ThenByDescending(s => s.Movie.ReleaseYear)
                                  .ThenBy(s => s.Id),
                _ => query.OrderBy(s => s.Movie.Title).ThenBy(m => m.Id)
            };
            query = query.Skip((page - 1) * size).Take(size);
            return new(await query.ToListAsync(), totalSize);
        }

        public async Task<bool> SaveChangesAsync()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0 ? true : false;
        }

        public async Task UpdateSessionAsync(MovieSession session)
        {
            try
            {
                var oldSession = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == session.Id);
                if (oldSession != null)
                {
                   oldSession.StartsAt = session.StartsAt;
                   oldSession.HallName = session.HallName;
                   oldSession.TicketPrice = session.TicketPrice;
                   oldSession.AvailableSeats = session.AvailableSeats;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
