using Authorization.API.Extention;
using Authorization.API.Middlewares;
using Authorization.Application;
using Authorization.Dal;
using Authorization.Infastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddlewareHandler>();

builder.Services
    .ConfigureApiServices()
    .ConfigureDalServices()
    .ConfigureAppSerrvices()
    .ConfigureInfastructureServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
