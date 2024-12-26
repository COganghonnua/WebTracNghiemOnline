using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Access
{

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        { }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamHistory> ExamHistories { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<OnlineRoom> OnlineRooms{ get; set; }
        public DbSet<UserOnlineRoom> UserOnlineRooms { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseAnswer> ExerciseAnswers { get; set; }
        public DbSet<ExerciseQuestion> ExerciseQuestions { get; set; }
        public DbSet<ExerciseHistory> ExerciseHistories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<ExamHistoryAnswer> ExamHistoryAnswers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Comment>().HasQueryFilter(c => !c.IsDeleted);

            // Nếu muốn áp dụng cho Post, thêm như sau:
            modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);


            // Cấu hình khóa chính cho bảng trung gian
            modelBuilder.Entity<ExamQuestion>()
                .HasKey(eq => new { eq.ExamId, eq.QuestionId });

            // Quan hệ với Exam
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.Cascade); // Chỉ áp dụng cascade cho Exam

            // Quan hệ với Question
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Bỏ cascade cho Question
            modelBuilder.Entity<ExerciseHistory>()
    .HasOne(eh => eh.Exercise)
    .WithMany(e => e.ExerciseHistories)
    .HasForeignKey(eh => eh.ExerciseId)
    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OnlineRoom>()
    .HasIndex(or => or.RoomCode)
    .IsUnique();
            modelBuilder.Entity<UserOnlineRoom>()
                .HasIndex(uor => new { uor.UserId, uor.OnlineRoomId })
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserOnlineRooms)
                .WithOne(uor => uor.User)
                .HasForeignKey(uor => uor.UserId)
                .OnDelete(DeleteBehavior.Cascade);


             modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Không dùng Cascade ở đây

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade chỉ với Post


            modelBuilder.Entity<UserOnlineRoom>()
            .HasIndex(uor => new { uor.UserId, uor.OnlineRoomId })
            .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserOnlineRooms)
                .WithOne(uor => uor.User)
                .HasForeignKey(uor => uor.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OnlineRoom>()
                .HasIndex(or => or.RoomCode)
                .IsUnique();


            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        }

    }
}
