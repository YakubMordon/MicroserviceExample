namespace CarRentalService.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using Data;
    using Model;
    using Services;

    /// <summary>
    /// ����������, ���� �������� 䳿, ��'���� � ������������ ���������
    /// </summary>
    [ApiController]
    [Route("api/v1")]
    public class CarRentalController : ControllerBase
    {
        /// <summary>
        /// �������� ��� ���������
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// �������� ��� ������ � ��
        /// </summary>
        private readonly ICarRentalDbContext _context;

        /// <summary>
        /// ��������� ��������� ���� � Redis
        /// </summary>
        private readonly DistributedCacheEntryOptions _options;

        /// <summary>
        /// ������� ��������� ����� <see cref="CarRentalController"/>
        /// </summary>
        /// <param name="cache">�������� ��� ���������</param>
        /// <param name="context">�������� ���� �����</param>
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
        /// ����� ������� ��� ��������� ��������� �������� ��� �����������
        /// </summary>
        /// <returns>������ ��������� �������� ��� �����������</returns>
        [HttpGet("cars")]
        public async Task<IActionResult> GetAvailableCarsAsync()
        {
            var cars = await _context.GetAvailableCarsFromDbAsync();

            return Ok(cars);
        }

        /// <summary>
        /// ����� ������� ��� ����������� ���������
        /// </summary>
        /// <param name="request">������ �����������</param>
        /// <returns>������ 200, ���� ��������� ����������, � ������ ������� 400</returns>
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
        /// ����� ������� ��� ���������� ��������� � �����������
        /// </summary>
        /// <param name="request">������ ����������</param>
        /// <returns>������ 200, ���� ��������� ��������� � �����������, � ������ ������� 400</returns>
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

        // �������� ��� ������������

        /// <summary>
        /// ����� �������, ��� �����, ��� ����������� ����������� ���������
        /// </summary>
        /// <param name="token">JWT-�����</param>
        /// <returns>������ ����������� ���������</returns>
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
        /// ����� �������, ��� �����, ��� ����������� ������ �����
        /// </summary>
        /// <param name="token">JWT-�����</param>
        /// <returns>������ �����</returns>
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
        /// ����� �������, ��� �����, ��� ����������� ����������� ���������
        /// </summary>
        /// <param name="token">JWT-�����</param>
        /// <returns>������ ����������� ���������</returns>
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
        /// ����� �������, ��� �����, ���������� � ����������
        /// </summary>
        /// <param name="orderId">������������� �����������</param>
        /// <param name="token">JWT-�����</param>
        /// <returns>������ 200, ���� ����������� ���������, � ������ ������� ������ 400</returns>
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
