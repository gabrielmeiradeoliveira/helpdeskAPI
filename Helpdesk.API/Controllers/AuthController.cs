using Microsoft.AspNetCore.Mvc;
using Helpdesk.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Helpdesk.Domain.Dto;
using System.Security.Claims;

public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        _logger.LogInformation("Login attempt for {Email}", dto.Email);

        var result = await _authService.LoginAsync(dto.Email, dto.Password);

        _logger.LogInformation("Login success for {Email}", dto.Email);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        _logger.LogInformation("Register attempt for {Email}", dto.Email);

        await _authService.RegisterUserAsync(dto.Email, dto.Password, dto.ConfirmPassword);

        _logger.LogInformation("User registered {Email}", dto.Email);

        return StatusCode(201);
    }

    [Authorize]
    [HttpPut("role")]
    public async Task<IActionResult> UpdateRole([FromBody] AlterRoleDto dto)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role != UserRoleEnum.Admin.ToString())
        {
            _logger.LogWarning("Unauthorized role change attempt");
            return Forbid();
        }

        _logger.LogInformation("Updating role for {Email} to {Role}", dto.Email, dto.Role);

        await _authService.UpdateRoleAsync(dto.Email, dto.Role);

        return NoContent();
    }
}