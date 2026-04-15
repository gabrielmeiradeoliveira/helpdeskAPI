namespace Helpdesk.Domain.Dto;

public class UserDto
{
    public string Email { get; set; } = null!;

    public UserRoleEnum Role { get; set; }

    public bool IsActive { get; set; }

    public string JwtToken { get; set; } = null!;
}