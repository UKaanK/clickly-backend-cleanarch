using Clickly.Infrastructure.ServiceRegistration;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

//API katmaný özgü servisler eklenecek
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DÝðer katman servisleri ekleneck
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

app.UseHttpsRedirection();

app.UseAuthorization();

// URL kýsaltma end-point'i için yönlendirme (URL redirection)
app.MapGet("/{shortCode}", async (string shortCode, IMediator mediator) =>
{
    var originalUrl = await mediator.Send(new Clickly.Application.Features.GetUrl.GetUrlQuery
    {
        ShortCode = shortCode,
        // HttpContext'ten IP, User-Agent gibi bilgileri alýp buraya ekleyeceðiz
    });

    return Results.Redirect(originalUrl, permanent: false);
});

app.MapControllers();
app.Run();
