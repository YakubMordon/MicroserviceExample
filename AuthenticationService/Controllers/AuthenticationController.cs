namespace AuthenticationService.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Data;
using Data.Entities;
using Models;

/// <summary>
/// ���������� ��� �������������� �����������
/// </summary>
[ApiController]
[Route("api/v1")]
public class AuthenticationController : ControllerBase
{
    /// <summary>
    /// ��������� ���� ��� ��������� JWT ������
    /// </summary>
    private const string TokenSecret = "MySecretKey";

    /// <summary>
    /// �������� ���� �����
    /// </summary>
    private readonly AuthenticationDbContext _context;

    /// <summary>
    /// �������� ��� ���������
    /// </summary>
    private readonly IDistributedCache _cache;

    /// <summary>
    /// ������� ��������� ����� <see cref="AuthenticationController"/>
    /// </summary>
    /// <param name="context">�������� ���� �����</param>
    /// <param name="cache">�������� ��� ���������</param>
    public AuthenticationController(AuthenticationDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    /// <summary>
    /// ����� ������� ��� ����������� ������ ��������������
    /// </summary>
    /// <param name="request">������ ��������������</param>
    /// <returns>������ 200, ���� ��� ������, ������ ������ 401</returns>
    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] UserModel request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

        if (user is null)
        {
            return BadRequest();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret.PadRight(128 / 8)));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Username),
            new("userid", user.Id.ToString()),
            new(ClaimTypes.Role, user.Role)
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = "https://authenticationservice:8084",
            Audience = "https://carrentalservice:8083 , https://paymentservice:8081, https://complexlabgateway:8082",
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
        };

        // Store the token in Redis for further authentication verification
        await _cache.SetStringAsync(jwt, user.Id.ToString(), cacheOptions);

        return Ok(new { JwtToken = jwt });
    }

    /// <summary>
    /// ����� ������� ��� ��������� �����������
    /// </summary>
    /// <param name="model">������ �����������</param>
    /// <returns>��������� ������, 200 ���� ������������, ������ ������ 400</returns>
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] UserModel model)
    {
        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == model.Username);

        if (entity is not null)
        {
            return BadRequest("This model already exists.");
        }

        var userId = _context.Users.Count() + 1;
        
        var user = new User
        {
            Id = userId,
            Username = model.Username,
            Password = model.Password,
            Role = "user",
        };

        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return Ok();
    }
}
