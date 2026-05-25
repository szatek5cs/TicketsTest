public record ReserveTicketsRequest
{
    public int EventId { get; init; }
    public int UserId { get; init; }
}