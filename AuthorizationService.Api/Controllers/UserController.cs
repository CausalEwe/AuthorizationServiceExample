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
    [Authorize]
    [Route("all")]
    public async Task<IActionResult> GetShortUsers()
    {
        var result = await _userManager.GetShortUsers();

        return Ok(_mapper.Map<List<ShortUserModel>>(result));
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> SetUserIsActive([FromBody]UpdateUserModel updateUserModel)
    {
        await _userManager.SetUserIsActive(updateUserModel.UserId, updateUserModel.IsActive);

        return Ok();
    }
}