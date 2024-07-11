namespace WebAPI;

public class StoredProcedureRequest
{
    public string ProcedureName { get; set; } = "";
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}
