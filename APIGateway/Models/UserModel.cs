namespace ComplexLabGateway.Models;

/// <summary>
/// Модель для користувача
/// </summary>
public class UserModel
{
    /// <summary>
    /// Псевдонім користувача
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Пароль користувача
    /// </summary>
    public string Password { get; set; }
}