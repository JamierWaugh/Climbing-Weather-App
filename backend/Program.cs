using Climbing_Weather_App.Weather;
using RocksStartService.RockTypeService;

//Create builder for app
var builder = WebApplication.CreateBuilder(args);

//Add CORS for communication with front end
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

//Caches rock type json
builder.Services.AddSingleton<RockTypeService>();

//Add controllers and services
builder.Services.AddControllers();

builder.Services.AddScoped<Weather>();



//Build app
var app = builder.Build();

//Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Link controllers to app
app.MapControllers();

app.UseCors();

//Run app
app.Run();