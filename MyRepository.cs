namespace WebAPI;

public class MyRepository
{
    private readonly IStoredProcedureService _storedProcedureService;
    private readonly IResultFormatter _resultFormatter;
    private readonly ILoggerService _logger;

    public MyRepository(IResultFormatter resultFormatter, IStoredProcedureService storedProcedureService, ILoggerService logger)
    {
        _resultFormatter = resultFormatter;
        _storedProcedureService = storedProcedureService;
        _logger = logger;
    }

    public async Task<IResult> ExecuteStoredProcedureAsync(string spName, Dictionary<string, string> parameters)
    {
        try
        {
            var result = await _storedProcedureService.ExecuteStoredProcedureAsync(spName, parameters);
            return result.Msg.Count != 0 ? _resultFormatter.FormatResult(result) : _resultFormatter.FormatDefaultResult();
        }
        catch(Exception ex)
        {
            _logger.LogError(spName, ex);
            return _resultFormatter.FormatDefaultResult();
        }
    }
}