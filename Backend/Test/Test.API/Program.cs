using Serilog;
using Test.API.Extensions;
using Test.API.Grpc.Services;
using Test.API.Hubs.Instance;
using Test.API.Middlewares;
using Test.Application;
using Test.Dal;
using Test.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomizedSwagger();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Host.UseSerilog();

builder.Services
    .ConfigureApiServices(builder.Configuration)
    .ConfigureAppServices()
    .ConfigureInfrastructureServices(builder.Configuration)
    .ConfigureDalServices(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<TestServiceGrpc>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TestSessionHub>("/hub/test-session");

app.ConfigureHangfireDashBoard();

app.UseExceptionHandler();

app.Run();

public partial class Program { };