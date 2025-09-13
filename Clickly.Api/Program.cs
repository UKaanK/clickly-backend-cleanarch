using Clickly.Infrastructure.Persistence;
using Clickly.Infrastructure.ServiceRegistration;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UAParser;

var builder = WebApplication.CreateBuilder(args);



//Serilog yapýlandýrmasý
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
                      .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


//API katmaný özgü servisler eklenecek
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Clickly.Api", Version = "v1" });
    c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = "http://127.0.0.1:8080" });
}
    );

//DÝðer katman servisleri ekleneck
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();


//Gerçek IP adresini almak için Forwarded Headers'ý ekle
app.UseForwardedHeaders(new ForwardedHeadersOptions{
    ForwardedHeaders=ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseAuthorization();

// URL kýsaltma end-point'i için yönlendirme (URL redirection)
app.MapGet("/{shortCode}", async (string shortCode, IMediator mediator,HttpContext httpContext) =>
{// Kullanýcýya ait verileri HTTP baðlamýndan alýyoruz
    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
    var userAgentHeader = httpContext.Request.Headers["User-Agent"].ToString();
    var referrerHeader = httpContext.Request.Headers["Referer"].ToString();

    // User-Agent'i ayrýþtýrma (örnek bir yaklaþýmdýr)
    var uaParser = Parser.GetDefault();
    ClientInfo clientInfo = uaParser.Parse(userAgentHeader);

    // Daha sonra IP adresi için GeoLite2 gibi bir servis eklenebilir.
    // Þimdilik boþ býrakýyoruz.
    var country = "";
    var city = "";

    var query = new Clickly.Application.Features.GetUrl.GetUrlQuery
    {
        ShortCode = shortCode,
        IpAddress = ipAddress,
        UserAgent = userAgentHeader,
        Referrer = referrerHeader,
        // Ayrýþtýrýlmýþ User-Agent bilgilerini sorguya ekle
        DeviceType = clientInfo.Device.Family,
        Browser = clientInfo.UA.Family,
        OperatingSystem = clientInfo.OS.Family,
        Country = country,
        City = city
    };

    var originalUrl = await mediator.Send(query);

    return Results.Redirect(originalUrl, permanent: false);
});

app.MapControllers();
//Serilog loglamasý
app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
app.Run();
