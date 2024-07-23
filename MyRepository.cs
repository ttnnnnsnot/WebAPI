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

    private async Task<StoredProcedureRequest> CheckHttpContext(HttpContext content)
    {
        var resultDefault = new StoredProcedureRequest();
        if (content.Request.ContentLength == 0)
        {
            return resultDefault;
        }

        var request = await content.Request.ReadFromJsonAsync<StoredProcedureRequest>();

        if (request == null)
        {
            return resultDefault;
        }

        if (string.IsNullOrEmpty(request.ProcedureName) || request.Parameters == null)
        {
            return resultDefault;
        }

        return request;
    }

    public async Task<IResult> ExecuteStoredProcedureAsync(HttpContext content)
    {
        var resultDefault = _resultFormatter.FormatDefaultResult();
        var request = await CheckHttpContext(content);
        if (string.IsNullOrEmpty(request.ProcedureName))
        {
            return resultDefault;
        }

        try
        {
            var result = await _storedProcedureService.ExecuteStoredProcedureAsync(request);

            return result.Result.Count != 0 ? _resultFormatter.FormatResult(result) : resultDefault;
        }
        catch(Exception ex)
        {
            _logger.LogError(request.ProcedureName, ex);
            return resultDefault;
        }
    }
}