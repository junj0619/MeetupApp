using System;
using MeetupApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        /* Override EF creating table behavior on the database  */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            /* 
                Define N to N relationships on Like Entity 
                1) PrimaryKey(LikerId, LikeeId)
                2) One Likee can have N Likers
                3) One Liker can have N Likees
                4) Restrict deletion behavior that deleting LIKE shouldn't cause cascading deletion on USER 
            */
            builder.Entity<Like>().HasKey(k => new { k.LikerId, k.LikeeId });

            builder.Entity<Like>()
                   .HasOne(u => u.Likee)
                   .WithMany(u => u.Likers)
                   .HasForeignKey(u => u.LikeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                   .HasOne(u => u.Liker)
                   .WithMany(u => u.Likees)
                   .HasForeignKey(u => u.LikerId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}