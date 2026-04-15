namespace Helpdesk.Domain.Dto;

public class UpdateTicketDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public TicketStatusEnum Status { get; set; }
}