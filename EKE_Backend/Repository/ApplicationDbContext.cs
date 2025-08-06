// Repository/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.Entities;
using Repository.Enums;
using Repository.UnitOfWork;

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
        public DbSet<Student> Students { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TutorSubject> TutorSubjects { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<SwipeAction> SwipeActions { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AiChatSession> AiChatSessions { get; set; }
        public DbSet<AiChatMessage> AiChatMessages { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<PayOSWebhook> PayOSWebhooks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.ProfileImage).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.District).HasMaxLength(100);
            });

            // Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Student)
                      .HasForeignKey<Student>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.SchoolName).HasMaxLength(255);
                entity.Property(e => e.GradeLevel).HasMaxLength(50);
                entity.Property(e => e.BudgetMin).HasPrecision(10, 2);
                entity.Property(e => e.BudgetMax).HasPrecision(10, 2);
            });

            // Tutor Configuration
            modelBuilder.Entity<Tutor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Tutor)
                      .HasForeignKey<Tutor>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.EducationLevel).HasMaxLength(100);
                entity.Property(e => e.University).HasMaxLength(255);
                entity.Property(e => e.Major).HasMaxLength(255);
                entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
                entity.Property(e => e.AverageRating).HasPrecision(3, 2);
            });

            // Subject Configuration
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Icon).HasMaxLength(255);
            });

            // TutorSubject Configuration
            modelBuilder.Entity<TutorSubject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.TutorId, e.SubjectId }).IsUnique();

                entity.HasOne(e => e.Tutor)
                      .WithMany(t => t.TutorSubjects)
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Subject)
                      .WithMany(s => s.TutorSubjects)
                      .HasForeignKey(e => e.SubjectId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Certification Configuration
            modelBuilder.Entity<Certification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Issuer).HasMaxLength(255);
                entity.Property(e => e.CertificateUrl).HasMaxLength(500);

                entity.HasOne(e => e.Tutor)
                      .WithMany(t => t.Certifications)
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SwipeAction Configuration
            modelBuilder.Entity<SwipeAction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.StudentId, e.TutorId }).IsUnique();

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.SwipeActions)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tutor)
                      .WithMany()
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Match Configuration
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.StudentId, e.TutorId }).IsUnique();

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.MatchesAsStudent)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tutor)
                      .WithMany(t => t.MatchesAsTutor)
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Conversation Configuration - COMPLETELY FIXED
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Match)
                      .WithMany(m => m.Conversations)
                      .HasForeignKey(e => e.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Store LastMessageId as a simple field, not as foreign key constraint
                entity.Property(e => e.LastMessageId);
                entity.Property(e => e.LastMessageAt);
            });

            // Message Configuration
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileUrl).HasMaxLength(500);

                entity.HasOne(e => e.Conversation)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(e => e.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Sender)
                      .WithMany(u => u.Messages)
                      .HasForeignKey(e => e.SenderId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Review Configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.TutorId, e.StudentId }).IsUnique();

                entity.HasOne(e => e.Tutor)
                      .WithMany(t => t.ReviewsReceived)
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Reviews)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.Property(e => e.Rating)
                      .HasAnnotation("Range", new[] { 1, 5 });
            });

            // Booking Configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
                entity.Property(e => e.LocationAddress).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.BookingsAsStudent)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tutor)
                      .WithMany(t => t.BookingsAsTutor)
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Subject)
                      .WithMany(s => s.Bookings)
                      .HasForeignKey(e => e.SubjectId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Data).HasColumnType("nvarchar(max)");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AiChatSession Configuration
            modelBuilder.Entity<AiChatSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.SessionId).IsUnique();
                entity.Property(e => e.SessionId).HasMaxLength(255).IsRequired();

                entity.HasOne(e => e.User)
                      .WithMany(u => u.AiChatSessions)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AiChatMessage Configuration
            modelBuilder.Entity<AiChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired();

                entity.HasOne(e => e.Session)
                      .WithMany(s => s.AiChatMessages)
                      .HasForeignKey(e => e.SessionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AppSetting Configuration
            modelBuilder.Entity<AppSetting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.KeyName).IsUnique();
                entity.Property(e => e.KeyName).HasMaxLength(255).IsRequired();
            });

            // Wallet Configuration
            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Wallet)
                      .HasForeignKey<Wallet>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PaymentTransaction Configuration
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderCode).IsUnique();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.PaymentTransactions)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PayOSWebhook Configuration
            modelBuilder.Entity<PayOSWebhook>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Payload).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Status).HasMaxLength(50);
            });


            // Add Indexes for Performance
            modelBuilder.Entity<User>().HasIndex(e => e.Role);
            modelBuilder.Entity<Tutor>().HasIndex(e => e.AverageRating);
            modelBuilder.Entity<Tutor>().HasIndex(e => e.HourlyRate);
            modelBuilder.Entity<SwipeAction>().HasIndex(e => e.StudentId);
            modelBuilder.Entity<Match>().HasIndex(e => e.StudentId);
            modelBuilder.Entity<Match>().HasIndex(e => e.TutorId);
            modelBuilder.Entity<Message>().HasIndex(e => new { e.ConversationId, e.CreatedAt });
            modelBuilder.Entity<Notification>().HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt });
            modelBuilder.Entity<Booking>().HasIndex(e => new { e.TutorId, e.StartTime });
            modelBuilder.Entity<Booking>().HasIndex(e => new { e.StudentId, e.Status });
            modelBuilder.Entity<Booking>().HasIndex(e => e.Status);
            modelBuilder.Entity<Review>().HasIndex(e => new { e.TutorId, e.CreatedAt });

            // Add index for LastMessageId lookup
            modelBuilder.Entity<Conversation>().HasIndex(e => e.LastMessageId);

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Subjects
            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Toán học", Code = "MATH", Category = "Khoa học tự nhiên", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { Id = 2, Name = "Vật lý", Code = "PHYSICS", Category = "Khoa học tự nhiên", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { Id = 3, Name = "Hóa học", Code = "CHEMISTRY", Category = "Khoa học tự nhiên", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { Id = 4, Name = "Sinh học", Code = "BIOLOGY", Category = "Khoa học tự nhiên", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { Id = 5, Name = "Văn học", Code = "LITERATURE", Category = "Khoa học xã hội", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { Id = 6, Name = "Tiếng Anh", Code = "ENGLISH", Category = "Ngoại ngữ", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { Id = 7, Name = "Lịch sử", Code = "HISTORY", Category = "Khoa học xã hội", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { Id = 8, Name = "Địa lý", Code = "GEOGRAPHY", Category = "Khoa học xã hội", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            // Seed App Settings
            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting { Id = 1, KeyName = "max_swipes_per_day", Value = "50", Description = "Số lần swipe tối đa mỗi ngày", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AppSetting { Id = 2, KeyName = "min_tutor_rating", Value = "3.0", Description = "Rating tối thiểu để hiển thị gia sư", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AppSetting { Id = 3, KeyName = "super_like_limit", Value = "5", Description = "Số lần super like mỗi ngày", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AppSetting { Id = 4, KeyName = "chat_ai_enabled", Value = "true", Description = "Bật/tắt tính năng chat AI", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

