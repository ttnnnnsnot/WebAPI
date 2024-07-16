namespace WebAPI;

public interface IResultFormatter
{
    IResult FormatResult(ResMessage data);
    IResult FormatDefaultResult();
}
