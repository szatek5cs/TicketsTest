using ReserveTicketsWebApp.Services;

namespace ReserveTicketsWebApp.Endpoints.CancelTickets;

public static class CancelTicketsEndpoint
{
    public static void MapCancelTicketsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/tickets/cancel-expired", async (
            ITicketsService ticketsService) =>
        {
            // Simulate ticket cancellation logic
            var cancelledTicketsCount = await ticketsService.CancelExpiredTicketsAsync();

            if (cancelledTicketsCount > 0)
            {
                return Results.Ok(
                    new CancelTicketsResponse(
                        Message: $"Successfully cancelled {cancelledTicketsCount} expired tickets."));
            }
            else
            {
                return Results.Ok(
                    new CancelTicketsResponse(
                        Message: "No expired tickets to cancel."));
            }
        });
    }
}