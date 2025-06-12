using Repository;
using Repository.Repositories.Users;  
using Service.Services.Users;        
using Service.Services.Jwt;           
using Service.Mapping;
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Repository.UnitOfWork;

namespace EKE_Backend
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddDbContext<ApplicationDbContext>();
            services.AddAutoMapper(typeof(UserMappingProfile));
           
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentService, StudentRepository>();
            services.AddScoped<ITutorRepository, TutorRepository>();



            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IStudentService, StudentService>();
            //services.AddScoped<ITutorService, TutorService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}