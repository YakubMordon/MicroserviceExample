namespace AuthenticationService.Data.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Клас-сутність, який представляє користувача
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Псевдонім користувача
        /// </summary>
        [Column("Username")]
        public string Username { get; set; }

        /// <summary>
        /// Пароль користувача
        /// </summary>
        [Column("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Роль користувача
        /// </summary>
        [Column("Role")]
        public string Role { get; set; }
    }
}
