using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeTile.API.Common.Api.Json
{
    public class PatchPropertyConverter<T> : JsonConverter<PatchOptionalProperty<T>>
    {
        public override PatchOptionalProperty<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return new PatchOptionalProperty<T>(default, true);
            }

            var value = JsonSerializer.Deserialize<T>(ref reader, options);
            return new PatchOptionalProperty<T>(value, true);
        }

        public override void Write(Utf8JsonWriter writer, PatchOptionalProperty<T> value, JsonSerializerOptions options)
        {
            if (!value.WasProvided)
                return;

            if (value.Value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
}
