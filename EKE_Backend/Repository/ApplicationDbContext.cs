using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.Entities;
using System.IO;

namespace Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Cấu hình chỉ khi chưa được thiết lập từ ngoài
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }

        private string GetConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'DefaultConnection' trong appsettings.json.");
            }

            return connectionString;
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<TutorProfile> TutorProfiles { get; set; }
        public DbSet<TeachingSubject> TeachingSubjects { get; set; }
        public DbSet<ClassRequest> ClassRequests { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MatchSwipe> MatchSwipes { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MatchSwipe: Student - Tutor
            modelBuilder.Entity<MatchSwipe>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchSwipe>()
                .HasOne(s => s.Tutor)
                .WithMany()
                .HasForeignKey(s => s.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // MatchResult: Student - Tutor
            modelBuilder.Entity<MatchResult>()
                .HasOne(m => m.Student)
                .WithMany()
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchResult>()
                .HasOne(m => m.Tutor)
                .WithMany()
                .HasForeignKey(m => m.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TutorProfile: one-to-one User
            modelBuilder.Entity<TutorProfile>()
                .HasOne(t => t.User)
                .WithOne(u => u.TutorProfile)
                .HasForeignKey<TutorProfile>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentProfile: one-to-many User
            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.StudentProfiles)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ClassRequest: optional Tutor
            modelBuilder.Entity<ClassRequest>()
                .HasOne(c => c.Tutor)
                .WithMany()
                .HasForeignKey(c => c.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review: Reviewer (User) - ClassRequest
            modelBuilder.Entity<Review>()
            .HasOne(r => r.Class)
            .WithMany()
             .HasForeignKey(r => r.ClassId)
            .OnDelete(DeleteBehavior.Restrict);  // hoặc DeleteBehavior.NoAction

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
