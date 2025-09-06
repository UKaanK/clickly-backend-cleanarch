using Clickly.Application.Interfaces;
using Clickly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Infrastructure.Persistence.Repositories
{
    public class ClickRepository : IClickRepository
    {
        private readonly ApplicationDbContext _context;
        public ClickRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Click click)
        {
           await _context.Clicks.AddAsync(click);
            await _context.SaveChangesAsync();
        }
    }
}
