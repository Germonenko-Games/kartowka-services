using Kartowka.Api.HostedServices;
using Kartowka.Api.Middleware;
using Kartowka.Authorization.Core.Contracts;
using Kartowka.Authorization.Core.Services;
using Kartowka.Authorization.Core.Services.Abstractions;
using Kartowka.Authorization.Infrastructure.Contracts;
using Kartowka.Authorization.Infrastructure.Options;
using Kartowka.Common.Crypto;
using Kartowka.Common.Crypto.Abstractions;
using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Registration.Core.Models;
using Kartowka.Registration.Core.Services;
using Kartowka.Registration.Core.Services.Abstractions;
using Kartowka.Registration.Core.Services.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Security:Jwt"));

builder.Services.AddControllers();
builder.Services.AddFeatureManagement();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLocalization();
builder.Services.AddRequestLocalization(options =>
{
    options.AddSupportedCultures("ru");
    options.AddSupportedUICultures("ru");
    options.ApplyCurrentCultureToResponseHeaders = true;
    options.FallBackToParentCultures = true;
});

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

// Hosted Services
builder.Services.AddHostedService<ApplyMigrationsHostedService>();

// Services
builder.Services.AddScoped<IHasher, Pbkdf2Hasher>();
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
builder.Services.AddScoped<IAccessTokenGenerator, JwtAccessTokenGenerator>();
builder.Services.AddScoped(typeof(IAsyncValidatorsRunner<>), typeof(AsyncValidatorsRunner<>));
builder.Services.AddScoped<IAsyncValidator<UserData>, UserDataUniquenessValidator>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

// Middleware DI registration
builder.Services.AddScoped<RequestBufferingMiddleware>();
builder.Services.AddScoped<KartowkaExceptionsHandlingMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseMiddleware<RequestBufferingMiddleware>();
app.UseMiddleware<KartowkaExceptionsHandlingMiddleware>();

app.MapControllers();

app.Run();
