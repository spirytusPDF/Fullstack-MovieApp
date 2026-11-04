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
        public bool AddComment(MovieComment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                var res = _context.SaveChanges();
                return res > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteComment(MovieComment comment)
        {
            try
            {
                _context.Comments.Remove(comment);
                var res = _context.SaveChanges();
                return res > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<MovieComment> GetByMovie(int movieId)
        {
           return _context.Comments.AsNoTracking().Include(c=>c.user).Where(m=>m.MovieId == movieId).ToList();
        }

        public List<MovieComment> GetByUser(int userId)
        {
            return _context.Comments.AsNoTracking().Include(c => c.movie).Where(m => m.UserId == userId).ToList();
        }

        public MovieComment GetOne(int id)
        {
            return _context.Comments.FirstOrDefault(c => c.Id == id);
        }
    }
}
