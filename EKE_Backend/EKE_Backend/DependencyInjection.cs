using Repository;
using Repository.Repositories.Users;  
using Service.Services.Users;        
using Service.Services.Jwt;           
using Service.Mapping;
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Repository.UnitOfWork;
using Service.Services.Auth;
using Repository.Repositories;
using Application.Services.Interfaces;
using Application.Services;

namespace EKE_Backend
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddDbContext<ApplicationDbContext>();
            services.AddAutoMapper(typeof(UserMappingProfile));
           
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ITutorRepository, TutorRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();


            services.AddScoped<IBookingService, BookingService>();  
            services.AddScoped<IMatchService, MatchService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IStudentService, StudentService>();
            //services.AddScoped<ITutorService, TutorService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}