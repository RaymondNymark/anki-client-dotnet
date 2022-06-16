using System.Text.Json.Serialization;

namespace AnkiWeb.Client.Helpers;

public class Field
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class NoteTypeFields
{
    [JsonPropertyName("fields")]
    public List<Field> Fields { get; set; } = new();
}
