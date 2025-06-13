using ApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureAppServices(builder.Configuration);

var app = builder.Build();


app.UseCors("client");

app.UseHttpsRedirection();

app.MapReverseProxy();

app.Run();
