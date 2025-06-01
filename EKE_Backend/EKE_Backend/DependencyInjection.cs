


namespace EKE_Backend
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //// Đăng ký các repository chung
            //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //// Đăng ký UnitOfWork
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Đăng ký các Service
 

            // Đăng ký các Repository
           
            return services;
        }
    }
}
