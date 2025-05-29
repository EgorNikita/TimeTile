using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.DataSeeders;

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
                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
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

        try
        {
            await db.Database.MigrateAsync();
            await AdminSeeder.SeedAsync(db, new PasswordHasher<User>(), Log.Logger);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database migration failed.");
        }
    }
}