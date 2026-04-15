using Helpdesk.Domain.Entities;
using Helpdesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<TicketRepository> _logger;

    public TicketRepository(
        AppDbContext context,
        ILogger<TicketRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Ticket ticket)
    {
        _logger.LogInformation("Adding ticket {TicketId}", ticket.Id);

        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} added successfully", ticket.Id);
    }

    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching ticket {TicketId}", id);

        return await _context.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Ticket>> GetByUserEmailAsync(string email)
    {
        _logger.LogInformation("Fetching tickets for user {Email}", email);

        return await _context.Tickets
            .AsNoTracking()
            .Where(t => t.UserEmail == email)
            .ToListAsync();
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _logger.LogInformation("Attempting to update ticket {TicketId}", ticket.Id);

        var existing = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == ticket.Id);

        if (existing == null)
        {
            _logger.LogError("Ticket {TicketId} not found for update", ticket.Id);
            throw new ApplicationException("Ticket not found");
        }

        existing.Update(ticket.Title, ticket.Description, ticket.Status);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} updated successfully", ticket.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogWarning("Attempting to delete ticket {TicketId}", id);

        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            _logger.LogError("Ticket {TicketId} not found for delete", id);
            throw new ApplicationException("Ticket not found");
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Ticket {TicketId} deleted successfully", id);
    }
}