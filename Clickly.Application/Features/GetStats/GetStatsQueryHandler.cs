using Clickly.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Application.Features.GetStats
{
    public class GetStatsQueryHandler : IRequestHandler<GetStatsQuery, GetStatsResponse>
    {
        private readonly IUrlRepository _urlRepository;
        // İstatistikler için ek metotlar IUrlRepository'de bulunabilir veya buraya yeni bir servis eklenebilir.
        // Şimdilik Click nesnelerini sorgulayarak yapacağız.
        private readonly ILogger<GetStatsQueryHandler> _logger;

        public GetStatsQueryHandler(IUrlRepository urlRepository, ILogger<GetStatsQueryHandler> logger)
        {
            _urlRepository = urlRepository;
            _logger = logger;
        }

        public async Task<GetStatsResponse> Handle(GetStatsQuery request, CancellationToken cancellationToken)
        {
            // URL'yi tıklamalarıyla birlikte alın
            var url = _urlRepository.GetByShortCodeAsync(request.ShortCode);

            if (url == null)
            {
                _logger.LogWarning("Kısa kod ile URL bulunamadı: {ShortCode}", request.ShortCode);
                throw new KeyNotFoundException("Kısa Kod Bulunamadı");

            }

            var clicks = url.Result.Clicks;
            // İstatistikleri hesaplayın
            var totalClicks = clicks.Count;
            var uniqueVisitors = clicks.Select(c => c.IpAddress).Distinct().Count();
            var averageTimeSpent = clicks.Where(c => c.TimeSpentSeconds.HasValue).Select(c => c.TimeSpentSeconds.Value).DefaultIfEmpty(0).Average();

            var dailyClicks = clicks.GroupBy(c => c.ClickedAt.Date)
                                    .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Count());

            var referrers = clicks.Where(c => c.Referrer != null)
                                  .GroupBy(c => c.Referrer)
                                  .ToDictionary(g => g.Key!, g => g.Count());

            var countries = clicks.Where(c => c.Country != null)
                                  .GroupBy(c => c.Country)
                                  .ToDictionary(g => g.Key!, g => g.Count());

            var devices = clicks.Where(c => c.DeviceType != null)
                                .GroupBy(c => c.DeviceType)
                                .ToDictionary(g => g.Key!, g => g.Count());

            // Yanıt nesnesini oluşturun
            return new GetStatsResponse
            {
                ShortUrl = $"http://localhost:5000/{url.Result.ShortCode}",
                OriginalUrl = url.Result.OriginalUrl,
                TotalClicks = totalClicks,
                UniqueVisitors = uniqueVisitors,
                AverageTimeSpent = averageTimeSpent,
                DailyClicks = dailyClicks,
                Referrers = referrers,
                Countries = countries,
                Devices = devices,
            };
        }
    }
}
