namespace CarRentalService.Data.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Абстрактний клас, який представляє принадлежність до сутності
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        [Key]
        [Column("Id")]
        public int Id { get; set; }
    }
}
