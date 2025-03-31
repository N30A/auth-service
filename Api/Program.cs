using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.MapGet("", () =>
    {
       
    })
    .WithName("");

app.Run();
