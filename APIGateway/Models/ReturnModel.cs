namespace ComplexLabGateway.Models;

/// <summary>
/// Клас-модель, яка представляє повернення
/// </summary>
public class ReturnModel : BaseModel
{
    /// <summary>
    /// Ідентифікатор замовлення, арендування машини якої завершено
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Індикатор пошкодження автомобіля
    /// </summary>
    public bool IsDamaged { get; set; }
}
