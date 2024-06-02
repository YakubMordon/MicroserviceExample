using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRentalService.Services;

/// <summary>
/// Сервіс для перевірки чи користувач аутентифікований
/// </summary>
public static class AuthenticationCheckerService
{
    /// <summary>
    /// Метод для перевірки чи користувач автентифікований
    /// </summary>
    /// <param name="token">JWT токен</param>
    /// <param name="cache">Контекст для роботи з кешом</param>
    /// <returns>True якщо автентифікований, false якщо ні</returns>
    public static async Task<bool> CheckAuthorizationAsync(string token, IDistributedCache cache)
    {
        var cachedToken = await cache.GetStringAsync(token);

        return cachedToken is not null;
    }

    /// <summary>
    /// Метод для перевірки чи користувач є адміном
    /// </summary>
    /// <param name="token">JWT токен</param>
    /// <returns>True якщо адмін, false якщо ні</returns>
    public static bool CheckAdminRole(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var decryptedToken = tokenHandler.ReadJwtToken(token);

        var roleClaim = decryptedToken.Claims.FirstOrDefault(c => c.Type == "role");

        return roleClaim?.Value.Contains("admin") is true;
    }
}