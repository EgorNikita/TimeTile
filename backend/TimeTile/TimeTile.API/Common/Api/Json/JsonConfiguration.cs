using System.Text.Json;

namespace TimeTile.API.Common.Api.Json
{
    public static class JsonConfiguration
    {
        public static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
