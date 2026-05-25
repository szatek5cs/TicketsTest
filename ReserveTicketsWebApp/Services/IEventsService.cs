namespace ReserveTicketsWebApp.Services;

public interface IEventsService
{
    Task SetVipPackage(int eventId);
}