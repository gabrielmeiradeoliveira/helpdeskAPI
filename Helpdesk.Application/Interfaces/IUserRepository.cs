using Helpdesk.Domain.Entities;

namespace Helpdesk.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateRoleAsync(string email, UserRoleEnum role);
}