using Clickly.Application.Interfaces;
using Clickly.Infrastructure.Persistence;
using Clickly.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Infrastructure.ServiceRegistration
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services,IConfiguration configuration)
        {
            //veritabanı bağlantı dizesini appsettings.json dosyasından alıyoruz
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //PostgreSQL için DbContext'i DI container'ına ekleyin
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            //Repository'leri DI container'ıan ekleyin
            services.AddScoped<IUrlRepository, UrlRepository>();
            services.AddScoped<IClickRepository, ClickRepository>();

            return services;
        }
    }
}
