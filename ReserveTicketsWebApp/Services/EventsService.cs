using Microsoft.EntityFrameworkCore;
using ReserveTicketsWebApp.Domain;
using ReserveTicketsWebApp.Exceptions;
using ReserveTicketsWebApp.Infrastructure.Persistence;

namespace ReserveTicketsWebApp.Services;

public class EventsService(AppDbContext appDbContext) : IEventsService
{
    public async Task SetVipPackage(int eventId)
    {
        var eventRecord = await appDbContext.Events.FindAsync(eventId);

        if (eventRecord is null)
        {
            throw new NotFoundException(nameof(Event), eventId);
        }

        var numberOfPaidTickets = await appDbContext.Tickets
            .Where(t => t.EventId == eventId && t.Status == TicketStatus.Paid)
            .CountAsync();

        eventRecord.SetVipPackage(numberOfPaidTickets);

        await appDbContext.SaveChangesAsync();
    }
}