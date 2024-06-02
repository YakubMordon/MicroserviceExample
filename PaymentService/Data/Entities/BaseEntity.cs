namespace PaymentService.Data.Entities
{
    /// <summary>
    /// Абстрактний клас, який представляє принадлежність до сутності
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        public int Id { get; set; }
    }
}
