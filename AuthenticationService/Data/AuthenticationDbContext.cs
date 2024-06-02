namespace AuthenticationService.Data
{
    using Microsoft.EntityFrameworkCore;
    using Entities;

    /// <summary>
    /// Клас-контекст для роботи з даними потрібними для автентифікації
    /// </summary>
    public class AuthenticationDbContext : DbContext
    {
        /// <summary>
        /// Повертає екземпляр класу <see cref="AuthenticationDbContext"/>
        /// </summary>
        /// <param name="options">Параметри для контексту бази даних</param>
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Метод створення моделі БД за допомогою ORM 
        /// </summary>
        /// <param name="modelBuilder">Будівельник моделі</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Набір користувачів у базі даних
        /// </summary>
        public DbSet<User> Users { get; set; }
    }
}
