using PressPause.Api.Endpoints.Authentication;
using PressPause.Api.Endpoints.Games;
using PressPause.Api.Endpoints.Genres;
using PressPause.Api.Endpoints.Platforms;
using PressPause.Api.Extensions;
using PressPause.Api.Endpoints.Developers;
using PressPause.Api.Endpoints.Publishers;
using PressPause.Api.Endpoints.Stores;
using PressPause.Api.Endpoints.Tags;
using PressPause.Api.Endpoints.UserLibrary;
using PressPause.Api.Endpoints.Users;
using PressPause.Application;
using PressPause.Infrastructure;
using PressPause.Infrastructure.Authentication;
using PressPause.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Service Registration
// -----------------------------------------------------------------------------

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

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

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/api/test/admin", () => Results.Ok())
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
}

if (!app.Environment.IsEnvironment("Testing"))
{
    await IdentityRoleSeeder.SeedAsync(app.Services);
}


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
app.MapGetDevelopersEndpoint();
app.MapUpdateDeveloperEndpoint();
app.MapDeleteDeveloperEndpoint();

// -----------------------------------------------------------------------------
// Publisher Endpoints
// -----------------------------------------------------------------------------

app.MapCreatePublisherEndpoint();
app.MapGetPublisherByIdEndpoint();
app.MapGetPublishersEndpoint();
app.MapUpdatePublisherEndpoint();
app.MapDeletePublisherEndpoint();

// -----------------------------------------------------------------------------
// Store Endpoints
// -----------------------------------------------------------------------------

app.MapCreateStoreEndpoint();
app.MapGetStoreByIdEndpoint();
app.MapGetStoresEndpoint();
app.MapUpdateStoreEndpoint();
app.MapDeleteStoreEndpoint();

// -----------------------------------------------------------------------------
// Tag Endpoints
// -----------------------------------------------------------------------------

app.MapCreateTagEndpoint();
app.MapGetTagByIdEndpoint();
app.MapGetTagsEndpoint();
app.MapUpdateTagEndpoint();
app.MapDeleteTagEndpoint();

// -----------------------------------------------------------------------------
// User Endpoints
// -----------------------------------------------------------------------------

app.MapGetCurrentUserEndpoint();
app.MapUserProfileEndpoints();
app.MapUserLibraryEndpoints();


app.Run();

public partial class Program;