using Microsoft.AspNetCore.SignalR;
using TimeTile.API.Auth.Authorization;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Common.Api.Hubs
{
    public class AuthenticatedHub : Hub
    {
        private readonly ITokenHandlerService _tokenHandlerService;

        public AuthenticatedHub(ITokenHandlerService tokenHandlerService)
        {
            _tokenHandlerService = tokenHandlerService;
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

        public override async Task OnConnectedAsync()
        {
            var userResult = await _tokenHandlerService.GetCurrentUser();

            if (userResult.IsFailure || userResult.Data == null)
            {
                await Clients.Caller.SendAsync("Error", userResult.Error);
                Context.Abort();
                return;
            }

            Context.Items[HubContextItemKeys.UserId] = userResult.Data!.Id;
            Context.Items[HubContextItemKeys.InstitutionId] = userResult.Data!.InstitutionId;

            await base.OnConnectedAsync();
        }
    }
}
