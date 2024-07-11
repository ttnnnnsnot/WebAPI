using System.Text.Json;

namespace WebAPI;

public class StoredProcedureService : IStoredProcedureService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILoggerService _loggerService;
    public StoredProcedureService(ApplicationDbContext dbcontext, ILoggerService loggerService)
    {
        _dbcontext = dbcontext;
        _loggerService = loggerService;
    }

    public async Task<List<List<Dictionary<string, object>>>> ExecuteStoredProcedureAsync(string spName, Dictionary<string, string> parameters)
    {
        var parameterString = string.Join(", ", parameters.Select(p => $"@{p.Key}=@{p.Key}"));
        var sql = $"EXEC {spName} {parameterString}";

        var result = new List<List<Dictionary<string, object>>>();
        using (var command = _dbcontext.GetDbConnection().CreateCommand())
        {
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;

            foreach (var param in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Key;
                parameter.Value = param.Value;
                command.Parameters.Add(parameter);
            }

            await _dbcontext.OpenConnectionAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                do
                {
                    var table = new List<Dictionary<string, object>>();
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        table.Add(row);
                    }
                    result.Add(table);
                } while (await reader.NextResultAsync());
            }

            await _dbcontext.CloseConnectionAsync();
        }

        return result;
        
    }
}
