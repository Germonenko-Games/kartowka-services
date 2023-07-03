using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kartowka.Api.Extensions;
using Kartowka.Api.HostedServices;
using Kartowka.Api.Middleware;
using Kartowka.Api.OpenApi.Operations;
using Kartowka.Api.Options;
using Kartowka.Authorization.Core.Contracts;
using Kartowka.Authorization.Core.Services;
using Kartowka.Authorization.Core.Services.Abstractions;
using Kartowka.Authorization.Infrastructure.Contracts;
using Kartowka.Authorization.Infrastructure.Options;
using Kartowka.Common.Blobs;
using Kartowka.Common.Blobs.Azure;
using Kartowka.Common.Crypto;
using Kartowka.Common.Crypto.Abstractions;
using Kartowka.Common.Messaging;
using Kartowka.Common.Messaging.Azure.Extensions;
using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Consumers;
using Kartowka.Packs.Core.Models;
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
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

var appConfigurationConnectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration");
if (!string.IsNullOrEmpty(appConfigurationConnectionString))
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(appConfigurationConnectionString)
            .Select(KeyFilter.Any)
            .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
            .UseFeatureFlags();
    });
}

var blobStorageConnectionString = builder.Configuration.GetConnectionString("BlobStorage");
builder.Services.AddAzureClients(configuration =>
{
    configuration.AddBlobServiceClient(blobStorageConnectionString);
    configuration.AddQueueServiceClient(blobStorageConnectionString);
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Security:Jwt"));
builder.Services.Configure<PacksOptions>(builder.Configuration.GetSection("Packs"));
builder.Services.Configure<UploadLimitsOptions>(builder.Configuration.GetSection("Uploads:Limits"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        var stringToEnumConverter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
        options.JsonSerializerOptions.Converters.Add(stringToEnumConverter);
    });
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

    options.OperationFilter<DefaultResponseHeadersOperationFilter>();
    options.OperationFilter<LanguageRequestHeaderOperationFilter>();
    options.OperationFilter<ServerErrorOperationFilter>();

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

builder.Services.ConfigureEndpoint<PackCleanupMessage>(options =>
{
    options.MaxConcurrentMessages = 1;
});
builder.Services.RegisterAzureQueueReceiver<PackCleanupMessage>(options =>
{
    options.QueueName = "pack-cleanup";
    options.DeadLetterQueueName = "pack-cleanup-dl";
});
builder.Services.RegisterAzureQueuePublisher<PackCleanupMessage>(options =>
{
    options.QueueName = "pack-cleanup";
});

// Services
builder.Services.AddScoped<IHasher, Pbkdf2Hasher>();
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
builder.Services.AddScoped<IAccessTokenGenerator, JwtAccessTokenGenerator>();
builder.Services.AddScoped(typeof(IValidatorsRunner<>), typeof(ValidatorsRunner<>));
builder.Services.AddScoped<IAsyncValidator<UserData>, UserDataUniquenessValidator>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IPacksService, PacksService>();
builder.Services.AddScoped<IPackBlobsCleanupService, PackBlobsCleanupService>();
builder.Services.AddScoped<IConsumer<PackCleanupMessage>, PackBlobsCleanupConsumer>();
builder.Services.AddScoped<IRoundsService, RoundsService>();
builder.Services.AddScoped<IQuestionsService, QuestionsService>();
builder.Services.AddScoped<IQuestionsCategoriesService, QuestionsCategoriesService>();
builder.Services.AddScoped<IAsyncValidator<Pack>, PackAuthorValidator>();
builder.Services.AddScoped<IAsyncValidator<Pack>, UserPacksLimitValidator>();
builder.Services.AddScoped<IValidator<Pack>, PackQuestionsLimitValidator>();
builder.Services.AddScoped<IValidator<Pack>, PackRoundsLimitValidator>();
builder.Services.AddScoped<IValidator<Pack>, PackQuestionsCategoriesLimitValidator>();
builder.Services.AddScoped<IValidator<Question>, TextQuestionContentValidator>();
builder.Services.AddScoped<IValidator<Question>, ImageQuestionContentValidator>();
builder.Services.AddScoped<IValidator<Question>, MusicQuestionContentValidator>();

builder.Services.AddScoped<IBlobsStore, AzureBlobStore>()
    .Decorate<IBlobsStore, BlobStoreExceptionsWrapperDecorator>();

builder.Services.AddScoped<IContentTypeProvider, ContentTypeProvider>(_ =>
{
    var supportedTypes = builder.Configuration
        .GetSection("Uploads:SupportedContentTypes:Assets")
        .GetChildren()
        .Where(config => config.Value is not null)
        .ToDictionary(config => config.Key, config => config.Value!);

    return new ContentTypeProvider(supportedTypes);
});

// Middleware DI registration
builder.Services.AddScoped<RequestBufferingMiddleware>();
builder.Services.AddScoped<KartowkaExceptionsHandlingMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocExpansion(DocExpansion.None);
});

app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseMiddleware<RequestBufferingMiddleware>();
app.UseMiddleware<KartowkaExceptionsHandlingMiddleware>();

app.MapControllers();

app.Run();
