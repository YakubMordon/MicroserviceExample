namespace CarRentalService.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using Data;
    using Model;
    using Services;

    /// <summary>
    /// Контроллер, який обробляє дії, зв'язані з арендуванням автомобіля
    /// </summary>
    [ApiController]
    [Route("api/v1")]
    public class CarRentalController : ControllerBase
    {
        /// <summary>
        /// Контекст для кешування
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Контекст для роботи з бд
        /// </summary>
        private readonly ICarRentalDbContext _context;

        /// <summary>
        /// Настройки зберігання кешу в Redis
        /// </summary>
        private readonly DistributedCacheEntryOptions _options;

        /// <summary>
        /// Повертає екземпляр класу <see cref="CarRentalController"/>
        /// </summary>
        /// <param name="cache">Контекст для кешування</param>
        /// <param name="context">Контекст бази даних</param>
        public CarRentalController(IDistributedCache cache, ICarRentalDbContext context)
        {
            _cache = cache;
            _context = context;
            _options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            };
        }

        /// <summary>
        /// Точка доступу для получення автомобілів можливих для арендування
        /// </summary>
        /// <returns>Список автомобілів можливих для арендування</returns>
        [HttpGet("cars")]
        public async Task<IActionResult> GetAvailableCarsAsync()
        {
            var cars = await _context.GetAvailableCarsFromDbAsync();

            return Ok(cars);
        }

        /// <summary>
        /// Точка доступу для арендування автомобіля
        /// </summary>
        /// <param name="request">Модель арендування</param>
        /// <returns>Статус 200, якщо автомобіль арендовано, в іншому випадку 400</returns>
        [HttpPost("orders")]
        public async Task<IActionResult> OrderCarAsync([FromBody] OrderModel request)
        {
            var isAuthenticated = await AuthenticationCheckerService.CheckAuthorizationAsync(request.Token, _cache);

            if (!isAuthenticated)
            {
                return BadRequest("Token is not existing or expired");
            }

            await _cache.SetStringAsync(request.Token, "orders-token-updated", _options);

            var isSuccess = await _context.OrderCarInDbAsync(request);

            return isSuccess 
                ? Ok("Car successfully ordered.") 
                : BadRequest("Car not available for order.");
        }

        /// <summary>
        /// Точка доступу для повернення автомобіля з арендування
        /// </summary>
        /// <param name="request">Модель повернення</param>
        /// <returns>Статус 200, якщо автомобіль повернено з арендування, в іншому випадку 400</returns>
        [HttpPost("returns")]
        public async Task<IActionResult> ReturnCarAsync([FromBody] ReturnModel request)
        {
            var isAuthenticated = await AuthenticationCheckerService.CheckAuthorizationAsync(request.Token, _cache);

            if (!isAuthenticated)
            {
                return BadRequest("Token is not existing or expired");
            }

            await _cache.SetStringAsync(request.Token, "returns-token-updated", _options);

            var isSuccess = await _context.ReturnCarFromDbAsync(request);

            return isSuccess 
                ? Ok("Car successfully returned.") 
                : BadRequest("Order or Car not found.");
        }

        // Ендпоінти для адміністратора

        /// <summary>
        /// Точка доступу, для адміна, для заполучення арендованих автомобілів
        /// </summary>
        /// <param name="token">JWT-токен</param>
        /// <returns>Список арендованих автомобілів</returns>
        [HttpGet("cars/rented")]
        public async Task<IActionResult> GetRentedCarsAsync(string token)
        {
            var isAuthenticated = await AuthenticationCheckerService.CheckAuthorizationAsync(token, _cache);

            if (!isAuthenticated)
            {
                return BadRequest("Token is not existing or expired");
            }

            var isAdmin = AuthenticationCheckerService.CheckAdminRole(token);

            if (!isAdmin)
            {
                return BadRequest("You do not have access to admin panel");
            }

            await _cache.SetStringAsync(token, "rented-cars-token-updated", _options);

            var rentedCars = await _context.GetRentedCarsFromDbAsync();
            return Ok(rentedCars);
        }

        /// <summary>
        /// Точка доступу, для адміна, для заполучення списку аренд
        /// </summary>
        /// <param name="token">JWT-токен</param>
        /// <returns>Список аренд</returns>
        [HttpGet("orders")]
        public async Task<IActionResult> GetRentalHistoryAsync(string token)
        {
            var isAuthenticated = await AuthenticationCheckerService.CheckAuthorizationAsync(token, _cache);

            if (!isAuthenticated)
            {
                return BadRequest("Token is not existing or expired");
            }

            var isAdmin = AuthenticationCheckerService.CheckAdminRole(token);

            if (!isAdmin)
            {
                return BadRequest("You do not have access to admin panel");
            }

            await _cache.SetStringAsync(token, "rental-history-token-updated", _options);

            var rentalHistory = await _context.GetRentalHistoryFromDbAsync();
            return Ok(rentalHistory);
        }

        /// <summary>
        /// Точка доступу, для адміна, для заполучення пошкоджених автомобілів
        /// </summary>
        /// <param name="token">JWT-токен</param>
        /// <returns>Список пошкоджених автомобілів</returns>
        [HttpGet("cars/damaged")]
        public async Task<IActionResult> GetDamagedCarsAsync(string token)
        {
            var isAuthenticated = await AuthenticationCheckerService.CheckAuthorizationAsync(token, _cache);

            if (!isAuthenticated)
            {
                return BadRequest("Token is not existing or expired");
            }

            var isAdmin = AuthenticationCheckerService.CheckAdminRole(token);

            if (!isAdmin)
            {
                return BadRequest("You do not have access to admin panel");
            }

            await _cache.SetStringAsync(token, "damaged-cars-token-updated", _options);

            var damagedCars = await _context.GetDamagedCarsFromDbAsync();
            return Ok(damagedCars);
        }

        /// <summary>
        /// Точка доступу, для адміна, відмовлення у арендуванні
        /// </summary>
        /// <param name="orderId">Ідентифікатор арендування</param>
        /// <param name="token">JWT-токен</param>
        /// <returns>Статус 200, якщо арендування відмовлено, в іншому випадку статус 400</returns>
        [HttpPatch("orders/{orderId}")]
        public async Task<IActionResult> RejectOrderAsync(int orderId, string token)
        {
            var isAuthenticated = await AuthenticationCheckerService.CheckAuthorizationAsync(token, _cache);

            if (!isAuthenticated)
            {
                return BadRequest("Token is not existing or expired");
            }

            var isAdmin = AuthenticationCheckerService.CheckAdminRole(token);

            if (!isAdmin)
            {
                return BadRequest("You do not have access to admin panel");
            }

            await _cache.SetStringAsync(token, "reject-order-token-updated", _options);

            await _context.RejectOrderFromDbAsync(orderId);

            return Ok("Order rejected successfully.");
        }
    }
}
