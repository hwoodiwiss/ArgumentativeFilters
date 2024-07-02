using ExampleMinimalApi;
using ExampleMinimalApi.Filters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#pragma warning disable CA1852
#pragma warning disable CS1591 // Would require XML Docs on Program

#if NET8_0_OR_GREATER
var builder = WebApplication.CreateSlimBuilder(args);
#else
var builder = WebApplication.CreateBuilder(args);
#endif

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions();
builder.Services.Configure<ExampleMinimalApiOptions>(builder.Configuration);

#if NET8_0_OR_GREATER
builder.Services.ConfigureHttpJsonOptions(opt =>
{
    opt.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonContext.Default);
});
builder.Services.AddKeyedScoped<ValidateIdFilter>(ValidateIdFilters.ValidateId);
#else
builder.Services.AddScoped<ValidateIdFilter>();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.MapGet("/country/{country}/{id}", (string country, int id) => new CountryDto(id, country))
    .AddEndpointFilterFactory(NormalizeRouteCountryFilter.Factory)
    .AddEndpointFilterFactory(ValidateIdFilter.ValidateIdFactory);

app.Run();


public partial class Program
{
}