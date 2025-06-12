using Test.Dal;
using Test.API.Middlewares;
using Test.API.Grpc.Services;
using Test.API.Extensions;
using Test.Application;
using Test.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomizedSwagger();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Services
    .ConfigureApiServices()
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

app.ConfigureHangfireDashBoard();

app.UseExceptionHandler();

app.Run();
