namespace WebAPI;

public interface IStoredProcedureService
{
    Task<ResultData> ExecuteStoredProcedureAsync(StoredProcedureRequest request);
}

