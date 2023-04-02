using System;
using System.Text.Json;
using t4m.DTOs;

namespace t4m.Helpers;

public class JsonSerializerHelper
{
    public static JsonSerializerOptions GetDefaultJsonSerializerOptions(IServiceProvider _ = null) =>
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

    public static T? Deserialize<T>(string serializedObject, JsonSerializerOptions options = null) where T : class
    {
        options ??= GetDefaultJsonSerializerOptions();

        return JsonSerializer.Deserialize<T>(serializedObject, options);
    }
}