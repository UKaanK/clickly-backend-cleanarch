using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Application.Features.ShortenUrl
{
    public class ShortenUrlCommand:IRequest<ShortenUrlResponse>
    {
        public string OriginalUrl { get; set; } = string.Empty;
    }

    public class ShortenUrlResponse
    {
        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
