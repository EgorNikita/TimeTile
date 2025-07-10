using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TimeTile.API.Assignments.Services;
using TimeTile.API.Authentication;
using TimeTile.API.Authentication.Services;
using TimeTile.API.ClassroomTypes.Services;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Filters;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Json;
using TimeTile.API.Courses.Services;
using TimeTile.API.Files.Repositories;
using TimeTile.API.Files.Services;
using TimeTile.API.Users.Services;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Common.Interfaces.Repositories;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.Seeders;

namespace TimeTile.API;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        Log.Information("Starting service configuration...");

        builder.AddSwagger();
        builder.AddDatabase();
        builder.AddSerilog();
        builder.AddJwtAuthentication();
        builder.AddAuthorization();
        builder.AddRateLimiting();
        builder.AddCors();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new PatchPropertyConverterFactory());
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);

        builder.Services.AddScoped<IInstitutionProvider, InstitutionProvider>();
        builder.Services.AddScoped<IUserProvider, UserProvider>();
        builder.Services.AddScoped<RequireInstitutionFilter>();
        builder.Services.AddScoped<RequireUserIdFilter>();
        builder.Services.AddScoped<DataSeeder>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IAvatarService, AvatarService>();
        builder.Services.AddScoped<IFileRepository, FileRepository>();
        builder.Services.AddScoped<IClassroomTypeService, ClassroomTypeService>();
        builder.Services.AddScoped<ICourseService, CourseService>();
        builder.Services.AddScoped<IAssignmentService, AssignmentService>();

        Log.Information("Service configuration completed.");
    }

    private static void AddCors(this WebApplicationBuilder builder)
    {
        Log.Information("Configuring CORS...");
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }
    
    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        Log.Information("Configuring Swagger...");
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);
        });
    }

    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }

    private static void AddRateLimiting(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, token) =>
            {
                Log.Warning("Rate limit exceeded for {RequestPath}", context.HttpContext.Request.Path);
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = "Too many requests. Please try again later."
                }, token);
            };

            options.AddFixedWindowLimiter("login", opt =>
            {
                Log.Information("Configuring login rate limiter: 5 requests per minute");
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0; // Reject immediately if limit exceeded
            });
        });
    }

    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        Log.Information("Configuring database...");
        var connectionString = builder.Configuration.GetConnectionString("Local")
                               ?? throw new InvalidOperationException("Connection string 'Local' not found.");

        connectionString = connectionString
            .Replace("${POSTGRES_HOST}", Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost")
            .Replace("${POSTGRES_DB}",
                Environment.GetEnvironmentVariable("POSTGRES_DB") ??
                throw new InvalidOperationException("POSTGRES_DB environment variable not set"))
            .Replace("${POSTGRES_USER}",
                Environment.GetEnvironmentVariable("POSTGRES_USER") ??
                throw new InvalidOperationException("POSTGRES_USER environment variable not set"))
            .Replace("${POSTGRES_PASSWORD}",
                Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ??
                throw new InvalidOperationException("POSTGRES_PASSWORD environment variable not set"));

        builder.Services.AddDbContext<TimetileDbContext>(options => { options.UseNpgsql(connectionString); });
    }
}