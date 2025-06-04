using Repository;
using Repository.Repositories.Users;  
using Service.Services.Users;        
using Service.Services.Jwt;           
using Service.Mapping;

namespace EKE_Backend
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddDbContext<ApplicationDbContext>();
            services.AddAutoMapper(typeof(UserMappingProfile));
           
            services.AddScoped<IUserRepository, UserRepository>();
           
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}