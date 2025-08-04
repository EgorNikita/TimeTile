using Microsoft.AspNetCore.SignalR;
using TimeTile.API.Auth.Authorization;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Common.Api.Hubs
{
    public class AuthenticatedHub : Hub
    {
        protected readonly TimetileDbContext _db;

        public AuthenticatedHub(TimetileDbContext db)
        {
            _db = db;
        }

        protected int UserId
        {
            get
            {
                if (Context.Items.TryGetValue(HubContextItemKeys.UserId, out var userId) && userId is int id)
                    return id;
                throw new InvalidOperationException("User ID not found in context");
            }
        }

        protected int InstitutionId
        {
            get
            {
                if (Context.Items.TryGetValue(HubContextItemKeys.InstitutionId, out var institutionId) && institutionId is int id)
                    return id;
                throw new InvalidOperationException("Institution ID not found in context");
            }
        }

        protected async Task<Result<int>> GetValidatedUserIdAsync()
        {
            if (!Context.User?.Identity?.IsAuthenticated == true)
            {
                return Result.Failure<int>("User not authorized");
            }

            var userResult = await Context.User!.GetValidatedUserIdAsync(_db, Context.ConnectionAborted);

            return userResult;
        }

        protected async Task<Result<int>> GetValidatedInstitutionIdAsync()
        {
            var institutionResult = await Context.User!.GetValidatedInstitutionIdAsync(_db, Context.ConnectionAborted);

            return institutionResult;
        }

        public override async Task OnConnectedAsync()
        {
            var userResult = await GetValidatedUserIdAsync();
            if (userResult.IsFailure)
            {
                await Clients.Caller.SendAsync("Error", userResult.Error);
                Context.Abort();
                return;
            }

            var institutionResult = await GetValidatedInstitutionIdAsync();
            if (institutionResult.IsFailure)
            {
                await Clients.Caller.SendAsync("Error", institutionResult.Error);
                Context.Abort();
                return;
            }

            Context.Items[HubContextItemKeys.UserId] = userResult.Data;
            Context.Items[HubContextItemKeys.InstitutionId] = institutionResult.Data;

            await base.OnConnectedAsync();
        }
    }
}
