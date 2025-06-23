using Dapper;
using System.Data;
using System.Text.Json;

/* Custom Dapper type handler for List<string>
   Serializes to JSON when writing to DB,
   Deserializes from JSON when reading from DB */
public class JsonListTypeHandler : SqlMapper.TypeHandler<List<string>>
{
    /* Serialize List<string> to JSON string */
    public override void SetValue(IDbDataParameter parameter, List<string> value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }

    /* Deserialize JSON string to List<string> */
    public override List<string> Parse(object value)
    {
        /* Handle null or DBNull gracefully */
        if (value == null || value == DBNull.Value)
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(value.ToString() ?? "[]") ?? new List<string>();
        }
        catch (JsonException)
        {
            /* Fail-safe return in case of invalid JSON */
            return new List<string>();
        }
    }
}
