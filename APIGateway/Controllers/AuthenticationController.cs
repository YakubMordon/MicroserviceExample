namespace ComplexLabGateway.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;

/// <summary>
/// Контроллер, точка доступа до мікросервісів, зв'язаних з арендуванням автомобілів
/// </summary>
[ApiController]
[Route("api/v1")]
public class AuthenticationController : ControllerBase
{
    /// <summary>
    /// Фабрика для заготованих HttpClient
    /// </summary>
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Повертає екземпляр класу <see cref="AuthenticationController"/>
    /// </summary>
    /// <param name="clientFactory">Фабрика для заготованих HttpClient</param>
    public AuthenticationController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    /// <summary>
    /// Точка доступу для автентифікації користувача
    /// </summary>
    /// <param name="request">Модель користувача</param>
    /// <returns>Статус 200, якщо успішно автентифіковано, інакше Статус 400</returns>
    [HttpPost("log-in")]
    public async Task<IActionResult> LogIn([FromBody] UserModel request)
    {
        var client = _clientFactory.CreateClient("authenticationService");
        var response = await client.PostAsJsonAsync("api/v1/token", request);
        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadAsStringAsync());
        }
        return BadRequest();
    }

    /// <summary>
    /// Точка доступу для реєстрації користувача
    /// </summary>
    /// <param name="user">Модель користувача</param>
    /// <returns>Статус 200, якщо успішно зареєстровано, інакше Статус 400</returns>
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] UserModel user)
    {
        var client = _clientFactory.CreateClient("authenticationService");
        var response = await client.PostAsJsonAsync("api/v1/sign-up", user);
        if (response.IsSuccessStatusCode)
        {
            return Ok(await response.Content.ReadAsStringAsync());
        }
        return BadRequest();
    }
}