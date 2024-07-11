namespace WebAPI;

public class JsonResultFormatter : IResultFormatter
{
    private Dictionary<string, object> DefaultResult()
    {
        return new Dictionary<string, object>
        {
            { "resultmsg", new Dictionary<string, bool> { { "msg", false } } }
        };
    }

    public IResult FormatDefaultResult()
    {
        return Results.Json(DefaultResult());
    }

    public IResult FormatResult(List<List<Dictionary<string, object>>> data)
    {
        var result = DefaultResult();

        if (data != null && data.Any())
        {
            ((Dictionary<string, bool>)result["resultmsg"])["msg"] = true;

            for (int i = 0; i < data.Count; i++)
            {
                result[$"table{i + 1}"] = data[i];
            }
        }

        return Results.Json(result);
    }
}
