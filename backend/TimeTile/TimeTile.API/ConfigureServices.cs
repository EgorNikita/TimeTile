using Microsoft.EntityFrameworkCore;
using TimeTile.Storage.Contexts;
using Serilog;

namespace TimeTile.API;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddSwagger();
        builder.AddDatabase();
        //builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);
        //builder.AddJwtAuthentication();
    }

    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
            // Uncomment if using Swashbuckle.AspNetCore.Filters or similar
            // options.InferSecuritySchemes();
        });
    }

    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        var connectionStringTemplate = configuration.GetConnectionString("Local");
        var connectionString = connectionStringTemplate?
            .Replace("${POSTGRES_HOST}", Environment.GetEnvironmentVariable("POSTGRES_HOST"))
            .Replace("${POSTGRES_DB}", Environment.GetEnvironmentVariable("POSTGRES_DB"))
            .Replace("${POSTGRES_USER}", Environment.GetEnvironmentVariable("POSTGRES_USER"))
            .Replace("${POSTGRES_PASSWORD}", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"));

        builder.Services.AddDbContext<TimetileDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }
}