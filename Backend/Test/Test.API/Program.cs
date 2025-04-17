using Test.Dal;
using Test.API.Middlewares;
using Test.API.Grpc.Services;
using Test.API.Extentions;
using Test.Infastructure;
using MediatR;
using Test.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Services
    .ConfigureApiServices()
    .ConfigureAppServices()
    .ConfigureInfastructureServices(builder.Configuration)
    .ConfigureDalServices();

var app = builder.Build();

app.MapGrpcService<ProfileServiceGrpc>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
