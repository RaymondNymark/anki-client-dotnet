namespace AnkiWeb.Client.Common.Models;
public record Card
{
    public string TypeId { get; private set; }
    public string Tags { get; private set; }
    public List<Field> Fields { get; private set; }
    public string EncodedData { get => EncodeDataToCorrectFormat(); }

    public Card(string typeId, List<Field> fields, string tags = "")
    {
        TypeId = typeId;
        Fields = fields;
        Tags = tags;
    }

    // Todo: Use string builder.
    private string EncodeDataToCorrectFormat()
    {
        List<string> fieldData = Fields.Select(x => $@"""{x.Value}""").ToList();
        var joinedFieldData = string.Join(",", fieldData);

        return @$"[[{joinedFieldData}], ""{Tags}""]";
    }
}