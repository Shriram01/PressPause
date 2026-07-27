using PressPause.Api.Endpoints.Games;
using PressPause.Application;
using PressPause.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCreateGameEndpoint();

app.MapGetGamesEndpoint();

app.MapGetGameByIdEndpoint();

app.MapUpdateGameEndpoint();

app.MapDeleteGameEndpoint();

app.Run();