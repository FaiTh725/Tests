using Authorization.API.Extension;
using Authorization.API.Middlewares;
using Authorization.Application;
using Authorization.Dal;
using Serilog;
using Authorization.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Host.UseSerilog();

builder.Services
    .ConfigureApiServices(builder.Configuration)
    .ConfigureDalServices(builder.Configuration)
    .ConfigureAppServices()
    .ConfigureInfastructureServices(builder.Configuration);


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapControllers();

app.UseExceptionHandler();

app.Run();


// For integration tests
public partial class Program() { }