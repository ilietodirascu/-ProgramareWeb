using API;
using API.Interfaces;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var services = builder.Services;
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();
services.AddDbContextFactory<DataContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddLogging();
services.AddHttpClient();
services.AddControllers().AddNewtonsoftJson();
services.AddSingleton<TelegramBot>();
services.AddSingleton<IConfiguration>(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();


