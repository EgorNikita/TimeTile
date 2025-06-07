namespace TimeTile.API.Users.Services.Interfaces;

public interface IAvatarService
{
    Task<Stream> GenerateDefaultAvatar(string firstname, string lastname, int size = 100);
}