using Microsoft.AspNetCore.Identity;
using PressPause.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PressPause.Infrastructure.Persistence;
using PressPause.Application.Common.Interfaces;
using PressPause.Infrastructure.Persistence.Repositories;
using PressPause.Infrastructure.Authentication;
using PressPause.Application.Authentication.Interfaces;
using FluentValidation;
using MediatR;
using PressPause.Application.Common.Behaviors;
using IdentityService = PressPause.Application.Authentication.Interfaces.IIdentityService;

namespace PressPause.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IGameRepository, GameRepository>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IdentityService, Identity.IdentityService>();

        services.AddScoped<IPlatformRepository, PlatformRepository>();

        services.AddScoped<IGenreRepository, GenreRepository>();

        services.AddScoped<IDeveloperRepository, DeveloperRepository>();

        services.AddScoped<IPublisherRepository, PublisherRepository>();

        services.AddScoped<IStoreRepository, StoreRepository>();

        services.AddScoped<ITagRepository, TagRepository>();

        return services;
    }
}