using Clickly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Infrastructure.Persistence
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }

        public DbSet<Url> Urls { get; set; }
        public DbSet<Click> Clicks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Urls tablosundaki ShortCode kolonunun benzersiz olmasını sağlıyoruz
            modelBuilder.Entity<Url>().HasIndex(u => u.ShortCode).IsUnique();
            //Url ve Click arasında bire çok ilişki(one-to-many)
            modelBuilder.Entity<Url>().HasMany(u => u.Clicks).WithOne(c => c.Url).HasForeignKey(c => c.UrlId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
