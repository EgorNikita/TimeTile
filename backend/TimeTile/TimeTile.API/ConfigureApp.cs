using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.Seeders;

namespace TimeTile.API;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.Seed();
        }

        app.UseHttpsRedirection();

        app.MapEndpoints();

        await app.EnsureDatabaseCreated();
    }
    
    private static async Task EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TimetileDbContext>();

        try
        {
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database migration failed.");
        }
    }
}