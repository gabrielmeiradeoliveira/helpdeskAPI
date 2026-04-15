using Helpdesk.Domain.Entities;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket);
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<List<Ticket>> GetByUserEmailAsync(string email);
    Task UpdateAsync(Ticket ticket);
    Task DeleteAsync(Guid id);
}