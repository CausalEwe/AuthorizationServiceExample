namespace AuthorizationService.Api.Dtos;

public class ShortUserModel
{
    public int Id { get; set; }

    public string Login { get; set; }

    public bool IsActive { get; set; }
}