using TestRating.API.Extensions;
using TestRating.API.Grpc.Services;
using TestRating.API.Middlewares;
using TestRating.Application;
using TestRating.Dal;
using TestRating.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Services
    .ConfigureApiServices()
    .ConfigureInfrastructureServices(builder.Configuration)
    .ConfigureAppServices()
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
