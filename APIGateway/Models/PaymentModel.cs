namespace ComplexLabGateway.Models;

/// <summary>
/// Клас модель, яка представляє оплату
/// </summary>
public class PaymentModel
{
    /// <summary>
    /// Ідентифікатор замовлення
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Ціна оплати
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// JWT-токен
    /// </summary>
    public string Token { get; set; }
}