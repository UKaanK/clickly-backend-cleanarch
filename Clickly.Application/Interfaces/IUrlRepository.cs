using Clickly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Application.Interfaces
{
    public interface IUrlRepository
    {
        Task<Url> GetByShortCodeAsync(string shortCode);
        Task AddAsync(Url url);
        Task UpdateAsync(Url url);
    }
}
