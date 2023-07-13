using System.Text.Json.Serialization;

namespace ExampleMinimalApi.Integration.Tests;

public sealed record ExampleResponse(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("country")]
    string Country);