using Microsoft.EntityFrameworkCore;
using ReserveTicketsWebApp.Domain;
using ReserveTicketsWebApp.Exceptions;
using ReserveTicketsWebApp.Infrastructure.Persistence;

namespace ReserveTicketsWebApp.Services;

public class TicketsService(AppDbContext appDbContext) : ITicketsService
{
    public async Task<int> CancelExpiredTicketsAsync()
    {
        // cancel tickets that are reserved but not paid within 15 minutes
        var expirationTime = DateTime.UtcNow.AddMinutes(-15);
        var expiredTickets = await appDbContext.Tickets
            .Where(t => t.Status == TicketStatus.Reserved && t.CreatedAtUtc < expirationTime)
            .ToListAsync();

        Dictionary<int, int> ticketsToRelease = [];
        foreach (var ticket in expiredTickets)
        {
            ticketsToRelease[ticket.EventId] = ticketsToRelease.GetValueOrDefault(ticket.EventId, 0) + 1;
            ticket.Cancel();
        }

        foreach (var kvp in ticketsToRelease)
        {
            var eventRecord = await appDbContext.Events.FindAsync(kvp.Key);

            eventRecord?.ReleaseSeats(kvp.Value);
        }

        await appDbContext.SaveChangesAsync();

        return expiredTickets.Count;
    }

    public async Task<int> ReserveTicketsAsync(int eventId, int userId)
    {
        var eventRecord = await appDbContext.Events.FindAsync(eventId);

        if (eventRecord == null)
        {
            throw new NotFoundException(nameof(Event), eventId);
        }

        var ticket = new Ticket(eventId, userId, eventRecord.BasePrice);
        appDbContext.Add(ticket);

        eventRecord.AddReservedTicket();
        await appDbContext.SaveChangesAsync();

        return ticket.Id;
    }
}