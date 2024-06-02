namespace ComplexLabGateway.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;

/// <summary>
/// Контроллер, точка доступа до мікросервісів, зв'язаних з оплатою арендування
/// </summary>
[ApiController]
[Route("api/v1")]
public class PaymentController : ControllerBase
{
    /// <summary>
    /// Фабрика для заготованих HttpClient
    /// </summary>
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Повертає екземпляр класу <see cref="PaymentController"/>
    /// </summary>
    /// <param name="clientFactory">Фабрика для заготованих HttpClient</param>
    public PaymentController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    /// <summary>
    /// Точка доступу для обробки покупки арендування
    /// </summary>
    /// <param name="payment">Модель оплати</param>
    /// <returns>Статус 200, якщо успішно оплачено, інакше Статус 400</returns>
    [HttpPost("payment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentModel payment)
    {
        var client = _clientFactory.CreateClient("paymentService");
        var response = await client.PostAsJsonAsync("api/v1/payment", payment);
        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadAsStringAsync());
        }
        return BadRequest();
    }
}
