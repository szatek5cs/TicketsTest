using System;
using ReserveTicketsWebApp.Services;

namespace ReserveTicketsWebApp.Endpoints.ReserveTickets;

public static class ReserveTicketsEndpoints
{
    public static void MapReserveTicketsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/tickets/reserve", async (
            ReserveTicketsRequest request,
            ITicketsService ticketsService) =>
        {
            // Simulate ticket reservation logic
            var ticketId = await ticketsService.ReserveTicketsAsync(request.EventId, request.UserId);
            return Results.Ok(new ReserveTicketsResponse(
                ticketId,
                 $"Successfully reserved ticket for event {request.EventId}."));
        });
    }
}