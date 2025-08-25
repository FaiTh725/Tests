using Notification.API.Extensions;
using Notification.API.Hubs.Instances;
using Notification.API.Middlewares;
using Notification.Application;
using Notification.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Host.UseSerilog();

builder.Services
    .ConfigureApiServices(builder.Configuration)
    .ConfigureAppServices(builder.Configuration)
    .ConfigureInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/hub/notification");

app.UseExceptionHandler();

app.Run();

// For integration tests
public partial class Program() { }
