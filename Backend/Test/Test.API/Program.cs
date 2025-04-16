using Test.Dal;
using Test.Application;
using Test.API.Middlewares;
using Test.API.Grpc;
using Test.API.Grpc.Services;
using Test.API.Extentions;

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
    .AddMediatorProvider()
    .ConfigureDalServices();

var app = builder.Build();

app.MapGrpcService<ProfileServiceGrpc>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
