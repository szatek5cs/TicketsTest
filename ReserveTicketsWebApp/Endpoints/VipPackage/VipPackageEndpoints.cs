using ReserveTicketsWebApp.Services;

namespace ReserveTicketsWebApp.Endpoints.VipPackage;

public static class VipPackageEndpoints
{
    public static void MapVipPackageEndpoints(this WebApplication app)
    {
        app.MapPost("/api/events/vip-package", async (
            VipPackageRequest request,
            IEventsService eventsService) =>
        {
            // Simulate ticket reservation logic
            await eventsService.SetVipPackage(request.EventId);
            return Results.Ok(new VipPackageResponse(
                Message: $"Successfully set VIP package for event {request.EventId}.")
            );
        });
    }
}