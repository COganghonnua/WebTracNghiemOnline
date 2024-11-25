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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserOnlineRoom>()
                .HasOne(uor => uor.User)
                .WithMany(u => u.UserOnlineRooms)
                .HasForeignKey(uor => uor.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa cascade

            modelBuilder.Entity<UserOnlineRoom>()
                .HasOne(uor => uor.OnlineRoom)
                .WithMany(or => or.UserOnlineRooms)
                .HasForeignKey(uor => uor.OnlineRoomId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa cascade

            modelBuilder.Entity<OnlineRoom>()
                .HasOne(or => or.HostUser)
                .WithMany(u => u.CreatedRooms)
                .HasForeignKey(or => or.HostUserId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa cascade

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
