namespace ComplexLabGateway.Models;

/// <summary>
/// Базова модель
/// </summary>
public abstract class BaseModel
{
    /// <summary>
    /// JWT-токен
    /// </summary>
    public string Token { get; set; }
}