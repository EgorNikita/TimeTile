using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.DataSeeders;
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
            var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

            await seeder.Seed(lifetime.ApplicationStopping);

            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseRateLimiter();
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature?.Error is not null)
                    {
                        Log.Error(exceptionHandlerPathFeature.Error, "Unhandled exception occurred.");
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
                    }
                });
            });
        }

        app.UseHttpsRedirection();

        app.MapEndpoints();

        await app.EnsureDatabaseCreated();
    }

    private static async Task EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<TimetileDbContext>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

        try
        {
            await db.Database.MigrateAsync();
            await AdminSeeder.SeedAsync(db, new PasswordHasher<User>(), userService, fileService, Log.Logger);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database migration failed.");
        }
    }
}