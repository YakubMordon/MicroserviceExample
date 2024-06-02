namespace ComplexLabGateway.Models;

/// <summary>
/// Клас-модель, яка представляє замовлення
/// </summary>
public class OrderModel : BaseModel
{
    /// <summary>
    /// Ідентифікатор машини, яку арендовано
    /// </summary>
    public int CarId { get; set; }

    /// <summary>
    /// Ідентифікатор користувача, який арендував
    /// </summary>
    public int UserId { get; set; }
}

