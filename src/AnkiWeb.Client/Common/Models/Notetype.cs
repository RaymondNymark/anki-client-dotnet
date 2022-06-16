using System.Text.Json.Serialization;
namespace AnkiWeb.Client.Common.Models;
public class Notetype
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = String.Empty;
}

