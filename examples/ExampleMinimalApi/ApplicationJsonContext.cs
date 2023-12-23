using System.Text.Json.Serialization;
namespace ExampleMinimalApi;

[JsonSerializable(typeof(CountryDto))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
    
}