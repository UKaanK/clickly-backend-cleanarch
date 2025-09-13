using Clickly.Infrastructure.Persistence;
using Clickly.Infrastructure.ServiceRegistration;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UAParser;

var builder = WebApplication.CreateBuilder(args);



//Serilog yap�land�rmas�
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


//API katman� �zg� servisler eklenecek
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Clickly.Api", Version = "v1" });
    c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = "http://127.0.0.1:8080" });
}
    );

//D��er katman servisleri ekleneck
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();


//Ger�ek IP adresini almak i�in Forwarded Headers'� ekle
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

// URL k�saltma end-point'i i�in y�nlendirme (URL redirection)
app.MapGet("/{shortCode}", async (string shortCode, IMediator mediator,HttpContext httpContext) =>
{// Kullan�c�ya ait verileri HTTP ba�lam�ndan al�yoruz
    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
    var userAgentHeader = httpContext.Request.Headers["User-Agent"].ToString();
    var referrerHeader = httpContext.Request.Headers["Referer"].ToString();

    // User-Agent'i ayr��t�rma (�rnek bir yakla��md�r)
    var uaParser = Parser.GetDefault();
    ClientInfo clientInfo = uaParser.Parse(userAgentHeader);

    // Daha sonra IP adresi i�in GeoLite2 gibi bir servis eklenebilir.
    // �imdilik bo� b�rak�yoruz.
    var country = "";
    var city = "";

    var query = new Clickly.Application.Features.GetUrl.GetUrlQuery
    {
        ShortCode = shortCode,
        IpAddress = ipAddress,
        UserAgent = userAgentHeader,
        Referrer = referrerHeader,
        // Ayr��t�r�lm�� User-Agent bilgilerini sorguya ekle
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
//Serilog loglamas�
app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
app.Run();
