using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebAPI
{
    public static class GlobalJsonSerializerOptions
    {
        public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // 其他需要的選項...
        };
    }
}
