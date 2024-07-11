namespace WebAPI;

public interface IStoredProcedureService
{
    Task<List<List<Dictionary<string, object>>>> ExecuteStoredProcedureAsync(string spName, Dictionary<string, string> parameters);
}
