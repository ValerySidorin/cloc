using System.Diagnostics;
using System.Text.Json;

namespace Cloc;

public class ClocJobContext
{
    private JsonDocument _args;

    public ClocJobContext(JsonDocument args)
    {
        Debug.Assert(args is not null);
        _args = args;
    }

    public JsonElement GetRoot()
    {
        return _args.RootElement;
    }

    public string GetString(string key)
    {
        return _args.RootElement.GetProperty(key).GetString();
    }

    public int GetInt(string key)
    {
        return _args.RootElement.GetProperty(key).GetInt32();
    }
}
