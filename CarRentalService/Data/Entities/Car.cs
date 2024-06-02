namespace CarRentalService.Data.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Клас-сутність, який представляє автомобіль
    /// </summary>
    public class Car : BaseEntity
    {
        /// <summary>
        /// Бренд автомобіля
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Модель автомобіля
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Індикатор можливості арендування
        /// </summary>
        public bool? IsAvailable { get; set; }

        /// <summary>
        /// Навігаційна властивість до класу <see cref="Order"/> (Залежність один-до-багатьох)
        /// </summary>
        public virtual Order? Order { get; set; }

        /// <summary>
        /// Навігаційна властивість до класу <see cref="Return"/> (Залежність один-до-одного)
        /// </summary>
        public virtual Return? Return { get; set; }
    }
}
