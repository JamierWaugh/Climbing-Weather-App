using Climbing_Weather_App.Weather;
using RocksStartService.RockTypeService;

//Create builder for app
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

//Add controllers and services
builder.Services.AddControllers();

builder.Services.AddScoped<Weather>();

//Caches rock type json
builder.Services.AddSingleton<RockTypeService>();

//Build app
var app = builder.Build();

//Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Link controllers to app
app.MapControllers();

//Run app
app.Run();