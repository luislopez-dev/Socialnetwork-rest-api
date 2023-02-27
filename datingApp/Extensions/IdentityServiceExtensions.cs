using System.Text;
using datingApp.Data;
using datingApp.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace datingApp.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, 
        IConfiguration config)
    {
        services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accesToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (string.IsNullOrEmpty(accesToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accesToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("AdminRole"));
            opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireClaim("AdminRole", "Moderator"));
        });
        return services;
    }
}