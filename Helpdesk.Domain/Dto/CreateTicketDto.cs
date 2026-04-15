namespace Helpdesk.Domain.Dto;

public class CreateTicketDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public TicketStatusEnum Status { get; set; }
}