using Clickly.Application.Interfaces;
using Clickly.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Application.Features.GetUrl
{
    public class GetUrlQueryHandler : IRequestHandler<GetUrlQuery, string>
    {
        private readonly IUrlRepository _urlRepository;
        private readonly ILogger<GetUrlQueryHandler> _logger;
        private readonly IClickRepository _clickRepository;

        public GetUrlQueryHandler(IUrlRepository urlRepository,ILogger<GetUrlQueryHandler> logger,IClickRepository clickRepository)
        {
            _urlRepository = urlRepository;
            _logger = logger;
            _clickRepository = clickRepository;
        }
        public async Task<string> Handle(GetUrlQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kısa kod sorgusu başlatıldı:Kısa Kod:{ShortCode}", request.ShortCode);

            //1. Kısa kodla URL'yi veritabanından al
            var url = await _urlRepository.GetByShortCodeAsync(request.ShortCode);

            if (url==null || !url.IsActive || (url.ExpiresAt.HasValue && url.ExpiresAt<DateTime.UtcNow))
            {
                _logger.LogWarning("Geçersiz veya süresi dolmuş kısa kod denemesi: {ShortCode}", request.ShortCode);
                //Özel Exception Daha Mantıklı olabilir
                throw new KeyNotFoundException("Kısa kod bulunamadı veya etkin değil");
            }


            //2. Yeni bir tıklama kaydı oluştur
            var click = new Click
            {
                UrlId=url.Id,
                IpAddress=request.IpAddress,
                UserAgent=request.UserAgent,
                Referrer=request.Referrer,
                ClickedAt=DateTime.UtcNow,
                
            };

            //3. Tıklama kaydını veritabanına ekleyin
            await _clickRepository.AddAsync(click);

            //4. Url nesnesinin tıklama sayacını artırın ve güncelleyin
            url.ClickCount += 1;
            //UniqueVisitorCount için Ip adresi kontrolü yapılabilir
            await _urlRepository.UpdateAsync(url);

            _logger.LogInformation("URL yönlendirme: {ShortCode} -> {OriginalUrl}", url.ShortCode, url.OriginalUrl);

            return url.OriginalUrl;
        }
    }
}
