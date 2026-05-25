using Microsoft.EntityFrameworkCore;
using ReserveTicketsWebApp.Domain;
using ReserveTicketsWebApp.Endpoints.CancelTickets;
using ReserveTicketsWebApp.Endpoints.ReserveTickets;
using ReserveTicketsWebApp.Endpoints.VipPackage;
using ReserveTicketsWebApp.Exceptions;
using ReserveTicketsWebApp.Infrastructure.Persistence;
using ReserveTicketsWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DbConnection")));
builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapReserveTicketsEndpoints();
app.MapCancelTicketsEndpoints();
app.MapVipPackageEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }

    // seed data
    if (db.Events.Count() == 0)
    {
        db.Events.AddRange(
            new Event("Event1", 20, 20.0m, false, new DateTime(2023, 2, 12)),
            new Event("Event2", 5, 10.0m, false, new DateTime(2023, 2, 12)),
            new Event("Event3", 30, 10.0m, false, new DateTime(2023, 2, 12)),
            new Event("Event4", 10, 30.0m, false, new DateTime(2023, 2, 12))
        );
        db.SaveChanges();
    }
}

app.Run();
