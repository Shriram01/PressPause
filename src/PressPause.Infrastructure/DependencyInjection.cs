using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PressPause.Infrastructure.Persistence;
using PressPause.Application.Common.Interfaces;
using PressPause.Infrastructure.Persistence.Repositories;

namespace PressPause.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PressPauseDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IGameRepository, GameRepository>();

        return services;
    }
}