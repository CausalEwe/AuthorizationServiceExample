using System.Security.Claims;
using AuthorizationService.Api.Dtos;
using AuthorizationService.Application.Interfaces;
using AuthorizationService.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;
    private readonly IMapper _mapper;

    public AuthController(
        IAuthManager authManager,
        IMapper mapper)
    {
        _authManager = authManager;
        _mapper = mapper;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody]RegisterModel registerModel)
    {
        if (string.IsNullOrEmpty(registerModel.Login) || string.IsNullOrEmpty(registerModel.Password))
        {
            return BadRequest("Wrong data.");
        }

        var message = await _authManager.RegisterAsync(_mapper.Map<User>(registerModel));

        return Ok(message);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
    {
        var token = Request.Cookies["token"];
        if (!string.IsNullOrEmpty(token))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isValid = await _authManager.ValidateToken(int.Parse(userId), token);

            if (isValid)
            {
                return Conflict("You are already logged in");
            }
        }

        if (string.IsNullOrEmpty(loginModel.Login) || string.IsNullOrEmpty(loginModel.Password))
        {
            return BadRequest("Wrong data.");
        }

        var userToken = await _authManager.LoginAsync(_mapper.Map<User>(loginModel));

        if (userToken == null)
        {
            return Unauthorized("We could not log you in. Please check your username/password and try again");
        }

        Response.Cookies.Append("token", userToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(180)
        });

        return Ok(new { Token = userToken.Token });
    }

    [HttpPost]
    [Authorize]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _authManager.RevokeTokenAsync(int.Parse(userId));

        Response.Cookies.Delete("token");

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("token/revoke")]
    public async Task<IActionResult> RevokeToken(int userId)
    {
        await _authManager.RevokeTokenAsync(userId);

        return Ok();
    }
}