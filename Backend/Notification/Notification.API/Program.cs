using Notification.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services
    .ConfigureAppServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

//app.MapControllers();

app.Run();
