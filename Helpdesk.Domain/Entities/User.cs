namespace Helpdesk.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public UserRoleEnum Role { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public User(string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        Role = UserRoleEnum.Basic;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateRole(UserRoleEnum role)
    {
        Role = role;
    }
}