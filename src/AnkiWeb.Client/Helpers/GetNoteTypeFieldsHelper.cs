using System.Text.Json.Serialization;

namespace AnkiWeb.Client.Helpers;

public class NoteTypeField
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class NoteTypeFields
{
    [JsonPropertyName("fields")]
    public List<NoteTypeField> Fields { get; set; } = new();
}
