using Clickly.Application.Interfaces;
using Clickly.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Application.Features.ShortenUrl
{
    public class ShortenUrlHandler:IRequestHandler<ShortenUrlCommand,ShortenUrlResponse>
    {
        private readonly IUrlRepository _urlRepository;
        private readonly ILogger<ShortenUrlHandler> _logger;

        public ShortenUrlHandler(IUrlRepository urlRepository, ILogger<ShortenUrlHandler> logger)
        {
            _urlRepository = urlRepository;
            _logger = logger;
        }

        public async Task<ShortenUrlResponse> Handle(ShortenUrlCommand request, CancellationToken cancellationToken)
        {
            //1. URL'nin geçerli olup olmadığını kontrol et
            if (!Uri.TryCreate(request.OriginalUrl,UriKind.Absolute,out var uriResult) || (uriResult.Scheme!=Uri.UriSchemeHttp && uriResult.Scheme!=Uri.UriSchemeHttps) )
            {
                _logger.LogWarning("Geçersiz URL kısaltma isteği:{OriginalUrl}", request.OriginalUrl);
                //Daha sonra özel bir hata yönetimi 
                throw new ArgumentException("Geçersiz URL formatı.");
            }

            //2. Benzersiz kısa kod oluştur
            string shortCode = GenerateShortCode();

            //3. Url varlığını oluşturun
            var newUrl = new Url {
                OriginalUrl =request.OriginalUrl,
                ShortCode=shortCode,
                CreatedAt=DateTime.UtcNow
            };

            //4. Url'yi veritabanına kaydet
            await _urlRepository.AddAsync(newUrl);

            _logger.LogInformation("URL kısaltıldı: {OriginalUrl} -> {ShortCode}", newUrl.OriginalUrl, newUrl.ShortCode);

            //5. Yanıt oluştur
            string shortUrl = $"http://localhost:5000/{newUrl.ShortCode}";

            return new ShortenUrlResponse
            {
                OriginalUrl=newUrl.OriginalUrl,
                ShortCode=newUrl.ShortCode,
                ShortUrl=shortUrl,
                CreatedAt=newUrl.CreatedAt
            };


        }

        //Rastgale bir kısa kod oluşturma fonksiyonu
        private string GenerateShortCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
