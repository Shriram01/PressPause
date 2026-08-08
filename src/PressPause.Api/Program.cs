using PressPause.Api.Endpoints.Authentication;
using PressPause.Api.Endpoints.Games;
using PressPause.Api.Endpoints.Genres;
using PressPause.Api.Endpoints.Platforms;
using PressPause.Api.Extensions;
using PressPause.Application;
using PressPause.Infrastructure;
using PressPause.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PressPause.Api.Endpoints.Developers;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Service Registration
// -----------------------------------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.SecretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// -----------------------------------------------------------------------------
// Middleware
// -----------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseGlobalExceptionHandling();

// -----------------------------------------------------------------------------
// Authentication Endpoints
// -----------------------------------------------------------------------------

app.MapRegisterEndpoint();
app.MapLoginEndpoint();

// -----------------------------------------------------------------------------
// Game Endpoints
// -----------------------------------------------------------------------------

app.MapCreateGameEndpoint();
app.MapGetGamesEndpoint();
app.MapGetGameByIdEndpoint();
app.MapUpdateGameEndpoint();
app.MapDeleteGameEndpoint();

// -----------------------------------------------------------------------------
// Platform Endpoints
// -----------------------------------------------------------------------------

app.MapCreatePlatformEndpoint();
app.MapGetPlatformsEndpoint();
app.MapGetPlatformByIdEndpoint();
app.MapUpdatePlatformEndpoint();
app.MapDeletePlatformEndpoint();

// -----------------------------------------------------------------------------
// Genre Endpoints
// -----------------------------------------------------------------------------

app.MapCreateGenreEndpoint();
app.MapGetGenresEndpoint();
app.MapGetGenreByIdEndpoint();
app.MapUpdateGenreEndpoint();
app.MapDeleteGenreEndpoint();

// -----------------------------------------------------------------------------
// Developer Endpoints
// -----------------------------------------------------------------------------

app.MapCreateDeveloperEndpoint();
app.MapGetDeveloperByIdEndpoint();
app.MapGetDeveloperByIdEndpoint();
app.MapUpdateDeveloperEndpoint();
app.MapDeleteDeveloperEndpoint();


app.Run();