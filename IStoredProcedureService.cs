namespace WebAPI;

public interface IStoredProcedureService
{
    Task<ResMessage> ExecuteStoredProcedureAsync(StoredProcedureRequest request);
}

public class ResMessage
{
    public List<List<Dictionary<string, object>>> Result { get; set; } = new List<List<Dictionary<string, object>>>();
}
