using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthorizationService.Application.Interfaces;
using AuthorizationService.Domain.Entities;
using AuthorizationService.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationService.Application.Services;

public class AuthManager : IAuthManager
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IAuthRepository _authRepository;
    private readonly IMemoryCache _memoryCache;

    public AuthManager(
        IUserRepository userRepository,
        IConfiguration configuration,
        IAuthRepository authRepository,
        IMemoryCache memoryCache)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _authRepository = authRepository;
        _memoryCache = memoryCache;
    }

    public async Task<string> RegisterAsync(User user)
    {
        var existingUser = await _userRepository.GetUserByLoginAsync(user.Login);

        if (existingUser != null)
        {
            return "A user with this login already exists.";
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _userRepository.RegisterAsync(user);

        return "User registered successfully.";
    }

    public async Task<UserToken> LoginAsync(User loginModel)
    {
        var user = await _userRepository.GetUserByLoginAsync(loginModel.Login);

        if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
        {
            return null;
        }

        var userRoles = await _userRepository.GetUserRoles(user.Id);

        var token = this.GenerateJwtToken(user.Id, user.Login, userRoles);

        var userToken = new UserToken
        {
            UserId = user.Id,
            CreatedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddMinutes(180),
            IsRevoked = false,
            Token = token
        };

        await this.AddToken(userToken);

        return userToken;
    }

    public async Task<bool> ValidateToken(int userId, string token)
    {
        if (_memoryCache.TryGetValue(userId, out bool isValid))
        {
            return isValid;
        }

        isValid = await _authRepository.ValidateToken(userId, token);

        return isValid;
    }

    public async Task AddToken(UserToken userToken)
    {
        await _authRepository.AddToken(userToken);

        _memoryCache.Set(userToken.UserId, !userToken.IsRevoked, TimeSpan.FromHours(3));
    }

    public async Task RevokeTokenAsync(int userId)
    {
        await _authRepository.RevokeTokenAsync(userId);

        _memoryCache.Remove(userId);
    }

    private string GenerateJwtToken(int userId, string login, List<string> userRoles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, login),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (userRoles != null && userRoles.Any())
        {
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(180);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}