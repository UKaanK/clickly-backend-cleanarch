using Clickly.Application.Interfaces;
using Clickly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Infrastructure.Persistence.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly ApplicationDbContext _context;

        public UrlRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }


        public async Task AddAsync(Url url)
        {
            await _context.Urls.AddAsync(url);
            await _context.SaveChangesAsync();
        }

        public async Task<Url> GetByShortCodeAsync(string shortCode)
        {
            return await _context.Urls.Include(u => u.Clicks).FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        }

        public async Task UpdateAsync(Url url)
        {
            _context.Update(url);
            await _context.SaveChangesAsync();
        }
    }
}
