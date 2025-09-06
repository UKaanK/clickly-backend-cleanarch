using Clickly.Infrastructure.ServiceRegistration;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

//API katman� �zg� servisler eklenecek
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

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
app.Run();
