using System.Collections.Generic;
using System.Reflection.Emit;
using BulkyWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRezor_Temp.Data
{
    public class ApplicationDBContext_Razor : DbContext
    {
        public ApplicationDBContext_Razor(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );
        }
    }
}
