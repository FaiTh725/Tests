using Test.Dal;
using Test.API.Middlewares;
using Test.API.Grpc.Services;
using Test.API.Extensions;
using Test.Application;
using Hangfire;
using Test.API.Filters;
using Test.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Services
    .ConfigureApiServices()
    .ConfigureAppServices()
    .ConfigureInfrastructureServices(builder.Configuration)
    .ConfigureDalServices();

var app = builder.Build();

app.MapGrpcService<ProfileServiceGrpc>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthorizationFilter()]
});

app.UseExceptionHandler();

app.Run();
