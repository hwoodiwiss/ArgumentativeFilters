using System.Text.Json.Serialization;
namespace ExampleMinimalApi;

[JsonSerializable(typeof(CountryDto))]
internal sealed partial class ApplicationJsonContext : JsonSerializerContext
{

}