using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Auth.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetInstitutionId(this ClaimsPrincipal user, out int institutionId)
    {
        institutionId = 0;

        var claim = user.FindFirst(CustomClaimTypes.InstitutionId);
        if (claim != null && int.TryParse(claim.Value, out institutionId))
            return true;

        Log.Error("Institution ID claim is missing or invalid");
        return false;
    }
    
    public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
    {
        userId = 0;

        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out userId))
            return true;

        Log.Error("User ID claim is missing or invalid");
        return false;
    }

    public static async Task<Result<int>> GetValidatedInstitutionIdAsync(
        this ClaimsPrincipal user,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetInstitutionId(out var institutionId))
        {
            var error = Error.From(
                "Institution ID is missing or invalid in the current user's claims.",
                "CLAIM_ID_MISSING"
            );
            return Result.Failure<int>(error);
        }

        var exists = await db.Institutions
            .AsNoTracking()
            .AnyAsync(i => i.Id == institutionId, cancellationToken);

        if (!exists)
        {
            var error = Error.From(
                $"Institution with ID '{institutionId}' does not exist.",
                "ENTITY_DOES_NOT_EXIST"
            );
            return Result.Failure<int>(error);
        }

        return Result.Success(institutionId);
    }
    
    public static async Task<Result<int>> GetValidatedUserIdAsync(
        this ClaimsPrincipal user,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserId(out var userId))
        {
            var error = Error.From(
                "User ID is missing or invalid in the current user's claims.",
                "CLAIM_ID_MISSING"
            );
            return Result.Failure<int>(error);
        }

        var exists = await db.Users
            .AsNoTracking()
            .AnyAsync(i => i.Id == userId, cancellationToken);

        if (!exists)
        {
            var error = Error.From(
                $"User with ID '{userId}' does not exist.",
                "ENTITY_DOES_NOT_EXIST"
            );
            return Result.Failure<int>(error);
        }

        return Result.Success(userId);
    }
}