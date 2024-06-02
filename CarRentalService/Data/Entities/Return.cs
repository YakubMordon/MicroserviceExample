namespace CarRentalService.Data.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Клас-сутність, який представляє повернення
    /// </summary>
    public class Return : BaseEntity
    {
        /// <summary>
        /// Ідентифікатор, який вказує на користувача, який арендує
        /// </summary>
        public int? CarId { get; set; }

        /// <summary>
        /// Дата завершення повернення автомобіля з арендування
        /// </summary>
        [Column("ReturnDate")]
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Індикатор пошкодження автомобіля
        /// </summary>
        [Column("IsDamaged")]
        public bool? IsDamaged { get; set; }

        /// <summary>
        /// Навігаційна властивість класу <see cref="Car"/> (Відношення одного-до-одного)
        /// </summary>
        [ForeignKey("CarId")]
        public virtual Car? Car { get; set; } 
    }
}
