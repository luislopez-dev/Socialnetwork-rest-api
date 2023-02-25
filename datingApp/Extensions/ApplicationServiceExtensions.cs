using datingApp.Config;
using datingApp.Data;
using datingApp.Helpers;
using datingApp.Interfaces;
using datingApp.Services;
using Microsoft.EntityFrameworkCore;
namespace datingApp.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlServer((config.GetConnectionString("datingApp")));
        });
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        
        return services;
    }
}