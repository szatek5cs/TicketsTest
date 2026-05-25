using ReserveTicketsWebApp.Exceptions;

namespace ReserveTicketsWebApp.Domain;

public class Event
{
    public int Id { get; private set; }
    public string Title { get; private set; } = default!;
    public int AvailableSeats { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsVipOnly { get; private set; }
    public int RowVersion { get; private set; } // For optimistic concurrency control
    public DateTime Date { get; private set; }

    private Event() { } // For EF Core

    public Event(
        string title, 
        int availableSeats, 
        decimal basePrice, 
        bool isVipOnly, 
        DateTime date)
    {
        Title = title;
        AvailableSeats = availableSeats;
        BasePrice = basePrice;
        IsVipOnly = isVipOnly;
        Date = date;
        RowVersion = 0;
    }

    internal void AddReservedTicket()
    {
        if (AvailableSeats <= 0)
        {
            throw new DomainValidationException("No available seats.", ErrorCodes.NotAvailableSeats);
        }

        AvailableSeats--;

        UpdateRowVersion();
    }

    private void UpdateRowVersion()
    {
        RowVersion++;
    }

    internal void ReleaseSeats(int value)
    {
        AvailableSeats += value;
        UpdateRowVersion();
    }

    internal void SetVipPackage(int numberOfPaidTickets)
    {
        if (IsVipOnly)
        {
            throw new DomainValidationException("VIP package is already set for this event.", ErrorCodes.VipPackageAlreadySet);
        }
        
        if (numberOfPaidTickets > 10)
        {
            throw new DomainValidationException("Cannot set VIP package for events with more than 10 paid tickets.", ErrorCodes.TooManyPaidTickets);
        }

        IsVipOnly = true;
        BasePrice *= 1.2m;
        UpdateRowVersion();
    }
}
