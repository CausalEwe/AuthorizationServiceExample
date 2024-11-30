namespace AuthorizationService.Api.Dtos;

public class UpdateUserModel
{
    public int UserId { get; set; }

    public bool IsActive { get; set; }
}