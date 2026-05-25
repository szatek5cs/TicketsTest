namespace ReserveTicketsWebApp.Domain;

public class Ticket
{
    public int Id { get; private set; }
    public int EventId { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public decimal Price { get; private set; }
    public TicketStatus Status { get; private set; }

    private Ticket() { } // For EF Core

    public Ticket(int eventId, int userId, decimal price)
    {
        EventId = eventId;
        UserId = userId;
        CreatedAtUtc = DateTime.UtcNow;
        Price = price;
        Status = TicketStatus.Reserved;
    }

    internal void Cancel()
    {
        Status = TicketStatus.Cancelled;
    }
}