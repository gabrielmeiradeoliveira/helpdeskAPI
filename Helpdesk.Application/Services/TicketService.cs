using Helpdesk.Application.Interfaces;
using Helpdesk.Domain.Dto;
using Helpdesk.Domain.Entities;
using Microsoft.Extensions.Logging;

public class TicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<TicketService> _logger;

    public TicketService(
        ITicketRepository ticketRepository,
        ILogger<TicketService> logger)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    public async Task CreateTicket(CreateTicketDto dto, string email)
    {
        email = NormalizeEmail(email);

        _logger.LogInformation("User {Email} attempting to create ticket", email);

        var ticket = new Ticket(dto.Title, dto.Description, email);

        await _ticketRepository.AddAsync(ticket);

        _logger.LogInformation("Ticket {TicketId} created by {Email}", ticket.Id, email);
    }

    public async Task UpdateTicket(Guid id, UpdateTicketDto dto)
    {
        _logger.LogInformation("Attempting to update ticket {TicketId}", id);

        var ticket = await GetTicketOrThrow(id);

        ticket.Update(dto.Title, dto.Description, dto.Status);

        await _ticketRepository.UpdateAsync(ticket);

        _logger.LogInformation("Ticket {TicketId} updated successfully", id);
    }

    public async Task DeleteTicket(Guid id)
    {
        _logger.LogWarning("Attempting to delete ticket {TicketId}", id);

        var ticket = await GetTicketOrThrow(id);

        await _ticketRepository.DeleteAsync(id);

        _logger.LogInformation("Ticket {TicketId} deleted successfully", id);
    }

    public async Task CloseTicket(Guid id)
    {
        _logger.LogInformation("Attempting to close ticket {TicketId}", id);

        var ticket = await GetTicketOrThrow(id);

        ticket.Close();

        await _ticketRepository.UpdateAsync(ticket);

        _logger.LogInformation("Ticket {TicketId} closed", id);
    }

    public async Task CancelTicket(Guid id)
    {
        _logger.LogInformation("Attempting to cancel ticket {TicketId}", id);

        var ticket = await GetTicketOrThrow(id);

        ticket.Cancel();

        await _ticketRepository.UpdateAsync(ticket);

        _logger.LogInformation("Ticket {TicketId} canceled", id);
    }

    public async Task<List<Ticket>> GetMyTickets(string email)
    {
        email = NormalizeEmail(email);

        _logger.LogInformation("Fetching tickets for user {Email}", email);

        var tickets = await _ticketRepository.GetByUserEmailAsync(email);

        _logger.LogInformation("User {Email} has {Count} tickets", email, tickets.Count);

        return tickets;
    }

    private async Task<Ticket> GetTicketOrThrow(Guid id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);

        if (ticket == null)
        {
            _logger.LogError("Ticket {TicketId} not found", id);
            throw new ApplicationException("Ticket not found");
        }

        return ticket;
    }

    private string NormalizeEmail(string email)
    {
        return email?.Trim().ToLower() ?? string.Empty;
    }
}