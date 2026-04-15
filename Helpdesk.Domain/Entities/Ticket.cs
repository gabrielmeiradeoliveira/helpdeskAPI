namespace Helpdesk.Domain.Entities;

public class Ticket
{
    public Guid Id { get; private set; }

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public TicketStatusEnum Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public string UserEmail { get; private set; } = null!;

    private Ticket() { }

    public Ticket(string title, string description, string userEmail)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ApplicationException("Title is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new ApplicationException("Description is required");

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        UserEmail = userEmail.Trim().ToLower();
        Status = TicketStatusEnum.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string description, TicketStatusEnum status)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ApplicationException("Title is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new ApplicationException("Description is required");

        Title = title;
        Description = description;
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status == TicketStatusEnum.Closed)
            throw new ApplicationException("Ticket already closed");

        Status = TicketStatusEnum.Closed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == TicketStatusEnum.Canceled)
            throw new ApplicationException("Ticket already canceled");

        Status = TicketStatusEnum.Canceled;
        UpdatedAt = DateTime.UtcNow;
    }
}