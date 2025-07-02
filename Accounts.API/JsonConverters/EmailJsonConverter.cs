using System.Text.Json;
using System.Text.Json.Serialization;
using Accounts.Domain.ValueObjects;

namespace Accounts.API.JsonConverters;

public class EmailJsonConverter : JsonConverter<Email>
{
    public override Email? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var emailString = reader.GetString();
        return emailString is null ? null : Email.Create(emailString);
    }

    public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}