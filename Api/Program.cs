using Api.Endpoints;
using Api.Configurations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Auth Service API";
        options.HideClientButton = true;
    });
}

app.UseHttpsRedirection();
app.MapAuthEndpoints();
app.MapUsersEndpoints();

app.Run();
