using System.Text.Json.Serialization;
namespace AnkiWeb.Client.Common.Models;

public class CollectionInfo
{
    [JsonPropertyName("notetypes")]
    public List<Notetype> Notetypes { get; set; }
    [JsonPropertyName("decks")]
    public List<Deck> Decks { get; set; }
    [JsonPropertyName("currentDeckId")]
    public string CurrentDeckId { get; set; } = String.Empty;
    [JsonPropertyName("currentNotetypeId")]
    public string CurrentNoteTypeId { get; set; } = String.Empty;
}

