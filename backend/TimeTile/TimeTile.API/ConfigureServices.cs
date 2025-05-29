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
        Log.Information("Configuring Serilog...");
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }

    private static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        Log.Information("Configuring JWT authentication...");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = Jwt.SecurityKey(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.")),
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
                    Log.Information("JWT OnChallenge triggered.");
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
                },
                OnForbidden = context =>
                {
                    Log.Information("JWT OnForbidden triggered.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Forbidden\"}");
                }
            };
        });

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddTransient<Jwt>();
    }

    private static void AddAuthorization(this WebApplicationBuilder builder)
    {
        Log.Information("Registering authorization policies...");
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AddStudent", policy =>
            {
                Log.Information("Adding AddStudent policy.");
                policy.Requirements.Add(new PermissionRequirement("AddStudent"));
            });

            options.AddPolicy("DeleteStudent", policy =>
            {
                Log.Information("Adding DeleteStudent policy.");
                policy.Requirements.Add(new PermissionRequirement("DeleteStudent"));
            });

            options.AddPolicy("CreateStudent", policy =>
            {
                Log.Information("Adding CreateStudent policy.");
                policy.Requirements.Add(new PermissionRequirement("CreateStudent"));
            });
            
            options.AddPolicy("CreateRole", policy =>
            {
                Log.Information("Adding CreateRole policy.");
                policy.Requirements.Add(new PermissionRequirement("CreateRole"));
            });
            
            options.AddPolicy("CreateInstitution", policy =>
            {
                Log.Information("Adding CreateInstitution policy.");
                policy.Requirements.Add(new PermissionRequirement("CreateInstitution"));
            });
        });

        Log.Information("Registering PermissionAuthorizationHandler...");
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
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