using Microsoft.Extensions.Options;
using System.Text.Json;

namespace WebAPI;

public class MyRepository
{
    private readonly IStoredProcedureService _storedProcedureService;
    private readonly ILoggerService _logger;

    public MyRepository(IStoredProcedureService storedProcedureService, ILoggerService logger)
    {
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

        var request = await content.Request.ReadFromJsonAsync<StoredProcedureRequest>(GlobalJsonSerializerOptions.Default);

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

    private IResult ResultsJson<T>(T request)
    {
        return Results.Json(request, GlobalJsonSerializerOptions.Default);
    }

    public async Task<IResult> ExecuteStoredProcedureAsync(HttpContext content)
    {
        var resultDefault = new ResultData();
        var request = await CheckHttpContext(content);
        if (string.IsNullOrEmpty(request.ProcedureName))
        {
            return ResultsJson(resultDefault);
        }

        try
        {
            ResultData result = await _storedProcedureService.ExecuteStoredProcedureAsync(request);

            if (result.Data.Count > 0)
            {
                result.resultMessage.Msg = true;
            }

            return ResultsJson(result);
        }
        catch(Exception ex)
        {
            _logger.LogError(request.ProcedureName, ex);
            return ResultsJson(resultDefault);
        }
    }
}