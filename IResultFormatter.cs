namespace WebAPI;

public interface IResultFormatter
{
    IResult FormatResult(List<List<Dictionary<string, object>>> data);
    IResult FormatDefaultResult();
}
