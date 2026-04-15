using Helpdesk.Application.Interfaces;
using Helpdesk.Application.Interfaces.Security;
using Helpdesk.Domain.Dto;
using Helpdesk.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task RegisterUserAsync(string email, string password, string confirmPassword)
    {
        email = NormalizeEmail(email);

        _logger.LogInformation("Register attempt for {Email}", email);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Register failed: empty email");
            throw new ApplicationException("Email is required");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Register failed for {Email}: empty password", email);
            throw new ApplicationException("Password is required");
        }

        if (password != confirmPassword)
        {
            _logger.LogWarning("Register failed for {Email}: passwords do not match", email);
            throw new ApplicationException("Passwords do not match");
        }

        if (await _userRepository.ExistsByEmailAsync(email))
        {
            _logger.LogWarning("Register failed: email already exists {Email}", email);
            throw new ApplicationException("Email already exists");
        }

        var passwordHash = _passwordHasher.Hash(password);

        var user = new User(email, passwordHash);

        await _userRepository.AddAsync(user);

        _logger.LogInformation("User registered successfully {Email}", email);
    }

    public async Task<UserDto> LoginAsync(string email, string password)
    {
        email = NormalizeEmail(email);

        _logger.LogInformation("Login attempt for {Email}", email);

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Login failed: user not found {Email}", email);
            throw new ApplicationException("Invalid credentials");
        }

        var isPasswordValid = _passwordHasher.Verify(password, user.PasswordHash);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Login failed: invalid password for {Email}", email);
            throw new ApplicationException("Invalid credentials");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Email, user.Role);

        _logger.LogInformation("Login successful for {Email}", email);

        return new UserDto
        {
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            JwtToken = token
        };
    }

    public async Task UpdateRoleAsync(string email, UserRoleEnum role)
    {
        email = NormalizeEmail(email);

        _logger.LogInformation("Updating role for {Email} to {Role}", email, role);

        await _userRepository.UpdateRoleAsync(email, role);

        _logger.LogInformation("Role updated successfully for {Email}", email);
    }

    private string NormalizeEmail(string email)
    {
        return email.ToLower().Trim();
    }
}