using System.Text.Json.Serialization;
namespace AnkiWeb.Client.Common.Models;
public class Deck
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = String.Empty;
}

