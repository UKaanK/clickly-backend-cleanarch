using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Application.Features.GetStats
{
    public class GetStatsQuery:IRequest<GetStatsResponse>
    {
        public string ShortCode { get; set; } = string.Empty;
    }

    public class GetStatsResponse
    {
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public int TotalClicks { get; set; }
        public int UniqueVisitors { get; set; }
        public double? AverageTimeSpent { get; set; }
        public Dictionary<string, int> DailyClicks { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> Referrers { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> Countries { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> Devices { get; set; } = new Dictionary<string, int>();
    }
}
