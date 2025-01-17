using System.Text.Json;
using System.Text.Json.Serialization;
using LongreachAi.Connectors.UiPath;
internal static class Extensions
{

    static readonly JsonSerializerOptions _JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = { new JsonStringEnumConverter() }
    };

    ///<summary>
    ///Serialize object to JSON string
    ///</summary>
    ///<param name="obj">Object to serialize</param>
    ///<returns>JSON string</returns>
    internal static string ToJson<T>(this T obj)
    {

        if (obj == null) return "";
        return JsonSerializer.Serialize(obj, _JsonOptions);
    }

}
