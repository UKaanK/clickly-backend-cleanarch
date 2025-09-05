using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Domain.Entities
{
    public class Url
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int ClickCount { get; set; } = 0;
        public int UniqueVisitorCount { get; set; } = 0;

        public virtual ICollection<Click> Clicks { get; set; } = new List<Click>();
    }
}
