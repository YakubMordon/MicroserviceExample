namespace PaymentService.Data.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Клас-сутність, яка представляє оплату
    /// </summary>
    [Table("Payments")]
    public class Payment : BaseEntity
    {
        /// <summary>
        /// Ідентифікатор оплаченого замовлення
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Оплачена ціна
        /// </summary>
        public decimal Amount { get; set; } 

        /// <summary>
        /// Дата оплати
        /// </summary>
        public DateTime PaymentDate { get; set; } 
    }
}
