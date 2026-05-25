namespace ReserveTicketsWebApp.Services;

public interface ITicketsService
{
    Task<int> CancelExpiredTicketsAsync();
    Task<int> ReserveTicketsAsync(int eventId, int userId);
}