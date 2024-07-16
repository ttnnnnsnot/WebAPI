namespace WebAPI;

public interface IStoredProcedureService
{
    Task<ResMessage> ExecuteStoredProcedureAsync(string spName, Dictionary<string, string> parameters);
}

public class ResMessage
{
    public List<List<Dictionary<string, object>>> Msg { get; set; } = new List<List<Dictionary<string, object>>>();
}
