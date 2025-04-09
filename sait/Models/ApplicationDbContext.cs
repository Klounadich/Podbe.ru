using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sait.Models;

namespace sait.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Важно вызывать базовый метод для Identity

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                // Игнорируем ненужные столбцы
                entity.Ignore(u => u.PhoneNumber);
                entity.Ignore(u => u.PhoneNumberConfirmed);
                entity.Ignore(u => u.TwoFactorEnabled);
                entity.Ignore(u => u.LockoutEnd);
                entity.Ignore(u => u.LockoutEnabled);
                entity.Ignore(u => u.AccessFailedCount);

                // Переименовываем таблицу (опционально)
                
            });

            // Если Authorization должен иметь ключ
            modelBuilder.Entity<Authorization>()
                .HasNoKey(); 

            modelBuilder.Entity<Registration>()
                .HasNoKey();

            modelBuilder.Entity<Request>()
                .HasNoKey();

            

        }
    }
}