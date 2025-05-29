using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TimeTile.Storage.Contexts;
using Serilog;
using TimeTile.API.Authentication.Services;
using TimeTile.API.Common.Api;
using TimeTile.API.Users;
using TimeTile.API.Users.Services;
using TimeTile.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using TimeTile.API.Common.Constants;

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
        
        builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);
        
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IUserService, UserService>();
        
        Log.Information("Service configuration completed.");
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

    private static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        builder.Services.Configure<JwtOptions>(jwtSection);

        var jwtOptions = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Key))
            throw new InvalidOperationException("JWT Key is not configured.");
        
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = Jwt.SecurityKey(jwtOptions.Key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Forbidden\"}");
                }
            };
        });
        
        builder.Services.AddTransient<Jwt>();
    }

    private static void AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission, policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement(permission));
                });
            }
        });

        builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
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
            .Replace("${POSTGRES_DB}", Environment.GetEnvironmentVariable("POSTGRES_DB") ?? throw new InvalidOperationException("POSTGRES_DB environment variable not set"))
            .Replace("${POSTGRES_USER}", Environment.GetEnvironmentVariable("POSTGRES_USER") ?? throw new InvalidOperationException("POSTGRES_USER environment variable not set"))
            .Replace("${POSTGRES_PASSWORD}", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? throw new InvalidOperationException("POSTGRES_PASSWORD environment variable not set"));

        builder.Services.AddDbContext<TimetileDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }
}