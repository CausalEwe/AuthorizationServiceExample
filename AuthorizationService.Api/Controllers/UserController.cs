using System.Security.Claims;
using AuthorizationService.Api.Dtos;
using AuthorizationService.Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserManager _userManager;
    private readonly IMapper _mapper;

    public UserController(
        IUserManager userManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("all")]
    public async Task<IActionResult> GetShortUsers()
    {
        var users = await _userManager.GetShortUsers();
        var shortUsers = _mapper.Map<List<ShortUserModel>>(users);
        var result = new PagedResult<ShortUserModel>(shortUsers.Count, shortUsers);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("update")]
    public async Task<IActionResult> SetUserIsActive([FromBody]UpdateUserModel updateUserModel)
    {
        await _userManager.SetUserIsActive(updateUserModel.UserId, updateUserModel.IsActive);

        return Ok();
    }

    [HttpGet]
    [Authorize]
    [Route("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        try
        {
            var username = User.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
            return Ok(username);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Произошла ошибка при обработке вашего запроса. {ex.Message}");
        }
    }
}