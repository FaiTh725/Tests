using ApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureAppServices(builder.Configuration);

var app = builder.Build();


app.UseHttpsRedirection();

app.UseCors("client");

app.MapReverseProxy();

app.Run();
