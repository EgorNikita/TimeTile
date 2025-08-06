using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.API.Messages.Hubs;
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

        app.UseCors();

        app.MapHub<MessagesHub>(MessagesHub.HUB_PATH);

        app.MapEndpoints();

        await app.EnsureDatabaseCreated();

        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();

            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.Seed();
        }
    }

    private static async Task EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<TimetileDbContext>();

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

        var logger = Log.Logger;

        try
        {
            await db.Database.MigrateAsync();

            await PermissionsSeeder.Seed(db, logger);
            await LessonStatusesSeeder.Seed(db, logger);
            await RolesSeeder.SeedRequiredRoles(db, logger);
            await AdminSeeder.Seed(db, hasher, userService, fileService, logger);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database migration failed.");
        }
    }
}