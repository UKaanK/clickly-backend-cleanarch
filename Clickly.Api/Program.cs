using Clickly.Infrastructure.Persistence;
using Clickly.Infrastructure.ServiceRegistration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
app.MapGet("/{shortCode}", async (string shortCode, IMediator mediator) =>
{
    var originalUrl = await mediator.Send(new Clickly.Application.Features.GetUrl.GetUrlQuery
    {
        ShortCode = shortCode,
        // HttpContext'ten IP, User-Agent gibi bilgileri al�p buraya ekleyece�iz
    });

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
