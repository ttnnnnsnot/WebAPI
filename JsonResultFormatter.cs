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

    public IResult FormatResult(ResMessage data)
    {
        var result = DefaultResult();

        if (data != null && data.Msg.Count != 0 )
        {
            ((Dictionary<string, bool>)result["resultmsg"])["msg"] = true;
            result["data"] = data.Msg;
        }

        return Results.Json(result);
    }
}
