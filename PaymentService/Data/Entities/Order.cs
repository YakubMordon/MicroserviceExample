﻿namespace PaymentService.Data.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Клас-сутність, який представляє замовлення
    /// </summary>
    [Table("Orders")]
    public class Order : BaseEntity
    {
        /// <summary>
        /// Ідентифікатор, який вказує на автомобіль, який був арендований
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Ідентифікатор, який вказує на користувача, який арендує
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Дата старту арендування автомобіля
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Дата завершення повернення автомобіля з арендування
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Індикатор стану аренди
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
