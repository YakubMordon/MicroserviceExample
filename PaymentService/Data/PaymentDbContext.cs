namespace PaymentService.Data
{
    using Microsoft.EntityFrameworkCore;
    using Entities;

    /// <summary>
    /// Клас-контекст для роботи з даними потрібними для оплати арендування
    /// </summary>
    public class PaymentDbContext : DbContext
    {
        /// <summary>
        /// Повертає екземпляр класу <see cref="PaymentDbContext"/>
        /// </summary>
        /// <param name="options">Параметри для контексту бази даних</param>
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options){ }

        /// <summary>
        /// Повертає екземпляр класу <see cref="PaymentDbContext"/>
        /// </summary>
        public PaymentDbContext() : base() {}

        /// <summary>
        /// Метод створення моделі БД за допомогою ORM 
        /// </summary>
        /// <param name="modelBuilder">Будівельник моделі</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Order>().ToTable("Orders");

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Набір оплат у базі даних
        /// </summary>
        public DbSet<Payment> Payments { get; set; }

        /// <summary>
        /// Набір арендувань у базі даних
        /// </summary>
        public DbSet<Order> Orders { get; set; }
    }
}
