using Application.Services;
using Application.Services.Interfaces;
using Repository;
using Repository.Repositories;
using Repository.Repositories.Conversations;
using Repository.Repositories.Matches;
using Repository.Repositories.Messages;
using Repository.Repositories.Notifications;
using Repository.Repositories.Reviews;
using Repository.Repositories.Students;
using Repository.Repositories.SwipeActions;
using Repository.Repositories.Tutors;
using Repository.Repositories.Users;  
using Repository.UnitOfWork;
using Service.Firebase;
using Service.Mapping;
using Service.Services.Auth;
using Service.Services.Certifications;
using Service.Services.Chat;
using Service.Services.Conversations;
using Service.Services.Jwt;
using Service.Services.Locations;
using Service.Services.Matches;
using Service.Services.Matching;
using Service.Services.Messages;
using Service.Services.Notifications;
using Service.Services.Reviews;
using Service.Services.Students;
using Service.Services.Subjects;
using Service.Services.SwipeActions;
using Service.Services.Tutors;
using Service.Services.TutorSubjects;
using Service.Services.Users;        

namespace EKE_Backend
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
           //Repo
            services.AddDbContext<ApplicationDbContext>();
            services.AddAutoMapper(typeof(UserMappingProfile));
           
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ITutorRepository, TutorRepository>();

            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ITutorSubjectService, TutorSubjectService>();
            services.AddScoped<ICertificationService, CertificationService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ISwipeActionRepository, SwipeActionRepository>();
            services.AddScoped<IMatchRepository,MatchRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();    
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ISwipeActionRepository, SwipeActionRepository>();

            //Service
            services.AddScoped<ITutorService, TutorService>();

            services.AddScoped<IBookingService, BookingService>();  
            services.AddScoped<IMatchService, MatchService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITutorService, TutorService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();         
            services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ITutorSubjectService, TutorSubjectService>();
            services.AddScoped<ICertificationService, CertificationService>();
            services.AddScoped<ILocationService, LocationService>();
            
            // Matching and Chat services
            services.AddScoped<IMatchingService, MatchingService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ISwipeActionService, SwipeActionService>();
            return services;
        }
    }
}