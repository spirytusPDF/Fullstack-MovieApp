
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Contexts
{

    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieComment> Comments { get; set; }
        public DbSet<MovieSession> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            //roles
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
               Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
               Name = "Admin",
               NormalizedName = "ADMIN",
               ConcurrencyStamp = "11111111-1111-1111-1111-111111111111"
                //IdentityRole<Guid> automatically
                //sets
                //ConcurrencyStamp = Guid.NewGuid().ToString()
                //inside its constructor whenever I write
                //new IdentityRole<Guid>().
            },
            new IdentityRole<Guid>
            {
               Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
               Name = "User",
               NormalizedName = "USER",
               ConcurrencyStamp = "22222222-2222-2222-2222-222222222222"
            }
             );

            //user
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            //movieSession
            modelBuilder.Entity<MovieSession>()
            .HasOne(m=>m.Movie)
            .WithMany(m=>m.Sessions)
            .HasForeignKey(m=>m.MovieId)
            .OnDelete(DeleteBehavior.Cascade);



            //comment
            modelBuilder.Entity<MovieComment>()
            .HasOne(c => c.movie)
            .WithMany(m => m.MovieComments)
            .HasForeignKey(c => c.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieComment>()
            .HasOne(c => c.user)
            .WithMany(u => u.MovieComments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            

        }

    }

}
