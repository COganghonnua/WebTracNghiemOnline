using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Access
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamHistory> ExamHistories { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<OnlineRoom> OnlineRooms{ get; set; }
        public DbSet<UserOnlineRoom> UserOnlineRooms { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }

    }
}
