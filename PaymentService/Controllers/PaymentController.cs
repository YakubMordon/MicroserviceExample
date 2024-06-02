namespace PaymentService.Controllers
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;
    using Data;
    using Data.Entities;
    using Models;
    using Microsoft.Extensions.Caching.Distributed;
    using System.Threading.Tasks;

    /// <summary>
    /// ����������, ���� �������� 䳿 ��'���� � �������
    /// </summary>
    [ApiController]
    [Route("api/v1")]
    public class PaymentController : ControllerBase
    {
        /// <summary>
        /// �������� ��� ������ � ��
        /// </summary>
        private readonly PaymentDbContext _context;

        /// <summary>
        /// �������� ��� ���������
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// ������� ��������� ����� <see cref="PaymentController"/>
        /// </summary>
        /// <param name="context">�������� ��� ������ � ��</param>
        /// <param name="cache">�������� ��� ���������</param>
        public PaymentController(PaymentDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// ����� ������� ��� ������ �����������
        /// </summary>
        /// <param name="request">������ ������</param>
        /// <returns>������ 200, ���� ������ ��������, ������ ������ 400</returns>
        [HttpPost("payment")]
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] PaymentModel request)
        {
            var token = await _cache.GetStringAsync(request.Token);

            if (token is null)
            {
                return BadRequest("Token is not existing or expired");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == request.OrderId);

            if(order is null)
            {
                return BadRequest("Order doesn't exist");
            }

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            };

            await _cache.SetStringAsync(request.Token, "payment-token-updated", options);

            var paymentId = _context.Payments.Count() + 1;

            // ����� ��� ������� ������� �� ������ ���������
            var payment = new Payment
            {
                Id = paymentId,
                OrderId = request.OrderId,
                Amount = request.Amount,
                PaymentDate = DateTime.Now,
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return Ok("Payment processed successfully.");
        }
    }
}
