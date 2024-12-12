using System;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF
{
    public class PersonContext : DbContext
    {
        private readonly string _connectionString;

        public PersonContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonChild> PersonChildRelations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonChild>()
                .HasKey(pc => new { pc.ParentId, pc.ChildId });

            modelBuilder.Entity<PersonChild>()
                .HasOne(pc => pc.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(pc => pc.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersonChild>()
                .HasOne(pc => pc.Child)
                .WithMany(p => p.Parents)
                .HasForeignKey(pc => pc.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Spouse)
                .WithOne()
                .HasForeignKey<Person>(p => p.SpouseId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
