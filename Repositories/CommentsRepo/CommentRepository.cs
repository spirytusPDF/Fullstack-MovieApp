using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.CommentsRepo
{
    public class CommentRepository(AppDbContext context) : ICommentsRepository
    {
        private readonly AppDbContext _context = context;
        public async Task AddCommentAsync(MovieComment comment)
        {
            try
            {
                 await _context.Comments.AddAsync(comment);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
        }

        public async Task DeleteComment(MovieComment comment)
        {
            try
            {
                _context.Comments.Remove(comment);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
        }

        public async Task<List<MovieComment>> GetByMovieAsync(int movieId)
        {
           return await _context.Comments.AsNoTracking().Include(c=>c.user).Where(m=>m.MovieId == movieId).ToListAsync();
        }

        public async Task<List<MovieComment>> GetByUserAsync(int userId)
        {
            return await _context.Comments.AsNoTracking().Include(c => c.movie).Where(m => m.UserId == userId).ToListAsync();
        }

        public Task<MovieComment?> GetOneAsync(int id)
        {
            return _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0 ? true : false;
        }
    }
}
