namespace TimeTile.Core.Common.Interfaces.Services;

public interface IAvatarService
{
    Task<Stream> GenerateDefaultAvatar(string firstname, string lastname, int size = 100);
}