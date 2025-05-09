using Notification.API.Extensions;
using Notification.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog();

builder.Services
    .ConfigureApiServices(builder.Configuration)
    .ConfigureAppServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
