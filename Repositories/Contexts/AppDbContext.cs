
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.RoleSeeding;
using Repositories.RoleSeeding.SeedConfiguration;
using System;
using System.Collections.Generic;
using System.Text;


namespace Repositories.Contexts
{

    public class AppDbContext : IdentityDbContext<User, Role, Guid>
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

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

            //user
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
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
