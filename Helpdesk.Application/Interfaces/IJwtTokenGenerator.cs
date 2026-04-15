namespace Helpdesk.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(string email, UserRoleEnum role);
}