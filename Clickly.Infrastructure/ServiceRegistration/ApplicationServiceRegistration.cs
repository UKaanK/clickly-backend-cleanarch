using Clickly.Application.Features.ShortenUrl;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clickly.Infrastructure.ServiceRegistration
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //MediatR,bu projedeki tüm IRequest/IRequestHandler uygulamalarını tarayarak otomatik olarak kaydet.
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ShortenUrlCommand).Assembly);
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            return services;
        }
    }
}
