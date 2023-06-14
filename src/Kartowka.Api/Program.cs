using System.Text;
using Kartowka.Api.Extensions;
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
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Services;
using Kartowka.Packs.Core.Services.Abstractions;
using Kartowka.Packs.Core.Validators;
using Kartowka.Registration.Core.Models;
using Kartowka.Registration.Core.Services;
using Kartowka.Registration.Core.Services.Abstractions;
using Kartowka.Registration.Core.Services.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Security:Jwt"));
builder.Services.Configure<PacksOptions>(builder.Configuration.GetSection("Packs"));

builder.Services.AddControllers();
builder.Services.AddFeatureManagement();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLocalization();
builder.Services.AddRequestLocalization(options =>
{
    options.AddSupportedCultures("ru");
    options.AddSupportedUICultures("ru");
    options.ApplyCurrentCultureToResponseHeaders = true;
    options.FallBackToParentCultures = true;
});
var jwtSecretKey = builder.Configuration.GetRequiredValue<string>("Security:Jwt:Secret");
var audience = builder.Configuration.GetRequiredValue<string>("Security:Jwt:Audience");
var issuer = builder.Configuration.GetRequiredValue<string>("Security:Jwt:Issuer");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidAudience = audience,
            ValidIssuer = issuer,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey)
            ),
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new()
    {
        Title = "Kartówka API",
        Description = "The Kartówka REST-like API.",
        Version = "0.1.0",
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put JWT token here",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
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
builder.Services.AddScoped<IPacksService, PacksService>();
builder.Services.AddScoped<IAsyncValidator<Pack>, PackAuthorValidator>();
builder.Services.AddScoped<IAsyncValidator<Pack>, UserPacksLimitValidator>();

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
