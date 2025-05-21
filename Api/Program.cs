using Api.Endpoints;
using Api.Configurations;
using Scalar.AspNetCore;

DotNetEnv.Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddDependencies();
builder.Services.AddValidators();
builder.Services.AddServices();
builder.Services.AddOpenApi();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapAuthEndpoints();
app.MapUsersEndpoints();

app.Run();
