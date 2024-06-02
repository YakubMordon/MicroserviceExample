namespace ComplexLabGateway.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;

/// <summary>
/// Контроллер, точка доступа до мікросервісів, зв'язаних з арендуванням автомобілів
/// </summary>
[ApiController]
[Route("api/v1")]
public class CarRentalController : ControllerBase
{
    /// <summary>
    /// Фабрика для заготованих HttpClient
    /// </summary>
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Повертає екземпляр класу <see cref="CarRentalController"/>
    /// </summary>
    /// <param name="clientFactory">Фабрика для заготованих HttpClient</param>
    public CarRentalController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    /// <summary>
    /// Точка доступу для получення доступних автомобілів
    /// </summary>
    /// <returns>Список доступних автомобілів</returns>
    [HttpGet("cars")]
    public async Task<IActionResult> GetAvailableCars()
    {
        var client = _clientFactory.CreateClient("carRentalService");
        var response = await client.GetAsync("api/v1/cars");
        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadAsStringAsync());
        }
        return BadRequest();
    }

    /// <summary>
    /// Точка доступу для арендування автомобіля
    /// </summary>
    /// <param name="order">Модель арендування</param>
    /// <returns>Статус 200, якщо успішно арендовано, інакше Статус 400</returns>
    [HttpPost("orders")]
    public async Task<IActionResult> OrderCar([FromBody] OrderModel order)
    {
        var client = _clientFactory.CreateClient("carRentalService");
        var response = await client.PostAsJsonAsync("api/v1/orders", order);
        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadAsStringAsync());
        }
        return BadRequest();
    }

    /// <summary>
    /// Точка доступу для повернення автомобіля з арендування
    /// </summary>
    /// <param name="returnRequest">Модель повернення</param>
    /// <returns>Статус 200, якщо автомобіль успішно повернено, інакше Статус 400</returns>
    [HttpPost("returns")]
    public async Task<IActionResult> ReturnCar([FromBody] ReturnModel returnRequest)
    {
        var client = _clientFactory.CreateClient("carRentalService");
        var response = await client.PostAsJsonAsync("api/v1/returns", returnRequest);
        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadAsStringAsync());
        }
        return BadRequest();
    }
}
