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

        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Важно вызывать базовый метод для Identity

            // Если Authorization должен иметь ключ
            modelBuilder.Entity<Authorization>()
                .HasNoKey(); // Предполагая, что в классе Authorization есть свойство Id

            modelBuilder.Entity<Registration>()
                .HasNoKey();

            modelBuilder.Entity<Request>()
                .HasNoKey();
            // ИЛИ если Authorization должен быть без ключа
            // modelBuilder.Entity<Authorization>().HasNoKey();

            // Дополнительные конфигурации для других сущностей, если нужно
        }
    }
}