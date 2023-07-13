using ExampleMinimalApi;
using ExampleMinimalApi.Filters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#pragma warning disable CA1852

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions();
builder.Services.Configure<ExampleMinimalApiOptions>(builder.Configuration);
builder.Services.AddScoped<ValidateIdFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/country/{country}/{id}", (string country, int id) => Results.Json(new { id, country }))
    .AddEndpointFilterFactory(NormalizeRouteCountryFilter.Factory)
    .AddEndpointFilterFactory(ValidateIdFilter.Factory);

app.Run();


