using Kartowka.Api.HostedServices;
using Kartowka.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CoreContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Core");
    options
        .UseNpgsql(connectionString, postgresOptions =>
        {
            postgresOptions.MigrationsAssembly("Kartowka.Migrations");
        })
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .UseSnakeCaseNamingConvention();
});

builder.Services.AddHostedService<ApplyMigrationsHostedService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
