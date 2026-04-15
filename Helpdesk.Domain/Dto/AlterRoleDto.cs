namespace Helpdesk.Domain.Dto;

public class AlterRoleDto
{
    public string Email { get; set; } = null!;
    public UserRoleEnum Role { get; set; }
}