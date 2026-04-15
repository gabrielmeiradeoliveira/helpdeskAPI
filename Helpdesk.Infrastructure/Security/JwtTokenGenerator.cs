using Helpdesk.Application.Interfaces.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secret;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _secret = configuration["JwtToken"] ?? throw new ApplicationException("JWT secret is missing in configuration");
    }

    public string GenerateToken(string email, UserRoleEnum role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}